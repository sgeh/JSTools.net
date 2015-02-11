/*
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 */

using System;

using JSTools.Parser.Cruncher.Nodes;

namespace JSTools.Parser.Cruncher
{
	/// <summary>
	/// This class implements the JavaScript parser.
	/// 
	/// It is based on the C source files jsparse.c and jsparse.h
	/// in the jsref package.
	/// </summary>
	internal class Parser 
	{
		private int _lastExprEndLine; // Hack to handle function expr termination.
		private IRFactory _nf;
		private bool _ok; // Did the parse encounter an error?

		private char[] _sourceBuffer = new char[128];
		private int _sourceTop;
		private int _functionNumber;

		internal Parser(IRFactory nf) 
		{
			_nf = nf;
		}

		/*
		 * Build a parse tree from the given TokenStream.
		 *
		 * @param ts the TokenStream to parse
		 *
		 * @return an object representing the parsed
		 * program.  If the parse fails, null will be returned.  (The
		 * parse failure will result in a call to the current Context's
		 * ErrorReporter.)
		 */
		internal Node Parse(TokenStream ts)
		{
			_ok = true;
			_sourceTop = 0;
			_functionNumber = 0;

			int baseLineno = ts.LineNo;		// line number where source starts

			/* so we have something to add nodes to until
			 * we've collected all the source */
			Node tempBlock = _nf.CreateLeaf(TokenType.Block);

			// Add script indicator
			SourceAdd(TokenType.Script);

			while (true) 
			{
				ts.Flags |= JSTokenStreamFlags.RegExp;
				TokenType tt = ts.GetToken(); // last token from GetToken();
				ts.Flags &= ~JSTokenStreamFlags.RegExp;

				if (tt <= TokenType.EOF) 
				{
					break;
				}

				if (tt == TokenType.Function) 
				{
					try 
					{
						_nf.AddChildToBack(tempBlock, Function(ts, false));
					}
					catch
					{
						_ok = false;
						throw;
					}
				} 
				else 
				{
					ts.UngetToken(tt);
					_nf.AddChildToBack(tempBlock, Statement(ts));
				}
			}

			if (!_ok) 
			{
				// XXX ts.ClearPushback() call here?
				return null;
			}

			SourceAdd(TokenType.LastToken);

			Node pn = _nf.CreateScript(
				tempBlock,
				ts.SourceName,
				baseLineno,
				ts.LineNo,
				SourceToCharArray() );

			_sourceBuffer = null; // To help GC
			return pn;
		}
	
		/*
		 * The C version of this function takes an argument list,
		 * which doesn't seem to be needed for tree generation...
		 * it'd only be useful for checking argument hiding, which
		 * I'm not doing anyway...
		 */
		private Node ParseFunctionBody(TokenStream ts)
		{
			JSTokenStreamFlags oldflags = ts.Flags;
			ts.Flags &= ~(JSTokenStreamFlags.ReturnExpression
				| JSTokenStreamFlags.ReturnVoid);
			ts.Flags |= JSTokenStreamFlags.Function;

			Node pn = _nf.CreateBlock(ts.LineNo);
			try 
			{
				TokenType tt;
				while((tt = ts.PeekToken()) > TokenType.EOF && tt != TokenType.Rc) 
				{
					if (tt == TokenType.Function) 
					{
						ts.GetToken();
						_nf.AddChildToBack(pn, Function(ts, false));
					} 
					else 
					{
						_nf.AddChildToBack(pn, Statement(ts));
					}
				}
			}
			finally 
			{
				// also in finally block:
				// flushNewLines, ClearPushback.
				ts.Flags = oldflags;
			}
			return pn;
		}

		private Node Function(TokenStream ts, bool isExpr)
		{
			int baseLineno = ts.LineNo;  // line number where source starts

			string name;
			Node memberExprNode = null;
			if (ts.MatchToken(TokenType.Name)) 
			{
				name = ts.ScannedString;

				if (!ts.MatchToken(TokenType.Lp)) 
					MustMatchToken(ts, TokenType.Lp, "msg.no.paren.parms");
			}
			else if (ts.MatchToken(TokenType.Lp)) 
			{
				// Anonymous function
				name = null;
			}
			else 
			{
				name = null;
				MustMatchToken(ts, TokenType.Lp, "msg.no.paren.parms");
			}

			if (memberExprNode != null) 
			{
				// transform 'function' <memberExpr> to  <memberExpr> = function
				// even in the decompilated source
				SourceAdd(TokenType.Assign);
				SourceAdd(TokenType.Nop);
			}

			// save a reference to the function in the enclosing source.
			SourceAdd(TokenType.Function);
			SourceAdd((char)_functionNumber);
			++_functionNumber;

			// Save current source top to restore it on exit not to include
			// function to parent source
			int savedSourceTop = _sourceTop;
			int savedFunctionNumber = _functionNumber;
			Node args;
			Node body;
			string source;
			try 
			{
				_functionNumber = 0;

				// FUNCTION as the first token in a Source means it's a function
				// definition, and not a reference.
				SourceAdd(TokenType.Function);

				if (name != null)
					SourceAddString(TokenType.Name, name);

				SourceAdd(TokenType.Lp);
				args = _nf.CreateLeaf(TokenType.Lp);

				if (!ts.MatchToken(TokenType.Rp)) 
				{
					bool first = true;
					do 
					{
						if (!first)
							SourceAdd(TokenType.Comma);
						first = false;
						MustMatchToken(ts, TokenType.Name, "msg.no.parm");
						string s = ts.ScannedString;
						_nf.AddChildToBack(args, _nf.CreateName(s));

						SourceAddString(TokenType.Name, s);
					} while (ts.MatchToken(TokenType.Comma));

					MustMatchToken(ts, TokenType.Rp, "msg.no.paren.after.parms");
				}
				SourceAdd(TokenType.Rp);

				MustMatchToken(ts, TokenType.Lc, "msg.no.brace.body");
				SourceAdd(TokenType.Lc);
				SourceAdd(TokenType.EOL);
				body = ParseFunctionBody(ts);
				MustMatchToken(ts, TokenType.Rc, "msg.no.brace.after.body");
				SourceAdd(TokenType.Rc);
				// skip the last TokenType.EOL so nested functions work...

				// name might be null;
				source = SourceToString(savedSourceTop);
			}
			finally 
			{
				_sourceTop = savedSourceTop;
				_functionNumber = savedFunctionNumber;
			}

			Node pn = _nf.CreateFunction(name, args, body,
				ts.SourceName,
				baseLineno, ts.LineNo,
				source.ToCharArray(),
				isExpr || memberExprNode != null);
			if (memberExprNode != null) 
			{
				pn = _nf.CreateBinary(TokenType.Assign, TokenType.Nop, memberExprNode, pn);
			}

			// Add TokenType.EOL but only if function is not part of expression, in which
			// case it gets SEMI + TokenType.EOL from Statement.
			if (!isExpr) 
			{
				// Add ';' to make 'function x.f(){}' and 'x.f = Function(){}'
				// to print the same strings when decompiling
				SourceAdd(TokenType.Semi);
				SourceAdd(TokenType.EOL);
				WellTerminated(ts, TokenType.Function);
			}

			return pn;
		}

		private Node Statements(TokenStream ts)
		{
			Node pn = _nf.CreateBlock(ts.LineNo);
			TokenType tt;

			while((tt = ts.PeekToken()) > TokenType.EOF && tt != TokenType.Rc) 
			{
				_nf.AddChildToBack(pn, Statement(ts));
			}
			return pn;
		}

		private Node Condition(TokenStream ts)
		{
			Node pn;
			MustMatchToken(ts, TokenType.Lp, "msg.no.paren.cond");
			SourceAdd(TokenType.Lp);
			pn = Expr(ts, false);
			MustMatchToken(ts, TokenType.Rp, "msg.no.paren.after.cond");
			SourceAdd(TokenType.Rp);

			// there's a check here in jsparse.c that corrects = to ==
			return pn;
		}

		private bool WellTerminated(TokenStream ts, TokenType lastExprType)
		{
			TokenType tt = ts.PeekTokenSameLine();

			if (tt == TokenType.Error) 
				return false;

			if (tt != TokenType.EOF && tt != TokenType.EOL
				&& tt != TokenType.Semi && tt != TokenType.Rc)
			{
				if ((tt == TokenType.Function || lastExprType == TokenType.Function)
					&& _nf.Version != ScriptVersion.Version_1_0
					&& _nf.Version != ScriptVersion.Version_1_1)
				{
					/*
					 * Checking against version < 1.2 and version >= 1.0
					 * in the above line breaks old javascript, so we keep it
					 * this way for now...
					 */
					return true;
				} 
				else 
				{
					ReportError("msg.no.semi.stmt");
				}
			}
			return true;
		}

		// match a NAME; return null if no match.
		private string MatchLabel(TokenStream ts)
		{
			int lineno = ts.LineNo;
			string label = null;
			TokenType tt = ts.PeekTokenSameLine();

			if (tt == TokenType.Name)
			{
				ts.GetToken();
				label = ts.ScannedString;
			}

			if (lineno == ts.LineNo)
				WellTerminated(ts, TokenType.Error);

			return label;
		}

		/**
		 * Whether the "catch (e: e instanceof Exception) { ... }" syntax
		 * is implemented.
		 */
		private Node Statement(TokenStream ts)
		{
			Node pn = null;

			// If skipsemi == true, don't add SEMI + TokenType.EOL to source at the
			// end of this statment.  For compound statements, IF/FOR etc.
			bool skipsemi = false;
			TokenType lastExprType = TokenType.EOF;  // For wellTerminated.  0 to avoid warning.
			TokenType tt = ts.GetToken();

			switch (tt) 
			{
				case TokenType.If: 
				{
					skipsemi = true;

					SourceAdd(TokenType.If);
					int lineno = ts.LineNo;
					Node cond = Condition(ts);
					SourceAdd(TokenType.Lc);
					SourceAdd(TokenType.EOL);
					Node ifTrue = Statement(ts);
					Node ifFalse = null;
					if (ts.MatchToken(TokenType.Else)) 
					{
						SourceAdd(TokenType.Rc);
						SourceAdd(TokenType.Else);
						SourceAdd(TokenType.Lc);
						SourceAdd(TokenType.EOL);
						ifFalse = Statement(ts);
					}
					SourceAdd(TokenType.Rc);
					SourceAdd(TokenType.EOL);
					pn = _nf.CreateIf(cond, ifTrue, ifFalse, lineno);
					break;
				}

				case TokenType.Switch: 
				{
					skipsemi = true;

					SourceAdd(TokenType.Switch);
					pn = _nf.CreateSwitch(ts.LineNo);

					Node cur_case = null;  // to kill warning
					Node case_statements;

					MustMatchToken(ts, TokenType.Lp, "msg.no.paren.switch");
					SourceAdd(TokenType.Lp);
					_nf.AddChildToBack(pn, Expr(ts, false));
					MustMatchToken(ts, TokenType.Rp, "msg.no.paren.after.switch");
					SourceAdd(TokenType.Rp);
					MustMatchToken(ts, TokenType.Lc, "msg.no.brace.switch");
					SourceAdd(TokenType.Lc);
					SourceAdd(TokenType.EOL);

					while ((tt = ts.GetToken()) != TokenType.Rc && tt != TokenType.EOF) 
					{
						switch(tt) 
						{
							case TokenType.Case:
								SourceAdd(TokenType.Case);
								cur_case = _nf.CreateUnary(TokenType.Case, Expr(ts, false));
								SourceAdd(TokenType.Colon);
								SourceAdd(TokenType.EOL);
								break;

							case TokenType.Default:
								cur_case = _nf.CreateLeaf(TokenType.Default);
								SourceAdd(TokenType.Default);
								SourceAdd(TokenType.Colon);
								SourceAdd(TokenType.EOL);
								// XXX check that there isn't more than one default
								break;

							default:
								ReportError("msg.bad.switch");
								break;
						}
						MustMatchToken(ts, TokenType.Colon, "msg.no.colon.case");

						case_statements = _nf.CreateLeaf(TokenType.Block);

						while ((tt = ts.PeekToken()) != TokenType.Rc && tt != TokenType.Case &&
							tt != TokenType.Default && tt != TokenType.EOF)
						{
							_nf.AddChildToBack(case_statements, Statement(ts));
						}
						// assert cur_case
						_nf.AddChildToBack(cur_case, case_statements);

						_nf.AddChildToBack(pn, cur_case);
					}
					SourceAdd(TokenType.Rc);
					SourceAdd(TokenType.EOL);
					break;
				}

				case TokenType.While: 
				{
					skipsemi = true;

					SourceAdd(TokenType.While);
					int lineno = ts.LineNo;
					Node cond = Condition(ts);
					SourceAdd(TokenType.Lc);
					SourceAdd(TokenType.EOL);
					Node body = Statement(ts);
					SourceAdd(TokenType.Rc);
					SourceAdd(TokenType.EOL);

					pn = _nf.CreateWhile(cond, body, lineno);
					break;

				}

				case TokenType.Do: 
				{
					SourceAdd(TokenType.Do);
					SourceAdd(TokenType.Lc);
					SourceAdd(TokenType.EOL);

					int lineno = ts.LineNo;

					Node body = Statement(ts);

					SourceAdd(TokenType.Rc);
					MustMatchToken(ts, TokenType.While, "msg.no.while.do");
					SourceAdd(TokenType.While);
					Node cond = Condition(ts);

					pn = _nf.CreateDoWhile(body, cond, lineno);
					break;
				}

				case TokenType.For: 
				{
					skipsemi = true;

					SourceAdd(TokenType.For);
					int lineno = ts.LineNo;

					Node init;  // Node init is also foo in 'foo in object'
					Node cond;  // Node cond is also object in 'foo in object'
					Node incr = null; // to kill warning
					Node body;

					MustMatchToken(ts, TokenType.Lp, "msg.no.paren.for");
					SourceAdd(TokenType.Lp);
					tt = ts.PeekToken();
					if (tt == TokenType.Semi) 
					{
						init = _nf.CreateLeaf(TokenType.Void);
					} 
					else 
					{
						if (tt == TokenType.Var) 
						{
							// set init to a var list or initial
							ts.GetToken();    // throw away the 'var' token
							init = Variables(ts, true);
						}
						else 
						{
							init = Expr(ts, true);
						}
					}

					tt = ts.PeekToken();
					if (tt == TokenType.RelOp && ts.Op == TokenType.In) 
					{
						ts.MatchToken(TokenType.RelOp);
						SourceAdd(TokenType.In);
						// 'cond' is the object over which we're iterating
						cond = Expr(ts, false);
					} 
					else 
					{  // ordinary for loop
						MustMatchToken(ts, TokenType.Semi, "msg.no.semi.for");
						SourceAdd(TokenType.Semi);
						if (ts.PeekToken() == TokenType.Semi) 
						{
							// no loop condition
							cond = _nf.CreateLeaf(TokenType.Void);
						} 
						else 
						{
							cond = Expr(ts, false);
						}

						MustMatchToken(ts, TokenType.Semi, "msg.no.semi.for.cond");
						SourceAdd(TokenType.Semi);
						if (ts.PeekToken() == TokenType.Rp) 
						{
							incr = _nf.CreateLeaf(TokenType.Void);
						} 
						else 
						{
							incr = Expr(ts, false);
						}
					}

					MustMatchToken(ts, TokenType.Rp, "msg.no.paren.for.ctrl");
					SourceAdd(TokenType.Rp);
					SourceAdd(TokenType.Lc);
					SourceAdd(TokenType.EOL);
					body = Statement(ts);
					SourceAdd(TokenType.Rc);
					SourceAdd(TokenType.EOL);

					if (incr == null) 
					{
						// cond could be null if 'in obj' got eaten by the init node.
						pn = _nf.CreateForIn(init, cond, body, lineno);
					} 
					else 
					{
						pn = _nf.CreateFor(init, cond, incr, body, lineno);
					}
					break;
				}

				case TokenType.Try: 
				{
					int lineno = ts.LineNo;

					Node tryblock;
					Node catchblocks = null;
					Node finallyblock = null;

					skipsemi = true;
					SourceAdd(TokenType.Try);
					SourceAdd(TokenType.Lc);
					SourceAdd(TokenType.EOL);
					tryblock = Statement(ts);
					SourceAdd(TokenType.Rc);
					SourceAdd(TokenType.EOL);

					catchblocks = _nf.CreateLeaf(TokenType.Block);

					bool sawDefaultCatch = false;
					TokenType peek = ts.PeekToken();

					if (peek == TokenType.Catch) 
					{
						while (ts.MatchToken(TokenType.Catch)) 
						{
							if (sawDefaultCatch) 
							{
								ReportError("msg.catch.unreachable");
							}
							SourceAdd(TokenType.Catch);
							MustMatchToken(ts, TokenType.Lp, "msg.no.paren.catch");
							SourceAdd(TokenType.Lp);

							MustMatchToken(ts, TokenType.Name, "msg.bad.catchcond");
							string varName = ts.ScannedString;
							SourceAddString(TokenType.Name, varName);

							Node catchCond = null;
							if (ts.MatchToken(TokenType.If)) 
							{
								SourceAdd(TokenType.If);
								catchCond = Expr(ts, false);
							} 
							else 
							{
								sawDefaultCatch = true;
							}

							MustMatchToken(ts, TokenType.Rp, "msg.bad.catchcond");
							SourceAdd(TokenType.Rp);
							MustMatchToken(ts, TokenType.Lc, "msg.no.brace.catchblock");
							SourceAdd(TokenType.Lc);
							SourceAdd(TokenType.EOL);

							_nf.AddChildToBack(catchblocks,
								_nf.CreateCatch(varName, catchCond,
								Statements(ts),
								ts.LineNo));

							MustMatchToken(ts, TokenType.Rc, "msg.no.brace.after.body");
							SourceAdd(TokenType.Rc);
							SourceAdd(TokenType.EOL);
						}
					} 
					else if (peek != TokenType.Finally) 
					{
						MustMatchToken(ts, TokenType.Finally, "msg.try.no.catchfinally");
					}

					if (ts.MatchToken(TokenType.Finally)) 
					{
						SourceAdd(TokenType.Finally);

						SourceAdd(TokenType.Lc);
						SourceAdd(TokenType.EOL);
						finallyblock = Statement(ts);
						SourceAdd(TokenType.Rc);
						SourceAdd(TokenType.EOL);
					}

					pn = _nf.CreateTryCatchFinally(tryblock, catchblocks,
						finallyblock, lineno);

					break;
				}
				case TokenType.Throw: 
				{
					int lineno = ts.LineNo;
					SourceAdd(TokenType.Throw);
					pn = _nf.CreateThrow(Expr(ts, false), lineno);
					if (lineno == ts.LineNo)
						WellTerminated(ts, TokenType.Error);
					break;
				}
				case TokenType.Break: 
				{
					int lineno = ts.LineNo;

					SourceAdd(TokenType.Break);

					// matchLabel only matches if there is one
					string label = MatchLabel(ts);
					if (label != null) 
					{
						SourceAddString(TokenType.Name, label);
					}
					pn = _nf.CreateBreak(label, lineno);
					break;
				}
				case TokenType.Continue: 
				{
					int lineno = ts.LineNo;

					SourceAdd(TokenType.Continue);

					// matchLabel only matches if there is one
					string label = MatchLabel(ts);
					if (label != null) 
					{
						SourceAddString(TokenType.Name, label);
					}
					pn = _nf.CreateContinue(label, lineno);
					break;
				}
				case TokenType.With: 
				{
					skipsemi = true;

					SourceAdd(TokenType.With);
					int lineno = ts.LineNo;
					MustMatchToken(ts, TokenType.Lp, "msg.no.paren.with");
					SourceAdd(TokenType.Lp);
					Node obj = Expr(ts, false);
					MustMatchToken(ts, TokenType.Rp, "msg.no.paren.after.with");
					SourceAdd(TokenType.Rp);
					SourceAdd(TokenType.Lc);
					SourceAdd(TokenType.EOL);

					Node body = Statement(ts);

					SourceAdd(TokenType.Rc);
					SourceAdd(TokenType.EOL);

					pn = _nf.CreateWith(obj, body, lineno);
					break;
				}
				case TokenType.Var: 
				{
					int lineno = ts.LineNo;
					pn = Variables(ts, false);
					if (ts.LineNo == lineno)
						WellTerminated(ts, TokenType.Error);
					break;
				}
				case TokenType.Return: 
				{
					Node retExpr = null;

					SourceAdd(TokenType.Return);

					// bail if we're not in a (toplevel) function
					if ((ts.Flags & JSTokenStreamFlags.Function) == 0)
						ReportError("msg.bad.return");

					/* This is ugly, but we don't want to require a semicolon. */
					ts.Flags |= JSTokenStreamFlags.RegExp;
					tt = ts.PeekTokenSameLine();
					ts.Flags &= ~JSTokenStreamFlags.RegExp;

					int lineno = ts.LineNo;
					if (tt != TokenType.EOF && tt != TokenType.EOL && tt != TokenType.Semi && tt != TokenType.Rc) 
					{
						retExpr = Expr(ts, false);
						if (ts.LineNo == lineno)
							WellTerminated(ts, TokenType.Error);
						ts.Flags |= JSTokenStreamFlags.ReturnExpression;
					} 
					else 
					{
						ts.Flags |= JSTokenStreamFlags.ReturnVoid;
					}

					// XXX ASSERT pn
					pn = _nf.CreateReturn(retExpr, lineno);
					break;
				}
				case TokenType.Lc:
					skipsemi = true;

					pn = Statements(ts);
					MustMatchToken(ts, TokenType.Rc, "msg.no.brace.block");
					break;

				case TokenType.Error:
					// Fall thru, to have a node for error recovery to work on
				case TokenType.EOL:
				case TokenType.Semi:
					pn = _nf.CreateLeaf(TokenType.Void);
					skipsemi = true;
					break;

				default: 
				{
					lastExprType = tt;
					int tokenno = ts.TokenNo;
					ts.UngetToken(tt);
					int lineno = ts.LineNo;

					pn = Expr(ts, false);

					if (ts.PeekToken() == TokenType.Colon) 
					{
						/* check that the last thing the tokenizer returned was a
						 * NAME and that only one token was consumed.
						 */
						if (lastExprType != TokenType.Name || (ts.TokenNo != tokenno))
							ReportError("msg.bad.label");

						ts.GetToken();  // eat the COLON

						/* in the C source, the label is associated with the
						 * statement that follows:
						 *                _nf.AddChildToBack(pn, Statement(ts));
						 */
						string name = ts.ScannedString;
						pn = _nf.CreateLabel(name, lineno);

						// depend on decompiling lookahead to guess that that
						// last name was a label.
						SourceAdd(TokenType.Colon);
						SourceAdd(TokenType.EOL);
						return pn;
					}

					if (lastExprType == TokenType.Function) 
					{
						if (_nf.GetLeafType(pn) != TokenType.Function) 
						{
							ReportError("msg.syntax");
						}
						_nf.SetFunctionExpressionStatement(pn);
					}

					pn = _nf.CreateExprStatement(pn, lineno);

					/*
					 * Check explicitly against (multi-line) function
					 * statement.

					 * _lastExprEndLine is a hack to fix an
					 * automatic semicolon insertion problem with function
					 * expressions; the ts.LineNo == lineno check was
					 * firing after a function definition even though the
					 * next statement was on a new line, because
					 * speculative GetToken calls advanced the line number
					 * even when they didn't succeed.
					 */
					if (ts.LineNo == lineno ||
						(lastExprType == TokenType.Function &&
						ts.LineNo == _lastExprEndLine))
					{
						WellTerminated(ts, lastExprType);
					}
					break;
				}
			}
			ts.MatchToken(TokenType.Semi);
			if (!skipsemi) 
			{
				SourceAdd(TokenType.Semi);
				SourceAdd(TokenType.EOL);
			}

			return pn;
		}

		private Node Variables(TokenStream ts, bool inForInit)
		{
			Node pn = _nf.CreateVariables(ts.LineNo);
			bool first = true;

			SourceAdd(TokenType.Var);

			for (;;) 
			{
				Node name;
				Node init;
				MustMatchToken(ts, TokenType.Name, "msg.bad.var");
				string s = ts.ScannedString;

				if (!first)
					SourceAdd(TokenType.Comma);
				first = false;

				SourceAddString(TokenType.Name, s);
				name = _nf.CreateName(s);

				// omitted check for argument hiding

				if (ts.MatchToken(TokenType.Assign)) 
				{
					if (ts.Op != TokenType.Nop)
						ReportError("msg.bad.var.init");

					SourceAdd(TokenType.Assign);
					SourceAdd(TokenType.Nop);

					init = AssignExpr(ts, inForInit);
					_nf.AddChildToBack(name, init);
				}
				_nf.AddChildToBack(pn, name);
				if (!ts.MatchToken(TokenType.Comma))
					break;
			}
			return pn;
		}

		private Node Expr(TokenStream ts, bool inForInit)
		{
			Node pn = AssignExpr(ts, inForInit);
			while (ts.MatchToken(TokenType.Comma)) 
			{
				SourceAdd(TokenType.Comma);
				pn = _nf.CreateBinary(TokenType.Comma, pn, AssignExpr(ts, inForInit));
			}
			return pn;
		}

		private Node AssignExpr(TokenStream ts, bool inForInit)
		{
			Node pn = CondExpr(ts, inForInit);

			if (ts.MatchToken(TokenType.Assign)) 
			{
				// omitted: "invalid assignment left-hand side" check.
				SourceAdd(TokenType.Assign);
				SourceAdd(ts.Op);
				pn = _nf.CreateBinary(TokenType.Assign, ts.Op, pn,
					AssignExpr(ts, inForInit));
			}

			return pn;
		}

		private Node CondExpr(TokenStream ts, bool inForInit)
		{
			Node ifTrue;
			Node ifFalse;

			Node pn = OrExpr(ts, inForInit);

			if (ts.MatchToken(TokenType.Hook)) 
			{
				SourceAdd(TokenType.Hook);
				ifTrue = AssignExpr(ts, false);
				MustMatchToken(ts, TokenType.Colon, "msg.no.colon.cond");
				SourceAdd(TokenType.Colon);
				ifFalse = AssignExpr(ts, inForInit);
				return _nf.CreateTernary(pn, ifTrue, ifFalse);
			}

			return pn;
		}

		private Node OrExpr(TokenStream ts, bool inForInit)
		{
			Node pn = AndExpr(ts, inForInit);
			if (ts.MatchToken(TokenType.Or)) 
			{
				SourceAdd(TokenType.Or);
				pn = _nf.CreateBinary(TokenType.Or, pn, OrExpr(ts, inForInit));
			}

			return pn;
		}

		private Node AndExpr(TokenStream ts, bool inForInit)
		{
			Node pn = BitOrExpr(ts, inForInit);
			if (ts.MatchToken(TokenType.And)) 
			{
				SourceAdd(TokenType.And);
				pn = _nf.CreateBinary(TokenType.And, pn, AndExpr(ts, inForInit));
			}

			return pn;
		}

		private Node BitOrExpr(TokenStream ts, bool inForInit)
		{
			Node pn = BitXorExpr(ts, inForInit);
			while (ts.MatchToken(TokenType.BitOr)) 
			{
				SourceAdd(TokenType.BitOr);
				pn = _nf.CreateBinary(TokenType.BitOr, pn, BitXorExpr(ts, inForInit));
			}
			return pn;
		}

		private Node BitXorExpr(TokenStream ts, bool inForInit)
		{
			Node pn = BitAndExpr(ts, inForInit);
			while (ts.MatchToken(TokenType.BitXOr)) 
			{
				SourceAdd(TokenType.BitXOr);
				pn = _nf.CreateBinary(TokenType.BitXOr, pn, BitAndExpr(ts, inForInit));
			}
			return pn;
		}

		private Node BitAndExpr(TokenStream ts, bool inForInit)
		{
			Node pn = EqExpr(ts, inForInit);
			while (ts.MatchToken(TokenType.BitAnd)) 
			{
				SourceAdd(TokenType.BitAnd);
				pn = _nf.CreateBinary(TokenType.BitAnd, pn, EqExpr(ts, inForInit));
			}
			return pn;
		}

		private Node EqExpr(TokenStream ts, bool inForInit)
		{
			Node pn = RelExpr(ts, inForInit);
			while (ts.MatchToken(TokenType.EqOp)) 
			{
				SourceAdd(TokenType.EqOp);
				SourceAdd(ts.Op);
				pn = _nf.CreateBinary(TokenType.EqOp, ts.Op, pn,
					RelExpr(ts, inForInit));
			}
			return pn;
		}

		private Node RelExpr(TokenStream ts, bool inForInit)
		{
			Node pn = ShiftExpr(ts);
			while (ts.MatchToken(TokenType.RelOp)) 
			{
				TokenType op = ts.Op;
				if (inForInit && op == TokenType.In) 
				{
					ts.UngetToken(TokenType.RelOp);
					break;
				}
				SourceAdd(TokenType.RelOp);
				SourceAdd(op);
				pn = _nf.CreateBinary(TokenType.RelOp, op, pn, ShiftExpr(ts));
			}
			return pn;
		}

		private Node ShiftExpr(TokenStream ts)
		{
			Node pn = AddExpr(ts);
			while (ts.MatchToken(TokenType.ShOp)) 
			{
				SourceAdd(TokenType.ShOp);
				SourceAdd(ts.Op);
				pn = _nf.CreateBinary(ts.Op, pn, AddExpr(ts));
			}
			return pn;
		}

		private Node AddExpr(TokenStream ts)
		{
			Node pn = MulExpr(ts);
			TokenType tt;

			while ((tt = ts.GetToken()) == TokenType.Add || tt == TokenType.Sub) 
			{
				SourceAdd(tt);
				// flushNewLines
				pn = _nf.CreateBinary(tt, pn, MulExpr(ts));
			}
			ts.UngetToken(tt);
			return pn;
		}

		private Node MulExpr(TokenStream ts)
		{
			Node pn = UnaryExpr(ts);
			TokenType tt;

			while ((tt = ts.PeekToken()) == TokenType.Mul
				|| tt == TokenType.Div
				|| tt == TokenType.Mod) 
			{
				tt = ts.GetToken();
				SourceAdd(tt);
				pn = _nf.CreateBinary(tt, pn, UnaryExpr(ts));
			}
			return pn;
		}

		private Node UnaryExpr(TokenStream ts)
		{
			ts.Flags |= JSTokenStreamFlags.RegExp;
			TokenType tt = ts.GetToken();
			ts.Flags &= ~JSTokenStreamFlags.RegExp;

			switch(tt) 
			{
				case TokenType.UnaryOp:
					SourceAdd(TokenType.UnaryOp);
					SourceAdd(ts.Op);
					return _nf.CreateUnary(TokenType.UnaryOp, ts.Op, UnaryExpr(ts));

				case TokenType.Add:
				case TokenType.Sub:
					SourceAdd(TokenType.UnaryOp);
					SourceAdd(tt);
					return _nf.CreateUnary(TokenType.UnaryOp, tt, UnaryExpr(ts));

				case TokenType.Inc:
				case TokenType.Dec:
					SourceAdd(tt);
					return _nf.CreateUnary(tt, TokenType.Pre, MemberExpr(ts, true));

				case TokenType.DelProp:
					SourceAdd(TokenType.DelProp);
					return _nf.CreateUnary(TokenType.DelProp, UnaryExpr(ts));

				case TokenType.Error:
					break;

				default:
					ts.UngetToken(tt);

					int lineno = ts.LineNo;

					Node pn = MemberExpr(ts, true);

					/* don't look across a newline boundary for a postfix incop.

					 * the rhino scanner seems to work differently than the js
					 * scanner here; in js, it works to have the line number check
					 * precede the PeekToken calls.  It'd be better if they had
					 * similar behavior...
					 */
					TokenType peeked = ts.PeekToken();
					if ((peeked == TokenType.Inc || peeked == TokenType.Dec)
						&& ts.LineNo == lineno)
					{
						TokenType pf = ts.GetToken();
						SourceAdd(pf);
						return _nf.CreateUnary(pf, TokenType.Post, pn);
					}
					return pn;
			}
			return _nf.CreateName("err"); // Only reached on error.  Try to continue.

		}

		private Node ArgumentList(TokenStream ts, Node listNode)
		{
			bool matched;
			ts.Flags |= JSTokenStreamFlags.RegExp;
			matched = ts.MatchToken(TokenType.Rp);
			ts.Flags &= ~JSTokenStreamFlags.RegExp;

			if (!matched) 
			{
				bool first = true;
				do 
				{
					if (!first)
						SourceAdd(TokenType.Comma);
					first = false;
					_nf.AddChildToBack(listNode, AssignExpr(ts, false));
				} while (ts.MatchToken(TokenType.Comma));

				MustMatchToken(ts, TokenType.Rp, "msg.no.paren.arg");
			}
			SourceAdd(TokenType.Rp);
			return listNode;
		}

		private Node MemberExpr(TokenStream ts, bool allowCallSyntax)
		{
			Node pn;

			/* Check for new expressions. */
			ts.Flags |= JSTokenStreamFlags.RegExp;
			TokenType tt = ts.PeekToken();
			ts.Flags &= ~JSTokenStreamFlags.RegExp;

			if (tt == TokenType.New) 
			{
				/* Eat the NEW token. */
				ts.GetToken();
				SourceAdd(TokenType.New);

				/* Make a NEW node to append to. */
				pn = _nf.CreateLeaf(TokenType.New);
				_nf.AddChildToBack(pn, MemberExpr(ts, false));

				if (ts.MatchToken(TokenType.Lp)) 
				{
					SourceAdd(TokenType.Lp);
					/* Add the arguments to pn, if any are supplied. */
					pn = ArgumentList(ts, pn);
				}

				/* XXX there's a check in the C source against
				 * "too many constructor arguments" - how many
				 * do we claim to support?
				 */

				/* Experimental syntax:  allow an object literal to follow a new expression,
				 * which will mean a kind of anonymous class built with the JavaAdapter.
				 * the object literal will be passed as an additional argument to the constructor.
				 */
				tt = ts.PeekToken();
				if (tt == TokenType.Lc) 
				{
					_nf.AddChildToBack(pn, PrimaryExpr(ts));
				}
			} 
			else 
			{
				pn = PrimaryExpr(ts);
			}

			return MemberExprTail(ts, allowCallSyntax, pn);
		}

		private Node MemberExprTail(TokenStream ts, bool allowCallSyntax, Node pn)
		{
			_lastExprEndLine = ts.LineNo;
			TokenType tt;

			while ((tt = ts.GetToken()) > TokenType.EOF) 
			{
				if (tt == TokenType.Dot) 
				{
					SourceAdd(TokenType.Dot);
					MustMatchToken(ts, TokenType.Name, "msg.no.name.after.dot");
					string s = ts.ScannedString;
					SourceAddString(TokenType.Name, s);
					pn = _nf.CreateBinary(TokenType.Dot, pn,
						_nf.CreateName(ts.ScannedString));
					/* pn = _nf.CreateBinary(ts.DOT, pn, MemberExpr(ts))
					 * is the version in Brendan's IR C version.  Not in ECMA...
					 * does it reflect the 'new' operator syntax he mentioned?
					 */
					_lastExprEndLine = ts.LineNo;
				} 
				else if (tt == TokenType.Lb) 
				{
					SourceAdd(TokenType.Lb);
					pn = _nf.CreateBinary(TokenType.Lb, pn, Expr(ts, false));

					MustMatchToken(ts, TokenType.Rb, "msg.no.bracket.index");
					SourceAdd(TokenType.Rb);
					_lastExprEndLine = ts.LineNo;
				} 
				else if (allowCallSyntax && tt == TokenType.Lp) 
				{
					/* make a call node */

					pn = _nf.CreateUnary(TokenType.Call, pn);
					SourceAdd(TokenType.Lp);

					/* Add the arguments to pn, if any are supplied. */
					pn = ArgumentList(ts, pn);
					_lastExprEndLine = ts.LineNo;
				} 
				else 
				{
					ts.UngetToken(tt);
					break;
				}
			}
			return pn;
		}

		private Node PrimaryExpr(TokenStream ts)
		{
			Node pn = null;

			ts.Flags |= JSTokenStreamFlags.RegExp;
			TokenType tt = ts.GetToken();
			ts.Flags &= ~JSTokenStreamFlags.RegExp;

			switch(tt) 
			{

				case TokenType.Function:
					return Function(ts, true);

				case TokenType.Lb:
				{
					SourceAdd(TokenType.Lb);
					pn = _nf.CreateLeaf(TokenType.ArrayLit);

					ts.Flags |= JSTokenStreamFlags.RegExp;
					bool matched = ts.MatchToken(TokenType.Rb);
					ts.Flags &= ~JSTokenStreamFlags.RegExp;

					if (!matched) 
					{
						bool first = true;
						do 
						{
							ts.Flags |= JSTokenStreamFlags.RegExp;
							tt = ts.PeekToken();
							ts.Flags &= ~JSTokenStreamFlags.RegExp;

							if (!first)
								SourceAdd(TokenType.Comma);
							else
								first = false;

							if (tt == TokenType.Rb) 
							{  // to fix [,,,].length behavior...
								break;
							}

							if (tt == TokenType.Comma) 
							{
								_nf.AddChildToBack(pn, _nf.CreateLeaf(TokenType.Primary,
									TokenType.Undefined));
							} 
							else 
							{
								_nf.AddChildToBack(pn, AssignExpr(ts, false));
							}

						} while (ts.MatchToken(TokenType.Comma));
						MustMatchToken(ts, TokenType.Rb, "msg.no.bracket.arg");
					}
					SourceAdd(TokenType.Rb);
					return _nf.CreateArrayLiteral(pn);
				}

				case TokenType.Lc: 
				{
					pn = _nf.CreateLeaf(TokenType.ObjLit);

					SourceAdd(TokenType.Lc);
					if (!ts.MatchToken(TokenType.Rc)) 
					{

						bool first = true;
					commaloop:
						do 
						{
							Node property = null;

							if (!first)
								SourceAdd(TokenType.Comma);
							else
								first = false;

							tt = ts.GetToken();
							switch(tt) 
							{
									// map NAMEs to STRINGs in object literal context.
								case TokenType.Name:
								case TokenType.String:
									string s = ts.ScannedString;
									SourceAddString(TokenType.Name, s);
									property = _nf.CreateString(ts.ScannedString);
									break;

								case TokenType.Number:
									string n = ts.ScannedNumber;
									SourceAddString(TokenType.Number, n);
									property = _nf.CreateNumber(n);
									break;

								case TokenType.Rc:
									// trailing comma is OK.
									ts.UngetToken(tt);
									goto commaloop;

								default:
									ReportError("msg.bad.prop");
									break;
							}
							MustMatchToken(ts, TokenType.Colon, "msg.no.colon.prop");

							// OBJLIT is used as ':' in object literal for
							// decompilation to solve spacing ambiguity.
							SourceAdd(TokenType.ObjLit);
							_nf.AddChildToBack(pn, property);
							_nf.AddChildToBack(pn, AssignExpr(ts, false));

						} while (ts.MatchToken(TokenType.Comma));

						MustMatchToken(ts, TokenType.Rc, "msg.no.brace.prop");
					}
					SourceAdd(TokenType.Rc);
					return _nf.CreateObjectLiteral(pn);
				}

				case TokenType.Lp:

					/* Brendan's IR-jsparse.c makes a new node tagged with
					 * TOK_LP here... I'm not sure I understand why.  Isn't
					 * the grouping already implicit in the structure of the
					 * parse tree?  also TOK_LP is already overloaded (I
					 * think) in the C IR as 'function call.'  */
					SourceAdd(TokenType.Lp);
					pn = Expr(ts, false);
					SourceAdd(TokenType.Rp);
					MustMatchToken(ts, TokenType.Rp, "msg.no.paren");
					return pn;

				case TokenType.Name:
					string name = ts.ScannedString;
					SourceAddString(TokenType.Name, name);
					return _nf.CreateName(name);

				case TokenType.Number:
					string num = ts.ScannedNumber;
					SourceAddString(TokenType.Number, num);
					return _nf.CreateNumber(num);

				case TokenType.String:
					string str = ts.ScannedString;
					SourceAddString(TokenType.String, str);
					return _nf.CreateString(str);

				case TokenType.RegExp:
				{
					string flags = ts.RegExpFlags;
					ts.RegExpFlags = null;
					string re = ts.ScannedString;
					SourceAddString(TokenType.RegExp, '/' + re + '/' + flags);
					return _nf.CreateRegExp(re, flags);
				}

				case TokenType.Primary:
					SourceAdd(TokenType.Primary);
					SourceAdd(ts.Op);
					return _nf.CreateLeaf(TokenType.Primary, ts.Op);

				case TokenType.Reserved:
					ReportError("msg.reserved.id");
					break;

				case TokenType.Error:
					/* the scanner or one of its subroutines reported the error. */
					break;

				default:
					ReportError("msg.syntax");
					break;

			}
			return null;    // should never reach here
		}

		/**
		 * The following methods save decompilation information about the source.
		 * Source information is returned from the parser as a string
		 * associated with function nodes and with the toplevel script.  When
		 * saved in the constant pool of a class, this string will be UTF-8
		 * encoded, and token values will occupy a single byte.

		 * Source is saved (mostly) as token numbers.  The tokens saved pretty
		 * much correspond to the token stream of a 'canonical' representation
		 * of the input program, as directed by the parser.  (There were a few
		 * cases where tokens could have been left out where decompiler could
		 * easily reconstruct them, but I left them in for clarity).  (I also
		 * looked adding source collection to TokenStream instead, where I
		 * could have limited the changes to a few lines in GetToken... but
		 * this wouldn't have saved any space in the resulting source
		 * representation, and would have meant that I'd have to duplicate
		 * parser logic in the decompiler to disambiguate situations where
		 * newlines are important.)

		 * Token types with associated ops (ASSIGN, SHOP, PRIMARY, etc.) are
		 * saved as two-token pairs.  Number tokens are stored inline, as a
		 * NUMBER token, a character representing the type, and either 1 or 4
		 * characters representing the bit-encoding of the number.  string
		 * types NAME, STRING and OBJECT are currently stored as a token type,
		 * followed by a character giving the length of the string (assumed to
		 * be less than 2^16), followed by the characters of the string
		 * inlined into the source string.  Changing this to some reference to
		 * to the string in the compiled class' constant pool would probably
		 * save a lot of space... but would require some method of deriving
		 * the final constant pool entry from information available at parse
		 * time.

		 * Nested functions need a similar mechanism... fortunately the nested
		 * functions for a given function are generated in source order.
		 * Nested functions are encoded as FUNCTION followed by a function
		 * number (encoded as a character), which is enough information to
		 * find the proper generated function instance.
		 */
		private void SourceAdd(char c) 
		{
			if (_sourceTop == _sourceBuffer.Length) 
			{
				IncreaseSourceCapacity(_sourceTop + 1);
			}
			_sourceBuffer[_sourceTop] = c;
			++_sourceTop;
		}

		private void SourceAdd(TokenType type) 
		{
			SourceAdd((char)type);
		}

		private void SourceAddString(TokenType type, string str) 
		{
			SourceAdd((char)type);
			SourceAddString(str);
		}

		private void SourceAddString(string str) 
		{
			int L = str.Length;
			int lengthEncodingSize = 1;
			if (L >= 0x8000) 
			{
				lengthEncodingSize = 2;
			}
			int nextTop = _sourceTop + lengthEncodingSize + L;
			if (nextTop > _sourceBuffer.Length) 
			{
				IncreaseSourceCapacity(nextTop);
			}
			if (L >= 0x8000)
			{
				// Use 2 chars to encode strings exceeding 32K, were the highest
				// bit in the first char indicates presence of the next byte
				_sourceBuffer[_sourceTop] = (char)(0x8000 | (L >> 16));
				++_sourceTop;
			}
			_sourceBuffer[_sourceTop] = (char)L;
			++_sourceTop;
			str.CopyTo(0, _sourceBuffer, _sourceTop, L);
			_sourceTop = nextTop;
		}

		private void IncreaseSourceCapacity(int minimalCapacity) 
		{
			// Call this only when capacity increase is must
			int newCapacity = _sourceBuffer.Length * 2;
			if (newCapacity < minimalCapacity) 
			{
				newCapacity = minimalCapacity;
			}
			char[] tmp = new char[newCapacity];
			Array.Copy(_sourceBuffer, 0, tmp, 0, _sourceTop);
			_sourceBuffer = tmp;
		}

		private char[] SourceToCharArray() 
		{
			char[] sourceData = new char[_sourceTop];
			Array.Copy(_sourceBuffer, 0, sourceData, 0, _sourceTop);
			return sourceData;
		}

		private string SourceToString(int offset) 
		{
			return new string(_sourceBuffer, offset, _sourceTop - offset);
		}

		private void MustMatchToken(TokenStream ts, TokenType toMatch, string messageId)
		{
			TokenType tt = ts.GetToken();

			if (tt != toMatch) 
				ReportError(messageId);
		}

		private void ReportError(string messageId)
		{
			_ok = false;
			throw new SyntaxException(messageId);
		}
	}
}

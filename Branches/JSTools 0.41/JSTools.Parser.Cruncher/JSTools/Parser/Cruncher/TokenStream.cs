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
using System.IO;
using System.Text;


namespace JSTools.Parser.Cruncher
{
	/// <summary>
	/// Token types.  These values correspond to JSTokenType values _lineBuffer
	/// jsscan.c.
	/// </summary>
	public enum TokenType
	{
		// start enum
		Error       = -1, // well-known as the only code < TokenType.EOF -> not written into the string buffer
		EOF         = 0,  // end of file token - (not EOF_CHAR)
		EOL         = 1,  // end of line
		// Beginning here are interpreter bytecodes. Their values
		// must not exceed 127.
		PopV        = 2,
		EnterWith   = 3,
		LeaveWith   = 4,
		Return      = 5,
		Goto        = 6,
		IfEq        = 7,
		IfNe        = 8,
		Dup         = 9,
		SetName     = 10,
		BitOr       = 11,
		BitXOr      = 12,
		BitAnd      = 13,
		Eq          = 14,
		Ne          = 15,
		Lt          = 16,
		Le          = 17,
		Gt          = 18,
		Ge          = 19,
		Lsh         = 20,
		Rsh         = 21,
		Ursh        = 22,
		Add         = 23,
		Sub         = 24,
		Mul         = 25,
		Div         = 26,
		Mod         = 27,
		BitNot      = 28,
		Neg         = 29,
		New         = 30,
		DelProp     = 31,
		TypeOf      = 32,
		NameInc     = 33,
		PropInc     = 34,
		ElemInc     = 35,
		NameDec     = 36,
		PropDec     = 37,
		ElemDec     = 38,
		GetProp     = 39,
		SetProp     = 40,
		GetElem     = 41,
		SetElem     = 42,
		Call        = 43,
		Name        = 44,
		Number      = 45,
		String      = 46,
		Zero        = 47,
		One         = 48,
		Null        = 49,
		This        = 50,
		False       = 51,
		True        = 52,
		ShEq        = 53,   // shallow equality (===)
		ShNe        = 54,   // shallow inequality (!==)
		Closure     = 55,
		RegExp      = 56,
		Pop         = 57,
		Pos         = 58,
		VarInc      = 59,
		VarDec      = 60,
		BindName    = 61,
		Throw       = 62,
		In          = 63,
		InstanceOf  = 64,
		GoSub       = 65,
		RetSub      = 66,
		CallSpecial = 67,
		GetThis     = 68,
		NewTemp     = 69,
		UseTemp     = 70,
		GetBase     = 71,
		GetVar      = 72,
		SetVar      = 73,
		Undefined   = 74,
		Try         = 75,
		EndTry      = 76,
		NewScope    = 77,
		TypeOfName  = 78,
		EnumInit    = 79,
		EnumNext    = 80,
		GetProto    = 81,
		GetParent   = 82,
		SetProto    = 83,
		SetParent   = 84,
		Scope       = 85,
		GetScopeParent = 86,
		ThisFn      = 87,
		JThrow      = 88,
		// End of interpreter bytecodes
		Semi        = 89,  // semicolon
		Lb          = 90,  // left and right brackets
		Rb          = 91,
		Lc          = 92,  // left and right curlies (braces)
		Rc          = 93,
		Lp          = 94,  // left and right parentheses
		Rp          = 95,
		Comma       = 96,  // comma operator
		Assign      = 97, // assignment ops (= += -= etc.)
		Hook        = 98, // conditional (?:)
		Colon       = 99,
		Or          = 100, // logical or (||)
		And         = 101, // logical and (&&)
		EqOp        = 102, // equality ops (== !=)
		RelOp       = 103, // relational ops (< <= > >=)
		ShOp        = 104, // shift ops (<< >> >>>)
		UnaryOp     = 105, // unary prefix operator
		Inc         = 106, // increment/decrement (++ --)
		Dec         = 107,
		Dot         = 108, // member operator (.)
		Primary     = 109, // true, false, null, this
		Function    = 110, // function keyword
		Export      = 111, // export keyword
		Import      = 112, // import keyword
		If          = 113, // if keyword
		Else        = 114, // else keyword
		Switch      = 115, // switch keyword
		Case        = 116, // case keyword
		Default     = 117, // default keyword
		While       = 118, // while keyword
		Do          = 119, // do keyword
		For         = 120, // for keyword
		Break       = 121, // break keyword
		Continue    = 122, // continue keyword
		Var         = 123, // var keyword
		With        = 124, // with keyword
		Catch       = 125, // catch keyword
		Finally     = 126, // finally keyword
		Reserved    = 127, // reserved keywords

		/** Added by Mike - these are JSOPs in the jsref, but I
		 * don't have them yet in the java implementation...
		 * so they go here.  Also whatever I needed.

		 * Most of these go in the 'op' field when returning
		 * more general token types, eg. 'TokenType.Div' as the op of 'TokenType.Assign'.
		 */
		Nop         = 128, // Nop
		Not         = 129, // etc.
		Pre         = 130, // for Inc, Dec nodes.
		Post        = 131,

		/**
		 * For JSOPs associated with keywords...
		 * eg. op = This; token = Primary
		 */

		Void        = 132,

		/* types used for the parse tree - these never get returned
		 * by the scanner.
		 */
		Block       = 133, // statement block
		ArrayLit    = 134, // array literal
		ObjLit      = 135, // object literal
		Label       = 136, // label
		Target      = 137,
		Loop        = 138,
		EnumDone    = 139,
		ExprStmt    = 140,
		Parent      = 141,
		Convert     = 142,
		Jsr         = 143,
		NewLocal    = 144,
		UseLocal    = 145,
		Script      = 146,   // top-level node for entire script

		LastToken  = 147
		// end enum
	}

	/// <summary>
	/// JSTokenStream _flags, mirroring those _inputReaderjsscan.h. 
	/// These are used by the parser to change/check the state of the scanner.
	/// </summary>
	[Flags()]
	internal enum JSTokenStreamFlags
	{
		None = 0,

		/// <summary>
		/// tokenize newlines
		/// </summary>
		NewLines			= 1 << 0,

		/// <summary>
		/// scanning inside function body
		/// </summary>
		Function			= 1 << 1,

		/// <summary>
		/// function has 'return Expr(;'
		/// </summary>
		ReturnExpression	= 1 << 2,

		/// <summary>
		/// function has 'return;'
		/// </summary>
		ReturnVoid			= 1 << 3,

		/// <summary>
		/// looking for a regular expression
		/// </summary>
		RegExp				= 1 << 4,

		/// <summary>
		/// stuff other than whitespace since
		/// </summary>
		DirtyLine			= 1 << 5
	}

	/// <summary>
	/// This class implements the JavaScript scanner.
	/// 
	/// It is based on the C source files jsscan.c and jsscan.h
	/// in the jsref package.
	/// </summary>
	internal class TokenStream 
	{
		/// <summary>
		/// For chars - because we need something out-of-range
		/// to check.  (And checking TokenType.EOF by exception is annoying.)
		/// Note distinction from TokenType.EOF token type!
		/// </summary>
		internal const int EOF_CHAR = -1;

		// instance variables
		private LineBuffer _lineBuffer;


		/* for JSTokenStreamFlags.RegExp, etc.
		 * should this be manipulated by gettor/settor functions?
		 * should it be passed to GetToken();
		 */
		private JSTokenStreamFlags _flags = JSTokenStreamFlags.None;
		private string _regExpFlags;

		private string _sourceName;
		private TokenType _pushbackToken;
		private int _tokenno;

		private TokenType _op;

		// Set this to an inital non-null value so that the Parser has
		// something to retrieve even if an error has occured and no
		// string is found.  Fosters one class of error, but saves lots of
		// code.
		private string _scannedString = string.Empty;
		private string _scannedNumber = null;

		private char[] _stringBuffer = new char[128];
		private int _stringBufferTop;
		private ScriptVersion _version = ScriptVersion.Unkonwn;


		internal string SourceName { get { return _sourceName; } }
		internal int LineNo { get { return _lineBuffer.LineNo; } }
		internal TokenType Op { get { return _op; } }
		internal string ScannedString { get { return _scannedString; } }
		internal string ScannedNumber { get { return _scannedNumber; } }
		internal string Line { get { return _lineBuffer.Line; } }
		internal int Offset { get { return _lineBuffer.Offset; } }
		internal int TokenNo { get { return _tokenno; } }
		internal bool EOF { get { return _lineBuffer.EOF; } }
		internal ScriptVersion Version { get { return _version; } }

		internal JSTokenStreamFlags Flags
		{
			get { return _flags; }
			set { _flags = value; }
		}

		internal string RegExpFlags
		{
			get { return _regExpFlags; }
			set { _regExpFlags = value; }
		}

		internal TokenStream(
			ScriptVersion version,
			TextReader inputReader,
			string sourceName,
			int lineno)
		{
			_lineBuffer = new LineBuffer(inputReader, lineno);
			_pushbackToken = TokenType.EOF;
			_sourceName = sourceName;
			_flags = JSTokenStreamFlags.None;
			_version = version;
		}



		/* return and pop the token from the stream if it matches...
		 * otherwise return null
		 */
		internal bool MatchToken(TokenType toMatch) 
		{
			TokenType token = GetToken();
			if (token == toMatch)
				return true;

			// didn't match, push back token
			_tokenno--;
			_pushbackToken = token;
			return false;
		}

		internal void ClearPushback() 
		{
			_pushbackToken = TokenType.EOF;
		}

		internal void UngetToken(TokenType tt) 
		{
			if (_pushbackToken != TokenType.EOF && tt != TokenType.Error) 
				throw new SyntaxException("msg.token.replaces.pushback");

			_pushbackToken = tt;
			_tokenno--;
		}

		internal TokenType PeekToken() 
		{
			TokenType result = GetToken();

			_pushbackToken = result;
			_tokenno--;
			return result;
		}

		internal TokenType PeekTokenSameLine() 
		{
			TokenType result;

			_flags |= JSTokenStreamFlags.NewLines;          // SCAN_NEWLINES from jsscan.h
			result = PeekToken();
			_flags &= ~JSTokenStreamFlags.NewLines;         // HIDE_NEWLINES from jsscan.h
			if (_pushbackToken == TokenType.EOL)
				_pushbackToken = TokenType.EOF;
			return result;
		}

		private void SkipLine() 
		{
			// skip to end of line
			int c;
			while ((c = _lineBuffer.Read()) != EOF_CHAR && c != '\n') { }
			_lineBuffer.Unread();
		}

		internal TokenType GetToken() 
		{
			int c;
			_tokenno++;

			// Check for pushed-back token
			if (_pushbackToken != TokenType.EOF) 
			{
				TokenType result = _pushbackToken;
				_pushbackToken = TokenType.EOF;
				return result;
			}

			// Eat whitespace, possibly sensitive to newlines.
			do 
			{
				c = _lineBuffer.Read();
				if (c == '\n') 
				{
					_flags &= ~JSTokenStreamFlags.DirtyLine;
					if ((_flags & JSTokenStreamFlags.NewLines) != 0)
						break;
				}
			} while (CharUtil.IsJSSpace(c) || c == '\n');

			if (c == EOF_CHAR)
				return TokenType.EOF;
			if (c != '-' && c != '\n')
				_flags |= JSTokenStreamFlags.DirtyLine;

			// identifier/keyword/instanceof?
			// watch out for starting with a <backslash>
			bool identifierStart;
			bool isUnicodeEscapeStart = false;
			if (c == '\\') 
			{
				c = _lineBuffer.Read();
				if (c == 'u') 
				{
					identifierStart = true;
					isUnicodeEscapeStart = true;
					_stringBufferTop = 0;
				} 
				else 
				{
					identifierStart = false;
					c = '\\';
					_lineBuffer.Unread();
				}
			} 
			else 
			{
				identifierStart = CharUtil.IsJavaScriptIdentifier((char)c);
				if (identifierStart) 
				{
					_stringBufferTop = 0;
					AddToString(c);
				}
			}

			if (identifierStart) 
			{
				bool containsEscape = isUnicodeEscapeStart;
				for (;;) 
				{
					if (isUnicodeEscapeStart) 
					{
						// strictly speaking we should probably push-back
						// all the bad characters if the <backslash>uXXXX
						// sequence is malformed. But since there isn't a
						// correct context (is there?) for a bad Unicode
						// escape sequence _inputReaderan identifier, we can report
						// an error here.
						int escapeVal = 0;
						for (int i = 0; i != 4; ++i) 
						{
							c = _lineBuffer.Read();
							escapeVal = (escapeVal << 4) | CharUtil.XDigitToInt(c);
							// Next check takes care about c < 0 and bad escape
							if (escapeVal < 0) { break; }
						}

						if (escapeVal < 0) 
							throw new SyntaxException("msg.invalid.escape");

						AddToString(escapeVal);
						isUnicodeEscapeStart = false;
					} 
					else 
					{
						c = _lineBuffer.Read();
						if (c == '\\') 
						{
							c = _lineBuffer.Read();
							if (c == 'u') 
							{
								isUnicodeEscapeStart = true;
								containsEscape = true;
							} 
							else
							{
								throw new SyntaxException("msg.illegal.character");
							}
						} 
						else 
						{
							if (!CharUtil.IsJavaScriptIdentifierPart((char)c)) 
								break;

							AddToString(c);
						}
					}
				}
				_lineBuffer.Unread();

				string str = GetStringFromBuffer();
				if (!containsEscape) 
				{
					// OPT we shouldn't have to make a string (object!) to
					// check if it's a keyword.

					// map keyword
					KeywordMapper.KeywordBucket bucket = KeywordMapper.Instance.StringToKeyword(str);
					_op = bucket.Operator;
					TokenType result = bucket.Token;

					// return the corresponding token if it's a keyword
					if (result != TokenType.EOF) 
						return result;
				}
				_scannedString = str;
				return TokenType.Name;
			}

			// is it a number?
			if (CharUtil.IsDigit(c) || (c == '.' && CharUtil.IsDigit(_lineBuffer.Peek()))) 
			{
				_stringBufferTop = 0;
				int baseDigit = 10;

				if (c == '0') 
				{
					c = _lineBuffer.Read();
					if (c == 'x' || c == 'X') 
					{
						AddToString('0');
						AddToString(c);

						baseDigit = 16;
						c = _lineBuffer.Read();
					} 
					else if (CharUtil.IsDigit(c)) 
					{
						baseDigit = 8;
					} 
					else 
					{
						AddToString('0');
					}
				}

				if (baseDigit == 16) 
				{
					while (0 <= CharUtil.XDigitToInt(c)) 
					{
						AddToString(c);
						c = _lineBuffer.Read();
					}
				} 
				else 
				{
					while ('0' <= c && c <= '9') 
					{
						/*
						 * We permit 08 and 09 as decimal numbers, which
						 * makes our behavior a superset of the ECMA
						 * numeric grammar.
						 */
						if (baseDigit == 8 && c >= '8') 
							baseDigit = 10;

						AddToString(c);
						c = _lineBuffer.Read();
					}
				}

				if (baseDigit == 10 && (c == '.' || c == 'e' || c == 'E')) 
				{
					if (c == '.') 
					{
						do 
						{
							AddToString(c);
							c = _lineBuffer.Read();
						} while (CharUtil.IsDigit(c));
					}
					if (c == 'e' || c == 'E') 
					{
						AddToString(c);
						c = _lineBuffer.Read();
						if (c == '+' || c == '-') 
						{
							AddToString(c);
							c = _lineBuffer.Read();
						}
						if (!CharUtil.IsDigit(c)) 
						{
							throw new SyntaxException("msg.missing.exponent");
						}
						do 
						{
							AddToString(c);
							c = _lineBuffer.Read();
						} while (CharUtil.IsDigit(c));
					}
				}
			
				_lineBuffer.Unread();
				_scannedNumber = GetStringFromBuffer();

				return TokenType.Number;
			}

			// is it a string?
			if (c == '"' || c == '\'') 
			{
				int quoteChar = c;
				_stringBufferTop = 0;

				AddToString(quoteChar);

				do
				{
					c = _lineBuffer.Read();

					if (c == '\n' || c == EOF_CHAR) 
					{
						throw new SyntaxException("msg.unterminated.string.lit");
					}

					// We've hit an escaped character
					if (c == '\\') 
					{
						AddToString(c);
						AddToString(_lineBuffer.Read());
						continue;
					}

					AddToString(c);
				}
				while (c != quoteChar);

				_scannedString = GetStringFromBuffer();
				return TokenType.String;
			}

			switch (c)
			{
				case '\n': return TokenType.EOL;
				case ';': return TokenType.Semi;
				case '[': return TokenType.Lb;
				case ']': return TokenType.Rb;
				case '{': return TokenType.Lc;
				case '}': return TokenType.Rc;
				case '(': return TokenType.Lp;
				case ')': return TokenType.Rp;
				case ',': return TokenType.Comma;
				case '?': return TokenType.Hook;
				case ':': return TokenType.Colon;
				case '.': return TokenType.Dot;

				case '|':
					if (_lineBuffer.Match('|')) 
					{
						return TokenType.Or;
					} 
					else if (_lineBuffer.Match('=')) 
					{
						_op = TokenType.BitOr;
						return TokenType.Assign;
					} 
					else 
					{
						return TokenType.BitOr;
					}

				case '^':
					if (_lineBuffer.Match('=')) 
					{
						_op = TokenType.BitXOr;
						return TokenType.Assign;
					} 
					else 
					{
						return TokenType.BitXOr;
					}

				case '&':
					if (_lineBuffer.Match('&')) 
					{
						return TokenType.And;
					} 
					else if (_lineBuffer.Match('=')) 
					{
						_op = TokenType.BitAnd;
						return TokenType.Assign;
					} 
					else 
					{
						return TokenType.BitAnd;
					}

				case '=':
					if (_lineBuffer.Match('=')) 
					{
						if (_lineBuffer.Match('='))
							_op = TokenType.ShEq;
						else
							_op = TokenType.Eq;
						return TokenType.EqOp;
					} 
					else 
					{
						_op = TokenType.Nop;
						return TokenType.Assign;
					}

				case '!':
					if (_lineBuffer.Match('=')) 
					{
						if (_lineBuffer.Match('='))
							_op = TokenType.ShNe;
						else
							_op = TokenType.Ne;
						return TokenType.EqOp;
					} 
					else 
					{
						_op = TokenType.Not;
						return TokenType.UnaryOp;
					}

				case '<':
					/* NB:treat HTML begin-comment as comment-till-eol */
					if (_lineBuffer.Match('!')) 
					{
						if (_lineBuffer.Match('-')) 
						{
							if (_lineBuffer.Match('-')) 
							{
								SkipLine();
								return GetToken();  // _inputReaderplace of 'goto retry'
							}
							_lineBuffer.Unread();
						}
						_lineBuffer.Unread();
					}
					if (_lineBuffer.Match('<')) 
					{
						if (_lineBuffer.Match('=')) 
						{
							_op = TokenType.Lsh;
							return TokenType.Assign;
						} 
						else 
						{
							_op = TokenType.Lsh;
							return TokenType.ShOp;
						}
					} 
					else 
					{
						if (_lineBuffer.Match('=')) 
						{
							_op = TokenType.Le;
							return TokenType.RelOp;
						} 
						else 
						{
							_op = TokenType.Lt;
							return TokenType.RelOp;
						}
					}

				case '>':
					if (_lineBuffer.Match('>')) 
					{
						if (_lineBuffer.Match('>')) 
						{
							if (_lineBuffer.Match('=')) 
							{
								_op = TokenType.Ursh;
								return TokenType.Assign;
							} 
							else 
							{
								_op = TokenType.Ursh;
								return TokenType.ShOp;
							}
						} 
						else 
						{
							if (_lineBuffer.Match('=')) 
							{
								_op = TokenType.Rsh;
								return TokenType.Assign;
							} 
							else 
							{
								_op = TokenType.Rsh;
								return TokenType.ShOp;
							}
						}
					} 
					else 
					{
						if (_lineBuffer.Match('=')) 
						{
							_op = TokenType.Ge;
							return TokenType.RelOp;
						} 
						else 
						{
							_op = TokenType.Gt;
							return TokenType.RelOp;
						}
					}

				case '*':
					if (_lineBuffer.Match('=')) 
					{
						_op = TokenType.Mul;
						return TokenType.Assign;
					} 
					else 
					{
						return TokenType.Mul;
					}

				case '/':
					// is it a // comment?
					if (_lineBuffer.Match('/')) 
					{
						SkipLine();
						return GetToken();
					}
					if (_lineBuffer.Match('*')) 
					{
						while ((c = _lineBuffer.Read()) != -1 &&
							!(c == '*' && _lineBuffer.Match('/'))) 
						{
							; // empty loop body
						}
						if (c == EOF_CHAR) 
						{
							throw new SyntaxException("msg.unterminated.comment");
						}
						return GetToken();  // `goto retry'
					}

					// is it a regexp?
					if ((_flags & JSTokenStreamFlags.RegExp) != 0) 
					{
						_stringBufferTop = 0;
						while ((c = _lineBuffer.Read()) != '/') 
						{
							if (c == '\n' || c == EOF_CHAR) 
							{
								throw new SyntaxException("msg.unterminated.re.lit");
							}
							if (c == '\\') 
							{
								AddToString(c);
								c = _lineBuffer.Read();
							}

							AddToString(c);
						}
						int reEnd = _stringBufferTop;

						while (true) 
						{
							if (_lineBuffer.Match('g'))
								AddToString('g');
							else if (_lineBuffer.Match('i'))
								AddToString('i');
							else if (_lineBuffer.Match('m'))
								AddToString('m');
							else
								break;
						}

						if (CharUtil.IsAlpha(_lineBuffer.Peek())) 
							throw new SyntaxException("msg.invalid.re.flag");

						_scannedString = new string(_stringBuffer, 0, reEnd);
						_regExpFlags = new string(_stringBuffer, reEnd,
							_stringBufferTop - reEnd);
						return TokenType.RegExp;
					}


					if (_lineBuffer.Match('=')) 
					{
						_op = TokenType.Div;
						return TokenType.Assign;
					} 
					else 
					{
						return TokenType.Div;
					}

				case '%':
					_op = TokenType.Mod;
					if (_lineBuffer.Match('=')) 
					{
						return TokenType.Assign;
					} 
					else 
					{
						return TokenType.Mod;
					}

				case '~':
					_op = TokenType.BitNot;
					return TokenType.UnaryOp;

				case '+':
					if (_lineBuffer.Match('=')) 
					{
						_op = TokenType.Add;
						return TokenType.Assign;
					} 
					else if (_lineBuffer.Match('+')) 
					{
						return TokenType.Inc;
					} 
					else 
					{
						return TokenType.Add;
					}

				case '-':
					TokenType tt = TokenType.EOF;

					if (_lineBuffer.Match('=')) 
					{
						_op = TokenType.Sub;
						tt = TokenType.Assign;
					} 
					else if (_lineBuffer.Match('-')) 
					{
						if (0 == (_flags & JSTokenStreamFlags.DirtyLine)) 
						{
							// treat HTML end-comment after possible whitespace
							// after line start as comment-utill-eol
							if (_lineBuffer.Match('>')) 
							{
								SkipLine();
								return GetToken();
							}
						}
						tt = TokenType.Dec;
					} 
					else 
					{
						tt = TokenType.Sub;
					}
					_flags |= JSTokenStreamFlags.DirtyLine;
					return tt;

				default:
					throw new SyntaxException("msg.illegal.character");
			}
		}

		private string GetStringFromBuffer() 
		{
			return new string(_stringBuffer, 0, _stringBufferTop);
		}

		private void AddToString(int c) 
		{
			if (_stringBufferTop == _stringBuffer.Length) 
			{
				char[] tmp = new char[_stringBuffer.Length * 2];
				Array.Copy(_stringBuffer, 0, tmp, 0, _stringBufferTop);
				_stringBuffer = tmp;
			}
			_stringBuffer[_stringBufferTop++] = (char)c;
		}
	}
}

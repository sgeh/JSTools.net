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

namespace JSTools.Parser.Cruncher
{
	internal enum LoopType
	{
		DoWhile = 0,
		While = 1,
		For = 2
	}

	/// <summary>
	/// This class allows the creation of nodes, and follows the Factory pattern.
	/// </summary>
	internal class IRFactory 
	{
		// Only needed to get file/line information. Could create an interface
		// that TokenStream implements if we want to make the connection less
		// direct.
		private TokenStream _ts;

		internal ScriptVersion Version
		{
			get { return _ts.Version; }
		}

		internal IRFactory(TokenStream ts) 
		{
			_ts = ts;
		}

		/**
		 * Script (for associating file/url names with toplevel scripts.)
		 */
		internal Node CreateScript(Node body, string sourceName,
			int baseLineno, int endLineno, char[] source)
		{
			Node result = Node.NewString(TokenType.Script, sourceName);
			Node children = body.FirstChild;
			if (children != null)
				result.AddChildrenToBack(children);
			result.PutProp(NodeProperty.SourceName, sourceName);
			result.PutIntProp(NodeProperty.BaseLineNo, baseLineno);
			result.PutIntProp(NodeProperty.EndLineNo, endLineno);
			if (source != null)
				result.PutProp(NodeProperty.Source, source);
			return result;
		}

		/**
		 * Leaf
		 */
		internal Node CreateLeaf(TokenType nodeType) 
		{
			return new Node(nodeType);
		}

		internal Node CreateLeaf(TokenType nodeType, TokenType nodeOp) 
		{
			return new Node(nodeType, nodeOp);
		}

		internal TokenType GetLeafType(Node leaf) 
		{
			return leaf.Type;
		}

		/**
		 * Statement leaf nodes.
		 */

		internal Node CreateSwitch(int lineno) 
		{
			return new Node(TokenType.Switch, lineno);
		}

		internal Node CreateVariables(int lineno) 
		{
			return new Node(TokenType.Var, lineno);
		}

		internal Node CreateExprStatement(Node expr, int lineno) 
		{
			return new Node(TokenType.ExprStmt, expr, lineno);
		}

		/**
		 * Name
		 */
		internal Node CreateName(string name) 
		{
			return Node.NewString(TokenType.Name, name);
		}

		/**
		 * string (for literals)
		 */
		internal Node CreateString(string stringToCreate) 
		{
			return Node.NewString(stringToCreate);
		}

		/**
		 * Number (for literals)
		 */
		internal Node CreateNumber(string number) 
		{
			return Node.NewNumber(number);
		}

		/**
		 * Catch clause of try/catch/finally
		 * @param varName the name of the variable to bind to the exception
		 * @param catchCond the condition under which to catch the exception.
		 *                  May be null if no condition is given.
		 * @param stmts the statements in the catch clause
		 * @param lineno the starting line number of the catch clause
		 */
		internal Node CreateCatch(string varName, Node catchCond, Node stmts,
			int lineno)
		{
			if (catchCond == null) 
			{
				catchCond = new Node(TokenType.Primary, TokenType.True);
			}
			return new Node(TokenType.Catch, CreateName(varName),
				catchCond, stmts, lineno);
		}

		/**
		 * Throw
		 */
		internal Node CreateThrow(Node expr, int lineno) 
		{
			return new Node(TokenType.Throw, expr, lineno);
		}

		/**
		 * Return
		 */
		internal Node CreateReturn(Node expr, int lineno) 
		{
			return expr == null
				? new Node(TokenType.Return, lineno)
				: new Node(TokenType.Return, expr, lineno);
		}

		/**
		 * Label
		 */
		internal Node CreateLabel(string label, int lineno) 
		{
			Node result = new Node(TokenType.Label, lineno);
			Node name = Node.NewString(TokenType.Name, label);
			result.AddChildToBack(name);
			return result;
		}

		/**
		 * Break (possibly labeled)
		 */
		internal Node CreateBreak(string label, int lineno) 
		{
			Node result = new Node(TokenType.Break, lineno);
			if (label == null) 
			{
				return result;
			} 
			else 
			{
				Node name = Node.NewString(TokenType.Name, label);
				result.AddChildToBack(name);
				return result;
			}
		}

		/**
		 * Continue (possibly labeled)
		 */
		internal Node CreateContinue(string label, int lineno) 
		{
			Node result = new Node(TokenType.Continue, lineno);
			if (label == null) 
			{
				return result;
			} 
			else 
			{
				Node name = Node.NewString(TokenType.Name, label);
				result.AddChildToBack(name);
				return result;
			}
		}

		/**
		 * Statement block
		 * Creates the empty statement block
		 * Must make subsequent calls to add statements to the node
		 */
		internal Node CreateBlock(int lineno) 
		{
			return new Node(TokenType.Block, lineno);
		}

		private Node CreateFunctionNode(string name, Node args,
			Node statements)
		{
			if (name == null)
				name = string.Empty;
			return new FunctionNode(name, args, statements);
		}

		internal Node CreateFunction(string name, Node args, Node statements,
			string sourceName, int baseLineno,
			int endLineno, char[] source,
			bool isExpr)
		{
			FunctionNode f = (FunctionNode) CreateFunctionNode(name, args,
				statements);
			f.FunctionType = (isExpr ? FunctionType.FunctionExpression : FunctionType.FunctionStatement);
			f.PutProp(NodeProperty.SourceName, sourceName);
			f.PutIntProp(NodeProperty.BaseLineNo, baseLineno);
			f.PutIntProp(NodeProperty.EndLineNo, endLineno);
			if (source != null)
				f.PutProp(NodeProperty.Source, source);
			Node result = Node.NewString(TokenType.Function, name);
			result.PutProp(NodeProperty.Function, f);
			return result;
		}

		internal void SetFunctionExpressionStatement(Node n) 
		{
			FunctionNode f = (FunctionNode)n.GetProp(NodeProperty.Function);
			f.FunctionType = FunctionType.FunctionExpressionStatement;
		}

		/**
		 * Add a child to the back of the given node.  This function
		 * breaks the Factory abstraction, but it removes a requirement
		 * from implementors of Node.
		 */
		internal void AddChildToBack(Node parent, Node child) 
		{
			parent.AddChildToBack(child);
		}

		/**
		 * While
		 */
		internal Node CreateWhile(Node cond, Node body, int lineno) 
		{
			return CreateLoop(LoopType.While, body, cond, null, null,
				lineno);
		}

		/**
		 * DoWhile
		 */
		internal Node CreateDoWhile(Node body, Node cond, int lineno) 
		{
			return CreateLoop(LoopType.DoWhile, body, cond, null, null,
				lineno);
		}

		/**
		 * For
		 */
		internal Node CreateFor(Node init, Node test, Node incr,
			Node body, int lineno)
		{
			return CreateLoop(LoopType.For, body, test,
				init, incr, lineno);
		}

		private Node CreateLoop(LoopType loopType, Node body, Node cond,
			Node init, Node incr, int lineno)
		{
			Node bodyTarget = new Node(TokenType.Target);
			Node condTarget = new Node(TokenType.Target);
			if (loopType == LoopType.For && cond.Type == TokenType.Void) 
			{
				cond = new Node(TokenType.Primary, TokenType.True);
			}
			Node IFEQ = new Node(TokenType.IfEq, (Node)cond);
			IFEQ.PutProp(NodeProperty.Target, bodyTarget);
			Node breakTarget = new Node(TokenType.Target);

			Node result = new Node(TokenType.Loop, lineno);
			result.AddChildToBack(bodyTarget);
			result.AddChildrenToBack(body);
			result.AddChildToBack(condTarget);
			result.AddChildToBack(IFEQ);
			result.AddChildToBack(breakTarget);

			result.PutProp(NodeProperty.Break, breakTarget);
			Node continueTarget = condTarget;

			if (loopType == LoopType.While || loopType == LoopType.For) 
			{
				// Just add a GOTO to the condition in the do..while
				Node GOTO = new Node(TokenType.Goto);
				GOTO.PutProp(NodeProperty.Target, condTarget);
				result.AddChildToFront(GOTO);

				if (loopType == LoopType.For) 
				{
					if (init.Type != TokenType.Void) 
					{
						if (init.Type != TokenType.Var) 
						{
							init = new Node(TokenType.Pop, init);
						}
						result.AddChildToFront(init);
					}
					Node incrTarget = new Node(TokenType.Target);
					result.AddChildAfter(incrTarget, body);
					if (incr.Type != TokenType.Void) 
					{
						incr = (Node)CreateUnary(TokenType.Pop, incr);
						result.AddChildAfter(incr, incrTarget);
					}
					continueTarget = incrTarget;
				}
			}

			result.PutProp(NodeProperty.Continue, continueTarget);

			return result;
		}

		/**
		 * For .. In
		 *
		 */
		internal Node CreateForIn(Node lhsNode, Node objNode, Node body, int lineno) 
		{
			TokenType type = lhsNode.Type;

			Node lvalue = lhsNode;
			switch (type) 
			{

				case TokenType.Name:
				case TokenType.GetProp:
				case TokenType.GetElem:
					break;

				case TokenType.Var:
					/*
					 * check that there was only one variable given.
					 * we can't do this in the parser, because then the
					 * parser would have to know something about the
					 * 'init' node of the for-in loop.
					 */
					Node lastChild = lhsNode.FirstChild;

					if (lhsNode.FirstChild != lastChild) 
						ReportError("msg.mult.index");

					lvalue = Node.NewString(TokenType.Name, lastChild.ToString());
					break;

				default:
					ReportError("msg.bad.for.in.lhs");
					break;
			}

			Node init = new Node(TokenType.EnumInit, objNode);
			Node next = new Node(TokenType.EnumNext);
			next.PutProp(NodeProperty.Enum, init);
			Node temp = CreateNewTemp(next);
			Node cond = new Node(TokenType.EqOp, TokenType.Ne);
			cond.AddChildToBack(temp);
			cond.AddChildToBack(new Node(TokenType.Primary, TokenType.Null));
			Node newBody = new Node(TokenType.Block);
			Node assign = (Node) CreateAssignment(TokenType.Nop, lvalue, CreateUseTemp(temp), false);
			newBody.AddChildToBack(new Node(TokenType.Pop, assign));
			newBody.AddChildToBack((Node) body);
			Node result = (Node) CreateWhile(cond, newBody, lineno);

			result.AddChildToFront(init);
			if (type == TokenType.Var)
				result.AddChildToFront(lhsNode);

			Node done = new Node(TokenType.EnumDone);
			done.PutProp(NodeProperty.Enum, init);
			result.AddChildToBack(done);

			return result;
		}

		/**
		 * Try/Catch/Finally
		 *
		 * The IRFactory tries to express as much as possible in the tree;
		 * the responsibilities remaining for Codegen are to add the Java
		 * handlers: (Either (but not both) of TARGET and FINALLY might not
		 * be defined)

		 * - a catch handler for javascript exceptions that unwraps the
		 * exception onto the stack and GOTOes to the catch target -
		 * TARGET_PROP in the try node.

		 * - a finally handler that catches any exception, stores it to a
		 * temporary, and JSRs to the finally target - FINALLY_PROP in the
		 * try node - before re-throwing the exception.

		 * ... and a goto to GOTO around these handlers.
		 */
		internal Node CreateTryCatchFinally(Node trynode, Node catchNodes,
			Node finallyNode, int lineno)
		{
			// short circuit
			if (trynode.Type == TokenType.Block && !trynode.HasChildren)
				return trynode;

			Node pn = new Node(TokenType.Try, trynode, lineno);
			bool hasCatch = catchNodes.HasChildren;
			bool hasFinally = false;
			Node finallyTarget = null;

			if (finallyNode != null) 
			{
				hasFinally = (finallyNode.Type != TokenType.Block
					|| finallyNode.HasChildren);
				if (hasFinally) 
				{
					// make a TARGET for the finally that the tcf node knows about
					finallyTarget = new Node(TokenType.Target);
					pn.PutProp(NodeProperty.Finally, finallyTarget);

					// add jsr finally to the try block
					Node jsrFinally = new Node(TokenType.Jsr);
					jsrFinally.PutProp(NodeProperty.Target, finallyTarget);
					pn.AddChildToBack(jsrFinally);
				}
			}

			// short circuit
			if (!hasFinally && !hasCatch)  // bc finally might be an empty block...
				return trynode;

			Node endTarget = new Node(TokenType.Target);
			Node GOTOToEnd = new Node(TokenType.Goto);
			GOTOToEnd.PutProp(NodeProperty.Target, endTarget);
			pn.AddChildToBack(GOTOToEnd);

			if (hasCatch) 
			{
				/*
				 *
				 Given

				 try {
				 throw 3;
				 } catch (e: e instanceof object) {
				 print("object");
				 } catch (e2) {
				 print(e2);
				 }

				 rewrite as

				 try {
				 throw 3;
				 } catch (x) {
				 o = newScope();
				 o.e = x;
				 with (o) {
				 if (e instanceof object) {
				 print("object");
				 }
				 }
				 o2 = newScope();
				 o2.e2 = x;
				 with (o2) {
				 if (true) {
				 print(e2);
				 }
				 }
				 }
				 */
				// make a TARGET for the catch that the tcf node knows about
				Node catchTarget = new Node(TokenType.Target);
				pn.PutProp(NodeProperty.Target, catchTarget);
				// mark it
				pn.AddChildToBack(catchTarget);

				// get the exception object and store it in a temp
				Node exn = CreateNewLocal(new Node(TokenType.Void));
				pn.AddChildToBack(new Node(TokenType.Pop, exn));

				Node endCatch = new Node(TokenType.Target);

				// add [jsr finally?] goto end to each catch block
				// expects catchNode children to be (cond block) pairs.
				Node cb = catchNodes.FirstChild;
				while (cb != null) 
				{
					Node catchStmt = new Node(TokenType.Block);
					int catchLineNo = cb.LineNo;

					Node name = cb.FirstChild;
					Node cond = name.Next;
					Node catchBlock = cond.Next;
					cb.RemoveChild(name);
					cb.RemoveChild(cond);
					cb.RemoveChild(catchBlock);

					Node newScope = CreateNewLocal(new Node(TokenType.NewScope));
					Node initScope = new Node(TokenType.SetProp, newScope,
						Node.NewString(
						name.ToString()),
						CreateUseLocal(exn));
					catchStmt.AddChildToBack(new Node(TokenType.Pop, initScope));

					catchBlock.AddChildToBack(new Node(TokenType.LeaveWith));
					Node GOTOToEndCatch = new Node(TokenType.Goto);
					GOTOToEndCatch.PutProp(NodeProperty.Target, endCatch);
					catchBlock.AddChildToBack(GOTOToEndCatch);

					Node ifStmt = (Node) CreateIf(cond, catchBlock, null, catchLineNo);
					// Try..catch produces "with" code in order to limit
					// the scope of the exception object.
					// OPT: We should be able to figure out the correct
					//      scoping at compile-time and avoid the
					//      runtime overhead.
					Node withStmt = (Node) CreateWith(CreateUseLocal(newScope),
						ifStmt, catchLineNo);
					catchStmt.AddChildToBack(withStmt);

					pn.AddChildToBack(catchStmt);

					// move to next cb
					cb = cb.Next;
				}

				// Generate code to rethrow if no catch clause was executed
				Node rethrow = new Node(TokenType.Throw, CreateUseLocal(exn));
				pn.AddChildToBack(rethrow);

				pn.AddChildToBack(endCatch);
				// add a JSR finally if needed
				if (hasFinally) 
				{
					Node jsrFinally = new Node(TokenType.Jsr);
					jsrFinally.PutProp(NodeProperty.Target, finallyTarget);
					pn.AddChildToBack(jsrFinally);
					Node GOTO = new Node(TokenType.Goto);
					GOTO.PutProp(NodeProperty.Target, endTarget);
					pn.AddChildToBack(GOTO);
				}
			}

			if (hasFinally) 
			{
				pn.AddChildToBack(finallyTarget);
				Node returnTemp = CreateNewLocal(new Node(TokenType.Void));
				Node popAndMake = new Node(TokenType.Pop, returnTemp);
				pn.AddChildToBack(popAndMake);
				pn.AddChildToBack(finallyNode);
				Node ret = CreateUseLocal(returnTemp);

				// add the magic prop that makes it output a RET
				ret.PutProp(NodeProperty.Target, true);
				pn.AddChildToBack(ret);
			}
			pn.AddChildToBack(endTarget);
			return pn;
		}

		/**
		 * Throw, Return, Label, Break and Continue are defined in ASTFactory.
		 */

		/**
		 * With
		 */
		internal Node CreateWith(Node obj, Node body, int lineno) 
		{
			Node result = new Node(TokenType.Block, lineno);
			result.AddChildToBack(new Node(TokenType.EnterWith, obj));
			Node bodyNode = new Node(TokenType.With, body, lineno);
			result.AddChildrenToBack(bodyNode);
			result.AddChildToBack(new Node(TokenType.LeaveWith));
			return result;
		}

		/**
		 * Array Literal
		 * <BR>createArrayLiteral rewrites its argument as array creation
		 * plus a series of array element entries, so later compiler
		 * stages don't need to know about array literals.
		 */
		internal Node CreateArrayLiteral(Node obj) 
		{
			Node array;
			array = new Node(TokenType.New,
				Node.NewString(TokenType.Name, "Array"));
			Node temp = CreateNewTemp(array);

			Node elem = null;
			int i = 0;
			Node comma = new Node(TokenType.Comma, temp);
			for (Node cursor = obj.FirstChild; cursor != null;) 
			{
				// Move cursor to cursor.next before elem.next can be
				// altered in new Node constructor
				elem = cursor;
				cursor = cursor.Next;
				if (elem.Type == TokenType.Primary && elem.Operation == TokenType.Undefined)
				{
					i++;
					continue;
				}
				Node addelem = new Node(TokenType.SetElem, CreateUseTemp(temp),
					Node.NewNumber(i.ToString()), elem);
				i++;
				comma.AddChildToBack(addelem);
			}

			/*
			 * If the version is 120, then new Array(4) means create a new
			 * array with 4 as the first element.  In this case, we might
			 * need to explicitly check against trailing undefined
			 * elements in the array literal, and set the length manually
			 * if these occur.  Otherwise, we can add an argument to the
			 * node specifying new Array() to provide the array length.
			 * (Which will make Array optimizations involving allocating a
			 * Java array to back the javascript array work better.)
			 */
			if (Version == ScriptVersion.Version_1_2) 
			{
				/* When last array element is empty, we need to set the
				 * length explicitly, because we can't depend on SETELEM
				 * to do it for us - because empty [,,] array elements
				 * never set anything at all. */
				if (elem != null &&
					elem.Type == TokenType.Primary &&
					elem.Operation == TokenType.Undefined)
				{
					Node setlength = new Node(TokenType.SetProp,
						CreateUseTemp(temp),
						Node.NewString("length"),
						Node.NewNumber(i.ToString()));
					comma.AddChildToBack(setlength);
				}
			} 
			else 
			{
				array.AddChildToBack(Node.NewNumber(i.ToString()));
			}
			comma.AddChildToBack(CreateUseTemp(temp));
			return comma;
		}

		/**
		 * object Literals
		 * <BR> createObjectLiteral rewrites its argument as object
		 * creation plus object property entries, so later compiler
		 * stages don't need to know about object literals.
		 */
		internal Node CreateObjectLiteral(Node obj) 
		{
			Node result = new Node(TokenType.New, Node.NewString(TokenType.Name,
				"object"));
			Node temp = CreateNewTemp(result);

			Node comma = new Node(TokenType.Comma, temp);
			for (Node cursor = obj.FirstChild; cursor != null;) 
			{
				Node n = cursor;
				cursor = cursor.Next;
				TokenType op = (n.Type == TokenType.Name) ? TokenType.SetProp : TokenType.SetElem;

				// Move cursor before next.next can be altered in new Node
				Node next = cursor;
				cursor = cursor.Next;
				Node addelem = new Node(op, CreateUseTemp(temp), n, next);
				comma.AddChildToBack(addelem);
			}
			comma.AddChildToBack(CreateUseTemp(temp));
			return comma;
		}

		/**
		 * Regular expressions
		 */
		internal Node CreateRegExp(string regExpString, string flags) 
		{
			if (flags.Length == 0)
				return new Node(TokenType.RegExp, Node.NewString(regExpString));
			else
				return new Node(TokenType.RegExp, Node.NewString(regExpString), Node.NewString(flags));
		}

		/**
		 * If statement
		 */
		internal Node CreateIf(Node cond, Node ifTrue, Node ifFalse,
			int lineno)
		{
			Node result = new Node(TokenType.Block, lineno);
			Node ifNotTarget = new Node(TokenType.Target);
			Node IFNE = new Node(TokenType.IfNe, cond);
			IFNE.PutProp(NodeProperty.Target, ifNotTarget);

			result.AddChildToBack(IFNE);
			result.AddChildrenToBack(ifTrue);

			if (ifFalse != null) 
			{
				Node GOTOToEnd = new Node(TokenType.Goto);
				Node endTarget = new Node(TokenType.Target);
				GOTOToEnd.PutProp(NodeProperty.Target, endTarget);
				result.AddChildToBack(GOTOToEnd);
				result.AddChildToBack(ifNotTarget);
				result.AddChildrenToBack(ifFalse);
				result.AddChildToBack(endTarget);
			} 
			else 
			{
				result.AddChildToBack(ifNotTarget);
			}

			return result;
		}

		internal Node CreateTernary(Node cond, Node ifTrue, Node ifFalse) 
		{
			return CreateIf(cond, ifTrue, ifFalse, -1);
		}

		/**
		 * Unary
		 */
		internal Node CreateUnary(TokenType nodeType, Node childNode) 
		{
			if (nodeType == TokenType.DelProp) 
			{
				TokenType childType = childNode.Type;
				Node left;
				Node right;
				if (childType == TokenType.Name) 
				{
					// Transform Delete(Name "a")
					//  to Delete(Bind("a"), string("a"))
					childNode.Type = TokenType.BindName;
					left = childNode;
					right = childNode.CloneNode();
					right.Type = TokenType.String;
				} 
				else if (childType == TokenType.GetProp ||
					childType == TokenType.GetElem)
				{
					left = childNode.FirstChild;
					right = childNode.FirstChild;
					childNode.RemoveChild(left);
					childNode.RemoveChild(right);
				} 
				else 
				{
					return new Node(TokenType.Primary, TokenType.True);
				}
				return new Node(nodeType, left, right);
			}
			return new Node(nodeType, childNode);
		}

		internal Node CreateUnary(TokenType nodeType, TokenType nodeOp, Node childNode) 
		{
			TokenType childType = childNode.Type;

			if (nodeOp == TokenType.TypeOf && childType == TokenType.Name)
			{
				childNode.Type = TokenType.TypeOf;
				return childNode;
			}

			if (nodeType == TokenType.Inc || nodeType == TokenType.Dec) 
			{

				if (!HasSideEffects(childNode)
					&& (nodeOp == TokenType.Post)
					&& (childType == TokenType.Name
					|| childType == TokenType.GetProp
					|| childType == TokenType.GetElem))
				{
					// if it's not a LHS type, createAssignment (below) will throw
					// an exception.
					return new Node(nodeType, childNode);
				}

				/*
				 * Transform INC/DEC ops to +=1, -=1,
				 * expecting later optimization of all +/-=1 cases to INC, DEC.
				 */
				// we have to use Double for now, because
				// 0.0 and 1.0 are stored as dconst_[01],
				// and using a Float creates a stack mismatch.
				Node rhs = (Node) CreateNumber("1");

				return CreateAssignment(nodeType == TokenType.Inc
					? TokenType.Add
					: TokenType.Sub,
					childNode,
					rhs,
					nodeOp == TokenType.Post);
			}

			Node result = new Node(nodeType, nodeOp);
			result.AddChildToBack(childNode);
			return result;
		}

		/**
		 * Binary
		 */
		internal Node CreateBinary(TokenType nodeType, Node left, Node right) 
		{
			switch (nodeType) 
			{

				case TokenType.Dot:
					nodeType = TokenType.GetProp;
					Node idNode = right;
					idNode.Type = TokenType.String;
					string id = idNode.ToString();
					if (id == "__proto__" || id == "__parent__")
					{
						Node result = new Node(nodeType, (Node) left);
						result.PutProp(NodeProperty.SpecialProperty, id);
						return result;
					}
					break;

				case TokenType.Lb:
					// OPT: could optimize to GETPROP iff string can't be a number
					nodeType = TokenType.GetElem;
					break;
			}
			return new Node(nodeType, left, right);
		}

		internal Node CreateBinary(TokenType nodeType, TokenType nodeOp, Node left,
			Node right)
		{
			if (nodeType == TokenType.Assign) 
			{
				return CreateAssignment(nodeOp, left, right, false);
			}
			return new Node(nodeType, left, right, nodeOp);
		}

		internal Node CreateAssignment(TokenType nodeOp, Node left, Node right, bool postfix)
		{
			TokenType nodeType = left.Type;
			string idString;
			Node id = null;
			switch (nodeType) 
			{
				case TokenType.Name:
					return CreateSetName(nodeOp, left, right, postfix);

				case TokenType.GetProp:
					idString = (string) left.GetProp(NodeProperty.SpecialProperty);
					if (idString != null)
						id = Node.NewString(idString);
					/* fall through */
					goto case TokenType.GetElem;

				case TokenType.GetElem:
					if (id == null)
						id = left.FirstChild;
					return CreateSetProp(nodeType, nodeOp, left.FirstChild,
						id, right, postfix);

				default:
					// TODO: This should be a ReferenceError--but that's a runtime
					//  exception. Should we compile an exception into the code?
					ReportError("msg.bad.lhs.assign");
					return left;
			}
		}

		private Node CreateSetName(TokenType nodeOp, Node left, Node right, bool postfix)
		{
			if (nodeOp == TokenType.Nop) 
			{
				left.Type = TokenType.BindName;
				return new Node(TokenType.SetName, left, right);
			}

			string s = left.ToString();

			if (s == "__proto__" || s == "__parent__") 
			{
				Node result = new Node(TokenType.SetProp, left, right);
				result.PutProp(NodeProperty.SpecialProperty, s);
				return result;
			}

			Node opLeft = Node.NewString(TokenType.Name, s);
			if (postfix)
				opLeft = CreateNewTemp(opLeft);
			Node op = new Node(nodeOp, opLeft, right);

			Node lvalueLeft = Node.NewString(TokenType.BindName, s);
			Node resultNode = new Node(TokenType.SetName, lvalueLeft, op);
			if (postfix) 
			{
				resultNode = new Node(TokenType.Comma, resultNode,
					CreateUseTemp(opLeft));
			}
			return resultNode;
		}

		internal Node CreateNewTemp(Node n) 
		{
			TokenType type = n.Type;

			if (type == TokenType.String || type == TokenType.Number) 
			{
				// Optimization: clone these values rather than storing
				// and loading from a temp
				return n;
			}
			Node result = new Node(TokenType.NewTemp, n);
			return result;
		}

		internal Node CreateUseTemp(Node newTemp) 
		{
			TokenType type = newTemp.Type;

			if (type == TokenType.NewTemp) 
			{
				Node result = new Node(TokenType.UseTemp);
				result.PutProp(NodeProperty.Temp, newTemp);
				int n = newTemp.GetIntProp(NodeProperty.Uses, 0);
				if (n != int.MaxValue) 
				{
					newTemp.PutIntProp(NodeProperty.Uses, n + 1);
				}
				return result;
			}
			return newTemp.CloneNode();
		}

		internal Node CreateNewLocal(Node n) 
		{
			Node result = new Node(TokenType.NewLocal, n);
			return result;
		}

		internal Node CreateUseLocal(Node newLocal) 
		{
			TokenType type = newLocal.Type;
			if (type == TokenType.NewLocal) 
			{
				Node result = new Node(TokenType.UseLocal);
				result.PutProp(NodeProperty.Local, newLocal);
				return result;
			}
			return newLocal.CloneNode();    // what's this path for ?
		}

		internal static bool HasSideEffects(Node exprTree) 
		{
			switch (exprTree.Type) 
			{
				case TokenType.Inc:
				case TokenType.Dec:
				case TokenType.SetProp:
				case TokenType.SetElem:
				case TokenType.SetName:
				case TokenType.Call:
				case TokenType.New:
					return true;
				default:
					Node child = exprTree.FirstChild;
					while (child != null) 
					{
						if (HasSideEffects(child))
							return true;
						else
							child = child.Next;
					}
					break;
			}
			return false;
		}

		private Node CreateSetProp(TokenType nodeType, TokenType nodeOp, Node obj, Node id, Node expr, bool postfix)
		{
			TokenType type = nodeType == TokenType.GetProp
				? TokenType.SetProp
				: TokenType.SetElem;

			if (type == TokenType.SetProp) 
			{
				string s = id.ToString();
				if (s != null && s == "__proto__" || s == "__parent__")
				{
					Node result = new Node(type, obj, expr);
					result.PutProp(NodeProperty.SpecialProperty, s);
					return result;
				}
			}

			if (nodeOp == TokenType.Nop)
				return new Node(type, obj, id, expr);

			/* If the RHS expression could modify the LHS we have
			 * to construct a temporary to hold the LHS context
			 * prior to running the expression. Ditto, if the id
			 * expression has side-effects.
			 *
			 * XXX If the hasSideEffects tests take too long, we
			 * could make this an optimizer-only transform
			 * and always do the temp assignment otherwise.
			 */
			Node tmp1, tmp2, opLeft;
			if (obj.Type != TokenType.Name || id.HasChildren ||
				HasSideEffects(expr) || HasSideEffects(id))
			{
				tmp1 = CreateNewTemp(obj);
				Node useTmp1 = CreateUseTemp(tmp1);

				tmp2 = CreateNewTemp(id);
				Node useTmp2 = CreateUseTemp(tmp2);

				opLeft = new Node(nodeType, useTmp1, useTmp2);
			} 
			else 
			{
				tmp1 = obj.CloneNode();
				tmp2 = id.CloneNode();
				opLeft = new Node(nodeType, obj, id);
			}

			if (postfix)
				opLeft = CreateNewTemp(opLeft);
			Node op = new Node(nodeOp, opLeft, expr);

			Node resultNode = new Node(type, tmp1, tmp2, op);
			if (postfix) 
				resultNode = new Node(TokenType.Comma, resultNode, CreateUseTemp(opLeft));
			return resultNode;
		}

		private void ReportError(string msgResource) 
		{
			// report error and throw exception
			throw new SyntaxException(msgResource,
				_ts.SourceName,
				_ts.LineNo,
				_ts.Offset,
				_ts.Line);
		}
	}
}

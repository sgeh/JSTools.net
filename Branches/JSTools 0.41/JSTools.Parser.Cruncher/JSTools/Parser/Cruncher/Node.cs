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
using System.Collections;

namespace JSTools.Parser.Cruncher
{
	public enum NodeProperty
	{
		Target = 1,
		Break = 2,
		Continue = 3,
		Enum = 4,
		Function = 5,
		Temp = 6,
		Local = 7,
		CodeOffset = 8,
		FixUps = 9,
		Vars = 10,
		Uses = 11,
		RegExp = 12,
		Cases = 13,
		Default = 14,
		CaseArray = 15,
		SourceName = 16,
		Source = 17,
		Type = 18,
		SpecialProperty = 19,
		Label = 20,
		Finally = 21,
		LocalCount = 22,

		/*
			 the following properties are defined and manipulated by the
			 optimizer -
			 TargetBlock - the block referenced by a branch node
			 Variable - the variable referenced by a BIND or NAME node
			 LastUse - that variable node is the _last reference before
			 a new def or the end of the block
			 IsNumber - this node generates code on Number children and
			 delivers a Number result (as opposed to Objects)
			 DirectCall - this call node should emit code to test the function
			 object against the known class and call diret if it
			 matches.
			 */

		TargetBlock = 23,
		Variable = 24,
		LastUse = 25,
		IsNumber = 26,
		DirectCall = 27,

		BaseLineNo = 28,
		EndLineNo = 29,
		SpecialCall = 30,
		DebugSource = 31
	}

	/// <summary>
	/// This class implements the root of the intermediate representation. 
	/// 
	/// Function nodes (and its child function nodes) are generated as follows:
	/// 
	/// Description             | Tree
	/// ------------------------------------------------------------
	/// - Top or function node     + Node : FunctionNode / Node
	/// - GetProp() Method of        + GetProp(NodeProperty.Source) : char[] 
	///   top or function node
	/// - First Property of top      + First : StringNode
	///   top or function node
	/// - GetProp() Method of         + GetProp(NodeProperty.Function) : FunctionNode
	///   first child node
	/// 
	/// The First property points into a nested hirarchy. The Next
	/// property determines the next Node of the current hirarchy.
	/// 
	/// 
	/// The node tree is generated as follows:
	/// 
	/// Description             | Tree
	/// ------------------------------------------------------------
	/// - Top or function node     + Node : FunctionNode / Node
	/// - First property of         + First : StringNode
	///   top or function node
	///	- Next property of          + Next
	///	  StringNode                + Next
	///	- ...                       + First
	///	                             + Next
	///	                             + Next
	///	                             + Last
	///	                            + Next
	///	- Last property of          + Last
	///	  top or function node
	/// </summary>
	public class Node : ICloneable
	{
		private TokenType _type = TokenType.EOF;	// _type of the node; TokenType.Name for example
		private Node _next = null;					// _next sibling
		private Node _first = null;					// _first element of a linked list of children
		private Node _last = null;					// _last element of a linked list of children
		private TokenType _datum = TokenType.EOF;	// encapsulated int data; depends on _type
		private Hashtable _props = new Hashtable();
		private int _lineNumber = -1;
	

		public TokenType Type 
		{
			get { return _type; }
			set { _type = value; }
		}

		public bool HasChildren { get { return (_first != null); } }
		public Node FirstChild { get { return _first; } }
		public Node LastChild { get { return _last; } }
		public Node Next { get { return _next; } }

		public int LineNo 
		{
			get
			{
				switch (_type) 
				{
					case TokenType.EqOp:
					case TokenType.RelOp:
					case TokenType.UnaryOp:
					case TokenType.Primary:
						throw new InvalidOperationException("Code bug");
				}
				return _lineNumber;
			}
		}

		public TokenType Operation 
		{
			get
			{
				switch (_type) 
				{
					case TokenType.EqOp:
					case TokenType.RelOp:
					case TokenType.UnaryOp:
					case TokenType.Primary:
						return _datum;
				}
				throw new InvalidOperationException("Invalid operation");
			}
		}

		private Hashtable Props
		{
			get 
			{
				if (_props == null)
					_props = new Hashtable();
				return _props;
			}
		}


		public Node(TokenType nodeType) 
		{
			_type = nodeType;
		}

		public Node(TokenType nodeType, TokenType dataType) 
		{
			_type = nodeType;
			_datum = dataType;
		}

		public Node(TokenType nodeType, int lineNumber) 
		{
			_type = nodeType;
			_lineNumber = lineNumber;
		}

		public Node(TokenType nodeType, Node child) 
		{
			_type = nodeType;
			_first = _last = child;
			child._next = null;
		}

		public Node(TokenType nodeType, Node left, Node right) 
		{
			_type = nodeType;
			_first = left;
			_last = right;
			left._next = right;
			right._next = null;
		}

		public Node(TokenType nodeType, Node left, Node mid, Node right) 
		{
			_type = nodeType;
			_first = left;
			_last = right;
			left._next = mid;
			mid._next = right;
			right._next = null;
		}

		public Node(TokenType nodeType, Node child, int lineNumber) : this(nodeType, child)
		{
			_lineNumber = lineNumber;
		}

		public Node(TokenType nodeType, Node left, Node right, TokenType data) : this(nodeType, left, right)
		{
			_datum = data;
		}

		public Node(TokenType nodeType, Node left, Node mid, Node right, int lineNumber) : this(nodeType, left, mid, right)
		{
			_lineNumber = lineNumber;
		}


		public static Node NewNumber(string number) 
		{
			return new NumberNode(number);
		}

		public static Node NewString(string str) 
		{
			return new StringNode(TokenType.String, str);
		}

		public static Node NewString(TokenType type, string str) 
		{
			return new StringNode(type, str);
		}


		public override string ToString()
		{
			return Enum.GetName(typeof(TokenType), _type);
		}

		public Node GetChildBefore(Node child) 
		{
			return GetChildBefore(child, true);
		}

		public Node GetChildBefore(Node child, bool checkNull) 
		{
			if (child == _first)
				return null;

			Node n = _first;

			while (n._next != child) 
			{
				n = n._next;

				if (n == null)
				{
					if (checkNull)
						throw new InvalidOperationException("The given node is not a child.");
					return null;
				}
			}
			return n;
		}

		public Node GetLastSibling() 
		{
			Node n = this;

			while (n._next != null) 
			{
				n = n._next;
			}
			return n;
		}

		public void AddChildToFront(Node child) 
		{
			child._next = _first;
			_first = child;

			if (_last == null) 
				_last = child;
		}

		public void AddChildToBack(Node child) 
		{
			child._next = null;

			if (_last == null) 
			{
				_first = _last = child;
				return;
			}

			_last._next = child;
			_last = child;
		}

		public void AddChildrenToFront(Node children) 
		{
			Node lastSib = children.GetLastSibling();
			lastSib._next = _first;
			_first = children;
			if (_last == null) 
			{
				_last = lastSib;
			}
		}

		public void AddChildrenToBack(Node children) 
		{
			if (_last != null) 
			{
				_last._next = children;
			}
			_last = children.GetLastSibling();
			if (_first == null) 
			{
				_first = children;
			}
		}

		/**
		 * Add 'child' before 'node'.
		 */
		public void AddChildBefore(Node newChild, Node node) 
		{
			if (newChild._next != null)
				throw new InvalidOperationException("The given new child has siblings in AddChildBefore");

			if (_first == node) 
			{
				newChild._next = _first;
				_first = newChild;
				return;
			}
			Node prev = GetChildBefore(node);
			AddChildAfter(newChild, prev);
		}

		/**
		 * Add 'child' after 'node'.
		 */
		public void AddChildAfter(Node newChild, Node node) 
		{
			if (newChild._next != null)
				throw new InvalidOperationException("The given new child has siblings in AddChildAfter");

			newChild._next = node._next;
			node._next = newChild;
			if (_last == node)
				_last = newChild;
		}

		public void RemoveChild(Node child) 
		{
			Node prev = GetChildBefore(child, false);
			if (prev == null)
				_first = _first._next;
			else
				prev._next = child._next;
			if (child == _last) _last = prev;
			child._next = null;
		}

		public void ReplaceChild(Node child, Node newChild) 
		{
			newChild._next = child._next;
			if (child == _first) 
			{
				_first = newChild;
			} 
			else 
			{
				Node prev = GetChildBefore(child);
				prev._next = newChild;
			}
			if (child == _last)
				_last = newChild;
			child._next = null;
		}

		public object GetProp(NodeProperty propType) 
		{
			return Props[propType];
		}

		public int GetIntProp(NodeProperty propType, int defaultValue) 
		{
			if (Props[propType] != null)
				return (int)Props[propType];
			return defaultValue;
		}

		public void PutProp(NodeProperty propType, object prop) 
		{
			Props[propType] = prop;
		}

		public void PutIntProp(NodeProperty propType, int prop) 
		{
			Props[propType] = prop;
		}

		public void RemoveProp(NodeProperty propType) 
		{
			Props.Remove(propType);
		}

		public Node CloneNode() 
		{
			return (Node)Clone();
		}

		public object Clone()
		{
			Node clonedNode = CreateCloneInstance();
			clonedNode._datum = _datum;
			clonedNode._props = ((Hashtable)Props.Clone());
			clonedNode._lineNumber = _lineNumber;
			return clonedNode;
		}

		protected virtual Node CreateCloneInstance()
		{
			return new Node(_type);
		}


		private class NumberNode : Node 
		{
			private string _number;

			public NumberNode(string number) : base(TokenType.Number)
			{
				_number = number;
			}

			public override string ToString() 
			{
				return _number;
			}

			protected override Node CreateCloneInstance()
			{
				return new NumberNode(_number);
			}
		}


		private class StringNode : Node 
		{
			private string _str;

			public StringNode(TokenType type, string str) : base(type)
			{
				_str = str;
			}

			public override string ToString() 
			{
				return _str;
			}

			protected override Node CreateCloneInstance()
			{
				return new StringNode(Type, _str);
			}
		}
	}
}


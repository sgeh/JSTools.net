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
using System.ComponentModel;
using System.Xml.Serialization;

namespace JSTools.Parser.Cruncher.Nodes
{
	/// <summary>
	/// This class represents the intermediate code representation. 
	/// 
	/// Function nodes (and its child function nodes) are generated as follows:
	/// 
	/// Description             | Tree
	/// ------------------------------------------------------------
	/// - Top or function node     + Node : FunctionNode / Node
	/// - GetProp() Method of        + Props.Source : byte[] 
	///   top or function node
	/// - First Property of top      + First : StringNode
	///   top or function node
	/// - GetProp() Method of         + Props.GetProp(NodeProperty.Function) : FunctionNode
	///   first child node
	/// 
	/// The First property points to a nested hirarchy. The Next
	/// property determines the next Node of the current hirarchy.
	/// 
	/// 
	/// The node tree is generated as follows:
	///  <c>
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
	///	 </c>
	/// </summary>
	/// <remarks>
	/// Deserialization of a node tree is not fully implemented yet.
	/// </remarks>
	[XmlRoot("NodeTree", Namespace=Node.NAMESPACE)]
	[XmlType(Namespace=Node.NAMESPACE)]
	[XmlInclude(typeof(NumberNode))]
	[XmlInclude(typeof(StringNode))]
	[XmlInclude(typeof(FunctionNode))]
	public class Node : ICloneable
	{
		public const string NAMESPACE = "http://www.jstools.net";

		private TokenType _type = TokenType.EOF;	// _type of the node; TokenType.Name for example
		private Node _next = null;					// _next sibling
		private Node _first = null;					// _first element of a linked list of children
		private Node _last = null;					// _last element of a linked list of children
		private TokenType _datum = TokenType.EOF;	// encapsulated int data; depends on _type
		private PropertyContainer _props = null;
		private int _lineNumber = -1;
		private string _data = null;

		[XmlIgnore()]
		public char[] Source
		{
			get { return (Props.GetProp(NodeProperty.Source) as char[]); }
			set { Props.PutProp(NodeProperty.Source, value); }
		}

		[XmlElement("NestedFunction")]
		public FunctionNode Function
		{
			get { return (Props.GetProp(NodeProperty.Function) as FunctionNode); }
			set { Props.PutProp(NodeProperty.Function, value); }
		}

		/// <summary>
		/// Gets/sets the source property -> required for serialization/deserialization.
		/// </summary>
		[XmlElement("Source")]
		public byte[] SourceBytes
		{
			get
			{
				if (Source == null)
					return null;

				char[] source = Source;
				byte[] destination = new byte[source.Length];

				for (int i = 0; i < source.Length; ++i)
				{
					destination[i] = (byte)source[i];
				}
				return destination;
			}
			set
			{
				if (value == null)
					value = new byte[0];

				char[] destination = new char[value.Length];

				for (int i = 0; i < value.Length; ++i)
				{
					destination[i] = (char)value[i];
				}
				Source = destination;
			}		
		}

		[XmlAttribute("type")]
		public virtual TokenType Type 
		{
			get { return _type; }
			set { _type = value; }
		}

		[XmlAttribute("datum")]
		[DefaultValue(TokenType.EOF)]
		public virtual TokenType Datum
		{
			get { return _datum; }
			set { _datum = value; }		
		}

		[XmlAttribute("data")]
		public virtual string Data
		{
			get { return _data; }
			set { _data = value; }
		}

		[XmlAttribute("lineNo")]
		[DefaultValue(-1)]
		public int LineNo 
		{
			get { return _lineNumber; }
			set { _lineNumber = value; }
		}

		[XmlIgnore()]
		public PropertyContainer Props
		{
			get 
			{
				if (_props == null)
					_props = new PropertyContainer();
				return _props;
			}
		}

		[XmlArray("ChildNodes")]
		public Node[] Children
		{
			get
			{
				if (_first == null)
					return null;

				ArrayList children = new ArrayList();
				Node n = _first;

				while (n != null) 
				{
					children.Add(n);
					n = n._next;
				}
				return (Node[])children.ToArray(typeof(Node));
			}
			set
			{
				if (value != null)
				{
					Node lastNode = null;

					foreach (Node node in value)
					{
						if (lastNode == null)
							_first = node;
						else
							lastNode._next = node;

						lastNode = node;
					}
					_last = lastNode;
				}
				else
				{
					_first = null;
					_last = null;
				}
			}
		}

		public bool HasChildren { get { return (_first != null); } }
		public Node Next { get { return _next; } }
		public Node FirstChild { get { return _first; } }
		public Node LastChild { get { return _last; } }

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


		public Node() 
		{
			// constructor used by xml-deserializer
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

		public override string ToString()
		{
			return Enum.GetName(typeof(TokenType), _type);
		}

		public Node GetChild(string childNodeTypePath)
		{
			if (childNodeTypePath == null)
				throw new ArgumentNullException("childNodeTypePath");

			Node currentNode = this;

			foreach (string nodeType in childNodeTypePath.Split('/'))
			{
				currentNode = currentNode.GetChild((TokenType)Enum.Parse(typeof(TokenType), nodeType, true));

				if (currentNode == null)
					break;
			}
			return currentNode;
		}

		public Node[] GetChildren(TokenType type)
		{
			ArrayList children = new ArrayList();
			Node n = _first;

			while (n != null) 
			{
				if (n.Type == type)
					children.Add(n);

				n = n._next;
			}
			return (Node[])children.ToArray(typeof(Node));
		}

		public Node GetChild(TokenType type)
		{
			Node n = _first;

			while (n != null) 
			{
				if (n.Type == type)
					return n;

				n = n._next;
			}
			return null;
		}

		public bool HasChild(TokenType type)
		{
			Node n = _first;

			while (n != null) 
			{
				if (n.Type == type)
					return true;

				n = n._next;
			}
			return false;
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

		public Node GetChildAfter(Node child) 
		{
			if (child != null)
				return child.Next;

			return null;
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

		public Node CloneNode() 
		{
			return (Node)Clone();
		}

		public object Clone()
		{
			Node clonedNode = CreateCloneInstance();
			clonedNode._datum = _datum;
			clonedNode._lineNumber = _lineNumber;

			if (_props != null)
				clonedNode._props = _props.Clone();
			
			return clonedNode;
		}

		protected virtual Node CreateCloneInstance()
		{
			return new Node(_type);
		}
	}
}

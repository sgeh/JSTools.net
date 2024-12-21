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

namespace JSTools.Parser
{
	/// <summary>
	/// Summary description for GlobalValue.
	/// </summary>
	public class GlobalNode : INode
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private ArrayList	_children			= new ArrayList();
		private int			_lineNumberBegin	= -1;
		private int			_lineNumberEnd		= -1;
		private int			_offsetBegin		= -1;
		private int			_offsetEnd			= -1;
		private int			_lineOffsetBegin	= -1;
		private int			_lineOffsetEnd		= -1;
		private	int			_codeLength			= 0;

		private	string		_globalCode			= null;
		private	string		_parseItemName		= String.Empty;


		/// <summary>
		/// Fired if a node begins.
		/// </summary>
		public event EventHandler OnBegin;


		/// <summary>
		/// Fired if a node ends.
		/// </summary>
		public event EventHandler OnEnd;


		/// <summary>
		/// Returns the children of the current node instance.
		/// </summary>
		public INode[] Children
		{
			get { return (INode[])_children.ToArray(typeof(INode)); }
		}


		/// <summary>
		/// Returns the line number on which this item begins.
		/// </summary>
		public int LineNumberBegin
		{
			get { return _lineNumberBegin; }
			set { _lineNumberBegin = value; }
		}


		/// <summary>
		/// Returns the line number on which this item ends.
		/// </summary>
		public int LineNumberEnd
		{
			get { return _lineNumberEnd; }
			set { _lineNumberEnd = value; }
		}


		/// <summary>
		/// Gets/sets the absolute start offset.
		/// </summary>
		public int OffsetEnd
		{
			get { return _offsetEnd; }
			set { _offsetEnd = value; }
		}


		/// <summary>
		/// Gets/sets the absolute end offset.
		/// </summary>
		public int OffsetBegin
		{
			get { return _offsetBegin; }
			set { _offsetBegin = value; }
		}


		/// <summary>
		/// Gets/sets the start offset of the start line.
		/// </summary>
		public int LineOffsetBegin
		{
			get { return _lineOffsetBegin; }
			set { _lineOffsetBegin = value; }
		}


		/// <summary>
		/// Gets/sets the end offset of the start end.
		/// </summary>
		public int LineOffsetEnd
		{
			get { return _lineOffsetEnd; }
			set { _lineOffsetEnd = value; }
		}


		/// <summary>
		/// Gets/sets the whole code.
		/// </summary>
		public string GlobalCode
		{
			get { return _globalCode; }
			set { _globalCode = value; }
		}


		/// <summary>
		/// Returns the name of the representing parse item.
		/// </summary>
		public string ParseItemName
		{
			get { return _parseItemName; }
		}


		/// <summary>
		/// Gets the code, which represents this object.
		/// </summary>
		public string ParsedCode
		{
			get
			{
				return GlobalCode.Substring(OffsetBegin, OffsetEnd - OffsetBegin);
			}
		}

		/// <summary>
		/// Gets/sets the child at the given index.
		/// </summary>
		/// <exception cref="IndexOutOfRangeException">The given index is out of bounds.</exception>
		/// <exception cref="ArgumentNullException">The given value contians a null reference.</exception>
		public INode this[int index]
		{
			get
			{
				if (index < 0 || index >= _children.Count)
					throw new IndexOutOfRangeException("The given index is out of bounds.");

				return (_children[index] as INode);
			}
			set
			{
				if (index < 0 || index >= _children.Count)
					throw new IndexOutOfRangeException("The given index is out of bounds.");

				if (value == null)
					throw new ArgumentNullException("value", "The given value contians a null reference!");

				_children[index] = value;
			}
		}


		/// <summary>
		/// Gets/sets the length of the code.
		/// </summary>
		public int CodeLength
		{
			get { return _codeLength; }
			set { _codeLength = value; }
		}


		/// <summary>
		/// Gets/sets the first child. Returns null, if no child exists.
		/// </summary>
		public INode FirstChild
		{
			get
			{
				return (_children.Count > 0) ? (INode)_children[0] : null;
			}
		}


		/// <summary>
		/// Gets/sets the last child. Returns null, if no child exists.
		/// </summary>
		public INode LastChild
		{
			get
			{
				return (_children.Count > 0) ? (INode)_children[_children.Count - 1] : null;
			}		
		}


		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new GlobalNode object.
		/// </summary>
		public GlobalNode()
		{
		}


		/// <summary>
		/// Creates a new GlobalNode object.
		/// </summary>
		/// <param name="parseItemName">Name of the representing parse item.</param>
		public GlobalNode(string parseItemName)
		{
			if (parseItemName != null)
			{
				_parseItemName = parseItemName;
			}
		}


		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Initilizes this node instance. Informations will be given by this method.
		/// </summary>
		/// <param name="globalCode">Whole code to parse.</param>
		/// <param name="offsetBegin">Absolute start offset.</param>
		/// <param name="lineOffsetBegin">Offset of the start line.</param>
		/// <param name="lineNumberBegin">Line number, at which this instance begins.</param>
		public void SetUpBegin(string globalCode, int offsetBegin, int lineOffsetBegin, int lineNumberBegin)
		{
			_lineNumberBegin	= lineNumberBegin;
			_offsetBegin		= offsetBegin;
			_lineOffsetBegin	= lineOffsetBegin;
			_globalCode			= globalCode;

			if (OnBegin != null)
			{
				OnBegin(this, new EventArgs());
			}
		}

		/// <summary>
		/// Initilizes the end of this instance.
		/// </summary>
		/// <param name="lineOffsetEnd">Offset of the end line.</param>
		/// <param name="lineNumberEnd">Line number, at which this instance ends.</param>
		public void SetUpEnd(int lineOffsetEnd, int lineNumberEnd)
		{
			_lineNumberEnd		= lineNumberEnd;
			_lineOffsetEnd		= lineOffsetEnd;
			_offsetEnd			= _offsetBegin + CodeLength;

			if (OnEnd != null)
			{
				OnEnd(this, new EventArgs());
			}
		}


		/// <summary>
		/// Adds a new child to the current node.
		/// </summary>
		/// <param name="child">Child node to add.</param>
		/// <exception cref="ArgumentNullException">The specified child contains a null reference.</exception>
		public void AddChild(INode child)
		{
			if (child == null)
				throw new ArgumentNullException("child", "The specified child contains a null reference!");

			_children.Add(child);
		}


		/// <summary>
		/// Returns the index of the specified child. 
		/// </summary>
		/// <param name="child">Child, which index should be determined.</param>
		/// <returns>Returns -1, if the given node was not found.</returns>
		public int GetChildNodeIndex(INode child)
		{
			if (child == null)
				return -1;

			int i = _children.Count - 1;

			for ( ; i >= 0 && _children[i] != child; --i)
			{
				;
			}
			return i;
		}


		/// <summary>
		/// Returns the child node, which is stored at the specified index.
		/// </summary>
		/// <param name="index">Index to determine the node instance.</param>
		/// <returns>Returns a null reference, if no node was found.</returns>
		public INode GetChildNodeByIndex(int index)
		{
			if (index > _children.Count || index < 0)
				return null;

			return (INode)_children[index];
		}


		/// <summary>
		/// Inserts the given child at the specified index.
		/// </summary>
		/// <param name="index">Index to insert the child.</param>
		/// <param name="child">Child node to insert.</param>
		/// <exception cref="IndexOutOfRangeException">The given index is out of bounds.</exception>
		/// <exception cref="ArgumentNullException">The specified child contains a null reference.</exception>
		public void InsertChild(int index, INode child)
		{
			if (index > _children.Count || index < 0)
				throw new IndexOutOfRangeException("The given index is out of bounds!");

			if (child == null)
				throw new ArgumentNullException("child", "The specified child contains a null reference!");

			_children.Insert(index, child);
		}


		/// <summary>
		/// Removes the specified child at the specified index.
		/// </summary>
		/// <param name="index">Index to remove a child.</param>
		/// <returns>Returns the removed node instance. You will obtain a null pointer if the specified
		/// index could not be found.</returns>
		public INode RemoveChild(int index)
		{
			INode childNode = GetChildNodeByIndex(index);

			if (childNode != null)
			{
				_children.Remove(childNode);
				return childNode;
			}
			return null;
		}


		/// <summary>
		/// Replaces the specified child with the given node to insert.
		/// </summary>
		/// <param name="nodeToReplace">Node to remove.</param>
		/// <param name="nodeToInsert">Node which should be inserted.</param>
		/// <exception cref="ArgumentNullException">Could not insert a null pointer into the child collection.</exception>
		/// <exception cref="InvalidOperationException">The given node to replace is not declared as child of this node.</exception>
		public void ReplaceChild(INode nodeToReplace, INode nodeToInsert)
		{
			if (nodeToInsert == null)
				throw new ArgumentNullException("nodeToInsert", "Could not insert a null pointer into the child collection!");

			int index = GetChildNodeIndex(nodeToReplace);

			if (index == -1)
				throw new InvalidOperationException("The given node to replace is not declared as child of this node!");

			_children.Remove(nodeToReplace);
			_children.Insert(index, nodeToInsert);
		}
	}
}

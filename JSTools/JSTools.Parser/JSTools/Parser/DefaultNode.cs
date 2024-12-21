/*
 * JSTools.Parser.dll / JSTools.net - A framework for JavaScript/ASP.NET applications.
 * Copyright (C) 2005  Silvan Gehrig
 *
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
 *
 * Author:
 *  Silvan Gehrig
 */

using System;
using System.Collections;

namespace JSTools.Parser
{
	/// <summary>
	/// Represents an implementation of the INode instance. It can be used
	/// as default node or you may derive from it.
	/// </summary>
	public class DefaultNode : INode
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private INode _parentNode = null;
		private ArrayList _children = new ArrayList();

		private int _lineNumberBegin = -1;
		private int _lineNumberEnd = -1;
		private int _offsetBegin = -1;
		private int _offsetEnd = -1;
		private int _lineOffsetBegin = -1;
		private int _lineOffsetEnd = -1;
		private int _codeLength = 0;

		private string _globalCode = null;
		private IParseItem _parseItem = null;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Returns the assigned parse item instance.
		/// </summary>
		public IParseItem ParseItem
		{
			get { return _parseItem; }
		}

		/// <summary>
		/// Returns the parent node instance.
		/// </summary>
		public virtual INode ParentNode
		{
			get { return _parentNode; }
		}

		/// <summary>
		/// Returns the children of the current node instance.
		/// </summary>
		public virtual INode[] Children
		{
			get { return (INode[])_children.ToArray(typeof(INode)); }
		}

		/// <summary>
		/// Returns the line number on which this item begins.
		/// </summary>
		public int LineNumberBegin
		{
			get { return _lineNumberBegin; }
		}

		/// <summary>
		/// Returns the line number on which this item ends.
		/// </summary>
		/// <exception cref="ArgumentException">The given end line number be greater than the line number begin.</exception>
		public int LineNumberEnd
		{
			get { return _lineNumberEnd; }
			set
			{
				if (value < _lineNumberBegin)
					throw new ArgumentException("The end line number must be greater or equal than the line number begin.");

				_lineNumberEnd = value;
			}
		}

		/// <summary>
		/// Gets/sets the absolute end offset.
		/// </summary>
		/// <exception cref="ArgumentException">The given end index must be greater than the begin offset.</exception>
		public int OffsetEnd
		{
			get { return _offsetEnd; }
			set 
			{
				if (value < _offsetBegin || value >= GlobalCode.Length)
					throw new ArgumentException("The end index must be greater or equal than the begin offset.");

				_offsetEnd = value;
				_codeLength = _offsetEnd - _offsetBegin + 1;
			}
		}

		/// <summary>
		/// Gets/sets the absolute start offset.
		/// </summary>
		public int OffsetBegin
		{
			get { return _offsetBegin; }
		}

		/// <summary>
		/// Gets/sets the start offset of the start line.
		/// </summary>
		public int LineOffsetBegin
		{
			get { return _lineOffsetBegin; }
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
		}

		/// <summary>
		/// Gets the code, which represents this object.
		/// </summary>
		public virtual string ParsedCode
		{
			get
			{
				if (CodeLength != 0)
					return GlobalCode.Substring(OffsetBegin, CodeLength);
				else
					return string.Empty;
			}
		}

		/// <summary>
		/// Gets/sets the child at the given index.
		/// </summary>
		/// <exception cref="IndexOutOfRangeException">The given index is out of bounds.</exception>
		/// <exception cref="ArgumentNullException">The given value contians a null reference.</exception>
		public virtual INode this[int index]
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
					throw new ArgumentNullException("value", "The given value contians a null reference.");

				_children[index] = value;
			}
		}

		/// <summary>
		/// Gets/sets the length of the code.
		/// </summary>
		/// <exception cref="ArgumentException">The given end index must be greater than the begin offset.</exception>
		public virtual int CodeLength
		{
			get { return _codeLength; }
			set { OffsetEnd = _offsetBegin + value - ((value != 0) ? 1 : 0); }
		}

		/// <summary>
		/// Gets/sets the first child. Returns null, if no child exists.
		/// </summary>
		public virtual INode FirstChild
		{
			get { return (_children.Count > 0) ? (INode)_children[0] : null; }
		}

		/// <summary>
		/// Gets/sets the last child. Returns null, if no child exists.
		/// </summary>
		public virtual INode LastChild
		{
			get { return (_children.Count > 0) ? (INode)_children[_children.Count - 1] : null; }		
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new DefaultNode object.
		/// </summary>
		/// <param name="parentNode">Specifies the parent node instance.</param>
		/// <param name="globalCode">Whole code to parse.</param>
		/// <param name="absOffsetBegin">Absolute begin offset.</param>
		/// <param name="lineOffsetBegin">Offset of the start line.</param>
		/// <param name="lineNumberBegin">Line number, at which this instance begins.</param>
		/// <exception cref="ArgumentException">The global code must not contain a null reference.</exception>
		/// <exception cref="ArgumentException">The absolute offset must be greater or equal than 0.</exception>
		/// <exception cref="ArgumentException">The line offset must be greater or equal than 0.</exception>
		/// <exception cref="ArgumentException">The line number must be greater or equal than 1.</exception>
		internal protected DefaultNode(INode parentNode, string globalCode, int absOffsetBegin, int lineOffsetBegin, int lineNumberBegin)
		{
			if (globalCode == null)
				throw new ArgumentException("globalCode");

			if (absOffsetBegin < 0)
				throw new ArgumentException("The absolute offset must be greater or equal than 0.");

			if (lineOffsetBegin < 0)
				throw new ArgumentException("The line offset must be greater or equal than 0.");

			if (lineNumberBegin < 1)
				throw new ArgumentException("The line number must be greater or equal than 1.");

			_parentNode = parentNode;
			_lineNumberBegin = lineNumberBegin;
			_offsetBegin = absOffsetBegin;
			_lineOffsetBegin = lineOffsetBegin;
			_globalCode = globalCode;
		}

		/// <summary>
		/// Creates a new DefaultNode object.
		/// </summary>
		/// <param name="parentNode">Specifies the parent node instance.</param>
		/// <param name="parseItem">Corresponding parse item instance.</param>
		/// <param name="globalCode">Whole code to parse.</param>
		/// <param name="absOffsetBegin">Absolute begin offset.</param>
		/// <param name="lineOffsetBegin">Offset of the start line.</param>
		/// <param name="lineNumberBegin">Line number, at which this instance begins.</param>
		/// <exception cref="ArgumentException">Invalid parse item specified.</exception>
		public DefaultNode(
			IParseItem parseItem,
			INode parentNode,
			string globalCode,
			int absOffsetBegin,
			int lineOffsetBegin,
			int lineNumberBegin) : this(parentNode, globalCode, absOffsetBegin, lineOffsetBegin, lineNumberBegin)
		{
			if (parseItem == null)
				throw new ArgumentException("Invalid parse item specified.", "parseItem");

			_parseItem = parseItem;
		}

		/// <summary>
		/// Creates a new DefaultNode object.
		/// </summary>
		/// <param name="parentNode">Specifies the parent node instance.</param>
		/// <param name="parseItem">Corresponding parse item instance.</param>
		/// <param name="globalCode">Whole code to parse.</param>
		/// <param name="absOffsetBegin">Absolute begin offset.</param>
		/// <param name="lineOffsetBegin">Offset of the start line.</param>
		/// <param name="lineNumberBegin">Line number, at which this instance begins.</param>
		/// <param name="codeLength">Absolute length of the node.</param>
		/// <exception cref="ArgumentException">Invalid parse item specified.</exception>
		public DefaultNode(
			IParseItem parseItem,
			INode parentNode,
			string globalCode,
			int absOffsetBegin,
			int lineOffsetBegin,
			int lineNumberBegin,
			int codeLength) : this(parseItem, parentNode, globalCode, absOffsetBegin, lineOffsetBegin, lineNumberBegin)
		{
			if (codeLength > 0)
				_codeLength =  codeLength;
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Adds a new child to the current node.
		/// </summary>
		/// <param name="child">Child node to add.</param>
		/// <exception cref="ArgumentNullException">The specified child contains a null reference.</exception>
		public virtual void AddChild(INode child)
		{
			if (child == null)
				throw new ArgumentNullException("child", "The specified child contains a null reference.");

			_children.Add(child);
		}

		/// <summary>
		/// Returns the index of the specified child. 
		/// </summary>
		/// <param name="child">Child, which index should be determined.</param>
		/// <returns>Returns -1, if the given node was not found.</returns>
		public virtual int GetChildNodeIndex(INode child)
		{
			if (child == null)
				return -1;

			int i = _children.Count - 1;

			for ( ; i > -1 && _children[i] != child; --i)
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
		public virtual INode GetChildNodeByIndex(int index)
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
		public virtual void InsertChild(int index, INode child)
		{
			if (index > _children.Count || index < 0)
				throw new IndexOutOfRangeException("The given index is out of bounds.");

			if (child == null)
				throw new ArgumentNullException("child", "The specified child contains a null reference.");

			_children.Insert(index, child);
		}

		/// <summary>
		/// Removes the specified child at the specified index.
		/// </summary>
		/// <param name="index">Index to remove a child.</param>
		/// <returns>Returns the removed node instance. You will obtain a null pointer if the specified
		/// index could not be found.</returns>
		public virtual INode RemoveChild(int index)
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
		public virtual void ReplaceChild(INode nodeToReplace, INode nodeToInsert)
		{
			if (nodeToInsert == null)
				throw new ArgumentNullException("nodeToInsert", "Could not insert a null pointer into the child collection.");

			int index = GetChildNodeIndex(nodeToReplace);

			if (index == -1)
				throw new InvalidOperationException("The given node to replace is not declared as child of this node.");

			_children.Remove(nodeToReplace);
			_children.Insert(index, nodeToInsert);
		}
	}
}

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

namespace JSTools.Parser
{
	/// <summary>
	/// Represents a node item.
	/// </summary>
	public interface INode
	{
		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Fired if a node begins.
		/// </summary>
		event EventHandler OnBegin;

		/// <summary>
		/// Fired if a node ends.
		/// </summary>
		event EventHandler OnEnd;

		/// <summary>
		/// Returns the children of the current node instance.
		/// </summary>
		INode[] Children
		{
			get;
		}

		/// <summary>
		/// Returns the line number on which this item begins.
		/// </summary>
		int LineNumberBegin
		{
			get;
		}

		/// <summary>
		/// Returns the line number on which this item ends.
		/// </summary>
		int LineNumberEnd
		{
			get;
		}

		/// <summary>
		/// Gets/sets the absolute start offset.
		/// </summary>
		int OffsetEnd
		{
			get;
		}

		/// <summary>
		/// Gets/sets the absolute end offset.
		/// </summary>
		int OffsetBegin
		{
			get;
		}

		/// <summary>
		/// Gets/sets the start offset of the start line.
		/// </summary>
		int LineOffsetBegin
		{
			get;
		}

		/// <summary>
		/// Gets/sets the end offset of the start end.
		/// </summary>
		int LineOffsetEnd
		{
			get;
		}

		/// <summary>
		/// Returns the name of the representing parse item.
		/// </summary>
		string ParseItemName
		{
			get;
		}

		/// <summary>
		/// Gets/sets the whole code.
		/// </summary>
		string GlobalCode
		{
			get;
		}

		/// <summary>
		/// Gets/sets the code, which represents this object.
		/// </summary>
		string ParsedCode
		{
			get;
		}

		/// <summary>
		/// Gets/sets the current length of the code. This value will be incremented after
		/// calling the End method.
		/// </summary>
		int CodeLength
		{
			get;
			set;
		}

		/// <summary>
		/// Gets/sets the child at the given index.
		/// </summary>
		/// <exception cref="IndexOutOfRangeException">The given index is out of bounds.</exception>
		/// <exception cref="ArgumentNullException">The given value contians a null reference.</exception>
		INode this[int index]
		{
			get;
			set;
		}

		/// <summary>
		/// Gets/sets the first child. Returns null, if no child exists.
		/// </summary>
		INode FirstChild
		{
			get;
		}

		/// <summary>
		/// Gets/sets the last child. Returns null, if no child exists.
		/// </summary>
		INode LastChild
		{
			get;
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Initilizes this node instance. Informations about the line number and line offset
		/// are specified with the arguments of this method.
		/// </summary>
		/// <param name="globalCode">Whole code to parse.</param>
		/// <param name="offsetBegin">Absolute start offset.</param>
		/// <param name="lineOffsetBegin">Offset of the start line.</param>
		/// <param name="lineNumberBegin">Line number, at which this instance begins.</param>
		void SetUpBegin(string globalCode, int offsetBegin, int lineOffsetBegin, int lineNumberBegin);

		/// <summary>
		/// Initilizes the end of this instance.
		/// </summary>
		/// <param name="lineOffsetEnd">Offset of the end line.</param>
		/// <param name="lineNumberEnd">Line number, at which this instance ends.</param>
		void SetUpEnd(int lineOffsetEnd, int lineNumberEnd);

		/// <summary>
		/// Adds a new child to the current node. It will be append on the end of the list.
		/// </summary>
		/// <param name="child">Child node to add.</param>
		/// <exception cref="ArgumentNullException">The specified child contains a null reference.</exception>
		void AddChild(INode child);

		/// <summary>
		/// Returns the index of the specified child.
		/// </summary>
		/// <param name="child">Child, which index should be determined.</param>
		/// <returns>Returns -1, if the given node was not found.</returns>
		int GetChildNodeIndex(INode child);

		/// <summary>
		/// Returns the child node, which is stored at the specified index.
		/// </summary>
		/// <param name="index">Index to determine the node instance.</param>
		/// <returns>Returns a null reference, if no node was found.</returns>
		INode GetChildNodeByIndex(int index);

		/// <summary>
		/// Inserts the given child at the specified index.
		/// </summary>
		/// <param name="index">Index to insert the child.</param>
		/// <param name="child">Child node to insert.</param>
		/// <exception cref="IndexOutOfRangeException">The given index is out of bounds.</exception>
		/// <exception cref="ArgumentNullException">The specified child contains a null reference.</exception>
		void InsertChild(int index, INode child);

		/// <summary>
		/// Removes the specified child at the specified index.
		/// </summary>
		/// <param name="index">Index to remove a child.</param>
		/// <returns>Returns the removed node instance. You will obtain a null pointer if the specified
		/// index could not be found.</returns>
		INode RemoveChild(int index);

		/// <summary>
		/// Replaces the specified child with the given node to insert.
		/// </summary>
		/// <param name="nodeToReplace">Node to remove.</param>
		/// <param name="nodeToInsert">Node which should be inserted.</param>
		/// <exception cref="ArgumentNullException">Could not insert a null pointer into the child collection.</exception>
		/// <exception cref="InvalidOperationException">The given node to replace is not declared as child of this node.</exception>
		void ReplaceChild(INode nodeToReplace, INode nodeToInsert);
	}
}

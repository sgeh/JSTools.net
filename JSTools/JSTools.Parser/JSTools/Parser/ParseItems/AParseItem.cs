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

namespace JSTools.Parser.ParseItems
{
	/// <summary>
	/// Represents a default implementation of the IParseItem interface.
	/// </summary>
	public abstract class AParseItem : IParseItem
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private ParserContext _context = null;
		private string _parseItemName = null;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		#region IParserContextItem Member

		ParserContext IParserContextItem.Context
		{
			get { return _context; }
			set { _context = value; }
		}

		string IParserContextItem.Name
		{
			get { return _parseItemName; }
		}

		#endregion

		/// <summary>
		///  <see cref="IParseItem.IsAbsoluteEnd" />
		/// </summary>
		public abstract bool IsAbsoluteEnd
		{
			get;
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new AParseItem instance.
		/// </summary>
		/// <param name="parseItemName">Name of the current parse item which is assigned to the created nodes.</param>
		/// <exception cref="ArgumentException">The specified parse item name is empty.</exception>
		public AParseItem(string parseItemName)
		{
			if (parseItemName == null || parseItemName.Length == 0)
				throw new ArgumentException("The specified parse item name is empty.", "parseItemName");

			_parseItemName = parseItemName;
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		///  <see cref="IParseItem.Begin" />
		/// </summary>
		/// <param name="scope"><see cref="IParseItem.Begin" /></param>
		/// <param name="parentNode"><see cref="IParseItem.Begin" /></param>
		/// <param name="parseString"><see cref="IParseItem.Begin" /></param>
		/// <param name="absOffsetBegin"><see cref="IParseItem.Begin" /></param>
		/// <param name="lineOffsetBegin"><see cref="IParseItem.Begin" /></param>
		/// <param name="lineNumberBegin"><see cref="IParseItem.Begin" /></param>
		/// <returns><see cref="IParseItem.Begin" /></returns>
		public virtual INode Begin(IScopeParser scope, INode parentNode, string parseString, int absOffsetBegin, int lineOffsetBegin, int lineNumberBegin)
		{
			if (MatchBegin(parentNode, parseString, absOffsetBegin))
			{
				INode createdNode = CreateNode(parentNode, parseString, absOffsetBegin, lineOffsetBegin, lineNumberBegin);
				AddNodeToTree(parentNode, createdNode);
				return createdNode;
			}
			return null;
		}

		/// <summary>
		///  <see cref="IParseItem.End" />
		/// </summary>
		/// <param name="scope"><see cref="IParseItem.End" /></param>
		/// <param name="parentNode"><see cref="IParseItem.End" /></param>
		/// <param name="parseString"><see cref="IParseItem.End" /></param>
		/// <param name="index"><see cref="IParseItem.End" /></param>
		/// <param name="length"><see cref="IParseItem.End" /></param>
		/// <returns><see cref="IParseItem.End" /></returns>
		public abstract bool End(IScopeParser scope, INode parentNode, string parseString, int index, int length);

		/// <summary>
		/// Checks whether the current item is machted.
		/// </summary>
		/// <param name="parentNode">Parent node instance.</param>
		/// <param name="parseString">String to parse.</param>
		/// <param name="index">Index of the current character to parse.</param>
		/// <returns>Returns true, if the current item is machted.</returns>
		protected abstract bool MatchBegin(INode parentNode, string parseString, int index);
		
		/// <summary>
		/// Creates a new node instance which will be added to the node tree.
		/// </summary>
		/// <param name="parentNode">Parent node instance.</param>
		/// <param name="parseString">String to parse.</param>
		/// <param name="absOffsetBegin">Index of the current character to parse.</param>
		/// <param name="lineOffsetBegin">Line offset begin.</param>
		/// <param name="lineNumberBegin">Line number begin.</param>
		/// <returns>Returns the created node.</returns>
		protected virtual INode CreateNode(INode parentNode, string parseString, int absOffsetBegin, int lineOffsetBegin, int lineNumberBegin)
		{
			return new DefaultNode(this, parentNode, parseString, absOffsetBegin, lineOffsetBegin, lineNumberBegin);
		}

		/// <summary>
		/// Adds the created node to the node tree.
		/// </summary>
		/// <param name="parentNode">Node tree, which should contain the created node.</param>
		/// <param name="createdNode">Created node, which should be added to the node tree.</param>
		protected virtual void AddNodeToTree(INode parentNode, INode createdNode)
		{
			parentNode.AddChild(createdNode);
		}
	}
}

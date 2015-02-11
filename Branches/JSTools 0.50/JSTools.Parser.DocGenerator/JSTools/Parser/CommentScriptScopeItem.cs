/*
 * JSTools.Parser.DocGenerator.dll / JSTools.net - A framework for JavaScript/ASP.NET applications.
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
	/// This item is used to assign a scope parser to a parse item. This
	/// is required to declare nested scopes.
	/// </summary>
	internal class CommentScriptScopeItem : ScopeParserItem
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new CommentScriptScopeItem instance.
		/// </summary>
		/// <param name="parseItemName">Name of the current parse item which is assigned to the created nodes.</param>
		/// <param name="scopeName">Name of the scope which is assigned to this parse item.</param>
		/// <param name="createNewNode">True to create a new node which contains the child nodes created by the parse items of the specified scope.</param>
		/// <exception cref="ArgumentException">The specified parse item name is empty.</exception>
		/// <exception cref="ArgumentException">The specified scope name is empty.</exception>
		public CommentScriptScopeItem(string parseItemName, string scopeName, bool createNewNode) : base(parseItemName, scopeName, createNewNode)
		{
		}

		/// <summary>
		/// Creates a new ScopeParserItem instance.
		/// </summary>
		/// <param name="parseItemName">Name of the current parse item which is assigned to the created nodes.</param>
		/// <param name="scopeName">Name of the scope which is assigned to this parse item.</param>
		/// <exception cref="ArgumentException">The specified parse item name is empty.</exception>
		/// <exception cref="ArgumentException">The specified scope name is empty.</exception>
		public CommentScriptScopeItem(string parseItemName, string scopeName) : base(parseItemName, scopeName, true)
		{
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		///  <see cref="ScopeParserItem.CreateNode"/>
		/// </summary>
		/// <param name="parentNode"><see cref="ScopeParserItem.CreateNode"/></param>
		/// <param name="parseString"><see cref="ScopeParserItem.CreateNode"/></param>
		/// <param name="absOffsetBegin"><see cref="ScopeParserItem.CreateNode"/></param>
		/// <param name="lineOffsetBegin"><see cref="ScopeParserItem.CreateNode"/></param>
		/// <param name="lineNumberBegin"><see cref="ScopeParserItem.CreateNode"/></param>
		protected override INode CreateNode(INode parentNode, string parseString, int absOffsetBegin, int lineOffsetBegin, int lineNumberBegin)
		{
			return new CommentNode(
				this,
				parentNode,
				parseString,
				absOffsetBegin,
				lineOffsetBegin,
				lineNumberBegin );
		}
	}
}

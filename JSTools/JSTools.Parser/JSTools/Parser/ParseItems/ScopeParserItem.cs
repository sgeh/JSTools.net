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
	/// This item is used to assign a scope parser to a parse item. This
	/// is required to declare nested scopes.
	/// </summary>
	public class ScopeParserItem : IParseItem
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private string _scopeName = null;
		private ParserContext _context = null;
		private string _parseItemName = null;
		private bool _createNewNode = false;

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
		public bool IsAbsoluteEnd
		{
			get { return true; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new ScopeParserItem instance.
		/// </summary>
		/// <param name="parseItemName">Name of the current parse item which is assigned to the created nodes.</param>
		/// <param name="scopeName">Name of the scope which is assigned to this parse item.</param>
		/// <param name="createNewNode">True to create a new node which contains the child nodes created by the parse items of the specified scope.</param>
		/// <exception cref="ArgumentException">The specified parse item name is empty.</exception>
		/// <exception cref="ArgumentException">The specified scope name is empty.</exception>
		public ScopeParserItem(string parseItemName, string scopeName, bool createNewNode)
		{
			if (scopeName == null || scopeName.Length == 0)
				throw new ArgumentException("The specified scope name is empty.", "scopeName");

			if (parseItemName == null || parseItemName.Length == 0)
				throw new ArgumentException("The specified parse item name is empty.", "parseItemName");

			_parseItemName = parseItemName;
			_scopeName = scopeName;
			_createNewNode = createNewNode;
		}

		/// <summary>
		/// Creates a new ScopeParserItem instance.
		/// </summary>
		/// <param name="parseItemName">Name of the current parse item which is assigned to the created nodes.</param>
		/// <param name="scopeName">Name of the scope which is assigned to this parse item.</param>
		/// <exception cref="ArgumentException">The specified parse item name is empty.</exception>
		/// <exception cref="ArgumentException">The specified scope name is empty.</exception>
		public ScopeParserItem(string parseItemName, string scopeName) : this(parseItemName, scopeName, true)
		{
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
			IScopeParser scopeParser = ((IParserContextItem)this).Context.GetScope(_scopeName);

			if (scopeParser == null)
			{
				throw new InvalidOperationException(string.Format(
					"The specified scope '{0}' is not registred in the context.",
					_scopeName ));
			}

			INode scopeParentNode = parentNode;
			INode scopeNode = CreateNode(
				parentNode,
				parseString,
				absOffsetBegin,
				lineOffsetBegin,
				lineNumberBegin );
			
			if (_createNewNode)
			{
				scopeParentNode.AddChild(scopeNode);
				scopeParentNode = scopeNode;
			}

			if (scopeParser.TryParse(scopeParentNode, scope))
			{
				scopeNode.CodeLength = scopeParser.ParseToEnd();
				scopeNode.LineOffsetEnd = scopeParser.LineOffset;
				scopeNode.LineNumberEnd = scopeParser.LineNumber;
			}

			if (scopeParser.HasError || scopeNode.CodeLength < 1)
			{
				if (_createNewNode)
					parentNode.RemoveChild(parentNode.GetChildNodeIndex(scopeNode));

				scopeNode = null;
			}
			return scopeNode;
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
		public virtual bool End(IScopeParser scope, INode parentNode, string parseString, int index, int length)
		{
			return true;
		}

		/// <summary>
		/// Returns a new INode object. During this method call, the
		/// newly created node will be added to the given parent node using
		/// the AddChild() method.
		/// </summary>
		/// <param name="parentNode">Parent node instance which is used to create the node tree.</param>
		/// <param name="parseString">Whole code to parse.</param>
		/// <param name="absOffsetBegin">Absolute begin offset.</param>
		/// <param name="lineOffsetBegin">Offset of the start line.</param>
		/// <param name="lineNumberBegin">Line number, at which this instance begins.</param>
		protected virtual INode CreateNode(INode parentNode, string parseString, int absOffsetBegin, int lineOffsetBegin, int lineNumberBegin)
		{
			return new DefaultNode(
				this,
				parentNode,
				parseString,
				absOffsetBegin,
				lineOffsetBegin,
				lineNumberBegin );
		}
	}
}

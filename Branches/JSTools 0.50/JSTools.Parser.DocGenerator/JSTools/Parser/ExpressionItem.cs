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

using JSTools.Parser.ParseItems;

namespace JSTools.Parser
{
	/// <summary>
	/// Represents an expression, which mostly stands after a keyword.
	/// <code>
	///  function Name() { }
	/// </code>
	/// </summary>
	public class ExpressionItem : AParseItem
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const char UNDER_LINE = '_';
		private const char DOLLAR = '$';
		private const char DOT = '.';

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		///  <see cref="IParseItem.IsAbsoluteEnd" />
		/// </summary>
		public override bool IsAbsoluteEnd
		{
			get { return false; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new ExpressionItem instance.
		/// </summary>
		/// <param name="itemName">Name of the item.</param>
		public ExpressionItem(string itemName) : base(itemName)
		{
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		///  <see cref="IParseItem.End" />
		/// </summary>
		/// <param name="scope"><see cref="IParseItem.End" /></param>
		/// <param name="parentNode"><see cref="IParseItem.End" /></param>
		/// <param name="parseString"><see cref="IParseItem.End" /></param>
		/// <param name="index"><see cref="IParseItem.End" /></param>
		/// <param name="length"><see cref="IParseItem.End" /></param>
		/// <returns><see cref="IParseItem.End" /></returns>
		public override bool End(IScopeParser scope, INode parentNode, string parseString, int index, int length)
		{
			return !(parseString[index] == UNDER_LINE
				|| parseString[index] == DOLLAR
				|| parseString[index] == DOT
				|| char.IsLetterOrDigit(parseString, index));
		}

		/// <summary>
		///  <see cref="AParseItem.MatchBegin" />
		/// </summary>
		/// <param name="parentNode"><see cref="AParseItem.MatchBegin" /></param>
		/// <param name="parseString"><see cref="AParseItem.MatchBegin" /></param>
		/// <param name="index"><see cref="AParseItem.MatchBegin" /></param>
		/// <returns><see cref="AParseItem.MatchBegin" /></returns>
		protected override bool MatchBegin(INode parentNode, string parseString, int index)
		{
			return (parseString[index] == UNDER_LINE
				|| parseString[index] == DOLLAR
				|| char.IsLetter(parseString, index));
		}

		/// <summary>
		///  <see cref="AParseItem.CreateNode"/>
		/// </summary>
		/// <param name="parentNode"><see cref="AParseItem.CreateNode"/></param>
		/// <param name="parseString"><see cref="AParseItem.CreateNode"/></param>
		/// <param name="absOffsetBegin"><see cref="AParseItem.CreateNode"/></param>
		/// <param name="lineOffsetBegin"><see cref="AParseItem.CreateNode"/></param>
		/// <param name="lineNumberBegin"><see cref="AParseItem.CreateNode"/></param>
		protected override INode CreateNode(INode parentNode, string parseString, int absOffsetBegin, int lineOffsetBegin, int lineNumberBegin)
		{
			return new ExpressionNode(
				this,
				parentNode,
				parseString,
				absOffsetBegin,
				lineOffsetBegin,
				lineNumberBegin );
		}
	}
}

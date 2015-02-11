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
	/// Represents a parse item which can be used to parse a part of 
	/// code except the specified character.
	/// </summary>
	public class ExcludeItem : DefaultParseItem
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private readonly char EXCLUDE_CHAR;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new ExcludeItem instance.
		/// </summary>
		/// <param name="parseItemName">Name of the current parse item which is assigned to the created nodes.</param>
		/// <param name="toExclude">Char which should not be matched by this item.</param>
		/// <exception cref="ArgumentException">The specified parse item name is empty.</exception>
		public ExcludeItem(string parseItemName, char toExclude) : base(parseItemName)
		{
			EXCLUDE_CHAR = toExclude;
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
		public override INode Begin(IScopeParser scope, INode parentNode, string parseString, int absOffsetBegin, int lineOffsetBegin, int lineNumberBegin)
		{
			if (parseString[absOffsetBegin] != EXCLUDE_CHAR)
				return base.Begin(scope, parentNode, parseString, absOffsetBegin, lineOffsetBegin, lineNumberBegin);
			else
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
		public override bool End(IScopeParser scope, INode parentNode, string parseString, int index, int length)
		{
			return base.End(scope, parentNode, parseString, index, length);
		}
	}
}

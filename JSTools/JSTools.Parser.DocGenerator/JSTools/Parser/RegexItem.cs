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
	/// Represents the a javascript regex.
	/// <c>/(a|ab)[a-z]*/</c>
	/// </summary>
	internal class RegexItem : AParseItem
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const char ESCAPE_CHAR = '\\';
		private const char BOUNDARY_CHAR = '/';

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		///  <see cref="IParseItem.IsAbsoluteEnd" />
		/// </summary>
		public override bool IsAbsoluteEnd
		{
			get { return true; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new RegexItem instance.
		/// </summary>
		/// <param name="itemName">Name of the item.</param>
		internal RegexItem(string itemName) : base(itemName)
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
			return (parseString[index] == BOUNDARY_CHAR  && !StringItem.IsEscaped(parseString, index));
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
			return (parseString[index] == BOUNDARY_CHAR
				&& (index - 1 == -1 || IsBegin(parseString, index)));
		}

		private bool IsBegin(string parseString, int index)
		{
			int i = index - 1;

			for ( ; (i > -1 && !Char.IsWhiteSpace(parseString, i)); --i)
			{
				;
			}

			if (i != -1)
				return Char.IsLetterOrDigit(parseString, i);
			else
				return true;
		}
	}
}

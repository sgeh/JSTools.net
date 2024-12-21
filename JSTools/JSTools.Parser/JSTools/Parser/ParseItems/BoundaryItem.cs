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
	/// Parses a part of the string which stands between the specified
	/// boundaries.
	/// <code>
	///  /* .... */
	/// </code>
	/// </summary>
	public class BoundaryItem : AParseItem
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private readonly string BOUND_START;
		private readonly string BOUND_END;

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
		/// Creates a new CharTokenItem instance.
		/// </summary>
		/// <param name="itemName">Name of the item.</param>
		/// <param name="boundaryStart">Start boundary of the item.</param>
		/// <param name="boundaryEnd">End boundary of the item.</param>
		public BoundaryItem(string itemName, string boundaryStart, string boundaryEnd) : base(itemName)
		{
			BOUND_START = boundaryStart;
			BOUND_END = boundaryEnd;
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
			return (length > BOUND_START.Length + BOUND_END.Length
				&& String.Compare(BOUND_END, 0, parseString, index, BOUND_END.Length) == 0);
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
			return (parseString.Length > index + BOUND_START.Length
				&& String.Compare(BOUND_START, 0, parseString, index, BOUND_START.Length) == 0);
		}
	}
}

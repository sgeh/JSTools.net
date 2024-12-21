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
	/// Represents a string, which specifies a parsing token.
	/// Following characters specify are typical tokens:
	/// <code>
	///  int typeof instanceOf
	/// </code>
	/// </summary>
	public class KeywordTokenItem : AParseItem
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private readonly string KEY_WORD;

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
		/// Creates a new KeywordTokenItem instance.
		/// </summary>
		/// <param name="itemName">Name of the item.</param>
		/// <param name="keyWord">Keyword to parse; minimal length should be 2.</param>
		/// <exception cref="ArgumentException">Invalid keyword specified. The value must not be null and longer than 2.</exception>
		public KeywordTokenItem(string itemName, string keyWord) : base(itemName)
		{
			if (keyWord == null || keyWord.Length < 2)
				throw new ArgumentException("Invalid keyword specified. The value must not be null and longer than 2.");

			KEY_WORD = keyWord;
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
			return true;
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
			return (parseString.Length > index + KEY_WORD.Length
				&& IsKeyWordEnd(parseString, index)
				&& String.Compare(parseString, index, KEY_WORD, 0, KEY_WORD.Length) == 0);
		}

		/// <summary>
		///  <see cref="AParseItem.CreateNode" />
		/// </summary>
		/// <param name="parentNode"><see cref="AParseItem.CreateNode" /></param>
		/// <param name="parseString"><see cref="AParseItem.CreateNode" /></param>
		/// <param name="absOffsetBegin"><see cref="AParseItem.CreateNode" /></param>
		/// <param name="lineOffsetBegin"><see cref="AParseItem.CreateNode" /></param>
		/// <param name="lineNumberBegin"><see cref="AParseItem.CreateNode" /></param>
		/// <returns><see cref="AParseItem.CreateNode" /></returns>
		protected override INode CreateNode(INode parentNode, string parseString, int absOffsetBegin, int lineOffsetBegin, int lineNumberBegin)
		{
			return new DefaultNode(this, parentNode, parseString, absOffsetBegin, lineOffsetBegin, lineNumberBegin, KEY_WORD.Length);
		}

		private bool IsKeyWordEnd(string parseString, int index)
		{
			if (parseString.Length == index + KEY_WORD.Length)
				return true;
			else
				return !Char.IsLetterOrDigit(parseString, index + KEY_WORD.Length);
		}
	}
}

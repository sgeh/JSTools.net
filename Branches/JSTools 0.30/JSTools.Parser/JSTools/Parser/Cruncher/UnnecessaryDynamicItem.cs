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

namespace JSTools.Parser.Cruncher
{
	/// <summary>
	/// Represents string items, which contain white spaces only.
	/// </summary>
	public class UnnecessaryDynamicItem : IParseItem
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		public	const			string	ITEM_NAME		= "Unnecessary Dynamic White Space Item";
		private const			bool	IS_ABSOLUTE_END	= false;

		private	TokenParser		_parent					= null;


		/// <summary>
		/// Returns the name of this parse item.
		/// </summary>
		public string ItemName
		{
			get { return ITEM_NAME; }
		}


		/// <summary>
		/// Returns true, if the ending character can not be a start character
		/// of another IParseItem instance. The parser will abort parsing of the
		/// current character and continue with the next. The method will be called
		/// after End() has returned true.
		/// </summary>
		public bool IsAbsoluteEnd
		{
			get { return IS_ABSOLUTE_END; }
		}


		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new UnnecessaryDynamicItem instance.
		/// </summary>
		public UnnecessaryDynamicItem()
		{
		}


		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Returns a new INode object. It is used to create the node tree.
		/// </summary>
		public INode CreateNode()
		{
			return new EmptyNode(ItemName);
		}


		/// <summary>
		/// Returns true, if this parse item begins at the specified index.
		/// </summary>
		/// <param name="parentNode">Parent node instance.</param>
		/// <param name="parseString">String, which should be parsed.</param>
		/// <param name="index">Long value, which represents the current scan index.</param>
		public bool Begin(INode parentNode, string parseString, int index)
		{
			if (!Char.IsWhiteSpace(parseString[index]))
				return false;

			if (index - 1 < 0)
				return false;

			if (!IsSpaceOrSeparator(parseString, index - 1) && IsSpaceOrSeparator(parseString, index + 1))
				return true;

			return false;
		}


		/// <summary>
		/// Returns true, if the given char ends this parse item.
		/// </summary>
		/// <param name="parentNode">Parent node instance.</param>
		/// <param name="parseString">String, which should be parsed.</param>
		/// <param name="index">Long value, which represents the current scan index.</param>
		public bool End(INode parentNode, string parseString, int index)
		{
			if (!Char.IsWhiteSpace(parseString, index))
				return true;

			// we return false because the IsAbsoluteEnd (false) property.
			// the scope will terminate the node automatically
			if (index + 1 == parseString.Length)
				return false;

			return !IsSpaceOrSeparator(parseString, index + 1);
		}


		/// <summary>
		/// Sets the parent parser, which can be used to create recursive calls.
		/// </summary>
		/// <param name="parent">The parent parser.</param>
		public void SetParser(TokenParser parent)
		{
			_parent = parent;
		}


		/// <summary>
		/// Returns true, if the char at the given position is a valid begin char.
		/// </summary>
		/// <param name="toCheck">String to check.</param>
		/// <param name="index">Position to check.</param>
		/// <returns>Returns true if it is a begin char, otherwise false.</returns>
		private bool IsSpaceOrSeparator(string toCheck, int index)
		{
			// if the white space is on the begin or end of the string, it is not necessary
			if (index < 0 || index >= toCheck.Length)
				return true;

			// if the next/previous character is a white space, this white space is not necessary
			if (Char.IsWhiteSpace(toCheck[index]))
				return true;

			// if the next/previous character is a separator, this white space is not necessary
			if (Cruncher.IsSeparator(toCheck[index]))
				return true;

			return false;
		}
	}
}

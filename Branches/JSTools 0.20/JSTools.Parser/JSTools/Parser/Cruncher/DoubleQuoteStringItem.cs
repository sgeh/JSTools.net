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
	/// Represents a JavaScript double quote string ("example").
	/// </summary>
	public class DoubleQuoteStringItem : StringItem
	{
		//------------------------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------------------------

		public	const char		BEGIN_CHAR		= '"';
		public	const string	ITEM_NAME		= "Double Quote String Item";


		/// <summary>
		/// Returns the string begin character.
		/// </summary>
		public override char BeginChar
		{
			get { return BEGIN_CHAR; }
		}


		/// <summary>
		/// Returns the name of this parse item.
		/// </summary>
		public override string ItemName
		{
			get { return ITEM_NAME; }
		}


		//------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Creates a new DoubleQuoteStringItem instance.
		/// </summary>
		public DoubleQuoteStringItem()
		{
		}


		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------
	}
}

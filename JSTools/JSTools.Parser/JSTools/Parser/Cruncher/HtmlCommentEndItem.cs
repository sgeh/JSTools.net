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
	/// Represents a comment line item, which holds strings like "--&gt;".
	/// </summary>
	public class HtmlCommentEndItem : HtmlCommentItem
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private	const string	COMMENT_STRING	= "-->";
		public	const string	ITEM_NAME		= "HTML Comment End Item";


		/// <summary>
		/// Returns the name of this parse item.
		/// </summary>
		public override string ItemName
		{
			get { return ITEM_NAME; }
		}


		/// <summary>
		/// Returns the comment string (e.g. &lt;!--).
		/// </summary>
		protected override string CommentString
		{
			get { return COMMENT_STRING; }
		}


		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new HtmlCommentEndItem instance.
		/// </summary>
		public HtmlCommentEndItem()
		{
		}


		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------
	}
}

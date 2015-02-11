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
using System.Collections;

namespace JSTools.Parser.Cruncher
{
	/// <summary>
	/// Summary description for CommentNode.
	/// </summary>
	public class WhiteSpaceNode : CrunchNode
	{
		//------------------------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------------------------

		private const string WHITE_SPACE = " ";


		//------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Creates a new CommentNode object.
		/// </summary>
		/// <param name="parseItemName">Name of the representing parse item.</param>
		public WhiteSpaceNode(string parseItemName) : base(parseItemName)
		{
		}


		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Returns a new crunch string, which does not contain unnecessary characters.
		/// </summary>
		/// <returns>Returns the crunch string of the current object.</returns>
		public override string ToCrunchString()
		{
			return WHITE_SPACE;
		}
	}
}

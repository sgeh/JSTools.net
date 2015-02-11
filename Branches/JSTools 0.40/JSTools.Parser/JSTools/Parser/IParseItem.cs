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

namespace JSTools.Parser
{
	/// <summary>
	/// Descripts an item, which is used to parse the specified string.
	/// 
	/// After the function Begin() has returned true, the CreateNode() method
	/// will be called to create the node tree.
	/// 
	/// For example, a for-loop statement can be specified with this interface.
	/// </summary>
	public interface IParseItem
	{
		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Returns the name of this parse item.
		/// </summary>
		string ItemName
		{
			get;
		}

		/// <summary>
		/// Returns true, if the ending character can not be a start character
		/// of another IParseItem instance. The parser will abort parsing of the
		/// current character and continue with the next. The method will be called
		/// after End() has returned true.
		/// </summary>
		bool IsAbsoluteEnd
		{
			get;
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Returns a new INode object. It is used to create the node tree.
		/// </summary>
		INode CreateNode();

		/// <summary>
		/// Returns true, if this parse item begins at the specified index.
		/// </summary>
		/// <param name="parentNode">Parent node instance.</param>
		/// <param name="parseString">String, which should be parsed.</param>
		/// <param name="index">Long value, which represents the current scan index.</param>
		bool Begin(INode parentNode, string parseString, int index);

		/// <summary>
		/// Returns true, if the given char ends this parse item.
		/// </summary>
		/// <param name="parentNode">Parent node instance.</param>
		/// <param name="parseString">String, which should be parsed.</param>
		/// <param name="index">Long value, which represents the current scan index.</param>
		bool End(INode parentNode, string parseString, int index);

		/// <summary>
		/// Sets the parent parser, which can be used to create recursive calls.
		/// </summary>
		/// <param name="parent">The parent parser.</param>
		/// <exception cref="InvalidOperationException">A parent parser instance was already set.</exception>
		void SetParser(TokenParser parent);
	}
}

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
using JSCompiler.CompileChars;

namespace JSCompiler.Script.Compile
{
	/// <summary>
	/// Zusammenfassungsbeschreibung für DefaultCodeItem.
	/// </summary>
	public class DefaultCodeItem
	{
		public DefaultCodeItem()
		{
		}


		public string ParsePosition(string compileString, string originalString, int currentPosition)
		{
			char toAppend = originalString[currentPosition];

			if (CompileChar.IsWhiteSpace(toAppend))
			{
				if (IsNeighborASeparator(compileString) || IsNeighborASeparator(originalString, currentPosition + 1))
				{
					return (compileString[compileString.Length - 1].ToString() == CompileChar.LINEREPLACE || originalString[currentPosition].ToString() + originalString[currentPosition + 1].ToString() != CompileChar.LINEBREAK) ? "" : CompileChar.LINEREPLACE;
				}
				else
				{
					return " ";
				}
			}
			return Convert.ToString(toAppend);
		}



		private bool IsNeighborASeparator(string compileString)
		{
			if (compileString.Length > 0)
			{
				char neighbor = compileString[compileString.Length - 1];
				return (CompileChar.IsSeparator(neighbor) || CompileChar.IsWhiteSpace(neighbor));
			}
			return false;
		}


		private bool IsNeighborASeparator(string compileString, int currentPosition)
		{
			string neighbor = CodeItemContainer.Instance.GetFigureFromString(currentPosition, compileString);
			return (neighbor != "" && (CompileChar.IsSeparator(neighbor[0]) || CompileChar.IsWhiteSpace(neighbor[0])));
		}
	}
}

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
using System.Text.RegularExpressions;

namespace JSCompiler.CompileChars
{
	/// <summary>
	/// Zusammenfassungsbeschreibung für CompileChar.
	/// </summary>
	public sealed class CompileChar
	{
		private	static	char[]		SEPARATOR					= { '{', '}', '[', ']', '(', ')', ';', ',', '+', '-', '/', '*', '=', '&', '|', '%', '>', '<', ':', '?', '!' };
		public	static	string		LINEBREAK					= ((char)13).ToString() + ((char)10).ToString();
		public	static	string		LINEREPLACE					= ((char)10).ToString();
		public	static	string[]	SINGLELINE_COMMENT_BEGIN	= { "//", "<!--" };
		public	static	string		SINGLELINE_COMMENT_END		= LINEBREAK;
		public	static	string		MULTILINE_COMMENT_BEGIN		= "/*";
		public	static	string		MULTILINE_COMMENT_END		= "*/";
		public	static	string		QUOTE_STRING				= "\"";
		public	static	string		SINGLEQUOTE_STRING			= "'";


		/// <summary>
		/// checks if "toCheck" is a white space
		/// </summary>
		/// <param name="toCheck">char to check</param>
		/// <returns>returns true, if "toCheck" is a wihte space (e.g. tabulator, return, ...)</returns>
		public static bool IsWhiteSpace(char toCheck)
		{
			Regex checkRegex = new Regex("\\s");
			return checkRegex.IsMatch(Convert.ToString(toCheck));
		}


		/// <summary>
		/// checks if "toCheck" is a separator
		/// </summary>
		/// <param name="toCheck">char to check</param>
		/// <returns>returns true, if "toCheck" is a separator (e.g. '{', '}', '[', ']', ...)</returns>
		public static bool IsSeparator(char toCheck)
		{
			for (int i = 0; i < SEPARATOR.Length; ++i)
			{
				if (SEPARATOR[i] == toCheck)
				{
					return true;
				}
			}
			return false;
		}


		/// <summary>
		/// checks two chars for a line break
		/// </summary>
		/// <param name="first">first char to check</param>
		/// <param name="second">second char to check</param>
		/// <returns>returns true, if "first" is a char(13) and "second" is a char(10)</returns>
		public static bool IsLineBreak(char first, char second)
		{
			return (first.ToString() + second.ToString() == LINEBREAK);
		}
	}
}

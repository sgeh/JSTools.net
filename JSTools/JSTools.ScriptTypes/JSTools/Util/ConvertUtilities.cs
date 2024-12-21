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
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace JSTools.Util
{
	/// <summary>
	/// Provides global string encode/decode functionalities. Hex2Dec and
	/// Dec2Hex can be used to translate a hex value into a dec value.
	/// ScriptEscape and ScriptUnescape are equal to the client side escape()
	/// and unescape() functions.
	/// </summary>
	public sealed class ConvertUtilities
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private static readonly Regex ESCAPED_STRING_PATTERN = new Regex("%([a-f0-9]{2,2})", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		private static readonly Regex UNESCAPED_STRING_PATTERN = new Regex(@"([^\w*@\-+./])", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// This class cannot be instantiated and provides global functionalities.
		/// </summary>
		private ConvertUtilities()
		{
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Parses the given hex value string and returns the representing
		/// decimal value.
		/// </summary>
		/// <param name="toParse">Hexadecimal value to parse.</param>
		/// <returns>Returns the specified value or -1 if the value could not
		/// be parsed.</returns>
		public static int Hex2Dec(string toParse)
		{
			try
			{
				return int.Parse(toParse, NumberStyles.HexNumber);
			}
			catch
			{
				return -1;
			}
		}

		/// <summary>
		/// Converts the given integer into a hexadecimal string value.
		/// </summary>
		/// <returns>Returns the converted or an empty string if the value
		/// is smaller than 0.</returns>
		public static string Dec2Hex(int toConvert)
		{
			if (toConvert < 0)
				return string.Empty;

			return toConvert.ToString("X", NumberFormatInfo.InvariantInfo);
		}

		/// <summary>
		/// Encodes spaces, punctuation, and all other characters which are not
		/// contained in the ASCII character table. This method is equal to the
		/// top-level function "escape()".
		/// </summary>
		/// <param name="toEscape">String to escape.</param>
		/// <returns>The ScriptEscape function encodes characters in the specified string
		/// and returns a new string. </returns>
		public static string ScriptEscape(string toEscape)
		{
			if (toEscape == null)
				return string.Empty;

			return UNESCAPED_STRING_PATTERN.Replace(toEscape, new MatchEvaluator(OnUnescapedCharMatch));
		}

		/// <summary>
		/// Decodes encoded spaces, punctuation, and all other characters which are
		/// not contained in the ASCII character table. These characters may be
		/// escaped using the ScriptEscape() method. This method is equal to the
		/// top-level function "unescape()".
		/// </summary>
		/// <param name="toUnescape">String to unescape.</param>
		/// <returns>Returns the ASCII string for the specified hexadecimal encoding value.</returns>
		public static string ScriptUnescape(string toUnescape)
		{
			return ESCAPED_STRING_PATTERN.Replace(toUnescape, new MatchEvaluator(OnEscapedCharMatch));
		}

		private static string OnEscapedCharMatch(Match matchedChar)
		{
			int convertedValue = Hex2Dec(matchedChar.Groups[1].Value);

			if (convertedValue > -1 && convertedValue < 256)
				return Encoding.ASCII.GetString(new byte[] { (byte)convertedValue } );
			else
				return matchedChar.Groups[0].Value;
		}

		private static string OnUnescapedCharMatch(Match matchedChar)
		{
			if (matchedChar.Groups[0].Value.Length != 0)
			{
				string hexValue = Dec2Hex(matchedChar.Groups[0].Value[0]);

				if (hexValue.Length > 2)
					return matchedChar.Groups[0].Value;

				return "%" + hexValue.PadLeft(2, '0');
			}
			else
				return string.Empty;
		}
	}
}

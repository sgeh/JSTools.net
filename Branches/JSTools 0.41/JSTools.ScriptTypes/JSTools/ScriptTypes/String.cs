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

using JSTools.Util;

namespace JSTools.ScriptTypes
{
	/// <summary>
	/// Represents the javascript string type. A number is mapped with the
	/// following .NET datatypes:
	///  string, char
	/// </summary>
	public class String : AScriptType
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const string SINGLE_QUOTE_BEGIN = "'";
		private const string DOUBLE_QUOTE_BEGIN = "\"";
		private const char ESCAPE_CHAR = '\\';
		private const string SCRIPT_STRING = DOUBLE_QUOTE_BEGIN + "{0}" + DOUBLE_QUOTE_BEGIN;

		// see '7.8.4 String Literals' chapter of ECMA-262
		private static readonly string[][] SINGLE_ESCAPE_CHARS = new string[][] 
			{
				new string[] { "\u0008", "\\b" },
				new string[] { "\u0009", "\\t" },
				new string[] { "\u000A", "\\n" },
				new string[] { "\u000B", "\\v" },
				new string[] { "\u000C", "\\f" },
				new string[] { "\u000D", "\\r" },
				new string[] { "\u0022", "\\\"" },
				new string[] { "\u0027", "\\'" }
				// new string[] { "\u005C", "\\" } -> replacement not required
			};

		private static readonly Regex SINGLE_ESCAPE_REGEX = new Regex("(\\" + ESCAPE_CHAR + "+)([btnvfr'\"]?)", RegexOptions.Compiled);

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		///  <see cref="AScriptType.ManagedTypes"/>
		/// </summary>
		internal protected override Type[] ManagedTypes
		{
			get { return new Type[] { typeof(string), typeof(char) }; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new String instance.
		/// </summary>
		public String()
		{
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		///  <see cref="AScriptType" />
		/// </summary>
		/// <param name="toCheck">
		///  <see cref="AScriptType" />
		/// </param>
		/// <returns>
		///  <see cref="AScriptType" />
		/// </returns>
		public override bool IsTypeOf(string toCheck)
		{
			return true;
		}

		/// <summary>
		///  <see cref="AScriptType" />
		/// </summary>
		/// <param name="valueToConvert">
		///  <see cref="AScriptType" />
		/// </param>
		/// <param name="encodeValue">
		///  <see cref="AScriptType"/>
		/// </param>
		/// <returns>
		///  <see cref="AScriptType" />
		/// </returns>
		protected override string GetStringRepresentation(object valueToConvert, bool encodeValue)
		{
			string convertedValue = valueToConvert.ToString();

			if (encodeValue)
			{
				// encode string and insert quotes
				return string.Format(
					SCRIPT_STRING,
					ConvertUtilities.ScriptEscape(convertedValue) );
			}
			else
			{
				// escape backslashes
				convertedValue = convertedValue.Replace(new string(ESCAPE_CHAR, 1), new string(ESCAPE_CHAR, 2));

				// replace escape characters
				foreach (string[] escapeItem in SINGLE_ESCAPE_CHARS)
				{
					convertedValue = convertedValue.Replace(escapeItem[0], escapeItem[1]);
				}

				// insert quotes
				return string.Format(SCRIPT_STRING, convertedValue);
			}
		}

		/// <summary>
		///  <see cref="AScriptType" />
		/// </summary>
		/// <param name="valueToConvert">
		///  <see cref="AScriptType" />
		/// </param>
		/// <param name="decodeValue">
		///  <see cref="AScriptType" />
		/// </param>
		/// <returns>
		///  <see cref="AScriptType" />
		/// </returns>
		protected override object GetValueFromString(string valueToConvert, bool decodeValue)
		{
			// remove ending and starting " / ' charaters
			valueToConvert = GetStringValue(valueToConvert);

			if (decodeValue)
				return ConvertUtilities.ScriptUnescape(valueToConvert);

			return valueToConvert;
		}

		private string GetStringValue(string toGetValue)
		{
			if (toGetValue.Length > 1)
			{
				if ((toGetValue.StartsWith(SINGLE_QUOTE_BEGIN) && toGetValue.EndsWith(SINGLE_QUOTE_BEGIN))
					|| (toGetValue.StartsWith(DOUBLE_QUOTE_BEGIN) && toGetValue.EndsWith(DOUBLE_QUOTE_BEGIN)))
				{
					string plainValue = toGetValue.Substring(1, toGetValue.Length - 2);
					string decodedValue = SINGLE_ESCAPE_REGEX.Replace(plainValue, new MatchEvaluator(ReplaceEscapeString));

					return decodedValue;
				}
			}
			return toGetValue;
		}

		private string ReplaceEscapeString(Match regexMatch)
		{
			string leadingBackslashes = regexMatch.Groups[1].Value;
			string singleEscapeChar = regexMatch.Groups[2].Value;

			string decodedBackslashes = new string(ESCAPE_CHAR, (leadingBackslashes.Length / 2));
			string decodedEscapeChar = singleEscapeChar;

			if (leadingBackslashes.Length % 2 == 1)
				decodedEscapeChar = GetDecodedString(ESCAPE_CHAR + singleEscapeChar);

			return decodedBackslashes + decodedEscapeChar;
		}

		private string GetDecodedString(string toDecode)
		{
			if (toDecode.Length != 0)
			{
				foreach (string[] escapeItem in SINGLE_ESCAPE_CHARS)
				{
					if (escapeItem[1] == toDecode)
						return escapeItem[0];
				}
			}
			return string.Empty;
		}
	}
}

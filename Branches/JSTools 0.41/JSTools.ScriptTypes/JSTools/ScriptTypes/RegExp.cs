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

namespace JSTools.ScriptTypes
{
	/// <summary>
	/// Represents the javascript RegExp type.
	/// The Regex flags are casted as follows:
	/// 
	///  -------------------------------------
	///   From:							To:
	///  -------------------------------------
	///   RegexOptions.Multiline		-> m
	///   RegexOptions.ExplicitCapture	-> g
	///   RegexOptions.IgnoreCase		-> i
	/// </summary>
	public class RegExp : AScriptType
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private static readonly Regex SCRIPT_REGEX_PATTERN = new Regex("^/(?<pattern>.+)/(?<flags>[a-zA-Z]*)$", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase);

		private const string REGEX_BOUNDARY = "/";
		private const string MULTILINE_FLAG = "m";
		private const string GLOBAL_FLAG = "g";
		private const string IGNORE_FLAG = "i";

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		///  <see cref="AScriptType.ManagedTypes"/>
		/// </summary>
		internal protected override Type[] ManagedTypes
		{
			get { return new Type[] { typeof(Regex) }; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new RegExp instance.
		/// </summary>
		public RegExp()
		{
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		///  <see cref="AScriptType.IsTypeOf" />
		/// </summary>
		/// <param name="toCheck">
		///  <see cref="AScriptType.IsTypeOf" />
		/// </param>
		/// <returns>
		///  <see cref="AScriptType.IsTypeOf" />
		/// </returns>
		public override bool IsTypeOf(string toCheck)
		{
			return (toCheck != null && SCRIPT_REGEX_PATTERN.IsMatch(toCheck));
		}

		/// <summary>
		///  <see cref="AScriptType.GetStringRepresentation" />
		/// </summary>
		/// <param name="valueToConvert">
		///  <see cref="AScriptType.GetStringRepresentation" />
		/// </param>
		/// <param name="encodeValue">
		///  <see cref="AScriptType.GetStringRepresentation"/>
		/// </param>
		/// <returns>
		///  <see cref="AScriptType.GetStringRepresentation" />
		/// </returns>
		protected override string GetStringRepresentation(object valueToConvert, bool encodeValue)
		{
			Regex regexToConvert = (Regex)valueToConvert;
			
			return REGEX_BOUNDARY
				+ regexToConvert.ToString()
				+ REGEX_BOUNDARY
				+ (((regexToConvert.Options & RegexOptions.Multiline) != 0) ? MULTILINE_FLAG : string.Empty)
				+ (((regexToConvert.Options & RegexOptions.ExplicitCapture) != 0) ? GLOBAL_FLAG : string.Empty)
				+ (((regexToConvert.Options & RegexOptions.IgnoreCase) != 0) ? IGNORE_FLAG : string.Empty);
		}

		/// <summary>
		///  <see cref="AScriptType.GetValueFromString" />
		/// </summary>
		/// <param name="valueToConvert">
		///  <see cref="AScriptType.GetValueFromString" />
		/// </param>
		/// <param name="decodeValue">
		///  <see cref="AScriptType.GetValueFromString" />
		/// </param>
		/// <returns>
		///  <see cref="AScriptType.GetValueFromString" />
		/// </returns>
		protected override object GetValueFromString(string valueToConvert, bool decodeValue)
		{
			Match regexValue = SCRIPT_REGEX_PATTERN.Match(valueToConvert);
			Regex parsedValue = null;

			if (regexValue.Success)
			{
				RegexOptions options = RegexOptions.Compiled;

				if (regexValue.Groups["pattern"].Value.IndexOf(MULTILINE_FLAG) != -1)
					options |= RegexOptions.Multiline;

				if (regexValue.Groups["pattern"].Value.IndexOf(GLOBAL_FLAG) != -1)
					options |= RegexOptions.ExplicitCapture;

				if (regexValue.Groups["pattern"].Value.IndexOf(IGNORE_FLAG) != -1)
					options |= RegexOptions.IgnoreCase;

				parsedValue = new Regex(regexValue.Groups["pattern"].Value, options);
			}
			return parsedValue;
		}
	}
}

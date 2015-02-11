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
		private const string ESCAPE_STRING = "\\";

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
			if (encodeValue)
				return DOUBLE_QUOTE_BEGIN
					+ ConvertUtilities.ScriptEscape(valueToConvert.ToString())
					+ DOUBLE_QUOTE_BEGIN;

			return DOUBLE_QUOTE_BEGIN
				+ valueToConvert.ToString().Replace(DOUBLE_QUOTE_BEGIN, ESCAPE_STRING + DOUBLE_QUOTE_BEGIN)
				+ DOUBLE_QUOTE_BEGIN;
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
			if (decodeValue)
				return ConvertUtilities.ScriptUnescape(valueToConvert);

			return valueToConvert;
		}
	}
}

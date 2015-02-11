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
using System.Text;
using System.Text.RegularExpressions;

namespace JSTools.ScriptTypes
{
	/// <summary>
	/// Represents the javascript Array type. The string representation will
	/// begin with a '[' and end with a ']' character.
	/// </summary>
	public class Array : AScriptType
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const string ARRAY_BEGIN = "[";
		private const string ARRAY_END = "]";
		private const string ARRAY_SEPARATOR = ",";

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		///  <see cref="AScriptType.ManagedTypes"/>
		/// </summary>
		internal protected override Type[] ManagedTypes
		{
			get { return new Type[] { typeof(IEnumerable) }; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new Array instance.
		/// </summary>
		public Array()
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
			return (toCheck != null);//(toCheck != null && SCRIPT_ARRAY_PATTERN.IsMatch(toCheck));
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
			StringBuilder arrayDeclaration = new StringBuilder(ARRAY_BEGIN);

			foreach (object itemToConvert in (IEnumerable)valueToConvert)
			{
				arrayDeclaration.Append(new ScriptValue(itemToConvert, encodeValue).ToString());
				arrayDeclaration.Append(ARRAY_SEPARATOR);
			}

			if (arrayDeclaration.Length > 1)
				return arrayDeclaration.ToString(0, arrayDeclaration.Length - 1) + ARRAY_END;
			else
				return arrayDeclaration.ToString() + ARRAY_END;
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
			return null;
		}
	}
}

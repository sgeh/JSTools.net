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

namespace JSTools.ScriptTypes
{
	/// <summary>
	/// Represents the javascript bool type.
	/// </summary>
	public class Boolean : AScriptType
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		///  <see cref="AScriptType.ManagedTypes"/>
		/// </summary>
		internal protected override Type[] ManagedTypes
		{
			get { return new Type[] { typeof(bool) }; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new Boolean instance.
		/// </summary>
		public Boolean()
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
			return (toCheck != null && (toCheck == bool.TrueString.ToLower() || toCheck == bool.FalseString.ToLower()));
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
			return ((bool)valueToConvert).ToString().ToLower();
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
			return (valueToConvert == bool.TrueString.ToLower());
		}
	}
}

/*
 * JSTools.ScriptTypes.dll / JSTools.net - A framework for JavaScript/ASP.NET applications.
 * Copyright (C) 2005  Silvan Gehrig
 *
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
 *
 * Author:
 *  Silvan Gehrig
 */

using System;
using System.Text;
using System.Text.RegularExpressions;

using JSTools.Util.Serialization;

namespace JSTools.ScriptTypes
{
	/// <summary>
	/// Represents the javascript Object type. The string representation will
	/// begin with a '[' or '{' and end with a ']' or '}' character.
	/// </summary>
	/// <remarks>Arrays are objects too.</remarks>
	public class Object : AScriptType
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private static readonly Regex OBJECT_PATTERN = new Regex(@"^\s*([\[\{]).*\1\s*$", RegexOptions.Compiled | RegexOptions.Singleline);

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		///  <see cref="AScriptType.ManagedTypes"/>
		/// </summary>
		internal protected override Type[] ManagedTypes
		{
			get { return new Type[] { typeof(object) }; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new Object instance.
		/// </summary>
		public Object()
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
			return (toCheck != null && OBJECT_PATTERN.IsMatch(toCheck));
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
			return new SimpleObjectSerializer().Serialize(valueToConvert, encodeValue);
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
			return new SimpleObjectSerializer().Deserialize(valueToConvert, decodeValue);
		}
	}
}

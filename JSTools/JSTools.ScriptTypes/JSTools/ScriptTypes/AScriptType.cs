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

namespace JSTools.ScriptTypes
{
	/// <summary>
	/// Represents the base class for all ScriptType's. To get the string
	/// representation of a type, you should call the ToString() method.
	/// </summary>
	public abstract class AScriptType
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the types associated with the given AScriptType. The
		/// specified values will be mapped with the returning types.
		/// </summary>
		internal protected abstract Type[] ManagedTypes
		{
			get;
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new AScriptType instance.
		/// </summary>
		public AScriptType()
		{
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Checks whether the specified type can be mapped with this
		/// script type.
		/// </summary>
		/// <param name="toCheck">Type which should be checked.</param>
		/// <returns>Returns true if the specified type can be mapped with this
		/// script type.</returns>
		public virtual bool CanMapType(Type toCheck)
		{
			foreach (Type managedType in ManagedTypes)
			{
				if (managedType == toCheck)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Checks whether the given string is compatible with this type. If
		/// this method returns true, you may cast the string into the
		/// representing object with the GetObjectFromString() method.
		/// </summary>
		/// <param name="toCheck">String which should be checked.</param>
		/// <returns>Returns true if this type is compatible with the given string.</returns>
		public abstract bool IsTypeOf(string toCheck);

		/// <summary>
		/// Converts the given string into the type given by the ManagedType.
		/// </summary>
		/// <param name="toConvert">String which should be converted.</param>
		/// <param name="decodeValue">True to decode the given value.</param>
		/// <returns>Gets the value of the given string.</returns>
		/// <exception cref="FormatException">Could not convert the given string.</exception>
		public object GetObjectFromString(string toConvert, bool decodeValue)
		{
			if (toConvert == null || toConvert.Length == 0)
				return null;

			if (!IsTypeOf(toConvert))
				throw new FormatException("Could not convert the given string.");

			object convetedValue;

			try
			{
				convetedValue = GetValueFromString(toConvert, decodeValue);
			}
			catch (Exception e)
			{
				throw new FormatException("Could not convert the given string.", e);
			}

			if (convetedValue == null || System.Array.IndexOf(ManagedTypes, convetedValue.GetType()) == -1)
				throw new FormatException("Could not convert the given string.");

			return convetedValue;
		}

		/// <summary>
		/// Creates the string representation for the current script type.
		/// </summary>
		/// <returns>Returns the string representation for the current script type.</returns>
		/// <param name="valueToConvert">Value which should be converted.</param>
		/// <param name="encodeValue">True to encode the given string representation.</param>
		/// <exception cref="ArgumentNullException">The given value contains a null reference.</exception>
		/// <exception cref="InvalidOperationException">The type of the given value could not be mapped with the type of the current ScriptType instance.</exception>
		/// <exception cref="InvalidOperationException">Error while creating the script string.</exception>
		public string GetScriptStringFromObject(object valueToConvert, bool encodeValue)
		{
			if (valueToConvert == null)
				throw new ArgumentNullException("valueToConvert", "The given value contains a null reference.");

			if (!IsManagedTypeSupported(valueToConvert.GetType()))
				throw new InvalidOperationException("The type of the given value could not be mapped with the current ScriptType instance.");

			try
			{
				return GetStringRepresentation(valueToConvert, encodeValue);
			}
			catch (Exception e)
			{
				throw new InvalidOperationException("Error while creating the script string.", e);
			}
		}

		/// <summary>
		/// Creates a JavaScript string representation of the specified value.
		/// </summary>
		/// <returns>Gets the JavaScript string representation of the specified value.</returns>
		/// <param name="valueToConvert">Value which should be converted.</param>
		/// <param name="encodeValue">True to encode the given string representation.</param>
		protected abstract string GetStringRepresentation(object valueToConvert, bool encodeValue);


		/// <summary>
		/// Converts the given string into the type given by the ManagedType.
		/// </summary>
		/// <param name="valueToConvert">String which should be converted.</param>
		/// <param name="decodeValue">True to decode the given value.</param>
		/// <returns>Gets the value of the given string.</returns>
		protected abstract object GetValueFromString(string valueToConvert, bool decodeValue);

		private bool IsManagedTypeSupported(Type toCheck)
		{
			foreach (Type managedType in ManagedTypes)
			{
				if (managedType.IsAssignableFrom(toCheck))
					return true;
			}
			return false;
		}
	}
}

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

namespace JSTools.Util.Serialization
{
	/// <summary>
	/// Represents a script tree deserializer. The MoveIntoObject method
	/// is called for each object to create. After creating an object,
	/// the SetPropertyValue method is called to assign the values.
	/// After the object is filled, the MoveBack method is called.
	/// </summary>
	internal interface IScriptDeserializer
	{
		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Returns the deserialized object.
		/// </summary>
		object DeserializedObject
		{
			get;
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Determines that the object associated with the specified property
		/// name will be deserialized. (nestlevel +)
		/// </summary>
		/// <param name="propertyName">Name of the property to deserialize.</param>
		/// <param name="isArray">True if the object represents an array.</param>
		void MoveIntoObject(string propertyName, bool isArray);

		/// <summary>
		/// Determines that the current object is deserialized. (nestlevel -)
		/// </summary>
		void MoveBack();

		/// <summary>
		/// Sets the value of the specified property.
		/// </summary>
		/// <param name="propertyName">Name of the property to set.</param>
		/// <param name="value">Value of the property to set.</param>
		void SetPropertyValue(string propertyName, string value);
	}
}

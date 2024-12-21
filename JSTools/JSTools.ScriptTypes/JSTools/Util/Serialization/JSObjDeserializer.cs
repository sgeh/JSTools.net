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

using JSTools.ScriptTypes;

namespace JSTools.Util.Serialization
{
	/// <summary>
	/// Represents a script tree deserializer. This serializer creates new
	/// JSScriptObject and JSScriptArray instances.
	/// </summary>
	internal class JSObjDeserializer : IScriptDeserializer
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private JSScriptObject _deserializedObject = null;
		private ArrayList _objectStack = new ArrayList();
		private bool _decodeValues = false;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Returns the deserialized object.
		/// </summary>
		public object DeserializedObject
		{
			get { return _deserializedObject; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSObjDeserializer instance.
		/// </summary>
		/// <param name="decodeValues">True if the values should be decoded.</param>
		internal JSObjDeserializer(bool decodeValues)
		{
			_decodeValues = decodeValues;
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
		public void MoveIntoObject(string propertyName, bool isArray)
		{
			JSScriptObject newObject = CreateObject(isArray);

			// if is top object
			if (propertyName == null)
				_deserializedObject = newObject;
			else
				((JSScriptObject)_objectStack[_objectStack.Count - 1])[propertyName] = newObject;

			_objectStack.Add(newObject);
		}

		/// <summary>
		/// Determines that the current object is deserialized. (nestlevel -)
		/// </summary>
		public void MoveBack()
		{
			_objectStack.RemoveAt(_objectStack.Count - 1);
		}

		/// <summary>
		/// Sets the value of the specified property.
		/// </summary>
		/// <param name="propertyName">Name of the property to set.</param>
		/// <param name="value">Value of the property to set.</param>
		public void SetPropertyValue(string propertyName, string value)
		{
			ScriptValue deserializedValue = new ScriptValue(value, _decodeValues);
			JSScriptObject ownerObject = ((JSScriptObject)_objectStack[_objectStack.Count - 1]);

			ownerObject[propertyName] = deserializedValue.Value;
		}

		private JSScriptObject CreateObject(bool isArray)
		{
			if (isArray)
				return new JSScriptArray();
			else
				return new JSScriptObject();
		}
	}
}

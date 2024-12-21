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
using System.Collections;
using System.Reflection;

using JSTools.ScriptTypes;

namespace JSTools.Util.Serialization
{
	/// <summary>
	/// Represents a script tree deserializer. This serializer fills the
	/// specified object.
	/// </summary>
	internal class CustomObjDeserializer : IScriptDeserializer
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private object _objToFill = null;
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
			get { return _objToFill; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new CustomObjDeserializer instance.
		/// </summary>
		/// <param name="toFill">Object which should be filled.</param>
		/// <param name="decodeValues">True if the values should be decoded.</param>
		internal CustomObjDeserializer(object toFill, bool decodeValues)
		{
			_objToFill = toFill;
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
			object currentObject = null;

			// if is top object
			if (propertyName == null)
				currentObject = _objToFill;
			else
				currentObject = GetObjectToFill(propertyName);

			// init array status
			if (isArray && (!(currentObject is IList) || ((IList)currentObject).IsReadOnly))
				currentObject = null;

			// add found object to stack
			_objectStack.Add(currentObject);
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
			bool isArray = (_objectStack[_objectStack.Count - 1] is IList);

			if (isArray)
				FillArray(propertyName, deserializedValue.Value);
			else
				FillObject(propertyName, deserializedValue.Value);
		}

		private void FillArray(string propertyName, object value)
		{
			IList parentObject = (_objectStack[_objectStack.Count - 1] as IList);

			if (parentObject != null)
			{
				int valueIndex = StringToNumber(propertyName);

				// check if given index is valid
				if (parentObject.Count > valueIndex)
					parentObject[valueIndex] = value;
				else if (parentObject.Count == valueIndex)
					parentObject.Add(value);
			}
		}

		private void FillObject(string name, object value)
		{
			object parentObject = _objectStack[_objectStack.Count - 1];
			
			if (CanDeserializeProperty(parentObject, name, false, true))
			{
				PropertyInfo prop = parentObject.GetType().GetProperty(name);
				prop.GetSetMethod().Invoke(parentObject, new object[] { value } );
			}
		}

		private object GetObjectToFill(string propertyName)
		{
			object parentObject = _objectStack[_objectStack.Count - 1];
		
			if (parentObject is IList)
				return GetArrayIndex((IList)parentObject, propertyName);
			else
				return GetPropertyValue(parentObject, propertyName);
		}

		private object GetArrayIndex(IList container, string index)
		{
			int propIndex = StringToNumber(index);

			if (propIndex > -1 && propIndex < container.Count)
				return container[propIndex];

			return null;
		}

		private object GetPropertyValue(object container, string propertyName)
		{
			if (CanDeserializeProperty(container, propertyName, true, false))
			{
				PropertyInfo prop = container.GetType().GetProperty(propertyName);
				return prop.GetGetMethod().Invoke(container, new object[0]);
			}
			return null;
		}

		private bool CanDeserializeProperty(object toCheck, string propertyName, bool requiresGetMethod, bool requiresSetMethod)
		{
			if (toCheck == null)
				return false;

			PropertyInfo objToFillProp = toCheck.GetType().GetProperty(propertyName);

			if (objToFillProp != null)
			{
				object[] valueTypeAttrib = objToFillProp.GetCustomAttributes(typeof(ScriptValueTypeAttribute), true);

				if (valueTypeAttrib != null && valueTypeAttrib.Length != 0)
				{
					bool propValid = true;

					if (requiresGetMethod)
						propValid = (objToFillProp.GetGetMethod(false) != null);
					else if (requiresSetMethod)
						propValid = (objToFillProp.GetSetMethod(false) != null);

					return propValid;
				}
			}
			return false;
		}

		private int StringToNumber(string toConvert)
		{
			try
			{
				return Convert.ToInt32(toConvert);
			}
			catch
			{
				return -1;
			}
		}
	}
}

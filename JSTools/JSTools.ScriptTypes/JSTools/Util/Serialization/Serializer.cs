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
using System.Text;

using JSTools.ScriptTypes;

namespace JSTools.Util.Serialization
{
	/// <summary>
	/// Represents the object serializer.
	/// </summary>
	internal class Serializer
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private readonly Type ARRAY_TYPE = typeof(IList);

		private const string NAME_VALUE_PAIR = "{0}:{1},";
		private const char VALUE_SEPARATOR = ',';
		private const char ARRAY_BEGIN = '[';
		private const char ARRAY_END = ']';
		private const char OBJECT_BEGIN = '{';
		private const char OBJECT_END = '}';

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new Serializer instance.
		/// </summary>
		internal Serializer()
		{
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		internal string Serialize(object toSerialize, bool encodeValues)
		{
			string serializedValue = null;

			if (toSerialize != null)
				serializedValue = SerializeValue(toSerialize, encodeValues, new ArrayList());

			return (serializedValue != null) ? serializedValue : string.Empty;
		}

		private string SerializeValue(object toSerialize, bool encodeValues, ArrayList serializedObjects)
		{
			ScriptValue scriptValue = new ScriptValue(toSerialize, encodeValues);

			if (IsObject(scriptValue))
				return SerializeInstance(scriptValue, encodeValues, (ArrayList)serializedObjects.Clone());
			else
				return scriptValue.ScriptStringValue;
		}

		private string SerializeInstance(ScriptValue scriptValue, bool encodeValues, ArrayList serializedObjects)
		{
			// do not serialize the same object twice
			if (!serializedObjects.Contains(scriptValue.Value))
			{
				serializedObjects.Add(scriptValue.Value);

				if (IsArray(scriptValue))
					return SerializeArray(scriptValue.Value, encodeValues, serializedObjects);
				else
					return SerializeObject(scriptValue.Value, encodeValues, serializedObjects);
			}
			return null;
		}

		private string SerializeObject(object toSerialize, bool encodeValues, ArrayList serializedObjects)
		{
			StringBuilder objDeclaration = new StringBuilder();
			ArrayList propertiesToSerialize = GetPropertiesToSerialize(toSerialize);

			#region Loop throught the properties to serialize.

			for (int i = 0; i < propertiesToSerialize.Count; ++i)
			{
				#region Serialize property value.

				PropertyInfo propertyToSerialize = (PropertyInfo)propertiesToSerialize[i];
				MethodInfo getMethod = propertyToSerialize.GetGetMethod(false);

				if (getMethod != null)
				{
					// get value to serialize
					string serializedValue = SerializeValue(
						getMethod.Invoke(toSerialize, null),
						encodeValues,
						serializedObjects );

					#region Append serialized value to result string.

					if (serializedValue != null)
					{
						objDeclaration.Append(string.Format(
							NAME_VALUE_PAIR,
							propertyToSerialize.Name,
							serializedValue ));
					}

					#endregion
				}

				#endregion
			}

			#endregion

			return OBJECT_BEGIN
				+ ((objDeclaration.Length != 0) ? objDeclaration.ToString(0, objDeclaration.Length - 1) : string.Empty)
				+ OBJECT_END;
		}

		private string SerializeArray(object valueToConvert, bool encodeValues, ArrayList serializedObjects)
		{
			StringBuilder arrayDeclaration = new StringBuilder();

			for (int i = 0; i < ((IList)valueToConvert).Count; ++i)
			{
				// get value to serialize
				string serializedValue = SerializeValue(
					((IList)valueToConvert)[i],
					encodeValues,
					serializedObjects );

				if (serializedValue != null)
				{
					arrayDeclaration.Append(serializedValue);
					arrayDeclaration.Append(VALUE_SEPARATOR);
				}
			}

			return ARRAY_BEGIN
				+ ((arrayDeclaration.Length != 0) ? arrayDeclaration.ToString(0, arrayDeclaration.Length - 1) : string.Empty)
				+ ARRAY_END;
		}

		private ArrayList GetPropertiesToSerialize(object valueToConvert)
		{
			PropertyInfo[] properties = valueToConvert.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
			ArrayList propertiesToSerialize = new ArrayList(properties.Length);

			foreach (PropertyInfo propInfo in properties)
			{
				object[] attributes = propInfo.GetCustomAttributes(typeof(ScriptValueTypeAttribute), true);

				if (attributes != null && attributes.Length > 0)
					propertiesToSerialize.Add(propInfo);
			}
			return propertiesToSerialize;
		}

		private bool IsObject(ScriptValue valueToCheck)
		{
			return (valueToCheck.ScriptType != null && valueToCheck.ScriptType.GetType() == typeof(JSTools.ScriptTypes.Object));
		}

		private bool IsArray(ScriptValue valueToCheck)
		{
			return (valueToCheck.Value != null && ARRAY_TYPE.IsAssignableFrom(valueToCheck.Value.GetType()));
		}
	}
}

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
using System.Reflection;
using System.Text;

using JSTools.ScriptTypes;

namespace JSTools.Util
{
	/// <summary>
	/// 
	/// </summary>
	public class SimpleObjectSerializer
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const string NAME_VALUE_PAIR = "{0}:{1}";
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
		/// Creates a new SimpleObjectSerializer instance.
		/// </summary>
		public SimpleObjectSerializer()
		{
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// 
		/// </summary>
		/// <param name="toDeserialze"></param>
		/// <returns></returns>
		public object Deserialize(string toDeserialze)
		{
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="toDeserialze"></param>
		/// <param name="decodeValues"></param>
		/// <returns></returns>
		public object Deserialize(string toDeserialze, bool decodeValues)
		{
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="toDeserialze"></param>
		/// <param name="toFill"></param>
		/// <returns></returns>
		public object Deserialize(string toDeserialze, object toFill)
		{
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="toDeserialze"></param>
		/// <param name="toFill"></param>
		/// <param name="decodeValues"></param>
		/// <returns></returns>
		public object Deserialize(string toDeserialze, object toFill, bool decodeValues)
		{
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="toSerialze"></param>
		/// <returns></returns>
		public string Serialize(object toSerialze)
		{
			return Serialize(toSerialze, false);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="toSerialze"></param>
		/// <param name="encodeValues"></param>
		/// <returns></returns>
		public string Serialize(object toSerialze, bool encodeValues)
		{
			string serializedValue = null;

			if (toSerialze != null)
				serializedValue = SerializeValue(toSerialze, encodeValues, new ArrayList());

			return (serializedValue != null) ? serializedValue : string.Empty;
		}

		private string SerializeValue(object toSerialze, bool encodeValues, ArrayList serializedObjects)
		{
			ScriptValue scriptValue = new ScriptValue(toSerialze, encodeValues);

			if (IsObject(scriptValue.ScriptType))
			{
				return SerializeInstance(scriptValue, encodeValues, (ArrayList)serializedObjects.Clone());
			}
			else
			{
				return scriptValue.ScriptStringValue;
			}
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

		private string SerializeObject(object toSerialze, bool encodeValues, ArrayList serializedObjects)
		{
			StringBuilder objDeclaration = new StringBuilder(OBJECT_BEGIN);
			ArrayList propertiesToSerialize = GetPropertiesToSerialize(toSerialze);

			for (int i = 0; i < propertiesToSerialize.Count; ++i)
			{
				PropertyInfo propertyToSerialize = (PropertyInfo)propertiesToSerialize[i];
				MethodInfo getMethod = propertyToSerialize.GetGetMethod(false);

				if (getMethod != null)
				{
					// get value to serialize
					string serializedValue = SerializeValue(
						getMethod.Invoke(toSerialze, null),
						encodeValues,
						serializedObjects );

					if (serializedValue != null)
					{
						objDeclaration.Append(string.Format(
							NAME_VALUE_PAIR,
							propertyToSerialize.Name,
							serializedValue ));

						if (i + 1 != propertiesToSerialize.Count)
							objDeclaration.Append(VALUE_SEPARATOR);
					}
				}
			}

			objDeclaration.Append(OBJECT_END);
			return objDeclaration.ToString();
		}

		private string SerializeArray(object valueToConvert, bool encodeValues, ArrayList serializedObjects)
		{
			StringBuilder arrayDeclaration = new StringBuilder(ARRAY_BEGIN);

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

					if (i + 1 != ((IList)valueToConvert).Count)
						arrayDeclaration.Append(VALUE_SEPARATOR);
				}
			}

			arrayDeclaration.Append(ARRAY_END);
			return arrayDeclaration.ToString();
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

		private bool IsObject(AScriptType typeToCheck)
		{
			return (typeToCheck.GetType() == typeof(JSTools.ScriptTypes.Object));
		}

		private bool IsArray(ScriptValue valueToCheck)
		{
			return (valueToCheck.Value != null && valueToCheck.Value.GetType() == typeof(IList));
		}
	}
}

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

using JSTools.Util;

namespace JSTools.ScriptTypes
{
	/// <summary>
	/// Represents a client script value converter. This class can be used
	/// to serialize managed objects into a client script string or to
	/// deserialize a client script string into a managed object.
	/// </summary>
	public class ScriptValue
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const string NULL_VALUE = "null";

		private ScriptValueMapper _mapper = null;
		private AScriptType _scriptType = null;
		private object _value = null;
		private string _scriptStringValue = null;

		private bool _encodeValue = false;
		private bool _decodeValue = true;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// True to encode the converted value. This feature is not supported
		/// by all ScriptTypes. Default value is false.
		/// </summary>
		public bool EncodeValue
		{
			get { return _encodeValue; }
			set { _encodeValue = value; }
		}

		/// <summary>
		/// True to decode the specified value. This feature is not supported
		/// by all ScriptTypes. Default value is true.
		/// </summary>
		public bool DecodeValue
		{
			get { return _decodeValue; }
			set { _decodeValue = value; }
		}

		/// <summary>
		/// Sets the managed value, which should be converted into a
		/// javascript compatible string.
		/// </summary>
		public object Value
		{
			get
			{
				if (_value == null && _scriptType != null)
					_value = _scriptType.GetObjectFromString(_scriptStringValue, DecodeValue);

				return _value;
			}
			set
			{
				_value = value;
				_scriptStringValue = null;

				if (_value != null)
					_scriptType = Mapper.MapType(_value.GetType());
				else
					_scriptType = null;
			}
		}

		/// <summary>
		/// Gets/sets the script string which may be used to render
		/// javascript compatible client script.
		/// </summary>
		public string ScriptStringValue
		{
			get
			{
				if (_scriptStringValue == null && _scriptType != null)
					_scriptStringValue = _scriptType.GetScriptStringFromObject(_value, EncodeValue);

				return (_scriptStringValue != null) ? _scriptStringValue : NULL_VALUE;
			}
			set
			{
				_value = null;
				_scriptStringValue = value;

				if (_scriptStringValue != null && _scriptStringValue != NULL_VALUE)
					_scriptType = Mapper.MapValue(_scriptStringValue);
				else
					_scriptType = null;
			}
		}

		/// <summary>
		/// Gets the mapped script type.
		/// </summary>
		public AScriptType ScriptType
		{
			get { return _scriptType; }
		}

		private ScriptValueMapper Mapper
		{
			get 
			{
				if (_mapper == null)
					_mapper = CreateValueMapper();

				return _mapper;
			}
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new ScriptValue instance.
		/// </summary>
		/// <param name="valueToConvert">Value to convert into a JavaScript string.</param>
		public ScriptValue(object valueToConvert)
		{
			Value = valueToConvert;
		}

		/// <summary>
		/// Creates a new ScriptValue instance.
		/// </summary>
		/// <param name="valueToConvert">Value to convert into a JavaScript string.</param>
		/// <param name="encodeValue">True to encode the converted string value. Default value is false.</param>
		public ScriptValue(object valueToConvert, bool encodeValue)
		{
			EncodeValue = encodeValue;
			Value = valueToConvert;
		}

		/// <summary>
		/// Creates a new ScriptValue instance.
		/// </summary>
		/// <param name="valueToDeserialialize">Value which should be deserialized.</param>
		/// <param name="decodeValue">True to decode the values to serialize.</param>
		public ScriptValue(string valueToDeserialialize, bool decodeValue)
		{
			DecodeValue = decodeValue;
			ScriptStringValue = valueToDeserialialize;
		}

		/// <summary>
		/// Creates a new ScriptValue instance.
		/// </summary>
		public ScriptValue()
		{
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Converts the given value into a javascript compatible string format.
		/// </summary>
		/// <returns>Returns the script value of the given object to convert.</returns>
		public override string ToString()
		{
			return ScriptStringValue;
		}

		/// <summary>
		/// Creates a new ScriptValueMapper instance.
		/// </summary>
		/// <returns>Returns the created instance.</returns>
		protected virtual ScriptValueMapper CreateValueMapper()
		{
			return new ScriptValueMapper();
		}
	}
}

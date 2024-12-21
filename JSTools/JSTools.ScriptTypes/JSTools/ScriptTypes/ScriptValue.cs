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
	public class ScriptValue
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

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
		/// True to encode the converted value. This feature is not available
		/// on all ScriptTypes. Default value is false.
		/// </summary>
		public bool EncodeValue
		{
			get { return _encodeValue; }
			set { _encodeValue = value; }
		}

		/// <summary>
		/// True to decode the specified value. This feature is not available
		/// on all ScriptTypes. Default value is true.
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
			get { return _value; }
			set
			{
				if (value != null)
				{
					_value = value;
					_scriptType = Mapper.MapType(value);
					_scriptStringValue = _scriptType.GetScriptStringFromObject(value, EncodeValue);
				}
			}
		}

		/// <summary>
		/// Gets/sets the script string which may be used to render
		/// javascript compatible client script.
		/// </summary>
		public string ScriptStringValue
		{
			get { return (_scriptStringValue != null) ? _scriptStringValue : string.Empty; }
			set
			{
				if (value != null)
				{
					_scriptStringValue = value;
					_scriptType = Mapper.MapValue(value);
					_value = _scriptType.GetObjectFromString(value, DecodeValue);
				}
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

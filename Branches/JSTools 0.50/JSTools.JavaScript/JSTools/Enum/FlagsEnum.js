/*
 * JSTools.JavaScript / JSTools.net - A JavaScript/C# framework.
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

namespace("JSTools.Enum");

using("JSTools");


/// <class>
/// Represents an enumeration, which values can be used to create binary comparsions.
/// </class>
/// <param name="params">String values, which represent the enum members.</param>
JSTools.Enum.FlagsEnum = function()
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	var _this = this;
	var _arguments = arguments;
	var _registeredValues = new Array();
	var _registeredNames = new Array();


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------

	/// <constructor>
	/// Creates a new JSTools.Enum.FlagsEnum instance.
	/// </constructor>
	function Init()
	{
		var flagsBegin = 0x01;

		// writes the given arguments into the _registeredValues array and the this object
		for (var i = 0; i < _arguments.length; ++i)
		{
			var enumValue = (flagsBegin <<= 1);
			var enumName = String(_arguments[i]);

			_registeredValues.Add(enumValue);
			_registeredNames.Add(enumName);
			_this[enumName] = enumValue;
		}
	}


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Creates a new array and copies the enum values.
	/// </method>
	/// <returns type="Array">Returns all values, which are stored in this enum.</returns>
	function GetValues()
	{
		return _registeredValues.Copy();
	}
	this.GetValues = GetValues; 


	/// <method>
	/// Creates a new array and copies the given enum names.
	/// </method>
	/// <returns type="Array">Returns all names, which are stored in this enum.</returns>
	function GetNames()
	{
		return _registeredNames.Copy();
	}
	this.GetNames = GetNames;


	/// <method>
	/// Searches for a name, which has the given value.
	/// </method>
	/// <param type="Integer" name="intValue">Value of the expected name.</param>
	/// <returns type="String">Returns the name of the requested value.</returns>
	function GetName(intValue)
	{
		var valueIndex = _registeredValues.IndexOf(intValue);
		
		if (valueIndex == -1)
			return null;
		
		var name = _registeredNames[valueIndex];
		
		if (typeof(name) == 'undefined')
		{
			return null;
		}
		else
		{
			return name;
		}
	}
	this.GetName = GetName;


	/// <method>
	/// Searches for a name, which has the given value.
	/// </method>
	/// <param type="Integer" name="intValue">Value of the expected name.</param>
	/// <returns type="String">Returns the name of the requested value.</returns>
	function ToString()
	{
		var enumString = "[enum";

		if (_registeredNames.length > 0)
		{
			enumString += " " + _registeredNames.toString();
		}
		return enumString + "]";
	}
	this.toString = ToString; 

	Init();
}

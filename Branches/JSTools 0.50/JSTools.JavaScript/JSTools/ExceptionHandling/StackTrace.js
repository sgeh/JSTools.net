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

namespace("JSTools.ExceptionHandling");


/// <class>
/// Provides informations about the call stack. This feature is not available in all browsers.
/// Mozilla 1.x will stop executing the current script (except 1.4), if you create an instance of
/// this class. Opera x.x/Surfin'Safari does not provide a Funciton.caller property, thus this
/// class will not return a valid call stack representation.
///
/// Caution:
/// This class uses deprecated functionalities (Function.caller).
/// </class>
JSTools.ExceptionHandling.StackTrace = function()
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	this.InitType(arguments, "JSTools.ExceptionHandling.StackTrace");

	var _this = this;
	var _methods = [ ];
	var _callee = arguments.callee;
	var _isAvailable = true;


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------

	/// <constructor>
	/// Creates a new JSTools.ExceptionHandling.StackTrace instance.
	/// </constructor>
	function Init()
	{
		if (typeof(_callee) != 'function' || !arguments.caller)
		{
			_isAvailable = false;
			return;
		}

		var functionObject = _callee.caller;

		while (typeof(functionObject) == 'function')
		{
			_methods.Add(functionObject);
			functionObject = eval("functionObject.caller");
		}
	}


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Checks whether the current browser supports reflection of the call stack.
	/// </method>
	/// <returns type="Boolean">Returns true if the current browser supports reflection of the call stack.</returns>
	function IsAvailable()
	{
		return _isAvailable;
	}
	this.IsAvailable = IsAvailable;


	/// <method>
	/// Gets the count of methods in the call stack.
	/// </method>
	/// <returns type="Integer">Returns the count of methods in the call stack.</returns>
	function GetMethodCount()
	{
		return _methods.length;
	}
	this.GetMethodCount = GetMethodCount;


	/// <method>
	/// Searches for the method at the given index.
	/// </method>
	/// <returns type="Function">Returns a function object if a function at the given index exist.
	/// Ohterwise you will obtian a null reference.</returns>
	function GetMethod(intIndex)
	{
		if (typeof(intIndex) != 'number' || isNaN(intIndex))
			return null;

		if (intIndex > -1 && _methods.length > intIndex)
		{
			return _methods[intIndex];
		}
		return null;
	}
	this.GetMethod = GetMethod;


	/// <method>
	/// Returns a string representation of this call stack.
	/// </method>
	function ToString()
	{
		var stackTrace = String.Empty;

		for (var i = 0; i < _methods.length; ++i)
		{
			if (_methods[i].GetName() == String.Empty)
				stackTrace += "[ function(...) ]\n";
			else
				stackTrace += _methods[i].GetName() + "(...)\n";
		}
		return stackTrace;
	}
	this.toString = ToString;

	Init();
}

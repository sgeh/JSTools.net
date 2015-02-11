/*
 * JSTools.Parser.DocGenerator.dll / JSTools.net - A framework for JavaScript/ASP.NET applications.
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

// ------------------------------------------------------------------
// OBJECT OBJECT
// ------------------------------------------------------------------

/// <class>
/// When Object is called as part of a new expression, it is a constructor that may create an object.
/// </class>
/// <param name="value" type="Object">
/// When the Object function is called with no arguments or with one argument value, the following
/// steps are taken:
/// 
/// <para>
///  1. If value is null, undefined or not supplied, create and return a new Object object exactly if the
///  object constructor had been called with the same arguments (15.2.2.1).
/// </para>
/// <para>
///  2. Return ToObject(value).
/// </para>
/// </param>
function Object(value)
{
	/// <variable type="Object">
	/// The initial value of Object.prototype is the Object prototype object.
	/// </variable>
	this.prototype = null;

	/// <variable type="Function">
	/// The initial value of Object.prototype.constructor is the built-in Object constructor.
	/// </variable>
	this.constructor = null;

	/// <function>
	/// Compute a string value by concatenating the three strings "[object " + [[Class]] + "]".
	/// </function>	
	/// <returns type="String"></returns>
	this.toString = function() { }
	
	/// <function>
	/// This function returns the result of calling toString().
	/// </function>	
	/// <remarks>
	/// This function is provided to give all Objects a generic toLocaleString interface, even though not
	/// all may use it. Currently, Array, Number, and Date provide their own locale-sensitive
	/// toLocaleString methods.
	/// </remarks>
	/// <returns type="String"></returns>
	this.toLocaleString = function() { }
	
	/// <function>
	/// The valueOf method returns its this value. If the object is the result of calling the Object
	/// constructor with a host object, it is implementation-defined whether valueOf returns its
	/// this value or another value such as the host object originally passed to the constructor.
	/// </function>	
	/// <returns type="Object"></returns>
	this.valueOf = function() { }
}

Function ( . . . )
See 15.3.1 and 15.3.2.
Array ( . . . )
See 15.4.1 and 15.4.2.
String ( . . . )
See 15.5.1 and 15.5.2.
Boolean ( . . . )
See 15.6.1 and 15.6.2.
Number ( . . . )
See 15.7.1 and 15.7.2.
Date ( . . . )
See 15.9.2.
RegExp ( . . . )
See 15.10.3 and 15.10.4.
Math ( . . . )   	 	 	
Error ( . . . )

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

namespace("JSTools.Timers");


/// <class>
/// A Timeout is used to raise one tick event at an user-defined timeout.
///
/// There are two different methods, which attach a function object
/// to the timeout event.
///
/// AddTimeoutMethodInfo
///  Attaches a method to the timeout event. The call will be executed in
///  the creation context (this pointer) of the object.
/// AddTimeoutFunction
///  Attaches a method to the timeout event. The call will be executed in
///  the global context (this pointer).
/// </class>
JSTools.Timers.Timeout = function()
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	this.InitType(arguments, "JSTools.Timers.Timeout");

	var DEFAULT_TIMEOUT = 20;

	var _this = this;
	var _timer = new JSTools.Timers.Timer();


	/// <property type="Integer">
	/// Gets or sets the timeout (milliseconds) at which to call the Timeout event. Default is 20ms.
	/// </property>
	this.Timeout = DEFAULT_TIMEOUT;


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------

	/// <constructor>
	/// Creates a new JSTools.Timers.Timeout instance.
	/// </constructor>
	function Init()
	{
		_this.AddTimeoutFunction(AutoStop);
	}


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// The returning array is passed as event argument to the tick method. This values are
	/// shared between your code and the reciever functions.
	/// </method>
	/// <returns type="Array">Returns an array, which is passed as event argument to the tick method.</returns>
	function GetArguments()
	{
		return _timer.GetArguments();
	}
	this.GetArguments = GetArguments;


	/// <method>
	/// Attaches the given method to this timer. It will be called by each tick.
	/// </method>
	/// <param name="objMethodInfoToAdd" type="JSTools.Reflection.MethodInfo">Attaches the given Function or MethodInfo object.
	/// The given method will be called by each tick.</param>
	function AddTimeoutMethodInfo(objMethodInfoToAdd)
	{
		return _timer.AddTickMethodInfo(objMethodInfoToAdd);
	}
	this.AddTimeoutMethodInfo = AddTimeoutMethodInfo;


	/// <method>
	/// Attaches the given method to this timer. It will be called by each tick.
	/// </method>
	/// <param name="objFunction" type="Function">Attaches the given Function or MethodInfo object.
	/// The given method will be called by each tick.</param>
	function AddTimeoutFunction(objFunction)
	{
		return _timer.AddTickFunction(objFunction);
	}
	this.AddTimeoutFunction = AddTimeoutFunction;


	/// <method>
	/// Returns true, if the timer is enabled.
	/// </method>
	function IsEnabled()
	{
		return _timer.IsEnabled();
	}
	this.IsEnabled = IsEnabled;


	/// <method>
	/// Stops the timer interval and sets the IsEnabled() flag to false.
	/// </method>
	function Stop()
	{
		_timer.Stop();
	}
	this.Stop = Stop;


	/// <method>
	/// Starts the timer interval and sets the IsEnabled() flag to true. If the 
	/// </method>
	function Start()
	{
		_timer.Interval = !isNaN(_this.Timeout) ? Number(_this.Timeout) : DEFAULT_TIMEOUT;
		_timer.Start();
	}
	this.Start = Start;


	/// <method>
	/// Stops the interval after the first tick event.
	/// </method>
	function AutoStop()
	{
		_timer.Stop();
	}
	Init();
}

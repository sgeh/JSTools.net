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
/// Stores and handles the error, log and warn events. To attach events, you can use
/// AddOn...Event(...) procedures. To remove events, use the corresponding
/// RemoveOn...Event(...) method.
///
/// Do not overwrite window.onerror, use JSTools.Exception.AddOnErrorEvent instead.
/// </class>
JSTools.ExceptionHandling.Handler = function()
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	this.InitType(arguments, "JSTools.ExceptionHandling.Handler");

	var LOG_EVENT_NAME = "onlog";
	var ERROR_EVENT_NAME = "onerror";
	var WARN_EVENT_NAME = "onwarn";

	var _this = this;
	var _events = new JSTools.Event.SubjectList();

	var _logs = [ ];
	var _errors = [ ];
	var _warnings = [ ];


	/// <property type="JSTools.ExceptionHandling.ErrorEvent">
	/// Represents a bit mask used to filter the events.
	/// </property>
	this.EventHandling = JSTools.ExceptionHandling.ErrorEvent.All;


	/// <property type="JSTools.ExceptionHandling.ErrorHandling">
	/// Determines wheter the exception should be thrown or catched.
	/// If the exception should be thrown, the public Log, Thorw and Warn
	/// methods will return false, otherwise true.
	/// </property>
	this.ErrorHandling = JSTools.ExceptionHandling.ErrorHandling.None;


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------

	/// <constructor>
	/// Creates a new JSTools.ExceptionHandling.Handler instance.
	/// </constructor>
	function Init()
	{
	}


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Creates a new Array instance, which contains the logging messages.
	/// </method>
	/// <returns type="Array">Returns an array which contains the given log
	/// (JSTools.ExceptionHandling.Exception) instances.</returns>
	function GetLogs()
	{
		return _logs.Copy();
	}
	this.GetLogs = GetLogs;


	/// <method>
	/// Creates a new Array instance, which contains the error messages.
	/// </method>
	/// <returns type="Array">Returns an array which contains the given error
	/// (JSTools.ExceptionHandling.Exception) instances.</returns>
	function GetErrors()
	{
		return _errors.Copy();
	}
	this.GetErrors = GetErrors;


	/// <method>
	/// Creates a new Array instance, which contains the warning messages.
	/// </method>
	/// <returns type="Array">Returns an array which contains the given warning
	/// (JSTools.ExceptionHandling.Exception) instances.</returns>
	function GetWarnings()
	{
		return _warnings.Copy();
	}
	this.GetWarnings = GetWarnings;


	/// <method>
	/// Adds a new observer to the OnLog subject (event).
	/// </method>
	/// <param name="varLogObserver" type="Function">Adds the given function object to the OnLog event.</param>
	/// <param name="varLogObserver" type="JSTools.Reflection.MethodInfo">Adds the given MethodInfo object to the OnLog event.</param>
	/// <param name="varLogObserver" type="JSTools.Event.IObserver">Adds the given IObserver object to the OnLog event.</param>
	/// <returns type="Integer">Returns the index, at which the observer object has been added.
	/// Returns -1 if the given observer object is invalid and not added.</returns>
	function AddOnLogEvent(varLogObserver)
	{
		return _events.Attach(LOG_EVENT_NAME, varLogObserver);
	}
	this.AddOnLogEvent = AddOnLogEvent;


	/// <method>
	/// Removes an observer from the OnLog subject (event).
	/// </method>
	/// <param name="varObserverToDetach" type="JSTools.Event.IObserver">Observer object which should be removed.</param>
	/// <param name="varObserverToDetach" type="Integer">Internal index of the observer object which should be removed.</param>
	function RemoveOnLogEvent(varObserverToDetach)
	{
		_events.Detach(LOG_EVENT_NAME, varObserverToDetach);
	}
	this.RemoveOnLogEvent = RemoveOnLogEvent;


	/// <method>
	/// Adds a new observer to the OnError subject (event).
	/// </method>
	/// <param name="varLogObserver" type="Function">Adds the given function object to the OnError event.</param>
	/// <param name="varLogObserver" type="JSTools.Reflection.MethodInfo">Adds the given MethodInfo object to the OnError event.</param>
	/// <param name="varLogObserver" type="JSTools.Event.IObserver">Adds the given IObserver object to the OnError event.</param>
	/// <returns type="Integer">Returns the index, at which the observer object has been added.
	/// Returns -1 if the given observer object is invalid and not added.</returns>
	function AddOnErrorEvent(varLogObserver)
	{
		return _events.Attach(ERROR_EVENT_NAME, varLogObserver);
	}
	this.AddOnErrorEvent = AddOnErrorEvent;


	/// <method>
	/// Removes an observer from the OnError subject (event).
	/// </method>
	/// <param name="varObserverToDetach" type="JSTools.Event.IObserver">Observer object which should be removed.</param>
	/// <param name="varObserverToDetach" type="Integer">Internal index of the observer object which should be removed.</param>
	function RemoveOnErrorEvent(varObserverToDetach)
	{
		_events.Detach(ERROR_EVENT_NAME, varObserverToDetach);
	}
	this.RemoveOnErrorEvent = RemoveOnErrorEvent;


	/// <method>
	/// Adds a new observer to the OnWarn subject (event).
	/// </method>
	/// <param name="varLogObserver" type="Function">Adds the given function object to the OnWarn event.</param>
	/// <param name="varLogObserver" type="JSTools.Reflection.MethodInfo">Adds the given MethodInfo object to the OnWarn event.</param>
	/// <param name="varLogObserver" type="JSTools.Event.IObserver">Adds the given IObserver object to the OnWarn event.</param>
	/// <returns type="Integer">Returns the index, at which the observer object has been added.
	/// Returns -1 if the given observer object is invalid and not added.</returns>
	function AddOnWarnEvent(varLogObserver)
	{
		return _events.Attach(WARN_EVENT_NAME, varLogObserver);
	}
	this.AddOnWarnEvent = AddOnWarnEvent;


	/// <method>
	/// Removes an observer from the OnWarn subject (event).
	/// </method>
	/// <param name="varObserverToDetach" type="JSTools.Event.IObserver">Observer object which should be removed.</param>
	/// <param name="varObserverToDetach" type="Integer">Internal index of the observer object which should be removed.</param>
	function RemoveOnWarnEvent(varObserverToDetach)
	{
		_events.Detach(WARN_EVENT_NAME, varObserverToDetach);
	}
	this.RemoveOnWarnEvent = RemoveOnWarnEvent;


	/// <method>
	/// Creates a new log entry for the given message. If the Handling property contains the 
	/// JSTools.ExceptionHandling.ErrorEvent.Log flag the OnLog event will fire.
	/// </method>
	/// <param name="strMessage" type="String">Error message which describes this exception.</param>
	/// <param name="strFile" type="String">File which has thrown this exception.</param>
	/// <param name="intLine" type="Integer">Line number of the exception.</param>
	function Log(strMessage, strFile, intLine)
	{
		var exception = new JSTools.ExceptionHandling.Exception(
			strMessage,
			strFile,
			intLine,
			arguments.callee,
			new JSTools.ExceptionHandling.StackTrace() );

		_logs.Add(exception);
		FireEvent(LOG_EVENT_NAME, exception, JSTools.ExceptionHandling.ErrorEvent.Log);
	}
	this.Log = Log;


	/// <method>
	/// Creates a new error entry for the given error. If the Handling property contains the 
	/// JSTools.ExceptionHandling.ErrorEvent.Error flag the OnError event will fire. If the first
	/// argument contains an exception object, this function ignors the other arguments.
	/// </method>
	/// <param name="varException" type="String">Error message which describes this exception.</param>
	/// <param name="varException" type="JSTools.ExceptionHandling.Exception">Exception object to throw.</param>
	/// <param name="strFile" type="String">File which has thrown this exception.</param>
	/// <param name="intLine" type="Integer">Line number of the exception.</param>
	function Throw(varException, strFile, intLine)
	{
		var exception;

		if (varException
			&& typeof(varException) == 'object'
			&& varException.IsTypeOf(JSTools.ExceptionHandling.Exception))
		{
			exception = varException;
		}
		else
		{
			// if it is not a manually thrown error
			exception = new JSTools.ExceptionHandling.Exception(
				varException,
				strFile,
				intLine,
				arguments.callee,
				new JSTools.ExceptionHandling.StackTrace() );
		}

		// log error
		_errors.Add(exception);
		
		// fire error event
		FireEvent(ERROR_EVENT_NAME, exception, JSTools.ExceptionHandling.ErrorEvent.Error);

		// throw error & stop script execution
		if (JSTools.Browser.HasDOM())
			eval("throw exception.ToNativeError();");
		else
			eval(exception.ToNativeError().defaultMessage);
	}
	this.Throw = Throw;


	/// <method>
	/// Do not use this code from your code. It's intended to be used to handle
	/// native errors which are handled by the window.onerror event.
	/// </method>
	/// <param name="strException" type="String">Error message which describes this exception.</param>
	/// <param name="strFile" type="String">File which has thrown this exception.</param>
	/// <param name="intLine" type="Integer">Line number of the exception.</param>
	/// <returns type="Boolean">Returns true if the given error should not be visualized.</returns>
	function ThrowNative(strException, strFile, intLine)
	{
		// checks whether the given exception has not been logged yet
		if (String(strException).indexOf(JSTools.ExceptionHandling.Exception.JSTOOLS_EXCEPTION_NUMBER) == -1)
		{
			var exception = new JSTools.ExceptionHandling.Exception(
				strException,
				strFile,
				intLine,
				arguments.callee,
				new JSTools.ExceptionHandling.StackTrace());

			// log error
			_errors.Add(exception);
			
			// fire error event
			FireEvent(ERROR_EVENT_NAME, exception, JSTools.ExceptionHandling.ErrorEvent.Error);
		}
		
		// do not rethrow errors, if they should be suppressed
		return GetReturnValueFromHandling();
	}
	this.ThrowNative = ThrowNative;


	/// <method>
	/// Creates a new warning entry. If the Handling property contains the
	/// JSTools.ExceptionHandling.ErrorEvent.Warn flag the OnLog event is fired.
	/// </method>
	/// <param name="strMessage" type="String">Error message which describes this exception.</param>
	/// <param name="strFile" type="String">File which has thrown this exception.</param>
	/// <param name="intLine" type="Integer">Line number of the exception.</param>
	function Warn(strMessage, strFile, intLine)
	{
		var exception = new JSTools.ExceptionHandling.Exception(
			strMessage,
			strFile,
			intLine,
			arguments.callee,
			new JSTools.ExceptionHandling.StackTrace());

		_warnings.Add(exception);
		FireEvent(WARN_EVENT_NAME, exception, JSTools.ExceptionHandling.ErrorEvent.Warn);
	}
	this.Warn = Warn;


	/// <method>
	/// Checks whether the ErrorHandling property is equal to JSTools.ExceptionHandling.ErrorHandling.Catch.
	/// </method>
	/// <returns type="Boolean">Returns true if the given error should be suppressed.</returns>
	function GetReturnValueFromHandling()
	{
		return (_this.ErrorHandling == JSTools.ExceptionHandling.ErrorHandling.Catch);
	}


	/// <method>
	/// Fires the given event, if it is allowed.
	/// </method>
	/// <param name="strName" type="String">Event to fire.</returns>
	/// <param name="objException" type="JSTools.ExceptionHandling.Exception">Exception object to fire.</returns>
	/// <param name="enuToCheck" type="JSTools.ExceptionHandling.ErrorEvent">Event flag.</returns>
	function FireEvent(strName, objException, enuToCheck)
	{
		if (isNaN(_this.EventHandling) || _this.EventHandling == JSTools.ExceptionHandling.ErrorEvent.None)
			return;

		if ((_this.EventHandling & enuToCheck) != 0 || _this.EventHandling == JSTools.ExceptionHandling.ErrorEvent.All)
			_events.Notify(strName, objException);
	}
	Init();
}


/// <enum>
/// Represents a bit mask used to filter events.
/// </enum>
/// <field name="None">The error is stored but no event will be fired.</field>
/// <field name="All">The error is stored and all events will be fired.</field>
/// <field name="Log">The log message is stored and an OnLog event will be fired.</field>
/// <field name="Error">The error message is stored and an OnError event will be fired.</field>
/// <field name="Warn">The warning message is stored and an OnWarn event will be fired.</field>
JSTools.ExceptionHandling.ErrorEvent = new JSTools.Enum.FlagsEnum(
	"None",
	"All",
	"Log",
	"Error",
	"Warn" );


/// <enum>
/// Used to determine the error handling mode on client side.
/// </enum>
/// <field name="None">Client side error handling is disabled, this represents the standard script error behaviour.</field>
/// <field name="Catch">Client side error messages are suppressed but error handling is enabled.</field>
/// <field name="Throw">Client side error messages are displayed and error handling is enabled.</field>
JSTools.ExceptionHandling.ErrorHandling = new JSTools.Enum.StringEnum(
	"None",
	"Catch",
	"Throw" );


/// <property type="JSTools.ExceptionHandling.Handler">
/// Default JSTools.ExceptionHandling.Handler instance.
/// </property>
JSTools.Exception = new JSTools.ExceptionHandling.Handler();
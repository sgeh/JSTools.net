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
/// Represents an exception object, which stores information about an error. To throw an exception,
/// you should use JSTools.Exception.Throw(strMessage, strFile, intLine);
/// </class>
/// <param name="strMessage" type="String">Error message which describes this exception.</param>
/// <param name="strFile" type="String">File which has thrown this exception. This argument is optional.</param>
/// <param name="intLine" type="Integer">Line number of the exception. This argument is optional.</param>
/// <param name="objTargetSiteFunction" type="Function">Function which has thrown this exception.
/// This argument is optional.</param>
/// <param name="objStackTrace" type="JSTools.ExceptionHandling.StackTrace">Call stack object.
/// This argument is optional.</param>
/// <param name="objInnerException" type="JSTools.ExceptionHandling.Exception">Inner exception instance. You
/// can chain exceptions with this argument. This argument is optional.</param>
JSTools.ExceptionHandling.Exception = function(
	strMessage,
	strFile,
	intLine,
	objTargetSiteFunction,
	objStackTrace,
	objInnerException)
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	this.InitType(arguments, "JSTools.ExceptionHandling.Exception");

	var _this = this;
	var _message = (strMessage) ? String(strMessage) : String.Empty;
	var _file = (strFile) ? String(strFile) : String.Empty;
	var _lineNumber = (!isNaN(intLine)) ? intLine : -1;
	var _targetSite = (typeof(objTargetSiteFunction) == 'function') ? objTargetSiteFunction : arguments.callee;
	var _stackTrace = null;
	var _innerException = null;


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------

	/// <constructor>
	/// Creates a new JSTools.ExceptionHandling.Exception instance.
	/// </constructor>
	function Init()
	{
		if (objStackTrace
			&& typeof(objStackTrace) == 'object'
			&& objStackTrace.IsTypeOf(JSTools.ExceptionHandling.StackTrace))
		{
			_stackTrace = objStackTrace;
		}
		else
		{
			_stackTrace = new JSTools.ExceptionHandling.StackTrace();
		}

		if (objInnerException
			&& typeof(objInnerException) == 'object'
			&& objInnerException.IsTypeOf(JSTools.ExceptionHandling.Exception))
		{
			_innerException = objInnerException;
		}
	}


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Gets the chained inner exception instance.
	/// </method>
	/// <returns type="String">Returns the chained inner exception instance.</returns>
	function GetInnerException()
	{
		return _innerException;
	}
	this.GetInnerException = GetInnerException;


	/// <method>
	/// Gets the StackTrace object of the exception.
	/// </method>
	/// <returns type="JSTools.ExceptionHandling.StackTrace">Returns the StackTrace object of the exception.</returns>
	function GetStackTrace()
	{
		return _stackTrace;
	}
	this.GetStackTrace = GetStackTrace;


	/// <method>
	/// Gets the file name that throws the current exception.
	/// </method>
	/// <returns type="String">Returns the file name that throws the current exception.</returns>
	function GetFile()
	{
		return _file;
	}
	this.GetFile = GetFile;


	/// <method>
	/// Gets the file number that throws the current exception.
	/// </method>
	/// <returns type="Integer">Returns the file number that throws the current exception.</returns>
	function GetLineNumber()
	{
		return _lineNumber;
	}
	this.GetLineNumber = GetLineNumber;


	/// <method>
	/// Gets the exception message.
	/// </method>
	/// <returns type="String">Returns the exception message.</returns>
	function GetMessage()
	{
		return _message;
	}
	this.GetMessage = GetMessage;


	/// <method>
	/// Gets the method that throws the current exception.
	/// </method>
	/// <returns type="Function">Returns the method that throws the current exception.</returns>
	function GetTargetSite()
	{
		return _targetSite;
	}
	this.GetTargetSite = GetTargetSite;


	/// <method>
	/// Converts the current exception into a native exception instance.
	/// </method>
	/// <returns type="Error">Returns the converted exception instance.</returns>
	function ToNativeError()
	{
		var nativeException = new Error();
		nativeException.name = _this.GetType().GetName();
		nativeException.number = JSTools.ExceptionHandling.Exception.JSTOOLS_EXCEPTION_NUMBER;
		nativeException.message = GetFullExceptionMessage();
		nativeException.description = nativeException.message;
		nativeException.defaultMessage = GetDefaultExceptionMessage();
		return nativeException;
	}
	this.ToNativeError = ToNativeError;


	function GetFullExceptionMessage()
	{
		var message = JSTools.ExceptionHandling.Exception.JSTOOLS_EXCEPTION_NUMBER;
		message += " " + _this.GetMessage();
		message += "\nLine: " + _this.GetLineNumber();
		message += "\nFile: " + _this.GetFile();
		
		if (_this.GetTargetSite())
			message += "\nTarget Site: " + _this.GetTargetSite().GetName() + "(...)";

		message += "\nStacktrace:\n" + _this.GetStackTrace() + "\n";
		
		if (_this.GetInnerException())
			message += "\nInner Exception:\n\n" + _this.GetInnerException().ToNativeError().message + "\n";

		return message;
	}
	
	function GetDefaultExceptionMessage()
	{
		var message = JSTools.ExceptionHandling.Exception.JSTOOLS_EXCEPTION_NUMBER;
		message += " " + _this.GetMessage();
		message += " | Line: " + _this.GetLineNumber();
		message += " | File: " + _this.GetFile();
		
		if (_this.GetTargetSite())
			message += " | Target Site: " + _this.GetTargetSite().GetName() + "(...)";

		message += " | Stacktrace: " + _this.GetStackTrace().toString().replace(/\n/g, " - ");
		
		if (_this.GetInnerException())
			message += " | Inner Exception: " + _this.GetInnerException().ToNativeError().message + "\n";

		return message;
	}
	Init();
}


/// <property type="Integer">
/// Represents the exception number of a manually thrown script exception.
/// </property>
JSTools.ExceptionHandling.Exception.JSTOOLS_EXCEPTION_NUMBER = 0xFED451; // random exception number


// provide down level compatibility -> declare error class if it does not exist
if (typeof(Error) == 'undefined')
{
	var Error = function()
	{
		this.message = "";
		this.defaultMessage = "";
		this.name = "";
		this.number = -1;
		this.description = "";
		this.toString = function() { return "[object Error]"; }
	}
}
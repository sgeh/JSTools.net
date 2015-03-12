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
/// Exception is thrown if the user has specified an invalid argument.
/// </class>
/// <param name="strMessage" type="String">Error message which describes this exception.</param>
/// <param name="strArgumentName" type="String">Name of the argument which has raised the error.</param>
/// <param name="strFile" type="String">File which has thrown this exception. This argument is optional.</param>
/// <param name="intLine" type="Integer">Line number of the exception. This argument is optional.</param>
/// <param name="objTargetSiteFunction" type="Function">Function which has thrown this exception.
/// This argument is optional.</param>
/// <param name="objStackTrace" type="JSTools.ExceptionHandling.StackTrace">Call stack object.
/// This argument is optional.</param>
/// <param name="objInnerException" type="JSTools.ExceptionHandling.Exception">Inner exception instance. You
/// can chain exceptions with this argument. This argument is optional.</param>
JSTools.ExceptionHandling.ArgumentException = function(
	strMessage,
	strArgumentName,
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
	this.Inherit(JSTools.ExceptionHandling.Exception,
		strMessage,
		strFile,
		intLine,
		objTargetSiteFunction,
		objStackTrace,
		objInnerException );

	var _argumentName = String(strArgumentName);


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------

	/// <constructor>
	/// Creates a new JSTools.ExceptionHandling.ArgumentException instance.
	/// </constructor>
	function Init()
	{
	}


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Gets the name of the argument which has raised the exception.
	/// </method>
	/// <returns type="String">Returns the argument name whach has raised the exception.</returns>
	function GetArgumentName()
	{
		return _argumentName;
	}
	this.GetArgumentName = GetArgumentName;

	Init();
}

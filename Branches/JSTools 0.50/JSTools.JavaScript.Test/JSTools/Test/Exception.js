/*
 * JSTools.JavaScript.Test / JSTools.net - A JavaScript/C# framework.
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

document.write("<h1>TEST EXCEPTION</h1>");

var logOccurences = 0;
var warnOccurences = 0;

function checkLog(objException)
{
	++logOccurences;
	logOk = (objException.GetMessage() == "log");
}

function checkWarning(objException)
{
	++warnOccurences;
	warningOk = (objException.GetMessage() == "warn");
}

function fireEvents()
{
	JSTools.Exception.Warn("Fire event, this is the warn message.", String.Empty, -1);
	JSTools.Exception.Log("Fire event, this is log the message.", String.Empty, -1);
}

function drawExceptionStatus(intStatus, intMaxOccurences, strEvent)
{
	if (intStatus != intMaxOccurences)
	{
		document.write("an error has occured while handling the exception event '"
			+ strEvent
			+ "' -> [status: " + intStatus + "; should: "
			+ intMaxOccurences
			+ "]<br>");
	}
	else
	{
		document.write("event '" + strEvent + "' successfull<br>");
	}
}

// attach on log event
var index = JSTools.Exception.AddOnLogEvent(checkLog);

if (index == -1)
	document.write("AddOnLogEvent has not returned an index.");

// attach on error event
index = JSTools.Exception.AddOnErrorEvent(function() { } );

if (index == -1)
	document.write("AddOnErrorEvent has not returned an index.");

// attach on warn event
index = JSTools.Exception.AddOnWarnEvent(checkWarning);

if (index == -1)
	document.write("AddOnWarnEvent has not returned an index.");


JSTools.Exception.EventHandling = JSTools.ExceptionHandling.ErrorEvent.All;

fireEvents();

JSTools.Exception.EventHandling = JSTools.ExceptionHandling.ErrorEvent.None;

fireEvents();

JSTools.Exception.EventHandling |= JSTools.ExceptionHandling.ErrorEvent.Log;

fireEvents();

JSTools.Exception.EventHandling |= JSTools.ExceptionHandling.ErrorEvent.Error;

fireEvents();

JSTools.Exception.EventHandling |= JSTools.ExceptionHandling.ErrorEvent.Warn;

fireEvents();


// check result
drawExceptionStatus(logOccurences, 4, "log");
drawExceptionStatus(warnOccurences, 2, "warn");

document.write("throw must be tested manually -> script execution will stop")

//	JSTools.Exception.Throw("Throw exception, this is the error message.", String.Empty, -1);
//	JSTools.Exception.Throw(new JSTools.ExceptionHandling.Exception("Throw exception, this is the error message."));


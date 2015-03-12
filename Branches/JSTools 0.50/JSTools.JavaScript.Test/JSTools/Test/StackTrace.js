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

document.write("<h1>TEST STACKTRACE</h1>");

var a1 = function()
{
	a2();
}

function a2()
{
	a3();
}

function a3()
{
	a4();
}

function a4()
{
	a5();
}

function a5()
{
	a6();
}

function a6()
{
	var stackTrace = new JSTools.ExceptionHandling.StackTrace();
	document.write("<p>Stacktrace: " + stackTrace.toString() + "</p>");
	document.write("<p>Method Count: " + stackTrace.GetMethodCount() + "</p>");
	document.write("<p>Method [0]: " + typeof(stackTrace.GetMethod(0)) + "</p>");
	
	if (!stackTrace.IsAvailable())
	{
		document.write("[Invalid Browser]");
	}
}

a1();

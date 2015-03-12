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

document.write("<h1>TEST REFLECTION</h1>");

function ReflectionTest()
{
	this.InitType(arguments, "ReflectionTest");

	var _protected = this.Inherit(Object);

	_protected.ProtectedFunction = function()
	{
		return "ProtectedFunction";
	}

	var b = 3;
	var _a, a_c, d34, e3_s ,f$d = "23";

	function InnerFunction()
	{
		function NestedInnerFunction()
		{
			var h = 0;
		}
		return "InnerFunction";
	}

	this.AFunction = function()
	{
		var i = 2;
		return "AFunction";
	}

	var g = 0;

	function AnOtherFunction()
	{
		this.NestedAnonymousFunction = function()
		{
			var j = 2;
			return "nested";
		}
		return "AnOtherFunction";
	}
	
	this.AnonymousFunction = function()
	{
		return "[null]";
	}
}
ReflectionTest.prototype.SomeFunction = function()
{
	return "SomeFunction";
}


var _testObject = new ReflectionTest();

document.write("<table border='0' cellspacing='2' cellpadding='0' width='800'>");
document.write("<tr>");
document.write("	<td width='100'><b>Index</b></td>");
document.write("	<td width='100'><b>Type</b></td>");
document.write("	<td width='200'><b>Name</b></td>");
document.write("	<td width='100'><b>Value</b></td>");
document.write("	<td width='100'><b>IsPublic</b></td>");
document.write("	<td width='100'><b>IsPrivate</b></td>");
document.write("	<td width='100'><b>IsProtected</b></td>");
document.write("</tr>");

// reflection of fields
document.write("<tr><td colspan='7'><hr></td></tr>");
document.write("<tr><td colspan='7'><b>Fields:</b></td></tr>");

var _fields = _testObject.GetType().GetFields();

for (var i = 0; i < _fields.length; ++i)
{
	var field =  _fields[i];

	if (!field)
	{
		alert("exception while reflect field [" + i + "]");
		continue;
	}

	document.write("<tr>");
	document.write("	<td width='100'>" + (i + 1) + "</td>");
	document.write("	<td width='100'>" + JSTools.Reflection.MemberType.GetName(field.GetMemberType()) + "</td>");
	document.write("	<td width='200'>" + field.GetName() + "</td>");
	document.write("	<td width='100'>" + String(field.GetValue()) + "</td>");
	document.write("	<td width='100'>" + (field.IsPublic() ? 1 : 0) + "</td>");
	document.write("	<td width='100'>" + (field.IsPrivate() ? 1 : 0) + "</td>");
	document.write("	<td width='100'>" + (field.IsProtected() ? 1 : 0) + "</td>");
	document.write("</tr>");
}

// reflection of methods
document.write("<tr><td colspan='7'><hr></td></tr>");
document.write("<tr><td colspan='7'><b>Methods:</b></td></tr>");

var _methods = _testObject.GetType().GetMethods();

for (var i = 0; i < _methods.length; ++i)
{
	var method =  _methods[i];

	if (!method)
	{
		alert("exception while reflect method [" + i + "]");
		continue;
	}

	document.write("<tr>");
	document.write("	<td width='100'>" + (i + 1) + "</td>");
	document.write("	<td width='100'>" + JSTools.Reflection.MemberType.GetName(method.GetMemberType()) + "</td>");
	document.write("	<td width='200'>" + method.GetName() + "</td>");
	document.write("	<td width='100'>" + String(method.Invoke()) + "</td>");
	document.write("	<td width='100'>" + (method.IsPublic() ? 1 : 0) + "</td>");
	document.write("	<td width='100'>" + (method.IsPrivate() ? 1 : 0) + "</td>");
	document.write("	<td width='100'>" + (method.IsProtected() ? 1 : 0) + "</td>");
	document.write("</tr>");
}

document.write("</table>");
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

document.write("<h1>TEST Cookie</h1>");

var cookieNames = [ "CookieName0", "CookieName1", "CookieName2", "CookieName3", "CookieName4" ];
var storedValues =
[
	{ Name: "Key0", Value: "Value0" },
	{ Name: "Key1", Value: "Value1" },
	{ Name: "Key2", Value: "Value2" },
	{ Name: "Key3", Value: "Value3" },
	{ Name: "Key4", Value: "Value4" },
	{ Name: "Key5", Value: "Value5" },
	{ Name: "Key6", Value: "Value6" }
];

for (var i = 0; i < cookieNames.length; ++i)
{
	// create cookie
	var cookie = new JSTools.Web.Cookie.CookieData(cookieNames[i]);

	var host = location.host;
	var expDate = (new Date().valueOf() + 3600000);
	var path = "/";
	var secure = true;
	var defaultValue = cookieNames[i];

	// set cookie data
	cookie.SetDomain(host);
	cookie.SetExpirationDate(expDate);
	cookie.SetPath(path);
	cookie.SetSecure(secure);
	cookie.SetDefaultValue(defaultValue);

	for (var j = 0; j < storedValues.length; ++j)
	{
		cookie.SetValue(storedValues[j].Name, storedValues[j].Value);
	}

	// add cookie to collection
	JSTools.Cookies.AddCookie(cookie);

	// write cookie, requires the cookie name
	JSTools.Cookies.StoreCookie(cookie.GetName());

	// check values
	if (host !=  cookie.GetDomain())
		document.write("could not retrieve the domain<br>");

	if (expDate !=  cookie.GetExpirationDate().valueOf())
		document.write("could not retrieve the expiration date<br>");

	if (path !=  cookie.GetPath())
		document.write("could not retrieve the path<br>");

	if (secure !=  cookie.GetSecure())
		document.write("could not retrieve the secure level<br>");

	if (defaultValue !=  cookie.GetDefaultValue())
		document.write("could not retrieve the default value<br>");

	if (!JSTools.Cookies.Contains(cookie.GetName()))
		document.write("the cookie was added but could not be retrieved<br>");
}

// test Remove
JSTools.Cookies.Remove("CookieName3");

if (JSTools.Cookies.Contains("CookieName3"))
	document.write("error while removing the cookie<br>");

// test GetCookie
if (JSTools.Cookies.GetCookie("CookieName0") == null)
	document.write("could not retrieve cookies<br>");

// test Clone
if (JSTools.Cookies.GetCookie("CookieName1").Clone().toString() != JSTools.Cookies.GetCookie("CookieName1").toString())
	document.write("could not clone a cookie<br>");

// test Clear
JSTools.Cookies.Clear();

if (JSTools.Cookies.GetCookie("CookieName0") != null || JSTools.Cookies.GetCookieNames().length != 0)
	document.write("could not clear the cookie collection<br>");

document.write("cookie test done<br>");
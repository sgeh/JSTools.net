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

namespace("JSTools.Web.Browser");


/// <class>
/// Contains matching information for a browser user agent string.
/// </class>
/// <param name="strBrowserPattern" type="String">Regular expression, which matches the browser user agent string.</param>
/// <param name="strBrowserName" type="String">Name of the Browser, e.g. Internet Explorer/Netscape/Opera...</param>
/// <param name="strShortName" type="String">Short name of the Browser, e.g. IE/NS/OP...</param>
JSTools.Web.Browser.BrowserType = function(strBrowserPattern, strBrowserName, strShortName)
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	this.InitType(arguments, "JSTools.Web.Browser.BrowserType");

	var _this = this;
	var _idPattern = new RegExp(String(strBrowserPattern));
	var _browserName = String(strBrowserName);
	var _shortName = String(strShortName);
	var _match = [ ];


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Checks, if the current browser matches this browser type item.
	/// </method>
	/// <returns type="Boolean">Returns true, if this item is appropriated for the current browser.</returns>
	function MatchBrowser()
	{
		var match = navigator.userAgent.match(_idPattern);

		if (match == null || match.length == 0)
			return false;

		_match = match;
		return true;
	}
	this.MatchBrowser = MatchBrowser;


	/// <method>
	/// Gets the browser name. (e.g. Netscape/Opera/Internet Explorer)
	/// </method>
	/// <returns type="String">Returns the name of the browser.</returns>
	function GetName()
	{
		return _browserName;
	}
	this.GetName = GetName;


	/// <method>
	/// Gets the short name of the browser. (e.g. NS/OP/IE)
	/// </method>
	/// <returns type="String">Returns the short name of the browser.</returns>
	function GetShortName()
	{
		return _shortName;
	}
	this.GetShortName = GetShortName;


	/// <method>
	/// Gets the browser version (e.g. 7.01, 6.2, 4.78, 4.06).
	/// </method>
	/// <returns type="Number">Returns the browser version.</returns>
	function GetVersion()
	{
		return Number(_match[1]);
	}
	this.GetVersion = GetVersion;


	/// <method>
	/// Gets the major version of the browser (e.g. 7, 6, 4).
	/// </method>
	/// <returns type="Number">Returns the major version of the browser.</returns>
	function GetMajor()
	{
		return Number(_match[2]);
	}
	this.GetMajor = GetMajor;


	/// <method>
	/// Gets the minor version of the browser (e.g. 0.01, 0.2, 0.78, 0.06).
	/// </method>
	/// <returns type="Number">Returns the minor version of the browser.</returns>
	function GetMinor()
	{
		return Number(_match[3]);
	}
	this.GetMinor = GetMinor;
}


/// <method type="JSTools.Web.Browser.BrowserType">
/// Gets the active JSTools.Web.Browser.BrowserType instance.
/// </method>
JSTools.Web.Browser.BrowserType.GetActiveBrowserType = function()
{
	var browserTypes = JSTools.Web.Browser.BrowserType.Types;

	for (var i = 0; i < browserTypes.length; ++i)
	{
		if (browserTypes[i].MatchBrowser())
		{
			return browserTypes[i];
		}
	}
	return new JSTools.Web.Browser.DefaultBrowserType();
}


/// <property type="Array">
/// Default implemented JSTools.Web.Browser.BrowserType instances.
/// First match ($2) contains the version, second match ($3) the major and
/// the thrid match ($4) contains the minor version.
///
/// This array can be extended if necessary.
/// </property>
JSTools.Web.Browser.BrowserType.Types =
[
	// Internet Explorer 4.x +
	new JSTools.Web.Browser.BrowserType("^Mozilla[^(]*\\(compatible; MSIE ((\\d+)(\\.\\d+)).*$", "Internet Explorer", "IE"),

	// Mozilla 1.x +
	new JSTools.Web.Browser.BrowserType("^Mozilla/5.0 \\(.*rv:((\\d+)(\\.\\d+)).*\\) Gecko/\\d{8,8}$", "Mozilla", "MO"),

	// Netscape Navigator 4.x
	new JSTools.Web.Browser.BrowserType("^Mozilla/((\\d+)(\\.\\d+)) \\[.*\\].*$", "Netscape", "NS"),

	// Netscape Navigator 6.x
	new JSTools.Web.Browser.BrowserType("^Mozilla.* Netscape6/((\\d+)(\\.\\d+))$", "Netscape", "NS"),

	// Netscape Navigator 7.x
	new JSTools.Web.Browser.BrowserType("^Mozilla.* Netscape/((\\d+)(\\.\\d+))$", "Netscape", "NS"),

	// Opera 5.x + (as Opera)
	new JSTools.Web.Browser.BrowserType("^Opera/((\\d+)(\\.\\d+)).*$", "Opera", "OP"),

	// Opera 5.x + (as Mozilla)
	new JSTools.Web.Browser.BrowserType("^Mozilla.* Opera ((\\d+)(\\.\\d+)).*$", "Opera", "OP")
];

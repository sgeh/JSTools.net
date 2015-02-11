/*
 * JSTools.Test.dll / JSTools.net - A framework for JavaScript/ASP.NET applications.
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
/// Advanced browser information object. 
///
/// A dynamic property is created, which contains the short name and the
/// major version of the browser (e.g. IsIE4/IsNS3/IsOP3).
/// </class>
/// <param name="objBrowserType" type="BrowserType">Browser type to initialize.</param>
JSTools.Web.Browser.BrowserCapabilities = function(objBrowserType)
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	this.InitType(arguments, "JSTools.Web.Browser.BrowserCapabilities");

	var TYPE_NAME_PATTERN = "Is{0}{1}";
	var _this = this;
	var _browserType = objBrowserType;


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------

	/// <constructor>
	/// JSTools.Web.Browser.BrowserCapabilities instance.
	/// </constructor>
	function Init()
	{
		_this[String.Format(TYPE_NAME_PATTERN, _this.GetShortName(), _this.GetMajor())] = true;
	}


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Gets a value indicating whether the browser supports ActiveX controls.
	/// </method>
	/// <returns type="Boolean">Returns true, if the browser supports ActiveX controls.</returns>
	function HasActiveXControls()
	{
		return (typeof(ActiveXObject) == 'function');
	}
	this.HasActiveXControls = HasActiveXControls;


	/// <method>
	/// Gets a value indicating whether the browser supports cookies.
	/// </method>
	/// <returns type="Boolean">Returns true, if the browser supports cookies.</returns>
	function HasCookies()
	{
		return (typeof(document.cookie) == 'string')
	}
	this.HasCookies = HasCookies;


	/// <method>
	/// Gets a value indicating whether the browser supports Java applets.
	/// </method>
	/// <returns type="Boolean">Returns true, if the browser supports Java applets.</returns>
	function HasJavaApplets()
	{
		return navigator.javaEnabled();
	}
	this.HasJavaApplets = HasJavaApplets;


	/// <method>
	/// Gets a value indicating wheter the browser supports the Document Object Model (DOM).
	/// </method>
	/// <returns type="Boolean">Returns true, if the browser supports DOM.</returns>
	function HasDOM()
	{
		return (document.documentElement
			&& document.implementation
			&& document.implementation.hasFeature
			&& document.implementation.hasFeature("html" ,"1.0"));
	}
	this.HasDOM = HasDOM;


	/// <method>
	/// Gets the browser name. (e.g. Netscape/Opera/Internet Explorer)
	/// </method>
	/// <returns type="String">Returns the name of the browser.</returns>
	function GetName()
	{
		return _browserType.GetName();
	}
	this.GetName = GetName;


	/// <method>
	/// Gets the short name of the browser. (e.g. NS/OP/IE)
	/// </method>
	/// <returns type="String">Returns the short name of the browser.</returns>
	function GetShortName()
	{
		return _browserType.GetShortName();
	}
	this.GetShortName = GetShortName;


	/// <method>
	/// Gets the browser generation (5 or 4). Older browser can't perform
	/// nested function, thus this script won't execute.
	/// </method>
	/// <returns type="Number">Returns browser generation.</returns>
	function GetGeneration()
	{
		return (_this.HasDOM() ? 5 : 4);
	}
	this.GetGeneration = GetGeneration;


	/// <method>
	/// Gets the browser agent (navigator.userAgent) string.
	/// </method>
	/// <returns type="String">Returns the browser agent (navigator.userAgent) string.</returns>
	function GetBrowser()
	{
		return navigator.userAgent;
	}
	this.GetBrowser = GetBrowser;


	/// <method>
	/// Gets the browser version (e.g. 7.01, 6.2, 4.78, 4.06).
	/// </method>
	/// <returns type="Number">Returns the browser version.</returns>
	function GetVersion()
	{
		return _browserType.GetVersion();
	}
	this.GetVersion = GetVersion;


	/// <method>
	/// Gets the major version of the browser (e.g. 7, 6, 4).
	/// </method>
	/// <returns type="Number">Returns the major version of the browser.</returns>
	function GetMajor()
	{
		return _browserType.GetMajor();
	}
	this.GetMajor = GetMajor;


	/// <method>
	/// Gets the minor version of the browser (e.g. 0.01, 0.2, 0.78, 0.06).
	/// </method>
	/// <returns type="Number">Returns the minor version of the browser.</returns>
	function GetMinor()
	{
		return _browserType.GetMinor();
	}
	this.GetMinor = GetMinor;


	/// <method>
	/// Gets the platform name (navigator.platform).
	/// </method>
	/// <returns type="String">Returns the platform name.</returns>
	function GetPlatform()
	{
		return navigator.platform;
	}
	this.GetPlatform = GetPlatform;


	/// <method>
	/// Gets the name and major version number of the browser.
	/// </method>
	/// <returns type="String">Returns the name and major version number of the browser.</returns>
	function GetFullName()
	{
		return _this.GetName() + " " + _this.GetMajor();
	}
	this.GetFullName = GetFullName;


	/// <method>
	/// Gets the browser language, (e.g. en or en-ca).
	/// </method>
	/// <returns type="String">Returns the language of the browser.</returns>
	function GetLanguage()
	{
		var language = (navigator.userLanguage) ? navigator.userLanguage.replace("_", "-") : navigator.language;
		return language.toLowerCase();
	}
	this.GetLanguage = GetLanguage;

	Init();
}

/// <property type="JSTools.Web.Browser.BrowserCapabilities">
/// Represents the default BrowserCapabilities instance.
/// </property>
JSTools.Browser = new JSTools.Web.Browser.BrowserCapabilities(JSTools.Web.Browser.BrowserType.GetActiveBrowserType());

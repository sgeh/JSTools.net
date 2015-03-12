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

namespace("JSTools.Web.Cookie");


/// <class>
/// Stores and retrieves values from cookies. Make sure that cookies
/// are enabled in your browser when using this functionality.
/// </class>
/// <remarks>
/// Some browsers do not support more than 12 cookies per page.
/// </remarks>
/// <example>
/// // create cookie
/// var cookie = new JSTools.Web.Cookie.CookieData("CookieName");
///
/// // set cookie data
/// cookie.SetDomain(location.host);
/// cookie.SetExpirationDate(new Date() + 3600000);
/// cookie.SetPath("/main");
/// cookie.SetSecure = false;
/// cookie.SetDefaultValue("DefaultValue");
///
/// cookie.SetValue("Key1", "Value1");
/// cookie.SetValue("Key2", "Value2");
/// cookie.SetValue("Key3", "Value3");
///
/// // add cookie to collection
/// JSTools.Cookies.AddCookie(cookie);
///
/// // write cookie, requires the cookie name
/// JSTools.Cookies.StoreCookie(cookie.GetName());
/// </example>
JSTools.Web.Cookie.CookieCollection = function()
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	this.InitType(arguments, "JSTools.Web.Cookie.CookieCollection");

	var COOKIE_SEPARATOR = "; ";
	var COOKIE_VALUE_SEPARATOR = "=";

	var _protected = this.Inherit();
	var _this = this;
	var _cookies = [ ];


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------

	/// <constructor>
	/// Creates a new JSTools.Web.Cookie.CookieCollection instance.
	/// </constructor>
	function Init()
	{
		var cookies = _protected.RetrieveCookieData().split(COOKIE_SEPARATOR);

		for (var i = 0; i < cookies.length; ++i)
		{
			var cookieSplit = cookies[i].indexOf(COOKIE_VALUE_SEPARATOR);
			var cookie = new JSTools.Web.Cookie.CookieData(
				cookies[i].substring(0, cookieSplit),
				cookies[i].substring(cookieSplit + 1) );

			_this.AddCookie(cookie);
		}
	}


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Deletes the associated cookie.
	/// </method>
	/// <param name="strCookieName" type="String">Name of the cookie.</param>
	/// <returns type="String">Returns the associated domain or a null reference if
	/// there is no domain specified</returns>
	function Remove(strCookieName)
	{
		var cookieIndex = IndexOfCookie(strCookieName);

		if (cookieIndex != -1)
		{
			DeleteCookie(cookieIndex);
		}
	}
	this.Remove = Remove;


	/// <method>
	/// Gets a boolean indicating whether the cookie with the given name is stored
	/// in this CookieContainer.
	/// </method>
	/// <param name="strCookieName" type="String">Name of the cookie.</param>
	/// <returns type="Boolean">Returns true if the value is stored in this container,
	/// otherwise false.</returns>
	function Contains(strCookieName)
	{
		return (IndexOfCookie(strCookieName) != -1);
	}
	this.Contains = Contains;


	/// <method>
	/// Gets the cookie associated with the given name.
	/// </method>
	/// <param name="strCookieName" type="String">Name of the cookie.</param>
	/// <returns type="JSTools.Web.Cookie.CookieData">Returns the cookie associated
	/// with the given name.</returns>
	function GetCookie(strCookieName)
	{
		var cookieIndex = IndexOfCookie(strCookieName);
		
		if (cookieIndex != -1)
		{
			return _cookies[cookieIndex];
		}
		return null;
	}
	this.GetCookie = GetCookie;


	/// <method>
	/// Creates a new array which contains the names of all stored cookies.
	/// </method>
	/// <returns type="Array">Returns a string array which contains the names
	/// of all stored cookies.</returns>
	function GetCookieNames()
	{
		var names = new Array(_cookies.length);

		for (var i = 0; i < _cookies.length; ++i)
		{
			names[i] = _cookies[i].GetName();
		}
		return names;
	}
	this.GetCookieNames = GetCookieNames;


	/// <method>
	/// Adds a new cookie instance to this collection object.
	/// </method>
	/// <param name="objCookieData" type="JSTools.Web.Cookie.CookieData">Cookie instance to set.</param>
	function AddCookie(objCookieData)
	{
		if (objCookieData
			&& typeof(objCookieData) == 'object'
			&& objCookieData.IsTypeOf(JSTools.Web.Cookie.CookieData))
		{
			_cookies.Add(objCookieData);
		}
	}
	this.AddCookie = AddCookie;


	/// <method>
	/// Stores the cookie with the associated name.
	/// </method>
	/// <param name="strCookieName" type="String">Name of the cookie.</param>
	function StoreCookie(strCookieName)
	{
		var cookie = _this.GetCookie(strCookieName);

		if (cookie != null)
		{
			_protected.WriteCookieData(cookie.toString());
		}
	}
	this.StoreCookie = StoreCookie;


	/// <method>
	/// Stores all cookies.
	/// </method>
	function Store()
	{
		for (var i = 0; i < _cookies.length; ++i)
		{
			_protected.WriteCookieData(_cookies[i]);
		}
	}
	this.Store = Store;


	/// <method>
	/// Deletes all cookies of this page.
	/// </method>
	function Clear()
	{
		for (var i = _cookies.length - 1; i > -1; --i)
		{
			DeleteCookie(i);
		}
	}
	this.Clear = Clear;
	

	/// <method>
	/// Retrieves the cookie data.
	/// </method>
	/// <returns type="String">Returns the retrieved data.</returns>
	function RetrieveCookieData()
	{
		return String(document.cookie);
	}
	_protected.RetrieveCookieData = RetrieveCookieData;


	/// <method>
	/// Stores the cookie data.
	/// </method>
	/// <param name="objCookie" type="JSTools.Web.Cookie.CookieData"></param>
	function WriteCookieData(objCookie)
	{
		document.cookie = objCookie + COOKIE_VALUE_SEPARATOR + objCookie.toString();
	}
	_protected.WriteCookieData = WriteCookieData;


	/// <method>
	/// Deletes the cookie at the given index.
	/// </method>
	/// <param name="intCookieIndex" type="Index">Index of the cookie to delete.</param>
	function DeleteCookie(intCookieIndex)
	{
		_cookies[intCookieIndex].SetExpirationDate(new Date(0));
		_protected.WriteCookieData(_cookies[intCookieIndex]);
		_cookies.RemoveAt(intCookieIndex);
	}


	/// <method>
	/// Returns the index of the cookie with the given name.
	/// </method>
	/// <param name="strCookieName" type="String">Name of the cookie.</param>
	function IndexOfCookie(strCookieName)
	{
		var cookieName = String(strCookieName);

		for (var i = 0; i < _cookies.length; ++i)
		{
			if (_cookies[i].GetName() == cookieName)
			{
				return i;
			}
		}
		return -1;
	}
	Init();
}


/// <property type="JSTools.Web.Cookie.CookieCollection">
/// Default CookieCollection instance.
/// </property>
JSTools.Cookies = new JSTools.Web.Cookie.CookieCollection();
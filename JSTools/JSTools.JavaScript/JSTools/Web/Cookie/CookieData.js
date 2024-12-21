namespace("JSTools.Web.Cookie");


/// <class>
/// The CookieValue class provides a container for storing and retrieving data
/// around cookies. If you'd like to write or delete a Cookie, you have to
/// use the CookieCollection class.
/// Domain, path, expires and secure could not be retrieved and could be
/// used only for creating a cookie.
/// </class>
/// <remarks>
/// Cookies which contains data larger than 4kBytes may not be stored by some
/// browsers.
/// </remarks>
/// <param name="strCookieName" type="String">Name of the cookie to create.
/// This string will be unescaped.</param>
/// <param name="strCookieData" type="String">A cookie data string, retrieved
/// from the document.cookie property.</param>
JSTools.Web.Cookie.CookieData = function(strCookieName, strCookieData)
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	this.InitType(arguments, "JSTools.Web.Cookie.CookieData");

	var _this			= this;
	var _domain			= null;
	var _path			= null;
	var _expires		= null;
	var _secure			= false;
	var _name			= unescape(String(strCookieName));
	var _defaultValue	= null;
	var _stringParser	= null;


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------

	/// <constructor>
	/// Creates a new JSTools.Web.Cookie.CookieData instance.
	/// </constructor>
	function Init()
	{
		if (typeof(strCookieData) != 'undefined' && strCookieData)
		{
			_stringParser = new JSTools.Web.QueryStringParser(String(strCookieData), true);
		}
		else
		{
			_stringParser = new JSTools.Web.QueryStringParser(null, true);
		}
	}


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Gets the domain to associate the cookie with. 
	/// </method>
	/// <returns type="String">Returns the associated domain or a null reference if
	/// there is no domain specified</returns>
	this.GetDomain = function()
	{
		return _domain;
	}


	/// <method>
	/// Sets the domain to associate the cookie with. 
	/// </method>
	/// <param name="strDomain" type="String">Domain to associat the cookie with.</param>
	this.SetDomain = function(strDomain)
	{
		_domain = String(strDomain);
	}


	/// <method>
	/// Gets the expiration date and time for the cookie.
	/// </method>
	/// <returns type="Date">Returns the expiration date or a null reference if
	/// there is no expiration date specified.</returns>
	this.GetExpirationDate = function()
	{
		return _expires;
	}


	/// <method>
	/// Sets the expiration date and time for the cookie.
	/// </method>
	/// <param name="objExpirationDate" type="Date">Expiration date of the cookie.</param>
	/// <param name="objExpirationDate" type="Integer">Expiration date (in milliseconds) of the cookie.</param>
	this.SetExpirationDate = function(objExpirationDate)
	{
		if (objExpirationDate && objExpirationDate.constructor == Date)
		{
			_expires = objExpirationDate;
		}
		else if (typeof(objExpirationDate) == 'number')
		{
			_expires = new Date(objExpirationDate);
		}
	}


	/// <method>
	/// Gets the name of a cookie.
	/// </method>
	/// <returns type="String">Returns the name of this cookie.</returns>
	this.GetName = function()
	{
		return _name;
	}


	/// <method>
	/// Gets the virtual path of this cookie.
	/// </method>
	/// <param name="objExpirationDate" type="Date">Domain to associate the cookie with.</param>
	/// <returns type="String">Returns the path of the cookie or a null reference if
	/// there is no path specified.</returns>
	this.GetPath = function()
	{
		return _path;
	}


	/// <method>
	/// Sets the virtual path of this cookie.
	/// </method>
	/// <param name="strPath" type="String">Path of the cookie.</param>
	this.SetPath = function(strPath)
	{
		_path = String(strPath);
	}


	/// <method>
	/// Gets a boolean whether to transmit the cookie securely (that is, over HTTPS only).
	/// Default value is false.
	/// </method>
	/// <returns type="Boolean"></returns>
	this.GetSecure = function(blnSecure)
	{
		return _secure;
	}


	/// <method>
	/// Sets a boolean whether to transmit the cookie securely (that is, over HTTPS only).
	/// </method>
	/// <param name="blnSecure" type="Boolean">Boolean whether to tramsmit the cookie securely.</param>
	this.SetSecure = function(blnSecure)
	{
		_secure = Boolean(blnSecure);
	}


	/// <method>
	/// Stores the given value. String will be stored in an escaped format. Cookies do
	/// not store data larger than 4kBytes.
	/// </method>
	/// <param name="strValue" type="String">Value to add.</param>
	this.SetDefaultValue = function(strValue)
	{
		_stringParser.SetFileUrl(String(strValue));
	}


	/// <method>
	/// Gets the default value.
	/// </method>
	/// <returns type="String">Returns the default value.</returns>
	this.GetDefaultValue = function()
	{
		return _stringParser.GetFileUrl();
	}


	/// <method>
	/// Adds the given value and the given name. If the specified name already exists, the value
	/// of the given name will be overriden. String will be stored in an escaped format. Cookies do
	/// not store data larger than 4kBytes.
	/// </method>
	/// <param name="strValueName" type="String">Name of the value to add.</param>
	/// <param name="strValue" type="String">Value to add.</param>
	this.SetValue = function(strValueName, strValue)
	{
		_stringParser.SetValue(strValueName, strValue);
	}


	/// <method>
	/// Returns the value with the given name.
	/// </method>
	/// <param name="strValueName" type="String">Name of the value to get.</param>
	/// <returns type="String">Returns the expected value or a null reference if there
	/// is no value associated with the given name.</returns>
	this.GetValue = function(strValueName)
	{
		_stringParser.GetValue(strValueName);
	}


	/// <method>
	/// Removes the value associated with the given name.
	/// </method>
	/// <param name="strValueName" type="String">Name of the value to remove.</param>
	/// <returns type="String">Returns the removed value or a null reference, if the given
	/// name could not be found.</returns>
	this.RemoveValue = function(strValueName)
	{
		_stringParser.RemoveValue(strValueName);
	}


	/// <method>
	/// Returns a copy of all registered value names.
	/// </method>
	/// <returns type="Array"></returns>
	this.GetValueNames = function()
	{
		return _stringParser.GetValueNames();
	}


	/// <method>
	/// Creates a new CookieData instance and copies the values of this object
	/// into the new CookieData object.
	/// </method>
	/// <param name="strNewCookieName" type="String">Name of the cookie to create.
	/// Default value is the name of this cookie.</param>
	/// <returns type="JSTools.Web.Cookie.CookieData">Returns a clone of this object.</returns>
	this.Clone = function(strNewCookieName)
	{
		var newCookieName = (typeof(strNewCookieName) != 'undefined') ? String(strNewCookieName) : _name;
		var newCookieData = new JSTools.Web.Cookie.CookieData(newCookieName, _stringParser.toString());

		newCookieData.SetDomain(_domain);
		newCookieData.SetExpirationDate(_expires.valueOf());
		newCookieData.SetPath(_path);
		newCookieData.SetSecure(_secure);
		return newCookieData;
	}


	/// <method>
	/// Creates the javascript cookie string representation of this object.
	/// </method>
	/// <returns type="String">Returns the javascript cookie string representation
	/// of this object.</returns>
	this.toString = function()
	{
		return _stringParser.toString() + ";"
			+ (_expires ? "expires=" + _expires.toGMTString() + ";" : "")
			+ (_path ? "path=" + _path + ";" : "")
			+ (_domain ? "domain=" + _domain + ";" : "")
			+ (_secure ? "secure" : "");
	}
	Init();
}
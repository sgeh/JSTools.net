namespace("JSTools.Web.Browser");


/// <class>
/// Advanced browser information object.
/// </class>
/// <param name="objBrowserType" type="BrowserType">Browser type to initialize.</param>
JSTools.Web.Browser.BrowserCapabilities = function(objBrowserType)
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	this.InitType(arguments, "JSTools.Web.Browser.BrowserCapabilities");

	var _this = this;
	var _browserType = objBrowserType;


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Gets a value indicating whether the browser supports ActiveX controls.
	/// </method>
	/// <returns type="Boolean">Returns true, if the browser supports ActiveX controls.</returns>
	this.HasActiveXControls = function()
	{
		return (typeof(ActiveXObject) == 'function');
	}


	/// <method>
	/// Gets a value indicating whether the browser supports cookies.
	/// </method>
	/// <returns type="Boolean">Returns true, if the browser supports cookies.</returns>
	this.HasCookies = function()
	{
		return (typeof(document.cookie) == 'string')
	}


	/// <method>
	/// Gets a value indicating whether the browser supports Java applets.
	/// </method>
	/// <returns type="Boolean">Returns true, if the browser supports Java applets.</returns>
	this.HasJavaApplets = function()
	{
		return navigator.javaEnabled();
	}


	/// <method>
	/// Gets a value indicating wheter the browser supports the Document Object Model (DOM).
	/// </method>
	/// <returns type="Boolean">Returns true, if the browser supports DOM.</returns>
	this.HasDOM = function()
	{
		return (document.documentElement && document.createElement);
	}


	/// <method>
	/// Gets the browser name. (e.g. Netscape/Opera/Internet Explorer)
	/// </method>
	/// <returns type="String">Returns the name of the browser.</returns>
	this.GetName = function()
	{
		return _browserType.GetName();
	}


	/// <method>
	/// Gets the short name of the browser. (e.g. NS/OP/IE)
	/// </method>
	/// <returns type="String">Returns the short name of the browser.</returns>
	this.GetShortName = function()
	{
		return _browserType.GetShortName();
	}


	/// <method>
	/// Gets the browser generation (5 or 4). Older browser can't perform
	/// nested function, thus this script won't execute.
	/// </method>
	/// <returns type="Number">Returns browser generation.</returns>
	this.GetGeneration = function()
	{
		return (_this.HasDOM() ? 5 : 4);
	}


	/// <method>
	/// Gets the browser agent (navigator.userAgent) string.
	/// </method>
	/// <returns type="String">Returns the browser agent (navigator.userAgent) string.</returns>
	this.GetBrowser = function()
	{
		return navigator.userAgent;
	}


	/// <method>
	/// Gets the browser version (e.g. 7.01, 6.2, 4.78, 4.06).
	/// </method>
	/// <returns type="Number">Returns the browser version.</returns>
	this.GetVersion = function()
	{
		return _browserType.GetVersion();
	}


	/// <method>
	/// Gets the major version of the browser (e.g. 7, 6, 4).
	/// </method>
	/// <returns type="Number">Returns the major version of the browser.</returns>
	this.GetMajor = function()
	{
		return _browserType.GetMajor();
	}


	/// <method>
	/// Gets the minor version of the browser (e.g. 0.01, 0.2, 0.78, 0.06).
	/// </method>
	/// <returns type="Number">Returns the minor version of the browser.</returns>
	this.GetMinor = function()
	{
		return _browserType.GetMinor();
	}


	/// <method>
	/// Gets the platform name (navigator.platform).
	/// </method>
	/// <returns type="String">Returns the platform name.</returns>
	this.GetPlatform = function()
	{
		return navigator.platform;
	}


	/// <method>
	/// Gets the name and major version number of the browser.
	/// </method>
	/// <returns type="String">Returns the name and major version number of the browser.</returns>
	this.GetFullName = function()
	{
		return _this.GetName() + " " + _this.GetMajor();
	}


	/// <method>
	/// Gets the browser language, (e.g. en or en-ca).
	/// </method>
	/// <returns type="String">Returns the language of the browser.</returns>
	this.GetLanguage = function()
	{
		var language = (navigator.userLanguage) ? navigator.userLanguage.replace("_", "-") : navigator.language;
		return language.toLowerCase();
	}
}

/// <property type="JSTools.Web.Browser.BrowserCapabilities">
/// Represents the default BrowserCapabilities instance.
/// </property>
JSTools.Browser = new JSTools.Web.Browser.BrowserCapabilities(JSTools.Web.Browser.BrowserType.GetActiveBrowserType());

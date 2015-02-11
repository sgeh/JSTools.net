namespace("JSTools.Web.Browser");


/// <class>
/// Default browser instance used for browsers, which are could not be identified.
/// </class>
JSTools.Web.Browser.DefaultBrowserType = function()
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	this.InitType(arguments, "JSTools.Web.Browser.DefaultBrowserType");
	this.Inherit(JSTools.Web.Browser.BrowserType);

	var _this				= this;
	var _versionRegExp		= /^((\d+)(\.\d+)).*$/;
	var _version			= 0;
	var _versionMajor		= 0;
	var _versionMinor		= 0;


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------

	/// <constructor>
	/// Creates a new JSTools.Web.Browser.DefaultBrowserType instance.
	/// </constructor>
	function Init()
	{
		_versionMatch = String(navigator.appVersion).match(_versionRegExp);
		
		if (_versionMatch && _versionMatch.length)
		{
			_version = Number(_versionMatch[1]);
			_versionMajor = Number(_versionMatch[2]);
			_versionMinor = Number(_versionMatch[3]);
		}
	}


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Checks, if the current browser matches this browser type item.
	/// </method>
	/// <returns type="Boolean">Returns true, if this item is appropriated for the current browser.</returns>
	this.MatchBrowser = function()
	{
		return true;
	}


	/// <method>
	/// Gets the browser name. (e.g. Netscape/Opera/Internet Explorer)
	/// </method>
	/// <returns type="String">Returns the name of the browser.</returns>
	this.GetName = function()
	{
		return navigator.appName;
	}


	/// <method>
	/// Gets the short name of the browser. (e.g. NS/OP/IE)
	/// </method>
	/// <returns type="String">Returns the short name of the browser.</returns>
	this.GetShortName = function()
	{
		return ((_this.GetName().length < 3) ? _this.GetName() : _this.GetName().substring(0, 2)).toUpperCase();
	}


	/// <method>
	/// Gets the browser version (e.g. 7.01, 6.2, 4.78, 4.06).
	/// </method>
	/// <returns type="Number">Returns the browser version.</returns>
	this.GetVersion = function()
	{
		return _version;
	}


	/// <method>
	/// Gets the major version of the browser (e.g. 7, 6, 4).
	/// </method>
	/// <returns type="Number">Returns the major version of the browser.</returns>
	this.GetMajor = function()
	{
		return _versionMajor;
	}


	/// <method>
	/// Gets the minor version of the browser (e.g. 0.01, 0.2, 0.78, 0.06).
	/// </method>
	/// <returns type="Number">Returns the minor version of the browser.</returns>
	this.GetMinor = function()
	{
		return _versionMinor;
	}
	Init();
}
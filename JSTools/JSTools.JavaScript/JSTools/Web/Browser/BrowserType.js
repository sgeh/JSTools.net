namespace("JSTools.Web.Browser");


/// <class>
/// Contains matching information for a browser user agent string.
/// </class>
/// <param name="strBrowserPattern" type="String">Regular expression, which matches the browser user agent string.</param>
/// <param name="strBrowserName" type="String">Name of the Browser, e.g. Internet Explorer/Netscape/Opera...</param>
JSTools.Web.Browser.BrowserType = function(strBrowserPattern, strBrowserName)
{
	//------------------------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------------------------

	var _this				= this;
	var _idPattern			= new RegExp(String(strBrowserPattern));
	var _browserName		= String(strBrowserName);
	var _match				= [ ];


	//------------------------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------------------------


	//------------------------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------------------------

	/// <method>
	/// 
	/// </method>
	this.MatchBrowser = function(strToCheck)
	{
		return ((_match = strToCheck.match(_idPattern)).length > 0);
	}


	/// <method>
	/// Gets the browser name. (e.g. Netscape/Opera/Internet Explorer)
	/// </method>
	/// <returns type="String">Returns the name of the browser.</returns>
	this.GetName = function()
	{
		return _browserName;
	}

	/// <method>
	/// Gets the browser version (e.g. 7.01, 6.2, 4.78, 4.06).
	/// </method>
	/// <returns type="Number">Returns the browser version.</returns>
	this.GetVersion = function()
	{
		return Number(_match[1]);
	}


	/// <method>
	/// Gets the major version of the browser (e.g. 7, 6, 4).
	/// </method>
	/// <returns type="Number">Returns the major version of the browser.</returns>
	this.GetMajor = function()
	{
		return Number(_match[2]);
	}


	/// <method>
	/// Gets the minor version of the browser (e.g. 0.01, 0.2, 0.78, 0.06).
	/// </method>
	/// <returns type="Number">Returns the minor version of the browser.</returns>
	this.GetMinor = function()
	{
		return Number(_match[3]);
	}
}


/// <property type="JSTools.Web.Browser.BrowserType">
/// Gets the active JSTools.Web.Browser.BrowserType instance.
/// </property>
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
	return null;
}


/// <property type="Array">
/// Default implemented JSTools.Web.Browser.BrowserType instances.
/// First match ($2) contains the version, second match ($3) the major and
/// the thrid match ($4) contains the minor version.
///
/// This array can be expanded, if necessary.
/// </property>
JSTools.Web.Browser.BrowserType.Types =
[
	// Internet Explorer 4.x +
	new JSTools.Web.Browser.BrowserType("^Mozilla[^(]*\\(compatible; MSIE ((\\d+)(\\.\\d+)).*$", "Internet Explorer"),

	// Netscape Navigator 4.x
	new JSTools.Web.Browser.BrowserType("^Mozilla/((\\d+)(\\.\\d+)) \\[.*\\].*$", "Netscape"),

	// Netscape Navigator 6.x
	new JSTools.Web.Browser.BrowserType("^Mozilla.* Netscape6/((\\d+)(\\.\\d+))$", "Netscape"),

	// Netscape Navigator 7.x
	new JSTools.Web.Browser.BrowserType("^Mozilla.* Netscape/((\\d+)(\\.\\d+))$", "Netscape"),

	// Opera 5.x + (as Opera)
	new JSTools.Web.Browser.BrowserType("^Opera/((\\d+)(\\.\\d+)).*$", "Opera"),

	// Opera 5.x + (as Mozilla)
	new JSTools.Web.Browser.BrowserType("^Mozilla.* Opera ((\\d+)(\\.\\d+)).*$", "Opera")
];
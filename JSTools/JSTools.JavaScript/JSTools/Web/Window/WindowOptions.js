function WindowOptions(intOptions)
{
	this.InitTypeManager(arguments);


	var _options	= intOptions;


	// returns all required fields in window.open like syntax
	this.GetArgumentString = function()
	{
		return ToString(
		((this.Height != null)	? "height="	+ this.Height + ","	: "") +
		((this.Width != null)	? "width=" + this.Width + ","	: "") +
		((this.Top != null)		? "top=" + this.Top + ","		: "") +
		((this.Left != null)	? "left=" + this.Left + ","		: "") +
		GetOptionString()).TrimEnd(",");
	}



	// returns a string which contains the option flags values
	function GetOptionString()
	{
		var optionString	= "";
		var flagOptions		= WindowOptions.Options.GetValues();

		for (var i = 0; i < flagOptions.length; ++i)
		{
			optionString += flagOptions[i].toLowerCase() + "=" + ((intOptions & WindowOptions.Options[flagOptions[i]]) ? "yes" : "no") + ",";
		}
		return optionString;
	}


	this.Url			= "about:blank";
	this.Name			= ToString(this.GetHashCode());
	this.Height			= null;
	this.Width			= null;
	this.Top			= null;
	this.Left			= null;

	// window closes, when the parent closes
	this.IsDependent	= false;
}
WindowOptions.prototype.toString = function()
{
    return "[object WindowOptions]";
}
WindowOptions.Options = new FlagsEnum(
	"Location",		// window contains a location bar
	"Menubar",		// window contains a menu bar
	"Resizable",	// window is resizable
	"Scrollbars",	// window contains the scroll bars
	"Status",		// window contains the status bar
	"Toolbar" );	// window contains the tool bar
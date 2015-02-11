function WindowErrorArgument(strMessage, strUrl, strLine)
{
	var _message		= ToString(strMessage);
	var _url			= ToString(strUrl);
	var _line			= ToNumber(strLine);
	var _returnValue	= false;


	// sets the value which the event will return
	this.SetReturnValue	= function(varValue)
	{
		_returnValue = varValue;
	}


	// returns the value which the event will return
	this.GetReturnValue	= function()
	{
		return _returnValue;
	}


	// returns the name of the event (e.g. onmouseover)
	this.GetEventName = function()
	{
		return "onerror";
	}


	// returns the id of the object on which is occured an event
	this.GetElementId = function()
	{
		return window.name;
	}


	// returns the error message
	this.GetMessage = function()
	{
		return _message;
	}


	// returns the url on which a script error has occured
	this.GetUrl = function()
	{
		return _url;
	}


	// returns the line on which a script error has occured
	this.GetLine = function()
	{
		return _line;
	}
}

WindowErrorArgument.prototype = new IEventArgument();

WindowErrorArgument.prototype.toString = function()
{
	return "[object WindowErrorArgument]";
}
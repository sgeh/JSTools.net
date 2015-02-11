function ConsoleErrorMessage(strMessage, strUrl, strLine, objErrorLogFunctionPtr)
{
	// init type manager, required for inheritance
	this.InitTypeManager(arguments);

	// inherit this object from other classes
	arguments.Call(ConsoleMessage, ConsoleWriter.MessageTypes.Error, ToString(strMessage) + "\n");

	// copies a reference of the protected members for using from outside the constructor
	var _protected = arguments.Protected;
	var _message = ToString(strMessage) + "\n";
	var _url = ToString(strUrl);
	var _line = ToString(strLine);


	// retruns the url, on which has an error occured
	this.GetUrl = function()
	{
		return _url;
	}


	// retruns the line, on which has an error occured
	this.GetLine = function()
	{
		return _line;
	}


	// returns the message, the type and the error informations in a console like string format
	this.ToLineString = function()
	{
		return _protected.ConsoleMessage.ToLineString() + " Line:  " + _line + "\n Url:   " + _url + "\n";
	}
}
function ConsoleMessage(strMessageType, strMessage)
{
	// init type manager, required for inheritance
	this.InitTypeManager(arguments);

	// copies a reference of the protected members for using from outside the constructor
	var _protected		= arguments.Protected;
	var _message		= ToString(strMessage);
	var _messageType	= strMessageType;


	// returns the message, which is given by the constructor or appended by the AppendMessage method
	this.GetMessage = function()
	{
		return _message;
	}


	// appends a string to the message, which is given by the constructor
	this.AppendMessage = function(strMessage)
	{
		_message += strMessage;
	}


	// overrides the message with a new value
	this.SetMessage = function(strValue)
	{
		_message = strValue;
	}


	// returns the message type (Warning, Message)
	this.GetMessageType = function()
	{
		return _messageType;
	}


	// returns the message and the type in a console like string format
	this.ToLineString = function()
	{
		return _protected.ToLineString();
	}


	// writes an anonym function pointer into the protected member container
	_protected.ToLineString = function()
	{
		return "~" + _messageType + ": " + _message;
	}
}
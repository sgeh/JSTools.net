function Exception(strMessage, objSender)
{
	var _exceptionMessage	= ToString(strMessage);
	var _logFunction		= window.Console.WriteWarning;
	var _debugObject		= objSender;

	if (typeof(objDebug) != 'object')
	{
		this.ThrowArgumentException("The given sender object contains an invalid reference!");
	}


	// returns the exception message
	this.GetMessage = function()
	{
		return _exceptionMessage;
	}


	// throws a new exception message as console warning
	this.Throw = function()
	{
		if (Exception.CanLog())
		{
			_logFunction(_exceptionMessage);
		}
		if (Exception.CanAlert())
		{
			if (window.confirm(_exceptionMessage) + "\n\nWould you like to get more informations?")
			{
				var objReflection = new ReflectionGenerator(_debugObject);
				objReflection.GetMembers();
				alert("Continue ->");
			}
		}
	}
}
Exception.prototype.toString = function()
{
	return "[object Exception]";
}

// returns true, if the DebugMode log flag is set to true
Exception.CanLog = function()
{
	return ToBoolean(Exception.DebugMode & Exception.Handling.LogError);
}


// returns true, if the DebugMode catch flag is set to true
Exception.CanCatch = function()
{
	return ToBoolean(Exception.DebugMode & Exception.Handling.CatchError);
}


// returns true, if the DebugMode alert flag is set to true
Exception.CanAlert = function()
{
	return ToBoolean(Exception.DebugMode & Exception.Handling.AlertError);
}


Exception.Handling			= new FlagsEnum(
	"AlertError",			// visualizes the error in a alert window
	"CatchError",			// all errors will be catched
	"None",					// no error handling
	"LogError" );			// log all errors

Exception.DebugMode			= Exception.Handling.None;
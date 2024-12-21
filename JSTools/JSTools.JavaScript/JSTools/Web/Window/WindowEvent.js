function WindowEvent()
{
	if (!IsUndefined(window.EventHandler))
	{
		this.ThrowRuntimeException("An instance of WindowEvent was already created.");
		return null;
	}

	var _errorEvent = new EventSubject("onerror");
	var _windowEventNames =
	[
		// window events
		"onblur", "onfocus", "onload", "onresize", "onunload"
	];

	var _documentEventNames =
	[
		// key events
		"onkeydown", "onkeypress", "onkeyup",
		// mouse events
		"onclick", "ondblclick", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup"
	];

	var _windowEvents = new ElementEvent(window, _windowEventNames);
	var _documentEvents = new ElementEvent(window.document, _documentEventNames);

	this.DocumentLoaded = false;
	this.MouseX = null;
	this.MouseY = null;


	// adds an error event trigger to the error subject container
	this.AddErrorListener = function(objEvent, strEventIdentifier)
	{
		return _errorEvent.Attach(objEvent, strEventIdentifier);
	}


	// removes an error event trigger from the error subject container
	this.RemoveErrorListener = function(strEventIdentifier)
	{
		_errorEvent.Detach(strEventIdentifier);
	}


	// removes an error event trigger from the error subject container
	this.RemoveErrorListenerByIndex = function(intFunctionIndex)
	{
		_errorEvent.DetachByIndex(intFunctionIndex);
	}


	// adds an event to the event subject container
	this.AddEventListener = function(strEventName, objEvent, strEventIdentifier)
	{
		var eventContainer = GetEventContainer(strEventName);

		if (eventContainer != null)
		{
			var eventIndex = eventContainer.Attach(strEventName, objEvent, strEventIdentifier);
			return eventIndex;
		}
		return null;
	}


	// removes an event from the event subject container
	this.RemoveEventListener = function(strEventName, strEventIdentifier)
	{
		var eventContainer = GetEventContainer(strEventName);

		if (eventContainer != null)
		{
			eventContainer.Detach(strEventName, strEventIdentifier);
		}
	}


	// removes an event from the event subject container
	this.RemoveEventListenerByIndex = function(strEventName, intFunctionIndex)
	{
		var eventContainer = GetEventContainer(strEventName);

		if (eventContainer != null)
		{
			eventContainer.DetachByIndex(strEventName, intFunctionIndex);
		}
	}



	// returns the event container which contains the given strEventName
	function GetEventContainer(strEventName)
	{
		if (_windowEventNames.Contains(strEventName))
		{
			return _windowEvents;
		}
		if (_documentEventNames.Contains(strEventName))
		{
			return _documentEvents;
		}
		return null;
	}


	// occurs if the page has completely finished loading
	function OnDocumentLoad()
	{
		window.EventHandler.DocumentLoaded = true;
	}


	// occurs if the user has moved the mouse over the document
	function OnMouseMove(objArgument)
	{
		window.EventHandler.MouseX = objArgument.GetMouseX();
		window.EventHandler.MouseY = objArgument.GetMouseY();
	}


	// occurs if a script error is thrown
	function OnError(strMessage, strUrl, strLine)
	{
		var errorArgs = new WindowErrorArgument(strMessage, strUrl, strLine);
		_errorEvent.FireUpdate(errorArgs);
		return errorArgs.GetReturnValue();
	}

	this.AddEventListener("onload", OnDocumentLoad);
	this.AddEventListener("onmousemove", OnMouseMove);
//	window.onerror = OnError;
}
WindowEvent.prototype.toString = function()
{
	return "[object WindowEvent]";
}

window.EventHandler = new WindowEvent();
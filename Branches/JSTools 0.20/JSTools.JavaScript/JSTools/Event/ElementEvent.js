function ElementEvent(objElement, arrEventNames)
{
	if (typeof(objElement) != 'object' || IsUndefined(arrEventNames) || IsUndefined(arrEventNames.length))
	{
		this.ThrowArgumentException("objElement must be a valid object and arrEventNames must be a valid array!");
		return null;
	}

	// init type manager, required for inheritance
	this.InitTypeManager(arguments);

	// inherit this object from other classes
	arguments.Call(BaseEventContainer);

	// copies a reference of the protected members for using from outside the constructor
	var _protected			= arguments.Protected;
	var _this				= this;
	var _elementObject		= objElement;

	with (_protected.BaseEventContainer)
	{
		EventTypes	= new Array();
		EventNames	= arrEventNames;
		CreateEventSubjects(EventTypes, EventNames);
	}



	// enables event bubbling and capturing for ie and ns
	this.EventForwarding		= true;


	// starts the event capturing for a specified event
	// if no event name was given, all events will be captured
	this.StartCapturing = function(strEventName)
	{
		HandleCapturing(strEventName, AddEventToObject);
	}


	// releases the event capturing for a specified event
	// if no event name was given, all events will be released
	this.ReleaseCapturing = function(strEventName)
	{
		HandleCapturing(strEventName, RemoveEventFromObject);
	}


	// attaches an event function to the specified container.
	// if the event was added successfully, this function returns the index
	// of the new EventItem in the strName container, otherwise it returns null
	this.Attach = function(strContainerName, objEvent, strFunctionName)
	{
		if (_protected.BaseEventContainer.IsValidContainer(strContainerName))
		{
			var eventCount = _protected.BaseEventContainer.EventTypes[strContainerName].Attach(objEvent, strFunctionName);

			if (!IsNull(eventCount) && !_elementObject[strContainerName])
			{
				this.StartCapturing(strContainerName);
			}
			return eventCount;
		}
		return null;
	}


	// detaches an event function from the specified container with the given name
	this.Detach = function(strContainerName, strFunctionName)
	{
		if (_protected.BaseEventContainer.IsValidContainer(strContainerName))
		{
			CheckReleaseEvents(_protected.BaseEventContainer.EventTypes[strContainerName].Detach(strFunctionName), strContainerName);
		}
	}


	// detaches an event function from the specified container with the given index
	this.DetachByIndex = function(strContainerName, intFunctionIndex)
	{
		if (_protected.BaseEventContainer.IsValidContainer(strContainerName))
		{
			CheckReleaseEvents(_protected.BaseEventContainer.EventTypes[strContainerName].DetachByIndex(intFunctionIndex), strContainerName);
		}
	}


	// fires an event and creates an EventArgument object
	// this function should not be used from global code
	this.FireUpdate = function(objEvent)
	{
		var eventArgument = GetEventArgument(objEvent);

		if (_protected.BaseEventContainer.IsValidContainer(eventArgument.GetEventName(), _protected.BaseEventContainer.EventTypes) && !IsNull(eventArgument.GetEventName()))
		{
			_protected.BaseEventContainer.EventTypes[eventArgument.GetEventName()].FireUpdate(eventArgument);
		}

		EventForwarding(objEvent);
		return eventArgument.ReturnValue;
	}


	// returns a valid EventArgument instance
	function GetEventArgument(objEvent)
	{
		return ((objEvent == "[object EventArgument]") ? objEvent : new EventArgument(objEvent));
	}


	// handles the event capturing procedure
	function HandleCapturing(strEventName, objFunction)
	{
		if (!IsUndefined(strEventName))
		{
			objFunction(strEventName);
		}
		else
		{
			for (var i = 0; i < _protected.BaseEventContainer.EventNames.length; ++i)
			{
				objFunction(_protected.BaseEventContainer.EventNames[i]);
			}
		}
	}


	// adds an event to the element object
	function AddEventToObject(strName)
	{
		var nsEventName = GetNSEventName(strName);

		if (window.Browser.GetType().IsNS && !IsUndefined(Event[nsEventName]))
		{
			_elementObject.captureEvents(Event[nsEventName]);
		}

		_elementObject[strName] = _this.FireUpdate;
	}


	// removes an event from the element object
	function RemoveEventFromObject(strName)
	{
		var nsEventName = GetNSEventName(strName);

		if (window.Browser.GetType().IsNS && !IsUndefined(Event[nsEventName]))
		{
			_elementObject.releaseEvents(Event[nsEventName]);
		}
		_elementObject[strName] = null;
	}


	// returns the event name without the "on" prefix
	function GetNSEventName(strEventName)
	{
		var returnValue = strEventName;

		if (strEventName.indexOf("on") == 0)
		{
			returnValue = strEventName.substring(2, strEventName.length);
		}
		return returnValue.toUpperCase();
	}


	// if the count of an eventSubject container is 0, this function will
	// release the event capturing
	function CheckReleaseEvents(intItemCount, strContainerName)
	{
		if (intItemCount == 0)
		{
			_this.ReleaseCapturing(strContainerName);
		}
	}


	// enables event forwarding for ie and ns
	function EventForwarding(objEvent)
	{
		if (window.Browser.GetType().IsNS)
		{
			if (ToBoolean(_this.EventForwarding))
			{
				_elementObject.routeEvent(objEvent);
			}
		}
		else
		{
			window.event.cancelBubble = !ToBoolean(_this.EventForwarding);
		}
	}
}
function EventContainer(arrEventNames)
{
	if (IsUndefined(arrEventNames) || IsUndefined(arrEventNames.length))
	{
		this.ThrowArgumentException("arrEventNames must be a valid array!");
		return null;
	}


	// init type manager, required for inheritance
	this.InitTypeManager(arguments);

	// inherit this object from other classes
	arguments.Call(BaseEventContainer);

	// copies a reference of the protected members for using from outside the constructor
	var _protected		= arguments.Protected;

	with (_protected.BaseEventContainer)
	{
		EventTypes	= new Array();
		EventNames	= arrEventNames;
		CreateEventSubjects(EventTypes, EventNames);
	}


	// attaches an event function to the specified container.
	// if the event was added successfully, this function returns the index
	// of the new EventItem in the strName container, otherwise it returns null
	this.Attach = function(strContainerName, objEvent, strFunctionName)
	{
		if (_protected.BaseEventContainer.IsValidContainer(strContainerName))
		{
			return _protected.BaseEventContainer.EventTypes[strContainerName].Attach(objEvent, strFunctionName);
		}
		return null;
	}


	// detaches an event function from the specified container with the given name
	this.Detach = function(strContainerName, strFunctionName)
	{
		if (_protected.BaseEventContainer.IsValidContainer(strContainerName))
		{
			_protected.BaseEventContainer.EventTypes[strContainerName].Detach(strFunctionName);
		}
	}


	// detaches an event function from the specified container with the given index
	this.DetachByIndex = function(strContainerName, intFunctionIndex)
	{
		if (_protected.BaseEventContainer.IsValidContainer(strContainerName))
		{
			_protected.BaseEventContainer.EventTypes[strContainerName].DetachByIndex(intFunctionIndex);
		}
	}


	// fires an event and creates an EventArgument object
	// this function should not be used from global code
	this.FireUpdate = function(strContainerName, objEventArgument)
	{
		if (_protected.BaseEventContainer.IsValidContainer(strContainerName))
		{
			_protected.BaseEventContainer.EventTypes[strContainerName].FireUpdate(objEventArgument);
		}
	}
}
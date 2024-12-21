function BaseEventContainer()
{
	// init type manager, required for inheritance
	this.InitTypeManager(arguments);

	// copies a reference of the protected members for using from outside the constructor
	var _protected			= arguments.Protected;
	_protected.EventTypes	= new Array();
	_protected.EventNames	= null;


	// creates the EventSubject objects with the specified event names
	_protected.CreateEventSubjects = function(arrEventTypes, arrEventNames)
	{
		for (var i = 0; i < arrEventNames.length; ++i)
		{
			arrEventNames[i]				= String.ToEnumerable(arrEventNames[i]);
			arrEventTypes[arrEventNames[i]]	= new EventSubject(arrEventNames[i]);
			arrEventTypes.Add(arrEventTypes[arrEventNames[i]]);
		}
	}


	// checks if the strName is a valid name of a container item
	_protected.IsValidContainer = function(strName)
	{
		if (!IsUndefined(strName) && !IsUndefined(_protected.EventTypes[String.ToEnumerable(strName)]))
		{
			return true;
		}
		else
		{
			this.ThrowRuntimeException("Could not find the specified container '" + String.ToEnumerable(strName) + "' in the current event object!");
			return false;
		}
	}



	// adds a new event to the _protected.EventNames and _protected.EventTypes container.
	this.AddEvent = function(strContainerName)
	{
		var newContainerName = String.ToEnumerable(strContainerName);
		_protected.EventNames.Add(newContainerName);

		_protected.EventTypes[newContainerName] = new EventSubject(newContainerName);
		_protected.EventTypes.Add(_protected.EventTypes[newContainerName]);
	}


	this.Attach			= function() { }
	this.Detach			= function() { }
	this.DetachByIndex	= function() { }
	this.FireUpdate		= function() { }
}
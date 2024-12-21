function EventSubject(strEventName)
{
	var _eventName			= (!IsUndefined(strEventName) ? strEventName : "");
	var _storedEvents		= new Array();
	var _storedEventsCount	= 0;


	// attaches an event to the _storedEvents array.
	this.Attach = function(objEvent, strIdentifer)
	{
		var newEventItem = new EventItem(objEvent, strIdentifer);

		if (ToString(newEventItem) == "[object EventItem]")
		{
			_storedEvents.Add(newEventItem);
			_storedEventsCount++;
			return _storedEvents.length - 1;
		}
		return null;
	}


	// detaches an event from _storedEvents container and
	// calls the DetachByIndex method if an object was found
	this.Detach = function(strName)
	{
		return this.DetachByIndex(GetItemNameIndex(strName));
	}


	// detaches an event from _storedEvents container by the index
	this.DetachByIndex = function(intIndex)
	{
		if (_storedEvents.IsValidIndex(intIndex) && _storedEventsCount > 0)
		{
			_storedEvents[intIndex] = null;
			return --_storedEventsCount;
		}
		return null;
	}


	// fires an event to all registered functions
	this.FireUpdate = function(objEvent)
	{
		for (var i = 0; i < _storedEvents.length; ++i)
		{
			_storedEvents[i].GetItemObject()((!IsUndefined(objEvent) ? objEvent : null));
		}
	}


	// returns the event name (e.g. "onmouseover")
	this.GetEventName = function()
	{
		return _eventName;
	}



	// returns true, if the function contains an object with the specified name
	function GetItemNameIndex(strName)
	{
		for (var i = _storedEvents.length - 1; i >= 0 && _storedEvents[i].GetItemName() != strName; --i)
		{
			;
		}
		return i;
	}
}
EventSubject.prototype.toString = function()
{
	return "[object EventSubject]";
}
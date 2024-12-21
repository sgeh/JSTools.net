function IEventArgument()
{
	// returns the value which the event will return
	this.GetReturnValue	= function() { };

	// sets the value which the event will return
	this.SetReturnValue	= function() { };

	// returns the name of the event (e.g. onmouseover)
	this.GetEventName = function() { }

	// returns the id of the object on which is occured an event
	this.GetElementId = function() { }
}
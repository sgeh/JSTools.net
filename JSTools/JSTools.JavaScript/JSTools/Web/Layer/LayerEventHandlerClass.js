function LayerEventHandler(objAttach, strLayerId)
{
	var _objAttach		= objAttach;
	var _strLayerId		= strLayerId;
	var _newEvents		= new EventLocator(this, _strLayerId);

	var _eventName		= "";
	var _triggerName	= "";
	var _eventObject	= null;
	var _eventService	= false;
	var _eventContainer	= new Array();


	// public function to handle an event
	this.HandleEvent = function(intEventNumber, objHandle, strTriggerName, blnIsService)
	{
		_eventService	= blnIsService;
		_triggerName	= ToString(strTriggerName).ToEnumerable();
		_eventObject	= objHandle;

		SearchLayerDefinedEvents(intEventNumber, "predefined", HandleRegistration);
		SearchLayerDefinedEvents(intEventNumber, "layerdefined", HandleRegistration);
		SearchLayerDefinedEvents(intEventNumber, "newdefined", HandleCombinedRegistration);
	}


	// delegates an event
	this.DelegateEvent = function(strContainerName, objEvent)
	{
		DelegateAllContainerEvents(strContainerName, objEvent);
	}


	// delegates all events in the _eventContainer[strContainerName] container
	function DelegateAllContainerEvents(strContainerName, objEvents)
	{
		if(!IsUndefined(_eventContainer[strContainerName]))
		{
			for(var i = 0; i < _eventContainer[strContainerName].length; ++i)
			{
				if(_eventContainer[strContainerName][i])
				{
					DelegateCurrentEvent(_eventContainer[strContainerName][_eventContainer[strContainerName][i]], objEvents, strContainerName);
				}
			}
		}
	}


	// checks if the current event is valid
	function DelegateCurrentEvent(objDelegateFunction, objLayerEvent, strType)
	{
		if(!IsUndefined(objDelegateFunction))
		{
			objDelegateFunction(GetEventDescriptionObject(objDelegateFunction, objLayerEvent, strType));
		}
	}


	// returns if the strEventType is a predefined event a new LayerMouseEvent
	function GetEventDescriptionObject(objDelegate, objEventDescription, strEventType)
	{
		return ((objDelegate.IsPreDefinedEvent) ? new LayerMouseEvent(objEventDescription, _strLayerId, strEventType) : objEventDescription);
	}


	// searches the incoming event in the LayerEnumeratorClass and delegate the event to the objRegister object
	function SearchLayerDefinedEvents(intEvent, strEventRange, objRegister)
	{
		var thisArray = LayerEvent.GetEventRange(strEventRange);

		for(var i = 0; i < thisArray.length; ++i)
		{
			if(intEvent & LayerEvent[thisArray[i]])
			{
				ChangeEventName(intEvent);
				objRegister(thisArray[i], (strEventRange == "predefined"));
			}
		}
	}


	// sets the _eventName to the current event name
	function ChangeEventName(intEventName)
	{
		_eventName = LayerEvent.GetEventName(intEventName);
	}


	// handles the combined events e.g. drag or drop
	function HandleCombinedRegistration(strNSEvent)
	{
		HandleRegistration(strNSEvent, false);

		if(!_newEvents.IsInitialized)
		{
			_newEvents.InitializeLocator(_eventName);
		}
	}


	// if _eventObject is null the current event will removed else it will added to the _eventContainer
	function HandleRegistration(strEventForNS, blnEventKind)
	{
		if(IsNull(_eventObject))
		{
			StartRemovingEvent(strEventForNS, blnEventKind);
		}
		else
		{
			StartAddingEvent(strEventForNS, blnEventKind);
		}
	}


	// removes an event form the _eventContainer
	function StartRemovingEvent(strNSEvent, blnIsPreDefinedEvent)
	{
		if(!DeleteEventInContainer() && blnIsPreDefinedEvent)
		{
			WatchEvent(null, strNSEvent, "releaseEvents");
		}
	}


	// adds an event to the _eventContainer
	function StartAddingEvent(strNSEvent, blnIsPreDefinedEvent)
	{
		if(!WriteEventToContainer(blnIsPreDefinedEvent) && blnIsPreDefinedEvent)
		{
			WatchEvent(window.document.Layer[_strLayerId].GetLayerEventObject()["layerEvent" + ToString(_eventName).toUpperCase()], strNSEvent, "captureEvents");
		}
	}


	// start event watching
	function WatchEvent(objWrite, strNSEvent, objNSCapture)
	{
		_objAttach[_eventName] = objWrite;

		if(Browser.GetTyp().isNC && !Browser.GetTyp().isDOM)
		{
			_objAttach[objNSCapture](Event[strNSEvent]);
		}
	}


	// deletes an event from the _eventContainer
	function DeleteEventInContainer()
	{
		if(!IsUndefined(_eventContainer[_eventName]) && !IsUndefined(_eventContainer[_eventName][_triggerName]))
		{
			if(!_eventContainer[_eventName][_triggerName].IsJavaScriptEventService)
			{
				delete _eventContainer[_eventName][_triggerName];
				_eventContainer[_eventName].Remove(_eventContainer[_eventName].Search(_eventContainer[_eventName]));
			}
		}
		return _eventContainer[_eventName].length - 1;
	}


	// adds an event from the _eventContainer
	function WriteEventToContainer(blnPreDefined)
	{
		if(IsUndefined(_eventContainer[_eventName]))
		{
			_eventContainer[_eventName] = new Array();
			SetEventFunction(_eventName);
		}

		_eventContainer[_eventName].Add(_triggerName);
		_eventContainer[_eventName][_triggerName]							= _eventObject;
		_eventContainer[_eventName][_triggerName].IsPreDefinedEvent			= ToBoolean(blnPreDefined);
		_eventContainer[_eventName][_triggerName].IsJavaScriptEventService	= ToBoolean(_eventService);

		return _eventContainer[_eventName].length - 1;
	}


	// writes a new public property to this object
	function SetEventFunction(strEvent)
	{
		eval('window.document.Layer.' + _strLayerId + '.GetLayerEventObject()["layerEvent' + ToString(strEvent).toUpperCase() + '"] = function(objNewDelegateEvent) { DelegateAllContainerEvents("' + strEvent + '", objNewDelegateEvent); }');
	}


	// event locator for drag and drop events
	function EventLocator(objEventHandler, strLayerId)
	{
		var _eventHandler	= objEventHandler;
		var _strLayerId		= strLayerId;
		var _systemEvents	= [LayerEvent.MOUSEUP, LayerEvent.MOUSEDOWN, LayerEvent.MOUSEMOVE, LayerEvent.MOUSEOVER];
		var _eventsIndexer	= new Array();

		var _returnValue	= true;
		var _mouseDown		= false;
		var _isDraged		= false;

		this.IsInitialized	= false;


		// if layer onmouseup
		this.SystemONMOUSEUP = function(objEvent)
		{
			if(_isDraged && _mouseDown)
			{
				_isDraged = false;
				_returnValue = true;
				DelegateDragDropEvent(LayerEvent.DROP, objEvent);
			}

			_mouseDown	= false;
		}


		// if layer onmousedown
		this.SystemONMOUSEDOWN = function(objEvent)
		{
			_mouseDown	= true;
		}


		// if layer onmousemove
		this.SystemONMOUSEMOVE = function(objEvent)
		{
			if(_mouseDown && !_isDraged)
			{
				_isDraged = true;
				_returnValue = false;
				DelegateDragDropEvent(LayerEvent.DRAG, objEvent);
			}
		}


		// if layer onmouseover
		this.SystemONMOUSEOVER = function(objEvent)
		{
			_mouseDown	= false;
			_isDraged	= false;
		}


		// initializes the current object
		this.InitializeLocator = function(strEventName)
		{
			var itemName;

			for(var i = 0; i < _systemEvents.length; ++i)
			{
				itemName = "System" + LayerEvent.GetEventName(_systemEvents[i]).toUpperCase();
				_eventsIndexer[i] = _eventHandler.HandleEvent(_systemEvents[i], this[itemName], itemName, true);
			}
			this.IsInitialized = true;
		}


		// delegates a drag or drop event to the layerevent object
		function DelegateDragDropEvent(intEventNumber, objBrowserEvent)
		{
			SetReturnValue();

			var eventName = LayerEvent.GetEventName(intEventNumber);
			_eventHandler.DelegateEvent(eventName, objBrowserEvent);
		}


		// sets the return value for drag'n'drop events
		function SetReturnValue()
		{
			if(Browser.GetTyp().IsIE)
			{
				event.returnValue = _returnValue;
			}
		}
	}
}
LayerEventHandler.prototype.toString = function()
{
	return "[object LayerEventHandler]";
}
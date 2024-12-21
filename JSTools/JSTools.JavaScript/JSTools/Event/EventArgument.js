function EventArgument(objEvent)
{
	var _this			= this;
	var _eventObj		= (IsUndefined(objEvent) ? window.event : objEvent);
	var _eventName		= (!IsUndefined(_eventObj.type) ? "on" + ToString(_eventObj.type).toLowerCase() : null);

	var _baseElementId	= null;
	var _mouseX			= null;
	var _mouseY			= null;
	var _button			= null;
	var _key			= null;
	var _returnValue	= false;


	// sets the value which the event will return
	this.SetReturnValue	= function(varValue)
	{
		_returnValue = varValue;
	}


	// returns the value which the event will return
	this.GetReturnValue	= function()
	{
		return _returnValue;
	}


	// returns the name of the event (e.g. onmouseover)
	this.GetEventName = function()
	{
		return _eventName;
	}


	// returns the pressed mouse button
	this.GetButton = function()
	{
		return _button;
	}


	// returns the left mouse position
	this.GetMouseX = function()
	{
		return _mouseX;
	}


	// returns the top mouse position
	this.GetMouseY = function()
	{
		return _mouseY;
	}


	// returns the event object
	this.GetEvent = function()
	{
		return _eventObj;
	}


	// returns the id of the object on which is occured an event
	this.GetElementId = function()
	{
		return _baseElementId;
	}


	// returns the id of the object on which is occured an event
	this.GetKeyCode = function()
	{
		return _key;
	}


	// initializes the values for ie and ns
	function InitValues()
	{
		if (_eventObj)
		{
			if (window.Browser.GetType().IsNS)
			{
				InitNCValues();
			}
			else
			{
				InitDOMValues();
			}
		}
	}


	// initializes ns event values
	function InitNCValues()
	{
		_baseElementId	= (_eventObj.target ? _eventObj.target.id : null);
		_mouseX			= _eventObj.pageX;
		_mouseY			= _eventObj.pageY;
		_key			= GetValidValue("key", _eventObj.which);
		_button			= GetValidValue("mouse", _eventObj.which);
	}


	// initializes ie event values
	function InitDOMValues()
	{
		_baseElementId	= (_eventObj.srcElement ? _eventObj.srcElement.id : null);
		_mouseX			= _eventObj.clientX + document.body.scrollLeft;
		_mouseY			= _eventObj.clientY + document.body.scrollTop;
		_key			= GetValidValue(/key/, _eventObj.button);
		_button			= GetValidValue(/(mouse|click)/, _eventObj.keyCode);
	}


	// if objValue is undefined this function will return null, otherwise the objValue
	function GetValidValue(regExpression, objValue)
	{
		return ((_eventName.search(regExpression) != -1) ? objValue : null);
	}


	if (!IsNull(_eventName))
	{
		InitValues();
	}
}

EventArgument.prototype = new IEventArgument();

EventArgument.prototype.toString = function()
{
	return "[object EventArgument]";
}
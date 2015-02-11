function LayerEnumerator()
{
	this.MOVE = 0x00000000;
	this.CLICK = 0x00000001;
	this.DBLCLICK = 0x00000002;
	this.LOAD = 0x00000004;
	this.MOUSEDOWN = 0x00000008;
	this.MOUSEOUT = 0x00000010;
	this.MOUSEOVER = 0x00000020;
	this.MOUSEUP = 0x00000001;
	this.MOUSEMOVE = 0x00000040;
	this.RESIZE = 0x00000080;
	this.INITIALIZE = 0x00000100;
	this.DRAG = 0x00000200;
	this.DROP = 0x00000400;



	// returns name of the specified event number
	this.GetEventName = function(intEventNumber)
	{
		var eventName = null;

		if(!IsUndefined(_eventNames[intEventNumber]))
		{
			eventName = _eventNames[intEventNumber];
		}
		return eventName;
	}


	// returns the specified event range
	this.GetEventRange = function(strRangeName)
	{
		var rangeArray = null;

		if(!IsUndefined(strRangeName))
		{
			rangeArray = _eventRanges[strRangeName];
		}
		return rangeArray;
	}


	var _eventRanges = new Array();
	_eventRanges["predefined"] = ["DBLCLICK", "MOUSEDOWN", "MOUSEOUT", "MOUSEOVER", "MOUSEUP", "MOUSEMOVE"];
	_eventRanges["layerdefined"] = ["MOVE", "LOAD", "RESIZE", "INITIALIZE"];
	_eventRanges["newdefined"] = ["DROP", "DRAG"];


	var _eventNames = new Array();
	_eventNames[this.MOVE] = "onmove";
	_eventNames[this.DBLCLICK] = "ondblclick";
	_eventNames[this.LOAD] = "onload";
	_eventNames[this.MOUSEDOWN] = "onmousedown";
	_eventNames[this.MOUSEOUT] = "onmouseout";
	_eventNames[this.MOUSEOVER] = "onmouseover";
	_eventNames[this.MOUSEUP] = "onmouseup";
	_eventNames[this.MOUSEMOVE] = "onmousemove";
	_eventNames[this.RESIZE] = "onresize";
	_eventNames[this.INITIALIZE] = "oninitialize";
	_eventNames[this.DRAG] = "ondrag";
	_eventNames[this.DROP] = "ondrop";
}
LayerEnumerator.prototype.toString = function()
{
	return "[object LayerEnumerator]";
}

window.LayerEvent = new LayerEnumerator();
function LayerEvents(objContainer)
{
	if (IsUndefined(objContainer))
	{
		this.ThrowArgumentException("Could not find the given layer container instance. Please do not create a LayerEvents instance manually.");
		return null;
	}
/*
	var _this			= this;
	var _moveEvent		= "onmousemove";
	var _clickEvent		= ["onclick", "ondblclick", "onmousedown"];

	var _selectedLayers	= new Array();


	this.GetSelectedLayers = function()
	{
		return _selectedLayers;
	}

	this.CaptureEvents	= true;


	function OnClick()
	{
	}


	function OnMove(objEvent)
	{
		if (_this.CaptureEvents)
		{
			CheckForMouseOver(objEvent);
		}
	}


	function CheckForMouseOver(objEvent)
	{
		var layerNames		= window.document.Layers.GetLayerNames();
		var layerOver		= new Array();

		for (var i = 0; i < layerNames.length; ++i)
		{
			if (window.document.Layers[layerNames[i]].IsLoaded && IsValidPosition(window.document.Layers[layerNames[i]], objEvent.GetMouseY(), objEvent.GetMouseX()))
			{
				layerOver.Add(window.document.Layers[layerNames]);
			}
		}
	}


	function IsValidPosition(objLayer, intMouseTop, intMouseLeft)
	{
		return (IsValidTopMargin(objLayer, intMouseTop) && IsValidLeftMargin(objLayer, intMouseLeft));
	}


	function IsValidTopMargin(objLayer, intMouseTop)
	{
		return (intMouseTop >= objLayer.GetTopMargin() && intMouseTop <= objLayer.GetTopMargin() + objLayer.GetHeight());
	}

	function IsValidLeftMargin(objLayer, intMouseLeft)
	{
		return (intMouseLeft >= objLayer.GetLeftMargin() && intMouseLeft <= objLayer.GetLeftMargin() + objLayer.GetWidth());
	}


	// adds the required events to the window object
	function AddEventsToWindow()
	{
		for (var i = 0; i < _clickEvent.length; ++i)
		{
			window.EventHandler.AddEventListener(_clickEvent[i], OnClick);
		}
		window.EventHandler.AddEventListener(_moveEvent, OnMove);
	}
	AddEventsToWindow();
*/
}
LayerEvents.prototype.toString = function()
{
    return "[object LayerEvents]";
}
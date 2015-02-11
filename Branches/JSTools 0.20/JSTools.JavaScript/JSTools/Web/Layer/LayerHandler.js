function LayerHandler(strLayerId, intIndex, strCssClass)
{
	var _this				= this;
	var _index				= ToNumber(intIndex);
	var _identifier			= ToString(strLayerId);
	var _name				= _identifier.ToEnumerable();

	var _cssClass			= (!IsUndefined(strCssClass) ? strCssClass : null);
	var _callTable			= new Array();
	var _layerModel			= null;

	// gets the model factory and writes it into the _layerModel variable
	if (LayerHandler.GetFactoryDescription() != null)
	{
		_layerModel = LayerHandler.GetFactoryDescription().GetFactory(_this);
	}

	// if the arguments are false or undefined, a ArgumentException will be thrown
	if (IsUndefined(strLayerId) || IsUndefined(intIndex))
	{
		this.ThrowArgumentException("The given arguments are not well formatted. Please create a layer with the window.document.CreateLayer() method.");
		return null;
	}

	// if the browser could not be identified, a RuntimeException will be thrown
	if (_layerModel == null)
	{
		this.ThrowRuntimeException("Invalid web browser detected. JSLayer Tools are not compatible with your browser!");
		return null;
	}


	// contains the original values, which are written at the load event
	this.OriginalValues = null;


	// adds a new event function to the event container. returns the index number
	// if the adding process was successfull, otherwise null
	this.AddEventListener = function(strEventName, objEventCall, strEventFunctionName)
	{
		return _layerModel.ExternEvents.Attach(strEventName, objEventCall, strEventFunctionName);
	}


	// removes an event from the event container
	this.RemoveEventListener = function(strEventName, strEventFunctionName)
	{
		_layerModel.ExternEvents.Detach(strEventNamem, strEventFunctionName);
	}


	// removes the specified index from the event container
	this.RemoveEventListenerByIndex = function(strEventName, intFunctionIndex)
	{
		_layerModel.ExternEvents.DetachByIndex(strEventName, intFunctionIndex);
	}


	// returns the loading status of this layer object
	this.GetStatus = function()
	{
		return _layerModel.Status;
	}


	// returns the id in an enumerable format. this name is equal with the meaning
	// of the index
	this.GetName = function()
	{
		return _name;
	}


	// returns the index of the LayerContainer collection
	this.GetIndex = function()
	{
		return _index;
	}


	// returns the layer object id
	this.GetID = function()
	{
		return _identifier;
	}


	// returns the visibility of the object
	// true means the element is visible, false
	// means the element is hidden
	this.GetVisibility = function()
	{
		return _layerModel.ValueGetter.Visibility;
	}


	// returns the content of the layer. the default
	// value of this property is [nothing]
	this.GetContent = function()
	{
		return _layerModel.ValueGetter.Content;
	}


	// returns the width of the layer
	this.GetWidth = function()
	{
		return _layerModel.ValueGetter.Width;
	}


	// returns the height of the layer
	this.GetHeight = function()
	{
		return _layerModel.ValueGetter.Height;
	}


	// returns the layer clip object
	this.GetClip = function()
	{
		var clip = _layerModel.ValueGetter.Clip;
		return ((clip != null) ? new LayerClip(this, clip.Top, clip.Left, clip.Bottom, clip.Right) : null);
	}


	// returns the z-index of the layer
	this.GetZIndex = function()
	{
		return _layerModel.ValueGetter.ZIndex;
	}


	// returns the margin to the top border
	this.GetTopMargin = function()
	{
		return _layerModel.ValueGetter.TopMargin;
	}


	// returns the margin to the left border
	this.GetLeftMargin = function()
	{
		return _layerModel.ValueGetter.LeftMargin;
	}


	// sets the visibility of this layer. false means the layer hides,
	// true means the layer visualizes
	this.SetVisibility = function(blnVisibility)
	{
		if (IsLayerReady(blnVisibility))
		{
			with(_layerModel)
			{
				ValueGetter.Visibility = ToBoolean(blnVisibility);
				ValueSetter.SetVisibility(ValueGetter.Visibility);
			}
		}
	}


	// writes the content of the layer object. fires an intern "onrefresh"
	// event.
	this.SetContent = function(strContent)
	{
		if (IsLayerReady(strContent))
		{
			with(_layerModel)
			{
				ValueGetter.Content = ToString(strContent);
				ValueSetter.SetContent(ValueGetter.Content);
			}
		}
	}


	this.SetWidth = function()
	{
	}


	this.SetHeight = function()
	{
	}


	// visualizes the layer object
	this.Show = function()
	{
		this.SetVisibility(true);
	}


	// hides the layer object
	this.Hide = function()
	{
		this.SetVisibility(false);
	}


	// sets the clip of the layer
	this.SetClip = function(strPosition, intValue)
	{
		if (IsNumeric(intValue) && LayerClip.Names.Contains(strPosition) && IsLayerReady(strPosition, intValue))
		{
			with (_layerModel)
			{
				ValueGetter.Clip[strPosition] = ToNumber(intValue);
				ValueSetter.SetClip(ValueGetter.Clip);
			}
		}
	}


	this.SetZIndex = function()
	{
	}


	this.SetTopMargin = function()
	{
	}


	this.SetLeftMargin = function()
	{
	}


	this.MoveTo = function()
	{
	}


	this.ResizeTo = function()
	{
	}


	// fires the intern construct event
	this.Construct = function()
	{
		_layerModel.InternEvents.FireUpdate("onconstruct");
	}


	// fires an extern event
	this.FireEvent = function(strEvent)
	{
		_layerModel.InternEvents.FireUpdate(strEvent);
	}


	// returns true, if the model status is equal to "complete"
	// this means, the layer object is loaded
	this.IsLoaded = function()
	{
		return (_layerModel.Status == LayerHandler.States.Complete);
	}



	// occurs when the layer is completely loaded
	function OnLoad()
	{
		CallFunctions();
		_this.OriginalValues = new LayerOriginalValues(_this);
		_layerModel.ChangeStatus(LayerHandler.States.Complete, null);
		_layerModel.ExternEvents.FireUpdate("onload", _this);
	}


	// executes all calls in the _callTable
	function CallFunctions()
	{
		for (var i = 0; i < _callTable.length; ++i)
		{
			var argumentString = "";

			for (var j = 0; j < _callTable[i].Arguments.length; ++j)
			{
				argumentString += "_callTable[i].Arguments[" + j + "]" + ((j + 1 != _callTable[i].Arguments.length) ? "," : "");
			}
			eval("_callTable[i].Method(" + argumentString + ");");
		}
	}


	// returns true, if the layer is ready for writing
	// if the user would like to write a value into the layer before
	// finishing the loading process, the call will be written in a
	// temporary table called _callTable.
	function IsLayerReady()
	{
		if (!_this.IsLoaded())
		{
			_callTable.Add( { Method: arguments.callee.caller, Arguments: arguments } );
			return false;
		}
		else
		{
			return true;
		}
	}

	_layerModel.InternEvents.Attach("onload", OnLoad);
}
LayerHandler.prototype.toString = function()
{
	return "[object LayerHandler]";
}

// make sure that the LayerHandler implements all ILayerValueCollection members
LayerHandler.prototype		= new ILayerValueContainer();

// contains a reference to the ModelFactory types.
LayerHandler.ModelFactories	= new Array();
LayerHandler.ModelFactories.ActiveFactoryIndex = -1;

// returns a valid instance of a model factory or a null reference
LayerHandler.GetFactoryDescription = function()
{
    if (LayerHandler.ModelFactories.ActiveFactoryIndex == -1)
    {
        for (var i = 0; i < LayerHandler.ModelFactories.length; ++i)
        {
            if (LayerHandler.ModelFactories[i].IsActive())
            {
				LayerHandler.ModelFactories.ActiveFactoryIndex = i;
                return LayerHandler.ModelFactories[i];
            }
        }
    }
    else
    {
        return LayerHandler.ModelFactories[LayerHandler.ModelFactories.ActiveFactoryIndex];
    }
    return null;
}

// status enum for loading events
LayerHandler.States = new StringEnum(
	"Uninitialized",	// layer instance created, nothing is initilized
	"Loading",			// write layer, user can only write properties. the values will be stored in a temporary and written to the layer object by the onload event
	"Loaded",			// layer object found, intilizing the layer object
	"Interactive",		// layer initilized, initilize layer values
	"Complete" );		// loading completed, user can write and read values
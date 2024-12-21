function IeLayerConstructor(objDelegate)
{
	// init type manager, required for inheritance
	this.InitTypeManager(arguments);

	// inherit this object from other classes
	arguments.Call(BaseLayerConstructor);

	// copies a reference of the protected members for using from outside the constructor
	var _protected		= arguments.Protected;
	var _modelObject	= objDelegate;
	var _this			= this;


	// constructs the layer and searches the layer object, fires the "init" event
	this.OnConstruct = function()
	{
		if (!IsLayerAvailable())
		{
			AddLayer();
			_this.TimerCall.Interval = _this.WriteTimeOut;
		}
		else
		{
			_this.TimerCall.Interval = _this.InitTimeOut;
		}
		_this.TimerCall.Start();
	}


	// fires the intern onInit event
	this.FireOnInitEvent = function()
	{
		_this.TimerCall.Abort();
		_modelObject.ChangeStatus(LayerHandler.States.Loading, "oninit");
	}


	// initializes the layer object and dispatches the "initComponent" event
	this.OnInit = function()
	{
		_modelObject.LayerObject = GetLayerObject();
		_modelObject.ChangeStatus(LayerHandler.States.Loaded, "oninitcomponent");
	}



	// adds a new layer to the document
	function AddLayer()
	{
		if (window.EventHandler.DocumentLoaded)
		{
			AddNewLayer();
		}
		else
		{
			WriteNewLayer();
		}
	}


	// returns the layer tag with the specific default style sheets
	function GetLayerContext()
	{
		var newLayer = "<div id='" + _modelObject.LayerHandler.GetID() + "' style='";

		for (var item in _this.DefaultLayerProperties)
		{
			if (_this.DefaultLayerProperties.IsField(item))
			{
				newLayer += item + ":" + _this.DefaultLayerProperties[item] + ";";
			}
		}
		return newLayer + "'></div>";
	}


	// adds a layer to the document, if the document content is loaded
	function AddNewLayer()
	{
		document.body.insertAdjacentHTML("afterEnd", GetLayerContext());
	}


	// adds a layer to the document, if the document content is not completely loaded
	function WriteNewLayer()
	{
		document.write(GetLayerContext());
	}


	// returns true, if the layer object was found
	function IsLayerAvailable()
	{
		return Boolean(GetLayerObject());
	}


	// returns the layer, which id was specified in the LayerHandler object
	function GetLayerObject()
	{
		return document.all[_modelObject.LayerHandler.GetID()];
	}


	this.TimerCall = new TimerThread(this.FireOnInitEvent);

	with (_modelObject.InternEvents)
	{
		Attach("onconstruct", this.OnConstruct);
		Attach("oninit", this.OnInit);
	}
}
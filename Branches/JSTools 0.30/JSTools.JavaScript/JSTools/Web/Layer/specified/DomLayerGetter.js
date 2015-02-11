function DomLayerGetter(objDelegate)
{
	// init type manager, required for inheritance
	this.InitTypeManager(arguments);

	// inherit this object from other classes
	arguments.Call(BaseLayerGetter);

	// copies a reference of the protected members for using from outside the constructor
	var _protected		= arguments.Protected;
	var _delegateObject	= objDelegate;
	var _this			= this;


	// initilizes the values of the current layer
	this.OnInitCompontent = function()
	{
		_this.OnRefresh();
		_this.TopMargin		= GetIntValue("offsetTop");
		_this.LeftMargin	= GetIntValue("offsetLeft");
		_this.Visibility	= (_delegateObject.LayerObject.style.visibility.toLowerCase() == "visible");
		_this.ZIndex		= ToNumber(_delegateObject.LayerObject.style.zIndex);
		_this.Clip			= GetClip();

		_delegateObject.ChangeStatus(LayerHandler.States.Interactive, "onload");
	}


	// initializes the values, which are depraced by a content change
	this.OnRefresh = function()
	{
		_this.Images	= _this.GetHtmlComponent("img");
		_this.Links		= _this.GetHtmlComponent("a");
		_this.Forms		= _this.GetHtmlComponent("form");

		_this.Width		= GetIntValue("offsetWidth");
		_this.Height	= GetIntValue("offsetHeight");
	}


	// returns all elements with the specified tag name in a new Array, [virtual]
	this.GetHtmlComponent = function(strTagName)
	{
		return document.getElementsByTagName(strTagName);
	}



	// parses a property of the LayerObject into an interger an returns it
	function GetIntValue(strProperty)
	{
		return parseInt(_delegateObject.LayerObject[strProperty]);
	}


	// returns a new object which contains the clip values
	function GetClip()
	{
		var layerClip	= String(_delegateObject.LayerObject.style.clip).toLowerCase().Trim();
		var clipObject	= CreateClipObject();

		if (!IsVoid(layerClip) && !IsUndefined(layerClip) && layerClip != "auto" && layerClip != "rect()")
		{
			FillValues(clipObject, layerClip);
		}
		return clipObject;
	}


	// creates a new clip object with the specified default value
	function CreateClipObject()
	{
		var clipObject	= new Object();
		var clipValues	= LayerClip.Names.GetValues();

		for (var i = 0; i < clipValues.length; ++i)
		{
			clipObject[clipValues[i]] = 0;
		}

		with (clipObject)
		{
			Right	= _this.Width;
			Bottom	= _this.Height;
		}
		return clipObject;
	}


	// fills the RegExp value into the layer object
	function FillValues(objFill, strCheck)
	{
		var valueArray	= strCheck.replace(/(rect\(|\)|px)/g, "").split(" ");
		var clipValues	= LayerClip.Names.GetValues();

		for (var i = 0; i < clipValues.length; ++i)
		{
			var currentValue = parseInt(valueArray[i]);

			if (IsNumeric(currentValue))
			{
				objFill[clipValues[i]] = currentValue;
			}
		}
	}


	with (_delegateObject.InternEvents)
	{
		Attach("oninitcomponent", this.OnInitCompontent);
		Attach("onrefresh", this.OnRefresh);
	}
}
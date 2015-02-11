function IeLayerSetter(objDelegate)
{
	// init type manager, required for inheritance
	this.InitTypeManager(arguments);

	// inherit this object from other classes
	arguments.Call(BaseLayerSetter);

	// copies a reference of the protected members for using from outside the constructor
	var _protected		= arguments.Protected;
	var _delegateObject	= objDelegate;
	var _this			= this;


	// sets the visibility of the layer
	this.SetVisibility = function(blnVisibility)
	{
		_this.Style.visibility = (blnVisibility ? "visible" : "hidden");
	}


	// sets the width of the layer
	this.SetWidth = function(intWidth)
	{
		_this.Style.width = ToNumber(intWidth);
	}


	// sets the height of the layer
	this.SetHeight = function(intHeight)
	{
		_this.Style.height = ToNumber(intHeight);
	}


	// sets the clip of the layer
	this.SetClip = function(objClip)
	{

	}


	// sets the left margin of the layer
	this.SetTopMargin = function(intTop)
	{
		_this.Style.top = ToNumber(intTop);
	}


	// sets the top margin of the layer
	this.SetLeftMargin = function(intLeft)
	{
		_this.Style.left = ToNumber(intLeft);
	}


	// writes the specified content into the layer document
	this.OnContentWrite = function(strContent)
	{
		with (_delegateObject)
		{
			LayerObject.innerHTML = strContent;
			InternEvents.FireUpdate("onrefresh");
		}
	}


	// writes the given z-Index if the layer
	this.OnZIndexWrite = function(intZIndex)
	{
		_this.Style.zIndex = ToNumber(intZIndex);
	}


	// initilizes the style object
	this.OnInitComponent = function()
	{
		_this.Style = _delegateObject.LayerObject.style;
	}

	_delegateObject.InternEvents.Attach("oninitcomponent", this.OnInitComponent);
}
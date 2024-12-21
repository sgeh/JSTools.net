function LayerOriginalValues(objBaseLayerGetter)
{
	if (!objBaseLayerGetter)
	{
		this.ThrowArgumentException("The given layer BaseLayerGetter is not a valid object or contains a null reference!");
		return null;
	}

	// init values
	var _content = objBaseLayerGetter.GetContent();
	var _leftMargin = objBaseLayerGetter.GetLeftMargin();
	var _topMargin = objBaseLayerGetter.GetTopMargin();
	var _visibility = objBaseLayerGetter.GetVisibility();
	var _zIndex = objBaseLayerGetter.GetZIndex();
	var _clip = objBaseLayerGetter.GetClip().Clone();
	var _width = objBaseLayerGetter.GetWidth();
	var _height = objBaseLayerGetter.GetHeight();


	// returns the content of the layer. the default
	// value of this property is [nothing]
	this.GetContent = function()
	{
		return _content;
	}


	// returns the margin to the left border
	this.GetLeftMargin = function()
	{
		return _leftMargin;
	}


	// returns the margin to the top border
	this.GetTopMargin = function()
	{
		return _topMargin;
	}


	// returns the z-index of the layer
	this.GetZIndex = function()
	{
		return _zIndex;
	}


	// returns the layer clip object
	this.GetClip = function()
	{
		return _clip;
	}


	// returns the visibility of the object
	// true means the element is visible, false
	// means the element is hidden
	this.GetVisibility = function()
	{
		return _visibility;
	}


	// returns the width of the layer
	this.GetWidth = function()
	{
		return _width;
	}


	// returns the height of the layer
	this.GetHeight = function()
	{
		return _height;
	}


	//
	this.GetBackgroundImage = function()
	{
		return _bgImage;
	}


	//
	this.GetBackgroundColor = function()
	{
		return _bgColor;
	}
}

LayerOriginalValues.prototype = new ILayerValueContainer();

LayerOriginalValues.prototype.toString = function()
{
	return "[object LayerOriginalValues]";
}
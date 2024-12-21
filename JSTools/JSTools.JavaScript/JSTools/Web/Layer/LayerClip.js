function LayerClip(objLayer, intTop, intLeft, intBottom, intRight)
{
	if (!objLayer || objLayer != "[object LayerHandler]")
	{
		this.ThrowArgumentException("The given layer object does not contains a LayerHandler instance!");
		return null;
	}

	var _topMargin = (IsNumeric(intTop) ? intTop : 0);
	var _leftMargin = (IsNumeric(intLeft) ? intLeft : 0);
	var _height = (IsNumeric(intBottom - intTop) && (intBottom - intTop > 0) ? intBottom - intTop : 0);
	var _width = (IsNumeric(intRight - intLeft) && (intRight - intLeft > 0) ? intRight - intLeft : 0);
	var _layer = objLayer;


	// contains an alias for GetTopMargin
	this.GetX = function()
	{
		return this.GetLeftMargin();
	}


	// contains an alias for GetLeftMargin
	this.GetY = function()
	{
		return this.GetTopMargin();
	}


	// returns the margin to the top layer border
	this.GetTopMargin = function()
	{
		return _topMargin;
	}


	// returns the margin to the left layer border
	this.GetLeftMargin = function()
	{
		return _leftMargin;
	}


	// returns the width of the visible layer area
	this.GetWidth = function()
	{
		return _width;
	}


	// returns the height of the visible layer area
	this.GetHeight = function()
	{
		return _height;
	}


	// contains an alias for SetLeftMargin
	this.SetX = function(intLeftMargin)
	{
		this.SetLeftMargin(intLeftMargin);
	}


	// contains an alias for SetTopMargin
	this.SetY = function(intTopMargin)
	{
		this.SetTopMargin(intTopMargin);
	}


	// sets the margin to the top layer border
	this.SetTopMargin = function(intTopMargin)
	{
		if (IsNumeric(intTopMargin))
		{
			_topMargin = GetValidNumber(intTopMargin);
			_layer.SetClip(LayerClip.Names.Top, _topMargin);
		}
	}


	// sets the margin to the left layer border
	this.SetLeftMargin = function(intLeftMargin)
	{
		if (IsNumeric(intLeftMargin))
		{
			_leftMargin = GetValidNumber(intLeftMargin);
			_layer.SetClip(LayerClip.Names.Left, _leftMargin);
		}
	}


	// stes the width of the visible layer area
	this.SetWidth = function(intWidth)
	{
		if (IsNumeric(intWidth))
		{
			_width = GetValidNumber(intWidth);
			_layer.SetClip(LayerClip.Names.Right, _width + _left);
		}
	}


	// sets the height of the visible layer area
	this.SetHeight = function(intHeight)
	{
		if (IsNumeric(intHeight))
		{
			_height = GetValidNumber(intHeight);
			_layer.SetClip(LayerClip.Names.Bottom, _height + _top);
		}
	}


	// writes the values of this object into the a new instance
	this.Clone = function()
	{
		return new LayerClip(_layer, _topMargin, _leftMargin, _topMargin + _height, _leftMargin + _width);
	}



	// returns a valid unsigned number
	function GetValidNumber(intNumber)
	{
		return Math.abs(ToNumber(intNumber));
	}
}
LayerClip.prototype.toString = function()
{
	return "[object LayerClip]";
}
LayerClip.Names = new StringEnum(
	"Top",
	"Right",
	"Bottom",
	"Left" );
function BaseLayerGetter()
{
	// layer values
	this.TopMargin	= null;
	this.LeftMargin	= null;
	this.ZIndex		= null;
	this.Content	= "[not assigned]";
	this.Visibility	= null;
	this.Width		= null;
	this.Height		= null;
	this.Clip		= null;

	// element arrays
	this.Images		= null;
	this.Links		= null;
	this.Forms		= null;

	// initilizes the values of the current layer
	this.OnComponentInit	= function() {}

	// initializes the values, which are depraced by a content change
	this.OnRefresh			= function() {}

	// returns a copy of all important values
	this.CopyValues = function()
	{
		return new LayerOriginalValues(this);
	}
}
BaseLayerGetter.prototype.toString = function()
{
    return "[abstract BaseLayerGetter]";
}
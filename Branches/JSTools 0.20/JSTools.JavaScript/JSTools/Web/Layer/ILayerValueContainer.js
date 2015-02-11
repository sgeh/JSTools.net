function ILayerValueContainer(objBaseLayerGetter)
{
	// returns the content of the layer. the default
	// value of this property is [nothing]
	this.GetContent			= function() { };

	// returns the margin to the left border
	this.GetLeftMargin		= function() { };

	// returns the margin to the top border
	this.GetTopMargin		= function() { };

	// returns the z-index of the layer
	this.GetZIndex			= function() { };

	// returns the layer clip object
	this.GetClip			= function() { };

	// sets the visibility of this layer. false means the layer hides,
	// true means the layer visualizes
	this.GetVisibility		= function() { };
	
	// returns the width of the layer
	this.GetWidth			= function() { };

	// returns the height of the layer
	this.GetHeight			= function() { };
	
	// returns the source of the background image object
	this.GetBackgroundImage	= function() { };

	// returns the background color
	this.GetBackgroundColor	= function() { };
}
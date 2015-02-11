function BaseLayerSetter()
{
	// timeout used by initilizing the new layer content
	this.ContentTimeout = 50;

	// contains the layer style object
	this.Style = null;

	// sets a timeout before wirting the layer content
	this.SetContent = function(strContent)
	{
		var layerJob = new TimerCallBack(this.OnContentWrite, this.ContentTimeout);
		layerJob.Start(strContent);
	}

	// sets the z-Index of the layer
	this.SetZIndex = function(intZIndex)
	{
		//window.document.Layers.
	}

	// abstract methods
	this.SetVisibility = function() { }
	this.SetWidth = function() { }
	this.SetHeight = function() { }
	this.SetClip = function() { }
	this.SetTopMargin = function() { }
	this.SetLeftMargin = function() { }
	this.OnContentWrite = function() { }
	this.OnZIndexWrite = function() { }
	this.OnInitComponent = function() { }
}
BaseLayerSetter.prototype.toString = function()
{
    return "[abstract BaseLayerSetter]";
}
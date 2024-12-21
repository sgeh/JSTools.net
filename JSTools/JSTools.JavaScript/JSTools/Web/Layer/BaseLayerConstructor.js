function BaseLayerConstructor()
{
	this.InitTimeOut = 0;
	this.WriteTimeOut = 50;


	// contains default properties for a new layer
	this.DefaultLayerProperties =
	{
		Visibility:	"visible",
		Position:	"absolute",
		Top:		"0",
		Left:		"0"
	};

	// initializes the layer object and calls the InitComponent event
	this.Init = function() { }

	// searches for a valid layer object with the specified id.
	// if it does not exists, this function writes a new layer.
	// fires the Init event
	this.Construct = function() { }

	// contains the timer call for the init events
	this.TimerCall = null;
}
BaseLayerConstructor.prototype.toString = function()
{
    return "[abstract BaseLayerConstructor]";
}
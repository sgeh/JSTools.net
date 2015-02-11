function BaseModelFactory()
{
	this.toString = function()
	{
		return "[abstract BaseModelFactory]";
	}


	var _this = this;

	// changes the status and fires the "onstatuschange" event
	this.ChangeStatus = function(strLayerStatus, strInternEvent)
	{
		if (window.document.Layers.Debug)
		{
			CreateDebugInfos(this.Status, strLayerStatus, strInternEvent);
		}

		this.ExternEvents.FireUpdate("onstatuschange", this.LayerHandler);
		this.Status = strLayerStatus;

		if (strInternEvent != null)
		{
			this.InternEvents.FireUpdate(strInternEvent);
		}
	}


	// creates debugging informations for the current event
	function CreateDebugInfos(strLayerOldStatus, strLayerNewStatus, strEventCall)
	{
		var newException = new Exception("Event Debug Informations for " + _this.LayerHandler + ":\n\nCurrent status: " + strLayerOldStatus + "\nNew status: " + strLayerNewStatus + "\nEvent call: " + strEventCall, _this.LayerHandler);
		newException.Throw();
	}


	// contains the init procedure
	this.Init = function() { };

	// contains the loading status of the layer object
	this.Status = LayerHandler.States.Uninitialized;

	// contains the base LayerHandler object
	this.LayerHandler = null;

	// contains the ModelConstructor instance
	// this instance must be overwritten
	this.Constructor = null;

	// contains an instance of the ILayerGetter type
	this.ValueGetter = null;

	// contains an instance of the ILayerSetter type
	this.ValueSetter = null;

	// contains a reference to the current browser layer object (e.g. document.layer['layer']) after the oninit event
	this.LayerObject = null;

	// contains the event container for all intern events
	// this events will be called before the extern events (e.g. the onload event)
	this.InternEvents = new EventContainer( ["onconstruct", "oninit", "oninitcomponent", "onload", "onrefresh"] );

	// contains the event container for all extern events
	this.ExternEvents = new EventContainer( ["ondblclick", "ondrag", "ondrop", "onlayerdrop", "onload", "onmousedown", "onmouseout", "onmouseover", "onmouseup", "onmove", "onrefresh", "onremove", "onresize", "onstatuschange"] );
}
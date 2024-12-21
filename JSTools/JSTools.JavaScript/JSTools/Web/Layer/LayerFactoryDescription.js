function LayerFactoryDescription(objModelFactory, blnIsActive)
{
	if (!IsFunction(objModelFactory))
	{
		this.ThrowArgumentException("The given ModelFactory is not a valid function!");
		return null;
	}

	var _objModelFactory = objModelFactory;
	var _blnIsActive = ToBoolean(blnIsActive);


	// returns true, if the used browser is compatible with the layer tools
	this.IsActive = function()
	{
		return _blnIsActive;
	}


	this.GetFactory = function(objLayerHandler)
	{
		if (typeof(objLayerHandler) != 'object' || objLayerHandler != "[object LayerHandler]")
		{
			this.ThrowArgumentException("The given LayerHandler instance contains an invalid reference!");
			return null;
		}
		return (new _objModelFactory(objLayerHandler));
	}
}
LayerFactoryDescription.prototype.toString = function()
{
	return "[object LayerFactoryDescription]";
}
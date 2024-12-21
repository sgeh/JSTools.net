function LayerContainer()
{
	if (!IsUndefined(window.document.CreateLayer))
	{
		this.ThrowRuntimeException("An instance of the LayerContainer does already exists!");
		return null;
	}


	var _this				= this;
	var _layerNames			= new Array();
	var _sortedLayerNames	= new Array();
	var _layerEventHandler	= new LayerEvents(this);


	// contains debug information for programmers
	this.Debug				= false;

	// returns an array which contains all layer names
	this.GetLayerNames = function()
	{
		return _sortedLayerNames;
	}


	// creates a new layer handler object. this function is available
	// in the global window.document container, too
	this.CreateLayer = window.document.CreateLayer = function(strLayerId, strCssClass)
	{
		var newLayerId = ToString(strLayerId);

		if (!_layerNames.Contains(newLayerId))
		{
			var containerLength		= _layerNames.length;
			_this[containerLength]	= _this[ToString(newLayerId).ToEnumerable()] = new LayerHandler(newLayerId, containerLength, strCssClass);

			return ConstructLayer(containerLength, newLayerId);
		}
		return _this.GetLayerById(newLayerId);
	}


	// returns the layer which contains the same property as the given strLayerId
	this.GetLayerById = window.document.GetLayerById = function(strLayerId)
	{
		var searchIndex = _layerNames.Search(ToString(strLayerId));

		if (searchIndex != -1)
		{
			return _this[searchIndex];
		}
		return null;
	}


	// deletes a layer by the specified index
	this.DeleteLayerByIndex = window.document.DeleteLayer = function(intLayerIndex)
	{
		RemoveIndex(intLayerIndex);
	}


	// deletes a layer with the specified name
	this.DeleteLayer = window.document.DeleteLayer = function(strLayerName)
	{
		RemoveIndex(_layerNames.Search(strLayerName));
	}


	// returns the count of registered layers
	this.Count = function()
	{
		return _sortedLayerNames.length;
	}



	// checks if the layer was created successfully and
	// registers the new one
	function ConstructLayer(intIndex, strLayerId)
	{
		if (!IsUndefined(_this[intIndex].Construct))
		{
			_sortedLayerNames.Add(strLayerId.ToEnumerable());
			_layerNames.Add(strLayerId);
			_this[intIndex].Construct();
			return _this[intIndex];
		}
		else
		{
			delete _this[intIndex];
			return null;
		}
	}


	// removes the specified index from all containers
	function RemoveIndex(intIndex)
	{
		if (_container.IsValidIndex(intIndex) && !IsUndefined(_layerNames[intIndex]))
		{
			_this[intIndex].FireUpdate("onremove");
			_sortedLayerNames.Remove(_container[intIndex].GetName());
			delete _layerNames[intIndex];
			delete _this[intIndex];
		}
	}
}
LayerContainer.prototype.toString = function()
{
	return "[object LayerContainer]";
}

window.document.Layers = new LayerContainer();
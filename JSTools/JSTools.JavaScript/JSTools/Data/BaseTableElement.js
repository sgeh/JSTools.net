function BaseTableElement()
{
	var _attributes		= new Array();
	var _attributeKeys	= new Array();


	// returns an attribute from the attribute collection
	this.GetAttribute = function(strAttributeName)
	{
		if (!IsUndefined(_attributes[strAttributeName]))
		{
			return _attributes[strAttributeName];
		}
		return null;
	}


	// adds an attribute to the object
	this.AddAttribute = function(strName, strValue)
	{
		_attributes[strName] = ToString(strValue);
		_attributeKeys.Add(strName);
	}


	// returns the rendered attribute collection
	this.RenderAttributes = function()
	{
		var renderString = "";

		for (var i = 0; i < _attributeKeys.length; ++i)
		{
			renderString += " " + _attributeKeys[i] + "='" + ToString(_attributes[_attributeKeys[i]]).RepressControlCharacters() + "'";
		}
		return renderString;
	}


	// overwrites all attributes with an empty string
	this.CleanAttributes = function()
	{
		for (var attributeName in _attributes)
		{
			_attributes[attributeName] = "";
		}
	}


	// removes an attribute from the attributes collection and returns the deleted value
	this.DeleteAttribute = function(strName)
	{
		return this.RemoveValue(_attributes, strName);
	}


	// deletes all attribute values
	this.DeleteAttributes = function()
	{
		_columnValue = null;

		for (var attributeName in _attributes)
		{
			this.DeleteAttribute(attributeName);
		}
	}


	// removes a value from the objContainer collection and returns the deleted value
	this.RemoveValue = function(objContainer, varName)
	{
		var removeValue = null;

		if (!IsUndefined(objContainer) && !IsUndefined(objContainer[varName]))
		{
			removeValue = objContainer[varName];
			objContainer[varName] = null;
			delete objContainer[varName];
		}
		return removeValue;
	}



	// abstract methods
	this.Render	= function() { }
	this.Clean	= function() { }
	this.Delete	= function() { }
}
BaseTableElement.prototype.toString = function()
{
    return "[abstract BaseTableElement]";
}
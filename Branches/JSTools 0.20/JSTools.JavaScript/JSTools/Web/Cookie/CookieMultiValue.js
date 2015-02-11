function CookieMultiValue()
{
	// returns the unescaped cookie string
	this.toString = function()
	{
		var toStringValue = "";

		for(var i = 0; i < _multiValueContainer.length; ++i)
		{
			toStringValue += _multiValueContainer[i].name + "=" + _multiValueContainer[i].value + ((i + 1 != _multiValueContainer.length) ? "&" : "");
		}
		return toStringValue;
	}


	// identifies a multi value object
	this.IsMultiValue = true;


	// returns a field value with the specified strName
	this.GetValue = function(strName)
	{
		var getContainerValue = null;

		for(var i = 0; i < _multiValueContainer.length; ++i)
		{
			if(_multiValueContainer[i].name == strName)
			{
				getContainerValue = _multiValueContainer[i].value;
			}
		}
		return getContainerValue;
	}


	// returns a field object (name and value) with the specified intIndex
	this.GetField = function(intIndex)
	{
		if(_multiValueContainer.IsValidIndex(intIndex))
		{
			return _multiValueContainer[intIndex];
		}
	}


	// returns the length of all registered fields
	this.GetLength = function()
	{
		return _multiValueContainer.length;
	}


	// adds a new value to the container
	this.AddValue = function(strName, strValue)
	{
		AddContainerValue(strName, strValue);
	}


	// adds a new value to the container
	this.Copy = function()
	{
		return new CookieMultiValue(this);
	}


	// removes a container value and returns the value of it
	this.RemoveValue = function(strName)
	{
		var removeValue = null;

		for(var i = 0; i < _multiValueContainer.length; ++i)
		{
			if(_multiValueContainer[i].name == strName)
			{
				removeValue = _multiValueContainer[i].value;
				_multiValueContainer.Remove(i);
			}
		}
		return removeValue;
	}



	//private statements

	// returns a "" string value, if strContainerValue contains undefined or null
	function GetValidContainerValue(strContainerValue)
	{
		return ((IsUndefined(strContainerValue) || IsNull(strContainerValue)) ? "" : strContainerValue);
	}


	// adds a new object to the current container
	function AddContainerValue(strNameValue, strCookieValue)
	{
		_multiValueContainer.Add( { name: strNameValue, value: GetValidContainerValue(strCookieValue) } );
	}


	// initialzies arrValues and generates the container content
	function ReadArrayValues(arrValues)
	{
		for(var i = 0; i < arrValues.length; ++i)
		{
			if(i % 2 == 0)
			{
				AddContainerValue(unescape(arrValues[i]), unescape(arrValues[i + 1]));
			}
		}
	}


	// copies the objCopy object to this
	function CopyContainer(objCopy)
	{
		for(var c = 0; c < objCopy.GetLength(); ++c)
		{
			AddContainerValue(objCopy.GetField(c).name, objCopy.GetField(c).value);
		}
	}


	// initialzies strValues and generates the container content
	function ReadStringValues(strValues)
	{
		var allValues = strValues.split("&");

		for(var i = 0; i < allValues.length; ++i)
		{
			var addValues = allValues[i].split("=");
			AddContainerValue(addValues[0], addValues[1]);
		}
	}


	// initializes the class argument
	function InitializeArguments(objFirstArgumentValue)
	{
		if(IsArray(objFirstArgumentValue))
		{
			ReadArrayValues(objFirstArgumentValue);
		}
		else if(!IsUndefined(objFirstArgumentValue.IsMultiValue))
		{
			CopyContainer(objFirstArgumentValue);
		}
		else
		{
			InitializeStringArguments(objFirstArgumentValue);
		}
	}


	// initializes the class string argument
	function InitializeStringArguments(objFirstValue)
	{
		if(typeof(objFirstValue) == "string" && _multiValueArguments.length == 1)
		{
			ReadStringValues(unescape(objFirstValue));
		}
		else
		{
			ReadArrayValues(_multiValueArguments);
		}
	}



	var _multiValueContainer	= new Array();
	var _multiValueArguments	= arguments;


	if(_multiValueArguments.length > 0)
	{
		InitializeArguments(_multiValueArguments[0]);
	}
}
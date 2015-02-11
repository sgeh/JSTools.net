function StringEnum()
{
	var _registeredValues = new Array();

	// writes the given arguments into the _registeredValues array and the this object
	for (var i = 0; i < arguments.length; ++i)
	{
		var argument = ToString(arguments[i]);
		_registeredValues.Add(argument);
		this[argument.ToEnumerable()] = argument;
	}


	// returns the registered values in a array format
	this.GetValues = function()
	{
		return _registeredValues.Copy();
	}


	// returns true, if the specified value is supported by this object
	this.Contains = function(strValue)
	{
		return _registeredValues.Contains(ToString(strValue));
	}


	this.toString = function()
	{
		return "[enum " + _registeredValues + "]";
	}
}
StringEnum.prototype = new IEnum();
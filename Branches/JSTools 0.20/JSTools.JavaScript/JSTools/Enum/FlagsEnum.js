function FlagsEnum()
{
	var _registeredValues	= new Array();
	var _flagsBegin			= 0x00000001;

	// writes the given arguments into the _registeredValues array and the this object
	for (var i = 0; i < arguments.length; ++i)
	{
		var argument = ToString(arguments[i]);
		_registeredValues.Add(argument);
		this[argument.ToEnumerable()] = (_flagsBegin <<= 1);
	}


	// returns the registered values in a array format
	this.GetValues = function()
	{
		return _registeredValues.Copy();
	}


	// returns true, if the specified value is supported by this object
	this.Contains = function(strValue)
	{
		return !IsUndefined(this[ToString(strValue).ToEnumerable()]);
	}


	this.toString = function()
	{
		var enumToString = "";

		for (var i = 0; i < _registeredValues.length; ++i)
		{
			enumToString += _registeredValues[i] + " = " + this[_registeredValues[i]] + ((i + i != _registeredValues.length) ? ", " : "");
		}
		return "[enum " + enumToString + "]";
	}
}
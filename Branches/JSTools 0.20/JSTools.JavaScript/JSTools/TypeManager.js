// Manages the types of an object. One object can accommodate multiple types, so
// multi-inheritance is allowed.
function TypeManager(objArguments, objObject)
{
	if (typeof(objArguments) != 'object' || !objArguments.callee)
	{
		this.ThrowArgumentException("You have to provide the argument collection of the class, from which the given object is derived!");
		return null;
	}

	if (typeof(objObject) != 'object')
	{
		this.ThrowArgumentException("The second parameter is not a valid object instance!");
		return null;
	}


	var _object		= objObject;
	var _hashCode	= GetHashCodeString();

	// represents the arguments collection of the class from which the _object is derived
	var _typeArgs	= objArguments;

	// represents the function pointer of the class from which the _object is derived
	var _typePtr	= _typeArgs.callee;

	// represents the type collection for multi-inheritance
	var _types		= new Array();
	_types.Add(_typePtr);


	// if the prototype property of the given object contains a type with a valid type manager, the function pointer
	// will be added to the types collection.
	if (_object.prototype && IsFunction(_object.prototype.HasTypeManager) && _object.prototype.HasTypeManager())
	{
		_types.Add(_object.prototype.GetTypePtr());
	}


	// represents an instance of a type manager
	this.IsTypeManager = true;


	// returns the function type pointer
	this.GetTypePtr = function()
	{
		return _typePtr;
	}


	// returns true, the current object is derived from the given function
	this.IsTypeOf = function(objFunction)
	{
		for (var i = 0; i < _types.length; ++i)
		{
			if (_types[i] == objFunction)
			{
				return true;
			}
		}
		return false;
	}


	// creates a call to the specified function object. the objArguments parameter
	// is required to make sure, that the call was send from the arguments.Call rocedure.
	// writes the protected members into the arguments.Protected collection with the
	// function name as indexer
	this.InheritType = function(objFunction, objArguments)
	{
		if (typeof(objArguments) == 'object' && _typeArgs.Call.toString() == objArguments.callee.toString())
		{
			var functionName = objFunction.GetName();
			_types.Add(objFunction);

			_typeArgs.Protected.Add(functionName);
			_typeArgs.Protected[functionName] = objFunction.Inherit(_object, GetCallParameters(objArguments));
		}
		else
		{
			this.ThrowArgumentException("Could not inherit from an other class because the parameters contain invalid references!");
		}
	}


	// returns the unique id for the current object
	this.GetHashCode = function()
	{
		return _hashCode;
	}


	// returns all members of the given objArgs collection in a new array.
	// the first entry of the objArgs collection will be removed
	function GetCallParameters(objArgs)
	{
		var params = new Array();

		for (var i = 1; i < objArgs.length; ++i)
		{
			params.Add(objArgs[i]);
		}
		return params;
	}


	// creates the unique hash key
	function GetHashCodeString()
	{
		var randomHash = ToString(Math.random());
		return randomHash.substring(2, randomHash.length);
	}
}
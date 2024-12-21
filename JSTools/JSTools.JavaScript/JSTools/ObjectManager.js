// Manages the protected members for derived classes of this function.
function ObjectManager(objFunction)
{
	if (!IsFunction(objFunction))
	{
		this.ThrowArgumentException("The given argument is not a valid function!");
		return null;
	}

	var _objects			= new Array();
	var _derivedTypes		= new Array();

	var _function			= objFunction;
	var _className			= null;
	var _classPrefix		= null;


	this.IsObjectManager	= true;


	// returns the name of the class, which is handled by this object manager, e.g. "ObjectManager"
	this.GetClassName = function()
	{
		if (_className == null)
		{
			InitFunctionValues();
		}
		return _className;
	}


	// returns the prefix of the class, which is handled by this object manager, e.g. "protected.ObjectManager."
	this.GetClassPrefix = function()
	{
		if (_classPrefix == null)
		{
			InitFunctionValues();
		}
		return _classPrefix;
	}


	// registers a new derived object. this call must be send from the "InitTypeManager" procedure.
	// returns an reference to an array, which will contain the protected members for the given object.
	this.Register = function(objObject, objArguments)
	{
		// if was created by the "InitTypeManager" procedure
		if (IsValidCaller(objObject, objArguments) && objArguments.callee == objObject.InitTypeManager)
		{
			if (!Contains(objObject))
			{
				_objects.Add(new ObjectManagerEntry(objObject));
			}
			return _objects[Search(objObject)].GetMembers();
		}
		return null;
	}


	// returns the protected members for the specified object. this call must be send from the "Inherit" procedure
	this.GetProtectedMembers = function(objObject, objArguments)
	{
		// if was created by the "Inherit" procedure
		if (IsValidCaller(objObject, objArguments) && objArguments.callee == Function.prototype.Inherit && Contains(objObject))
		{
			return _objects[Search(objObject)].GetMembers();
		}
		return null;
	}


	// gets a type array containing the derived classes
	this.GetDerivedTypes = function()
	{
		if (_derivedTypes.length != _objects.length)
		{
			_derivedTypes = new Array();

			for (var i = 0; i < _objects.length; ++i)
			{
				_derivedTypes.Add(_objects[i].GetObject().GetType());
			}
		}
		return _derivedTypes;
	}


	// returns true, if the given object hash code was found in the _objects array, otherwise false
	function Contains(objObject)
	{
		return (Search(objObject) != -1);
	}


	// returns the index of the given objObject in the _objects array
	function Search(objObject)
	{
        for(var keyCount = _objects.length - 1; keyCount > -1 && objObject.GetHashCode() != _objects[keyCount].GetEntryHash(); --keyCount)
        {
            ;
        }
		return keyCount;
	}


	// reports true, if the specified arguments are valid caller types
	function IsValidCaller(objObject, objArguments)
	{
		return (typeof(objObject) == 'object' && objArguments && IsNumeric(objArguments.length));
	}


	// evaluates the name and the prefix of the function and writes it into private _className and _classPrefix variables
	function InitFunctionValues()
	{
        var valueMatch = new RegExp('^\\s*function\\s+((\\w*)\\.)*([\\w]+)\\s*\\(');

        if (valueMatch.test(_function.toString()))
        {
            _classPrefix    = RegExp.$1;
            _className      = RegExp.$3;
        }
	}
}
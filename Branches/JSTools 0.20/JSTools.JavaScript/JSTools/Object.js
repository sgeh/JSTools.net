// returns a reflection array within all properties and methods
Object.prototype.GetReflection = function()
{
	var objReflection	= new ReflectionGenerator(this);
	return objReflection.GetMembers();
}

// returns the type of the current object
Object.prototype.GetType = function()
{
// todo: create the type instance here
	return null;
}

// alerts an argument exception message box
Object.prototype.ThrowArgumentException = function(strMessage)
{
	var exception = new Exception("Argument Exception in " + this.toString() + (!IsUndefined(strMessage) ? "\n " + strMessage : ""));
	exception.Throw();
}


// alerts an argument exception message box
Object.prototype.ThrowRuntimeException = function(strMessage)
{
	var exception = new Exception("Runtime Exception in " + this.toString() + (!IsUndefined(strMessage) ? "\n " + strMessage : ""));
	exception.Throw();
}


// used by the inherit procedure
// contains the unique id of an object
// do not change the value of this property!
Object.prototype.GetHashCode = function()
{
	if (!this.HasTypeManager())
	{
		this.ThrowRuntimeException("The type manager of this object is not initilized. To get the unique HashCode you have to initilize the TypeManager!");
		return null;
	}
	return this.__typeManager.GetHashCode();
}


// copies all fields of the current object into a new object and returns it.
Object.prototype.Copy = function()
{
	var copyObject = new Array();

	for (var item in this)
	{
		if(this.IsField(item))
		{
			copyObject[item] = this.CopyField(item);
		}
	}
	return copyObject;
}


// copies a field of this object. if the specified field is an object,
// this function makes a deep copy of it. this function iterates recursive
// through the object hirarchy of the specified item.
Object.prototype.CopyField = function(strItem)
{
	if (typeof(this[strItem]) == 'object' && typeof(this[strItem].Copy) == 'function')
	{
		return this[strItem].Copy();
	}
	else
	{
		return this[strItem];
	}
}


// returns true, if the given item is a valid field (property) of this object
Object.prototype.IsField = function(strItem)
{
	return (!IsUndefined(this[strItem]) && !IsFunction(this[strItem]) && String(this[strItem]).indexOf("__") == -1);
}


// required method for inheritance and member protection. registers the current object
// by the given function and creates a new type manager, if required
Object.prototype.InitTypeManager = function(objArguments)
{
	if (IsUndefined(objArguments) || !IsFunction(objArguments.callee))
	{
		this.ThrowArgumentException("The given argument object contains invalid references!");
		return;
	}

	// if a no type manager instance exits, create a new with the given parameters
	if (!this.HasTypeManager())
	{
        // creates a new type manager instance
        this.__typeManager = new TypeManager(objArguments, this);

        // creates a default toString function which returns the name of the class with an object prefix
        this.toString = function()
        {
            return "[object " + this.__typeManager.GetTypePtr().GetName() + "]";
        }
    }

	// create an anonymous function in the arguments collection.
	// with this function you're able to call the inheritance mechanism
	objArguments.Call = function(objFunction)
	{
		if (!IsFunction(objFunction))
		{
			this.ThrowArgumentException("The given argument is not a valid function pointer!");
			return;
		}

		if (objFunction == this.Object.__typeManager.GetTypePtr())
		{
			this.ThrowArgumentException("Cannot inherit the own function!");
			return;
		}
		this.Object.__typeManager.InheritType(objFunction, arguments);
	}

	// get the array which contains the protected members
	objArguments.Protected  = objArguments.callee.GetObjectManager().Register(this, arguments);
	objArguments.Object		= this;
}


// returns true, if this object controls a valid TypeManager, otherwise false
Object.prototype.HasTypeManager = function()
{
	return (this.__typeManager && this.__typeManager.IsTypeManager);
}


// returns a valid type manager instance or null
Object.prototype.GetTypeManager = function()
{
	return (this.HasTypeManager() ? this.__typeManager : null);
}

// contains type and function informations
// !!DO NOT CHANGE THIS VARIABLE FROM GLOBAL SCRIPT CODE!!
Object.prototype.__typeManager = null;
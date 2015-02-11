// applies this constructor to the given object and calls the reference.
// !!CAUTION: THIS FUNCTION OVERRIDES ALL PULBIC MEMBERS WITH THE SAME NAME!!
Function.prototype.Inherit = function(objApply, arrParams)
{
	if(IsUndefined(objApply) || IsNull(objApply))
	{
		this.ThrowArgumentException("You must specify a valid object");
		return null;
	}

	var constructArguments	= (IsUndefined(arrParams) || arrParams.length == 0) ? null : arrParams;
	var constructName		= "__constructor";
	var callResult;

	// search for defined constructors in the objApply object
	for (var constructCount = 0; !IsUndefined(objApply[constructName + constructCount]); ++constructCount)
	{
		;
	}

	// add the function pointer to the objApply object
	objApply[constructName += constructCount] = this;

	if (constructArguments != null)
	{
		// create argument string
		var constructArgString = String.CreateArgumentString(constructArguments.length, "arrParams");

		// call function pointer. this procedure will write all members into the objApply object
		callResult = eval("objApply." + constructName + "(" + constructArgString + ")");
	}
	else
	{
		// call function pointer, without parameters.
		// this call is much faster than the call with parameters
		callResult = objApply[constructName]();
	}

	// clean up memory, this will remove the function pointer
	delete objApply[constructName];

	// return the protected members
	return this.GetObjectManager().GetProtectedMembers(objApply, arguments);
}


// returns the name of the function
Function.prototype.GetName = function()
{
	return this.GetObjectManager().GetClassName();
}


// contains protected object informations
// !!DO NOT CHANGE THIS VARIABLE FROM GLOBAL SCRIPT CODE!!
Function.prototype.__objectManager = null;


// returns a valid ObjectManager instance
Function.prototype.GetObjectManager = function()
{
	if (!this.__objectManager || !this.__objectManager.IsObjectManager)
	{
		this.__objectManager = new ObjectManager(this);
	}
	return this.__objectManager;
}
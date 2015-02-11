// Manages the handling of the protected array of a derived type.
function ObjectManagerEntry(objBase)
{
	if (typeof(objBase) != 'object')
	{
		this.ThrowArgumentException("The given parameter is not a valid object!");
	}

	var _baseObject			= objBase;
	var _protectedMembers	= new Array();


	// returns the derived type of the given object
	this.GetObject = function()
	{
		return _baseObject;
	}


	// returns the hash code of the given object
	this.GetEntryHash = function()
	{
		return _baseObject.GetHashCode();
	}


	// returns the protected members of the handling object
	this.GetMembers = function()
	{
		return _protectedMembers;
	}
}
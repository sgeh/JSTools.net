// this class is immutable
function EventItem(objEvent, strIdentifer)
{
	if (IsUndefined(objEvent) || typeof(objEvent) != 'function')
	{
		this.ThrowArgumentException("The given argument '" + objEvent + "' must be a valid function object!");
		return null;
	}

	var _itemName	= (typeof(strIdentifer) == 'string') ? strIdentifer : null;
	var _itemObject	= objEvent;



	// returns the given name of the event item
	this.GetItemName = function()
	{
		return _itemName;
	}


	// returns the event function object
	this.GetItemObject = function()
	{
		return _itemObject;
	}
}
EventItem.prototype.toString = function()
{
	return "[object EventItem]";
}
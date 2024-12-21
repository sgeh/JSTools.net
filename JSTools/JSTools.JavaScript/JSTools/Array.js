/// <method>
/// Gets the string representation of the current array. The array
/// values will be formated as follows:
///  String output: [ value1, value2, null, undefined, 5, ... ]
/// </method>
/// <returns type="String">Calls the toString method of each child item,
/// separates them with ', ' and returns the concated string.</returns>
Array.prototype.toString = function()
{
	var output = "[ ";
	
	for (var i = 0; i < this.length; ++i)
	{
		if (typeof(this[i]) == 'undefined')
			output += 'undefined';
		else if (this[i] == null)
			output += 'null';
		else
			output += this[i].toString();
		
		if (i + 1 != this.length)
			output += ", ";
	}
	return output + (this.length > 0 ? " ]" : "]");
}


/// <method>
/// Gets the number of stored items.
/// </method>
/// <returns type="Integer">Returns the number of stored items.</returns>
Array.prototype.Count = function()
{
	return this.length;
}


/// <method>
/// Creates a copy of this array.
/// </method>
/// <param name="intStartIndex" type="Integer">Index, at which the copy begins. Default value is 0.</param>
/// <returns type="Array">Returns the copy of this array.</returns>
Array.prototype.Copy = function(intStartIndex)
{
	var startIndex = Math.abs(!isNaN(intStartIndex) ? Number(intStartIndex) : 0);
	var newArray = new Array(this.length);

	for (var n = startIndex; n < this.length; ++n)
	{
		newArray[n] = this[n];
	}
	return newArray;
}


/// <method>
/// Copies all the elements of the current Array to the specified Array.
/// </method>
/// <param name="arrToInsert" type="Array">Array to insert the element.</param>
Array.prototype.CopyTo = function(arrToInsert)
{
	if (!arrToInsert || typeof(arrToInsert.Add) != 'function')
		return;

	for (var i = 0; i < this.length; ++i)
	{
		arrToInsert.Add(this[i]);
	}
}


/// <method>
/// Clears the array and sets the length property to zero.
/// </method>
Array.prototype.Clear = function()
{
	for (var i = this.length - 1; i > -1; --i)
	{
		this.Pop();
	}
}


/// <method>
/// Adds a new value to the array.
/// </method>
/// <param name="objValue" type="Object">Value to add to the object.</param>
Array.prototype.Add = function(objValue)
{
	return this[this.length] = objValue;
}


/// <method>
/// Gets the value at the given index/key.
/// </method>
/// <param name="objKey" type="Object">Key of the value to get.</param>
/// <returns type="Object">Returns the expected value or an null reference, if the given key does
/// not exist.</returns>
Array.prototype.Get = function(objKey)
{
	if (typeof(this[objKey]) == 'undefined')
		return null;

	return this[objKey];
}


/// <method>
/// Searches for the index of the given value.
/// </method>
/// <param name="objValue" type="Object">Value to search.</param>
/// <returns type="Integer">Returns the index (begins with 0) of the found value, if nothing found
/// this function will return -1.</returns>
Array.prototype.IndexOf = function(objValue)
{
	for (var keyCount = this.length - 1; keyCount >= 0 && this[keyCount] != objValue; --keyCount)
	{
		;
	}
	return keyCount;
}


/// <method>
/// Searches in this array for the given value.
/// </method>
/// <param name="objValue" type="Object">Value to search.</param>
/// <returns type="Boolean">Returns true if an item of this array is equal to the given value.</returns>
Array.prototype.Contains = function(objValue)
{
	return (this.IndexOf(objValue) != -1);
}


/// <method>
/// Removes the last entry of the array and returns the deleted value.
/// </method>
/// <returns type="Object">Returns the deleted value.</returns>
Array.prototype.Pop = function()
{
	var oldArrayContent = this.GetLast();

	if (oldArrayContent != null)
	{
		delete this[this.length - 1];
		this.length--;
	}
	return oldArrayContent;
}


/// <method>
/// Removes the given object and returns the current array length.
/// </method>
/// <param name="objToRemove" type="Integer">Object to remove.</param>
/// <returns type="Integer">Returns the current array length.</returns>
Array.prototype.Remove = function(objToRemove)
{
	var indexToRemove = this.IndexOf(objToRemove);
	
	if (indexToRemove != -1)
	{
		this.RemoveAt(indexToRemove);
	}
	return this.length;
}


/// <method>
/// Removes an array item and returns the current array length.
/// </method>
/// <param name="intRemoveIndex" type="Integer">Index to remove.</param>
/// <returns type="Integer">Returns the current array length.</returns>
Array.prototype.RemoveAt = function(intRemoveIndex)
{
	var index = Number(intRemoveIndex);

	if (!isNaN(index) && index > -1 && index < this.length)
	{
		for (var i = 0; i < this.length - 1; ++i)
		{
			if (i >= intRemoveIndex)
			{
				this[i] = this[i + 1];
			}
		}
		this.Pop();
	}
	return this.length;
}

/// <method>
/// Returns the nearest element of the specified index. If you specify the
/// index 5 of an array, which has only 3 elements, this function will return
/// 2, because the second index of this array is the last valid element of
/// the array. If you specify an index like -5 (or a number lower than 0) you
/// will obtain the first element of this array at index 0.
/// </method>
/// <param name="intIndex" type="Integer">Index to check.</param>
/// <returns type="Integer">Returns a valid array index.</returns>
Array.prototype.GetNearestElement = function(intIndex)
{
	var index = Number(intIndex);

	if (!isNaN(index))
	{
		if (index >= this.length)
		{
			return this.length - 1;
		}
		else
		{
			return 0;
		}
	}
	return index;
}


/// <method>
/// Returns the last element of this array. If the array has no entries,
/// you will obtain a null reference.
/// </method>
/// <returns type="Object">Returns the last array entry.</returns>
Array.prototype.GetLast = function()
{
	return ((this.length > 0) ? this[this.length -1] : null);
}


/// <method>
/// Returns the first element of this array. If the array has no entries,
/// you will obtain a null reference.
/// </method>
/// <returns type="Object">Returns the first array entry.</returns>
Array.prototype.GetFirst = function()
{
	return ((this.length > 0) ? this[0] : null);
}


/// <method>
/// Sorts the array content.
/// </method>
/// <param name="blnToSort" type="Boolean">True to sort ascend, false to sort descend.
/// The default value is false.</param>
Array.prototype.Sort = function(blnToSort)
{
	if (blnToSort)
	{
		this.SortAscend();
	}
	else
	{
		this.SortDescend();
	}
}


/// <method>
/// Sorts the array content descend.
/// </method>
Array.prototype.SortDescend = function()
{
	this.sort( function(objA, objB)
		{
			if (objA < objB)
				return 1;
			
			if (objA > objB)
				return -1;
				
			return 0;
		}
	);
}


/// <method>
/// Sorts the array content ascend.
/// </method>
Array.prototype.SortAscend = function()
{
	this.sort( function(objA, objB)
		{
			if (objA < objB)
				return -1;
			
			if (objA > objB)
				return 1;
				
			return 0;
		}
	);
}


/// <method>
/// Gets the range of the specified array.
/// </method>
/// <param name="arrToGetRange" type="Array">Array which subset should be returned.</param>
/// <param name="intIndex" type="Integer">A zero-based index at which the range starts.</param>
/// <param name="intLength" type="Integer">The number of elements in the range.</param>
/// <returns type="Array">Returns a new array instance which contains the specified range.</returns>
Array.GetRange = function(arrToGetRange, intIndex, intLength)
{
	var range = [ ];
	var index = (!isNaN(intIndex) && Number(intIndex) > -1) ? Number(intIndex) : 0;
	var length = (!isNaN(intLength) && Number(intLength) > -1) ? Number(intLength) : 0;

	if (arrToGetRange
		&& typeof(arrToGetRange) == 'object'
		&& typeof(arrToGetRange.length) == 'number')
	{
		for (var i = 0; i < length && index < arrToGetRange.length; ++i)
		{
			range.Add(arrToGetRange[index++]);
		}
	}
	return range;
}
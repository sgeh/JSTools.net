// identifies an array
Array.prototype.IsArray = true;


// makes a deep copy of this array and returns a new array instance
Array.prototype.Copy = function()
{
	var newArray = new Array();

	for(var n = 0; n < this.length; ++n)
	{
		newArray.Add(this[n]);
	}

	for(var item in this)
	{
		if (this.IsField(item))
		{
			newArray[item] = this[item];
		}
	}
	return newArray;
}


// adds a new value to the array
Array.prototype.Add = function(objValue)
{
	return this[this.length] = objValue;
}


// removes all empty slots from the objArray and returns this new Array
Array.prototype.Trim = function()
{
	var trimArray		= new Array();
	var trimCount		= 0;

	for(var o = 0; o < this.length; ++o)
	{
		if(!IsUndefined(this[o]) && !IsVoid(this[o]))
		{
			trimArray[trimCount] = this[o];
			trimCount++;
		}
	}

	return trimArray;
}


// returns the index (begins with 0) of the found value, if nothing found you'll obtain -1
Array.prototype.Search = function(objValue)
{
	for(var keyCount = this.length - 1; keyCount >= 0 && this[keyCount] != objValue; --keyCount)
	{
		;
	}

	return keyCount;
}


// returns true if any item of this array contains objKey
Array.prototype.Contains = function(objKey)
{
	return (this.Search(objKey) != -1);
}


// adds all parameters to the array and returns the new length of it
Array.prototype.Push = function()
{
	for(var i = 0; i < arguments.length; ++i)
	{
		this.Add(arguments[i]);
	}

	return this.length;
}


// removes the last entry of the array and returns the deleted value
Array.prototype.Pop = function()
{
	var oldArrayContent = this[this.length - 1];

	if(!IsUndefined(oldArrayContent))
	{
		delete this[this.length - 1];
		this.length--;
	}

	return oldArrayContent;
}


// adds intLength parameters by possition intStart to the array and overwrites existing entries
Array.prototype.Splice = function(intStart, intLength)
{
	if(this.IsValidIndex(intStart) && this.IsValidIndex(intLength))
	{
		var innerCount = 2;

		for(var i = 0; i < intStart + intLength; ++i)
		{
			if(i >= intStart)
			{
				this[i] = (IsUndefined(arguments[innerCount])) ? "" : arguments[innerCount];
				innerCount++;
			}
		}
	}
}


// removes an array item and returns the current array length
Array.prototype.Remove = function(intRemoveIndex)
{
	if(this.IsValidIndex(intRemoveIndex))
	{
		for(var i = 0; i < this.length - 1; ++i)
		{
			if(i >= intRemoveIndex)
			{
				this[i] = this[i + 1];
			}
		}

		this.Pop();
	}

	return this.length;
}


// returns true, if the intIndex is a valid index of this array
Array.prototype.IsValidIndex = function(intIndex)
{
	return (IsNumeric(intIndex) ? (ToNumber(intIndex) < this.length && ToNumber(intIndex) >= 0) : false);
}


// removes the first index of this array and returns the deleted value
Array.prototype.Shift = function()
{
	var oldArrayContent = this[0];

	if(!IsUndefined(oldArrayContent))
	{
		for(var i = 0; i < this.length - 1; ++i)
		{
			this[i] = this[i + 1];
		}

		this.Pop();
	}

	return oldArrayContent;
}


// adds the given arguments to the array and returns the new array length
Array.prototype.Unshift = function()
{
	for(var i = this.length - 1; i >= 0; --i)
	{
		this[i + arguments.length] = this[i];
	}

	for(var i = 0; i < arguments.length; ++i)
	{
		this[i] = arguments[i];
	}

	return this.length;
}


// returns the sum of all array values
Array.prototype.GetSum = function()
{
	var totalSum = this[0];

	for(var i = 1; i < this.length; ++i)
	{
		totalSum += this[i];
	}

	return totalSum;
}


// returns the max value
Array.prototype.GetMax = function()
{
	var maxValue = this[0];

	for(var i = 1; i < this.length; ++i)
	{
		if(maxValue < this[i])
		{
			maxValue = this[i];
		}
	}

	return maxValue;
}


// returns the min value
Array.prototype.GetMin = function()
{
	var minValue = this[0];

	for(var i = 1; i < this.length; ++i)
	{
		if(minValue > this[i])
		{
			minValue = this[i];
		}
	}

	return minValue;
}


// returns a random element of this array
Array.prototype.GetRandomElement = function()
{
	return this[Math.floor(Math.random() * this.length)];
}


// returns the nearest element of the specified intIndex.
// if you specifie the index 5 of an array, which has only
// 3 elements, this function will return 2, because the
// second index of this array is the last valid element of
// the array. if you specifie an index like -5 (or a number lower
// than 0) you will obtain the first element of this array, thus 0.
Array.prototype.GetNearestElement = function(intIndex)
{
	var index = ToNumber(intIndex);

	if (IsNumeric(index) && !this.IsValidIndex(index) && this.length > 0)
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
	return intIndex;
}


// returns the last element of this array. if the array has no entries,
// you will obtain a null reference.
Array.prototype.GetLast = function()
{
	return ((this.length > 0) ? this[this.length -1] : null);
}


// swaps two values by the given arguments
Array.prototype.Swap = function(intFirstIndex, intSecondIndex)
{
	var tmpSwap = this[intFirstIndex];
	this[intFirstIndex] = this[intSecondIndex];
	this[intSecondIndex] = tmpSwap;
}


// sorts the array content descend
Array.prototype.Sort = function()
{
	return this.SortASC(false);
}


// sorts the array content ascend
Array.prototype.SortASC = function(blnSensitive)
{
	var sortDescObj = new Array.ValueSorter(this);
	return sortDescObj.SortValues(blnSensitive, "SwapASC");
}


// sorts the array content descend
Array.prototype.SortDESC = function(blnSensitive)
{
	var sortDescObj = new Array.ValueSorter(this);
	return sortDescObj.SortValues(blnSensitive, "SwapDESC");
}


// contains the required sort functions
Array.ValueSorter = function(objParent)
{
	// initializes the sort object
	this.SortValues = function(blnSensitive, strFunctionAssign)
	{
		_keySensitive = (IsUndefined(blnSensitive)) ? false : blnSensitive;
		SortArrayCopy(eval(strFunctionAssign));
		return _arrayObject;
	}


	// starts the bubble sort algorythm
	function SortArrayCopy(objSwapFunction)
	{
		if (!objSwapFunction)
			return;

		for(var j = _arrayObject.length; j >= 0; --j)
		{
			for(var i = 0; i < j - 1; ++i)
			{
				objSwapFunction(i);
			}
		}
	}


	// returns the a lower string, if the blnSensitive was false
	function GetKeySensitiveString(intValue)
	{
		if(IsNumeric(_arrayObject[intValue]))
		{
			return ToNumber(_arrayObject[intValue]);
		}
		else
		{
			return (_keySensitive) ? ToString(_arrayObject[intValue]).toLowerCase() : ToString(_arrayObject[intValue]);
		}
	}


	// swaps the values ascend
	function SwapASC(intSwapIndex)
	{
		SwapValues(intSwapIndex, intSwapIndex + 1);
	}


	// swaps the values descend
	function SwapDESC(intSwapIndex)
	{
		SwapValues(intSwapIndex + 1, intSwapIndex);
	}


	// swaps the values
	function SwapValues(intStatusFirst, intStatusSecond)
	{
		if(GetKeySensitiveString(intStatusFirst) < GetKeySensitiveString(intStatusSecond))
		{
			_arrayObject.Swap(intStatusFirst, intStatusSecond);
		}
	}

	var _arrayObject	= objParent.Copy();
	var _keySensitive	= false;
}
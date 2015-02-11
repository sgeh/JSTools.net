// returns a string without white spaces on the left side
String.prototype.TrimStart = function(strTrim)
{
	var myTrim = new String(this);
	var myChar = ((strTrim) ? ToString(strTrim) : " ");

	while(myTrim.charAt(0) == myChar)
	{
		myTrim = myTrim.substring(1, myTrim.length);
	}
	return myTrim;
}


// returns a string without white spaces on the left side
String.prototype.TrimEnd = function(strTrim)
{
	var myTrim = new String(this);
	var myChar = ((strTrim) ? ToString(strTrim) : " ");

	while(myTrim.charAt(myTrim.length - 1) == myChar)
	{
		myTrim = myTrim.substring(0, myTrim.length - 1);
	}
	return myTrim;
}


// returns a string without white spaces on the left or right side
String.prototype.Trim = function(strTrimChar)
{
	var firstReplace = this.TrimStart(strTrimChar);
	return firstReplace.TrimEnd(strTrimChar);
}


// returns the string strOldValue replaced with strReplace on first strFind
String.prototype.ReplaceFirst = function(strFind, strReplace)
{
	return this.ReplaceBy(strFind, strReplace, this.indexOf(strFind));
}


// returns the string strOldValue replaced with strReplace on last strFind
String.prototype.ReplaceLast = function(strFind, strReplace)
{
	return this.ReplaceBy(strFind, strReplace, this.lastIndexOf(strFind));
}


// returns true if this string is alpha numeric
String.prototype.IsAlphaNumeric = function()
{
	return (this.search(/^\w*$/) != -1);
}


// returns true if this string is a white space
String.prototype.IsWhiteSpace = function()
{
	return (this.search(/^\s+$/) != -1);
}


// returns true if this string contains a white space
String.prototype.ContainsWhiteSpace = function()
{
	return (this.search(/\s/) != -1);
}


// replaces the string "strToFind" with "strToReplace" in string "strValue" on position "intStart"
String.prototype.ReplaceBy = function(strToFind, strToReplace, intStart, strValue)
{
	intStart		= ToNumber(intStart);
	strValue		= (IsUndefined(strValue)) ? ToString(this) : strValue;

	if(intStart != -1)
	{
		var newBegin	= strValue.substring(0, intStart);
		var newEnd		= strValue.substring(intStart + ToString(strToFind).length, strValue.length);
		return newBegin + ToString(strToReplace) + newEnd;
	}
	return strValue;
}


// returns the string strOldValue replaced with strReplace on strFind
String.prototype.Replace = function(strFind, strReplace)
{
	strFind		= ToString(strFind);
	strReplace	= ToString(strReplace);

	var replacedString = ToString(this);

	for(var i = 0; i < replacedString.length; ++i)
	{
		if(replacedString.substring(i, strFind.length + i) == strFind)
		{
			replacedString = this.ReplaceBy(strFind, strReplace, i, replacedString);
			i += Math.abs(strFind.length - strReplace.length);
		}
	}
	return replacedString;
}


// returns a valid enumerable item string
String.prototype.ToEnumerable = function()
{
	return ((this.IsAlphaNumeric()) ? this.valueOf() : this.replace(/\W/g, "_"));
}


// returns the intIndex letter of this string in upper case format
String.prototype.UpperCaseAt = function(intIndex)
{
	if (this.length && this.IsValidChar(intIndex))
	{
		return this.ReplaceBy(this.charAt(intIndex), String.fromCharCode(this.charCodeAt(intIndex) & 0x0DF), intIndex, this.valueOf());
	}
	return this.valueOf();
}


// returns true, if the intIndex is a valid index of this string
String.prototype.IsValidChar = function(intIndex)
{
	return (IsNumeric(intIndex) ? (ToNumber(intIndex) < this.length && ToNumber(intIndex) >= 0) : false);
}


// replaces all string control characters with repressed string characters
String.prototype.RepressControlCharacters = function()
{
	return this.replace(/"/g, "\\\"").replace(/'/g, '\\\'');
}


// changes all string control characters from " to '
String.prototype.ChangeControlCharacters = function()
{
	return this.replace(/'/g, '\\\'').replace(/"/g, "'");
}


// returns the objValue in a lower cased and enumerable string
String.ToEnumerable = function(objValue)
{
	return String(objValue).toLowerCase().ToEnumerable();
}


// creates a comma separated string with the specified length and the specified name.
// if you specify the parameters like (3, "myName") this function creates a string
// like this "myName[0],myName[1],myName[2]". the default value for the strArgsName
// is "arguments".
String.CreateArgumentString = function(intLength, strArgsName)
{
	var argumentName	= (!IsUndefined(strArgsName) ? strArgsName : "arguments");
	var argumentString	= "";
	var argumentCount	= ToNumber(intLength);

	if (IsNumeric(argumentCount))
	{
		for (var i = 0; i < argumentCount; ++i)
		{
			argumentString += argumentName + "[" + i + "]" + ((i + 1 != argumentCount) ? "," : "");
		}
	}
	return argumentString;
}
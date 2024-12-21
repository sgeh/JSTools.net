/// <method>
/// Checks, if this string ends with the given string to check.
/// </method>
/// <param name="strTrimChar" type="String">String to check.</param>
/// <returns type="Boolean">Returns true, if this string end with the given string to check.</returns>
String.prototype.EndsWith = function(strToCheck)
{
	if (typeof(strToCheck) != 'string' || strToCheck == String.Empty)
		return false;

	return (this.lastIndexOf(strToCheck) == this.length - strToCheck.length);
}


/// <method>
/// Checks, if this string starts with the given string to check.
/// </method>
/// <param name="strTrimChar" type="String">String to check.</param>
/// <returns type="Boolean">Returns true, if this string starts with the given string to check.</returns>
String.prototype.StartsWith = function(strToCheck)
{
	if (typeof(strToCheck) != 'string' || strToCheck == String.Empty)
		return false;

	return (this.indexOf(strToCheck) == 0);
}


/// <method>
/// Right aligns this string, padding with spaces on the left for a specified total length.
/// </method>
/// <param name="intLength" type="Integer">Total length of the string.</param>
/// <param name="intLength" type="String">String to insert.</param>
/// <returns type="String">Returns a stirng with the specified length.</returns>
String.prototype.PadLeft = function(intLength, strToPad)
{
	if (isNaN(intLength) || !intLength)
		return this.valueOf();

	var returnValue = this.valueOf();
	var padString = (typeof(strToPad) != 'undefined') ? String(strToPad) : " ";

	for (var i = 0; i < intLength - this.length; ++i)
	{
		returnValue = padString + returnValue;
	}
	return returnValue;
}


/// <method>
/// Left aligns this string, padding with spaces on the right for a specified total length.
/// </method>
/// <param name="intLength" type="Integer">Total length of the string.</param>
/// <param name="intLength" type="String">String to insert.</param>
/// <returns type="String">Returns a stirng with the specified length.</returns>
String.prototype.PadRight = function(intLength, strToPad)
{
	if (isNaN(intLength) || !intLength)
		return this.valueOf();

	var returnValue = this.valueOf();
	var padString = (typeof(strToPad) != 'undefined') ? String(strToPad) : " ";

	for (var i = 0; i < intLength - this.length; ++i)
	{
		returnValue += padString;
	}
	return returnValue;
}


/// <method>
/// Returns a string without the given trim char on the right side.
/// </method>
/// <param name="strTrimChar" type="String">Char, which should be trimmed.</param>
/// <returns type="String">Returns the trimmed string.</returns>
String.prototype.TrimStart = function(strTrim)
{
	var toTrim = this;
	var trimString = (strTrim) ? String(strTrim) : " ";

	while (toTrim.charAt(0) == trimString.charAt(0))
	{
		toTrim = toTrim.substring(1);
	}
	return toTrim;
}


/// <method>
/// Returns a string without the given trim char on the left side.
/// </method>
/// <param name="strTrimChar" type="String">Char, which should be trimmed.</param>
/// <returns type="String">Returns the trimmed string.</returns>
String.prototype.TrimEnd = function(strTrim)
{
	var toTrim = this;
	var trimString = (strTrim) ? String(strTrim) : " ";

	while (toTrim.charAt(toTrim.length - 1) == trimString.charAt(0))
	{
		toTrim = toTrim.substring(0, toTrim.length - 1);
	}
	return toTrim;
}


/// <method>
/// Returns a string without the given trim char on the left or right side.
/// </method>
/// <param name="strTrimChar" type="String">Char, which should be trimmed.</param>
/// <returns type="String">Returns the trimmed string.</returns>
String.prototype.Trim = function(strTrimChar)
{
	var firstReplace = this.TrimStart(strTrimChar);
	return firstReplace.TrimEnd(strTrimChar);
}


/// <method>
/// Replaces the first occurence of the given string to find with the specified replacement string.
/// </method>
/// <param name="strFind" type="String">String to find.</param>
/// <param name="strReplace" type="String">String to replace.</param>
/// <returns type="String">Returns the replaced string.</returns>
String.prototype.ReplaceFirst = function(strFind, strReplace)
{
	return this.ReplaceBy(strFind, strReplace, this.indexOf(strFind));
}


/// <method>
/// Replaces the last occurence of the given string to find with the specified replacement string.
/// </method>
/// <param name="strFind" type="String">String to find.</param>
/// <param name="strReplace" type="String">String to replace.</param>
/// <returns type="String">Returns the replaced string.</returns>
String.prototype.ReplaceLast = function(strFind, strReplace)
{
	return this.ReplaceBy(strFind, strReplace, this.lastIndexOf(strFind));
}


/// <method>
/// Checks, if this string is alpha numeric.
/// </method>
/// <returns type="String">Returns true, if this string is alpha numeric (a-zA-Z0-9).</returns>
String.prototype.IsAlphaNumeric = function()
{
	return (this.search(/^\w*$/) != -1);
}


/// <method>
/// Checks, if this string contains only white spaces.
/// </method>
/// <returns type="String">Returns true, if this string contains only white spaces.</returns>
String.prototype.IsWhiteSpace = function()
{
	return (this.search(/^\s+$/) != -1);
}


/// <method>
/// Checks, if this string contains some white spaces.
/// </method>
/// <returns type="String">Returns true, if this string contains some white spaces.</returns>
String.prototype.ContainsWhiteSpace = function()
{
	return (this.search(/\s/) != -1);
}


/// <method>
/// Replaces the given string at the specified index with the value of the string to replace param.
/// </method>
/// <param name="strToFind" type="String">String which should be found.</param>
/// <param name="strToReplace" type="String">String which should be used for the replacement.</param>
/// <param name="intReplaceAt" type="Integer">Index, at which the replacement should be done.</param>
/// <returns type="String">Returns the replaced string.</returns>
String.prototype.ReplaceBy = function(strToFind, strToReplace, intReplaceAt)
{
	if (isNaN(intReplaceAt))
		return this;

	intReplaceAt = Number(intReplaceAt);

	if (intReplaceAt > 0 && intReplaceAt < this.length)
	{
		if (this.substring(intReplaceAt).BeginsWith(strToFind))
		{
			var newBegin = this.substring(0, intReplaceAt);
			var newEnd = this.substring(intReplaceAt + String(strToFind).length);
			return newBegin + String(strToReplace) + newEnd;
		}
	}
	return this;
}


/// <method>
/// Escapes all non alphanumeric characters with a backslash "\".
/// </method>
/// <returns type="String">Returns the escaped string.</returns>
String.prototype.EscapeNonAlphanumerics = function()
{
	return this.replace(/(\W)/g, "\\$1");
}


/// <method>
/// Replaces the char at the given index with a upper char.
/// </method>
/// <param name="intIndex" type="Integer">Index of the char.</param>
/// <returns type="String">Returns the replaced string.</returns>
String.prototype.UpperCaseAt = function(intIndex)
{
	if (isNaN(intLength))
		return this;

	if (intIndex > 0 && intIndex < this.length)
	{
		return this.ReplaceBy(this.charAt(intIndex), String.fromCharCode(this.charCodeAt(intIndex) & 0x0DF), intIndex, this.valueOf());
	}
	return this;
}


/// <property type="String">/// Returns an empty string ("")./// </property>String.Empty = "";/// <method>/// Replaces the specified {x} patterns in the pattern string with the specified/// arguments. The first argument after the pattern string will be replaced with/// the {0} mark, the second one with the {1}, and so on./// </method>/// <param name="strPattern">String which contains the place holders (pattern).</param>/// <param name="params">Values which should be inserted at the corresponding pattern index.</param>/// <returns type="String">Returns the resulting string.</returns>String.Format = function(strPattern){	var stringToFormat = String(strPattern);	for (var i = 1; i < arguments.length; ++i)	{		if (typeof(arguments[i]) == 'undefined')			continue;		stringToFormat = stringToFormat.replace(new RegExp("\\{" + (i - 1) + "\\}", "gi"), String(arguments[i]));	}	return stringToFormat;}


/// <method>
/// Creates a comma separated string with the specified length and the specified name.
/// </method>
/// <param name="intLength" type="Integer">Length of arguments to create.</param>
/// <param name="strArgsName" type="String">Name of arguments to create. The default value is "arguments".</param>
/// <param name="intStart" type="Integer">Index, at which the first item begins. The tefault value is 0.</param>
/// <returns type="String">Returns the created string.</returns>
/// <example>
/// If you specify parameters like (3, "myName") this function creates a string
/// like "myName[0],myName[1],myName[2]".
///  <code>
///  var argString = String.CreateArgumentString(3, "myName");
///  // argString contains myName[0],myName[1],myName[2]
///  </code>
/// </example>
String.CreateArgumentString = function(intLength, strArgsName, intStart)
{
	if (isNaN(intLength))
		return String.Empty;

	var argumentName = (typeof(strArgsName) != 'undefined') ? strArgsName : "arguments";
	var argumentString = "";
	var argumentCount = Number(intLength);
	var argumentStart = (!isNaN(intStart) && intStart > 0 && intStart < intLength) ? intStart : 0;

	for (var i = argumentStart; i < argumentCount; ++i)
	{
		argumentString += argumentName + "[" + i + "]" + ((i + 1 != argumentCount) ? "," : "");
	}
	return argumentString;
}
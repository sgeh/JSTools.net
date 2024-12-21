/*
 * JSTools.JavaScript / JSTools.net - A JavaScript/C# framework.
 * Copyright (C) 2005  Silvan Gehrig
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 *
 * Author:
 *  Silvan Gehrig
 */

namespace("JSTools.Reflection");


/// <class>
/// The TypeParser is required for parsing the private members (methods / fields).
/// </class>
/// <param name="objToParse" type="Object">Any type of object, which contains the members to reflect.</param>
JSTools.Reflection.TypeParser = function(objToParse)
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	if (typeof(objToParse) != 'object' || !objToParse)
		return;

	if (!objToParse.GetType || objToParse.GetType() == null || objToParse.GetType().GetConstructor() == null)
		return;


	var FUNCTION_ANONYMOUS_REGEXP = Function.__anonymousParsePattern;
	var FUNCTION_QUALIFIED_REGEXP = Function.__qualifiedParsePattern;
	var WHITE_SPACE_REGEXP = new RegExp("\\s+", "g");

	var PRIVATE_FIELD_REGEXP = new RegExp("var\\s+([0-9a-zA-Z$_]+\\s*(,\\s*[0-9a-zA-Z$_]+\\s*)*)", "g");
	var PRIVATE_FIELD_DECL = new RegExp("var|\\s+", "g");

	var _this = this;
	var _object = objToParse;
	var _function = objToParse.GetType().GetConstructor();
	var _functionString = String.Empty;

	var _functions = [ ];
	var _fields = [ ];


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------

	/// <constructor>
	/// Creates a new JSTools.Reflection.TypeParser instance.
	/// </constructor>
	function Init()
	{
		var _functionString = _function.toString().replace(WHITE_SPACE_REGEXP, " ");

		// get function body
		var _functionString = GetFunctionBody(_functionString);

		// remove anonymous functions
		_functionString = RemoveNestedFunctions(_functionString, FUNCTION_ANONYMOUS_REGEXP);

		// remove qualified functions
		_functionString = RemoveNestedFunctions(_functionString, FUNCTION_QUALIFIED_REGEXP, _functions, 1);

		// initialize private fields
		InitPrivateFields(_functionString);
	}


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Gets the private methods of the representing class.
	/// </method>
	/// <returns type="Array">Returns the name of the private methods.</returns>
	function GetPrivateMethods()
	{
		return _functions;
	}
	this.GetPrivateMethods = GetPrivateMethods;


	/// <method>
	/// Gets the private fields of the representing class.
	/// </method>
	/// <returns type="Array">Returns the name of the private fields.</returns>
	function GetPrivateFields()
	{
		return _fields;
	}
	this.GetPrivateFields = GetPrivateFields;


	/// <method>
	/// Initializes the private members of the given class.
	/// </method>
	/// <param type="String" name="strToInit">String, which contains the class declaration.</param>
	function InitPrivateFields(strToInit)
	{
		var fieldDeclarations = strToInit.match(PRIVATE_FIELD_REGEXP);

		if (!fieldDeclarations)
			return;

		for (var i = 0; i < fieldDeclarations.length; ++i)
		{
			var privateFields = fieldDeclarations[i].replace(PRIVATE_FIELD_DECL, String.Empty).split(",");

			for (var j = 0; j < privateFields.length; ++j)
			{
				_fields.Add(privateFields[j]);
			}
		}
	}


	/// <method>
	/// Removes all nested functions, which are matched by the given pattern. Stores the
	/// specified match value into the given array, if it was specified.
	/// </method>
	function RemoveNestedFunctions(strToParse, objRegExpPattern, arrStore, intMatchIndex)
	{
		var functionIndex = strToParse.search(objRegExpPattern);

		// remove qualified functions
		while (functionIndex != -1)
		{
			var endingBracket = GetEndingBracket(strToParse, functionIndex);

			// store first regexp item
			if (typeof(arrStore) == 'object')
			{
				arrStore.Add(RegExp["$" + intMatchIndex]);
			}

			// remove inner nested function
			strToParse = strToParse.substring(0, functionIndex) + strToParse.substring(endingBracket);
			
			// search for more nested functions
			functionIndex = strToParse.search(objRegExpPattern);
		}
		return strToParse;
	}


	/// <method>
	/// Checks for a ending bracket at the given index.
	/// </method>
	/// <param type="String" name="strToSearch">String which contains the brackets.</param>
	/// <param type="Integer" name="intStartIndex">Start position.</param>
	/// <returns type="Integer">Returns the position of the ending bracket.</param>
	function GetEndingBracket(strToSearch, intStartIndex)
	{
		var bracketsCount = 0;
		var bracketStarted = false;

		for (var i = intStartIndex; i < strToSearch.length && !(bracketStarted && bracketsCount == 0); ++i)
		{
			if (strToSearch.charAt(i) == '{')
			{
				++bracketsCount;
				bracketStarted = true;
			}
			else if (strToSearch.charAt(i) == '}')
			{
				--bracketsCount;
			}
		}
		return (bracketsCount > 0) ? -1 : i;
	}

	
	/// <method>
	/// Returns the function body of the given function string.
	/// </method>
	/// <param type="String" name="strFunction">Function to get the body.</param>
	/// <returns type="String">Returns body of the given function.</param>
	function GetFunctionBody(strFunction)
	{
		var firstIndex = strFunction.indexOf("{");
		var lastIndex = strFunction.lastIndexOf("}");
		
		if (firstIndex == -1 || lastIndex == -1)
		{
			return String.Empty;
		}
		return strFunction.substring(firstIndex + 1, lastIndex);
	}
	Init();
}

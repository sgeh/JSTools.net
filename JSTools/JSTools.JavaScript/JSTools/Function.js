/// <property type="MemberProtector">
/// Gets/sets the MemberProtector instance. This property should not be used
/// directly from your code. Do not change the value of this property,
/// otherwise script execution may result in an error.
/// </property>
Function.prototype.__memberProtector = null;


/// <method>
/// Gets the member protector instance, which is required for the inheritance
/// mechanism.
/// </method>
Function.prototype.GetMemberProtector = function()
{
	if (!this.__memberProtector ||
		typeof(this.__memberProtector) != 'object' ||
		this.__memberProtector.GetType() == null ||
		this.__memberProtector.GetType().GetConstructor() != JSTools.Reflection.MemberProtector)
	{
		this.__memberProtector = new JSTools.Reflection.MemberProtector();
	}
	return this.__memberProtector;
}


/// <method>
/// Gets the name of this function. Default return value is "".
/// </method>
Function.prototype.GetName = function()
{
	var nameResult = Function.__qualifiedParsePattern.exec(this.toString());

	if (nameResult && nameResult.length)
		return nameResult[1];

	return String.Empty;
}


/// <method>
/// A sealed class does not allow a derived class. If you do not override
/// this method, it will always return false.
/// </method>
/// <returns type="Boolean">Returns true if this function or class is sealed.</returns>
Function.prototype.IsSealed = function()
{
	return false;
}


/// <method>
/// Returns true if the specified object represents a callable function
/// pointer.
/// Attention: IE provides native functions, which could not be called if the
/// window handle was disposed. Native functions do not provide a property or
/// something to find out whether the function could be called.
/// </method>
/// <param type="Object" name="objFunction">Object which should be checked.</param>
/// <returns type="Boolean">Returns true if the specified object can be called (script- or native function).</returns>
Function.IsCallable = function(objFunction)
{
	return ((typeof(objFunction) == 'function' || typeof(objFunction) == 'object')
		&& typeof(objFunction) != 'unknown'
		&& typeof(objFunction.toString) != 'unknown'
		&& String(objFunction).StartsWith("\\s*function\\s*", true));
}


/// <method>
/// Calls the a function pointer with the specified arguments. This
/// implementation is slow, you should avoid using it.
/// If the function could not be called, a null reference is returned.
/// </method>
/// <param type="Function" name="objFunctionToCall">Function pointer to call.</param>
/// <param type="Array" name="varArguments">An array of arguments which items are passed as parameter to the function to call.</param>
/// <param type="param" name="varArguments">Arguments which are passed as parameter to the function to call.</param>
/// <returns type="Object">Returns the function call result or a null reference, if the specified
/// function could not be called.</returns>
Function.Call = function(objFunctionToCall, varArguments)
{
	var FUNC_CALL = "objFunctionToCall( {0} );"
	var funcCallResult = null;

	if (Function.IsCallable(objFunctionToCall))
	{
		var callArguments;

		if (varArguments && varArguments.length)
			callArguments = varArguments;
		else
			callArguments = Array.GetRange(arguments, 1, arguments.length - 1);
			
		if (callArguments.length > 0)
			funcCallResult = eval(String.Format(FUNC_CALL, String.CreateArgumentString(callArguments.length, "callArguments")));
		else
			funcCallResult = objFunctionToCall();
	}
	return funcCallResult;
}


/// <method>
/// Calls a function with the given name and the specified arguments in
/// the specified context. This implementation is slow, you should avoid
/// using it.
/// If the function could not be called, a null reference is returned.
/// </method>
/// <param type="String" name="strFunctionToCall">Name of the function to call.</param>
/// <param type="Object" name="objContext">Context which is used to call the function.</param>
/// <param type="Array" name="varArguments">An array of arguments which items are passed as parameter to the function to call.</param>
/// <param type="param" name="varArguments">Arguments which are passed as parameter to the function to call.</param>
/// <returns type="Object">Returns the function call result or a null reference, if the specified
/// function could not be called.</returns>
Function.CallContext = function(strFunctionToCall, objContext, varArguments)
{
	var FUNC_CALL = "objContext[strFunctionToCall]( {0} );"
	var funcCallResult = null;

	if (typeof(objContext) == 'object' && Function.IsCallable(objContext[strFunctionToCall]))
	{
		var callArguments;

		if (varArguments && varArguments.length)
			callArguments = varArguments;
		else
			callArguments = Array.GetRange(arguments, 1, arguments.length - 1);
			
		if (callArguments.length > 0)
			funcCallResult = eval(String.Format(FUNC_CALL, String.CreateArgumentString(callArguments.length, "callArguments")));
		else
			funcCallResult = objContext[strFunctionToCall]();
	}
	return funcCallResult;
}


/// <method>
/// Calls the a function pointer with the specified arguments in the given
/// context. This implementation is slow, you should avoid using it.
/// If the function could not be called, a null reference is returned.
/// </method>
/// <param type="String" name="strFunctionToCall">Name of the function to call.</param>
/// <param type="Object" name="objContext">Context which is used to call the function.</param>
/// <param type="Array" name="varArguments">An array of arguments which items are passed as parameter to the function to call.</param>
/// <param type="param" name="varArguments">Arguments which are passed as parameter to the function to call.</param>
/// <returns type="Object">Returns the function call result or a null reference, if the specified
/// function could not be called.</returns>
Function.CallChangeContext = function(objFunctionToCall, objContext, varArguments)
{
	var FUNCTION_NAME_PATTERN = "__tmpFuncPtrName_${0}";
	var funcCallResult = null;

	if (typeof(objContext) == 'object')
	{
		// evaluate a function name, which is currently not in use
		var functionName;

		for (var i = 0; ; ++i)
		{
			functionName = String.Format(FUNCTION_NAME_PATTERN, i);
			
			if (typeof(objContext[functionName]) == 'undefined')
				break;
		}

		// assign temporary function name
		objContext[functionName] = objFunctionToCall;

		// call function pointer
		if (arguments.length == 3)
			funcCallResult = Function.CallContext(functionName, objContext, varArguments);
		else
			funcCallResult = Function.CallContext(functionName, objContext, Array.GetRange(arguments, 2, arguments.length - 2));

		// clean up memory, this will remove the function pointer from the context
		objContext[functionName] = null;
		delete objContext[functionName];
	}
	return funcCallResult;
}


/// <property type="RegExp">
/// Gets/sets a pattern, which is used to parse the code of a qualified
/// javascript function. This property should not be used directly from
/// your code. Do not change the value of this property, otherwise script
/// execution will result in an error.
/// </property>
Function.__qualifiedParsePattern = new RegExp("\\s*function\\s+([0-9a-zA-Z$_]+)\\s*\\(\\s*(([0-9a-zA-Z$_]+\\s*)(,\\s*[0-9a-zA-Z$_]+\\s*)*)?\\)\\s*\\{");



/// <property type="RegExp">
/// Gets/sets a pattern, which is used to parse the code of a anonymous
/// javascript function. This property should not be used directly from
/// your code. Do not change the value of this property, otherwise script
/// execution will result in an error.
/// </property>
Function.__anonymousParsePattern = new RegExp("\\s*function\\s*\\(\\s*(([0-9a-zA-Z$_]+\\s*)(,\\s*([0-9a-zA-Z$_]+)\\s*)*)?\\)\\s*\\{");
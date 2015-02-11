// Undefined variable, required for generation 4 browsers.
var undefined;


/// <property type="JSTools.Reflection.Type">
/// Gets/sets the Type instance. This property should not be used
/// directly from your code. Do not change the value of this property,
/// otherwise script execution will result in an error.
/// </property>
Object.prototype.__type = null;
/// <method>/// Initializes the type object. This is required for reflection and inheritance/// mechanism./// </method>/// <remarks>Do not use this function inside classes, which are designed to use/// with prototype inheritance mechanism.</remarks>/// <param name="objArgument" type="String">Object, which contains the callee of this object.</param>/// <param name="strTypeName" type="String">Name of the type to initialize.</param>/// <param name="blnSealed" type="Boolean">A boolean indicating whether the class is sealead and/// cannot be inherited.</param>Object.prototype.InitType = function(objArgument, strTypeName, blnSealed){	if (typeof(objArgument) == 'object' && typeof(objArgument.callee) == 'function')	{
		if (!this.__type || typeof(this.__type) != 'object' || !this.__type.IsTypeInstance)
		{
			this.__type = new JSTools.Reflection.Type(this, objArgument.callee, strTypeName, blnSealed);
		}
		else
		{
			this.__type.AddBaseType(new JSTools.Reflection.Type(this, objArgument.callee, strTypeName, blnSealed));
		}
	}
}/// <method>/// Returns the type of this object. It reflects the members and other values./// </method>/// <returns type="JSTools.Reflection.Type">Returns the Type instance this value contains a null/// reference, if the type was not initialized by the the "InitType" method.</returns>Object.prototype.GetType = function(){	return this.__type;}/// <method>/// Returns true, if the given function is equal to the type or a base type of this object. To use/// this method, you have to initialize the current type. Otherwise you will always obtain false./// </method>/// <param name="objFunctionType" type="Function">Function to check.</param>/// <returns type="Boolean">Returns true, if the given function is equal to the type or a base type/// of this object.</returns>Object.prototype.IsTypeOf = function(objFunctionType){	if (typeof(objFunctionType) != 'function')		return false;	return (this.GetType		&& this.GetType() != null		&& this.GetType().IsTypeOf(objFunctionType));}/// <method>
/// Inherits from the given function object. The given params after the first argument
/// will be passed to the function constructor. You should not use arguments, it is a
/// performance hit.
///
/// All public members declared in this object will be overriden with those specified
/// in the given function.
/// <method>
/// <returns type="Object">Returns an object, which contains the protected members.</returns>
/// <remarks>
/// This runtime inheritance mechanism is a performance hit (about 40 ms / object). You should
/// use it with caution if you have performance restrictions.
/// Prototype inheritance is not directly supported. Do not combine the prototype inheritance/// mechanism and this runtime inheritance mechanism./// </remarks>
/// <param name="objFunction" type="Function">Function (class) to inherit. Default
/// function (class) is Object.</param>
/// <param name="params">Values which should be passed to the base constructor.</param>
/// <returns type="Array">Returns the protected members of the base or the current function object.
/// If the return value contains a null reference, the inheritance has failed. Please check that
/// the type is initialized. To initialize the type, add the following statements at the first line
/// of your class definition: this.InitType(arguments, "[TypeName]").</returns>
Object.prototype.Inherit = function(objFunction)
{
	// check current object for valid status
	// do not derive a class from itself
	if (this.GetType() == null
		|| this.GetType().GetConstructor() == null
		|| this.IsTypeOf(objFunction))
		return null;

	// is the given class sealed
	if (typeof(objFunction) == 'function'
		&& typeof(objFunction.IsSealed) == 'function'
		&& objFunction.IsSealed())
		return null;

	// return default members if Object or nothing is specified
	if (objFunction == Object || typeof(objFunction) != 'function')
		return this.GetType().GetConstructor().GetMemberProtector().GetProtectedItems(this);

	// add the function pointer to this object
	this.__constructor = objFunction;

	if (arguments.length > 1)
	{
		// create argument string
		var constructArgString = String.CreateArgumentString(arguments.length, "arguments", 1);

		// call function pointer. this procedure will write all members into the objApply object
		eval("this.__constructor(" + constructArgString + ");");
	}
	else
	{
		// call function pointer, without parameters.
		// this call is much faster than the call with parameters
		this.__constructor();
	}

	// clean up memory, this will remove the function pointer
	this.__constructor = null;
	delete this.__constructor;

	// get protected members of top object
	return this.GetType().GetConstructor().GetMemberProtector().GetProtectedItems(this);
}
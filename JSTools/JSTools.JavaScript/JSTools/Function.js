/// <property type="MemberProtector">
/// Gets/sets the MemberProtector instance. This property should not be used
/// directly from your code. Do not change the value of this property,
/// otherwise script execution will result in an error.
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
/// Gets the name of this function. Default value is "".
/// </method>
Function.prototype.GetName = function()
{
	var nameResult = Function.__qualifiedParsePattern.exec(this.toString());

	if (nameResult && nameResult.length)
	{
		return nameResult[1];
	}
	return String.Empty;
}


/// <method>
/// A sealed class does not allow a derived class. If you do not override
/// this method, it will always return false.
/// </method>
/// <returns type="Boolean">Returns true if this function is sealed.</returns>
Function.prototype.IsSealed = function()
{
	return false;
}


/// <property type="RegExp">
/// Gets/sets a pattern, which is used to parse the code of a qualified
/// javascript function. This property should not be used directly from
/// your code. Do not change the value of this property, otherwise script
/// execution will result in an error.
/// </property>
Function.__qualifiedParsePattern = new RegExp("function\\s+([0-9a-zA-Z$_]+)\\s*\\(\\s*(([0-9a-zA-Z$_]+\\s*)(,\\s*[0-9a-zA-Z$_]+\\s*)*)?\\)\\s*\\{");



/// <property type="RegExp">
/// Gets/sets a pattern, which is used to parse the code of a anonymous
/// javascript function. This property should not be used directly from
/// your code. Do not change the value of this property, otherwise script
/// execution will result in an error.
/// </property>
Function.__anonymousParsePattern = new RegExp("function\\s*\\(\\s*(([0-9a-zA-Z$_]+\\s*)(,\\s*([0-9a-zA-Z$_]+)\\s*)*)?\\)\\s*\\{");
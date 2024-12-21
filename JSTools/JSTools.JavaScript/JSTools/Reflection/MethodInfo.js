namespace("JSTools.Reflection");


/// <class>
/// Provides reflection informations about a field. Private methods cannot be invoked because inner
/// scopes can't be called from the Invoke function scope.
/// </class>
/// <param name="objToRepresent" type="Object">Any type of object, which contains the field to reflect.</param>
/// <param name="strMethodName" type="String">Name of the method, which should be reflected.</param>
/// <param name="enuMemberVisibility" type="JSTools.Reflection.MemberVisibility">Visibility of the field to reflect.</param>
JSTools.Reflection.MethodInfo = function(objToRepresent, strMethodName, enuMemberVisibility)
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	if (typeof(objToRepresent) != 'object' || !objToRepresent)
		return;

	var _this = this;
	var _object = objToRepresent;
	var _methodName = String(strMethodName);
	var _memberVisiblity = enuMemberVisibility;


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------

	/// <constructor>
	/// Creates a new JSTools.Reflection.MethodInfo instance.
	/// </constructor>
	function Init()
	{
	}


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Returns the name of this member.
	/// </method>
	/// <returns type="String">Returns the name of this member.</returns>
	function GetName()
	{
		return _methodName;
	}
	this.GetName = GetName;


	/// <method>
	/// Returns the member type.
	/// </method>
	/// <returns type="JSTools.Reflection.MemberType">Returns the type of this member.</returns>
	function GetMemberType()
	{
		return JSTools.Reflection.MemberType.Method;
	}
	this.GetMemberType = GetMemberType;


	/// <method>
	/// Returns true, if this member is internal (begins with __). This
	/// Members should not be used from your code.
	/// </method>
	/// <returns type="Boolean">Returns true, if this member is used for internal purposes.</returns>
	function IsInternal()
	{
		return _this.GetName().StartsWith("__");
	}
	this.IsInternal = IsInternal;


	/// <method>
	/// Invokes the representing method with the specified arguments. The type of the
	/// object to call must be initialized, otherwise the protected members will not
	/// be called.
	/// </method>
	/// <param name="param">Arguments which should be passed to the function to invoke.</param>
	/// <returns type="Object">Returns the result of the called function.</returns>
	function Invoke()
	{
		var resultValue;

		if (_this.IsPublic())
		{
			resultValue = Function.CallContext(_methodName, _object, arguments);
		}
		else if (_this.IsProtected())
		{
			// get constructor
			var objectConstructor = _object.GetType().GetConstructor();

			if (objectConstructor)
			{
				// get protected members from MemberProtector
				resultValue = Function.CallContext(
					_methodName,
					objectConstructor.GetMemberProtector().GetProtectedItems(_object, arguments),
					arguments );
			}
		}
		return resultValue;
	}
	this.Invoke = Invoke;


	/// <method>
	/// Checks if this field is public.
	/// </method>
	/// <returns type="Boolean">Returns true if this field is public.</returns>
	function IsPublic()
	{
		return (_memberVisiblity == JSTools.Reflection.MemberVisibility.Public);
	}
	this.IsPublic = IsPublic;


	/// <method>
	/// Checks if this field is protected.
	/// </method>
	/// <returns type="Boolean">Returns true if this field is protected.</returns>
	function IsProtected()
	{
		return (_memberVisiblity == JSTools.Reflection.MemberVisibility.Protected);
	}
	this.IsProtected = IsProtected;


	/// <method>
	/// Checks if this field is private.
	/// </method>
	/// <returns type="Boolean">Returns true if this field is private.</returns>
	function IsPrivate()
	{
		return (_memberVisiblity == JSTools.Reflection.MemberVisibility.Private);
	}
	this.IsPrivate = IsPrivate;
}


// implement JSTools.Reflection.IMemberInfo interface
JSTools.Reflection.MethodInfo.prototype = new JSTools.Reflection.IMemberInfo();

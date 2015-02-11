namespace("JSTools.Reflection");


/// <class>
/// Provides reflection informations about a field. Values of private fields cannot be read or written.
/// </class>
/// <param name="objToRepresent" type="Object">Any type of object, which contains the field to reflect.</param>
/// <param name="strFieldName" type="String">Name of the field, which should be reflected.</param>
/// <param name="enuMemberVisibility" type="JSTools.Reflection.MemberVisibility">Visibility of the field to reflect.</param>
JSTools.Reflection.FieldInfo = function(objToRepresent, strFieldName, enuMemberVisibility)
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	if (typeof(objToRepresent) != 'object' || !objToRepresent)
		return;

	var _this = this;
	var _object = objToRepresent;
	var _fieldName = String(strFieldName);
	var _memberVisiblity = enuMemberVisibility;


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------

	/// <constructor>
	/// Creates a new JSTools.Reflection.FieldInfo instance.
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
		return _fieldName;
	}
	this.GetName = GetName;


	/// <method>
	/// Returns the member type.
	/// </method>
	/// <returns type="JSTools.Reflection.MemberType">Returns the type of this member.</returns>
	function GetMemberType()
	{
		return JSTools.Reflection.MemberType.Field;
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


	/// <method>
	/// Returns the value of this field.
	/// </method>
	/// <returns type="Object">Returns the value of the given field.</returns>	
	function GetValue()
	{
		if (_this.IsPublic())
		{
			return _object[_fieldName];
		}
		else if (_this.IsProtected())
		{
			// get constructor
			var objectConstructor = _object.GetType().GetConstructor();

			if (objectConstructor)
			{
				// get protected members from MemberProtector
				var protectedItems = objectConstructor.GetMemberProtector().GetProtectedItems(_object, arguments);
				return protectedItems[_fieldName];
			}
		}
		return undefined;
	}
	this.GetValue = GetValue;


	/// <method>
	/// Writes the value of this field.
	/// </method>
	/// <param name="objValue" type="Object">Value to write.</param>
	function SetValue(objValue)
	{
		if (_this.IsPublic())
		{
			_object[_fieldName] = objValue;
		}
		else if (_this.IsProtected())
		{
			// get constructor
			var objectConstructor = _object.GetType().GetConstructor();

			if (objectConstructor)
			{
				// get protected members from MemberProtector
				var protectedItems = objectConstructor.GetMemberProtector().GetProtectedItems(_object, arguments);
				protectedItems[_fieldName] = objValue;
			}
		}
	}
	this.SetValue = SetValue;

	Init();
}


// implement JSTools.Reflection.IMemberInfo interface
JSTools.Reflection.FieldInfo.prototype = new JSTools.Reflection.IMemberInfo();

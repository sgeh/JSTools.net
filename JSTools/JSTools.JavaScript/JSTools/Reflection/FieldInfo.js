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

	var _this				= this;
	var _object				= objToRepresent;
	var _fieldName			= String(strFieldName);
	var _memberVisiblity	= enuMemberVisibility;


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
	this.GetName = function()
	{
		return _fieldName;
	}


	/// <method>
	/// Returns the member type.
	/// </method>
	/// <returns type="JSTools.Reflection.MemberType">Returns the type of this member.</returns>
	this.GetMemberType = function()
	{
		return JSTools.Reflection.MemberType.Field;
	}
	

	/// <method>
	/// Returns true, if this member is internal (begins with __). This
	/// Members should not be used from your code.
	/// </method>
	/// <returns type="Boolean">Returns true, if this member is used for internal purposes.</returns>
	this.IsInternal = function()
	{
		return _this.GetName().StartsWith("__");
	}


	/// <method>
	/// Checks if this field is public.
	/// </method>
	/// <returns type="Boolean">Returns true if this field is public.</returns>
	this.IsPublic = function()
	{
		return (_memberVisiblity == JSTools.Reflection.MemberVisibility.Public);
	}


	/// <method>
	/// Checks if this field is protected.
	/// </method>
	/// <returns type="Boolean">Returns true if this field is protected.</returns>
	this.IsProtected = function()
	{
		return (_memberVisiblity == JSTools.Reflection.MemberVisibility.Protected);
	}


	/// <method>
	/// Checks if this field is private.
	/// </method>
	/// <returns type="Boolean">Returns true if this field is private.</returns>
	this.IsPrivate = function()
	{
		return (_memberVisiblity == JSTools.Reflection.MemberVisibility.Private);
	}


	/// <method>
	/// Returns the value of this field.
	/// </method>
	/// <returns type="Object">Returns the value of the given field.</returns>	
	this.GetValue = function()
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


	/// <method>
	/// Writes the value of this field.
	/// </method>
	/// <param name="objValue" type="Object">Value to write.</param>
	this.SetValue = function(objValue)
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
	Init();
}


// implement JSTools.Reflection.IMemberInfo interface
JSTools.Reflection.FieldInfo.prototype = new JSTools.Reflection.IMemberInfo();

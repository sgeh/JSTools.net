namespace("JSTools.Reflection");


/// <interface>
/// Represents an interface, which contains informations about a type member.
/// </interface>
JSTools.Reflection.IMemberInfo = function()
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Returns the name of this member.
	/// </method>
	/// <returns type="String">Returns the name of this member.</returns>
	this.GetName = function() { }


	/// <method>
	/// Returns the member type.
	/// </method>
	/// <returns type="Boolean">Returns the type of this member.</returns>
	this.GetMemberType = function() { }


	/// <method>
	/// Returns true, if this member is internal (begins with __). This
	/// Members should not be used from your code.
	/// </method>
	/// <returns type="Boolean">Returns true, if this member is used for internal purposes.</returns>
	this.IsInternal = function() { }
}

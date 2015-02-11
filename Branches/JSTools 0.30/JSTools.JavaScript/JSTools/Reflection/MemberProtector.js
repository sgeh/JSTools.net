namespace("JSTools.Reflection");


/// <class>
/// Contains the protected members, which should be protected from
/// the global script code. All instances of the corresponding Function
/// will be stored too. Use GetInstances() to recieve the instances.
/// </class>
JSTools.Reflection.MemberProtector = function()
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	this.InitType(arguments, "JSTools.Reflection.MemberProtector");

	var _this		= this;
	var _instances	= new Array();


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Registers the given object in this JSTools.Reflection.MemberProtector instance.
	/// </method>
	/// <param name="objInstance" type="Object">Object to register.</param>
	/// <returns type="Array">Returns the protected member array.</returns>
	this.GetProtectedItems = function(objInstance)
	{
		if (!objInstance || typeof(objInstance) != 'object' || !objInstance.GetType)
			return null;

		var guid = objInstance.GetType().GetGuid();

		if (_instances[guid])
		{
			return _instances[guid].ProtectedMembers;
		}
		else
		{
			return Register(objInstance);
		}
	}


	/// <method>
	/// returns all objects, which are derived from the representing function.
	/// </method>
	this.GetInstances = function()
	{
		var instances = new Array(_instances.length);

		for (var i = 0; i < _instances.length; ++i)
		{
			instances.Add(_instances[_instances[i]].Object);
		}
		return instances;
	}


	/// <method>
	/// Registers the given object in this MemberProtector instance.
	/// </method>
	/// <param name="objInstance" type="Object">Object to register.</param>
	/// <returns type="Object">Returns an object, which contains the protected members.</returns>
	function Register(objInstance)
	{
		if (typeof(objInstance) != 'object' || !objInstance.GetType)
			return null;

		var guid = objInstance.GetType().GetGuid();

		// the array was already obtained
		if (_instances[guid])
			return _instances[guid];

		_instances[guid] = { Object: objInstance, ProtectedMembers: { } };
		_instances.Add(guid);

		return _instances[guid].ProtectedMembers;
	}
}

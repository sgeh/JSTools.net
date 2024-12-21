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

	var _this = this;
	var _instances = new Array();


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
	function GetProtectedItems(objInstance)
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
	this.GetProtectedItems = GetProtectedItems;


	/// <method>
	/// returns all objects, which are derived from the representing function.
	/// </method>
	function GetInstances()
	{
		var instances = new Array(_instances.length);

		for (var i = 0; i < _instances.length; ++i)
		{
			instances.Add(_instances[_instances[i]].Object);
		}
		return instances;
	}
	this.GetInstances = GetInstances;


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

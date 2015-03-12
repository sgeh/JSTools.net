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

namespace("JSTools.Util");


/// <class>
/// Represents a collection of key-and-value pairs that are organized based on
/// the object property assignment.
/// </class>
JSTools.Util.Hashtable = function()
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	var _this = this;
	var _storage = null;


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------

	/// <constructor>
	/// Creates a new JSTools.Util.Hashtable instance.
	/// </constructor>
	function Init()
	{
		_this.Clear();
	}


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Gets the number of stored items.
	/// </method>
	/// <returns type="Integer">Returns the number of stored items.</returns>
	function Count()
	{
		var count = 0;

		for (var item in _storage)
		{
			++count;
		}
		return count;
	}
	this.Count = Count;


	/// <method>
	/// Creates a new array which contains the stored keys.
	/// </method>
	/// <returns type="Array">Returns all keys stored in this hashtable.</param>
	function GetKeys()
	{
		var keys = new Array(_this.Count());
		var i = 0;

		for (var item in _storage)
		{
			keys[i++] = item;
		}
		return keys;
	}
	this.GetKeys = GetKeys;


	/// <method>
	/// Creates a new array which contains the stored values.
	/// </method>
	/// <returns type="Array">Returns all values stored in this hashtable.</param>
	function GetValues()
	{
		var values = new Array(_this.Count());
		var i = 0;

		for (var item in _storage)
		{
			values[i++] = _storage[item];
		}
		return values;
	}
	this.GetValues = GetValues;


	/// <method>
	/// Gets an enumerator which contains the whole storage. This enumerator
	/// can be used to iterate through the elements.
	/// </method>
	/// <example>
	/// var keyValueStorage = new JSTools.Util.Hashtable();
	///
	/// for (var key in keyValueStorage.GetElements())
	/// {
	///		alert(key + " " + keyValueStorage.Get(key);
	/// }
	/// </example>
	/// <returns type="Object">Returns an enumerator object with the internal data.</param>
	function GetElements()
	{
		return _storage;
	}
	this.GetElements = GetElements;


	/// <method>
	/// Adds the given key/value pair into this hashtable. This function will perform
	/// no operations if an array enty with the given name already exists.
	/// </method>
	/// <param name="objKey" type="Object">The key whose value to add.</param>
	/// <param name="objValue" type="Object">The value to store.</param>
	function Add(objKey, objValue)
	{
		if (!_this.ContainsKey(objKey))
		{
			_storage[objKey] = objValue;
		}
	}
	this.Add = Add;


	/// <method>
	/// Assigns the given value to the given key. If the key does not
	/// exist the given pair is added to the hashtable.
	/// </method>
	/// <param name="objKey" type="Object">The key whose value to set.</param>
	/// <param name="objValue" type="Object">The value to store.</param>
	function Set(objKey, objValue)
	{
		_storage[objKey] = objValue;
	}
	this.Set = Set;


	/// <method>
	/// Gets the value associated with the given key.
	/// </method>
	/// <param name="objKey" type="Object">The key whose value to get.</param>
	/// <returns type="Object">The value of the associated key or a null reference
	/// if the key does not exist.</returns>
	function Get(objKey)
	{
		return (typeof(_storage[objKey]) != 'undefined') ? _storage[objKey] : null;
	}
	this.Get = Get;


	/// <method>
	/// Clears the Hashtable. This will remove the stored items and its values.
	/// </method>
	function Clear()
	{
		_storage = CreateEmptyObject();
	}
	this.Clear = Clear;


	/// <method>
	/// Returns true if this hashtable contains the given key.
	/// </method>
	/// <param name="objKeyToCheck" type="Object">Key to search.</param>
	/// <returns type="Boolean">Returns true if this hashtable contains the given key.</returns>
	function Contains(objKeyToCheck)
	{
		return _this.ContainsKey(objKeyToCheck);
	}
	this.Contains = Contains;


	/// <method>
	/// Returns true if this hashtable contains the given key.
	/// </method>
	/// <param name="objKeyToCheck" type="Object">Key to search.</param>
	/// <returns type="Boolean">Returns true if this hashtable contains the given key.</returns>
	function ContainsKey(objKeyToCheck)
	{
		return (typeof(_storage[objKeyToCheck]) != 'undefined');
	}
	this.ContainsKey = ContainsKey;


	/// <method>
	/// Returns true if this hashtable contains the given value.
	/// </method>
	/// <param name="objValueToCheck" type="Object">Value to search.</param>
	/// <returns type="Boolean">Returns true if this hashtable contains the given value.</returns>
	function ContainsValue(objValueToCheck)
	{
		for (var item in _storage)
		{
			if (_storage[item] == objValueToCheck)
				return true;
		}
		return false;
	}
	this.ContainsValue = ContainsValue;


	/// <method>
	/// Removes the key/value pair of the associated key.
	/// </method>
	/// <param name="objKey" type="Object">Value to remove.</param>
	/// <returns type="Boolean">Returns true if this hashtable contains the given value.</returns>
	function Remove(objKey)
	{
		if (_this.ContainsKey(objKey))
		{
			var storedValue = _storage[objKey];
			_storage[objKey] = null;
			delete _storage[objKey];
			return storedValue;
		}
		return null;
	}
	this.Remove = Remove;


	/// <method>
	/// Creates a new object instance, which has no items stored.
	/// </method>
	function CreateEmptyObject()
	{
		var emptyObject = new Object();
		
		for (var item in emptyObject)
		{
			emptyObject[item] = null;
			delete emptyObject[item];
		}
		return emptyObject;
	}
	Init();
}

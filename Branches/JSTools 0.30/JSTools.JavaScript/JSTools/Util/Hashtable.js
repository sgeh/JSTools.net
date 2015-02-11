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

	var _this		= this;
	var _storage	= null;


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
	this.Count = function()
	{
		var count = 0;

		for (var item in _storage)
		{
			++count;
		}
		return count;
	}


	/// <method>
	/// Creates a new array which contains the stored keys.
	/// </method>
	/// <returns type="Array">Returns all keys stored in this hashtable.</param>
	this.GetKeys = function()
	{
		var keys = new Array(_this.Count());
		var i = 0;

		for (var item in _storage)
		{
			keys[i++] = item;
		}
		return keys;
	}


	/// <method>
	/// Creates a new array which contains the stored values.
	/// </method>
	/// <returns type="Array">Returns all values stored in this hashtable.</param>
	this.GetValues = function()
	{
		var values = new Array(_this.Count());
		var i = 0;

		for (var item in _storage)
		{
			values[i++] = _storage[item];
		}
		return values;
	}


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
	this.GetElements = function()
	{
		return _storage;
	}


	/// <method>
	/// Adds the given key/value pair into this hashtable. This function will perform
	/// no operations if an array enty with the given name already exists.
	/// </method>
	/// <param name="objKey" type="Object">The key whose value to add.</param>
	/// <param name="objValue" type="Object">The value to store.</param>
	this.Add = function(objKey, objValue)
	{
		if (!_this.ContainsKey(objKey))
		{
			_storage[objKey] = objValue;
		}
	}


	/// <method>
	/// Assigns the given value to the given key. If the key does not
	/// exist the given pair is added to the hashtable.
	/// </method>
	/// <param name="objKey" type="Object">The key whose value to set.</param>
	/// <param name="objValue" type="Object">The value to store.</param>
	this.Set = function(objKey, objValue)
	{
		_storage[objKey] = objValue;
	}


	/// <method>
	/// Gets the value associated with the given key.
	/// </method>
	/// <param name="objKey" type="Object">The key whose value to get.</param>
	/// <returns type="Object">The value of the associated key or a null reference
	/// if the key does not exist.</returns>
	this.Get = function(objKey)
	{
		return (typeof(_storage[objKey]) != 'undefined') ? _storage[objKey] : null;
	}


	/// <method>
	/// Clears the Hashtable. This will remove the stored items and its values.
	/// </method>
	this.Clear = function()
	{
		_storage = CreateEmptyObject();
	}


	/// <method>
	/// Returns true if this hashtable contains the given key.
	/// </method>
	/// <param name="objKeyToCheck" type="Object">Key to search.</param>
	/// <returns type="Boolean">Returns true if this hashtable contains the given key.</returns>
	this.Contains = function(objKeyToCheck)
	{
		return _this.ContainsKey(objKeyToCheck);
	}


	/// <method>
	/// Returns true if this hashtable contains the given key.
	/// </method>
	/// <param name="objKeyToCheck" type="Object">Key to search.</param>
	/// <returns type="Boolean">Returns true if this hashtable contains the given key.</returns>
	this.ContainsKey = function(objKeyToCheck)
	{
		return (typeof(_storage[objKeyToCheck]) != 'undefined');
	}


	/// <method>
	/// Returns true if this hashtable contains the given value.
	/// </method>
	/// <param name="objValueToCheck" type="Object">Value to search.</param>
	/// <returns type="Boolean">Returns true if this hashtable contains the given value.</returns>
	this.ContainsValue = function(objValueToCheck)
	{
		for (var item in _storage)
		{
			if (_storage[item] == objValueToCheck)
				return true;
		}
		return false;
	}


	/// <method>
	/// Removes the key/value pair of the associated key.
	/// </method>
	/// <param name="objKey" type="Object">Value to remove.</param>
	/// <returns type="Boolean">Returns true if this hashtable contains the given value.</returns>
	this.Remove = function(objKey)
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

namespace("JSTools.Util");


/// <class>
/// Represents an object serializer which serializes the property values and
/// the corresponding property names. Properties which contain a function or
/// a recursion will not be serialized.
/// </class>
JSTools.Util.SimpleObjectSerializer = function()
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	var PUBLIC_HIDDEN_FIELD_ID = "__";
	var _this = this;


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------

	/// <constructor>
	/// Creates a new JSTools.Util.SimpleObjectSerializer instance.
	/// </constructor>
	function Init()
	{
	}


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Serializes the given object. If the given objToSerialize is not an
	/// object, the objToSerialize is converted to a string and written into
	/// the 'value' property of the resulting string.
	/// Public hidden fields which start with "__" will not be serialized.
	/// </method>
	/// <param type="Object" name="objToSerialize">Object which should be serialized.</param>
	/// <returns type="String">Returns the serialized string.</returns>
	function SerializeObject(objToSerialize)
	{
		if (typeof(objToSerialize) != 'object')
			return "{value:" + SerializeString(objToSerialize)  + "}";
		else
			return Serialize(objToSerialize, [ ] );
	}
	this.SerializeObject = SerializeObject;


	/// <method>
	/// Deserializes the given string.
	/// </method>
	/// <param type="String" name="strToSerialize">String which should be deserialized.</param>
	/// <param type="Object" name="objContainer">Object which should contain the deserialized values. If this value does not contain a valid object, a new object is created.</param>
	/// <returns type="String">Returns the deserialized object.</returns>
	function Deserialize(strToSerialize, objContainer)
	{
		// deserialize object
		var rawObj = null;
		eval("rawObj=" + strToSerialize);

		// deserialize its values
		return DeserializeObject(objContainer, rawObj);
	}
	this.Deserialize = Deserialize;


	function DeserializeObject(objContainer, objToDeserialize)
	{
		if (typeof(objContainer) != 'object')
			objContainer = new Object();

		// fill given object
		for (var item in objToDeserialize)
		{
			objContainer[item] = DeserializeValue(objContainer[item], objToDeserialize[item]);
		}
		return objContainer;
	}


	function DeserializeValue(objContainer, objValueToDeserialize)
	{
		if (typeof(objValueToDeserialize) == 'string')
			return unescape(objValueToDeserialize);
		
		if (IsArray(objValueToDeserialize))
			return DeserializeArray(objContainer, objValueToDeserialize);

		if (typeof(objValueToDeserialize) == 'object')
			return DeserializeObject(objContainer, objValueToDeserialize);

		return objValueToDeserialize;
	}

	function DeserializeArray(objContainer, objToDeserialize)
	{
		if (typeof(objContainer) != 'object' || !objContainer)
			objContainer = [ ];

		// clear current container
		for (var i = 0; i < objContainer.length; ++i)
		{
			objContainer[i] = null;
			delete objContainer[i];
		}

		// fill given object
		for (var i = 0; i < objToDeserialize.length; ++i)
		{
			objContainer[i] = DeserializeValue(objContainer[i], objToDeserialize[i]); 
		}
		return objContainer;
	}


	function Serialize(objToSerialize, arrSerializedObjects)
	{
		arrSerializedObjects.Add(objToSerialize);
		var resultString = "";

		for (var item in objToSerialize)
		{
			if (CanSerialize(objToSerialize[item], arrSerializedObjects, item))
			{
				resultString += item;
				resultString += ":";
				resultString += SerializeValue(objToSerialize[item], arrSerializedObjects);
				resultString += ",";
			}
		}
		
		if (resultString)
			return "{" + resultString.substring(0, resultString.length - 1) + "}";
		else 
			return "{}";
	}


	function SerializeValue(objPropValue, arrSerializedObjects)
	{
		if (typeof(objPropValue) == 'object')
			return SerializeInstance(objPropValue, arrSerializedObjects.Copy());

		if (typeof(objPropValue) == 'number')
			return objPropValue.toString();

		if (typeof(objPropValue) == 'boolean')
			return (objPropValue) ? "true" : "false";
		
		return SerializeString(objPropValue);
	}


	function SerializeInstance(objPropValue, arrSerializedObjects)
	{
		if (objPropValue == null)
			return "null";
		else if (IsArray(objPropValue))
			return SerializeArray(objPropValue, arrSerializedObjects);
		else
			return Serialize(objPropValue, arrSerializedObjects);
	}


	function SerializeArray(objPropValue, arrSerializedObjects)
	{
		var resultValue = "";

		for (var i = 0; i < objPropValue.length; ++i)
		{
			if (CanSerialize(objPropValue[i], arrSerializedObjects))
			{
				resultValue += SerializeValue(objPropValue[i], arrSerializedObjects);
				resultValue += ",";
			}
		}

		if (resultValue)
			return "[" + resultValue.substring(0, resultValue.length - 1) + "]";
		else 
			return "[]";
	}


	function SerializeString(strToDeserialize)
	{
		// do only escape characters which cause problems
		var escapedString = String(strToDeserialize);
		escapedString = escapedString.replace(/\n/g, "%0A");
		escapedString = escapedString.replace(/\r/g, "%0D");
		escapedString = escapedString.replace(/'/g, "%27");
		escapedString = escapedString.replace(/</g, "%3C");
		escapedString = escapedString.replace(/>/g, "%3E");
		return "'" + escapedString + "'";
	}


	function CanSerialize(objToSerialize, arrSerializedObjects, strItemName)
	{
		// do not serialize functions
		if (typeof(objToSerialize) == 'function')
			return false;
		
		// do not serialize hidden public fields
		if (strItemName && String(strItemName).StartsWith(PUBLIC_HIDDEN_FIELD_ID))
			return false;
		
		// avoid endless recursion
		if (typeof(objToSerialize) == 'object' && arrSerializedObjects.Contains(objToSerialize))
			return false;
			
		return (typeof(objToSerialize) != 'undefined');
	}


	function IsArray(objToCheck)
	{
		return (objToCheck && typeof(objToCheck) == 'object' && typeof(objToCheck.length) != 'undefined');
	}
	Init();
}

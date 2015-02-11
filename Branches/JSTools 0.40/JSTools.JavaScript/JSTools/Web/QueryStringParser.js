namespace("JSTools.Web");


/// <class>
/// Parses the given query string and unescapes the name-value pairs.
/// </class>
/// <param name="strQueryStringData" type="String">QueryString to initialize
/// (e.g. http://www.jstools.net or http://www.jstools.net/default.aspx?values=23).</param>
/// <param name="blnEscapeFileUrl" type="Boolean">A boolean indicating whether the url
/// should be unescaped when deserializing and should be escaped when serializing.</param>
JSTools.Web.QueryStringParser = function(strQueryStringData, blnEscapeFileUrl)
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	this.InitType(arguments, "JSTools.Web.QueryStringParser");

	var DEFAULT_VALUE_SEPARATOR = "?";
	var VALUE_SEPARATOR = "=";
	var NAME_VALUE_SEPARATOR = "&";
	var PARSE_REGEXP = /^(([^?]+)\?)?(.+)$/;

	var _this = this;
	var _rawData = (strQueryStringData) ? String(strQueryStringData) : String.Empty;
	var _fileUrl = String.Empty;
	var _escapeFileUrl = Boolean(blnEscapeFileUrl);
	var _storage = new JSTools.Util.Hashtable();


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------

	/// <constructor>
	/// Creates a new JSTools.Web.QueryStringParser instance.
	/// </constructor>
	function Init()
	{
		if (!_rawData)
			return;

		var defaultValueMatch = _rawData.match(PARSE_REGEXP);

		if (defaultValueMatch
			&& defaultValueMatch.length
			&& defaultValueMatch[2])
		{
			SetEscapedFileUrl(defaultValueMatch[2]);

			if (defaultValueMatch[3])
			{
				DeserializeString(defaultValueMatch[3]);
			}
		}
		else
		{
			SetEscapedFileUrl(_rawData);
		}
	}


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Gets a boolean indicating whether the url should be unescaped when deserializing
	/// and should be escaped when serializing.
	/// </method>
	/// <returns type="Boolean">Returns true if the url escaping mechanism is enabled.</returns>
	function IsFileUrlEscaped()
	{
		return _escapeFileUrl;
	}
	this.IsFileUrlEscaped = IsFileUrlEscaped;


	/// <method>
	/// Gets the file url. 
	/// </method>
	/// <returns type="String">Returns the file url path of the given query string.</returns>
	function GetFileUrl()
	{
		return _fileUrl;
	}
	this.GetFileUrl = GetFileUrl;


	/// <method>
	/// Sets the file url.
	/// </method>
	/// <param name="strFileUrl" type="String">New file url path.</param>
	function SetFileUrl(strFileUrl)
	{
		_fileUrl = String(strFileUrl);
	}
	this.SetFileUrl = SetFileUrl;


	/// <method>
	/// Adds the given value and the given name. If the specified name already exists, the value
	/// of the given name will be overriden.
	/// </method>
	/// <param name="strValueName" type="String">Name of the value to add.</param>
	/// <param name="strValue" type="String">Value to add.</param>
	function SetValue(strValueName, strValue)
	{
		_storage.Set(String(strValueName), String(strValue));
	}
	this.SetValue = SetValue;


	/// <method>
	/// Returns the value with the given name.
	/// </method>
	/// <param name="strValueName" type="String">Name of the value to get.</param>
	/// <returns type="String">Returns the expected value or a null reference if there
	/// is no value associated with the given name.</returns>
	function GetValue(strValueName)
	{
		return (typeof(_storage.Get(strValueName)) != 'undefined') ? _storage[strValueName] : null;
	}
	this.GetValue = GetValue;


	/// <method>
	/// Removes the value associated with the given name.
	/// </method>
	/// <param name="strValueName" type="String">Name of the value to remove.</param>
	/// <returns type="String">Returns the removed value or a null reference, if the given
	/// name could not be found.</returns>
	function RemoveValue(strValueName)
	{
		var valueToRemove = _storage.Get(strValueName);

		if (typeof(valueToRemove) != 'undefined')
		{
			_storage.Remove(strValueName);
			return valueToRemove;
		}
		return null;
	}
	this.RemoveValue = RemoveValue;


	/// <method>
	/// Returns a copy of all registered value names.
	/// </method>
	/// <returns type="Array"></returns>
	function GetValueNames()
	{
		return _storage.GetKeys();
	}
	this.GetValueNames = GetValueNames;


	/// <method>
	/// Creates a new QueryStringParser instance and copies the values of this object
	/// into the new QueryStringParser object.
	/// </method>
	/// <returns type="JSTools.Web.QueryStringParser">Returns a clone of this object.</returns>
	function Clone()
	{
		return new JSTools.Web.QueryStringParser(_this.toString(), _escapeFileUrl);
	}
	this.Clone = Clone;


	/// <method>
	/// Serializes the values of this object into a string. To deserialize this
	/// string, you have to create a new QueryStringParser instance and pass the
	/// string representation of this object to the first constructor argument.
	/// </method>
	function ToString()
	{
		var serializedString = GetEscapedFileUrl();
		var count = 0;

		for (var item in _storage.GetElements())
		{
			if (count != 0)
			{
				serializedString += NAME_VALUE_SEPARATOR;
			}
			else if (_storage.Count() > 0)
			{
				serializedString += DEFAULT_VALUE_SEPARATOR;
			}

			serializedString += escape(item);
			serializedString += VALUE_SEPARATOR;
			serializedString += escape(_storage.Get(item));
			++count;
		}
		return serializedString;
	}
	this.toString = ToString;


	/// <method>
	/// Deserializes the given string and fills the values into this object.
	/// </method>
	/// <param name="strToDeserialize" type="String">String which should be deserialized.</param>
	function DeserializeString(strToDeserialize)
	{
		var keyValuePairs = strToDeserialize.split(NAME_VALUE_SEPARATOR);

		for (var i = 0; i < keyValuePairs.length; ++i)
		{
			var pair = keyValuePairs[i].split(VALUE_SEPARATOR);
			var pairValue = (pair.length > 0) ? pair[1] : String.Empty;

			_this.SetValue(unescape(pair[0]), unescape(pairValue));
		}
	}


	/// <method>
	/// Initializes the given file url escaped if the escape boolean was set to false.
	/// </method>
	/// <param name="strFileUrl" type="String">File url to initialize.</param>
	function SetEscapedFileUrl(strFileUrl)
	{
		_fileUrl = (_escapeFileUrl) ? unescape(String(strFileUrl)) : String(strFileUrl);
	}


	/// <method>
	/// Returns the file url escaped if the escape boolean was set to false.
	/// </method>
	/// <returns type="String">Gets the file in the right format.</returns>
	function GetEscapedFileUrl()
	{
		return (_escapeFileUrl) ? escape(_fileUrl) : _fileUrl;
	}
	Init();
}
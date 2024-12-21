namespace("JSTools.Util");


/// <class>
/// Represents an unique class id, similar to the globally unique identifier (GUID).
/// </class>
/// <param name="strRawGuid" type="String">String, which contains the inner value of the guid.
/// This value must have the length of a guid and can contains only 0-9 and a-f characters.</param>
JSTools.Util.Guid = function(strRawGuid)
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	var GUID_LENGTH			= 32;
	var GUID_SEPARATOR		= "-";
	var GUID_BLOCK_SIZE		= 4;
	var INPUT_CHECK_REGEXP	= new RegExp("^[0-9a-f]{" + GUID_LENGTH + "," + GUID_LENGTH + "}$", "i");
	var MAX_BLOCK_NUMBER	= 0xFFFF;

	var DEFAULT_PATTERN		= "{D}";
	var NORMAL_PATTERN		= "{N}";
	var PATTERN_CATALOG		= [
								{ Letter: "N", Pattern: "{0}", SeparatorMask: 0x00 },
								{ Letter: "D", Pattern: "{0}", SeparatorMask: 0x1E },
								{ Letter: "B", Pattern: "({0})", SeparatorMask: 0x1E },
								{ Letter: "P", Pattern: "{{0}}", SeparatorMask: 0x1E }
							  ];

	var _this				= this;
	var _guidBlocks			= Math.ceil(GUID_LENGTH / GUID_BLOCK_SIZE);
	var _intGuid			= new Array(_guidBlocks);


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------

	/// <constructor>
	/// Creates a new JSTools.Util.Guid instance.
	/// </constructor>
	function Init()
	{
		if (typeof(strRawGuid) != 'string' || !strRawGuid.match(INPUT_CHECK_REGEXP))
		{
			for (var i = 0; i < _guidBlocks - 1; ++i)
			{
				// create random guid block (max 65535)
				_intGuid[i] = GetHexPattern(Math.random() * MAX_BLOCK_NUMBER);
			}
			_intGuid[_intGuid.length - 1] = GetHexPattern(++JSTools.Util.Guid.__count);
		}
		else
		{
			for (var i = 0; i < _guidBlocks; ++i)
			{
				var guidBlockValue = strRawGuid.substring(i * GUID_BLOCK_SIZE, (i * GUID_BLOCK_SIZE) + GUID_BLOCK_SIZE);
				_intGuid[i] = guidBlockValue.toLowerCase();
			}
		}
	}


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Creates a new guid string and converts it into a number.
	/// </method>
	/// <returns type="Integer">Returns the guid as a number (integer).</returns>
	this.valueOf = function()
	{
		return this.toString(NORMAL_PATTERN);
	}


	/// <method>
	/// Returns a valid guid string. If the given pattern was not found, the
	/// default pattern ( {D} ) is used.
	///
	/// Allowd patterns:
	/// {N} - 32 digits
	///       xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
	/// {D} - 32 digits separated by hyphens
	///       xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
	/// {B} - 32 digits separated by hyphens, enclosed in brackets
	///       {xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}
	/// {P} - 32 digits separated by hyphens, enclosed in parentheses
	///       (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)
	/// </method>
	/// <param name="strFormat" type="String">Object to register.</param>
	/// <returns type="String">Returns the protected member array.</returns>
	this.toString = function(strFormat)
	{
		var generatedGuid = "";

		// search for specified pattern
		if (typeof(strFormat) == 'string' && strFormat.search(/^\{(\w)\}$/) != -1)
		{
			generatedGuid = PerformSearchResult(RegExp.$1);
		}
		
		// invalid pattern specified, we have to return the default pattern
		if (!generatedGuid)
		{
			generatedGuid = this.toString(DEFAULT_PATTERN);
		}
		return generatedGuid;
	}


	/// <method>
	/// Initializes the given search result and returns a guid pattern.
	/// </method>
	/// <param name="strFormat" type="String">Search result, which contians the pattern.</param>
	/// <returns type="String">Returns a GUID string.</returns>
	function PerformSearchResult(strSearchResult)
	{
		for (var i = 0; i < PATTERN_CATALOG.length; ++i)
		{
			if (!PATTERN_CATALOG[i])
				continue;

			if (strSearchResult == PATTERN_CATALOG[i].Letter)
			{
				return CreateDigitString(PATTERN_CATALOG[i].SeparatorMask, PATTERN_CATALOG[i].Pattern);
			}
		}
		return String.Emtpy;
	}


	/// <method>
	/// Creates a the digit string with the given block.
	/// </method>
	/// <param name="intFlags" type="Integer">Bits mask of separators to set.</param>
	/// <param name="strFormatString" type="String">Format string, {0} will be replaced with the guid.</param>
	/// <returns type="String">Returns the created digit string.</returns>
	function CreateDigitString(intFlags, strFormatString)
	{
		var output = String.Empty;

		for (var i = 0; i < _guidBlocks; ++i)
		{
			output += _intGuid[i];

			if ((intFlags & (0x01 << i)) != 0)
			{
				output += GUID_SEPARATOR;
			}
		}
		return String.Format(strFormatString, output).toLowerCase();
	}
	

	/// <method>
	/// Converts the given number into a hex-string pattern.
	/// </method>
	/// <param name="intToConvert" type="Integer">Number to convert.</param>
	/// <returns type="String">Returns the converted string.</returns>
	function GetHexPattern(intToConvert)
	{
		return JSTools.Convert.DecToHex(intToConvert % MAX_BLOCK_NUMBER).PadLeft(GUID_BLOCK_SIZE, "0");
	}
	Init();
}


/// <property type="Integer">
/// Describes the count of guid instances. Do not change this value from
/// your code.
/// </property>
JSTools.Util.Guid.__count = 0;

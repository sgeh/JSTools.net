namespace("JSTools.Util");


/// <class>
/// Provides convert functionalities, e.g. DecToHex or HexToDex.
/// </class>
JSTools.Util.StringConverter = function()
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	var MAX_HEX_CONVERT		= 2147483647;
	var HEX_FIGURES			= "0123456789ABCDEF";

	var _this = this;


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Converts the given int into a hex number. If the given param is not a valid number,
	/// you will obtain an empty string. This method will not convert the letters after the ".".
	/// </method>
	/// <param name="intToConvert" type="Integer">Integer which should be converted.</param>
	/// <returns type="String">Returns a string, which contains the hex number.</returns>
	this.DecToHex = function(intToConvert)
	{
		if (isNaN(intToConvert))
			return String.Empty;

		if (intToConvert > MAX_HEX_CONVERT)
			return Number.NaN;

		var outPutString = "";
		var inputNumber = Math.abs(Number(intToConvert));

		for (var toConvert = Math.ceil(inputNumber); toConvert > 0; )
		{
			outPutString = HEX_FIGURES.charAt(toConvert % 16) + outPutString;
			toConvert >>= 4;
		}
		return outPutString;
	}


	/// <method>
	/// Converts the given hex number string into a decimal number. If the param does
	/// not contain a valid hex string, you will obtain a Number.NaN value.
	/// </method>
	/// <param name="strToConvert" type="Integer">String which should be converted.</param>
	/// <returns type="Integer">Returns the converted string.</returns>
	this.HexToDec = function(strToConvert)
	{
		if (typeof(strToConvert) != 'string')
			return Number.NaN;

		return parseInt(strToConvert.toUpperCase(), 16);
	}
}


/// <property type="JSTools.Util.StringConverter">
/// Default StringConverter instance.
/// </property>
JSTools.Convert = new JSTools.Util.StringConverter();

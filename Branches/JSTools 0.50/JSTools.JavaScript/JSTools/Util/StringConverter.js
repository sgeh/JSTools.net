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
/// Provides convert functionalities, e.g. DecToHex or HexToDex.
/// </class>
JSTools.Util.StringConverter = function()
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	var MAX_HEX_CONVERT = 2147483647;
	var HEX_FIGURES = "0123456789ABCDEF";

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
	function DecToHex(intToConvert)
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
	this.DecToHex = DecToHex;


	/// <method>
	/// Converts the given hex number string into a decimal number. If the param does
	/// not contain a valid hex string, you will obtain a Number.NaN value.
	/// </method>
	/// <param name="strToConvert" type="Integer">String which should be converted.</param>
	/// <returns type="Integer">Returns the converted string.</returns>
	function HexToDec(strToConvert)
	{
		if (typeof(strToConvert) != 'string')
			return Number.NaN;

		return parseInt(strToConvert.toUpperCase(), 16);
	}
	this.HexToDec = HexToDec;
}


/// <property type="JSTools.Util.StringConverter">
/// Default StringConverter instance.
/// </property>
JSTools.Convert = new JSTools.Util.StringConverter();

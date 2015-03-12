/*
 * JSTools.JavaScript.Test / JSTools.net - A JavaScript/C# framework.
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

document.write("<h1>TEST STRINGCONVERTER</h1>");

var toConvert = 1398567236;

if (JSTools.Convert.HexToDec(JSTools.Convert.DecToHex(toConvert)) != toConvert)
{
	document.write("<p>hex/dec convert failed<br>DEC: " + toConvert + "<br>HEX: " + JSTools.Convert.DecToHex(toConvert) + "<br>Converted HEX: " + JSTools.Convert.HexToDec(JSTools.Convert.DecToHex(toConvert)) + "</p>");
}
else
{
	document.write("<p>hex/dec convert successfull<br>DEC: " + toConvert + "<br>HEX: " + JSTools.Convert.DecToHex(toConvert) + "<br>Converted HEX: " + JSTools.Convert.HexToDec(JSTools.Convert.DecToHex(toConvert)) + "</p>");
}
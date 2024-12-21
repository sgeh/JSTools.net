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
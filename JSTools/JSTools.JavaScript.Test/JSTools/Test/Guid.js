document.write("<h1>TEST GUID</h1>");
document.write("<p>");

for (var i = 0; i < 100; ++i)
{
	var myGuid1 = new JSTools.Util.Guid();

	if (new JSTools.Util.Guid(myGuid1.valueOf()).valueOf() != myGuid1.valueOf())
	{
		document.write("<p>error in guid " + myGuid1 + " -> " + new JSTools.Util.Guid(myGuid1.valueOf()) + "</p>");
	}
	else
	{
		document.write("guid " + myGuid1 + " okay<br>");
	}
}

document.write("</p>");
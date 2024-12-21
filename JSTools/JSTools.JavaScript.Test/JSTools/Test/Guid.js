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
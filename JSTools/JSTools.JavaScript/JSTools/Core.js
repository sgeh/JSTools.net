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

//------------------------------------------------------------------------
// NameSpace directive
//------------------------------------------------------------------------

/// <procedure>
/// Represents the namespace directive. Initilizes the specified namespace, if it
/// was not given jet.
/// </procedure>
/// <param name="strNameSpace" type="String">NameSpace to create, separated by '.'.</param>
function namespace(strNameSpace)
{
	if (!namespace.Manager.IsNameSpaceLoaded(strNameSpace))
		namespace.Manager.Add(strNameSpace);
}


/// <property type="NameSpaceManager">
/// Default NameSpaceManager instance.
/// </property>
namespace.Manager = new NameSpaceManager(this, this.top);


// init default namespace
namespace("JSTools");


//------------------------------------------------------------------------
// Using directive
//------------------------------------------------------------------------

/// <procedure>
/// Loads the given NameSpace. The given NameSpace will be separated
/// by the "." character. This characters will be replaced with a
/// slash "/" and a request will be send to the server. If you specify
/// a NameSpace like "JSTools.Web.Data", the file "/JSTools/Web/Data.js"
/// will be used.
///
/// The given NameSpace can contain browser identification patterns.
/// They will be replaced as follows:
///
/// {I} Browser Identification
///  IE - Internet Explorer
///  NS - Netscape Navigator
///  OP - Opera
///
/// {G} Browser Generation
///  4 - 4. Browser Generation
///   Netscape 4.x
///   Internet Explorer 4.0
///   Opera 5.x / 6.x
///  5 - 5. Browser Generation
///   Netscape 6.x +
///   Internet Explorer 5.x +
///   Opera 7.x
/// </procedure>
/// <remarks>
/// Do not use the using directive inside code files, which are included with
/// a document.write() call. Netscape 4.x may crash, if you do so. You should
/// use the using directive inside an html file as direct call only.
/// </remarks>
function using(strNameSpaceToLoad)
{
	var nameSpaceToLoad = String(strNameSpaceToLoad);

	if (namespace.Manager.IsNameSpaceLoaded(nameSpaceToLoad))
		return;

	nameSpaceToLoad = nameSpaceToLoad.replace(new RegExp("\\{G\\}", "gi"), JSTools.Browser.GetMajor());
	nameSpaceToLoad = nameSpaceToLoad.replace(new RegExp("\\{I\\}", "gi"), JSTools.Browser.GetShortName());

	JSTools.ScriptLoader.LoadFile(strNameSpaceToLoad.replace(/\./g, "/"));
}

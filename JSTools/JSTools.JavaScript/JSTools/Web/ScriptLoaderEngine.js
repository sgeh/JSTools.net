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

namespace("JSTools.Web");


/// <class>
/// Loads a file script at html document design-time. Represents the using directive.
/// </class>
JSTools.Web.ScriptLoaderEngine = function()
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	this.InitType(arguments, "JSTools.Web.ScriptLoaderEngine");

	var SCRIPT_TAG_NAME = "script";
	var LANGUAGE_ATTRIB = "language";
	var TYPE_ATTRIB = "type";
	var SOURCE_ATTRIB = "src";

	var _this = this;


	/// <property type="String">
	/// Sepcifies the script type (javascript / vbscript)
	/// </property>
	this.ScriptLanguage = "JavaScript";


	/// <property type="String">
	/// Web location of the script files. The param given by the LoadFile() method will
	/// be replaced with the {0} mark.
	/// </property>
	this.ScriptFileLocation = String.Empty;


	/// <property type="Boolean">
	/// Gets/sets if the requested file location replacement should be encoded. This
	/// may be useful, if the replacement mark {0} is placed in a query string request.
	/// </property>
	this.EncodeFileLocation = false;


	/// <property type="Number">
	/// Represents the script version, which should be used to import the script.
	/// </property>
	this.ScriptVersion = 1.3;


	/// <property type="Number">
	/// Represents the script version, which should be used to import the script (e.g. ".js").
	/// </property>
	this.ScriptExtension = ".js";


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Initializes the string and creates a new java script tag which will
	/// be written into the document.
	/// </method>
	/// <param name="strFileLocation" type="String">The script file source (e.g. "/JSTools/Web.js").</param>
	function LoadFile(strFileLocation)
	{
		if (typeof(strFileLocation) != 'string' || strFileLocation == String.Empty)
			return;

		if (!strFileLocation.EndsWith(_this.ScriptExtension))
		{
			strFileLocation += _this.ScriptExtension;
		}

		if (_this.EncodeFileLocation)
		{
			strFileLocation = escape(strFileLocation);
		}

		var filePattern = String(_this.ScriptFileLocation);
		
		if (filePattern.indexOf("{0}") != -1)
		{
			WriteScriptTag(String.Format(filePattern, strFileLocation));
		}
		else
		{
			WriteScriptTag(strFileLocation);
		}
	}
	this.LoadFile = LoadFile;
	
	
	/// <method>
	/// Writes a new &lt;script&gt; tag with the specified location.
	/// </method>
	/// <param name="strLocation" type="String">Location to write.</param>
	function WriteScriptTag(strLocation)
	{
		var scriptVersion = (_this.ScriptVersion) ? _this.ScriptVersion : String.Empty;
		var scriptElement = new JSTools.Web.Element(SCRIPT_TAG_NAME);

		scriptElement.RenderEndTag = true;
		scriptElement.GetAttributes().Add(LANGUAGE_ATTRIB, _this.ScriptLanguage + scriptVersion);
		scriptElement.GetAttributes().Add(TYPE_ATTRIB, "text/" + String(_this.ScriptLanguage).toLowerCase());
		scriptElement.GetAttributes().Add(SOURCE_ATTRIB, strLocation);
		window.document.write(scriptElement.Render());
	}
}


/// <property type="JSTools.Web.ScriptLoaderEngine">
/// Default ScriptLoader instance.
/// </property>
JSTools.ScriptLoader = new JSTools.Web.ScriptLoaderEngine();

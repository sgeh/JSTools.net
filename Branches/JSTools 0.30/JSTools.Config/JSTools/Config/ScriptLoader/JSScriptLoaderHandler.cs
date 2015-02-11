/*
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
 */

/// <file>
///     <copyright see="prj:///doc/copyright.txt"/>
///     <license see="prj:///doc/license.txt"/>
///     <owner name="Silvan Gehrig" email="silvan.gehrig@mcdark.ch"/>
///     <version value="$version"/>
///     <since>JSTools.dll 0.1.0</since>
/// </file>

using System;
using System.Configuration;
using System.Text;
using System.Xml;

using JSTools.Config.ScriptFileManagement;
using JSTools.Xml;

namespace JSTools.Config.ScriptLoader
{
	/// <summary>
	/// Represents an &lt;scriptFileLoader&gt; configuration node instance.
	/// </summary>
	public class JSScriptLoaderHandler : AJSToolsSection
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		public	readonly	string							SECTION_NAME;

		private	const		string							REQUIRES_ATTIRB			= "requires";
		private	const		string							LOCATION_ATTIRB			= "scriptFileLocation";
		private	const		string							INSERT_PREFIX_ATTIRB	= "insertAppPrefix";
		private	const		string							ENCODE_LOCATION_ATTIRB	= "encodeFileLocation";

		private				string							_requiredModule			= "";
		private				string							_location				= "";
		private				bool							_insertLocationPrefix	= false;
		private				bool							_encodeLocation			= false;


		/// <summary>
		/// Returns the name of the representing element.
		/// </summary>
		public string SectionName
		{
			get { return SECTION_NAME; }
		}


		/// <summary>
		/// Returns the location, where the scripts are stored.
		/// </summary>
		public string ScriptFileLocation
		{
			get { return _location; }
		}


		/// <summary>
		/// Returns, if the current application path should be inserted as location prefix.
		/// </summary>
		public bool InsertLocationPrefix
		{
			get { return _insertLocationPrefix; }
		}


		/// <summary>
		/// Returns, if the inserted location ({0} pattern) should be encoded.
		/// </summary>
		public bool EncodeFileLocation
		{
			get { return _encodeLocation; }
		}


		/// <summary>
		/// Gets the name of the required module.
		/// </summary>
		public string RequiredModule
		{
			get { return _requiredModule; }
		}


		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Initializes a new JSScriptLoaderHandler instance.
		/// </summary>
		/// <param name="scriptLoaderNode">XmlNode, which contains the configuration data.</param>
		/// <param name="ownerConfig">Owner (parent) configuration instance.</param>
		/// <param name="nodeName">Contains the name of the representing node.</param>
		/// <exception cref="ArgumentNullException">An argument contains a null reference.</exception>
		public JSScriptLoaderHandler(XmlNode scriptLoaderNode, IJSToolsConfiguration ownerConfig, string nodeName) : base(ownerConfig)
		{
			if (scriptLoaderNode == null)
				throw new ArgumentNullException("scriptLoaderNode", "The given xml section contains a null reference!");

			if (nodeName == null)
				throw new ArgumentNullException("nodeName", "The given node name contains a null reference!");

			SECTION_NAME			= nodeName;
			_requiredModule			= JSToolsXmlFunctions.GetAttributeFromNode(scriptLoaderNode, REQUIRES_ATTIRB);
			_location				= JSToolsXmlFunctions.GetAttributeFromNode(scriptLoaderNode, LOCATION_ATTIRB);
			_insertLocationPrefix	= JSToolsXmlFunctions.GetBoolFromNodeValue(scriptLoaderNode.Attributes[INSERT_PREFIX_ATTIRB]);
			_encodeLocation			= JSToolsXmlFunctions.GetBoolFromNodeValue(scriptLoaderNode.Attributes[ENCODE_LOCATION_ATTIRB]);
		}


		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Checks the relations between the configuration sections. The script section is initilialized
		/// before calling this method.
		/// </summary>
		/// <exception cref="InvalidOperationException">Could not find a module with the required module.</exception>
		/// <exception cref="ConfigurationException">The script file handling section was not initialized.</exception>
		public override void CheckRelations()
		{
			if (OwnerConfiguration.ScriptFileHandler == null)
				throw new ConfigurationException("The script file handling section was not initialized!");

			if (OwnerConfiguration.ScriptFileHandler.GetModuleByName(_requiredModule) == null)
				throw new InvalidOperationException("Could not find a module with the name '" + _requiredModule + "'!");
		}
	}
}

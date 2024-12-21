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

using System;
using System.Configuration;
using System.Xml;

namespace JSTools.Config.ScriptLoader
{
	/// <summary>
	/// Creates new JSScriptLoaderHandler instances, which represent the
	/// scriptFileLoader section of the configuration document.
	/// </summary>
	public class JSScriptLoaderHandlerFactory : AJSToolsConfigSectionHandlerFactory
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the name of the script file loader xml section.
		/// </summary>
		public const string SECTION_NAME = "scriptFileLoader";

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Name of the section node.
		/// </summary>
		public override string SectionName
		{
			get { return SECTION_NAME; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Initializes a new JSScriptLoaderHandlerFactory instance.
		/// </summary>
		public JSScriptLoaderHandlerFactory()
		{
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new configuration handler.
		/// </summary>
		/// <param name="section">Configuration, which should be initialized.</param>
		/// <param name="ownerConfig">Owner (parent) configuration instance.</param>
		public override AJSToolsSection CreateInstance(XmlNode section, IJSToolsConfiguration ownerConfig)
		{
			return new JSScriptLoaderHandler(section, ownerConfig, SECTION_NAME);
		}
	}
}

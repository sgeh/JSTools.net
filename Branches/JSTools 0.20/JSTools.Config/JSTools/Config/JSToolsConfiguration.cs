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
using System.IO;
using System.Xml;

using JSTools.Config.ExceptionHandling;
using JSTools.Config.ScriptFileManagement;
using JSTools.Config.Session;

namespace JSTools.Config
{
	/// <summary>
	/// Contains all configuration capatibilities of the JSTools Framework for the asp.net
	/// environment. The application configuration instance is immutable, session and page specific
	/// instances are writeable.
	/// </summary>
	public class JSToolsConfiguration : AJSToolsConfiguration
	{
		//------------------------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------------------------



		//------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Initializes a new JavaScript configuration handler instance. This constructer does not need
		/// a XmlDocument instance, so if a JSTools.net section is specified in the web.config, this
		/// constructor will initilizes the specified configuration document.
		/// </summary>
		/// <param name="sessionHandler">Specifies the AJSToolsSessionHandler handler, which handles copy-on-write mechanism.</param>
		/// <exception cref="ArgumentNullException">The specified AJSToolsSessionHandler contains a null reference.</exception>
		public JSToolsConfiguration(AJSToolsSessionHandler sessionHandler) : base(sessionHandler)
		{
		}


		/// <summary>
		/// Initializes a new JavaScript configuration handler instance. This constructer does not need
		/// a XmlDocument instance, so if a JSTools.net section is specified in the web.config, this
		/// constructor will initilizes the specified configuration document.
		/// </summary>
		/// <param name="configFilePath">Path of the configuration file.</param>
		/// <param name="sessionHandler">Specifies the AJSToolsSessionHandler handler, which handles copy-on-write mechanism.</param>
		/// <exception cref="ArgumentNullException">The given configuration path or the session handler contains a null reference.</exception>
		/// <exception cref="ArgumentException">Could not load the given configuration file.</exception>
		/// <exception cref="ConfigurationException">Could not initialize a type specified in a configuration xml section.</exception>
		public JSToolsConfiguration(string configFilePath, AJSToolsSessionHandler sessionHandler) : base(configFilePath, sessionHandler)
		{
		}


		/// <summary>
		/// Initializes a new JavaScript configuration handler instance.
		/// </summary>
		/// <param name="configDocument">Loads the configuration from the specified xml document.</param>
		/// <param name="sessionHandler">Specifies the AJSToolsSessionHandler handler, which handles copy-on-write mechanism.</param>
		/// <exception cref="ArgumentNullException">The given XmlDocument or the session handler contains a null reference.</exception>
		/// <exception cref="ArgumentException">Could not load the given configuration file.</exception>
		/// <exception cref="ConfigurationException">Could not initialize a type specified in a configuration xml section.</exception>
		public JSToolsConfiguration(XmlDocument configDocument, AJSToolsSessionHandler sessionHandler) : base(configDocument, sessionHandler)
		{
		}


		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Creates a new instance of the given section handler. If you'd like to create a writeable
		/// handler instance, you have to override this method and call the CreateWriteableInstance method.
		/// </summary>
		/// <param name="sectionHandler">Section handler that creates the configuration handler instance.</param>
		/// <param name="section">Contains the configuration XmlNode.</param>
		/// <returns>Returns the created instance.</returns>
		protected override AJSToolsEventHandler CreateSectionInstance(AJSToolsConfigSectionHandlerFactory sectionHandler, XmlNode section)
		{
			return sectionHandler.CreateInstance(section);
		}
	}
}

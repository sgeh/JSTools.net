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
using System.Collections;
using System.IO;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Text;
using System.Xml;

using JSTools.Config.Session;

namespace JSTools.Config
{
	/// <summary>
	/// Derives from AJSToolsConfiguration class and overrides the CreateSectionInstance method
	/// to provide a writeable configuration.
	/// </summary>
	public class JSToolsConfigurationWriteable : AJSToolsConfiguration
	{
		//------------------------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------------------------


		//------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Initializes a new JavaScript configuration writer instance.
		/// </summary>
		/// <param name="configDocument">Loads the configuration from the specified xml document.</param>
		/// <param name="sessionHandler">Specifies the AJSToolsSessionHandler handler, which handles copy-on-write mechanism.</param>
		/// <exception cref="ArgumentNullException">The given XmlDocument or the session handler contains a null reference.</exception>
		/// <exception cref="ArgumentException">Could not load the given configuration file.</exception>
		/// <exception cref="ConfigurationException">Could not initialize a type specified in a configuration xml section.</exception>
		public JSToolsConfigurationWriteable(XmlDocument configDocument, AJSToolsSessionHandler sessionHandler) : base(configDocument, sessionHandler)
		{
		}


		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Overrides the CreateSectionInstance to create writeable configuration sections.
		/// </summary>
		/// <param name="sectionHandler">Section handler that creates the configuration handler instance.</param>
		/// <param name="section">Specifies the AJSToolsSessionHandler handler, which handles copy-on-write mechanism.</param>
		/// <returns>Returns the created instance.</returns>
		protected override AJSToolsEventHandler CreateSectionInstance(AJSToolsConfigSectionHandlerFactory sectionHandler, XmlNode section)
		{
			AJSToolsEventHandler renderHandler = sectionHandler.CreateWriteableInstance(section);
			return (renderHandler != null) ? renderHandler : _handler.ImmutableInstance.GetConfig(sectionHandler.SectionName);
		}
	}
}

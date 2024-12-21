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
using System.IO;
using System.Xml;

using JSTools.Config.ExceptionHandling;
using JSTools.Config.ScriptFileManagement;

namespace JSTools.Config
{
	/// <summary>
	/// Represents the configuration handler interface.
	/// </summary>
	public interface IJSToolsConfiguration
	{
		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the script file handler, which handles using of the debug and release scripts.
		/// </summary>
		JSScriptFileHandler ScriptFileHandler
		{
			get;
		}

		/// <summary>
		/// Returns true, if a XmlDocument is initialized.
		/// </summary>
		bool IsXmlDocumentInitialized
		{
			get;
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Returns requested configuration information for the JSTools.
		/// </summary>
		/// <param name="configNodeName">Name of the section node.</param>
		/// <returns>Returns the requested configuration instance.</returns>
		/// <exception cref="ArgumentNullException">The specified name contains a null reference.</exception>
		AJSToolsSection GetConfig(string configNodeName);

		/// <summary>
		/// Loads the given XmlDocument and initializes the configuration sections.
		/// </summary>
		/// <param name="configDocument">Loads the configuration from the specified xml document.</param>
		/// <exception cref="ArgumentNullException">The given xml document contains a null reference.</exception>
		/// <exception cref="InvalidOperationException">The configuration XmlDocument was not already specified.</exception>
		/// <exception cref="ConfigurationException">Could not initialize a type specified in a configuration xml section.</exception>
		void LoadXml(XmlDocument configDocument);

		/// <summary>
		/// Loads the given XmlDocument and initializes the configuration sections.
		/// </summary>
		/// <param name="configDocument">Loads the configuration from the specified string.</param>
		/// <exception cref="ArgumentNullException">The given xml document contains a null reference.</exception>
		/// <exception cref="InvalidOperationException">The configuration XmlDocument was not already specified.</exception>
		/// <exception cref="ConfigurationException">Could not initialize a type specified in a configuration xml section.</exception>
		/// <exception cref="XmlException">There is a load or parse error in the XML.</exception>
		void LoadXml(string configDocument);

		/// <summary>
		/// Loads the given XmlDocument and initializes the configuration sections.
		/// </summary>
		/// <param name="configDocument">Loads the configuration from the specified Stream.</param>
		/// <exception cref="ArgumentNullException">The given xml document contains a null reference.</exception>
		/// <exception cref="InvalidOperationException">The configuration XmlDocument was not already specified.</exception>
		/// <exception cref="ConfigurationException">Could not initialize a type specified in a configuration xml section.</exception>
		/// <exception cref="XmlException">There is a load or parse error in the XML.</exception>
		void LoadXml(Stream configDocument);

		/// <summary>
		/// Loads the given XmlDocument and initializes the configuration sections.
		/// </summary>
		/// <param name="configDocument">Loads the configuration from the specified XmlReader.</param>
		/// <exception cref="ArgumentNullException">The given xml document contains a null reference.</exception>
		/// <exception cref="InvalidOperationException">The configuration XmlDocument was not already specified.</exception>
		/// <exception cref="ConfigurationException">Could not initialize a type specified in a configuration xml section.</exception>
		/// <exception cref="XmlException">There is a load or parse error in the XML.</exception>
		void LoadXml(XmlReader configDocument);

		/// <summary>
		/// Loads the given XmlDocument and initializes the configuration sections.
		/// </summary>
		/// <param name="configDocument">Loads the configuration from the specified TextReader.</param>
		/// <exception cref="ArgumentNullException">The given xml document contains a null reference.</exception>
		/// <exception cref="InvalidOperationException">The configuration XmlDocument was not already specified.</exception>
		/// <exception cref="ConfigurationException">Could not initialize a type specified in a configuration xml section.</exception>
		/// <exception cref="XmlException">There is a load or parse error in the XML.</exception>
		void LoadXml(TextReader configDocument);

		/// <summary>
		/// Writes the JSTools sections into the given render context.
		/// </summary>
		/// <param name="renderContext">The RenderProcessTicket object.</param>
		/// <exception cref="ConfigurationException">Could not render a configuration section.</exception>
		/// <exception cref="ConfigurationException">A section with the specified name does not exist.</exception>
		/// <exception cref="InvalidOperationException">The configuration XmlDocument is not specified.</exception>
		/// <exception cref="ArgumentNullException">The given RenderProcessTicket object contains a null reference</exception>
		void Render(RenderProcessTicket renderContext);
	}
}

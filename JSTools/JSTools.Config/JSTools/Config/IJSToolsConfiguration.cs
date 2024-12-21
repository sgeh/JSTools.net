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
using System.Configuration;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Xml;

using JSTools.Config.ExceptionHandling;
using JSTools.Config.ScriptFileManagement;
using JSTools.Config.Session;

namespace JSTools.Config
{
	/// <summary>
	/// Represents the configuration handler interface.
	/// </summary>
	public interface IJSToolsConfiguration
	{
		//------------------------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Returns the immutable application configuration.
		/// </summary>
		/// <exception cref="InvalidSessionHandlerException">Could not create an AJSToolsConfiguration instance, the session handler has returned an error.</exception>
		/// <exception cref="InvalidOperationException">Could not refer to the immutable instance, it is not fully created yet.</exception>
		IJSToolsConfiguration ImmutableInstance
		{
			get;
		}


		/// <summary>
		/// Returns the writeable configuration. If it does not exist, this method will create a new
		/// writeable configuration and store it in the session cache.
		/// </summary>
		/// <exception cref="InvalidSessionHandlerException">Could not create an AJSToolsConfiguration instance, the session handler has returned an error.</exception>
		/// <exception cref="InvalidOperationException">Could not create a new writeable instance, the immutable instance is not initialized yet.</exception>
		/// <exception cref="JSToolsEventException">An error has occured during event bubbling.</exception>
		IJSToolsConfiguration WriteableInstance
		{
			get;
		}


		/// <summary>
		/// Gets/sets the type of client exception handling.
		/// </summary>
		/// <exception cref="InvalidSessionHandlerException">Could not create an AJSToolsConfiguration instance, the session handler has returned an error.</exception>
		AJSExceptionHandler ErrorHandling
		{
			get;
		}


		/// <summary>
		/// Gets the script file handler, which handles using of the debug and release scripts.
		/// </summary>
		/// <exception cref="InvalidSessionHandlerException">Could not create an AJSToolsConfiguration instance, the session handler has returned an error.</exception>
		AJSScriptFileHandler ScriptFileHandler
		{
			get;
		}


		/// <summary>
		/// Returns true, if a XmlDocument is initialized.
		/// </summary>
		/// <exception cref="InvalidSessionHandlerException">Could not create an AJSToolsConfiguration instance, the session handler has returned an error.</exception>
		bool IsXmlDocumentInitialized
		{
			get;
		}


		/// <summary>
		/// Will be fired if the user writes a value.
		/// </summary>
		event JSToolsWriteableCreateEvent OnWriteableCreate;


		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Writes the JSTools sections into the given render context.
		/// </summary>
		/// <param name="renderContext">The RenderProcessTicket object.</param>
		/// <exception cref="ConfigurationException">Could not render a configuration section.</exception>
		/// <exception cref="InvalidOperationException">The configuration XmlDocument is not specified.</exception>
		/// <exception cref="ArgumentNullException">The given RenderProcessTicket object contains a null reference</exception>
		void Render(RenderProcessTicket renderContext);


		/// <summary>
		/// Serializes the configuration into a xml document and saves it to the specified file path.
		/// Default encoding is UTF-8.
		/// </summary>
		/// <param name="filePath">File path where the serialized file should be placed in.</param>
		/// <exception cref="ArgumentNullException">The specified file path contains a null reference.</exception>
		/// <exception cref="ArgumentException">Could not write into the specified stream or the specified file path is invalid.</exception>
		/// <exception cref="InvalidOperationException">The configuration XmlDocument is not specified.</exception>
		/// <exception cref="IOException">An I/O error occurs.</exception>
		void SaveConfiguration(string filePath);


		/// <summary>
		/// Serializes the configuration into a xml document and saves it to the specified file path.
		/// </summary>
		/// <param name="filePath">File path where the serialized file should be placed in.</param>
		/// <param name="encoding">Ecoding to write.</param>
		/// <exception cref="ArgumentNullException">The specified file path contains a null reference.</exception>
		/// <exception cref="ArgumentException">Could not write into the specified stream or the specified file path is invalid.</exception>
		/// <exception cref="InvalidOperationException">The configuration XmlDocument is not specified.</exception>
		/// <exception cref="IOException">An I/O error occurs.</exception>
		/// <exception cref="ArgumentNullException">The given encoding contains a null reference.</exception>
		void SaveConfiguration(string filePath, Encoding encoding);


		/// <summary>
		/// Serializes the configuration into a xml document which will be written into the specified stream. Be sure
		/// that you close the stream manually.
		/// </summary>
		/// <param name="toWrite">Specifies the stream.</param>
		/// <param name="encoding">Ecoding to write.</param>
		/// <exception cref="ArgumentNullException">The specified file path contains a null reference.</exception>
		/// <exception cref="ArgumentException">Could not write into the given stream.</exception>
		/// <exception cref="InvalidOperationException">The configuration XmlDocument is not specified.</exception>
		/// <exception cref="IOException">An I/O error occurs, such as the file being closed.</exception>
		/// <exception cref="ObjectDisposedException">Methods were called after the stream was closed.</exception>
		/// <exception cref="ArgumentNullException">The given encoding contains a null reference.</exception>
		void SaveConfiguration(Stream toWrite, Encoding encoding);


		/// <summary>
		/// Serializes the configuration into a xml document which will be written into the specified stream. Be sure
		/// that you close the stream manually. Default encoding is UTF-8.
		/// </summary>
		/// <param name="toWrite">Specifies the stream.</param>
		/// <exception cref="ArgumentNullException">The specified file path contains a null reference.</exception>
		/// <exception cref="ArgumentException">Could not write into the given stream.</exception>
		/// <exception cref="InvalidOperationException">The configuration XmlDocument is not specified.</exception>
		/// <exception cref="IOException">An I/O error occurs, such as the file being closed.</exception>
		/// <exception cref="ObjectDisposedException">Methods were called after the stream was closed.</exception>
		void SaveConfiguration(Stream toWrite);


		/// <summary>
		/// Returns requested configuration information for the JSTools.
		/// </summary>
		/// <param name="configNodeName">Name of the section node.</param>
		/// <returns>Returns the requested configuration instance.</returns>
		/// <exception cref="ArgumentNullException">The specified name contains a null reference.</exception>
		AJSToolsEventHandler GetConfig(string configNodeName);


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
	}
}

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
using System.Collections;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

using JSTools.Config.ExceptionHandling;
using JSTools.Config.ScriptFileManagement;
using JSTools.Xml;

namespace JSTools.Config
{
	/// <summary>
	/// Contains all configuration capatibilities of the JSTools Framework for the .NET
	/// environment.
	/// </summary>
	public class JSToolsConfiguration : AJSToolsSection, IJSToolsConfiguration
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the begin tag of an HTML comment.
		/// </summary>
		public const string COMMENT_BEGIN = "<!--";

		/// <summary>
		/// Gets the end tag of an HTML comment.
		/// </summary>
		public const string COMMENT_END = "-->";

		private const string CONFIG_NODE_NAME = "configuration";
		private const string TYPE_ATTRIB = "type";
		private const string SCRIPTS_NODE = "scripts";

		private const string SECTION_COMMENT_BEGIN = "\n" + COMMENT_BEGIN + " SECTION {0} BEGIN " + COMMENT_END + "\n\n";
		private const string SECTION_COMMENT_END = "\n" + COMMENT_BEGIN + " SECTION {0} END " + COMMENT_END + " \n\n";

		private JSScriptFileHandler _scriptFileHandler = null;
		private Hashtable _configSections = new Hashtable();

		private XmlDocument _configDocument = new XmlDocument();
		private bool _configInitialized = false;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Returns the name of the representing element.
		/// </summary>
		public string SectionName
		{
			get { return CONFIG_NODE_NAME; }
		}

		/// <summary>
		/// Returns true, if the internal XmlDocument is initialized.
		/// </summary>
		public bool IsXmlDocumentInitialized
		{
			get { return _configInitialized; }
		}

		/// <summary>
		/// Gets the script file handler, which handles using of the debug and release scripts.
		/// </summary>
		public JSScriptFileHandler ScriptFileHandler
		{
			get { return _scriptFileHandler; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Initializes a new JavaScript configuration handler instance.
		/// </summary>
		public JSToolsConfiguration()
		{
		}

		/// <summary>
		/// Initializes a new JavaScript configuration handler instance.
		/// </summary>
		/// <param name="configDocument">Loads the configuration from the specified xml document.</param>
		/// <exception cref="ArgumentNullException">The given XmlDocument contains a null reference.</exception>
		/// <exception cref="ArgumentException">Could not load the given configuration file.</exception>
		/// <exception cref="ConfigurationException">Could not initialize a type specified in a configuration xml section.</exception>
		public JSToolsConfiguration(XmlDocument configDocument)
		{
			if (configDocument == null)
				throw new ArgumentNullException("configDocument", "The given XmlDocument contains a null reference.");

			LoadXml(configDocument);
		}

		/// <summary>
		/// Initializes a new JavaScript configuration handler instance.
		/// </summary>
		/// <param name="configFilePath">Loads the configuration from the specified path.</param>
		/// <exception cref="ArgumentNullException">The given configuration path contains a null reference.</exception>
		/// <exception cref="ArgumentException">Could not load the given configuration file.</exception>
		/// <exception cref="ConfigurationException">Could not initialize a type specified in a configuration xml section.</exception>
		public JSToolsConfiguration(string configFilePath)
		{
			if (configFilePath == null)
				throw new ArgumentNullException("configFilePath", "The given configuration contains a null reference.");

			try
			{
				_configDocument.Load(configFilePath);
			}
			catch (Exception e)
			{
				throw new ArgumentException("Could not load the given configuration file '" + configFilePath + "'! Error description: " + e.Message, "configFilePath", e);
			}
			InitConfiguration();
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Writes the JSTools sections into the given render context.
		/// </summary>
		/// <param name="renderContext">The RenderProcessTicket object.</param>
		/// <exception cref="ConfigurationException">Could not render a configuration section.</exception>
		/// <exception cref="ConfigurationException">A section with the specified name does not exist.</exception>
		/// <exception cref="InvalidOperationException">The configuration XmlDocument is not specified.</exception>
		/// <exception cref="ArgumentNullException">The given RenderProcessTicket object contains a null reference</exception>
		public void Render(RenderProcessTicket renderContext)
		{
			if (!IsXmlDocumentInitialized)
				throw new InvalidOperationException("The configuration XmlDocument is not specified.");

			if (renderContext == null)
				throw new ArgumentNullException("renderContext", "The given RenderProcessTicket object contains a null reference.");

			foreach (IJSToolsRenderHandler renderHandler in renderContext)
			{
				AJSToolsSection section = GetConfig(renderHandler.SectionName);

				if (section == null)
					throw new ConfigurationException("A section with the specified name '" + renderHandler.SectionName + "' does not exist.");

				try
				{
					renderHandler.RenderSection(renderContext, section);
				}
				catch (Exception e)
				{
					throw new ConfigurationException("Could not render configuration section '" + renderHandler.SectionName + "'. Error description: " + e.Message, e);
				}
			}
		}

		/// <summary>
		/// Returns the requested configuration section, specified in the JSTools configuration file.
		/// You will obtain a null reference, if no handler was found.
		/// </summary>
		/// <param name="configNodeName">Name of the section node.</param>
		/// <returns>Returns the requested configuration instance.</returns>
		/// <exception cref="ArgumentNullException">The specified name contains a null reference.</exception>
		public AJSToolsSection GetConfig(string configNodeName)
		{
			if (configNodeName == null)
				throw new ArgumentNullException("configNodeName", "The specified name contains a null reference.");

			if (_configSections.Contains(configNodeName))
			{
				return (_configSections[configNodeName] as AJSToolsSection);
			}
			return null;
		}

		/// <summary>
		/// Loads the given XmlDocument and initializes the configuration sections.
		/// </summary>
		/// <param name="configDocument">Loads the configuration from the specified xml document.</param>
		/// <exception cref="ArgumentNullException">The given xml document contains a null reference.</exception>
		/// <exception cref="InvalidOperationException">The configuration XmlDocument was already specified.</exception>
		/// <exception cref="ConfigurationException">Could not initialize a type specified in a configuration xml section.</exception>
		public void LoadXml(XmlDocument configDocument)
		{
			if (configDocument == null)
				throw new ArgumentNullException("configDocument", "The given xml document contains a null reference.");

			if (_configInitialized)
				throw new InvalidOperationException("The configuration XmlDocument was already specified.");

			// import nodes
			foreach (XmlNode node in configDocument.ChildNodes)
			{
				XmlNode importedNode = _configDocument.ImportNode(node, true);
				_configDocument.AppendChild(importedNode);
			}
			InitConfiguration();
		}

		/// <summary>
		/// Loads the given XmlDocument and initializes the configuration sections.
		/// </summary>
		/// <param name="configDocument">Loads the configuration from the specified string.</param>
		/// <exception cref="ArgumentNullException">The given xml document contains a null reference.</exception>
		/// <exception cref="InvalidOperationException">The configuration XmlDocument was already specified.</exception>
		/// <exception cref="ConfigurationException">Could not initialize a type specified in a configuration xml section.</exception>
		/// <exception cref="XmlException">There is a load or parse error in the XML.</exception>
		public void LoadXml(string configDocument)
		{
			if (configDocument == null)
				throw new ArgumentNullException("configDocument", "The given xml document contains a null reference.");

			if (_configInitialized)
				throw new InvalidOperationException("The configuration XmlDocument was already specified.");

			_configDocument.Load(configDocument);
			InitConfiguration();
		}

		/// <summary>
		/// Loads the given XmlDocument and initializes the configuration sections.
		/// </summary>
		/// <param name="configDocument">Loads the configuration from the specified Stream.</param>
		/// <exception cref="ArgumentNullException">The given xml document contains a null reference.</exception>
		/// <exception cref="InvalidOperationException">The configuration XmlDocument was already specified.</exception>
		/// <exception cref="ConfigurationException">Could not initialize a type specified in a configuration xml section.</exception>
		/// <exception cref="XmlException">There is a load or parse error in the XML.</exception>
		public void LoadXml(Stream configDocument)
		{
			if (configDocument == null)
				throw new ArgumentNullException("configDocument", "The given xml document contains a null reference.");

			if (_configInitialized)
				throw new InvalidOperationException("The configuration XmlDocument was already specified.");

			_configDocument.Load(configDocument);
			InitConfiguration();
		}

		/// <summary>
		/// Loads the given XmlDocument and initializes the configuration sections.
		/// </summary>
		/// <param name="configDocument">Loads the configuration from the specified XmlReader.</param>
		/// <exception cref="ArgumentNullException">The given xml document contains a null reference.</exception>
		/// <exception cref="InvalidOperationException">The configuration XmlDocument was already specified.</exception>
		/// <exception cref="ConfigurationException">Could not initialize a type specified in a configuration xml section.</exception>
		/// <exception cref="XmlException">There is a load or parse error in the XML.</exception>
		public void LoadXml(XmlReader configDocument)
		{
			if (configDocument == null)
				throw new ArgumentNullException("configDocument", "The given xml document contains a null reference.");

			if (_configInitialized)
				throw new InvalidOperationException("The configuration XmlDocument was already specified.");

			_configDocument.Load(configDocument);
			InitConfiguration();
		}

		/// <summary>
		/// Loads the given XmlDocument and initializes the configuration sections.
		/// </summary>
		/// <param name="configDocument">Loads the configuration from the specified TextReader.</param>
		/// <exception cref="ArgumentNullException">The given xml document contains a null reference.</exception>
		/// <exception cref="InvalidOperationException">The configuration XmlDocument was already specified.</exception>
		/// <exception cref="ConfigurationException">Could not initialize a type specified in a configuration xml section.</exception>
		/// <exception cref="XmlException">There is a load or parse error in the XML.</exception>
		public void LoadXml(TextReader configDocument)
		{
			if (configDocument == null)
				throw new ArgumentNullException("configDocument", "The given xml document contains a null reference.");

			if (_configInitialized)
				throw new InvalidOperationException("The configuration XmlDocument was already specified.");

			_configDocument.Load(configDocument);
			InitConfiguration();
		}

		/// <summary>
		/// Initializes the values of this configuration object.
		/// </summary>
		/// <remarks>This method can throw a ConfigurationException.</remarks>
		private void InitConfiguration()
		{
			InitScriptConfiguration();

			foreach (XmlNode configNode in _configDocument.DocumentElement.SelectNodes("*[name(.)!='" + SCRIPTS_NODE + "']"))
			{
				if (configNode.Attributes[TYPE_ATTRIB] == null)
				{
					throw new ConfigurationException("Could not identify the 'type' attribute of the configuration node '" + configNode.Name + "'.", configNode);
				}
				InitNodeAndCheckForExceptions(configNode);
			}

			InitDefaultSections();
			InitRelations();
			_configInitialized = true;
		}

		/// <summary>
		/// Call CheckRelations procedure to initialize relations between the sections.
		/// </summary>
		private void InitRelations()
		{
			foreach (DictionaryEntry entry in _configSections)
			{
				((AJSToolsSection)entry.Value).CheckRelations();
			}
		}

		/// <summary>
		/// Initializes the script handler configuration section as first.
		/// </summary>
		private void InitScriptConfiguration()
		{
			XmlNode scriptNode = _configDocument.DocumentElement.SelectSingleNode("./" + SCRIPTS_NODE);

			if (scriptNode != null)
			{
				InitNodeAndCheckForExceptions(scriptNode);
			}
		}

		/// <summary>
		/// Creates a new section instance and casts all exceptions into a ConfigurationException.
		/// </summary>
		/// <param name="configNode">The XmlNode that describes the type.</param>
		private void InitNodeAndCheckForExceptions(XmlNode configNode)
		{
			try
			{
				InitNodeType(configNode);
			}
			catch (Exception e)
			{
				throw new ConfigurationException("Could not initialize the type '" + configNode.Attributes[TYPE_ATTRIB].Value + "' specified in the '" + configNode.Name + "' node! Error description: " + e.Message, e, configNode);
			}
		}

		/// <summary>
		/// Initilizes the type specified in the "type" attribute of the given node and adds it to the
		/// _configSections Hashtable.
		/// </summary>
		/// <param name="configNode">The XmlNode that describes the type.</param>
		private void InitNodeType(XmlNode configNode)
		{
			if (_configSections.Contains(configNode.Name))
				throw new InvalidOperationException("A configuration section with the name '" + configNode.Name + "' already exists.");

			AJSToolsConfigSectionHandlerFactory handlerInstance = GetInstanceOfType(configNode.Attributes[TYPE_ATTRIB].Value);

			if (handlerInstance == null)
				throw new InvalidOperationException("The specified type is not derived from AJSToolsConfigSectionHandler.");

			if (configNode.Name != handlerInstance.SectionName)
				throw new InvalidOperationException("The given node has not the same section name as specified in the configuration section handler.");

			_configSections.Add(configNode.Name, handlerInstance.CreateInstance(configNode, this));
		}

		/// <summary>
		/// Initilizes the default configuration sections.
		/// </summary>
		private void InitDefaultSections()
		{
			_scriptFileHandler = (GetConfig(SCRIPTS_NODE) as JSScriptFileHandler);

			if (_scriptFileHandler == null)
			{
				throw new ConfigurationException("The configuration file does not contain a '" + SCRIPTS_NODE + "' section.");
			}
		}

		/// <summary>
		/// Creates an instance from the given type and assembly name.
		/// </summary>
		/// <param name="typeAttributeValue">String of the type attribute.</param>
		/// <returns>Returns the created instance or null, if no instance was created.</returns>
		private AJSToolsConfigSectionHandlerFactory GetInstanceOfType(string typeAttributeValue)
		{
			string[] typeDefinition = typeAttributeValue.Split(',');
			
			string type = GetStringFromArray(typeDefinition, 0);
			string assembly = GetStringFromArray(typeDefinition, 1);

			if (type == null)
			{
				throw new NullReferenceException("The given node contains an invalid type definition.");
			}
			return CreateInstanceOfType(assembly, type);
		}

		/// <summary>
		/// Creates an instance of the specified type.
		/// </summary>
		/// <param name="assembly">Name of the assembly.</param>
		/// <param name="type">Name of the type.</param>
		/// <returns>Returns the created type.</returns>
		private AJSToolsConfigSectionHandlerFactory CreateInstanceOfType(string assembly, string type)
		{
			if (assembly != null)
			{
				return (Activator.CreateInstance(assembly, type).Unwrap() as AJSToolsConfigSectionHandlerFactory);
			}
			else
			{
				return (Activator.CreateInstance(Assembly.GetExecutingAssembly().FullName, type).Unwrap() as AJSToolsConfigSectionHandlerFactory);
			}
		}

		/// <summary>
		/// Returns the value of the given index form the specified array. If an error occurs
		/// (IndexOutOfRangeException, NullReferenceException, ...) you will get a null reference.
		/// </summary>
		/// <param name="array">The array, which contains the given index.</param>
		/// <param name="index">The position of the Array element to get.</param>
		/// <returns>Returns the value of the given index form the specified array.</returns>
		private string GetStringFromArray(string[] array, int index)
		{
			if (array != null && array.Length > index && (array.GetValue(index) as string) != null)
			{
				return (array.GetValue(index) as string).Trim();
			}
			return null;
		}
	}
}

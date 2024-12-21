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
using System.Reflection;
using System.Xml;

using JSTools.Config.ExceptionHandling;
using JSTools.Config.ScriptFileManagement;

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
		/// <param name="configDocument">Loads the configuration from the specified xml document.</param>
		/// <exception cref="ArgumentNullException">The given XmlDocument contains a null reference.</exception>
		/// <exception cref="ConfigurationException">Error while initializing the configuration.</exception>
		public JSToolsConfiguration(XmlDocument configDocument)
		{
			if (configDocument == null)
				throw new ArgumentNullException("configDocument", "The given XmlDocument contains a null reference.");

			try
			{
				InitConfiguration((XmlDocument)configDocument.Clone());
			}
			catch (ConfigurationException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new ConfigurationException("Error while initializing the configuration.", e);
			}
		}

		/// <summary>
		/// Initializes a new JavaScript configuration handler instance.
		/// </summary>
		/// <param name="configFilePath">Loads the configuration from the specified path.</param>
		/// <exception cref="ArgumentNullException">The given configuration path contains a null reference.</exception>
		/// <exception cref="ConfigurationException">Error while initializing the configuration.</exception>
		public JSToolsConfiguration(string configFilePath)
		{
			if (configFilePath == null)
				throw new ArgumentNullException("configFilePath", "The given configuration contains a null reference.");

			try
			{
				XmlDocument document = new XmlDocument();
				document.Load(configFilePath);
				InitConfiguration(document);
			}
			catch (ConfigurationException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new ConfigurationException("Error while initializing the configuration.", e);
			}
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
		/// <exception cref="ArgumentNullException">The given RenderProcessTicket object contains a null reference</exception>
		public void Render(RenderProcessTicket renderContext)
		{
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
					throw new ConfigurationException(
						string.Format(
							"Could not render configuration section '{0}'. Error description: {1}",
							renderHandler.SectionName,
							e.Message ),
						e );
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
				return (_configSections[configNodeName] as AJSToolsSection);

			return null;
		}

		/// <summary>
		/// This method is called if a section is initialized.
		/// </summary>
		/// <param name="sectionToLoad">Section which should be loaded.</param>
		/// <returns>Returns the initilized section.</returns>
		protected virtual AJSToolsSection InitSection(XmlNode sectionToLoad)
		{
			if (sectionToLoad == null)
				throw new ArgumentNullException("sectionToLoad", "Sections which should be loaded must not contain null references.");

			try
			{
				return InitNodeType(sectionToLoad);
			}
			catch (Exception e)
			{
				throw new ConfigurationException(
					string.Format("Could not initialize the type '{0}' specified in the '{1}' node.",
						sectionToLoad.Attributes[TYPE_ATTRIB].Value,
						sectionToLoad.Name ),
					e,
					sectionToLoad );
			}
		}

		/// <summary>
		/// This method is called before the sections are initilialized. You
		/// can initilialize default sections here.
		/// </summary>
		/// <param name="configDocument">Configuration document to initialize.</param>
		protected virtual void BeforeInitConfiguration(XmlDocument configDocument)
		{
			XmlNamespaceManager manager = new XmlNamespaceManager(configDocument.NameTable);
			manager.AddNamespace(
				JSScriptFileHandlerFactory.NAMESPACE_ABBR,
				JSScriptFileHandlerFactory.NAMESPACE );

			XmlNode scriptFileHandler = configDocument.DocumentElement.SelectSingleNode(
				JSScriptFileHandlerFactory.NAMESPACE_XPATH,
				manager );

			_scriptFileHandler = (InitSection(scriptFileHandler) as JSScriptFileHandler);
		}

		/// <summary>
		/// This method is called after the configuration has been initialized.
		/// </summary>
		protected virtual void AfterInitConfiguration()
		{
			InitRelations();
		}

		private void InitConfiguration(XmlDocument configDocument)
		{
			BeforeInitConfiguration(configDocument);

			foreach (XmlNode configNode in configDocument.DocumentElement.ChildNodes)
			{
				if (configNode.NodeType == XmlNodeType.Element)
					InitSection(configNode);
			}
			AfterInitConfiguration();
		}

		private void InitRelations()
		{
			foreach (DictionaryEntry entry in _configSections)
			{
				((AJSToolsSection)entry.Value).CheckRelations();
			}
		}

		private AJSToolsSection InitNodeType(XmlNode configNode)
		{
			AJSToolsSection loadedSection = null;

			if (!_configSections.Contains(configNode.Name))
			{
				AJSToolsConfigSectionHandlerFactory handlerInstance = GetInstanceOfType(configNode.Attributes[TYPE_ATTRIB].Value);

				if (handlerInstance == null)
					throw new InvalidOperationException("The specified type is not derived from AJSToolsConfigSectionHandler.");

				if (configNode.Name != handlerInstance.SectionName)
					throw new InvalidOperationException("The given node has not the same section name as specified in the configuration section handler.");

				loadedSection = handlerInstance.CreateInstance(configNode, this);
				_configSections.Add(configNode.Name, loadedSection);
			}
			return loadedSection;
		}

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

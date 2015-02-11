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
using System.Reflection;
using System.Text;
using System.Xml;

using JSTools.Config.ExceptionHandling;
using JSTools.Config.ScriptFileManagement;
using JSTools.Config.Session;
using JSTools.Xml;

namespace JSTools.Config
{
	/// <summary>
	/// Used to serialize the specified sections.
	/// </summary>
	public delegate void JSToolsWriteableCreateEvent(IJSToolsConfiguration writeableInstance);


	/// <summary>
	/// Contains all configuration capatibilities of the JSTools Framework for the asp.net
	/// environment.
	/// </summary>
	public abstract class AJSToolsConfiguration : AJSToolsEventHandler, IJSToolsConfiguration, IWriteable
	{
		//------------------------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------------------------

		public	const	string							COMMENT_BEGIN			= "<!--";
		public	const	string							COMMENT_END				= "-->";
		public	const	string							CONFIG_NODE_NAME		= "configuration";

		protected		AJSToolsSessionHandler			_handler				= null;

		private	const	string							TYPE_ATTRIB				= "type";
		private	const	string							SCRIPTS_NODE			= "scripts";
		private	const	string							EXCEPTION_NODE			= "exception";

		private	const	string							SECTION_COMMENT_BEGIN	= "\n" + COMMENT_BEGIN + " SECTION {0} BEGIN " + COMMENT_END + "\n\n";
		private	const	string							SECTION_COMMENT_END		= "\n" + COMMENT_BEGIN + " SECTION {0} END " + COMMENT_END + " \n\n";

		private			AJSExceptionHandler				_exceptionHandling		= null;
		private			AJSScriptFileHandler			_scriptFileHandler		= null;
		private			Hashtable						_configSections			= new Hashtable();

		private			XmlDocument						_configDocument			= new XmlDocument();
		private			bool							_configInitialized		= false;


		/// <summary>
		/// Returns the writeable configuration. If it does not exist, this method will create a new
		/// writeable configuration and store it in the session cache.
		/// </summary>
		/// <exception cref="InvalidSessionHandlerException">Could not create an AJSToolsConfiguration instance, the session handler has returned an error.</exception>
		/// <exception cref="InvalidOperationException">Could not create a new writeable instance, the immutable instance is not initialized yet.</exception>
		/// <exception cref="JSToolsEventException">An error has occured during event bubbling.</exception>
		IJSToolsConfiguration IJSToolsConfiguration.WriteableInstance
		{
			get { return WriteableInstance; }
		}


		/// <summary>
		/// Returns the writeable configuration. If it does not exist, this method will create a new
		/// writeable configuration and store it in the session cache.
		/// </summary>
		/// <exception cref="InvalidSessionHandlerException">Could not create an AJSToolsConfiguration instance, the session handler has returned an error.</exception>
		/// <exception cref="InvalidOperationException">Could not create a new writeable instance, the immutable instance is not initialized yet.</exception>
		/// <exception cref="JSToolsEventException">An error has occured during event bubbling.</exception>
		AJSToolsEventHandler IWriteable.WriteableInstance
		{
			get { return WriteableInstance; }
		}


		/// <summary>
		/// Returns the name of the representing element.
		/// </summary>
		public string SectionName
		{
			get { return CONFIG_NODE_NAME; }
		}


		/// <summary>
		/// Will be fired if the user writes a value.
		/// </summary>
		public event JSToolsWriteableCreateEvent OnWriteableCreate;



		/// <summary>
		/// Returns the immutable application configuration.
		/// </summary>
		/// <exception cref="InvalidSessionHandlerException">Could not create an AJSToolsConfiguration instance, the session handler has returned an error.</exception>
		/// <exception cref="InvalidOperationException">Could not refer to the immutable instance, it is not fully created yet.</exception>
		public IJSToolsConfiguration ImmutableInstance
		{
			get { return _handler.ImmutableInstance; }
		}


		/// <summary>
		/// Returns the writeable configuration. If it does not exist, this method will create a new
		/// writeable configuration and store it in the session cache.
		/// </summary>
		/// <exception cref="InvalidSessionHandlerException">Could not create an AJSToolsConfiguration instance, the session handler has returned an error.</exception>
		/// <exception cref="InvalidOperationException">Could not create a new writeable instance, the immutable instance is not initialized yet.</exception>
		/// <exception cref="JSToolsEventException">An error has occured during event bubbling.</exception>
		public AJSToolsConfiguration WriteableInstance
		{
			get { return _handler.WriteableInstance; }
		}


		/// <summary>
		/// Returns true, if the internal XmlDocument is initialized.
		/// </summary>
		public bool IsXmlDocumentInitialized
		{
			get { return _configInitialized; }
		}


		/// <summary>
		/// Gets/sets the type of client exception handling.
		/// </summary>
		public AJSExceptionHandler ErrorHandling
		{
			get { return _exceptionHandling; }
		}


		/// <summary>
		/// Gets the script file handler, which handles using of the debug and release scripts.
		/// </summary>
		public AJSScriptFileHandler ScriptFileHandler
		{
			get { return _scriptFileHandler; }
		}


		/// <summary>
		/// Represents the owner configuration.
		/// </summary>
		public override IJSToolsConfiguration OwnerConfiguration
		{
			get { return null; }
		}


		/// <summary>
		/// Represents the owner configuration.
		/// </summary>
		public override AJSToolsEventHandler OwnerSection
		{
			get { return null; }
		}


		/// <summary>
		/// Returns the internal configuration XmlDocument.
		/// </summary>
		internal XmlDocument ConfigDocument
		{
			get { return _configDocument; }
		}


		//------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Initializes a new JavaScript configuration handler instance. This constructer does not need
		/// a XmlDocument instance, so you have to initialize it with the LoadXml method.
		/// </summary>
		/// <param name="sessionHandler">A reference to the session handler.</param>
		/// <exception cref="ArgumentNullException">The specified AJSToolsSessionHandler contains a null reference.</exception>
		protected AJSToolsConfiguration(AJSToolsSessionHandler sessionHandler)
		{
			if (sessionHandler == null)
			{
				throw new ArgumentNullException("sessionHandler", "The specified AJSToolsSessionHandler contains a null reference!");
			}

			_handler = sessionHandler;
			_handler.OnWriteableCreate += new JSToolsWriteableCreateEvent(FireOnWriteableCreate);
		}


		/// <summary>
		/// Initializes a new JavaScript configuration handler instance.
		/// </summary>
		/// <param name="configDocument">Loads the configuration from the specified xml document.</param>
		/// <param name="sessionHandler">A reference to the session handler.</param>
		/// <exception cref="ArgumentNullException">The given XmlDocument or the session handler contains a null reference.</exception>
		/// <exception cref="ArgumentException">Could not load the given configuration file.</exception>
		/// <exception cref="ConfigurationException">Could not initialize a type specified in a configuration xml section.</exception>
		protected AJSToolsConfiguration(XmlDocument configDocument, AJSToolsSessionHandler sessionHandler)
		{
			if (configDocument == null)
				throw new ArgumentNullException("configDocument", "The given XmlDocument contains a null reference!");

			if (sessionHandler == null)
				throw new ArgumentNullException("sessionHandler", "The specified AJSToolsSessionHandler contains a null reference!");

			_handler = sessionHandler;
			_handler.OnWriteableCreate += new JSToolsWriteableCreateEvent(FireOnWriteableCreate);
			LoadXml(configDocument);
		}


		/// <summary>
		/// Initializes a new JavaScript configuration handler instance.
		/// </summary>
		/// <param name="configFilePath">Loads the configuration from the specified path.</param>
		/// <param name="sessionHandler">A reference to the session handler.</param>
		/// <exception cref="ArgumentNullException">The given configuration path or the session handler contains a null reference.</exception>
		/// <exception cref="ArgumentException">Could not load the given configuration file.</exception>
		/// <exception cref="ConfigurationException">Could not initialize a type specified in a configuration xml section.</exception>
		protected AJSToolsConfiguration(string configFilePath, AJSToolsSessionHandler sessionHandler)
		{
			if (configFilePath == null)
				throw new ArgumentNullException("configFilePath", "The given configuration contains a null reference!");

			if (sessionHandler == null)
				throw new ArgumentNullException("sessionHandler", "The specified AJSToolsSessionHandler contains a null reference!");

			_handler = sessionHandler;
			_handler.OnWriteableCreate += new JSToolsWriteableCreateEvent(FireOnWriteableCreate);

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
		public void Render(RenderProcessTicket renderContext)
		{
			if (!IsXmlDocumentInitialized)
				throw new InvalidOperationException("The configuration XmlDocument is not specified!");

			if (renderContext == null)
				throw new ArgumentNullException("renderContext", "The given RenderProcessTicket object contains a null reference!");

			FirePreRenderEvent(renderContext);
			FireRenderEvent(renderContext);
		}


		/// <summary>
		/// Serializes the configuration into a xml document and saves it to the specified file path.
		/// Default encoding is UTF-8.
		/// </summary>
		/// <param name="filePath">File path where the serialized file should be placed in.</param>
		/// <exception cref="ArgumentNullException">The specified file path contains a null reference.</exception>
		/// <exception cref="ArgumentException">Could not write into the specified stream or the specified file path is invalid.</exception>
		/// <exception cref="InvalidOperationException">The configuration XmlDocument is not specified.</exception>
		/// <exception cref="IOException">An I/O error occurs.</exception>
		public void SaveConfiguration(string filePath)
		{
			SaveConfiguration(filePath, Encoding.UTF8);
		}


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
		public void SaveConfiguration(string filePath, Encoding encoding)
		{
			if (!IsXmlDocumentInitialized)
				throw new InvalidOperationException("The configuration XmlDocument is not specified!");

			if (filePath == null)
				throw new ArgumentNullException("filePath", "The specified file path contains a null reference!");

			if (encoding == null)
				throw new ArgumentNullException("encoding", "The given encoding contains a null reference!");

			FileStream file = null;

			try
			{
				try
				{
					file = new FileStream(filePath, FileMode.Truncate, FileAccess.Write);
				}
				catch (Exception e)
				{
					throw new ArgumentException("The specified file path is invalid! Error description: " + e.Message, "filePath");
				}

				SerializeConfiguration(file, encoding);
				file.Flush();
			}
			finally
			{
				if (file != null)
				{
					file.Close();
				}
			}
		}


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
		public void SaveConfiguration(Stream toWrite, Encoding encoding)
		{
			if (!IsXmlDocumentInitialized)
				throw new InvalidOperationException("The configuration XmlDocument is not specified!");

			if (toWrite == null)
				throw new ArgumentNullException("toWrite", "The specified stream contains a null reference!");

			if (!toWrite.CanWrite)
				throw new ArgumentException("Could not write into the given stream, it is write protected!");

			if (encoding == null)
				throw new ArgumentNullException("encoding", "The given encoding contains a null reference!");

			SerializeConfiguration(toWrite, encoding);
		}


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
		public void SaveConfiguration(Stream toWrite)
		{
			SerializeConfiguration(toWrite, Encoding.UTF8);
		}


		/// <summary>
		/// Returns the requested configuration section, specified in the JSTools configuration file.
		/// You will obtain a null reference, if no handler was found.
		/// </summary>
		/// <param name="configNodeName">Name of the section node.</param>
		/// <returns>Returns the requested configuration instance.</returns>
		/// <exception cref="ArgumentNullException">The specified name contains a null reference.</exception>
		public AJSToolsEventHandler GetConfig(string configNodeName)
		{
			if (configNodeName == null)
				throw new ArgumentNullException("configNodeName", "The specified name contains a null reference!");

			if (_configSections.Contains(configNodeName))
			{
				return (_configSections[configNodeName] as AJSToolsEventHandler);
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
				throw new ArgumentNullException("configDocument", "The given xml document contains a null reference!");

			AcquireWriterLock();

			try
			{
				if (_configInitialized)
					throw new InvalidOperationException("The configuration XmlDocument was already specified!");

				// import nodes
				foreach (XmlNode node in configDocument.ChildNodes)
				{
					XmlNode importedNode = _configDocument.ImportNode(node, true);
					_configDocument.AppendChild(importedNode);
				}
				InitConfiguration();
			}
			finally
			{
				ReleaseWriterLock();
			}
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
				throw new ArgumentNullException("configDocument", "The given xml document contains a null reference!");

			AcquireWriterLock();
			
			try
			{
				if (_configInitialized)
					throw new InvalidOperationException("The configuration XmlDocument was already specified!");

				_configDocument.Load(configDocument);
				InitConfiguration();
			}
			finally
			{
				ReleaseWriterLock();
			}
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
				throw new ArgumentNullException("configDocument", "The given xml document contains a null reference!");

			AcquireWriterLock();

			try
			{
				if (_configInitialized)
					throw new InvalidOperationException("The configuration XmlDocument was already specified!");

				_configDocument.Load(configDocument);
				InitConfiguration();
			}
			finally
			{
				ReleaseWriterLock();
			}
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
				throw new ArgumentNullException("configDocument", "The given xml document contains a null reference!");

			AcquireWriterLock();

			try
			{
				if (_configInitialized)
					throw new InvalidOperationException("The configuration XmlDocument was already specified!");

				_configDocument.Load(configDocument);
				InitConfiguration();
			}
			finally
			{
				ReleaseWriterLock();
			}
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
				throw new ArgumentNullException("configDocument", "The given xml document contains a null reference!");

			AcquireWriterLock();

			try
			{
				if (_configInitialized)
					throw new InvalidOperationException("The configuration XmlDocument was already specified!");

				_configDocument.Load(configDocument);
				InitConfiguration();
			}
			finally
			{
				ReleaseWriterLock();
			}
		}


		/// <summary>
		/// Factory method, creates a new instance of the given section handler. If you'd like to create a
		/// writeable handler instance, you have to override this method and call the CreateHandler method.
		/// </summary>
		/// <param name="sectionHandler">Section handler that creates the configuration handler instance.</param>
		/// <param name="section">Contains the configuration XmlNode.</param>
		/// <returns>Returns the created instance.</returns>
		protected abstract AJSToolsEventHandler CreateSectionInstance(AJSToolsConfigSectionHandlerFactory sectionHandler, XmlNode section);


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
					throw new ConfigurationException("Could not identify the 'type' attribute of the configuration node '" + configNode.Name + "'!", configNode);
				}
				InitNodeAndCheckForExceptions(configNode);
			}

			InitDefaultSections();
			_configInitialized = true;
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
				throw new InvalidOperationException("A configuration section with the name '" + configNode.Name + "' already exists!");

			AJSToolsConfigSectionHandlerFactory handlerInstance = GetInstanceOfType(configNode.Attributes[TYPE_ATTRIB].Value);

			if (handlerInstance == null)
			{
				throw new InvalidOperationException("The specified type is not derived from AJSToolsConfigSectionHandler!");
			}
			if (configNode.Name != handlerInstance.SectionName)
			{
				throw new InvalidOperationException("The given node has not the same section name as specified in the configuration section handler!");
			}

			// create instance of section handler
			AJSToolsEventHandler sectionInstance = CreateSectionInstance(handlerInstance, configNode);

			// set parent element
			sectionInstance.SetParent(this);

			// append created section instance to configuration section array
			_configSections.Add(configNode.Name, sectionInstance);
		}


		/// <summary>
		/// Initilizes the default configuration sections.
		/// </summary>
		private void InitDefaultSections()
		{
			_exceptionHandling = (GetConfig(EXCEPTION_NODE) as AJSExceptionHandler);
			_scriptFileHandler = (GetConfig(SCRIPTS_NODE) as AJSScriptFileHandler);

			if (_exceptionHandling == null || _scriptFileHandler == null)
			{
				throw new ConfigurationException("The configuration file does not contain a '" + EXCEPTION_NODE + "' or '" + SCRIPTS_NODE + "' section!");
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
				throw new NullReferenceException("The given node contains an invalid type definition!");
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


		/// <summary>
		/// Serializes this configuraiton.
		/// </summary>
		/// <param name="stream">Stream in which the configuration will be saved.</param>
		private void SerializeConfiguration(Stream stream, Encoding encoding)
		{
			XmlDocument newDocument = new XmlDocument();

			// clone dtd, xml-declaration, ... nodes of the old xml document
			foreach (XmlNode node in _configDocument)
			{
				if (node != _configDocument.DocumentElement)
				{
					XmlNode importedNode = newDocument.ImportNode(node, true);

					if ((importedNode as XmlDeclaration) != null)
					{
						(importedNode as XmlDeclaration).Encoding = encoding.WebName;
					}
					newDocument.AppendChild(importedNode);
				}
			}

			XmlNode documentElement = newDocument.CreateElement(CONFIG_NODE_NAME);
			newDocument.AppendChild(documentElement);

			FireSerializeEvent(newDocument.DocumentElement, true);
			WriteToStream(stream, encoding, newDocument.OuterXml);
		}


		/// <summary>
		/// Writes the specified string into the given stream. The encoding format is UTF-8.
		/// </summary>
		/// <param name="stream">Stream in which the string will be written in.</param>
		/// <param name="toWrite">String to write.</param>
		private void WriteToStream(Stream stream, Encoding encoding, string toWrite)
		{
			Byte[] bytes = encoding.GetBytes(toWrite);
			stream.Write(bytes, 0, bytes.Length);
			stream.Flush();
		}


		/// <summary>
		/// Fires the OnWriteableCreate event, which was created by the session handler.
		/// </summary>
		/// <param name="writeableInstance">Writeable instance.</param>
		private void FireOnWriteableCreate(IJSToolsConfiguration writeableInstance)
		{
			if (OnWriteableCreate != null)
			{
				OnWriteableCreate(writeableInstance);
			}
		}
	}
}

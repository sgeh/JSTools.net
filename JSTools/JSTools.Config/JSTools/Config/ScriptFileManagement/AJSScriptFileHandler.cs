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
using System.Text;
using System.Xml;

using JSTools.Config.Session;
using JSTools.Xml;

namespace JSTools.Config.ScriptFileManagement
{
	/// <summary>
	/// Represents an instance of the &lt;scripts&gt; configuration section in the JSTools.net configuration.
	/// </summary>
	public abstract class AJSScriptFileHandler : AFileManagementSection, IWriteable
	{
		//------------------------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------------------------

		public	readonly	string						SCRIPTS_NODE_NAME;

		protected			bool						_debug					= false;
		protected			double						_scriptVersion			= 1.0;
		protected			string						_scriptType				= "";
		protected			string						_debugScriptSource		= "";
		protected			string						_releaseScriptSource	= "";
		protected			AJSToolsConfiguration		_ownerConfig			= null;

		private	const		string						TYPE_ATTRIB				= "type";
		private	const		string						SCRIPT_VERSION_ATTRIB	= "version";
		private	const		string						SCRIPT_TYPE_ATTRIB		= "language";
		private	const		string						DEBUG_ATTRIB			= "debug";
		private	const		string						DEBUG_SOURCE_ATTRIB		= "debugSource";
		private	const		string						RELEASE_SOURCE_ATTRIB	= "releaseSource";

		private const		string						COMMENT_BEGIN			= "\n" + AJSToolsConfiguration.COMMENT_BEGIN + "\n";
		private const		string						COMMENT_END				= "\n//" + AJSToolsConfiguration.COMMENT_END + "\n";

		private const		string						SCRIPT_END				= COMMENT_END + "</script>";
		private const		string						SCRIPT_BEGIN			= "<script language=\"{0}\" type=\"text/{1}\">" + COMMENT_BEGIN;
		private const		string						SCRIPT_FILE				= "<script language=\"{0}\" type=\"text/{1}\" src=\"{2}\"></script>";
		private const		string						SCRIPT_FILE_FALLBACK	= "<script language=\"{0}\" type=\"text/{1}\" src=\"{2}\">{3}</script>";
		private	const		string						DEBUG_MESSAGE			= AJSToolsConfiguration.COMMENT_BEGIN + " CAUTION: DEBUG MODE IS ACTIVE " + AJSToolsConfiguration.COMMENT_END + "\n\n";

		private				AJSModuleContainer			_childModules			= null;
		private				XmlDocument					_configDocument			= null;
		private				XmlNode						_configSection			= null;


		/// <summary>
		/// Returns a writeable instance of this object.
		/// </summary>
		AJSToolsEventHandler IWriteable.WriteableInstance
		{
			get { return WriteableInstance; }
		}


		/// <summary>
		/// Gets/Sets the scripts, which are defined in the configuration document.
		/// True means the debug script will be used, false means, the release scripts will be used.
		/// </summary>
		public abstract bool Debug
		{
			get;
			set;
		}


		/// <summary>
		/// Gets/Sets the type of the scripts e.g. "JavaScript".
		/// </summary>
		/// <exception cref="ArgumentNullException">The specified string contains a null reference.</exception>
		public abstract string ScriptType
		{
			get;
			set;
		}


		/// <summary>
		/// Gets/Sets the type of the scripts e.g. 1.3
		/// </summary>
		public abstract double ScriptVersion
		{
			get;
			set;
		}


		/// <summary>
		/// Gets/Sets the url, where the debug scripts are stored.
		/// </summary>
		/// <exception cref="ArgumentNullException">The specified string contains a null reference.</exception>
		public abstract string DebugScriptSource
		{
			get;
			set;
		}


		/// <summary>
		/// Gets/Sets the url, where the debug scripts are stored.
		/// </summary>
		/// <exception cref="ArgumentNullException">The specified string contains a null reference.</exception>
		public abstract string ReleaseScriptSource
		{
			get;
			set;
		}


		/// <summary>
		/// Returns all child modules.
		/// </summary>
		public AJSModuleContainer ChildModules
		{
			get { return _childModules; }
		}


		/// <summary>
		/// Returns the script source folder for the debug or release mode.
		/// </summary>
		public string ScriptSourceFolder
		{
			get { return (_debug ? _debugScriptSource : _releaseScriptSource); }
		}


		/// <summary>
		/// Gets the name of the representing xml node.
		/// </summary>
		public string SectionName
		{
			get { return SCRIPTS_NODE_NAME; }
		}


		/// <summary>
		/// Returns a writeable instance of this object.
		/// </summary>
		public AJSScriptFileHandler WriteableInstance
		{
			get { return _ownerConfig.WriteableInstance.ScriptFileHandler; }
		}


		/// <summary>
		/// Returns the configuration instance which has created this FileHandler.
		/// </summary>
		public override IJSToolsConfiguration OwnerConfiguration
		{
			get { return _ownerConfig; }
		}


		/// <summary>
		/// Returns the configuration section handler, which contains this module.
		/// </summary>
		public override AJSToolsEventHandler OwnerSection
		{
			get { return null; }
		}


		/// <summary>
		/// Returns the XmlDocument, which was used to initialize the JSTools configuration.
		/// </summary>
		internal XmlDocument OwnerConfigurationDocument
		{
			get { return _ownerConfig.ConfigDocument; }
		}


		//------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Creates a new AJSScriptFileHandler instance.
		/// </summary>
		/// <param name="section">Xml section.</param>
		/// <param name="nodeName">Contains the name of the representing node.</param>
		/// <exception cref="ArgumentNullException">An argument contains a null reference.</exception>
		public AJSScriptFileHandler(XmlNode section, string nodeName)
		{
			if (section == null)
				throw new ArgumentNullException("section", "The given xml section contains a null reference!");

			if (nodeName == null)
				throw new ArgumentNullException("nodeName", "The given node name contains a null reference!");

			OnInit += new JSToolsInitEvent(OnParentInit);

			SCRIPTS_NODE_NAME	= nodeName;
			_configSection		= section;
			_configDocument		= _configSection.OwnerDocument;
			_childModules		= CreateModuleContainer();
		}


		/// <summary>
		/// Send last remove event.
		/// </summary>
		~AJSScriptFileHandler()
		{
			if (_remove != null)
			{
				_remove(this);
			}
		}


		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Checks, if a module is registered.
		/// </summary>
		/// <param name="toCheck">JSModule you'd like to check.</param>
		/// <returns>Returns true, if the given JSModule is registered.</returns>
		public bool IsModuleRegistered(AJSModule toCheck)
		{
			return (toCheck != null && IsModuleRegistered(toCheck.FullName));
		}


		/// <summary>
		/// Checks, if a module is registered.
		/// </summary>
		/// <param name="fullModuleName">Name of the JSModule you'd like to check.</param>
		/// <returns>Returns true, if the given JSModule is registered.</returns>
		public bool IsModuleRegistered(string fullModuleName)
		{
			return (GetModule(fullModuleName) != null);
		}


		/// <summary>
		/// Checks, if a script file is registered.
		/// </summary>
		/// <param name="toCheck">JSModule you'd like to check.</param>
		/// <returns>Returns true, if the given JSScript is registered.</returns>
		public bool IsScriptRegistered(AJSScript toCheck)
		{
			return (toCheck != null && IsScriptRegistered(toCheck.Path));
		}


		/// <summary>
		/// Checks, if a script file is registered.
		/// </summary>
		/// <param name="scriptPath">Name of the JSScript you'd like to check.</param>
		/// <returns>Returns true, if the given JSScript is registered.</returns>
		public bool IsScriptRegistered(string scriptPath)
		{
			return (GetScript(scriptPath) != null);
		}


		/// <summary>
		/// Searches in the configuration document for a module with the given name.
		/// </summary>
		/// <param name="fullModuleName">Specifies the name of the module.</param>
		/// <returns>Returns a null reference, if no module is found, otherwise this method will
		/// return the expected AJSModule instance.</returns>
		public AJSModule GetModule(string fullModuleName)
		{
			if (fullModuleName != null)
			{
				return GetModule(fullModuleName.Split(AJSModule.NAME_SEPARATOR));
			}
			return null;
		}


		/// <summary>
		/// Searches in the configuration document for a module with the given name.
		/// </summary>
		/// <param name="moduleNames">Array of module names, which determines the excepted module.</param>
		/// <returns>Returns a null reference, if no module is found, otherwise this method will
		/// return the expected AJSModule instance.</returns>
		/// <exception cref="ArgumentNullException">The specified module name collection contains a null reference.</exception>
		public AJSModule GetModule(string[] moduleNames)
		{
			if (moduleNames == null)
				throw new ArgumentNullException("moduleNames", "The specified module name collection contains a null reference!");

			AJSModuleContainer resultList = ChildModules;

			foreach (string moduleName in moduleNames)
			{
				try
				{
					resultList = resultList[moduleName].ChildModules;
				}
				catch (ArgumentException)
				{
					// module does not exist
					return null;
				}
				catch (NullReferenceException)
				{
					// module does not exist
					return null;
				}
			}
			return (resultList.ParentSection as AJSModule);
		}


		/// <summary>
		/// Searches in the configuration document for a script file with the given path.
		/// </summary>
		/// <param name="scriptPath">Specifies the path of the script.</param>
		/// <returns>Returns a null reference, if no script file is found, otherwise this method will
		/// return the expected AJSScript instance.</returns>
		public AJSScript GetScript(string scriptPath)
		{
			if (scriptPath != null && scriptPath != String.Empty)
			{
				int splitIndex = scriptPath.LastIndexOf(AJSModule.PATH_SEPARATOR);

				if (splitIndex == -1)
					return null;

				string scriptName = scriptPath.Remove(0, splitIndex + 1);
				string moduleName = scriptPath.Remove(scriptPath.Length - splitIndex, splitIndex);

				AJSModule module = GetModule(moduleName.Split(AJSModule.PATH_SEPARATOR));

				if (module != null)
				{
					return module.ScriptFiles[scriptName];
				}
			}
			return null;
		}


		/// <summary>
		/// Creates a copy of the specified module and assigns the created copy to the current
		/// configuration section. The resulting instance can be added to this configuration instance.
		/// </summary>
		/// <param name="toRegister">JSModule to register.</param>
		/// <returns>Returns the created module.</returns>
		/// <exception cref="ArgumentException">The given AJSModule has an invalid reference or contains an invalid name.</exception>
		/// <exception cref="InvalidOperationException">The specified module is already imported.</exception>
		public AJSModule ImportModule(AJSModule toRegister)
		{
			if (toRegister == null || toRegister.Name == String.Empty)
				throw new ArgumentException("The given AJSModule has an invalid reference or contains an invalid name!", "toRegister");

			if (toRegister.OwnerSection == this)
				throw new InvalidOperationException("The specified module is already imported!");

			return toRegister.Clone(this, true);
		}


		/// <summary>
		/// Creates a copy of the specified script file and assigns the created copy to the current
		/// configuration section. The resulting instance can be added to this configuration instance.
		/// </summary>
		/// <param name="fileScript">JSScript to register.</param>
		/// <returns>Returns the created file.</returns>
		/// <exception cref="ArgumentException">The given AJSScript has an invalid reference or contains an invalid path.</exception>
		/// <exception cref="InvalidOperationException">The specified script is already imported.</exception>
		public AJSScript ImportScript(AJSScript toRegister)
		{
			if (toRegister == null || toRegister.Path == String.Empty)
				throw new ArgumentException("The given AJSScript has an invalid reference or contains an invalid name!", "toRegister");

			if (toRegister.OwnerSection == this)
				throw new InvalidOperationException("The specified script is already imported!");

			return toRegister.Clone(this);
		}


		/// <summary>
		/// Returns a script begin tag which contains the specified file path attribute.
		/// </summary>
		/// <param name="filePath">This path will be written into the value of the 'src' attribute.</param>
		/// <returns>Returns the generated string.</returns>
		public string GetScriptFileTag(string filePath)
		{
			return String.Format(SCRIPT_FILE, GetScriptLanguageString(), _scriptType, filePath);
		}


		/// <summary>
		/// Returns a script begin tag which contains the specified file path attribute.
		/// </summary>
		/// <param name="filePath">This path will be written into the value of the 'src' attribute.</param>
		/// <param name="commentCode">Comment to descript the functionality, which is written in the file.</param>
		/// <returns>Returns the generated string.</returns>
		public string GetScriptFileTag(string filePath, string commentCode)
		{
			return String.Format(SCRIPT_FILE_FALLBACK, GetScriptLanguageString(), _scriptType, filePath, COMMENT_BEGIN + commentCode + COMMENT_END);
		}


		/// <summary>
		/// Returns a script begin tag without a source attribute.
		/// </summary>
		/// <returns>Returns the generated string.</returns>
		public string GetScriptBeginTag()
		{
			return String.Format(SCRIPT_BEGIN, GetScriptLanguageString(), _scriptType);
		}


		/// <summary>
		/// Returns a script end tag.
		/// </summary>
		/// <returns>Returns the generated string.</returns>
		public string GetScriptEndTag()
		{
			return SCRIPT_END;
		}


		/// <summary>
		/// Creates a new child module object.
		/// </summary>
		/// <param name="moduleName">Module name you'd like to create.</param>
		/// <returns>Returns the created instance.</returns>
		/// <exception cref="ArgumentNullException">The specified name contains a null reference.</exception>
		public AJSModule CreateModule(string moduleName)
		{
			if (moduleName == null)
				throw new ArgumentNullException("moduleName", "The specified name contains a null reference!");

			return new JSModuleWriteable(moduleName, this);
		}


		/// <summary>
		/// Creates a new child module object.
		/// </summary>
		/// <param name="moduleNode">Node, which contains the module informations.</param>
		/// <returns>Returns the created instance.</returns>
		/// <exception cref="ArgumentNullException">The specified node contains a null reference.</exception>
		public AJSModule CreateModule(XmlNode moduleNode)
		{
			if (moduleNode == null)
				throw new ArgumentNullException("moduleNode", "The specified node contains a null reference!");

			return new JSModuleWriteable(moduleNode, this);
		}


		/// <summary>
		/// Creates a new script file object.
		/// </summary>
		/// <param name="folder">Folder, in which the new script file is stored.</param>
		/// <param name="fileNode">Node, which contains the folder and file informations.</param>
		/// <returns>Returns the created file instance.</returns>
		/// <exception cref="ArgumentException">A AJSScript with the given path exists already in this configuration.</exception>
		public AJSScript CreateScriptFile(string folder, XmlNode fileNode)
		{
			return new JSScriptWriteable(folder, fileNode, this);
		}


		/// <summary>
		/// Creates a new script file object.
		/// </summary>
		/// <param name="fileNode">Node, which contains the folder and file informations.</param>
		/// <returns>Returns the created file instance.</returns>
		/// <exception cref="ArgumentException">A AJSScript with the given path exists already in this configuration.</exception>
		public AJSScript CreateScriptFile(XmlNode fileNode)
		{
			return new JSScriptWriteable(fileNode, this);
		}


		/// <summary>
		/// Creates a new immutalbe child module object.
		/// </summary>
		/// <param name="moduleNode">Node, which contains the module informations.</param>
		/// <returns>Returns the created instance.</returns>
		/// <exception cref="ArgumentNullException">The specified node contains a null reference.</exception>
		internal AJSModule CreateInnerModule(XmlNode moduleNode)
		{
			if (moduleNode == null)
				throw new ArgumentNullException("moduleNode", "The specified node contains a null reference!");

			return new JSModule(moduleNode, this);
		}


		/// <summary>
		/// Creates a new immutalbe script file object.
		/// </summary>
		/// <param name="folder">Folder, in which the new script file is stored.</param>
		/// <param name="fileNode">Node, which contains the folder and file informations.</param>
		/// <returns>Returns the created file instance.</returns>
		/// <exception cref="ArgumentException">A AJSScript with the given path exists already in this configuration.</exception>
		internal AJSScript CreateInnerScriptFile(string folder, XmlNode fileNode)
		{
			return new JSScript(folder, fileNode, this);
		}


		/// <summary>
		/// Creates a new AJSModuleContainer instance for internal use.
		/// </summary>
		/// <returns>Returns the created AJSModuleContainer.</returns>
		protected abstract AJSModuleContainer CreateModuleContainer();


		/// <summary>
		/// Renders the JavaScript configuration of the current section into the given StringBuilder.
		/// </summary>
		/// <param name="renderContext"></param>
		/// <exception cref="ArgumentNullException">The specified RenderProcessTicket contains a null reference.</exception>
		protected override void RenderScriptConfiguration(RenderProcessTicket renderContext)
		{
			if (renderContext == null)
				throw new ArgumentNullException("renderContext", "The specified RenderProcessTicket contains a null reference!");

			if (_debug)
			{
				renderContext.Write(DEBUG_MESSAGE);
			}

			// call base function to enable event bubbling
			base.RenderScriptConfiguration(renderContext);
		}


		/// <summary>
		/// This method will be called, if the parent element has fired the OnSerialize event. Renders the configuration
		/// settings of the current section into the given XmlNode instance. The current instance will not fire an event,
		/// if the deep flag is set to false.
		/// </summary>
		/// <param name="parentNode">Parent xml node instance. You have to create a new xml node instance and append it
		/// to the parent node.</param>
		/// <param name="deep">True to copy all sub elements, otherwise only the settings of the current node will be
		///  copied.</param>
		protected override void SerializeXmlConfiguration(XmlNode parentNode, bool deep)
		{
			// get type name
			Type factoryType = typeof(JSScriptFileSectionHandlerFactory);
			string assemblyName = factoryType.Assembly.FullName.Substring(0, factoryType.Assembly.FullName.IndexOf(","));
			string typeDefinition = factoryType.FullName + ", " + assemblyName;

			// create new exception node, fill in type name
			XmlNode sectionNode = parentNode.OwnerDocument.CreateElement(SectionName);
			JSToolsXmlFunctions.AppendAttributeToNode(sectionNode, TYPE_ATTRIB, typeDefinition);

			// fill in values
			JSToolsXmlFunctions.AppendAttributeToNode(sectionNode, DEBUG_SOURCE_ATTRIB, _debugScriptSource);
			JSToolsXmlFunctions.AppendAttributeToNode(sectionNode, RELEASE_SOURCE_ATTRIB, _releaseScriptSource);
			JSToolsXmlFunctions.AppendAttributeToNode(sectionNode, DEBUG_ATTRIB, _debug.ToString().ToLower());
			JSToolsXmlFunctions.AppendAttributeToNode(sectionNode, SCRIPT_TYPE_ATTRIB, _scriptType);
			JSToolsXmlFunctions.AppendAttributeToNode(sectionNode, SCRIPT_VERSION_ATTRIB, _scriptVersion.ToString());

			// append exception node to parent node
			parentNode.AppendChild(sectionNode);

			if (deep)
			{
				// call base function to enable event bubbling
				base.SerializeXmlConfiguration(sectionNode, deep);
			}
		}


		/// <summary>
		/// Initializes the values of this configuration object.
		/// </summary>
		private void InitConfiguration()
		{
			_debugScriptSource		= GetValidPath(JSToolsXmlFunctions.GetAttributeFromNode(_configSection, DEBUG_SOURCE_ATTRIB));
			_releaseScriptSource	= GetValidPath(JSToolsXmlFunctions.GetAttributeFromNode(_configSection, RELEASE_SOURCE_ATTRIB));

			InitScriptNode();
			InitModuleNodes();
		}


		/// <summary>
		/// Returns the specified path with a valid termination.
		/// </summary>
		/// <param name="path">Path which should be evaluated.</param>
		/// <returns>Returns a string.</returns>
		private string GetValidPath(string path)
		{
			if (path.EndsWith(AJSModule.PATH_SEPARATOR.ToString()))
			{
				return path.Remove(path.Length - 1, 1);
			}
			else
			{
				return path;
			}
		}


		/// <summary>
		/// Initializes the values of the <scripts> node.
		/// </summary>
		private void InitScriptNode()
		{
			_debug = JSToolsXmlFunctions.GetBoolFromNodeValue(_configSection.Attributes[DEBUG_ATTRIB]);
			_scriptType = JSToolsXmlFunctions.GetValueFromNode(_configSection.Attributes[SCRIPT_TYPE_ATTRIB]);

			string scriptVersion = JSToolsXmlFunctions.GetValueFromNode(_configSection.Attributes[SCRIPT_VERSION_ATTRIB]);

			try
			{
				_scriptVersion = Convert.ToDouble(scriptVersion);
			}
			catch
			{
				// ignore value if an exception occurs
			}
		}


		/// <summary>
		/// Initializes the module nodes.
		/// </summary>
		private void InitModuleNodes()
		{
			XmlNodeList moduleNodes = _configSection.SelectNodes(AJSModule.MODULE_NODE_NAME);

			foreach (XmlNode moduleNode in moduleNodes)
			{
				_childModules.AppendInnerModule(moduleNode);
			}
		}


		/// <summary>
		/// Appends the script type to the script version. (e.g. JavaScript1.3)
		/// </summary>
		/// <returns>Returns the created string.</returns>
		private string GetScriptLanguageString()
		{
			return _scriptType + _scriptVersion;
		}


		/// <summary>
		/// Initializes the parent instance.
		/// </summary>
		/// <param name="sender">This objcect.</param>
		/// <param name="newParent">New parent object.</param>
		/// <exception cref="ArgumentException">The given parent is not a valid AJSToolsConfiguration instance.</exception>
		private void OnParentInit(AJSToolsEventHandler sender, AJSToolsEventHandler newParent)
		{
			if (sender != this)
				throw new InvalidOperationException("Invalid sender specified!");

			if ((newParent as AJSToolsConfiguration) == null)
				throw new ArgumentException("The given parent is not a valid AJSToolsConfiguration instance!", "sender");

			_ownerConfig = (newParent as AJSToolsConfiguration);

			InitConfiguration();

			if (_load != null)
			{
				_load(this);
			}
		}
	}
}

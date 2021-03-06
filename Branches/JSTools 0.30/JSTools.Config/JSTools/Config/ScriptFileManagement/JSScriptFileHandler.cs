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

using JSTools.Xml;

namespace JSTools.Config.ScriptFileManagement
{
	/// <summary>
	/// Specifies if the script should be crunched and additional debug informations should
	/// be rendered.
	/// </summary>
	public enum DebugMode
	{
		/// <summary>
		/// The source files of a module will be crunched and written into one file which is
		/// renderd to the client.
		/// </summary>
		None,

		/// <summary>
		///  The whole source files of a module will be written into one file which is rendered
		///  to the client.
		/// </summary>
		Module,

		/// <summary>
		/// The whole source files will be rendered to the client. This feature is not supported
		/// by netscape 4.x. Use module instead.
		/// </summary>
		File
	}


	/// <summary>
	/// Represents an instance of the &lt;scripts&gt; configuration section in the JSTools.net configuration.
	/// </summary>
	public class JSScriptFileHandler : AJSToolsScriptFileSection
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		public	readonly	string						SCRIPTS_NODE_NAME;
		public	const		char						PATH_SEPARATOR			= '/';
		public	const		char						NAME_SEPARATOR			= '.';

		protected			DebugMode					_debugMode				= DebugMode.Module;
		protected			float						_scriptVersion			= 1.0F;
		protected			string						_scriptType				= string.Empty;
		protected			string						_contentType			= string.Empty;
		protected			string						_source					= string.Empty;
		protected			string						_extension				= ".js";
		protected			JSModuleContainer			_childModules			= null;

		private	const		string						TYPE_ATTRIB				= "type";
		private	const		string						SCRIPT_VERSION_ATTRIB	= "version";
		private	const		string						SCRIPT_TYPE_ATTRIB		= "language";
		private	const		string						CONTENT_TYPE_ATTRIB		= "contentType";
		private	const		string						DEBUG_ATTRIB			= "debug";
		private	const		string						SOURCE_ATTRIB			= "src";
		private	const		string						EXTENSION_ATTRIB		= "extension";

		private const		string						COMMENT_BEGIN			= "\n" + JSToolsConfiguration.COMMENT_BEGIN;
		private const		string						COMMENT_END				= "\n//" + JSToolsConfiguration.COMMENT_END;

		private const		string						SCRIPT_END				= COMMENT_END + "\n</script>";
		private const		string						SCRIPT_BEGIN			= "<script language=\"{0}\" type=\"text/{1}\">" + COMMENT_BEGIN;
		private const		string						SCRIPT_FILE				= "<script language=\"{0}\" type=\"text/{1}\" src=\"{2}\">{3}</script>";
		private	const		string						SCRIPT_FILE_DEBUG		= "'<SCRIPT LANGUAGE=\"{0}\" TYPE=\"text/{1}\" SRC=\"{2}\"><\\/SCRIPT>'";

		private				XmlNode						_configSection			= null;


		/// <summary>
		/// Returns the child modules of this module container.
		/// </summary>
		public JSModuleContainer ChildModules 
		{
			get { return _childModules; }
		}


		/// <summary>
		/// Specifies if the script should be crunched and additional debug informations should
		/// be rendered.
		/// </summary>
		public DebugMode DebugMode
		{
			get { return _debugMode; }
		}


		/// <summary>
		/// Gets the type of the scripts e.g. "javascript".
		/// </summary>
		public string ScriptType
		{
			get { return _scriptType; }
		}


		/// <summary>
		/// Gets the content type of the scripts e.g. "text/javascript". This content type is used by
		/// rendering the crunched or generated scripts.
		/// </summary>
		public string ContentType
		{
			get { return _contentType; }
		}


		/// <summary>
		/// Gets the version of the scripts e.g. 1.3 .
		/// </summary>
		public float ScriptVersion
		{
			get { return _scriptVersion; }
		}


		/// <summary>
		/// Gets the type of the scripts e.g. "JavaScript1.3".
		/// </summary>
		public string ScriptLanguage
		{
			get { return ScriptType + ScriptVersion; }
		}


		/// <summary>
		/// Gets the url, where the debug scripts are stored.
		/// </summary>
		public string Source
		{
			get { return _source; }
		}


		/// <summary>
		/// Gets the file extension, which will be used for rendering modules and files. (e.g. ".js")
		/// </summary>
		public string ScriptExtension
		{
			get { return _extension; }
		}


		/// <summary>
		/// Gets the name of the representing xml node.
		/// </summary>
		public string SectionName
		{
			get { return SCRIPTS_NODE_NAME; }
		}


		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSScriptFileHandler instance.
		/// </summary>
		/// <param name="section">Xml section.</param>
		/// <param name="ownerConfig">Owner (parent) configuration instance.</param>
		/// <param name="nodeName">Contains the name of the representing node.</param>
		/// <exception cref="ArgumentNullException">An argument contains a null reference.</exception>
		/// <exception cref="InvalidOperationException">A required module could not be found.</exception>
		public JSScriptFileHandler(XmlNode section, IJSToolsConfiguration ownerConfig, string nodeName) : base(ownerConfig)
		{
			if (section == null)
				throw new ArgumentNullException("section", "The given xml section contains a null reference!");

			if (nodeName == null)
				throw new ArgumentNullException("nodeName", "The given node name contains a null reference!");

			SCRIPTS_NODE_NAME	= nodeName;
			_configSection		= section;

			InitConfiguration();
		}


		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Checks, if a module is registered.
		/// </summary>
		/// <param name="toCheck">JSModule you'd like to check.</param>
		/// <returns>Returns true, if the given JSModule is registered.</returns>
		public bool IsModuleRegistered(JSModule toCheck)
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
			return (GetModuleByName(fullModuleName) != null);
		}


		/// <summary>
		/// Checks, if a script file is registered.
		/// </summary>
		/// <param name="toCheck">JSModule you'd like to check.</param>
		/// <returns>Returns true, if the given JSScript is registered.</returns>
		public bool IsScriptRegistered(JSScript toCheck)
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
		/// Searches for a section instance with the given path.
		/// </summary>
		/// <param name="sectionPath">Section to serach.</param>
		/// <returns>Returns a section instance, which is located at the given path. You will obtain
		/// a null reference, if there is no section found.</returns>
		public AJSToolsScriptFileSection GetSection(string sectionPath)
		{
			AJSToolsScriptFileSection section = GetScript(sectionPath);

			if (section == null)
			{
				section = GetModuleByPath(sectionPath);
			}
			return section; 
		}


		/// <summary>
		/// Searches for a section instance with the given path.
		/// </summary>
		/// <param name="sectionPath">Section to serach.</param>
		/// <returns>Returns true, if there is a section with the given path.</returns>
		public bool HasSection(string sectionPath)
		{
			return (GetSection(sectionPath) != null);
		}


		/// <summary>
		/// Searches in the configuration document for a module with the given name (e.g. JSTools.Web).
		/// </summary>
		/// <param name="fullModuleName">Specifies the name of the module.</param>
		/// <returns>Returns a null reference, if no module is found, otherwise this method will
		/// return the expected JSModule instance.</returns>
		public JSModule GetModuleByPath(string fullModuleName)
		{
			if (fullModuleName != null)
			{
				return GetModule(fullModuleName.Split(PATH_SEPARATOR));
			}
			return null;
		}


		/// <summary>
		/// Searches in the configuration document for a module with the given path (e.g. JSTools/Web).
		/// </summary>
		/// <param name="fullModuleName">Specifies the name of the module.</param>
		/// <returns>Returns a null reference, if no module is found, otherwise this method will
		/// return the expected JSModule instance.</returns>
		public JSModule GetModuleByName(string fullModuleName)
		{
			if (fullModuleName != null)
			{
				return GetModule(fullModuleName.Split(NAME_SEPARATOR));
			}
			return null;
		}


		/// <summary>
		/// Searches in the configuration document for a module with the given name.
		/// </summary>
		/// <param name="moduleNames">Array of module names, which determines the excepted module.</param>
		/// <returns>Returns a null reference, if no module is found, otherwise this method will
		/// return the expected JSModule instance.</returns>
		/// <exception cref="ArgumentNullException">vThe specified module name collection contains an invalid value.</exception>
		public JSModule GetModule(string[] moduleNames)
		{
			if (moduleNames == null || moduleNames.Length == 0)
				throw new ArgumentNullException("moduleNames", "The specified module name collection contains an invalid value!");

			if (moduleNames.Length == 1)
				return ChildModules[moduleNames[0]];

			try
			{
				return GetChildModuleByName(ChildModules[moduleNames[0]], moduleNames);
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


		/// <summary>
		/// Searches in the configuration document for a script file with the given
		/// path (e.g. JSTools/Web/ImageHandler).
		/// </summary>
		/// <param name="scriptPath">Specifies the path of the script.</param>
		/// <returns>Returns a null reference, if no script file is found, otherwise this method will
		/// return the expected AJSScript instance.</returns>
		public JSScript GetScript(string scriptPath)
		{
			if (scriptPath != null && scriptPath != String.Empty)
			{
				int splitIndex = scriptPath.LastIndexOf(PATH_SEPARATOR);

				if (splitIndex == -1)
					return null;

				string scriptName = scriptPath.Remove(0, splitIndex + 1);
				string moduleName = scriptPath.Substring(0, splitIndex);

				JSModule module = GetModuleByPath(moduleName);

				if (module != null)
				{
					return module.ScriptFiles[scriptName];
				}
			}
			return null;
		}

		/// <summary>
		/// Returns a script tag which contains the specified file path attribute. The returning string
		/// can be used in conjunction with document.write() method.
		/// </summary>
		/// <param name="script">Script file object to get tag.</param>
		/// <returns>Returns the generated string.</returns>
		/// <exception cref="ArgumentNullException">The given script contains a null reference.</exception>
		/// <exception cref="ArgumentNullException">The given path prefix contains a null reference.</exception>
		public string GetScriptFileJavaScriptTag(JSScript script, string pathPrefix)
		{
			if (script == null)
				throw new ArgumentNullException("script", "The given script contains a null reference!");

			if (pathPrefix == null)
				throw new ArgumentNullException("pathPrefix", "The given path prefix contains a null reference!");

			return String.Format(SCRIPT_FILE_DEBUG,
				GetScriptLanguageString(_scriptType, _scriptVersion),
				_scriptType,
				CombinePathPrefix(pathPrefix, script.RequestPath));
		}


		/// <summary>
		/// Returns a script tag which contains the specified file path attribute.
		/// </summary>
		/// <param name="filePath">This path will be written into the value of the 'src' attribute.</param>
		/// <returns>Returns the generated string.</returns>
		public string GetScriptFileTag(string filePath)
		{
			return String.Format(SCRIPT_FILE,
				GetScriptLanguageString(_scriptType, _scriptVersion),
				_scriptType,
				filePath,
				String.Empty);
		}


		/// <summary>
		/// Returns a script tag which contains the specified file path attribute.
		/// </summary>
		/// <param name="filePath">This path will be written into the value of the 'src' attribute.</param>
		/// <param name="scriptType">Script type (e.g. JavaScript / VBScript)</param>
		/// <param name="scriptVersion">Script version (e.g. 1.2 / 1.5)</param>
		/// <returns>Returns the generated string.</returns>
		public string GetScriptFileTag(string filePath, string scriptType, float scriptVersion)
		{
			return String.Format(SCRIPT_FILE,
				GetScriptLanguageString(scriptType, scriptVersion),
				_scriptType,
				filePath,
				String.Empty);
		}


		/// <summary>
		/// Returns a script tag which contains the specified file path attribute.
		/// </summary>
		/// <param name="filePath">This path will be written into the value of the 'src' attribute.</param>
		/// <param name="scriptType">Script type (e.g. JavaScript / VBScript)</param>
		/// <param name="scriptVersion">Script version (e.g. 1.2 / 1.5)</param>
		/// <returns>Returns the generated string.</returns>
		public string GetScriptFileTag(string filePath, string commentCode, string scriptType, float scriptVersion)
		{
			return String.Format(SCRIPT_FILE,
				GetScriptLanguageString(scriptType, scriptVersion),
				_scriptType,
				filePath,
				COMMENT_BEGIN + commentCode + COMMENT_END);
		}


		/// <summary>
		/// Returns a script tag which contains the specified file path attribute.
		/// </summary>
		/// <param name="filePath">This path will be written into the value of the 'src' attribute.</param>
		/// <param name="commentCode">Comment to descript the functionality, which is written in the file.</param>
		/// <returns>Returns the generated string.</returns>
		public string GetScriptFileTag(string filePath, string commentCode)
		{
			return String.Format(SCRIPT_FILE,
				GetScriptLanguageString(_scriptType, _scriptVersion),
				_scriptType,
				filePath,
				COMMENT_BEGIN + commentCode + COMMENT_END);
		}


		/// <summary>
		/// Returns a script tag which contains the specified file path attribute.
		/// </summary>
		/// <param name="script">Script file object to get tag.</param>
		/// <returns>Returns the generated string.</returns>
		/// <exception cref="ArgumentNullException">The given script contains a null reference.</exception>
		/// <exception cref="ArgumentNullException">The given path prefix contains a null reference.</exception>
		public string GetScriptFileTag(JSScript script, string pathPrefix)
		{
			if (script == null)
				throw new ArgumentNullException("script", "The given script contains a null reference!");

			if (pathPrefix == null)
				throw new ArgumentNullException("pathPrefix", "The given path prefix contains a null reference!");

			return GetScriptFileTag(CombinePathPrefix(pathPrefix, script.RequestPath));
		}


		/// <summary>
		/// Returns a script tag which contains the specified file path attribute.
		/// </summary>
		/// <param name="script">Script file object to get tag.</param>
		/// <returns>Returns the generated string.</returns>
		/// <exception cref="ArgumentNullException">The given script contains a null reference.</exception>
		public string GetScriptFileTag(JSScript script)
		{
			return GetScriptFileTag(script, string.Empty);
		}


		/// <summary>
		/// Returns a script begin tag without a source attribute.
		/// </summary>
		/// <returns>Returns the generated string.</returns>
		public string GetScriptBeginTag()
		{
			return String.Format(SCRIPT_BEGIN,
				GetScriptLanguageString(_scriptType, _scriptVersion),
				_scriptType);
		}


		/// <summary>
		/// Returns a script begin tag without a source attribute.
		/// </summary>
		/// <param name="scriptType">Script type (e.g. JavaScript / VBScript)</param>
		/// <param name="scriptVersion">Script version (e.g. 1.2 / 1.5)</param>
		/// <returns>Returns the generated string.</returns>
		public string GetScriptBeginTag(string scriptType, float scriptVersion)
		{
			return String.Format(SCRIPT_BEGIN,
				GetScriptLanguageString(scriptType, scriptVersion),
				_scriptType);
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
		/// Combines the given prefix with the specified path.
		/// </summary>
		/// <param name="prefix">Prefix to insert.</param>
		/// <param name="path">Path to adjust.</param>
		public string CombinePathPrefix(string prefix, string path)
		{
			if (prefix == string.Empty)
				return path;

			if (prefix[prefix.Length - 1] == JSScriptFileHandler.PATH_SEPARATOR)
			{
				return prefix.Substring(0, prefix.Length - 1) + path;
			}
			else
			{
				return prefix + path;
			}
		}


		/// <summary>
		/// Returns a module, which is contained was specified by the names array. This
		/// procedure will create a recursive loop to find the specified module.
		/// </summary>
		/// <param name="module">Parent module instance.</param>
		/// <param name="names">Array, which contains the module names.</param>
		/// <param name="namePointer">Pointer (nestlevel), used by the names array.</param>
		/// <returns>Returns the expected module or a null reference.</returns>
		private JSModule GetChildModuleByName(JSModule module, string[] names)
		{
			if (module == null || names.Length == 0)
				return null;

			JSModule result = module;

			// the first module was directly assigned to the OwnerSection node
			for (int i = 1; i < names.Length; ++i)
			{
				if ((result = result.ChildModules[(string)names.GetValue(i)]) == null)
				{
					break;
				}
			}
			return result;
		}


		/// <summary>
		/// Initializes the values of this configuration object.
		/// </summary>
		private void InitConfiguration()
		{
			_source = GetValidPath(JSToolsXmlFunctions.GetAttributeFromNode(_configSection, SOURCE_ATTRIB));
			_extension = GetExtension();

			InitScriptNode();
			InitModuleNodes();
		}


		/// <summary>
		/// Checks the extension given by the configuration for validity.
		/// </summary>
		/// <returns>Retruns a valid script extension.</returns>
		private string GetExtension()
		{
			string extension = JSToolsXmlFunctions.GetAttributeFromNode(_configSection, EXTENSION_ATTRIB);

			if (extension == string.Empty)
			{
				return _extension;
			}
			if (!extension.StartsWith("."))
			{
				return "." + extension;
			}
			return extension;
		}


		/// <summary>
		/// Returns the specified path with a valid termination.
		/// </summary>
		/// <param name="path">Path which should be evaluated.</param>
		/// <returns>Returns a string.</returns>
		private string GetValidPath(string path)
		{
			if (path.EndsWith(PATH_SEPARATOR.ToString()))
			{
				return path.Remove(path.Length - 1, 1);
			}
			else
			{
				return path;
			}
		}


		/// <summary>
		/// Initializes the values of the &lt;scripts&gt; node.
		/// </summary>
		private void InitScriptNode()
		{
			InitScriptVersion();
			InitDebugMode();

			_scriptType = JSToolsXmlFunctions.GetValueFromNode(_configSection.Attributes[SCRIPT_TYPE_ATTRIB]).ToLower();
			_contentType = JSToolsXmlFunctions.GetValueFromNode(_configSection.Attributes[CONTENT_TYPE_ATTRIB]).ToLower();
		}


		/// <summary>
		/// Initializes the value of the debug attribute.
		/// </summary>
		private void InitDebugMode()
		{
			try
			{
				_debugMode = (DebugMode)Enum.Parse(typeof(DebugMode), JSToolsXmlFunctions.GetValueFromNode(_configSection.Attributes[DEBUG_ATTRIB]), true);
			}
			catch
			{
				// ignore value if an exception occurs
			}
		}


		/// <summary>
		/// Initializes the value of the scriptVersion attribute.
		/// </summary>
		private void InitScriptVersion()
		{
			string scriptVersion = JSToolsXmlFunctions.GetValueFromNode(_configSection.Attributes[SCRIPT_VERSION_ATTRIB]);

			try
			{
				_scriptVersion = Convert.ToSingle(scriptVersion);
			}
			catch
			{
				// ignore value if an exception occurs
			}
		}


		/// <summary>
		/// Initializes all module nodes.
		/// </summary>
		private void InitModuleNodes()
		{
			XmlNodeList moduleNodes = _configSection.SelectNodes(JSModule.MODULE_NODE_NAME);
			JSModule[] modules = new JSModule[moduleNodes.Count];

			for (int i = 0; i < moduleNodes.Count; ++i)
			{
				XmlNode moduleNode = moduleNodes[i];
				modules.SetValue(new JSModule(moduleNode, this), i);
			}

			_childModules = new JSModuleContainer(modules);

			// check created module hierarchy for valid relations
			base.OnCheckModuleRelations(this, EventArgs.Empty);
		}


		/// <summary>
		/// Appends the script type to the script version. (e.g. JavaScript1.3)
		/// </summary>
		/// <returns>Returns the created string.</returns>
		private string GetScriptLanguageString(string scriptType, float scriptVersion)
		{
			string type = (scriptType != null) ? scriptType : string.Empty;
			string version = (scriptVersion != 0) ? scriptVersion.ToString() : string.Empty;
			return type + version;
		}
	}
}

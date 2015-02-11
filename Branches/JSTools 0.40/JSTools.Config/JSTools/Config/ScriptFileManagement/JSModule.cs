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
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Xml;

using JSTools.Xml;

namespace JSTools.Config.ScriptFileManagement
{
	/// <summary>
	/// Represents a &lt;module&gt; node in a configuration XmlDocument.
	/// </summary>
	public class JSModule : AJSToolsScriptFileSection
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the name of a JSModule xml node.
		/// </summary>
		public const string MODULE_NODE_NAME = "module";

		private const string NAME_ATTRIB = "name";
		private const string DEFAULT_ATTRIB = "default";
		private const string MODULE_REL_ATTRIB = "module";
		private const string REQUIRES_NODE_NAME = "requires";

		private string _name = "";
		private bool _default = false;
		private StringCollection _relations = new StringCollection();

		private JSModuleContainer _childModules = null;
		private JSScriptContainer _childScripts = null;

		private XmlNode _moduleNode = null;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the unique id of this section.
		/// </summary>
		public override string Id
		{
			get { return Path; }
		}

		/// <summary>
		/// Returns an absolute module request path. (e.g. /src/JSTools/Web.js)
		/// </summary>
		public string RequestPath
		{
			get
			{
				return System.IO.Path.ChangeExtension(
					JSScriptFileHandler.PATH_SEPARATOR + Path,
					((JSScriptFileHandler)OwnerSection).ScriptExtension );
			}
		}

		/// <summary>
		/// Returns true, if the default flag is set.
		/// </summary>
		public bool IsDefaultModule
		{
			get { return _default; }
		}

		/// <summary>
		/// Gets the name of the representing xml node.
		/// </summary>
		public string SectionName
		{
			get { return MODULE_NODE_NAME; }
		}

		/// <summary>
		/// Returns all child modules.
		/// </summary>
		public JSModuleContainer ChildModules
		{
			get { return _childModules; }
		}

		/// <summary>
		/// Returns the parent module instance. If the parent is not a valid Module instance,
		/// you will obtain a null reference.
		/// </summary>
		public JSModule ParentModule
		{
			get { return (ParentSection as JSModule); }
		}

		/// <summary>
		/// Returns the full name of this module. The modules are separated by ".".
		/// </summary>
		public string FullName
		{
			get
			{
				if (ParentModule != null)
					return ParentModule.FullName + JSScriptFileHandler.NAME_SEPARATOR + _name;

				return _name;
			}
		}

		/// <summary>
		/// Returns the module path.
		/// </summary>
		public string Path
		{
			get
			{
				if (ParentModule != null)
					return ParentModule.Path + JSScriptFileHandler.PATH_SEPARATOR + _name;

				return _name;
			}
		}

		/// <summary>
		/// Returns the name of this module.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// Returns the module relations.
		/// </summary>
		public string[] Relations
		{
			get
			{
				string[] relations = new string[_relations.Count];
				_relations.CopyTo(relations, 0);
				return relations;
			}
		}

		/// <summary>
		/// Returns all files, which are registered in this module.
		/// </summary>
		public JSScriptContainer ScriptFiles
		{
			get { return _childScripts; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Initializes a new JavaScript configuration module.
		/// </summary>
		/// <param name="moduleNode">XmlNode which contians the module node.</param>
		/// <param name="parentSection">Parent JSToolsConfiguration of this node.</param>
		/// <exception cref="ArgumentNullException">An argument contains a null reference.</exception>
		/// <exception cref="InvalidOperationException">This module already contains a module definition for a name of a child script.</exception>
		internal JSModule(XmlNode moduleNode, AJSToolsScriptFileSection parentSection) : base(parentSection)
		{
			if (moduleNode == null)
				throw new ArgumentNullException("moduleNode", "The given XmlNode contains a null reference.");

			_moduleNode = moduleNode;

			InitModule();

			// init child elements
			InitChildModules();
			InitFileSources();
			InitRelations();
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		/// <summary>
		/// Bubbles the CheckRelations event.
		/// </summary>
		/// <param name="sender">Sender object.</param>
		/// <param name="e">Event argument object.</param>
		/// <exception cref="InvalidOperationException">A required module could not be found.</exception>
		protected override void OnCheckModuleRelations(object sender, EventArgs e)
		{
			foreach (string relation in _relations)
			{
				if (!(OwnerSection as JSScriptFileHandler).IsModuleRegistered(relation))
					throw new InvalidOperationException("Error in module definition '" + FullName + "': The required module '" + relation + "' could not be found.");
			}
			base.OnCheckModuleRelations(sender, e);
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Returns true, if this module contains a relation module with the
		/// specified name.
		/// </summary>
		/// <param name="realtionModuleName">Module name to check.</param>
		public bool HasRelation(string realtionModuleName)
		{
			foreach (string relationName in _relations)
			{
				if (relationName == realtionModuleName)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Returns true, if this module contains the specified module as relation.
		/// </summary>
		/// <param name="relationModule">Module to check.</param>
		public bool HasRelation(JSModule relationModule)
		{
			return HasRelation(relationModule.FullName);
		}

		/// <summary>
		/// Searches for the given file path and returns true, if it is stored in this
		/// module.
		/// </summary>
		/// <param name="filePath">File path to check.</param>
		/// <returns>Returns true, if the given file path is stored in this module.</returns>
		public bool IsFileRegistered(string filePath)
		{
			foreach (JSScript script in _childScripts)
			{
				if (script.Path == filePath)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Initializes the values of the XmlNode.
		/// </summary>
		private void InitModule()
		{
			_name = JSToolsXmlFunctions.GetValueFromNode(_moduleNode.Attributes[NAME_ATTRIB]);
			_default = JSToolsXmlFunctions.GetBoolFromNodeValue(_moduleNode.Attributes[DEFAULT_ATTRIB]);
		}

		/// <summary>
		/// Initializes the xml required nodes.
		/// </summary>
		private void InitRelations()
		{
			foreach (XmlNode childModule in _moduleNode.SelectNodes(REQUIRES_NODE_NAME))
			{
				_relations.Add(JSToolsXmlFunctions.GetAttributeFromNode(childModule, MODULE_REL_ATTRIB));
			}
		}

		/// <summary>
		/// Initializes the child modules of this module.
		/// </summary>
		private void InitChildModules()
		{
			XmlNodeList moduleNodes = _moduleNode.SelectNodes(MODULE_NODE_NAME);
			JSModule[] modules = new JSModule[moduleNodes.Count];

			for (int i = 0; i < moduleNodes.Count; ++i)
			{
				XmlNode moduleNode = moduleNodes[i];
				modules.SetValue(new JSModule(moduleNode, this), i);
			}
			_childModules = new JSModuleContainer(modules);
		}

		/// <summary>
		/// Initilializes the file tags.
		/// </summary>
		private void InitFileSources()
		{
			XmlNodeList scriptNodes = _moduleNode.SelectNodes(JSScript.FILE_NODE_NAME);
			JSScript[] scripts = new JSScript[scriptNodes.Count];

			for (int i = 0; i < scriptNodes.Count; ++i)
			{
				XmlNode scriptNode = scriptNodes[i];
				JSScript script = new JSScript(scriptNode, this); 
				scripts.SetValue(script, i);

				if (_childModules.Contains(script.Name))
				{
					throw new InvalidOperationException("The module '" + Name + "' already contains a module definition for '" + script.Name + "'");
				}
			}
			_childScripts = new JSScriptContainer(scripts);
		}
	}
}

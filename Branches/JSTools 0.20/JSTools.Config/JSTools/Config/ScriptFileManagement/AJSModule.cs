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
using System.Collections.Specialized;
using System.Text;
using System.Xml;

using JSTools.Config.Session;
using JSTools.Xml;

namespace JSTools.Config.ScriptFileManagement
{
	/// <summary>
	/// Occurs if the user moves a module. The sender argument represents the old module instance,
	/// the newMocule parameter determines the created clone. Child elements will be initialized after
	/// the OnInit event. An OnInit event is fired if a parent section was set.
	/// </summary>
	public delegate void JSToolsMoveEvent(AJSModule sender, AJSModule newModule);


	/// <summary>
	/// Represents a &lt;module&gt; node in a configuration XmlDocument. To create a new JSModule, you
	/// should use the AJSScriptFileHandler.CreateModule() method.
	/// </summary>
	public abstract class AJSModule : AFileManagementSection, IWriteable, ICloneable
	{
		//------------------------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------------------------

		public	const	string						MODULE_NODE_NAME	= "module";
		public	const	char						PATH_SEPARATOR		= '/';
		public	const	char						NAME_SEPARATOR		= '.';

		protected		string						_release			= "";
		protected		StringCollection			_relations			= new StringCollection();

		private	const	string						MODULE_HTML_BEGIN	= AJSToolsConfiguration.COMMENT_BEGIN + " MODULE ";
		private	const	string						MODULE_HTML_END		= " " + AJSToolsConfiguration.COMMENT_END + "\n";

		private	const	string						NAME_ATTRIB			= "name";
		private	const	string						RELEASE_ATTRIB		= "release";
		private	const	string						MODULE_REL_ATTRIB	= "module";
		private	const	string						REQUIRES_NODE_NAME	= "requires";

		private			string						_name				= "";
		private			XmlNode						_moduleNode			= null;

		private			AJSModuleContainer			_childModules		= null;
		private			AJSScriptContainer			_childScripts		= null;

		protected		AJSScriptFileHandler		_ownerSection		= null;
		protected		JSToolsFileSectionEvent		_remover			= null;
		protected		JSToolsMoveEvent			_mover				= null;


		/// <summary>
		/// Returns a writeable instance of this object.
		/// </summary>
		AJSToolsEventHandler IWriteable.WriteableInstance
		{
			get { return WriteableInstance; }
		}


		/// <summary>
		/// Returns the name of a file, which contains the release script of this module.
		/// </summary>
		public abstract string ReleaseFile
		{
			get;
			set;
		}


		/// <summary>
		/// Returns the configuration section handler, which contains this module.
		/// </summary>
		public override AJSToolsEventHandler OwnerSection
		{
			get { return _ownerSection; }
		}


		/// <summary>
		/// Returns the configuration section handler, which contains this module.
		/// </summary>
		public override IJSToolsConfiguration OwnerConfiguration
		{
			get { return _ownerSection.OwnerConfiguration; }
		}


		/// <summary>
		/// Occurs if the user moves this module.
		/// </summary>
		public event JSToolsMoveEvent OnMove;


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
		public AJSModuleContainer ChildModules
		{
			get { return _childModules; }
		}


		/// <summary>
		/// Returns the parent module.
		/// </summary>
		/// <exception cref="InvalidOperationException">Could not reference to the parent section, it is not given yet.</exception>
		public AJSModule ParentModule
		{
			get { return (ParentSection as AJSModule); }
		}


		/// <summary>
		/// Returns the full name of this module. The modules are separated by ".".
		/// </summary>
		/// <exception cref="InvalidOperationException">Could not reference to the parent section, it is not given yet.</exception>
		public string FullName
		{
			get
			{
				if (ParentModule != null)
				{
					return ParentModule.FullName + NAME_SEPARATOR + _name;
				}
				return _name;
			}
		}


		/// <summary>
		/// Returns the module path.
		/// </summary>
		/// <exception cref="InvalidOperationException">Could not reference to the parent section, it is not given yet.</exception>
		public string Path
		{
			get
			{
				if (ParentModule != null)
				{
					return ParentModule.Path + PATH_SEPARATOR + _name;
				}
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
		public AJSScriptContainer ScriptFiles
		{
			get { return _childScripts; }
		}


		/// <summary>
		/// Returns a writeable instance of this object.
		/// </summary>
		/// <exception cref="InvalidOperationException">Could not reference to the parent section, it is not given yet.</exception>
		public AJSModule WriteableInstance
		{
			get
			{
				if (ParentModule != null)
				{
					return ParentModule.WriteableInstance.ChildModules[Name];
				}
				else
				{
					return _ownerSection.WriteableInstance.ChildModules[Name];
				}
			}
		}


		//------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Initializes a new JavaScript configuration module.
		/// </summary>
		/// <param name="moduleNode">XmlNode which contians the module node.</param>
		/// <param name="parentConfiguration">Parent JSToolsConfiguration of this node.</param>
		/// <exception cref="ArgumentNullException">An argument contains a null reference.</exception>
		protected AJSModule(XmlNode moduleNode, AJSScriptFileHandler parentConfiguration)
		{
			if (parentConfiguration == null)
				throw new ArgumentNullException("parentConfiguration", "The given JSToolsConfiguration contains a null reference!");

			if (moduleNode == null)
				throw new ArgumentNullException("moduleNode", "The given XmlNode contains a null reference!");

			_remover		= new JSToolsFileSectionEvent(OnRelationDelete);
			_mover			= new JSToolsMoveEvent(OnRelationMove);
			OnInit			+= new JSToolsInitEvent(OnParentInit);

			_moduleNode		= moduleNode;
			_ownerSection	= parentConfiguration;
			_childModules	= CreateModuleContainer();
			_childScripts	= CreateScriptContainer();
			InitModule();
		}


		/// <summary>
		/// Initializes a new JavaScript configuration module.
		/// </summary>
		/// <param name="moduleName">Name of the new module.</param>
		/// <param name="parentConfiguration">Parent JSToolsConfiguration of this node.</param>
		/// <exception cref="ArgumentNullException">An argument contains a null reference.</exception>
		protected AJSModule(string moduleName, AJSScriptFileHandler parentConfiguration)
		{
			if (parentConfiguration == null)
				throw new ArgumentNullException("parentConfiguration", "The given JSToolsConfiguration contains a null reference!");

			if (moduleName == null || moduleName == String.Empty)
				throw new ArgumentNullException("moduleName", "The given module name contains an invalid value!");

			_remover		= new JSToolsFileSectionEvent(OnRelationDelete);
			_mover			= new JSToolsMoveEvent(OnRelationMove);
			OnInit			+= new JSToolsInitEvent(OnParentInit);

			_name			= moduleName;
			_ownerSection	= parentConfiguration;
			_childModules	= CreateModuleContainer();
			_childScripts	= CreateScriptContainer();
			InitModule();
		}


		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Creates a new module that is a copy of the current instance. The new copy will be assigned to the
		/// same AJSScriptFileHandler, as this module is assigned to.
		/// </summary>
		/// <returns>Returns the created copy.</returns>
		object ICloneable.Clone()
		{
			return Clone();
		}


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
		public bool HasRelation(AJSModule relationModule)
		{
			return HasRelation(relationModule.FullName);
		}


		/// <summary>
		/// Appends a new module relation to this module.
		/// </summary>
		/// <param name="relationModule">Relation module you'd like to add.</param>
		/// <exception cref="ArgumentNullException">The specified relation module contains a null reference.</exception>
		/// <exception cref="InvalidOperationException">The specified module is not contained in the owner section of this module.</exception>
		public abstract void AppendRelation(AJSModule relationModule);


		/// <summary>
		/// Removes the specified module from the relation collection.
		/// </summary>
		/// <param name="relationModule">Relation module you'd like to remove.</param>
		/// <exception cref="InvalidOperationException">The specified module is not a valid relation.</exception>
		/// <exception cref="ArgumentException">The specified module has an other owner section than this module has.</exception>
		/// <exception cref="ArgumentNullException">The specified relation module contains a null reference.</exception>
		public abstract void RemoveRelation(AJSModule relationModule);


		/// <summary>
		/// Same as Clone(AJSScriptFileHandler handler, true)
		/// </summary>
		/// <param name="handler">New parent handler.</param>
		/// <returns>Returns the created copy.</returns>
		public AJSModule Clone(AJSScriptFileHandler handler)
		{
			return Clone(handler, true);
		}


		/// <summary>
		/// Creates a new module that is a copy of the current instance. The new copy will be assigned to the
		/// given AJSScriptFileHandler.
		/// </summary>
		/// <param name="handler">New parent handler.</param>
		/// <param name="deep">True to copy the child modules, otherwise false.</param>
		/// <returns>Returns the created copy.</returns>
		public AJSModule Clone(AJSScriptFileHandler handler, bool deep)
		{
			XmlElement cloneElement = _ownerSection.OwnerConfigurationDocument.CreateElement(MODULE_NODE_NAME);
			SerializeXmlConfiguration(cloneElement, deep);

			return new JSModuleWriteable(cloneElement, handler);
		}


		/// <summary>
		/// Creates a new module that is a copy of the current instance. The new copy will be assigned to the
		/// same AJSScriptFileHandler as this module is assigned to.
		/// </summary>
		/// <param name="deep">True to copy the child modules, otherwise false.</param>
		/// <returns>Returns the created copy.</returns>
		public AJSModule Clone(bool deep)
		{
			return Clone(_ownerSection, deep);
		}


		/// <summary>
		/// Same as Clone(true)
		/// </summary>
		/// <returns>Returns the created copy.</returns>
		public AJSModule Clone()
		{
			return Clone(true);
		}


		/// <summary>
		/// Searches for the given file path and returns true, if it is stored in this
		/// module.
		/// </summary>
		/// <param name="filePath">File path to check.</param>
		/// <returns>Returns true, if the given file path is stored in this module.</returns>
		public bool IsFileRegistered(string filePath)
		{
			foreach (AJSScript script in _childScripts)
			{
				if (script.Path == filePath)
				{
					return true;
				}
			}
			return false;
		}


		/// <summary>
		/// Fires the public on remove event. The module will be removed from the parent collection after this event.
		/// This event will be fired from the ModuleCollection.
		/// </summary>
		internal void FireOnRemoveEvent()
		{
			if (_remove != null)
			{
				_remove(this);
			}
		}


		/// <summary>
		/// Fires the public on move event. The module will be cloned and removed from the parent collection after
		/// this event.
		/// This event will be fired from the ModuleCollection.
		/// </summary>
		internal void FireOnMoveEvent(AJSModule newModule)
		{
			if (OnMove != null)
			{
				OnMove(this, newModule);
			}
		}


		/// <summary>
		/// Writes the script informations of this module into the given RenderProcessTicket.
		/// </summary>
		/// <param name="renderContext">Render context.</param>
		/// <exception cref="InvalidOperationException">A relation module with the given name is not registered.</exception>
		/// <exception cref="ArgumentNullException">The specified RenderProcessTicket contains a null reference.</exception>
		/// <exception cref="InvalidOperationException">Could not reference to the parent section, it is not given yet.</exception>
		protected override void RenderScriptConfiguration(RenderProcessTicket renderContext)
		{
			if (renderContext == null)
				throw new ArgumentNullException("renderContext", "The specified RenderProcessTicket contains a null reference!");

			if (!renderContext.IsModuleRendered(FullName))
			{
				renderContext.ModuleRendered(FullName);

				RenderParentModule(renderContext);
				RenderRelations(renderContext);
				RenderModule(renderContext);
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
			XmlNode moduleNode = parentNode.OwnerDocument.CreateElement(SectionName);
			parentNode.AppendChild(moduleNode);

			JSToolsXmlFunctions.AppendAttributeToNode(moduleNode, NAME_ATTRIB, Name);
			JSToolsXmlFunctions.AppendAttributeToNode(moduleNode, RELEASE_ATTRIB, ReleaseFile);

			AppendRelationNodes(moduleNode);


			if (deep)
			{
				// call base function to enable event bubbling
				base.SerializeXmlConfiguration(moduleNode, deep);
			}
		}


		/// <summary>
		/// This method will be called, if configuration is loading this file management section. Event bubbling
		/// functionality must be implemented by this method.
		/// </summary>
		/// <param name="sender">The event sender.</param>
		protected override void LoadFileManagementSection(AFileManagementSection sender)
		{
			// init relations
			foreach (string relation in _relations)
			{
				AJSModule module = _ownerSection.GetModule(relation);

				if (module == null)
					throw new InvalidOperationException("The required module '" + relation + "' was not registered!");

				AppendRelationEvents(module);
			}

			// fire OnLoad event
			base.LoadFileManagementSection(sender);
		}


		/// <summary>
		/// Creates a new AJSModuleContainer instance for internal use.
		/// </summary>
		/// <returns>Returns the created AJSModuleContainer.</returns>
		protected abstract AJSModuleContainer CreateModuleContainer();


		/// <summary>
		/// Creates a new AJSScriptContainer instance for internal use.
		/// </summary>
		/// <returns>Returns the created AJSScriptContainer.</returns>
		protected abstract AJSScriptContainer CreateScriptContainer();


		/// <summary>
		/// Removes OnMove and OnRemove event handler from the specified module.
		/// </summary>
		/// <param name="relation">Module to remove event handlers.</param>
		protected void RemoveRelationEvents(AJSModule relation)
		{
			if (HasRelation(relation))
			{
				relation.OnRemove	-= _remover;
				relation.OnMove		-= _mover;
			}
		}


		/// <summary>
		/// Appends OnMove and OnRemove event handler to the specified module.
		/// </summary>
		/// <param name="relation">Module to append event handlers.</param>
		protected void AppendRelationEvents(AJSModule relation)
		{
			if (!HasRelation(relation))
			{
				relation.OnRemove	+= _remover;
				relation.OnMove		+= _mover;
			}
		}


		/// <summary>
		/// Appends the required modules to the given node.
		/// </summary>
		/// <param name="parentNode">Node which should contain the relation nodes.</param>
		private void AppendRelationNodes(XmlNode parentNode)
		{
			foreach (string relation in _relations)
			{
				XmlNode element = parentNode.OwnerDocument.CreateElement(REQUIRES_NODE_NAME);
				JSToolsXmlFunctions.AppendAttributeToNode(element, MODULE_REL_ATTRIB, relation);
				parentNode.AppendChild(element);
			}
		}


		/// <summary>
		/// Initializes the values of the XmlNode.
		/// </summary>
		private void InitModule()
		{
			_name = JSToolsXmlFunctions.GetValueFromNode(_moduleNode.Attributes[NAME_ATTRIB]);
			_release = JSToolsXmlFunctions.GetValueFromNode(_moduleNode.Attributes[RELEASE_ATTRIB]);
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
			foreach (XmlNode childModuleNode in _moduleNode.SelectNodes(MODULE_NODE_NAME))
			{
				_childModules.AppendInnerModule(childModuleNode);
			}
		}


		/// <summary>
		/// Initilializes the file tags.
		/// </summary>
		private void InitFileSources()
		{
			foreach (XmlNode fileNode in _moduleNode.SelectNodes(AJSScript.FILE_NODE_NAME))
			{
				_childScripts.AppendInnerScript(Path, fileNode);
			}
		}


		/// <summary>
		/// Renders the module in debug or release mode.
		/// </summary>
		private void RenderModule(RenderProcessTicket renderContext)
		{
			if (_ownerSection.Debug)
			{
				renderContext.Write(MODULE_HTML_BEGIN);
				renderContext.Write(FullName);
				renderContext.Write(MODULE_HTML_END);

				FireRenderEvent(renderContext);
			}
			else
			{
				renderContext.Write(_ownerSection.GetScriptFileTag(_ownerSection.ScriptSourceFolder + PATH_SEPARATOR + _release) + "\n");
			}
		}


		/// <summary>
		/// Renders the parent module, if a parent module exists.
		/// </summary>
		/// <param name="renderContext">Render context.</param>
		private void RenderParentModule(RenderProcessTicket renderContext)
		{
			if (ParentModule != null)
			{
				ParentModule.RenderScriptConfiguration(renderContext);
			}
		}


		/// <summary>
		/// Renders all relation modules.
		/// </summary>
		/// <param name="renderContext">Render context.</param>
		private void RenderRelations(RenderProcessTicket renderContext)
		{
			foreach (string relation in _relations)
			{
				if (!_ownerSection.IsModuleRegistered(relation))
					throw new InvalidOperationException("The relation module '" + relation + "' is not registered!");

				_ownerSection.GetModule(relation).RenderScriptConfiguration(renderContext);
			}
		}


		/// <summary>
		/// Fires the public on load event. Initilizes the events of the relation modules.
		/// </summary>
		private void OnParentInit(AJSToolsEventHandler sender, AJSToolsEventHandler newParent)
		{
			// init child elements
			InitFileSources();
			InitChildModules();
			InitRelations();
		}


		/// <summary>
		/// Removes the specified relation node.
		/// </summary>
		/// <param name="sender">Module, which has send the event.</param>
		/// <param name="eventArgs">Event arguments.</param>
		private void OnRelationDelete(AFileManagementSection sender)
		{
			AJSModule senderModule = (sender as AJSModule);

			if (sender != null)
			{
				RemoveRelation(senderModule);
			}
		}


		/// <summary>
		/// Moves the specified relation node.
		/// </summary>
		/// <param name="sender">Module, which has send the event.</param>
		/// <param name="newModule">New module instance.</param>
		private void OnRelationMove(AJSModule sender, AJSModule newModule)
		{
			for (int i = 0; i < _relations.Count; ++i)
			{
				if (sender.FullName == _relations[i])
				{
					_relations[i] = newModule.FullName;

					// delete references to release memory
					RemoveRelationEvents(sender);
				}
			}
		}
	}
}

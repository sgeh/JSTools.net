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
using System.Text;
using System.Xml;

using JSTools.Config.Session;
using JSTools.Xml;

namespace JSTools.Config.ScriptFileManagement
{
	/// <summary>
	/// Contains the &lt;file&gt; node implementation. A AJSScript can render two different javascript tags,
	/// a link tag (the value is specified by the &lt;file src=""&gt; tag), and a code tag (the value
	/// is specified by the CDATA value of the &lt;file&gt; node. To create a new JSScript, you
	/// should use the AJSScriptFileHandler.CreateScriptFile() method.
	/// </summary>
	public abstract class AJSScript : AFileManagementSection, IWriteable, ICloneable
	{
		//------------------------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------------------------

		public	const	string					FILE_NODE_NAME		= "file";

		protected		string					_fileName			= "";
		protected		string					_code				= null;
		protected		AJSScriptFileHandler	_ownerSection		= null;

		private	const	string					SRC_ATTRIB			= "src";

		private			XmlNode					_fileNode			= null;
		private			string					_folder				= "";


		/// <summary>
		/// Sets the fallback code, if the specified file path not exists.
		/// </summary>
		public abstract string CommentCode
		{
			get;
			set;
		}


		/// <summary>
		/// Returns the name of the file.
		/// </summary>
		public abstract string FileName
		{
			get;
			set;
		}


		/// <summary>
		/// Returns a writeable instance of this object.
		/// </summary>
		AJSToolsEventHandler IWriteable.WriteableInstance
		{
			get { return WriteableInstance; }
		}


		/// <summary>
		/// Gets the name of the representing xml node.
		/// </summary>
		public string SectionName
		{
			get { return FILE_NODE_NAME; }
		}


		/// <summary>
		/// Returns the folder path.
		/// </summary>
		public string Folder
		{
			get { return ParentModule.Path; }
		}


		/// <summary>
		/// Gets the script path (folder + file name).
		/// </summary>
		public string Path
		{
			get { return Folder + AJSModule.PATH_SEPARATOR + _fileName; }
		}




		/// <summary>
		/// Returns the owner module.
		/// </summary>
		/// <exception cref="InvalidOperationException">Could not reference to the parent section, it is not given yet.</exception>
		public AJSModule ParentModule
		{
			get { return (ParentSection as AJSModule); }
		}


		/// <summary>
		/// Returns a writeable instance of this object.
		/// </summary>
		/// <exception cref="InvalidOperationException">Could not access the writeable instance because it is not assigned to a module collection.</exception>
		public AJSScript WriteableInstance
		{
			get
			{
				if (ParentModule == null)
					throw new InvalidOperationException("Could not access the writeable instance because this instance is not assigned to a module collection!");

				return ParentModule.WriteableInstance.ScriptFiles[FileName];
			}
		}


		/// <summary>
		/// Returns the configuration instance which has created this FileHandler.
		/// </summary>
		public override IJSToolsConfiguration OwnerConfiguration
		{
			get { return _ownerSection.OwnerConfiguration; }
		}


		/// <summary>
		/// Returns the configuration section handler, which contains this module.
		/// </summary>
		public override AJSToolsEventHandler OwnerSection
		{
			get { return _ownerSection; }
		}


		//------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSScript instance.
		/// </summary>
		/// <param name="scriptNode">Xml node, which contains the source informations.</param>
		/// <param name="ownerHandler">Parent JSScriptFileHandler.</param>
		/// <exception cref="ArgumentNullException">An argument contains a null reference.</exception>
		/// <exception cref="ArgumentException">A AJSScript with the given path is already imported.</exception>
		internal AJSScript(XmlNode scriptNode, AJSScriptFileHandler ownerHandler)
		{
			if (scriptNode == null)
				throw new ArgumentNullException("scriptNode", "The file xml node contains a null reference!");

			if (ownerHandler == null)
				throw new ArgumentNullException("ownerHandler", "The JSScriptFileHandler contains a null reference!");

			_ownerSection	= ownerHandler;
			_fileNode		= scriptNode;
			OnInit			+= new JSToolsInitEvent(OnParentInit);
		}


		/// <summary>
		/// Creates a new JSScript instance.
		/// </summary>
		/// <param name="folder">Folder, in which this script is stored.</param>
		/// <param name="scriptNode">Xml node, which contains the source informations.</param>
		/// <param name="ownerHandler">Parent JSScriptFileHandler.</param>
		/// <exception cref="ArgumentNullException">An argument contains a null reference.</exception>
		/// <exception cref="ArgumentException">A AJSScript with the given path is already imported.</exception>
		internal AJSScript(string folder, XmlNode scriptNode, AJSScriptFileHandler ownerHandler)
		{
			if (folder == null)
				throw new ArgumentNullException("folder", "The specified path contains a null reference!");

			if (scriptNode == null)
				throw new ArgumentNullException("scriptNode", "The file xml node contains a null reference!");

			if (ownerHandler == null)
				throw new ArgumentNullException("ownerHandler", "The JSScriptFileHandler contains a null reference!");

			_ownerSection	= ownerHandler;
			_folder			= folder;
			_fileNode		= scriptNode;
			OnInit			+= new JSToolsInitEvent(OnParentInit);
		}


		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Creates a new script that is a copy of the current instance. The new copy will be assigned to the
		/// same AJSScriptFileHandler, as this module is assigned to.
		/// </summary>
		/// <returns>Returns the created copy.</returns>
		object ICloneable.Clone()
		{
			return Clone();
		}


		/// <summary>
		/// Creates a new script that is a copy of the current instance. The new copy will be assigned to the
		/// given AJSScriptFileHandler.
		/// </summary>
		/// <param name="handler">New parent handler.</param>
		/// <returns>Returns the created copy.</returns>
		public AJSScript Clone(AJSScriptFileHandler handler)
		{
			XmlElement cloneElement = _ownerSection.OwnerConfigurationDocument.CreateElement(FILE_NODE_NAME);
			SerializeXmlConfiguration(cloneElement, true);

			return new JSScriptWriteable(cloneElement, handler);
		}


		/// <summary>
		/// Creates a new module that is a copy of the current instance. The new copy will be assigned to the
		/// same AJSScriptFileHandler as this module is assigned to.
		/// </summary>
		/// <param name="deep">True to copy the child modules, otherwise false.</param>
		/// <returns>Returns the created copy.</returns>
		public AJSScript Clone()
		{
			return Clone(_ownerSection);
		}


		/// <summary>
		/// Fires the public on remove event. The module will be removed from the parent collection after this event.
		/// </summary>
		internal void FireOnRemoveEvent()
		{
			if (_remove != null)
			{
				_remove(this);
			}
		}


		/// <summary>
		/// Renders the JavaScript context of the current section to the client.
		/// </summary>
		/// <param name="renderContext">A StringBuilder object which content will be rendered to the client.</param>
		/// <exception cref="ArgumentNullException">The specified RenderProcessTicket contains a null reference.</exception>
		protected override void RenderScriptConfiguration(RenderProcessTicket renderContext)
		{
			if (renderContext == null)
				throw new ArgumentNullException("renderContext", "The specified RenderProcessTicket contains a null reference!");

			string absPath = _ownerSection.ScriptSourceFolder + AJSModule.PATH_SEPARATOR + Path;

			if (_code != String.Empty && _code != null)
			{
				renderContext.Write(_ownerSection.GetScriptFileTag(absPath, _code));
			}
			else
			{
				renderContext.Write(_ownerSection.GetScriptFileTag(absPath));
			}

			renderContext.Write("\n");

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
			XmlNode scriptNode = parentNode.OwnerDocument.CreateElement(SectionName);
			parentNode.AppendChild(scriptNode);

			JSToolsXmlFunctions.AppendAttributeToNode(scriptNode, SRC_ATTRIB, _fileName);

			if (_code != null && _code != String.Empty)
			{
				scriptNode.AppendChild(parentNode.OwnerDocument.CreateCDataSection(_code));
			}

			// call base function to enable event bubbling
			base.SerializeXmlConfiguration(parentNode, deep);
		}


		/// <summary>
		/// Fires the public on load event.
		/// </summary>
		private void OnParentInit(AJSToolsEventHandler sender, AJSToolsEventHandler newParent)
		{
			InitScript();

			if (_load != null)
			{
				_load(this);
			}
		}


		/// <summary>
		/// Initializes the script file node.
		/// </summary>
		private void InitScript()
		{
			if (_fileNode.FirstChild != null)
			{
				_code = _fileNode.FirstChild.Value;
			}
			_fileName = JSToolsXmlFunctions.GetAttributeFromNode(_fileNode, SRC_ATTRIB);
		}
	}
}

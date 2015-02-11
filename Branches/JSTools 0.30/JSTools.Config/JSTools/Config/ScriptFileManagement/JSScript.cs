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
using System.IO;
using System.Text;
using System.Xml;

using JSTools.Xml;

namespace JSTools.Config.ScriptFileManagement
{
	/// <summary>
	/// Contains the &lt;file&gt; node implementation. A JSScript can render two different javascript tags,
	/// a link tag (the value is specified by the &lt;file src=""&gt; tag), and a code tag (the value
	/// is specified by the CDATA value of the &lt;file&gt; node.
	/// </summary>
	public class JSScript : AJSToolsScriptFileSection
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		public	const	string					FILE_NODE_NAME		= "file";

		protected		string					_fileName			= string.Empty;
		protected		string					_name				= string.Empty;
		protected		string					_code				= string.Empty;
		protected		string					_physicalPath		= string.Empty;

		private	const	string					SRC_ATTRIB			= "src";
		private			XmlNode					_fileNode			= null;


		/// <summary>
		/// Returns the physical path of this script file.
		/// </summary>
		public string PhysicalPath
		{
			get { return _physicalPath; }
		}


		/// <summary>
		/// Gets the script source code.
		/// </summary>
		public string Code
		{
			get { return _code; }
		}


		/// <summary>
		/// Returns the name of the file with extension (e.g. ScriptLoader.js)
		/// </summary>
		public string FileName
		{
			get { return _fileName; }
		}


		/// <summary>
		/// Returns the name of the file without extension (e.g. ScriptLoader).
		/// </summary>
		public string Name
		{
			get { return _name; }
		}


		/// <summary>
		/// Gets the name of the representing xml node.
		/// </summary>
		public string SectionName
		{
			get { return FILE_NODE_NAME; }
		}


		/// <summary>
		/// Returns the path of the parent module.
		/// </summary>
		public string Folder
		{
			get { return (ParentModule != null) ? ParentModule.Path : string.Empty; }
		}


		/// <summary>
		/// Gets the script path (folder + name).
		/// </summary>
		public string Path
		{
			get { return Folder + JSScriptFileHandler.PATH_SEPARATOR + _name; }
		}


		/// <summary>
		/// Returns the absolute path for the current script file (e.g. /JSTools/Web/ScriptLoader.js).
		/// </summary>
		public string RequestPath
		{
			get { return JSScriptFileHandler.PATH_SEPARATOR + ChangeExtension(Path); }
		}


		/// <summary>
		/// Returns the owner module.
		/// </summary>
		public JSModule ParentModule
		{
			get { return (ParentSection as JSModule); }
		}


		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSScript instance.
		/// </summary>
		/// <param name="scriptNode">Xml node, which contains the source informations.</param>
		/// <param name="parent">Parent AJSToolsScriptFileSection instance.</param>
		/// <exception cref="ArgumentNullException">An argument contains a null reference.</exception>
		internal JSScript(XmlNode scriptNode, AJSToolsScriptFileSection parent) : base(parent)
		{
			if (scriptNode == null)
				throw new ArgumentNullException("scriptNode", "The file xml node contains a null reference!");

			_fileNode = scriptNode;
			InitScript();
		}


		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Initializes the script file node.
		/// </summary>
		private void InitScript()
		{
			_fileName = JSToolsXmlFunctions.GetAttributeFromNode(_fileNode, SRC_ATTRIB);
			_name = System.IO.Path.GetFileNameWithoutExtension(_fileName);

			if (_fileNode.FirstChild != null)
			{
				_code = _fileNode.FirstChild.Value;
			}
			InitPhysicalFilePath();
		}


		/// <summary>
		/// Initializes the physical path of this script file.
		/// </summary>
		private void InitPhysicalFilePath()
		{
			string physPath = Folder + JSScriptFileHandler.PATH_SEPARATOR + _fileName;
			_physicalPath = System.IO.Path.Combine(((JSScriptFileHandler)OwnerSection).Source, physPath);
		}


		/// <summary>
		/// Changes the extension of the given path.
		/// </summary>
		/// <param name="scriptPath">Path to change the extension.</param>
		private string ChangeExtension(string scriptPath)
		{
			return System.IO.Path.ChangeExtension(scriptPath, ((JSScriptFileHandler)OwnerSection).ScriptExtension);
		}
	}
}

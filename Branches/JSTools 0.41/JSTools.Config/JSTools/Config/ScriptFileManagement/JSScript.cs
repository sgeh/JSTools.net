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

using JSTools.Config.ScriptFileManagement.Serialization;

namespace JSTools.Config.ScriptFileManagement
{
	/// <summary>
	/// Represents a &lt;file&gt; node in the configuration XmlDocument.
	/// </summary>
	public class JSScript : AJSToolsScriptFileSection
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const string FILE_NODE_NAME = "file";

		private string _fileName = string.Empty;
		private string _name = string.Empty;
		private string _physicalPath = string.Empty;

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
		/// Returns the physical path of this script file.
		/// </summary>
		public string PhysicalPath
		{
			get { return _physicalPath; }
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
		/// <param name="fileData">Instance which contains the script data.</param>
		/// <param name="parent">Parent AJSToolsScriptFileSection instance.</param>
		/// <exception cref="ArgumentNullException">An argument contains a null reference.</exception>
		internal JSScript(File fileData, AJSToolsScriptFileSection parent) : base(parent)
		{
			if (fileData == null)
				throw new ArgumentNullException("scriptNode", "The given script data contain null.");

			_fileName = fileData.Src;
			_name = System.IO.Path.GetFileNameWithoutExtension(_fileName);

			InitPhysicalFilePath();
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

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

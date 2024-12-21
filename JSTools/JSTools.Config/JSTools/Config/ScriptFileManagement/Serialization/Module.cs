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
using System.Xml.Serialization;

using JSTools.Config.ScriptFileManagement;

namespace JSTools.Config.ScriptFileManagement.Serialization
{
	/// <summary>
	/// Represents the "module" node specified in the configuration.
	/// </summary>
	[XmlType("module", Namespace=JSScriptFileHandlerFactory.NAMESPACE)]
	public class Module
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private string _name = string.Empty;
		private bool _isDefault = false;
		private Module[] _modules = null;
		private Requires[] _requires = null;
		private File[] _files = null;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// "name" attribute of the node.
		/// </summary>
		[XmlAttribute("name")]
		public string Name
		{
			get { return _name; } 
			set { _name = value; }
		}

		/// <summary>
		/// "default" attribute of the node.
		/// </summary>
		[XmlAttribute("default")]
		public bool IsDefault
		{
			get { return _isDefault; } 
			set { _isDefault = value; }
		}

		/// <summary>
		/// "requires" elements of the node.
		/// </summary>
		[XmlElement("requires")]
		public Requires[] Requires
		{
			get { return (_requires != null) ? _requires : new Requires[0]; }
			set { _requires = value; }
		}

		/// <summary>
		/// "file" elements of the node.
		/// </summary>
		[XmlElement("file")]
		public File[] Files
		{
			get { return (_files != null) ? _files : new File[0]; }
			set { _files = value; }
		}

		/// <summary>
		/// "module" elements of the node.
		/// </summary>
		[XmlElement("module")]
		public Module[] Modules
		{
			get { return (_modules != null) ? _modules : new Module[0]; }
			set { _modules = value; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new Module instance.
		/// </summary>
		public Module()
		{
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------
	}
}

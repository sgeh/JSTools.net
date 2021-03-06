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
	/// Represents the "file" node specified in the configuration.
	/// </summary>
	[XmlType("file", Namespace=JSScriptFileHandlerFactory.NAMESPACE)]
	public class File
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private string _src = string.Empty;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// "src" attribute of the node.
		/// </summary>
		[XmlAttribute("src")]
		public string Src
		{
			get { return _src; } 
			set { _src = value; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new File instance.
		/// </summary>
		public File()
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

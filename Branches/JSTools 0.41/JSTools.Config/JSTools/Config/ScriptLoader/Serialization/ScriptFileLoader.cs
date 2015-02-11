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

namespace JSTools.Config.ScriptLoader.Serialization
{
	/// <summary>
	/// Represents the "scriptFileLoader" node specified in the configuration.
	/// </summary>
	[XmlRoot(Namespace=JSScriptLoaderHandlerFactory.NAMESPACE)]
	[XmlType("scriptFileLoader", Namespace=JSScriptLoaderHandlerFactory.NAMESPACE)]
	public class ScriptFileLoader
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private string _requires = string.Empty;
		private string _scriptFileLocation = string.Empty;
		private bool _insertAppPrefix = false;
		private bool _encodeFileLocation = false;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// "requires" attribute of the node.
		/// </summary>
		[XmlAttribute("requires")]
		public string Requires
		{
			get { return _requires; }
			set { _requires = value; }
		}

		/// <summary>
		/// "scriptFileLocation" attribute of the node.
		/// </summary>
		[XmlAttribute("scriptFileLocation")]
		public string ScriptFileLocation
		{
			get { return _scriptFileLocation; }
			set { _scriptFileLocation = value; }
		}

		/// <summary>
		/// "insertAppPrefix" attribute of the node.
		/// </summary>
		[XmlAttribute("insertAppPrefix")]
		public bool InsertAppPrefix
		{
			get { return _insertAppPrefix; }
			set { _insertAppPrefix = value; }
		}

		/// <summary>
		/// "encodeFileLocation " attribute of the node.
		/// </summary>
		[XmlAttribute("encodeFileLocation ")]
		public bool EncodeFileLocation 
		{
			get { return _encodeFileLocation ; }
			set { _encodeFileLocation  = value; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new ScriptFileLoaders instance.
		/// </summary>
		public ScriptFileLoader()
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

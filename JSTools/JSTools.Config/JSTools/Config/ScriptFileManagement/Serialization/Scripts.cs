/*
 * JSTools.Config.dll / JSTools.net - A framework for JavaScript/ASP.NET applications.
 * Copyright (C) 2005  Silvan Gehrig
 *
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
 *
 * Author:
 *  Silvan Gehrig
 */

using System;
using System.Xml.Serialization;

using JSTools.Config.ScriptFileManagement;

namespace JSTools.Config.ScriptFileManagement.Serialization
{
	/// <summary>
	/// Represents the "scripts" node specified in the configuration.
	/// </summary>
	[XmlRoot(Namespace=JSScriptFileHandlerFactory.NAMESPACE)]
	[XmlType("scripts", Namespace=JSScriptFileHandlerFactory.NAMESPACE)]
	public class Scripts
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private int _cacheExpiration = 0;
		private float _version = 1.0F;
		private string _language = string.Empty;
		private string _contentType = string.Empty;
		private DebugMode _debug = DebugMode.None;
		private string _src = string.Empty;
		private string _extension = string.Empty;
		private Module[] _modules = null;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// "version" attribute of the node.
		/// </summary>
		[XmlAttribute("version")]
		public float Version
		{
			get { return _version; } 
			set { _version = value; }
		}

		/// <summary>
		/// "language" attribute of the node.
		/// </summary>
		[XmlAttribute("language")]
		public string Language
		{
			get { return _language; } 
			set { _language = value; }
		}

		/// <summary>
		/// "contentType" attribute of the node.
		/// </summary>
		[XmlAttribute("contentType")]
		public string ContentType
		{
			get { return _contentType; } 
			set { _contentType = value; }
		}

		/// <summary>
		/// "debug" attribute of the node.
		/// </summary>
		[XmlAttribute("debug")]
		public DebugMode Debug
		{
			get { return _debug; } 
			set { _debug = value; }
		}

		/// <summary>
		/// "src" attribute of the node.
		/// </summary>
		[XmlAttribute("src")]
		public string Src
		{
			get { return _src; } 
			set { _src = value; }
		}

		/// <summary>
		/// "extension" attribute of the node.
		/// </summary>
		[XmlAttribute("extension")]
		public string Extension
		{
			get { return _extension; } 
			set { _extension = value; }
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

		/// <summary>
		/// "cacheExpiration" attribute of the node.
		/// </summary>
		[XmlAttribute("cacheExpiration")]
		public int CacheExpiration
		{
			get { return _cacheExpiration; }
			set { _cacheExpiration = value; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new Scripts instance.
		/// </summary>
		public Scripts()
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

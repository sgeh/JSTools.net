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
using System.Xml;

using JSTools.Config;

namespace JSTools.Web.Config
{
	/// <summary>
	/// Represents a JSToolsConfiguration proxy object. An instance of this class will be return by the
	/// HttpContext.GetAppConfig() method. This class is immutable.
	/// </summary>
	public class JSToolsConfigurationProxy
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private XmlDocument		_xmlDocument = null;


		/// <summary>
		/// Gets the configuration instance.
		/// </summary>
		public IJSToolsConfiguration Configuration
		{
			get { return JSToolsWebConfiguration.Instance.Configuration; }
		}


		/// <summary>
		/// Gets the configuration XmlDocument, used by JSToolsWebConfigHandler.
		/// </summary>
		internal XmlDocument Document
		{
			get { return _xmlDocument; }
		}


		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Initializes a new JSToolsConfigurationProxy configuration handler instance.
		/// </summary>
		/// <param name="xmlDoc">XmlDocument which is used by the JSToolsConfiguration.</param>
		/// <exception cref="ArgumentNullException">The given XmlDocument contains a null pointer.</exception>
		public JSToolsConfigurationProxy(XmlDocument xmlDoc)
		{
			if (xmlDoc == null)
			{
				throw new ArgumentNullException("xmlDoc", "The given XmlDocument contains a null pointer!");
			}

			_xmlDocument = xmlDoc;
		}


		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------
	}
}

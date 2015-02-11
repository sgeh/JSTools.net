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

namespace JSTools.Config.ScriptFileManagement
{
	/// <summary>
	/// Summary description for JSExceptionHandling.
	/// </summary>
	public class JSScriptFileSectionHandlerFactory : AJSToolsConfigSectionHandlerFactory
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		public const string SECTION_NAME = "scripts";


		/// <summary>
		/// Returns the name of the section.
		/// </summary>
		public override string SectionName
		{
			get { return SECTION_NAME; }
		}


		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		public JSScriptFileSectionHandlerFactory()
		{
		}


		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new configuration handler.
		/// </summary>
		/// <param name="section">Configuration, which should be initialized.</param>
		/// <param name="ownerConfig">Owner (parent) configuration instance.</param>
		public override AJSToolsSection CreateInstance(XmlNode section, IJSToolsConfiguration ownerConfig)
		{
			return new JSScriptFileHandler(section, ownerConfig, SECTION_NAME);
		}
	}
}

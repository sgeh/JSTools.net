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

using JSTools.Config.Session;

namespace JSTools.Config.ScriptFileManagement
{
	/// <summary>
	/// Summary description for JSExceptionHandling.
	/// </summary>
	public class JSScriptFileSectionHandlerFactory : AJSToolsConfigSectionHandlerFactory
	{
		//------------------------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------------------------

		private static string SCRIPT_SECTION_NAME = "scripts";


		/// <summary>
		/// Returns the name of the section.
		/// </summary>
		public override string SectionName
		{
			get { return SCRIPT_SECTION_NAME; }
		}


		//------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//------------------------------------------------------------------------------------------

		public JSScriptFileSectionHandlerFactory()
		{
		}


		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Creates a new writeable JSScriptFileHandler instance.
		/// </summary>
		public override AJSToolsEventHandler CreateWriteableInstance(XmlNode section)
		{
			return new JSScriptFileHandlerWriteable(section, SCRIPT_SECTION_NAME);
		}


		/// <summary>
		/// Creates a new readable JSScriptFileHandler handler.
		/// </summary>
		public override AJSToolsEventHandler CreateInstance(XmlNode section)
		{
			return new JSScriptFileHandler(section, SCRIPT_SECTION_NAME);
		}
	}
}

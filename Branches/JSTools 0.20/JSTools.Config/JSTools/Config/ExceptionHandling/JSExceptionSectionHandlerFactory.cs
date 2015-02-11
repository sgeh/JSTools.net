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
using System.Configuration;
using System.Xml;

using JSTools.Config.Session;

namespace JSTools.Config.ExceptionHandling
{
	/// <summary>
	/// Represents the section handler, which will be called from the JSTools.config file. The
	/// Create methods will return the instances, which represents the &lt;exception&gt; node.
	/// </summary>
	public class JSExceptionSectionHandlerFactory : AJSToolsConfigSectionHandlerFactory
	{
		//------------------------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------------------------

		private const string SECTION_NAME = "exception";


		/// <summary>
		/// Name of the section node.
		/// </summary>
		public override string SectionName
		{
			get { return SECTION_NAME; }
		}


		//------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Initializes a new JSExceptionHandlingSection instance.
		/// </summary>
		public JSExceptionSectionHandlerFactory()
		{
		}


		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSExceptionHandlingWriteable instance.
		/// </summary>
		public override AJSToolsEventHandler CreateWriteableInstance(XmlNode section)
		{
			return new JSExceptionHandlerWriteable(section, SECTION_NAME);
		}


		/// <summary>
		/// Creates a new JSExceptionHandling instance.
		/// </summary>
		public override AJSToolsEventHandler CreateInstance(XmlNode section)
		{
			return new JSExceptionHandler(section, SECTION_NAME);
		}
	}
}

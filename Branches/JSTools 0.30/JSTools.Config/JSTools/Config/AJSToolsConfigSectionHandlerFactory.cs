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

namespace JSTools.Config
{
	/// <summary>
	/// Represents a configuration section in the JSTools configuration file. The SectionName
	/// property of the specified type should be the same as you have specified by the section node name.
	/// </summary>
	/// <example>
	/// If the type "JSTools.Config.ExceptionHandling.JSExceptionHandlingSection" looks like
	/// <code>
	/// public class JSExceptionHandlingSection : AJSToolsConfigSectionHandler
	/// {
	///		public override string SectionName
	///		{
	///			get { return "exception"; }
	///		}
	///		...
	///	}
	/// </code>
	/// you should implement the section node like
	///	&lt;exception type="JSTools.Config.ExceptionHandling.JSExceptionHandlingSection, JSTools" /&gt;
	///	
	///	The parent instance will be set with the SetParent(...) method.
	/// </example>
	public abstract class AJSToolsConfigSectionHandlerFactory
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		/// <summary>
		/// Name of the section node.
		/// </summary>
		public abstract string SectionName
		{
			get;
		}


		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new configuration handler.
		/// </summary>
		/// <param name="section">Configuration, which should be initialized.</param>
		/// <param name="ownerConfig">Owner (parent) configuration instance.</param>
		public abstract AJSToolsSection CreateInstance(XmlNode section, IJSToolsConfiguration ownerConfig);
	}
}

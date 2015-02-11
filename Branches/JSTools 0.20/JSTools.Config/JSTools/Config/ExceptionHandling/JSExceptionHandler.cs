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
using System.Collections;
using System.Text;
using System.Xml;

using JSTools.Config.Session;

namespace JSTools.Config.ExceptionHandling
{
	/// <summary>
	/// Represents an immutable JSExceptionHandling instance of the &lt;exception&gt; configuration section
	/// in the JSTools.net configuration.
	/// </summary>
	public class JSExceptionHandler : AJSExceptionHandler
	{
		//------------------------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Gets/sets the type of error handling on the client side.
		/// </summary>
		public override ErrorHandling Handling
		{
			get { return _errorHandling; }
			set
			{
				// create writeable instance
				OwnerConfiguration.WriteableInstance.ErrorHandling.Handling = value;
			}
		}


		/// <summary>
		/// Gets/sets the name of the required module.
		/// </summary>
		public override string RequiredModule
		{
			get { return _requiredModule; }
			set
			{
				// create writeable instance
				OwnerConfiguration.WriteableInstance.ErrorHandling.RequiredModule = value;
			}
		}


		//------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Initializes a new immutable JSExceptionHandlingWriteable instance.
		/// </summary>
		/// <param name="exceptionNode">XmlNode, which contains the configuration data.</param>
		/// <param name="nodeName">Contains the name of the representing node.</param>
		public JSExceptionHandler(XmlNode exceptionNode, string nodeName) : base(exceptionNode, nodeName)
		{
		}


		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------
	}
}

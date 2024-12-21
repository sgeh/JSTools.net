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
using System.Text;
using System.Xml;

namespace JSTools.Config.ScriptFileManagement
{
	/// <summary>
	/// Represents a &lt;module&gt; node of the configuration. To create a new JSModule, you should
	/// use the JSToolsConfiguration.CreateModule() method.
	/// </summary>
	public class JSModule : AJSModule
	{
		//------------------------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Returns the name of a file, which contains the release script of this module.
		/// </summary>
		/// <exception cref="InvalidOperationException">The current node was not assigned to a configuration section. Before using this instance you have to import it.</exception>
		public override string ReleaseFile
		{
			get { return _release; }
			set { WriteableInstance.ReleaseFile = value; }
		}


		//------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Initializes a new immutable JavaScript configuration module.
		/// </summary>
		/// <param name="moduleNode">XmlNode which contians the module node.</param>
		/// <param name="parentConfiguration">Parent JSToolsConfiguration of this node.</param>
		/// <exception cref="ArgumentNullException">An argument contains a null reference.</exception>
		internal JSModule(XmlNode moduleNode, AJSScriptFileHandler parentConfiguration) : base(moduleNode, parentConfiguration)
		{
		}


		/// <summary>
		/// Initializes a new immutable JavaScript configuration module.
		/// </summary>
		/// <param name="moduleName">Name of the new module.</param>
		/// <param name="parentConfiguration">Parent JSToolsConfiguration of this node.</param>
		/// <exception cref="ArgumentNullException">An argument contains a null reference.</exception>
		internal JSModule(string moduleName, AJSScriptFileHandler parentConfiguration) : base(moduleName, parentConfiguration)
		{
		}

		
		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Appends a new module relation to this module.
		/// </summary>
		/// <param name="relationModule">Relation module you'd like to add.</param>
		/// <exception cref="ArgumentNullException">The specified relation module contains a null reference.</exception>
		/// <exception cref="InvalidOperationException">The specified module is not contained in the owner section of this module.</exception>
		public override void AppendRelation(AJSModule relationModule)
		{
			WriteableInstance.AppendRelation(relationModule);
		}


		/// <summary>
		/// Removes the specified module from the relation collection.
		/// </summary>
		/// <param name="relationModule">Relation module you'd like to remove.</param>
		/// <exception cref="InvalidOperationException">The specified module is not a valid relation.</exception>
		/// <exception cref="ArgumentException">The specified module has an other owner section than this module has.</exception>
		/// <exception cref="ArgumentNullException">The specified relation module contains a null reference.</exception>
		public override void RemoveRelation(AJSModule relationModule)
		{
			WriteableInstance.RemoveRelation(relationModule);
		}


		/// <summary>
		/// Creates a new AJSModuleContainer instance for internal use.
		/// </summary>
		/// <returns>Returns the created AJSModuleContainer.</returns>
		protected override AJSModuleContainer CreateModuleContainer()
		{
			return new JSModuleContainer(this);
		}


		/// <summary>
		/// Creates a new AJSScriptContainer instance for internal use.
		/// </summary>
		/// <returns>Returns the created AJSScriptContainer.</returns>
		protected override AJSScriptContainer CreateScriptContainer()
		{
			return new JSScriptContainer(this);
		}
	}
}

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
///     <since>JSTools.dll 0.1.1</since>
/// </file>

using System;
using System.Collections;
using System.Text;
using System.Xml;

using JSTools.Config.Session;
using JSTools.Xml;

namespace JSTools.Config.ScriptFileManagement
{
	/// <summary>
	/// Represents an immutable module container.
	/// </summary>
	public class JSModuleContainer : AJSModuleContainer
	{
		//------------------------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Gets/sets the module at the specified index.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">The index is less than zero or equal or higher than the module count.</exception>
		/// <exception cref="ArgumentNullException">The specified module contains a null reference.</exception>
		/// <exception cref="InvalidOperationException">A module with the specified name was already registered.</exception>
		/// <exception cref="NotImportedException">The specified module was not imported into the current owner section.</exception>
		public override AJSModule this[int index]
		{
			get
			{
				if (!IsValidIndex(index))
					throw new ArgumentOutOfRangeException("index", "The index is less than zero or equal or higher than the module count.");

				return (AJSModule)_childModules[index];
			}
			set { WriteableInstance[index] = value; }
		}


		
		/// <summary>
		/// Gets/sets the module with the specified name. The module with the specified name will be replaced
		/// with the given value.
		/// </summary>
		/// <exception cref="ArgumentNullException">The specified module contains a null reference.</exception>
		/// <exception cref="ArgumentExcpetion">Could not find a module with the specified name.</exception>
		/// <exception cref="InvalidOperationException">A module with the specified name was already registered.</exception>
		/// <exception cref="NotImportedException">The specified module was not imported into the current owner section.</exception>
		public override AJSModule this[string fullModuleName]
		{
			get
			{
				if (fullModuleName == null)
					throw new NullReferenceException("The specified module name contains a null reference!");

				return Contains(fullModuleName) ? this[IndexOf(fullModuleName)] : null;
			}
			set { WriteableInstance[fullModuleName] = value; }
		}


		//------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSModuleContainer instance.
		/// </summary>
		/// <param name="parentConfiguration">Owner section.</param>
		/// <exception cref="ArgumentNullException">The specified session handler contains a null reference.</exception>
		internal JSModuleContainer(AJSScriptFileHandler parentConfiguration) : base(parentConfiguration)
		{
		}


		/// <summary>
		/// Creates a new JSModuleContainer instance.
		/// </summary>
		/// <param name="parentModule">Module, which contains this collection.</param>
		/// <exception cref="ArgumentNullException">The specified parent module contains a null reference.</exception>
		internal JSModuleContainer(AJSModule parentModule) : base(parentModule)
		{
		}


		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Appends the given JSModule the module collection.
		/// </summary>
		/// <param name="module">JSModule which should be added.</param>
		/// <exception cref="ArgumentNullException">The specified module contains a null reference.</exception>
		/// <exception cref="NotImportedException">The specified module was not imported into the current owner section.</exception>
		/// <exception cref="InvalidOperationException">A module with the specified name was already registered.</exception>
		public override int Add(AJSModule module)
		{
			return WriteableInstance.Add(module);
		}


		/// <summary>
		/// Inserts a JSModule at the specified index.
		/// </summary>
		/// <param name="index">Index to insert.</param>
		/// <param name="moduleToInsert">Module to insert.</param>
		/// <exception cref="ArgumentOutOfRangeException">The index is less than zero or higher than the module count.</exception>
		/// <exception cref="ArgumentNullException">The specified module contains a null reference.</exception>
		/// <exception cref="NotImportedException">The specified module was not imported into the current owner section.</exception>
		/// <exception cref="InvalidOperationException">A module with the specified name was already registered.</exception>
		public override void Insert(int index, AJSModule moduleToInsert)
		{
			WriteableInstance.Insert(index, moduleToInsert);
		}


		/// <summary>
		/// Inserts a JSModule before the specified reference module.
		/// </summary>
		/// <param name="module">Module to insert.</param>
		/// <param name="refModule">The module to insert should be appended before this module.</param>
		/// <exception cref="ArgumentException">Could not find the specified module in this collection.</exception>
		/// <exception cref="ArgumentNullException">A parameter contains a null reference.</exception>
		/// <exception cref="NotImportedException">The specified module was not imported into the current owner section.</exception>
		/// <exception cref="InvalidOperationException">A module with the specified name was already registered.</exception>
		public override void InsertBefore(AJSModule module, AJSModule refModule)
		{
			WriteableInstance.InsertBefore(module, refModule);
		}


		/// <summary>
		/// Inserts a JSModule after the specified reference module.
		/// </summary>
		/// <param name="module">Module to insert.</param>
		/// <param name="refModule">The module to insert should be appended after this module.</param>
		/// <exception cref="ArgumentException">Could not find the specified module in this collection.</exception>
		/// <exception cref="ArgumentNullException">A parameter contains a null reference.</exception>
		/// <exception cref="NotImportedException">The specified module was not imported into the current owner section.</exception>
		/// <exception cref="InvalidOperationException">A module with the specified name was already registered.</exception>
		public override void InsertAfter(AJSModule module, AJSModule refModule)
		{
			WriteableInstance.InsertAfter(module, refModule);
		}


		/// <summary>
		/// Removes a module from the module container.
		/// </summary>
		/// <param name="module">Module, which should be deleted.</param>
		/// <exception cref="ArgumentException">Could not find the specified module in this collection.</exception>
		public override void Remove(AJSModule module)
		{
			WriteableInstance.Remove(module);
		}


		/// <summary>
		/// Removes a module permanently from the module container.
		/// </summary>
		/// <param name="position">Module, which should be deleted.</param>
		/// <exception cref="ArgumentOutOfRangeException">The index is less than zero or equal or higher than the module count.</exception>
		/// <exception cref="ArgumentException">Could not find the specified module in this collection.</exception>
		public override void RemoveAt(int position)
		{
			WriteableInstance.RemoveAt(position);
		}


		/// <summary>
		/// Creates a deep copy of the specified module and inserts it into the global file section.
		/// Afert inserting, the specified module will be removed from this collection.
		/// </summary>
		/// <param name="sourceModuleName">Module to move.</param>
		/// <returns>Returns the copy.</returns>
		/// <exception cref="ArgumentNullException">The specified module name contains a null reference.</exception>
		/// <exception cref="ArgumentException">A module with the specified name is not registered.</exception>
		/// <exception cref="InvalidOperationException">Could not move the specified module to the same parent module.</exception>
		public override AJSModule MoveModule(string sourceModuleName)
		{
			return WriteableInstance.MoveModule(sourceModuleName);
		}


		/// <summary>
		/// Creates a copy of the specified module and inserts it into the global file section.
		/// Afert inserting, the specified module will be removed from this collection.		
		/// </summary>
		/// <param name="sourceModuleName">Child module to move.</param>
		/// <param name="deep">True to copy the child modules, otherwise false.</param>
		/// <returns>Returns the copy.</returns>		
		/// <exception cref="ArgumentNullException">The specified module name contains a null reference.</exception>
		/// <exception cref="ArgumentException">A module with the specified name is not registered.</exception>	
		/// <exception cref="InvalidOperationException">Could not move the specified module to the same parent module.</exception>
		public override AJSModule MoveModule(string sourceModuleName, bool deep)
		{
			return WriteableInstance.MoveModule(sourceModuleName, deep);		
		}


		/// <summary>
		/// Creates a copy of the specified module and inserts it at the specified index into the
		/// global file section. The elements that follow the insertion point move down.
		/// Afert inserting, the specified module will be removed from this collection.		
		/// </summary>
		/// <param name="sourceModuleName">Child module to move.</param>
		/// <param name="deep">True to copy the child modules, otherwise false.</param>
		/// <param name="indexToInsert">Index to insert the module.</param>
		/// <returns>Returns the copy.</returns>
		/// <exception cref="ArgumentNullException">The specified module name contains a null reference.</exception>
		/// <exception cref="ArgumentException">A module with the specified name is not registered.</exception>	
		/// <exception cref="ArgumentOutOfRangeException">The index is less than zero or higher than the module count.</exception>
		/// <exception cref="InvalidOperationException">Could not move the specified module to the same parent module.</exception>
		public override AJSModule MoveModule(string sourceModuleName, bool deep, int indexToInsert)
		{
			return WriteableInstance.MoveModule(sourceModuleName, deep, indexToInsert);
		}


		/// <summary>
		/// Creates a deep copy of the specified module and inserts it into the specified
		/// module. Afert inserting, the specified module will be removed from this collection.
		/// </summary>
		/// <param name="sourceModuleName">Module to move.</param>
		/// <param name="targetModule">Module to insert the copy.</param>
		/// <returns>Returns the copy.</returns>
		/// <exception cref="ArgumentNullException">A given value contains a null reference.</exception>
		/// <exception cref="ArgumentException">A module with the specified name is not registered.</exception>	
		/// <exception cref="InvalidOperationException">Could not move the specified module to the same parent module.</exception>
		public override AJSModule MoveModule(string sourceModuleName, AJSModule targetModule)
		{
			return WriteableInstance.MoveModule(sourceModuleName, targetModule);
		}


		/// <summary>
		/// Creates a copy of the specified module and inserts it into the specified
		/// module. Afert inserting, the specified module will be removed from this collection.
		/// </summary>
		/// <param name="sourceModuleName">Child module to move.</param>
		/// <param name="targetModule">Module to insert the copy.</param>
		/// <param name="deep">True to copy the child modules, otherwise false.</param>
		/// <returns>Returns the copy.</returns>
		/// <exception cref="ArgumentNullException">A given value contains a null reference.</exception>
		/// <exception cref="ArgumentException">A module with the specified name is not registered.</exception>	
		/// <exception cref="InvalidOperationException">Could not move the specified module to the same parent module.</exception>
		public override AJSModule MoveModule(string sourceModuleName, AJSModule targetModule, bool deep)
		{
			return WriteableInstance.MoveModule(sourceModuleName, targetModule, deep);
		}


		/// <summary>
		/// Creates a copy of the specified module and inserts it into the specified
		/// module. The elements that follow the insertion point move down.
		/// Afert inserting, the specified module will be removed from this collection.
		/// </summary>
		/// <param name="sourceModuleName">Child module to move.</param>
		/// <param name="targetModule">Module to insert the copy.</param>		
		/// <param name="deep">True to copy the child modules, otherwise false.</param>
		/// <param name="indexToInsert">Index to insert the module.</param>
		/// <returns>Returns the copy.</returns>
		/// <exception cref="ArgumentNullException">A given value contains a null reference.</exception>
		/// <exception cref="ArgumentException">A module with the specified name is not registered.</exception>	
		/// <exception cref="ArgumentOutOfRangeException">The index is less than zero or higher than the module count.</exception>
		/// <exception cref="InvalidOperationException">Could not move the specified module to the same parent module.</exception>
		public override AJSModule MoveModule(string sourceModuleName, AJSModule targetModule, bool deep, int indexToInsert)
		{
			return WriteableInstance.MoveModule(sourceModuleName, targetModule, deep, indexToInsert);
		}


		/// <summary>
		/// Removes all modules, which are contained in this collection.
		/// </summary>
		public override void Clear()
		{
			WriteableInstance.Clear();
		}

		
		/// <summary>
		/// Creates a new AJSModule instance for internal use.
		/// </summary>
		/// <param name="moduleNode">XmlNode which contains the values of the AJSModule.</param>
		/// <returns>Returns the created AJSModule.</returns>
		protected override AJSModule CreateInnerModule(XmlNode moduleNode)
		{
			return ((AJSScriptFileHandler)OwnerSection).CreateInnerModule(moduleNode);
		}
	}
}

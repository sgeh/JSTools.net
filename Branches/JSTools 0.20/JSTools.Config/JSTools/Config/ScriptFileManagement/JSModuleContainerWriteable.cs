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
	/// Represents an writeable module container.
	/// </summary>
	public class JSModuleContainerWriteable : AJSModuleContainer
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
			set
			{
				if (!IsValidIndex(index))
					throw new ArgumentOutOfRangeException("index", "The index is less than zero or equal or higher than the module count!");

				if (value == null)
					throw new ArgumentNullException("moduleToInsert", "The specified module contains a null reference!");

				if (value.OwnerSection != OwnerSection)
					throw new NotImportedException("The specified module was not imported into the current owner section!");

				if (Contains(value))
					throw new InvalidOperationException("A module with the specified name was already registered!");

				if (_childModules[index] != null)
				{
					((AJSModule)_childModules[index]).FireOnRemoveEvent();
				}
				_childModules[index] = value;
			}
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

				if (!Contains(fullModuleName))
					throw new ArgumentException("Could not find a module with the specified name!", "fullModuleName");

				return this[IndexOf(fullModuleName)];
			}
			set
			{
				if (!Contains(fullModuleName))
					throw new ArgumentException("Could not find a module with the specified name!", "fullModuleName");

				this[IndexOf(fullModuleName)] = value;
			}
		}


		//------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Creates a new writeable JSModuleContainer instance.
		/// </summary>
		/// <param name="parentConfiguration">Owner section.</param>
		/// <exception cref="ArgumentNullException">The specified session handler contains a null reference.</exception>
		internal JSModuleContainerWriteable(AJSScriptFileHandler parentConfiguration) : base(parentConfiguration)
		{
		}


		/// <summary>
		/// Creates a new writeable JSModuleContainer instance.
		/// </summary>
		/// <param name="parentModule">Module, which contains this collection.</param>
		/// <exception cref="ArgumentNullException">The specified parent module contains a null reference.</exception>
		internal JSModuleContainerWriteable(AJSModule parentModule) : base(parentModule)
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
			if (module == null)
				throw new ArgumentNullException("module", "The specified module contains a null reference!");

			return AppendModule(module);
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
			if (index < 0 || index > Count)
				throw new ArgumentOutOfRangeException("index", "The index is less than zero or higher than the module count!");

			if (moduleToInsert == null)
				throw new ArgumentNullException("moduleToInsert", "The specified module contains a null reference!");

			if (Contains(moduleToInsert))
				throw new InvalidOperationException("A module with the specified name was already registered!");

			_childModules.Insert(index, moduleToInsert);
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
			if (module == null)
				throw new ArgumentNullException("module", "The specified module contains a null reference!");

			if (refModule == null)
				throw new ArgumentNullException("refModule", "The specified ref module contains a null reference!");

			if (!Contains(refModule))
				throw new ArgumentException("Could not find the specified module in this collection!", "refModule");

			if (Contains(module))
				throw new InvalidOperationException("A module with the specified name was already registered!");

			int insertIndex = IndexOf(refModule) - 1;

			if (insertIndex < 0)
			{
				Insert(0, module);
			}
			else
			{
				Insert(insertIndex, module);
			}
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
			if (!Contains(refModule))
				throw new ArgumentException("Could not find the specified module in this collection!", "refModule");

			Insert(IndexOf(refModule) + 1, module);
		}


		/// <summary>
		/// Removes a module from the module container.
		/// </summary>
		/// <param name="module">Module, which should be deleted.</param>
		/// <exception cref="ArgumentException">Could not find the specified module in this collection.</exception>
		public override void Remove(AJSModule module)
		{
			if (!Contains(module))
				throw new ArgumentException("Could not find the specified module in this collection!", "module");

			RemoveAt(IndexOf(module)); 
		}


		/// <summary>
		/// Removes a module permanently from the module container.
		/// </summary>
		/// <param name="position">Module, which should be deleted.</param>
		/// <exception cref="ArgumentOutOfRangeException">The index is less than zero or equal or higher than the module count.</exception>
		/// <exception cref="ArgumentException">Could not find the specified module in this collection.</exception>
		public override void RemoveAt(int position)
		{
			if (!IsValidIndex(position))
				throw new ArgumentOutOfRangeException("position", "The index is less than zero or equal or higher than the module count!");

			if (_childModules[position] != null)
			{
				((AJSModule)_childModules[position]).FireOnRemoveEvent();
			}
			_childModules.RemoveAt(position);
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
			return MoveModule(sourceModuleName, true);
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
			return MoveModule(sourceModuleName, deep, OwnerSection.ChildModules.Count);
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
			if (sourceModuleName == null)
				throw new ArgumentNullException("sourceModuleName", "The specified module name contains a null reference!");

			AJSModule module = this[sourceModuleName];

			if (module == null)
				throw new ArgumentException("A module with the specified name is not registered!", "sourceModuleName");

			if (module.ParentModule == null)
				throw new InvalidOperationException("Could not move the specified module to the same parent module!");

			return CopyAndRemoveModule(module, OwnerSection.ChildModules, deep, indexToInsert);
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
			return MoveModule(sourceModuleName, targetModule, true);
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
			if (targetModule == null)
				throw new ArgumentNullException("targetModule", "The specified target module contains a null reference!");

			return MoveModule(sourceModuleName, targetModule, deep, targetModule.ChildModules.Count);
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
			if (targetModule == null)
				throw new ArgumentNullException("targetModule", "The specified target module contains a null reference!");

			if (sourceModuleName == null)
				throw new ArgumentNullException("sourceModuleName", "The specified module name contains a null reference!");

			AJSModule module = this[sourceModuleName];

			if (module == null)
				throw new ArgumentException("A module with the specified name is not registered!", "sourceModuleName");

			if (targetModule == module.ParentModule)
				throw new InvalidOperationException("Could not move the specified module to the same parent module!");

			return CopyAndRemoveModule(module, targetModule.ChildModules, deep, indexToInsert);
		}


		/// <summary>
		/// Removes all modules, which are contained in this collection.
		/// </summary>
		public override void Clear()
		{
			foreach (AJSModule childModule in _childModules)
			{
					childModule.FireOnRemoveEvent();
			}
			_childModules.Clear();
		}

		
		/// <summary>
		/// Creates a new AJSModule instance for internal use.
		/// </summary>
		/// <param name="moduleNode">XmlNode which contains the values of the AJSModule.</param>
		/// <returns>Returns the created AJSModule.</returns>
		protected override AJSModule CreateInnerModule(XmlNode moduleNode)
		{
			return ((AJSScriptFileHandler)OwnerSection).CreateModule(moduleNode);
		}


		/// <summary>
		/// Creates a copy of the specified module and inserts it into the specified
		/// collection. The elements that follow the insertion point move down.
		/// Afert inserting, the specified module will be removed from this collection.
		/// </summary>
		/// <param name="moduleToMove">Module element to move.</param>
		/// <param name="targetContainer">Target element container.</param>
		/// <param name="deep">True to copy the child modules, otherwise false.</param>
		/// <param name="indexToInsert">Index to insert the module.</param>
		/// <returns>Returns the moved module instance.</returns>
		private AJSModule CopyAndRemoveModule(AJSModule moduleToMove, AJSModuleContainer targetContainer, bool deep, int indexToInsert)
		{
			// create deep copy
			AJSModule newModule = moduleToMove.Clone(targetContainer.OwnerSection, deep);

			// append created module to the target collection
			targetContainer.Insert(indexToInsert, newModule);

			// fire on move event, so relations will be adjusted
			moduleToMove.FireOnMoveEvent(newModule);

			// remove old module from this collection
			Remove(moduleToMove);

			// return created module
			return newModule;
		}
	}
}

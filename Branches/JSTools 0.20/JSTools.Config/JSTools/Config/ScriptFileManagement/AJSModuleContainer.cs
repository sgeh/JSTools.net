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
	/// Represents a module container. Operations like Add, MoveModule or InsertModule are provided by
	/// this class.
	/// </summary>
	public abstract class AJSModuleContainer : AJSToolsFileManagementContainer, IList
	{
		//------------------------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------------------------

		private		const	bool						IS_SYNCHRONIZED		= false;
		private		const	bool						IS_READ_ONLY		= false;
		private		const	bool						IS_FIXED_SIZE		= false;

		protected			ArrayList					_childModules		= new ArrayList();


		/// <summary>
		/// Returns the parent module.
		/// </summary>
		public AJSModule OwnerModule
		{
			get { return (ParentSection as AJSModule); }
		}


		/// <summary>
		/// Returns the writeable instance of the current instance.
		/// </summary>
		protected AJSModuleContainer WriteableInstance
		{
			get
			{
				if (OwnerModule != null)
				{
					return OwnerModule.ChildModules;
				}
				else
				{
					return OwnerSection.ChildModules;
				}
			}
		}


		/// <summary>
		/// Gets/sets the module at the specified index.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">The index is less than zero or equal or higher than the module count.</exception>
		/// <exception cref="ArgumentNullException">The specified module contains a null reference.</exception>
		/// <exception cref="InvalidOperationException">A module with the specified name was already registered.</exception>
		/// <exception cref="ArgumentException">The specified module is not a valid AJSModule instance.</exception>
		/// <exception cref="NotImportedException">The specified module was not imported into the current owner section.</exception>
		object IList.this[int index]
		{
			get { return this[index]; }
			set
			{
				if (value as AJSModule == null)
					throw new ArgumentException("The specified module is not a valid AJSModule instance!");

				this[index] = (value as AJSModule);
			}
		}


		/// <summary>
		/// Gets/sets the module at the specified index.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">The index is less than zero or equal or higher than the module count.</exception>
		/// <exception cref="ArgumentNullException">The specified module contains a null reference.</exception>
		/// <exception cref="InvalidOperationException">A module with the specified name was already registered.</exception>
		/// <exception cref="NotImportedException">The specified module was not imported into the current owner section.</exception>
		public abstract AJSModule this[int index]
		{
			get;
			set;
		}


		/// <summary>
		/// Gets/sets the module with the specified name. The module with the specified name will be replaced
		/// with the given value.
		/// </summary>
		/// <exception cref="ArgumentNullException">The specified module contains a null reference.</exception>
		/// <exception cref="ArgumentException">Could not find a module with the specified name.</exception>
		/// <exception cref="InvalidOperationException">A module with the specified name was already registered.</exception>
		/// <exception cref="NotImportedException">The specified module was not imported into the current owner section.</exception>
		public abstract AJSModule this[string fullModuleName]
		{
			get;
			set;
		}


		/// <summary>
		/// Returns the count of modules.
		/// </summary>
		public int Count
		{
			get { return _childModules.Count; }
		}


		/// <summary>
		/// Returns true, if all operations are thread save.
		/// </summary>
		public bool IsSynchronized
		{
			get { return IS_SYNCHRONIZED; }
		}


		/// <summary>
		/// Returns true, it this container has a fixed size, otherwise false.
		/// </summary>
		public bool IsFixedSize
		{
			get { return IS_FIXED_SIZE; }
		}


		/// <summary>
		/// Returns true, if this container is read only.
		/// </summary>
		public bool IsReadOnly
		{
			get { return IS_READ_ONLY; }
		}


		/// <summary>
		/// Returns true, if the operations a thread save.
		/// </summary>
		public object SyncRoot
		{
			get { return this; }
		}


		//------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Creates a new AJSModuleContainer instance.
		/// </summary>
		internal AJSModuleContainer(AJSModule parentSection) : base(parentSection)
		{
		}


		/// <summary>
		/// Creates a new AJSModuleContainer instance.
		/// </summary>
		internal AJSModuleContainer(AJSScriptFileHandler parentSection) : base(parentSection)
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
		int IList.Add(object module)
		{
			if (module as AJSModule == null)
				throw new ArgumentException("The specified module is not a valid AJSModule instance!", "module");

			return Add(module as AJSModule);
		}


		/// <summary>
		/// Inserts a JSModule at the specified index.
		/// </summary>
		/// <param name="index">Index to insert.</param>
		/// <param name="moduleToInsert">Module to insert.</param>
		/// <exception cref="ArgumentOutOfRangeException">The index is less than zero or higher than the module count.</exception>
		/// <exception cref="NullReferenceException">The specified module contains a null reference.</exception>
		/// <exception cref="InvalidOperationException">A module with the specified name was already registered.</exception>
		/// <exception cref="NotImportedException">The specified module was not imported into the current owner section.</exception>
		/// <exception cref="ArgumentException">The specified module is not a valid AJSModule instance.</exception>
		void IList.Insert(int index, object moduleToInsert)
		{
			if (moduleToInsert as AJSModule == null)
				throw new ArgumentException("The specified module is not a valid AJSModule instance!", "moduleToInsert");

			Insert(index, (moduleToInsert as AJSModule));
		}


		/// <summary>
		/// Checks if the specified module is contained in this collection.
		/// </summary>
		/// <param name="module">JSModule to check.</param>
		/// <returns>Returns true, if the specified JSModule instance is contained in this collection.</returns>
		/// <exception cref="ArgumentException">The specified module is not a valid AJSModule instance.</exception>
		bool IList.Contains(object module)
		{
			if (module as AJSModule == null)
				throw new ArgumentException("The specified module is not a valid AJSModule instance!", "module");

			return Contains(module as AJSModule);
		}

		
		/// <summary>
		/// Determines the index of the specified item in the container. Returns -1 if the module was not found
		/// in this container.
		/// </summary>
		/// <param name="module">JSModule to get the index.</param>
		/// <returns>Returns the index of the requested module.</returns>
		/// <exception cref="ArgumentException">The specified module is not a valid AJSModule instance.</exception>
		int IList.IndexOf(object module)
		{
			if (module as AJSModule == null)
				throw new ArgumentException("The specified module contains a null reference!", "module");

			return IndexOf(module as AJSModule);
		}


		/// <summary>
		/// Copies the elements of this collection into the specified destination collection.
		/// </summary>
		/// <param name="array">The one-dimensional Array that is the destination of the elements.</param>
		/// <param name="index">The zero-based index in array at which copying begins.</param>
		/// <exception cref="ArgumentOutOfRangeException">The index is less than zero or equal or higher than the module count.</exception>
		void ICollection.CopyTo(Array array, int index)
		{
			CopyTo(array, index);
		}


		/// <summary>
		/// Removes a module from the module container.
		/// </summary>
		/// <param name="module">Module, which should be deleted.</param>
		/// <exception cref="ArgumentException">The specified module is not a valid AJSModule instance.</exception>
		void IList.Remove(object module)
		{
			if (module as AJSModule == null)
				throw new ArgumentException("The specified module contains a null reference!", "module");

			Remove(module as AJSModule);
		}


		/// <summary>
		/// Removes a module from the module container.
		/// </summary>
		/// <param name="position">Module, which should be deleted.</param>
		/// <exception cref="ArgumentOutOfRangeException">The index is less than zero or equal or higher than the module count.</exception>
		void IList.RemoveAt(int position)
		{
			RemoveAt(position);
		}


		/// <summary>
		/// Gets an enumerator for this module container.
		/// </summary>
		/// <returns>Returns a new enumerator for navigating through the container.</returns>
		public override IEnumerator GetEnumerator()
		{
			return new JSFileSectionEnumerator(this);
		}


		/// <summary>
		/// Appends the given JSModule the module collection.
		/// </summary>
		/// <param name="module">JSModule which should be added.</param>
		/// <exception cref="ArgumentNullException">The specified module contains a null reference.</exception>
		/// <exception cref="NotImportedException">The specified module was not imported into the current owner section.</exception>
		/// <exception cref="InvalidOperationException">A module with the specified name was already registered.</exception>
		public abstract int Add(AJSModule module);


		/// <summary>
		/// Inserts a JSModule at the specified index.
		/// </summary>
		/// <param name="index">Index to insert.</param>
		/// <param name="moduleToInsert">Module to insert.</param>
		/// <exception cref="ArgumentOutOfRangeException">The index is less than zero or higher than the module count.</exception>
		/// <exception cref="ArgumentNullException">The specified module contains a null reference.</exception>
		/// <exception cref="NotImportedException">The specified module was not imported into the current owner section.</exception>
		/// <exception cref="InvalidOperationException">A module with the specified name was already registered.</exception>
		public abstract void Insert(int index, AJSModule moduleToInsert);


		/// <summary>
		/// Inserts a JSModule before the specified reference module.
		/// </summary>
		/// <param name="module">Module to insert.</param>
		/// <param name="refModule">The module to insert should be appended before this module.</param>
		/// <exception cref="ArgumentException">Could not find the specified module in this collection.</exception>
		/// <exception cref="ArgumentNullException">A parameter contains a null reference.</exception>
		/// <exception cref="NotImportedException">The specified module was not imported into the current owner section.</exception>
		/// <exception cref="InvalidOperationException">A module with the specified name was already registered.</exception>
		public abstract void InsertBefore(AJSModule module, AJSModule refModule);


		/// <summary>
		/// Inserts a JSModule after the specified reference module.
		/// </summary>
		/// <param name="module">Module to insert.</param>
		/// <param name="refModule">The module to insert should be appended after this module.</param>
		/// <exception cref="ArgumentException">Could not find the specified module in this collection.</exception>
		/// <exception cref="ArgumentNullException">A parameter contains a null reference.</exception>
		/// <exception cref="NotImportedException">The specified module was not imported into the current owner section.</exception>
		/// <exception cref="InvalidOperationException">A module with the specified name was already registered.</exception>
		public abstract void InsertAfter(AJSModule module, AJSModule refModule);


		/// <summary>
		/// Removes a module from the module container.
		/// </summary>
		/// <param name="module">Module, which should be deleted.</param>
		/// <exception cref="ArgumentException">Could not find the specified module in this collection.</exception>
		public abstract void Remove(AJSModule module);


		/// <summary>
		/// Removes a module from the module container.
		/// </summary>
		/// <param name="position">Module, which should be deleted.</param>
		/// <exception cref="ArgumentOutOfRangeException">The index is less than zero or equal or higher than the module count.</exception>
		/// <exception cref="ArgumentException">Could not find the specified module in this collection.</exception>
		public abstract void RemoveAt(int position);


		/// <summary>
		/// Copies the elements of this collection into the specified destination collection.
		/// </summary>
		/// <param name="destination">The one-dimensional Array that is the destination of the elements.</param>
		/// <param name="index">The zero-based index in array at which copying begins.</param>
		/// <exception cref="ArgumentNullException">Destination is a null reference.</exception>
		/// <exception cref="ArgumentOutOfRangeException">The index is less than zero or equal or higher than the module count.</exception>
		/// <exception cref="ArgumentException">The specified destination list is null or read only.</exception>
		public void CopyTo(Array destination, int index)
		{
			if (destination == null)
				throw new ArgumentNullException("destination", "Destination is a null reference!");

			if (destination.IsReadOnly)
				throw new ArgumentException("The specified destination list is null or read only!", "destination");

			if (!IsValidIndex(index))
				throw new ArgumentOutOfRangeException("index", "The index is less than zero or equal or higher than the module count!");

			_childModules.CopyTo(destination, index);
		}


		/// <summary>
		/// Copies the specified module to the destination container.
		/// </summary>
		/// <param name="destination">The one-dimensional Array that is the destination of the elements.</param>
		/// <param name="moduleToCopy">The zero-based index in array at which copying begins.</param>
		/// <exception cref="ArgumentNullException">Destination is a null reference.</exception>
		/// <exception cref="ArgumentException">Could not find the specified module.</exception>
		public void CopyTo(AJSModuleContainer destination, string moduleToCopy)
		{
			if (!Contains(moduleToCopy))
				throw new ArgumentException("Could not find the specified module!", moduleToCopy);

			if (destination == null)
				throw new ArgumentNullException("destination", "Destination is a null reference!");

			CopyTo(destination, moduleToCopy, destination.Count);
		}


		/// <summary>
		/// Copies the specified module to the destination container.
		/// </summary>
		/// <param name="destination">The one-dimensional Array that is the destination of the elements.</param>
		/// <param name="moduleToCopy">The zero-based index in array at which copying begins.</param>
		/// <param name="index">Index to insert the copied module.</param>
		/// <exception cref="ArgumentNullException">Destination is a null reference.</exception>
		/// <exception cref="ArgumentException">Could not find the specified module.</exception>
		/// <exception cref="InvalidOperationException">Could not copy a module to the same owner section instance. Use the MoveModule procedure instead.</exception>
		/// <exception cref="ArgumentOutOfRangeException">The index is less than zero or equal or higher than the module count.</exception>
		public void CopyTo(AJSModuleContainer destination, string moduleToCopy, int index)
		{
			if (!Contains(moduleToCopy))
				throw new ArgumentException("Could not find the specified module!", moduleToCopy);

			if (destination == null)
				throw new ArgumentNullException("destination", "Destination is a null reference!");

			if (destination.OwnerSection == OwnerSection)
				throw new InvalidOperationException("Could not copy a module to the same owner section instance! Use the MoveModule procedure instead.");

			destination[index] = this[moduleToCopy].Clone((AJSScriptFileHandler)destination.OwnerSection);
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
		public abstract AJSModule MoveModule(string sourceModuleName);


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
		public abstract AJSModule MoveModule(string sourceModuleName, bool deep);


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
		public abstract AJSModule MoveModule(string sourceModuleName, bool deep, int indexToInsert);


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
		public abstract AJSModule MoveModule(string sourceModuleName, AJSModule targetModule);


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
		public abstract AJSModule MoveModule(string sourceModuleName, AJSModule targetModule, bool deep);


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
		public abstract AJSModule MoveModule(string sourceModuleName, AJSModule targetModule, bool deep, int indexToInsert);


		/// <summary>
		/// Removes all modules, which are contained in this collection.
		/// </summary>
		public abstract void Clear();


		/// <summary>
		/// Checks if the specified module is contained in this collection.
		/// </summary>
		/// <param name="module">JSModule to check.</param>
		/// <returns>Returns true, if the specified JSModule instance is contained in this collection.</returns>
		/// <exception cref="ArgumentNullException">The specified module contains a null reference.</exception>
		public bool Contains(AJSModule module)
		{
			if (module == null)
				throw new ArgumentNullException("module", "The specified module contains a null reference!");

			return _childModules.Contains(module);
		}


		/// <summary>
		/// Checks if the specified module is contained in this collection.
		/// </summary>
		/// <param name="fullModuleName">JSModule to check.</param>
		/// <returns>Returns true, if the specified JSModule instance is contained in this collection.</returns>
		/// <exception cref="ArgumentNullException">The specified module name contains a null reference.</exception>
		public bool Contains(string fullModuleName)
		{
			if (fullModuleName == null)
				throw new ArgumentNullException("fullModuleName", "The specified module name contains a null reference!");

			return (IndexOf(fullModuleName) != -1);
		}


		/// <summary>
		/// Determines the index of a specific item in this collection. Returns -1 if nothing was found.
		/// </summary>
		/// <param name="module">Module to search.</param>
		/// <returns>Returns -1 if nothing was found. Otherwise the expected index.</returns>
		/// <exception cref="ArgumentNullException">The specified module contains a null reference.</exception>
		public int IndexOf(AJSModule module)
		{
			if (module == null)
				throw new ArgumentNullException("module", "The specified module contains a null reference!");

			return _childModules.IndexOf(module);
		}


		/// <summary>
		/// Determines the index of a specific item in this collection.
		/// </summary>
		/// <param name="moduleName">Module to search.</param>
		/// <returns>Returns -1 if nothing was found. Otherwise the expected index.</returns>
		/// <exception cref="ArgumentNullException">The specified module name contains a null reference.</exception>
		public int IndexOf(string moduleName)
		{
			if (moduleName == null)
				throw new ArgumentNullException("moduleName", "The specified module name contains a null reference!");

			int index = _childModules.Count - 1;

			for ( ; index > -1 && this[index].Name != moduleName; --index)
			{
			}
			return index;
		}


		/// <summary>
		/// Appends a new JSModule to the container, without creating a writeable instance.
		/// </summary>
		/// <param name="moduleNode">Node, which contains the module informations.</param>
		/// <exception cref="InvalidOperationException">A module with the specified name was already registered.</exception>
		internal void AppendInnerModule(XmlNode moduleNode)
		{
			if (moduleNode == null)
				throw new ArgumentNullException("moduleNode", "The specified module node contains a null reference!");

			AppendModule(CreateInnerModule(moduleNode));
		}


		/// <summary>
		/// Creates a new AJSModule instance for internal use.
		/// </summary>
		/// <param name="moduleNode">XmlNode which contains the values of the AJSModule.</param>
		/// <returns>Returns the created AJSModule.</returns>
		protected abstract AJSModule CreateInnerModule(XmlNode moduleNode);


		/// <summary>
		/// Appends the specified module to this collection.
		/// </summary>
		/// <param name="toAppend">Module to append.</param>
		/// <returns>The position into which the new element was inserted.</returns>
		/// <exception cref="InvalidOperationException">A module with the specified name was already registered.</exception>
		/// <exception cref="NotImportedException">The specified module was not imported into the current owner section.</exception>
		protected int AppendModule(AJSModule toAppend)
		{
			if (toAppend.OwnerSection != OwnerSection)
				throw new NotImportedException("The specified module was not imported into the current owner section!");

			if (Contains(toAppend))
				throw new InvalidOperationException("A module with the specified name was already registered!");

			// set parent instance
			toAppend.SetParent(ParentSection);

			return _childModules.Add(toAppend);
		}


		/// <summary>
		/// Checks the given index for validity.
		/// </summary>
		/// <returns>Returns true, if the specified index is valid.</returns>
		/// <param name="index">Index to check.</param>
		protected bool IsValidIndex(int index)
		{
			return (index > -1 && index < Count);
		}
	}
}

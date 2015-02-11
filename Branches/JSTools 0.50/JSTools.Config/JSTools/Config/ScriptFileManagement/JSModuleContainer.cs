/*
 * JSTools.Config.dll / JSTools.net - A framework for JavaScript/ASP.NET applications.
 * Copyright (C) 2005  Silvan Gehrig
 *
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
 *
 * Author:
 *  Silvan Gehrig
 */

using System;
using System.Collections;

namespace JSTools.Config.ScriptFileManagement
{
	/// <summary>
	/// Represents a module container.
	/// </summary>
	public class JSModuleContainer : ICollection, IEnumerable
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private ArrayList _childModules = null;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets a value indicating whether access to the ICollection is synchronized.
		/// </summary>
		bool ICollection.IsSynchronized
		{
			get { return _childModules.IsSynchronized; }
		}

		/// <summary>
		/// Gets an object that can be used to synchronize access to the ICollection.
		/// </summary>
		object ICollection.SyncRoot
		{
			get { return _childModules.SyncRoot; }
		}

		/// <summary>
		/// Gets the module at the specified index.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">The index is less than zero or equal or higher than the module count.</exception>
		/// <exception cref="ArgumentNullException">The specified module contains a null reference.</exception>
		public JSModule this[int index]
		{
			get
			{
				if (!IsValidIndex(index))
					throw new ArgumentOutOfRangeException("index", "The index is less than zero or equal or higher than the module count.");

				return (JSModule)_childModules[index];
			}
		}

		
		/// <summary>
		/// Gets the module with the specified name. The module with the specified name will be replaced
		/// with the given value.
		/// </summary>
		/// <exception cref="ArgumentNullException">The specified module contains a null reference.</exception>
		public JSModule this[string moduleName]
		{
			get
			{
				if (moduleName == null)
					throw new ArgumentNullException("moduleName", "The specified module name contains a null reference.");

				return Contains(moduleName) ? this[IndexOf(moduleName)] : null;
			}
		}

		/// <summary>
		/// Returns the count of modules.
		/// </summary>
		public int Count
		{
			get { return _childModules.Count; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSModuleContainer instance.
		/// </summary>
		/// <param name="childModules">Child modules to manage.</param>
		/// <exception cref="ArgumentNullException">The given array contains a null reference.</exception>
		internal JSModuleContainer(JSModule[] childModules)
		{
			if (childModules == null)
				throw new ArgumentNullException("childModules", "The given array contains a null reference.");

			_childModules = new ArrayList(childModules.Length);

			foreach (JSModule module in childModules)
			{
				_childModules.Add(module);
			}
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets an enumerator for this module container.
		/// </summary>
		/// <returns>Returns a new enumerator for navigating through the container.</returns>
		public IEnumerator GetEnumerator()
		{
			return _childModules.GetEnumerator();
		}

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
				throw new ArgumentNullException("destination", "Destination is a null reference.");

			if (destination.IsReadOnly)
				throw new ArgumentException("The specified destination list is null or read only.", "destination");

			if (!IsValidIndex(index))
				throw new ArgumentOutOfRangeException("index", "The index is less than zero or equal or higher than the module count.");

			_childModules.CopyTo(destination, index);
		}

		/// <summary>
		/// Checks if the specified module is contained in this collection.
		/// </summary>
		/// <param name="module">JSModule to check.</param>
		/// <returns>Returns true, if the specified JSModule instance is contained in this collection.</returns>
		/// <exception cref="ArgumentNullException">The specified module contains a null reference.</exception>
		public bool Contains(JSModule module)
		{
			if (module == null)
				throw new ArgumentNullException("module", "The specified module contains a null reference.");

			return (IndexOf(module) != -1);
		}

		/// <summary>
		/// Checks if the specified module is contained in this collection.
		/// </summary>
		/// <param name="moduleName">JSModule to check.</param>
		/// <returns>Returns true, if the specified JSModule instance is contained in this collection.</returns>
		/// <exception cref="ArgumentNullException">The specified module name contains a null reference.</exception>
		public bool Contains(string moduleName)
		{
			if (moduleName == null)
				throw new ArgumentNullException("moduleName", "The specified module name contains a null reference.");

			return (IndexOf(moduleName) != -1);
		}

		/// <summary>
		/// Determines the index of a specific item in this collection. Returns -1 if nothing was found.
		/// </summary>
		/// <param name="module">Module to search.</param>
		/// <returns>Returns -1 if nothing was found. Otherwise the expected index.</returns>
		/// <exception cref="ArgumentNullException">The specified module contains a null reference.</exception>
		public int IndexOf(JSModule module)
		{
			if (module == null)
				throw new ArgumentNullException("module", "The specified module contains a null reference.");

			int index = Count - 1;

			for ( ; index > -1 && this[index] != module; --index)
			{
			}
			return index;
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
				throw new ArgumentNullException("moduleName", "The specified module name contains a null reference.");

			int index = Count - 1;

			for ( ; index > -1 && this[index].Name != moduleName; --index)
			{
			}
			return index;
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

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
	/// Represents a script container. Operations like Add, MoveScript or Insert are provided by
	/// this class.
	/// </summary>
	public class JSScriptContainer : ICollection, IEnumerable
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private ArrayList _childScripts = null;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets a value indicating whether access to the ICollection is synchronized.
		/// </summary>
		bool ICollection.IsSynchronized
		{
			get { return _childScripts.IsSynchronized; }
		}

		/// <summary>
		/// Gets an object that can be used to synchronize access to the ICollection.
		/// </summary>
		object ICollection.SyncRoot
		{
			get { return _childScripts.SyncRoot; }
		}

		/// <summary>
		/// Gets/sets the script at the specified index.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">The index is less than zero or equal or higher than the script count.</exception>
		/// <exception cref="ArgumentNullException">The specified script contains a null reference.</exception>
		public JSScript this[int index]
		{
			get { return (JSScript)_childScripts[index]; }
		}

		/// <summary>
		/// Gets/sets the script with the specified name. The script with the specified name will be replaced
		/// with the given value. 
		/// </summary>
		/// <exception cref="ArgumentNullException">The specified script name contains a null reference.</exception>
		public JSScript this[string scriptName]
		{
			get
			{
				if (scriptName == null)
					throw new ArgumentNullException("scriptName", "The specified script name contains a null reference.");

				return Contains(scriptName) ? this[IndexOf(scriptName)] : null;
			}
		}

		/// <summary>
		/// Returns the count of scripts.
		/// </summary>
		public int Count
		{
			get { return _childScripts.Count; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSScriptContainer instance.
		/// </summary>
		/// <param name="childScripts">Child scripts to manage.</param>
		/// <exception cref="ArgumentNullException">The given array contains a null reference.</exception>
		internal JSScriptContainer(JSScript[] childScripts)
		{
			if (childScripts == null)
				throw new ArgumentNullException("childScripts", "The given array contains a null reference.");

			_childScripts = new ArrayList(childScripts.Length);

			foreach (JSScript script in childScripts)
			{
				_childScripts.Add(script);
			}
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets an enumerator for this script container.
		/// </summary>
		/// <returns>Returns a new enumerator for navigating through the container.</returns>
		public IEnumerator GetEnumerator()
		{
			return _childScripts.GetEnumerator();
		}

		/// <summary>
		/// Copies the elements of this collection into the specified destination collection.
		/// </summary>
		/// <param name="destination">The one-dimensional Array that is the destination of the elements.</param>
		/// <param name="index">The zero-based index in array at which copying begins.</param>
		/// <exception cref="ArgumentNullException">Destination is a null reference.</exception>
		/// <exception cref="ArgumentOutOfRangeException">The index is less than zero or equal or higher than the script count.</exception>
		/// <exception cref="ArgumentException">The specified destination list is null or read only.</exception>
		public void CopyTo(Array destination, int index)
		{
			if (destination == null)
				throw new ArgumentNullException("destination", "Destination is a null reference.");

			if (destination.IsReadOnly)
				throw new ArgumentException("The specified destination list is null or read only.", "destination");

			if (!IsValidIndex(index))
				throw new ArgumentOutOfRangeException("index", "The index is less than zero or equal or higher than the script count.");

			_childScripts.CopyTo(destination, index);
		}

		/// <summary>
		/// Checks if the specified script is contained in this collection.
		/// </summary>
		/// <param name="script">Script to check.</param>
		/// <returns>Returns true, if the specified JSScript instance is contained in this collection.</returns>
		/// <exception cref="ArgumentNullException">The specified script contains a null reference.</exception>
		public bool Contains(JSScript script)
		{
			if (script == null)
				throw new ArgumentNullException("The specified script contains a null reference.");

			return (IndexOf(script) != -1);
		}

		/// <summary>
		/// Checks if the specified script is contained in this collection.
		/// </summary>
		/// <param name="scriptName">JSScript to check.</param>
		/// <returns>Returns true, if the specified JSScript instance is contained in this collection.</returns>
		/// <exception cref="ArgumentNullException">The specified script name contains a null reference.</exception>
		public bool Contains(string scriptName)
		{
			if (scriptName == null)
				throw new ArgumentNullException("The specified script name contains a null reference.");

			return (IndexOf(scriptName) != -1);
		}

		/// <summary>
		/// Determines the index of a specific item in this collection. Returns -1 if nothing was found.
		/// </summary>
		/// <param name="script">Script to search.</param>
		/// <returns>Returns -1 if nothing was found. Otherwise the expected index.</returns>
		/// <exception cref="ArgumentNullException">The specified script contains a null reference.</exception>
		public int IndexOf(JSScript script)
		{
			if (script == null)
				throw new ArgumentNullException("The specified script contains a null reference.");

			int index = Count - 1;

			for ( ; index > -1 && this[index] != script; --index)
			{
			}
			return index;
		}

		/// <summary>
		/// Determines the index of a specific item in this collection.
		/// </summary>
		/// <param name="scriptName">Script to search.</param>
		/// <returns>Returns -1 if nothing was found. Otherwise the expected index.</returns>
		/// <exception cref="ArgumentNullException">The specified script name contains a null reference.</exception>
		public int IndexOf(string scriptName)
		{
			if (scriptName == null)
				throw new ArgumentNullException("The specified script name contains a null reference.");

			int index = Count - 1;

			for ( ; index > -1 && this[index].Name != scriptName; --index)
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

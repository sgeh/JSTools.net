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

using System;
using System.Collections;

using JSTools.Config.ScriptFileManagement;

namespace JSTools.Config.ScriptFileManagement
{
	/// <summary>
	/// Represents a IFileManagementSection enumerator.
	/// </summary>
	public class JSFileSectionEnumerator : IEnumerator
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private int _count = -1;
		private IList _container = null;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the current element in the collection.
		/// </summary>
		/// <exception cref="InvalidCastException">The given list does not contain IFileManagementSection elements.</exception>
		object IEnumerator.Current
		{
			get { return Current; }
		}

		/// <summary>
		/// Gets the current element in the collection.
		/// </summary>
		/// <exception cref="InvalidCastException">The given list does not contain AJSToolsScriptFileSection elements.</exception>
		public AJSToolsScriptFileSection Current
		{
			get
			{
				if (_count < 0 || _count >= _container.Count)
				{
					return null;
				}
				return (AJSToolsScriptFileSection)_container[_count];
			}
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSModuleEnumerator instance.
		/// </summary>
		/// <param name="container">Container to walk througt.</param>
		/// <exception cref="ArgumentNullException">The specified container contains a null reference.</exception>
		internal JSFileSectionEnumerator(IList container)
		{
			if (container == null)
				throw new ArgumentNullException("container", "The specified container contains a null reference.");

			_container = container;
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Sets the enumerator to its initial position, which is before the first element in the collection.
		/// </summary>
		public void Reset()
		{
			_count = -1;
		}

		/// <summary>
		/// Advances the enumerator to the next element of the collection.
		/// </summary>
		/// <returns>Returs true if the enumerator was successfully advanced to the next element, false if the
		/// enumerator has passed the end of the collection.</returns>
		public bool MoveNext()
		{
			if (_count + 1 != _container.Count)
			{
				_count++;
				return true;
			}
			return false;
		}
	}
}

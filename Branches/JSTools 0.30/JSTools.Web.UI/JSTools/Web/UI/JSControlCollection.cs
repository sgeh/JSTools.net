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
using System.Web.UI;

namespace JSTools.Web.UI
{
	/// <summary>
	/// Represents a control collection, which is used to render the configuration sections.
	/// </summary>
	public class JSControlCollection : IEnumerable
	{
		private ArrayList _controls = new ArrayList();


		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the number of controls stored in this collection.
		/// </summary>
		public int Count
		{
			get { return _controls.Count; }
		}


		/// <summary>
		/// Gets the control at the specified index.
		/// </summary>
		public Control this[int index]
		{
			get { return (_controls[index] as Control); }
		}


		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSControlCollection instance.
		/// </sumary>
		public JSControlCollection()
		{
		}


		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Adds the given control to the end of this collection.
		/// </summary>
		/// <param name="toAdd">Control to add.</param>
		/// <exception cref="ArgumentNullException">The given control contains a null reference.</exception>
		public void Add(Control toAdd)
		{
			if (toAdd == null)
				throw new ArgumentNullException("toAdd", "The given control contains a null reference!");

			_controls.Add(toAdd);
		}


		/// <summary>
		/// Adds a range of controls to the end of this collection.
		/// </summary>
		/// <param name="toAdd">Control range to add.</param>
		public void AddRange(Control[] toAdd)
		{
			if (toAdd == null)
				throw new ArgumentNullException("toAdd", "The given control contains a null reference!");

			_controls.AddRange(toAdd);
		}


		/// <summary>
		/// Inserts the given control at the specified index.
		/// </summary>
		/// <param name="toInsert">Control to insert.</param>
		/// <param name="index">Index to insert the control.</param>
		/// <exception cref="ArgumentOutOfRangeException">Index is less than zero.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Index is greater than Count.</exception>
		public void Insert(Control toInsert, int index)
		{
			_controls.Insert(index, toInsert);
		}


		/// <summary>
		/// Determines whether a control is in this collection.
		/// </summary>
		/// <param name="toSearch">Control to search.</param>
		/// <returns>Returns true, if the given control was found.</returns>
		public bool Contains(Control toSearch)
		{
			return _controls.Contains(toSearch);
		}


		/// <summary>
		/// Searches for the specified control.
		/// </summary>
		/// <param name="toSearch">Control to search.</param>
		/// <returns>Returns a zero-based index of the control.</returns>
		public int IndexOf(Control toSearch)
		{
			return _controls.IndexOf(toSearch);
		}


		/// <summary>
		/// Returns an enumerator for the entire control collection.
		/// </summary>
		public IEnumerator GetEnumerator()
		{
			return _controls.GetEnumerator();
		}
	}
}

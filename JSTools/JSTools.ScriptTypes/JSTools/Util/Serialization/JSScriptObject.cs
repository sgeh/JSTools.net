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

namespace JSTools.Util.Serialization
{
	/// <summary>
	/// Represents a container object which represents a javascript object.
	/// This class has a similar behaviour as the javascript Object, but is
	/// not fully ECMA-262 compatible.
	/// </summary>
	public class JSScriptObject : IEnumerable
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private Hashtable _properties = null;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets/sets the properties of this object.
		/// </summary>
		public virtual object this[string name]
		{
			get
			{
				if (name == null)
					throw new ArgumentNullException("name");

				return Properties[name];
			}
			set
			{
				if (name == null)
					throw new ArgumentNullException("name");

				Properties[name] = value;
			}
		}

		/// <summary>
		/// Gets the collection which contains the name/value pairs.
		/// </summary>
		protected Hashtable Properties
		{
			get
			{
				if (_properties == null)
					_properties = new Hashtable();

				return _properties; 
			}
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSScriptObject instance.
		/// </summary>
		internal JSScriptObject()
		{
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		#region IEnumerable Member

		/// <summary>
		/// Creates a new IEnumerator instance which can be used to iterate
		/// throught the list.
		/// </summary>
		/// <returns>Returns the enumerator instance.</returns>
		public IEnumerator GetEnumerator()
		{
			return Properties.GetEnumerator();
		}

		#endregion
	}
}

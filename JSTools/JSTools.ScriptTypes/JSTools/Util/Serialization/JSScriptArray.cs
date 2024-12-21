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
using System.Reflection;
using System.Text;

using JSTools.ScriptTypes;

namespace JSTools.Util.Serialization
{
	/// <summary>
	/// Represents a container object which represents a javascript array.
	/// This class has a similar behaviour as the javascript Array, but is
	/// not fully ECMA-262 compatible.
	/// </summary>
	public class JSScriptArray : JSScriptObject, IList
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private ArrayList _items = null;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets/sets the properties of this object. If the given name
		/// represents a valid array index, the corresponding value of the
		/// array item is returned.
		/// </summary>
		public override object this[string name]
		{
			get
			{
				int convertedNumber = StringToNumber(name);

				if (convertedNumber > -1 && convertedNumber < _items.Count)
					return this[convertedNumber];
				else
					return base[name];
			}
			set
			{
				int convertedNumber = StringToNumber(name);

				if (convertedNumber > -1)
					this[convertedNumber] = value;
				else
					base[name] = value;
			}
		}

		/// <summary>
		/// Gets/sets the value of the specified index.
		/// </summary>
		public object this[int index]
		{
			get { return _items[index]; }
			set
			{
				int newCount = index + 1;

				if (newCount > _items.Count)
				{
					if (newCount > _items.Capacity)
						_items.Capacity += (newCount - _items.Count);

					for (int i = _items.Count; i < newCount; ++i)
						_items.Add(null);
				}
				_items[index] = value; 
			}
		}

		/// <summary>
		/// Gets the length of the current script array. This property
		/// has the same functionality as the Count property.
		/// </summary>
		public int Length
		{
			get { return _items.Count; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSScriptArray instance.
		/// </summary>
		/// <param name="capacity">Capacity of the script array.</param>
		internal JSScriptArray(int capacity)
		{
			_items = new ArrayList(capacity);
		}

		/// <summary>
		/// Creates a new JSScriptArray instance.
		/// </summary>
		internal JSScriptArray()
		{
			_items = new ArrayList();
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		#region IList Member

		/// <summary>
		///  <see cref="IList.IsReadOnly" />
		/// </summary>
		public bool IsReadOnly
		{
			get { return _items.IsReadOnly; }
		}

		/// <summary>
		///  <see cref="IList.RemoveAt" />
		/// </summary>
		/// <param name="index">
		///  <see cref="IList.RemoveAt" />
		/// </param>
		public void RemoveAt(int index)
		{
			_items.RemoveAt(index);
		}

		/// <summary>
		///  <see cref="IList.Insert" />
		/// </summary>
		/// <param name="index">
		///  <see cref="IList.Insert" />
		/// </param>
		/// <param name="value">
		///  <see cref="IList.Insert" />
		/// </param>
		public void Insert(int index, object value)
		{
			_items.Insert(index, value);
		}

		/// <summary>
		///  <see cref="IList.Remove" />
		/// </summary>
		/// <param name="value">
		///  <see cref="IList.Remove" />
		/// </param>
		public void Remove(object value)
		{
			_items.Remove(value);
		}

		/// <summary>
		///  <see cref="IList.Contains" />
		/// </summary>
		/// <param name="value">
		///  <see cref="IList.Contains" />
		/// </param>
		/// <returns>
		///  <see cref="IList.Contains" />
		/// </returns>
		public bool Contains(object value)
		{
			return _items.Contains(value);
		}

		/// <summary>
		///  <see cref="IList.Clear" />
		/// </summary>
		public void Clear()
		{
			_items.Clear();
		}

		/// <summary>
		///  <see cref="IList.IndexOf" />
		/// </summary>
		/// <param name="value">
		///  <see cref="IList.IndexOf" />
		/// </param>
		/// <returns>
		///  <see cref="IList.IndexOf" />
		/// </returns>
		public int IndexOf(object value)
		{
			return _items.IndexOf(value);
		}

		/// <summary>
		///  <see cref="IList.Add" />
		/// </summary>
		/// <param name="value">
		///  <see cref="IList.Add" />
		/// </param>
		/// <returns>
		///  <see cref="IList.Add" />
		/// </returns>
		public int Add(object value)
		{
			return _items.Add(value);
		}

		/// <summary>
		///  <see cref="IList.IsFixedSize" />
		/// </summary>
		public bool IsFixedSize
		{
			get { return _items.IsFixedSize; }
		}

		#endregion

		#region ICollection Member

		/// <summary>
		///  <see cref="ICollection.IsSynchronized"/>
		/// </summary>
		public bool IsSynchronized
		{
			get { return false; }
		}

		/// <summary>
		///  <see cref="ICollection.Count"/>
		/// </summary>
		public int Count
		{
			get { return Length; }
		}

		/// <summary>
		///  <see cref="ICollection.CopyTo"/>
		/// </summary>
		/// <param name="array">
		///  <see cref="ICollection.CopyTo"/>
		/// </param>
		/// <param name="index">
		///  <see cref="ICollection.CopyTo"/>
		/// </param>
		public void CopyTo(Array array, int index)
		{
			_items.CopyTo(array, index);
		}

		/// <summary>
		///  <see cref="ICollection.SyncRoot"/>
		/// </summary>
		public object SyncRoot
		{
			get { return this; }
		}

		#endregion

		private int StringToNumber(string toConvert)
		{
			try
			{
				return Convert.ToInt32(toConvert);
			}
			catch
			{
				return -1;
			}
		}
	}
}

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

namespace JSTools.Parser.Cruncher.Nodes
{
	public class PropertyEnumerator : IEnumerator
	{
		private Hashtable _buckets = null;
		private IEnumerator _keyEnum = null;
		private NodeProperty[] _disallowedProps = null;

		internal PropertyEnumerator(Hashtable buckets, NodeProperty[] disallowedProps)
		{
			if (buckets == null)
				throw new ArgumentNullException("buckets");

			if (disallowedProps == null)
				_disallowedProps = new NodeProperty[0];
			else
				_disallowedProps = disallowedProps;

			_buckets = buckets;
			_keyEnum = _buckets.Keys.GetEnumerator();
		}

		#region IEnumerator Member

		public void Reset()
		{
			_keyEnum = _buckets.Keys.GetEnumerator();
		}

		public PropertyValueBucket Current
		{
			get { return (PropertyValueBucket)((IEnumerator)this).Current; }
		}

		object IEnumerator.Current
		{
			get
			{
				return new PropertyValueBucket(
					(NodeProperty)_keyEnum.Current,
					_buckets[_keyEnum.Current] );
			}			
		}

		public bool MoveNext()
		{
			bool next = false;
			
			do
			{
				next = _keyEnum.MoveNext();
			}
			while (next && Array.IndexOf(_disallowedProps, _keyEnum.Current) != -1);

			return next;
		}

		#endregion
	}
}

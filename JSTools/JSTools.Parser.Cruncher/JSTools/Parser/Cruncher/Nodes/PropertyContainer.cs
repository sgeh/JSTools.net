/*
 * JSTools.Parser.Cruncher.dll / JSTools.net - A framework for JavaScript/ASP.NET applications.
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
using System.ComponentModel;
using System.Xml.Serialization;

namespace JSTools.Parser.Cruncher.Nodes
{
	public enum NodeProperty
	{
		Target = 1,
		Break = 2,
		Continue = 3,
		Enum = 4,
		Function = 5,
		Temp = 6,
		Local = 7,
		CodeOffset = 8,
		FixUps = 9,
		Vars = 10,
		Uses = 11,
		RegExp = 12,
		Cases = 13,
		Default = 14,
		CaseArray = 15,
		SourceName = 16,
		Source = 17,
		Type = 18,
		SpecialProperty = 19,
		Label = 20,
		Finally = 21,
		LocalCount = 22,

		/*
			 the following properties are defined and manipulated by the
			 optimizer -
			 TargetBlock - the block referenced by a branch node
			 Variable - the variable referenced by a BIND or NAME node
			 LastUse - that variable node is the _last reference before
			 a new def or the end of the block
			 IsNumber - this node generates code on Number children and
			 delivers a Number result (as opposed to Objects)
			 DirectCall - this call node should emit code to test the function
			 object against the known class and call diret if it
			 matches.
			 */

		TargetBlock = 23,
		Variable = 24,
		LastUse = 25,
		IsNumber = 26,
		DirectCall = 27,

		BaseLineNo = 28,
		EndLineNo = 29,
		SpecialCall = 30,
		DebugSource = 31
	}

	[XmlType(Namespace=Node.NAMESPACE)]
	public class PropertyContainer : IEnumerable, ICloneable
	{
		private static readonly NodeProperty[] DISALLOWED_SER_PROPERTIES = { };
		private Hashtable _buckets;

		public PropertyContainer()
		{
			_buckets = new Hashtable();
		}

		private PropertyContainer(Hashtable buckets)
		{
			if (buckets == null)
				_buckets = new Hashtable();
			else
				_buckets = buckets;
		}

		public bool HasProps()
		{
			return (_buckets.Count != 0);
		}

		public object GetProp(NodeProperty propType) 
		{
			return _buckets[propType];
		}

		public int GetIntProp(NodeProperty propType, int defaultValue) 
		{
			if (_buckets[propType] != null)
				return (int)_buckets[propType];
			return defaultValue;
		}

		public void Add(PropertyValueBucket bucketToAdd)
		{
			if (bucketToAdd == null)
				throw new ArgumentNullException("bucketToAdd");

			_buckets[bucketToAdd.Key] = bucketToAdd.Value;
		}

		public void PutProp(NodeProperty propType, object prop) 
		{
			_buckets[propType] = prop;
		}

		public void PutIntProp(NodeProperty propType, int prop) 
		{
			PutProp(propType, prop);
		}

		public void RemoveProp(NodeProperty propType) 
		{
			_buckets.Remove(propType);
		}

		#region IEnumerable Member

		public PropertyEnumerator GetEnumerator()
		{
			return (PropertyEnumerator)((IEnumerable)this).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new PropertyEnumerator(_buckets, DISALLOWED_SER_PROPERTIES);
		}

		#endregion

		#region ICloneable Member

		public PropertyContainer Clone()
		{
			return (PropertyContainer)((ICloneable)this).Clone();
		}

		object ICloneable.Clone()
		{
			return new PropertyContainer((Hashtable)_buckets.Clone());
		}

		#endregion
	}
}

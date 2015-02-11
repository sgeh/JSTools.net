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
using System.Xml.Serialization;

namespace JSTools.Parser.Cruncher.Nodes
{
	[XmlType(Namespace=Node.NAMESPACE)]
	public class StringNode : Node 
	{
		private string _str = null;

		[XmlAttribute("data")]
		public override string Data
		{
			get { return _str; }
			set { _str = value; }		
		}

		public StringNode(TokenType type, string str) : base(type)
		{
			_str = str;
		}

		public StringNode(string str) : base(TokenType.String)
		{
			_str = str;
		}

		public StringNode()
		{
		}

		public override string ToString() 
		{
			return _str;
		}

		protected override Node CreateCloneInstance()
		{
			return new StringNode(Type, _str);
		}
	}
}

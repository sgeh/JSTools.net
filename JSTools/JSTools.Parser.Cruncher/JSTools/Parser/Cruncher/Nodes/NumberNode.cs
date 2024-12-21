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
	public class NumberNode : Node 
	{
		private string _number = null;

		[XmlAttribute("data")]
		public override string Data
		{
			get { return _number; }
			set { _number = value; }		
		}

		public NumberNode(string number) : base(TokenType.Number)
		{
			_number = number;
		}

		public NumberNode()
		{
		}

		public override string ToString() 
		{
			return _number;
		}

		protected override Node CreateCloneInstance()
		{
			return new NumberNode(_number);
		}
	}
}

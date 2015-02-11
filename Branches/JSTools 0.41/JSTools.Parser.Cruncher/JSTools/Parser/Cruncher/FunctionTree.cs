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

using JSTools.Parser.Cruncher.Nodes;

namespace JSTools.Parser.Cruncher
{
	internal class FunctionTree
	{
		private ArrayList _childTrees = new ArrayList();
		private Node _currentNode = null;

		internal Node Current
		{
			get { return _currentNode; }
		}

		internal FunctionTree this[int index]
		{
			get 
			{
				if (index < 0 || index >= _childTrees.Count)
				{
					throw new ArgumentException(
						string.Format("The given function node index {0} is invalid.", index.ToString()) );
				}
				return (FunctionTree)_childTrees[index];
			}
		}

		internal FunctionTree(Node nodeTree)
		{
			_currentNode = nodeTree;
			InitFunctionNodes(nodeTree);
		}

		private void InitFunctionNodes(Node parentNode)
		{
			Node currentNode = parentNode.FirstChild;

			while (currentNode != null)
			{
				// search for function property
				if (currentNode.Props.GetProp(NodeProperty.Function) != null)
					_childTrees.Add(new FunctionTree((Node)currentNode.Props.GetProp(NodeProperty.Function)));

				// search in the current tree for a node with a function property
				InitFunctionNodes(currentNode);

				// walk through the hirarchy
				currentNode = currentNode.Next;
			}
		}
	}
}

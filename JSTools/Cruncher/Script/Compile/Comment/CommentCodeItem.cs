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
using JSCompiler.CompileChars;

namespace JSCompiler.Script.Compile
{
	/// <summary>
	/// Zusammenfassungsbeschreibung für CommentCodeItem.
	/// </summary>
	public abstract class CommentCodeItem : AbstractCodeItem
	{
		protected abstract string EndString
		{
			get;
		}



		public CommentCodeItem()
		{
		}



		public override bool IsEnd(int position, string toCheck)
		{
			return (CodeItemContainer.Instance.GetFigureFromString(position - 2, toCheck) + CodeItemContainer.Instance.GetFigureFromString(position - 1, toCheck) == EndString);
		}


		public abstract override bool IsBegin(int position, string toCheck);


		public override string ParsePosition(int position, string toCheck)
		{
			return "";
		}
	}
}

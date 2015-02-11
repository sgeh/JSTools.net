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
using System.Text;
using JSCompiler.CompileChars;

namespace JSCompiler.Script.Compile
{
	/// <summary>
	/// Zusammenfassungsbeschreibung für SingleLineCommentCodeItem.
	/// </summary>
	public class SingleLineCommentCodeItem : CommentCodeItem
	{
		protected override string EndString
		{
			get { return CompileChar.LINEBREAK; }
		}



		public SingleLineCommentCodeItem()
		{
		}



		public override bool IsBegin(int position, string toCheck)
		{
			for (int i = 0; i < CompileChar.SINGLELINE_COMMENT_BEGIN.Length; ++i)
			{
				if (IsSingleLineBegin(position, toCheck, i))
				{
					return true;
				}
			}
			return false;
		}


		private bool IsSingleLineBegin(int position, string toCheck, int index)
		{
			StringBuilder checkString = new StringBuilder();

			for (int i = 0; i < CompileChar.SINGLELINE_COMMENT_BEGIN[index].Length; ++i)
			{
				checkString.Append(CodeItemContainer.Instance.GetFigureFromString(position + i, toCheck));
			}
			return (checkString.ToString() == CompileChar.SINGLELINE_COMMENT_BEGIN[index]);
		}
	}
}

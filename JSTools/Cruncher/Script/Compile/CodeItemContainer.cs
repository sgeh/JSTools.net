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

namespace JSCompiler.Script.Compile
{
	/// <summary>
	/// Zusammenfassungsbeschreibung für CodeItemContainer.
	/// </summary>
	public class CodeItemContainer
	{
		private static	CodeItemContainer	_instance;
		private			DefaultCodeItem		_defaultItem	= new DefaultCodeItem();
		private			ICodeItem[]			_item			= {
																new QuoteStringCodeItem(),
																new SingleStringCodeItem(),
																new MultiLineCommentCodeItem(),
																new SingleLineCommentCodeItem()
															  };



		public static CodeItemContainer Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new CodeItemContainer();

				}
				return _instance;
			}
		}


		public ICodeItem this[int index]
		{
			get { return _item[index]; }
		}


		public int Count
		{
			get { return _item.Length; }
		}



		public DefaultCodeItem DefaultItem
		{
			get { return _defaultItem; }
		}



		private CodeItemContainer()
		{
		}



		public string GetFigureFromString(int position, string toCheck)
		{
			return (position < toCheck.Length && position > -1) ? Convert.ToString(toCheck[position]) : "";
		}
	}
}

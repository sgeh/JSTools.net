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
using System.Text.RegularExpressions;

namespace JSCompiler.Script.Compile
{
	/// <summary>
	/// Zusammenfassungsbeschreibung für AbstractCodeItem.
	/// </summary>
	public abstract class AbstractCodeItem : ICodeItem
	{
		public AbstractCodeItem()
		{
		}


		protected bool IsIgnored(int position, string toCheck)
		{
			Regex ignoreRegex = new Regex("(\\\\+)$", RegexOptions.None);
			Match ignoreMatch = ignoreRegex.Match(toCheck.Substring(0, position));

			if (ignoreMatch != null && ignoreMatch.Captures.Count > 0)
			{
				return (ignoreMatch.Captures[0].Value.Length % 2 == 1);
			}
			return false;
		}


		public abstract bool IsEnd(int position, string toCheck);
		public abstract bool IsBegin(int position, string toCheck);
		public abstract string ParsePosition(int position, string toCheck);
	}
}

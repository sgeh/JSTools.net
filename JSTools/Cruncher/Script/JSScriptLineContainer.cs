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
using System.Text;
using JSCompiler.CompileChars;

namespace JSCompiler.Script
{
	/// <summary>
	/// Zusammenfassungsbeschreibung für JSScriptLineContainer.
	/// </summary>
	public class JSScriptLineContainer
	{
		private ArrayList	_javaScriptLines;
		private	string		_javaScript;


		/// <summary>
		/// returns the count of all script lines
		/// </summary>
		public int Count
		{
			get { return _javaScriptLines.Count; }
		}


		/// <summary>
		/// returns the specified script line
		/// </summary>
		public string this[int index]
		{
			get { return (string)_javaScriptLines[index]; }
		}


		/// <summary>
		/// initializes the JSScriptLineContainer object
		/// </summary>
		/// <param name="javaScript">script, which contains line breaks</param>
		public JSScriptLineContainer(string javaScript)
		{
			_javaScript			= javaScript;
			_javaScriptLines	= new ArrayList();
			GenerateScriptLines();
		}


		/// <summary>
		/// searches in the whole script for line breaks and generates the script lines
		/// </summary>
		private void GenerateScriptLines()
		{
			int lastLine		= 0;

			for (int i = 0; i < _javaScript.Length - 1; ++i)
			{
				if (CompileChar.IsLineBreak(_javaScript[i], _javaScript[i + 1]))
				{
					AddLineToArrayList(lastLine, i - 1);
					lastLine = i + CompileChar.LINEBREAK.Length;
				}
			}
			AddLineToArrayList(lastLine, _javaScript.Length - 1);
		}


		/// <summary>
		/// adds a new script line to the array list
		/// </summary>
		/// <param name="start">start position in the javascript</param>
		/// <param name="end">end position in the javascript</param>
		private void AddLineToArrayList(int start, int end)
		{
			StringBuilder newArrayEntry = new StringBuilder();

			for (int i = start; i < end + 1; ++i)
			{
				newArrayEntry.Append(_javaScript[i]);
			}
			_javaScriptLines.Add(newArrayEntry.ToString());
		}
	}
}

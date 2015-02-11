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
using System.Windows.Forms;
using JSCompiler.Base;
using JSCompiler.CompileChars;
using JSCompiler.Script.Parse;

namespace JSCompiler.Script
{
	/// <summary>
	/// Zusammenfassungsbeschreibung für JSScript.
	/// </summary>
	public class JSScript
	{
		private string					_javaScript				= "";
		private	JSScriptLineContainer	_javaScriptLines;
		private	JSScriptCompiler		_javaScriptComiler;
		private	JSBaseCompiler			_javaScriptBaseCompiler;
		private JSScriptLineParser		_javaScriptLineParser;

		
		public char this[int index]
		{
			get { return _javaScript[index]; }
		}


		public int Length
		{
			get { return _javaScript.Length; }
		}


		public string ScriptCompiler
		{
			get { return _javaScriptComiler; }
		}


		public JSScriptLineContainer ScriptLines
		{
			get { return _javaScriptLines; }
		}


		public JSScript(string javaScript)
		{
			_javaScript				= javaScript;
			_javaScriptLines		= new JSScriptLineContainer(javaScript);
		}



		public string GetSingleLine(int number)
		{
			return _javaScriptLines[number];
		}
	}
}

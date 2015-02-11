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
using System.Text.RegularExpressions;
using System.Windows.Forms;
using JSCompiler.CompileChars;
using JSCompiler.Script.Compile;

namespace JSCompiler.Script
{
	/// <summary>
	/// Zusammenfassungsbeschreibung für JSScriptCompiler.
	/// </summary>
	public class JSScriptCompiler
	{
		private		StringBuilder				_javaScriptComiled		= new StringBuilder();
		private		ICodeItem					_codeItemActive			= null;

		private		int							_currentPosition		= 0;
		private		JSScript					_baseScript;
		private		string						_compileString;


		public string CompiledScript
		{
			get { return _javaScriptComiled.ToString(); }
		}


		public JSScriptCompiler(JSScript baseScript, string compileString)
		{
			_baseScript		= baseScript;
			_compileString	= compileString;
		}


		public void Compile()
		{
			for (int i = 0; i < _baseScript.ScriptLines.Count; ++i)
			{
				for (int j = 0; j < _baseScript.ScriptLines[i].Length; ++j)
				{
					AppendChar();
				}
				AddLineBreakWhiteSpaceChar();
				WriteLineMessage(i + 1);
			}
		}


		private void AddLineBreakWhiteSpaceChar()
		{
			if (_baseScript.Length > _currentPosition)
			{
				for (int i = 0; i < CompileChar.LINEBREAK.Length; ++i)
				{
					AppendChar();
				}
			}
		}


		private void AppendChar()
		{
			_javaScriptComiled.Append(GetInstanceSpecificValue());
			_currentPosition++;
		}


		private string GetInstanceSpecificValue()
		{
			CheckCodeItemInstance();
			return (_codeItemActive == null) ? CodeItemContainer.Instance.DefaultItem.ParsePosition(_javaScriptComiled.ToString(), _compileString, _currentPosition) : _codeItemActive.ParsePosition(_currentPosition, _compileString);
		}


		private void CheckCodeItemInstance()
		{
			if (_codeItemActive != null && _codeItemActive.IsEnd(_currentPosition, _compileString))
			{
				_codeItemActive = null;
			}

			if (_codeItemActive == null)
			{
				_codeItemActive = GetInstance();
			}
		}


		private ICodeItem GetInstance()
		{
			for (int i = 0; i < CodeItemContainer.Instance.Count; ++i)
			{
				if (CodeItemContainer.Instance[i].IsBegin(_currentPosition, _compileString))
				{
					return CodeItemContainer.Instance[i];
				}
			}
			return null;
		}


		private void WriteLineMessage(int line)
		{
			_baseScript.BaseCompilerFunctions.InsertlnMessage(_baseScript.BaseCompilerFunctions.GetConfiguration("compileLine") + " " + line + _baseScript.BaseCompilerFunctions.SpacerValue + _baseScript.BaseCompilerFunctions.DoneValue);
			_baseScript.BaseCompilerFunctions.Write();
			Application.DoEvents();
		}
	}
}

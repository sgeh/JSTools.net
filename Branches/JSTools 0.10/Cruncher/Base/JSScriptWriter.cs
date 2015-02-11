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
using System.IO;
using System.Windows.Forms;
using JSCompiler.Script;

namespace JSCompiler.Base
{
	/// <summary>
	/// Zusammenfassungsbeschreibung für JSCompileScript.
	/// </summary>
	public class JSScriptWriter : JSBaseCompiler
	{
		private			JSScript		_scriptToCompile;
		private			string			_saveTarget			= "";
		private			bool			_scriptWritten		= false;



		protected override string ConfigXPath
		{
			get { return "//ioMessages/add"; }
		}


		protected override string ConfigAttributeName
		{
			get { return "value"; }
		}


		public bool ScriptWritten
		{
			get { return _scriptWritten; }
		}


		public JSScript Script
		{
			get { return _scriptToCompile; }
		}


		public JSScriptWriter(string fileDestination, TextBox outputText, JSScript scriptToWrite) : base(outputText)
		{
			_scriptToCompile	= scriptToWrite;
			_saveTarget			= fileDestination;
		}


		public void WriteCompiledScript()
		{
			WriteScript(_scriptToCompile.CompiledScript);
		}


		private void WriteScript(string script)
		{
			try
			{
				using(StreamWriter newFile = File.CreateText(_saveTarget))
				{
					newFile.Write(script);
					newFile.Flush();
				}
				_scriptWritten = true;
			}
			catch
			{
				_scriptWritten = false;
			}
			finally
			{
				WritelnMessage(GetConfiguration("createFile") + SpacerValue + GetStatusValue(_scriptWritten));
			}
		}
	}
}

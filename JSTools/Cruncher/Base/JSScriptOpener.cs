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
using System.IO;
using System.Text;
using System.Windows.Forms;
using JSCompiler.Script;

namespace JSCompiler.Base
{
	/// <summary>
	/// Zusammenfassungsbeschreibung für JSCompileScript.
	/// </summary>
	public class JSScriptOpener : JSBaseCompiler
	{
		private			JSScript		_wholeScript;
		private			bool			_scriptLoaded		= false;
		private			ArrayList		_filesToCompile		= new ArrayList();



		protected override string ConfigXPath
		{
			get { return "//ioMessages/add"; }
		}


		protected override string ConfigAttributeName
		{
			get { return "value"; }
		}


		public bool ScriptLoaded
		{
			get { return _scriptLoaded; }
		}


		public JSScript Script
		{
			get { return _wholeScript; }
		}



		public JSScriptOpener(TextBox outputTextBox, IList filesToCompile) : base(outputTextBox)
		{
			LoadFiles(filesToCompile);
		}


		public JSScriptOpener(TextWriter outputWriter, IList filesToCompile) : base(outputWriter)
		{
			LoadFiles(filesToCompile);
		}


		public JSScriptOpener(TextBox outputTextBox, TextWriter outputWriter, IList filesToCompile) : base(outputTextBox, outputWriter)
		{
			LoadFiles(filesToCompile);
		}


		public JSScriptOpener(IList filesToCompile)
		{
			LoadFiles(filesToCompile);
		}



		private void LoadFiles(IList filesToCompile)
		{
			try
			{
				StringBuilder	scriptToCompile = new StringBuilder();
				TextReader		scriptStream;

				GetFilesFromComboBox(filesToCompile);

				for (int i = 0; i < _filesToCompile.Count; ++i)
				{
					using(scriptStream = File.OpenText((string)_filesToCompile[i]))
					{
						scriptToCompile.Append(scriptStream.ReadToEnd());
					}
				}
				_scriptLoaded	= true;
				_wholeScript	= new JSScript(scriptToCompile.ToString(), this);
			}
			catch
			{
				_scriptLoaded = false;
			}
			finally
			{
				WritelnMessage(GetConfiguration("open", ConfigAttributeName) + SpacerValue + GetStatusValue(_scriptLoaded));
			}	
		}


		private void GetFilesFromComboBox(IList filesToRead)
		{
			for (int i = 0; i < filesToRead.Count; ++i)
			{
				if (!HasItemInArrayList((string)filesToRead[i]))
				{
					_filesToCompile.Add(filesToRead[i]);
				}
			}
		}


		private bool HasItemInArrayList(string item)
		{
			for (int i = 0; i < _filesToCompile.Count; ++i)
			{
				if ((string)_filesToCompile[i] == item)
				{
					return true;
				}
			}
			return false;
		}
	}
}

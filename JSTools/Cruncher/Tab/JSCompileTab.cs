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
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using JSCompiler.Base;

namespace JSCompiler.Tab
{
	/// <summary>
	/// Zusammenfassungsbeschreibung für JSCompileTab.
	/// </summary>
	public class JSCompileTab : JSBaseTab
	{
		private	ComboBox	_filesToCompileBox;
		private	Button		_filesToCompileAdd;
		private	Button		_filesToCompileRemove;
		private	Button		_outputFileChoose;
		private	Button		_compileStart;
		private Label		_filesToCompileDescLabel;
		private	Label		_outputFileLabel;
		private	Label		_outputFileLocation;
		private	Label		_compileInfoLabel;
		private TextBox		_compileOutput;

		private	string		_outputFilePath				= "";


		protected override string ElementName
		{
			get { return "compile"; }
		}


		public JSCompileTab(JSCompiler baseClass)
		{
			_baseClass							= baseClass;
			_elementPage						= _baseClass.JSDesignDefinition.CreateStyledTabPage(JSCompiler.COMPILE_PAGE_NAME, TabName, true, true);

			_filesToCompileDescLabel			= _baseClass.JSDesignDefinition.CreateStyledLabel("filesToCompileLabel", GetConfiguration("filesToCompile", "value"), true, true, false);
			_filesToCompileDescLabel.Location	= new Point(20, 20);
			_filesToCompileDescLabel.Size		= new Size(160, 30);
			_elementPage.Controls.Add(_filesToCompileDescLabel);

			_filesToCompileBox					= _baseClass.JSDesignDefinition.CreateStyledComboBox("filesToCompileBox", false, false, true);
			_filesToCompileBox.Location			= new Point(20, 60);
			_filesToCompileBox.Size				= new Size(160, 20);
			_filesToCompileBox.ItemHeight		= 10;
			_elementPage.Controls.Add(_filesToCompileBox);

			_filesToCompileAdd					= _baseClass.JSDesignDefinition.CreateStyledButton("filesToCompileAdd", GetConfiguration("add", "value"), true, true);
			_filesToCompileAdd.Location			= new Point(40, 90);
			_filesToCompileAdd.Size				= new Size(50, 20);
			_filesToCompileAdd.Click			+= new EventHandler(FilesToCompileAddButton_Click);
			_elementPage.Controls.Add(_filesToCompileAdd);

			_filesToCompileRemove				= _baseClass.JSDesignDefinition.CreateStyledButton("filesToCompileRemove", GetConfiguration("remove", "value"), false, true);
			_filesToCompileRemove.Location		= new Point(100, 90);
			_filesToCompileRemove.Size			= new Size(60, 20);
			_filesToCompileRemove.Click			+= new EventHandler(FilesToCompileRemoveButton_Click);
			_elementPage.Controls.Add(_filesToCompileRemove);

			_outputFileLabel					= _baseClass.JSDesignDefinition.CreateStyledLabel("outputFile", GetConfiguration("outputFile", "value"), true, true, false);
			_outputFileLabel.Location			= new Point(250, 20);
			_outputFileLabel.Size				= new Size(160, 30);
			_elementPage.Controls.Add(_outputFileLabel);

			_outputFileLocation					= _baseClass.JSDesignDefinition.CreateStyledLabel("outputFileLocation", "no location selected", true, true, false);
			_outputFileLocation.Location		= new Point(250, 60);
			_outputFileLocation.Size			= new Size(160, 30);
			_elementPage.Controls.Add(_outputFileLocation);

			_outputFileChoose					= _baseClass.JSDesignDefinition.CreateStyledButton("outputFileChoose", GetConfiguration("choose", "value"), true, true);
			_outputFileChoose.Location			= new Point(270, 90);
			_outputFileChoose.Size				= new Size(60, 20);
			_outputFileChoose.Click				+= new EventHandler(OutputFileLocation_Click);
			_elementPage.Controls.Add(_outputFileChoose);

			_compileStart						= _baseClass.JSDesignDefinition.CreateStyledButton("compileStart", GetConfiguration("compile", "value"), false, true);
			_compileStart.Location				= new Point(340, 90);
			_compileStart.Size					= new Size(60, 20);
			_compileStart.Click					+= new EventHandler(CompileStart_Click);
			_elementPage.Controls.Add(_compileStart);

			_compileInfoLabel					= _baseClass.JSDesignDefinition.CreateStyledLabel("compileInfoLabel", GetConfiguration("compileInfo", "value"), true, false, false);
			_compileInfoLabel.Location			= new Point(20, 130);
			_compileInfoLabel.Size				= new Size(160, 20);
			_elementPage.Controls.Add(_compileInfoLabel);

			_compileOutput						= _baseClass.JSDesignDefinition.CreateStyledTextBox("compileOutput", "", false, false);
			_compileOutput.Location				= new Point(20, 150);
			_compileOutput.Size					= new Size(420, 50);
			_compileOutput.Multiline			= true;
			_compileOutput.ScrollBars			= ScrollBars.Vertical;
			_compileOutput.ReadOnly				= true;
			_elementPage.Controls.Add(_compileOutput);
		}


		private void FilesToCompileAddButton_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog newCompileFile	= new OpenFileDialog();
			newCompileFile.Filter			= "text files (*.txt)|*.txt|javascript files (*.js)|*.js";
			newCompileFile.CheckFileExists	= true;
			newCompileFile.Multiselect		= true;

			if (newCompileFile.ShowDialog() == DialogResult.OK)
			{
				_filesToCompileBox.Enabled	= true;
				_filesToCompileBox.BeginUpdate();

				foreach(string fileName in newCompileFile.FileNames)
				{
					_filesToCompileBox.Items.Add(fileName);
				}
				_filesToCompileBox.EndUpdate();

				if (_filesToCompileBox.Items.Count > 0)
				{
					_filesToCompileBox.SelectedIndex = 0;
					_filesToCompileRemove.Enabled = true;
				}
			}
			CheckForValidInputFields();
		}


		private void FilesToCompileRemoveButton_Click(object sender, System.EventArgs e)
		{
			if (_filesToCompileBox.SelectedIndex != -1)
			{
				_filesToCompileBox.Items.Remove(_filesToCompileBox.SelectedItem);
			}
			if(_filesToCompileBox.Items.Count == 0)
			{
				((Button)sender).Enabled	= false;
				_filesToCompileBox.Enabled	= false;
			}
			else
			{
				_filesToCompileBox.SelectedIndex = 0;
			}
			CheckForValidInputFields();
		}


		private void OutputFileLocation_Click(object sender, System.EventArgs e)
		{
			SaveFileDialog newCompiledFile	= new SaveFileDialog();
			newCompiledFile.Filter			= "text files (*.txt)|*.txt|javascript files (*.js)|*.js";
			newCompiledFile.CheckFileExists	= false;
			newCompiledFile.CheckPathExists	= true;

			if (newCompiledFile.ShowDialog() == DialogResult.OK)
			{
				_outputFilePath	= newCompiledFile.FileName;
				_outputFileLocation.Text = (newCompiledFile.FileName.Length > 40) ? GetSmallOutputFileLocation(newCompiledFile.FileName) : newCompiledFile.FileName;
			}
			CheckForValidInputFields();
		}


		private void CompileStart_Click(object sender, System.EventArgs e)
		{
			if (MessageBox.Show(GetConfiguration("askCompile", "value"), "Compiling...", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
			{
				BaseTabControl.Enabled			= false;
				_compileOutput.Enabled			= false;
				_compileInfoLabel.Visible		= true;
				_compileOutput.Visible			= true;

				JSScriptOpener compileScript = new JSScriptOpener(_compileOutput, _filesToCompileBox.Items);

				if (compileScript.ScriptLoaded)
				{
					compileScript.Script.CompileScript(true);
				}

				JSScriptWriter writeScript = new JSScriptWriter(_outputFilePath, _compileOutput, compileScript.Script);
				writeScript.WriteCompiledScript();

				BaseTabControl.Enabled			= true;
				_compileOutput.Enabled			= true;
				RemoveOutPutFileInfos();
			}
		}


		private void RemoveOutPutFileInfos()
		{
			_outputFilePath				= "";
			_outputFileLocation.Text	= "";
			CheckForValidInputFields();
		}


		private string GetSmallOutputFileLocation(string longPath)
		{
			Regex replaceRegExp = new Regex(@"\\.*\\", RegexOptions.IgnoreCase);
			longPath = replaceRegExp.Replace(longPath, @"\...\");
			return (longPath.Length < 40) ? longPath : longPath.Substring(0, longPath.IndexOf("\\") + 1) + "...";
		}


		private void CheckForValidInputFields()
		{
			_compileStart.Enabled = (_outputFilePath.Length != 0 && _filesToCompileBox.Items.Count > 0);
		}
	}
}

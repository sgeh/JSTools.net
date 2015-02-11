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
using System.Windows.Forms;

namespace JSCompiler
{
	/// <summary>
	/// Zusammenfassungsbeschreibung für JSStyle.
	/// </summary>
	public class JSStyle
	{
		private		JSCompiler	_jsCompiler;

		// button
		private		Color		_buttonColor;
		private		Color		_buttonForeColor;
		private		FontFamily	_buttonFontFamily;
		private		Font		_buttonFont;

		//label
		private		Color		_labelColor;
		private		Color		_labelForeColor;
		private		FontFamily	_labelFontFamily;
		private		Font		_labelFont;
		private		Font		_labelFontBold;

		//tab
		private		Color		_tabColor;
		private		Color		_tabForeColor;
		private		FontFamily	_tabFontFamily;
		private		Font		_tabFont;

		//combo
		private		Color		_comboColor;
		private		Color		_comboForeColor;
		private		FontFamily	_comboFontFamily;
		private		Font		_comboFont;

		//textbox
		private		Color		_textBoxColor;
		private		Color		_textBoxForeColor;
		private		FontFamily	_textBoxFontFamily;
		private		Font		_textBoxFont;




		/// <summary>
		/// 
		/// </summary>
		/// <param name="baseCompiler"></param>
		public JSStyle(JSCompiler baseCompiler)
		{
			_jsCompiler = baseCompiler;

			// base frame
			_jsCompiler.Size			= new Size(500, 300);
			_jsCompiler.Text			= JSCompiler.APP_TITLE + " <Console " + JSCompiler.APP_VERSION + ">";
			_jsCompiler.BackColor		= SystemColors.Menu;
			_jsCompiler.ForeColor		= Color.Black;
			_jsCompiler.Font			= new Font("Arial", 8, FontStyle.Regular);
			_jsCompiler.FormBorderStyle	= FormBorderStyle.FixedDialog;
			_jsCompiler.StartPosition	= FormStartPosition.CenterScreen;
			_jsCompiler.MaximizeBox		= false;
			_jsCompiler.MinimizeBox		= false;

			// button
			_buttonColor				= SystemColors.Menu;
			_buttonForeColor			= Color.Black;
			_buttonFontFamily			= new FontFamily("Arial");
			_buttonFont					= new Font(_buttonFontFamily.Name, 8, FontStyle.Regular);

			// label
			_labelColor					= SystemColors.Menu;
			_labelForeColor				= Color.Black;
			_labelFontFamily			= new FontFamily("Arial");
			_labelFont					= new Font(_buttonFontFamily.Name, 8, FontStyle.Regular);
			_labelFontBold				= new Font(_buttonFontFamily.Name, 8, FontStyle.Bold);

			// tab
			_tabColor					= SystemColors.Menu;
			_tabForeColor				= Color.White;
			_tabFontFamily				= new FontFamily("Arial");
			_tabFont					= new Font(_tabFontFamily.Name, 8);

			// combobox
			_comboColor					= Color.White;
			_comboForeColor				= Color.Black;
			_comboFontFamily			= new FontFamily("Arial");
			_comboFont					= new Font(_tabFontFamily.Name, 8);

			// textbox
			_textBoxColor				= Color.White;
			_textBoxForeColor			= Color.Black;
			_textBoxFontFamily			= new FontFamily("Arial");
			_textBoxFont				= new Font(_tabFontFamily.Name, 7);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="buttonName"></param>
		/// <returns></returns>
		public Button CreateStyledButton(string buttonName)
		{
			return CreateStyledButton(buttonName, "", true, false);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="buttonName"></param>
		/// <param name="text"></param>
		/// <param name="boolIsEnabled"></param>
		/// <param name="boolIsVisible"></param>
		/// <returns></returns>
		public Button CreateStyledButton(string buttonName, string text, bool boolIsEnabled, bool boolIsVisible)
		{
			Button newButton		= new Button();
			newButton.Enabled		= boolIsEnabled;
			newButton.Visible		= boolIsVisible;
			newButton.Text			= text;
			newButton.BackColor		= _buttonColor;
			newButton.Name			= buttonName;
			newButton.Font			= _buttonFont;
			newButton.ForeColor		= _buttonForeColor;

			_jsCompiler.ButtonCollection.Add(buttonName, newButton);
			return newButton;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="labelName"></param>
		/// <returns></returns>
		public Label CreateStyledLabel(string labelName)
		{
			return CreateStyledLabel(labelName, "", true, false, false);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="labelName"></param>
		/// <param name="text"></param>
		/// <param name="boolIsEnabled"></param>
		/// <param name="boolIsVisible"></param>
		/// <returns></returns>
		public Label CreateStyledLabel(string labelName, string text, bool boolIsEnabled, bool boolIsVisible, bool boldFont)
		{
			Label newLabel			= new Label(); 
			newLabel.Enabled		= boolIsEnabled;
			newLabel.Visible		= boolIsVisible;
			newLabel.Text			= text;
			newLabel.BackColor		= _labelColor;
			newLabel.Name			= labelName;
			newLabel.Font			= ((boldFont) ? _labelFontBold : _labelFont);
			newLabel.ForeColor		= _labelForeColor;

			_jsCompiler.LabelCollection.Add(labelName, newLabel);
			return newLabel;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="tabControlName"></param>
		/// <returns></returns>
		public TabControl CreateStyledTabControl(string tabControlName)
		{
			return CreateStyledTabControl(tabControlName, 0, true, false);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="tabControlName"></param>
		/// <param name="selectedIndex"></param>
		/// <param name="boolIsEnabled"></param>
		/// <param name="boolIsVisible"></param>
		/// <returns></returns>
		public TabControl CreateStyledTabControl(string tabControlName, int selectedIndex, bool boolIsEnabled, bool boolIsVisible)
		{
			TabControl newTab		= new TabControl();
			newTab.SizeMode			= TabSizeMode.Normal;
			newTab.Visible			= boolIsVisible;
			newTab.SelectedIndex	= selectedIndex;
			newTab.BackColor		= _tabColor;
			newTab.Name				= tabControlName;
			newTab.Font				= _tabFont;
			newTab.ForeColor		= _tabForeColor;
			newTab.Enabled			= boolIsEnabled;

			_jsCompiler.TabCollection.Add(tabControlName, newTab);
			return newTab;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="tabPageName"></param>
		/// <returns></returns>
		public TabPage CreateStyledTabPage(string tabPageName)
		{
			return CreateStyledTabPage(tabPageName, "", true, false);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="tabControlName"></param>
		/// <param name="tabText"></param>
		/// <param name="boolIsEnabled"></param>
		/// <param name="boolIsVisible"></param>
		/// <returns></returns>
		public TabPage CreateStyledTabPage(string tabControlName, string tabText, bool boolIsEnabled, bool boolIsVisible)
		{
			TabPage newTabPage		= new TabPage();
            newTabPage.BorderStyle	= BorderStyle.None;
			newTabPage.Text			= tabText;
			newTabPage.Visible		= boolIsVisible;
			newTabPage.BackColor	= _tabColor;
			newTabPage.Name			= tabControlName;
			newTabPage.Font			= _tabFont;
			newTabPage.ForeColor	= _tabForeColor;
			newTabPage.Enabled		= boolIsEnabled;

			_jsCompiler.TabPageCollection.Add(tabControlName, newTabPage);
			return newTabPage;
		}


		public ComboBox CreateStyledComboBox(string comboBoxName, bool boolIsSorted, bool boolIsEnabled, bool boolIsVisible)
		{
			ComboBox newComboBox	= new ComboBox();
			newComboBox.BackColor	= _comboColor;
			newComboBox.Enabled		= boolIsEnabled;
			newComboBox.Visible		= boolIsVisible;
			newComboBox.Font		= _comboFont;
			newComboBox.ForeColor	= _comboForeColor;
			newComboBox.Sorted		= boolIsSorted;
			newComboBox.Name		= comboBoxName;

			_jsCompiler.ComobCollection.Add(comboBoxName, newComboBox);
			return newComboBox;
		}


		public TextBox CreateStyledTextBox(string textBoxName, string boxText, bool boolIsEnabled, bool boolIsVisible)
		{
			TextBox newTextBox		= new TextBox();
			newTextBox.BackColor	= _textBoxColor;
			newTextBox.Enabled		= boolIsEnabled;
			newTextBox.Visible		= boolIsVisible;
			newTextBox.Font			= _textBoxFont;
			newTextBox.ForeColor	= _textBoxForeColor;
			newTextBox.Name			= textBoxName;

			_jsCompiler.TextBoxCollection.Add(textBoxName, newTextBox);
			return newTextBox;
		}
	}
}

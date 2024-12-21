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
using System.Text;
using System.Windows.Forms;
using JSCompiler.CompileChars;

namespace JSCompiler.Base
{
	/// <summary>
	/// Zusammenfassungsbeschreibung für JSBaseCompiler.
	/// </summary>
	public abstract class JSBaseCompiler
	{
		private	const	string			SPACER				= " ... ";
		private	const	string			SUCCESS				= "success";
		private	const	string			FAIL				= "fail";

		protected	TextBox				_messageBox;
		protected	TextWriter			_textWriter;
		private		StringBuilder		_messageString		= new StringBuilder();


		protected abstract string ConfigXPath
		{
			get;
		}


		protected abstract string ConfigAttributeName
		{
			get;
		}

		
		public string DoneValue
		{
			get { return JSConfig.Instance.GetValue(ConfigXPath + "[@key='" + JSBaseCompiler.SUCCESS + "']", ConfigAttributeName); }
		}


		public string FailValue
		{
			get { return JSConfig.Instance.GetValue(ConfigXPath + "[@key='" + JSBaseCompiler.FAIL + "']", ConfigAttributeName); }
		}


		public string SpacerValue
		{
			get { return JSBaseCompiler.SPACER; }
		}



		public JSBaseCompiler(TextBox messageBox)
		{
			_messageBox	= messageBox;
		}


		public JSBaseCompiler(TextWriter messageWriter)
		{
			_textWriter	= messageWriter;
		}

 
		public JSBaseCompiler(TextBox messageBox, TextWriter messageWriter)
		{
			_messageBox	= messageBox;
			_textWriter	= messageWriter;
		}



		public JSBaseCompiler()
		{
		}


		public string GetStatusValue(bool isSuccess)
		{
			return (isSuccess) ? DoneValue : FailValue;
		}


		public void WritelnMessage(string message)
		{
			_messageString.Append(message + CompileChar.LINEBREAK);
		}


		public void WriteMessage(string message)
		{
			_messageString.Append(message);
		}


		public void InsertlnMessage(string message)
		{
			_messageString.Insert(0, message + CompileChar.LINEBREAK);
		}


		public void InsertMessage(string message)
		{
			_messageString.Insert(0, message);
		}


		public string GetConfigurationNodeValue(string xPath, string attributeName)
		{
			return JSConfig.Instance.GetValue(xPath, attributeName);
		}


		public string GetConfiguration(string keyName)
		{
			return GetConfiguration(keyName, ConfigAttributeName);
		}


		public string GetConfiguration(string keyName, string attributeName)
		{
			return JSConfig.Instance.GetValue(ConfigXPath + "[@key='" + keyName + "']", attributeName);
		}


		public void Write()
		{
			if (_messageBox != null)
			{
				_messageBox.Text = _messageString.ToString();
			}
			if (_textWriter != null)
			{
				_textWriter.Write( _messageString.ToString());
			}
		}
	}
}

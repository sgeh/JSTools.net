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
using System.Security;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

namespace JSCompiler
{
	/// <summary>
	/// Zusammenfassungsbeschreibung für JSConfig.
	/// </summary>
	public class JSConfig
	{
		private		string						_jsConfigFilePath		= Application.StartupPath + "\\" + JSCompiler.APP_TEXT_CONFIG;
		private		bool						_configDocumentValid	= false;
		private		XmlDocument					_configDocument			= new XmlDocument();
		private		static			JSConfig	_jsConfig				= null;


		private string _langauge
		{
			get { return "en-en"; }
		}


		public XmlDocument ConfigDocument
		{
			get { return _configDocument; }
		}


		public XmlNode ConfigSection
		{
			get
			{
				if (_configDocumentValid)
				{
					XmlNodeList selectedNodes = _configDocument.SelectNodes("/JSCompiler/language[@select='" + _langauge + "']");
					return (selectedNodes.Count > 0) ? selectedNodes[0] : null;
				}
				return null;
			}
		}


		private JSConfig()
		{
		}


		public static JSConfig Instance
		{
			get
			{
				if (_jsConfig == null)
				{
					_jsConfig = new JSConfig();
				}
				return _jsConfig;
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool OpenConfigXmlDocument()
		{
			FileIOPermission xmlFile = new FileIOPermission(FileIOPermissionAccess.Read, _jsConfigFilePath);
				
			try
			{
				xmlFile.Demand();
				_configDocument.Load(ReadFile(_jsConfigFilePath));
				_configDocumentValid = true;
				return true;
			}
			catch(FileNotFoundException thrownException)
			{
				CallErrorMessage(thrownException, "Could not find the XML configuration file! Please make sure the file " + _jsConfigFilePath + " is available.");
				return false;
			}
			catch(PathTooLongException thrownException)
			{
				CallErrorMessage(thrownException, "Could not find the XML configuration file (path too long)! Please make sure the file " + _jsConfigFilePath + " is available.");
				return false;
			}
			catch(DirectoryNotFoundException thrownException)
			{
				CallErrorMessage(thrownException, "Could not find the XML configuration file (directory not found)! Please make sure the file " + _jsConfigFilePath + " is available.");
				return false;
			}
			catch(UnauthorizedAccessException thrownException)
			{
				CallErrorMessage(thrownException, "Could not open the XML configuration file (access denied)!");
				return false;
			}
			catch(NotSupportedException thrownException)
			{
				CallErrorMessage(thrownException, "Could not open the XML configuration file (action not supported)!");
				return false;
			}
			catch(SecurityException thrownException)
			{
				CallErrorMessage(thrownException, "Could not open the XML configuration file! Have a look at your security configuration.");
				return false;
			}
			catch(IOException thrownException)
			{
				CallErrorMessage(thrownException, "Could not read the XML configuration file! Please restart this application.");
				return false;
			}
			catch(XmlException thrownException)
			{
				CallErrorMessage(thrownException, "Configuration XML file has a bad format! Please reinstall this software.");
				return false;
			}
		}


		public string GetValue(string xPath, string attributeName)
		{
			XmlAttribute xmlAttrib = GetAttribute(xPath, attributeName);
			return (xmlAttrib != null) ? xmlAttrib.Value : "[not found]";
		}


		public XmlNodeList GetNodeList(string xPath)
		{
			if (ConfigSection != null)
			{
				try
				{
					return ConfigSection.SelectNodes(xPath);
				}
				catch(XmlException thrownException)
				{
					CallErrorMessage(thrownException, "Configuration XML file has a bad format! Please reinstall this software.");
				}
				catch(XPathException thrownException)
				{
					CallErrorMessage(thrownException, "Configuration XPath has a bad format! Please reinstall this software.");
				}
			}
			return null;
		}


		public XmlAttribute GetAttribute(string xPath, string attributeName)
		{
			XmlNodeList xmlNodes = GetNodeList(xPath);

			if (xmlNodes != null && xmlNodes.Count > 0)
			{
				return xmlNodes[0].Attributes[attributeName];
			}
			return null;
		}


		private void CallErrorMessage(Exception error, string message)
		{
			_configDocumentValid = false;
			MessageBox.Show(message + "\n\nError specification:\n" + error.Source, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}


		private TextReader ReadFile(string path)
		{
			return new StreamReader((System.IO.Stream)File.OpenRead(path));
		}
	}
}

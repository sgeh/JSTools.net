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
using System.Configuration;
using System.IO;
using System.Xml;
using JSTools.Config;

namespace JSTools.Config
{
	/// <summary>
	/// Summary description for JSToolsConfigurationSection.
	/// </summary>
	public class JSToolsConfigurationSection : IConfigurationSectionHandler
	{
		private	const	string		STANDALONE_ATTRIBUTE	= "standalone";
		private	const	string		SOURCE_ATTRIBUTE		= "source";

		private			XmlDocument	_configurationNode		= new XmlDocument();
		private			bool		_standalone				= false;


		/// <summary>
		/// 
		/// </summary>
		public JSToolsConfigurationSection()
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="configContext"></param>
		/// <param name="section"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public object Create(object parent, object configContext, XmlNode section)
		{
			InitStandaloneAttribute(section);

			if (_standalone)
			{
				InitConfigXmlNode(section);
			}
			else
			{
				InitConfigXmlFile(section);
			}
			return new JSToolsConfiguration(_configurationNode);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="section"></param>
		private void InitStandaloneAttribute(XmlNode section)
		{
			if (section.Attributes[STANDALONE_ATTRIBUTE] != null)
			{
				try
				{
					_standalone = Convert.ToBoolean(section.Attributes[STANDALONE_ATTRIBUTE].Value);
				}
				catch(InvalidCastException)
				{
					_standalone = false;
				}
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="section"></param>
		private void InitConfigXmlNode(XmlNode section)
		{
			XmlDeclaration delcaration	= _configurationNode.CreateXmlDeclaration("1.0", "UTF-8", "yes");
			_configurationNode.AppendChild(delcaration);

			XmlElement documentElement	=_configurationNode.CreateElement("configuration");
			_configurationNode.AppendChild(documentElement);

			for (int i = 0; i < section.ChildNodes.Count; ++i)
			{
				_configurationNode.AppendChild(_configurationNode.ImportNode(section.ChildNodes[i], false));
			}			
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="section"></param>
		private void InitConfigXmlFile(XmlNode section)
		{
			if (section.Attributes[SOURCE_ATTRIBUTE] == null)
			{
				throw new ConfigurationException("Could not find the source attribute of the JSTools configuration section!");
			}

			FileStream xmlDocument = null;

			try
			{
				xmlDocument = new FileStream(section.Attributes[SOURCE_ATTRIBUTE].Value, FileMode.Open, FileAccess.Read);
				_configurationNode.Load(xmlDocument);
			}
			catch(Exception e)
			{
				throw new ConfigurationException("Could not read from the given file '" + section.Attributes[SOURCE_ATTRIBUTE].Value + "'! Error description: " + e.Message);
			}
			finally
			{
				if (xmlDocument != null)
				{
					xmlDocument.Close();
				}
			}
		}
	}
}

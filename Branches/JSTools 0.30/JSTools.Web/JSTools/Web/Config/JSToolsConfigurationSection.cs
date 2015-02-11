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

/// <file>
///     <copyright see="prj:///doc/copyright.txt"/>
///     <license see="prj:///doc/license.txt"/>
///     <owner name="Silvan Gehrig" email="silvan.gehrig@mcdark.ch"/>
///     <version value="$version"/>
///     <since>JSTools.dll 0.1.0</since>
/// </file>

using System;
using System.Configuration;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;

using JSTools.Config;

namespace JSTools.Web.Config
{
	/// <summary>
	/// Represents the &lt;JSTools.net&gt; section in the web.config file.
	/// </summary>
	public class JSToolsConfigurationSection : IConfigurationSectionHandler
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private	const	string		STANDALONE_ATTRIBUTE	= "standalone";
		private	const	string		SOURCE_ATTRIBUTE		= "source";
		private	const	string		FILE_CONFIG_DOC_ELEMENT	= "configuration";

		private			XmlDocument	_configurationNode		= new XmlDocument();
		private			bool		_standalone				= false;


		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSToolsConfigurationSection instance.
		/// </summary>
		public JSToolsConfigurationSection()
		{
		}


		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSToolsConfigurationProxy instance that represents a proxy object for the
		/// JSToolsConfiguration instance.
		/// Have a look at <see cref="IConfigurationSectionHandler.Create"/>IConfigurationSectionHandler.Create</see>.
		/// </summary>
		/// <returns>Returns a XmlDocument, which contains the JSTools configuration section.</returns>
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
			return new JSToolsConfigurationProxy(_configurationNode);
		}


		/// <summary>
		/// Stores the boolean of the given standalone attribute into the private _standalone variable.
		/// </summary>
		/// <param name="section"></param>
		private void InitStandaloneAttribute(XmlNode section)
		{
			if (section.Attributes[STANDALONE_ATTRIBUTE] != null)
			{
				_standalone = (section.Attributes[STANDALONE_ATTRIBUTE].Value == "true");
			}
		}


		/// <summary>
		/// Initializes the given xml node and appends the configuration to a new XmlDocument node.
		/// </summary>
		/// <param name="section">Node which contains the configuration section.</param>
		private void InitConfigXmlNode(XmlNode section)
		{
			XmlDeclaration delcaration	= _configurationNode.CreateXmlDeclaration("1.0", "UTF-8", "yes");
			_configurationNode.AppendChild(delcaration);

			XmlElement documentElement	=_configurationNode.CreateElement(FILE_CONFIG_DOC_ELEMENT);

			for (int i = 0; i < section.ChildNodes.Count; ++i)
			{
				documentElement.AppendChild(_configurationNode.ImportNode(section.ChildNodes[i], false));
			}
			_configurationNode.AppendChild(documentElement);
		}


		/// <summary>
		/// Initializes the file path, which was given by the source attribute of the JSTools.net
		/// configuration section.
		/// </summary>
		/// <param name="section">Node which contains the configuration section.</param>
		/// <remarks>This method can throw a ConfigurationException.</remarks>
		private void InitConfigXmlFile(XmlNode section)
		{
			if (section.Attributes[SOURCE_ATTRIBUTE] == null)
			{
				throw new ConfigurationException("Could not find the source attribute of the JSTools configuration section!");
			}

			FileStream xmlStream = null;

			try
			{
				xmlStream = new FileStream(section.Attributes[SOURCE_ATTRIBUTE].Value, FileMode.Open, FileAccess.Read);
				_configurationNode.Load(xmlStream);
			}
			catch(IOException e)
			{
				throw new ConfigurationException("Could not read the given file '" + section.Attributes[SOURCE_ATTRIBUTE].Value + "'! Error description: " + e.Message);
			}
			catch(XmlException e)
			{
				throw new ConfigurationException("The given file is not well formated! Error description: " + e.Message);
			}
			finally
			{
				if (xmlStream != null)
				{
					xmlStream.Close();
				}
			}
		}
	}
}

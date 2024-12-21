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
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Xml;

namespace JSTools.Context
{
	/// <summary>
	/// Represents an interface which is used to determine the configuration
	/// document for the current environment (e.g. asp.net or win-app).
	/// </summary>
	public class WebContextConfigHandler : IContextConfigHandler
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const string STANDALONE_ATTRIBUTE = "standalone";
		private const string SOURCE_ATTRIBUTE = "source";
		private const string FILE_CONFIG_DOC_ELEMENT = "configuration";

		private XmlNode _sectionToInitialize = null;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// This event should be fired if the configuration has changed.
		/// The associated context will be reinitialzed.
		/// </summary>
		public event EventHandler Refresh
		{
			add { }
			remove { }
		}

		/// <summary>
		/// Gets the configuration document which contains the
		/// configuration settings for the current environment.
		/// </summary>
		public XmlDocument Configuration
		{
			get
			{
				if (IsStandaloneConfig(_sectionToInitialize))
					return InitConfigXmlNode(_sectionToInitialize);
				else
					return InitConfigXmlFile(_sectionToInitialize);
			}
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new WebContextConfigHandler instance.
		/// </summary>
		/// <param name="toInitialize">XmlNode to initialize.</param>
		internal WebContextConfigHandler(XmlNode toInitialize)
		{
			_sectionToInitialize = toInitialize;
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Stores the boolean of the given standalone attribute into the private _standalone variable.
		/// </summary>
		/// <param name="section"></param>
		private bool IsStandaloneConfig(XmlNode section)
		{
			if (section.Attributes[STANDALONE_ATTRIBUTE] != null)
				return (section.Attributes[STANDALONE_ATTRIBUTE].Value.ToLower() == bool.TrueString.ToLower());

			return true;
		}

		/// <summary>
		/// Initializes the given xml node and appends the configuration to a new XmlDocument node.
		/// </summary>
		/// <param name="section">Node which contains the configuration section.</param>
		private XmlDocument InitConfigXmlNode(XmlNode section)
		{
			XmlDocument configuration = new XmlDocument();
			XmlDeclaration delcaration = configuration.CreateXmlDeclaration("1.0", "UTF-8", "yes");
			configuration.AppendChild(delcaration);

			XmlElement documentElement =configuration.CreateElement(FILE_CONFIG_DOC_ELEMENT);

			foreach (XmlNode childNode in section.ChildNodes)
			{
				documentElement.AppendChild(configuration.ImportNode(childNode, false));
			}

			configuration.AppendChild(documentElement);
			return configuration;
		}

		/// <summary>
		/// Initializes the file path, which was given by the source attribute of the JSTools.net
		/// configuration section.
		/// </summary>
		/// <param name="section">Node which contains the configuration section.</param>
		/// <exception cref="ConfigurationException">Could not read the given configuration file.</exception>
		private XmlDocument InitConfigXmlFile(XmlNode section)
		{
			if (section.Attributes[SOURCE_ATTRIBUTE] == null)
				throw new ConfigurationException("Could not find the source attribute of the JSTools configuration section.");

			string configSource = section.Attributes[SOURCE_ATTRIBUTE].Value;
			XmlDocument configuration = new XmlDocument();

			try
			{
				// load xml configuration file
				using (FileStream xmlStream = new FileStream(configSource, FileMode.Open, FileAccess.Read))
				{
					configuration.Load(xmlStream);
				}
			}
			catch (XmlException e)
			{
				throw new ConfigurationException("The given file is not well formated. Error description: " + e.Message);
			}
			catch (Exception e)
			{
				throw new ConfigurationException("Could not read the given configuration file '" + section.Attributes[SOURCE_ATTRIBUTE].Value + "'. Error description: " + e.Message);
			}
			return configuration;
		}
	}
}

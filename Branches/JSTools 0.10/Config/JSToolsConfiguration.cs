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
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Xml;

namespace JSTools.Config
{
	/// <summary>
	/// 
	/// </summary>
	[Flags]
	public enum ExceptionHandling
	{
		LogError	= 0x01,
		AlertError	= 0x02,
		CatchError	= 0x04
	}


	/// <summary>
	/// Contains all configuration capatibilities of the JSTools Framework for the asp.net
	/// environment. The application configuration instance is immutable, session and page specific
	/// instances are writeable.
	/// </summary>
	public class JSToolsConfiguration : JSConfigWriter, ICloneable
	{
		private const	string						DEFAULT_CONFIG	= "JSTools.config";
		private static	JSToolsConfiguration		_instance		= null;

		private			XmlDocument					_configDocument	= null;


		/// <summary>
		/// Initializes a new JavaScript configuration handler instance.
		/// </summary>
		/// <param name="configFilePath">Loads the configuration from the specified url.</param>
		/// <remarks>This constructor can throw an ArgumentException.</remarks>
		private JSToolsConfiguration(string configFilePath)
		{
			if (configFilePath == null)
			{
				throw new ArgumentException("The given configuration contains a null reference!");
			}

			try
			{
				_configDocument = new XmlDocument();
				_configDocument.Load(configFilePath);
			}
			catch
			{
				throw new ArgumentException("Could not open the default '" + configFilePath + "' configuration path!");
			}
			InitConfiguration();
		}


		/// <summary>
		/// Initializes a new JavaScript configuration handler instance.
		/// </summary>
		/// <param name="configDocument">Loads the configuration from the specified xml document.</param>
		/// <remarks>This property can throw an ArgumentException.</remarks>
		public JSToolsConfiguration(XmlDocument configDocument)
		{
			if (configDocument == null)
			{
				throw new ArgumentException("The given xml document contains a null reference!");
			}

			_configDocument = configDocument;
			InitConfiguration();
		}


		/// <summary>
		/// Returns a JavaScript configuration instance. The instance is static, therefore the reference of the handler
		/// is constantly the same.
		/// </summary>
		/// <remarks>This property can throw a InvalidOperationException.</remarks>
		public static JSToolsConfiguration Instance 
		{
			get
			{
				if (_instance == null)
				{
					Page currentPage = (HttpContext.Current.Handler as Page);

					if (currentPage == null)
					{
						throw new InvalidOperationException("The current web request is not initialized or has an incorrect type!");
					}
					_instance = GetConfigInstance(currentPage);
				}
				return _instance;
			}
		}


		/// <summary>
		/// Creates a duplicate of this JSToolsConfiguration instance.
		/// </summary>
		/// <returns>Returns the cloned JSToolsConfiguration instance.</returns>
		public object Clone()
		{
			return new JSToolsConfiguration((XmlDocument)_configDocument.Clone());
		}


		private void InitConfiguration()
		{
			((Page)HttpContext.Current.Handler).PreRender += new EventHandler(RenderScriptConfiguration);
		}


		private static JSToolsConfiguration GetConfigInstance(Page currentPage)
		{
			JSToolsConfiguration webConfig = (HttpContext.Current.GetConfig("JSTools.net/settings") as JSToolsConfiguration);

			if (webConfig != null)
			{
				return webConfig;
			}
			else
			{
				return new JSToolsConfiguration(currentPage.Request.PhysicalApplicationPath + DEFAULT_CONFIG);
			}
		}


		private void RenderScriptConfiguration(object sender, EventArgs e)
		{
			;
		}
	}
}

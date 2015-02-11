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
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Xml;

using JSTools.Config.ExceptionHandling;
using JSTools.Config.ScriptFileManagement;

namespace JSTools.Config.Session
{
	/// <summary>
	/// Represents a session storage handler for the asp.net web environment.
	/// </summary>
	public class JSToolsWebSessionHandler : AJSToolsSessionHandler
	{
		//------------------------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------------------------

		private	const	string					DEFAULT_WEB_CONFIG		= "JSTools.net/settings";

		private			AJSToolsConfiguration	_immutableInstance		= null;
		private			AJSToolsConfiguration	_writeableInstance		= null;


		//------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Initializes a new JSToolsWebSessionHandler instance.
		/// </summary>
		/// <param name="sessionName">Name of the session storage.</param>
		/// <param name="isDefault">True to determine the default instance.</param>
		/// <exception cref="ArgumentException">The specified value does not contain a valid session key.</exception>
		internal JSToolsWebSessionHandler(string sessionName, bool isDefault) : base(sessionName, isDefault)
		{
		}


		/// <summary>
		/// Initializes a new JSToolsWebSessionHandler instance.
		/// </summary>
		/// <param name="sessionName">Name of the session storage.</param>
		/// <exception cref="ArgumentException">The specified value does not contain a valid session key.</exception>
		internal JSToolsWebSessionHandler(string sessionName) : base(sessionName)
		{
		}


		/// <summary>
		/// Initializes a new AJSToolsSessionHandler instance.
		/// </summary>
		internal JSToolsWebSessionHandler()
		{
		}


		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Returns the writeable created instance. If the instance was not created yet,
		/// this method returns a null reference.
		/// </summary>
		/// <returns>Retruns a AJSToolsConfiguration instance.</returns>
		protected override AJSToolsConfiguration GetWriteableInstance()
		{
			if (StoreInSession)
			{
				return (GetPage().Session[SessionName] as AJSToolsConfiguration);
			}
			else
			{
				return _writeableInstance;
			}
		}


		/// <summary>
		/// Creates a new writeable session instance. This method must be overwritten for client and 
		/// web applications.
		/// </summary>
		/// <exception cref="ArgumentException">Could not load the given configuration file.</exception>
		/// <exception cref="ConfigurationException">Could not initialize a type specified in a configuration xml section.</exception>
		protected override void CreateWriteableInstance()
		{
			if (StoreInSession)
			{
				CreateWriteableSessionInstance();
			}
			else
			{
				_writeableInstance = new JSToolsConfigurationWriteable((XmlDocument)_immutableInstance.ConfigDocument.Clone(), this);
			}
		}


		/// <summary>
		/// Returns the immutable created instance. If the instance was not created yet,
		/// this method returns a null reference.
		/// </summary>
		/// <returns>Retruns a AJSToolsConfiguration instance.</returns>
		protected override AJSToolsConfiguration GetImmutableInstance()
		{
			return _immutableInstance;
		}


		/// <summary>
		/// Creates a new immutable JSToolsConfiguration instance.
		/// </summary>
		/// <exception cref="ArgumentException">Could not load the given configuration file or the given XmlDocument or the session handler contains a null reference.</exception>
		/// <exception cref="ConfigurationException">Could not initialize a type specified in a configuration xml section.</exception>
		protected override void CreateImmutableInstance()
		{
			JSToolsConfigurationProxy proxy = (HttpContext.GetAppConfig(DEFAULT_WEB_CONFIG) as JSToolsConfigurationProxy);

			if (IsDefaultInstance && proxy != null && proxy.Document != null)
			{
				_immutableInstance = new JSToolsConfiguration(proxy.Document, this);
			}
			else
			{
				_immutableInstance = new JSToolsConfiguration(this);
			}
		}


		/// <summary>
		/// Creates a new writeable configuration instance, which is stored in the session cache.
		/// </summary>
		private void CreateWriteableSessionInstance()
		{
			if ((GetPage().Session[SessionName] as AJSToolsConfiguration) == null)
			{
				GetPage().Session[SessionName] = new JSToolsConfigurationWriteable((XmlDocument)_immutableInstance.ConfigDocument.Clone(), this);
			}
		}


		/// <summary>
		/// Returns the actual page context handler.
		/// </summary>
		/// <returns>Returns a page instance or null, if no page object exists.</returns>
		private Page GetPage()
		{
			if (HttpContext.Current != null && (HttpContext.Current.Handler as Page) != null)
			{
				return (HttpContext.Current.Handler as Page);
			}
			return null;
		}
	}
}

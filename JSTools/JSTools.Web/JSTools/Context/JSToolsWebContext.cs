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
using System.Web;

namespace JSTools.Context
{
	/// <summary>
	/// Represents the JSTools context for the ASP.NET Environment. This class
	/// will be called from the JSTools.Controls.Page class.
	/// </summary>
	public class JSToolsWebContext : AJSToolsContext
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the one and only JSToolsWebContext instance.
		/// </summary>
		/// <remarks>
		/// You should not work with the global context instance because the values
		/// of the properties may change during a request. You should create an
		/// immutalbe clone and use it to perform mutliple operations on the context.
		///  <code>
		///   JSToolsWebContext webContext = JSToolsWebContext.Instance;
		///   JSToolsWebContext immutableWebContext = (JSToolsWebContext)_webContext.Clone();
		///   
		///   // do some operations with "immutableWebContext"
		///  </code>
		/// </remarks>
		public static readonly JSToolsWebContext Instance = new JSToolsWebContext();

		private const string JSTOOLS_SETTINGS_SECTION = "JSTools.net";
		private readonly string _applicationPath = null;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the path of the current application. Its dependent on the
		/// asp.net environment.
		/// </summary>
		public override string ApplicationPath
		{
			get { return _applicationPath; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates the one and only JSToolsWebContext instance. This part of
		/// code is similar to the singleton pattern.
		/// </summary>
		private JSToolsWebContext()
		{
			if (HttpContext.Current == null)
				throw new InvalidOperationException("Please use JSToolsWebContext in an asp.net environment only.");

			_applicationPath = HttpContext.Current.Request.ApplicationPath;
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new clone of the current instance.
		/// </summary>
		/// <returns>Returns the cloned instance.</returns>
		protected override AJSToolsContext CloneInstance()
		{
			return new JSToolsWebContext();
		}

		/// <summary>
		/// Creates a new IContextConfigHandler instance, which is appropriated
		/// to the current environment.
		/// </summary>
		/// <returns>Returns the created IContextConfigHandler instance.</returns>
		protected override IContextConfigHandler CreateContextConfigHandler()
		{
			IContextConfigHandler configHandler = (HttpContext.GetAppConfig(JSTOOLS_SETTINGS_SECTION) as IContextConfigHandler);

			if (configHandler == null)
				throw new ConfigurationException("Could not find a '" + JSTOOLS_SETTINGS_SECTION + "' section in the weg.config file.");

			return configHandler;
		}
	}
}

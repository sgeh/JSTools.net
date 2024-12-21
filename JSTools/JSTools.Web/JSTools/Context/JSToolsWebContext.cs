/*
 * JSTools.Web.dll / JSTools.net - A framework for JavaScript/ASP.NET applications.
 * Copyright (C) 2005  Silvan Gehrig
 *
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
 *
 * Author:
 *  Silvan Gehrig
 */

using System;
using System.Configuration;
using System.IO;
using System.Threading;
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

		private static readonly JSToolsWebContext GLOBAL_INSTANCE = new JSToolsWebContext();
		
		private const string JSTOOLS_SETTINGS_SECTION = "JSTools.net";
		private const string JSTOOLS_DATA_SLOT = "JSTools.net DataSlot";

		private string _appPath = null;	

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the global JSToolsWebContext instance. You will always
		/// obtain a clone of the global context because the properties
		/// of the global context instance may change.
		/// </summary>
		public static JSToolsWebContext Instance
		{
			get { return (JSToolsWebContext)GLOBAL_INSTANCE.Clone(); }
		}

		/// <summary>
		/// Gets the path of the current application. It's dependent on the
		/// asp.net environment.
		/// </summary>
		public override string ApplicationPath
		{
			get { return _appPath; }
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

			_appPath = HttpContext.Current.Request.ApplicationPath;
		}

		/// <summary>
		/// Creates the one and only JSToolsWebContext instance. This part of
		/// code is similar to the singleton pattern.
		/// </summary>
		private JSToolsWebContext(bool initConfig) : base(initConfig)
		{
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
			JSToolsWebContext context = new JSToolsWebContext(false);
			context._appPath = _appPath;
			return context;
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

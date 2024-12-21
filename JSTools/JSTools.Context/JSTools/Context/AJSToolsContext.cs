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
using System.Runtime.CompilerServices;
using System.Xml;

using JSTools.Config;
using JSTools.Config.ScriptFileManagement;
using JSTools.Context.Cache;
using JSTools.Context.ScriptGenerator;
using JSTools.Parser.Cruncher;
using JSTools.Util;

namespace JSTools.Context
{
	/// <summary>
	/// Represents the JSTools context for the current environment. This class
	/// is abstract because the configuration source may differ on the current
	/// enviroment (e.g. ASP.NET uses web.config file with web dependent
	/// configuration settings, weg-apps may use app.config files, ...)
	/// </summary>
	public abstract class AJSToolsContext
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const int CACHE_EXPIRATION_MINUTES = 20;

		private readonly IContextConfigHandler _configHandler = null;
		private readonly EventHandler _configEventHandler = null;

		private IJSToolsConfiguration _configuration = null;
		private ScriptCache _cache = null;
		private ScriptVersion _scriptVersion = ScriptVersion.Unkonwn;
		private JSScriptGenerator _scriptGenerator = null;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Returns a configuration section instance which contains JSTools.net
		/// configuration the settings for the current environment.
		/// </summary>
		/// <exception cref="JSToolsContextException">Could not load the given configuration document.</exception>
		public IJSToolsConfiguration Configuration
		{
			get { return _configuration; }
		}

		/// <summary>
		/// Gets a cruncher instance associated with the settings of
		/// the current configuration.
		/// </summary>
		public ScriptCruncher Cruncher
		{
			get { return ScriptCruncher.Instance; }
		}

		/// <summary>
		/// Gets a cache instance associated with the settings of
		/// the current configuration.
		/// </summary>
		public ScriptCache Cache
		{
			get { return _cache; }
		}

		/// <summary>
		/// Gets a script generator instance, which can be used to create scripts
		/// which are .
		/// </summary>
		public JSScriptGenerator ScriptGenerator
		{
			get { return _scriptGenerator; }
		}

		/// <summary>
		/// Gets the default script version specified in the configuration.
		/// </summary>
		public ScriptVersion ScriptVersion
		{
			get { return _scriptVersion; }
		}

		/// <summary>
		/// Gets the path of the current application. It may be environment
		/// dependent thus mark it as abstract.
		/// </summary>
		public abstract string ApplicationPath
		{
			get;
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSToolsWebContext instance.
		/// </summary>
		/// <exception cref="JSToolsContextException">Error while creating the configuration handler.</exception>
		protected AJSToolsContext()
		{
			try
			{
				_configHandler = CreateContextConfigHandler();
				_configEventHandler = new EventHandler(OnConfigHandlerRefresh);
				_configHandler.Refresh += _configEventHandler;
			}
			catch (Exception e)
			{
				throw new JSToolsContextException("Error while creating the configuration handler.", e);
			}

			if (_configHandler == null)
				throw new JSToolsContextException("Error while creating the configuration handler, IContextConfigHandler.CreateContextConfigHandler() has returned a null reference.");

			ReinitContext();
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		private void OnConfigHandlerRefresh(object sender, EventArgs e)
		{
			ReinitContext();
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the cached item associated with the specified path. A path
		/// may contain the unique id of a cached item. The application path
		/// is cutted off at the start of the string in order to find the
		/// right cache item.
		/// 
		/// If a configuration section is requested (e.g. path JSTools/Enum)
		/// and it is not cached yet, the script for the requested section is
		/// lazzily generated and the generated cache item is returned.
		/// </summary>
		/// <param name="path">Path which should be searched.</param>
		/// <returns>Returns a null reference or the found cache item.</returns>
		public IScriptContainer GetCachedItemByPath(string path)
		{
			if (path != null && path.Length != 0)
			{
				if (path.StartsWith(ApplicationPath))
					path = path.Substring(ApplicationPath.Length);

				if (Configuration.ScriptFileHandler.ScriptExtension == Path.GetExtension(path))
				{
					return GetCachedItem(
						path.Substring(
							(path.StartsWith("/") ? 1 : 0),
							path.Length - Configuration.ScriptFileHandler.ScriptExtension.Length - 1) );
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the cached item associated with the specified chache key.
		/// 
		/// If a configuration section is requested (e.g. path JSTools/Enum)
		/// and it is not cached yet, the script for the requested section is
		/// lazzily generated and the generated cache item is returned. Modules
		/// are not cached because the associated files may change.
		/// </summary>
		/// <param name="cacheKey">Key of the cache item to get.</param>
		/// <returns>Returns a null reference or the found cache item.</returns>
		public IScriptContainer GetCachedItem(string cacheKey)
		{
			if (cacheKey != null)
			{
				if (Cache.HasKey(cacheKey))
				{
					return Cache[cacheKey];
				}
				else
				{
					AJSToolsScriptFileSection section = Configuration.ScriptFileHandler.GetSection(cacheKey);

					if (section != null)
						return GetSectionFromCache(section);
				}
			}
			return null;
		}

		private IScriptContainer GetSectionFromCache(AJSToolsScriptFileSection section)
		{
			bool doCrunch = (section.OwnerConfiguration.ScriptFileHandler.DebugMode == DebugMode.None);
			int cacheExpiration = (doCrunch ? CACHE_EXPIRATION_MINUTES : 0);

			if (section is JSScript)
			{
				// add requested script to cache
				return Cache.AddFileToCache(
					((JSScript)section).Path,
					cacheExpiration,
					((JSScript)section).PhysicalPath,
					false,
					doCrunch,
					ScriptVersion );
			}
			else if (section is JSModule)
			{
				// render module (do not 
				JSScriptModuleRenderProcessTicket moduleTicket = new JSScriptModuleRenderProcessTicket((JSModule)section, this);
				moduleTicket.AddRenderHandler(new JSScriptModuleRenderHandler());
				section.OwnerConfiguration.Render(moduleTicket);

				// return rendered module
				return moduleTicket.ScriptContainer;
			}
			else
			{
				throw new InvalidOperationException("Invalid section requested.");
			}
		}

		/// <summary>
		/// Refreshes the current context and reinitializes the associated
		/// instances.
		/// </summary>
		public void Refresh()
		{
			ReinitContext();
		}

		/// <summary>
		///  <see cref="ConvertUtilities" />
		/// </summary>
		/// <param name="toEscape">
		///  <see cref="ConvertUtilities" />
		/// </param>
		/// <returns>
		///  <see cref="ConvertUtilities" />
		/// </returns>
		public string ScriptEscape(string toEscape)
		{
			return ConvertUtilities.ScriptEscape(toEscape);
		}

		/// <summary>
		///  <see cref="ConvertUtilities" />
		/// </summary>
		/// <param name="toUnescape">
		///  <see cref="ConvertUtilities" />
		/// </param>
		/// <returns>
		///  <see cref="ConvertUtilities" />
		/// </returns>
		public string ScriptUnescape(string toUnescape)
		{
			return ConvertUtilities.ScriptUnescape(toUnescape);
		}

		/// <summary>
		///  <see cref="ConvertUtilities" />
		/// </summary>
		/// <param name="toParse">
		///  <see cref="ConvertUtilities" />
		/// </param>
		/// <returns>
		///  <see cref="ConvertUtilities" />
		/// </returns>
		public int Hex2Dec(string toParse)
		{
			return ConvertUtilities.Hex2Dec(toParse);
		}

		/// <summary>
		///  <see cref="ConvertUtilities" />
		/// </summary>
		/// <param name="toConvert">
		///  <see cref="ConvertUtilities" />
		/// </param>
		/// <returns>
		///  <see cref="ConvertUtilities" />
		/// </returns>
		public string Dec2Hex(int toConvert)
		{
			return ConvertUtilities.Dec2Hex(toConvert);
		}

		/// <summary>
		/// Creates a new IContextConfigHandler instance, which is appropriated
		/// to the current environment.
		/// </summary>
		/// <returns>Returns the created IContextConfigHandler instance.</returns>
		protected abstract IContextConfigHandler CreateContextConfigHandler();

		/// <summary>
		/// Initializes the current context instance. This method may be
		/// called more than once.
		/// </summary>
		[MethodImpl(MethodImplOptions.Synchronized)]
		protected virtual void ReinitContext()
		{
			_configuration = InitConfiguration();
			_scriptVersion = ScriptVersionUtil.ValueToScriptVersion(_configuration.ScriptFileHandler.ScriptVersion);
			_cache = new ScriptCache(ScriptVersion);
			_scriptGenerator = new JSScriptGenerator();
		}

		private IJSToolsConfiguration InitConfiguration()
		{
			XmlDocument configuration;

			try
			{
				configuration = _configHandler.Configuration;
			}
			catch (Exception e)
			{
				throw new JSToolsContextException("Could not load the given configuration document.", e);
			}

			if (configuration == null)
				throw new JSToolsContextException("Could not load the given configuration document, IContextConfigHandler.Configuration has returned a null reference.");

			return new JSToolsConfiguration(configuration);
		}
	}
}

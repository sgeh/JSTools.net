/*
 * JSTools.Context.dll / JSTools.net - A framework for JavaScript/ASP.NET applications.
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
using System.IO;
using System.Reflection;
using System.Threading;
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
	public abstract class AJSToolsContext : ICloneable
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private readonly ReaderWriterLock LOCK = new ReaderWriterLock();

		private IContextConfigHandler _configHandler = null;
		private EventHandler _configEventHandler = null;
		private bool _watchConfig = false;

		private IJSToolsConfiguration _configuration = null;
		private ConvertUtilities _util = null;
		private ScriptCache _cache = null;
		private IScriptCruncher _cruncher = null;
		private IScriptGenerator _scriptGenerator = null;

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
			get
			{
				LOCK.AcquireReaderLock(Timeout.Infinite);

				try { return _configuration; }
				finally { LOCK.ReleaseReaderLock(); }
			}
		}

		/// <summary>
		/// Gets string utililities for client side data encryption/decryption.
		/// </summary>
		public ConvertUtilities Util
		{
			get
			{
				LOCK.AcquireReaderLock(Timeout.Infinite);

				try { return _util; }
				finally { LOCK.ReleaseReaderLock(); }			
			}
		}

		/// <summary>
		/// Gets a cruncher instance associated with the settings of
		/// the current configuration.
		/// </summary>
		public IScriptCruncher Cruncher
		{
			get
			{
				LOCK.AcquireReaderLock(Timeout.Infinite);

				try { return _cruncher; }
				finally { LOCK.ReleaseReaderLock(); }
			}		
		}

		/// <summary>
		/// Gets a cache instance associated with the settings of
		/// the current configuration.
		/// </summary>
		public ScriptCache Cache
		{
			get
			{
				LOCK.AcquireReaderLock(Timeout.Infinite);

				try { return _cache; }
				finally { LOCK.ReleaseReaderLock(); }
			}		
		}

		/// <summary>
		/// Gets a script generator instance, which can be used to create scripts
		/// which are .
		/// </summary>
		public IScriptGenerator ScriptGenerator
		{
			get
			{
				LOCK.AcquireReaderLock(Timeout.Infinite);

				try { return _scriptGenerator; }
				finally { LOCK.ReleaseReaderLock(); }
			}
		}

		/// <summary>
		/// Gets the path of the current application. It may be environment
		/// dependent thus mark it as abstract.
		/// </summary>
		public abstract string ApplicationPath
		{
			get;
		}

		private IContextConfigHandler ConfigHandler
		{
			get
			{
				if (_configHandler == null)
				{
					try
					{
						_configHandler = CreateContextConfigHandler();

						if (_watchConfig)
						{
							_configEventHandler = new EventHandler(OnConfigHandlerRefresh);
							_configHandler.Refresh += _configEventHandler;
						}
					}
					catch (Exception e)
					{
						throw new JSToolsContextException("Error while creating the configuration handler.", e);
					}
				}
				return _configHandler;
			}
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSToolsWebContext instance. This constructor
		/// initializes the configuration and watches for configuration
		/// changes.
		/// </summary>
		/// <exception cref="JSToolsContextException">Error while creating the configuration handler.</exception>
		protected AJSToolsContext() : this(true)
		{
		}

		/// <summary>
		/// Creates a new JSToolsWebContext instance. If you'd like to
		/// (re)initialize the configuration and watch for changes, you
		/// should pass 'true' to initConfig argument.
		/// </summary>
		/// <param name="initConfig">True to (re)initialize the configuration and watch for configuration changes.</param>
		/// <exception cref="JSToolsContextException">Error while creating the configuration handler.</exception>
		protected AJSToolsContext(bool initConfig)
		{
			_watchConfig = initConfig;

			if (_watchConfig)
				LockAndReinitContext();
		}

		/// <summary>
		/// Clean up used resources.
		/// </summary>
		~AJSToolsContext()
		{
			if (_configHandler != null && _configEventHandler != null)
				_configHandler.Refresh -= _configEventHandler;
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		private void OnConfigHandlerRefresh(object sender, EventArgs e)
		{
			LockAndReinitContext();
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Refreshes the current context and reinitializes the associated
		/// instances.
		/// </summary>
		/// <remarks>
		/// Caution:
		/// Clones cannot be refreshed, they are immutable.
		/// </remarks>
		public void Refresh()
		{
			LockAndReinitContext();
		}

		/// <summary>
		/// Gets the cached item associated with the specified path. A path
		/// may contain the unique id of a cached item. The application path
		/// is cutted off at the start of the string in order to find the
		/// right cache item.
		/// 
		/// <para>
		/// If a configuration section is requested (e.g. path JSTools/Enum)
		/// and it is not cached yet, the script for the requested section is
		/// lazzily generated and the generated cache item is returned.
		/// </para>
		/// </summary>
		/// <param name="path">Path which should be searched.</param>
		/// <returns>Returns a null reference or the found cache item.</returns>
		/// <exception cref="JSToolsContextException">Error while getting/creating the cache item.</exception>
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
		/// <para>
		/// If a configuration section is requested (e.g. path JSTools/Enum)
		/// and it is not cached yet, the script for the requested section is
		/// lazzily generated and the generated cache item is returned. Modules
		/// are not cached because the associated files may change.
		/// </para>
		/// </summary>
		/// <param name="cacheKey">Key of the cache item to get.</param>
		/// <returns>Returns a null reference or the found cache item.</returns>
		/// <exception cref="JSToolsContextException">Error while getting/creating the cache item.</exception>
		public IScriptContainer GetCachedItem(string cacheKey)
		{
			IScriptContainer cachedItem = null;

			try
			{
				// get item from cache
				cachedItem = Cache[cacheKey];

				// if item is not stored in the cache
				if (cachedItem == null)
				{
					AJSToolsScriptFileSection section = Configuration.ScriptFileHandler.GetSection(cacheKey);

					if (section != null)
						cachedItem = GetSectionFromCache(section);
				}
			}
			catch (Exception e)
			{
				throw new JSToolsContextException(
					string.Format("Error while getting/creating the cache item '{0}'.", cacheKey),
					e );
			}
			return cachedItem;
		}

		/// <summary>
		/// Adds the given script to the cache and returns the created cache bucket.
		/// </summary>
		/// <param name="section">Section which should be stored in the cache.</param>
		/// <returns>Returns the created cache bucket.</returns>
		protected virtual IScriptContainer GetFileScriptContainer(JSScript section)
		{
			bool doCrunch = (section.OwnerConfiguration.ScriptFileHandler.DebugMode == DebugMode.None);

			// add requested script to cache
			return Cache.AddFileToCache(
				section.Path,
				(doCrunch ? Configuration.ScriptFileHandler.CacheExpiration : 0),
				section.PhysicalPath,
				false,
				doCrunch,
				Configuration.ScriptFileHandler.ScriptVersion );
		}

		/// <summary>
		/// Renders the given module and returns a new script container instance,
		/// which contains the rendered content.
		/// </summary>
		/// <param name="section">Moudle which should be rendered.</param>
		/// <returns>Returns the rendered content.</returns>
		protected virtual IScriptContainer GetModuleScriptContainer(JSModule section)
		{
			// render module
			JSScriptModuleRenderProcessTicket moduleTicket = new JSScriptModuleRenderProcessTicket(section, this);
			moduleTicket.AddRenderHandler(new JSScriptModuleRenderHandler());
			section.OwnerConfiguration.Render(moduleTicket);

			// return rendered module
			return moduleTicket.ScriptContainer;
		}

		private IScriptContainer GetSectionFromCache(AJSToolsScriptFileSection section)
		{
			if (section is JSScript)
				return GetFileScriptContainer((JSScript)section);
			else if (section is JSModule)
				return GetModuleScriptContainer((JSModule)section);
			else
				throw new InvalidOperationException("Invalid section requested.");
		}

		#region Context Initialization

		/// <summary>
		/// Creates a new IContextConfigHandler instance, which is appropriated
		/// to the current environment.
		/// </summary>
		/// <returns>Returns the created IContextConfigHandler instance.</returns>
		protected abstract IContextConfigHandler CreateContextConfigHandler();

		/// <summary>
		/// Reinitializes the configuration, cache, script generator and 
		/// cruncher instance.
		/// </summary>
		protected virtual void ReinitContext()
		{
			_configuration = InitConfiguration();
			_util = ConvertUtilities.Instance;
			_cache = ScriptCache.Instance;
			_scriptGenerator = new JSScriptGenerator();
			_cruncher = ScriptCruncher.Instance;
		}

		private void LockAndReinitContext()
		{
			LOCK.AcquireWriterLock(Timeout.Infinite);

			try { ReinitContext(); }
			finally { LOCK.ReleaseWriterLock(); }
		}

		private IJSToolsConfiguration InitConfiguration()
		{
			XmlDocument configuration;

			try
			{
				configuration = ConfigHandler.Configuration;
			}
			catch (Exception e)
			{
				throw new JSToolsContextException("Could not load the given configuration document.", e);
			}

			if (configuration == null)
				throw new JSToolsContextException("Could not load the given configuration document, IContextConfigHandler.Configuration has returned a null reference.");

			return new JSToolsConfiguration(configuration);
		}
		#endregion

		#region ICloneable Member

		/// <summary>
		/// Creates a new immutable clone of this instance.
		/// </summary>
		/// <returns>Returns the cloned instance.</returns>
		public AJSToolsContext Clone()
		{
			AJSToolsContext clonedInstance = CloneInstance();
			LOCK.AcquireReaderLock(Timeout.Infinite);

			try
			{
				clonedInstance._configuration = _configuration;
				clonedInstance._util = _util;
				clonedInstance._cache = _cache;
				clonedInstance._scriptGenerator = _scriptGenerator;
				clonedInstance._cruncher = _cruncher;
			}
			finally { LOCK.ReleaseReaderLock(); }

			return clonedInstance;
		}

		/// <summary>
		/// Creates a new clone of the current instance.
		/// </summary>
		/// <returns>Returns the cloned instance.</returns>
		protected abstract AJSToolsContext CloneInstance();

		/// <summary>
		/// Creates a new immutable clone of this instance.
		/// </summary>
		/// <returns>Returns the cloned instance.</returns>
		object ICloneable.Clone()
		{
			return Clone();
		}

		#endregion
	}
}

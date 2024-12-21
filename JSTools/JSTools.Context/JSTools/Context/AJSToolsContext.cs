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

		private const int CACHE_EXPIRATION_MINUTES = 20;

		private readonly IContextConfigHandler _configHandler = null;
		private readonly EventHandler _configEventHandler = null;

		private ReaderWriterLock _lock = new ReaderWriterLock();
		private IJSToolsConfiguration _configuration = null;
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
				_lock.AcquireReaderLock(Timeout.Infinite);

				try { return _configuration; }
				finally { _lock.ReleaseReaderLock(); }
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
				_lock.AcquireReaderLock(Timeout.Infinite);

				try { return _cruncher; }
				finally { _lock.ReleaseReaderLock(); }
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
				_lock.AcquireReaderLock(Timeout.Infinite);

				try { return _cache; }
				finally { _lock.ReleaseReaderLock(); }
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
				_lock.AcquireReaderLock(Timeout.Infinite);

				try { return _scriptGenerator; }
				finally { _lock.ReleaseReaderLock(); }
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

			LockAndReinitContext();
		}

		/// <summary>
		/// Clean up used resources.
		/// </summary>
		~AJSToolsContext()
		{
			if (_configHandler != null)
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

		#region ICloneable Member

		/// <summary>
		/// Creates a new immutable clone of this instance.
		/// </summary>
		/// <returns>Returns the cloned instance.</returns>
		public AJSToolsContext Clone()
		{
			AJSToolsContext clonedInstance = CloneInstance();

			_lock.AcquireReaderLock(Timeout.Infinite);

			try
			{
				clonedInstance._configuration = _configuration;
				clonedInstance._cache = _cache;
				clonedInstance._scriptGenerator = _scriptGenerator;
				clonedInstance._cruncher = _cruncher;
			}
			finally { _lock.ReleaseReaderLock(); }

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
		/// If a configuration section is requested (e.g. path JSTools/Enum)
		/// and it is not cached yet, the script for the requested section is
		/// lazzily generated and the generated cache item is returned.
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
		/// If a configuration section is requested (e.g. path JSTools/Enum)
		/// and it is not cached yet, the script for the requested section is
		/// lazzily generated and the generated cache item is returned. Modules
		/// are not cached because the associated files may change.
		/// </summary>
		/// <param name="cacheKey">Key of the cache item to get.</param>
		/// <returns>Returns a null reference or the found cache item.</returns>
		/// <exception cref="JSToolsContextException">Error while getting/creating the cache item.</exception>
		public IScriptContainer GetCachedItem(string cacheKey)
		{
			try
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
			catch (Exception e)
			{
				throw new JSToolsContextException(
					string.Format("Error while getting/creating the cache item '{0}'.", cacheKey),
					e );
			}
		}

		/// <summary>
		/// Adds the given script to the cache and returns the created cache bucket.
		/// </summary>
		/// <param name="section">Section which should be stored in the cache.</param>
		/// <returns>Returns the created cache bucket.</returns>
		protected virtual IScriptContainer GetFileScriptContainer(JSScript section)
		{
			bool doCrunch = (section.OwnerConfiguration.ScriptFileHandler.DebugMode == DebugMode.None);
			int cacheExpiration = (doCrunch ? CACHE_EXPIRATION_MINUTES : 0);

			// add requested script to cache
			return Cache.AddFileToCache(
				section.Path,
				cacheExpiration,
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

		#region Proxy Methods

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
		
		#endregion

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
			_cache = new ScriptCache(Configuration.ScriptFileHandler.ScriptVersion);
			_scriptGenerator = new JSScriptGenerator();
			_cruncher = ScriptCruncher.Instance;
		}

		private void LockAndReinitContext()
		{
			if (_configHandler == null)
				throw new InvalidOperationException("Could not refresh the context because the current context does not provide a configuration handler instance.");

			_lock.AcquireWriterLock(Timeout.Infinite);

			try { ReinitContext(); }
			finally { _lock.ReleaseWriterLock(); }
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
		#endregion
	}
}

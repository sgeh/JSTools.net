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
using System.Runtime.CompilerServices;
using System.Web;

using JSTools.Config;
using JSTools.Config.ScriptFileManagement;
using JSTools.Web.Config;

namespace JSTools.Web
{
	/// <summary>
	/// Represents the JSTools context for the ASP.NET Environment. This class
	/// will be called from the JSTools.Controls.Page class.
	/// </summary>
	public class JSToolsWebContext
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const	string						JSTOOLS_APP_CONFIG	= "JSTools.net";
		private			IJSToolsConfiguration		_configuration		= null;
		private			JSScriptCruncher			_cruncher			= null;
		private			JSScriptCache				_cache				= null;

		public static readonly	JSToolsWebContext	Instance			= new JSToolsWebContext();

		/// <summary>
		/// Returns a configuration section instance, which has stored the settings of
		/// the web.config file.
		/// </summary>
		/// <exception cref="InvalidOperationException">Could not localize a config section with the name 'JSTools'.</exception>
		public IJSToolsConfiguration Configuration
		{
			get { return GetConfiguration(); }
		}

		public JSScriptCruncher Cruncher
		{
			get { return GetCruncher(); }
		}

		public JSScriptCache Cache
		{
			get { return GetCache(); }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSToolsWebContext instance.
		/// </summary>
		private JSToolsWebContext()
		{
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------
/*
		public string[] GetCachedScript(string cacheId)
		{
			if (Configuration.ScriptFileHandler.HasSection(cacheId))
			{
				AddSectionToCache(section);
				return GetSectionFromCache(section);
			}
			else if (Cache.HasKey(cacheId))
			{
				return new string[] { Cache[cacheId] };
			}
			return new string[0];
		}

		public void AddSectionToCache(section)
		{
			if (section is Module)
				AddModuleToCache(Module);
			if (section is Script)
				AddScriptToCache(Script);
		}		

		public void AddModuleToCache(module)
		{
			foreach (Script in module)
			{
				AddScriptToCache(script);
			}
		}

		public void AddScriptToCache(script)
		{
			if (!Cache.HasKey(script.PhysicalPath))
				Cache.AddFileToCache(script.PhysicalPath, script.PhysicalPath, true, true))
		}

		public string[] GetSectionFromCache(section)
		{
			if (section is Module)
				GetModuleFromCache(Module);
			if (section is Script)
				GetScriptFromCache(Script);
		}

		public string[] GetModuleFromCache(module)
		{
			string[] scripts = new string[module.Count];

			foreach (Script in module)
			{
				scripts[i] = GetScriptFromCache(script);
			}
			return scripts;
		}

		public string GetScriptFromCache(script)
		{
			if (Cache.HasKey(script.PhysicalPath))
				return Cache[script.PhysicalPath, (Configuration.ScriptFileSection.DebugMode == DebugMode.None)];
			else
				return null;
		}
*/
		[MethodImpl(MethodImplOptions.Synchronized)]
		private IJSToolsConfiguration GetConfiguration()
		{
			if (_configuration == null)
			{
				_configuration = (HttpContext.GetAppConfig(JSTOOLS_APP_CONFIG) as IJSToolsConfiguration);

				if (_configuration == null)
					throw new InvalidOperationException("Could not localize a valid config section with the name '" + JSTOOLS_APP_CONFIG + "'!");
			}
			return _configuration;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private JSScriptCruncher GetCruncher()
		{
			if (_cruncher == null)
				_cruncher = new JSScriptCruncher();
				
			return _cruncher;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private JSScriptCache GetCache()
		{
			if (_cache == null)
				_cache = new JSScriptCache(Cruncher, Configuration.ScriptFileHandler.ScriptVersion);

			return _cache;
		}
	}
}

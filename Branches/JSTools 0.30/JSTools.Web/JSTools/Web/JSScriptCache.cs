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
using System.Collections;
using System.IO;

namespace JSTools.Web
{
	/// <summary>
	/// Caches script files and crunches them, if required. This class is safe for multithreaded
	/// operations.
	/// </summary>
	public class JSScriptCache
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		public static readonly JSScriptCache	Instance			= new JSScriptCache();

		private Hashtable						_cache				= new Hashtable();
		private	float							_version			= 0;


		/// <summary>
		/// JavaScript version of the script files, used for crunching the script files.
		/// The given float should have a format like 1.5 or 1.2 .
		/// </summary>
		public float Version
		{
			get { return _version; }
			set { _version = value; }
		}


		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSScriptCache instance.
		/// </summary>
		private JSScriptCache()
		{
		}


		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Initializes the given script path and adds it to the cache, if it was not cached yet.
		/// </summary>
		/// <param name="scriptPath">Script path to get.</param>
		/// <param name="crunch">True to get a crunched version of the script.</param>
		/// <returns>Returns the content of a cached script.</returns>
		/// <exception cref="ArgumentNullException">The given path contains a null reference.</exception>
		/// <exception cref="IOException">Could not work with file given path.</exception>
		/// <exception cref="CruncherException">An error has occured during parsing the given string.</exception>
		/// <exception cref="NotSupportedException">The given script version is not supported.</exception>
		public string GetScript(string scriptPath, bool crunch)
		{
			lock (this)
			{
				if (scriptPath == null)
					throw new ArgumentNullException("scriptPath", "The given path contains a null reference!");

				JSCacheItem item = GetScriptFromCache(scriptPath);

				if (crunch)
				{
					return item.GetCrunchedCode(Version);
				}
				else
				{
					return item.GetCode();
				}
			}
		}


		/// <summary>
		/// Returns the last modification time of the represention file.
		/// </summary>
		/// <param name="scriptPath">File path, which is stored in the cache.</param>
		/// <returns>Returns the last modification time of the given script file. If the given file
		/// could not be found you will obtain DateTime.MinValue.</returns>
		/// <exception cref="ArgumentNullException">The given path contains a null reference.</exception>
		public DateTime GetLastUpdateOfScript(string scriptPath)
		{
			lock (this)
			{
				if (scriptPath == null)
					throw new ArgumentNullException("scriptPath", "The given path contains a null reference!");

				JSCacheItem item = GetScriptFromCache(scriptPath);
				return (item == null) ? DateTime.MinValue : item.LastModifiedTime;
			}
		}


		/// <summary>
		/// Returns a valid cache item instance.
		/// </summary>
		/// <param name="scriptPath">Path to the script.</param>
		private JSCacheItem GetScriptFromCache(string scriptPath)
		{
			JSCacheItem item = GetCacheItem(scriptPath);

			if (item == null)
			{
				InitCacheItem(scriptPath);
				item = GetCacheItem(scriptPath);
			}
			return item;
		}


		/// <summary>
		/// Gets an item, which was stored in the cache. This method is thread save.
		/// </summary>
		/// <param name="itemKey"></param>
		/// <returns></returns>
		private JSCacheItem GetCacheItem(string itemKey)
		{
			return (_cache[itemKey] as JSCacheItem);
		}


		/// <summary>
		/// Writes the given item into the cache. This method is thread save.
		/// </summary>
		/// <param name="scriptPath"></param>
		/// <param name="toWrite"></param>
		private void InitCacheItem(string scriptPath)
		{
			_cache[scriptPath] = new JSCacheItem(scriptPath);
		}


		//--------------------------------------------------------------------
		// Nested Classes
		//--------------------------------------------------------------------

		/// <summary>
		/// Represents an innner cache item.
		/// </summary>
		private class JSCacheItem
		{
			//--------------------------------------------------------------------
			// Declarations
			//--------------------------------------------------------------------

			private	FileInfo		_file;
			private string			_scriptPath			= string.Empty;

			private string			_crunchedScript		= null;
			private DateTime		_scriptEditDate		= DateTime.Now;
			private	string			_script				= string.Empty;


			/// <summary>
			/// Returns the last modification time of the represention file.
			/// </summary>
			public DateTime LastModifiedTime
			{
				get { return _scriptEditDate; }
			}


			/// <summary>
			/// Gets the file instance.
			/// </summary>
			private FileInfo FileShot
			{
				get
				{
					if (_file == null)
					{
						_file = new FileInfo(_scriptPath);
					}
					return _file;
				}
			}


			//--------------------------------------------------------------------
			// Constructors / Destructor
			//--------------------------------------------------------------------

			/// <summary>
			/// Creates a new JSScriptCache instance.
			/// </summary>
			/// <param name="scriptPath">Physical path to the representing script.</param>
			/// <param name="cruncher">Cruncher, which should parse the given script file.</param>
			/// <exception cref="ArgumentNullException">The given path contains a null reference.</exception>
			public JSCacheItem(string scriptPath)
			{
				if (scriptPath == null)
					throw new ArgumentNullException("scriptPath", "The given path contains a null reference!");

				_scriptPath = scriptPath;
			}


			//--------------------------------------------------------------------
			// Methods
			//--------------------------------------------------------------------

			/// <summary>
			/// Returns the whole script code of the given path.
			/// </summary>
			/// <exception cref="IOException">Could not work with file given path.</exception>
			public string GetCode()
			{
				try
				{
					ReadFromFile(new FileInfo(_scriptPath));
				}
				catch (Exception e)
				{
					throw new IOException("Could not work with file '" + _scriptPath + "'! Error description: " + e.Message, e);
				}
				return _script;
			}


			/// <summary>
			/// Returns the crunched script code of the given path.
			/// </summary>
			/// <exception cref="IOException">Could not work with file given path.</exception>
			/// <exception cref="CruncherException">An error has occured during parsing the given string.</exception>
			/// <exception cref="NotSupportedException">The given script version is not supported.</exception>
			public string GetCrunchedCode(float scriptVersion)
			{
				if (_crunchedScript == null)
				{
					_crunchedScript = JSScriptCruncher.Instance.Crunch(GetCode(), scriptVersion);
				}
				return _crunchedScript;
			}


			/// <summary>
			/// Updates the file data of the given file object.
			/// </summary>
			/// <param name="fileShot">FileInfo to read from.</param>
			private void ReadFromFile(FileInfo fileShot)
			{
				FileStream fileStream = null;
				StreamReader reader = null;

				try
				{
					if (fileShot.LastWriteTime != _scriptEditDate)
					{
						fileStream = fileShot.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
						reader = new StreamReader(fileStream);

						_scriptEditDate = fileShot.LastWriteTime;
						_script = reader.ReadToEnd();
					}
				}
				finally
				{
					if (reader != null)
					{
						reader.Close();
					}
					if (fileStream != null)
					{
						fileStream.Close();
					}
				}
			}
		}
	}
}

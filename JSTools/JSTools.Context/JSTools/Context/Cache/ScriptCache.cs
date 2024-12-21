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
using System.Collections;
using System.IO;

using JSTools.Parser.Cruncher;

namespace JSTools.Context.Cache
{
	/// <summary>
	/// Caches script files and crunches them, if required. This class and
	/// all its instance methods are safe for multithreaded operations.
	/// </summary>
	/// <remarks>
	/// To override some functionalities of this class, you have to derive
	/// from AJSToolsContext and override the ReinitContext method in order
	/// to return your own ScriptCache implementation.
	/// </remarks>
	public class ScriptCache : ICollection
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the one and only ScriptCache instance.
		/// </summary>
		public static ScriptCache Instance = new ScriptCache();

		private ScriptCacheCollector _collector = null;
		private Hashtable _cache = new Hashtable();
		// attention: Hashtable.Synchronized(new Hashtable()); does not provide read sync

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the cached code associated with the given cache id.
		/// </summary>
		public IScriptContainer this[string cacheId]
		{
			get { return GetBucketById(cacheId); }
		}

		/// <summary>
		/// Gets the cached code associated with the given cache id.
		/// </summary>
		/// <exception cref="CacheException">Could not read out the script code of the cache item.</exception>
		public string this[string cacheId, bool crunchedCode]
		{
			get
			{
				ScriptCacheItem bucket = GetBucketById(cacheId);

				if (bucket != null)
				{
					if (crunchedCode)
						return bucket.CrunchedScriptCode;
					else
						return bucket.ScriptCode;
				}
				return null;
			}
		}

		/// <summary>
		/// Gets an object which can be used to synchronize the
		/// access to the cache. You should lock this property if you
		/// d'like to iterate throught the cache.
		/// </summary>
		public object SyncRoot
		{
			get { return this; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSScriptCache instance.
		/// </summary>
		protected ScriptCache()
		{
			_collector = new ScriptCacheCollector(SyncRoot, _cache);
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Clears all stored items in the cache.
		/// </summary>
		public void Clear()
		{
			lock (SyncRoot)
			{
				_cache.Clear();
			}
		}

		/// <summary>
		/// Removes the cached item with the given cache id. The method call is
		/// ignored if the given cache id cannot be found.
		/// </summary>
		/// <remarks>
		/// Caution:<br/>
		/// The item is not deleted immediately. It's removed after the collector
		/// has cleaned up all expired cache items.</remarks>
		/// <param name="cacheId">Id of the cached item to remove.</param>
		public void Remove(string cacheId)
		{
			IScriptContainer specifiedItem = GetBucketById(cacheId);

			if (specifiedItem != null)
				specifiedItem.IsExpired = true;
		}

		/// <summary>
		/// Adds a new bucket to this cache object. This type of cache
		/// initializes the given data by the script argument. The data
		/// are not mutated until the cache time expires.
		/// </summary>
		/// <param name="cacheId">Id of the bucket to add.</param>
		/// <param name="cacheExpiration">Expiration time (minutes) of the bucket to add.</param>
		/// <param name="script">Script code to cache.</param>
		/// <param name="checkSyntax">True to check the syntax of the read script data.</param>
		/// <param name="crunch">True to crunch the read script data. This will implicit check the script data syntax.</param>
		/// <param name="scriptVersion">Script version, which is used to crunch and to check the script syntax.</param>
		public virtual IScriptContainer AddScriptToChache(string cacheId, int cacheExpiration, string script, bool checkSyntax, bool crunch, float scriptVersion)
		{
			return AddBucketToCache(cacheId, new ScriptDataLoader(script, scriptVersion), cacheExpiration, checkSyntax, crunch);
		}

		/// <summary>
		/// Adds a new bucket to this cache object. This type of cache reads
		/// the data of a script files and watches for the modification time
		/// of a the corresponding file.
		/// </summary>
		/// <param name="cacheId">Id of the bucket to add.</param>
		/// <param name="cacheExpiration">Expiration time (minutes) of the bucket to add.</param>
		/// <param name="scriptFilePath">Path of the script file to cache.</param>
		/// <param name="checkSyntax">True to check the syntax of the read script data.</param>
		/// <param name="crunch">True to crunch the read script data. This will implicit check the script data syntax.</param>
		/// <param name="scriptVersion">Script version, which is used to crunch and to check the script syntax.</param>
		public virtual IScriptContainer AddFileToCache(string cacheId, int cacheExpiration, string scriptFilePath, bool checkSyntax, bool crunch, float scriptVersion)
		{
			return AddBucketToCache(cacheId, new FileDataLoader(scriptFilePath, scriptVersion), cacheExpiration, checkSyntax, crunch);
		}

		/// <summary>
		/// Adds a new bucket to this cache object. The bucket type is specified
		/// by the bucketType argument.
		/// </summary>
		/// <param name="cacheId">Id of the bucket to add.</param>
		/// <param name="cacheExpiration">Expiration time (minutes) of the bucket to add. Lower than 0 means the expiration mechanism is disabled.</param>
		/// <param name="dataLoader">Data loader instance which is able to load the data if required.</param>
		/// <param name="checkSyntax">True to check the syntax of the read script data.</param>
		/// <param name="crunch">True to crunch the read script data. This will implicit check the script data syntax.</param>
		public IScriptContainer AddBucketToCache(string cacheId, ICacheDataLoader dataLoader, int cacheExpiration, bool checkSyntax, bool crunch)
		{
			if (cacheId == null)
				throw new ArgumentNullException("cacheId");

			if (dataLoader == null)
				throw new ArgumentNullException("dataLoader");

			ScriptCacheItem item = new ScriptCacheItem(dataLoader, cacheId, cacheExpiration, checkSyntax, crunch);

			if (cacheExpiration != 0)
				SetValue(cacheId, item);

			return item;
		}

		private ScriptCacheItem GetBucketById(string cacheId)
		{
			if (cacheId != null)
			{
				ScriptCacheItem foundItem = GetValue(cacheId);

				if (foundItem != null && !foundItem.IsExpired)
					return foundItem;
			}
			return null;
		}

		private void SetValue(string key, ScriptCacheItem value)
		{
			lock (SyncRoot)
			{
				_cache[key] = value;
			}
		}

		private ScriptCacheItem GetValue(string key)
		{
			lock (SyncRoot)
			{
				return (_cache[key] as ScriptCacheItem);
			}
		}

		#region ICollection Member

		bool ICollection.IsSynchronized
		{
			get { return true; }
		}

		int ICollection.Count
		{
			get 
			{
				lock (SyncRoot)
				{
					return _cache.Count;
				}
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			lock (SyncRoot)
			{
				_cache.CopyTo(array, index);
			}
		}

		#endregion

		#region IEnumerable Member

		IEnumerator IEnumerable.GetEnumerator()
		{
			lock (SyncRoot)
			{
				return _cache.GetEnumerator();
			}
		}

		#endregion
	}
}

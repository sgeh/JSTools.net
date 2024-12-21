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

		private Hashtable _cache = Hashtable.Synchronized(new Hashtable());
		private float _version = -1;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the cached code associated with the given cache id.
		/// </summary>
		public IScriptContainer this[string cacheId]
		{
			get
			{
				try
				{
					return GetBucketById(cacheId);
				}
				catch (Exception e)
				{
					throw new CacheException(cacheId, "Error while reading the cache item data.", e);
				}
			}
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

		private Hashtable Cache
		{
			get { return _cache; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSScriptCache instance.
		/// </summary>
		/// <param name="scriptVersion">
		/// JavaScript version of the script files, used for crunching the script files.
		/// The given float should have a format like 1.5 or 1.2 .
		/// </param>
		internal ScriptCache(float scriptVersion)
		{
			_version = scriptVersion;
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
			Cache.Clear();
		}

		/// <summary>
		/// Checks whether the specified id is stored in the cache.
		/// </summary>
		/// <param name="cacheId">Cache id to check.</param>
		/// <returns>Returns true if an item with the given id is stored in the cache.</returns>
		public bool HasKey(string cacheId)
		{
			return (GetBucketById(cacheId) != null);
		}

		/// <summary>
		/// Removes the cached item with the given cache id. The method call is
		/// ignored if the given cache id cannot be found.
		/// </summary>
		/// <param name="cacheId">Id of the cached item to remove.</param>
		public void Remove(string cacheId)
		{
			if (HasKey(cacheId))
				Cache.Remove(cacheId);
		}

		/// <summary>
		/// Adds a new bucket to this cache object. This type of cache
		/// initializes the given data by the script argument. The data
		/// are never mutated and stored until the user will deleted them.
		/// </summary>
		/// <param name="cacheId">Id of the bucket to add.</param>
		/// <param name="script">Script code to cache.</param>
		/// <param name="checkSyntax">True to check the syntax of the read script data.</param>
		/// <param name="crunch">True to crunch the read script data. This will implicit check the script data syntax.</param>
		public virtual IScriptContainer AddScriptToChache(string cacheId, string script, bool checkSyntax, bool crunch)
		{
			return AddScriptToChache(cacheId, -1, script, checkSyntax, crunch, _version);
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
		/// <param name="scriptFilePath">Path of the script file to cache.</param>
		/// <param name="checkSyntax">True to check the syntax of the read script data.</param>
		/// <param name="crunch">True to crunch the read script data. This will implicit check the script data syntax.</param>
		public virtual IScriptContainer AddFileToCache(string cacheId, string scriptFilePath, bool checkSyntax, bool crunch)
		{
			return AddFileToCache(cacheId, -1, scriptFilePath, checkSyntax, crunch, _version);
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
				Cache[cacheId] = item;

			return item;
		}

		private ScriptCacheItem GetBucketById(string cacheId)
		{
			if (cacheId == null)
				throw new ArgumentNullException("cacheId");

			if (Cache.ContainsKey(cacheId))
			{
				ScriptCacheItem cachedItem = (ScriptCacheItem)Cache[cacheId];

				if (!cachedItem.IsExpired)
					return cachedItem;
				else
					Cache.Remove(cacheId);
			}
			return null;
		}

		#region ICollection Member

		bool ICollection.IsSynchronized
		{
			get { return Cache.IsSynchronized; }
		}

		int ICollection.Count
		{
			get { return Cache.Count; }
		}

		void ICollection.CopyTo(Array array, int index)
		{
			Cache.CopyTo(array, index);
		}

		object ICollection.SyncRoot
		{
			get { return Cache.SyncRoot; }
		}

		#endregion

		#region IEnumerable Member

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Cache.GetEnumerator();
		}

		#endregion
	}
}

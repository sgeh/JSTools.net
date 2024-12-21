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

using JSTools.Context.Cruncher;
using JSTools.Parser.Cruncher;

namespace JSTools.Context.Cache
{
	/// <summary>
	/// Caches script files and crunches them, if required. This class is safe for multithreaded
	/// operations.
	/// </summary>
	public class JSScriptCache : ICollection
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private Hashtable _cache = Hashtable.Synchronized(new Hashtable());

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the cached code associated with the given cache id.
		/// </summary>
		/// <exception cref="ArgumentNullException">The given path contains a null reference.</exception>
		/// <exception cref="JSScriptCacheBucketException">Could not read the current cache time of the script code referenced by the DataHandle.</exception>
		/// <exception cref="JSScriptCacheBucketException">Could not read the script code referenced by the DataHandle.</exception>
		/// <exception cref="CruncherException">An error has occured during parsing the given string.</exception>
		/// <exception cref="NotSupportedException">The given script version is not supported.</exception>
		public string this[string cacheId]
		{
			get
			{
				AJSCacheBucket bucket = GetBucketById(cacheId);

				if (bucket != null)
					return bucket.GetCachedCode();

				return null;
			}
		}

		/// <summary>
		/// Gets the cached code associated with the given cache id.
		/// </summary>
		/// <exception cref="ArgumentNullException">The given path contains a null reference.</exception>
		/// <exception cref="JSScriptCacheBucketException">Could not read the current cache time of the script code referenced by the DataHandle.</exception>
		/// <exception cref="JSScriptCacheBucketException">Could not read the script code referenced by the DataHandle.</exception>
		/// <exception cref="CruncherException">An error has occured during parsing the given string.</exception>
		/// <exception cref="NotSupportedException">The given script version is not supported.</exception>
		public string this[string cacheId, bool crunchedCode]
		{
			get
			{
				AJSCacheBucket bucket = GetBucketById(cacheId);

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
		internal JSScriptCache(JSScriptCruncher cruncher, ScriptVersion scriptVersion)
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
			return Cache.ContainsKey(cacheId);
		}

		/// <summary>
		/// Removes the cached item with the given cache id.
		/// </summary>
		/// <param name="cacheId">Id of the cached item to remove.</param>
		public void Remove(string cacheId)
		{
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
		public void AddScriptToChache(string cacheId, string script, bool checkSyntax, bool crunch)
		{
			AddScriptToChache(cacheId, -1, script, checkSyntax, crunch, _version);
		}

		/// <summary>
		/// Adds a new bucket to this cache object. This type of cache
		/// initializes the given data by the script argument. The data
		/// are not mutated until the cache time expires.
		/// </summary>
		/// <param name="cacheId">Id of the bucket to add.</param>
		/// <param name="cacheExpiration">Expiration time of the bucket to add.</param>
		/// <param name="script">Script code to cache.</param>
		/// <param name="checkSyntax">True to check the syntax of the read script data.</param>
		/// <param name="crunch">True to crunch the read script data. This will implicit check the script data syntax.</param>
		/// <param name="scriptVersion">Script version, which is used to crunch and to check the script syntax.</param>
		public void AddScriptToChache(string cacheId, int cacheExpiration, string script, bool checkSyntax, bool crunch, float scriptVersion)
		{
			AddBucketToCache(cacheId, cacheExpiration, typeof(JSScriptCacheBucket), script, checkSyntax, crunch, scriptVersion);
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
		public void AddFileToCache(string cacheId, string scriptFilePath, bool checkSyntax, bool crunch)
		{
			AddFileToCache(cacheId, -1, scriptFilePath, checkSyntax, crunch, _version);
		}

		/// <summary>
		/// Adds a new bucket to this cache object. This type of cache reads
		/// the data of a script files and watches for the modification time
		/// of a the corresponding file.
		/// </summary>
		/// <param name="cacheId">Id of the bucket to add.</param>
		/// <param name="cacheExpiration">Expiration time of the bucket to add.</param>
		/// <param name="scriptFilePath">Path of the script file to cache.</param>
		/// <param name="checkSyntax">True to check the syntax of the read script data.</param>
		/// <param name="crunch">True to crunch the read script data. This will implicit check the script data syntax.</param>
		/// <param name="scriptVersion">Script version, which is used to crunch and to check the script syntax.</param>
		public void AddFileToCache(string cacheId, int cacheExpiration, string scriptFilePath, bool checkSyntax, bool crunch, float scriptVersion)
		{
			AddBucketToCache(cacheId, cacheExpiration, typeof(JSFileCacheBucket), scriptFilePath, checkSyntax, crunch, scriptVersion);
		}

		/// <summary>
		/// Adds a new bucket to this cache object. The bucket type is specified
		/// by the bucketType argument.
		/// </summary>
		/// <param name="cacheId">Id of the bucket to add.</param>
		/// <param name="bucketType">Bucket type, must be derived from AJSCacheBucket.</param>
		/// <param name="dataHandle">Data handle instance, which is used by the cache bucket to initialize the data.</param>
		/// <param name="checkSyntax">True to check the syntax of the read script data.</param>
		/// <param name="crunch">True to crunch the read script data. This will implicit check the script data syntax.</param>
		/// <param name="scriptVersion">Script version, which is used to crunch and to check the script syntax.</param>
		/// <exception cref="TypeLoadException">Could not instantiate the given bucket type.</exception>
		public void AddBucketToCache(string cacheId, Type bucketType, object dataHandle, bool checkSyntax, bool crunch, float scriptVersion)
		{
			AddBucketToCache(cacheId, -1, bucketType, dataHandle, checkSyntax, crunch, _version);
		}

		/// <summary>
		/// Adds a new bucket to this cache object. The bucket type is specified
		/// by the bucketType argument.
		/// </summary>
		/// <param name="cacheId">Id of the bucket to add.</param>
		/// <param name="cacheExpiration">Expiration time of the bucket to add.</param>
		/// <param name="bucketType">Bucket type, must be derived from AJSCacheBucket.</param>
		/// <param name="dataHandle">Data handle instance, which is used by the cache bucket to initialize the data.</param>
		/// <param name="checkSyntax">True to check the syntax of the read script data.</param>
		/// <param name="crunch">True to crunch the read script data. This will implicit check the script data syntax.</param>
		/// <param name="scriptVersion">Script version, which is used to crunch and to check the script syntax.</param>
		/// <exception cref="TypeLoadException">Could not instantiate the given bucket type.</exception>
		public void AddBucketToCache(string cacheId, int cacheExpiration, Type bucketType, object dataHandle, bool checkSyntax, bool crunch, float scriptVersion)
		{
			if (cacheId == null)
				throw new ArgumentNullException("cacheId", "The given cache id contains a null pointer!");

			if (!bucketType.IsSubclassOf(typeof(AJSCacheBucket)))
				throw new ArgumentException("The given bucket type is not derived from AJSCacheBucket!", "bucketType");

			AddBucketToCache(
				cacheId,
				InitCacheBucket(bucketType, dataHandle, checkSyntax, crunch, scriptVersion),
				cacheExpiration);
		}

		private void AddBucketToCache(string cacheId, AJSCacheBucket bucketItem, int expiration)
		{
			Cache[cacheId] = new JSScriptCacheItem(bucketItem, expiration);
		}

		private AJSCacheBucket GetBucketById(string cacheId)
		{
			if (cacheId == null)
				throw new ArgumentNullException("cacheId", "The given cache id contains a null reference!");

			if (Cache.ContainsKey(cacheId))
			{
				JSScriptCacheItem cachedItem = (JSScriptCacheItem)Cache[cacheId];

				if (!cachedItem.IsExpired)
					return cachedItem.Bucket;
				else
					Cache.Remove(cacheId);
			}
			return null;
		}

		private AJSCacheBucket InitCacheBucket(Type bucketType, object dataHandle, bool checkSyntax, bool crunch, float scriptVersion)
		{
			try
			{
				return (AJSCacheBucket)Activator.CreateInstance(bucketType,
					new object[] {
									 scriptVersion,
									 checkSyntax,
									 crunch,
									 dataHandle
								 } );
			}
			catch (TypeLoadException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new TypeLoadException("Could not instantiate the given bucket type!", e);
			}
		}


		//--------------------------------------------------------------------
		// Nested Classes
		//--------------------------------------------------------------------

		/// <summary>
		/// Represents a cache item which is used to determine the expiration
		/// time of each item.
		/// </summary>
		private class JSScriptCacheItem
		{
			//----------------------------------------------------------------
			// Declarations
			//----------------------------------------------------------------

			private AJSCacheBucket _bucket;
			private DateTime _expirationTime = DateTime.MaxValue;

			//----------------------------------------------------------------
			// Properties
			//----------------------------------------------------------------

			/// <summary>
			/// Returns true, if this item is expired.
			/// </summary>
			public bool IsExpired
			{
				get { return (_expirationTime < DateTime.Now); }
			}

			/// <summary>
			/// Gets the cached bucket.
			/// </summary>
			public AJSCacheBucket Bucket
			{
				get
				{
					if (!IsExpired)
						return _bucket;
					else
						return null;
				}
			}

			//----------------------------------------------------------------
			// Constructors / Destructor
			//----------------------------------------------------------------

			/// <summary>
			/// Initializes a new nested JSScriptCacheItem instance with the 
			/// bucket, and an expirationMinutes param.
			/// </summary>
			/// <param name="bucket">Bucket which contains the corresponding data.</param>
			/// <param name="expirationMinutes">Expiration minutes, use -1 to disable
			/// this param.</param>
			public JSScriptCacheItem(AJSCacheBucket bucket, int expirationMinutes)
			{
				_bucket = bucket;

				if (expirationMinutes > 0)
					_expirationTime = DateTime.Now.AddMinutes(expirationMinutes);
			}

			//----------------------------------------------------------------
			// Events
			//----------------------------------------------------------------

			//----------------------------------------------------------------
			// Methods
			//----------------------------------------------------------------
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

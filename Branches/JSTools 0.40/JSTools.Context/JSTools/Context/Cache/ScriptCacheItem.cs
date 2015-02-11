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
using System.Threading;

namespace JSTools.Context.Cache
{
	/// <summary>
	/// Represents a cache item used to cache script items. This cache item
	/// is used as bucket.
	/// </summary>
	internal class ScriptCacheItem : IScriptContainer
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private readonly ReaderWriterLock CACHE_LOCK = new ReaderWriterLock();
		private readonly string _key = null;

		private ICacheDataLoader _dataLoader = null;

		private DateTime _expirationTime = DateTime.MinValue;
		private DateTime _lastUpdate = DateTime.Now;

		private string _scriptCode = null;
		private string _crunchedScriptCode = null;
		private bool _crunch = false;
		private bool _checkSyntax = false;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the date of the last update.
		/// </summary>
		public DateTime LastUpdate
		{
			get
			{
				CACHE_LOCK.AcquireReaderLock(Timeout.Infinite);

				try
				{
					return _lastUpdate;
				}
				finally
				{
					CACHE_LOCK.ReleaseReaderLock();
				}
			}
		}

		/// <summary>
		/// Gets the expiration date time.
		/// </summary>
		public DateTime ExpirationTime
		{
			get { return _expirationTime; }
		}

		/// <summary>
		/// Returns true, if this item is expired.
		/// </summary>
		public bool IsExpired
		{
			get { return (_expirationTime < DateTime.Now); }
		}

		/// <summary>
		/// Gets the cached script code.
		/// </summary>
		/// <exception cref="CacheException">Could not read out the script code of the cache item.</exception>
		public virtual string ScriptCode
		{
			get
			{
				CACHE_LOCK.AcquireWriterLock(Timeout.Infinite);

				try
				{
					if (_scriptCode == null || _dataLoader.RefreshCache)
					{
						_scriptCode = _dataLoader.LoadScript(_checkSyntax);
						_lastUpdate = DateTime.Now;
					}
					return _scriptCode;
				}
				catch (Exception e)
				{
					throw new CacheException(
						_key,
						string.Format("Could not read out the script code of the cache item '{0}'.", _key),
						e );
				}
				finally
				{
					CACHE_LOCK.ReleaseWriterLock();
				}
			}
		}

		/// <summary>
		/// Crunches the cached script code and returns the crunched script.
		/// </summary>
		/// <exception cref="CacheException">Could not crunch the script code of the cache item.</exception>
		public virtual string CrunchedScriptCode
		{
			get
			{
				CACHE_LOCK.AcquireWriterLock(Timeout.Infinite);

				try
				{

					if (_crunchedScriptCode == null || _dataLoader.RefreshCache)
					{
						_crunchedScriptCode = _dataLoader.LoadCrunchedScript();
						_lastUpdate = DateTime.Now;
					}
					return _crunchedScriptCode;
				}
				catch (Exception e)
				{
					throw new CacheException(
						_key,
						string.Format("Could not crunch the script code of the cache item '{0}'.", _key),
						e );
				}
				finally
				{
					CACHE_LOCK.ReleaseWriterLock();
				}
			}
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new ScriptCacheItem instance.
		/// </summary>
		internal ScriptCacheItem(ICacheDataLoader dataLoader, string key, int expirationMinutes, bool checkSyntax, bool crunchCode)
		{
			if (dataLoader == null)
				throw new ArgumentNullException("dataLoader", "The given data loader contains a null reference.");

			if (expirationMinutes > 0)
				_expirationTime = DateTime.Now.AddMinutes(expirationMinutes);

			_dataLoader = dataLoader;
			_checkSyntax = checkSyntax;
			_crunch = crunchCode;
			_key = key;
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the code of the cached script. The script will be cruched, 
		/// if this was specified by the crunch param of the contstructor.
		/// </summary>
		/// <returns>Returns the script code of this cache bucket.</returns>
		public string GetCachedCode()
		{
			if (_crunch)
				return CrunchedScriptCode;
			else
				return ScriptCode;
		}
	}
}

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

		private readonly string KEY = null;
		private readonly ReaderWriterLock LOCK = new ReaderWriterLock();

		private ICacheDataLoader _dataLoader = null;
		private TimeSpan _expirationTime = TimeSpan.MinValue;
		private DateTime _lastAccessTime = DateTime.Now;
		private DateTime _lastUpdate = DateTime.Now;

		private string _scriptCode = null;
		private string _crunchedScriptCode = null;
		private bool _isExpired = false;
		private bool _crunch = false;
		private bool _checkSyntax = false;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the date time of the last access to this script container.
		/// </summary>
		public DateTime LastAccess
		{
			get
			{
				LOCK.AcquireReaderLock(Timeout.Infinite);

				try { return _lastAccessTime; }
				finally { LOCK.ReleaseReaderLock(); }
			}
		}

		/// <summary>
		/// Gets the date of the last update.
		/// </summary>
		public DateTime LastUpdate
		{
			get
			{
				LOCK.AcquireReaderLock(Timeout.Infinite);

				try { return _lastUpdate; }
				finally { LOCK.ReleaseReaderLock(); }
			}
		}

		/// <summary>
		/// Gets the expiration date time.
		/// </summary>
		public TimeSpan ExpirationTime
		{
			get { return _expirationTime; }
		}

		/// <summary>
		/// Returns true, if this item is expired.
		/// </summary>
		public bool IsExpired
		{
			get
			{
				LOCK.AcquireReaderLock(Timeout.Infinite);

				try { return (_isExpired || DateTime.Now - _lastAccessTime > _expirationTime); }
				finally { LOCK.ReleaseReaderLock(); }
			}
			set
			{
				LOCK.AcquireWriterLock(Timeout.Infinite);

				try { _isExpired = value; }
				finally { LOCK.ReleaseWriterLock(); }
			}
		}

		/// <summary>
		/// Gets the cached script code.
		/// </summary>
		/// <exception cref="CacheException">Could not read out the script code of the cache item.</exception>
		public virtual string ScriptCode
		{
			get
			{
				LOCK.AcquireWriterLock(Timeout.Infinite);

				try
				{
					_lastAccessTime = DateTime.Now;

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
						KEY,
						string.Format("Could not read out the script code of the cache item '{0}'.", KEY),
						e );
				}
				finally
				{
					LOCK.ReleaseWriterLock();
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
				LOCK.AcquireWriterLock(Timeout.Infinite);

				try
				{
					_lastAccessTime = DateTime.Now;

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
						KEY,
						string.Format("Could not crunch the script code of the cache item '{0}'.", KEY),
						e );
				}
				finally
				{
					LOCK.ReleaseWriterLock();
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
				_expirationTime = new TimeSpan(0, expirationMinutes, 0);

			_dataLoader = dataLoader;
			_checkSyntax = checkSyntax;
			_crunch = crunchCode;
			KEY = key;
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

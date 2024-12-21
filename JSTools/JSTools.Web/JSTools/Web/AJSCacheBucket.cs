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

namespace JSTools.Web
{
	/// <summary>
	/// Represents a cache bucket used to cache script items. All pulbic
	/// members of this class are save for multithreaded operations.
	/// </summary>
	public abstract class AJSCacheBucket
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private	float				_scriptVersion			= -1;
		private	JSScriptCruncher	_cruncher				= null;
		private	string				_cachedScriptCode		= null;
		private	string				_crunchedScriptCode		= null;
		private	bool				_checkSyntax			= false;
		private	bool				_crunch					= false;
		private	object				_dataHandle				= false;

		private DateTime			_lastCacheTime			= DateTime.MinValue;
		private ReaderWriterLock	_lastCacheTimeLock		= new ReaderWriterLock();

		/// <summary>
		/// Gets the data handle, which contains the required informations
		/// to initialize and get the script data.
		/// </summary>
		public object DataHandle
		{
			get { return _dataHandle; }
		}

		/// <summary>
		/// Gets the cached script code.
		/// </summary>
		/// <exception cref="NotSupportedException">The given script version is not supported.</exception>
		/// <exception cref="ArgumentNullException">The given string value contains a null reference.</exception>
		/// <exception cref="JSScriptCacheBucketException">Could not read the current cache time of the script code referenced by the DataHandle.</exception>
		/// <exception cref="JSScriptCacheBucketException">Could not read the script code referenced by the DataHandle.</exception>
		/// <exception cref="NotSupportedException">The given script version is not supported.</exception>
		/// <exception cref="InvalidOperationException">The given script code is invalid.</exception>
		public virtual string CachedScriptCode
		{
			get
			{
				lock (this)
				{
					DateTime currentCacheTime = TryGetCurrentCacheTime();

					if (_cachedScriptCode == null || _lastCacheTime != currentCacheTime)
					{
						_cachedScriptCode = InitializeDataFromHandle();
						SyncLastCacheTime = currentCacheTime;
					}
					return _cachedScriptCode;
				}
			}
		}

		/// <summary>
		/// Crunches the cached script code and returns the crunched script.
		/// </summary>
		/// <exception cref="JSScriptCacheBucketException">Could not read the current cache time of the script code referenced by the DataHandle.</exception>
		/// <exception cref="JSScriptCacheBucketException">Could not read the script code referenced by the DataHandle.</exception>
		/// <exception cref="CruncherException">An error has occured during parsing the given string.</exception>
		/// <exception cref="NotSupportedException">The given script version is not supported.</exception>
		/// <exception cref="InvalidOperationException">The given script code is invalid.</exception>
		public virtual string CrunchedScriptCode
		{
			get
			{
				if (_crunchedScriptCode == null)
					_crunchedScriptCode = _cruncher.Crunch(CachedScriptCode, _scriptVersion);

				return _crunchedScriptCode;
			}
		}

		/// <summary>
		/// Specifies the time stamp of the last access time of this cache bucket.
		/// </summary>
		public DateTime LastCacheTime
		{
			get { return SyncLastCacheTime; }
		}

		/// <summary>
		/// Gets the current cache time of the script given by the DataHandle object.
		/// </summary>
		protected abstract DateTime CurrentCacheTime
		{
			get;
		}

		private DateTime SyncLastCacheTime
		{
			get
			{
				_lastCacheTimeLock.AcquireReaderLock(Timeout.Infinite);

				try
				{
					return _lastCacheTime;
				}
				finally
				{
					_lastCacheTimeLock.ReleaseReaderLock();
				}
			}
			set
			{
				_lastCacheTimeLock.AcquireWriterLock(Timeout.Infinite);

				try
				{
					_lastCacheTime = value;
				}
				finally
				{
					_lastCacheTimeLock.ReleaseWriterLock();
				}
			}
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new AJSCacheBucket instance.
		/// </summary>
		public AJSCacheBucket(JSScriptCruncher cruncher, float scriptVersion, bool checkSyntax, bool crunch, object dataHandle)
		{
			_scriptVersion = scriptVersion;
			_cruncher = cruncher;
			_checkSyntax = checkSyntax;
			_crunch = crunch;
			_dataHandle = dataHandle;
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the code of the cached script. The script will be cruched, 
		/// if this was specified by the crunch param of the contstructor.
		/// </summary>
		/// <returns>Returns the script code of this cache bucket.</returns>
		/// <exception cref="JSScriptCacheBucketException">Could not read the current cache time of the script code referenced by the DataHandle.</exception>
		/// <exception cref="JSScriptCacheBucketException">Could not read the script code referenced by the DataHandle.</exception>
		/// <exception cref="CruncherException">An error has occured during parsing the given string.</exception>
		/// <exception cref="NotSupportedException">The given script version is not supported.</exception>
		public string GetCachedCode()
		{
			if (_crunch)
				return CrunchedScriptCode;
			else
				return CachedScriptCode;
		}

		/// <summary>
		/// Reads out the data of the handle specified by the DataHandle property.
		/// </summary>
		/// <returns>Returns the read script code data in a string format.</returns>
		protected abstract string GetDataFromHandle();

		private DateTime TryGetCurrentCacheTime()
		{
			try
			{
				return CurrentCacheTime;
			}
			catch (Exception e)
			{
				throw new JSScriptCacheBucketException(DataHandle, "Could not read the current cache time of the script code referenced by the DataHandle!", e);
			}
		}

		private string TryGetDataFromHandle()
		{
			try
			{
				return GetDataFromHandle();
			}
			catch (Exception e)
			{
				throw new JSScriptCacheBucketException(DataHandle, "Could not read the script code referenced by the DataHandle!", e);
			}
		}

		private string InitializeDataFromHandle()
		{
			string cachedScript = TryGetDataFromHandle();

			if (_checkSyntax && !_cruncher.CheckSyntax(cachedScript, _scriptVersion))
				throw new InvalidOperationException("The given script code is invalid!");

			return cachedScript;
		}
	}
}

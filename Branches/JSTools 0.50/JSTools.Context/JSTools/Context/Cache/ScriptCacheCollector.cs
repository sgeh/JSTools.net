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
using System.Timers;

namespace JSTools.Context.Cache
{
	/// <summary>
	/// Represents the garbagge collector for expired script cache items.
	/// </summary>
	internal class ScriptCacheCollector
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const double COLLECT_INTERVAL = 300000; // 5 min = 300s = 300'000 ms
		private Timer _collectorThread = null;
		private Hashtable _cacheToCollect = null;
		private object _syncRoot = null;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new ScriptCacheCollector instance.
		/// </summary>
		internal ScriptCacheCollector(object syncRoot, Hashtable toCollect)
		{
			if (toCollect == null)
				throw new ArgumentNullException("toCollect");

			if (syncRoot == null)
				throw new ArgumentNullException("syncRoot");

			_syncRoot = syncRoot;
			_cacheToCollect = toCollect;
			_collectorThread = new Timer(COLLECT_INTERVAL);
			_collectorThread.Elapsed += new ElapsedEventHandler(OnCollectorElapsed);
			_collectorThread.Start();
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		private void OnCollectorElapsed(object sender, ElapsedEventArgs e)
		{
			lock (_syncRoot)
			{
				ArrayList keysToCollect = new ArrayList();

				foreach (object key in _cacheToCollect.Keys)
				{
					IScriptContainer item = (IScriptContainer)_cacheToCollect[key];

					if (item.IsExpired)
						keysToCollect.Add(key);
				}

				foreach (object keyToCollect in keysToCollect)
				{
					_cacheToCollect.Remove(keyToCollect);
				}
			}		
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------
	}
}

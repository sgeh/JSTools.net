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

namespace JSTools.Web.UI.Controls
{
	/// <summary>
	/// Stores script codes into three different script containers.
	/// </summary>
	internal class ScriptCodeCache
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private Hashtable _crunchCacheItems = new Hashtable();
		private Hashtable _commentCacheItems = new Hashtable();
		private Hashtable _defaultCacheItems = new Hashtable();

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new ScriptCodeCache instance.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public ScriptCodeCache()
		{
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Checks if an entry with the given key exists in this cache.
		/// </summary>
		/// <param name="type">Optimization type of the script.</param>
		/// <param name="key">Script key.</param>
		/// <returns>Returns true, if this cache contains a script with the given key.</returns>
		/// <exception cref="ArgumentNullException">The given key contains a null reference.</exception>
		public bool Contains(ScriptOptimization type, string key)
		{
			return (GetScriptCode(type, key) != null);
		}

		/// <summary>
		/// Clears all containers.
		/// </summary>
		public void Clear()
		{
			_commentCacheItems.Clear();
			_crunchCacheItems.Clear();
			_defaultCacheItems.Clear();
		}

		/// <summary>
		/// Adds a new script item.
		/// </summary>
		/// <param name="type">Optimization type of the script.</param>
		/// <param name="key">Script key.</param>
		/// <param name="code">Code to store.</param>
		/// <exception cref="ArgumentNullException">The given key contains a null reference.</exception>
		public void AddScriptCode(ScriptOptimization type, string key, string code)
		{
			if (key == null)
				throw new ArgumentNullException("key", "The given key contains a null reference!");

			if (type == ScriptOptimization.RemoveComments)
			{
				_commentCacheItems[key] = code;
			}
			else if (type == ScriptOptimization.Crunch)
			{
				_crunchCacheItems[key] = code;
			}
			else
			{
				_defaultCacheItems[key] = code;
			}
		}

		/// <summary>
		/// Searches for a script with the given key.
		/// </summary>
		/// <param name="type">Optimization type of the script.</param>
		/// <param name="key">Script key.</param>
		/// <returns>Retruns the code with the given key or a null reference.</returns>
		/// <exception cref="ArgumentNullException">The given key contains a null reference.</exception>
		public string GetScriptCode(ScriptOptimization type, string key)
		{
			if (key == null)
				throw new ArgumentNullException("key", "The given key contains a null reference!");

			if (type == ScriptOptimization.RemoveComments)
			{
				return (_commentCacheItems[key] as string);
			}
			else if (type == ScriptOptimization.Crunch)
			{
				return (_crunchCacheItems[key] as string);
			}
			else
			{
				return (_defaultCacheItems[key] as string);
			}
		}
	}
}

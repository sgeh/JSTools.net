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

namespace JSTools.Context.Cache
{
	/// <summary>
	/// Represents the interface class for all script data loader. A data
	/// loader is used to lazily load the script data at run time. You have
	/// to override the RefreshCache property if the script data will change
	/// during the life time the cached item.
	/// </summary>
	public interface ICacheDataLoader
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Returns true if the cached item has been chanced and should be
		/// refreshed.
		/// </summary>
		bool RefreshCache
		{
			get;
		}

		/// <summary>
		/// Gets the type of script which is supported with this data loader.
		/// </summary>
		string SupportedScriptType
		{
			get;
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Loads the requested script data.
		/// </summary>
		/// <param name="checkSyntax">True to force a syntax check.</param>
		/// <returns>Returns the loaded data. The returing is cached until the
		/// RefreshCache flag is set to true.</returns>
		string LoadScript(bool checkSyntax);

		/// <summary>
		/// Loads the requested script data. After loading the data, the data
		/// is cached by the data loader.
		/// </summary>
		/// <returns>Returns the loaded data. The returing is cached until the
		/// RefreshCache flag is set to true.</returns>
		string LoadCrunchedScript();
	}
}

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

namespace JSTools.Context
{
	/// <summary>
	/// Represents an interface which is used to determin a script container.
	/// This interface is generally used in the jstools context and jstools
	/// script cache.
	/// </summary>
	public interface IScriptContainer
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the date of the last update.
		/// </summary>
		DateTime LastUpdate
		{
			get;
		}

		/// <summary>
		/// Gets the expiration date time.
		/// </summary>
		DateTime ExpirationTime
		{
			get;
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the code of the stored script.
		/// </summary>
		/// <returns>Returns the script code of this container.</returns>
		string GetCachedCode();
	}
}

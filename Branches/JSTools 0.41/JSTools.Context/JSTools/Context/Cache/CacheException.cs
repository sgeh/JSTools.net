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
	/// This exception is thrown if an implementaion of a ACacheDataLoader
	/// has thrown an exception. More informations about the exception
	/// is stored in the InnerException property.
	/// </summary>
	public class CacheException : Exception
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private string _cacheId;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the id of the cache item which has throw the exception.
		/// </summary>
		public string CacheId
		{
			get { return _cacheId; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new CacheException instance.
		/// </summary>
		/// <param name="cacheId">Id of the cache item which has thrown the exception.</param>
		/// <param name="message">Description of this exception.</param>
		/// <param name="innerException">Nested exception.</param>
		internal CacheException(string cacheId, string message, Exception innerException) : base(message, innerException)
		{
			_cacheId = cacheId;
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------
	}
}

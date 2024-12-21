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

namespace JSTools.Web
{
	/// <summary>
	/// This exception is thrown if an implementaion of a AJSCacheBucket
	/// has thrown an exception. More informations about the exception
	/// is stored in the InnerException property.
	/// </summary>
	public class JSScriptCacheBucketException : Exception
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private object _dataHandle;

		/// <summary>
		/// Gets the data handle object of the corresponding AJSCacheBucket.
		/// </summary>
		public object DataHandle
		{
			get { return _dataHandle; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSScriptCacheBucketException with a data handle
		/// and a message argument.
		/// </summary>
		/// <param name="dataHandle">The data handle object of the corresponding AJSCacheBucket</param>
		/// <param name="message">Description of this exception.</param>
		public JSScriptCacheBucketException(object dataHandle, string message) : base(message)
		{
			_dataHandle = dataHandle;
		}

		/// <summary>
		/// Creates a new JSScriptCacheBucketException with a data handle
		/// and a message argument.
		/// </summary>
		/// <param name="dataHandle">The data handle object of the corresponding AJSCacheBucket</param>
		/// <param name="message">Description of this exception.</param>
		/// <param name="innerException">Inner exception instance.</param>
		public JSScriptCacheBucketException(object dataHandle, string message, Exception innerException) : base(message, innerException)
		{
			_dataHandle = dataHandle;
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

	}
}

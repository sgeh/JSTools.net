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

namespace JSTools.Parser.Cruncher
{
	/// <summary>
	/// Summary description for CruncherException.
	/// </summary>
	public class CruncherException : ParseItemException
	{
		//------------------------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------------------------


		//------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//------------------------------------------------------------------------------------------
		
		/// <summary>
		/// Create new ParseItemException instance.
		/// </summary>
		/// <param name="message">Error message (e.g. Missing Statement).</param>
		/// <param name="errorName">Name of the error (e.g. Syntax Error)</param>
		/// <param name="lineNumber">Error line number.</param>
		/// <param name="lineOffset">Error line column.</param>
		/// <param name="errorCode">Code, in which this error has occured.</param>
		public CruncherException(string message, string errorName, int lineNumber, int lineOffset, string errorCode) :
			base(message, errorName, lineNumber, lineOffset, errorCode)
		{
		}


		/// <summary>
		/// Creates a new CruncherException instance.
		/// </summary>
		/// <param name="message">Message to throw.</param>
		/// <param name="inner">Exception to chain.</param>
		public CruncherException(string message, Exception inner) : base(message, inner)
		{
		}


		/// <summary>
		/// Creates a new CruncherException instance.
		/// </summary>
		/// <param name="message">Message to throw.</param>
		public CruncherException(string message) : base(message)
		{
		}


		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------
	}
}

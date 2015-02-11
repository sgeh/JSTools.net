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

/// <file>
///     <copyright see="prj:///doc/copyright.txt"/>
///     <license see="prj:///doc/license.txt"/>
///     <owner name="Silvan Gehrig" email="silvan.gehrig@mcdark.ch"/>
///     <version value="$version"/>
///     <since>JSTools.dll 0.1.1</since>
/// </file>

using System;

namespace JSTools.Config.ScriptFileManagement
{
	/// <summary>
	/// This excpetion will be thrown, if a module or a file is not assigned to the specified
	/// AJSToolsSessionHandler instance. You have to import the module or script instance before
	/// adding it to a new session handler. 
	/// </summary>
	public class NotImportedException : Exception
	{
		//------------------------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------------------------


		//------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Creates a new NotImportedException.
		/// </summary>
		/// <param name="innerException">Inner exception.</param>
		/// <param name="message">An error message.</param>
		public NotImportedException(Exception innerException, string message) : base(message, innerException)
		{
		}

		/// <summary>
		/// Creates a new NotImportedException.
		/// </summary>
		/// <param name="innerException">Inner exception.</param>
		/// <param name="message">An error message.</param>
		public NotImportedException(string message) : base(message)
		{
		}


		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------
	}
}

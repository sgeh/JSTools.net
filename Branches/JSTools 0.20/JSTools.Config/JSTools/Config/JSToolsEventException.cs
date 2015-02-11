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
///     <since>JSTools.dll 0.1.0</since>
/// </file>

using System;

namespace JSTools.Config
{
	/// <summary>
	/// Occurs if the user has thrown an exception during event bubbling.
	/// </summary>
	public class JSToolsEventException : Exception
	{
		//------------------------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------------------------

		private string _eventName = null;


		/// <summary>
		/// Returns the name of the event, which has thrown an exception.
		/// </summary>
		public string EventName
		{
			get { return _eventName; }
		}


		//------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSToolsEventException.
		/// </summary>
		/// <param name="innerException">Inner exception.</param>
		/// <param name="message">An error message.</param>
		public JSToolsEventException(Exception innerException, string message, string eventName) : base(message, innerException)
		{
			_eventName = eventName;
		}


		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------
	}
}

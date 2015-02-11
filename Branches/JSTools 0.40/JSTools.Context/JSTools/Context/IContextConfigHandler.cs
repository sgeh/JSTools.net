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
using System.Xml;

namespace JSTools.Context
{
	/// <summary>
	/// Represents an interface which is used to determine the configuration
	/// document for the current environment (e.g. asp.net or win-app).
	/// </summary>
	public interface IContextConfigHandler
	{
		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// This event should be fired if the configuration has changed.
		/// The associated context will be reinitialzed.
		/// </summary>
		event EventHandler Refresh;

		/// <summary>
		/// Gets the configuration document which contains the
		/// configuration settings for the current environment.
		/// </summary>
		XmlDocument Configuration
		{
			get;
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------
	}
}

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
using System.Web;

namespace JSTools.Config
{
	/// <summary>
	/// Represents a writable proxy object for the static instance of the JSToolsConfiguration object.
	/// </summary>
	public abstract class JSConfigWriter
	{
		private const string JS_MODULE_SESSION = "JSModuleSessionCache";


		//-----------------------------------------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------------------------------------


		//-----------------------------------------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------------------------------------


		//-----------------------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//-----------------------------------------------------------------------------------------------------------


		//-----------------------------------------------------------------------------------------------------------
		// Public Methods
		//-----------------------------------------------------------------------------------------------------------


		//-----------------------------------------------------------------------------------------------------------
		// Protected Methods
		//-----------------------------------------------------------------------------------------------------------	

		/// <summary>
		/// Returns a writeable configuration. The configuration will be stored in the session cache.
		/// </summary>
		/// <returns>Returns a writeable configuration object, which is a clone of the static configuration instance.</returns>
		protected JSToolsConfiguration GetWriteableConfig()
		{
			if (HttpContext.Current.Session[JS_MODULE_SESSION] == null)
			{
				HttpContext.Current.Session[JS_MODULE_SESSION] = JSToolsConfiguration.Instance.Clone();
			}
			return (JSToolsConfiguration)HttpContext.Current.Session[JS_MODULE_SESSION];
		}


		//-----------------------------------------------------------------------------------------------------------
		// Private Methods
		//-----------------------------------------------------------------------------------------------------------	
	}
}

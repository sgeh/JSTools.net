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

using JSTools.Config;

namespace JSTools.Web.Config
{
	/// <summary>
	/// Represents the JSTools configuration for the ASP.NET Environment. This class
	/// will be called from the JSTools.Controls.Page class.
	/// </summary>
	public class JSToolsWebConfiguration
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const			string						JSTOOLS_APP_CONFIG	= "JSTools.net";
		private					IJSToolsConfiguration		_configuration		= null;

		public static readonly	JSToolsWebConfiguration		Instance			= new JSToolsWebConfiguration();


		/// <summary>
		/// Returns a configuration section instance, which has stored the settings of
		/// the web.config file.
		/// </summary>
		public IJSToolsConfiguration Configuration
		{
			get { return _configuration; }
		}


		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSToolsWebConfigHandler instance.
		/// </summary>
		/// <exception cref="InvalidOperationException">Could not localize a config section with the name 'JSTools'.</exception>
		private JSToolsWebConfiguration()
		{
			JSToolsConfigurationProxy configSection = (HttpContext.GetAppConfig(JSTOOLS_APP_CONFIG) as JSToolsConfigurationProxy);

			if (configSection == null)
			{
				throw new InvalidOperationException("Could not localize a valid config section with the name '" + JSTOOLS_APP_CONFIG + "'!");
			}
			_configuration = new JSToolsConfiguration(configSection.Document);
		}


		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

	}
}

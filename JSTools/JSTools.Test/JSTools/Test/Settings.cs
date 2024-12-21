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
using System.IO;
using System.Text;
using System.Xml;
using System.Resources;
using System.Globalization;

using csUnit;

using JSTools.Config.ExceptionHandling;
using JSTools.Config;

namespace JSTools.Test
{
	/// <summary>
	/// Test settings of test dll.
	/// </summary>
	public class Settings
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		public static readonly Settings Instance = new Settings();

		private const string RESOURCE_NAME = "JSTools.Config.Test.Resources.Settings";
		private ResourceManager	_resources = null;


		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new test instance.
		/// </summary>
		private Settings()
		{
			_resources = new ResourceManager(RESOURCE_NAME, typeof(Settings).Assembly);
		}


		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Reads the value of the given name.
		/// </summary>
		/// <param name="sourceName">Name of the value to read.</param>
		/// <returns>Returns the read value.</returns>
		public string GetString(string sourceName)
		{
			return _resources.GetString(sourceName);
		}


		/// <summary>
		/// Returns the path of the JSTools test configuration file.
		/// </summary>
		public string ConfigFilePath
		{
			get { return GetString("ConfigFilePath"); }
		}


		/// <summary>
		/// Returns the path of the JSTools test configuration file.
		/// </summary>
		public string ConfigSavePath
		{
			get { return GetString("ConfigSavePath"); }
		}


		/// <summary>
		/// Returns the path of the Crunch-Script test file.
		/// </summary>
		public string CrunchFilePath
		{
			get { return GetString("CrunchFilePath"); }
		}


		/// <summary>
		/// Returns the path of the Crunch-Script test file.
		/// </summary>
		public string CrunchSavePath
		{
			get { return GetString("CrunchSavePath"); }
		}
	}
}

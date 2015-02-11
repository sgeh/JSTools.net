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
using System.IO;
using System.Text;
using System.Web.UI;
using System.Xml;

using JSTools.Config;
using JSTools.Config.ExceptionHandling;
using JSTools.Test.Resources;

using NUnit.Framework;

namespace JSTools.Test.Config
{
	/// <summary>
	/// Test of namespace JSTools.Config.
	/// </summary>
	[TestFixture]
	public class Config
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		IJSToolsConfiguration _configuration = null;

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new test instance.
		/// </summary>
		public Config()
		{
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Initialize this test instance.
		/// </summary>
		[SetUp()]
		public void SetUp()
		{	
			XmlDocument document = new XmlDocument();

			// init document data
			StreamReader reader = new StreamReader(Settings.Instance.ConfigFilePath);
			document.LoadXml(reader.ReadToEnd());

			// load document
			_configuration = new JSToolsConfiguration(document);
		}

		/// <summary>
		/// Clear up this test instance.
		/// </summary>
		[TearDown()]
		public void TearDown()
		{
			_configuration = null;
		}

		[Test()]
		public void GetConfig()
		{
			Assert.IsNotNull(_configuration.GetConfig("scripts"));
		}

		[Test()]
		public void ScriptFileHandler()
		{
			Assert.IsNotNull(_configuration.ScriptFileHandler);
		}
	}
}

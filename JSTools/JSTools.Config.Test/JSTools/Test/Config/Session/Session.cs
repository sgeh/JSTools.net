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

using csUnit;

using JSTools.Config.ExceptionHandling;
using JSTools.Config.Session;
using JSTools.Config;

namespace JSTools.Test.Config.Session
{
	/// <summary>
	/// Test of namespace JSTools.Config.
	/// </summary>
	[TestFixture]
	public class Session
	{
		//------------------------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------------------------

		private bool _createEventOccured = false;


		//------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Creates a new test instance.
		/// </summary>
		public Session()
		{
			System.Console.Out.WriteLine(System.Console.Out.NewLine + "SessionTest started at " + DateTime.Now);
		}


		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Initialize this test instance.
		/// </summary>
		[csUnit.SetUp()]
		public void SetUp()
		{
		}


		/// <summary>
		/// Clear up this test instance.
		/// </summary>
		[csUnit.TearDown()]
		public void ClearUp()
		{
		}


		[csUnit.Test()]
		public void Instance()
		{
			System.Console.Out.WriteLine("AJSToolsSessionHandler.Instance");

			System.Console.Out.WriteLine(" is " + AJSToolsSessionHandler.Instance);
			System.Console.Out.WriteLine(" should [not null]");

			if (AJSToolsSessionHandler.Instance == null)
				throw new TestFailed("The instance property must not be null!");

			System.Console.Out.WriteLine(" done");
		}


		[csUnit.Test()]
		public void CreateEnvInstance()
		{
			System.Console.Out.WriteLine("AJSToolsSessionHandler.CreateEnvInstance()");

			IJSToolsConfiguration config = AJSToolsSessionHandler.CreateEnvInstance();

			System.Console.Out.WriteLine(" is " + config);
			System.Console.Out.WriteLine(" should [not null]");

			if (config == null)
				throw new TestFailed("The created configuration must not be null!");

			System.Console.Out.WriteLine(" done");
		}


		[csUnit.Test()]
		public void TestOnWriteableCreateEvent()
		{
			System.Console.Out.WriteLine("AJSToolsSessionHandler.OnWriteableCreate");

			IJSToolsConfiguration config	= AJSToolsSessionHandler.CreateEnvInstance();
			config.LoadXml(Settings.Instance.ConfigFilePath);

			config.OnWriteableCreate		+= new JSToolsWriteableCreateEvent(OnWriteableCreate);
			config.ErrorHandling.Handling	= ErrorHandling.LogError;

			System.Console.Out.WriteLine(" is " + _createEventOccured);
			System.Console.Out.WriteLine(" should [True]");

			if (!_createEventOccured)
				throw new TestFailed("The OnWriteableCreate event was never fired!");

			System.Console.Out.WriteLine(" done");
		}


		private void OnWriteableCreate(IJSToolsConfiguration writeableInstance)
		{
			System.Console.Out.WriteLine(" -AJSToolsSessionHandler.OnWriteableCreate");

			if (writeableInstance == null)
				throw new TestFailed("The given writeable configuration instance must not be null!");

			_createEventOccured = true;
		}
	}
}

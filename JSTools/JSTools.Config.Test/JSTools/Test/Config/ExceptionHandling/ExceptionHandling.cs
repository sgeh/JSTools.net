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

namespace JSTools.Test.Config.ExceptionHandling
{
	/// <summary>
	/// Test of namespace JSTools.Config.
	/// </summary>
	[TestFixture]
	public class ExceptionHandling
	{
		//------------------------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------------------------

		private IJSToolsConfiguration	_config					= null;
		private AJSExceptionHandler		_handler				= null;

		private bool					_renderOccured			= false;
		private bool					_preRenderOccured		= false;
		private bool					_serializeOccured		= false;


		//------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Creates a new test instance.
		/// </summary>
		public ExceptionHandling()
		{
			System.Console.Out.WriteLine(System.Console.Out.NewLine + "ExceptionHandlingTest started at " + DateTime.Now);
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
			_config = AJSToolsSessionHandler.CreateEnvInstance();
			_config.LoadXml(Settings.Instance.ConfigFilePath);

			_handler				= _config.ErrorHandling;
			_handler.OnPreRender	+= new JSToolsRenderEvent(OnPreRender);
			_handler.OnRender		+= new JSToolsRenderEvent(OnRender);
			_handler.OnSerialize	+= new JSToolsSerializeEvent(OnSerialize);
		}


		/// <summary>
		/// Clear up this test instance.
		/// </summary>
		[csUnit.TearDown()]
		public void ClearUp()
		{
		}


		[csUnit.Test()]
		public void OwnerConfiguration()
		{
			System.Console.Out.WriteLine("AJSExceptionHandler.OwnerConfiguration");

			System.Console.Out.WriteLine(" is " + _handler.OwnerConfiguration);
			System.Console.Out.WriteLine(" should [not null]");

			if (_handler.OwnerConfiguration == null)
				throw new TestFailed("The OwnerConfiguration property must not be null!");

			System.Console.Out.WriteLine(" done");
		}


		[csUnit.Test()]
		public void Handling()
		{
			System.Console.Out.WriteLine("AJSExceptionHandler.Handling");

			System.Console.Out.WriteLine(" is " + _handler.Handling);
			System.Console.Out.WriteLine(" should [" + (ErrorHandling.AlertError | ErrorHandling.LogError) + "]");

			if ((_handler.Handling & (ErrorHandling.AlertError | ErrorHandling.LogError)) == 0)
				throw new TestFailed("The Handling property has an invalid value!");

			System.Console.Out.WriteLine(" done");
		}


		[csUnit.Test()]
		public void RequiredModule()
		{
			System.Console.Out.WriteLine("AJSExceptionHandler.RequiredModule");

			System.Console.Out.WriteLine(" is " + _handler.RequiredModule);
			System.Console.Out.WriteLine(" should [JSTools.Web.Diagnostics]");

			if (_handler.RequiredModule != "JSTools.Web.Diagnostics")
				throw new TestFailed("Invalid content initialized!");

			System.Console.Out.WriteLine(" done");
		}


		[csUnit.Test()]
		public void TestOnRenderEvent()
		{
			System.Console.Out.WriteLine("AJSExceptionHandler.OnPreRender & AJSExceptionHandler.OnRender");

			RenderProcessTicket ticket = new RenderProcessTicket();
			_config.Render(ticket);

			if (!_preRenderOccured)
				throw new TestFailed("The OnPreRender event was never fired!");

			if (!_renderOccured)
				throw new TestFailed("The OnRender event was never fired!");

			if (!ticket.IsModuleRendered(_handler.RequiredModule))
				throw new TestFailed("The required module was not rendered!");

			System.Console.Out.WriteLine(" done");
		}


		[csUnit.Test()]
		public void TestOnSerializeEvent()
		{
			System.Console.Out.WriteLine("AJSExceptionHandler.OnSerialize");

			_config.SaveConfiguration(Settings.Instance.ConfigSavePath);

			if (!_serializeOccured)
				throw new TestFailed("The OnSerialize event was never fired!");

			System.Console.Out.WriteLine(" done");
		}


		private void OnPreRender(RenderProcessTicket target)
		{
			System.Console.Out.WriteLine(" -AJSExceptionHandler.OnPreRender");

			System.Console.Out.WriteLine(" is " + target);
			System.Console.Out.WriteLine(" should [not null]");

			if (target == null)
				throw new TestFailed("The specified target ticket contains a null reference!");

			_preRenderOccured = true;
		}


		private void OnRender(RenderProcessTicket target)
		{
			System.Console.Out.WriteLine(" -AJSExceptionHandler.OnRender");

			System.Console.Out.WriteLine(" is " + target);
			System.Console.Out.WriteLine(" should [not null]");

			if (target == null)
				throw new TestFailed("The specified target ticket contains a null reference!");

			_renderOccured = true;
		}


		private void OnSerialize(XmlNode toAppend, bool deep)
		{
			System.Console.Out.WriteLine(" -AJSExceptionHandler.OnSerialize");

			System.Console.Out.WriteLine(" is " + toAppend);
			System.Console.Out.WriteLine(" should [not null]");

			if (toAppend == null)
				throw new TestFailed("The specified XmlNode contains a null reference!");

			_serializeOccured = true;
		}
	}
}

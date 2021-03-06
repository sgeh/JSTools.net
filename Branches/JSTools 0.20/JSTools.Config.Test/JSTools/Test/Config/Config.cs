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

namespace JSTools.Test.Config
{
	/// <summary>
	/// Test of namespace JSTools.Config.
	/// </summary>
	[TestFixture]
	public class Config
	{
		//------------------------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------------------------

		IJSToolsConfiguration _configuration = null;


		//------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Creates a new test instance.
		/// </summary>
		public Config()
		{
			System.Console.Out.WriteLine(System.Console.Out.NewLine + "ConfigTest started at " + DateTime.Now);
			_configuration = AJSToolsSessionHandler.CreateEnvInstance();
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
		public void LoadXml()
		{
			System.Console.Out.WriteLine("IJSToolsConfiguration.LoadXml(xmlDocument)");
			XmlDocument document = new XmlDocument();

			// init document data
			StreamReader reader = new StreamReader(Settings.Instance.ConfigFilePath);
			document.LoadXml(reader.ReadToEnd());

			// load document
			_configuration.LoadXml(document);
			System.Console.Out.WriteLine(" done");
		}



		[csUnit.Test()]
		public void ErrorHandling()
		{
			System.Console.Out.WriteLine("IJSToolsConfiguration.ErrorHandling");

			System.Console.Out.WriteLine(" is " + _configuration.ErrorHandling);
			System.Console.Out.WriteLine(" should [not null]");

			if (_configuration.ErrorHandling == null)
				throw new TestFailed("The ErrorHandling property must not be null!");

			System.Console.Out.WriteLine(" done");
		}


		[csUnit.Test()]
		public void GetConfig()
		{
			System.Console.Out.WriteLine("IJSToolsConfiguration.GetConfig(\"scripts\")");

			AJSToolsEventHandler renderHandler = _configuration.GetConfig("scripts");

			System.Console.Out.WriteLine(" is " + renderHandler);
			System.Console.Out.WriteLine(" should [not null]");

			if (renderHandler == null)
				throw new TestFailed("The returned render handler instance must not be null!");

			System.Console.Out.WriteLine(" done");
		}


		[csUnit.Test()]
		public void ImmutableInstance()
		{
			System.Console.Out.WriteLine("IJSToolsConfiguration.ImmutableInstance");

			System.Console.Out.WriteLine(" is " + _configuration.ImmutableInstance);
			System.Console.Out.WriteLine(" should [not null]");

			if (_configuration.ImmutableInstance == null)
				throw new TestFailed("The ImmutableInstance property must not be null!!");

			System.Console.Out.WriteLine(" done");
		}


		[csUnit.Test()]
		public void IsXmlDocumentInitialized()
		{
			System.Console.Out.WriteLine("IJSToolsConfiguration.IsXmlDocumentInitialized");

			System.Console.Out.WriteLine(" is " + _configuration.IsXmlDocumentInitialized);
			System.Console.Out.WriteLine(" should [True]");

			if (!_configuration.IsXmlDocumentInitialized)
				throw new TestFailed("The xml document was initialized!");

			System.Console.Out.WriteLine(" done");
		}


		[csUnit.Test()]
		public void Render()
		{
			System.Console.Out.WriteLine("IJSToolsConfiguration.Render(ticket)");

			RenderProcessTicket ticket = new RenderProcessTicket();
			ticket.AddRequiredModule("JSTools.Web.Layer.IE");

			if (ticket.IsModuleRendered("JSTools.Web.Layer.IE"))
				throw new TestFailed("A module cannot be rendered before the rendering was started!");

			_configuration.Render(ticket);

			if (!ticket.IsModuleRendered("JSTools.Web.Layer.IE"))
				throw new TestFailed("A module was rendered but not marked as rendered!");

			if (ticket.RenderContext == String.Empty)
				throw new TestFailed("The rendered string cannot be empty!");

			System.Console.Out.WriteLine(" done");
		}


		[csUnit.Test()]
		public void SaveConfiguration()
		{
			System.Console.Out.WriteLine("IJSToolsConfiguration.SaveConfiguration(\"" + Settings.Instance.ConfigSavePath + "\")");
			_configuration.SaveConfiguration(Settings.Instance.ConfigSavePath, Encoding.ASCII);
			System.Console.Out.WriteLine(" done");
		}


		[csUnit.Test()]
		public void ScriptFileHandler()
		{
			System.Console.Out.WriteLine("IJSToolsConfiguration.ScriptFileHandler");

			System.Console.Out.WriteLine(" is " + _configuration.ScriptFileHandler);
			System.Console.Out.WriteLine(" should [not null]");

			if (_configuration.ScriptFileHandler == null)
				throw new TestFailed("The ScriptFileHandler property must not be null!");

			System.Console.Out.WriteLine(" done");
		}
	}
}

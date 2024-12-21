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
using System.Text;
using System.Web.UI;
using System.Xml;

using JSTools.Config;
using JSTools.Config.ScriptFileManagement;
using JSTools.Config.ScriptLoader;
using JSTools.Context;
using JSTools.Context.ScriptGenerator;
using JSTools.Web.UI.Controls;

namespace JSTools.Web.Config
{
	/// <summary>
	/// Represents a render handler instance for the JSScriptLoader section.
	/// </summary>
	public class JSScriptLoaderRenderHandler : IJSToolsRenderHandler
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const string SCRIPT_LANGUAGE = "JSTools.ScriptLoader.ScriptLanguage";
		private const string ENCODE_FILE_LOC = "JSTools.ScriptLoader.EncodeFileLocation";
		private const string SCRIPT_VERSION = "JSTools.ScriptLoader.ScriptVersion";
		private const string SCRIPT_EXTENSION = "JSTools.ScriptLoader.ScriptExtension";
		private const string SCRIPT_FILE_LOC = "JSTools.ScriptLoader.ScriptFileLocation";

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Name of the section to render with this render handler.
		/// </summary>
		public string SectionName
		{
			get { return JSScriptLoaderHandlerFactory.SECTION_NAME; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSScriptLoaderRenderHandler instance.
		/// </summary>
		public JSScriptLoaderRenderHandler()
		{
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// The configuration will call this method to render the section with the name
		/// given by the SectionName attribute.
		/// </summary>
		/// <param name="ticket">Ticket, which contains the render informations.</param>
		/// <param name="sectionToRender">Configuration section to render.</param>
		/// <exception cref="ArgumentException">The given ticket is not derived from WebRenderProcessTicket.</exception>
		public void RenderSection(RenderProcessTicket ticket, AJSToolsSection sectionToRender)
		{
			JSScriptLoaderHandler section = (sectionToRender as JSScriptLoaderHandler);
			WebRenderProcessTicket webTicket = (ticket as WebRenderProcessTicket);
			string appPath = webTicket.RenderHandler.Page.Request.ApplicationPath;

			if (webTicket == null)
				throw new ArgumentException("The given ticket is not derived from WebRenderProcessTicket!", "webTicket");

			// create script
			Script sectionScript = new Script();
			sectionScript.Code = RenderLoadScript(webTicket, section, appPath);
			webTicket.RenderHandler.Controls.Add(sectionScript);

			// create white space literal
			webTicket.RenderHandler.Controls.Add(new LiteralControl("\n"));
		}

		private string RenderLoadScript(WebRenderProcessTicket ticket, JSScriptLoaderHandler sectionHandler, string appPath)
		{
			JSScriptFileHandler scriptSection = sectionHandler.OwnerConfiguration.ScriptFileHandler;
			JSScriptWriter writer = new JSScriptWriter();

			writer.AppendAssignment(SCRIPT_LANGUAGE, scriptSection.ScriptType, true);
			writer.AppendAssignment(ENCODE_FILE_LOC, sectionHandler.EncodeFileLocation, true);
			writer.AppendAssignment(SCRIPT_VERSION, scriptSection.ScriptVersion, true);
			writer.AppendAssignment(SCRIPT_EXTENSION, scriptSection.ScriptExtension, true);
			writer.AppendAssignment(SCRIPT_FILE_LOC, GetScriptFileLocation(sectionHandler, appPath), true);

			return writer.ToString();
		}

		private string GetScriptFileLocation(JSScriptLoaderHandler sectionHandler, string appPath)
		{
			if (sectionHandler.InsertLocationPrefix)
			{
				return sectionHandler.OwnerConfiguration.ScriptFileHandler.CombinePathPrefix(appPath, sectionHandler.ScriptFileLocation);
			}
			else
			{
				return sectionHandler.ScriptFileLocation;
			}
		}
	}
}

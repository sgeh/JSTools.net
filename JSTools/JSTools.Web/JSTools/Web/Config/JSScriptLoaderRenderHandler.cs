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
using JSTools.Web.Config;
using JSTools.Web.UI.Controls;

namespace JSTools.Web.UI
{
	/// <summary>
	/// Represents a render handler instance for the JSScriptLoader section.
	/// </summary>
	public class JSScriptLoaderRenderHandler : IJSToolsRenderHandler
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

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
			sectionScript.Code = RenderLoadScript(section, appPath);
			webTicket.RenderHandler.Controls.Add(sectionScript);

			// create white space literal
			webTicket.RenderHandler.Controls.Add(new LiteralControl("\n"));
		}

		/// <summary>
		/// Renders the script loader settings.
		/// </summary>
		/// <param name="sectionHandler">Section to render.</param>
		/// <param name="appPath">Application path.</param>
		private string RenderLoadScript(JSScriptLoaderHandler sectionHandler, string appPath)
		{
			StringBuilder toWrite = new StringBuilder();
			JSScriptFileHandler scriptSection = sectionHandler.OwnerConfiguration.ScriptFileHandler;

			RenderStringVariable(toWrite, "JSTools.ScriptLoader.ScriptLanguage", scriptSection.ScriptType);
			RenderVariable(toWrite, "JSTools.ScriptLoader.EncodeFileLocation", sectionHandler.EncodeFileLocation.ToString().ToLower());
			RenderVariable(toWrite, "JSTools.ScriptLoader.ScriptVersion", scriptSection.ScriptVersion.ToString());
			RenderStringVariable(toWrite, "JSTools.ScriptLoader.ScriptExtension", scriptSection.ScriptExtension);
			RenderStringVariable(toWrite, "JSTools.ScriptLoader.ScriptFileLocation", GetScriptFileLocation(sectionHandler, appPath, toWrite));

			return toWrite.ToString();
		}

		/// <summary>
		/// Returns the script file location, which should be used for the using directive.
		/// </summary>
		/// <param name="toWrite">Context, in which should be written.</param>
		/// <param name="appPath">Application path.</param>
		/// <param name="sectionHandler">Section to render.</param>
		/// <returns>Retruns the script file location.</returns>
		private string GetScriptFileLocation(JSScriptLoaderHandler sectionHandler, string appPath, StringBuilder toWrite)
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

		/// <summary>
		/// Renders a variable and assignes the given value with double quotes (").
		/// </summary>
		/// <param name="toWrite">Context, in which should be written.</param>
		/// <param name="variableName">Name of the variable.</param>
		/// <param name="variableValue">Value of the variable.</param>
		private void RenderStringVariable(StringBuilder toWrite, string variableName, string variableValue)
		{
			toWrite.Append("\n");
			toWrite.Append(variableName);
			toWrite.Append(" = \"");
			toWrite.Append(variableValue);
			toWrite.Append("\";");
		}

		/// <summary>
		/// Renders a variable and assignes the given value.
		/// </summary>
		/// <param name="toWrite">Context, in which should be written.</param>
		/// <param name="variableName">Name of the variable.</param>
		/// <param name="variableValue">Value of the variable.</param>
		private void RenderVariable(StringBuilder toWrite, string variableName, string variableValue)
		{
			toWrite.Append("\n");
			toWrite.Append(variableName);
			toWrite.Append(" = ");
			toWrite.Append(variableValue);
			toWrite.Append(";");
		}
	}
}

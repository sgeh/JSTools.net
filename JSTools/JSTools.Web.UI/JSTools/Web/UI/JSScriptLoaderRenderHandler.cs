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
using System.Text;
using System.Web.UI;
using System.Xml;

using JSTools.Config;
using JSTools.Config.ScriptFileManagement;
using JSTools.Config.ScriptLoader;
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

		private const string SCRIPT_SECTION = "// Section {0}";

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
		/// </sumary>
		public JSScriptLoaderRenderHandler()
		{
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// The configuration will call this method to render the section with the name
		/// given by the SectionName attribute.
		/// </summary>
		/// <param name="ticket">Ticket, which contains the render informations.</param>
		/// <param name="sectionToRender">Configuration section to render.</param>
		/// <exception cref="ArgumentException">The given section is not a valid JSScriptLoaderHandler instance.</exception>
		/// <exception cref="ArgumentException">The given ticket does not contain a ControlCollection item.</exception>
		/// <exception cref="ArgumentException">The given ticket does not contain a application path item.</exception>
		public void RenderSection(RenderProcessTicket ticket, AJSToolsSection sectionToRender)
		{
			JSScriptLoaderHandler section = (sectionToRender as JSScriptLoaderHandler);
			JSControlCollection controls = (ticket.Items[JSToolsPage.RENDER_HANDLER_CTRL_ATTRIBUTE] as JSControlCollection);
			string appPath = (ticket.Items[JSToolsPage.RENDER_HANDLER_APP_ATTRIBUTE] as string);

			if (section == null)
				throw new ArgumentException("The given section is not a valid JSScriptLoaderHandler instance!", "sectionToRender");

			if (controls == null)
				throw new ArgumentException("The given ticket does not contain a JSControlCollection item!", "ticket");

			if (appPath == null)
				throw new ArgumentException("The given ticket does not contain a application path item!", "ticket");

			// create script
			Script sectionScript = new Script();
			sectionScript.Code = RenderLoadScript(section, appPath);
			controls.Add(sectionScript);

			// create white space literal
			controls.Add(new LiteralControl("\n"));
		}

		/// <summary>
		/// Renders the script loader settings.
		/// </summary>
		/// <param name="toWrite">Ticket, in which should be written.</param>
		/// <param name="appPath">Application path.</param>
		/// <param name="sectionHandler">Section to render.</param>
		private string RenderLoadScript(JSScriptLoaderHandler sectionHandler, string appPath)
		{
			StringBuilder toWrite = new StringBuilder();
			toWrite.Append(string.Format(SCRIPT_SECTION, sectionHandler.SectionName));

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

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
using JSTools.Config.ExceptionHandling;
using JSTools.Web.UI.Controls;

namespace JSTools.Web.UI
{
	/// <summary>
	/// Represents a render handler instance for the JSExceptionHandler section.
	/// </summary>
	public class JSExceptionRenderHandler : IJSToolsRenderHandler
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
			get { return JSExceptionHandlerFactory.SECTION_NAME; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSExceptionRenderHandler instance.
		/// </sumary>
		public JSExceptionRenderHandler()
		{
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// The configuration will call this method to render the section with the name
		/// given by the SectionName attribute.
		/// </summary>
		/// <param name="ticket">Ticket which contains the render informations.</param>
		/// <param name="sectionToRender">Configuration section to render.</param>
		/// <exception cref="ArgumentException">The given section is not a valid JSExceptionHandler instance.</exception>
		public void RenderSection(RenderProcessTicket ticket, AJSToolsSection sectionToRender)
		{
			JSExceptionHandler section = (sectionToRender as JSExceptionHandler);
			JSControlCollection controls = (ticket.Items[JSToolsPage.RENDER_HANDLER_CTRL_ATTRIBUTE] as JSControlCollection);

			if (section == null)
				throw new ArgumentException("The given section is not a valid JSExceptionHandler instance!", "sectionToRender");

			if (controls == null)
				throw new ArgumentException("The given ticket does not contain a JSControlCollection item!", "ticket");

			StringBuilder toWrite = new StringBuilder();

			toWrite.Append(string.Format(SCRIPT_SECTION, section.SectionName));
			toWrite.Append("\n" + section.ErrorProvider + " = JSTools.Exception.Throw;");

			RenderErrorEventScript(toWrite, section);
			RenderErrorHandlingScript(toWrite, section);

			// create script
			Script handlingScript = new Script();
			handlingScript.Code = toWrite.ToString();
			controls.Add(handlingScript);

			// create white space literal
			controls.Add(new LiteralControl("\n"));
		}

		/// <summary>
		/// Renders the error event handling client script.
		/// </summary>
		/// <param name="toWrite">StringBuilder into which should be written.</param>
		/// <param name="sectionHandler">Section to render.</param>
		private void RenderErrorEventScript(StringBuilder toWrite, JSExceptionHandler sectionHandler)
		{
			if (sectionHandler.ErrorEvent == ErrorEvent.None)
			{
				toWrite.Append("\nJSTools.Exception.EventHandling = JSTools.ExceptionHandling.ErrorEvent.None;");
			}
			else if (sectionHandler.ErrorEvent == ErrorEvent.All)
			{
				toWrite.Append("\nJSTools.Exception.EventHandling = JSTools.ExceptionHandling.ErrorEvent.All;");
			}
			else
			{
				if ((sectionHandler.ErrorEvent & ErrorEvent.Log) != 0)
					toWrite.Append("\nJSTools.Exception.EventHandling |= JSTools.ExceptionHandling.ErrorEvent.Log;");

				if ((sectionHandler.ErrorEvent & ErrorEvent.Error) != 0)
					toWrite.Append("\nJSTools.Exception.EventHandling |= JSTools.ExceptionHandling.ErrorEvent.Error;");

				if ((sectionHandler.ErrorEvent & ErrorEvent.Warn) != 0)
					toWrite.Append("\nJSTools.Exception.EventHandling |= JSTools.ExceptionHandling.ErrorEvent.Warn;");
			}
		}

		/// <summary>
		/// Renders the error handling client script.
		/// </summary>
		/// <param name="toWrite">StringBuilder into which should be written.</param>
		/// <param name="sectionHandler">Section to render.</param>
		private void RenderErrorHandlingScript(StringBuilder toWrite, JSExceptionHandler sectionHandler)
		{
			if (sectionHandler.ErrorHandling == ErrorHandling.Catch)
			{
				toWrite.Append("\nJSTools.Exception.ErrorHandling = JSTools.ExceptionHandling.ErrorHandling.Catch;");
			}
			else
			{
				toWrite.Append("\nJSTools.Exception.ErrorHandling = JSTools.ExceptionHandling.ErrorHandling.Throw;");
			}
		}
	}
}

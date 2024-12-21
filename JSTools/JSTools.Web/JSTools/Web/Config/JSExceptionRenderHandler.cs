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
using JSTools.Config.ExceptionHandling;
using JSTools.Web.Config;
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

		private const string EVENT_HANDLING_SCRIPT = "\nJSTools.Exception.ErrorHandling = JSTools.ExceptionHandling.ErrorHandling.{0};";
		private const string ERROR_HANDLING_SCRIPT = "\nJSTools.Exception.EventHandling {0} JSTools.ExceptionHandling.ErrorEvent.{1};";
		private const string NATIVE_ERROR_EVENT_ASSIGNMENT = "\n{0} = JSTools.Exception.ThrowNative;";

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

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
		/// </summary>
		public JSExceptionRenderHandler()
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
		/// <param name="ticket">Ticket which contains the render informations.</param>
		/// <param name="sectionToRender">Configuration section to render.</param>
		/// <exception cref="ArgumentException">The given ticket is not derived from WebRenderProcessTicket.</exception>
		public void RenderSection(RenderProcessTicket ticket, AJSToolsSection sectionToRender)
		{
			if (!(ticket is WebRenderProcessTicket))
				throw new InvalidOperationException("The given RenderProcessTicket have to be derived from WebRenderProcessTicket.");

			if (!(sectionToRender is JSExceptionHandler))
				throw new InvalidOperationException("The given AJSToolsSection have to be derived from JSExceptionHandler.");

			JSExceptionHandler section = (JSExceptionHandler)sectionToRender;
			WebRenderProcessTicket webTicket = (WebRenderProcessTicket)ticket;
			StringBuilder toWrite = new StringBuilder();

			// do not attach native script error handling method, if it is disbled in the configuration
			if (section.ErrorHandling != ErrorHandling.None)
				toWrite.Append(string.Format(NATIVE_ERROR_EVENT_ASSIGNMENT, section.ErrorProvider));

			RenderErrorEventScript(toWrite, section.ErrorEvent);
			RenderErrorHandlingScript(toWrite, section.ErrorHandling);

			// create script
			Script handlingScript = new Script();
			handlingScript.Code = toWrite.ToString();
			webTicket.RenderHandler.Controls.Add(handlingScript);

			// create white space literal
			webTicket.RenderHandler.Controls.Add(new LiteralControl("\n"));
		}

		private void RenderErrorEventScript(StringBuilder toWrite, ErrorEvent errorEventMode)
		{
			if (errorEventMode == ErrorEvent.None)
			{
				RenderErrorEventScript(toWrite, ErrorEvent.None, "=");
			}
			else if (errorEventMode == ErrorEvent.All)
			{
				RenderErrorEventScript(toWrite, ErrorEvent.All, "=");
			}
			else
			{
				if ((errorEventMode & ErrorEvent.Log) != 0)
					RenderErrorEventScript(toWrite, ErrorEvent.Log, "|=");

				if ((errorEventMode & ErrorEvent.Error) != 0)
					RenderErrorEventScript(toWrite, ErrorEvent.Error, "|=");

				if ((errorEventMode & ErrorEvent.Warn) != 0)
					RenderErrorEventScript(toWrite, ErrorEvent.Warn, "|=");
			}
		}

		private void RenderErrorEventScript(StringBuilder toWrite, ErrorEvent valueToWrite, string assignmentOperator)
		{
			toWrite.Append(string.Format(
				ERROR_HANDLING_SCRIPT,
				assignmentOperator,
				Enum.GetName(typeof(ErrorEvent), valueToWrite)) );
		}

		private void RenderErrorHandlingScript(StringBuilder toWrite, ErrorHandling valueToWrite)
		{
			toWrite.Append(string.Format(
				EVENT_HANDLING_SCRIPT,
				Enum.GetName(typeof(ErrorHandling), valueToWrite)) );
		}
	}
}

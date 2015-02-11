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

/*
using System;
using System.Text;
using System.Xml;

using JSTools.Config.ScriptFileManagement;
using JSTools.Config;
using JSTools.Context;

namespace JSTools.Web.Request
{
	/// <summary>
	/// Represents a render hander for the configuration. It is used, if the
	/// ASP.NET module (JSScriptModule) is requesting the script code for a module
	/// a script file.
	/// 
	/// A rendered file has an expiration time of two days.
	/// </summary>
	public class JSScriptModuleRenderHandler : IJSToolsRenderHandler
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const double EXPIRATION_DAYS = 2;
		private const string DEBUG_FILE_MODE = "document.write({0});\n\n";
		private const string DEBUG_FILE_FOOTER = "\n\n\n\n";
		private const string DEBUG_FILE_HEADER =
			"//----------------------------------------------------------------------------\n"
			+ "//--- Source File: {0}\n"
			+ "//--- Last Update: {1}\n"
			+ "//----------------------------------------------------------------------------\n\n";

		private JSScriptFileHandler _section = null;
		private AJSToolsScriptFileSection _requestedSection = null;
		private string _applicationPath = null;
		private StringBuilder _renderContext = null;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Name of the section to render with this render handler.
		/// </summary>
		public string SectionName
		{
			get { return JSScriptFileSectionHandlerFactory.SECTION_NAME; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSScriptWebRenderHandler instance.
		/// </summary>
		/// <param name="requestedSection">The requested section path.</param>
		/// <param name="applicationPath">Absolute path to the current application.</param>
		/// <exception cref="ArgumentNullException">The given requested section contains a null reference.</exception>
		public JSScriptModuleRenderHandler(AJSToolsScriptFileSection requestedSection)
		{
			if (requestedSection == null)
				throw new ArgumentNullException("requestedSection", "The given requested section contains a null reference!");

			_requestedSection = requestedSection;
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
		/// <exception cref="ArgumentException">The given section is not a valid JSScriptFileHandler instance.</exception>
		/// <exception cref="ArgumentException">The given ticket does not contain an application path item.</exception>
		/// <exception cref="ArgumentException">The given ticket does not contain a render context item.</exception>
		public void RenderSection(RenderProcessTicket ticket, AJSToolsSection sectionToRender)
		{/*
			_section = (sectionToRender as JSScriptFileHandler);
			_applicationPath = (ticket.Items[JSScriptModule.RENDER_HANDLER_APPLICATION_KEY] as string);
			_renderContext = (ticket.Items[JSScriptModule.RENDER_HANDLER_CONTEXT_KEY] as StringBuilder);

			if (_section == null)
				throw new ArgumentException("The given section is not a valid JSScriptFileHandler instance!", "sectionToRender");

			if (_applicationPath == null)
				throw new ArgumentException("The given ticket does not contain an application path item!");

			if (_renderContext == null)
				throw new ArgumentException("The given ticket does not contain a render context item!");

			if (_requestedSection is JSScript)
			{
				RenderScript((JSScript)_requestedSection, ticket, true);
			}
			else if (_requestedSection is JSModule)
			{
				RenderModule((JSModule)_requestedSection, ticket);
			}
		}

		/// <summary>
		/// Renders the given module to the client.
		/// </summary>
		/// <param name="moduleToRender">Module to render.</param>
		/// <param name="ticket">Ticket into which should be written.</param>
		private void RenderModule(JSModule moduleToRender, RenderProcessTicket ticket)
		{
			if (_section.DebugMode == DebugMode.File)
			{
				RenderModuleInDebugFileMode(moduleToRender);
			}
			else if(_section.DebugMode == DebugMode.Module)
			{
				RenderModuleInDebugModuleMode(moduleToRender, ticket);
			}
			else
			{
				RenderModuleInReleaseMode(moduleToRender, ticket);
			}
		}

		/// <summary>
		/// Renders the given module in debug file mode to the client.
		/// </summary>
		/// <param name="moduleToRender">Module to render.</param>
		private void RenderModuleInDebugFileMode(JSModule moduleToRender)
		{
			foreach (JSScript script in moduleToRender.ScriptFiles)
			{
				_renderContext.Append(String.Format(DEBUG_FILE_MODE,
					_section.GetScriptFileJavaScriptTag(script, _applicationPath)));
			}
		}

		/// <summary>
		/// Renders the given module in debug module mode to the client.
		/// </summary>
		/// <param name="moduleToRender">Module to render.</param>
		/// <param name="ticket">Ticket into which should be written.</param>
		private void RenderModuleInDebugModuleMode(JSModule moduleToRender, RenderProcessTicket ticket)
		{
			foreach (JSScript script in moduleToRender.ScriptFiles)
			{
				_renderContext.Append(String.Format(DEBUG_FILE_HEADER,
					script.RequestPath,
					JSScriptCache.Instance.GetLastUpdateOfScript(script.PhysicalPath)));

				RenderScript(script, ticket, false);

				_renderContext.Append(DEBUG_FILE_FOOTER);
			}
		}

		/// <summary>
		/// Renders the given module in release mode to the client.
		/// </summary>
		/// <param name="moduleToRender">Module to render.</param>
		/// <param name="ticket">Ticket into which should be written.</param>
		private void RenderModuleInReleaseMode(JSModule moduleToRender, RenderProcessTicket ticket)
		{
			foreach (JSScript script in moduleToRender.ScriptFiles)
			{
				RenderScript(script, ticket, false);
			}
		}

		/// <summary>
		/// Render given script instance (with crunching in debug mode).
		/// </summary>
		/// <param name="scriptToRender">Script to render.</param>
		/// <param name="ticket">Ticket into which should be written.</param>
		/// <param name="renderUpdateHeader">True to fill the Last_Updated header.</param>
		private void RenderScript(JSScript scriptToRender, RenderProcessTicket ticket, bool renderUpdateHeader)
		{
			// render headers
			if (renderUpdateHeader)
				ticket.Items[JSScriptModule.RENDER_HANDLER_LAST_UPDATED] = JSToolsContext.Instance.GetLastUpdateOfScript(scriptToRender.PhysicalPath);

			// set cache header, if we are not in debug mode
			if (_section.DebugMode == DebugMode.None)
				ticket.Items[JSScriptModule.RENDER_HANDLER_DATE_EXPIRE] = DateTime.Now.AddDays(EXPIRATION_DAYS);

			// crunch, if we are not in debug mode
			_renderContext.Append(JSScriptCache.Instance.GetScript(scriptToRender.PhysicalPath, (_section.DebugMode == DebugMode.None)));
			_renderContext.Append("\n");
		}
	}
}*/

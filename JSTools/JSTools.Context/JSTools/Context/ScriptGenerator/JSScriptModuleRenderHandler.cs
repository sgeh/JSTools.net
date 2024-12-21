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

using JSTools.Config.ScriptFileManagement;
using JSTools.Config;
using JSTools.Context.Cache;

namespace JSTools.Context.ScriptGenerator
{
	/// <summary>
	/// Represents a render hander for the configuration. It is used, if the
	/// ASP.NET module (JSScriptModule) is requesting the script code for a module
	/// a script file.
	/// </summary>
	internal class JSScriptModuleRenderHandler : IJSToolsRenderHandler
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

		private JSScriptModuleRenderProcessTicket _moduleTicket = null;
		private JSScriptFileHandler _section = null;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Name of the section to render with this render handler.
		/// </summary>
		public string SectionName
		{
			get { return JSScriptFileHandlerFactory.SECTION_NAME; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSScriptWebRenderHandler instance.
		/// </summary>
		internal JSScriptModuleRenderHandler()
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
		public void RenderSection(RenderProcessTicket ticket, AJSToolsSection sectionToRender)
		{
			if (!(ticket is JSScriptModuleRenderProcessTicket))
				throw new InvalidOperationException("The given RenderProcessTicket have to be derived from JSScriptModuleRenderProcessTicket.");

			if (!(sectionToRender is JSScriptFileHandler))
				throw new InvalidOperationException("The given AJSToolsSection have to be derived from JSScriptFileHandler.");

			_moduleTicket = (JSScriptModuleRenderProcessTicket)ticket;
			_section = (JSScriptFileHandler)sectionToRender;

			if (_section.DebugMode == DebugMode.File)
				RenderModuleInDebugFileMode();
			else if (_section.DebugMode == DebugMode.Module)
				RenderModuleInDebugModuleMode();
			else
				RenderModuleInReleaseMode();
		}

		private void RenderModuleInDebugFileMode()
		{
			foreach (JSScript script in _moduleTicket.SectionToRender.ScriptFiles)
			{
				_moduleTicket.ScriptContainer.Script.Append(
					String.Format(
						DEBUG_FILE_MODE,
						_section.GetScriptFileJavaScriptTag(script, _moduleTicket.Context.ApplicationPath)) );
			}
		}

		private void RenderModuleInDebugModuleMode()
		{
			foreach (JSScript script in _moduleTicket.SectionToRender.ScriptFiles)
			{
				IScriptContainer cachedScript = _moduleTicket.Context.GetCachedItem(script.Id);

				_moduleTicket.ScriptContainer.Script.Append(
					String.Format(DEBUG_FILE_HEADER,
						script.RequestPath,
						cachedScript.LastUpdate) );

				RenderScript(cachedScript);
				_moduleTicket.ScriptContainer.Script.Append(DEBUG_FILE_FOOTER);
			}
		}

		private void RenderModuleInReleaseMode()
		{
			foreach (JSScript script in _moduleTicket.SectionToRender.ScriptFiles)
			{
				RenderScript(_moduleTicket.Context.GetCachedItem(script.Id));
			}
		}

		private void RenderScript(IScriptContainer cachedItem)
		{
			// get script from cache
			_moduleTicket.ScriptContainer.Script.Append(cachedItem.GetCachedCode());
		}
	}
}

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

		private const string DEBUG_FILE_HEADER =
			  "-----------------------------------------------------{2}"
			+ "--- Source File: {0}{2}"
			+ "--- Last Update: {1}{2}"
			+ "-----------------------------------------------------{2}{2}";

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
				// init content to render
				string scriptTag = _section.GetScriptFileJavaScriptTag(script, _moduleTicket.Context.ApplicationPath);
				string scriptOutput = _moduleTicket.Context.ScriptGenerator.CreatePlainOutput(scriptTag);

				// render content
				_moduleTicket.ScriptContainer.Script.Append(scriptOutput);
				_moduleTicket.ScriptContainer.Script.Append(_moduleTicket.Context.ScriptGenerator.LineBreak);
				_moduleTicket.ScriptContainer.Script.Append(_moduleTicket.Context.ScriptGenerator.LineBreak);
			}
		}

		private void RenderModuleInDebugModuleMode()
		{
			foreach (JSScript script in _moduleTicket.SectionToRender.ScriptFiles)
			{
				IScriptContainer cachedScript = _moduleTicket.Context.GetCachedItem(script.Id);

				// init content to render
				string header = string.Format(
					DEBUG_FILE_HEADER,
					script.RequestPath,
					cachedScript.LastUpdate,
					_moduleTicket.Context.ScriptGenerator.LineBreak );
				string headerComment = _moduleTicket.Context.ScriptGenerator.CreateMultiLineComment(header);
				
				// render content
				_moduleTicket.ScriptContainer.Script.Append(headerComment);
				RenderScript(cachedScript);
				_moduleTicket.ScriptContainer.Script.Append(_moduleTicket.Context.ScriptGenerator.LineBreak);
				_moduleTicket.ScriptContainer.Script.Append(_moduleTicket.Context.ScriptGenerator.LineBreak);
				_moduleTicket.ScriptContainer.Script.Append(_moduleTicket.Context.ScriptGenerator.LineBreak);
				_moduleTicket.ScriptContainer.Script.Append(_moduleTicket.Context.ScriptGenerator.LineBreak);
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

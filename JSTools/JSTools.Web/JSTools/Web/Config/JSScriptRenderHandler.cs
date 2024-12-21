/*
 * JSTools.Web.dll / JSTools.net - A framework for JavaScript/ASP.NET applications.
 * Copyright (C) 2005  Silvan Gehrig
 *
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
 *
 * Author:
 *  Silvan Gehrig
 */

using System;
using System.Web.UI;
using System.Xml;

using JSTools.Config;
using JSTools.Config.ScriptFileManagement;
using JSTools.Web.Config;
using JSTools.Web.UI.Controls;

namespace JSTools.Web.Config
{
	/// <summary>
	/// Represents a render handler instance for the JSExceptionHandler section. Renders
	/// the default script module request directives (e.g. &lt;script src="..." /&gt;).
	/// </summary>
	public class JSScriptRenderHandler : IJSToolsRenderHandler
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const string DEBUG_WARNING = " CAUTION: DEBUG MODE IS ACTIVE ";
		private const string MODULE_COMMENT = " Module {0} ";

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
		/// Creates a new JSScriptRenderHandler instance.
		/// </summary>
		public JSScriptRenderHandler()
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
			JSScriptFileHandler section = (sectionToRender as JSScriptFileHandler);
			WebRenderProcessTicket webTicket = (ticket as WebRenderProcessTicket);
			string appPath = webTicket.RenderHandler.Page.Request.ApplicationPath;

			if (webTicket == null)
				throw new ArgumentException("The given ticket is not derived from WebRenderProcessTicket!", "webTicket");

			if (section.DebugMode != DebugMode.None)
			{
				webTicket.RenderHandler.Controls.Add(new LiteralControl("\n\n"));

				// create comment node
				Comment moduleComment = new Comment();
				moduleComment.Text = DEBUG_WARNING;
				webTicket.RenderHandler.Controls.Add(moduleComment);

				// insert line break after comment
				webTicket.RenderHandler.Controls.Add(new LiteralControl("\n"));
			}
			RenderRequiredModulesRecursive(section, webTicket.RenderHandler.Controls, appPath, section.ChildModules);
		}

		/// <summary>
		/// Searches for modules with a default flag in a recursive loop.
		/// </summary>
		/// <param name="section">Section to render.</param>
		/// <param name="controls">Collection, to which the controls will be added.</param>
		/// <param name="appPath">Application path.</param>
		/// <param name="moduleContainer">Module container, which contains the modules.</param>
		private void RenderRequiredModulesRecursive(JSScriptFileHandler section, ControlCollection controls, string appPath, JSModuleContainer moduleContainer)
		{
			if (moduleContainer == null || moduleContainer.Count == 0)
				return;

			foreach (JSModule module in moduleContainer)
			{
				if (module.IsDefaultModule)
				{
					RenderModule(section, controls, appPath, module);
				}
				RenderRequiredModulesRecursive(section, controls, appPath, module.ChildModules);
			}
		}

		/// <summary>
		/// Renders a module, which has a default flag.
		/// </summary>
		/// <param name="section">Section to render.</param>
		/// <param name="controls">Collection, to which the controls will be added.</param>
		/// <param name="appPath">Application path.</param>
		/// <param name="toRender">Module, which should be rendered.</param>
		private void RenderModule(JSScriptFileHandler section, ControlCollection controls, string appPath, JSModule toRender)
		{
			if (section.DebugMode != DebugMode.None)
			{
				controls.Add(new LiteralControl("\n"));

				// create comment node
				Comment moduleComment = new Comment();
				moduleComment.Text = string.Format(MODULE_COMMENT, toRender.FullName);
				controls.Add(moduleComment);

				// insert line break after comment
				controls.Add(new LiteralControl("\n"));
			}

			string modulePath = section.CombinePathPrefix(appPath, toRender.RequestPath);

			// create script
			Script moduleScript = new Script();
			moduleScript.SourceFile = modulePath;
			controls.Add(moduleScript);

			// create white space literal
			controls.Add(new LiteralControl("\n"));
		}
	}
}

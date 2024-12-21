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
using System.Web.UI;
using System.Xml;

using JSTools.Config;
using JSTools.Config.ScriptFileManagement;
using JSTools.Web.UI.Controls;

namespace JSTools.Web.UI
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

		private	const string DEBUG_WARNING = " CAUTION: DEBUG MODE IS ACTIVE ";
		private const string MODULE_COMMENT = " Module {0} ";

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
		/// Creates a new JSScriptRenderHandler instance.
		/// </summary>
		public JSScriptRenderHandler()
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
		/// <exception cref="ArgumentException">The given section is not a valid JSScriptFileHandler instance.</exception>
		/// <exception cref="ArgumentException">The given ticket does not contain a ControlCollection item.</exception>
		/// <exception cref="ArgumentException">The given ticket does not contain a application path item.</exception>
		public void RenderSection(RenderProcessTicket ticket, AJSToolsSection sectionToRender)
		{
			JSScriptFileHandler section = (sectionToRender as JSScriptFileHandler);
			JSControlCollection controls = (ticket.Items[JSToolsPage.RENDER_HANDLER_CTRL_ATTRIBUTE] as JSControlCollection);
			string appPath = (ticket.Items[JSToolsPage.RENDER_HANDLER_APP_ATTRIBUTE] as string);

			if (section == null)
				throw new ArgumentException("The given section is not a valid JSScriptFileHandler instance!", "sectionToRender");

			if (controls == null)
				throw new ArgumentException("The given ticket does not contain a JSControlCollection item!", "ticket");

			if (appPath == null)
				throw new ArgumentException("The given ticket does not contain a application path item!", "ticket");

			if (section.DebugMode != DebugMode.None)
			{
				controls.Add(new LiteralControl("\n\n"));

				// create comment node
				Comment moduleComment = new Comment();
				moduleComment.Text = DEBUG_WARNING;
				controls.Add(moduleComment);

				// insert line break after comment
				controls.Add(new LiteralControl("\n"));
			}
			RenderRequiredModulesRecursive(section, controls, appPath, section.ChildModules);
		}

		/// <summary>
		/// Searches for modules with a default flag in a recursive loop.
		/// </summary>
		/// <param name="section">Section to render.</param>
		/// <param name="controls">Collection, to which the controls will be added.</param>
		/// <param name="appPath">Application path.</param>
		/// <param name="moduleContainer">Module container, which contains the modules.</param>
		private void RenderRequiredModulesRecursive(JSScriptFileHandler section, JSControlCollection controls, string appPath, JSModuleContainer moduleContainer)
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
		private void RenderModule(JSScriptFileHandler section, JSControlCollection controls, string appPath, JSModule toRender)
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

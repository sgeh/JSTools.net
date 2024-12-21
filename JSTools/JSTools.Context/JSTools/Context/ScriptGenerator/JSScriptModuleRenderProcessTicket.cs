/*
 * JSTools.Context.dll / JSTools.net - A framework for JavaScript/ASP.NET applications.
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
using System.Text;
using System.Web;

using JSTools.Config;
using JSTools.Config.ScriptFileManagement;

namespace JSTools.Context.ScriptGenerator
{
	/// <summary>
	/// Contains additional web informations, which are requried to render
	/// configuration sections for the web environment.
	/// </summary>
	internal class JSScriptModuleRenderProcessTicket : RenderProcessTicket
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private JSModule _sectionToRender = null;
		private JSModuleScriptContainer _scriptContainer = null;
		private AJSToolsContext _context = null;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Module instance which should be rendered.
		/// </summary>
		internal JSModule SectionToRender
		{
			get { return _sectionToRender; }
		}

		/// <summary>
		/// Render context of the content to render.
		/// </summary>
		internal JSModuleScriptContainer ScriptContainer
		{
			get
			{
				if (_scriptContainer == null)
					_scriptContainer = new JSModuleScriptContainer();

				return _scriptContainer;
			}
		}

		internal AJSToolsContext Context
		{
			get { return _context; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSScriptModuleRenderProcessTicket instance.
		/// </summary>
		internal JSScriptModuleRenderProcessTicket(JSModule sectionToRender, AJSToolsContext context)
		{
			if (sectionToRender == null)
				throw new ArgumentNullException("sectionToRender");

			if (context == null)
				throw new ArgumentNullException("context");

			_sectionToRender = sectionToRender;
			_context = context;
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------
	}
}

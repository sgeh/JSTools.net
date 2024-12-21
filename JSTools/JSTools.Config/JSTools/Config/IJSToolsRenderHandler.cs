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

using JSTools.Config.ExceptionHandling;
using JSTools.Config.ScriptFileManagement;

namespace JSTools.Config
{
	/// <summary>
	/// This interface is used to render a JSTools section. To enable A IJSToolsRenderHandler,
	/// you have to add it to a RenderProcessTicket object.
	/// </summary>
	public interface IJSToolsRenderHandler
	{
		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Name of the section to render with this render handler.
		/// </summary>
		string SectionName
		{
			get;
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
		void RenderSection(RenderProcessTicket ticket, AJSToolsSection sectionToRender);
	}
}

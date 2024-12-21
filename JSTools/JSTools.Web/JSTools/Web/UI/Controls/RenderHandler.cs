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
using System.Collections;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;

using JSTools.Config;
using JSTools.Web.Config;

namespace JSTools.Web.UI.Controls
{
	/// <summary>
	/// Represents a control which is used to render the contents of
	/// a IJSToolsRenderHandler instance. The child controls will be
	/// filled into the Controls collection an instance of this class.
	/// 
	/// <para>
	/// As default, this collection accommodates three controls:
	/// 
	/// <list type="bullet">
	///  <item>
	///   <description>
	///   A LiteralControl, which contains a carriage return.
	///   </description>
	///  </item>
	///  <item>
	///   <description>
	///   A Comment control, which contains the name of the section
	///   to render.
	///   </description>
	///  </item>
	///  <item>
	///   <description>
	///   A LiteralControl, which contains a carriage return.
	///   </description>
	///  </item>
	/// </list>
	/// </para>
	/// </summary>
	public class RenderHandler : JSToolsControl
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const string SCRIPT_SECTION = " Section {0} ";
		private IJSToolsRenderHandler _renderHandler = null;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new RenderHandler instance.
		/// </summary>
		/// <param name="renderHandler">Render handler instance which is used to render the configuration section.</param>
		/// <exception cref="ArgumentNullException">The given render handler array contains a null reference.</exception>
		internal RenderHandler(IJSToolsRenderHandler renderHandler)
		{
			if (renderHandler == null)
				throw new ArgumentNullException("renderHandler", "The given render handler array contains a null reference.");

			_renderHandler = renderHandler;
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		/// <summary>
		///  <see cref="Page" />
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			// add section name control
			Controls.Add(new LiteralControl("\n"));
			Controls.Add(new Comment(string.Format(SCRIPT_SECTION, _renderHandler.SectionName)));
			Controls.Add(new LiteralControl("\n"));

			// create ticket to render controls
			WebRenderProcessTicket ticket = new WebRenderProcessTicket(this);
			ticket.AddRenderHandler(_renderHandler);
			JSToolsContext.Configuration.Render(ticket);
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------
	}
}

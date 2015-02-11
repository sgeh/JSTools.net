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
using System.Text;
using System.Web;

using JSTools.Config;
using JSTools.Web.UI.Controls;

namespace JSTools.Web.Config
{
	/// <summary>
	/// Contains additional web informations, which are requried to render
	/// configuration sections for the web environment.
	/// </summary>
	internal class WebRenderProcessTicket : RenderProcessTicket
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private RenderHandler _renderHandler;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Returns the associated RenderHandler instance. The controls
		/// collection of this RenderHandler can be used to render content.
		/// </summary>
		public RenderHandler RenderHandler
		{
			get { return _renderHandler; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new WebRenderProcessTicket instance.
		/// </summary>
		internal WebRenderProcessTicket(RenderHandler renderHandler)
		{
			if (renderHandler == null)
				throw new ArgumentNullException("renderHandler");

			_renderHandler = renderHandler;
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------
	}
}

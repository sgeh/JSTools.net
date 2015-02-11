/*
 * JSTools.Config.dll / JSTools.net - A framework for JavaScript/ASP.NET applications.
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
using System.Text;

namespace JSTools.Config
{
	/// <summary>
	/// Contains informations, which are requried to render configuration sections.
	/// </summary>
	public class RenderProcessTicket : IEnumerable
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private ArrayList _renderHandlers = new ArrayList();

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets all registered render handler instances.
		/// </summary>
		/// <returns>Returns the registered render handler instances.</returns>
		private IJSToolsRenderHandler[] RenderHandlers
		{
			get { return (IJSToolsRenderHandler[])_renderHandlers.ToArray(typeof(IJSToolsRenderHandler)); }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new RenderProcessTicket instance.
		/// </summary>
		public RenderProcessTicket()
		{
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Adds a new render handler instance to the ticket. It is used to render the sections.
		/// The sections will be rendered in the order as you have added them.
		/// </summary>
		/// <param name="renderHandler">Render handler to add.</param>
		/// <exception cref="ArgumentNullException">The given render handler contains a null reference.</exception>
		/// <exception cref="ArgumentException">The given render handler contains an invalid section name.</exception>
		/// <exception cref="InvalidOperationException">A render handler for the given section was already sepcified.</exception>
		public void AddRenderHandler(IJSToolsRenderHandler renderHandler)
		{
			if (renderHandler == null)
				throw new ArgumentNullException("renderHandler", "The given render handler contains a null reference.");

			if (renderHandler.SectionName == null || renderHandler.SectionName.Length == 0)
				throw new ArgumentException("The given render handler contains an invalid section name.", "renderHandler");

			if (_renderHandlers.Contains(renderHandler))
				throw new InvalidOperationException("A render handler for the given section was already sepcified.");

			_renderHandlers.Add(renderHandler);
		}

		#region IEnumerable Member

		/// <summary>
		///  <see cref="IEnumerator"/>
		/// </summary>
		/// <returns>
		///  <see cref="IEnumerator"/>
		/// </returns>
		public IEnumerator GetEnumerator()
		{
			return new RenderProcessTicketEnumerator(RenderHandlers);
		}

		#endregion
	}
}

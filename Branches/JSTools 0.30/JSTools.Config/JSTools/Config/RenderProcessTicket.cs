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
using System.Collections;
using System.Text;

namespace JSTools.Config
{
	/// <summary>
	/// Contains informations, which are requried to render configuration sections.
	/// </summary>
	public class RenderProcessTicket
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private	ArrayList			_renderHandlers			= new ArrayList();
		private	Hashtable			_items					= new Hashtable();


		/// <summary>
		/// Gets a key-value collection that can be used to organize and share data between your
		/// code and an IJSToolsRenderHandler during rendering the section scripts.
		/// </summary>
		public IDictionary Items
		{
			get { return _items; }
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
				throw new ArgumentNullException("renderHandler", "The given render handler contains a null reference!");

			if (renderHandler.SectionName == string.Empty || renderHandler.SectionName == null)
				throw new ArgumentException("The given render handler contains an invalid section name!", "renderHandler");

			if (_renderHandlers.Contains(renderHandler))
				throw new InvalidOperationException("A render handler for the given section was already sepcified!");

			_renderHandlers.Add(renderHandler);
		}


		/// <summary>
		/// Gets all registered render handler instances.
		/// </summary>
		/// <returns>Returns the registered render handler instances.</returns>
		public IJSToolsRenderHandler[] GetRenderHandlers()
		{
			return (IJSToolsRenderHandler[])_renderHandlers.ToArray(typeof(IJSToolsRenderHandler));
		}
	}
}

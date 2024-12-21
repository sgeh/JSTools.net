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
using System.Xml;

namespace JSTools.Config
{
	/// <summary>
	/// Each JSTools.net section handler must be derived form this class.
	/// </summary>
	public abstract class AJSToolsSection
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private IJSToolsConfiguration _owner = null;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Returns the configuration instance which has created this FileHandler.
		/// </summary>
		public IJSToolsConfiguration OwnerConfiguration
		{
			get { return _owner; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new AJSToolsSection instance.
		/// </summary>
		/// <param name="owner">The parent event handler.</param>
		/// <exception cref="ArgumentNullException">The specified handler contains a null reference.</exception>
		public AJSToolsSection(IJSToolsConfiguration owner)
		{
			if (owner == null)
				throw new ArgumentNullException("owner", "The specified owner configuration contains a null reference.");

			_owner = owner;
		}

		/// <summary>
		/// Creates a new top level AJSToolsSection instance.
		/// </summary>
		/// <exception cref="InvalidOperationException">This instance is not derived from IJSToolsConfiguration.</exception>
		public AJSToolsSection()
		{
			if ((_owner = (this as IJSToolsConfiguration)) == null)
				throw new InvalidOperationException("This instance is not derived from IJSToolsConfiguration.");
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Checks the relations between the configuration sections. The script section is initilialized
		/// before calling this method.
		/// </summary>
		public virtual void CheckRelations()
		{
		}
	}
}

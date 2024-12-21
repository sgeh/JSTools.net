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
using System.ComponentModel;
using System.Collections;
using System.Web.UI;
using System.Web.UI.HtmlControls;

using JSTools.Context;
using JSTools.Web;

namespace JSTools.Web.UI.Controls
{
	/// <summary>
	///  <see cref="ControlCollection"/>
	///  AddAt() method is disabled.
	/// </summary>
	public class JSToolsControlCollection : ControlCollection
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSToolsControlCollection instance.
		/// </summary>
		/// <param name="owner">Owner of this control collection.</param>
		public JSToolsControlCollection(JSToolsControl owner) : base(owner)
		{
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Use of this method is disabled.
		/// </summary>
		/// <param name="index">
		///  <see cref="ControlCollection" />
		/// </param>
		/// <param name="child">
		///  <see cref="ControlCollection" />
		/// </param>
		/// <exception cref="InvalidOperationException">Could not add the given control at a specified index, use Add instead.</exception>
		public override void AddAt(int index, Control child)
		{
			throw new InvalidOperationException("Could not add the given control at a specified index, use Add instead.");
		}
	}
}

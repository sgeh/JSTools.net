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
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using JSTools.Web.UI;

namespace JSTools.Web.UI.Controls
{
	/// <summary>
	/// Represents a control builder which is used to parse a script control.
	/// </summary>
	public class ScriptControlBuilder : ControlBuilder
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
		/// Creates a new ScriptControlBuilder instance.
		/// </summary>
		public ScriptControlBuilder()
		{
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Do not allow white space literals.
		/// </summary>
		/// <returns></returns>
		public override bool AllowWhitespaceLiterals()
		{
			return false;
		}

		/// <summary>
		/// Do not decode literals.
		/// </summary>
		/// <returns></returns>
		public override bool HtmlDecodeLiterals()
		{
			return false;
		}

		/// <summary>
		/// The control does not need inner text.
		/// </summary>
		/// <returns></returns>
		public override bool NeedsTagInnerText()
		{
			return false;
		}
	}
}

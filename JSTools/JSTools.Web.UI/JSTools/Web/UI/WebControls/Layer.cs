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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Diagnostics;
using System.Web.UI;


namespace JSTools.Controls
{
	/// <summary>
	/// Summary description for "Class".
	/// </summary>
	[DefaultProperty("ClientID")]
	public class Layer : System.Web.UI.WebControls.WebControl
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private int		_topMargin	= 0;
		private	int		_leftMargin	= 0;


		/// <summary>
		/// Sets or gets the margin to the top border of the window
		/// </summary>
		[property: Category("Layout"), Description("Margin to the top border of the window.")]
		public int TopMargin 
		{
			get { return _topMargin; }
			set { _topMargin = value; }
		}


		/// <summary>
		/// Sets or gets the margin to the left border of the window
		/// </summary>
		[property: Category("Layout"), Description("Margin to the left border of the window.")]
		public int LeftMargin 
		{
			get { return _leftMargin; }
			set { _leftMargin = value; }
		}


		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new instance of the Layer class
		/// </summary>
		public Layer()
		{
		}


		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------
	}
}

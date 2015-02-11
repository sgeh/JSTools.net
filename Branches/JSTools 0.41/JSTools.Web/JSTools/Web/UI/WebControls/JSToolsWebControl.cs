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
using System.Collections;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using JSTools.Context;
using JSTools.Web.UI.Controls;

namespace JSTools.Web.UI.WebControls
{
	/// <summary>
	/// Base control implementation for WebControls, which will use the JSTools framework.
	/// </summary>
	public class JSToolsWebControl : WebControl
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private JSToolsPage _jsToolsPage = null;

		/// <summary>
		/// Returns the parent JSToolsPage instance.
		/// </summary>
		[property: Browsable(false)]
		[property: EditorBrowsable(EditorBrowsableState.Always)]
		[property: Bindable(BindableSupport.No)]
		[property: DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new JSToolsPage Page
		{
			get { return _jsToolsPage; }
		}

		/// <summary>
		/// Returns the current JSToolsWebContext, which contains a parser, a cache
		/// and a configuration instance.
		/// </summary>
		/// <remarks>The attributes are neccessary for the designer. Otherwise, it crashes.</remarks>
		[property: Browsable(false)]
		[property: DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual JSToolsWebContext JSToolsContext
		{
			get { return Page.JSToolsContext; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSToolsControl instance.
		/// </summary>
		public JSToolsWebControl()
		{
			Init += new EventHandler(OnInit);
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		private void OnInit(object sender, EventArgs args)
		{
			if ((_jsToolsPage = (base.Page as JSToolsPage)) == null)
				throw new InvalidOperationException("Could not initialize the parent page instance. The parent page must be derived from JSToolsPage!");
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------
	}
}

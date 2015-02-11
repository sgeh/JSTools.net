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
using System.ComponentModel;
using System.Collections;
using System.Web.UI;
using System.Web.UI.HtmlControls;

using JSTools.Config;
using JSTools.Web;

namespace JSTools.Web.UI.Controls
{
	/// <summary>
	/// Base control implementation for default controls, which will use the JSTools framework.
	/// </summary>
	public class JSToolsControl : Control
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private JSToolsPage		_jsToolsPage = null;


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


		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSToolsControl instance.
		/// </summary>
		public JSToolsControl()
		{
			Init += new EventHandler(OnInit);
		}


		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		private void OnInit(object sender, EventArgs args)
		{
			if ((_jsToolsPage = (base.Page as JSToolsPage)) == null)
			{
				throw new InvalidOperationException("Could not initialize the parent page instance. The parent page must be derived from JSToolsPage!");
			}
		}
	}
}

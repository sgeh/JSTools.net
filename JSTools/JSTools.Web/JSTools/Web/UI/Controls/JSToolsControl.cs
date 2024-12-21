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
	/// Base control implementation for default controls, which will use the JSTools framework.
	/// </summary>
	public class JSToolsControl : Control
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private JSToolsPage _jsToolsPage = null;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

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

		/// <summary>
		/// If you specify code blocks inside the value of a control, the
		/// .NET framework does not provide an implementation to add child
		/// controls. To add child controls even you have specified code
		/// blocks inside the control value, you should override this method
		/// and return true.
		/// 
		/// <para>
		/// At least, the CreateChildControls() method is called before
		/// the OnInit event. If the EnableEarlyChildrenCreation is set to
		/// true, use of AddAt() method of the child control collection is
		/// disalbed.
		/// </para>
		/// </summary>
		protected virtual bool EnableEarlyChildrenCreation
		{
			get { return false; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSToolsControl instance.
		/// </summary>
		public JSToolsControl()
		{
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		/// <summary>
		///  <see cref="Control" />
		/// </summary>
		/// <param name="e">
		///  <see cref="Control" />
		/// </param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			if ((_jsToolsPage = (base.Page as JSToolsPage)) == null)
				throw new InvalidOperationException("Could not initialize the parent page instance. The parent page must be derived from JSToolsPage.");
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		///  <see cref="Control" />
		/// </summary>
		/// <param name="renderMethod">
		///  <see cref="Control" />
		/// </param>
		public new void SetRenderMethodDelegate(RenderMethod renderMethod)
		{
			// This method is called by the aspx-code file after setting the
			// property values and initializing the databound literals.
			// It's the only way to add (NOT INSERT) child controls, because
			// the SetRenderMethodDelegate method of the control class will
			// mark the control collection as read only.

			// call create child controls event, if this functionality is
			// enabled
			if (EnableEarlyChildrenCreation)
				CreateChildControls();

			base.SetRenderMethodDelegate(renderMethod);
		}

		/// <summary>
		///  <see cref="Control" />
		/// </summary>
		/// <returns>
		///  <see cref="Control" />
		/// </returns>
		protected override ControlCollection CreateControlCollection()
		{
			if (EnableEarlyChildrenCreation)
				return new JSToolsControlCollection(this);
			else
				return base.CreateControlCollection();
		}
	}
}

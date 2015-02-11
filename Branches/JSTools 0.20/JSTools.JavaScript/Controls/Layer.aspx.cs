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
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using JSTools.Config;
using JSTools.Config.ExceptionHandling;
using JSTools.Config.Session;

namespace JSTools_Web.Controls
{
	/// <summary>
	/// Summary description for Layer.
	/// </summary>
	public class Layer : System.Web.UI.Page
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			ErrorHandling proxyValue = (Context.GetConfig("JSTools.net/settings") as JSToolsConfigurationProxy).Configuration.ErrorHandling.Handling;
			Response.Write("Immutable (PROXY): " + proxyValue + "<br>");
			Response.Write("Copy-on-write: Set ErrorHandling to LogError<br>");
			AJSToolsSessionHandler.Instance.ErrorHandling.Handling = ErrorHandling.LogError;
			Response.Write("Session: " + AJSToolsSessionHandler.Instance.ErrorHandling.Handling + "<br>");
			Response.Write("Immutable: " + AJSToolsSessionHandler.Instance.ImmutableInstance.ErrorHandling.Handling + "<br>");

			if (proxyValue == AJSToolsSessionHandler.Instance.ErrorHandling.Handling)
			{
				Response.Write("The Session is equal to the default instance.<br>");
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}

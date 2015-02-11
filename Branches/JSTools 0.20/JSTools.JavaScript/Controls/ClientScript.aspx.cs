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
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using JSTools.Config;
using JSTools.Config.ExceptionHandling;
using JSTools.Config.ScriptFileManagement;
using JSTools.Config.Session;

namespace JSTools_Web
{
	/// <summary>
	/// Summary description for WebForm1.
	/// </summary>
	public class ClientScriptTestPage : System.Web.UI.Page
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
/*
			IJSToolsConfiguration configuration = AJSToolsSessionHandler.CreateEnvInstance();
			configuration.LoadXml(@"E:\pages\webs\JSTools_Web\JSTools.config");
*/
			AJSExceptionHandler proxyValue = (Context.GetConfig("JSTools.net/settings") as JSToolsConfigurationProxy).Configuration.ErrorHandling;
			Response.Write("Immutable (PROXY): " + proxyValue.Handling + " - " + proxyValue + "<br>");
			Response.Write("Copy-on-write: Set ErrorHandling to LogError<br>");
			AJSToolsSessionHandler.Instance.ErrorHandling.Handling = ErrorHandling.LogError;
			Response.Write("Session: " + AJSToolsSessionHandler.Instance.ErrorHandling.Handling + "<br>");
			Response.Write("Immutable: " + AJSToolsSessionHandler.Instance.ImmutableInstance.ErrorHandling.Handling + "<br>");

			if (proxyValue == AJSToolsSessionHandler.Instance.ErrorHandling)
			{
				Response.Write("The Session is equal to the default instance.<br>");
			}

			AJSToolsSessionHandler.Instance.ScriptFileHandler.ScriptVersion = 1.4;
			AJSToolsSessionHandler.Instance.ScriptFileHandler.GetModule("JSTools.Web").ChildModules.MoveModule("Browser", true);

			/* for testing only
						AJSScript myScript = AJSToolsSessionHandler.Instance.ScriptFileHandler.GetScript("JSTools/Web/Diagnostics/ConsoleErrorMessage.js");
						Trace.Warn(myScript.ToString());
			*/
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			RenderProcessTicket processInfo = new RenderProcessTicket();
			processInfo.AddRequiredModule("JSTools.Web.Layer.IE");

			AJSToolsSessionHandler.Instance.Render(processInfo);

			AJSToolsSessionHandler.Instance.SaveConfiguration("C:\\Temp\\save_configuration.xml");

			Page.RegisterStartupScript("any_script_key", processInfo.RenderContext);
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

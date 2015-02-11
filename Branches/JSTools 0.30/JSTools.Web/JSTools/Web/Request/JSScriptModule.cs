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
using System.IO;
using System.Text;
using System.Threading;
using System.Web;

using JSTools.Config;
using JSTools.Config.ScriptFileManagement;
using JSTools.Web.Config;

namespace JSTools.Web.Request
{
	/// <summary>
	/// Handles requests of files with .js extensions. The script files must be declared in the
	/// JSTools config section.
	/// </summary>
	public class JSScriptModule : IHttpModule
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		/// <summary>
		/// Application path key. (string)
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public const string RENDER_HANDLER_APPLICATION_KEY = "JSScriptModule_Application_Attribute";


		/// <summary>
		/// Render context of the file. (StringBuilder)
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public const string RENDER_HANDLER_CONTEXT_KEY = "JSScriptModule_Context_Attribute";


		/// <summary>
		/// Expiration date of the file. (DateTime)
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public const string RENDER_HANDLER_DATE_EXPIRE = "JSScriptModule_Date_Expired";


		/// <summary>
		/// Last update of the file. (DateTime)
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public const string RENDER_HANDLER_LAST_UPDATED = "JSScriptModule_Date_Modified";

		private const			string		CONTENT_LENGTH_HEADER	= "Content-Length";


		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSScriptModule instance.
		/// </summary>
		public JSScriptModule()
		{
		}


		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// <see cref="System.Web.IHttpModule.Init"/>
		/// </summary>
		public void Init(HttpApplication context)
		{
			context.BeginRequest += new EventHandler(OnBeginRequest);
		}


		/// <summary>
		/// <see cref="System.Web.IHttpModule.Dispose"/>
		/// </summary>
		public void Dispose()
		{
		}


		/// <summary>
		/// Handles a request begin.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void OnBeginRequest(object sender, EventArgs args)
		{
			try
			{
				HttpApplication application = (sender as HttpApplication);
				JSScriptFileHandler handler = JSToolsWebConfiguration.Instance.Configuration.ScriptFileHandler;

				if (Path.GetExtension(application.Request.Path) == handler.ScriptExtension)
				{
					AJSToolsScriptFileSection section = handler.GetSection(GetSectionPathFromUri(application.Request));

					if (section != null)
					{
						ProcessRequestedConfigScript(application.Context, section);
					}
				}
			}
			catch (ThreadAbortException)
			{
				// we have to expect a thread abort exception, because the request
				// was terminated prematurely
				throw;
			}
			catch (Exception e)
			{
				throw new HttpException(500, "Internal Server Error! Error description: " + e.Message, e);
			}
		}


		/// <summary>
		/// Writes the requested config file into the response stream.
		/// </summary>
		/// <param name="context">Encapsulates all HTTP-specific information about an individual HTTP request.</param>
		private void ProcessRequestedConfigScript(HttpContext context, AJSToolsScriptFileSection requestedSection)
		{
			// create render ticket
			RenderProcessTicket ticket = new RenderProcessTicket();

			// add required items to render the configuration
			ticket.Items[RENDER_HANDLER_APPLICATION_KEY] = context.Request.ApplicationPath;
			ticket.Items[RENDER_HANDLER_CONTEXT_KEY] = new StringBuilder();
			ticket.Items[RENDER_HANDLER_DATE_EXPIRE] = null;
			ticket.Items[RENDER_HANDLER_LAST_UPDATED] = null;

			// create render handler for rendering the configuration
			ticket.AddRenderHandler(new JSScriptModuleRenderHandler(requestedSection));

			// render configuration
			JSToolsWebConfiguration.Instance.Configuration.Render(ticket);

			// send created script to the client, this procedure may fire a ThreadAbortException
			SendResponse(context, ticket);
		}


		/// <summary>
		/// Initializes the response and sends the given string to the client browser.
		/// </summary>
		/// <param name="context">Context into which should be written.</param>
		/// <param name="toWrite">Ticket (script) to write.</param>
		private void SendResponse(HttpContext context, RenderProcessTicket toWrite)
		{
			// clear response before writing
			context.Response.Clear();

			// buffer script output
			context.Response.BufferOutput = true;

			// init content type
			context.Response.ContentType = JSToolsWebConfiguration.Instance.Configuration.ScriptFileHandler.ContentType;

			InitCacheHeaders(context.Response, toWrite);

			// write rendered configuration into the output stream
			if ((toWrite.Items[RENDER_HANDLER_CONTEXT_KEY] as StringBuilder) != null)
			{
				string contentToWrite = (toWrite.Items[RENDER_HANDLER_CONTEXT_KEY] as StringBuilder).ToString();
				byte[] bytes = context.Response.ContentEncoding.GetBytes(contentToWrite);

				context.Response.AppendHeader(CONTENT_LENGTH_HEADER, bytes.Length.ToString());
				context.Response.BinaryWrite(bytes);
			}

			// flush and quit response
			context.Response.Flush();
			context.Response.End();
		}


		/// <summary>
		/// Initializes the headers, if they are provided by the JSScriptModuleRenderHandler
		/// instance.
		/// </summary>
		/// <param name="toInit">Response to initialize.</param>
		/// <param name="toWrite">Ticket (script) to write.</param>
		private void InitCacheHeaders(HttpResponse toInit, RenderProcessTicket toWrite)
		{
			if (toWrite.Items[RENDER_HANDLER_LAST_UPDATED] != null
				&& toWrite.Items[RENDER_HANDLER_LAST_UPDATED].GetType() == typeof(DateTime))
			{
				toInit.Cache.SetLastModified((DateTime)toWrite.Items[RENDER_HANDLER_LAST_UPDATED]);
			}

			if (toWrite.Items[RENDER_HANDLER_DATE_EXPIRE] != null
				&& toWrite.Items[RENDER_HANDLER_DATE_EXPIRE].GetType() == typeof(DateTime))
			{
				toInit.Cache.SetExpires((DateTime)toWrite.Items[RENDER_HANDLER_DATE_EXPIRE]);
			}

			// enable client side cache
			toInit.Cache.SetCacheability(HttpCacheability.Public);
		}


		/// <summary>
		/// Gets the section path from the given uri.
		/// </summary>
		/// <param name="toGetSection">Uri, which contains the section path.</param>
		/// <returns>Returns a valid JSTools config section path.</returns>
		private string GetSectionPathFromUri(HttpRequest toGetSection)
		{
			if (toGetSection.Url.AbsolutePath == null || toGetSection.Url.AbsolutePath == string.Empty)
				return string.Empty;

			string sectionPath = GetValidSectionStart(toGetSection.ApplicationPath, toGetSection.Url.AbsolutePath);
			return GetValidSectionEnd(sectionPath);
		}


		/// <summary>
		/// Gets a valid section path start string.
		/// </summary>
		/// <param name="toCheck">Section path to check.</param>
		/// <returns>Returns the validated string.</returns>
		private string GetValidSectionStart(string trimStart, string toCheck)
		{
			string trim = (trimStart.EndsWith("/") ? trimStart : trimStart + "/");

			if (toCheck.StartsWith(trim))
			{
				return toCheck.Substring(trim.Length);
			}
			return toCheck;
		}


		/// <summary>
		/// Gets a valid section path end string.
		/// </summary>
		/// <param name="toCheck">Section path to check.</param>
		/// <returns>Returns the validated string.</returns>
		private string GetValidSectionEnd(string toCheck)
		{
			int indexOfDot = toCheck.LastIndexOf('.');

			if (indexOfDot != -1)
			{
				return toCheck.Substring(0, indexOfDot);
			}
			return toCheck;
		}
	}
}

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
using System.IO;
using System.Text;
using System.Threading;
using System.Web;

using JSTools.Config;
using JSTools.Config.ScriptFileManagement;
using JSTools.Context;
using JSTools.Context.Cache;

namespace JSTools.Web.Request
{
	/// <summary>
	/// Handles requests of files with .js extensions. The script files must be declared in the
	/// JSTools config section.
	/// </summary>
	public class JSScriptModule : IHttpModule, IDisposable
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const string CONTENT_LENGTH_HEADER = "Content-Length";

		private HttpApplication _application = null;
		private EventHandler _beginRequestHandler = null;
		private bool _isDisposed = false;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSScriptModule instance.
		/// </summary>
		public JSScriptModule()
		{
		}

		/// <summary>
		/// Releases the current instance. Follows the dispose pattern.
		/// </summary>
		~JSScriptModule()
		{
			EnsureDispose(false);
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		/// <summary>
		/// Handles a request begin.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void OnBeginRequest(object sender, EventArgs args)
		{
			JSToolsWebContext context = JSToolsWebContext.Instance;
			HttpApplication currentApp = (HttpApplication)sender;

			try
			{
				IScriptContainer requestedItem = context.GetCachedItemByPath(currentApp.Request.Path);
				
				if (requestedItem != null)
					RespondRequestedItem(currentApp, context, requestedItem);
			}
			catch (ThreadAbortException)
			{
				// we have to expect a thread abort exception, because the request
				// was terminated prematurely
				throw;
			}
			catch (Exception e)
			{
				// ooops! an error has occured during getting the requested script container
				RenderData(
					currentApp,
					context.Configuration.ScriptFileHandler.ContentType,
					context.ScriptGenerator.CreateExceptionAlert(e) );
			}
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		#region Dispose Pattern

		/// <summary>
		///  <see cref="System.Web.IHttpModule.Dispose"/>
		/// </summary>
		public void Dispose()
		{
			EnsureDispose(true);

			// This object will be cleaned up by the Dispose method.
			// Therefore, you should call GC.SupressFinalize to
			// take this object off the finalization queue 
			// and prevent finalization code for this object
			// from executing a second time.
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Dispose resources associated with the current instance.
		/// </summary>
		/// <param name="disposing">True to clean up external managed resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				// clean up external referenced resources
				// -> call _member.Dispose() method

				// detach begin event
				if (_beginRequestHandler != null)
					_application.BeginRequest -= _beginRequestHandler;
			}

			// clean up unmanaged resources
		}

		private void EnsureDispose(bool disposing)
		{
			if (!_isDisposed)
			{
				// call protected dispose method
				Dispose(disposing);

				// mark instance as disposed
				_isDisposed = true;
			}
		}

		#endregion

		/// <summary>
		///  <see cref="System.Web.IHttpModule.Init"/>
		/// </summary>
		public void Init(HttpApplication application)
		{
			_application = application; // store global web-application object in order to detach the event later
			_application.BeginRequest += (_beginRequestHandler = new EventHandler(OnBeginRequest));
		}

		private void RespondRequestedItem(HttpApplication currentApp, JSToolsWebContext context, IScriptContainer toRespond)
		{
			#region Enable client side cache.

			if (toRespond.ExpirationTime != TimeSpan.MinValue)
			{
				// init cache headers
				currentApp.Response.Cache.SetLastModified(toRespond.LastUpdate);
				currentApp.Response.Cache.SetCacheability(HttpCacheability.Public);

				if (toRespond.ExpirationTime != TimeSpan.MaxValue)
					currentApp.Response.Cache.SetExpires(toRespond.LastAccess + toRespond.ExpirationTime);
			}

			#endregion

			#region Write rendered script data into the output stream.

			RenderData(
				currentApp,
				context.Configuration.ScriptFileHandler.ContentType,
				toRespond.GetCachedCode() );

			#endregion
		}

		private void RenderData(HttpApplication currentApp, string contentType, string toRender)
		{
			// clear response before writing
			currentApp.Response.Clear();

			// buffer script output
			currentApp.Response.BufferOutput = true;

			// init content type
			currentApp.Response.ContentType = contentType;

			// render data
			if (toRender != null)
			{
				byte[] bytesToRespond = currentApp.Response.ContentEncoding.GetBytes(toRender);
				currentApp.Response.AppendHeader(CONTENT_LENGTH_HEADER, bytesToRespond.Length.ToString());
				currentApp.Response.BinaryWrite(bytesToRespond);
			}

			// flush and quit response
			currentApp.Response.Flush();
			currentApp.Response.End();
		}
	}
}

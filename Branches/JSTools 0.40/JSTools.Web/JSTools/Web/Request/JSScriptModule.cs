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
	public class JSScriptModule : IHttpModule
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const string CONTENT_LENGTH_HEADER = "Content-Length";

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
			Dispose(true);
			// This object will be cleaned up by the Dispose method.
			// Therefore, you should call GC.SupressFinalize to
			// take this object off the finalization queue 
			// and prevent finalization code for this object
			// from executing a second time.
			GC.SuppressFinalize(this);
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
			try
			{
				IScriptContainer requestedItem = JSToolsWebContext.Instance.GetCachedItemByPath(((HttpApplication)sender).Request.Path);
				
				if (requestedItem != null)
					RespondRequestedItem((HttpApplication)sender, requestedItem);
			}
			catch (ThreadAbortException)
			{
				// we have to expect a thread abort exception, because the request
				// was terminated prematurely
				throw;
			}
			catch (Exception e)
			{
				throw new HttpException(500, "Internal Server Error. Error description: " + e.Message, e);
			}
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		#region Dispose Pattern
		/// <summary>
		/// <see cref="System.Web.IHttpModule.Dispose"/>
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Dispose resources associated with the current instance.
		/// </summary>
		/// <param name="disposing">True to clean up external managed resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (_isDisposed)
			{
				if (!disposing)
				{
					// clean up external referenced resources
					// -> call _member.Dispose() method
				}

				// clean up unmanaged resources
				_isDisposed = true;
			}
		}
		#endregion

		/// <summary>
		/// <see cref="System.Web.IHttpModule.Init"/>
		/// </summary>
		public void Init(HttpApplication context)
		{
			context.BeginRequest += new EventHandler(OnBeginRequest);
		}

		private void RespondRequestedItem(HttpApplication currentApp, IScriptContainer toRespond)
		{
			// clear response before writing
			currentApp.Response.Clear();

			// buffer script output
			currentApp.Response.BufferOutput = true;

			// init content type
			currentApp.Response.ContentType = JSToolsWebContext.Instance.Configuration.ScriptFileHandler.ContentType;

			#region Enable client side cache.

			if (toRespond.ExpirationTime != DateTime.MinValue)
			{
				// init cache headers
				currentApp.Response.Cache.SetLastModified(toRespond.LastUpdate);
				currentApp.Response.Cache.SetCacheability(HttpCacheability.Public);
				currentApp.Response.Cache.SetExpires(toRespond.ExpirationTime);
			}

			#endregion

			#region Write rendered script data into the output stream.

			byte[] bytes = currentApp.Response.ContentEncoding.GetBytes(toRespond.GetCachedCode());

			currentApp.Response.AppendHeader(CONTENT_LENGTH_HEADER, bytes.Length.ToString());
			currentApp.Response.BinaryWrite(bytes);

			#endregion

			// flush and quit response
			currentApp.Response.Flush();
			currentApp.Response.End();
		}
	}
}

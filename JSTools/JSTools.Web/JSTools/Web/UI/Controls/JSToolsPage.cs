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

using JSTools.Config;
using JSTools.Context;
using JSTools.Web.Config;
using JSTools.Web.UI;

namespace JSTools.Web.UI.Controls
{
	/// <summary>
	/// Contains configuration and other JSTools script capabilities. If a page uses some
	/// features of the JSTools framework, you have to derive from this class.
	/// </summary>
	/// <remarks>
	/// Each JSToolsPage must contain a header control.
	/// 
	/// If there are errors when opening the page designer, you have to add all JSTools
	/// library references to your web project.
	/// </remarks>
	public class JSToolsPage : Page
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const string HEADER_SCRIPT_ID = "RENDER_HANDLER_HEADER_SCRIPT";

		private static readonly IJSToolsRenderHandler[] DEFAULT_RENDER_HANDLERS = new IJSToolsRenderHandler[]
			{
				new JSScriptRenderHandler(),
				new JSScriptLoaderRenderHandler(),
				new JSExceptionRenderHandler()
			};

		private Head _header = null;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Returns the current JSToolsWebContext, which contains a parser, a cache
		/// and a configuration instance.
		/// </summary>
		/// <remarks>The attributes are neccessary for the designer. Otherwise, it crashes.</remarks>
		[property: Browsable(false)]
		[property: DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public JSToolsWebContext JSToolsContext
		{
			get { return JSToolsWebContext.Instance; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSToolsPage instance.
		/// </summary>
		public JSToolsPage()
		{
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		/// <summary>
		/// Initializes this page instance and the representing form object.
		/// </summary>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			// init header
			_header = GetHeaderInstance();

			// init default render handlers
			for (int i = 0; i < DEFAULT_RENDER_HANDLERS.Length; ++i)
			{
				AddConfigRenderHandler(DEFAULT_RENDER_HANDLERS[i]);
			}

			if (_header == null)
				throw new ControlNotFoundException("Could not find a header control. A JSToolsPage must contain a head control!");
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		///  <see cref="Head" />
		/// </summary>
		/// <param name="handler">
		///  <see cref="Head" />
		/// </param>
		public void AddConfigRenderHandler(IJSToolsRenderHandler handler)
		{
			if (handler == null)
				throw new ArgumentNullException("handler", "The given handler contains a null reference!");

			_header.AddConfigRenderHandler(handler);
		}

		/// <summary>
		///  <see cref="Head" />
		/// </summary>
		/// <param name="key">
		///  <see cref="Head" />
		/// </param>
		/// <param name="code">
		///  <see cref="Head" />
		/// </param>
		/// <param name="scriptType">
		///  <see cref="Head" />
		/// </param>
		/// <param name="scriptVersion">
		///  <see cref="Head" />
		/// </param>
		public void RegisterHeaderScript(string key, string code, string scriptType, float scriptVersion)
		{
			_header.RegisterHeaderScript(key, code, scriptType, scriptVersion);
		}

		/// <summary>
		///  <see cref="Head" />
		/// </summary>
		/// <param name="key">
		///  <see cref="Head" />
		/// </param>
		/// <param name="path">
		///  <see cref="Head" />
		/// </param>
		public void RegisterHeaderScript(string key, Uri path)
		{
			_header.RegisterHeaderScript(key, path);
		}

		/// <summary>
		///  <see cref="Head" />
		/// </summary>
		/// <param name="key">
		///  <see cref="Head" />
		/// </param>
		/// <param name="path">
		///  <see cref="Head" />
		/// </param>
		/// <param name="scriptType">
		///  <see cref="Head" />
		/// </param>
		/// <param name="scriptVersion">
		///  <see cref="Head" />
		/// </param>
		public void RegisterHeaderScript(string key, Uri path, string scriptType, float scriptVersion)
		{
			_header.RegisterHeaderScript(key, path, scriptType, scriptVersion);
		}

		/// <summary>
		///  <see cref="Head" />
		/// </summary>
		/// <param name="key">
		///  <see cref="Head" />
		/// </param>
		/// <param name="code">
		///  <see cref="Head" />
		/// </param>
		public void RegisterHeaderScript(string key, string code)
		{
			_header.RegisterHeaderScript(key, code);
		}

		/// <summary>
		///  <see cref="Head" />
		/// </summary>
		/// <param name="key">
		///  <see cref="Head" />
		/// </param>
		/// <param name="content">
		///  <see cref="Head" />
		/// </param>
		public void RegisterHeaderContent(string key, string content)
		{
			_header.RegisterHeaderContent(key, content);
		}

		/// <summary>
		///  <see cref="Head" />
		/// </summary>
		/// <param name="key">
		///  <see cref="Head" />
		/// </param>
		/// <returns>
		///  <see cref="Head" />
		/// </returns>
		public bool IsHeaderContentRegistered(string key)
		{
			return _header.HasHeaderContent(key);
		}

		/// <summary>
		/// Searches in the ChildControlCollection for a Header object. This method
		/// my be overridden in derived classes if the Head control is not a direct sub
		/// control of the page control.
		/// </summary>
		/// <returns>Returns the found header object or a null reference.</returns>
		protected virtual Head GetHeaderInstance()
		{
			Head header = null;

			foreach (Control control in Controls)
			{
				if ((header = (control as Head)) != null)
					break;
			}
			return header;
		}
	}
}

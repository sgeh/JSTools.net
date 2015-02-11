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
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;

using JSTools.Config;

namespace JSTools.Web.UI.Controls
{
	/// <summary>
	/// Each JSToolsPage must contain a header section. This header section represents the
	/// &lt;head&gt;...&lt;/head&gt; tag definition and is required for rendering the script files.
	/// </summary>
	public class Head : JSToolsControl
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const string TAG_NAME = "head";
		private const string HEADER_POSTFIX = "_HEAD";

		private PlaceHolder _renderHandlerControls = new PlaceHolder();
		private PlaceHolder _headerControls = new PlaceHolder();

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		///  <see cref="JSToolsControl.EnableEarlyChildrenCreation"/>
		/// </summary>
		protected override bool EnableEarlyChildrenCreation
		{
			get { return true; }
		}

		private ControlCollection RenderHandlerControls
		{
			get { return _renderHandlerControls.Controls; }
		}

		private ControlCollection HeaderControls
		{
			get { return _headerControls.Controls; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new Header instance.
		/// </summary>
		public Head()
		{
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		/// <summary>
		///  <see cref="Page" />
		/// </summary>
		/// <param name="output">
		///  <see cref="Page" />
		/// </param>
		protected override void Render(HtmlTextWriter output)
		{
			output.WriteFullBeginTag(TAG_NAME);
			base.Render(output);
			output.WriteEndTag(TAG_NAME);
		}

		/// <summary>
		///  <see cref="Page" />
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			Controls.Add(_renderHandlerControls);
			Controls.Add(_headerControls);
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Adds a new configuration render handler. The configuration sections are
		/// rendered into the header control.
		/// 
		/// <para>
		/// Render handlers are required for rendering configuration sections, which
		/// are specified in the JSToolsConfiguration (web.config).
		/// </para>
		/// </summary>
		/// <remarks>
		/// The render handlers should be added before the render event occurs.
		/// </remarks>
		/// <param name="handler">Handler to add.</param>
		/// <exception cref="ArgumentNullException">The given handler contains a null reference.</exception>
		public void AddConfigRenderHandler(IJSToolsRenderHandler handler)
		{
			if (handler == null)
				throw new ArgumentNullException("handler", "The given handler contains a null reference.");

			RenderHandlerControls.Add(new RenderHandler(handler));
		}

		/// <summary>
		/// Registers a new header script. The script tags will be rendered
		/// automatically. The script version and language are specified by
		/// the parameters.
		/// </summary>
		/// <param name="key">Key to identify the code.</param>
		/// <param name="code">Script code to register.</param>
		/// <param name="scriptType">Script type (e.g. JavaScript / VBScript)</param>
		/// <param name="scriptVersion">Script version (e.g. 1.2 / 1.5)</param>
		/// <exception cref="ArgumentNullException">The given key contains a null reference.</exception>
		/// <exception cref="ArgumentNullException">The given script contains a null reference.</exception>
		/// <exception cref="ArgumentNullException">The given script type contains a null reference.</exception>
		public void RegisterHeaderScript(string key, string code, string scriptType, float scriptVersion)
		{
			if (key == null)
				throw new ArgumentNullException("key", "The given key contains a null reference.");

			if (code == null)
				throw new ArgumentNullException("code", "The given code type contains a null reference.");

			if (scriptType == null)
				throw new ArgumentNullException("scriptType", "The given script type contains a null reference.");

			Script headerScript = new Script();
			headerScript.ID = key + HEADER_POSTFIX;
			headerScript.Code = code;
			headerScript.Type = scriptType;
			headerScript.Version = scriptVersion;

			HeaderControls.Add(headerScript);
		}

		/// <summary>
		/// Registers a new header script. The script tags will be rendered
		/// automatically. The script version and language are specified in the 
		/// JSToolsConfiguration (web.config).
		/// </summary>
		/// <param name="key">Key to identify the code.</param>
		/// <param name="path">Path of the script to register. This url will be written into the "src" attribute.</param>
		/// <exception cref="ArgumentNullException">The given key contains a null reference.</exception>
		public void RegisterHeaderScript(string key, Uri path)
		{
			if (key == null)
				throw new ArgumentNullException("key", "The given key contains a null reference.");

			Script headerScript = new Script();
			headerScript.ID = key + HEADER_POSTFIX;
			headerScript.SourceFile = path.ToString();

			HeaderControls.Add(headerScript);
		}

		/// <summary>
		/// Registers a new header script. The script tags will be rendered
		/// automatically. The script version and language are specified by
		/// the parameters.
		/// </summary>
		/// <param name="key">Key to identify the code.</param>
		/// <param name="path">Path of the script to register. This url will be written into the "src" attribute.</param>
		/// <param name="scriptType">Script type (e.g. JavaScript / VBScript)</param>
		/// <param name="scriptVersion">Script version (e.g. 1.2 / 1.5)</param>
		/// <exception cref="ArgumentNullException">The given key contains a null reference.</exception>
		/// <exception cref="ArgumentNullException">The given script type contains a null reference.</exception>
		public void RegisterHeaderScript(string key, Uri path, string scriptType, float scriptVersion)
		{
			if (key == null)
				throw new ArgumentNullException("key", "The given key contains a null reference.");

			if (scriptType == null)
				throw new ArgumentNullException("scriptType", "The given script type contains a null reference.");

			Script headerScript = new Script();
			headerScript.ID = key + HEADER_POSTFIX;
			headerScript.Type = scriptType;
			headerScript.SourceFile = path.ToString();
			headerScript.Version = scriptVersion;

			HeaderControls.Add(headerScript);
		}

		/// <summary>
		/// Registers a new header script. The script tags will be rendered
		/// automatically. The script version and language are specified by
		/// the parameters.
		/// </summary>
		/// <param name="key">Key to identify the content to register.</param>
		/// <param name="code">Code to register.</param>
		/// <exception cref="ArgumentNullException">The given key contains a null reference.</exception>
		/// <exception cref="ArgumentNullException">The given code contains a null reference.</exception>
		public void RegisterHeaderScript(string key, string code)
		{
			if (key == null)
				throw new ArgumentNullException("key", "The given key contains a null reference.");

			if (code == null)
				throw new ArgumentNullException("code", "The given code contains a null reference.");

			Script headerScript = new Script();
			headerScript.ID = key + HEADER_POSTFIX;
			headerScript.Code = code;

			HeaderControls.Add(headerScript);
		}

		/// <summary>
		/// Registers a new content section. This method will not create
		/// tags around the given content.
		/// </summary>
		/// <param name="key">Key to identify the content to register.</param>
		/// <param name="content">Code to register.</param>
		/// <exception cref="ArgumentNullException">The given key contains a null reference.</exception>
		/// <exception cref="ArgumentNullException">The given content contains a null reference.</exception>
		public void RegisterHeaderContent(string key, string content)
		{
			if (key == null)
				throw new ArgumentNullException("key", "The given key contains a null reference.");

			if (content == null)
				throw new ArgumentNullException("content", "The given script contains a null reference.");

			LiteralControl headerContent = new LiteralControl();
			headerContent.ID = key + HEADER_POSTFIX;
			headerContent.Text = content;

			HeaderControls.Add(headerContent);
		}

		/// <summary>
		/// Checks whether a key was already registered.
		/// </summary>
		/// <param name="key">Script key to check.</param>
		/// <returns>Returns true if a content with the given key was registered.</returns>
		public bool HasHeaderContent(string key)
		{
			foreach (Control scriptControl in HeaderControls)
			{
				if (scriptControl.ID == key + HEADER_POSTFIX)
					return true;
			}
			return false;
		}
	}
}

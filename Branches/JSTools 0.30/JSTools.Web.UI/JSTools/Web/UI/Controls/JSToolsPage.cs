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
using JSTools.Config.ScriptFileManagement;
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		public	const	string	RENDER_HANDLER_APP_ATTRIBUTE	= "JSToolsPage_ApplictionPath_Attribute";

		[EditorBrowsable(EditorBrowsableState.Never)]
		public	const	string	RENDER_HANDLER_CTRL_ATTRIBUTE	= "JSToolsPage_ControlCollection_Attribute";

		private static readonly IJSToolsRenderHandler[]	_defaultRenderHandlers	= {
																					  new JSScriptRenderHandler(),
																					  new JSScriptLoaderRenderHandler(),
																					  new JSExceptionRenderHandler()
																				  };

		private		ArrayList							_renderHandlers			= new ArrayList();
		private		Head								_header					= null;


		/// <summary>
		/// Returns the JSToolsConfiguration settings.
		/// </summary>
		/// <remarks>The attributes are neccessary for the designer. Otherwise, it crashes.</remarks>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IJSToolsConfiguration Configuration
		{
			get { return JSToolsWebConfiguration.Instance.Configuration; }
		}


		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSToolsPage instance.
		/// </summary>
		public JSToolsPage()
		{
			Init += new System.EventHandler(OnPageInit);
		}


		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Adds a new configuration render handler. The configuration sections are
		/// rendered into the header control.
		/// 
		/// Render handlers are required for rendering configuration sections, which
		/// are specified in the JSToolsConfiguration (web.config).
		/// 
		/// The render handlers should be added before the render event occurs. The
		/// RenderProcessTicket will provide two items. The first is the application
		/// path. It is required to render valid script pathes. The name of this item
		/// is declared by the public JSTools.RENDER_HANDLER_APP_ATTRIBUTE field.
		/// The second property is an instance of the JSControlCollection type. It can
		/// be used to fill in controls, which represents the rendered context. The
		/// name is stored in the pulbic JSTools.RENDER_HANDLER_CTRL_ATTRIBUTE field.
		/// 
		/// All script controls which are added to the JSControlCollection will be
		/// crunched in release mode.
		/// </summary>
		/// <param name="handler">Handler to add.</param>
		public void AddConfigRenderHandler(IJSToolsRenderHandler handler)
		{
			if (handler == null)
				throw new ArgumentNullException("handler", "The given handler contains a null reference!");

			_renderHandlers.Add(handler);
		}


		/// <summary>
		/// Registers a new header script. The script tags will be rendered
		/// automatically. The script version and language are specified in the 
		/// JSToolsConfiguration (web.config).
		/// </summary>
		/// <param name="key">Key to identify the code.</param>
		/// <param name="code">Script code to register.</param>
		public void RegisterHeaderScript(string key, string code)
		{
			_header.AddHeaderScript(key,
				Configuration.ScriptFileHandler.GetScriptBeginTag() +
				code + 
				Configuration.ScriptFileHandler.GetScriptEndTag());
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
		public void RegisterHeaderScript(string key, string code, string scriptType, float scriptVersion)
		{
			_header.AddHeaderScript(key,
				Configuration.ScriptFileHandler.GetScriptBeginTag(scriptType, scriptVersion) +
				code + 
				Configuration.ScriptFileHandler.GetScriptEndTag());
		}


		/// <summary>
		/// Registers a new header script. The script tags will be rendered
		/// automatically. The script version and language are specified in the 
		/// JSToolsConfiguration (web.config).
		/// </summary>
		/// <param name="key">Key to identify the code.</param>
		/// <param name="path">Path to the script to register. This url will be written into the "src" attribute.</param>
		public void RegisterHeaderScript(string key, Uri path)
		{
			_header.AddHeaderScript(key, Configuration.ScriptFileHandler.GetScriptFileTag(path.ToString()));
		}


		/// <summary>
		/// Registers a new header script. The script tags will be rendered
		/// automatically. The script version and language are specified by
		/// the parameters.
		/// </summary>
		/// <param name="key">Key to identify the code.</param>
		/// <param name="path">Path to the script to register. This url will be written into the "src" attribute.</param>
		/// <param name="scriptType">Script type (e.g. JavaScript / VBScript)</param>
		/// <param name="scriptVersion">Script version (e.g. 1.2 / 1.5)</param>
		public void RegisterHeaderScript(string key, Uri path, string scriptType, float scriptVersion)
		{
			_header.AddHeaderScript(key, Configuration.ScriptFileHandler.GetScriptFileTag(path.ToString(), scriptType, scriptVersion));
		}


		/// <summary>
		/// Checks for a script with the given key.
		/// </summary>
		/// <param name="key">Key to check.</param>
		/// <returns>Returns true, if a script with the given key with registered.</returns>
		public bool IsHeaderScriptRegistered(string key)
		{
			return (_header.GetHeaderScript(key) != null);
		}


		/// <summary>
		/// <see cref="System.Web.UI.Page.Render()"/>
		/// </summary>
		/// <param name="output"></param>
		protected override void Render(HtmlTextWriter output)
		{
			RenderConfigurationSections();
			base.Render(output);
		}


		/// <summary>
		/// Creates a new RenderProcessTicket and renders the configuration sections.
		/// </summary>
		private void RenderConfigurationSections()
		{
			RenderProcessTicket ticket = new RenderProcessTicket();
			JSControlCollection controls = new JSControlCollection();

			ticket.Items[RENDER_HANDLER_APP_ATTRIBUTE] = Request.ApplicationPath;
			ticket.Items[RENDER_HANDLER_CTRL_ATTRIBUTE] = controls;

			foreach (IJSToolsRenderHandler renderHandler in _renderHandlers)
			{
				ticket.AddRenderHandler(renderHandler);
			}

			// create controls, which will render the configuration
			Configuration.Render(ticket);

			// add controls to header
			foreach (Control renderControl in controls)
			{
				// crunch script sections, if we are not in debug mode
				if (renderControl.GetType() == typeof(Script)
					&& Configuration.ScriptFileHandler.DebugMode == DebugMode.None)
				{
					((Script)renderControl).Optimization = ScriptOptimization.Crunch;
				}
				_header.Controls.Add(renderControl);
			}
		}


		/// <summary>
		/// Initializes this page instance and the representing form object.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void OnPageInit(object sender, EventArgs args)
		{
			// init default render handlers
			for (int i = 0; i < _defaultRenderHandlers.Length; ++i)
			{
				_renderHandlers.Add(_defaultRenderHandlers.GetValue(i));
			}

			// init header
			_header = GetHeaderInstance();

			if (_header == null)
			{
				throw new ControlNotFoundException("Could not find a header control. Each JSToolsPage has to contain a head control!");
			}
		}


		/// <summary>
		/// Searches in the ChildControlCollection for a Header object.
		/// </summary>
		/// <returns>Returns the found header object or a null reference.</returns>
		private Head GetHeaderInstance()
		{
			Head header = null;

			foreach (Control control in Controls)
			{
				if ((header = (control as Head)) != null)
				{
					break;
				}
			}
			return header;
		}
	}
}

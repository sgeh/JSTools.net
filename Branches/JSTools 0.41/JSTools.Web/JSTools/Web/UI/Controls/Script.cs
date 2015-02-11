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
using System.Web.UI;
using System.Web.UI.WebControls;

using JSTools.Config.ScriptFileManagement;
using JSTools.Parser.Cruncher;
using JSTools.Web.UI;

namespace JSTools.Web.UI.Controls
{
	/// <summary>
	/// Describes, where a script control should be rendered.
	/// </summary>
	public enum ScriptSection
	{
		/// <summary>
		/// The script is rendered in the &lt;head&gt; tag.
		/// </summary>
		Head,

		/// <summary>
		/// The script is rendered direct below the &lt;form&gt; tag.
		/// </summary>
		Top,

		/// <summary>
		/// The script is rendered above the &lt;/form&gt; tag.
		/// </summary>
		Bottom,

		/// <summary>
		/// The script is rendered at the definition position.
		/// </summary>
		Inline
	}

	/// <summary>
	/// Crunches or removes the comments of the current script. This settings are not
	/// used if you specify a script source (with src tag). If you'd like to crunch the
	/// script source include, you have to add your file to the configuration.
	/// </summary>
	public enum ScriptOptimization
	{
		/// <summary>
		/// Default optimization instruction, settings of the configuration are used.
		/// </summary>
		Default,

		/// <summary>
		/// No script optimizations.
		/// </summary>
		None,

		/// <summary>
		/// Remove script comments. This will check the script syntax.
		/// </summary>
		RemoveComments,

		/// <summary>
		/// Remove comments and crunch the script. This will check the script syntax.
		/// </summary>
		Crunch,

		/// <summary>
		/// Check for javascript syntax errors.
		/// </summary>
		SyntaxCheck
	}

	/// <summary>
	/// Defines the properties for a &gt;script&lt; html tag. The default values of the properties
	/// are recieved from the JSTools configuration.
	/// </summary>
	/// <remarks>If there is no version specified, the default script language version (javascript1.3)
	/// is used.</remarks>
	[type: DefaultProperty("Code")]
	[type: ParseChildren(false)]
	[type: ToolboxData("<{0}:Script runat=\"server\"></{0}:Script>")]
	[type: DesignerCategory("Code")]
	[type: ToolboxItemFilter("JSTools.Web.UI.Controls", ToolboxItemFilterType.Require)]
	[type: ControlBuilder(typeof(ScriptControlBuilder))]
	public class Script : JSToolsControl
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const string SCRIPT_CACHE_ID = "JSTOOLS_SCRIPT_CACHE";
		private const string DEFAULT_SCRIPT_TYPE = "javascript";
		private const float DEFAULT_SCRIPT_VER = 1.3F;

		private bool _scriptCacheEnabled = true;
		private bool _renderHtmlComments = true;
		private string _src = string.Empty;
		private string _code = string.Empty;

		private string _type = string.Empty;
		private float _version = -1;

		private ScriptSection _section = ScriptSection.Inline;
		private ScriptOptimization _optimization = ScriptOptimization.Default;

		private bool _isRendered = false;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets/sets the section, where this script should be rendered. This property
		/// must be set before the OnPreRender event was fired.
		/// </summary>
		[property: Browsable(true)]
		[property: EditorBrowsable(EditorBrowsableState.Always)]
		[property: Bindable(BindableSupport.Yes)]
		[property: DefaultValue(ScriptSection.Inline)]
		[property: Category("Layout")]
		[property: Description("Describes the section, where this script should be rendered.")]
		[property: DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public ScriptSection Section
		{
			get { return _section; }
			set { _section = value; }
		}

		/// <summary>
		/// Gets/sets the script optimization. This optimizations are only available with the 
		/// script type "javascript". The default value of this property is specified in the
		/// configuration. If the configuration is in debug mode, optimizations are disabled.
		/// Otherwise the 'RemoveComments' optimization is used.
		/// </summary>
		/// <exception cref="InvalidOperationException">The DebugMode in the configuration is not supported.</exception>
		[property: Browsable(true)]
		[property: EditorBrowsable(EditorBrowsableState.Always)]
		[property: Bindable(BindableSupport.Yes)]
		[property: DefaultValue(ScriptOptimization.None)]
		[property: Category("Format")]
		[property: Description("Describes the script optimization, by default none.")]
		[property: DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public ScriptOptimization Optimization
		{
			get
			{
				if (Type != DEFAULT_SCRIPT_TYPE)
					return ScriptOptimization.None;

				if (_optimization == ScriptOptimization.Default)
				{
					switch (JSToolsContext.Configuration.ScriptFileHandler.DebugMode)
					{
						case DebugMode.None:
						{
							_optimization = ScriptOptimization.None;
							break;
						}
						case DebugMode.Module:
						{
							_optimization = ScriptOptimization.Crunch;
							break;
						}
						case DebugMode.File:
						{
							_optimization = ScriptOptimization.SyntaxCheck;
							break;
						}
						default:
						{
							throw new InvalidOperationException("The DebugMode in the configuration is not supported.");
						}
					}
				}
				return _optimization;
			}
			set { _optimization = value; }
		}

		/// <summary>
		/// Sets or gets the script version (e.g. 1.3). If the specified value is
		/// less than 1, this property will return the default version from the configuration.
		/// If the script version given by the configuration is equal to 0, no script version
		/// is used.
		/// </summary>
		[property: Browsable(true)]
		[property: EditorBrowsable(EditorBrowsableState.Always)]
		[property: Bindable(BindableSupport.Yes)]
		[property: DefaultValue(1.3F)]
		[property: Category("Default")]
		[property: Description("Describes the script version (e.g. 1.3), which should be used to render the script.")]
		[property: DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public float Version
		{
			get
			{
				if (_version < 1 && Type == JSToolsContext.Configuration.ScriptFileHandler.ScriptType)
					_version = JSToolsContext.Configuration.ScriptFileHandler.ScriptVersion;

				return (_version < 1) ? DEFAULT_SCRIPT_VER : _version;
			}
			set { _version = value; }
		}

		/// <summary>
		/// Sets or gets the script type (e.g. JavaScript). If the specified value is
		/// empty or contains a null reference, this property will return the default
		/// script type from the configuration.
		/// </summary>
		[property: Browsable(true)]
		[property: EditorBrowsable(EditorBrowsableState.Always)]
		[property: Bindable(BindableSupport.Yes)]
		[property: DefaultValue("javascript")]
		[property: Category("Default")]
		[property: Description("Describes the script type (e.g. JavaScript), which should be used to render the script.")]
		[property: DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public string Type
		{
			get
			{
				if (_type == null || _type.Length == 0)
					_type = JSToolsContext.Configuration.ScriptFileHandler.ScriptType;

				return (_type == null || _type.Length == 0) ? DEFAULT_SCRIPT_TYPE : _type;
			}
			set { _type = value; }
		}

		/// <summary>
		/// Sets or gets the script source file (e.g. /JSTools/Web.js). Optimizations are not
		/// available for source files.
		/// </summary>
		[property: Browsable(true)]
		[property: EditorBrowsable(EditorBrowsableState.Always)]
		[property: Bindable(BindableSupport.Yes)]
		[property: DefaultValue("")]
		[property: Category("Default")]
		[property: Description("Describes the script source file. Optimizations are not available.")]
		[property: DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public string SourceFile
		{
			get { return _src; }
			set { _src = (value != null) ? value : string.Empty; }
		}

		/// <summary>
		/// Gets/sets the script code which is written inside the &lt;script&gt; tags. This
		/// property allows to overwrite the script content specified as script-node value.
		/// If you specify data bound contents, you should make sure that you have called the
		/// DataBind() method before accessing this property.
		/// </summary>
		[property: Browsable(true)]
		[property: EditorBrowsable(EditorBrowsableState.Always)]
		[property: Bindable(BindableSupport.Yes)]
		[property: DefaultValue("")]
		[property: Category("Default")]
		[property: Description("Script code of this control. Not available, if the Source porperty is used.")]
		[property: DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string Code
		{
			get
			{
				if (_code == null || _code.Length == 0)
				{
					using (StringWriter writer = new StringWriter())
					{
						base.Render(new HtmlTextWriter(writer));
						_code = writer.ToString();
					}
				}
				return _code;
			}
			set { _code = (value != null) ? value : string.Empty; }
		}

		/// <summary>
		/// Gets/sets the cache id of the current instance. If the cache id is not empty and
		/// not null, the crunched code will be stored in the application cache.
		/// </summary>
		[property: Browsable(true)]
		[property: EditorBrowsable(EditorBrowsableState.Always)]
		[property: Bindable(BindableSupport.Yes)]
		[property: DefaultValue(true)]
		[property: Category("Behavior")]
		[property: Description("True to enable caching of crunched/comment removed scripts.")]
		[property: DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public bool IsScriptCacheEnabled
		{
			get { return _scriptCacheEnabled; }
			set { _scriptCacheEnabled = value; }
		}

		/// <summary>
		/// Gets/sets whether html-comment tags should be rendered (&lt;!-- CODE //--&gt;). 
		/// This is required for 3. generation browser, which can't perform the specified
		/// script. With the comment tags, those browers ignore the specifed script.
		/// </summary>
		[property: Browsable(true)]
		[property: EditorBrowsable(EditorBrowsableState.Always)]
		[property: Bindable(BindableSupport.Yes)]
		[property: DefaultValue(true)]
		[property: Category("Behavior")]
		[property: Description("True to render html-comment tags.")]
		[property: DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public bool RenderHtmlComments
		{
			get { return _renderHtmlComments; }
			set { _renderHtmlComments = value; }
		}

		/// <summary>
		/// Returns the script code chache instance.
		/// </summary>
		private string CachedCode
		{
			get 
			{
				if (Page.Cache[SCRIPT_CACHE_ID + UniqueID] == null)
					return string.Empty;

				return (Page.Cache[SCRIPT_CACHE_ID + UniqueID] as string);
			}
			set { Page.Cache[SCRIPT_CACHE_ID + UniqueID] = value; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new Script instance.
		/// </summary>
		public Script()
		{
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		/// <summary>
		/// If the script is visible and the RenderOnTop flag is set to true, this function will
		/// render the script to the client with the RegisterClientScriptBlock method.
		/// </summary>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			if (Visible)
			{
				switch (Section)
				{
					case ScriptSection.Head:
					{
						Page.RegisterHeaderContent(UniqueID, GetScriptTag());
						_isRendered = true;
						break;
					}
					case ScriptSection.Top:
					{
						Page.RegisterClientScriptBlock(UniqueID, GetScriptTag());
						_isRendered = true;
						break;
					}
					case ScriptSection.Bottom:
					{
						Page.RegisterStartupScript(UniqueID, GetScriptTag());
						_isRendered = true;
						break;
					}
				}
			}
		}

		/// <summary>
		/// Renders the script into the output, if it was not rendered yet and
		/// the section was marked as inline.
		/// </summary>
		protected override void Render(HtmlTextWriter output) 
		{
			if (Visible && Section == ScriptSection.Inline && !_isRendered)
				output.Write(GetScriptTag());
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Returns the script tag with the specified tag attributes.
		/// </summary>
		private string GetScriptTag()
		{
			if (SourceFile.Length != 0)
				return GetFileScriptTag();
			else
				return GetCodeScriptTag();
		}

		/// <summary>
		/// Returns the script tag with a source attribute.
		/// </summary>
		private string GetFileScriptTag()
		{
			if (Code.Length != 0)
				return JSToolsContext.Configuration.ScriptFileHandler.GetScriptFileTag(SourceFile, Code, Type, Version);
			else
				return JSToolsContext.Configuration.ScriptFileHandler.GetScriptFileTag(SourceFile, Type, Version);
		}

		/// <summary>
		/// Retruns a script tag without a source attribute and the script code
		/// as node value.
		/// </summary>
		private string GetCodeScriptTag()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(JSToolsContext.Configuration.ScriptFileHandler.GetScriptBeginTag(Type, Version, RenderHtmlComments));

			RenderScriptCode(builder);

			builder.Append(JSToolsContext.Configuration.ScriptFileHandler.GetScriptEndTag(RenderHtmlComments));
			return builder.ToString();
		}

		/// <summary>
		/// Append script requested code to the StringBuilder instance.
		/// </summary>
		/// <param name="toAppend">Code to append.</param>
		private void RenderScriptCode(StringBuilder toAppend)
		{
			string scriptCode;

			try
			{
				scriptCode = GetScriptFromCache();
			}
			catch (Exception e)
			{
				throw new ScriptOptimizationException("Error while optimize the given script source.", e);
			}

			if (!scriptCode.StartsWith("\n"))
				toAppend.Append("\n");

			toAppend.Append(scriptCode);
		}

		/// <summary>
		/// Retruns the script code from the cache, if caching was enabled.
		/// </summary>
		/// <returns>Returns the optimized script code.</returns>
		private string GetScriptFromCache()
		{
			// cache is not required, optimization disabled
			if (Optimization == ScriptOptimization.None)
				return Code;

			// is cache enabled
			if (IsScriptCacheEnabled)
			{
				if (CachedCode.Length == 0)
					CachedCode = OptimizeScript();

				return CachedCode;
			}
			return OptimizeScript();
		}

		/// <summary>
		/// Optimizes the given script source code, if that is required.
		/// </summary>
		/// <returns>Returns the optimized script.</returns>
		private string OptimizeScript()
		{
			switch (Optimization)
			{
				case ScriptOptimization.RemoveComments:
				{
					return JSToolsContext.Cruncher.RemoveComments(Code, Version);
				}
				case ScriptOptimization.Crunch:
				{
					return JSToolsContext.Cruncher.CrunchScript(Code, null, Version);
				}
				case ScriptOptimization.SyntaxCheck:
				{
					if (!JSToolsContext.Cruncher.IsValidScript(Code, Version))
						throw new ScriptOptimizationException("The specified script contains a syntax error.");

					return Code;
				}
				default:
				{
					// no optimization
					return Code;
				}
			}
		}
	}
}

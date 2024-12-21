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
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

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
		/// The script is rendered where it was created.
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
		/// No script optimizations (default).
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
	[type: DefaultProperty("Type")]
	[type: ParseChildren(false)]
	[type: ControlBuilder(typeof(LiteralControlBuilder))]
	[type: ToolboxData("<{0}:Script runat=\"server\"></{0}:Script>")]
	[type: DesignerCategory("Code")]
	[type: ToolboxItemFilter("JSTools.Web.Controls", ToolboxItemFilterType.Require)]
	public class Script : JSToolsControl
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private	const	string		SCRIPT_CACHE_ID		= "JSTOOLS_SCRIPT_CACHE";
		private	const	string		DEFAULT_SCRIPT_TYPE	= "javascript";
		private	const	float		DEFAULT_SCRIPT_VER	= 1.3F;

		private	bool				_scriptCacheEnabled	= true;
		private	string				_src				= string.Empty;
		private	string				_code				= string.Empty;

		private	string				_type				= DEFAULT_SCRIPT_TYPE;
		private	float				_version			= DEFAULT_SCRIPT_VER;

		private ScriptSection		_section			= ScriptSection.Inline;
		private	ScriptOptimization	_optimization		= ScriptOptimization.None;

		private	bool				_isRendered			= false;


		/// <summary>
		/// Returns the script code chache instance.
		/// </summary>
		private ScriptCodeCache Cache
		{
			get 
			{
				if (Page.Cache[SCRIPT_CACHE_ID] == null)
				{
					Page.Cache[SCRIPT_CACHE_ID] = new ScriptCodeCache();
				}
				return (Page.Cache[SCRIPT_CACHE_ID] as ScriptCodeCache);
			}
		}


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
		/// script type "javascript".
		/// </summary>
		[property: Browsable(true)]
		[property: EditorBrowsable(EditorBrowsableState.Always)]
		[property: Bindable(BindableSupport.Yes)]
		[property: DefaultValue(ScriptOptimization.None)]
		[property: Category("Format")]
		[property: Description("Describes the script optimization, by default none.")]
		[property: DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public ScriptOptimization Optimization
		{
			get { return _optimization; }
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
				if (_version < 1 && Type == Page.Configuration.ScriptFileHandler.ScriptType)
				{
					_version = Page.Configuration.ScriptFileHandler.ScriptVersion;
				}
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
				if (_type == null || _type == string.Empty)
				{
					_type = Page.Configuration.ScriptFileHandler.ScriptType;
				}
				return (_type == null || _type == string.Empty) ? DEFAULT_SCRIPT_TYPE : _type ;
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
			set { _src = value; }
		}


		/// <summary>
		/// Returns the node value of this script control.
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
			get { return _code; }
			set { _code = value; }
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
		[property: Description("True to enable caching of the crunched/comment removed scripts.")]
		[property: DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public bool IsScriptCacheEnabled
		{
			get { return _scriptCacheEnabled; }
			set { _scriptCacheEnabled = value; }
		}


		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new instance of the ClientScript class
		/// </summary>
		public Script()
		{
		}


		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Initilizes the script code.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs args)
		{
			if (HasControls() && (Controls[0] is LiteralControl))
			{
				_code = (Controls[0] as LiteralControl).Text;
			}
		}


		/// <summary>
		/// If the script is visible and the RenderOnTop flag is set to true, this function will
		/// render the script to the client with the RegisterClientScriptBlock method.
		/// </summary>
		/// <param name="output">
		/// TextWriter in which the render method should write the script content
		/// </param>
		protected override void OnPreRender(EventArgs eventSource)
		{
			if (Visible)
			{
				if (Section == ScriptSection.Head)
				{
					Page.RegisterStartupScript(UniqueID, GetScriptTag());
					_isRendered = true;
				}
				else if (Section == ScriptSection.Top)
				{
					Page.RegisterClientScriptBlock(UniqueID, GetScriptTag());
					_isRendered = true;
				}
				else if (Section == ScriptSection.Bottom)
				{
					Page.RegisterStartupScript(UniqueID, GetScriptTag());
					_isRendered = true;
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
			{
				output.Write(GetScriptTag());
			}
		}


		/// <summary>
		/// Returns the script tag with the specified tag attributes.
		/// </summary>
		private string GetScriptTag()
		{
			if (_src != null && _src != string.Empty)
			{
				return GetFileScriptTag();
			}
			else
			{
				return GetCodeScriptTag();
			}
		}


		/// <summary>
		/// Returns the script tag with a source attribute.
		/// </summary>
		private string GetFileScriptTag()
		{
			if (Code == null || Code == string.Empty)
			{
				return Page.Configuration.ScriptFileHandler.GetScriptFileTag(_src, Type, Version);
			}
			else
			{
				return Page.Configuration.ScriptFileHandler.GetScriptFileTag(_src, Code, Type, Version);
			}
		}


		/// <summary>
		/// Retruns a script tag without a source attribute and the script code
		/// as node value.
		/// </summary>
		private string GetCodeScriptTag()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(Page.Configuration.ScriptFileHandler.GetScriptBeginTag(Type, Version));

			AppendScriptCode(builder);

			builder.Append(Page.Configuration.ScriptFileHandler.GetScriptEndTag());
			return builder.ToString();
		}


		/// <summary>
		/// Append script requested code to the StringBuilder instance.
		/// </summary>
		/// <param name="toAppend">Code to append.</param>
		private void AppendScriptCode(StringBuilder toAppend)
		{
			string scriptCode;

			try
			{
				scriptCode = GetScriptFromCache();
			}
			catch (Exception e)
			{
				throw new ScriptOptimizationException("Error while optimize the given script source!", e);
			}

			if (!scriptCode.StartsWith("\n"))
			{
				toAppend.Append("\n");
			}
			toAppend.Append(scriptCode);
		}


		/// <summary>
		/// Retruns the script code from the cache, if caching was enabled.
		/// </summary>
		/// <returns>Returns the optimized script code.</returns>
		private string GetScriptFromCache()
		{
			// cache is not required without optimization
			if (_optimization != ScriptOptimization.None)
				return _code;

			// is cache enabled
			if (_scriptCacheEnabled)
			{
				if (!Cache.Contains(_optimization, _code))
				{
					Cache.AddScriptCode(_optimization, _code, OptimizeScript());
				}
				return Cache.GetScriptCode(_optimization, _code);
			}
			return OptimizeScript();
		}


		/// <summary>
		/// Optimizes the given script source code, if that is required.
		/// </summary>
		/// <returns>Returns the optimized script.</returns>
		private string OptimizeScript()
		{
			if (_optimization == ScriptOptimization.RemoveComments)
			{
				return RemoveComments();
			}
			else if (_optimization == ScriptOptimization.Crunch)
			{
				return CrunchScript();
			}
			else if (_optimization == ScriptOptimization.SyntaxCheck)
			{
				CheckForSyntaxErrors();
			}

			// no optimization
			return _code;
		}


		/// <summary>
		/// Removes all comments from the script.
		/// </summary>
		/// <returns>Returns the script without comments.</returns>
		private string RemoveComments()
		{
			return JSScriptCruncher.Instance.RemoveComments(_code, Version, true);
		}


		/// <summary>
		/// Crunches the script.
		/// </summary>
		/// <returns>Returns the crunched script.</returns>
		private string CrunchScript()
		{
			return JSScriptCruncher.Instance.Crunch(_code, Version);
		}


		/// <summary>
		/// Checks the script for syntax errors.
		/// </summary>
		private void CheckForSyntaxErrors()
		{
			JSScriptCruncher.Instance.Check(_code, Version);
		}
	}
}

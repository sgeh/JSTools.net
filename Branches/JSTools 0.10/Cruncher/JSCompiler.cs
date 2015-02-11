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
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using JSCompiler.Tab;

namespace JSCompiler
{
	/// <summary>
	/// Application Class
	/// </summary>
	public class JSCompiler : Form
	{
		public		const string		START_PAGE_NAME		= "start";
		public		const string		COMPILE_PAGE_NAME	= "compile";
		public		const string		DECOMPILE_PAGE_NAME	= "decompile";
		public		const string		OPTIMIZE_PAGE_NAME	= "optimize";

		public		const	string		APP_TITLE			= "JavaScript Compiler";
		public		const	string		APP_VERSION			= "0.1 a";
		public		const	string		APP_TEXT_CONFIG		= "JSText.xml";

		private		JSStyle				_appDesign;
		private		bool				_configLoaded;

		private		JSBaseTab			_startPage;
		private		JSBaseTab			_compilePage;
		private		JSBaseTab			_decompilePage;
		private		JSBaseTab			_optimizePage;

		private		Container			_components			= null;
		private		Hashtable			_buttonCollection	= new Hashtable();
		private		Hashtable			_labelCollection	= new Hashtable();
		private		Hashtable			_tabCollection		= new Hashtable();
		private		Hashtable			_tabPageCollection	= new Hashtable();
		private		Hashtable			_comobCollection	= new Hashtable();
		private		Hashtable			_textBoxCollection	= new Hashtable();



		public Container Components
		{
			get { return _components; }
		}


		public Hashtable ButtonCollection
		{
			get { return _buttonCollection; }
		}


		public Hashtable LabelCollection
		{
			get { return _labelCollection; }
		}


		public Hashtable TabCollection
		{
			get { return _tabCollection; }
		}

		
		public Hashtable TabPageCollection
		{
			get { return _tabPageCollection; }
		}


		public Hashtable ComobCollection
		{
			get { return _comobCollection; }
		}

		public Hashtable TextBoxCollection
		{
			get { return _textBoxCollection; }
		}


		internal JSStyle JSDesignDefinition
		{
			get { return _appDesign; }
		}


		internal bool JSConfigurationLoaded
		{
			get { return _configLoaded; }
		}



		public JSCompiler()
		{
			_appDesign		= new JSStyle(this);
			_components		= new Container();

			_configLoaded	= JSConfig.Instance.OpenConfigXmlDocument();
			InitializeComponent();
		}


		/// <summary>
		/// cleans up the ressources
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if (disposing)
			{
				if (_components != null) 
				{
					_components.Dispose();
				}
			}
			base.Dispose(disposing);
		}


		#region Window Coponent Code
		/// <summary>
		/// initializes the window components
		/// </summary>
		private void InitializeComponent()
		{
			TabControl jsCompTab		= _appDesign.CreateStyledTabControl("jsCompTab", 0, _configLoaded, true);
			jsCompTab.SizeMode			= TabSizeMode.Normal;
			jsCompTab.Size				= new Size(472,250);
			jsCompTab.Alignment			= TabAlignment.Top;
			jsCompTab.Location			= new Point(10, 10);
			jsCompTab.TabIndex			= 0;
			jsCompTab.SelectedIndex 	= 0;
			Controls.Add(jsCompTab);

			_startPage					= new JSStartPageTab(this);
			_compilePage				= new JSCompileTab(this);
			_decompilePage				= new JSDecompileTab(this);
			_optimizePage				= new JSOptimizeTab(this);
			
			jsCompTab.Controls.AddRange(
				new Control[]
				{
					_startPage.ElementPage, 
					_compilePage.ElementPage,
					_decompilePage.ElementPage,
					_optimizePage.ElementPage
				} );
		}
		#endregion


		/// <summary>
		/// static startup method
		/// </summary>
		[STAThread]
		public static void Main() 
		{
			Application.Run(new JSCompiler());
		}
	}
}

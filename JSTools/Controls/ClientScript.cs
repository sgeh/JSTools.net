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
using System.IO;
using System.Text;
using System.Web.UI;

namespace JSTools.Controls
{
	/// <summary>
	/// Defines the properties for a &gt;script&lt; html tag
	/// </summary>
	public class ClientScript : System.Web.UI.WebControls.WebControl
	{
		private	string			_id					= "";
		private	string			_src				= "";
		private	string			_language			= "javascript";
		private	bool			_visible			= true;
		private StringBuilder	_literalContent		= new StringBuilder();

		private bool			_renderOnTop		= false;


		//-----------------------------------------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------------------------------------


		//-----------------------------------------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------------------------------------

		/// <summary>
		/// sets or gets the information if the script should be rendered on the top
		/// </summary>
		public bool RenderOnTop
		{
			get { return _renderOnTop; }
			set { _renderOnTop = value; }
		}


		/// <summary>
		/// sets or gets the script language, by default "javascript"
		/// </summary>
		public string Language
		{
			get { return _language; }
			set { _language = value; }
		}


		/// <summary>
		/// sets or gets the script source (e.g. http://www.jstools.com/scripts/sample.js)
		/// </summary>
		public string Src
		{
			get { return _src; }
			set { _src = value; }
		}


		/// <summary>
		/// sets or gets the script visibility
		/// </summary>
		public override bool Visible
		{
			get { return _visible; }
			set { _visible = value; }
		}

		
		/// <summary>
		/// sets or gets the server control id
		/// </summary>
		public override string ID
		{
			get { return _id; }
			set { _id = value; }
		}


		/// <summary>
		/// returns the literal content of the script control
		/// </summary>
		public StringBuilder LiteralContent
		{
			get { return _literalContent; }
		}


		//-----------------------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//-----------------------------------------------------------------------------------------------------------

		/// <summary>
		/// creates a new instance of the ClientScript class
		/// </summary>
		public ClientScript() : base(HtmlTextWriterTag.Unknown) 
		{
		}


		//-----------------------------------------------------------------------------------------------------------
		// Public Methods
		//-----------------------------------------------------------------------------------------------------------


		//-----------------------------------------------------------------------------------------------------------
		// Protected Methods
		//-----------------------------------------------------------------------------------------------------------		

		/// <summary>
		/// initilizes the written literal script content
		/// </summary>
		/// <param name="e">EventArguments e</param>
		protected override void OnLoad(EventArgs e)
		{
			if (HasControls() && (Controls[0] is LiteralControl))
			{
				_literalContent.Append(((LiteralControl)Controls[0]).Text);
			}		
		}


		/// <summary>
		/// if the script is visible and the RenderOnTop flag is set to
		/// true, this function will render the script to the client with the
		/// RegisterClientScriptBlock method.
		/// </summary>
		/// <param name="output">
		/// TextWriter in which the render method should write
		/// the script content
		/// </param>
		protected override void OnPreRender(EventArgs eventSource)
		{
			if (_visible && _renderOnTop)
			{
				StringWriter renderString = new StringWriter();
				InsertClientScript(renderString);
				Page.RegisterClientScriptBlock(UniqueID, renderString.ToString());
			}
		}


		/// <summary>
		/// if the script is visible and the RenderOnTop flag is set to
		/// false, this function will render the script to the client on
		/// the render event.
		/// </summary>
		/// <param name="output">
		/// TextWriter in which the render method should write
		/// the script content
		/// </param>
		protected override void Render(HtmlTextWriter output) 
		{
			if(_visible && !_renderOnTop)
			{
				InsertClientScript(output);
			}
		}


		//-----------------------------------------------------------------------------------------------------------
		// Private Methods
		//-----------------------------------------------------------------------------------------------------------	

		/// <summary>
		/// renders the script with the specified tag attributes into the output parameter
		/// </summary>
		/// <param name="output">TextWriter where you d'like to insert the rendered html source</param>
		private void InsertClientScript(TextWriter output)
		{
			if (output == null)
				throw new ArgumentException("The given reference to a StringWriter instance contains a null pointer!");

			output.Write("<script language=\"");
			output.Write(_language);
			output.Write("\"");
			output.Write(GetScriptSource());
			output.Write("><!--\n");
			output.Write(LiteralContent.ToString());
			output.Write("\n//--></script>\n");
		}


		/// <summary>
		/// if _src is not equal to an empty string this function will return a 
		/// string which includes the _src value and the src attribute, otherwise
		/// it will return an empty string ("")
		/// </summary>
		/// <returns>returns the given source string</returns>
		private string GetScriptSource()
		{
			return (_src != "") ? " src=\"" + _src + "\" " : "";
		}
	}
}

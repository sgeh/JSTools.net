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
///     <since>JSTools.dll 0.2.0</since>
/// </file>

using System;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using JSTools.Web.Request;

namespace JSTools.Web.UI.Controls
{
	/// <summary>
	/// Represents a comment tag (&lt;-- and --&gt;).
	/// </summary>
	[type: DefaultProperty("Text")]
	[type: ParseChildren(false)]
	[type: ControlBuilder(typeof(LiteralControlBuilder))]
	public class Comment : JSToolsControl
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const string BEGIN_TAG = "<!--";
		private const string END_TAG = "-->";

		private string _text;


		/// <summary>
		/// Gets/sets the comment code, which will be rendered to the client.
		/// </summary>
		[Category("Misc")]
		[Description("Represents the comment code, which will be rendered to the client.")]
		public string Text
		{
			get { return _text; }
			set { _text = value; }
		}


		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new instance of the Comment class.
		/// </summary>
		public Comment()
		{
		}


		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Renders the script into the output, if it was not rendered yet and
		/// the section was marked as inline.
		/// </summary>
		protected override void Render(HtmlTextWriter output) 
		{
			if (Visible)
			{
				output.Write(BEGIN_TAG);
				output.Write(Text);
				output.Write(END_TAG);
			}
		}
	}
}

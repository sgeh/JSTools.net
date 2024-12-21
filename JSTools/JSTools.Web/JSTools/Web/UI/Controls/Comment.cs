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

		private string _text = string.Empty;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets/sets the comment code, which will be rendered to the client.
		/// </summary>
		[Category("Misc")]
		[Description("Represents the comment code, which will be rendered to the client.")]
		public string Text
		{
			get { return (_text != null) ? _text : string.Empty; }
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

		/// <summary>
		/// Creates a new instance of the Comment class.
		/// </summary>
		/// <param name="commentText">Sets the comment code, which will be rendered to the client.</param>
		public Comment(string commentText)
		{
			Text = commentText;
		}

		//--------------------------------------------------------------------
		// Events
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

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------
	}
}

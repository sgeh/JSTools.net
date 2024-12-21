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
using System.Web.UI;
using System.Web.UI.HtmlControls;

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

		private	const string	TAG_NAME	= "head";
		private Hashtable _headerScripts	= new Hashtable();


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
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Adds a new script section to the header. The script tags will be filled in
		/// automatically.
		/// </summary>
		/// <param name="key">Script key.</param>
		/// <param name="script">Script code.</param>
		/// <exception cref="ArgumentNullException">The given key contains a null reference.</exception>
		/// <exception cref="ArgumentNullException">The given script contains a null reference.</exception>
		public void AddHeaderScript(string key, string script)
		{
			if (key == null)
				throw new ArgumentNullException("key", "The given key contains a null reference!");

			if (script == null)
				throw new ArgumentNullException("script", "The given script contains a null reference!");

			_headerScripts.Add(key, script);
		}


		/// <summary>
		/// Adds a new script section to the header. The script tags will be filled in
		/// automatically.
		/// </summary>
		/// <param name="key">Script key.</param>
		/// <exception cref="ArgumentNullException">The given key contains a null reference.</exception>
		public string GetHeaderScript(string key)
		{
			if (key == null)
				throw new ArgumentNullException("key", "The given key contains a null reference!");

			return (string)_headerScripts[key];
		}


		/// <summary>
		/// <see cref="System.Web.UI.Page.Render"/>
		/// </summary>
		/// <param name="output"></param>
		protected override void Render(HtmlTextWriter output)
		{
			output.RenderBeginTag(TAG_NAME);
			base.Render(output);

			foreach (DictionaryEntry entry in _headerScripts)
			{
				output.Write(entry.Value);
			}
			output.RenderEndTag();
		}
	}
}

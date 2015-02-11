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
using System.Text;
using System.Xml;

namespace JSTools.Config.ScriptFileManagement
{
	/// <summary>
	/// Contains an writeable &lt;file&gt; node implementation.
	/// </summary>
	public class JSScriptWriteable : AJSScript
	{
		//------------------------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Sets the code of the script.
		/// </summary>
		public override string CommentCode
		{
			get { return _code; }
			set { _code = value; }
		}


		/// <summary>
		/// Returns the name of the file.
		/// </summary>
		public override string FileName
		{
			get { return _fileName; }
			set { _fileName = value; }
		}


		//------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Creates a new writeable JSScript instance.
		/// </summary>
		/// <param name="scriptNode">Xml node, which contains the source informations.</param>
		/// <param name="ownerHandler">Parent JSScriptFileHandler.</param>
		/// <exception cref="ArgumentNullException">An argument contains a null reference.</exception>
		/// <exception cref="ArgumentException">A AJSScript with the given path is already imported.</exception>
		internal JSScriptWriteable(XmlNode scriptNode, AJSScriptFileHandler ownerHandler) : base(scriptNode, ownerHandler)
		{
		}


		/// <summary>
		/// Creates a new writeable JSScript instance.
		/// </summary>
		/// <param name="folder">Folder, in which this script is stored.</param>
		/// <param name="scriptNode">Xml node, which contains the source informations.</param>
		/// <param name="ownerHandler">Parent JSScriptFileHandler.</param>
		/// <exception cref="ArgumentNullException">An argument contains a null reference.</exception>
		/// <exception cref="ArgumentException">A AJSScript with the given path is already imported.</exception>
		internal JSScriptWriteable(string folder, XmlNode scriptNode, AJSScriptFileHandler ownerHandler) : base(folder, scriptNode, ownerHandler)
		{
		}


		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------
	}
}

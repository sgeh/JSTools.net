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
using System.Xml;

using JSTools.Config.Session;

namespace JSTools.Config.ScriptFileManagement
{
	/// <summary>
	/// Represents an writeable instance of the &lt;scripts&gt; configuration section in the
	/// JSTools.net configuration.
	/// </summary>
	public class JSScriptFileHandlerWriteable : AJSScriptFileHandler
	{
		//------------------------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Gets/Sets the scripts, which are defined in the configuration document.
		/// True means the debug script will be used, false means, the release scripts will be used.
		/// </summary>
		public override bool Debug
		{
			get { return _debug; }
			set { _debug = value; }
		}


		/// <summary>
		/// Gets/Sets the type of the scripts e.g. "JavaScript".
		/// </summary>
		/// <exception cref="ArgumentNullException">The specified string contains a null reference.</exception>
		public override string ScriptType
		{
			get { return _scriptType; }
			set
			{
				if (_scriptType == null)
				{
					throw new ArgumentNullException("ScriptType", "The specified string contains a null reference!");
				}
				_scriptType = value;
			}
		}


		/// <summary>
		/// Gets/Sets the type of the scripts e.g. "JavaScript".
		/// </summary>
		public override double ScriptVersion
		{
			get { return _scriptVersion; }
			set { _scriptVersion = value; }
		}


		/// <summary>
		/// Gets/Sets the url, where the debug scripts are stored.
		/// </summary>
		/// <exception cref="ArgumentNullException">The specified string contains a null reference.</exception>
		public override string DebugScriptSource
		{
			get { return _debugScriptSource; }
			set
			{
				if (_debugScriptSource == null)
				{
					throw new ArgumentNullException("DebugScriptSource", "The specified string contains a null reference!");
				}
				_debugScriptSource = value;
			}
		}


		/// <summary>
		/// Gets/Sets the url, where the debug scripts are stored.
		/// </summary>
		/// <exception cref="ArgumentNullException">The specified string contains a null reference.</exception>
		public override string ReleaseScriptSource
		{
			get { return _releaseScriptSource; }
			set
			{
				if (_releaseScriptSource == null)
				{
					throw new ArgumentNullException("ReleaseScriptSource", "The specified string contains a null reference!");
				}
				_releaseScriptSource = value;
			}
		}


		//------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSScriptFileHandler instance.
		/// </summary>
		/// <param name="section">Xml section.</param>
		/// <param name="nodeName">Contains the name of the representing node.</param>
		public JSScriptFileHandlerWriteable(XmlNode section, string nodeName) : base(section, nodeName)
		{
		}


		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Creates a new AJSModuleContainer instance for internal use.
		/// </summary>
		/// <returns>Returns the created AJSModuleContainer.</returns>
		protected override AJSModuleContainer CreateModuleContainer()
		{
			return new JSModuleContainerWriteable(this);
		}
	}
}

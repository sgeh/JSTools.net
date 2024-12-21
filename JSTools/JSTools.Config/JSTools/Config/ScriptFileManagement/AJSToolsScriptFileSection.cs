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
using System.Collections;
using System.Text;
using System.Xml;

namespace JSTools.Config.ScriptFileManagement
{
	/// <summary>
	/// Represents an element below the &lt;script&gt; section.
	/// </summary>
	public abstract class AJSToolsScriptFileSection : AJSToolsSection
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		/// <summary>
		/// This event is fired if the hierarchy was created.
		/// </summary>
		internal event EventHandler CheckModuleRelations;


		private	AJSToolsScriptFileSection _parent = null;


		/// <summary>
		/// Returns the configuration section handler, which contains this instance.
		/// </summary>
		public AJSToolsScriptFileSection OwnerSection
		{
			get { return (_parent != null) ? _parent.OwnerSection : this; }
		}


		/// <summary>
		/// Returns the configuration section handler, which contains this instance.
		/// </summary>
		public AJSToolsScriptFileSection ParentSection
		{
			get { return _parent; }
		}


		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new AJSToolsEventHandler instance.
		/// </summary>
		/// <param name="parent">The parent event handler.</param>
		public AJSToolsScriptFileSection(AJSToolsScriptFileSection parent) : base(parent.OwnerConfiguration)
		{
			_parent = parent;
			_parent.CheckModuleRelations += new EventHandler(OnCheckModuleRelations);
		}


		/// <summary>
		/// Creates a new AJSToolsEventHandler instance.
		/// </summary>
		/// <param name="parent">The parent event handler.</param>
		public AJSToolsScriptFileSection(IJSToolsConfiguration parent) : base(parent)
		{
			_parent = null;
		}


		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Bubbles the CheckRelations event.
		/// </summary>
		/// <param name="sender">Sender object.</param>
		/// <param name="e">Event argument object.</param>
		/// <exception cref="InvalidOperationException">A required module could not be found.</exception>
		protected virtual void OnCheckModuleRelations(object sender, EventArgs e)
		{
			if (CheckModuleRelations != null)
			{
				CheckModuleRelations(this, e);
			}
		}
	}
}

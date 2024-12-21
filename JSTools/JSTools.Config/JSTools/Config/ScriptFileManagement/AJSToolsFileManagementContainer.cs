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
///     <since>JSTools.dll 0.1.1</since>
/// </file>

using System;
using System.Collections;
using System.Text;
using System.Xml;

using JSTools.Config.Session;
using JSTools.Xml;

namespace JSTools.Config.ScriptFileManagement
{
	/// <summary>
	/// Represents an abstract FileManagementSection list.
	/// </summary>
	public abstract class AJSToolsFileManagementContainer : IEnumerable
	{
		//------------------------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------------------------

		private AFileManagementSection	_parentSection	= null;
		private AJSScriptFileHandler	_ownerSection	= null;


		/// <summary>
		/// Returns the owner section of this container.
		/// </summary>
		public AJSScriptFileHandler OwnerSection
		{
			get { return _ownerSection; }
		}


		/// <summary>
		/// Returns the parent section, which was given by the constructor.
		/// </summary>
		public AFileManagementSection ParentSection 
		{
			get { return _parentSection; }
		}


		//------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Creates a new AJSToolsRenderHandlerContainer instance.
		/// </summary>
		public AJSToolsFileManagementContainer(AFileManagementSection parentSection)
		{
			if (parentSection == null)
				throw new ArgumentNullException("parentSection", "The given parent section contains a null reference!");

			if (parentSection as IWriteable == null)
				throw new ArgumentException("The given argument is not derived from the IWriteable interface!");

			if (parentSection as AJSScriptFileHandler != null)
			{
				_ownerSection = (AJSScriptFileHandler)parentSection;
			}
			else
			{
				_ownerSection = (AJSScriptFileHandler)parentSection.OwnerSection;
			}
			_parentSection = parentSection;
		}


		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Gets an enumerator for this container.
		/// </summary>
		/// <returns>Returns a new enumerator for navigating through the container.</returns>
		public abstract IEnumerator GetEnumerator();
	}
}

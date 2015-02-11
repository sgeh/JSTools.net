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

using JSTools.Config.Session;

namespace JSTools.Config.ScriptFileManagement
{
	/// <summary>
	/// Used by all file management sections for load and remove events.
	/// </summary>
	public delegate void JSToolsFileSectionEvent(AFileManagementSection sender);


	/// <summary>
	/// Represents an interface for all file management sections.
	/// </summary>
	public abstract class AFileManagementSection : AJSToolsEventHandler
	{
		//------------------------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------------------------

		private		ArrayList				_externalLoadEvents		= new ArrayList();
		private		ArrayList				_externalRemoveEvents	= new ArrayList();

		protected	JSToolsFileSectionEvent	_load;
		protected	JSToolsFileSectionEvent	_remove;


		/// <summary>
		/// Occurs when the whole section was loaded successfully.
		/// </summary>
		public event JSToolsFileSectionEvent OnLoad
		{
			add
			{
				_externalLoadEvents.Add(value);
				_load += value;
			}
			remove
			{
				if (_externalLoadEvents.Contains(value))
				{
					_externalLoadEvents.Remove(value);
				}
				_load -= value;
			}
		}


		/// <summary>
		/// Occurs if the user deletes this module.
		/// </summary>
		public event JSToolsFileSectionEvent OnRemove
		{
			add
			{
				_externalRemoveEvents.Add(value);
				_remove += value;
			}
			remove
			{
				if (_externalRemoveEvents.Contains(value))
				{
					_externalRemoveEvents.Remove(value);
				}
				_remove -= value;
			}
		}


		//------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Creates a new AFileManagementSection instance.
		/// </summary>
		public AFileManagementSection()
		{
			OnInit += new JSToolsInitEvent(OnInitFired);
		}


		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Adds the specified event to the OnRemove event handler. All internal registered event
		/// handlers will not be copied into the writeable instance if the OnMove event occurs.
		/// </summary>
		/// <param name="removeEvent">JSToolsFileSectionEvent handler to register.</param>
		public void AppendInternalRemoveEvent(JSToolsFileSectionEvent removeEvent)
		{
			_remove += removeEvent;
		}


		/// <summary>
		/// Adds the specified event to the OnLoad event handler. All internal registered event
		/// handlers will not be copied into the writeable instance if the OnMove event occurs.
		/// </summary>
		/// <param name="loadEvent">JSToolsFileSectionEvent handler to register.</param>
		public void AppendInternalLoadEvent(JSToolsFileSectionEvent loadEvent)
		{
			_load += loadEvent;
		}


		/// <summary>
		/// This method will be called, if the user is deleting this file management section. Event bubbling
		/// functionality must be implemented by this method.
		/// </summary>
		/// <param name="sender">The event sender.</param>
		protected virtual void RemoveFileManagementSection(AFileManagementSection sender)
		{
			if (_remove != null)
			{
				_remove(this);
			}
		}


		/// <summary>
		/// This method will be called, if configuration is loading this file management section. Event bubbling
		/// functionality must be implemented by this method.
		/// </summary>
		/// <param name="sender">The event sender.</param>
		protected virtual void LoadFileManagementSection(AFileManagementSection sender)
		{
			if (_load != null)
			{
				_load(this);
			}
		}


		/// <summary>
		/// Will be called when a parent section was set.
		/// </summary>
		/// <param name="sender">Sender, which has send the event.</param>
		/// <param name="newParent">New parent element instance.</param>
		private void OnInitFired(AJSToolsEventHandler sender, AJSToolsEventHandler newParent)
		{
			AFileManagementSection parentElement = (ParentSection as AFileManagementSection);

			if (parentElement != null)
			{
				parentElement.AppendInternalRemoveEvent(new JSToolsFileSectionEvent(RemoveFileManagementSection));
				parentElement.AppendInternalLoadEvent(new JSToolsFileSectionEvent(LoadFileManagementSection));
			}
		}
	}
}

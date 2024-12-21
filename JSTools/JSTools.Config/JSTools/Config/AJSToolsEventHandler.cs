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
using System.Threading;
using System.Xml;

namespace JSTools.Config
{
	/// <summary>
	/// Used for rendering and initializing modules.
	/// </summary>
	public delegate void JSToolsRenderEvent(RenderProcessTicket processTicket);


	/// <summary>
	/// Used to serialize the specified sections.
	/// </summary>
	public delegate void JSToolsSerializeEvent(XmlNode sectionNode, bool deep);


	/// <summary>
	/// Used by initializing an element instance.
	/// </summary>
	public delegate void JSToolsInitEvent(AJSToolsEventHandler sender, AJSToolsEventHandler newParent);

	/// <summary>
	/// Describes the initial state of a section.
	/// </summary>
	public enum InitState
	{
		NothingDone,
		Instantiated,
		Initialized
	}


	/// <summary>
	/// Each JSTools.net event handler instance must be derived form this abstract class. This class is
	/// used to handling events for a configuration section.
	/// </summary>
	public abstract class AJSToolsEventHandler
	{
		//------------------------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------------------------

		private		readonly	ReaderWriterLock	_dataLock					= new ReaderWriterLock();

		private		JSToolsRenderEvent				_preRender;
		private		JSToolsRenderEvent				_render;
		private		JSToolsSerializeEvent			_serialize;
		private		JSToolsInitEvent				_init;

		private		ArrayList						_externalPreRenderEvents	= new ArrayList();
		private		ArrayList						_externalRenderEvents		= new ArrayList();
		private		ArrayList						_externalSerializeEvents	= new ArrayList();

		private		AJSToolsEventHandler			_parent						= null;
		private		InitState						_state						= InitState.NothingDone;


		/// <summary>
		/// Returns the initial state of this object.
		/// </summary>
		public InitState InitialState
		{
			get
			{
                AcquireReaderLock();

				try
				{
					return _state;
				}
				finally
				{
					ReleaseReaderLock();
				}
			}
		}


		/// <summary>
		/// Occurs if the user adds this section to a parent element. This event does not need
		/// an external event container because this event will only occure in two cases:
		///  - If the JSToolsConfiguration is initializing, so the user can't add events.
		///  - If the user has created a new (writeable) element, and the user adds it to a parent
		///    element.
		/// Events which must be shared between a writeable and immutable instance, aren't required
		/// in both cases. This event is already thread-synchronized, you don't need to acquire an
		/// additional lock statement.
		/// </summary>
		public event JSToolsInitEvent OnInit
		{
			add
			{
				AcquireWriterLock();

				try
				{
					_init += value;
				}
				finally
				{
					ReleaseWriterLock();
				}
			}
			remove
			{
				AcquireWriterLock();

				try
				{
					_init -= value;
				}
				finally
				{
					ReleaseWriterLock();
				}
			}
		}


		/// <summary>
		/// Fires if the user renders the configuration with a RenderProcessTicket. This event
		/// will be fired before calling the OnRender event, to prepare render operations.
		/// </summary>
		public event JSToolsRenderEvent OnPreRender
		{
			add
			{
				AcquireWriterLock();

				try
				{
					_externalPreRenderEvents.Add(value);
					_preRender += value;
				}
				finally
				{
					ReleaseWriterLock();
				}
			}
			remove
			{
				AcquireWriterLock();

				try
				{
					if (_externalPreRenderEvents.Contains(value))
					{
						_externalPreRenderEvents.Remove(value);
					}
					_preRender -= value;
				}
				finally
				{
					ReleaseWriterLock();
				}
			}
		}


		/// <summary>
		/// Fires if the user renders the configuration with a RenderProcessTicket.
		/// </summary>
		public event JSToolsRenderEvent OnRender
		{
			add
			{
				AcquireWriterLock();

				try
				{
					_externalRenderEvents.Add(value);
					_render += value;
				}
				finally
				{
					ReleaseWriterLock();
				}
			}
			remove
			{
				AcquireWriterLock();

				try
				{
					if (_externalRenderEvents.Contains(value))
					{
						_externalRenderEvents.Remove(value);
					}
					_render -= value;
				}
				finally
				{
					ReleaseWriterLock();
				}
			}
		}


		/// <summary>
		/// Fires if the user serializes the configuration. The output format will be XML.
		/// </summary>
		public event JSToolsSerializeEvent OnSerialize
		{
			add
			{
				AcquireWriterLock();

				try
				{
					_externalSerializeEvents.Add(value);
					_serialize += value;
				}
				finally
				{
					ReleaseWriterLock();
				}
			}
			remove
			{
				AcquireWriterLock();

				try
				{
					if (_externalSerializeEvents.Contains(value))
					{
						_externalSerializeEvents.Remove(value);
					}
					_serialize -= value;
				}
				finally
				{
					ReleaseWriterLock();
				}
			}
		}


		/// <summary>
		/// Returns the configuration instance which has created this FileHandler.
		/// </summary>
		public abstract IJSToolsConfiguration OwnerConfiguration
		{
			get;
		}


		/// <summary>
		/// Returns the configuration section handler, which contains this instance.
		/// </summary>
		public abstract AJSToolsEventHandler OwnerSection
		{
			get;
		}


		/// <summary>
		/// Returns the configuration section handler, which contains this instance.
		/// </summary>
		/// <exception cref="InvalidOperationException">Could not reference to the parent section, it is not given yet.</exception>
		public AJSToolsEventHandler ParentSection
		{
			get
			{
				AcquireReaderLock();

				if (InitialState != InitState.Initialized)
					throw new InvalidOperationException("Could not reference to the parent section, it is not given yet!");
				
				try
				{
					return _parent;
				}
				finally
				{
					ReleaseReaderLock();
				}
			}
		}


		//------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Creates a new AJSToolsEventHandler instance.
		/// </summary>
		public AJSToolsEventHandler()
		{
			_state = InitState.Instantiated;
		}


		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Adds the specified event to the OnPreRender event handler. All internal registered event
		/// handlers will not be copied into the writeable instance if the OnMove event occurs.
		/// </summary>
		/// <param name="preRenderEvent">Event handler to register.</param>
		public void AppendInternalPreRenderEvent(JSToolsRenderEvent preRenderEvent)
		{
			AcquireWriterLock();

			try
			{
				_preRender += preRenderEvent;
			}
			finally
			{
				ReleaseWriterLock();
			}
		}


		/// <summary>
		/// Adds the specified event to the OnRender event handler. All internal registered event
		/// handlers will not be copied into the writeable instance if the OnMove event occurs.
		/// </summary>
		/// <param name="renderEvent">Event handler to register.</param>
		public void AppendInternalRenderEvent(JSToolsRenderEvent renderEvent)
		{
			AcquireWriterLock();

			try
			{
				_render += renderEvent;
			}
			finally
			{
				ReleaseWriterLock();
			}
		}


		/// <summary>
		/// Adds the specified event to the OnSerialize event handler. All internal registered event
		/// handlers will not be copied into the writeable instance if the OnMove event occurs.
		/// </summary>
		/// <param name="serializeEvent">Event handler to register.</param>
		public void AppendInternalSerializeEvent(JSToolsSerializeEvent serializeEvent)
		{
			AcquireWriterLock();

			try
			{
				_serialize += serializeEvent;
			}
			finally
			{
				ReleaseWriterLock();
			}
		}


		/// <summary>
		/// Registers the render, prerender and serialize event into the parent instance.
		/// </summary>
		/// <param name="parent">The parent event handler.</param>
		/// <exception cref="InvalidOperationException">Could not initialize the parent twice.</exception>
		/// <exception cref="ArgumentNullException">The specified handler contains a null reference.</exception>
		/// <exception cref="JSToolsEventException">An error has occured while firing the OnInit event.</exception>
		public void SetParent(AJSToolsEventHandler parent)
		{
			AcquireWriterLock();

			try
			{
				// check init state
				if (InitialState != InitState.Instantiated)
					throw new InvalidOperationException("Could not initialize the parent twice!");

				if (parent == null)
					throw new ArgumentNullException("parent", "The specified handler contains a null reference!");

				_parent = parent;

				_parent.AppendInternalPreRenderEvent(new JSToolsRenderEvent(PreRenderScriptConfiguration));
				_parent.AppendInternalRenderEvent(new JSToolsRenderEvent(RenderScriptConfiguration));
				_parent.AppendInternalSerializeEvent(new JSToolsSerializeEvent(SerializeXmlConfiguration));

				// set init state to initialized
				_state = InitState.Initialized;

				try
				{
					_init(this, parent);
				}
				catch (Exception e)
				{
					throw new JSToolsEventException(e, "An error has occured while firing the OnInit event!", "OnInit");
				}
			}
			finally
			{
				ReleaseWriterLock();
			}
		}


		/// <summary>
		/// Aquires a reader lock (multi-threading).
		/// </summary>
		protected void AcquireReaderLock()
		{
			_dataLock.AcquireReaderLock(0);
		}


		/// <summary>
		/// Releases a reader lock (multi-threading).
		/// </summary>
		protected void ReleaseReaderLock()
		{
			_dataLock.ReleaseReaderLock();
		}


		/// <summary>
		/// Aquires a writer lock (multi-threading).
		/// </summary>
		protected void AcquireWriterLock()
		{
			_dataLock.AcquireWriterLock(0);
		}


		/// <summary>
		/// Releases a writer lock (multi-threading).
		/// </summary>
		protected void ReleaseWriterLock()
		{
			_dataLock.ReleaseWriterLock();
		}


		/// <summary>
		/// This method will be called, if the parent element has fired the OnPreRender event. Event bubbling
		/// functionality must be implemented by this method. This event will be fired before calling the OnRender
		/// event, to prepare render operations.
		/// </summary>
		/// <param name="processTicket">A StringBuilder object which content will be rendered to the client.</param>
		protected virtual void PreRenderScriptConfiguration(RenderProcessTicket processTicket)
		{
			FirePreRenderEvent(processTicket);
		}


		/// <summary>
		/// This method will be called, if the parent element has fired the OnRender event. Event bubbling
		/// functionality must be implemented by this method. Renders the JavaScript context of the current section
		/// to the client.
		/// </summary>
		/// <param name="processTicket">A StringBuilder object which content will be rendered to the client.</param>
		protected virtual void RenderScriptConfiguration(RenderProcessTicket processTicket)
		{
			FireRenderEvent(processTicket);
		}


		/// <summary>
		/// This method will be called, if the parent element has fired the OnSerialize event. Renders the configuration
		/// settings of the current section into the given XmlNode instance. The current instance will not fire an event,
		/// if the deep flag is set to false.
		/// </summary>
		/// <param name="parentNode">Parent xml node instance. You have to create a new xml node instance and append it
		/// to the parent node.</param>
		/// <param name="deep">True to copy all sub elements, otherwise only the settings of the current node will be
		///  copied.</param>
		protected virtual void SerializeXmlConfiguration(XmlNode parentNode, bool deep)
		{
			FireSerializeEvent(parentNode, deep);
		}


		/// <summary>
		/// Fires the PreRender event.
		/// </summary>
		/// <param name="processTicket"></param>
		protected void FirePreRenderEvent(RenderProcessTicket processTicket)
		{
			AcquireReaderLock();

			try
			{
				if (_preRender != null)
				{
					_preRender(processTicket);
				}
			}
			finally
			{
				ReleaseReaderLock();
			}
		}


		/// <summary>
		/// Fires the Render event.
		/// </summary>
		/// <param name="processTicket"></param>
		protected void FireRenderEvent(RenderProcessTicket processTicket)
		{
			AcquireReaderLock();

			try
			{
				if (_render != null)
				{
					_render(processTicket);
				}
			}
			finally
			{
				ReleaseReaderLock();
			}
		}


		/// <summary>
		/// Fires the Serialize event.
		/// </summary>
		/// <param name="deep"></param>
		/// <param name="parentNode"></param>
		protected void FireSerializeEvent(XmlNode parentNode, bool deep)
		{
			AcquireReaderLock();

			try
			{
				if (_serialize != null)
				{
					_serialize(parentNode, deep);
				}
			}
			finally
			{
				ReleaseReaderLock();
			}
		}
	}
}

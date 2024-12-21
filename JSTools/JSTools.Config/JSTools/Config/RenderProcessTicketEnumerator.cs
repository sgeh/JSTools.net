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
using System.Text;

namespace JSTools.Config
{
	/// <summary>
	/// Loops through the IJSToolsRenderHandler specified in the
	/// RenderProcessTicket.
	/// </summary>
	public class RenderProcessTicketEnumerator : IEnumerator
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		/// <summary>
		/// Event is fired before an item is rendered.
		/// </summary>
		public event EventHandler ItemRenderBegin;

		/// <summary>
		/// Event is fired after an item is rendered.
		/// </summary>
		public event EventHandler ItemRenderEnd;

		private Array _renderHandlers;
		private int _itemIndex = -1;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		#region IEnumerator Member

		/// <summary>
		///  <see cref="IEnumerator.Current" />
		/// </summary>
		public object Current
		{
			get
			{ 
				if (_itemIndex < 0 || _itemIndex > _renderHandlers.Length - 1)
					return null;

				return _renderHandlers.GetValue(_itemIndex);
			}
		}

		#endregion

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new RenderProcessTicketEnumerator instance.
		/// </summary>
		/// <param name="renderHandlers">Render handlers which should be enumerated.</param>
		internal RenderProcessTicketEnumerator(Array renderHandlers)
		{
			_renderHandlers = renderHandlers;
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Event is fired before an item is rendered.
		/// </summary>
		/// <param name="e">Event argument which are passed to the event listeners.</param>
		protected virtual void OnItemRenderBegin(EventArgs e)
		{
			if (ItemRenderBegin != null)
				ItemRenderBegin(this, e);
		}

		/// <summary>
		/// Event is fired after an item is rendered.
		/// </summary>
		/// <param name="e">Event argument which are passed to the event listeners.</param>
		protected virtual void OnItemRenderEnd(EventArgs e)
		{
			if (ItemRenderEnd != null)
				ItemRenderEnd(this, e);
		}

		#region IEnumerator Member

		/// <summary>
		///  <see cref="IEnumerator" />
		/// </summary>
		public void Reset()
		{
			_itemIndex = -1;
		}

		/// <summary>
		///  <see cref="IEnumerator" />
		/// </summary>
		public bool MoveNext()
		{
			if (_itemIndex > -1)
				OnItemRenderEnd(EventArgs.Empty);

			if (_itemIndex + 1 == _renderHandlers.Length)
				return false;

			OnItemRenderBegin(EventArgs.Empty);

			++_itemIndex;
			return true;
		}

		#endregion
	}
}

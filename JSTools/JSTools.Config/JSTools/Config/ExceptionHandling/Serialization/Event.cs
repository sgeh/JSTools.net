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
using System.Xml.Serialization;

namespace JSTools.Config.ExceptionHandling.Serialization
{
	/// <summary>
	/// Represents the "event" node specified in the configuration.
	/// </summary>
	[XmlType("event", Namespace=JSExceptionHandlerFactory.NAMESPACE)]
	public class Event
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private bool _log = false;
		private bool _error = false;
		private bool _warn = false;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// "log" attribute of the node.
		/// </summary>
		[XmlAttribute("log")]
		public bool Log
		{
			get { return _log; }
			set { _log = value; }
		}

		/// <summary>
		/// "error" attribute of the node.
		/// </summary>
		[XmlAttribute("error")]
		public bool Error
		{
			get { return _error; }
			set { _error = value; }
		}

		/// <summary>
		/// "warn" attribute of the node.
		/// </summary>
		[XmlAttribute("warn")]
		public bool Warn
		{
			get { return _warn; }
			set { _warn = value; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new Event instance.
		/// </summary>
		public Event()
		{
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------
	}
}

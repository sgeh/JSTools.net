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
	/// Represents the "exceptionHandling" node specified in the configuration.
	/// </summary>
	[XmlRoot(Namespace=JSExceptionHandlerFactory.NAMESPACE)]
	[XmlType("exceptionHandling", Namespace=JSExceptionHandlerFactory.NAMESPACE)]
	public class ExceptionHandling
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private string _requires = string.Empty;
		private string _errorProvider = string.Empty;
		private ErrorHandling _errorHandling = ErrorHandling.None;
		private Event _event = null;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// "requires" attribute of the node.
		/// </summary>
		[XmlAttribute("requires")]
		public string Requires
		{
			get { return _requires; }
			set { _requires = value; }
		}

		/// <summary>
		/// "errorProvider" attribute of the node.
		/// </summary>
		[XmlAttribute("errorProvider")]
		public string ErrorProvider
		{
			get { return _errorProvider; }
			set { _errorProvider = value; }
		}

		/// <summary>
		/// "errorHandling" attribute of the node.
		/// </summary>
		[XmlAttribute("errorHandling")]
		public ErrorHandling ErrorHandling
		{
			get { return _errorHandling; }
			set { _errorHandling = value; }
		}

		/// <summary>
		/// "event" element of the node.
		/// </summary>
		[XmlElement("event")]
		public Event Event
		{
			get { return _event; }
			set { _event = value; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new ExceptionHandling instance.
		/// </summary>
		public ExceptionHandling()
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

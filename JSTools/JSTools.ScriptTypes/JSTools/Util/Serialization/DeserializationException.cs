/*
 * JSTools.ScriptTypes.dll / JSTools.net - A framework for JavaScript/ASP.NET applications.
 * Copyright (C) 2005  Silvan Gehrig
 *
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
 *
 * Author:
 *  Silvan Gehrig
 */

using System;
using System.Collections;
using System.Reflection;
using System.Text;

using JSTools.ScriptTypes;
using JSTools.Parser.Cruncher;
using JSTools.Parser.Cruncher.Nodes;

namespace JSTools.Util.Serialization
{
	/// <summary>
	/// This exception is thrown if an error has occured during the
	/// deserialization of a given script object string.
	/// </summary>
	public class DeserializationException : Exception
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private SyntaxException _scriptSyntaxException = null;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the inner syntax exception instance, which contains the
		/// exception reason.
		/// </summary>
		public SyntaxException ScriptSyntaxException
		{
			get { return _scriptSyntaxException; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new DeserializationException instance.
		/// </summary>
		internal DeserializationException(string message) : base(message)
		{
		}

		/// <summary>
		/// Creates a new DeserializationException instance.
		/// </summary>
		internal DeserializationException(string message, SyntaxException innerException) : base(message, innerException)
		{
			 _scriptSyntaxException = innerException;
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------
	}
}

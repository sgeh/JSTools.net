/*
 * JSTools.Web.dll / JSTools.net - A framework for JavaScript/ASP.NET applications.
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
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace JSTools.Web.UI.Controls
{
	/// <summary>
	/// This exception will be thrown, if the optimization procedure fails.
	/// </summary>
	public class ScriptOptimizationException : Exception
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new ScriptOptimizationException instance.
		/// </summary>
		/// <param name="message">Message to throw.</param>
		/// <param name="inner">Inner exception, which has contains the reason about this exception.</param>
		public ScriptOptimizationException(string message, Exception inner) : base(message, inner)
		{
		}

		/// <summary>
		/// Creates a new ScriptOptimizationException instance.
		/// </summary>
		/// <param name="message">Message to throw.</param>
		public ScriptOptimizationException(string message) : base(message)
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

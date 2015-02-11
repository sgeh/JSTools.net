/*
 * JSTools.Parser.dll / JSTools.net - A framework for JavaScript/ASP.NET applications.
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

namespace JSTools.Parser
{
	/// <summary>
	/// Represents an item which is used to parse a string. This may be
	/// a scope parser or a parse item. This elements are also used
	/// by the ParserContext.
	/// </summary>
	public interface IParserContextItem
	{
		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Returns the parent token parser.
		/// </summary>
		ParserContext Context
		{
			get;
			set;
		}

		/// <summary>
		/// Returns the name of this parse item.
		/// </summary>
		string Name
		{
			get;
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------
	}
}

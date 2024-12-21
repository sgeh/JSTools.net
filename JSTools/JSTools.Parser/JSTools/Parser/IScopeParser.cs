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
using System.Collections;
using System.Collections.Specialized;

namespace JSTools.Parser
{
	/// <summary>
	/// Represents the parser of a code scope. Each parser is bound to a
	/// context, which can be used to parse the child instances. You may
	/// implement different scope parsers for the different code scopes to
	/// parse. The scopes can be chained with the <c>Parse(INode, IScopeParser)</c>
	/// method.
	/// </summary>
	public interface IScopeParser : IParserContextItem, ICloneable
	{
		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Returns true if the specified string could not be parsed and the
		/// parser raised an exception.
		/// </summary>
		bool HasError
		{
			get;
		}

		/// <summary>
		/// Determines if the parser should continue parsing if a syntax
		/// error occurs. If this flag is set to true, the parser will
		/// throw an error. Otherwise the parser ignores the error and aborts
		/// parsing.
		/// </summary>
		bool ThrowErrors
		{
			get;
			set;
		}

		/// <summary>
		/// Returns the names of the associated parse items.
		/// </summary>
		string[] ParseItems
		{
			get;
		}

		/// <summary>
		/// Returns the current line number.
		/// </summary>
		int LineNumber
		{
			get;
		}

		/// <summary>
		/// Returns the current line offset.
		/// </summary>
		int LineOffset
		{
			get;
		}

		/// <summary>
		/// Returns the offset absolute to the string start.
		/// </summary>
		int AbsOffset
		{
			get;
		}

		/// <summary>
		/// Returns the string instance which should be parsed.
		/// </summary>
		string ParseString
		{
			get;
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Parses the specified item.
		/// </summary>
		/// <param name="parent">Current INode instance.</param>
		/// <param name="toParse">String, you'd like to parse.</param>
		/// <returns>Returns the number of parsed characters.</returns>
		int Parse(INode parent, string toParse);

		/// <summary>
		/// Parses the child scope specified by the try parse method.
		/// </summary>
		/// <returns>Returns the number of parsed characters.</returns>
		int ParseToEnd();

		/// <summary>
		/// Tries to parse the specified child scope.
		/// </summary>
		/// <param name="parent">Current INode instance.</param>
		/// <param name="parentScope">Parent scope which contains the required information to parse.</param>
		/// <returns>Returns true if the current scope can parse the child parse.</returns>
		bool TryParse(INode parent, IScopeParser parentScope);
	}
}

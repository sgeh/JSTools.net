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
	/// Descripts an item, which is used to parse the specified string.
	/// 
	/// <para>
	/// After the function Begin() has returned true, the CreateNode() method
	/// will be called to create the node tree.
	/// </para>
	/// 
	/// <para>
	/// For example, a for-loop statement can be specified with this interface.
	/// </para>
	/// </summary>
	public interface IParseItem : IParserContextItem
	{
		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Returns true, if the ending character can not be a start character
		/// of another IParseItem instance. The parser will abort parsing of the
		/// current character and continue with the next. The method will be called
		/// after End() has returned true.
		/// </summary>
		/// <example>
		/// The following string should be parsed:
		/// 
		/// <para>
		/// <c>{hello}</c>
		/// </para>
		/// 
		/// <para>
		/// The parse item begins with <c>{</c> and ends with <c>}</c>. 
		/// This means that <c>}</c> is the last character of the item (the
		/// <c>bool End()</c> method returns true) which should not be
		/// parsed by another parse item. Then the IsAbsoluteEnd should
		/// return true.
		/// </para>
		/// </example>
		bool IsAbsoluteEnd
		{
			get;
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Checks whether the current item is active or not.
		/// </summary>
		/// <returns>
		/// Returns the created node.
		/// </returns>
		/// <param name="scope">IScopeParser which parses the string.</param>
		/// <param name="parentNode">Parent node instance.</param>
		/// <param name="parseString">String, which should be parsed.</param>
		/// <param name="absOffsetBegin">Absolute begin offset.</param>
		/// <param name="lineOffsetBegin">Offset of the start line.</param>
		/// <param name="lineNumberBegin">Line number, at which this instance begins.</param>
		INode Begin(IScopeParser scope, INode parentNode, string parseString, int absOffsetBegin, int lineOffsetBegin, int lineNumberBegin);

		/// <summary>
		/// Checks whether the current item is inactive.
		/// </summary>
		/// <returns>
		/// Returns true, if the current item is not anctive anymore.
		/// </returns>
		/// <param name="scope">IScopeParser which parses the string.</param>
		/// <param name="parentNode">Current node instance.</param>
		/// <param name="parseString">String, which should be parsed.</param>
		/// <param name="index">Long value, which represents the current scan index.</param>
		/// <param name="length">Length (number of parsed characters) of the current parse item.</param>
		bool End(IScopeParser scope, INode parentNode, string parseString, int index, int length);
	}
}

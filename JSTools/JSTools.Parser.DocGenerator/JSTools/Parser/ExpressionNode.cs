/*
 * JSTools.Parser.DocGenerator.dll / JSTools.net - A framework for JavaScript/ASP.NET applications.
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
using System.Text;

namespace JSTools.Parser
{
	/// <summary>
	/// Represents the comment node, which contains xml comment.
	/// </summary>
	public class ExpressionNode : DefaultNode
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
		/// Creates a new ExpressionNode object.
		/// </summary>
		/// <param name="parentNode">Specifies the parent node instance.</param>
		/// <param name="parseItem">Corresponding parse item instance.</param>
		/// <param name="globalCode">Whole code to parse.</param>
		/// <param name="absOffsetBegin">Absolute begin offset.</param>
		/// <param name="lineOffsetBegin">Offset of the start line.</param>
		/// <param name="lineNumberBegin">Line number, at which this instance begins.</param>
		/// <exception cref="ArgumentException">Invalid parse item specified.</exception>
		public ExpressionNode(
			IParseItem parseItem,
			INode parentNode,
			string globalCode,
			int absOffsetBegin,
			int lineOffsetBegin,
			int lineNumberBegin) : base(parentNode, globalCode, absOffsetBegin, lineOffsetBegin, lineNumberBegin)
		{
		}

		/// <summary>
		/// Creates a new ExpressionNode object.
		/// </summary>
		/// <param name="parentNode">Specifies the parent node instance.</param>
		/// <param name="parseItem">Corresponding parse item instance.</param>
		/// <param name="globalCode">Whole code to parse.</param>
		/// <param name="absOffsetBegin">Absolute begin offset.</param>
		/// <param name="lineOffsetBegin">Offset of the start line.</param>
		/// <param name="lineNumberBegin">Line number, at which this instance begins.</param>
		/// <param name="codeLength">Absolute length of the node.</param>
		/// <exception cref="ArgumentException">Invalid parse item specified.</exception>
		public ExpressionNode(
			IParseItem parseItem,
			INode parentNode,
			string globalCode,
			int absOffsetBegin,
			int lineOffsetBegin,
			int lineNumberBegin,
			int codeLength) : base(parseItem, parentNode, globalCode, absOffsetBegin, lineOffsetBegin, lineNumberBegin)
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

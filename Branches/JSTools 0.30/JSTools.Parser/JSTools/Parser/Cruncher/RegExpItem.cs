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

namespace JSTools.Parser.Cruncher
{
//	// Internet Explorer 4.x+ does not execute more than 4KByte data on the same line
//	private	const	int		MAX_COLUMNS_PER_LINE		= 4048;
//
//	// Unix line breaks will be used for the workaround
//	private	const	char	LINE_BREAK_CHAR				= 0x0A;

	/// <summary>
	/// Represents a javascript regular expression section (e.g. /industr(?:y|ies)/gi).
	/// </summary>
	public class RegExpItem : IParseItem
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		public	const		string	ITEM_NAME			= "Regular Expression Item";

		private const		bool	IS_ABSOLUTE_END		= true;
		private	const		char	REG_EXP_BOUNDS		= '/';
		private	readonly	char[]	REG_EXP_BEGIN_CHARS	= {
															  ';',
															  '=',
															  '(',
															  ','
														  };

		private	TokenParser			_parent				= null;


		/// <summary>
		/// Returns the name of this parse item.
		/// </summary>
		public string ItemName
		{
			get { return ITEM_NAME; }
		}


		/// <summary>
		/// Returns true, if the ending character can not be a start character
		/// of another IParseItem instance. The parser will abort parsing of the
		/// current character and continue with the next. The method will be called
		/// after End() has returned true.
		/// </summary>
		public bool IsAbsoluteEnd
		{
			get { return IS_ABSOLUTE_END; }
		}


		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new RegExpItem instance.
		/// </summary>
		public RegExpItem()
		{
		}


		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Returns a new INode object. It is used to create the node tree.
		/// </summary>
		public INode CreateNode()
		{
			return new CrunchNode(ItemName);
		}


		/// <summary>
		/// Returns true, if this parse item begins at the specified index.
		/// </summary>
		/// <param name="parentNode">Parent node instance.</param>
		/// <param name="parseString">String, which should be parsed.</param>
		/// <param name="index">Long value, which represents the current scan index.</param>
		public bool Begin(INode parentNode, string parseString, int index)
		{
			return (parseString[index] == REG_EXP_BOUNDS && IsRegExpBegin(parseString, index));
		}


		/// <summary>
		/// Returns true, if the given char ends this parse item.
		/// </summary>
		/// <param name="parentNode">Parent node instance.</param>
		/// <param name="parseString">String, which should be parsed.</param>
		/// <param name="index">Long value, which represents the current scan index.</param>
		public bool End(INode parentNode, string parseString, int index)
		{
			return (parseString[index] == REG_EXP_BOUNDS && !Cruncher.IsEscapedChar(parseString, index));
		}


		/// <summary>
		/// Sets the parent parser, which can be used to create recursive calls.
		/// </summary>
		/// <param name="parent">The parent parser.</param>
		public void SetParser(TokenParser parent)
		{
			_parent = parent;
		}


		/// <summary>
		/// Checks the given index for a valid regexp start.
		/// </summary>
		/// <param name="parseString">String, which should be parsed.</param>
		/// <param name="index">Long value, which represents the current scan index.</param>
		/// <returns>Returns true, if the given index represents a valid regexp start.</returns>
		private bool IsRegExpBegin(string parseString, int index)
		{
			// if the script file is to short
			if (parseString.Length == 1)
				return false;

			// if the regexp is on the script file begin
			if (index == 1)
				return true;

			int i = index - 1;

			for ( ; i > -1 && Char.IsWhiteSpace(parseString, i); --i)
			{
				;
			}

			// the regexp is the first expression
			if (i == -1)
				return true;

			// if we have a valid regexp start character
			foreach (char item in REG_EXP_BEGIN_CHARS)
			{
				if (parseString[i] == item)
					return true;
			}
			return false;
		}
	}
}

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

using JSTools.Parser.ParseItems;

namespace JSTools.Parser
{
	/// <summary>
	/// 
	/// </summary>
	internal class ScriptParser
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		internal static readonly ScriptParser Instance = new ScriptParser();

		// global items
		private const string GLOBAL_SINGLE_LINE_COMMENT = "GLOBAL_SINGLE_LINE_COMMENT";
		private const string GLOBAL_MULTILINE_COMMENT = "GLOBAL_MULTILINE_COMMENT";
		private const string GLOBAL_SINGLE_QUOTE_STRING = "GLOBAL_SINGLE_QUOTE_STRING";
		private const string GLOBAL_DOUBLE_QUOTE_STRING = "GLOBAL_DOUBLE_QUOTE_STRING";
		private const string GLOBAL_REGEX = "GLOBAL_REGEX";

		private const string CALL_GLOBAL = "CALL_GLOBAL";
		private const string CALL_COMMENT_SCRIPT = "CALL_COMMENT_SCRIPT";
		private const string CALL_COMMENT = "CALL_COMMENT";
		private const string CALL_SCRIPT = "CALL_SCRIPT";
		private const string CALL_BLOCK = "CALL_BLOCK";
		private const string CALL_FUNCTION = "CALL_FUNCTION";
		private const string CALL_EXPRESSION = "CALL_EXPRESSION";
		private const string CALL_ASSIGNMENT = "CALL_ASSIGNMENT";

		// global scope
		private const string GLOBAL_SCOPE = "GLOBAL_SCOPE";
		private const string GLOBAL_SCOPE_DEFAULT = "GLOBAL_SCOPE_DEFAULT";

		// comment script scope
		private const string COMMENT_SCRIPT_SCOPE = "COMMENT_SCRIPT_SCOPE";
		private const string COMMENT_SCRIPT_SCOPE_COMMENT = "COMMENT_SCRIPT_SCOPE_COMMENT";

		// comment scope
		private const string COMMENT_SCOPE = "COMMENT_SCOPE";
		private const string COMMENT_SCOPE_COMMENT = "COMMENT_SCOPE_COMMENT";
		private const string COMMENT_SCOPE_WHITE_SPACE = "COMMENT_SCOPE_WHITE_SPACE";

		// script scope
		private const string SCRIPT_SCOPE = "SCRIPT_SCOPE";

		// function scope
		private const string FUNCTION_SCOPE = "FUNCTION_SCOPE";
		private const string FUNCTION_SCOPE_KEYWORD = "FUNCTION_SCOPE_KEYWORD";
		private const string FUNCTION_SCOPE_WHITE_SPACE = "FUNCTION_SCOPE_WHITE_SPACE";
		private const string FUNCTION_SCOPE_NAME = "FUNCTION_SCOPE_NAME";
		private const string FUNCTION_SCOPE_ANY = "FUNCTION_SCOPE_ANY";

		// block scope
		private const string BLOCK_SCOPE = "BLOCK_SCOPE";
		private const string BLOCK_SCOPE_BRACKET_BEGIN = "BLOCK_SCOPE_BRACKET_BEGIN";
		private const string BLOCK_SCOPE_BRACKET_END = "BLOCK_SCOPE_BRACKET_END";

		// expression scope
		private const string EXPRESSION_SCOPE = "EXPRESSION_SCOPE";
		private const string EXPRESSION_SINGLE_LINE_COMMENT = "EXPRESSION_SINGLE_LINE_COMMENT";
		private const string EXPRESSION_SCOPE_SINGLE_QUOTE_STRING = "EXPRESSION_SCOPE_SINGLE_QUOTE_STRING";
		private const string EXPRESSION_SCOPE_DOUBLE_QUOTE_STRING = "EXPRESSION_SCOPE_DOUBLE_QUOTE_STRING";
		private const string EXPRESSION_SCOPE_REGEX = "EXPRESSION_SCOPE_REGEX";
		private const string EXPRESSION_SCOPE_CODE = "EXPRESSION_SCOPE_CODE";

		// assignment scope
		private const string ASSIGNMENT_SCOPE = "ASSIGNMENT_SCOPE";
		private const string ASSIGNMENT_SCOPE_KEYWORD = "ASSIGNMENT_SCOPE_KEYWORD";
		private const string ASSIGNMENT_SCOPE_TERM = "ASSIGNMENT_SCOPE_TERM";
		private const string ASSIGNMENT_SCOPE_WHITE_SPACE = "ASSIGNMENT_SCOPE_WHITE_SPACE";
		private const string ASSIGNMENT_SCOPE_ASSIGNMENT = "ASSIGNMENT_SCOPE_ASSIGNMENT";

		private ParserContext _parserContext = null;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new ScriptParser instance. This class follows the
		/// singleton pattern.
		/// </summary>
		private ScriptParser()
		{
			// define parse items
			IParseItem[] parseItems = new IParseItem[]
				{
					// global items
					new SingleLineCommentItem(GLOBAL_SINGLE_LINE_COMMENT),
					new BoundaryItem(GLOBAL_MULTILINE_COMMENT, "/*", "*/"),
					new StringItem(GLOBAL_SINGLE_QUOTE_STRING, '\''),
					new StringItem(GLOBAL_DOUBLE_QUOTE_STRING, '"'),
					new RegexItem(GLOBAL_REGEX),

					new ScopeParserItem(CALL_GLOBAL, GLOBAL_SCOPE),
					new CommentScriptScopeItem(CALL_COMMENT_SCRIPT, COMMENT_SCRIPT_SCOPE),
					new ScopeParserItem(CALL_COMMENT, COMMENT_SCOPE, false),
					new ScopeParserItem(CALL_SCRIPT, SCRIPT_SCOPE, false),
					new ScopeParserItem(CALL_BLOCK, BLOCK_SCOPE),
					new ScopeParserItem(CALL_FUNCTION, FUNCTION_SCOPE),
					new ScopeParserItem(CALL_EXPRESSION, EXPRESSION_SCOPE, false),
					new ScopeParserItem(CALL_ASSIGNMENT, ASSIGNMENT_SCOPE),

					// global scope items
					new DefaultParseItem(GLOBAL_SCOPE_DEFAULT),

					// comment script scope items
					new CommentItem(COMMENT_SCRIPT_SCOPE_COMMENT),

					// comment scope items
					new CommentItem(COMMENT_SCOPE_COMMENT),
					new WhiteSpaceItem(COMMENT_SCOPE_WHITE_SPACE),

					// function scope items
					new KeywordTokenItem(FUNCTION_SCOPE_KEYWORD, "function"),
					new WhiteSpaceItem(FUNCTION_SCOPE_WHITE_SPACE),
					new ExpressionItem(FUNCTION_SCOPE_NAME),
					new ExcludeItem(FUNCTION_SCOPE_ANY, '{'),

					// block scope items
					new CharTokenItem(BLOCK_SCOPE_BRACKET_BEGIN, '{'),
					new CharTokenItem(BLOCK_SCOPE_BRACKET_END, '}'),

					// expression scope items
					new SingleLineCommentItem(EXPRESSION_SINGLE_LINE_COMMENT),
					new StringItem(EXPRESSION_SCOPE_SINGLE_QUOTE_STRING, '\''),
					new StringItem(EXPRESSION_SCOPE_DOUBLE_QUOTE_STRING, '"'),
					new RegexItem(EXPRESSION_SCOPE_REGEX),
					new ExcludeItem(EXPRESSION_SCOPE_CODE, '}'),
					
					// assignment scope items
					new KeywordTokenItem(ASSIGNMENT_SCOPE_KEYWORD, "var"),
					new ExpressionItem(ASSIGNMENT_SCOPE_TERM),
					new WhiteSpaceItem(ASSIGNMENT_SCOPE_WHITE_SPACE),
					new CharTokenItem(ASSIGNMENT_SCOPE_ASSIGNMENT, '=')
				};

			// create context, assign global scope
			_parserContext = new ParserContext(parseItems, GetGlobalScope());
			_parserContext.RegisterScope(GetCommentScriptScope());
			_parserContext.RegisterScope(GetCommentScope());
			_parserContext.RegisterScope(GetScriptScope());
			_parserContext.RegisterScope(GetFunctionScope());
			_parserContext.RegisterScope(GetBlockScope());
			_parserContext.RegisterScope(GetExpressionScope());
			_parserContext.RegisterScope(GetAssignmentScope());
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		internal INode ParseScript(string toParse)
		{
			if (toParse != null)
				return _parserContext.Parse(toParse);

			return null;
		}

		private IScopeParser GetGlobalScope()
		{
			// create global scope
			AlternativeScopeParser globalScope = new AlternativeScopeParser(
				GLOBAL_SCOPE,
				new string[]
				{
					GLOBAL_MULTILINE_COMMENT,
					CALL_COMMENT_SCRIPT,
					GLOBAL_SINGLE_LINE_COMMENT,
					GLOBAL_SINGLE_QUOTE_STRING,
					GLOBAL_DOUBLE_QUOTE_STRING,
					GLOBAL_REGEX,
					GLOBAL_SCOPE_DEFAULT
				} );

			globalScope.ItemRequired = false;
			globalScope.DefaultItem = GLOBAL_SCOPE_DEFAULT;
			return globalScope;
		}

		private IScopeParser GetCommentScriptScope()
		{
			// create comment script scope
			StrictScopeParser commentScriptScope = new StrictScopeParser(
				COMMENT_SCRIPT_SCOPE,
				new StrictScopeParser.StrictScopeParseItem[]
				{
					new StrictScopeParser.StrictScopeParseItem(COMMENT_SCRIPT_SCOPE_COMMENT, 1, 1),
					new StrictScopeParser.StrictScopeParseItem(CALL_COMMENT, 0, -1),
					new StrictScopeParser.StrictScopeParseItem(CALL_SCRIPT, 1, 1)
				} );

			return commentScriptScope;
		}

		private IScopeParser GetCommentScope()
		{
			// create comment scope
			AlternativeScopeParser commentScope = new AlternativeScopeParser(
				COMMENT_SCOPE,
				new string[]
				{
					COMMENT_SCOPE_COMMENT,
					COMMENT_SCOPE_WHITE_SPACE
				} );

			commentScope.ItemRequired = true;
			commentScope.MaxParseItemCount = 1;
			return commentScope;
		}

		private IScopeParser GetScriptScope()
		{
			// create script scope
			AlternativeScopeParser scriptScope = new AlternativeScopeParser(
				SCRIPT_SCOPE,
				new string[]
				{
					CALL_FUNCTION,
					CALL_ASSIGNMENT
				} );

			scriptScope.ItemRequired = true;
			scriptScope.MaxParseItemCount = 1;
			return scriptScope;
		}

		private IScopeParser GetFunctionScope()
		{
			// create function scope
			StrictScopeParser functionScope = new StrictScopeParser(
				FUNCTION_SCOPE,
				new StrictScopeParser.StrictScopeParseItem[]
				{
					new StrictScopeParser.StrictScopeParseItem(FUNCTION_SCOPE_KEYWORD, 1, 1),
					new StrictScopeParser.StrictScopeParseItem(FUNCTION_SCOPE_WHITE_SPACE, 0, -1),
					new StrictScopeParser.StrictScopeParseItem(FUNCTION_SCOPE_NAME, 0, 1),
					new StrictScopeParser.StrictScopeParseItem(FUNCTION_SCOPE_ANY, 0, -1),
					new StrictScopeParser.StrictScopeParseItem(CALL_BLOCK, 1, 1),
				} );

			return functionScope;
		}

		private IScopeParser GetBlockScope()
		{
			// create block scope
			StrictScopeParser blockScope = new StrictScopeParser(
				BLOCK_SCOPE,
				new StrictScopeParser.StrictScopeParseItem[]
				{
					new StrictScopeParser.StrictScopeParseItem(BLOCK_SCOPE_BRACKET_BEGIN, 1, 1),
					new StrictScopeParser.StrictScopeParseItem(CALL_EXPRESSION, 0, -1),
					new StrictScopeParser.StrictScopeParseItem(BLOCK_SCOPE_BRACKET_END, 1, 1),
				} );

			return blockScope;
		}

		private IScopeParser GetExpressionScope()
		{
			// create expression scope
			AlternativeScopeParser expressionScope = new AlternativeScopeParser(
				EXPRESSION_SCOPE,
				new string[]
				{
					GLOBAL_MULTILINE_COMMENT,
					CALL_COMMENT_SCRIPT,
					GLOBAL_SINGLE_LINE_COMMENT,
					GLOBAL_SINGLE_QUOTE_STRING,
					GLOBAL_DOUBLE_QUOTE_STRING,
					GLOBAL_REGEX,
					CALL_BLOCK,
					EXPRESSION_SCOPE_CODE
				} );

			expressionScope.ItemRequired = true;
			expressionScope.MaxParseItemCount = 1;
			return expressionScope;
		}

		private IScopeParser GetAssignmentScope()
		{
			// create assignment scope
			StrictScopeParser assignmentScope = new StrictScopeParser(
				ASSIGNMENT_SCOPE,
				new StrictScopeParser.StrictScopeParseItem[]
				{
					new StrictScopeParser.StrictScopeParseItem(ASSIGNMENT_SCOPE_KEYWORD, 0, 1),
					new StrictScopeParser.StrictScopeParseItem(ASSIGNMENT_SCOPE_WHITE_SPACE, 0, -1),
					new StrictScopeParser.StrictScopeParseItem(ASSIGNMENT_SCOPE_TERM, 1, 1),
					new StrictScopeParser.StrictScopeParseItem(ASSIGNMENT_SCOPE_WHITE_SPACE, 0, -1),
					new StrictScopeParser.StrictScopeParseItem(ASSIGNMENT_SCOPE_ASSIGNMENT, 1, 1),
					new StrictScopeParser.StrictScopeParseItem(ASSIGNMENT_SCOPE_WHITE_SPACE, 0, -1),
					new StrictScopeParser.StrictScopeParseItem(CALL_FUNCTION, 0, 1)
				} );

			return assignmentScope;
		}
	}
}

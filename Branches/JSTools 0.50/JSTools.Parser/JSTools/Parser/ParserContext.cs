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

namespace JSTools.Parser
{
	/// <summary>
	/// Manages the scopes and parse items, which will be used to parse the
	/// given string.
	/// </summary>
	public class ParserContext
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private Hashtable _parseItems = null;
		private Hashtable _scopes = null;

		private string _globalScope = null;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Returns the name of the global scope instance, which is used to
		/// parse the global context.
		/// </summary>
		public string GlobalScope
		{
			get { return _globalScope; }
			set
			{
				if (value != null && _scopes[value] != null)
					_globalScope = value;
			}
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new ParserContext instance.
		/// </summary>
		/// <param name="parseItems">Items, which are used to parse the string.</param>
		/// <param name="globalScopeParser">Global scope parser instance.</param>
		/// <exception cref="ArgumentException">Could not parse a string without parse items.</exception>
		/// <exception cref="ParseItemException">The specified parse item has returned an error.</exception>
		public ParserContext(IParseItem[] parseItems, IScopeParser globalScopeParser)
		{
			if (parseItems == null || parseItems.Length == 0)
				throw new ArgumentException("Could not parse a string without parse items.", "parseItems");

			_parseItems = new Hashtable(parseItems.Length);
			_scopes = new Hashtable();

			InitParseItems(parseItems);

			if (globalScopeParser != null)
			{
				RegisterScope(globalScopeParser);
				GlobalScope = globalScopeParser.Name;
			}
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Checks, if the given char is a valid line break.
		/// </summary>
		/// <param name="toCheck">Character to check.</param>
		/// <returns>Returns true, if the given character represents a valid line break.</returns>
		public static bool IsLineBreak(char toCheck)
		{
			return (toCheck == '\n' || toCheck == '\r' || toCheck == 0x02028 || toCheck == 0x02029);
		}

		/// <summary>
		/// Checks, if the given char is a valid windows line break.
		/// </summary>
		/// <param name="toCheck">String, which should be checked.</param>
		/// <param name="index">Index to check.</param>
		/// <returns>Returns true, if the given character represents a valid windows line break.</returns>
		public static bool IsWinLineBreak(string toCheck, int index)
		{
			return (toCheck.Length > index + 1 && toCheck[index] == 0x0D && toCheck[index + 1] == 0x0A);
		}

		/// <summary>
		/// Returns the parse item with the specified name.
		/// </summary>
		/// <param name="itemName">Name of the item, which should be returned.</param>
		/// <returns>Returns a parse item instance, or an null reference, if nothing was found.</returns>
		public IParseItem GetItem(string itemName)
		{
			return (IParseItem)_parseItems[itemName];
		}

		/// <summary>
		/// Searches for a scope with the given name.
		/// </summary>
		/// <param name="scopeName">IScopeParser to get.</param>
		/// <returns>Returns the scope with the given name or null, if it does not exist.</returns>
		public IScopeParser GetScope(string scopeName)
		{
			if (_scopes.ContainsKey(scopeName))
				return (IScopeParser)((IScopeParser)_scopes[scopeName]).Clone();

			return null;
		}

		/// <summary>
		/// Creates a new scope instance.
		/// </summary>
		/// <param name="parserToRegister">IScopeParser parser to register.</param>
		/// <exception cref="ArgumentException">Invalid scope specified.</exception>
		/// <exception cref="ArgumentException">A scope with the specified name was already given.</exception>
		public void RegisterScope(IScopeParser parserToRegister)
		{
			if (parserToRegister == null || (parserToRegister.Context != null && parserToRegister.Context != this))
				throw new ArgumentException("Invalid scope specified.", "parserToRegister");

			if (_scopes.Contains(parserToRegister.Name))
				throw new ArgumentException("A scope with the specified name was already given.", "name");

			foreach (string parseItemName in parserToRegister.ParseItems)
			{
				if (parseItemName == null || parseItemName.Length == 0)
					throw new ArgumentException("Invalid parser name specified.", "name");

				if (_parseItems[parseItemName] == null)
					throw new InvalidOperationException("There is no parse item with the given name registered.");
			}

			parserToRegister.Context = this;
			_scopes[parserToRegister.Name] = parserToRegister;
		}

		/// <summary>
		/// Parses the specified string with the given scope. Creates a new GlobalNode instance, in
		/// which the parsed items will be stored.
		/// </summary>
		/// <param name="toParse">String, you'd like to parse.</param>
		/// <param name="scopeName">Name of the scope, which should parse the string.</param>
		/// <returns>Returns a INode, which represents the parsed string.</returns>
		/// <exception cref="ParseItemException">Error while executing the parsing operations.</exception>
		/// <exception cref="ArgumentNullException">The given scope contains a null reference.</exception>
		/// <exception cref="ArgumentException">The specified scope has an invalid parent parser.</exception>
		/// <exception cref="ArgumentNullException">The given parent node contains a null reference.</exception>
		/// <exception cref="ArgumentException">The given scope name is invalid.</exception>
		public INode ParseScope(string toParse, string scopeName)
		{
			if (scopeName == null)
				throw new ArgumentNullException("scopeName", "The given scope name contains a null reference.");

			IScopeParser scope = GetScope(scopeName);

			if (scope == null)
				throw new ArgumentException("The given scope name is invalid.");

			return ParseScope(toParse, scope);
		}

		/// <summary>
		/// Parses the specified string. Creates a new GlobalNode instance, in
		/// which the parsed items will be stored.
		/// </summary>
		/// <param name="toParse">String, you'd like to parse.</param>
		/// <returns>Returns a INode, which represents the parsed string.</returns>
		/// <exception cref="InvalidOperationException">There is no default parser specified.</exception>
		/// <exception cref="ParseItemException">Error while executing the parsing operations.</exception>
		public INode Parse(string toParse)
		{
			if (toParse == null || toParse.Length == 0)
				return null;

			if (_globalScope == null)
				throw new InvalidOperationException("There is no default scope parser specified.");

			return ParseScope(toParse, _globalScope);
		}

		private INode ParseScope(string toParse, IScopeParser scopeParser)
		{
			if (toParse == null || toParse.Length == 0)
				return null;

			INode global = new DefaultNode(null, toParse, 0, 0, 1);

			try
			{
				// parse string and terminate global node
				global.CodeLength = scopeParser.Parse(global, toParse);
				global.LineOffsetEnd = scopeParser.LineOffset;
				global.LineNumberEnd = scopeParser.LineNumber;
			}
			catch (ParseItemException parseItemException)
			{
				throw parseItemException;
			}
			catch (Exception e)
			{
				throw new ParseItemException(
					"Error while executing the parsing operations.",
					e,
					"Internal Parser Error",
					scopeParser.LineNumber,
					scopeParser.LineOffset,
					toParse);
			}
			return global;
		}

		private void InitParseItems(IParseItem[] parseItems)
		{
			foreach (IParseItem item in parseItems)
			{
				if (item != null)
				{
					try
					{
						item.Context = this;
						_parseItems.Add(item.Name, item);
					}
					catch (Exception e)
					{
						throw new ParseItemException("The specified parse item has returned an error.", e);
					}
				}
			}
		}
	}
}

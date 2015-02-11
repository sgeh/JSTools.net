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
using System.Collections;

namespace JSTools.Parser
{
	/// <summary>
	/// Manages the scopes and parse items, which will be used to parse the given string.
	/// </summary>
	public class TokenParser
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private	const	string	GLOBAL_SCOPE	= "Global Parser Scope";

		private Hashtable		_parseItems		= null;
		private	Hashtable		_scopes			= null;

		private	Scope			_globalScope	= null;


		/// <summary>
		/// Returns the global scope instance, which is used to parse the global context.
		/// </summary>
		public Scope GlobalScope
		{
			get { return _globalScope; }
		}


		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new Parser instance.
		/// </summary>
		/// <param name="parseItems">Items, which are used to parse the string.</param>
		/// <exception cref="ArgumentException">Could not parse a string without parse items.</exception>
		/// <exception cref="ParseItemException">The specified parse item has returned an error.</exception>
		public TokenParser(IParseItem[] parseItems)
		{
			if (parseItems == null || parseItems.Length == 0)
				throw new ArgumentException("Could not parse a string without parse items!", "parseItems");

			_parseItems = new Hashtable(parseItems.Length);
			_scopes		= new Hashtable();
			_scopes.Add(GLOBAL_SCOPE, null);

			InitParseItems(parseItems);
		}


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
		/// Sets the global scope instance, which is used to parse the global context.
		/// </summary>
		/// <param name="parseItems">Parse item names, which are used to parse the global code.</param>
		/// <exception cref="ArgumentNullException">The given scope contains a null reference.</exception>
		/// <exception cref="ArgumentException">Could not find a registered scope with the given name.</exception>
		public void SetGlobalScope(string[] parseItems)
		{
			if (parseItems == null)
				throw new ArgumentNullException("The parse item array contains a null reference!", "parseItems");

			_scopes[GLOBAL_SCOPE] = parseItems;
			_globalScope = GetScopeInstance(GLOBAL_SCOPE);
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
		/// <param name="name">Scope to get.</param>
		/// <returns>Returns the scope with the given name or null, if it does not exist.</returns>
		public Scope GetScopeInstance(string scopeName)
		{
			if (_scopes.ContainsKey(scopeName))
			{
				return new Scope(scopeName, (string[])_scopes[scopeName], this);
			}
			return null;
		}


		/// <summary>
		/// Creates a new scope instance.
		/// </summary>
		/// <param name="name">Name of the new scope instance.</param>
		/// <param name="parseItems">Parse item, which should be used to parse the scope.</param>
		/// <returns>Returns the registered scope.</returns>
		/// <exception cref="ArgumentNullException">The given string array contains a null reference.</exception>
		/// <exception cref="ArgumentException">Invalid scope name specified.</exception>
		/// <exception cref="ArgumentException">A scope with the specified name was already given.</exception>
		/// <exception cref="InvalidOperationException">There is no parse item with the given name registered.</exception>
		public void RegisterScope(string name, string[] parseItems)
		{
			if (parseItems == null)
				throw new ArgumentNullException("parseItmes", "The given string array contains a null reference!");

			if (name == null || name == String.Empty)
				throw new ArgumentException("Invalid scope name specified.", "name");

			if (_scopes.Contains(name))
				throw new ArgumentException("A scope with the specified name was already given.", "name");

			foreach (string parseItemName in parseItems)
			{
				if (parseItemName == null || parseItemName == String.Empty)
					throw new ArgumentException("Invalid parser name specified.", "name");

				if (_parseItems[parseItemName] == null)
					throw new InvalidOperationException("There is no parse item with the given name registered!");
			}
			_scopes.Add(name, parseItems);
		}


		/// <summary>
		/// Parses the specified string with the given scope. Creates a new GlobalNode instance, in
		/// which the parsed items will be stored.
		/// </summary>
		/// <param name="toParse">String, you'd like to parse.</param>
		/// <param name="scopeToParse">Name of the scope, which should parse the string.</param>
		/// <returns>Returns a INode, which represents the parsed string.</returns>
		/// <exception cref="ParseItemException">Error while executing the parsing operations.</exception>
		/// <exception cref="ArgumentNullException">The given scope contains a null reference.</exception>
		/// <exception cref="ArgumentException">The specified scope has an invalid parent parser.</exception>
		/// <exception cref="ArgumentNullException">The given parent node contains a null reference.</exception>
		/// <exception cref="ArgumentException">The given scope name is invalid.</exception>
		public INode ParseScope(string toParse, string scopeName)
		{
			if (scopeName == null)
				throw new ArgumentNullException("scopeName", "The given scope name contains a null reference!");

			Scope scope = GetScopeInstance(scopeName);

			if (scope == null)
				throw new ArgumentException("The given scope name is invalid!");

			return ParseScope(toParse, scope);
		}


		/// <summary>
		/// Parses the specified string with the given scope. Creates a new GlobalNode instance, in
		/// which the parsed items will be stored.
		/// </summary>
		/// <param name="toParse">String, you'd like to parse.</param>
		/// <param name="scopeToParse">Name of the scope, which should parse the string.</param>
		/// <returns>Returns a INode, which represents the parsed string.</returns>
		/// <exception cref="ParseItemException">Error while executing the parsing operations.</exception>
		/// <exception cref="ArgumentNullException">The given scope contains a null reference.</exception>
		/// <exception cref="ArgumentException">The specified scope has an invalid parent parser.</exception>
		/// <exception cref="ArgumentNullException">The given parent node contains a null reference.</exception>
		public INode ParseScope(string toParse, Scope scopeToParse)
		{
			if (toParse == null || toParse == string.Empty)
				return null;

			INode global = new GlobalNode();
			global.SetUpBegin(toParse, 0, 0, 0);

			ParseScope(toParse, scopeToParse, global);

			// if there are children, get ending values from the last child
			if (global.Children.Length > 0)
			{
				INode child = global.Children[global.Children.Length - 1];
				global.SetUpEnd(child.LineOffsetEnd, child.LineNumberEnd);
			}
			return global;
		}


		/// <summary>
		/// Parses the specified string with the given scope. Fills the children
		/// into the given parent node instance.
		/// </summary>
		/// <param name="toParse">String, you'd like to parse.</param>
		/// <param name="scopeToParse">Scope, which should parse the string.</param>
		/// <param name="parentNode">The parsing result will be filled into this node.</param>
		/// <exception cref="ParseItemException">Error while executing the parsing operations.</exception>
		/// <exception cref="ArgumentNullException">The given scope name contains a null reference.</exception>
		/// <exception cref="ArgumentException">The specified scope has an invalid parent parser.</exception>
		/// <exception cref="ArgumentNullException">The given parent node contains a null reference.</exception>
		/// <exception cref="ArgumentException">The given scope name is invalid.</exception>
		public void ParseScope(string toParse, string scopeName, INode parentNode)
		{
			if (scopeName == null)
				throw new ArgumentNullException("scopeName", "The given scope name contains a null reference!");

			Scope scope = GetScopeInstance(scopeName);

			if (scope == null)
				throw new ArgumentException("The given scope name is invalid!");

			ParseScope(toParse, scope, parentNode);
		}


		/// <summary>
		/// Parses the specified string with the given scope. Fills the children
		/// into the given parent node instance.
		/// </summary>
		/// <param name="toParse">String, you'd like to parse.</param>
		/// <param name="scopeToParse">Scope, which should parse the string.</param>
		/// <param name="parentNode">The parsing result will be filled into this node.</param>
		/// <exception cref="ParseItemException">Error while executing the parsing operations.</exception>
		/// <exception cref="ArgumentNullException">The given scope contains a null reference.</exception>
		/// <exception cref="ArgumentException">The specified scope has an invalid parent parser.</exception>
		/// <exception cref="ArgumentNullException">The given parent node contains a null reference.</exception>
		public void ParseScope(string toParse, Scope scopeToParse, INode parentNode)
		{
			if (toParse == null || toParse == string.Empty)
				return;

			if (scopeToParse == null)
				throw new ArgumentNullException("scopeToParse", "The given scope contains a null reference!");

			if (scopeToParse.Parser != this)
				throw new ArgumentException("The specified scope has an invalid parent parser!", "scopeToParse");

			if (parentNode == null)
				throw new ArgumentNullException("parentNode", "The given parent node contains a null reference!");


			try
			{
				// parse given string
				scopeToParse.Parse(parentNode, toParse, 0);
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
					_globalScope.LineNumber,
					_globalScope.LineOffset,
					toParse);
			}
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
			if (toParse == null || toParse == string.Empty)
				return null;

			if (_globalScope == null)
				throw new InvalidOperationException("There is no default parser specified!");

			return ParseScope(toParse, _globalScope);
		}


		/// <summary>
		/// Initializes the given parse items.
		/// </summary>
		/// <param name="parseItems">Items, which are used to parse the string.</param>
		private void InitParseItems(IParseItem[] parseItems)
		{
			foreach (IParseItem item in parseItems)
			{
				if (item != null)
				{
					try
					{
						item.SetParser(this);
						_parseItems.Add(item.ItemName, item);
					}
					catch (Exception e)
					{
						throw new ParseItemException("The specified parse item has returned an error!", e);
					}
				}
			}
		}
	}
}

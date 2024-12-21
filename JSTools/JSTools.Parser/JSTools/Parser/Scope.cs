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
using System.Collections.Specialized;

namespace JSTools.Parser
{
	/// <summary>
	/// Represents a scope, which should be parsed. Caution, the parse items specified
	/// by the AddParseItem() methods will be called in the given order. Each parser will step
	/// through the string and call the "Begin" method of each parse item.
	/// </summary>
	public class Scope
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private string[] _parseItems = null;
		private string _name = null;
		private TokenParser _parent = null;

		private IParseItem _activeItem = null;
		private INode _activeValue = null;
		private bool _itemRequired = true;

		private IParseItem _defaultItem = null;
		private INode _defaultValue = null;

		private int _lineOffsetBegin = -1;

		private int _lineNumber = -1;
		private int _lineOffset = -1;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Returns the parent token parser.
		/// </summary>
		public TokenParser Parser
		{
			get { return _parent; }
		}

		/// <summary>
		/// Returns the current line number.
		/// </summary>
		public int LineNumber
		{
			get { return _lineNumber; }
		}

		/// <summary>
		/// Returns the current line offset.
		/// </summary>
		public int LineOffset
		{
			get { return _lineOffset; }
		}

		/// <summary>
		/// Returns the current line offset.
		/// </summary>
		public int LineOffsetBegin
		{
			get { return _lineOffsetBegin; }
		}

		/// <summary>
		/// True if the a parse item is required. The parse method will throw an ParseItemException,
		/// if there is no appropriated IParseItem in the current scope. If this exception should
		/// not be thrown, you have to set this flag to false.
		/// </summary>
		public bool ItemRequired
		{
			get { return _itemRequired; }
			set { _itemRequired = value; }
		}

		/// <summary>
		/// Gets/sets the default parse item. It will be used, if no other IParseItem instance is
		/// appropriated to an expression. The Begin() and End() methods of the given parse item will
		/// be never used. Returns a null pointer, if no item was set.
		/// </summary>
		/// <exception cref="ArgumentNullException">The given item name contains a null reference.</exception>
		/// <exception cref="ArgumentException">An item with the specified name was not registered in the parent TokenParser.</exception>
		public string DefaultItem
		{
			get
			{
				return ((_defaultItem != null) ? _defaultItem.ItemName : null);
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException("value", "The given item name contains a null reference!");

                IParseItem item = _parent.GetItem(value);

				if (item == null)
					throw new ArgumentException("An item with the specified name was not registered in the parent TokenParser.", "value");

				_defaultItem = item;
			}
		}

		/// <summary>
		/// Returns the name of this scope.
		/// </summary>
		public string ScopeName
		{
			get { return _name; }
		}

		/// <summary>
		/// Returns the item, which is currently active.
		/// </summary>
		public IParseItem ActiveItem
		{
			get { return _activeItem; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new Scope instance.
		/// </summary>
		/// <param name="name">Name of the current scope instance.</param>
		/// <param name="items">Parse items, which should be used in this scope.</param>
		/// <param name="parent">Parent TokenParser instance.</param>
		/// <exception cref="ArgumentException">Invalid scope name specified.</exception>
		/// <exception cref="ArgumentNullException">The given parse item array contains a null reference.</exception>
		/// <exception cref="ArgumentNullException">The given parent object contains a null reference.</exception>
		public Scope(string name, string[] items, TokenParser parent)
		{
			if (name == null || name.Length == 0)
				throw new ArgumentException("Invalid name specified!", "parseItems");

			if (items == null)
				throw new ArgumentNullException("items", "The given parse item array contains a null reference!");

			if (parent == null)
				throw new ArgumentNullException("parent", "The given parent object contains a null reference!");

			_parseItems = items;
			_parent = parent;
			_name = name;
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
		/// <param name="indexBegin">Index, at which the parsing begins.</param>
		/// <exception cref="ArgumentNullException">The given string contains a null reference.</exception>
		/// <exception cref="ArgumentNullException">The parent node instance contains a null reference.</exception>
		/// <exception cref="ArgumentOutOfRangeException">The begin index can not be higher than the string length.</exception>
		/// <exception cref="ParseItemException">An error has occured while parsing the given string.</exception>
		public void Parse(INode parent, string toParse, int indexBegin)
		{
			if (toParse == null)
				throw new ArgumentNullException("toParse", "The given string contains a null reference!");

			if (parent == null)
				throw new ArgumentNullException("parent", "The parent node instance contains a null reference!");

			if (indexBegin > toParse.Length)
				throw new ArgumentOutOfRangeException("indexBegin", "The begin index can not be higher than the string length!");

			_lineOffsetBegin = indexBegin;
			_activeItem = null;
			_activeValue = null;

			_lineNumber = 0;
			_lineOffset = indexBegin;

			for (int offset = 0; offset < toParse.Length; ++offset)
			{
				ParseIndex(parent, toParse, offset);
				++_lineOffset;
				CheckLineBreak(toParse, offset);
			}
			ParseEnd(toParse.Length);
		}

		/// <summary>
		/// Controls the line break and line offset.
		/// </summary>
		/// <param name="toCheck">String, which should be controlled.</param>
		/// <param name="offset">Index to check.</param>
		private void CheckLineBreak(string toCheck, int offset)
		{
			// avoid mutliple adding of line breaks
			if (TokenParser.IsLineBreak(toCheck[offset]) && !TokenParser.IsWinLineBreak(toCheck, offset))
			{
				++_lineNumber;
				_lineOffset = 0;
			}
		}

		/// <summary>
		/// Parses the specified index.
		/// </summary>
		/// <param name="parent">Current INode instance.</param>
		/// <param name="toParse">String, you'd like to parse.</param>
		/// <param name="index">Current index pointer.</param>
		private void ParseIndex(INode parent, string toParse, int index)
		{
			// if an element is active
			if (_activeItem != null)
			{
				// if the active element has an absolute end, abort parsing of current character
				if (IsAbsoluteEnd(parent, toParse, index))
				{
					return;
				}
			}

			if (_activeItem == null)
			{
				CheckForActiveItem(parent, toParse, index);
			}
		}

		/// <summary>
		/// Checks, if the active node ends, and returns true, if the end
		/// is absolute.
		/// </summary>
		/// <param name="parent">Parent node.</param>
		/// <param name="toParse">String to parse.</param>
		/// <param name="index">Character index.</param>
		/// <returns>Returns true, if the end is absolute, otherwise false.</returns>
		private bool IsAbsoluteEnd(INode parent, string toParse, int index)
		{
			// if the active item does not end
			if (!_activeItem.End(parent, toParse, index))
			{
				// increment code length
				++_activeValue.CodeLength;
				return false;
			}

			if (_activeItem.IsAbsoluteEnd)
			{
				++_activeValue.CodeLength;
				_activeValue.SetUpEnd(_lineOffset, _lineNumber);

				// reset active parse item and active node value
				_activeValue = null;
				_activeItem = null;

				// abort parsing and continue with the next char
				return true;
			}
			else
			{
				_activeValue.SetUpEnd(_lineOffset - 1, _lineNumber);

				// reset active parse item and active node value
				_activeItem = null;
				_activeValue = null;

				return false;
			}
		}

		/// <summary>
		/// Terminates the last active node.
		/// </summary>
		/// <param name="stringLength">Length of the parsed string.</param>
		private void ParseEnd(int stringLength)
		{
			if (_activeValue != null)
			{
				_activeValue.SetUpEnd(_lineOffset, _lineNumber);
				_activeValue = null;
			}
			if (_defaultValue != null)
			{
				_defaultValue.SetUpEnd(_lineOffset, _lineNumber);
				_defaultValue = null;
			}
		}

		/// <summary>
		/// Checks, if the given pointer represents a parse item.
		/// </summary>
		/// <param name="parent">Current INode instance.</param>
		/// <param name="toParse">String, you'd like to parse.</param>
		/// <param name="index">Index of the current pointer.</param>
		private void CheckForActiveItem(INode parent, string toParse, int index)
		{
			foreach (string parseItem in _parseItems)
			{
				IParseItem item = _parent.GetItem(parseItem);

				if (item.Begin(parent, toParse, index))
				{
					// if a default value is active, end it
					if (_defaultValue != null)
					{
						_defaultValue.SetUpEnd(_lineOffset - 1, _lineNumber);
						_defaultValue = null;
					}

					_activeItem = item;
					_activeValue = SetUpBegin(parent, _activeItem, toParse, index);

					// return if an item was found
					return;
				}
			}

			ManageDefaultValue(parent, toParse, index);
		}

		/// <summary>
		/// Manages the default value, if there is no appropriated IParseItem.
		/// </summary>
		/// <param name="parent">Current INode instance.</param>
		/// <param name="toParse">String, you'd like to parse.</param>
		/// <param name="index">Index of the current pointer.</param>
		private void ManageDefaultValue(INode parent, string toParse, int index)
		{
			// throw exception, if there is no appropriated IParseItem instance
			if (_itemRequired)
				throw new ParseItemException("Unexpected token specified!",
					"The given toke can not be parsed, there is no appropriated IParseItem specified.",
					_lineNumber,
					_lineOffset,
					string.Empty);

			// if the default item can be used, we have to create a new default value,
			// if there is no active default value instance
			if (_defaultItem != null)
			{
				if (_defaultValue == null)
				{
					_defaultValue = SetUpBegin(parent, _defaultItem, toParse, index);
				}
				else
				{
					++_defaultValue.CodeLength;
				}
			}
		}

		/// <summary>
		/// Creates a new node with the given parse item.
		/// </summary>
		/// <param name="parentNode">Parent node of the new node.</param>
		/// <param name="activeItem">Item, which creates a new node.</param>
		/// <param name="toParse">String to parse.</param>
		/// <param name="index">Character index.</param>
		/// <returns>Returns the created node.</returns>
		private INode SetUpBegin(INode parentNode, IParseItem activeItem, string toParse, int index)
		{
			INode newNode = activeItem.CreateNode();

			if (newNode == null)
				throw new ParseItemException("Item '" + activeItem.ItemName + "' has returned a null reference. Could not work with null reference nodes!");

			newNode.SetUpBegin(toParse, index, _lineOffset, _lineNumber);
			++newNode.CodeLength;

			parentNode.AddChild(newNode);
			return newNode;
		}
	}
}

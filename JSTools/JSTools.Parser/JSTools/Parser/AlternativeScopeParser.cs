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
	/// Represents an alternative scope parser.
	/// <see cref="JSTools.Parser.IScopeParser"/>
	/// </summary>
	/// <remarks>
	/// The parse items will be used in the given order. The parser will
	/// step through the string to parse and try to detect the appropriated
	/// parse item. If no parse item is found, the default item is returned,
	/// if specified.
	/// 
	/// <para>
	///  Thus the items will be parsed in the
	/// </para>
	/// </remarks>
	public class AlternativeScopeParser : AScopeParser
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private bool _itemRequired = true;
		private string _defaultItem = null;
		private string[] _parseItems = null;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

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
		/// <exception cref="ArgumentException">The given default item name is empty.</exception>
		public string DefaultItem
		{
			get
			{
				return (_defaultItem != null) ? _defaultItem : null;
			}
			set
			{
				if (value == null || value.Length == 0)
					throw new ArgumentException("The given default item name is empty.", "value");

				_defaultItem = value;
			}
		}

		/// <summary>
		/// Returns the names of the associated parse items.
		/// </summary>
		public override string[] ParseItems
		{
			get { return _parseItems; }
		}

		private IParseItem DefaultItemInstance
		{
			get
			{
				if (_defaultItem == null)
					return null;

				IParseItem item = Context.GetItem(_defaultItem);

				if (item == null)
				{
					throw new InvalidOperationException(string.Format(
						"An item with the specified name '{0}' was not registered in the parent ParserContext.",
						_defaultItem ));
				}
				return item;
			}
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new AlternativeScopeParser instance.
		/// </summary>
		/// <param name="name">Name of the current scope instance.</param>
		/// <param name="items">Parse items, which should be used in this scope.</param>
		/// <exception cref="ArgumentException">Invalid scope name specified.</exception>
		/// <exception cref="ArgumentNullException">The given parse item array is empty.</exception>
		public AlternativeScopeParser(string name, string[] items) : base(name)
		{
			if (items == null || items.Length == 0)
				throw new ArgumentNullException("items", "The given parse item array is empty.");

			_parseItems = items;
		}

		/// <summary>
		/// Creates a new AlternativeScopeParser instance.
		/// </summary>
		protected AlternativeScopeParser()
		{
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------
		
		/// <summary>
		/// Creates a new clone of this instance.
		/// </summary>
		/// <returns>Returns the cloned instance object.</returns>
		protected override AScopeParser CreateClone()
		{
			AlternativeScopeParser clone = new AlternativeScopeParser();
			clone._defaultItem = _defaultItem;
			clone._itemRequired = _itemRequired;
			clone._parseItems = _parseItems;
			return clone;
		}

		/// <summary>
		/// Parses the code and returns the next node instance.
		/// </summary>
		/// <param name="parent">Parent node instance.</param>
		/// <returns>Returns the found and created node.</returns>
		protected override INode GetNextNode(INode parent)
		{
			INode foundNode = InitNextItem(parent);

			if (foundNode == null)
				foundNode = ManageDefaultItem(parent);

			return foundNode;
		}

		private INode InitNextItem(INode parent)
		{
			foreach (string parseItem in ParseItems)
			{
				IParseItem item = Context.GetItem(parseItem);

				if (item == null)
					throw new InvalidOperationException(string.Format(
						"The specified item '{0}' is not registered in the parser context.",
						parseItem ));

				// if an item is not required, the default parse item is used when
				// entering the ManageDefaultItem() method only
				//  -> it's the default item which always begins.
				if (_itemRequired || parseItem != DefaultItem)
				{
					INode foundNode = item.Begin(this, parent, ParseString, AbsOffset, LineOffset, LineNumber);

					if (foundNode != null)
						return foundNode;
				}
			}
			return null;
		}

		private INode ManageDefaultItem(INode parent)
		{
			// return a null reference so the parser will throw a syntax error
			if (_itemRequired || DefaultItemInstance == null)
				return null;
			else
				return DefaultItemInstance.Begin(this, parent, ParseString, AbsOffset, LineOffset, LineNumber);
		}
	}
}

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
	/// Represents the exact scope parser.
	/// <see cref="JSTools.Parser.IScopeParser"/>
	/// </summary>
	/// <remarks>
	/// The parse items will be used in the given order. Each parser will
	/// step through the string and call the "Begin" method of each parse
	/// item.
	/// </remarks>
	public class StrictScopeParser : AScopeParser
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private StrictScopeParseItem[] _parseItems = null;

		private int _currentItem = -1;
		private int _currentItemMultiplicity = 0;
		private int _lastFoundItem = 0;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Returns the names of the associated parse items.
		/// </summary>
		public override string[] ParseItems
		{
			get
			{
				string[] parseItems = new string[_parseItems.Length];
				
				for (int i = 0; i < _parseItems.Length; ++i)
				{
					parseItems[i] = _parseItems[i].ParseItemName;
				}
				return parseItems;
			}
		}

		/// <summary>
		/// Returns true if parsing of the current scope should be
		/// aborted.
		/// </summary>
		protected override bool AbortParsing
		{
			get { return (base.AbortParsing || !CurrentIndexValid); }
		}

		/// <summary>
		/// Returns true if all required items are parsed.
		/// </summary>
		protected override bool ParsingFinished
		{
			get { return (LastRequiredItem == _parseItems.Length); }
		}

		private int LastRequiredItem
		{
			get
			{
				int item = _lastFoundItem;

				do
				{
					++item;
				}
				// check if there are items, which have a multiplicity begin greater than 0
				while (item < _parseItems.Length && _parseItems[item].MultiplicityBegin == 0);

				return item;
			}
		}

		private bool CurrentIndexValid
		{
			get { return (_currentItem > -1 && _currentItem < _parseItems.Length); }
		}

		private bool CanUseCurrentItem
		{
			get
			{
				return (CurrentIndexValid
					&& (CurrentParseItem.MultiplicityEnd == -1
					|| _currentItemMultiplicity < CurrentParseItem.MultiplicityEnd));
			}
		}

		private StrictScopeParseItem CurrentParseItem
		{
			get { return _parseItems[_currentItem]; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new StrictScopeParser instance.
		/// </summary>
		/// <param name="name">Name of the current scope instance.</param>
		/// <param name="items">Parse items, which should be used in this scope.</param>
		/// <exception cref="ArgumentException">Invalid scope name specified.</exception>
		/// <exception cref="ArgumentNullException">The given parse item array is empty.</exception>
		public StrictScopeParser(string name, StrictScopeParseItem[] items) : base(name)
		{
			if (items == null || items.Length == 0)
				throw new ArgumentNullException("items", "The given parse item array is empty.");

			_parseItems = items;
		}

		/// <summary>
		/// Creates a new StrictScopeParser instance.
		/// </summary>
		protected StrictScopeParser()
		{
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		/// <summary>
		/// This method is called just when parsing of the current scope
		/// was aborted or finished.
		/// </summary>
		protected override void OnParsingFinished()
		{
			if (!ParsingFinished)
			{
				ThrowError(
					string.Format(
						"Error while parsing the given string. Item '{0}' expected.",
						_parseItems[LastRequiredItem].ParseItemName ),
					"Syntax error" );
			}
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------
		
		/// <summary>
		/// Creates a new clone of this instance.
		/// </summary>
		/// <returns>Returns the cloned instance object.</returns>
		protected override AScopeParser CreateClone()
		{
			StrictScopeParser scopeParser = new StrictScopeParser();
			scopeParser._parseItems = _parseItems;
			return scopeParser;
		}

		/// <summary>
		/// Parses the code and returns the next node instance.
		/// </summary>
		/// <param name="parent">Parent node instance.</param>
		/// <returns>Returns the found and created node.</returns>
		protected override INode GetNextNode(INode parent)
		{
			INode foundNode = null;

			// first try to get active item
			if (CanUseCurrentItem && (foundNode = TryParseCurrentItem(parent)) != null)
				return foundNode;

			do
			{
				MoveToNextItem();

				// try to get next active item
				if (CanUseCurrentItem && (foundNode = TryParseCurrentItem(parent)) != null)
					return foundNode;
			}
			while (CurrentIndexValid && CurrentParseItem.MultiplicityBegin == 0);

			// return a null reference so the parser will throw a syntax error
			return null;
		}

		private INode TryParseCurrentItem(INode parent)
		{
			IParseItem item = Context.GetItem(CurrentParseItem.ParseItemName);

			if (item == null)
			{
				throw new InvalidOperationException(string.Format(
					"The specified item '{0}' is not registered in the parser context.",
					CurrentParseItem.ParseItemName ));
			}

			INode foundNode = item.Begin(this, parent, ParseString, AbsOffset, LineOffset, LineNumber);

			if (foundNode != null)
			{
				_currentItemMultiplicity++;
				_lastFoundItem = _currentItem;
				return foundNode;
			}
			return null;
		}

		private void MoveToNextItem()
		{
			_currentItemMultiplicity = 0;
			++_currentItem;
		}


		//--------------------------------------------------------------------
		// Nested Classes
		//--------------------------------------------------------------------

		/// <summary>
		/// Accociates a parse item with a multiplicity.
		/// </summary>
		public class StrictScopeParseItem
		{
			//----------------------------------------------------------------
			// Declarations
			//----------------------------------------------------------------
		
			private string _parseItemName = null;
			private int _multiplicityBegin = 1;
			private int _multiplicityEnd = 1;

			//----------------------------------------------------------------
			// Properties
			//----------------------------------------------------------------

			/// <summary>
			/// Gets the name of the parse item.
			/// </summary>
			public string ParseItemName
			{
				get { return _parseItemName; }
			}

			/// <summary>
			/// Gets the matches at least. 0 means that the item must not be
			/// matched.
			/// </summary>
			public int MultiplicityBegin
			{
				get { return _multiplicityBegin; }
			}

			/// <summary>
			/// Gets the number of matches. -1 means that the item can be
			/// matched {n} times.
			/// </summary>
			public int MultiplicityEnd
			{
				get { return _multiplicityEnd; }
			}

			//----------------------------------------------------------------
			// Constructors / Destructor
			//----------------------------------------------------------------

			/// <summary>
			/// Creates a new StrictScopeItem instance.
			/// </summary>
			/// <param name="parseItemName">Name of the parse item to add.</param>
			/// <param name="multiplicityBegin">Matches at least 'multiplicityBegin' times. <see cref="MultiplicityBegin"/> </param>
			/// <param name="multiplicityEnd">Matches not more than 'multiplicityEnd' times. <see cref="MultiplicityEnd"/> </param>
			/// <exception cref="ArgumentException">The given parse item name is invalid.</exception>
			public StrictScopeParseItem(string parseItemName, int multiplicityBegin, int multiplicityEnd) : this(parseItemName)
			{
				_multiplicityBegin = (multiplicityBegin < 0) ? 0 : multiplicityBegin;
				_multiplicityEnd = (multiplicityEnd < 0) ? -1 : multiplicityEnd;
			}

			/// <summary>
			/// Creates a new StrictScopeItem instance. This item will match exactly one time.
			/// </summary>
			/// <param name="parseItemName">Name of the parse item to add.</param>
			/// <exception cref="ArgumentException">The given parse item name is invalid.</exception>
			public StrictScopeParseItem(string parseItemName)
			{
				if (parseItemName == null || parseItemName.Length == 0)
					throw new ArgumentException("The given parse item name is invalid.", "parseItemName");

				_parseItemName = parseItemName;
			}

			//----------------------------------------------------------------
			// Events
			//----------------------------------------------------------------

			//----------------------------------------------------------------
			// Methods
			//----------------------------------------------------------------
		}
	}
}

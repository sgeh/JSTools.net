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
	/// Represents the base class for the implemented scope parsers. If
	/// you don't like to use the default implementation, you should
	/// inherit from IScopeParser instead.
	/// </summary>
	/// <remarks>
	/// Contains the base parsing algorithm which steps throught each
	/// character and tries to detect the appropriated parse item.
	/// </remarks>
	public abstract class AScopeParser : IScopeParser
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private string _name = null;
		private ParserContext _context = null;
		private string _toParse = string.Empty;

		private IParseItem _activeItem = null;
		private INode _activeValue = null;
		private INode _parent = null;

		private int _offsetStart = -1;
		private int _absOffset = -1;
		private int _absLineOffset = -1;
		private int _lineNumber = -1;
		private int _parseItemCount = 0;

		private int _maxParseItemCount = -1;
		private bool _throwErrors = true;
		private bool _isTryMode = false;
		private bool _abortParsing = false;
		private bool _hasError = false;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Returns true if the specified string could not be parsed and the
		/// parser raised an exception.
		/// </summary>
		public bool HasError
		{
			get { return _hasError; }
		}

		/// <summary>
		/// Determines if the parser should continue parsing if a syntax
		/// error occurs. If this flag is set to true, the parser will
		/// throw an error. Otherwise the parser ignores the error and aborts
		/// parsing.
		/// </summary>
		public bool ThrowErrors
		{
			get { return _throwErrors; }
			set { _throwErrors = value; }
		}

		/// <summary>
		/// Specifies the number of parse items to parse. The scope will
		/// end parsing in case the given count is reached. Default value
		/// is -1 which means that the current scope will parse unlimited
		/// parse items.
		/// </summary>
		public int MaxParseItemCount
		{
			get { return _maxParseItemCount; }
			set { _maxParseItemCount = (value < 1) ? -1 : value; }
		}

		/// <summary>
		/// Returns the parent token parser.
		/// </summary>
		public ParserContext Context
		{
			get { return _context; }
			set { _context = value; }
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
			get { return _absLineOffset; }
		}

		/// <summary>
		/// Returns the offset absolute to the string start.
		/// </summary>
		public int AbsOffset
		{
			get { return _absOffset; }
		}

		/// <summary>
		/// Returns the name of this scope.
		/// </summary>
		public string Name
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

		/// <summary>
		/// Returns the string instance which should be parsed.
		/// </summary>
		public string ParseString
		{
			get { return _toParse; }
		}

		/// <summary>
		/// Returns the names of the associated parse items.
		/// </summary>
		public abstract string[] ParseItems
		{
			get;
		}

		/// <summary>
		/// Returns true if parsing of the current scope should be
		/// aborted. Important: Call base property.
		/// </summary>
		protected virtual bool AbortParsing
		{
			get { return _abortParsing; }
		}

		/// <summary>
		/// Returns true if all required items are parsed.
		/// </summary>
		protected virtual bool ParsingFinished
		{
			get { return false; }
		}

		private bool ParseItemCountValid
		{
			get { return (_maxParseItemCount == -1 || _maxParseItemCount > _parseItemCount); }
		}

		private bool StringToParseValid
		{
			get { return (_toParse != null && _toParse.Length > 0); }
		}

		private bool IsOffsetValid
		{
			get { return (_absOffset < _toParse.Length); }
		}

		private int ParsedCharaterCount
		{
			get
			{
				if (_activeValue != null && _activeValue.ParseItem != null)
					return (_activeValue.OffsetEnd - _offsetStart) + 1;
				else
					return 0;
			}
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new AScopeParser instance.
		/// </summary>
		/// <param name="name">Name of the current scope instance.</param>
		/// <exception cref="ArgumentException">Invalid scope name specified.</exception>
		public AScopeParser(string name)
		{
			if (name == null || name.Length == 0)
				throw new ArgumentException("Invalid name specified.", "name");

			_name = name;
		}

		/// <summary>
		/// Creates a new AScopeParser instance. This constructor should
		/// be used in conjunction with the clone functionality.
		/// </summary>
		protected AScopeParser()
		{
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		/// <summary>
		/// This method is called just when parsing of the current scope
		/// was aborted or finished.
		/// </summary>
		protected virtual void OnParsingFinished()
		{
		}

		/// <summary>
		/// This method is called just when parsing of the current scope
		/// was started.
		/// </summary>
		protected virtual void OnParsingStarted()
		{
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		#region ICloneable Member

		/// <summary>
		///  <see cref="ICloneable.Clone" />
		/// </summary>
		/// <returns>
		///  <see cref="ICloneable.Clone" />
		/// </returns>
		public object Clone()
		{
			AScopeParser clone = CreateClone();
			clone._maxParseItemCount = _maxParseItemCount;
			clone._name = _name;
			clone._context = _context;
			clone._throwErrors = _throwErrors;
			return clone;
		}

		/// <summary>
		/// Creates a new clone of this instance.
		/// </summary>
		/// <returns>Returns the cloned instance object.</returns>
		protected abstract AScopeParser CreateClone();

		#endregion

		/// <summary>
		/// Parses the specified item.
		/// </summary>
		/// <param name="parent">Current INode instance.</param>
		/// <param name="toParse">String, you'd like to parse.</param>
		/// <exception cref="ArgumentNullException">The given string contains a null reference.</exception>
		/// <exception cref="ArgumentNullException">The parent node instance contains a null reference.</exception>
		/// <exception cref="ArgumentOutOfRangeException">The begin index can not be higher than the string length.</exception>
		/// <exception cref="ParseItemException">An error has occured while parsing the given string.</exception>
		public int Parse(INode parent, string toParse)
		{
			if (toParse == null)
				throw new ArgumentNullException("toParse", "The given string contains a null reference.");

			if (parent == null)
				throw new ArgumentNullException("parent", "The parent node instance contains a null reference.");

			_activeItem = null;
			_activeValue = null;

			_offsetStart = 0;
			_absOffset = 0;
			_lineNumber = 1;
			_absLineOffset = 0;
			_toParse = toParse;
			_parent = parent;

			_isTryMode = false;
			_abortParsing = false;

			if (StringToParseValid)
			{
				OnParsingStarted();
				Parse();
			}
			return ParsedCharaterCount;
		}

		/// <summary>
		/// Parses the current scope to end.
		/// </summary>
		public int ParseToEnd()
		{
			if (!_isTryMode || HasError)
				throw new InvalidOperationException("Could not parse the current scope. You have to call TryParse first.");

			_isTryMode = false;
			Parse();
			return ParsedCharaterCount;
		}

		/// <summary>
		/// Tries to parse the specified child scope.
		/// </summary>
		/// <param name="parent">Current INode instance.</param>
		/// <param name="parentScope">Parent scope which contains the required information to parse.</param>
		/// <returns>Returns true if the current scope can parse the child parse.</returns>
		/// <exception cref="ArgumentNullException">The parent node instance contains a null reference.</exception>
		/// <exception cref="ArgumentNullException">The parent scope instance contains a null reference.</exception>
		public bool TryParse(INode parent, IScopeParser parentScope)
		{
			if (parent == null)
				throw new ArgumentNullException("parent", "The parent node instance contains a null reference.");

			if (parentScope == null)
				throw new ArgumentNullException("parentScope", "The parent scope instance contains a null reference.");

			_offsetStart = parentScope.AbsOffset;
			_absOffset = parentScope.AbsOffset;
			_lineNumber = parentScope.LineNumber;
			_absLineOffset = parentScope.LineOffset;
			_toParse = parentScope.ParseString;
			_parent = parent;

			_isTryMode = true;
			_abortParsing = false;

			if (StringToParseValid)
			{
				OnParsingStarted();
				ParseIndex();
				MoveForward(1);
				return !HasError;
			}
			return false;
		}

		/// <summary>
		/// Parses the code and returns the next node instance.
		/// </summary>
		/// <param name="parent">Parent node instance.</param>
		/// <returns>Returns the found and created node.</returns>
		protected abstract INode GetNextNode(INode parent);

		/// <summary>
		/// This method should be called if an error has occured.
		/// </summary>
		/// <param name="errorMessage">Error message to throw.</param>
		/// <param name="errorName">Name of the error. (e.g. Syntax error)</param>
		protected void ThrowError(string errorMessage, string errorName)
		{
			_abortParsing = true;
			_hasError = true;

			if (_throwErrors && !_isTryMode)
			{
				throw new ParseItemException(
					errorMessage,
					errorName,
					_lineNumber,
					_absLineOffset,
					_toParse );
			}
		}

		private void Parse()
		{
			while (IsOffsetValid && !AbortParsing)
			{
				ParseIndex();
				MoveForward(1);
			}
			ParseEnd();
		}

		private void MoveForward(int numberOfChars)
		{
			for (int i = 0; i < numberOfChars; ++i)
			{
				++_absOffset;
				++_absLineOffset;

				if (IsOffsetValid)
					CheckLineBreak();
				else
					break;
			}
		}

		private void CheckLineBreak()
		{
			// avoid mutliple adding of line breaks
			if (ParserContext.IsLineBreak(_toParse[_absOffset]) && !ParserContext.IsWinLineBreak(_toParse, _absOffset))
			{
				++_lineNumber;
				_absLineOffset = 0;
			}
		}

		private void ParseIndex()
		{
			// if an element is active and if the active element has an
			// absolute end, abort parsing of current character
			if (IsActiveItemAbsoluteEnd())
				return;

			// if there is no active item try to parse the current index
			if (_activeItem == null && IsParseItemCountValid())
				InitNextNode();
		}

		private void InitNextNode()
		{
			#region Try to get next node instance.

			INode activeValue = GetNextNode(_parent);

			if (activeValue == null)
			{
				if (!IsParsingFinished())
					ThrowError("No parse item found.", "Syntax Error");

				return;
			}

			#endregion

			#region Try to get parse item of retrieved node.

			_parseItemCount++;
			_activeValue = activeValue;
			_activeItem = activeValue.ParseItem;

			if (_activeItem == null)
			{
				// invalid item begin specified
				ThrowError("The node must be assigned to a parse item instance.", "Internal Parser Error");
				return;
			}

			#endregion

			// clean up nodes, if an absolute node was given
			if (_activeValue.CodeLength - (_absOffset - _activeValue.OffsetBegin) > 0)
			{
				MoveForward(_activeValue.CodeLength - 1);
				SetUpEnd(true);
			}
		}

		private bool IsParseItemCountValid()
		{
			if (!ParseItemCountValid)
				_abortParsing = true;

			return ParseItemCountValid;
		}

		private bool IsParsingFinished()
		{
			if (!_isTryMode)
			{
				if (ParsingFinished)
					_abortParsing = true;

				return ParsingFinished;
			}
			return false;
		}

		private bool IsActiveItemAbsoluteEnd()
		{
			if (_activeItem != null && _activeItem.End(this, _parent, _toParse, _absOffset, (_absOffset - _activeValue.OffsetBegin)))
			{
				bool isAbsEnd = _activeItem.IsAbsoluteEnd;
				SetUpEnd(isAbsEnd);
				return isAbsEnd;
			}
			return false;
		}

		private void SetUpEnd(bool isAbsolute)
		{
			_activeValue.OffsetEnd = (isAbsolute) ? _absOffset : _absOffset - 1;
			_activeValue.LineOffsetEnd = (isAbsolute || _absLineOffset == 0) ? _absLineOffset : _absLineOffset - 1;
			_activeValue.LineNumberEnd = _lineNumber;

			// reset active parse item
			_activeItem = null;
		}

		private void ParseEnd()
		{
			if (_activeItem != null)
				SetUpEnd(_activeItem.IsAbsoluteEnd);

			OnParsingFinished();
		}
	}
}

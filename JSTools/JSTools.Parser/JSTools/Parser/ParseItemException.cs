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
using System.Text;

namespace JSTools.Parser
{
	/// <summary>
	/// Will be thrown, if the a IParseItem has thrown an error or returned
	/// an invalid value.
	/// </summary>
	public class ParseItemException : Exception
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const string PARSER_ERROR = "Parser Error";

		private int _lineNumber = -1;
		private int _columnNumber = -1;
		private string _code = string.Empty;
		private string _errorMessage = string.Empty;
		private string _errorName = string.Empty;
		
		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Returns the line number, which contains the error.
		/// </summary>
		public int LineNumber
		{
			get { return _lineNumber; }
		}

		/// <summary>
		/// Returns the column number, which has occured the error.
		/// </summary>
		public int ColumnNumber
		{
			get { return _columnNumber; }
		}

		/// <summary>
		/// Returns the error code.
		/// </summary>
		public string Code
		{
			get { return _code; }
		}

		/// <summary>
		/// Returns the error message.
		/// </summary>
		public string ErrorMessage
		{
			get { return _errorMessage; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Create new ParseItemException instance.
		/// </summary>
		/// <param name="message">Message to throw.</param>
		/// <param name="inner">Inner exception.</param>
		public ParseItemException(string message, Exception inner) : base(message, inner)
		{
		}

		/// <summary>
		/// Create new ParseItemException instance.
		/// </summary>
		/// <param name="message">Message to throw.</param>
		public ParseItemException(string message) : base(message)
		{
		}

		/// <summary>
		/// Create new ParseItemException instance.
		/// </summary>
		/// <param name="message">Error message (e.g. Missing Statement).</param>
		/// <param name="errorName">Name of the error (e.g. Syntax Error)</param>
		/// <param name="lineNumber">Error line number.</param>
		/// <param name="lineOffset">Error line column.</param>
		/// <param name="errorCode">Code, in which this error has occured.</param>
		public ParseItemException(string message, string errorName, int lineNumber, int lineOffset, string errorCode) : base(PARSER_ERROR)
		{
			_lineNumber = lineNumber;
			_columnNumber = lineOffset;
			_code = errorCode;
			_errorMessage = message;
			_errorName = errorName;
		}

		/// <summary>
		/// Create new ParseItemException instance.
		/// </summary>
		/// <param name="message">Error message (e.g. Missing Statement).</param>
		/// <param name="inner">Inner exception.</param>
		/// <param name="errorName">Name of the error (e.g. Syntax Error)</param>
		/// <param name="lineNumber">Error line number.</param>
		/// <param name="lineOffset">Error line column.</param>
		/// <param name="errorCode">Code, in which this error has occured.</param>
		public ParseItemException(string message, Exception inner, string errorName, int lineNumber, int lineOffset, string errorCode) : base(message, inner)
		{
			_lineNumber = lineNumber;
			_columnNumber = lineOffset;
			_code = errorCode;
			_errorMessage = message;
			_errorName = errorName;
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Returns a summary of this parser exception.
		/// </summary>
		public override string ToString()
		{
			if (_errorName == null || _errorName.Length == 0
				|| _errorMessage == null || _errorMessage.Length == 0)
				return base.ToString();

			StringBuilder builder = new StringBuilder();
			builder.Append(_errorName);
			builder.Append(" (");

			builder.Append(_errorMessage);
			builder.Append("; ");

			if (_lineNumber > 0)
			{
				builder.Append("line ");
				builder.Append(_lineNumber);
			}

			builder.Append(")");
			return builder.ToString();
		}
	}
}

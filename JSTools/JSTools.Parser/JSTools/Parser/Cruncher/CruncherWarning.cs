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
using System.Text;

namespace JSTools.Parser.Cruncher
{
	/// <summary>
	/// Summary description for CruncherWarning.
	/// </summary>
	public class CruncherWarning
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private int				_lineNumber		= -1;
		private int				_columnNumber	= -1;
		private	string			_code			= String.Empty;
		private	string			_wholeCode		= String.Empty;
		private	string			_message		= null;


		/// <summary>
		/// Returns the warning message.
		/// </summary>
		public string Message
		{
			get { return _message; }
		}


		/// <summary>
		/// Returns the column number.
		/// </summary>
		public int LineNumber
		{
			get { return _lineNumber; }
		}


		/// <summary>
		/// Returns the column number.
		/// </summary>
		public int ColumnNumber
		{
			get { return _columnNumber; }
		}


		/// <summary>
		/// Returns a part of the code.
		/// </summary>
		public string Code
		{
			get { return _code; }
		}


		/// <summary>
		/// Returns the whole code message.
		/// </summary>
		public string WholeCode
		{
			get { return _wholeCode; }
		}


		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new CruncherWarning instance.
		/// </summary>
		/// <param name="message">Warning message.</param>
		/// <param name="wholeCode">Whole script code.</param>
		/// <param name="code">Warning code.</param>
		/// <param name="lineNumber">Line number.</param>
		/// <param name="lineOffset">Line offset.</param>
		/// <exception cref="ArgumentNullException">The given message contains a null reference.</exception>
		public CruncherWarning(string message, string wholeCode, string code, int lineOffset, int lineNumber)
		{
			if (message == null)
				throw new ArgumentNullException("message", "The given message contains a null reference!");

			_message		= message;
			_lineNumber		= lineNumber;
			_columnNumber	= lineOffset;
			_wholeCode		= (wholeCode != null) ? wholeCode : string.Empty;
			_code			= (code != null) ? code : string.Empty;
		}


		/// <summary>
		/// Creates a new CruncherWarning instance.
		/// </summary>
		/// <param name="message">Warning message.</param>
		/// <param name="lineNumber">Line number.</param>
		/// <param name="lineOffset">Line offset.</param>
		/// <param name="wholeCode">Whole script code.</param>
		/// <exception cref="ArgumentNullException">The given message contains a null reference.</exception>
		public CruncherWarning(string message, string wholeCode, int lineOffset, int lineNumber)
		{
			if (message == null)
				throw new ArgumentNullException("message", "The given message contains a null reference!");

			_message		= message;
			_lineNumber		= lineNumber;
			_columnNumber	= lineOffset;
			_wholeCode		= (wholeCode != null) ? wholeCode : string.Empty;
			_code			= (wholeCode != null) ? GetWarningCode(wholeCode) : string.Empty;
		}


		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new string, which represents the current warning.
		/// </summary>
		/// <returns>Returns the created warning string.</returns>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(_message);
			builder.Append(" ( Line: ");
			builder.Append(_lineNumber);
			builder.Append("; Column: ");
			builder.Append(_columnNumber);
			builder.Append("; Source: '");
			builder.Append(_code);
			builder.Append("')");
			return builder.ToString();
		}


		/// <summary>
		/// Returns the warning code.
		/// </summary>
		/// <param name="wholeCode">String to get the warning code.</param>
		/// <returns>Returns the generated string.</returns>
		private string GetWarningCode(string wholeCode)
		{
			return wholeCode.Substring(GetStartIndex(wholeCode), GetEndIndex(wholeCode) - GetStartIndex(wholeCode));
		}


		/// <summary>
		/// Returns the index of the first separator before the given warning index.
		/// </summary>
		/// <param name="wholeCode"></param>
		/// <returns></returns>
		private int GetStartIndex(string wholeCode)
		{
			int startIndex = 0;

			for (int index = _columnNumber - 1; index > -1; --index)
			{
				if (Cruncher.IsSeparator(wholeCode[index]))
				{
					return index;
				}
			}
			return startIndex;
		}


		/// <summary>
		/// Returns the index of the first separator after the given warning index.
		/// </summary>
		/// <param name="wholeCode"></param>
		/// <returns></returns>
		private int GetEndIndex(string wholeCode)
		{
			int endIndex = wholeCode.Length;

			// The error is reported on different indexes. We can avoid that,
			// if we add + 3 to the error index (_columnNumber).
			for (int index = _columnNumber + 3; index < wholeCode.Length - 1; ++index)
			{
				if (Cruncher.IsSeparator(wholeCode[index]))
				{
					return index + 1;
				}
			}
			return endIndex;
		}
	}
}

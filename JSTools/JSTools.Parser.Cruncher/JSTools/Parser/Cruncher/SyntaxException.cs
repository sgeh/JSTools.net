/*
 * JSTools.Parser.Cruncher.dll / JSTools.net - A framework for JavaScript/ASP.NET applications.
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
using System.Resources;

namespace JSTools.Parser.Cruncher
{
	/// <summary>
	/// Exception is throw if an error has occured during
	/// parsing the specified javascript string.
	/// </summary>
	public class SyntaxException : Exception
	{
		private const string ERROR_DETAIL_SRC = "\n\nSource: {0}\nLineNo: {1}\nOffset: {2}\nLine: {3}";
		private static readonly ResourceManager MESSAGES = new ResourceManager(typeof(SyntaxException));

		private string _message = "Syntax error";
		private string _sourceName = string.Empty;
		private string _line = null;
		private int _lineNo = 0;
		private int _offset = -1;

		public override string Message { get { return _message; } }
		public string ErrorMessage { get { return _message; } }
		public string SourceName { get { return _sourceName; } }
		public string Line { get { return _line; } }
		public int LineNumber { get { return _lineNo; } }
		public int Offset { get { return _offset; } }


		/// <summary>
		/// Creates a JavaScript syntax exception.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="sourceName"></param>
		/// <param name="lineNo"></param>
		/// <param name="offset"></param>
		/// <param name="line"></param>
		public SyntaxException(string message, string sourceName, int lineNo, int offset, string line) : base("SyntaxError")
		{
			if (MESSAGES.GetString(message) != null)
				_message = MESSAGES.GetString(message);

			_sourceName = sourceName;
			_line = line;
			_lineNo = lineNo;
			_offset = offset;
			_message += string.Format(ERROR_DETAIL_SRC, sourceName, lineNo, offset, line);
		}

		/// <summary>
		/// Creates a JavaScript syntax exception.
		/// </summary>
		public SyntaxException(string message)
		{
			if (MESSAGES.GetString(message) != null)
				_message = MESSAGES.GetString(message);
		}
	}
}

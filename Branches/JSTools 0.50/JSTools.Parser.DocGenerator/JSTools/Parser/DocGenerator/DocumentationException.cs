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
using System.IO;

using JSTools.Parser;

namespace JSTools.Parser.DocGenerator
{
	/// <summary>
	/// Represents an exception which is throw/logged if an exception was
	/// thrown during documentation generating.
	/// </summary>
	public class DocumentationException : Exception
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private int _lineNumber = -1;
		private int _lineOffset = -1;
		private DocumentationException[] _innerExceptions = new DocumentationException[0];
		private string _docName = string.Empty;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the line number of the error reason.
		/// </summary>
		public int LineNumber
		{
			get { return _lineNumber; }
		}

		/// <summary>
		/// Gets the line offset of the error reason.
		/// </summary>
		public int LineOffset
		{
			get { return _lineOffset; }
		}

		/// <summary>
		/// Gets the exception reason.
		/// </summary>
		public DocumentationException[] InnerExceptions
		{
			get { return _innerExceptions; }
		}

		/// <summary>
		/// Gets the documentation name.
		/// </summary>
		public string DocumentationName
		{
			get { return _docName; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new DocumentationException instance.
		/// </summary>
		/// <param name="message">Error message which describes the error.</param>
		/// <param name="lineNumber">Specifies the error line.</param>
		/// <param name="lineOffset">Specifies the error line offset.</param>
		/// <param name="innerException">Specifies the inner error.</param>
		public DocumentationException(
			string message,
			int lineNumber,
			int lineOffset,
			Exception innerException) : base(message, innerException)
		{
			_lineNumber = lineNumber;
			_lineOffset = lineOffset;
		}

		/// <summary>
		/// Creates a new DocumentationException instance.
		/// </summary>
		/// <param name="message">Error message which describes the error.</param>
		/// <param name="docName">Specifies the name of the document.</param>
		/// <param name="innerExceptions">Inner exception instance.</param>
		public DocumentationException(
			string message,
			string docName,
			DocumentationException[] innerExceptions) : base(message)
		{
			if (_docName != null)
				_docName = docName = null;

			if (innerExceptions != null)
				_innerExceptions = innerExceptions;
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------
	}
}

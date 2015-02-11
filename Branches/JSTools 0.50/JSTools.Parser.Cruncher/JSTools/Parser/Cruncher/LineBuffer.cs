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
 *  Mike McCabe
 *  Silvan Gehrig
 */

using System;
using System.IO;
using System.Text;

namespace JSTools.Parser.Cruncher
{
	/// <summary>
	/// An input buffer that combines fast character-based access with
	/// (slower) support for retrieving the text of the current line.  It
	/// also supports building strings directly out of the internal buffer
	/// to support fast scanning with minimal object creation.
	/// </summary>
	/// <remarks>
	/// Note that it is customized in several ways to support the
	/// TokenStream class, and should not be considered general.
	/// </remarks>
	internal sealed class LineBuffer 
	{
		/// <summary>
		/// for smooth operation of Line, this should be greater than
		/// the length of any expected line.  Currently, 256 is 3% slower
		/// than 4096 for large compiles, but seems safer given evaluateString.
		/// Strings for the scanner are are built with StringBuffers
		/// instead of directly out of the buffer whenever a string crosses
		/// a buffer boundary, so small buffer sizes will mean that more
		/// objects are created.
		/// </summary>
		private const int BUFLEN = 4096;

		// Optimization for faster check for eol character: CharUtil.IsEOLChar(c) returns
		// true only when (c & EOL_HINT_MASK) == 0
		private const int EOL_HINT_MASK = 0xdfd0;

		private TextReader _inputReader;
		private char[] _otherBuffer = null;
		private char[] _buffer = null;

		// Yes, there are too too many of these.
		private int _offset = 0;
		private int _end = 0;
		private int _otherEnd;
		private int _lineno;

		private int _lineStart = 0;
		private int _otherStart = 0;
		private int _prevStart = 0;

		private bool _lastWasCR = false;
		private bool _hitEOF = false;

		/// <summary>
		/// Get the offset of the current character, relative to 
		/// the line that Line returns.
		/// </summary>
		internal int Offset 
		{
			get
			{
				if (_lineStart < 0)
					// The line begins somewhere in the other buffer.
					return _offset + (_otherEnd - _otherStart);
				else
					return _offset - _lineStart;
			}
		}

		/// <summary>
		/// Reconstruct a source line from the buffers.  This can be slow...
		/// </summary>
		internal string Line
		{
			get
			{
				// Look for line end in the unprocessed buffer
				int i = _offset;
				while(true) 
				{
					if (i == _end) 
					{
						// if we're out of buffer, let's just expand it.  We do
						// this instead of reading into a StringBuffer to
						// preserve the stream for later reads.
						if (_end == _buffer.Length) 
						{
							char[] tmp = new char[_buffer.Length * 2];
							Array.Copy(_buffer, 0, tmp, 0, _end);
							_buffer = tmp;
						}

						int charsRead = _inputReader.Read(_buffer, _end, _buffer.Length - _end);

						if (charsRead < 0)
							break;
						_end += charsRead;
					}
					int c = _buffer[i];
					if ((c & EOL_HINT_MASK) == 0 && CharUtil.IsEOLChar(c))
						break;
					i++;
				}

				if (_lineStart < 0) 
				{
					// the line begins somewhere _inputReader the other buffer; get that first.
					StringBuilder sb = new StringBuilder(_otherEnd - _otherStart + i);
					sb.Append(_otherBuffer, _otherStart, _otherEnd - _otherStart);
					sb.Append(_buffer, 0, i);
					return sb.ToString();
				} 
				else 
				{
					return new string(_buffer, _lineStart, i - _lineStart);
				}
			}
		}

		internal int LineNo { get { return _lineno; } }
		internal bool EOF { get { return _hitEOF; } }


		internal LineBuffer(TextReader inputReader, int lineno) 
		{
			_inputReader = inputReader;
			_lineno = lineno;
		}

		internal int Read() 
		{
			for(;;) 
			{
				if (_end == _offset && !Fill())
					return -1;

				int c = _buffer[_offset];
				++_offset;

				if ((c & EOL_HINT_MASK) == 0) 
				{
					switch (c) 
					{
						case '\r':
						{
							// if the next character is a newline, skip past it.
							if (_offset != _end) 
							{
								if (_buffer[_offset] == '\n')
									++_offset;
							} 
							else 
							{
								// set a flag for Fill(), in case the first char
								// of the next fill is a newline.
								_lastWasCR = true;
							}
							_prevStart = _lineStart;
							_lineStart = _offset;
							_lineno++;
							return '\n';
						}

						default:
						{
							if (c == '\n' || c == '\u2028' || c == '\u2029')
							{
								_prevStart = _lineStart;
								_lineStart = _offset;
								_lineno++;
								return '\n';
							}
							break;
						}
					}
				}

				if (c < 128 || !CharUtil.IsFormat(c)) 
				{
					return c;
				}
			}
		}

		internal void Unread() 
		{
			// offset can only be 0 when we're asked to Unread() an implicit
			// EOF_CHAR.

			// This would be wrong behavior in the general case,
			// because a Peek() could map a buffer.length offset to 0
			// in the process of a Fill(), and leave it there.  But
			// the scanner never calls Peek() or a failed Match()
			// followed by Unread()... this would violate 1-character
			// lookahead.
			if (_offset == 0 && !_hitEOF)
				throw new InvalidOperationException();

			if (_offset == 0) // Same as if (_hitEOF)
				return;
			_offset--;
			int c = _buffer[_offset];
			if ((c & EOL_HINT_MASK) == 0 && CharUtil.IsEOLChar(c)) 
			{
				_lineStart = _prevStart;
				_lineno--;
			}
		}

		internal int Peek()
		{
			for (;;) 
			{
				if (_end == _offset && !Fill()) 
				{
					return -1;
				}

				int c = _buffer[_offset];
				if ((c & EOL_HINT_MASK) == 0 && CharUtil.IsEOLChar(c)) 
				{
					return '\n';
				}
				if (c < 128 || !CharUtil.IsFormat(c)) 
				{
					return c;
				}
				SkipFormatChar();
			}
		}

		internal bool Match(int test) 
		{
			// TokenStream never looks ahead for '\n', which allows simple code
			if ((test & EOL_HINT_MASK) == 0 && CharUtil.IsEOLChar(test))
				throw new InvalidOperationException("TokenStream never looks ahead for '\n'");

			// Format chars are not allowed either
			if (test >= 128 && CharUtil.IsFormat(test))
				throw new InvalidOperationException("Format chars are not allowed");

			for (;;) 
			{
				if (_end == _offset && !Fill())
					return false;

				int c = _buffer[_offset];
				if (test == c) 
				{
					++_offset;
					return true;
				}
				if (c < 128 || !CharUtil.IsFormat(c)) 
				{
					return false;
				}
				SkipFormatChar();
			}
		}

		private bool Fill() 
		{
			// fill should be called only for empty buffer
			if (_end != _offset)
				throw new InvalidOperationException("Fill should be called only for empty buffer");

			// swap buffers
			char[] tempBuffer = _buffer;
			_buffer = _otherBuffer;
			_otherBuffer = tempBuffer;

			// allocate the buffers lazily, in case we're handed a short string.
			if (_buffer == null) 
			{
				_buffer = new char[BUFLEN];
			}

			// buffers have switched, so move the newline marker.
			if (_lineStart >= 0) 
			{
				_otherStart = _lineStart;
			} 
			else 
			{
				// discard beging of the old line
				_otherStart = 0;
			}

			_otherEnd = _end;

			// set _lineStart to a sentinel value, unless this is the first
			// time around.
			_prevStart = _lineStart = (_otherBuffer == null) ? 0 : -1;

			_offset = 0;
			_end = _inputReader.Read(_buffer, 0, _buffer.Length);

			if (_end == 0) 
			{
				// can't null buffers here, because a string might be retrieved
				// out of the other buffer, and a 0-length string might be
				// retrieved out of this one.
				_hitEOF = true;
				return false;
			}

			// If the last character of the previous fill was a carriage return,
			// then ignore a newline.

			// There's another bizzare special case here.  If _lastWasCR is
			// true, and we see a newline, and the buffer length is
			// 1... then we probably just read the last character of the
			// file, and returning after advancing _offset is not the right
			// thing to do.  Instead, we try to ignore the newline (and
			// likely get to TokenType.EOF for real) by doing yet another Fill().
			if (_lastWasCR) 
			{
				if (_buffer[0] == '\n') 
				{
					_offset++;
					if (_end == 1)
						return Fill();
				}
				_lineStart = _offset;
				_lastWasCR = false;
			}
			return true;
		}

		private void SkipFormatChar() 
		{
			if (!CharUtil.IsFormat(_buffer[_offset]))
				throw new InvalidOperationException();

			// swap prev character with format one so possible call to
			// startString can assume that previous non-format char is at
			// _offset - 1. Note it causes getLine to return not exactly the
			// source LineBuffer read, but it is used only in error reporting
			// and should not be a problem.
			if (_offset != 0) 
			{
				char tmp = _buffer[_offset];
				_buffer[_offset] = _buffer[_offset - 1];
				_buffer[_offset - 1] = tmp;
			}
			else if (_otherEnd != 0) 
			{
				char tmp = _buffer[_offset];
				_buffer[_offset] = _otherBuffer[_otherEnd - 1];
				_otherBuffer[_otherEnd - 1] = tmp;
			}

			++_offset;
		}
	}
}

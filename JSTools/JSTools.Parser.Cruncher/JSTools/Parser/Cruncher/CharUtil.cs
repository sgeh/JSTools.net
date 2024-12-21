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

namespace JSTools.Parser.Cruncher
{
	public sealed class CharUtil
	{
		private CharUtil()
		{
		}

		public static bool IsAlpha(int c) 
		{
			return ((c >= 'a' && c <= 'z')
				|| (c >= 'A' && c <= 'Z'));
		}

		public static bool IsJavaScriptIdentifierPart(char toCheck)
		{
			return (char.IsLetter(toCheck)
				|| IsCurrency(toCheck)
				|| IsPunctuation(toCheck)
				|| IsDigit(toCheck));
 		}

		public static bool IsEOLChar(int c) 
		{
			return c == '\r' || c == '\n' || c == '\u2028' || c == '\u2029';
		}

		public static bool IsFormat(int c)
		{
			return (c == 0x10);
		}

		public static bool IsDigit(int c) 
		{
			return (c >= '0' && c <= '9');
		}

		public static int XDigitToInt(int c) 
		{
			if ('0' <= c && c <= '9') { return c - '0'; }
			if ('a' <= c && c <= 'f') { return c - ('a' - 10); }
			if ('A' <= c && c <= 'F') { return c - ('A' - 10); }
			return -1;
		}

		/* As defined _inputReaderECMA.  jsscan.c uses C isspace() (which allows
		 * \v, I think.)  note that code _inputReaderin.Read() implicitly accepts
		 * '\r' == \u000D as well.
		 */
		public static bool IsJSSpace(int c) 
		{
			return (c == '\u0020' || c == '\u0009'
				|| c == '\u000C' || c == '\u000B'
				|| c == '\u00A0'
				|| char.IsWhiteSpace((char)c));
		}

		public static bool IsJavaScriptIdentifier(char toCheck)
		{
			return char.IsLetter(toCheck) || IsCurrency(toCheck) || IsPunctuation(toCheck);
		}

		private static bool IsCurrency(char toCheck)
		{
			return (toCheck == '$');
		}

		private static bool IsPunctuation(char toCheck)
		{
			return (toCheck == '_');
		}
	}
}

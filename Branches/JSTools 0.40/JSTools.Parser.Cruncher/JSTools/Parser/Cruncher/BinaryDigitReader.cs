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
	internal sealed class BinaryDigitReader 
	{
		private int _lgBase = 0;         // Logarithm of base of number
		private int _digit = 0;          // Current _digit value in radix given by base
		private int _digitPos = 0;       // Bit position of last bit extracted from _digit
		private string _digits = null;   // string containing the _digits
		private int _start = 0;          // Index of the first remaining _digit
		private int _end = 0;            // Index past the last remaining _digit

		internal BinaryDigitReader(int baseValue, string digits, int start, int end) 
		{
			_lgBase = 0;

			while (baseValue != 1) 
			{
				_lgBase++;
				baseValue >>= 1;
			}

			_digitPos = 0;
			_digits = digits;
			_start = start;
			_end = end;
		}

		/* Return the next binary _digit from the number or -1 if done */
		public int getNextBinaryDigit()
		{
			if (_digitPos == 0) 
			{
				if (_start == _end)
					return -1;

				char c = _digits[_start++];
				if ('0' <= c && c <= '9')
					_digit = c - '0';
				else if ('a' <= c && c <= 'z')
					_digit = c - 'a' + 10;
				else _digit = c - 'A' + 10;
				_digitPos = _lgBase;
			}
			return _digit >> --_digitPos & 1;
		}
	}
}

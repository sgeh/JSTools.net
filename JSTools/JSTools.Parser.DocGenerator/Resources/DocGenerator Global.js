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

// ------------------------------------------------------------------
// GLOBAL OBJECT
// ------------------------------------------------------------------

/// <variable type="Object">
/// The initial value of NaN is NaN (Not A Number). 
/// </variable>
var NaN;

/// <variable type="Object">
/// The initial value of Infinity is +8.
/// </variable>
var Infinity;

/// <variable type="Object">
/// The initial value of undefined is undefined. (Default value of an uninitialized variable).
/// </variable>
var undefined;

/// <function>
/// When the eval function is called with one argument x. Can be used to parse code.
/// </function>
/// <param name="x" type="String"></param>
/// <returns type="Object"></returns>
function eval(x) { }

/// <function>
/// The unescape function is a property of the global object. It computes a new version of a string value in
/// which each escape sequence of the sort that might be introduced by the escape function is replaced with
/// the character that it represents.
/// </function>
/// <param name="x" type="String"></param>
/// <returns type="String"></returns>
function unescape(x) { }

/// <function>
/// The escape function is a property of the global object. It computes a new version of a string value in
/// which certain characters have been replaced by a hexadecimal escape sequence.
/// For those characters being replaced whose code point value is 0xFF or less, a two-digit escape sequence
/// of the form %xx is used. For those characters being replaced whose code point value is greater than 0xFF,
/// a four-digit escape sequence of the form %uxxxx is used
/// </function>
/// <param name="string" type="String"></param>
/// <returns type="String"></returns>
function escape(string) { }

/// <function>
/// The parseInt function produces an integer value dictated by interpretation of the contents of the
/// string argument according to the specified radix. Leading whitespace in the string is ignored. If radix
/// is undefined or 0, it is assumed to be 10 except when the number begins with the character pairs 0x
/// or 0X, in which case a radix of 16 is assumed. Any radix-16 number may also optionally begin with
/// the character pairs 0x or 0X.
/// </function>
/// <param name="string" type="String"></param>
/// <param name="radix" type="Number"></param>
/// <returns type="Number"></returns>
function parseInt(string, radix) { }

/// <function>
/// The parseFloat function produces a number value dictated by interpretation of the contents of the
/// string argument as a decimal literal.
/// </function>
/// <param name="string" type="String"></param>
/// <returns type="Number"></returns>
function parseFloat(string) { }

/// <function>
/// Applies ToNumber to its argument, then returns true if the result is NaN, and otherwise returns false.
/// </function>
/// <param name="number" type="Number"></param>
/// <returns type="Boolean"></returns>
function isNaN(number) { }

/// <function>
/// Applies ToNumber to its argument, then returns false if the result is NaN, +8, or -8, and otherwise
/// returns true.
/// </function>
/// <param name="number" type="Number"></param>
/// <returns type="Boolean"></returns>
function isFinite(number) { }

/// <function>
/// The decodeURI function computes a new version of a URI in which each escape sequence and UTF-
/// 8 encoding of the sort that might be introduced by the encodeURI function is replaced with the
/// character that it represents. Escape sequences that could not have been introduced by encodeURI are
/// not replaced.
/// </function>
/// <param name="encodedURI" type="String"></param>
/// <returns type="String"></returns>
function decodeURI(encodedURI) { }

/// <function>
/// The encodeURI function computes a new version of a URI in which each instance of certain
/// characters is replaced by one, two or three escape sequences representing the UTF-8 encoding of the
/// character.
/// </function>
/// <param name="uri" type="String"></param>
/// <returns type="String"></returns>
function encodeURI(uri) { }

/// <function>
/// The encodeURIComponent function computes a new version of a URI in which each instance of
/// certain characters is replaced by one, two or three escape sequences representing the UTF-8 encoding
/// of the character.
/// </function>
/// <param name="uriComponent" type="String"></param>
/// <returns type="String"></returns>
function encodeURIComponent(uriComponent) { }

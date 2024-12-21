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
using System.Collections;

namespace JSTools.Parser.Cruncher
{
	/// <summary>
	/// Maps a keyword to the representing token type. The
	/// resulting mapping is stored in the public Operator,
	/// Token and Keyword property.
	/// </summary>
	public class KeywordMapper
	{
		public static readonly KeywordMapper Instance = new KeywordMapper();
		private static readonly KeywordMappingContiner MAPPINGS = new KeywordMappingContiner();

		/// <summary>
		/// Get the name of the keyword associated with the specified
		/// token type.
		/// </summary>
		public string this[TokenType toMap]
		{
			get { return MAPPINGS.GetKeywordOrOperator(toMap).Keyword; }
		}

		/// <summary>
		/// Initialize the mappings only once.
		/// </summary>
		static KeywordMapper()
		{
			MAPPINGS.AddMapping("break", TokenType.Break);
			MAPPINGS.AddMapping("case", TokenType.Case);
			MAPPINGS.AddMapping("continue", TokenType.Continue);
			MAPPINGS.AddMapping("default", TokenType.Default);
			MAPPINGS.AddMapping("delete", TokenType.DelProp);
			MAPPINGS.AddMapping("do", TokenType.Do);
			MAPPINGS.AddMapping("else", TokenType.Else);
			MAPPINGS.AddMapping("export", TokenType.Export);
			MAPPINGS.AddMapping("false", TokenType.Primary, TokenType.False);
			MAPPINGS.AddMapping("for", TokenType.For);
			MAPPINGS.AddMapping("function", TokenType.Function);
			MAPPINGS.AddMapping("if", TokenType.If);
			MAPPINGS.AddMapping("in",TokenType.RelOp, TokenType.In);
			MAPPINGS.AddMapping("new", TokenType.New);
			MAPPINGS.AddMapping("null", TokenType.Primary, TokenType.Null);
			MAPPINGS.AddMapping("return", TokenType.Return);
			MAPPINGS.AddMapping("switch", TokenType.Switch);
			MAPPINGS.AddMapping("this", TokenType.Primary, TokenType.This);
			MAPPINGS.AddMapping("true", TokenType.Primary, TokenType.True);
			MAPPINGS.AddMapping("typeof", TokenType.UnaryOp, TokenType.TypeOf);
			MAPPINGS.AddMapping("var", TokenType.Var);
			MAPPINGS.AddMapping("void", TokenType.UnaryOp, TokenType.Void);
			MAPPINGS.AddMapping("while", TokenType.While);
			MAPPINGS.AddMapping("with", TokenType.With);

			// javascript 1.4 keywords
			MAPPINGS.AddMapping("catch", TokenType.Catch);
			MAPPINGS.AddMapping("finally", TokenType.Finally);
			MAPPINGS.AddMapping("import", TokenType.Import);
			MAPPINGS.AddMapping("instanceof", TokenType.RelOp, TokenType.InstanceOf);
			MAPPINGS.AddMapping("throw", TokenType.Throw);
			MAPPINGS.AddMapping("try", TokenType.Try);

			// the following are reserved keywords
			MAPPINGS.AddMapping("abstract", TokenType.Reserved);
			MAPPINGS.AddMapping("boolean", TokenType.Reserved);
			MAPPINGS.AddMapping("byte", TokenType.Reserved);
			MAPPINGS.AddMapping("char", TokenType.Reserved);
			MAPPINGS.AddMapping("class", TokenType.Reserved);
			MAPPINGS.AddMapping("const", TokenType.Reserved);

			// provide compatibility with IE-debugger keyword
//			MAPPINGS.AddMapping("debugger", TokenType.Reserved);
			MAPPINGS.AddMapping("double", TokenType.Reserved);
			MAPPINGS.AddMapping("enum", TokenType.Reserved);
			MAPPINGS.AddMapping("extends", TokenType.Reserved);
			MAPPINGS.AddMapping("final", TokenType.Reserved);
			MAPPINGS.AddMapping("float", TokenType.Reserved);
			MAPPINGS.AddMapping("goto", TokenType.Reserved);
			MAPPINGS.AddMapping("implements", TokenType.Reserved);
			MAPPINGS.AddMapping("int", TokenType.Reserved);
			MAPPINGS.AddMapping("interface", TokenType.Reserved);
			MAPPINGS.AddMapping("long", TokenType.Reserved);
			MAPPINGS.AddMapping("native", TokenType.Reserved);
			MAPPINGS.AddMapping("package", TokenType.Reserved);
			MAPPINGS.AddMapping("private", TokenType.Reserved);
			MAPPINGS.AddMapping("protected", TokenType.Reserved);
			MAPPINGS.AddMapping("public", TokenType.Reserved);
			MAPPINGS.AddMapping("short", TokenType.Reserved);
			MAPPINGS.AddMapping("static", TokenType.Reserved);
			MAPPINGS.AddMapping("super", TokenType.Reserved);
			MAPPINGS.AddMapping("synchronized", TokenType.Reserved);
			MAPPINGS.AddMapping("throws", TokenType.Reserved);
			MAPPINGS.AddMapping("transient", TokenType.Reserved);
			MAPPINGS.AddMapping("volatile", TokenType.Reserved);
		}

		/// <summary>
		/// Creates a new KeywordMapper instance.
		/// </summary>
		private KeywordMapper()
		{
		}


		/// <summary>
		/// Maps the given keyword. The resulting token
		/// type is stored in the Token and Operator properties.
		/// </summary>
		/// <param name="name">Keyword which sould be mapped.</param>
		/// <returns>Returns the mapped keyword bucket.</returns>
		public KeywordBucket StringToKeyword(string name)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			return MAPPINGS.GetKeyword(name);
		}

		/// <summary>
		/// Mappes the given token type. The resulting key
		/// word is stored in the Keyword property.
		/// </summary>
		/// <param name="keyWord">Token type to map.</param>
		/// <returns>Returns the mapped keyword bucket.</returns>
		public KeywordBucket KeywordToString(TokenType keyWord)
		{
			return MAPPINGS.GetKeyword(keyWord);
		}


		/// <summary>
		/// Internal mapping container class. This class stores the
		/// mappings and maps them.
		/// </summary>
		private class KeywordMappingContiner
		{
			private SortedList _mappings = new SortedList();

			internal KeywordMappingContiner()
			{
			}

			internal void AddMapping(string keyword, TokenType token, TokenType tokenOperator)
			{
				if (keyword == null)
					throw new ArgumentNullException("keyword");

				// check if key already exists (sort mappings by keyword length)
				if (!_mappings.ContainsKey(keyword.Length))
					_mappings.Add(keyword.Length, new Hashtable());

				// add mapping to collection
				((Hashtable)_mappings[keyword.Length]).Add(
					keyword,
					new KeywordBucket(keyword, token, tokenOperator) );
			}

			internal void AddMapping(string keyWord, TokenType token)
			{
				AddMapping(keyWord, token, TokenType.EOF);
			}

			internal KeywordBucket GetKeyword(string keyWord)
			{
				if (_mappings[keyWord.Length] != null)
				{
					Hashtable mappings = (Hashtable)_mappings[keyWord.Length];

					if (mappings[keyWord] != null)
						return (KeywordBucket)mappings[keyWord];
				}
				return KeywordBucket.EMPTY;
			}

			internal KeywordBucket GetKeyword(TokenType token)
			{
				// loop through the mapping table (keywords are sorted by the length)
				foreach (DictionaryEntry keyWordEntry in _mappings)
				{
					// loop through the bucket list
					foreach (DictionaryEntry bucket in (Hashtable)keyWordEntry.Value)
					{
						if (((KeywordBucket)bucket.Value).Token == token)
							return (KeywordBucket)bucket.Value;
					}
				}
				return KeywordBucket.EMPTY;
			}

			internal KeywordBucket GetKeywordOrOperator(TokenType token)
			{
				// loop through the mapping table (keywords are sorted by the length)
				foreach (DictionaryEntry keyWordEntry in _mappings)
				{
					// loop through the bucket list
					foreach (DictionaryEntry bucket in (Hashtable)keyWordEntry.Value)
					{
						KeywordBucket retrievedBucket = (KeywordBucket)bucket.Value;

						if (retrievedBucket.Operator != TokenType.EOF && retrievedBucket.Operator == token)
							return retrievedBucket; // operator is not empty, and it matches the given token
						if (retrievedBucket.Token == token)
							return retrievedBucket; // operator is not equal to the given token type & the given token matches the bucket token
					}
				}
				return KeywordBucket.EMPTY;
			}
		}

		/// <summary>
		/// Represents an internal keyword bucket for mappings.
		/// </summary>
		public class KeywordBucket
		{
			internal static readonly KeywordBucket EMPTY = new KeywordBucket();

			private TokenType _token = TokenType.EOF;
			private TokenType _operator = TokenType.EOF;
			private string _keyword = string.Empty;

			public TokenType Token { get { return _token; } }
			public TokenType Operator { get { return _operator; } }
			public string Keyword { get { return _keyword; } }

			internal KeywordBucket()
			{
			}

			internal KeywordBucket(string keyword, TokenType token, TokenType tokenOperator)
			{
				_token = token;
				_operator = tokenOperator;
				_keyword = keyword;
			}
		}
	}
}

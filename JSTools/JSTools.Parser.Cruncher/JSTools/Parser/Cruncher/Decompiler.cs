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
 *  Patrick Beard
 *  Norris Boyd
 *  Igor Bukanov
 *  Brendan Eich
 *  Roger Lawrence
 *  Mike McCabe
 *  Ian D. Stewart
 *  Andi Vajda
 *  Andrew Wason
 *  Kemal Bayram
 *  Silvan Gehrig
 */

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using JSTools.Parser.Cruncher.Nodes;

namespace JSTools.Parser.Cruncher
{
	public enum DecompilationType
	{
		TopLevelScriptOrFunction = 0,
		ConstructedFunction = 1,
		NestedFunction = 2
	}

	internal class Decompiler 
	{
		// how much to indent
		private int _offset = 4;

		// less how much for case labels
		private int _labelSetBack = 2;

		// true to crunch the decompiled source
		private bool _doCrunch = false;

		private bool _useTabs = false;
		private bool _newLineBeforeLB = true;
		private int _defaultIndent = 2;

		private string NewLineString
		{
			get
			{
				if (!_doCrunch)
					return "\n";
				else
					return string.Empty;
			}
		}

		private string IndentCharacter
		{
			get
			{
				if (_useTabs)
					return TabString;
				else
					return WhiteSpaceString;

			}
		}

		private string WhiteSpaceString
		{
			get
			{
				if (!_doCrunch)
					return " ";
				else
					return string.Empty;
			}
		}

		private string TabString
		{
			get
			{
				if (!_doCrunch)
					return "\t";
				else
					return string.Empty;
			}
		}


		internal Decompiler()
		{
		}

		internal Decompiler(int offset, int defaultIndent, int labelSetBack, bool useTabs, bool newLineBeforeLB)
		{
			_offset = offset;
			_labelSetBack = labelSetBack;
			_newLineBeforeLB = newLineBeforeLB;
			_useTabs = useTabs;
			_defaultIndent = defaultIndent;
		}

		internal Decompiler(bool crunch)
		{
			if (crunch)
			{
				_offset = 0;
				_labelSetBack = 0;
				_doCrunch = crunch;
			}
		}


		/*
		 * Decompile the source information associated with this js
		 * function/script back into a string.  For the most part, this
		 * just means translating tokens back to their string
		 * representations; there's a little bit of lookahead logic to
		 * decide the proper spacing/indentation.  Most of the work in
		 * mapping the original source to the prettyprinted decompiled
		 * version is done by the parser.
		 *
		 * @param encodedSourcesTree See {@link NativeFunction#getSourcesTree()}
		 *        for definition
		 *
		 * @param fromFunctionConstructor true if encodedSourcesTree represents
		 *                                result of Function(...)
		 *
		 * @param version engine version used to compile the source
		 *
		 * @param indent How much to indent the decompiled result
		 *
		 * @param justbody Whether the decompilation should omit the
		 * function header and trailing brace.
		 */
		public string Decompile(
			FunctionTree compiledTree,
			bool fromFunctionConstructor,
			ScriptVersion version,
			bool justbody)
		{
			StringBuilder result = new StringBuilder();
			object[] srcData = new object[1];
	
			if (compiledTree != null)
			{
				InternalDecompile(
					compiledTree,
					version,
					_defaultIndent,
					(fromFunctionConstructor ? DecompilationType.ConstructedFunction : DecompilationType.TopLevelScriptOrFunction),
					justbody,
					srcData,
					result );
			}
			return result.ToString();
		}

		public void ToXmlFile(Node treeToStore, string filePathToStore)
		{
			ToXml(treeToStore).Save(filePathToStore);
		}

		public XmlDocument ToXml(Node treeToStore)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(Node));
			XmlDocument document = new XmlDocument();

			using (StringWriter writer = new StringWriter())
			{
				serializer.Serialize(writer, treeToStore);
				document.LoadXml(writer.ToString());
			}
			return document;
		}

		private void InternalDecompile(
			FunctionTree compiledTree,
			ScriptVersion version,
			int indent,
			DecompilationType type,
			bool justbody,
			object[] srcData,
			StringBuilder result)
		{
			// get node source (function nodes and top nodes accommodate this property)
			char[] source = compiledTree.Current.Source;

			if (source.Length == 0)
				return;

			// start decompiling
			if (type != DecompilationType.NestedFunction) 
			{
				// add an initial newline to exactly match js.
				if (!justbody)
					result.Append(NewLineString);
				for (int j = 0; j < indent; j++)
					result.Append(IndentCharacter);
			}

			int length = source.Length;
			int charIdx = 0;

			// If the first token is TokenType.Script, then we're
			// decompiling the toplevel script, otherwise it a function
			// and should start with TokenType.Function

			char token = source[charIdx];
			++charIdx;
			if (token == (char)TokenType.Function) 
			{
				if (!justbody) 
				{
					result.Append(KeywordMapper.Instance[TokenType.Function]);

					if (source[charIdx] != (char)TokenType.Lp)
					{
						result.Append(' ');
					}
					else if (version != ScriptVersion.Version_1_2
						&& type == DecompilationType.ConstructedFunction)
					{
						/* version != 1.2 Function constructor behavior -
						* print 'anonymous' as the function name if the
						* version (under which the function was compiled) is
						* less than 1.2... or if it's greater than 1.2, because
						* we need to be closer to ECMA.  (ToSource, please?)
						*/
						result.Append("anonymous");
					}
				} 
				else 
				{
					// Skip past the entire function header pass the next TokenType.EOL.
				skipLoop: for (;;) 
						  {
							  token = source[charIdx];
							  ++charIdx;
							  switch (token) 
							  {
								  case (char)TokenType.EOL:
									  goto skipLoop;
								  case (char)TokenType.Name:
									  // Skip function or argument name
									  charIdx = GetSourceString(source, charIdx, null);
									  break;
								  case (char)TokenType.Lp:
								  case (char)TokenType.Comma:
								  case (char)TokenType.Rp:
									  break;
								  default:
									  // Bad function header
									  throw new InvalidOperationException("Bad function header.");
							  }
						  }
				}
			}
			else if (token != (char)TokenType.Script) 
				throw new InvalidOperationException("Bad source header.");

			while (charIdx < length && source[charIdx] != (char)TokenType.LastToken) 
			{
				switch (source[charIdx]) 
				{
					case (char)TokenType.Name:
					case (char)TokenType.RegExp:  // re-wrapped in '/'s in parser...
						/* NAMEs are encoded as NAME, (char) length, string...
						 * Note that lookahead for detecting labels depends on
						 * this encoding; change there if this changes.

						 * Also change function-header skipping code above,
						 * used when decompling under decompileFunctionBody.
						 */
						charIdx = GetSourceString(source, charIdx + 1, srcData);
						result.Append((string)srcData[0]);
						continue;

					case (char)TokenType.Number: 
					{
						charIdx = GetSourceString(source, charIdx + 1, srcData);
						result.Append((string)srcData[0]);
						continue;
					}

					case (char)TokenType.String:
					{
						/*
						 * It's just a small script optimization. Remove all
						 * unnecessary string concate-notations.
						 */
						bool hasConcateNotation = false;

						do
						{
							charIdx = GetSourceString(source, charIdx + 1, srcData);
							string stringData = (string)srcData[0];

							if (source[charIdx] == (char)TokenType.Add 
								&& source[charIdx + 1] == (char)TokenType.String)
							{
								if (!hasConcateNotation) // first conate notation found
									result.Append(stringData.Substring(0, stringData.Length - 1));
								else // conate notation found
									result.Append(stringData.Substring(1, stringData.Length - 2));

								hasConcateNotation = true;
								charIdx += 1;
							}
							else
							{
								if (!hasConcateNotation) // no conate notation found
									result.Append(stringData);
								else // last conate notation found
									result.Append(stringData.Substring(1, stringData.Length - 1));

								hasConcateNotation = false;
							}
						}
						while (hasConcateNotation);

						continue;
					}

					case (char)TokenType.Primary:
					{
						++charIdx;
						switch(source[charIdx]) 
						{
							case (char)TokenType.True:
								result.Append(KeywordMapper.Instance[TokenType.True]);
								break;

							case (char)TokenType.False:
								result.Append(KeywordMapper.Instance[TokenType.False]);
								break;

							case (char)TokenType.Null:
								result.Append(KeywordMapper.Instance[TokenType.Null]);
								break;

							case (char)TokenType.This:
								result.Append(KeywordMapper.Instance[TokenType.This]);
								break;

							case (char)TokenType.TypeOf:
								result.Append(KeywordMapper.Instance[TokenType.TypeOf]);
								break;

							case (char)TokenType.Void:
								result.Append(KeywordMapper.Instance[TokenType.Void]);
								break;

							case (char)TokenType.Undefined:
								result.Append(KeywordMapper.Instance[TokenType.Undefined]);
								break;
						}
						break;
					}

					case (char)TokenType.Function: 
					{
						/* functions are stored inside
						 * the node tree, get them out
						 * there
						 */
						++charIdx;
						int functionNumber = source[charIdx];
					
						InternalDecompile(
							compiledTree[functionNumber],
							version,
							indent,
							DecompilationType.NestedFunction,
							false,
							srcData,
							result );
						break;
					}
					case (char)TokenType.Comma:
						result.Append(",");
						result.Append(WhiteSpaceString);
						break;

					case (char)TokenType.Lc:
					{
						if (_newLineBeforeLB)
						{
							result.Append(NewLineString);
							for (int less = 0; less < indent; less++)
								result.Append(IndentCharacter);
						}
						result.Append("{");

						if (NextIs(source, length, charIdx, TokenType.EOL))
							indent += _offset;

						break;
					}

					case (char)TokenType.Rc:
						/* don't print the closing RC if it closes the
						 * toplevel function and we're called from
						 * decompileFunctionBody.
						 */
						if (justbody && type != DecompilationType.NestedFunction && charIdx + 1 == length)
							break;

						if (NextIs(source, length, charIdx, TokenType.EOL))
							indent -= _offset;
						if (NextIs(source, length, charIdx, TokenType.While)
							|| NextIs(source, length, charIdx, TokenType.Else)) 
						{
							indent -= _offset;
							result.Append('}');
							result.Append(WhiteSpaceString);
						}
						else
							result.Append('}');
						break;

					case (char)TokenType.Lp:
						result.Append('(');
						break;

					case (char)TokenType.Rp:
						result.Append(')');
						if (NextIs(source, length, charIdx, TokenType.Lc))
							result.Append(WhiteSpaceString);
						break;

					case (char)TokenType.Lb:
						result.Append('[');
						break;

					case (char)TokenType.Rb:
						result.Append(']');
						break;

					case (char)TokenType.EOL:
						result.Append(NewLineString);

						/* add indent if any tokens remain,
						 * less setback if next token is
						 * a label, case or default.
						 */
						if (charIdx + 1 < length) 
						{
							int less = 0;
							int nextToken = source[charIdx + 1];
							if (nextToken == (char)TokenType.Case || nextToken == (char)TokenType.Default)
								less = _labelSetBack;
							else if (nextToken == (char)TokenType.Rc)
								less = _offset;

								/* elaborate check against label... skip past a
								 * following inlined NAME and look for a COLON.
								 * Depends on how NAME is encoded.
								 */
							else if (nextToken == (char)TokenType.Name) 
							{
								int afterName = GetSourceString(source, charIdx + 2,
									null);
								if (source[afterName] == (char)TokenType.Colon)
									less = _offset;
							}

							for (; less < indent; less++)
								result.Append(IndentCharacter);
						}
						break;

					case (char)TokenType.Dot:
						result.Append('.');
						break;

					case (char)TokenType.New:
						result.Append(KeywordMapper.Instance[TokenType.New]);
						result.Append(" ");
						break;

					case (char)TokenType.DelProp:
						result.Append(KeywordMapper.Instance[TokenType.DelProp]);
						result.Append(" ");
						break;

					case (char)TokenType.If:
						result.Append(KeywordMapper.Instance[TokenType.If]);
						result.Append(WhiteSpaceString);
						break;

					case (char)TokenType.Else:
						result.Append(KeywordMapper.Instance[TokenType.Else]);
						result.Append(WhiteSpaceString);
						break;

					case (char)TokenType.For:
						result.Append(KeywordMapper.Instance[TokenType.For]);
						result.Append(WhiteSpaceString);
						break;

					case (char)TokenType.In:
						result.Append(" ");
						result.Append(KeywordMapper.Instance[TokenType.In]);
						result.Append(" ");
						break;

					case (char)TokenType.With:
						result.Append(KeywordMapper.Instance[TokenType.With]);
						result.Append(WhiteSpaceString);
						break;

					case (char)TokenType.While:
						result.Append(KeywordMapper.Instance[TokenType.While]);
						result.Append(WhiteSpaceString);
						break;

					case (char)TokenType.Do:
						result.Append(KeywordMapper.Instance[TokenType.Do]);
						result.Append(WhiteSpaceString);
						break;

					case (char)TokenType.Try:
						result.Append(KeywordMapper.Instance[TokenType.Try]);
						result.Append(WhiteSpaceString);
						break;

					case (char)TokenType.Catch:
						result.Append(KeywordMapper.Instance[TokenType.Catch]);
						result.Append(WhiteSpaceString);
						break;

					case (char)TokenType.Finally:
						result.Append(KeywordMapper.Instance[TokenType.Finally]);
						result.Append(WhiteSpaceString);
						break;

					case (char)TokenType.Throw:
						result.Append(KeywordMapper.Instance[TokenType.Throw]);
						result.Append(" ");
						break;

					case (char)TokenType.Switch:
						result.Append(KeywordMapper.Instance[TokenType.Switch]);
						result.Append(WhiteSpaceString);
						break;

					case (char)TokenType.Break:
						result.Append(KeywordMapper.Instance[TokenType.Break]);

						if (NextIs(source, length, charIdx, TokenType.Name))
							result.Append(" ");
						break;

					case (char)TokenType.Continue:
						result.Append(KeywordMapper.Instance[TokenType.Continue]);

						if (NextIs(source, length, charIdx, TokenType.Name))
							result.Append(" ");
						break;

					case (char)TokenType.Case:
						result.Append(KeywordMapper.Instance[TokenType.Case]);
						result.Append(" ");
						break;

					case (char)TokenType.Default:
						result.Append(KeywordMapper.Instance[TokenType.Default]);
						break;

					case (char)TokenType.Return:
						result.Append(KeywordMapper.Instance[TokenType.Return]);

						if (!NextIs(source, length, charIdx, TokenType.Semi)
							&& source[charIdx + 1] != (char)TokenType.Lp)
						{
							result.Append(" ");
						}
						break;

					case (char)TokenType.Var:
						result.Append(KeywordMapper.Instance[TokenType.Var]);
						result.Append(" ");
						break;

					case (char)TokenType.Semi:
						// statement termination
						result.Append(';');

						if (!NextIs(source, length, charIdx, TokenType.EOL))
							// separators in FOR
							result.Append(WhiteSpaceString);
						break;

					case (char)TokenType.Assign:
					{
						++charIdx;
						switch (source[charIdx]) 
						{
							case (char)TokenType.Nop:
								result.Append(WhiteSpaceString);
								result.Append("=");
								result.Append(WhiteSpaceString);
								break;

							case (char)TokenType.Add:
								result.Append(WhiteSpaceString);
								result.Append("+=");
								result.Append(WhiteSpaceString);
								break;

							case (char)TokenType.Sub:
								result.Append(WhiteSpaceString);
								result.Append("-=");
								result.Append(WhiteSpaceString);
								break;

							case (char)TokenType.Mul:
								result.Append(WhiteSpaceString);
								result.Append("*=");
								result.Append(WhiteSpaceString);
								break;

							case (char)TokenType.Div:
								result.Append(WhiteSpaceString);
								result.Append("/=");
								result.Append(WhiteSpaceString);
								break;

							case (char)TokenType.Mod:
								result.Append(WhiteSpaceString);
								result.Append("%=");
								result.Append(WhiteSpaceString);
								break;

							case (char)TokenType.BitOr:
								result.Append(WhiteSpaceString);
								result.Append("|=");
								result.Append(WhiteSpaceString);
								break;

							case (char)TokenType.BitXOr:
								result.Append(WhiteSpaceString);
								result.Append("^=");
								result.Append(WhiteSpaceString);
								break;

							case (char)TokenType.BitAnd:
								result.Append(WhiteSpaceString);
								result.Append("&=");
								result.Append(WhiteSpaceString);
								break;

							case (char)TokenType.Lsh:
								result.Append(WhiteSpaceString);
								result.Append("<<=");
								result.Append(WhiteSpaceString);
								break;

							case (char)TokenType.Rsh:
								result.Append(WhiteSpaceString);
								result.Append(">>=");
								result.Append(WhiteSpaceString);
								break;

							case (char)TokenType.Ursh:
								result.Append(WhiteSpaceString);
								result.Append(">>>=");
								result.Append(WhiteSpaceString);
								break;
						}
						break;
					}

					case (char)TokenType.Hook:
						result.Append(WhiteSpaceString);
						result.Append("?");
						result.Append(WhiteSpaceString);
						break;

					case (char)TokenType.ObjLit:
						// pun OBJLIT to mean colon in objlit property initialization.
						// this needs to be distinct from COLON in the general case
						// to distinguish from the colon in a ternary... which needs
						// different spacing.
						result.Append(':');
						break;

					case (char)TokenType.Colon:
						if (NextIs(source, length, charIdx, TokenType.EOL))
							// it's the end of a label
							result.Append(':');
						else
							// it's the middle part of a ternary
							result.Append(WhiteSpaceString);
						result.Append(':');
						result.Append(WhiteSpaceString);
						break;

					case (char)TokenType.Or:
						result.Append(WhiteSpaceString);
						result.Append("||");
						result.Append(WhiteSpaceString);
						break;

					case (char)TokenType.And:
						result.Append(WhiteSpaceString);
						result.Append("&&");
						result.Append(WhiteSpaceString);
						break;

					case (char)TokenType.BitOr:
						result.Append(WhiteSpaceString);
						result.Append("|");
						result.Append(WhiteSpaceString);
						break;

					case (char)TokenType.BitXOr:
						result.Append(WhiteSpaceString);
						result.Append("^");
						result.Append(WhiteSpaceString);
						break;

					case (char)TokenType.BitAnd:
						result.Append(WhiteSpaceString);
						result.Append("&");
						result.Append(WhiteSpaceString);
						break;

					case (char)TokenType.EqOp:
					{
						++charIdx;
						switch(source[charIdx]) 
						{
							case (char)TokenType.ShEq:
								/*
								 * Emulate the C engine; if we're under version
								 * 1.2, then the == operator behaves like the ===
								 * operator (and the source is generated by
								 * decompiling a === opcode), so print the ===
								 * operator as ==.
								 */
								result.Append(WhiteSpaceString);
								result.Append(version == ScriptVersion.Version_1_2 ? "==" : "===");
								result.Append(WhiteSpaceString);
								break;

							case (char)TokenType.ShNe:
								result.Append(WhiteSpaceString);
								result.Append(version == ScriptVersion.Version_1_2 ? "!=" : "!==");
								result.Append(WhiteSpaceString);
								break;

							case (char)TokenType.Eq:
								result.Append(WhiteSpaceString);
								result.Append("==");
								result.Append(WhiteSpaceString);
								break;

							case (char)TokenType.Ne:
								result.Append(WhiteSpaceString);
								result.Append("!=");
								result.Append(WhiteSpaceString);
								break;
						}
						break;
					}

					case (char)TokenType.RelOp:
					{
						++charIdx;
						switch (source[charIdx]) 
						{
							case (char)TokenType.Le:
								result.Append(WhiteSpaceString);
								result.Append("<=");
								result.Append(WhiteSpaceString);
								break;

							case (char)TokenType.Lt:
								result.Append(WhiteSpaceString);
								result.Append("<");
								result.Append(WhiteSpaceString);
								break;

							case (char)TokenType.Ge:
								result.Append(WhiteSpaceString);
								result.Append(">=");
								result.Append(WhiteSpaceString);
								break;

							case (char)TokenType.Gt:
								result.Append(WhiteSpaceString);
								result.Append(">");
								result.Append(WhiteSpaceString);
								break;

							case (char)TokenType.InstanceOf:
								result.Append(" ");
								result.Append(KeywordMapper.Instance[TokenType.InstanceOf]);
								result.Append(" ");
								break;
						}
						break;
					}

					case (char)TokenType.ShOp:
					{
						++charIdx;
						switch (source[charIdx]) 
						{
							case (char)TokenType.Lsh:
								result.Append(WhiteSpaceString);
								result.Append("<<");
								result.Append(WhiteSpaceString);
								break;

							case (char)TokenType.Rsh:
								result.Append(WhiteSpaceString);
								result.Append(">>");
								result.Append(WhiteSpaceString);
								break;

							case (char)TokenType.Ursh:
								result.Append(WhiteSpaceString);
								result.Append(">>>");
								result.Append(WhiteSpaceString);
								break;
						}
						break;
					}

					case (char)TokenType.UnaryOp:
					{
						++charIdx;
						switch(source[charIdx]) 
						{
							case (char)TokenType.TypeOf:
								result.Append(KeywordMapper.Instance[TokenType.TypeOf]);

								if (source[charIdx + 1] != (char)TokenType.Lp)
									result.Append(" ");
								break;

							case (char)TokenType.Void:
								result.Append(KeywordMapper.Instance[TokenType.Void]);

								if (source[charIdx + 1] != (char)TokenType.Lp)
									result.Append(" ");
								break;

							case (char)TokenType.Not:
								result.Append('!');
								break;

							case (char)TokenType.BitNot:
								result.Append('~');
								break;

							case (char)TokenType.Add:
								result.Append('+');
								break;

							case (char)TokenType.Sub:
								result.Append('-');
								break;
						}
						break;
					}

					case (char)TokenType.Inc:
						result.Append("++");
						break;

					case (char)TokenType.Dec:
						result.Append("--");
						break;

					case (char)TokenType.Add:
						result.Append(WhiteSpaceString);
						result.Append("+");
						result.Append(WhiteSpaceString);
						break;

					case (char)TokenType.Sub:
						result.Append(WhiteSpaceString);
						result.Append("-");
						result.Append(WhiteSpaceString);
						break;

					case (char)TokenType.Mul:
						result.Append(WhiteSpaceString);
						result.Append("*");
						result.Append(WhiteSpaceString);
						break;

					case (char)TokenType.Div:
						result.Append(WhiteSpaceString);
						result.Append("/");
						result.Append(WhiteSpaceString);
						break;

					case (char)TokenType.Mod:
						result.Append(WhiteSpaceString);
						result.Append("%");
						result.Append(WhiteSpaceString);
						break;

					default:
						// If we don't know how to Decompile it, raise an exception.
						throw new InvalidOperationException("Unkown node found.");
				}
				++charIdx;
			}

			// add that trailing newline if it's an outermost function.
			if (type != DecompilationType.NestedFunction && !justbody)
				result.Append(NewLineString);
		}

		private bool NextIs(char[] source, int length, int charIdx, TokenType token) 
		{
			return (charIdx + 1 < length) ? (source[charIdx + 1] == (char)token) : false;
		}

		private int GetSourceString(char[] source, int offset,
			object[] result)
		{
			int length = source[offset];
			++offset;
			if ((0x8000 & length) != 0) 
			{
				length = ((0x7FFF & length) << 16) | source[offset];
				++offset;
			}
			if (result != null) 
			{
				result[0] = new string(source, offset, length);
			}
			return offset + length;
		}
	}
}

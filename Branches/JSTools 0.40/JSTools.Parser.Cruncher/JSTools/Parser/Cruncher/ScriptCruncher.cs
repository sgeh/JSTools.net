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
using System.IO;

namespace JSTools.Parser.Cruncher
{
	/// <summary>
	/// This class represents the parsing context of a script.
	/// </summary>
	public sealed class ScriptCruncher
	{
		public static readonly ScriptCruncher Instance = new ScriptCruncher();
		
		private const int DEFAULT_OFFSET = 1;
		private const int DEFAULT_INDENT = 0;
		private const int DEFAULT_SET_LABELBACK = 0;
		private static readonly ScriptVersion DEFAULT_VERSION = ScriptVersion.Version_1_3;
		

		/// <summary>
		/// Gets the implementation version.
		/// 
		/// <p>
		/// The implementation version is of the form
		/// <pre>
		///    "<i>name langVer</i> <code>release</code> <i>relNum date</i>"
		/// </pre>
		/// where <i>name</i> is the name of the product, <i>langVer</i> is
		/// the language version, <i>relNum</i> is the release number, and
		/// <i>date</i> is the release date for that specific
		/// release in the form "yyyy mm dd".
		/// </summary>
		public string ImplementationVersion
		{
			get { return "Rhino 1.5 release 4.1 2003-04-21 (PARSER ONLY)"; }
		}


		/// <summary>
		/// Do not create new ScriptCruncher instances. The implementation
		/// follows the singleton pattern.
		/// </summary>
		private ScriptCruncher() 
		{
		}

		#region FormatScript Method

		public string FormatScript(string toFormat)
		{
			return FormatScript(toFormat, DEFAULT_VERSION);
		}

		public string FormatScript(string toFormat, ScriptVersion version)
		{
			return FormatScriptTree(
				ParseScript(toFormat, version), 
				DEFAULT_OFFSET, 
				DEFAULT_INDENT,
				DEFAULT_SET_LABELBACK,
				true,
				true,
				version );
		}

		public string FormatScript(string toFormat, int offset, int defaultIndent, int labelSetBack, bool useTabs, bool newLineBeforeLB, ScriptVersion version)
		{
			return FormatScriptTree(
				ParseScript(toFormat, version),
				offset,
				defaultIndent,
				labelSetBack,
				useTabs,
				newLineBeforeLB,
				version );
		}

		#endregion

		#region FormatScriptFile Method

		public string FormatScriptFile(string toFormat)
		{
			return FormatScriptFile(toFormat, DEFAULT_VERSION);
		}

		public string FormatScriptFile(string toFormat, ScriptVersion version)
		{
			return FormatScriptTree(
				ParseScriptFile(toFormat, version), 
				DEFAULT_OFFSET, 
				DEFAULT_INDENT,
				DEFAULT_SET_LABELBACK,
				true,
				true,
				version );
		}

		public string FormatScriptFile(string toFormat, int offset, int defaultIndent, int labelSetBack, bool useTabs, bool newLineBeforeLB, ScriptVersion version)
		{
			return FormatScriptTree(
				ParseScriptFile(toFormat, version),
				offset,
				defaultIndent,
				labelSetBack,
				useTabs,
				newLineBeforeLB,
				version );
		}

		#endregion

		#region RemoveComments Method

		/// <summary>
		/// Parses the given script.
		/// </summary>
		/// <param name="toRemoveComments"></param>
		/// <param name="sourceName"></param>
		public string RemoveComments(string toRemoveComments)
		{
			return RemoveComments(toRemoveComments, DEFAULT_VERSION);
		}

		/// <summary>
		/// Parses the given script.
		/// </summary>
		/// <param name="toRemoveComments"></param>
		/// <param name="sourceName"></param>
		public string RemoveComments(string toRemoveComments, ScriptVersion version)
		{
			return FormatScriptTree(ParseScript(toRemoveComments, version), 0, 0, 0, false, false, version);
		}

		#endregion

		#region RemoveScriptFileComments Method

		/// <summary>
		/// Parses the given script file.
		/// </summary>
		/// <param name="fileLocation"></param>
		public string RemoveScriptFileComments(string toRemoveComments)
		{
			return RemoveScriptFileComments(toRemoveComments, DEFAULT_VERSION);
		}

		/// <summary>
		/// Parses the given script file.
		/// </summary>
		/// <param name="fileLocation"></param>
		public string RemoveScriptFileComments(string toRemoveComments, ScriptVersion version)
		{
			return FormatScriptTree(ParseScriptFile(toRemoveComments, version), 0, 0, 0, false, false, version);
		}

		#endregion

		#region CrunchScript Method

		/// <summary>
		/// Parses the given script.
		/// </summary>
		/// <param name="toParse"></param>
		/// <param name="sourceName"></param>
		public string CrunchScript(string toCrunch)
		{
			return CrunchScript(toCrunch, null, DEFAULT_VERSION);
		}

		/// <summary>
		/// Parses the given script.
		/// </summary>
		/// <param name="toParse"></param>
		/// <param name="sourceName"></param>
		public string CrunchScript(string toCrunch, string sourceName)
		{
			return CrunchScript(toCrunch, sourceName, DEFAULT_VERSION);
		}

		/// <summary>
		/// Parses the given script.
		/// </summary>
		/// <param name="toParse"></param>
		/// <param name="sourceName"></param>
		public string CrunchScript(string toCrunch, string sourceName, ScriptVersion version)
		{
			return CrunchScriptTree(ParseScript(toCrunch, sourceName, version), version);
		}

		#endregion

		#region CrunchScriptFile Method

		/// <summary>
		/// Parses the given script file.
		/// </summary>
		/// <param name="fileLocation"></param>
		public string CrunchScriptFile(string fileLocation)
		{
			return CrunchScriptFile(fileLocation, DEFAULT_VERSION);
		}

		/// <summary>
		/// Parses the given script file.
		/// </summary>
		/// <param name="fileLocation"></param>
		public string CrunchScriptFile(string fileLocation, ScriptVersion version)
		{
			return CrunchScriptTree(ParseScriptFile(fileLocation, version), version);
		}

		#endregion

		#region ParseScript Method

		/// <summary>
		/// Parses the given script.
		/// </summary>
		/// <param name="toParse"></param>
		/// <param name="sourceName"></param>
		public Node ParseScript(string toParse, string sourceName)
		{
			return ParseScript(toParse, sourceName, DEFAULT_VERSION);
		}

		/// <summary>
		/// Parses the given script.
		/// </summary>
		/// <param name="toParse"></param>
		/// <param name="sourceName"></param>
		public Node ParseScript(string toParse, string sourceName, ScriptVersion version)
		{
			using (StringReader reader = new StringReader(toParse))
			{
				return ParseScript(reader, sourceName, version);
			}
		}

		/// <summary>
		/// Parses the given script.
		/// </summary>
		/// <param name="toParse"></param>
		/// <param name="sourceName"></param>
		public Node ParseScript(string toParse)
		{
			return ParseScript(toParse, null);
		}

		/// <summary>
		/// Parses the given script.
		/// </summary>
		/// <param name="toParse"></param>
		/// <param name="sourceName"></param>
		public Node ParseScript(string toParse, ScriptVersion version)
		{
			return ParseScript(toParse, null, version);
		}

		#endregion

		#region ParseScriptFile Method

		/// <summary>
		/// Parses the given script file.
		/// </summary>
		/// <param name="fileLocation"></param>
		public Node ParseScriptFile(string fileLocation)
		{
			return ParseScriptFile(fileLocation, DEFAULT_VERSION);
		}

		/// <summary>
		/// Parses the given script file.
		/// </summary>
		/// <param name="fileLocation"></param>
		public Node ParseScriptFile(string fileLocation, ScriptVersion version)
		{
			using (FileStream stream = new FileStream(fileLocation, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				using (StreamReader reader = new StreamReader(stream))
				{
					return ParseScript(reader, fileLocation, version);
				}
			}
		}

		#endregion

		#region IsValidScript Method

		/// <summary>
		/// Checks whether the given script contains valid javascript syntax.
		/// </summary>
		/// <param name="toParse">String which should be parsed.</param>
		/// <returns>Returns true, if the given string contains valid javascript.</returns>
		public bool IsValidScript(string toParse)
		{
			return IsValidScript(toParse, DEFAULT_VERSION);
		}

		/// <summary>
		/// Checks whether the given script contains valid javascript syntax.
		/// </summary>
		/// <param name="toParse">String which should be parsed.</param>
		/// <returns>Returns true, if the given string contains valid javascript.</returns>
		public bool IsValidScript(string toParse, ScriptVersion version)
		{
			try
			{
				ParseScript(toParse, null, version);
				return true;
			}
			catch
			{
				return false;
			}
		}

		#endregion

		#region IsValidScriptFile Method

		/// <summary>
		/// Checks whether the given file contains valid javascript syntax.
		/// </summary>
		/// <param name="fileLocation">Script file to parse.</param>
		/// <returns>Returns true, if the given string contains valid javascript.</returns>
		public bool IsValidScriptFile(string fileLocation)
		{
			return IsValidScriptFile(fileLocation, DEFAULT_VERSION);
		}

		/// <summary>
		/// Checks whether the given file contains valid javascript syntax.
		/// </summary>
		/// <param name="fileLocation">Script file to parse.</param>
		/// <returns>Returns true, if the given string contains valid javascript.</returns>
		public bool IsValidScriptFile(string fileLocation, ScriptVersion version)
		{
			try
			{
				ParseScriptFile(fileLocation, version);
				return true;
			}
			catch
			{
				return false;
			}
		}

		#endregion

		private Node ParseScript(TextReader toParse, string sourceName, ScriptVersion version)
		{
			TokenStream ts = new TokenStream(version, toParse, sourceName, 0);
			IRFactory irf = new IRFactory(ts);

			Parser parser = new Parser(irf);
			return parser.Parse(ts);
		}

		private string CrunchScriptTree(Node treeNode, ScriptVersion version)
		{
			FunctionTree scriptTree = new FunctionTree(treeNode);
			Decompiler scriptDecompiler = new Decompiler(true);
			return scriptDecompiler.Decompile(scriptTree, false, version, false);
		}

		private string FormatScriptTree(Node treeNode, int offset, int defaultIndent, int labelSetBack, bool useTabs, bool newLineBeforeLB, ScriptVersion version)
		{
			FunctionTree scriptTree = new FunctionTree(treeNode);
			Decompiler scriptDecompiler = new Decompiler(offset, defaultIndent, labelSetBack, useTabs, newLineBeforeLB);
			return scriptDecompiler.Decompile(scriptTree, false, version, false);
		}
	}
}

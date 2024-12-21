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
using System.Globalization;
using System.IO;
using System.Text;

using JSTools.Parser.Rhino;

namespace JSTools.Parser.Cruncher
{
	/// <summary>
	/// Represents the script versions, which can be used to parse a script with mozilla rhino.
	/// </summary>
	public enum ScriptVersion : short
	{
		/// <summary>
		/// Default script version is Version_1_3.
		/// </summary>
		Default		= 130,

		/// <summary>
		/// Represents JavaScript Version 1.0
		/// </summary>
		Version_1_0	= 100,

		/// <summary>
		/// Represents JavaScript Version 1.2
		/// </summary>
		Version_1_1	= 110,

		/// <summary>
		/// Represents JavaScript Version 1.2
		/// </summary>
		Version_1_2	= 120,

		/// <summary>
		/// Represents JavaScript Version 1.3
		/// </summary>
		Version_1_3	= 130,

		/// <summary>
		/// Represents JavaScript Version 1.4
		/// </summary>
		Version_1_4	= 140,

		/// <summary>
		/// Represents JavaScript Version 1.5
		/// </summary>
		Version_1_5	= 150
	}


	/// <summary>
	/// Used to fire warning events.
	/// </summary>
	public delegate void CrunchWarningHandler(Cruncher sender, CruncherWarning warning);


	/// <summary>
	/// Used to remove unnecessary white spaces, line breaks and comments from a javascript. At first,
	/// the given file/string will be parsed with the rhino 1.5.0.4. After that, the parsing algorithm
	/// of the JSTools.Parser namespace will be used to remove unnecessary comments and other characters
	/// (e.g. white spaces).
	/// </summary>
	public class Cruncher
	{
		//------------------------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------------------------

		public CrunchWarningHandler OnWarn;

		private	const	char	ESCAPE_CHAR					= '\\';
		private const	string	COMMENT_REPLACEMENT_SCOPE	= "Comment Replacement Scope";

		private Context			_context					= new Context();
		private	ScriptVersion	_version					= ScriptVersion.Default;
		private	CultureInfo		_culture					= CultureInfo.CurrentCulture;

		private	TokenParser		_tokenParser				= null;
		private	bool			_warningsEnabled			= false;

		private	static readonly	char[]			SEPARATORS	= new char[] {
																			 '.',
																			 '{',
																			 '}',
																			 '[',
																			 ']',
																			 '(',
																			 ')',
																			 ';',
																			 ',',
																			 '+',
																			 '-',
																			 '/',
																			 '*',
																			 '=',
																			 '&',
																			 '|',
																			 '%',
																			 '>',
																			 '<',
																			 ':',
																			 '?',
																			 '!',
																			 '"',
																			 '\''
																		 };

		private	static readonly IParseItem[]	PARSE_ITEMS	= new IParseItem[] {
																				   new DoubleQuoteStringItem(),
																				   new SingleQuoteStringItem(),
																				   new CommentLineItem(),
																				   new CommentMultiLineItem(),
																				   new UnnecessaryStaticItem(),
																				   new UnnecessaryDynamicItem(),
																				   new WhiteSpaceItem(),
																				   new HtmlCommentBeginItem(),
																				   new HtmlCommentEndItem(),
																				   new RegExpItem(),
																				   new DefaultItem()
																			   };

		/// <summary>
		/// Enables the warning events.
		/// </summary>
		public bool EnableWarnings
		{
			get { return _warningsEnabled; }
			set { _warningsEnabled = value; }
		}


		/// <summary>
		/// Gets/sets the version of the script to parse.
		/// </summary>
		public ScriptVersion Version
		{
			get { return _version; }
			set
			{
				_version = value;
				_context.setLanguageVersion((int)_version);
			}
		}


		/// <summary>
		/// Gets/sets the language of the error message. Currently supported is English only.
		/// </summary>
		/// <exception cref="ArgumentNullException">The given language contains a null reference.</exception>
		public CultureInfo Language
		{
			get { return _culture; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value", "The given language contains a null reference!");

				_culture = value;
				_context.setLocale(value);
			}
		}


		//------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Creates a new Cruncher instance.
		/// </summary>
		public Cruncher()
		{
			// set default language version
			_context.setLanguageVersion((int)Version);

			// get default localization
			Language = _context.getLocale();

			// create parser.
			_tokenParser = new TokenParser(PARSE_ITEMS);

			// register global scope (for crunching)
			_tokenParser.SetGlobalScope(
				new string[] {
								 DoubleQuoteStringItem.ITEM_NAME,
								 SingleQuoteStringItem.ITEM_NAME,
								 UnnecessaryStaticItem.ITEM_NAME,
								 UnnecessaryDynamicItem.ITEM_NAME,
								 WhiteSpaceItem.ITEM_NAME,
								 RegExpItem.ITEM_NAME
							 } );

			_tokenParser.GlobalScope.DefaultItem = DefaultItem.ITEM_NAME;
			_tokenParser.GlobalScope.ItemRequired = false;

			// register crunch scope (for comment replacing)
			_tokenParser.RegisterScope(COMMENT_REPLACEMENT_SCOPE,
				new string[] {
								 DoubleQuoteStringItem.ITEM_NAME,
								 SingleQuoteStringItem.ITEM_NAME,
								 CommentLineItem.ITEM_NAME,
								 CommentMultiLineItem.ITEM_NAME,
								 RegExpItem.ITEM_NAME,
								 HtmlCommentBeginItem.ITEM_NAME,
								 HtmlCommentEndItem.ITEM_NAME
							 } );
		}


		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Returns the last separator of the given string at the specified
		/// pointer.
		/// </summary>
		/// <param name="toCheck">String to checks.</param>
		/// <param name="pointer">Index to search.</param>
		/// <returns>Returns the last found index of a separator or -1, if no
		/// separator was found.</returns>
		public static int GetLastSeparator(string toCheck, int pointer)
		{
			int index = pointer - 1;

			if (index < 0)
				return -1;

			for ( ; index > -1 && !IsSeparator(toCheck[index]); --index)
			{
				;
			}
			return index;
		}


		/// <summary>
		/// Returns true, if the specified char is a separator.
		/// </summary>
		/// <param name="toCheck">Char to check.</param>
		/// <returns>Returns true, if the specified char is a separator.</returns>
		public static bool IsSeparator(char toCheck)
		{
			for (int i = 0; i < SEPARATORS.Length; ++i)
			{
				if ((char)SEPARATORS.GetValue(i) == toCheck)
				{
					return true;
				}
			}
			return false;
		}


		/// <summary>
		/// Checks the char at the given index for an escape sequence.
		/// </summary>
		/// <param name="parseString">String, which should be parsed.</param>
		/// <param name="index">Long value, which represents the current scan index.</param>
		/// <returns>Returns true if the given parse string end character is not escaped.</returns>
		public static bool IsEscapedChar(string parseString, int index)
		{
			int count = 0;
			--index;

			for ( ; index > -1 && parseString[index] == ESCAPE_CHAR; --index)
			{
				++count;
			}
			return (count % 2 == 1);
		}


		/// <summary>
		/// Crunches the given string.
		/// 
		/// This method will report warnings, if the EnableWarnings switch contains
		/// true.
		/// </summary>
		/// <param name="toCrunch">String to crunch.</param>
		/// <exception cref="CruncherException">An error has occured during parsing the given string.</exception>
		public String Crunch(string toCrunch)
		{
			if (toCrunch == String.Empty)
				return String.Empty;

			// check for syntax errors, remove comments and crunch the script string
			return CrunchString(RemoveComments(toCrunch));
		}


		/// <summary>
		/// Checks the given string for javascript syntax errors. Throws a CruncherException
		/// if there is an error detected.
		/// 
		/// This method will report warnings, if the EnableWarnings switch contains
		/// true.
		/// </summary>
		/// <param name="toCheck">String you'd like to check.</param>
		/// <exception cref="CruncherException">An error has occured during parsing the given string.</exception>
		/// <exception cref="ArgumentNullException">The given string contains a null reference.</exception>
		public void Check(string toCheck)
		{
			if (toCheck == null)
				throw new ArgumentNullException("toCheck", "The given string contains a null reference!");

			try
			{
				_context.parseString(toCheck, String.Empty);
				ReportWarning(toCheck);
			}
			catch (JavaScriptException jsException)
			{
				throw new CruncherException(jsException.getMessage(), jsException);
			}
			catch (EvaluatorException evalException)
			{
				throw new CruncherException(evalException.getErrorMessage(),
					evalException.getSourceName(),
					evalException.getLineNumber(),
					evalException.getOffset(),
					evalException.getLine());
			}
			catch (Exception e)
			{
				throw new CruncherException("An error has occured during parsing the given string!", e);
			}
		}


		/// <summary>
		/// Checks the given string for a valid javascript syntax.
		/// </summary>
		/// <param name="toCheck">String to check.</param>
		/// <returns>Returns true, if the specified string has a valid javascript syntax.</returns>
		public bool CheckSyntax(string toCheck)
		{
			// is valid script
			bool returnValue = _context.isValidScript(toCheck);

			// clear warnings array.
			_context.clearWarnings();
			return returnValue;
		}


		public string RemoveComments(string toParse)
		{
			return RemoveComments(toParse, true);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="toParse"></param>
		/// <param name="checkSyntax"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException">The given string contains a null reference.</exception>
		/// <exception cref="CruncherException">An error has occured during parsing the given string.</exception>
		public string RemoveComments(string toParse, bool checkSyntax)
		{
			if (toParse == null)
				throw new ArgumentNullException("toParse", "The given string contains a null reference!");

			if (checkSyntax)
			{
				Check(toParse);
			}

			Scope commentReplacer = _tokenParser.GetScopeInstance(COMMENT_REPLACEMENT_SCOPE);
			commentReplacer.ItemRequired = false;
			commentReplacer.DefaultItem = DefaultItem.ITEM_NAME;

			INode parsedNode = _tokenParser.ParseScope(toParse, commentReplacer);
			StringBuilder parsedString = new StringBuilder(toParse.Length);
			
			foreach (INode childNode in parsedNode.Children)
			{
				parsedString.Append(((CrunchNode)childNode).ToCrunchString());
			}

			string replacedString = parsedString.ToString();

			if (checkSyntax)
			{
				Check(replacedString);
			}
			return replacedString;
		}


		/// <summary>
		/// Crunches the given string.
		/// </summary>
		/// <param name="toCrunch">String to crunch.</param>
		/// <returns>Returns the crunched string.</returns>
		private String CrunchString(String toCrunch)
		{
			StringBuilder crunchBuffer = new StringBuilder(toCrunch.Length);

			try
			{
				// create node tree
				INode tree = _tokenParser.Parse(toCrunch);

				foreach (CrunchNode node in tree.Children)
				{
					crunchBuffer.Append(node.ToCrunchString());
				}
			}
			catch (ParseItemException parseItemException)
			{
				throw new CruncherException(
					parseItemException.Message,
					parseItemException.ErrorMessage,
					parseItemException.LineNumber,
					parseItemException.ColumnNumber,
					parseItemException.Code);
			}

			// try to correct syntax errors
			CrunchFormater formater = new CrunchFormater(crunchBuffer.ToString(), _context);

			if (_warningsEnabled)
			{
				formater.OnSyntaxErrorFound += new CrunchSyntaxCorrection(OnSyntaxErrorReport);
			}
			return formater.CorrectScriptString();
		}


		/// <summary>
		/// Reparts a new warning. The default rhino error message and the default CRUNCH_SYNTAX_ERROR
		/// message will be reported.
		/// </summary>
		/// <param name="codeToCrunch">Code to crunch.</param>
		/// <param name="index">Index, where the warning has occured.</param>
		private void OnSyntaxErrorReport(CrunchFormater sender, string errorMessage, string codeToCrunch, int index)
		{
			CruncherWarning crunchWarning = new CruncherWarning(
				errorMessage,
				codeToCrunch,
				index,
				0);

			FireWarnEvent(crunchWarning);
		}


		/// <summary>
		/// Reports all warning of the _context instance and resets the warnings array
		/// of the _context object.
		/// </summary>
		/// <param name="codeToCrunch"></param>
		private void ReportWarning(string codeToCrunch)
		{
			EvaluatorWarning[] warnings = _context.getWarnings();

			if (warnings.Length == 0)
				return;

			// clear warnings array.
			_context.clearWarnings();

			if (!_warningsEnabled)
				return;

			for (int i = 0; i < warnings.Length; ++i)
			{
				EvaluatorWarning warning = (warnings.GetValue(i) as EvaluatorWarning);

				if (warning == null)
					continue;

				CruncherWarning crunchWarning = new CruncherWarning(
					warning.getErrorMessage(),
					codeToCrunch,
					warning.getLine(),
					warning.getOffset(),
					warning.getLineNumber());

				FireWarnEvent(crunchWarning);
			}
		}


		/// <summary>
		/// Fires the given warning.
		/// </summary>
		/// <param name="warningToFire">Warning you'd like to fire.</param>
		private void FireWarnEvent(CruncherWarning warningToFire)
		{
			if (OnWarn != null)
			{
				OnWarn(this, warningToFire);
			}
		}
	}
}

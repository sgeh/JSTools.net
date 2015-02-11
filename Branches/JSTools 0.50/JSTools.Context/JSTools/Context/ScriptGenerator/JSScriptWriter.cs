/*
 * JSTools.Context.dll / JSTools.net - A framework for JavaScript/ASP.NET applications.
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
using System.Text;

using JSTools;
using JSTools.Config;
using JSTools.ScriptTypes;

namespace JSTools.Context.ScriptGenerator
{
	/// <summary>
	/// Represents a javascript string writer implementation. The methods
	/// of this class provide functionalities which may be used to avoid
	/// client side script syntax errors.
	/// </summary>
	public class JSScriptWriter : StringWriter
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the javascript line break character.
		/// </summary>
		public const string LINE_BREAK = "\n";

		/// <summary>
		/// Returns the bitwase or assignement operator "|=".
		/// </summary>
		public const string BITWISE_OR_ASSIGNMENT_OP = "|=";

		/// <summary>
		/// Returns the bitwase and assignement operator "&amp;=".
		/// </summary>
		public const string BITWISE_AND_ASSIGNMENT_OP = "&=";

		/// <summary>
		/// Returns the bitwase xor assignement operator "^=".
		/// </summary>
		public const string BITWISE_XOR_ASSIGNMENT_OP = "^=";

		/// <summary>
		/// Returns the bitwase not assignement operator "~=".
		/// </summary>
		public const string BITWISE_NOT_ASSIGNMENT_OP = "~=";

		/// <summary>
		/// Returns the assignement operator "=".
		/// </summary>
		public const string ASSIGNMENT_OP = "=";


		private const string LINE_END_CHAR = ";";
		private const string MULTI_LINE_COMMENT_BEGIN = "/*";
		private const string MULTI_LINE_COMMENT_END = "*/";

		private const string SINGLE_LINE_COMMENT_BEGIN = "//";
		private const string SINGLE_LINE_COMMENT_END = LINE_BREAK;

		private const string FUNCTION_CALL_BEGIN = "{0}(";
		private const string FUNCTION_CALL_END = ")" + LINE_END_CHAR;
		private const string FUNCTION_ARGUMENT = ",";

		private const string DEF_ASSIGNMENT = "{0}" + ASSIGNMENT_OP;
		private const string OP_ASSIGNMENT = "{0}{1}";
		private const string VAR_ASSIGNMENT = "var " + DEF_ASSIGNMENT;
		private const string VAR_DECLARATION = "var {0}" + LINE_END_CHAR;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the new line constant. This value connot be changed.
		/// </summary>
		public override string NewLine
		{
			get { return base.NewLine; }
			set { throw new InvalidOperationException("Could not set this property, it's read only."); }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSScriptWriter instance.
		/// </summary>
		public JSScriptWriter()
		{
			CoreNewLine = NewLine.ToCharArray();
		}

		/// <summary>
		/// Creates a new JSScriptWriter instance which uses the provided
		/// IFormatProvider.
		/// </summary>
		/// <param name="formatProvider">Format provider which is used to control the format patterns.</param>
		public JSScriptWriter(IFormatProvider formatProvider) : base(formatProvider)
		{
			CoreNewLine = NewLine.ToCharArray();
		}

		/// <summary>
		/// Creates a new JSScriptWriter instance which uses the provided
		/// StringBuilder.
		/// </summary>
		/// <param name="builder">String builder into which should be written.</param>
		public JSScriptWriter(StringBuilder builder) : base(builder)
		{
			CoreNewLine = NewLine.ToCharArray();
		}


		/// <summary>
		/// Creates a new JSScriptWriter instance which uses the provided
		/// StringBuilder and IFormatProvider.
		/// </summary>
		/// <param name="builder">String builder into which should be written.</param>
		/// <param name="formatProvider">Format provider which is used to control the format patterns.</param>
		public JSScriptWriter(StringBuilder builder, IFormatProvider formatProvider) : base(builder, formatProvider)
		{
			CoreNewLine = NewLine.ToCharArray();
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Appends a new single line comment begin to content "//".
		/// </summary>
		public void AppendSingleLineCommentBegin()
		{
			Write(SINGLE_LINE_COMMENT_BEGIN);
		}


		/// <summary>
		/// Appends a new single line comment end to content "\n".
		/// </summary>
		public void AppendSingleLineCommentEnd()
		{
			Write(SINGLE_LINE_COMMENT_END);
		}

		/// <summary>
		/// Appends the full comment with the appropriated ending and start charaters.
		/// </summary>
		/// <param name="comment">Comment content to add.</param>
		/// <remarks>
		/// The comment string must not contain a new line break, otherwise
		/// the written string will result in a client script syntax error.
		/// </remarks>
		public void AppendSingleLineComment(string comment)
		{
			AppendSingleLineCommentBegin();
			Write(comment);
			AppendSingleLineCommentEnd();
		}

		/// <summary>
		/// Appends the full comment with appropriated ending and start.
		/// </summary>
		/// <param name="comment">Comment content to add.</param>
		/// <param name="newLineReplacement">String which is used to replace the new line characters.</param>
		/// <remarks>
		/// If the comment string contains a line break it will be replaced
		/// with the given value.
		/// </remarks>
		public void AppendSingleLineComment(string comment, string newLineReplacement)
		{
			if (newLineReplacement == null)
				newLineReplacement = string.Empty;

			AppendSingleLineCommentBegin();
			Write(comment.Replace(SINGLE_LINE_COMMENT_END, newLineReplacement));
			AppendSingleLineCommentEnd();
		}

		/// <summary>
		/// Appends a new multline comment begin. "/*"
		/// </summary>
		public void AppendMultiLineCommentBegin()
		{
			Write(MULTI_LINE_COMMENT_BEGIN);
		}

		/// <summary>
		/// Appends a new multline comment end. "*/"
		/// </summary>
		public void AppendMultiLineCommentEnd()
		{
			Write(MULTI_LINE_COMMENT_END);
		}

		/// <summary>
		/// Appends the full comment with appropriated ending and start.
		/// </summary>
		/// <param name="comment">Comment which should be added.</param>
		/// <remarks>
		/// The comment string must not contain a comment end string "*/", otherwise
		/// the written string will result in a client script syntax error.
		/// </remarks>
		public void AppendMultiLineComment(string comment)
		{
			AppendMultiLineCommentBegin();
			Write(comment);
			AppendMultiLineCommentEnd();
		}
		
		/// <summary>
		/// Appends the full comment with appropriated ending and start.
		/// </summary>
		/// <param name="comment">Comment which should be added.</param>
		/// <param name="commentEndReplacement">String which is used to replace the comment end characters.</param>
		/// <remarks>
		/// If the comment string contains a comment end string "*/" it will be
		/// replaced with the given value.
		/// </remarks>
		public void AppendMultiLineComment(string comment, string commentEndReplacement)
		{
			if (commentEndReplacement == null)
				commentEndReplacement = string.Empty;

			AppendMultiLineCommentBegin();
			Write(comment.Replace(MULTI_LINE_COMMENT_END, commentEndReplacement));
			AppendMultiLineCommentEnd();
		}

		/// <summary>
		/// Appends a new variable and its assignment. (e.g. var a = 5;)
		/// </summary>
		/// <param name="variableName">Name of the variable (e.g. a).</param>
		/// <param name="variableValue">Value of the variable (e.g. 5).</param>
		/// <param name="assigmentOperator">Assignment operator (e.g. =, +=, ...)</param>
		public void AppendVariableAssignment(string variableName, string variableValue, string assigmentOperator)
		{
			AppendVariableAssignment(variableName, variableValue, assigmentOperator, false);
		}

		/// <summary>
		/// Appends a new variable and its assignment. (e.g. var a = 5;)
		/// </summary>
		/// <param name="variableName">Name of the variable (e.g. a).</param>
		/// <param name="variableValue">Value of the variable (e.g. {5} ).</param>
		/// <param name="assigmentOperator">Assignment operator (e.g. =, +=, ...)</param>
		/// <param name="newLine">True to add a new line after the varible definition.</param>
		public void AppendVariableAssignment(string variableName, object variableValue, string assigmentOperator, bool newLine)
		{
			AppendVariableAssignment(variableName,
				new ScriptValue(variableValue).ToString(),
				assigmentOperator,
				newLine );
		}

		/// <summary>
		/// Appends a new variable and its assignment. (e.g. var a = 5;)
		/// </summary>
		/// <param name="variableName">Name of the variable (e.g. a).</param>
		/// <param name="variableValue">Value of the variable (e.g. 5).</param>
		/// <param name="assigmentOperator">Assignment operator (e.g. =, +=, ...)</param>
		/// <param name="newLine">True to add a new line after the varible definition.</param>
		public void AppendVariableAssignment(string variableName, string variableValue, string assigmentOperator, bool newLine)
		{
			Write(OP_ASSIGNMENT, variableName, assigmentOperator);
			Write(variableValue);

			if (newLine)
				WriteLine(LINE_END_CHAR);
			else
				Write(LINE_END_CHAR);
		}

		/// <summary>
		/// Appends a new variable and its assignment. (e.g. var a = 5;)
		/// </summary>
		/// <param name="variableName">Name of the variable (e.g. a).</param>
		/// <param name="variableValue">Value of the variable (e.g. 5).</param>
		public void AppendVariableAssignment(string variableName, string variableValue)
		{
			AppendVariableAssignment(variableName, variableValue, false);
		}

		/// <summary>
		/// Appends a new variable and its assignment. (e.g. var a = 5;)
		/// </summary>
		/// <param name="variableName">Name of the variable (e.g. a).</param>
		/// <param name="variable">Value of the variable (e.g. 5).</param>
		/// <param name="newLine">True to add a new line after the varible definition.</param>
		public void AppendVariableAssignment(string variableName, string variable, bool newLine)
		{
			Write(DEF_ASSIGNMENT, variableName);
			Write(variable);

			if (newLine)
				WriteLine(LINE_END_CHAR);
			else
				Write(LINE_END_CHAR);
		}

		/// <summary>
		/// Appends a new variable assignment. (e.g. a = 'hello';)
		/// </summary>
		/// <param name="variableName">Name of the variable (e.g. a).</param>
		/// <param name="variableValue">Value of the variable (e.g. "hello").</param>
		public void AppendAssignment(string variableName, object variableValue)
		{
			AppendAssignment(variableName, variableValue, false);
		}

		/// <summary>
		/// Appends a variable assignment. (e.g. a = 'hello';)
		/// </summary>
		/// <param name="variableName">Name of the variable (e.g. a).</param>
		/// <param name="variableValue">Value of the variable (e.g. "hello").</param>
		/// <param name="newLine">True to add a new line after the varible definition.</param>
		public void AppendAssignment(string variableName, object variableValue, bool newLine)
		{
			Write(DEF_ASSIGNMENT, variableName);
			WriteValue(variableValue);

			if (newLine)
				WriteLine(LINE_END_CHAR);
			else
				Write(LINE_END_CHAR);
		}

		/// <summary>
		/// Appends a new variable declaration. (e.g. var a;)
		/// </summary>
		/// <param name="variableName">Name of the variable (e.g. a).</param>
		public void AppendVariableDeclaration(string variableName)
		{
			AppendVariableDeclaration(variableName, false);
		}

		/// <summary>
		/// Appends a new variable declaration. (e.g. var a;)
		/// </summary>
		/// <param name="variableName">Name of the variable (e.g. a).</param>
		/// <param name="newLine">True to add a new line after the varible definition.</param>
		public void AppendVariableDeclaration(string variableName, bool newLine)
		{
			Write(VAR_DECLARATION, variableName);

			if (newLine)
				WriteLine(LINE_END_CHAR);
			else
				Write(LINE_END_CHAR);
		}

		/// <summary>
		/// Appends a new script function call. (e.g. window.alert("8");)
		/// </summary>
		/// <param name="functionName">Name of the function to call. (e.g. "window.alert")</param>
		/// <param name="arguments">Arguments which are written into the function header as arguments.</param>
		public void AppendFunctionCall(string functionName, params object[] arguments)
		{
			AppendFunctionCall(functionName);

			if (arguments != null)
			{
				for (int i = 0; i < arguments.Length; ++i)
				{
					WriteValue(arguments[i]);

					if (i + 1 != arguments.Length)
						AppendArgumentSeparator();
				}
			}
			AppendFunctionCallEnd();
		}

		/// <summary>
		/// Appends the a new script function call without arguments.
		/// (e.g. window.alert();)
		/// </summary>
		/// <param name="functionName">Name of the function to call "window.alert".</param>
		public void AppendFunctionCall(string functionName)
		{
			Write(FUNCTION_CALL_BEGIN, functionName);
		}

		/// <summary>
		/// Appends the end of a script function call. ();)
		/// </summary>
		public void AppendFunctionCallEnd()
		{
			Write(FUNCTION_CALL_END);
		}

		/// <summary>
		/// Appends an argument separator used in conjunction with a script
		/// function call. (,)
		/// </summary>
		public void AppendArgumentSeparator()
		{
			Write(FUNCTION_ARGUMENT);
		}

		/// <summary>
		/// Appends a value and serializes it in order to be used on client side.
		/// </summary>
		/// <param name="valueToAppend">Value which should be encoded.</param>
		public void WriteValue(object valueToAppend)
		{
			Write(new ScriptValue(valueToAppend).ToString());
		}
	}
}

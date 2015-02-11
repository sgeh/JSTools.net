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
using System.Text;

using JSTools;
using JSTools.Config;
using JSTools.ScriptTypes;
using JSTools.Util.Serialization;

namespace JSTools.Context.ScriptGenerator
{
	/// <summary>
	/// Represents the default javascript script generator, which is used
	/// to render javascript sections. This class provides functionalities
	/// which may be used to avoid client side script syntax errors.
	/// </summary>
	public class JSScriptGenerator : IScriptGenerator
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const string DEFAULT_OUTPUT_FUNCTION = "document.write";
		private const string DEFAULT_ALERT_FUNCTION = "window.alert";

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		///  <see cref="IScriptGenerator.LineBreak" />
		/// </summary>
		public string LineBreak
		{
			get { return JSScriptWriter.LINE_BREAK; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSScriptGenerator instance.
		/// </summary>
		internal JSScriptGenerator()
		{
		}
		
		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		///  <see cref="IScriptGenerator.CreateSingleLineComment" />
		/// </summary>
		/// <param name="comment">
		///  <see cref="IScriptGenerator.CreateSingleLineComment" />
		/// </param>
		/// <returns>
		///  <see cref="IScriptGenerator.CreateSingleLineComment" />
		/// </returns>
		public virtual string CreateSingleLineComment(string comment)
		{
			JSScriptWriter writer = new JSScriptWriter();
			writer.AppendSingleLineComment(comment);
			return writer.ToString();
		}

		/// <summary>
		///  <see cref="IScriptGenerator.CreateMultiLineComment" />
		/// </summary>
		/// <param name="comment">
		///  <see cref="IScriptGenerator.CreateMultiLineComment" />
		/// </param>
		/// <returns>
		///  <see cref="IScriptGenerator.CreateMultiLineComment" />
		/// </returns>
		public virtual string CreateMultiLineComment(string comment)
		{
			JSScriptWriter writer = new JSScriptWriter();
			writer.AppendMultiLineComment(comment);
			return writer.ToString();
		}

		/// <summary>
		///  <see cref="IScriptGenerator.CreateAssignment" />
		/// </summary>
		/// <param name="variableName">
		///  <see cref="IScriptGenerator.CreateAssignment" />
		/// </param>
		/// <param name="variableValue">
		///  <see cref="IScriptGenerator.CreateAssignment" />
		/// </param>
		/// <returns>
		///  <see cref="IScriptGenerator.CreateAssignment" />
		/// </returns>
		public virtual string CreateAssignment(string variableName, object variableValue)
		{
			JSScriptWriter writer = new JSScriptWriter();
			writer.AppendAssignment(variableName, variableValue);
			return writer.ToString();
		}

		/// <summary>
		///  <see cref="IScriptGenerator.CreateVariableDeclaration" />
		/// </summary>
		/// <param name="variableName">
		///  <see cref="IScriptGenerator.CreateVariableDeclaration" />
		/// </param>
		/// <returns>
		///  <see cref="IScriptGenerator.CreateVariableDeclaration" />
		/// </returns>
		public virtual string CreateVariableDeclaration(string variableName)
		{
			JSScriptWriter writer = new JSScriptWriter();
			writer.AppendVariableDeclaration(variableName);
			return writer.ToString();
		}

		/// <summary>
		///  <see cref="IScriptGenerator.CreateVariableDeclaration" />
		/// </summary>
		/// <param name="variableName">
		///  <see cref="IScriptGenerator.CreateVariableDeclaration" />
		/// </param>
		/// <param name="variableValue">
		///  <see cref="IScriptGenerator.CreateVariableDeclaration" />
		/// </param>
		/// <returns>
		///  <see cref="IScriptGenerator.CreateVariableDeclaration" />
		/// </returns>
		public virtual string CreateVariableDeclaration(string variableName, object variableValue)
		{
			JSScriptWriter writer = new JSScriptWriter();
			writer.AppendVariableAssignment(variableName, variableValue, JSScriptWriter.ASSIGNMENT_OP, false);
			return writer.ToString();
		}

		/// <summary>
		///  <see cref="IScriptGenerator.CreateFunctionCall" />
		/// </summary>
		/// <param name="functionName">
		///  <see cref="IScriptGenerator.CreateFunctionCall" />
		/// </param>
		/// <param name="arguments">
		///  <see cref="IScriptGenerator.CreateFunctionCall" />
		/// </param>
		/// <returns>
		///  <see cref="IScriptGenerator.CreateFunctionCall" />
		/// </returns>
		public virtual string CreateFunctionCall(string functionName, params object[] arguments)
		{
			JSScriptWriter writer = new JSScriptWriter();
			writer.AppendFunctionCall(functionName, arguments);
			return writer.ToString();
		}

		/// <summary>
		///  <see cref="IScriptGenerator.SerializeObject" />
		/// </summary>
		/// <param name="toSerialze">
		///  <see cref="IScriptGenerator.SerializeObject" />
		/// </param>
		/// <returns>
		///  <see cref="IScriptGenerator.SerializeObject" />
		/// </returns>
		public virtual string SerializeObject(object toSerialze)
		{
			JSScriptWriter writer = new JSScriptWriter();
			writer.WriteValue(toSerialze);
			return writer.ToString();
		}

		/// <summary>
		///  <see cref="IScriptGenerator.DeserializeObject" />
		/// </summary>
		/// <param name="toDeserialze">
		///  <see cref="IScriptGenerator.DeserializeObject" />
		/// </param>
		/// <returns>
		///  <see cref="IScriptGenerator.DeserializeObject" />
		/// </returns>
		public virtual object DeserializeObject(string toDeserialze)
		{
			return new SimpleObjectSerializer().Deserialize(toDeserialze);
		}

		/// <summary>
		///  <see cref="IScriptGenerator.DeserializeObject" />
		/// </summary>
		/// <param name="toDeserialze">
		///  <see cref="IScriptGenerator.DeserializeObject" />
		/// </param>
		/// <param name="toFill">
		///  <see cref="IScriptGenerator.DeserializeObject" />
		/// </param>
		/// <returns>
		///  <see cref="IScriptGenerator.DeserializeObject" />
		/// </returns>
		public virtual object DeserializeObject(string toDeserialze, object toFill)
		{
			return new SimpleObjectSerializer().Deserialize(toDeserialze, toFill);
		}

		/// <summary>
		///  <see cref="IScriptGenerator.CreatePlainOutput" />
		/// </summary>
		/// <param name="toWrite">
		///  <see cref="IScriptGenerator.CreatePlainOutput" />
		/// </param>
		/// <returns>
		///  <see cref="IScriptGenerator.CreatePlainOutput" />
		/// </returns>
		public virtual string CreatePlainOutput(object toWrite)
		{
			JSScriptWriter writer = new JSScriptWriter();
			writer.AppendFunctionCall(DEFAULT_OUTPUT_FUNCTION);
			writer.Write(toWrite);
			writer.AppendFunctionCallEnd();
			return writer.ToString();
		}

		/// <summary>
		///  <see cref="IScriptGenerator.CreateOutput" />
		/// </summary>
		/// <param name="toWrite">
		///  <see cref="IScriptGenerator.CreateOutput" />
		/// </param>
		/// <returns>
		///  <see cref="IScriptGenerator.CreateOutput" />
		/// </returns>
		public virtual string CreateOutput(object toWrite)
		{
			return CreateFunctionCall(DEFAULT_OUTPUT_FUNCTION, toWrite);
		}

		/// <summary>
		///  <see cref="IScriptGenerator.CreateAlert" />
		/// </summary>
		/// <param name="toAlert">
		///  <see cref="IScriptGenerator.CreateAlert" />
		/// </param>
		/// <returns>
		///  <see cref="IScriptGenerator.CreateAlert" />
		/// </returns>
		public virtual string CreateAlert(object toAlert)
		{
			return CreateFunctionCall(DEFAULT_ALERT_FUNCTION, toAlert);
		}

		/// <summary>
		///  <see cref="IScriptGenerator.CreateException" />
		/// </summary>
		/// <param name="outputFunction">
		///  <see cref="IScriptGenerator.CreateException" />
		/// </param>
		/// <param name="exception">
		///  <see cref="IScriptGenerator.CreateException" />
		/// </param>
		/// <returns>
		///  <see cref="IScriptGenerator.CreateException" />
		/// </returns>
		public virtual string CreateException(string outputFunction, Exception exception)
		{
			JSScriptWriter writer = new JSScriptWriter();

			// render exception output
			Exception exceptionStack = exception;
			StringBuilder alertArgument = new StringBuilder();

			while (exceptionStack != null)
			{
				alertArgument.Append(exceptionStack.Message);
				alertArgument.Append(LineBreak);
				alertArgument.Append(exceptionStack.StackTrace.ToString());
				alertArgument.Append(LineBreak);
				alertArgument.Append(LineBreak);
				exceptionStack = exceptionStack.InnerException;
			}

			writer.AppendFunctionCall(outputFunction, alertArgument.ToString());
			return writer.ToString();
		}

		/// <summary>
		///  <see cref="IScriptGenerator.CreateExceptionAlert" />
		/// </summary>
		/// <param name="exception">
		///  <see cref="IScriptGenerator.CreateExceptionAlert" />
		/// </param>
		/// <returns>
		///  <see cref="IScriptGenerator.CreateExceptionAlert" />
		/// </returns>
		public virtual string CreateExceptionAlert(Exception exception)
		{
			return CreateException(DEFAULT_ALERT_FUNCTION, exception);
		}
	}
}

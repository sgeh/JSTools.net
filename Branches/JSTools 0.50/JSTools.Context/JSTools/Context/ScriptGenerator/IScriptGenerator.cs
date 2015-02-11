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
using System.Text;

using JSTools;
using JSTools.Config;
using JSTools.ScriptTypes;

namespace JSTools.Context.ScriptGenerator
{
	/// <summary>
	/// Represents a script generator, which is used to render script
	/// sections. This class provides functionalities which should be used
	/// to avoid client side script syntax errors.
	/// </summary>
	/// <remarks>
	/// To override some functionalities of this class, you have to derive
	/// from AJSToolsContext and override the ReinitContext method in order
	/// to assign your own JSScriptGenerator implementation.
	/// </remarks>
	public interface IScriptGenerator
	{
		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the line break string.
		/// </summary>
		string LineBreak { get; }

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new string which contains the full comment with the
		/// appropriated ending and start charaters.
		/// </summary>
		/// <param name="comment">Comment string.</param>
		/// <returns>Returns the created string.</returns>
		string CreateSingleLineComment(string comment);

		/// <summary>
		/// Creates a new string which contains the full comment with
		/// appropriated ending and start characters.
		/// </summary>
		/// <param name="comment">Comment string.</param>
		/// <returns>Returns the created string.</returns>
		string CreateMultiLineComment(string comment);

		/// <summary>
		/// Creates a new variable declaration string including its assignment.
		/// </summary>
		/// <param name="variableName">Name of the variable (e.g. a).</param>
		/// <param name="variableValue">Value of the variable (e.g. 5).</param>
		/// <returns>Returns the created string.</returns>
		string CreateAssignment(string variableName, object variableValue);

		/// <summary>
		/// Creates a new variable declaration string.
		/// </summary>
		/// <param name="variableName">Name of the variable (e.g. a).</param>
		/// <returns>Returns the created string.</returns>
		string CreateVariableDeclaration(string variableName);

		/// <summary>
		/// Creates a new script function call string.
		/// </summary>
		/// <param name="functionName">Name of the function to call. (e.g. "window.alert")</param>
		/// <param name="arguments">Arguments which are written into the function header as arguments.</param>
		/// <returns>Returns the created string.</returns>
		string CreateFunctionCall(string functionName, params object[] arguments);

		/// <summary>
		/// Serializes the specified object. All properties which are
		/// marked with a ScriptValueType attribute will be serialized. If the
		/// given object or one of its property is derived from IList it will
		/// be serialized as an array.
		/// </summary>
		/// <param name="toSerialze">Object which should be serialized.</param>
		/// <returns>Returns the created string.</returns>
		string SerializeObject(object toSerialze);

		/// <summary>
		/// Deserializes the given script and creates a new object which
		/// contains the deserialized values.
		/// </summary>
		/// <param name="toDeserialze">String which should be deserialized.</param>
		/// <returns>Returns the deserialized object.</returns>
		object DeserializeObject(string toDeserialze);

		/// <summary>
		/// Deserializes the given script and creates assigns the deserialized
		/// values to the appropriated properties of the specified object.
		/// </summary>
		/// <param name="toDeserialze">String which should be deserialized.</param>
		/// <param name="toFill">Object which should be filled.</param>
		/// <returns>Returns the deserialized object.</returns>
		object DeserializeObject(string toDeserialze, object toFill);

		/// <summary>
		/// Creates a new string which is displayed on the client
		/// as output.
		/// </summary>
		/// <param name="toWrite">Value which should be written.</param>
		/// <returns>Returns the created string.</returns>
		string CreatePlainOutput(object toWrite);

		/// <summary>
		/// Creates a new string which is displayed on the client
		/// as output.
		/// </summary>
		/// <param name="toWrite">Value which should be written. This
		/// value will be converted into a client script string.</param>
		/// <returns>Returns the created string.</returns>
		string CreateOutput(object toWrite);

		/// <summary>
		/// Creates a new string which is displayd on the client as
		/// an alert output.
		/// </summary>
		/// <param name="toAlert">Value which should be written. This
		/// value will be converted into a client script string.</param>
		/// <returns>Returns the created string.</returns>
		string CreateAlert(object toAlert);

		/// <summary>
		/// Creates a new exception output string which is displayed on
		/// the client as an alert output.
		/// </summary>
		/// <param name="alertFunction">Alert function which is called on the client to display the exception data.</param>
		/// <param name="exception">Exception which should be displayed on the client.</param>
		/// <returns>Returns the created string.</returns>
		string CreateException(string alertFunction, Exception exception);

		/// <summary>
		/// Creates a new exception output string which is displayed on
		/// the client as an alert output.
		/// </summary>
		/// <param name="exception">Exception which should be displayed on the client.</param>
		/// <returns>Returns the created string.</returns>
		string CreateExceptionAlert(Exception exception);
	}
}

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


namespace JSTools.Context.ScriptGenerator
{
	/// <summary>
	/// 
	/// </summary>
	public class JSScriptGenerator
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

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

		public string CreateSingleLineComment(string comment)
		{
			return null;
		}

		public string CreateMultiLineComment(string comment)
		{
			return null;
		}

		public string CreateAssignment(string variableName, object variableValue)
		{
			return null;
		}

		public string CreateAssignment(string variableName, AScriptType variableValue)
		{
			return null;
		}

		public string CreateVariableDeclaration(string variableName)
		{
			return null;
		}

		public string CreateVariableDeclaration(string variableName, object variableValue)
		{
			return null;
		}

		public string CreateVariableDeclaration(string variableName, AScriptType variableValue)
		{
			return null;
		}

		public string CreateFunctionCall(string functionName, params object[] arguments)
		{
			return null;
		}

		public string CreateFunctionCall(string functionName, params AScriptType[] arguments)
		{
			return null;
		}

		public string SerializeObject(object toSerialze)
		{
			return null;
		}

		public string CreateException(string outputFunction, Exception exception)
		{
			return null;
			/*
			StringBuilder builder = new StringBuilder();

			while (exception != null)
			{
				builder.Append(exception.Message);
				builder.Append("\n");
				builder.Append(exception.StackTrace.ToString());
				builder.Append("\n");
				builder.Append(exception.Message);
				builder.Append(exception.Message);
			}
			CreateFunctionCall(outputFunction, ;
			*/
		}
	}
}

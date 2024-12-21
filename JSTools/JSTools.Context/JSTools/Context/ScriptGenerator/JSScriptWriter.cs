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
using System.Text;

using JSTools;
using JSTools.Config;
using JSTools.ScriptTypes;


namespace JSTools.Context.ScriptGenerator
{
	/// <summary>
	/// 
	/// </summary>
	public class JSScriptWriter : StringWriter
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		public const string NEW_SCRIPT_LINE = "\n";

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		public JSScriptWriter()
		{
		}

		public JSScriptWriter(IFormatProvider formatProvider) : base(formatProvider)
		{
		}

		public JSScriptWriter(StringBuilder builder) : base(builder)
		{
		}

		public JSScriptWriter(StringBuilder builder, IFormatProvider formatProvider) : base(builder, formatProvider)
		{
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		public void AppendSingleLineComment(string comment)
		{
		}

		public void AppendMultiLineComment(string comment)
		{
		}

		public void AppendAssignment(string variableName, object variableValue)
		{
		}

		public void AppendAssignment(string variableName, AScriptType variableValue)
		{
		}

		public void AppendVariableDeclaration(string variableName)
		{
		}

		public void AppendVariableDeclaration(string variableName, object variableValue)
		{
		}

		public void AppendVariableDeclaration(string variableName, AScriptType variableValue)
		{
		}

		public void AppendFunctionCall(string functionName, params object[] arguments)
		{
		}

		public void AppendFunctionCall(string functionName, params AScriptType[] arguments)
		{
		}
	}
}

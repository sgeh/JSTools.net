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
	public interface IScriptCruncher
	{
		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the implementation version.
		/// </summary>
		string ImplementationVersion
		{
			get;
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		#region FormatScript Method

		/// <summary>
		/// Reformats the given script.
		/// </summary>
		/// <param name="toFormat">Script to reformat.</param>
		string FormatScript(string toFormat);

		/// <summary>
		/// Reformats the given script.
		/// </summary>
		/// <param name="toFormat">Script to reformat.</param>
		/// <param name="version">Script version which should be used to parse the script.</param>
		string FormatScript(string toFormat, float version);

		/// <summary>
		/// Reformats the given file.
		/// </summary>
		/// <param name="toFormat">Script to reformat.</param>
		/// <param name="offset">Left offset of the output.</param>
		/// <param name="defaultIndent">Number of indent characters.</param>
		/// <param name="labelSetBack">Number of indent characters for labels.</param>
		/// <param name="useTabs">True to use tabs for indenting. Otherwise spaces will be used.</param>
		/// <param name="newLineBeforeLB">True to insert a line break (and indents) before a left bracket '{'.</param>
		/// <param name="version">Script version which should be used to parse the script.</param>
		string FormatScript(string toFormat, int offset, int defaultIndent, int labelSetBack, bool useTabs, bool newLineBeforeLB, float version);

		#endregion

		#region FormatScriptFile Method

		/// <summary>
		/// Reformats the given script file.
		/// </summary>
		/// <param name="toFormat">Path of the script file to reformat.</param>
		string FormatScriptFile(string toFormat);

		/// <summary>
		/// Reformats the given script file.
		/// </summary>
		/// <param name="toFormat">Path of the script file to reformat.</param>
		/// <param name="version">Script version which should be used to parse the script.</param>
		string FormatScriptFile(string toFormat, float version);

		/// <summary>
		/// Reformats the given script file.
		/// </summary>
		/// <param name="toFormat">Path of the script file to reformat.</param>
		/// <param name="offset">Left offset of the output.</param>
		/// <param name="defaultIndent">Number of indent characters.</param>
		/// <param name="labelSetBack">Number of indent characters for labels.</param>
		/// <param name="useTabs">True to use tabs for indenting. Otherwise spaces will be used.</param>
		/// <param name="newLineBeforeLB">True to insert a line break (and indents) before a left bracket '{'.</param>
		/// <param name="version">Script version which should be used to parse the script.</param>
		string FormatScriptFile(string toFormat, int offset, int defaultIndent, int labelSetBack, bool useTabs, bool newLineBeforeLB, float version);

		#endregion

		#region RemoveComments Method

		/// <summary>
		/// Removes the comments of the given script.
		/// </summary>
		/// <param name="toRemoveComments">Script which should be parsed.</param>
		string RemoveComments(string toRemoveComments);

		/// <summary>
		/// Parses the given script.
		/// </summary>
		/// <param name="toRemoveComments">Script which should be parsed.</param>
		/// <param name="version">Script version which should be used to parse the script.</param>
		string RemoveComments(string toRemoveComments, float version);

		#endregion

		#region RemoveScriptFileComments Method

		/// <summary>
		/// Removes the comment of the given script.
		/// </summary>
		/// <param name="toRemoveComments">File which should be parsed.</param>
		string RemoveScriptFileComments(string toRemoveComments);

		/// <summary>
		/// Parses the given script file.
		/// </summary>
		/// <param name="toRemoveComments">File which should be parsed.</param>
		/// <param name="version">Script version which should be used to parse the file.</param>
		string RemoveScriptFileComments(string toRemoveComments, float version);

		#endregion

		#region CrunchScript Method

		/// <summary>
		/// Crunches the given script.
		/// </summary>
		/// <param name="toCrunch">Script which should be crunched.</param>
		string CrunchScript(string toCrunch);

		/// <summary>
		/// Crunches the given script.
		/// </summary>
		/// <param name="toCrunch">Script which should be crunched.</param>
		/// <param name="sourceName">Name of the script source.</param>
		string CrunchScript(string toCrunch, string sourceName);

		/// <summary>
		/// Crunches the given script.
		/// </summary>
		/// <param name="toCrunch">Script which should be crunched.</param>
		/// <param name="sourceName">Name of the script source.</param>
		/// <param name="version">Script version which should be used to parse the script.</param>
		string CrunchScript(string toCrunch, string sourceName, float version);

		#endregion

		#region CrunchScriptFile Method

		/// <summary>
		/// Crunches the given script file.
		/// </summary>
		/// <param name="fileLocation">Location of the file to crunch.</param>
		string CrunchScriptFile(string fileLocation);

		/// <summary>
		/// Crunches the given script file.
		/// </summary>
		/// <param name="fileLocation">Location of the file to crunch.</param>
		/// <param name="version">Script version which should be used to parse the file.</param>
		string CrunchScriptFile(string fileLocation, float version);

		#endregion

		#region IsValidScript Method

		/// <summary>
		/// Checks whether the given script contains valid script syntax.
		/// </summary>
		/// <param name="toParse">String which should be parsed.</param>
		/// <returns>Returns true, if the given string contains valid script.</returns>
		bool IsValidScript(string toParse);

		/// <summary>
		/// Checks whether the given script contains valid script syntax.
		/// </summary>
		/// <param name="toParse">String which should be parsed.</param>
		/// <returns>Returns true, if the given string contains valid script.</returns>
		bool IsValidScript(string toParse, float version);

		#endregion

		#region IsValidScriptFile Method

		/// <summary>
		/// Checks whether the given file contains valid script syntax.
		/// </summary>
		/// <param name="fileLocation">Script file to parse.</param>
		/// <returns>Returns true, if the given string contains valid script.</returns>
		bool IsValidScriptFile(string fileLocation);

		/// <summary>
		/// Checks whether the given file contains valid script syntax.
		/// </summary>
		/// <param name="fileLocation">Script file to parse.</param>
		/// <returns>Returns true, if the given string contains valid script.</returns>
		bool IsValidScriptFile(string fileLocation, float version);

		#endregion
	}
}

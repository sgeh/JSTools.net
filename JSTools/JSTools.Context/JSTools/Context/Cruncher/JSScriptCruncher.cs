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

using JSTools.Parser.Cruncher;

namespace JSTools.Context.Cruncher
{
	/// <summary>
	/// Represents a cruncher instance, which is saved for multithreading operations. This
	/// instance will not fire any parser warnings.
	/// 
	/// To get a list of all script supported versions, see <see cref="JSTools.Parser.Cruncher.ScriptVersion"/>
	/// </summary>
	public class JSScriptCruncher
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const string ENUM_PATTERN = "Version_{0}_{1}";

		private JSTools.Parser.Cruncher.Cruncher _cruncher = new JSTools.Parser.Cruncher.Cruncher();

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSScriptCruncher instance.
		/// </summary>
		internal JSScriptCruncher()
		{
			// optimize cruncher
			_cruncher.EnableWarnings = false;
		}
		
		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Crunches the given script code and returns the crunched string.
		/// </summary>
		/// <param name="scriptCode"></param>
		/// <param name="scriptVersion">Script version, used by the compiler (e.g. 1.3).</param>
		/// <exception cref="NotSupportedException">The given script version is not supported.</exception>
		/// <exception cref="CruncherException">An error has occured during parsing the given string.</exception>
		public string Crunch(string scriptCode, float scriptVersion)
		{
			lock (this)
			{
				_cruncher.Version = GetScriptVersion(scriptVersion);
				return _cruncher.Crunch(scriptCode);
			}
		}

		/// <summary>
		/// Crunches the given script code and returns the crunched string. The default
		/// script version will be used to crunch the script (javascript1.3).
		/// </summary>
		/// <param name="scriptCode">String to crunch.</param>
		/// <exception cref="CruncherException">An error has occured during parsing the given string.</exception>
		public string Crunch(string scriptCode)
		{
			lock (this)
			{
				_cruncher.Version = ScriptVersion.Default;
				return _cruncher.Crunch(scriptCode);
			}
		}

		/// <summary>
		/// Removes all the comments from the given script code. The default
		/// script version will be used to crunch the script (javascript1.3).
		/// </summary>
		/// <param name="scriptCode">Script code to remove the comments.</param>
		/// <param name="checkSyntax">True, to check for javascript syntax errors.</param>
		/// <returns>Returns the script without comments.</returns>
		/// <exception cref="ArgumentNullException">The given string contains a null reference.</exception>
		/// <exception cref="CruncherException">An error has occured during parsing the given string.</exception>
		public string RemoveComments(string scriptCode, bool checkSyntax)
		{
			lock (this)
			{
				_cruncher.Version = ScriptVersion.Default;
				return _cruncher.RemoveComments(scriptCode, checkSyntax);
			}
		}

		/// <summary>
		/// Removes all the comments from the given script code.
		/// </summary>
		/// <param name="scriptCode">Script code to remove the comments.</param>
		/// <param name="scriptVersion">Script version, used by the compiler (e.g. 1.3).</param>
		/// <param name="checkSyntax">True, to check for javascript syntax errors.</param>
		/// <returns>Returns the script without comments.</returns>
		/// <exception cref="ArgumentNullException">The given string contains a null reference.</exception>
		/// <exception cref="NotSupportedException">The given script version is not supported.</exception>
		/// <exception cref="CruncherException">An error has occured during parsing the given string.</exception>
		public string RemoveComments(string scriptCode, float scriptVersion, bool checkSyntax)
		{
			lock (this)
			{
				_cruncher.Version = GetScriptVersion(scriptVersion);
				return _cruncher.RemoveComments(scriptCode, checkSyntax);
			}
		}

		/// <summary>
		/// Removes all the comments from the given script code. The default
		/// script version will be used to crunch the script (javascript1.3).
		/// </summary>
		/// <param name="scriptCode">Script code to remove the comments.</param>
		/// <returns>Returns the script without comments.</returns>
		/// <exception cref="ArgumentNullException">The given string contains a null reference.</exception>
		/// <exception cref="CruncherException">An error has occured during parsing the given string.</exception>
		public string RemoveComments(string scriptCode)
		{
			lock (this)
			{
				_cruncher.Version = ScriptVersion.Default;
				return _cruncher.RemoveComments(scriptCode);
			}
		}

		/// <summary>
		/// Checks for syntax errors and returns true, if there are no errors.
		/// </summary>
		/// <param name="scriptCode">Script code to check.</param>
		/// <param name="scriptVersion">Script version, used by the compiler (e.g. 1.3).</param>
		/// <returns>Returns true, if there are no errors.</returns>
		/// <exception cref="NotSupportedException">The given script version is not supported.</exception>
		public bool CheckSyntax(string scriptCode, float scriptVersion)
		{
			lock (this)
			{
				_cruncher.Version = GetScriptVersion(scriptVersion);
				return _cruncher.CheckSyntax(scriptCode);
			}
		}

		/// <summary>
		/// Checks for syntax errors and throws an error, if there are errors.
		/// </summary>
		/// <param name="scriptCode">Script code to check.</param>
		/// <param name="scriptVersion">Script version, used by the compiler (e.g. 1.3).</param>
		/// <exception cref="NotSupportedException">The given script version is not supported.</exception>
		/// <exception cref="CruncherException">An error has occured during parsing the given string.</exception>
		/// <exception cref="ArgumentNullException">The given string contains a null reference.</exception>
		public void Check(string scriptCode, float scriptVersion)
		{
			lock (this)
			{
				_cruncher.Version = GetScriptVersion(scriptVersion);
				_cruncher.Check(scriptCode);
			}
		}

		/// <summary>
		/// Checks for syntax errors and throws an error, if there are errors. The default
		/// script version will be used to check the script (javascript1.3).
		/// </summary>
		/// <param name="scriptCode">Script code to check.</param>
		/// <exception cref="CruncherException">An error has occured during parsing the given string.</exception>
		/// <exception cref="ArgumentNullException">The given string contains a null reference.</exception>
		public void Check(string scriptCode)
		{
			lock (this)
			{
				_cruncher.Version = ScriptVersion.Default;
				_cruncher.Check(scriptCode);
			}
		}

		/// <summary>
		/// Converts the given script version into a representing ScriptVersion enum.
		/// </summary>
		/// <param name="scriptVersion">Script version to convert (e.g. 1.3).</param>
		/// <returns>Returns the script version, which represents the given float.</returns>
		private ScriptVersion GetScriptVersion(float scriptVersion)
		{
			short major = (short)(scriptVersion % 10);
			short minor = (short)((scriptVersion * 10) % 10);
			string enumName = string.Format(ENUM_PATTERN, major, minor);

			try
			{
				return (ScriptVersion)Enum.Parse(typeof(ScriptVersion), enumName);
			}
			catch (ArgumentException argExcpetion)
			{
				throw new NotSupportedException("The given script version is not supported!", argExcpetion);
			}
		}
	}
}

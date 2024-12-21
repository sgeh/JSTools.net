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
	/// Will be used, if the CrunchFormater has found a syntax error and has corrected
	/// the error.
	/// </summary>
	public delegate void CrunchSyntaxCorrection(CrunchFormater sender, string errorMessage, string codeToCrunch, int index);


	/// <summary>
	/// Checks the syntax format of the given string and tries to correct syntax errors.
	/// </summary>
	public class CrunchFormater
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private	const	string	INTERNAL_CRUNCH_ERROR		= "Internal Error; Could not correct a syntax error of the given string. Have you implemented the same code twice?";
		private	const	string	CRUNCH_SYNTAX_ERROR			= "Syntax error found: '{0}'. Error corrected.";
		private	const	string	RHINO_ERROR_MESSAGE_ID		= "msg.no.semi.stmt";

		private	Context			_context					= null;
		private	string			_toCheck					= null;
		private	string			_rhinoErrorMessage			= String.Empty;
		private	string			_errorMessage				= String.Empty;


		/// <summary>
		/// Will be fired, if the CorrectScriptString() method has found a syntax error
		/// and has corrected it.
		/// </summary>
		public event CrunchSyntaxCorrection OnSyntaxErrorFound;


		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new CrunchFormater instance.
		/// </summary>
		/// <param name="toCheck"></param>
		/// <param name="scriptContext"></param>
		public CrunchFormater(string toCheck, Context scriptContext)
		{
			if (toCheck == null)
				throw new ArgumentNullException("toCheck", "The given string contains a null reference!");

			if (scriptContext == null)
				throw new ArgumentNullException("scriptContext", "The given context contains a null reference!");

			_toCheck			= toCheck;
			_context			= scriptContext;
			_rhinoErrorMessage	= _context.getMessage(RHINO_ERROR_MESSAGE_ID, new Object[] { });
			_errorMessage		= String.Format(CRUNCH_SYNTAX_ERROR, _rhinoErrorMessage);
		}


		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Checks the given string for validity and tries to correct failures.
		/// Failures can happen if the user has terminated an instruction with a '\n'
		/// instead a ';'. The cruncher will replace the '\n' with a single white
		/// space ' ' or remove the '\n' character and a javascript error occurs.
		/// </summary>
		/// <returns>Returns the corrected string.</returns>
		/// <exception cref="CruncherException">Internal Error; Could not correct all syntax errors
		/// of the given string. Have you implemented the same code twice?</exception>
		public string CorrectScriptString()
		{
			return CheckForSyntaxError(_toCheck, -1);
		}


		/// <summary>
		/// Checks for a syntax error and calls the CorrectSyntaxError method.
		/// </summary>
		/// <param name="toCheck">String to check.</param>
		/// <param name="errorIndex">Error index.</param>
		/// <returns>Returns the corrected string.</returns>
		private string CheckForSyntaxError(string toCheck, int errorIndex)
		{
			try
			{
				_context.parseString(toCheck, String.Empty);
			}
			catch (EvaluatorException evalException)
			{
				int errorOffset = GetErrorOffset(evalException, toCheck);

				if (errorIndex >= errorOffset || evalException.getErrorMessage() != _rhinoErrorMessage)
					throw new CruncherException(INTERNAL_CRUNCH_ERROR);

				string correction = CorrectSyntaxError(toCheck, errorOffset);

				if (correction == null)
					throw new CruncherException(INTERNAL_CRUNCH_ERROR);

				// create crecursion to check the string
				return CheckForSyntaxError(correction, errorOffset);
			}
			catch (Exception e)
			{
				throw new CruncherException(INTERNAL_CRUNCH_ERROR, e);
			}
			return toCheck;
		}


		/// <summary>
		/// Evaluates the error offset of the given EvaluatorException.
		/// </summary>
		/// <param name="evalException">Exception, which contains the invalid offset.</param>
		/// <param name="toCheck">String to check.</param>
		/// <param name="currentOffset">Old error offset.</param>
		/// <returns>Returns the evaluated error offset.</returns>
		private int GetErrorOffset(EvaluatorException evalException, string toCheck)
		{
			int offset = toCheck.IndexOf(evalException.getLine());

			// invalid line source specified
			if (offset == -1)
				return -1;

			return offset + evalException.getOffset();
		}


		/// <summary>
		/// Corrects an occuration of a javascript failure.
		/// </summary>
		/// <param name="toCorrect">String to correct.</param>
		/// <param name="index">Index, at which a javascript error has occured.</param>
		/// <returns>Returns the corrected string.</returns>
		private string CorrectSyntaxError(string toCorrect, int index)
		{
			StringBuilder correctedString = new StringBuilder(toCorrect.Length + 1);

			for (int i = index - 1; i > 0; --i)
			{
				if (Cruncher.IsSeparator(toCorrect[i]))
				{
					// report warning
					ReportWarning(toCorrect, index);

					// correct the failure
					correctedString.Append(toCorrect, 0, i + 1);
					correctedString.Append(";"); // insert ';' to correct the failure
					correctedString.Append(toCorrect, i + 1, toCorrect.Length - i - 1);
					return correctedString.ToString();
				}
				if (Char.IsWhiteSpace(toCorrect[i]))
				{
					// report warning
					ReportWarning(toCorrect, index);

					// correct the failure
					correctedString.Append(toCorrect, 0, i);
					correctedString.Append(";"); // replace white space with a ';' to correct the failure
					correctedString.Append(toCorrect, i + 1, toCorrect.Length - i - 1);
					return correctedString.ToString();
				}
			}
			return null;
		}


		/// <summary>
		/// Fires a new OnSyntaxErrorFound event.
		/// </summary>
		private void ReportWarning(string toCorrect, int index)
		{
			if (OnSyntaxErrorFound != null)
			{
				OnSyntaxErrorFound(this, _errorMessage, toCorrect, index);
			}
		}
	}
}

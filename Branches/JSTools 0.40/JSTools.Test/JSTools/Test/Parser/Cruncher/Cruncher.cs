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

using csUnit;

using System;
using System.IO;

using JSTools;
using JSTools.Parser.Cruncher;

namespace JSTools.Test.Parser.Cruncher
{
	/// <summary>
	/// Summary description for Cruncher.
	/// </summary>
	[csUnit.TestFixture]
	public class Cruncher
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private JSTools.Parser.Cruncher.Cruncher _cruncher = null;
		private string _fileContent = string.Empty;
		private bool _onWarnOccured = false;


		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new test instance.
		/// </summary>
		public Cruncher()
		{
			System.Console.Out.WriteLine(System.Console.Out.NewLine + "CrunchTest started at " + DateTime.Now);
		}


		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Initialize this test instance.
		/// </summary>
		[csUnit.SetUp()]
		public void SetUp()
		{
			_cruncher = new JSTools.Parser.Cruncher.Cruncher();
		}


		/// <summary>
		/// Clear up this test instance.
		/// </summary>
		[csUnit.TearDown()]
		public void ClearUp()
		{
		}


		[csUnit.Test()]
		public void Check()
		{
			System.Console.Out.WriteLine("Cruncher.Check()");
	
			_cruncher.Check(LoadFile());

			System.Console.Out.WriteLine(" done");
		}


		[csUnit.Test()]
		public void CheckSyntax()
		{
			System.Console.Out.WriteLine("Cruncher.CheckSyntax()");
	
			if (!_cruncher.CheckSyntax(LoadFile()))
				throw new TestFailed("The CheckSyntax method has returned a syntax error!");

			System.Console.Out.WriteLine(" done");
		}


		[csUnit.Test()]
		public void Crunch()
		{
			System.Console.Out.WriteLine("Cruncher.Crunch()");
	
			string crunchedString = _cruncher.Crunch(LoadFile());
			string crunchOutput = ((crunchedString.Length > 120) ? crunchedString.Substring(0, 120) : crunchedString);

			System.Console.Out.WriteLine(" is " + crunchOutput + "...");
			System.Console.Out.WriteLine(" should [not empty]");

			if (crunchedString == null || crunchedString.Length == 0)
				throw new TestFailed("Crunch() has returned an empty string!");

			WriteFile(crunchedString);

			System.Console.Out.WriteLine(" done");
		}


		[csUnit.Test()]
		public void EnableWarnings()
		{
			System.Console.Out.WriteLine("Cruncher.EnableWarnings");

			_cruncher.EnableWarnings = true;
			_cruncher.Crunch(LoadFile());

			System.Console.Out.WriteLine(" is " + _cruncher.EnableWarnings);
			System.Console.Out.WriteLine(" should [True]");

			if (!_cruncher.EnableWarnings)
				throw new TestFailed("The EnableWarnings property was set to true but the value was not stored!");

			System.Console.Out.WriteLine(" done");
		}


		[csUnit.Test()]
		public void Version()
		{
			System.Console.Out.WriteLine("Cruncher.Version");

			_cruncher.Version = JSTools.Parser.Cruncher.ScriptVersion.Version_1_3;
			_cruncher.Crunch(LoadFile());

			System.Console.Out.WriteLine(" is " + (int)_cruncher.Version);
			System.Console.Out.WriteLine(" should [130]");

			if ((int)_cruncher.Version != 130)
				throw new TestFailed("The Version property was set to [Version_1_3] but the value was not stored!");

			System.Console.Out.WriteLine(" done");
		}


		[csUnit.Test()]
		public void Language()
		{
			System.Console.Out.WriteLine("Cruncher.Language");

			_cruncher.Language = new System.Globalization.CultureInfo("de-CH");
			_cruncher.Crunch(LoadFile());

			System.Console.Out.WriteLine(" is " + _cruncher.Language.Name);
			System.Console.Out.WriteLine(" should [de-CH]");

			if (_cruncher.Language.Name != "de-CH")
				throw new TestFailed("The Language property was set to [de-CH] but the value was not stored!");

			System.Console.Out.WriteLine(" done");
		}


		[csUnit.Test()]
		public void RemoveComments()
		{
			System.Console.Out.WriteLine("Cruncher.RemoveComments()");

			string output = _cruncher.RemoveComments(LoadFile(), true);
			string removedComment = ((output.Length > 120) ? output.Substring(0, 120) : output);

			System.Console.Out.WriteLine(" is " + removedComment + "...");
			System.Console.Out.WriteLine(" should [not empty]");

			if (removedComment == null || removedComment.Length == 0)
				throw new TestFailed("The returned string is an empty string!");

			System.Console.Out.WriteLine(" done");
		}


		[csUnit.Test()]
		public void TestOnWarnEvent()
		{
			System.Console.Out.WriteLine("AJSExceptionHandler.OnWarn");

			_cruncher.EnableWarnings = true;
			_cruncher.Warn += new JSTools.Parser.Cruncher.CrunchWarningHandler(OnWarn);
			_cruncher.Crunch(LoadFile());

			if (!_onWarnOccured)
				throw new TestFailed("The OnWarn event was never fired!");

			System.Console.Out.WriteLine(" done");
		}


		private void OnWarn(JSTools.Parser.Cruncher.Cruncher sender, JSTools.Parser.Cruncher.CruncherWarning warning)
		{
			System.Console.Out.WriteLine(" -Cruncher.OnWarn");

			_onWarnOccured = true;

			System.Console.Out.WriteLine(" is " + sender);
			System.Console.Out.WriteLine(" should [not null]");

			if (sender == null)
				throw new TestFailed("The given sender instance contains a null reference!");

			System.Console.Out.WriteLine(" is " + warning.ToString());
			System.Console.Out.WriteLine(" should [not null]");

			if (warning == null)
				throw new TestFailed("The given warning contains a null reference!");
		}


		private string LoadFile()
		{
			if (_fileContent != null && _fileContent.Length != 0)
				return _fileContent;

			StreamReader reader = null;

			try
			{
				reader = new StreamReader(Settings.Instance.CrunchFilePath);
				return (_fileContent = reader.ReadToEnd());
			}
			finally
			{
				if (reader != null)
				{
					reader.Close();
				}
			}
		}


		private void WriteFile(string toWrite)
		{
			StreamWriter writer = null;

			try
			{
				writer = new StreamWriter(Settings.Instance.CrunchSavePath);
				writer.Write(toWrite);
			}
			finally
			{
				if (writer != null)
				{
					writer.Close();
				}
			}
		}
	}
}

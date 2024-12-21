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

using JSTools;
using JSTools.Parser.Cruncher;
using JSTools.Test.Resources;

using NUnit.Framework;

namespace JSTools.Test.Parser.Cruncher
{
	/// <summary>
	/// Summary description for Cruncher.
	/// </summary>
	[TestFixture]
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
		}

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Initialize this test instance.
		/// </summary>
		[SetUp()]
		public void SetUp()
		{
			_cruncher = new JSTools.Parser.Cruncher.Cruncher();
		}

		/// <summary>
		/// Clear up this test instance.
		/// </summary>
		[TearDown()]
		public void TearDown()
		{
			_cruncher = null;
		}

		[Test()]
		public void Check()
		{
			_cruncher.Check(LoadFile());
		}

		[Test()]
		public void CheckSyntax()
		{
			Assert.IsTrue(_cruncher.CheckSyntax(LoadFile()));
		}

		[Test()]
		public void Crunch()
		{
			string crunchedString = _cruncher.Crunch(LoadFile());
			string crunchOutput = ((crunchedString.Length > 120) ? crunchedString.Substring(0, 120) : crunchedString);

			Assert.IsFalse(crunchedString == null || crunchedString.Length == 0);

			// wirte down the file to manually check if the file was crunched
			WriteFile(crunchedString);
		}

		[Test()]
		public void EnableWarnings()
		{
			_cruncher.EnableWarnings = true;
			_cruncher.Crunch(LoadFile());

			Assert.IsTrue(_cruncher.EnableWarnings);
		}

		[Test()]
		public void Version()
		{
			_cruncher.Version = JSTools.Parser.Cruncher.ScriptVersion.Version_1_3;
			_cruncher.Crunch(LoadFile());

			Assert.AreEqual((int)_cruncher.Version, 130);
		}

		[Test()]
		public void Language()
		{
			_cruncher.Language = new System.Globalization.CultureInfo("de-CH");
			_cruncher.Crunch(LoadFile());

			Assert.AreEqual(_cruncher.Language.Name, "de-CH");
		}

		[Test()]
		public void RemoveComments()
		{
			string output = _cruncher.RemoveComments(LoadFile(), true);
			string removedComment = ((output.Length > 120) ? output.Substring(0, 120) : output);

			Assert.IsFalse(removedComment == null || removedComment.Length == 0);
		}

		[Test()]
		public void TestOnWarnEvent()
		{
			_cruncher.EnableWarnings = true;
			_cruncher.Warn += new JSTools.Parser.Cruncher.CrunchWarningHandler(OnWarn);
			_cruncher.Crunch(LoadFile());

			Assert.IsTrue(_onWarnOccured, "OnWarn event was not fired!");
		}

		private void OnWarn(JSTools.Parser.Cruncher.Cruncher sender, JSTools.Parser.Cruncher.CruncherWarning warning)
		{
			_onWarnOccured = true;

			Assert.IsNotNull(sender, "The given sender instance contains a null reference.");
			Assert.IsNotNull(warning, "The given warning contains a null reference.");
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

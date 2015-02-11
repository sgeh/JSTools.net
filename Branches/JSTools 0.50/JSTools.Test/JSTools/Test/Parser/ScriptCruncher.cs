/*
 * JSTools.Test.dll / JSTools.net - A framework for JavaScript/ASP.NET applications.
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

		private ScriptCruncher _cruncher = null;
		private string _fileContent = string.Empty;

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
			_cruncher = ScriptCruncher.Instance;
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
		public void CrunchScriptFile()
		{
			string output = _cruncher.CrunchScriptFile(Settings.Instance.CrunchFilePath);
			Assert.IsFalse(output == null || output.Length == 0);

			// wirte down the file to manually check if the file was crunched
			WriteFile(output);
		}

		[Test()]
		public void CrunchScript()
		{
			//string output = _cruncher.CrunchScript("var crunchy={a3:3,b6:{},c0:{},d5:{},d3i:\"<tes\\ \t\",e0$:' testy\\n',_d3e:false,ar:[38,56,'string',[0,2,4],{}]}", "Crunch Script Test", ScriptVersion.Version_1_3);
			string output = _cruncher.CrunchScript(ReadFile(), "Crunch Script Test", ScriptVersion.Version_1_3);
			Assert.IsFalse(output == null || output.Length == 0);
		}

		[Test()]
		public void IsValidScriptFile()
		{
			Assert.IsTrue(_cruncher.IsValidScriptFile(Settings.Instance.CrunchFilePath));
		}

		
		[Test()]
		public void RemoveComments()
		{
			string output = _cruncher.RemoveComments(ReadFile());
			Assert.IsFalse(output == null || output.Length == 0);
		}

		[Test()]
		public void RemoveScriptFileComments()
		{
			string output = _cruncher.RemoveScriptFileComments(Settings.Instance.CrunchFilePath);
			Assert.IsFalse(output == null || output.Length == 0);
		}

		private void WriteFile(string toWrite)
		{
			using (StreamWriter writer = new StreamWriter(Settings.Instance.CrunchSavePath))
			{
				writer.Write(toWrite);
			}
		}

		private string ReadFile()
		{
			using (StreamReader reader = new StreamReader(Settings.Instance.CrunchFilePath))
			{
				return reader.ReadToEnd();
			}
		}
	}
}

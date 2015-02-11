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
using JSTools.Parser.DocGenerator;
using JSTools.Test.Resources;

using NUnit.Framework;

namespace JSTools.Test.Parser.DocGenerator
{
	/// <summary>
	/// Summary description for DocGenerator.
	/// </summary>
	[TestFixture]
	public class DocGenerator
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new test instance.
		/// </summary>
		public DocGenerator()
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
		}

		/// <summary>
		/// Clear up this test instance.
		/// </summary>
		[TearDown()]
		public void TearDown()
		{
		}

		[Test()]
		public void GenerateDoc()
		{
			DocumentationGenerator generator = new DocumentationGenerator();
			generator.ScriptFilePath = Settings.Instance.DocGeneratorFilePath;
			generator.GenerateDoc();
		}
	}
}

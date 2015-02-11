/*
 * JSTools.Parser.DocGenerator.dll / JSTools.net - A framework for JavaScript/ASP.NET applications.
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
using System.Collections;
using System.IO;

using JSTools.Parser;

namespace JSTools.Parser.DocGenerator
{
	/// <summary>
	/// This class can be used to parse the comment tags of javascript
	/// source files. After parsing the code comments, two output files
	/// will be generated. The first file contains the public and protected
	/// interface of the javascript classes and procedures as a .NET
	/// assembly. The second file contain the parsed XML comments itself.
	/// </summary>
	/// <remarks>
	/// The xml comments will be parsed as follows:
	/// 
	/// <list type="table">
	///  <listheader>
	///   <term>Comment-Tag</term>
	///   <description>Description</description>
	///  </listheader>
	///  <item>
	///   <term><include file="Docs/DocumentationGenerator.xml" path="/doc/enum/example" /></term>
	///   <include file="Docs/DocumentationGenerator.xml" path="/doc/enum/description" />
	///  </item>
	///  <item>
	///   <term><include file="Docs/DocumentationGenerator.xml" path="/doc/class/example" /></term>
	///   <include file="Docs/DocumentationGenerator.xml" path="/doc/class/description" />
	///  </item>
	///  <item>
	///   <term><include file="Docs/DocumentationGenerator.xml" path="/doc/function/example" /></term>
	///   <include file="Docs/DocumentationGenerator.xml" path="/doc/function/description" />
	///  </item>
	///  <item>
	///   <term><include file="Docs/DocumentationGenerator.xml" path="/doc/variable/example" /></term>
	///   <include file="Docs/DocumentationGenerator.xml" path="/doc/variable/description" />
	///  </item>
	/// </list>
	/// 
	/// <para>
	/// The following tags may be applied to the comment tags listed above.
	/// </para>
	/// 
	/// <list type="table">
	///  <listheader>
	///   <term>Description-Tag</term>
	///   <description>Description</description>
	///  </listheader>
	///  <item>
	///   <term><include file="Docs/DocumentationGenerator.xml" path="/doc/returns/example" /></term>
	///   <include file="Docs/DocumentationGenerator.xml" path="/doc/returns/description" />
	///  </item>
	///  <item>
	///   <term><include file="Docs/DocumentationGenerator.xml" path="/doc/param/example" /></term>
	///   <include file="Docs/DocumentationGenerator.xml" path="/doc/param/description" />
	///  </item>
	///  <item>
	///   <term><include file="Docs/DocumentationGenerator.xml" path="/doc/field/example" /></term>
	///   <include file="Docs/DocumentationGenerator.xml" path="/doc/field/description" />
	///  </item>
	///  <item>
	///   <term><include file="Docs/DocumentationGenerator.xml" path="/doc/example/example" /></term>
	///   <include file="Docs/DocumentationGenerator.xml" path="/doc/example/description" />
	///  </item>
	///  <item>
	///   <term><include file="Docs/DocumentationGenerator.xml" path="/doc/remarks/example" /></term>
	///   <include file="Docs/DocumentationGenerator.xml" path="/doc/remarks/description" />
	///  </item>
	///  <item>
	///   <term><include file="Docs/DocumentationGenerator.xml" path="/doc/exception/example" /></term>
	///   <include file="Docs/DocumentationGenerator.xml" path="/doc/exception/description" />
	///  </item>
	/// </list>
	/// </remarks>
	public class DocumentationGenerator
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private string _defaultListType = "Global.Window.Array";
		private string _defaultClass = "Window";
		private string _defaultNameSpace = "Global";
		private string _defaultBaseType = "Global.Window.Object";

		private string _scriptFilePath = string.Empty;
		private string _script = string.Empty;
		private string _extension = string.Empty;

		private string _includeFilePath = string.Empty;
		private string _outputFileName = string.Empty;
		private string _outputFilePath = string.Empty;
		private bool _throwErrors = true;

		private ArrayList _errors = new ArrayList();

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		public virtual string DefaultListType
		{
			get { return _defaultListType; }
			set
			{
				if (value != null && value.Length > 0)
					_defaultListType = value;
			}
		}

		public virtual string DefaultClass
		{
			get { return _defaultClass; }
			set
			{
				if (value != null && value.Length > 0)
					_defaultClass = value;
			}
		}

		public virtual string DefaultNameSpace
		{
			get { return _defaultNameSpace; }
			set
			{
				if (value != null && value.Length > 0)
					_defaultNameSpace = value;
			}
		}

		public virtual string DefaultBaseType
		{
			get { return _defaultBaseType; }
			set { _defaultBaseType = value; }
		}

		public bool ThrowErrors
		{
			get { return _throwErrors; }
			set { _throwErrors = value; }
		}

		public string ScriptFileExtension
		{
			get { return _extension; }
			set { _extension = (value != null) ? value : string.Empty; }
		}

		public string ScriptFilePath
		{
			get { return _scriptFilePath; }
			set
			{
				if (value == null || value.Length == 0)
					throw new ArgumentException("Invalid script file path specified.", "value");

				if (!Directory.Exists(value))
					throw new ArgumentException("The given path does not exist.");

				_scriptFilePath = value;
			}
		}

		public string Script
		{
			get { return _script; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value", "Invalid script specified.");

				_script = value; 
			}
		}

		public string IncludeFilePath
		{
			get { return _includeFilePath; }
			set
			{
				if (value == null || !Directory.Exists(value))
					throw new ArgumentException("The given path does not exist.");

				_includeFilePath = value;
			}
		}

		public string OutputFileName
		{
			get { return _outputFileName; }
			set { _outputFileName = (value != null) ? value : string.Empty; }
		}

		private string OutputFilePath
		{
			get { return _outputFilePath; }
			set { _outputFilePath = (value != null) ? value : string.Empty; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new DocumentationGenerator instance.
		/// </summary>
		public DocumentationGenerator()
		{
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		public virtual void GenerateDoc()
		{
		}

		/// <summary>
		/// Generates the documentation for the given script file.
		///  <paramref name="filePath" />
		/// </summary>
		/// <param name="filePath">Full path of the file to parse.</param>
		public virtual void GenerateDoc(string filePath)
		{
			using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				using (StreamReader reader = new StreamReader(file, true))
				{
					CommentItemContext context = new CommentItemContext(
						OutputFileName,
						OutputFilePath,
						DefaultClass,
						DefaultNameSpace,
						DefaultListType,
						DefaultBaseType );
					context.ParseString(filePath, reader.ReadToEnd(), ThrowErrors);
					context.LoadIncludes();
					context.CreateType();
					context.InitType();
					context.ResolveReferences();
					context.Save(Path.Combine(OutputFilePath, OutputFileName));
				}
			}
		}
	}
}

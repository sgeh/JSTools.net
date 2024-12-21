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
using System.Xml;

using JSTools.Parser;

namespace JSTools.Parser.DocGenerator
{
	/// <summary>
	/// Represents a loader class which loads the specified xml files
	/// and evaluates the associated xml node.
	/// </summary>
	internal class IncludeManager
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private string _commentFilePath = null;
		private Hashtable _loadedFiles = new Hashtable();

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------


		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new IncludeManager instance.
		/// </summary>
		internal IncludeManager(string commentFilePath)
		{
			_commentFilePath = commentFilePath;
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Initializes the specified file and returns the node associated
		/// to the given file name.
		/// </summary>
		/// <param name="fileName">File name to load.</param>
		/// <param name="xPath">X-Path instruction to evaluate.</param>
		/// <returns>Returns the evaluated XmlNode.</returns>
		/// <exception cref="ArgumentNullException">The given file name is empty.</exception>
		/// <exception cref="ArgumentNullException">The given xpath is empty.</exception>
		/// <exception cref="ArgumentException">The given xpath is invalid.</exception>
		/// <exception cref="ArgumentException">Invalid XML file specified.</exception>
		public XmlNode GetNodeFromFile(string fileName, string xPath)
		{
			if (fileName == null || fileName.Length == 0)
				throw new ArgumentNullException("The given file name is empty.");

			if (xPath == null || xPath.Length == 0)
				throw new ArgumentNullException("The given xpath is empty.");

			XmlDocument file = LoadFile(fileName);
			XmlNode loadedNode = file.SelectSingleNode(xPath);

			if (loadedNode == null)
				throw new ArgumentException("The given xpath is invalid.");

			return loadedNode;
		}

		private XmlDocument LoadFile(string fileName)
		{
			fileName = GetPath(fileName);

			if (_loadedFiles[fileName] == null)
			{
				try
				{
					XmlDocument toLoad = new XmlDocument();
					toLoad.Load(fileName);
					_loadedFiles[fileName] = toLoad;
				}
				catch (Exception e)
				{
					throw new ArgumentException("Invalid XML file specified.", "fileName", e);
				}
			}
			return (XmlDocument)_loadedFiles[fileName];
		}

		private string GetPath(string pathToLoad)
		{
			string fileName = pathToLoad;

			if (!Path.IsPathRooted(fileName))
			{
				if (_commentFilePath != null)
					fileName = Path.Combine(_commentFilePath, fileName);
				else
					Path.GetFullPath(fileName);
			}
			return fileName;
		}
	}
}

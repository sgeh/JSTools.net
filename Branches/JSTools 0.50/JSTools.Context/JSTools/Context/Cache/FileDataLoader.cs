/*
 * JSTools.Context.dll / JSTools.net - A framework for JavaScript/ASP.NET applications.
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

using JSTools.Parser.Cruncher;

namespace JSTools.Context.Cache
{
	/// <summary>
	/// Represents the loader for a script file.
	/// </summary>
	internal class FileDataLoader : AJScriptDataLoader
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private string _scriptPath = null;
		private DateTime _lastReadTime = DateTime.MinValue;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Override refresh cache property to determine that the right file
		/// version is published.
		/// </summary>
		public override bool RefreshCache
		{
			get { return (_lastReadTime < File.GetLastWriteTime(_scriptPath)); }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new FileDataLoader instance.
		/// </summary>
		internal FileDataLoader(string scriptFilePath, float version) : base(version)
		{
			if (scriptFilePath == null)
				throw new ArgumentNullException("scriptFilePath");

			_scriptPath = scriptFilePath;
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Reads out the data of the handle specified by the DataHandle property.
		/// </summary>
		/// <returns>Returns the read script code data in a string format.</returns>
		protected override string LoadData()
		{
			FileInfo info = new FileInfo(_scriptPath);
			_lastReadTime = info.LastWriteTime;

			using (FileStream fileStream = info.OpenRead())
			{
				using (StreamReader reader = new StreamReader(fileStream))
				{
					return reader.ReadToEnd();
				}
			}
		}
	}
}

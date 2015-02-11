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

using JSTools.Context.Cruncher;

namespace JSTools.Context.Cache
{
	/// <summary>
	/// Represents file cache bucket used to cache and crunch script files.
	/// </summary>
	internal class JSFileCacheBucket : AJSCacheBucket
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private string _scriptPath = null;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the current cache time of the script given by the DataHandle object.
		/// </summary>
		protected override DateTime CurrentCacheTime
		{
			get { return File.GetLastWriteTime(ScriptPath); }
		}

		private string ScriptPath
		{
			get 
			{
				if (_scriptPath == null)
				{
					_scriptPath = (DataHandle as string);

					if (_scriptPath == null)
						throw new ArgumentException("The given DataHandle does not contain a script path!");

					if (!File.Exists(_scriptPath))
						throw new ArgumentException("Could not find a part of path '" + _scriptPath + "'!");
				}
				return _scriptPath;
			}
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new AJSCacheBucket instance.
		/// </summary>
		internal JSFileCacheBucket(JSScriptCruncher cruncher, float scriptVersion, bool checkSyntax, bool crunch, object dataHandle) :
			base (cruncher, scriptVersion, checkSyntax, crunch, dataHandle)
		{
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
		protected override string GetDataFromHandle()
		{
			using (FileStream fileStream = File.Open(ScriptPath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				using (StreamReader reader = new StreamReader(fileStream))
				{
					return reader.ReadToEnd();
				}
			}
		}
	}
}

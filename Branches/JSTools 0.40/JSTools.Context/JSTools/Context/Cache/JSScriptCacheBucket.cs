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

using JSTools.Context.Cruncher;

namespace JSTools.Context.Cache
{
	/// <summary>
	/// Represents file cache bucket used to cache and crunch scripts.
	/// </summary>
	internal class JSScriptCacheBucket : AJSCacheBucket
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private DateTime _dateLastUpdate = DateTime.MinValue;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the current cache time of the script given by the DataHandle object.
		/// </summary>
		protected override DateTime CurrentCacheTime
		{
			get
			{
				if (_dateLastUpdate == DateTime.MinValue)
					_dateLastUpdate = DateTime.Now;

				return _dateLastUpdate;
			}
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new AJSCacheBucket instance.
		/// </summary>
		internal JSScriptCacheBucket(JSScriptCruncher cruncher, float scriptVersion, bool checkSyntax, bool crunch, object dataHandle) :
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
			string script = (DataHandle as string);

			if (script == null)
				return string.Empty;
			else
                return script;
		}
	}
}

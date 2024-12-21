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

using JSTools.Parser.Cruncher;

namespace JSTools.Context.Cache
{
	/// <summary>
	/// Represents the loader for a script.
	/// </summary>
	internal class ScriptDataLoader : AJScriptDataLoader
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private string _scriptData;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new ScriptDataLoader instance.
		/// </summary>
		internal ScriptDataLoader(string scriptData, float version) : base(version)
		{
			if (scriptData == null)
				throw new ArgumentNullException("scriptData");

			_scriptData = scriptData;
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the cached script data.
		/// </summary>
		protected override string LoadData()
		{
			return _scriptData;
		}
	}
}

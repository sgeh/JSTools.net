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

using JSTools.Parser.Cruncher;

namespace JSTools.Context.Cache
{
	/// <summary>
	/// Represents the base class for all script data loader. A data loader
	/// is used to lazily load the script data at run time. You should
	/// override the RefreshCache property if the script data will change
	/// during the life time the cached item.
	/// 
	/// <para>
	/// The default implementation supports JavaScript/JScript only. If
	/// you'd like to support other languages, you have to override the
	/// ParseScript/CrunchScript methods.
	/// </para>
	/// </summary>
	public abstract class AJScriptDataLoader : ICacheDataLoader
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const string SUPPORTED_SCRIPT_TYPE = "text/javascript";
		private float _version = -1;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Returns true if the cached item has been chanced and should be
		/// refreshed.
		/// </summary>
		public virtual bool RefreshCache
		{
			get { return false; }
		}

		/// <summary>
		/// Gets the type of script which is supported with this data loader.
		/// </summary>
		public string SupportedScriptType
		{
			get { return SUPPORTED_SCRIPT_TYPE; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new ACacheDataLoader instance.
		/// </summary>
		/// <param name="version">Script version which is used to crunch the script.</param>
		internal AJScriptDataLoader(float version)
		{
			_version = version;
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Loads the requested script data.
		/// </summary>
		/// <param name="checkSyntax">True to force a syntax check.</param>
		/// <returns>Returns the loaded data. The returing is cached until the
		/// RefreshCache flag is set to true.</returns>
		public string LoadScript(bool checkSyntax)
		{
			string loadedData = LoadData();

			if (checkSyntax)
				ParseScript(loadedData);

			return loadedData;
		}

		/// <summary>
		/// Loads the requested script data. After loading the data, the data
		/// is cached by the data loader.
		/// </summary>
		/// <returns>Returns the loaded data. The returing is cached until the
		/// RefreshCache flag is set to true.</returns>
		public string LoadCrunchedScript()
		{
			return CrunchScript(LoadData());
		}

		/// <summary>
		/// Loads the data of the cached item.
		/// </summary>
		/// <returns>Returns the loaded script data.</returns>
		protected abstract string LoadData();

		/// <summary>
		/// Parses the script with the specified script version.
		/// </summary>
		/// <param name="loadedData">Script which should be parsed.</param>
		protected virtual void ParseScript(string loadedData)
		{
			ScriptCruncher.Instance.ParseScript(loadedData, ScriptVersionUtil.ValueToScriptVersion(_version));
		}

		/// <summary>
		/// Crunches the script with the specified script version.
		/// </summary>
		/// <param name="loadedData">Script which should be crunched.</param>
		protected virtual string CrunchScript(string loadedData)
		{
			return ScriptCruncher.Instance.CrunchScript(LoadData(), null, ScriptVersionUtil.ValueToScriptVersion(_version));
		}
	}
}

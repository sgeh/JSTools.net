/*
 * JSTools.Parser.Cruncher.dll / JSTools.net - A framework for JavaScript/ASP.NET applications.
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

namespace JSTools.Parser.Cruncher
{
	/// <summary>
	/// Language versions. All integral values are
	/// reserved for future version numbers.
	/// </summary>
	public enum ScriptVersion : short
	{
		Unkonwn = -1,
		Default = 0,
		Version_1_0 = 100,
		Version_1_1 = 110,
		Version_1_2 = 120,
		Version_1_3 = 130,
		Version_1_4 = 140,
		Version_1_5 = 150,
	}

	/// <summary>
	/// Contains some convert methods which can be used to convert a
	/// script version enum into a float.
	/// </summary>
	public sealed class ScriptVersionUtil
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// This class cannot be instantiated and provides global functionalities.
		/// </summary>
		private ScriptVersionUtil()
		{
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Parses the given value and tries to convert it into a ScriptVersion
		/// value. You will obtain ScriptVersion.Unkown if the specified value
		/// cannot be converted.
		/// </summary>
		/// <param name="toParse">Integer value to parse (e.g. 1.0 / 1.2 / ...).</param>
		/// <returns>Returns the specified value or -1 if the value could not
		/// be parsed.</returns>
		public static ScriptVersion ValueToScriptVersion(float toParse)
		{
			try
			{
				return (ScriptVersion)Enum.ToObject(typeof(ScriptVersion), (short)(toParse * 100));
			}
			catch
			{
				return ScriptVersion.Unkonwn;
			}
		}

		/// <summary>
		/// Converts the specified script version enum into a float value
		/// (e.g. 1.0 / 1.2 / ...).
		/// </summary>
		/// <param name="toConvert">Enum object which should be converted.</param>
		/// <param name="defaultValue">Default value if the script version
		/// value contains ScriptVersion.Default.</param>
		/// <returns>Returns the specified value or -1 if toConvert contains
		/// ScriptVersion.Unkown.</returns>
		public static float ScriptVersionToValue(ScriptVersion toConvert, float defaultValue)
		{
			short convertedValue = (short)toConvert;

			if (convertedValue == 0)
				return defaultValue;

			if (convertedValue == -1)
				return convertedValue;

			return (convertedValue / 100);
		}
	}
}

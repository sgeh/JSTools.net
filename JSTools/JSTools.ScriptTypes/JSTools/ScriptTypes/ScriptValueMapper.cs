/*
 * JSTools.ScriptTypes.dll / JSTools.net - A framework for JavaScript/ASP.NET applications.
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

namespace JSTools.ScriptTypes
{
	/// <summary>
	/// Represents a mapper which can be used to map types with script
	/// types and map script strings with managed types.
	/// 
	/// <para>
	/// The mapped type can be used to decode values form the client
	/// or the encode data to render them to the client.
	/// </para>
	/// </summary>
	/// <example>
	///  <code>
	///  // a value which should be written to the client
	///  string valueToMap = "a dummy \t value";
	///  
	///  // create a new ScriptValueMapper instance
	///  ScriptValueMapper mapper = new ScriptValueMapper();
	///  
	///  // map the type, this will return the appropriated script type
	///  AScriptType mappedType = mapper.MapType(aValueToMap.GetType());
	///  
	///  // get string representation, which may be rendered to the client in a
	///  // &lt;script&gt; tag, e.g. var a = &lt;%# scriptString %&gt;
	///  </code>
	/// </example>
	public class ScriptValueMapper
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private static readonly AScriptType[] DEFAULT_MAPPINGS = {
																	 new JSTools.ScriptTypes.Boolean(),
																	 new JSTools.ScriptTypes.Number(),
																	 new JSTools.ScriptTypes.RegExp(),
																	 new JSTools.ScriptTypes.Object(),
																	 new JSTools.ScriptTypes.String()
																 };

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Represents the default type mapping and it's used if no other
		/// type can be mapped.
		/// </summary>
		protected virtual AScriptType DefaultTypeMapping
		{
			get { return DEFAULT_MAPPINGS[3]; }
		}

		/// <summary>
		/// Represents the default value type mapping and it's used if no
		/// other type can be mapped.
		/// </summary>
		protected virtual AScriptType DefaultValueMapping
		{
			get { return DEFAULT_MAPPINGS[4]; }
		}

		/// <summary>
		/// Gets the implemented mappings.
		/// </summary>
		protected virtual AScriptType[] Mappings
		{
			get { return DEFAULT_MAPPINGS; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new ScriptValueMapper instance.
		/// </summary>
		public ScriptValueMapper()
		{
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Maps the specified object type with a managed script type.
		/// Default map type is Object.
		/// </summary>
		/// <param name="typeToMap">Type which should be mapped.</param>
		/// <returns>Returns the mapped script type or a null reference, if the given string contains a null reference.</returns>
		public virtual AScriptType MapType(Type typeToMap)
		{
			if (typeToMap == null)
				return null;

			foreach (AScriptType scriptType in Mappings)
			{
				if (scriptType == null || scriptType == DefaultTypeMapping)
					continue;

				if (scriptType.CanMapType(typeToMap))
					return scriptType;
			}
			return DefaultTypeMapping;
		}

		/// <summary>
		/// Maps the specified script value (e.g. "[ 20, 210, 'a', { } ]")
		/// with a managed script type. Default map type is String.
		/// </summary>
		/// <param name="valueToMap">Script value which should be mapped.</param>
		/// <returns>Returns the mapped type or a null reference, if the given string contains a null reference.</returns>
		public virtual AScriptType MapValue(string valueToMap)
		{
			if (valueToMap == null)
				return null;

			foreach (AScriptType scriptType in Mappings)
			{
				if (scriptType == null || scriptType == DefaultValueMapping)
					continue;

				if (scriptType.IsTypeOf(valueToMap))
					return scriptType;
			}
			return DefaultValueMapping;
		}
	}
}

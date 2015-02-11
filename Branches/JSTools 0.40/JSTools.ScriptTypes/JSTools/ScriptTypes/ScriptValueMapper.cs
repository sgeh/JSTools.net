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
using System.Collections;

namespace JSTools.ScriptTypes
{
	public class ScriptValueMapper
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private static readonly AScriptType[] DEFAULT_MAPPINGS = {
																	 new JSTools.ScriptTypes.Boolean(),
																	 new JSTools.ScriptTypes.Number(),
																	 new JSTools.ScriptTypes.RegExp(),
																	 new JSTools.ScriptTypes.Array(),
																	 new JSTools.ScriptTypes.Object(),
																	 new JSTools.ScriptTypes.String()
																 };

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		protected virtual AScriptType DefaultTypeMapping
		{
			get { return DEFAULT_MAPPINGS[4]; }
		}

		protected virtual AScriptType DefaultValueMapping
		{
			get { return DEFAULT_MAPPINGS[5]; }
		}

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

		public virtual AScriptType MapType(object objectToMap)
		{
			if (objectToMap != null)
			{
				foreach (AScriptType scriptType in Mappings)
				{
					if (scriptType == null || scriptType == DefaultTypeMapping)
						continue;

					foreach (Type managedType in scriptType.ManagedTypes)
					{
						if (objectToMap.GetType() == managedType || objectToMap.GetType().IsSubclassOf(managedType))
							return scriptType;
					}
				}
			}
			return DefaultTypeMapping;
		}

		public virtual AScriptType MapValue(string valueToMap)
		{
			if (valueToMap != null)
			{
				foreach (AScriptType scriptType in Mappings)
				{
					if (scriptType == null || scriptType == DefaultValueMapping)
						continue;

					if (scriptType.IsTypeOf(valueToMap))
						return scriptType;
				}
			}
			return DefaultValueMapping;
		}
	}
}

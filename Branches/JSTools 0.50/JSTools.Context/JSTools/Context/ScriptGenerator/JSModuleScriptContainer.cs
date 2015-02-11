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
using System.Text;

using JSTools.Parser.Cruncher;

namespace JSTools.Context.ScriptGenerator
{
	internal class JSModuleScriptContainer : IScriptContainer
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private StringBuilder _renderContext = null;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Render context of the content to render.
		/// </summary>
		internal StringBuilder Script
		{
			get
			{
				if (_renderContext == null)
					_renderContext = new StringBuilder();

				return _renderContext;
			}
		}

		#region IScriptContainer Member

		DateTime IScriptContainer.LastAccess
		{
			get { return DateTime.Now; }
		}

		bool IScriptContainer.IsExpired
		{
			get { return false; }
			set { }
		}

		TimeSpan IScriptContainer.ExpirationTime
		{
			get { return TimeSpan.MaxValue; }
		}

		DateTime IScriptContainer.LastUpdate
		{
			get { return DateTime.Now; }
		}

		#endregion

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new JSModuleScriptContainer instance.
		/// </summary>
		internal JSModuleScriptContainer()
		{
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		#region IScriptContainer Member

		string IScriptContainer.GetCachedCode()
		{
			return _renderContext.ToString();
		}

		#endregion
	}
}

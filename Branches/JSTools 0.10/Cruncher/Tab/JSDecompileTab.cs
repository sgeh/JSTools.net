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

namespace JSCompiler.Tab
{
	/// <summary>
	/// Zusammenfassungsbeschreibung für JSDecompileTab.
	/// </summary>
	public class JSDecompileTab : JSBaseTab
	{
		protected override string ElementName
		{
			get { return "decompile"; }
		}

		public JSDecompileTab(JSCompiler baseClass)
		{
			_baseClass					= baseClass;
			_elementPage				= _baseClass.JSDesignDefinition.CreateStyledTabPage(JSCompiler.DECOMPILE_PAGE_NAME, TabName, true, true);
		}
	}
}

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
using System.Windows.Forms;

namespace JSCompiler.Tab
{
	public abstract class JSBaseTab
	{
		protected JSCompiler		_baseClass;
		protected TabPage			_elementPage;
		protected TabControl		_baseTabControl;



		protected abstract string ElementName
		{
			get;
		}


		protected TabControl BaseTabControl
		{
			get
			{
				if (_baseTabControl == null)
				{
					_baseTabControl = (TabControl)_baseClass.TabCollection["jsCompTab"];
				}
				return _baseTabControl;
			}
		}


		public virtual TabPage ElementPage
		{
			get { return _elementPage; }
		}


		public virtual string TabName
		{
			get { return JSConfig.Instance.GetValue("//tabNames/add[@key='" + ElementName + "']", "name"); }
		}


		protected string GetConfiguration(string nodeName, string attributeName)
		{
			return JSConfig.Instance.GetValue("//" + ElementName + "/add[@key='" + nodeName + "']", attributeName);
		}
	}
}

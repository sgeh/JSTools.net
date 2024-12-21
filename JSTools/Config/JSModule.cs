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
using System.Text;
using System.Xml;

namespace JSTools.Config
{
	/// <summary>
	/// Summary description for JSModule.
	/// </summary>
	public class JSModule : JSConfigWriter
	{
		private	string					_name			= null;
		private	JSModuleCollection		_relations		= null;
		private	JSModuleCollection		_childModules	= null;
		private	JSSourceFileCollection	_sourceFiles	= null;
		private	XmlNode					_moduleNode		= null;
		private	JSModule				_parentModule	= null;


		/// <summary>
		/// Initializes a new JavaScript configuration module.
		/// </summary>
		/// <param name="moduleNode">XmlNode which contians the module node.</param>
		public JSModule(XmlNode moduleNode, JSModule parentModule)
		{
			if (moduleNode == null)
			{
				throw new ArgumentException("The given XmlNode contains a null reference!");
			}
			_moduleNode = moduleNode;
		}


		/// <summary>
		/// Gets/Sets the parent module.
		/// </summary>
		public JSModule ParentModule
		{
			get { return _parentModule; }
			set
			{
				if (value == this)
				{
					throw new ArgumentException("Cannot assign the parent module to itself!");
				}
//				GetWriteableConfig()
			}
		}


		public string Name
		{
			get { return _name; }
			set
			{
			//	this.GetWriteableConfig
			}
		}


		public void Render(StringBuilder renderContext)
		{
		}


		public JSModuleCollection Relations
		{
			get { return _relations; }
		}


		public JSModuleCollection ChildModules
		{
			get { return _childModules; }
		}


		public JSSourceFileCollection Files
		{
			get { return _sourceFiles; }
		}
	}
}

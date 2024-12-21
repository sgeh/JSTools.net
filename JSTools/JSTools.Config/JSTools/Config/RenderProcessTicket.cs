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

/// <file>
///     <copyright see="prj:///doc/copyright.txt"/>
///     <license see="prj:///doc/license.txt"/>
///     <owner name="Silvan Gehrig" email="silvan.gehrig@mcdark.ch"/>
///     <version value="$version"/>
///     <since>JSTools.dll 0.1.0</since>
/// </file>

using System;
using System.Collections;
using System.Text;

namespace JSTools.Config
{
	/// <summary>
	/// Contains informations, which are requried by rendering the modules. This class
	/// is saved for multithreading operations.
	/// </summary>
	public class RenderProcessTicket
	{
		//------------------------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------------------------

		private	Hashtable		_renderContextSections	= new Hashtable();
		private	ArrayList		_requiredModules		= new ArrayList();
		private	ArrayList		_renderedModules		= new ArrayList();
		private	StringBuilder	_context				= new StringBuilder();


		/// <summary>
		/// Returns the rendered html context.
		/// </summary>
		public string RenderContext
		{
			get { return _context.ToString(); }
		}


		/// <summary>
		/// Returns an array, which contains the name of all required modules.
		/// </summary>
		public string[] RequiredModules
		{
			get { return (_requiredModules.ToArray(typeof(string)) as string[]); }
		}


		/// <summary>
		/// Returns an array, which contains the name of all rendered modules.
		/// </summary>
		public string[] RenderedModules
		{
			get { return (_renderedModules.ToArray(typeof(string)) as string[]); }
		}


		//------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Creates a new RenderProcessTicket instance.
		/// </summary>
		public RenderProcessTicket()
		{
		}


		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// All module, which are marked as "required", will be rendered to the client browser.
		/// So you can specify, which module should be rendered.
		/// </summary>
		/// <param name="moduleName">Name of the module, which should be added to the output queue.</param>
		/// <exception cref="ArgumentNullException">The specified module name contains a null reference.</exception>
		public void AddRequiredModule(string moduleName)
		{
			if (moduleName == null)
				throw new ArgumentNullException("moduleName", "The specified module name contains a null reference!");

			if (!_requiredModules.Contains(moduleName))
			{
				lock (_requiredModules)
				{
					_requiredModules.Add(moduleName);
				}
			}
		}


		/// <summary>
		/// Writes the specified value into the render context.
		/// </summary>
		/// <param name="valueToWrite">Value to write.</param>
		/// <exception cref="ArgumentNullException">An argument contains a null reference.</exception>
		public void Write(string valueToWrite)
		{
			if (valueToWrite == null)
				throw new ArgumentNullException("valueToWrite", "The specified value contains a null reference!");

			lock (_context)
			{
				_context.Append(valueToWrite);
			}
		}


		/// <summary>
		/// Checks for the render status of the specified module.
		/// </summary>
		/// <param name="moduleName">Module name.</param>
		/// <returns>Returns true, if the specified module is rendered.</returns>
		public bool IsModuleRendered(string moduleName)
		{
			return (_renderedModules.Contains(moduleName));
		}


		/// <summary>
		/// Sets the given module as rendred.
		/// </summary>
		/// <param name="moduleName">Name of the rendered module.</param>
		/// <exception cref="ArgumentException">The specified module was already rendered.</exception>
		public void ModuleRendered(string moduleName)
		{
			if (IsModuleRendered(moduleName))
				throw new ArgumentException("The specified module was already rendered!");

			lock (_renderedModules)
			{
				_renderedModules.Add(moduleName);
			}
		}
	}
}

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
///     <since>JSTools.dll 0.1.1</since>
/// </file>

using System;
using System.Collections;
using System.Text;
using System.Xml;

using JSTools.Config.Session;
using JSTools.Xml;

namespace JSTools.Config.ScriptFileManagement
{
	/// <summary>
	/// Represents an immutalbe script container. Operations like Add, MoveScript or Insert are provided by
	/// this class.
	/// </summary>
	public class JSScriptContainer : AJSScriptContainer
	{
		//------------------------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Gets/sets the script at the specified index.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">The index is less than zero or equal or higher than the script count.</exception>
		/// <exception cref="ArgumentNullException">The specified script contains a null reference.</exception>
		/// <exception cref="InvalidOperationException">A script file with the specified path was already registered.</exception>
		/// <exception cref="NotImportedException">The specified script file was not imported into the current owner section.</exception>
		public override AJSScript this[int index]
		{
			get { return WriteableInstance[index]; }
			set { WriteableInstance[index] = value; }
		}


		/// <summary>
		/// Gets/sets the script with the specified name. The script with the specified name will be replaced
		/// with the given value.
		/// </summary>
		/// <exception cref="ArgumentNullException">The specified script contains a null reference.</exception>
		/// <exception cref="ArgumentExcpetion">Could not find a script with the specified name.</exception>
		/// <exception cref="InvalidOperationException">A script file with the specified path was already registered.</exception>
		/// <exception cref="NotImportedException">The specified script file was not imported into the current owner section.</exception>
		public override AJSScript this[string scriptPath]
		{
			get { return WriteableInstance[scriptPath]; }
			set { WriteableInstance[scriptPath] = value; }
		}


		//------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Creates a new AJSScriptContainer instance.
		/// </summary>
		/// <param name="parentModule">Module, which contains this collection.</param>
		/// <exception cref="ArgumentNullException">The specified parent module contains a null reference.</exception>
		internal JSScriptContainer(AJSModule parentModule) : base(parentModule)
		{
		}


		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Appends the given JSScript the script collection.
		/// </summary>
		/// <param name="script">JSScript which should be added.</param>
		/// <exception cref="ArgumentNullException">The specified script contains a null reference.</exception>
		/// <exception cref="NotImportedException">The specified script was not imported into the current owner section.</exception>
		/// <exception cref="InvalidOperationException">A script file with the specified path was already registered.</exception>
		public override int Add(AJSScript script)
		{
			return WriteableInstance.Add(script);
		}


		/// <summary>
		/// Inserts a JSScript at the specified index.
		/// </summary>
		/// <param name="index">Index to insert.</param>
		/// <param name="scriptToInsert">Script to insert.</param>
		/// <exception cref="ArgumentOutOfRangeException">The index is less than zero or higher than the script count.</exception>
		/// <exception cref="ArgumentNullException">The specified script contains a null reference.</exception>
		/// <exception cref="NotImportedException">The specified script was not imported into the current owner section.</exception>
		/// <exception cref="InvalidOperationException">A script file with the specified path was already registered.</exception>
		public override void Insert(int index, AJSScript scriptToInsert)
		{
			WriteableInstance.Insert(index, scriptToInsert);
		}


		/// <summary>
		/// Inserts a JSScript before the specified reference script.
		/// </summary>
		/// <param name="script">Script to insert.</param>
		/// <param name="refScript">The script to insert should be appended before this script.</param>
		/// <exception cref="ArgumentException">Could not find the specified script in this collection.</exception>
		/// <exception cref="ArgumentNullException">A parameter contains a null reference.</exception>
		/// <exception cref="NotImportedException">The specified script was not imported into the current owner section.</exception>
		/// <exception cref="InvalidOperationException">A script file with the specified path was already registered.</exception>
		public override void InsertBefore(AJSScript script, AJSScript refScript)
		{
			WriteableInstance.InsertBefore(script, refScript);
		}


		/// <summary>
		/// Inserts a JSScript after the specified reference script.
		/// </summary>
		/// <param name="script">Script to insert.</param>
		/// <param name="refScript">The script to insert should be appended after this script.</param>
		/// <exception cref="ArgumentException">Could not find the specified script in this collection.</exception>
		/// <exception cref="ArgumentNullException">A parameter contains a null reference.</exception>
		/// <exception cref="NotImportedException">The specified script was not imported into the current owner section.</exception>
		/// <exception cref="InvalidOperationException">A script file with the specified path was already registered.</exception>
		public override void InsertAfter(AJSScript script, AJSScript refScript)
		{
			WriteableInstance.InsertAfter(script, refScript);
		}


		/// <summary>
		/// Removes a script from the script container.
		/// </summary>
		/// <param name="script">Script, which should be deleted.</param>
		/// <exception cref="ArgumentException">Could not find the specified script in this collection.</exception>
		public override void Remove(AJSScript script)
		{
			WriteableInstance.Remove(script);
		}


		/// <summary>
		/// Removes a script from the script container.
		/// </summary>
		/// <param name="position">Script, which should be deleted.</param>
		/// <exception cref="ArgumentOutOfRangeException">The index is less than zero or equal or higher than the script count.</exception>
		/// <exception cref="ArgumentException">Could not find the specified script in this collection.</exception>
		public override void RemoveAt(int position)
		{
			WriteableInstance.RemoveAt(position);
		}


		/// <summary>
		/// Creates a copy of the specified script and inserts it into the specified
		/// module. Afert inserting, the specified script will be removed from this collection.
		/// </summary>
		/// <param name="sourceScriptPath">Child script to move.</param>
		/// <param name="targetModule">Module to insert the copy.</param>
		/// <returns>Returns the copy.</returns>
		/// <exception cref="ArgumentNullException">A given value contains a null reference.</exception>
		/// <exception cref="ArgumentException">A script with the specified path is not registered.</exception>
		/// <exception cref="InvalidOperationException">Could not move the specified file to the same parent module.</exception>
		public override AJSScript MoveScript(string sourceScriptPath, AJSModule targetModule)
		{
			return WriteableInstance.MoveScript(sourceScriptPath, targetModule);
		}


		/// <summary>
		/// Creates a copy of the specified script and inserts it into the specified
		/// module. The elements that follow the insertion point move down.
		/// Afert inserting, the specified script will be removed from this collection.
		/// </summary>
		/// <param name="sourceScriptPath">Child script to move.</param>
		/// <param name="targetModule">Module to insert the copy.</param>		
		/// <param name="indexToInsert">Index to insert the script.</param>
		/// <returns>Returns the copy.</returns>
		/// <exception cref="ArgumentNullException">A given value contains a null reference.</exception>	
		/// <exception cref="ArgumentException">A script with the specified path is not registered.</exception>
		/// <exception cref="ArgumentOutOfRangeException">The index is less than zero or higher than the script count.</exception>
		/// <exception cref="InvalidOperationException">Could not move the specified file to the same parent module.</exception>
		public override AJSScript MoveScript(string sourceScriptPath, AJSModule targetModule, int indexToInsert)
		{
			return WriteableInstance.MoveScript(sourceScriptPath, targetModule, indexToInsert);
		}


		/// <summary>
		/// Removes all script, which are contained in this collection.
		/// </summary>
		public override void Clear()
		{
			WriteableInstance.Clear();
		}

		
		/// <summary>
		/// Creates a new AJSScript instance for internal use.
		/// </summary>
		/// <param name="scriptNode">XmlNode which contians the values of the AJSScript.</param>
		/// <param name="path">Path to the script.</param>
		/// <returns>Returns the created AJSScript.</returns>
		protected override AJSScript CreateInnerScript(string path, XmlNode scriptNode)
		{
			return ((AJSScriptFileHandler)OwnerSection).CreateInnerScriptFile(path, scriptNode);
		}
	}
}

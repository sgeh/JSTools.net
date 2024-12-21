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
	/// Represents a script container. Operations like Add, MoveScript or Insert are provided by
	/// this class.
	/// </summary>
	public abstract class AJSScriptContainer : AJSToolsFileManagementContainer, IList
	{
		//------------------------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------------------------

		protected			ArrayList					_childScripts		= new ArrayList();

		private	const		bool						IS_SYNCHRONIZED		= false;
		private	const		bool						IS_READ_ONLY		= false;
		private	const		bool						IS_FIXED_SIZE		= false;

		private	const		string						FOLDER_NAME_ATTRIB	= "name";
		private	const		char						FOLDER_SEPARATOR	= '/';
		private	const		string						FOLDER_NODE_NAME	= "folder";


		/// <summary>
		/// Gets/sets the script at the specified index.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">The index is less than zero or equal or higher than the script count.</exception>
		/// <exception cref="ArgumentNullException">The specified script contains a null reference.</exception>
		/// <exception cref="InvalidOperationException">A script file with the specified path was already registered.</exception>
		/// <exception cref="ArgumentException">The specified scripts is not a valid AJSScript instance.</exception>
		/// <exception cref="NotImportedException">The specified script file was not imported into the current owner section.</exception>
		object IList.this[int index]
		{
			get { return this[index]; }
			set
			{
				if (value as AJSScript == null)
					throw new ArgumentException("The specified scripts is not a valid AJSScript instance!");

				this[index] = (value as AJSScript);
			}
		}


		/// <summary>
		/// Gets/sets the script at the specified index.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">The index is less than zero or equal or higher than the script count.</exception>
		/// <exception cref="ArgumentNullException">The specified script contains a null reference.</exception>
		/// <exception cref="InvalidOperationException">A script file with the specified path was already registered.</exception>
		/// <exception cref="NotImportedException">The specified script file was not imported into the current owner section.</exception>
		public abstract AJSScript this[int index]
		{
			get;
			set;
		}


		/// <summary>
		/// Gets/sets the script with the specified name. The script with the specified name will be replaced
		/// with the given value.
		/// </summary>
		/// <exception cref="ArgumentNullException">The specified script contains a null reference.</exception>
		/// <exception cref="ArgumentExcpetion">Could not find a script with the specified name.</exception>
		/// <exception cref="InvalidOperationException">A script file with the specified path was already registered.</exception>
		/// <exception cref="NotImportedException">The specified script file was not imported into the current owner section.</exception>
		public abstract AJSScript this[string scriptPath]
		{
			get;
			set;
		}


		/// <summary>
		/// Returns the count of scripts.
		/// </summary>
		public int Count
		{
			get { return _childScripts.Count; }
		}


		/// <summary>
		/// Returns true, if all operations are thread save.
		/// </summary>
		public bool IsSynchronized
		{
			get { return IS_SYNCHRONIZED; }
		}


		/// <summary>
		/// Returns true, it this container has a fixed size, otherwise false.
		/// </summary>
		public bool IsFixedSize
		{
			get { return IS_FIXED_SIZE; }
		}


		/// <summary>
		/// Returns true, if this container is read only.
		/// </summary>
		public bool IsReadOnly
		{
			get { return IS_READ_ONLY; }
		}


		/// <summary>
		/// Returns true, if the operations a thread save.
		/// </summary>
		public object SyncRoot
		{
			get { return this; }
		}


		/// <summary>
		/// Returns the parent module.
		/// </summary>
		public AJSModule ParentModule
		{
			get { return (ParentSection as AJSModule); }
		}


		/// <summary>
		/// Returns the writeable instance of the current instance.
		/// </summary>
		protected AJSScriptContainer WriteableInstance
		{
			get { return ParentModule.WriteableInstance.ScriptFiles; }
		}


		//------------------------------------------------------------------------------------------
		// Constructors / Destructor
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Creates a new AJSScriptContainer instance.
		/// </summary>
		/// <param name="parentModule">Module, which contains this collection.</param>
		/// <exception cref="ArgumentNullException">The specified parent module contains a null reference.</exception>
		internal AJSScriptContainer(AJSModule parentModule) : base(parentModule)
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
		int IList.Add(object script)
		{
			if (script as AJSScript == null)
				throw new ArgumentException("The specified script is not a valid AJSScript instance!", "script");

			return Add(script as AJSScript);
		}


		/// <summary>
		/// Inserts a JSScript at the specified index.
		/// </summary>
		/// <param name="index">Index to insert.</param>
		/// <param name="scriptToInsert">Script to insert.</param>
		/// <exception cref="ArgumentOutOfRangeException">The index is less than zero or higher than the script count.</exception>
		/// <exception cref="NullReferenceException">The specified script contains a null reference.</exception>
		/// <exception cref="InvalidOperationException">A script with the specified path was already registered.</exception>
		/// <exception cref="NotImportedException">The specified script was not imported into the current owner section.</exception>
		/// <exception cref="ArgumentException">The specified script is not a valid AJSScript instance.</exception>
		void IList.Insert(int index, object scriptToInsert)
		{
			if (scriptToInsert as AJSScript == null)
				throw new ArgumentException("The specified script is not a valid AJSScript instance!", "scriptToInsert");

			Insert(index, (scriptToInsert as AJSScript));
		}


		/// <summary>
		/// Checks if the specified script is contained in this collection.
		/// </summary>
		/// <param name="script">JSScript to check.</param>
		/// <returns>Returns true, if the specified JSScript instance is contained in this collection.</returns>
		/// <exception cref="ArgumentException">The specified script is not a valid AJSScript instance.</exception>
		bool IList.Contains(object script)
		{
			if (script as AJSScript == null)
				throw new ArgumentException("The specified script is not a valid AJSScript instance!", "script");

			return Contains(script as AJSScript);
		}

		
		/// <summary>
		/// Determines the index of the specified item in the container. Returns -1 if the script was not found
		/// in this container.
		/// </summary>
		/// <param name="script">JSScript to get the index.</param>
		/// <returns>Returns the index of the requested script.</returns>
		/// <exception cref="ArgumentException">The specified script is not a valid AJSScript instance.</exception>
		int IList.IndexOf(object script)
		{
			if (script as AJSScript == null)
				throw new ArgumentException("The specified script contains a null reference!", "script");

			return IndexOf(script as AJSScript);
		}


		/// <summary>
		/// Copies the elements of this collection into the specified destination collection.
		/// </summary>
		/// <param name="array">The one-dimensional Array that is the destination of the elements.</param>
		/// <param name="index">The zero-based index in array at which copying begins.</param>
		/// <exception cref="ArgumentOutOfRangeException">The index is less than zero or equal or higher than the script count.</exception>
		void ICollection.CopyTo(Array array, int index)
		{
			CopyTo(array, index);
		}


		/// <summary>
		/// Removes a script from the script container.
		/// </summary>
		/// <param name="script">Script, which should be deleted.</param>
		/// <exception cref="ArgumentException">The specified script is not a valid AJSScript instance.</exception>
		void IList.Remove(object script)
		{
			if (script as AJSScript == null)
				throw new ArgumentException("The specified script contains a null reference!", "script");

			Remove(script as AJSScript);
		}


		/// <summary>
		/// Removes a script from the script container.
		/// </summary>
		/// <param name="position">Script, which should be deleted.</param>
		/// <exception cref="ArgumentOutOfRangeException">The index is less than zero or equal or higher than the script count.</exception>
		void IList.RemoveAt(int position)
		{
			RemoveAt(position);
		}


		/// <summary>
		/// Gets an enumerator for this script container.
		/// </summary>
		/// <returns>Returns a new enumerator for navigating through the container.</returns>
		public override IEnumerator GetEnumerator()
		{
			return new JSFileSectionEnumerator(this);
		}


		/// <summary>
		/// Appends the given JSScript the script collection.
		/// </summary>
		/// <param name="script">JSScript which should be added.</param>
		/// <exception cref="ArgumentNullException">The specified script contains a null reference.</exception>
		/// <exception cref="NotImportedException">The specified script was not imported into the current owner section.</exception>
		/// <exception cref="InvalidOperationException">A script file with the specified path was already registered.</exception>
		public abstract int Add(AJSScript script);


		/// <summary>
		/// Inserts a JSScript at the specified index.
		/// </summary>
		/// <param name="index">Index to insert.</param>
		/// <param name="scriptToInsert">Script to insert.</param>
		/// <exception cref="ArgumentOutOfRangeException">The index is less than zero or higher than the script count.</exception>
		/// <exception cref="ArgumentNullException">The specified script contains a null reference.</exception>
		/// <exception cref="NotImportedException">The specified script was not imported into the current owner section.</exception>
		/// <exception cref="InvalidOperationException">A script file with the specified path was already registered.</exception>
		public abstract void Insert(int index, AJSScript scriptToInsert);


		/// <summary>
		/// Inserts a JSScript before the specified reference script.
		/// </summary>
		/// <param name="script">Script to insert.</param>
		/// <param name="refScript">The script to insert should be appended before this script.</param>
		/// <exception cref="ArgumentException">Could not find the specified script in this collection.</exception>
		/// <exception cref="ArgumentNullException">A parameter contains a null reference.</exception>
		/// <exception cref="NotImportedException">The specified script was not imported into the current owner section.</exception>
		/// <exception cref="InvalidOperationException">A script file with the specified path was already registered.</exception>
		public abstract void InsertBefore(AJSScript script, AJSScript refScript);


		/// <summary>
		/// Inserts a JSScript after the specified reference script.
		/// </summary>
		/// <param name="script">Script to insert.</param>
		/// <param name="refScript">The script to insert should be appended after this script.</param>
		/// <exception cref="ArgumentException">Could not find the specified script in this collection.</exception>
		/// <exception cref="ArgumentNullException">A parameter contains a null reference.</exception>
		/// <exception cref="NotImportedException">The specified script was not imported into the current owner section.</exception>
		/// <exception cref="InvalidOperationException">A script file with the specified path was already registered.</exception>
		public abstract void InsertAfter(AJSScript script, AJSScript refScript);


		/// <summary>
		/// Removes a script from the script container.
		/// </summary>
		/// <param name="script">Script, which should be deleted.</param>
		/// <exception cref="ArgumentException">Could not find the specified script in this collection.</exception>
		public abstract void Remove(AJSScript script);


		/// <summary>
		/// Removes a script from the script container.
		/// </summary>
		/// <param name="position">Script, which should be deleted.</param>
		/// <exception cref="ArgumentOutOfRangeException">The index is less than zero or equal or higher than the script count.</exception>
		/// <exception cref="ArgumentException">Could not find the specified script in this collection.</exception>
		public abstract void RemoveAt(int position);


		/// <summary>
		/// Copies the elements of this collection into the specified destination collection.
		/// </summary>
		/// <param name="destination">The one-dimensional Array that is the destination of the elements.</param>
		/// <param name="index">The zero-based index in array at which copying begins.</param>
		/// <exception cref="ArgumentNullException">Destination is a null reference.</exception>
		/// <exception cref="ArgumentOutOfRangeException">The index is less than zero or equal or higher than the script count.</exception>
		/// <exception cref="ArgumentException">The specified destination list is null or read only.</exception>
		public void CopyTo(Array destination, int index)
		{
			if (destination == null)
				throw new ArgumentNullException("destination", "Destination is a null reference!");

			if (destination.IsReadOnly)
				throw new ArgumentException("The specified destination list is null or read only!", "destination");

			if (!IsValidIndex(index))
				throw new ArgumentOutOfRangeException("index", "The index is less than zero or equal or higher than the script count!");

			_childScripts.CopyTo(destination, index);
		}


		/// <summary>
		/// Copies the specified script to the destination container.
		/// </summary>
		/// <param name="destination">The one-dimensional Array that is the destination of the elements.</param>
		/// <param name="scriptToCopy">The zero-based index in array at which copying begins.</param>
		/// <exception cref="ArgumentNullException">Destination is a null reference.</exception>
		/// <exception cref="ArgumentException">Could not find the specified script.</exception>
		public void CopyTo(AJSScriptContainer destination, string scriptToCopy)
		{
			if (!Contains(scriptToCopy))
				throw new ArgumentException("Could not find the specified script!", scriptToCopy);

			if (destination == null)
				throw new ArgumentNullException("destination", "Destination is a null reference!");

			CopyTo(destination, scriptToCopy, destination.Count);
		}


		/// <summary>
		/// Copies the specified script to the destination container.
		/// </summary>
		/// <param name="destination">The one-dimensional Array that is the destination of the elements.</param>
		/// <param name="scriptToCopy">The zero-based index in array at which copying begins.</param>
		/// <param name="index">Index to insert the copied script.</param>
		/// <exception cref="ArgumentNullException">Destination is a null reference.</exception>
		/// <exception cref="ArgumentException">Could not find the specified script.</exception>
		/// <exception cref="InvalidOperationException">Could not copy a script to the same owner section instance. Use the MoveScript procedure instead.</exception>
		public void CopyTo(AJSScriptContainer destination, string scriptToCopy, int index)
		{
			if (!Contains(scriptToCopy))
				throw new ArgumentException("Could not find the specified script!", scriptToCopy);

			if (destination == null)
				throw new ArgumentNullException("destination", "Destination is a null reference!");

			if (destination.OwnerSection == OwnerSection)
				throw new InvalidOperationException("Could not copy a script to the same owner section instance! Use the MoveScript procedure instead.");

			destination[index] = this[scriptToCopy].Clone((AJSScriptFileHandler)destination.OwnerSection);
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
		public abstract AJSScript MoveScript(string sourceScriptPath, AJSModule targetModule);


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
		public abstract AJSScript MoveScript(string sourceScriptPath, AJSModule targetModule, int indexToInsert);


		/// <summary>
		/// Removes all script, which are contained in this collection.
		/// </summary>
		public abstract void Clear();


		/// <summary>
		/// Checks if the specified script is contained in this collection.
		/// </summary>
		/// <param name="script">Script to check.</param>
		/// <returns>Returns true, if the specified JSScript instance is contained in this collection.</returns>
		/// <exception cref="ArgumentNullException">The specified script contains a null reference.</exception>
		public bool Contains(AJSScript script)
		{
			if (script == null)
				throw new ArgumentNullException("The specified script contains a null reference!");

			return _childScripts.Contains(script);
		}


		/// <summary>
		/// Checks if the specified script is contained in this collection.
		/// </summary>
		/// <param name="scriptPath">JSScript to check.</param>
		/// <returns>Returns true, if the specified JSScript instance is contained in this collection.</returns>
		/// <exception cref="ArgumentNullException">The specified script name contains a null reference.</exception>
		public bool Contains(string scriptPath)
		{
			if (scriptPath == null)
				throw new ArgumentNullException("The specified script name contains a null reference!");

			return (IndexOf(scriptPath) != -1);
		}


		/// <summary>
		/// Determines the index of a specific item in this collection. Returns -1 if nothing was found.
		/// </summary>
		/// <param name="script">Script to search.</param>
		/// <returns>Returns -1 if nothing was found. Otherwise the expected index.</returns>
		/// <exception cref="ArgumentNullException">The specified script contains a null reference.</exception>
		public int IndexOf(AJSScript script)
		{
			if (script == null)
				throw new ArgumentNullException("The specified script contains a null reference!");

			return _childScripts.IndexOf(script);
		}


		/// <summary>
		/// Determines the index of a specific item in this collection.
		/// </summary>
		/// <param name="scriptFileName">Script to search.</param>
		/// <returns>Returns -1 if nothing was found. Otherwise the expected index.</returns>
		/// <exception cref="ArgumentNullException">The specified script name contains a null reference.</exception>
		public int IndexOf(string scriptFileName)
		{
			if (scriptFileName == null)
				throw new ArgumentNullException("The specified script name contains a null reference!");

			int index = _childScripts.Count - 1;

			for ( ; index > -1 && this[index].FileName != scriptFileName; --index)
			{
			}
			return index;
		}


		/// <summary>
		/// Appends a new JSScript to the container, without creating a writeable instance.
		/// </summary>
		/// <param name="path">Path to the script.</param>
		/// <param name="scriptNode">XmlNode which contians the values of the AJSScript.</param>
		/// <exception cref="InvalidOperationException">A script with the specified path was already registered.</exception>
		/// <exception cref="ArgumentNullException">The specified script node contains a null reference.</exception>
		internal void AppendInnerScript(string path, XmlNode scriptNode)
		{
			if (scriptNode == null)
				throw new ArgumentNullException("scriptNode", "The specified script node contains a null reference!");

			AppendScript(CreateInnerScript(path, scriptNode));
		}


		/// <summary>
		/// Creates a new AJSScript instance for internal use.
		/// </summary>
		/// <param name="scriptNode">XmlNode which contians the values of the AJSScript.</param>
		/// <param name="path">Path to the script.</param>
		/// <returns>Returns the created AJSScript.</returns>
		protected abstract AJSScript CreateInnerScript(string path, XmlNode scriptNode);


		/// <summary>
		/// Appends the specified script to this collection.
		/// </summary>
		/// <param name="toAppend">Script to append.</param>
		/// <returns>The position into which the new element was inserted.</returns>
		/// <exception cref="InvalidOperationException">A script with the specified path was already registered.</exception>
		/// <exception cref="NotImportedException">The specified script was not imported into the current owner section.</exception>
		protected int AppendScript(AJSScript toAppend)
		{
			if (toAppend.OwnerSection != OwnerSection)
				throw new NotImportedException("The specified script was not imported into the current owner section!");

			if (Contains(toAppend))
				throw new InvalidOperationException("A script with the specified path was already registered!");

			toAppend.SetParent(ParentSection);
			return _childScripts.Add(toAppend);
		}


		/// <summary>
		/// Checks the given index for validity.
		/// </summary>
		/// <returns>Returns true, if the specified index is valid.</returns>
		/// <param name="index">Index to check.</param>
		protected bool IsValidIndex(int index)
		{
			return (index > -1 && index < Count);
		}
	}
}

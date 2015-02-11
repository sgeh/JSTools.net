/*
 * JSTools.Parser.DocGenerator.dll / JSTools.net - A framework for JavaScript/ASP.NET applications.
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
using System.IO;

using JSTools.Parser;
using JSTools.Parser.DocGenerator.CommentItems;

namespace JSTools.Parser.DocGenerator
{
	/// <summary>
	/// Contains the comment items and manages the events.
	/// </summary>
	public class CommentItemContext
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const string LIST_PARAM_NAME = "param";

		/// <summary>
		/// Gets the name of the default global type.
		/// </summary>
		public readonly string DefaultType;

		/// <summary>
		/// Gets the name of the default global namespace.
		/// </summary>
		public readonly string DefaultNameSpace;

		private readonly string DEFAULT_LIST_TYPE;
		private readonly string DEFAULT_BASE_TYPE;

		private string _name = string.Empty;
		private Hashtable _commentItems = new Hashtable();
		private IncludeManager _includeManager = null;
		private TypeContext _typeContext = null;
		private CommentItemSerializer _serializer = null;

		private bool _includesLoaded = false;
		private bool _typesCreated = false;
		private bool _typesInitialized = false;
		private bool _referencesResolved = false;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the name of the current context.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// Gets the comment item instance with the specified name.
		/// </summary>
		public ACommentItem this[string fullItemName]
		{
			get { return (_commentItems[ResolveItemName(fullItemName)] as ACommentItem); }
			set
			{
				fullItemName = ResolveItemName(fullItemName);

				if (_commentItems[fullItemName] != null)
					Remove(fullItemName);

				if (value != null)
				{
					_commentItems[fullItemName] = value;

					if (_includesLoaded) value.LoadIncludes();
					if (_typesCreated) value.CreateType();
					if (_typesInitialized) value.InitType();
					if (_referencesResolved) value.ResolveReferences();
				}
			}
		}

		/// <summary>
		/// Gets the type associated to the given class comment item.
		/// </summary>
		public ACommentItem this[string fullTypeName, bool returnDefaultType]
		{
			get
			{
				ClassItem foundItem = (this[fullTypeName] as ClassItem);

				if (foundItem == null && returnDefaultType)
					foundItem = (_commentItems[ResolveItemName(DEFAULT_BASE_TYPE)] as ClassItem);

				return foundItem;
			}
		}

		/// <summary>
		/// Gets a collection which contains the stored item keys.
		/// </summary>
		public ICollection ItemKeys
		{
			get { return _commentItems.Keys; }
		}

		/// <summary>
		/// Gets the comment item serializer/deserializer.
		/// </summary>
		public CommentItemSerializer Serializer
		{
			get 
			{
				if (_serializer == null)
					_serializer = new CommentItemSerializer();

				return _serializer;
			}
		}

		/// <summary>
		/// Gets the include manager which contains the loaded includes.
		/// </summary>
		internal IncludeManager IncludeManager
		{
			get { return _includeManager; }
		}

		/// <summary>
		/// Gets the type creater/container.
		/// </summary>
		internal TypeContext TypeContext
		{
			get { return _typeContext; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new CommentItemContext instance.
		/// </summary>
		/// <param name="name">Name of the comment/library file.</param>
		/// <param name="commentFilePath">Path which contains the comment include files.</param>
		/// <param name="defaultType">Name of the default (global) type.</param>
		/// <param name="defaultBaseType">Name of the default base type.</param>
		/// <param name="defaultNameSpace">Name of the default (global) namespace.</param>
		/// <param name="defaultListType">Name of the default list (array) type.</param>
		/// <exception cref="ArgumentException">An argument contains null or is empty.</exception>
		internal protected CommentItemContext(
			string name,
			string commentFilePath,
			string defaultType,
			string defaultNameSpace,
			string defaultListType,
			string defaultBaseType )
		{
			if (name == null || name.Length == 0)
				throw new ArgumentException("name");

			if (commentFilePath == null || commentFilePath.Length == 0)
				throw new ArgumentException("commentFilePath");

			if (defaultType == null || defaultType.Length == 0)
				throw new ArgumentException("defaultType");

			if (defaultNameSpace == null || defaultNameSpace.Length == 0)
				throw new ArgumentException("defaultNameSpace");

			if (defaultListType == null || defaultListType.Length == 0)
				throw new ArgumentException("defaultListType");

			DefaultType = defaultType;
			DefaultNameSpace = defaultNameSpace;
			DEFAULT_LIST_TYPE = ResolveItemName(defaultListType);
			DEFAULT_BASE_TYPE = ResolveItemName(defaultBaseType);

			_includeManager = new IncludeManager(commentFilePath);
			_typeContext = new TypeContext(name);
			_name = name;
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Resolves the given string as argument type. This means that if
		/// the given string contains "param" the default list type is returned.
		/// </summary>
		/// <param name="fullItemName">Full argument type name.</param>
		/// <returns>Returns the resolved name.</returns>
		public string ResolveArgumentName(string fullItemName)
		{
			string resolvedType = null;

			if (fullItemName == LIST_PARAM_NAME)
				resolvedType = LIST_PARAM_NAME;

			return ResolveItemName(resolvedType);
		}

		/// <summary>
		/// Resolves the given item name and appends the default namespace
		/// if the given item has no namespace definition. If no item
		/// name is specified, the default type name is appended.
		/// </summary>
		/// <param name="fullItemName">Full name of the item to resolve.</param>
		/// <returns>Returns the resolved name.</returns>
		public string ResolveItemName(string fullItemName)
		{
			if (fullItemName == null || fullItemName.Length < 1)
				fullItemName = DefaultType;

			if (!Expression.HasParentElement(fullItemName))
				fullItemName = Expression.Combine(DefaultNameSpace, fullItemName);

			return fullItemName;
		}


		/// <summary>
		/// Parses the specified string and adds the parsed comments to the
		/// context.
		/// </summary>
		/// <param name="name">Name of the string to parse.</param>
		/// <param name="toParse">String which should be parsed.</param>
		/// <param name="throwError">True to throw errors.</param>
		/// <returns>Returns true if the string was successfully parsed.</returns>
		public virtual bool ParseString(string name, string toParse, bool throwError)
		{
			CommentItemParser parser = new CommentItemParser(this, name, toParse);
			
			if (parser.HasError && throwError)
			{
				throw new DocumentationException(
					string.Format("Error while parsing the given string '{0}'.", name),
					name,
					parser.Errors );
			}
			return parser.HasError;
		}

		/// <summary>
		/// Saves the generated files on the harddisk.
		/// </summary>
		/// <param name="outputPath">Output path to generate the dll and xml files.</param>
		public void Save(string outputPath)
		{
			_typeContext.Serialize(outputPath);
		}

		/// <summary>
		/// Loads the includes of the parsed nodes.
		/// </summary>
		public void LoadIncludes()
		{
			foreach (DictionaryEntry commentEntry in _commentItems)
			{
				((ACommentItem)commentEntry.Value).LoadIncludes();
			}
			_includesLoaded = true;
		}

		/// <summary>
		/// Creates the managed types.
		/// </summary>
		public void CreateType()
		{
			foreach (DictionaryEntry commentEntry in _commentItems)
			{
				((ACommentItem)commentEntry.Value).CreateType();
			}
			_typesCreated = true;
		}

		/// <summary>
		/// Returns the type item with the specfied name. If it cannot be found,
		/// the default item is returned or a new item with the specified parent
		/// class is created.
		/// </summary>
		public ACommentItem CreateType(string fullName, bool hasParentClass, bool returnDefault, bool createNew)
		{
			ClassItem currentItem = (this[fullName, returnDefault] as ClassItem);

			if (currentItem == null && createNew)
			{
				currentItem = new ClassItem(this, fullName, hasParentClass);
				this[currentItem.ItemName.FullName] = currentItem;
			}
		
			if (currentItem != null)
				currentItem.CreateType();

			return currentItem;
		}

		/// <summary>
		/// Initializes the created managed types.
		/// </summary>
		public void InitType()
		{
			foreach (DictionaryEntry commentEntry in _commentItems)
			{
				((ACommentItem)commentEntry.Value).InitType();
			}
			_typesInitialized = true;
		}

		/// <summary>
		/// Resolves the comment references.
		/// </summary>
		public void ResolveReferences()
		{
			foreach (DictionaryEntry commentEntry in _commentItems)
			{
				((ACommentItem)commentEntry.Value).ResolveReferences();
			}
			_referencesResolved = true;
		}

		private void Remove(string parentName)
		{
			ICollection keys = _commentItems.Keys;

			for (int i = _commentItems.Count - 1; i > -1; --i)
			{
				string commentItem = (string)_commentItems[i];

				if (Expression.IsParent(commentItem, parentName) || commentItem == parentName)
					_commentItems.Remove(commentItem);
			}
		}
	}
}

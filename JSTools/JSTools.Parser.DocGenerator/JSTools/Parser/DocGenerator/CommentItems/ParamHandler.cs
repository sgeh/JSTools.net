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
using System.Text;
using System.Xml;

using JSTools.Parser;

namespace JSTools.Parser.DocGenerator.CommentItems
{
	/// <summary>
	/// Evaluates the number of overloadings of the given param list.
	/// </summary>
	internal class ParamHandler
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const char ARG_LIST_BEGIN = '(';
		private const char ARG_LIST_END = ')';
		private const char ARG_LIST_SEPARATOR = ',';

		private const string NAME_ATTRIB = "name";
		private const string PARAM_NODES = "/param[@" + NAME_ATTRIB + "]";
		private const string TYPE_ATTRIB = "type";

		private Hashtable _paramTable = new Hashtable();
		private int _overloadingCount = -1;
		private Param[][] _overloadings = null;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets all overloadings which are specified in this param list.
		/// </summary>
		public Param[][] Overloadings
		{
			get
			{
				if (_overloadings == null)
				{
					// init overloadings
					_overloadings = new Param[OverloadingCount][];
					int paramIndex = 0;

					// init all possible method combinations
					for (int i = 0; i < OverloadingCount; ++i)
					{
						paramIndex = 0;
						_overloadings[i] = new Param[_paramTable.Count];

						foreach (object key in _paramTable.Keys)
						{
							ParamGroupContainer paramGroup = (ParamGroupContainer)_paramTable[key];
							int paramGroupIndex = (i / paramGroup.Quantifier) % paramGroup.Count;

							_overloadings[i][paramIndex++] = paramGroup[paramGroupIndex];
						}
					}
				}
				return _overloadings;
			}
		}

		/// <summary>
		/// Gets the number of overloadings.
		/// </summary>
		public int OverloadingCount
		{
			get { return _overloadingCount; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new ParamHandler instance.
		/// </summary>
		/// <param name="commentDoc">Specifies the comment document which contains the param tags.</param>
		/// <param name="context">Context which contans the type definitions.</param>
		internal ParamHandler(XmlDocument commentDoc, CommentItemContext context)
		{
			// group params
			foreach (XmlNode paramNode in commentDoc.SelectNodes(PARAM_NODES))
			{
				string paramName = paramNode.Attributes[NAME_ATTRIB].Value;
				Type paramType = GetType(paramNode, context);

				if (paramType == null)
					continue;

				if (!_paramTable.ContainsKey(paramName))
					_paramTable[paramName] = new ParamGroupContainer();

				((ParamGroupContainer)_paramTable[paramName])[paramType] = new Param(paramName, paramType, paramNode);
			}
	
			// get number of overloadings and init group scoring
			_overloadingCount = 1;

			foreach (DictionaryEntry entry in _paramTable)
			{
				ParamGroupContainer groupContainer = (ParamGroupContainer)entry.Value;
				groupContainer.Quantifier = _overloadingCount;
				_overloadingCount *= groupContainer.Count;
			}
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates an array which contains the method headers of all
		/// overloadings.
		/// </summary>
		/// <param name="methodDocName">Name of the method.</param>
		/// <returns>Returns the created array.</returns>
		public string[] GetMethodHeaders(string methodDocName)
		{
			return GetMethodHeaders(methodDocName, OverloadingCount);
		}

		/// <summary>
		/// Creates an array which contains the method headers of all
		/// overloadings.
		/// </summary>
		/// <param name="methodDocName">Name of the method.</param>
		/// <returns>Returns the created array.</returns>
		public string GetMethodHeader(string methodDocName)
		{
			string[] argList = GetMethodHeaders(methodDocName, 1);

			if (argList.Length == 0)
				return string.Empty;
			else
				return argList[0];
		}

		private string[] GetMethodHeaders(string methodDocName, int count)
		{
			if (methodDocName == null)
				return new string[0];

			string[] argList = new string[OverloadingCount];

			for (int i = 0; i < count; ++i)
			{
				StringBuilder builder = new StringBuilder(methodDocName);
				builder.Append(ARG_LIST_BEGIN);

				for (int j = 0; j < Overloadings[i].Length; ++j)
				{
					builder.Append(Overloadings[i][j].Type.FullName);

					if (j + 1 != Overloadings[i].Length)
						builder.Append(ARG_LIST_SEPARATOR);
				}

				builder.Append(ARG_LIST_END);
				argList[i] = builder.ToString();
			}
			return argList;
		}

		private Type GetType(XmlNode paramNode, CommentItemContext context)
		{
			string resolvedName = null;

			if (paramNode.Attributes[TYPE_ATTRIB] != null)
				resolvedName = context.ResolveArgumentName(paramNode.Attributes[TYPE_ATTRIB].Value);

			ClassItem retrievedType = (context[resolvedName, true] as ClassItem);
			return (retrievedType != null) ? retrievedType.CreatedType : null;
		}

		//--------------------------------------------------------------------
		// Nested Classes
		//--------------------------------------------------------------------

		/// <summary>
		/// Represents an argument which contains the param name and its type.
		/// </summary>
		internal class Param
		{
			//----------------------------------------------------------------
			// Declarations
			//----------------------------------------------------------------

			private XmlNode _comment = null;
			private Type _type = null;
			private string _name = string.Empty;

			//----------------------------------------------------------------
			// Properties
			//----------------------------------------------------------------

			/// <summary>
			/// Gets the argument type.
			/// </summary>
			public Type Type
			{
				get { return _type; }
			}

			/// <summary>
			/// Gets the argument name.
			/// </summary>
			public string Name
			{
				get { return _name; }
			}

			/// <summary>
			/// Gets the comment xml node.
			/// </summary>
			public XmlNode Comment
			{
				get { return _comment; }
			}

			//----------------------------------------------------------------
			// Constructors / Destructor
			//----------------------------------------------------------------

			/// <summary>
			/// Creates a new param instance.
			/// </summary>
			/// <param name="name">Name of the param.</param>
			/// <param name="type">Type of the param.</param>
			/// <param name="comment">Comment string.</param>
			internal Param(string name, Type type, XmlNode comment)
			{
				_name = name;
				_comment = comment;

				if (type != null)
					_type = type;
			}

			//----------------------------------------------------------------
			// Events
			//----------------------------------------------------------------

			//----------------------------------------------------------------
			// Methods
			//----------------------------------------------------------------
		}

		/// <summary>
		/// Contains the param lists.
		/// </summary>
		private class ParamGroupContainer
		{
			//----------------------------------------------------------------
			// Declarations
			//----------------------------------------------------------------

			private ArrayList _entries = new ArrayList();
			private Hashtable _paramList = new Hashtable();
			private int _quantifier = 1;

			//----------------------------------------------------------------
			// Properties
			//----------------------------------------------------------------

			/// <summary>
			/// Gets/sets the quantifier of the current group which is used
			/// to generate the method overloadings.
			/// </summary>
			internal int Quantifier
			{
				get { return _quantifier; }
				set
				{
					if (value > 0)
						_quantifier = value;
				}
			}

			/// <summary>
			/// Gets the a param instance by its index.
			/// </summary>
			internal Param this[int index]
			{
				get 
				{
					if (index > -1 && index < _entries.Count)
						return (Param)_paramList[_entries[index]];
					else
						return null;
				}
			}

			/// <summary>
			/// Gets a param instance by its key.
			/// </summary>
			internal Param this[Type key]
			{
				get { return (Param)_paramList[key]; }
				set 
				{
					if (!_paramList.ContainsKey(key))
						_entries.Add(key);

					_paramList[key] = value;
				}
			}

			/// <summary>
			/// Gets the index count.
			/// </summary>
			internal int Count
			{
				get { return _entries.Count; }
			}

			//----------------------------------------------------------------
			// Constructors / Destructor
			//----------------------------------------------------------------

			/// <summary>
			/// Creates a new ParamGroupContainer instance.
			/// </summary>
			internal ParamGroupContainer()
			{
			}

			//----------------------------------------------------------------
			// Events
			//----------------------------------------------------------------

			//----------------------------------------------------------------
			// Methods
			//----------------------------------------------------------------
		}

	}
}

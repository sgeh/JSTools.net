/*
 * JSTools.ScriptTypes.dll / JSTools.net - A framework for JavaScript/ASP.NET applications.
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
using System.Reflection;
using System.Text;

using JSTools.ScriptTypes;
using JSTools.Parser.Cruncher;
using JSTools.Parser.Cruncher.Nodes;

namespace JSTools.Util.Serialization
{
	/// <summary>
	/// Represents the script string deserializer.
	/// </summary>
	internal class Deserializer
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const string DESERIALIZATION_PREFIX = "var USE_TEMP = {0};";
		private const ScriptVersion DESERIALIZATION_VERSION = ScriptVersion.Version_1_3;

		private const string ENTRY_POINT_NODE = "Var/Name/Comma";
		private const string OBJECT_TYPE_NODE = "NewTemp/New/Name";
		private const string OBJECT_ARG_NODE = "NewTemp/New/Number"; 

		private const string OBJECT_TYPE_ID = "object";
		private const string ARRAY_TYPE_ID = "array";

		private const char GET_PROP_CHAR = '.';
		private const char ADD_CHAR = '+';
		private const char SUB_CHAR = '-';

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new Deserializer instance.
		/// </summary>
		internal Deserializer()
		{
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		internal object Deserialize(string toDeserialze, bool decodeValues)
		{
			JSObjDeserializer deserializer = new JSObjDeserializer(decodeValues);
			DeserializeScriptString(toDeserialze, deserializer);
			return deserializer.DeserializedObject;
		}

		internal object Deserialize(string toDeserialze, object toFill, bool decodeValues)
		{
			CustomObjDeserializer deserializer = new CustomObjDeserializer(toFill, decodeValues);
			DeserializeScriptString(toDeserialze, deserializer);
			return deserializer.DeserializedObject;
		}

		private void DeserializeScriptString(string toDeserialze, IScriptDeserializer deserializer)
		{
			try
			{
				Node entryPointNode = GetEntryPointNode(toDeserialze);

				if (entryPointNode == null)
					throw new DeserializationException("The specified script does not contain an object definition.");

				DeserializeObject(entryPointNode, null, deserializer);
			}
			catch (SyntaxException se)
			{
				throw new DeserializationException("Error while parsing the specified script string.", se);
			}
			catch (DeserializationException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new DeserializationException(string.Format("Could not deserialize the given script string. {0}", ex.Message));
			}
		}

		private void DeserializeObject(Node objectNode, string propName, IScriptDeserializer deserializer)
		{
			bool isArray = (GetObjectType(objectNode) == ARRAY_TYPE_ID);
			TokenType propNameNodeType = (!isArray ? TokenType.String : TokenType.Number);

			deserializer.MoveIntoObject(propName, isArray);

			// get property nodes
			foreach (Node propNode in objectNode.GetChildren(TokenType.SetElem))
			{
				// get property name node
				Node propNameNode = propNode.GetChild(propNameNodeType);

				if (propNameNode == null || propNameNode.Next == null)
					throw new DeserializationException("The given object contains invalid property assignments.");

				// propNameNode.Next contains the property value
				Node propValueNode = propNameNode.Next; 

				// if the current property contains an object, create recursion
				if (IsObjectNode(propValueNode))
					DeserializeObject(propValueNode, propNameNode.Data, deserializer);
				else
					deserializer.SetPropertyValue(propNameNode.Data, GetNodeValue(propValueNode));
			}
			deserializer.MoveBack();
		}

		private string GetNodeValue(Node propertyValueNode)
		{
			if (propertyValueNode.Type == TokenType.GetProp)
			{
				Node nameNode = propertyValueNode.GetChild(TokenType.Name);

				// it's a property get expression (e.g. Number.MAX_VALUE)
				if (nameNode != null && nameNode.Next != null)
					return nameNode.Data + GET_PROP_CHAR + NodeDataToString(nameNode.Next);
				else
					throw new DeserializationException("The given object contains an invalid property get expression.");
			}
			else
			{
				// it's a value expression; primary tokens contain null/false/true/void
				if (propertyValueNode.Type == TokenType.Primary)
					return Enum.GetName(typeof(TokenType), propertyValueNode.Datum).ToLower();
				else
					return NodeDataToString(propertyValueNode);
			}
		}

		private string NodeDataToString(Node propertyValueNode)
		{
			if (propertyValueNode.Type == TokenType.UnaryOp
				&& propertyValueNode.FirstChild != null)
			{
				if (propertyValueNode.Datum == TokenType.Sub)
					return SUB_CHAR + propertyValueNode.FirstChild.Data;

				if (propertyValueNode.Datum == TokenType.Add)
					return ADD_CHAR + propertyValueNode.FirstChild.Data;
			}
			return propertyValueNode.Data;
		}

		private bool IsObjectNode(Node toCheck)
		{
			return (toCheck.Type == TokenType.Comma);
		}

		private string GetObjectType(Node objectNode)
		{
			Node typeNode = objectNode.GetChild(OBJECT_TYPE_NODE);

			if (typeNode == null || typeNode.Data == null)
				throw new DeserializationException("The specified script does not provide valid object definitions.");

			return typeNode.Data.ToLower();
		}

		private Node GetEntryPointNode(string toDeserialize)
		{
			Node deserializationTree = ScriptCruncher.Instance.ParseScript(
				GetDeserializationString(toDeserialize),
				DESERIALIZATION_VERSION );

			return deserializationTree.GetChild(ENTRY_POINT_NODE);
		}

		private string GetDeserializationString(string toDeserialize)
		{
			return string.Format(DESERIALIZATION_PREFIX, toDeserialize);
		}
	}
}

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
using System.Xml;

using JSTools.Parser;

namespace JSTools.Parser.DocGenerator
{
	/// <summary>
	/// Represents a comment item serializer/deserializer. You can deserialize
	/// a single comment item using the <c>Deserialize()</c> method or serialize
	/// the whole comment item context using the <c>Serialize()</c> method.
	/// </summary>
	public class CommentItemSerializer
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const string COMMENT_ITEM_NS = "JSTools.Parser.DocGenerator.CommentItems.{0}Item";

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new CommentItemSerializer instance.
		/// </summary>
		internal CommentItemSerializer()
		{
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Deserializes the specified comment node and returns the
		/// deserialized value.
		/// </summary>
		public ACommentItem Deserialize(CommentItemContext context, ACommentItem parentItem, CommentNode toDeserialize)
		{
			XmlDocument commentDoc = CommentItemSerializationContext.ToXmlDocument(toDeserialize.XmlComment);

			foreach (XmlNode node in commentDoc.DocumentElement.ChildNodes)
			{
				Type toCreate = GetType().Assembly.GetType(
					string.Format(COMMENT_ITEM_NS, node.Name),
					false,
					true );

				if (toCreate != null && toCreate.IsAssignableFrom(typeof(ACommentItem)))
				{
					return (ACommentItem)toCreate.TypeInitializer.Invoke(
						new object[]
						{
							context,
							parentItem,
							node,
							toDeserialize
						} );
				}
			}
			return null;
		}

		/// <summary>
		/// Serializes the comment nodes of the given comment item context
		/// and returns the serialized xml document.
		/// </summary>
		/// <param name="contextToSerialize">Comment item context to serialize.</param>
		/// <param name="assemblyName">Name of the assembly to generate.</param>
		/// <param name="filePath">Path of the file to serialize.</param>
		public void Serialize(CommentItemContext contextToSerialize, string assemblyName, string filePath)
		{
			using (CommentItemSerializationContext context = new CommentItemSerializationContext(assemblyName, filePath))
			{
				// serialize items
				foreach (string key in contextToSerialize.ItemKeys)
				{
					contextToSerialize[key].Serialize(context);
				}
			}
		}
	}
}

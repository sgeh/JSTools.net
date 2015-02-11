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
using System.Xml;


namespace JSTools.Xml
{
	/// <summary>
	/// This class provides basic xml functionalities, which are required for the JSTools.
	/// </summary>
	public sealed class JSToolsXmlFunctions
	{
		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Converts the value of the given node in a boolean.
		/// </summary>
		/// <param name="boolValue">The value of this XmlNode will be converted into a boolean.</param>
		/// <returns>Returns true, if the given XmlNode contains a value which has a true-like 
		/// format (e.g. TrUE, true, True, ...), otherwise false.</returns>
		public static bool GetBoolFromNodeValue(XmlNode boolValue)
		{
			return (GetValueFromNode(boolValue) != null && GetValueFromNode(boolValue).ToLower() == bool.TrueString.ToLower());
		}


		/// <summary>
		/// Returns the attribute value of the specified XmlNode.
		/// </summary>
		/// <param name="parent">Node which contains the attribute.</param>
		/// <param name="attributeName">Name of the attribute.</param>
		/// <returns>Returns the value of the specified attribute. If the attribute or the parent node were
		/// not found, you will obtain an empty string.</returns>
		public static string GetAttributeFromNode(XmlNode parent, string attributeName)
		{
			if (parent != null && parent.Attributes[attributeName] != null)
			{
				return parent.Attributes[attributeName].Value;
			}
			return string.Empty;
		}


		/// <summary>
		/// Returns the value of the specified node.
		/// </summary>
		/// <param name="node">The XmlNode object.</param>
		/// <returns>Returns an empty string, if the given XmlNode or the value contains
		/// a null reference, otherwise the value.</returns>
		public static string GetValueFromNode(XmlNode node)
		{
			if (node != null && node.Value != null)
			{
				return node.Value;
			}
			return string.Empty;
		}


		/// <summary>
		/// Creates a new attribute with the specified name and value. Appends it to the given
		/// XmlNode.
		/// </summary>
		/// <param name="node">XmlNode to append the attribute.</param>
		/// <param name="attributeName">Attribute name.</param>
		/// <param name="attributeValue">Attribute value.</param>
		/// <exception cref="ArgumentNullException">An argument contains a null reference.</exception>
		public static void AppendAttributeToNode(XmlNode node, string attributeName, string attributeValue)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node", "The specified node contains a null reference!");
			}
			if (node == null)
			{
				throw new ArgumentNullException("attributeName", "The specified attribute name contains a null reference!");
			}
			if (node == null)
			{
				throw new ArgumentNullException("attributeValue", "The specified attribute value a null reference!");
			}

			XmlAttribute attribToAppend = node.OwnerDocument.CreateAttribute(attributeName);
			attribToAppend.Value = attributeValue;
			node.Attributes.Append(attribToAppend);
		}
	}
}

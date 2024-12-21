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
using System.IO;
using System.Text;
using System.Xml;

namespace JSTools.Parser.DocGenerator
{
	/// <summary>
	/// Represents the comment item serializer.
	/// </summary>
	public class CommentItemSerializationContext : IDisposable
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const string XML_FILE_HEADER = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
		private const string XML_EXTENSION = ".xml";
		
		private const string TOP_NODE = "doc";
		private const string ASSEMBLY_NODE = "assembly";
		private const string NAME_NODE = "name";
		private const string MEMBERS_NODE = "members";
		private const string MEMBER_NODE = "member";
		private const string SUMMARY_NODE = "summary";
		private const string PARAM_NODE = "param";
		private const string REMARKS_NODE = "remarks";
		private const string EXAMPLE_NODE = "example";

		private const string NAME_ATTRIB = "name";

		private bool _isDisposed = false;
		private XmlTextWriter _serializationDoc = null;
		private FileStream _file = null;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new CommentItemSerializationContext instance.
		/// </summary>
		internal CommentItemSerializationContext(string assemblyName, string filePath)
		{
			if (Path.GetExtension(filePath) != XML_EXTENSION)
				filePath = Path.Combine(filePath, XML_EXTENSION);

			_file = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Write);
			_serializationDoc = new XmlTextWriter(_file, Encoding.UTF8);
			_serializationDoc.Formatting = Formatting.Indented;

			_serializationDoc.WriteString(XML_FILE_HEADER);
			_serializationDoc.WriteStartElement(TOP_NODE);
			_serializationDoc.WriteStartElement(ASSEMBLY_NODE);
			_serializationDoc.WriteStartElement(NAME_NODE);
			_serializationDoc.WriteString(assemblyName);
			_serializationDoc.WriteEndElement(); // </assembly>
			_serializationDoc.WriteStartElement(MEMBERS_NODE);
		}

		/// <summary>
		/// Releases the current instance. Follows the dispose pattern.
		/// </summary>
		~CommentItemSerializationContext()
		{
			EnsureDispose(false);
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new XmlDocument and parses the specified string. The
		/// parsed nodes will be added to the document element of the created
		/// node. The DocumentElement has the name "doc".
		/// </summary>
		/// <param name="toParse">XmlNodes which should be added to the created doc.</param>
		/// <returns>Returns the created XmlDocument instance.</returns>
		public static XmlDocument ToXmlDocument(string toParse)
		{
			XmlDocument commentDoc = new XmlDocument();

			// create declaration
			XmlDeclaration declaration = commentDoc.CreateXmlDeclaration(
				"1.0",
				Encoding.UTF8.EncodingName,
				null );
			commentDoc.AppendChild(declaration);

			// create document elemetn
			XmlElement docElement = commentDoc.CreateElement(TOP_NODE);
			commentDoc.AppendChild(docElement);

			// load comment
			commentDoc.DocumentElement.InnerXml = toParse;;

			return commentDoc;
		}

		#region Dispose Pattern

		/// <summary>
		///  <see cref="System.IDisposable"/>
		/// </summary>
		public void Dispose()
		{
			EnsureDispose(true);

			// This object will be cleaned up by the Dispose method.
			// Therefore, you should call GC.SupressFinalize to
			// take this object off the finalization queue 
			// and prevent finalization code for this object
			// from executing a second time.
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Dispose resources associated with the current instance.
		/// </summary>
		/// <param name="disposing">True to clean up external managed resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				// clean up external referenced resources
				// -> call _member.Dispose() method

				// finish document and save it
				_serializationDoc.WriteEndElement(); // </members>
				_serializationDoc.WriteEndElement(); // </doc>
				_serializationDoc.Flush();
				_serializationDoc.Close();
			}

			// clean up unmanaged resources
		}

		private void EnsureDispose(bool disposing)
		{
			if (!_isDisposed)
			{
				// call protected dispose method
				Dispose(disposing);

				// mark instance as disposed
				_isDisposed = true;
			}
		}

		#endregion

		/// <summary>
		/// Creates a new parameter node and adds it to the current member
		/// declaration.
		/// </summary>
		/// <param name="argumentName">Name of the argument to add.</param>
		/// <param name="descriptionText">Argument description text.</param>
		public void CreateParam(string argumentName, string descriptionText)
		{
			_serializationDoc.WriteStartElement(PARAM_NODE);
			_serializationDoc.WriteAttributeString(NAME_ATTRIB, argumentName);
			_serializationDoc.WriteString(descriptionText);
			_serializationDoc.WriteEndElement(); // </param>
		}

		/// <summary>
		/// Creates a new remarks node and adds it to the current member
		/// declaration.
		/// </summary>
		/// <param name="remarksText">Text which should be written into the remarks node.</param>
		public void CreateRemarks(string remarksText)
		{
			_serializationDoc.WriteStartElement(REMARKS_NODE);
			_serializationDoc.WriteString(remarksText);
			_serializationDoc.WriteEndElement(); // </remarks>
		}

		/// <summary>
		/// Creates a new example node and adds it to the current member
		/// declaration.
		/// </summary>
		/// <param name="exampleText">Text which should be written into the example node.</param>
		public void CreateExample(string exampleText)
		{
			_serializationDoc.WriteStartElement(EXAMPLE_NODE);
			_serializationDoc.WriteString(exampleText);
			_serializationDoc.WriteEndElement(); // </example>
		}

		/// <summary>
		/// Creates a new member entry in the output document. After
		/// you have finished adding child elements, you should call
		/// "CloseMember".
		/// </summary>
		/// <param name="memberName">Name of the member to add.</param>
		/// <param name="summaryText">Text which should be inserted into the summary node.</param>
		public void CreateMember(string memberName, string summaryText)
		{
			_serializationDoc.WriteStartElement(MEMBER_NODE);
			_serializationDoc.WriteAttributeString(NAME_ATTRIB, memberName);

			_serializationDoc.WriteStartElement(SUMMARY_NODE);
			_serializationDoc.WriteStartElement(summaryText);
			_serializationDoc.WriteEndElement(); // </summary>
		}

		/// <summary>
		/// Finishes the current member node.
		/// </summary>
		public void EndMember()
		{
			_serializationDoc.WriteEndElement(); // </member>
		}
	}
}

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

namespace JSTools.Parser.DocGenerator
{
	/// <summary>
	/// Parses the specified string and attaches the deserialized nodes to
	/// the parse item context.
	/// </summary>
	internal class CommentItemParser
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private string _treeName = string.Empty;
		private string _code = string.Empty;

		private ArrayList _errors = new ArrayList();
		private CommentItemContext _context = null;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Retruns true if an error has occured during parsing the current
		/// tree.
		/// </summary>
		internal bool HasError
		{
			get { return (_errors.Count > 0); }
		}

		/// <summary>
		/// Returns an array which contains the thrown errors.
		/// </summary>
		internal DocumentationException[] Errors
		{
			get { return (DocumentationException[])_errors.ToArray(typeof(DocumentationException[])); }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new CommentItemParser instance.
		/// </summary>
		/// <param name="context">Context which is used to load the types.</param>
		/// <param name="treeName">Name of the tree (e.g. file name).</param>
		/// <param name="code">Javascript code to parse.</param>
		internal CommentItemParser(CommentItemContext context, string treeName, string code)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			if (treeName == null || treeName.Length == 0)
				throw new ArgumentException("treeName");

			if (code == null || code.Length == 0)
				throw new ArgumentException("code");

			_treeName = treeName;
			_code = code;
			_context = context;

			InitTree();
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		private void InitTree()
		{
			try
			{
				InitTreeRecursive(null, null);
			}
			catch (ParseItemException e)
			{
				throw new DocumentationException(
					"Error while parsing the specified script code. " + e.Message,
					e.LineNumber,
					e.ColumnNumber,
					e );
			}
		}

		private void InitTreeRecursive(ACommentItem parentItem, INode parentNode)
		{
			// init children collection
			INode[] children = null;
			
			if (parentNode != null)
				children = parentNode.Children;
			else
				children = ScriptParser.Instance.ParseScript(_code).Children;

			// init children recursive
			foreach (INode childNode in children)
			{
				if (childNode is CommentNode)
				{
					ACommentItem childCommentItem = CreateCommentItem(parentItem, (CommentNode)childNode);

					if (childCommentItem == null)
						continue;

					_context[childCommentItem.ItemName.FullName] = childCommentItem;

					if (childCommentItem != null)
						InitTreeRecursive(childCommentItem, childNode);
				}
				else
				{
					InitTreeRecursive(parentItem, childNode);
				}
			}
		}

		private ACommentItem CreateCommentItem(ACommentItem parentItem, CommentNode childNode)
		{
			ACommentItem createdItem = null;

			try
			{
				createdItem = _context.Serializer.Deserialize(_context, parentItem, childNode);
			}
			catch (DocumentationException de)
			{
				ReportError(de);
			}
			catch (Exception e)
			{
				ReportError(
					"Specified comment has an invalid format.",
					childNode.LineNumberBegin,
					childNode.LineOffsetBegin,
					e );
			}

			if (createdItem == null)
			{
				ReportError(
					"Specified comment tag is not supported.",
					childNode.LineNumberBegin,
					childNode.LineOffsetBegin,
					null );
			}
			return createdItem;
		}

		private void ReportError(string message, int lineNumber, int lineOffset, Exception inner)
		{
			ReportError(new DocumentationException(
				string.Format("An error has occured in '{0}'. {1}", _treeName, message),
				lineNumber,
				lineOffset,
				inner ));
		}

		private void ReportError(DocumentationException exception)
		{
			_errors.Add(exception);
		}
	}
}

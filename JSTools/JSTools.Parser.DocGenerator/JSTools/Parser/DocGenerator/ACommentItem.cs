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
using System.Xml;

using JSTools.Parser;
using JSTools.Parser.DocGenerator.CommentItems;

namespace JSTools.Parser.DocGenerator
{
	/// <summary>
	/// Represents the base class for all comment items.
	/// </summary>
	public abstract class ACommentItem
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const string REMARKS_NODE = "remark";
		private const string EXAMPLE_NODE = "example";
		private const string EXCEPTION_NODE = "exception";

		private const string INCLUDE_XPATH = "//include";
		private const string INCLUDE_FILE_ATTRIB = "file";
		private const string INCLUDE_PATH_ATTRIB = "path";

		private const string REF_NODES = "//see|seealso";
		private const string REF_ATTRIB = "cref";

		private XmlNode _remarksNode = null;
		private XmlNode _exampleNode = null;
		private XmlNodeList _exceptionNodes = null;

		private ACommentItem _parentScope = null;
		private XmlNode _commentXmlNode = null;
		private INode _commentNode = null;
		private CommentItemContext _context = null;
		private Expression _name = null;
		private string[] _parentScopeClasses = null;

		private bool _isCreated = false;
		private bool _isInitialized = false;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the parent scope in the code hierarchy (not equal to namespace hierarchy).
		/// </summary>
		public ACommentItem ParentScope
		{
			get { return _parentScope; }
		}

		/// <summary>
		/// Gets the documentation name of the current comment item.
		/// (e.g. T:JSTools.Util.SimpleObjectSerializer)
		/// </summary>
		public abstract string DocName
		{
			get;
		}

		/// <summary>
		/// Gets the expression information of this comment item.
		/// (e.g. JSTools.Util.SimpleObjectSerializer)
		/// </summary>
		public Expression ItemName
		{
			get
			{
				if (_name == null)
					_name = InternalName;

				return _name;
			}
		}

		/// <summary>
		/// Initializes expression information of this comment item.
		/// (e.g. JSTools.Util.SimpleObjectSerializer)
		/// </summary>
		protected abstract Expression InternalName
		{
			get;
		}

		/// <summary>
		/// Returns true if this item has parent calsses.
		/// </summary>
		protected bool HasParentScopeClasses
		{
			get { return (ParentScopeClasses.Length != 0); }
		}

		/// <summary>
		/// Gets an array which contains the names of the parent classes.
		/// </summary>
		protected string[] ParentScopeClasses
		{
			get
			{
				if (_parentScopeClasses == null)
					_parentScopeClasses = GetParentScopeClasses();

				return _parentScopeClasses;
			}
		}

		/// <summary>
		/// Returns the remarks xml-node or a null reference.
		/// </summary>
		protected XmlNode RemarksNode
		{
			get { return _remarksNode; }
		}

		/// <summary>
		/// Returns the exception node list.
		/// </summary>
		protected XmlNodeList ExceptionNodes
		{
			get { return _exceptionNodes; }
		}

		/// <summary>
		/// Returns the example xml-node or a null reference.
		/// </summary>
		protected XmlNode ExampleNode
		{
			get { return _exampleNode; }
		}

		/// <summary>
		/// Gets the parsed comment node.
		/// </summary>
		protected INode CommentNode
		{
			get { return _commentNode; }
		}

		/// <summary>
		/// Returns true if the corresponding managed type was created.
		/// </summary>
		protected bool IsCreated
		{
			get { return _isCreated; }
		}

		/// <summary>
		/// Returns true if the corresponding managed type was initialized.
		/// </summary>
		protected bool IsInitialized
		{
			get { return _isInitialized; }
		}

		/// <summary>
		/// Returns the comment context.
		/// </summary>
		protected CommentItemContext Context
		{
			get { return _context; }
		}

		/// <summary>
		/// Gets the comment xml document.
		/// </summary>
		protected XmlDocument CommentXmlDocument
		{
			get { return (CommentXmlNode != null) ? CommentXmlNode.OwnerDocument : null; }
		}

		/// <summary>
		/// Gets the comment xml node which has identified this instance.
		/// </summary>
		protected XmlNode CommentXmlNode
		{
			get { return _commentXmlNode; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new ACommentItem instance.
		/// </summary>
		/// <param name="context">Context which contains the type creater and include manager.</param>
		/// <param name="parentScope">Parent item in the code hierarchy (not equal to namespace hierarchy!!).</param>
		/// <param name="commentXmlNode">Specifies the comment xml node which has identified this instance..</param>
		/// <param name="commentNode">Node which contains the parsed javascript instructions.</param>
		internal ACommentItem(
			CommentItemContext context,
			ACommentItem parentScope,
			XmlNode commentXmlNode,
			INode commentNode)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			_parentScope = parentScope;
			_commentXmlNode = commentXmlNode;
			_commentNode = commentNode;
			_context = context;
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Serializes the current instance into the given xml document.
		/// </summary>
		/// <param name="serializationContext">Context which is used to serialize the item.</param>
		public abstract void Serialize(CommentItemSerializationContext serializationContext);

		/// <summary>
		/// Loads the includes.
		/// </summary>
		public virtual void LoadIncludes()
		{
			if (CommentXmlNode != null)
			{
				// init includes
				foreach (XmlNode includeNode in CommentXmlDocument.DocumentElement.SelectNodes(INCLUDE_XPATH))
				{
					#region Init required attributes.
				
					XmlNode fileAttrib = includeNode.Attributes[INCLUDE_FILE_ATTRIB];
					XmlNode pathAttrib = includeNode.Attributes[INCLUDE_PATH_ATTRIB];
					XmlComment errorComment = CommentXmlDocument.CreateComment(string.Empty);
				
					if (fileAttrib == null)
						errorComment.Value += "Missing 'file' attribute.";

					if (pathAttrib == null)
						errorComment.Value += "Missing 'path' attribute.";
				
					#endregion

					#region Replace import node with node from file.

					if (errorComment.Value.Length == 0)
					{
						try
						{
							XmlNode toReplacement = Context.IncludeManager.GetNodeFromFile(fileAttrib.Value, pathAttrib.Value);
							XmlNode importedNode = CommentXmlDocument.ImportNode(toReplacement, true);
							includeNode.ParentNode.ReplaceChild(importedNode, includeNode);
						}
						catch (Exception e)
						{
							errorComment.Value += e.Message;
						}
					}

					#endregion

					#region Add error comment node.

					if (errorComment.Value.Length != 0)
						includeNode.ParentNode.ReplaceChild(errorComment, includeNode);

					#endregion
				}

				// init comment nodes
				LoadCommentNodes();
			}
		}

		/// <summary>
		/// Creates the type associated to this comment item.
		/// </summary>
		public virtual void CreateType()
		{
			_isCreated = true;

			// create parent classes
			if (ItemName.HasParentClass)
			{
				if (Context[ItemName.QualifiedParentClasses[0]] == null)
					Context.CreateType(ItemName.QualifiedParentClasses[0], false, false, true);

				for (int i = 1; i < ItemName.QualifiedParentClasses.Length; ++i)
				{
					Context.CreateType(ItemName.QualifiedParentClasses[i], true, false, true);
				}
			}
		}

		/// <summary>
		/// Initializes the type associated to this comment item.
		/// </summary>
		public virtual void InitType()
		{
			_isInitialized = true;
		}

		/// <summary>
		/// Resolves the references specified in the comment.
		/// </summary>
		public void ResolveReferences()
		{
			if (CommentXmlNode != null)
			{
				ResolveReferences(_exceptionNodes);
				ResolveReferences(CommentXmlDocument.DocumentElement.SelectNodes(REF_NODES));
			}
		}

		/// <summary>
		/// Loads the comment nodes. This method is called after initialize
		/// the xml include nodes and it is not called if no comment xml node
		/// was given.
		/// </summary>
		protected virtual void LoadCommentNodes()
		{
			// init remarks, example, exception nodes
			_remarksNode = CommentXmlDocument.DocumentElement.SelectSingleNode(REMARKS_NODE);
			_exampleNode = CommentXmlDocument.DocumentElement.SelectSingleNode(EXAMPLE_NODE);
			_exceptionNodes = CommentXmlDocument.DocumentElement.SelectNodes(EXCEPTION_NODE);
		}

		private void ResolveReferences(IEnumerable nodesToResolve)
		{
			// resolve reference
			foreach (XmlNode referenceNode in nodesToResolve)
			{
				XmlAttribute crefAttrib = referenceNode.Attributes[REF_ATTRIB];

				if (crefAttrib == null)
				{
					crefAttrib = CommentXmlDocument.CreateAttribute(REF_ATTRIB);
					crefAttrib.Value = Context.ResolveItemName(null);
					referenceNode.Attributes.Append(crefAttrib);
				}
				crefAttrib.Value = Context[crefAttrib.Value].DocName;
			}
		}

		private string[] GetParentScopeClasses()
		{
			ArrayList parentItemNames = new ArrayList();

			#region Evaluate parent items.

			ACommentItem parentItem = ParentScope;

			while (parentItem != null)
			{
				if (parentItem is ClassItem)
				{
					if (parentItem.ParentScope != null)
						parentItemNames.Add(parentItem.ItemName.Name);
					else
						parentItemNames.Add(parentItem.ItemName.FullName);
				}
				parentItem = parentItem.ParentScope;
			}

			#endregion

			#region Invert array and evaluate full name of top item.

			parentItemNames.Reverse();
			return (string[])parentItemNames.ToArray(typeof(string[]));

			#endregion
		}
	}
}

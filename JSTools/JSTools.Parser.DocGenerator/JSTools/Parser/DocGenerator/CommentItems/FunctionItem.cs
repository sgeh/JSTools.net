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

namespace JSTools.Parser.DocGenerator.CommentItems
{
	/// <summary>
	/// Represents the function comment item.
	/// </summary>
	internal class FunctionItem : ACommentItem
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const string RETURNS_NODE = "returns";
		private const string TYPE_ATTRIB = "type";

		private const string ACCESSOR_ATTRIB = "accessor";
		private const string MODIFIER_ATTRIB = "modifier";
		private const string CLASS_ATTRIB = "class";

		private const char RETURN_TYPE_SEPARATOR = '#';
		private const string DOC_PREFIX = "M:{0}";
		private const string HEADER_PATTERN = "M:{0}.{1}#{2}";

		private Accessor _accessor = Accessor.Public;
		private MemberModifier _modifier = MemberModifier.None;
		private string _class = string.Empty;
		private string _type = string.Empty;

		private ClassItem _returnType = null;
		private XmlNode _returnNode = null;
		private string _methodHeader = null;

		private string _codeExpression = string.Empty;
		private ParamHandler _params = null;
		private string _docName = null;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets the documentation name of the current comment item.
		/// (e.g. M:JSTools.Util.SimpleObjectSerializer(Global.Window.Object)
		/// </summary>
		public override string DocName
		{
			get
			{
				if (_docName == null)
					_docName = _params.GetMethodHeader(MethodHeaderBegin);

				return _docName;
			}
		}

		/// <summary>
		/// Initializes expression information of this comment item.
		/// (e.g. JSTools.Util.SimpleObjectSerializer)
		/// </summary>
		protected override Expression InternalName
		{
			get
			{
				return new Expression(
					Context.DefaultType,
					(_class.Length > 0) ? new string[] { _class } : ParentScopeClasses,
					_codeExpression );
			}
		}

		private string MethodHeaderBegin
		{
			get
			{
				if (_methodHeader == null)
				{
					// if a return node was specified
					if (_returnType != null)
					{
						_methodHeader = string.Format(
							HEADER_PATTERN,
							ItemName.ToString(true),
							_returnType.ItemName.ToString(false, RETURN_TYPE_SEPARATOR),
							ItemName.Name );
					}
					else
					{
						_methodHeader = string.Format(
							DOC_PREFIX,
							ItemName.FullName );
					}
				}
				return _methodHeader;
			}
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new FunctionItem instance.
		/// </summary>
		/// <param name="context">Context which contains the type creater and include manager.</param>
		/// <param name="parentScope">Parent item in the code hierarchy (not equal to namespace hierarchy!!).</param>
		/// <param name="commentXmlNode">Specifies the comment xml node which has identified this instance..</param>
		/// <param name="parsedNode">Node which contains the parsed javascript instructions.</param>
		internal FunctionItem(
			CommentItemContext context,
			ACommentItem parentScope,
			XmlNode commentXmlNode,
			INode parsedNode) : base(context, parentScope, commentXmlNode, parsedNode)
		{
			XmlAttribute accessorNode = CommentXmlNode.Attributes[ACCESSOR_ATTRIB];
			XmlAttribute modifierNode = CommentXmlNode.Attributes[MODIFIER_ATTRIB];
			XmlAttribute classNode = CommentXmlNode.Attributes[CLASS_ATTRIB];

			if (accessorNode != null)
			{
				try { _accessor = (Accessor)Enum.Parse(typeof(Accessor), accessorNode.Value, true); }
				catch { /* ignore exceptions */ }
			}

			if (modifierNode != null)
			{
				try { _modifier = (MemberModifier)Enum.Parse(typeof(MemberModifier), modifierNode.Value, true); }
				catch { /* ignore exceptions */ }
			}
			
			if (classNode != null)
				_class = classNode.Value;

			_codeExpression = InitNodeExpression();
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
		public override void Serialize(CommentItemSerializationContext serializationContext)
		{
			// create constructors
			string[] methodHeaders = _params.GetMethodHeaders(MethodHeaderBegin);

			for (int i = 0; i < _params.OverloadingCount; ++i)
			{
				serializationContext.CreateMember(methodHeaders[i], CommentXmlNode.InnerXml);

				for (int j = 0; j < _params.Overloadings[i].Length; ++j)
				{
					serializationContext.CreateParam(
						_params.Overloadings[i][j].Name,
						_params.Overloadings[i][j].Comment.InnerXml );
				}
				serializationContext.EndMember();
			}
		}

		/// <summary>
		/// Initializes the type associated to this comment item.
		/// </summary>
		public override void InitType()
		{
			if (!IsInitialized)
			{
				base.InitType();
				CreateMethods();
				CreateReturnType();
			}
		}

		/// <summary>
		/// Loads the comment nodes. This method is called after initialize
		/// the xml include nodes.
		/// </summary>
		protected override void LoadCommentNodes()
		{
			// init remarks, example, exception nodes
			base.LoadCommentNodes();

			// init param list
			_params = new ParamHandler(CommentXmlDocument, Context);
			_returnNode = CommentXmlDocument.SelectSingleNode(RETURNS_NODE);

			// init return type value
			if (_returnNode != null)
			{
				XmlAttribute typeAttribute = _returnNode.Attributes[TYPE_ATTRIB];

				if (typeAttribute != null)
					_type = typeAttribute.Value;
			}
		}

		private string InitNodeExpression()
		{
			foreach (INode childNode in CommentNode.LastChild.Children)
			{
				if (childNode is ExpressionNode)
					return childNode.ParsedCode;
			}
			return string.Empty;
		}

		private void CreateMethods()
		{
			for (int i = 0; i < _params.OverloadingCount; ++i)
			{
				Type[] paramTypes = new Type[_params.Overloadings[i].Length];
				string[] paramNames = new string[_params.Overloadings[i].Length];

				for (int j = 0; j < _params.Overloadings[i].Length; ++j)
				{
					paramTypes[j] = _params.Overloadings[i][j].Type;
					paramNames[j] = _params.Overloadings[i][j].Name;
				}

				Context.TypeContext.RegisterFunction(
					ItemName.ToString(true),
					ItemName.Name,
					_accessor,
					_modifier,
					(_returnType != null) ? _returnType.CreatedType : null,
					paramTypes,
					paramNames );
			}
		}

		private void CreateReturnType()
		{
			if (_returnNode != null)
				_returnType = (Context[_type, true] as ClassItem);
		}
	}
}

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
	/// Represents the class comment item.
	/// </summary>
	internal class ClassItem : ACommentItem
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const string ACCESSOR_ATTRIB = "accessor";
		private const string MODIFIER_ATTRIB = "modifier";
		private const string PARENT_ATTRIB = "parent";
		private const string NAMESPACE_ATTRIB = "namespace";
		private const string EXTENDS_ATTRIB = "extends";
		private const string IMPLEMENTS_ATTRIB = "implements";

		private const string DOC_PREFIX = "T:{0}";
		private const string DOC_CONSTR = "M:{0}.#ctor";
		private const string CONSTR_SUMMARY = "<see cref=\"{0}\" />";

		private Accessor _accessor = Accessor.Public;
		private ClassModifier _modifier = ClassModifier.None;
		private ParentType _parent = ParentType.NameSpace;
		private string _namespace = string.Empty;
		private string _extends = string.Empty;
		private string[] _implements = new string[0];
		private string _codeExpression = string.Empty;

		private ParamHandler _params = null;
		private Type _createdType = null;

		private enum ParentType
		{
			Class,
			NameSpace
		}

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Returns the type instance which was created by the CreatType()
		/// method.
		/// </summary>
		public Type CreatedType
		{
			get { return _createdType; }
		}

		/// <summary>
		/// Gets the documentation name of the current comment item.
		/// (e.g. T:JSTools.Util.SimpleObjectSerializer)
		/// </summary>
		public override string DocName
		{
			get { return string.Format(DOC_PREFIX, ItemName.FullName); }
		}

		/// <summary>
		/// Initializes expression information of this comment item.
		/// (e.g. JSTools.Util.SimpleObjectSerializer)
		/// </summary>
		protected override Expression InternalName
		{
			get
			{
				// 1. priority -> namespace attribute
				if (_namespace.Length > 0)
					return new Expression(Context.DefaultNameSpace,_namespace, _codeExpression);

				// 2. priority -> script code
				if (Expression.HasParentElement(_codeExpression))
				{
					return new Expression(
						Context.DefaultType,
						Context.DefaultNameSpace,
						_codeExpression,
						(_parent == ParentType.Class));
				}

				// 3. priority -> parent
				if (HasParentScopeClasses)
					return new Expression(Context.DefaultType, ParentScopeClasses, _codeExpression);

				// 4. priority -> no parent item, only class name specified
				return new Expression(Context.DefaultNameSpace, string.Empty, _codeExpression);
			}
		}

		private bool IsInterface
		{
			get { return (_modifier == ClassModifier.Interface); }
		}

		private bool IsSealed
		{
			get { return (_modifier == ClassModifier.Sealed); }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new ClassItem instance.
		/// </summary>
		/// <param name="context">Context which contains the type creater and include manager.</param>
		/// <param name="parentScope">Parent item in the code hierarchy (not equal to namespace hierarchy!!).</param>
		/// <param name="commentXmlNode">Specifies the comment xml node which has identified this instance..</param>
		/// <param name="parsedNode">Node which contains the parsed javascript instructions.</param>
		internal ClassItem(
			CommentItemContext context,
			ACommentItem parentScope,
			XmlNode commentXmlNode,
			INode parsedNode) : base(context, parentScope, commentXmlNode, parsedNode)
		{
			XmlAttribute accessorNode = CommentXmlNode.Attributes[ACCESSOR_ATTRIB];
			XmlAttribute modifierNode = CommentXmlNode.Attributes[MODIFIER_ATTRIB];
			XmlAttribute parentNode = CommentXmlNode.Attributes[PARENT_ATTRIB];
			XmlAttribute namespaceNode = CommentXmlNode.Attributes[NAMESPACE_ATTRIB];
			XmlAttribute extendsNode = CommentXmlNode.Attributes[EXTENDS_ATTRIB];
			XmlAttribute implementsNode = CommentXmlNode.Attributes[IMPLEMENTS_ATTRIB];

			if (accessorNode != null)
			{
				try { _accessor = (Accessor)Enum.Parse(typeof(Accessor), accessorNode.Value, true); }
				catch { /* ignore exceptions */ }
			}

			if (modifierNode != null)
			{
				try { _modifier = (ClassModifier)Enum.Parse(typeof(ClassModifier), modifierNode.Value, true); }
				catch { /* ignore exceptions */ }
			}
			
			if (parentNode != null)
			{
				try { _parent = (ParentType)Enum.Parse(typeof(ParentType), parentNode.Value, true); }
				catch { /* ignore exceptions */ }
			}

			if (namespaceNode != null) _namespace = namespaceNode.Value;
			if (extendsNode != null) _extends = extendsNode.Value;
			if (implementsNode != null) _implements = implementsNode.Value.Split(new char[] { ',' } );
			_codeExpression = InitNodeExpression();
		}

		internal ClassItem(CommentItemContext context, string fullItemName, bool hasParentClass) : base(context, null, null, null)
		{
			_codeExpression = fullItemName;

			if (hasParentClass)
				_parent = ParentType.Class;
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
			// create class
			serializationContext.CreateMember(
				DocName,
				(CommentXmlNode != null) ? CommentXmlNode.InnerXml : string.Empty );

			if (RemarksNode != null)
				serializationContext.CreateRemarks(RemarksNode.InnerXml);

			if (ExampleNode != null)
				serializationContext.CreateExample(ExampleNode.InnerXml);

			serializationContext.EndMember();

			// create constructors
			string constrSummary = string.Format(CONSTR_SUMMARY, DocName);
			string[] constrHeaders = _params.GetMethodHeaders(string.Format(DOC_CONSTR, ItemName.FullName));

			for (int i = 0; i < _params.OverloadingCount; ++i)
			{
				serializationContext.CreateMember(constrHeaders[i], constrSummary);
				
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
		/// Creates the type associated to this comment item.
		/// </summary>
		public override void CreateType()
		{
			if (!IsCreated)
			{
				base.CreateType();

				if (!ItemName.HasParentClass)
				{
					_createdType = Context.TypeContext.RegisterClass(
						ItemName.FullName,
						_accessor,
						_modifier,
						GetBaseType(),
						GetBaseInterfaces() );
				}
				else
				{
					_createdType = Context.TypeContext.RegisterNestedClass(
						ItemName.QualifiedParentClasses[ItemName.QualifiedParentClasses.Length - 1],
						ItemName.Name,
						_accessor,
						_modifier,
						GetBaseType(),
						GetBaseInterfaces() );		
				}
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
				CreateConstructors();
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

		private void CreateConstructors()
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

				Context.TypeContext.RegisterConstructor(
					ItemName.FullName,
					paramTypes,
					paramNames );
			}
		}

		private Type GetBaseType()
		{
			if (!IsInterface)
			{
				ClassItem baseType = (ClassItem)Context.CreateType(_extends, false, true, false);

				if (baseType != null && !baseType.IsSealed)
					return baseType.CreatedType;
			}
			return null;
		}

		private Type[] GetBaseInterfaces()
		{
			ArrayList baseInterfaces = new ArrayList(_implements.Length);

			for (int i = 0; i < baseInterfaces.Count; ++i)
			{
				ClassItem interfaceType = (ClassItem)Context.CreateType(_implements[i], false, false, false);
				
				if (interfaceType != null && interfaceType.IsInterface)
					baseInterfaces.Add(interfaceType.CreatedType);
			}
			return (Type[])baseInterfaces.ToArray(typeof(Type[]));
		}
	}
}

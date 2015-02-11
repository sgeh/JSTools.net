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

namespace JSTools.Parser.DocGenerator
{
	/// <summary>
	/// Represents a javascript code expression which contains the parent
	/// classes, namespace and class and member name of the specified
	/// term.
	/// </summary>
	public class Expression
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const string CLASS_PROPERTY = "property";
		private const char EXPRESSION_SEPARATOR = '.';
		private const string NAMESPACE_COMBINE = "{0}.{1}";

		private string[] _parentClasses = null;
		private string[] _qualifiedParentClasses = null;
		private string _namespace = string.Empty;
		private string _name = string.Empty;
		private string _expression = null;

		private string _defaultClass = string.Empty;
		private string _defaultNamespace = string.Empty;

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		/// <summary>
		/// Gets a list which contains the names of the full qualified parent
		/// classes. The top class is the first entry of the list.
		/// </summary>
		public string[] QualifiedParentClasses
		{
			get { return _qualifiedParentClasses; }
		}

		/// <summary>
		/// Gets a list which contains the names of the parent classes. The
		/// top class is the first entry of the list.
		/// </summary>
		public string[] ParentClasses
		{
			get { return _parentClasses; }
		}

		/// <summary>
		/// Returns true if this expression contains a parent class expression.
		/// </summary>
		public bool HasParentClass
		{
			get { return (ParentClasses.Length > 0); }
		}

		/// <summary>
		/// Gets the namespace part of the string.
		/// </summary>
		public string NameSpace
		{
			get { return _namespace; }
		}

		/// <summary>
		/// Gets the class name part of the string.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// Gets the full qualified expression name.
		/// </summary>
		public string FullName
		{
			get
			{
				if (_expression == null)
					_expression = ToString();
				return _expression;
			}
		}

		/// <summary>
		/// Specifies the default namespace, which is used if no namespace
		/// is specified but a namespace is required.
		/// </summary>
		public string DefaultNameSpace
		{
			get { return _defaultNamespace; }
		}

		/// <summary>
		/// Specifies the default type, which is used if no type
		/// is specified but a type is required.
		/// </summary>
		public string DefaultClass
		{
			get { return _defaultClass; }
		}

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		internal Expression(string defaultClassExpression, string[] classNames)
		{
			_defaultClass = (defaultClassExpression != null) ? defaultClassExpression : string.Empty;

			if (classNames != null && classNames.Length > 0)
			{
				_name = GetNameFragment(classNames[classNames.Length - 1]);

				// init parent classes
				string[] parentClasses = new string[classNames.Length - 1];

				if (parentClasses.Length > 0)
					Array.Copy(classNames, parentClasses, _parentClasses.Length - 1);

				InitParentClasses(parentClasses);
			}
		}

		internal Expression(string defaultClassExpression, string[] classNames, string memberName)
		{
			if (memberName == null || memberName.Length == 0)
				throw new ArgumentNullException("memberName");

			_defaultClass = (defaultClassExpression != null) ? defaultClassExpression : string.Empty;
			_name = memberName;

			if (classNames != null && classNames.Length > 0)
				InitParentClasses(classNames);
		}

		internal Expression(string defaultNamespaceExpression, string namespaceName, string className)
		{
			if (className == null || className.Length == 0)
				throw new ArgumentNullException("memberName");

			_defaultNamespace = (defaultNamespaceExpression != null) ? defaultNamespaceExpression : string.Empty;
			_namespace = (namespaceName != null && namespaceName.Length > 0) ? namespaceName : defaultNamespaceExpression;
			_name = className;
		}

		internal Expression(string defaultClassExpression, string namespaceName, string className, string memberName)
		{
			if (memberName == null || memberName.Length == 0)
				throw new ArgumentNullException("memberName");

			_name = memberName;
			_defaultClass = (defaultClassExpression != null) ? defaultClassExpression : string.Empty;			

			if (className != null && className.Length > 0)
			{
				_namespace = (namespaceName != null && namespaceName.Length > 0) ? namespaceName : string.Empty;
				_parentClasses = new string[] { className };
				_qualifiedParentClasses = new string[] { (_namespace.Length > 0) ? string.Format(NAMESPACE_COMBINE, _namespace, className) : className };
			}
			else
			{
				_namespace = GetNameSpaceFragment(_defaultClass);
				_parentClasses = new string[] { GetNameFragment(_defaultClass) };
				_qualifiedParentClasses = new string[] { _defaultClass };
			}
		}

		internal Expression(
			string defaultClassExpression,
			string defaultNamespaceExpression,
			string expression,
			bool hasParentClass )
		{
			_defaultClass = (defaultClassExpression != null) ? defaultClassExpression : string.Empty;
			_defaultNamespace = (defaultNamespaceExpression != null) ? defaultNamespaceExpression : string.Empty;

			if (expression == null || expression.Length == 0)
				throw new ArgumentNullException("expression");

			InitExpression(expression, hasParentClass);
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		public static string Combine(string nsOrClass, string className, string defaultNsOrClass, string defaultClassName)
		{
			if (nsOrClass == null)
				nsOrClass = defaultNsOrClass;

			if (className == null)
				className = defaultClassName;

			if (nsOrClass == null && className == null)
				return string.Empty;

			if (nsOrClass == null)
				return className;

			if (className == null)
				return nsOrClass;

			return string.Format(NAMESPACE_COMBINE, nsOrClass, className);
		}

		public static string Combine(string ns, string className)
		{
			return Combine(ns, className, null, null);
		}

		public static bool IsParent(string fullChildName, string parentName)
		{
			if (fullChildName == null || parentName == null)
				return false;

			return fullChildName.StartsWith(parentName + EXPRESSION_SEPARATOR);
		}

		public static bool HasParentElement(string toCheck)
		{
			return (toCheck != null && toCheck.IndexOf(EXPRESSION_SEPARATOR) != -1);
		}

		/// <summary>
		/// Evaluates the full expression.
		/// </summary>
		/// <returns>Returns the full expression.</returns>
		public override string ToString()
		{
			return ToString(false, EXPRESSION_SEPARATOR);
		}

		/// <summary>
		/// Evaluates the given parts of the expression.
		/// </summary>
		/// <param name="parentOnly">True to render only the parent elements.</param>
		/// <param name="parentSeparator">Separator which is used to render the string.</param>
		/// <returns>Returns the evaluated expression string.</returns>
		public string ToString(bool parentOnly, char parentSeparator)
		{
			StringBuilder expression = new StringBuilder(NameSpace);

			if (NameSpace.Length > 0)
				expression.Append(parentSeparator);

			for (int i = 0; i < _parentClasses.Length; ++i)
			{
				expression.Append(_parentClasses[i]);

				if (i + i != _parentClasses.Length)
					expression.Append(parentSeparator);
			}

			if (!parentOnly && Name.Length > 0)
			{
				expression.Append(parentSeparator);
				expression.Append(Name);
			}
			return expression.ToString();
		}

		/// <summary>
		/// Evaluates the given parts of the expression.
		/// </summary>
		/// <param name="parentOnly">True to render only the parent elements.</param>
		/// <returns>Returns the evaluated expression string.</returns>
		public string ToString(bool parentOnly)
		{
			return ToString(parentOnly, EXPRESSION_SEPARATOR);
		}

		private string GetNameSpaceFragment(string expression)
		{
			int lastExprSeparator = expression.LastIndexOf(EXPRESSION_SEPARATOR);

			if (lastExprSeparator != -1)
				return expression.Substring(0, expression.Length - lastExprSeparator);

			return string.Empty;
		}

		private string GetNameFragment(string expression)
		{
			int lastExprSeparator = expression.LastIndexOf(EXPRESSION_SEPARATOR);

			if (lastExprSeparator != -1)
				return expression.Substring(lastExprSeparator + 1, expression.Length - lastExprSeparator);

			return expression;
		}

		private void InitParentClasses(IList classNames)
		{
			ArrayList parentClasses = new ArrayList(classNames.Count);
			ArrayList qualifiedParentClasses = new ArrayList(classNames.Count);

			// get default type if no parent class was specified
			if (classNames.Count == 0 && DefaultClass.Length > 0)
				classNames = new string[] { DefaultClass };

			for (int i = 0; i < classNames.Count; ++i)
			{
				// init current index
				string className = (classNames[i] as string);

				if (className == null || className.Length == 0)
					continue;

				// init parent class name
				parentClasses[i] = GetNameFragment(className);

				// first entry contains the full qualified name of the top class
				if (i == 0)
				{
					qualifiedParentClasses[i] = className;
				}
				else
				{
					qualifiedParentClasses[i] = Combine(
						(string)qualifiedParentClasses[i - 1],
						(string)parentClasses[i] );
				}
			}

			_namespace = GetNameSpaceFragment((string)classNames[0]);
			_parentClasses = (string[])parentClasses.ToArray(typeof(string[]));
			_qualifiedParentClasses = (string[])qualifiedParentClasses.ToArray(typeof(string[]));
		}

		private void InitExpression(string expression, bool hasParentClass)
		{
			if (!hasParentClass)
			{
				_name = GetNameFragment(expression);
				_namespace = GetNameSpaceFragment(expression);

				if (_namespace.Length == 0)
					_namespace = DefaultNameSpace;
			}
			else
			{
				ArrayList expressions = InitExpressions(expression.Split(EXPRESSION_SEPARATOR));
				_name = (string)expressions[0];

				InitParentClasses(expressions.GetRange(1, expressions.Count - 1));
			}
		}

		private ArrayList InitExpressions(string[] expressions)
		{
			ArrayList output = new ArrayList(expressions.Length);
			int lastPrototypeIndex = expressions.Length;

			// try to evaluate 'property' identifier, which specifies a parent class
			for (int i = 0; i < expressions.Length - 1; ++i)
			{
				if (expressions[i] == CLASS_PROPERTY)
				{
					output.Add(string.Join(
						string.Empty,
						expressions,
						i + 1,
						lastPrototypeIndex - i - 1) );

					lastPrototypeIndex = i;
				}
			}
			return output;
		}
	}
}

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
using System.Reflection;
using System.Reflection.Emit;

namespace JSTools.Parser.DocGenerator
{
	/// <summary>
	/// Describes the accessor of a class/function/variable/enum declaration.
	/// </summary>
	internal enum Accessor
	{
		Private,
		Protected,
		Public
	}

	/// <summary>
	/// Describes the modifiers of a class declaration.
	/// </summary>
	internal enum ClassModifier
	{
		None,
		Interface,
		Abstract,
		Sealed
	}

	/// <summary>
	/// Describes the modifiers of a member (function/variable) declaration.
	/// </summary>
	internal enum MemberModifier
	{
		None,
		Static,
		Abstract
	}

	/// <summary>
	/// Represents the .NET assembly generator, which contains all types
	/// and the assigned methods.
	/// </summary>
	internal class TypeContext
	{
		//--------------------------------------------------------------------
		// Declarations
		//--------------------------------------------------------------------

		private const string DLL_EXTENSION = ".dll";

		private const string JAVASCRIPT_MODULE = "JavaScriptModule";
		private const string PROP_SET_PREFIX = "set_{0}";
		private const string PROP_GET_PREFIX = "get_{0}";
		private const string SPECIAL_ENUM_VALUE = "value__";

		private AssemblyBuilder _assembly = null;
		private ModuleBuilder _codeModule = null;
		private Hashtable _typeBuilders = new Hashtable();

		//--------------------------------------------------------------------
		// Properties
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Constructors / Destructor
		//--------------------------------------------------------------------

		/// <summary>
		/// Creates a new TypeContext instance.
		/// </summary>
		internal TypeContext(string assemblyName)
		{
			if (assemblyName == null || assemblyName.Length == 0)
				throw new ArgumentException("assemblyName");

			// get the current AppDomain
			AppDomain appDomain = AppDomain.CurrentDomain;
			AssemblyName assemblyNameId = new AssemblyName();
			assemblyNameId.Name = assemblyName;

			// create the dynamic assembly and set its access mode to 'Save'
			_assembly = appDomain.DefineDynamicAssembly(assemblyNameId, AssemblyBuilderAccess.Save);

			// create a dynamic script module
			_codeModule = _assembly.DefineDynamicModule(JAVASCRIPT_MODULE, true);
		}

		//--------------------------------------------------------------------
		// Events
		//--------------------------------------------------------------------

		//--------------------------------------------------------------------
		// Methods
		//--------------------------------------------------------------------

		/// <summary>
		/// Saves the created assembly.
		/// </summary>
		public void Serialize(string filePath)
		{
			if (Path.GetExtension(filePath) != DLL_EXTENSION)
				filePath = Path.Combine(filePath, DLL_EXTENSION);

			_assembly.Save(filePath);
		}

		/// <summary>
		/// Registers a new class in the assembly.
		/// </summary>
		public Type RegisterClass(
			string fullTypeName,
			Accessor accessor,
			ClassModifier modifier,
			Type parentType,
			Type[] interfaces)
		{
			TypeAttributes attribute = InitAccessor(accessor, TypeAttributes.AutoClass);
			TypeBuilder typeBuilder = _codeModule.DefineType(
				fullTypeName,
				InitModifier(modifier, attribute),
				(parentType != null) ? parentType : typeof(object),
				interfaces );

			_typeBuilders[fullTypeName] = typeBuilder;
			return typeBuilder;
		}

		/// <summary>
		/// Registers a new nested class in the assembly.
		/// </summary>
		public Type RegisterNestedClass(
			string fullParentTypeName,
			string typeName,
			Accessor accessor,
			ClassModifier modifier,
			Type parentType,
			Type[] interfaces)
		{
			string parentClassKey = fullParentTypeName;
			string fullTypeName = Expression.Combine(parentClassKey, typeName);

			if (_typeBuilders[parentClassKey] == null)
				throw new ArgumentException("The given parent class does not exist.");

			TypeAttributes attribute = InitNestedAccessor(accessor, TypeAttributes.AutoClass);
			TypeBuilder typeBuilder = ((TypeBuilder)_typeBuilders[fullTypeName]).DefineNestedType(
				typeName,
				InitModifier(modifier, attribute),
				(parentType != null) ? parentType : typeof(object),
				interfaces );

			_typeBuilders[fullTypeName] = typeBuilder;
			return typeBuilder;
		}

		/// <summary>
		/// Registers a new constructor assigned to the specified class in the assembly.
		/// </summary>
		public void RegisterConstructor(
			string fullTypeName,
			Type[] parameterTypes,
			string[] parameterNames)
		{
			TypeBuilder typeBuilder = (_typeBuilders[fullTypeName] as TypeBuilder);

			if (typeBuilder == null)
				throw new ArgumentException("The given class does not exist.");

			ConstructorBuilder constrBuilder = typeBuilder.DefineConstructor(
				MethodAttributes.Public,
				CallingConventions.Standard,
				parameterTypes );

			#region Initialize parameters.

			if (parameterNames != null && parameterTypes != null)
			{
				for (int i = 0; i < parameterNames.Length; ++i)
				{
					constrBuilder.DefineParameter(i, ParameterAttributes.None, parameterNames[i]);
				}
			}

			#endregion

			#region Initialize method body.

			ILGenerator methodBody = constrBuilder.GetILGenerator();
			methodBody.Emit(OpCodes.Ret);

			#endregion
		}

		/// <summary>
		/// Registers a new enumeration in the assembly.
		/// </summary>
		public void RegisterEnum(
			string fullEnumTypeName,
			Accessor accessor,
			string[] enumLiterals)
		{
			EnumBuilder enumBuilder = _codeModule.DefineEnum(
				fullEnumTypeName,
				InitAccessor(accessor, TypeAttributes.AutoClass) | TypeAttributes.Sealed,
				typeof(int));

			#region Define enum fields/literals.

			if (enumLiterals != null)
			{
				for (int i = 0; i < enumLiterals.Length; ++i)
				{
					enumBuilder.DefineLiteral(enumLiterals[i], i);
				}
			}

			#endregion
		}

		/// <summary>
		/// Registers a new nested enumeration in the assembly.
		/// </summary>
		public void RegisterNestedEnum(
			string fullParentTypeName,
			string enumName,
			Accessor accessor,
			string[] enumLiterals)
		{
			string parentClassKey = fullParentTypeName;

			if (_typeBuilders[parentClassKey] == null)
				throw new ArgumentException("The given parent class does not exist.");

			TypeBuilder enumBuilder = ((TypeBuilder)_typeBuilders[parentClassKey]).DefineNestedType(
				enumName,
				InitNestedAccessor(accessor, TypeAttributes.AutoClass) | TypeAttributes.Sealed,
				typeof(Enum) );

			enumBuilder.DefineField(
				SPECIAL_ENUM_VALUE,
				typeof(int),
				FieldAttributes.Private | FieldAttributes.SpecialName );

			#region Define enum fields/literals.

			if (enumLiterals != null)
			{
				for (int i = 0; i < enumLiterals.Length; ++i)
				{
					FieldBuilder fieldBuilder = enumBuilder.DefineField(
						enumLiterals[i],
						typeof(int),
						FieldAttributes.Public | FieldAttributes.Literal | FieldAttributes.Static );

					fieldBuilder.SetConstant(i);
				}
			}

			#endregion
		}

		/// <summary>
		/// Registers a new property in a class in the assembly.
		/// </summary>
		public void RegisterVariable(
			string typeName,
			string variableName,
			Type variableType,
			Accessor accessor,
			MemberModifier modifier)
		{
			TypeBuilder typeBuilder = (_typeBuilders[typeName] as TypeBuilder);

			if (typeBuilder == null)
				throw new ArgumentException("The given class does not exist.");

			if (variableName == null || variableName.Length == 0)
				throw new ArgumentException("The given name is empty.", "variableName");

			if (variableType == null)
				throw new ArgumentNullException("variableType", "The given variable type is empty.");

			if (typeBuilder.GetProperty(variableName) == null)
			{
				MethodBuilder setMethod = CreateMethod(
					typeBuilder,
					string.Format(PROP_SET_PREFIX, variableName),
					accessor,
					modifier,
					null,
					new Type[] { variableType } ,
					new string[] { variableName } );

				MethodBuilder getMethod = CreateMethod(
					typeBuilder,
					string.Format(PROP_GET_PREFIX, variableName),
					accessor,
					modifier,
					variableType,
					null,
					null );

				PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(
					variableName,
					PropertyAttributes.HasDefault,
					variableType,
					new Type[] { variableType } );

				propertyBuilder.SetSetMethod(setMethod);
				propertyBuilder.SetGetMethod(getMethod);
			}
		}

		/// <summary>
		/// Registers a new method in a class in the assembly.
		/// </summary>
		public void RegisterFunction(
			string typeName,
			string methodName,
			Accessor accessor,
			MemberModifier modifier,
			Type returnType,
			Type[] parameterTypes,
			string[] parameterNames)
		{
			TypeBuilder typeBuilder = (_typeBuilders[typeName] as TypeBuilder);

			if (typeBuilder == null)
				throw new ArgumentException("The given class does not exist.");

			if (methodName == null || methodName.Length == 0)
				throw new ArgumentException("The given name is empty.", "methodName");

			if (typeBuilder.GetMethod(methodName, parameterTypes) == null)
			{
				CreateMethod(
					typeBuilder,
					methodName,
					accessor,
					modifier,
					returnType,
					parameterTypes,
					parameterNames );
			}
		}

		private MethodBuilder CreateMethod(
			TypeBuilder typeBuilder,
			string methodName,
			Accessor accessor,
			MemberModifier modifier,
			Type returnType,
			Type[] parameterTypes,
			string[] parameterNames)
		{
			#region Initialize method attributes.
			
			MethodAttributes attributes = MethodAttributes.Public;

			if (accessor == Accessor.Protected)
				attributes = MethodAttributes.Family;
			else if (accessor == Accessor.Private)
				attributes = MethodAttributes.Private;

			if (modifier == MemberModifier.Abstract)
				attributes |= MethodAttributes.Abstract;
			else if (modifier == MemberModifier.Static)
				attributes |= MethodAttributes.Static;

			#endregion

			MethodBuilder methodBuilder = typeBuilder.DefineMethod(methodName, attributes, returnType, parameterTypes);

			#region Initialize parameters.
			
			if (parameterNames != null && parameterTypes != null)
			{
				for (int i = 0; i < parameterNames.Length && i < parameterTypes.Length; ++i)
				{
					methodBuilder.DefineParameter(i, ParameterAttributes.None, parameterNames[i]);
				}
			}

			#endregion

			#region Initialize method body.

			ILGenerator methodBody = methodBuilder.GetILGenerator();

			if (returnType != null)
			{
				methodBody.DeclareLocal(typeof(Object));
				methodBody.Emit(OpCodes.Ldnull);
				methodBody.Emit(OpCodes.Stloc_0);
				methodBody.Emit(OpCodes.Ldloc_0);
			}
			methodBody.Emit(OpCodes.Ret);

			#endregion

			return methodBuilder;
		}

		private TypeAttributes InitModifier(ClassModifier modifier, TypeAttributes attributes)
		{
			if (modifier == ClassModifier.Abstract)
				attributes |= TypeAttributes.Abstract;
			else if (modifier == ClassModifier.Interface)
				attributes |= TypeAttributes.Interface;
			else if (modifier == ClassModifier.Sealed)
				attributes |= TypeAttributes.Sealed;

			return attributes;
		}

		private TypeAttributes InitAccessor(Accessor accessor, TypeAttributes attributes)
		{
			if (accessor == Accessor.Public)
				attributes |= TypeAttributes.Public;
			else
				attributes |= TypeAttributes.NestedAssembly;

			return attributes;
		}

		private TypeAttributes InitNestedAccessor(Accessor accessor, TypeAttributes attributes)
		{
			if (accessor == Accessor.Public)
				attributes |= TypeAttributes.NestedPublic;
			else if (accessor == Accessor.Protected)
				attributes |= TypeAttributes.NestedFamily;
			else if (accessor == Accessor.Private)
				attributes |= TypeAttributes.NestedPrivate;

			return attributes;
		}
	}
}

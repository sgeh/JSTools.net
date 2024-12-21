namespace("JSTools.Reflection");


/// <class>
/// Type class, used for reflection and inherit calls. Each object has a type, which
/// contains the metadata of the members.
/// </class>
/// <param name="objToRepresent" type="Object">Any type of object, which should be reflected.</param>
/// <param name="objFunction" type="Function">Function of the object, which should be reflected.</param>
/// <param name="strFunctionName" type="String">Function name.</param>
JSTools.Reflection.Type = function(objToRepresent, objFunction, strFunctionName)
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	if (!objToRepresent || typeof(objToRepresent) != 'object')
		return;

	if (!objFunction || typeof(objFunction) != 'function')
		return;

	var NAME_PATTERN	= "[object {0}]";

	var _this			= this;
	var _object			= objToRepresent;
	var _function		= objFunction;

	var _name			= (typeof(strFunctionName) == 'undefined') ? String.Empty : String(strFunctionName);
	var _toStringName	= String.Format(NAME_PATTERN, _name);

	var _guid			= new JSTools.Util.Guid();
	var _typeParser		= null;
	var _baseTypes		= [ ];


	/// <property type="Boolean">
	/// Identifies a type instance.
	/// </property>
	this.IsTypeInstance = true;


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------

	/// <constructor>
	/// Creates a new JSTools.Reflection.Type instance.
	/// </constructor>
	function Init()
	{
		// create new toString method, if that is required
		if (_object.toString == Object.prototype.toString && _name)
		{
			_object.toString = function()
			{
				return _toStringName;
			}
		}
	}


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Returns true if the given function is equal to the type or a base type of this object. To use
	/// this method you have to initialize the current type. Otherwise you will always obtain false.
	/// </method>
	/// <param name="objFunctionType" type="Function">Function to check.</param>
	/// <returns type="Boolean">Returns true, if the given function is equal to the type or a base type
	/// of this object.</returns>
	this.IsTypeOf = function(objFunctionType)
	{
		if (typeof(objFunctionType) != 'function')
			return false;

		var functionType = objFunctionType.toString();

		if (_function.toString() == functionType)
		{
			return true;
		}

		var baseTypes = _this.GetBaseTypes();

		for (var i = 0; i < baseTypes.length; ++i)
		{
			if (baseTypes[i].IsTypeOf(objFunctionType))
			{
				return true;
			}
		}
		return false;
	}


	/// <method>
	/// Adds the given type as base type to the current type instance.
	/// </method>
	/// <param name="objType" type="JSTools.Reflection.Type">Base type to add.</param>
	this.AddBaseType = function(objType)
	{
		if (objType && typeof(objType) == 'object' && objType.IsTypeInstance)
		{
			_baseTypes.Add(objType);
		}
	}


	/// <method>
	/// Returns the base types of this instance.
	/// </method>
	/// <returns type="Array">Returns the base type instances (JSTools.Reflection.Type)
	/// of this type instance.</returns>
	this.GetBaseTypes = function()
	{
		var baseTypes = [ ];
		_baseTypes.CopyTo(baseTypes);

		if (_function.prototype
			&& _function.prototype.GetType
			&& _function.prototype.GetType() != null)
		{
			baseTypes.Add(_function.prototype.GetType());
		}
		return baseTypes;
	}


	/// <method>
	/// Returns the name of this member.
	/// </method>
	/// <returns type="String">Returns the name of this member.</returns>
	this.GetName = function()
	{
		return _name;
	}


	/// <method>
	/// Returns the member type.
	/// </method>
	/// <returns type="JSTools.Reflection.MemberType">Returns the type of this member.</returns>
	this.GetMemberType = function()
	{
		return JSTools.Reflection.MemberType.Type;
	}


	/// <method>
	/// Returns true, if this member is internal (begins with __). This
	/// Members should not be used from your code.
	/// </method>
	/// <returns type="Boolean">Returns true, if this member is used for internal purposes.</returns>
	this.IsInternal = function()
	{
		return _this.GetName().StartsWith("__");
	}


	/// <method>
	/// Gets the constructor function object.
	/// </method>
	/// <returns type="Function">Returns the constructor function object.</returns>
	this.GetConstructor = function()
	{
		return _function;
	}


	/// <method>
	/// Checks if the representing object is an array.
	/// </method>
	/// <returns type="Boolean">Returns true, if the representing object is an array.</returns>
	this.IsArray = function()
	{
		return (_function == Array);
	}


	/// <method>
	/// Checks if the representing object is a function.
	/// </method>
	/// <returns type="Boolean">Returns true, if the representing object is a function.</returns>
	this.IsFunction = function()
	{
		return (typeof(_object) == 'function');
	}

	/// <method>
	/// Checks if the representing object is an enumeration.
	/// </method>
	/// <returns type="Boolean">Returns true, if the representing object is an enumeration.</returns>
	this.IsEnum = function()
	{
		return (_function == JSTools.Enum.FlagsEnum || _function == JSTools.Enum.StringEnum);
	}


	/// <method>
	/// Gets the guid of the current object type.
	/// </method>
	/// <returns type="JSTools.Util.Guid">Returns the guid of the current object type.</returns>
	this.GetGuid = function()
	{
		return _guid;
	}


	/// <method>
	/// Gets the NameSpace of the current type.
	/// </method>
	/// <returns type="String">Returns the NameSpace of this type. If there is no NameSpace,
	/// you will obtain an empty string.</returns>
	this.GetNameSpace = function()
	{
		if (_name.indexOf(".") != -1)
		{
			return _name.substring(0, _name.indexOf("."));
		}
		return String.Empty;
	}


	/// <method>
	/// Searches for a JSTools.Reflection.IMemberInfo object which has the given name.
	/// </method>
	/// <param name="strMemberName">Name of the member to search.</param>
	/// <returns type="JSTools.Reflection.IMemberInfo">Returns the first MethodInfo object with
	/// the given member name.</returns>
	this.GetMember = function(strMemberName)
	{
		return GetMemberFromArray(this.GetMembers(), strMemberName);
	}


	/// <method>
	/// Gets all members which are contained in this type.
	/// </method>
	/// <returns type="Array">Returns all members which are contained in this type.</returns>
	this.GetMembers = function()
	{
		var fields = this.GetFields();
		var methods = this.GetMethods();

		var members = new Array(fields.length + methods.length);
		fields.CopyTo(members);
		methods.CopyTo(members);

		return members;
	}


	/// <method>
	/// Searches for a JSTools.Reflection.FieldInfo object which has the given name.
	/// </method>
	/// <param name="strFieldName">Name of the member to search.</param>
	/// <returns type="JSTools.Reflection.FieldInfo">Returns the first FieldInfo object with
	/// the given member name.</returns>
	this.GetField = function(strFieldName)
	{
		return GetMemberFromArray(this.GetFields(), strFieldName);
	}


	/// <method>
	/// Gets all FieldInfo members of this type.
	/// </method>
	/// <returns type="Array">Returns all JSTools.Reflection.FieldInfo members of this type.</returns>
	this.GetFields = function()
	{
		var fields = [ ];

		// init private methods
		var privateFields = GetTypeParser().GetPrivateFields();

		for (var i = 0; i < privateFields.length; ++i)
		{
			fields.Add(new JSTools.Reflection.FieldInfo(_object, privateFields[i], JSTools.Reflection.MemberVisibility.Private));
		}

		// init protected methods
		InitFieldsFromObject(_function.GetMemberProtector().GetProtectedItems(_object),
			fields,
			JSTools.Reflection.MemberVisibility.Protected );

		// init public methods
		InitFieldsFromObject(_object,
			fields,
			JSTools.Reflection.MemberVisibility.Public );

		return fields;
	}


	/// <method>
	/// Searches for a JSTools.Reflection.MethodInfo object which has the given name.
	/// </method>
	/// <param name="strMethodName">Name of the member to search.</param>
	/// <returns type="JSTools.Reflection.MethodInfo">Returns the first FieldInfo object with
	/// the given member name.</returns>
	this.GetMethod = function(strMethodName)
	{
		return GetMemberFromArray(this.GetMethods(), strMethodName);
	}


	/// <method>
	/// Gets all MethodInfo members of this type.
	/// </method>
	/// <returns type="Array">Returns all JSTools.Reflection.MethodInfo members of this type.</returns>
	this.GetMethods = function()
	{
		var methods = [ ];

		// init private methods
		var privateMethods = GetTypeParser().GetPrivateMethods();

		for (var i = 0; i < privateMethods.length; ++i)
		{
			methods.Add(new JSTools.Reflection.MethodInfo(_object, privateMethods[i], JSTools.Reflection.MemberVisibility.Private));
		}

		// init protected methods
		InitMethodsFromObject(_function.GetMemberProtector().GetProtectedItems(_object),
			methods,
			JSTools.Reflection.MemberVisibility.Protected );

		// init public methods
		InitMethodsFromObject(_object,
			methods,
			JSTools.Reflection.MemberVisibility.Public );

		return methods;
	}


	/// <method>
	/// Initializes all methods of the given object to search.
	/// </method>
	/// <param name="objToSearch" type="Object">Object to search.</param>
	/// <param name="arrToStore" type="Array">The found methods will be stored into this array.</param>
	/// <param name="enuVisibility" type="JSTools.Reflection.MemberVisibility">Member visibility.</param>
	function InitMethodsFromObject(objToSearch, arrToStore, enuVisibility)
	{
		if (objToSearch != null)
		{
			for (var item in objToSearch)
			{
				if (typeof(objToSearch[item]) == 'function')
				{
					arrToStore.Add(new JSTools.Reflection.MethodInfo(_object, item, enuVisibility));
				}
			}
		}
	}


	/// <method>
	/// Initializes all fields of the given object to search.
	/// </method>
	/// <param name="objToSearch" type="Object">Object to search.</param>
	/// <param name="arrToStore" type="Array">The found fields will be stored into this array.</param>
	/// <param name="enuVisibility" type="JSTools.Reflection.MemberVisibility">Member visibility.</param>
	function InitFieldsFromObject(objToSearch, arrToStore, enuVisibility)
	{
		if (objToSearch != null)
		{
			for (var item in objToSearch)
			{
				if (typeof(objToSearch[item]) != 'function')
				{
					arrToStore.Add(new JSTools.Reflection.FieldInfo(_object, item, enuVisibility));
				}
			}
		}
	}


	/// <method>
	/// Gets the first member with the given name from the specified array.
	/// </method>
	/// <param name="arrToSearch" type="Array">Object to search.</param>
	/// <param name="strMemberName" type="String">Member to search.</param
	/// <returns type="JSTools.Reflection.IMethodInfo"></returns>
	function GetMemberFromArray(arrToSearch, strMemberName)
	{
		for (var i = 0; i < arrToSearch.length; ++i)
		{
			if (arrToSearch[i].GetName() == strMemberName)
			{
				return arrToSearch[i];
			}
		}
		return null;
	}


	/// <method>
	/// Gets the type parser of this type. It is required for parsing the private members.
	/// </method>
	/// <returns type="JSTools.Reflection.TypeParser">Returns the type parser of this type.</returns>
	function GetTypeParser()
	{
		if (_typeParser == null)
		{
			_typeParser = new JSTools.Reflection.TypeParser(_object);
		}
		return _typeParser;
	}
	Init();
}


// implement JSTools.Reflection.IMemberInfo interface
JSTools.Reflection.Type.prototype = new JSTools.Reflection.IMemberInfo();


/// <enum>
/// Represents the type of a member.
/// </enum>
/// <field name="Type">Specifies that the member is a class (Type class).</field>
/// <field name="Field">Specifies that the member is a field (FieldInfo class).</field>
/// <field name="Method">Specifies that the member is a method (MethodInfo class).</field>
JSTools.Reflection.MemberType = new JSTools.Enum.StringEnum(
	"Type",
	"Field",
	"Method" );


/// <enum>
/// Represents the visibility of a member.
/// </enum>
/// <field name="Public">Specifies that the member is accessible from the global code.</field>
/// <field name="Protected">Specifies that the member is accessible from the class internal and from derived code.</field>
/// <field name="Private">Specifies that the member is accessible from the class internal only.</field>
JSTools.Reflection.MemberVisibility = new JSTools.Enum.StringEnum(
	"Public",
	"Protected",
	"Private" );

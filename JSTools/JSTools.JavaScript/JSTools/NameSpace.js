/// <class>
/// Creates new NameSpaces and assignes them to the global object. Manages
/// the namespace directive. The functions will be cached in the global (top)
/// window object. Thus script files will not be loaded twice if they are specified
/// in the same window object.
/// </class>
/// <param name="objCurrent" type="Object">The current scope object (this).</param>
/// <param name="objTop" type="Object">The top scope object (this.top).</param>
function NameSpaceManager(objCurrent, objTop)
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	var NAME_SPACE_SEPARATOR = ".";

	var _this = this;
	var _scope = (typeof(objCurrent) != 'object' || !objCurrent) ? new Object() : objCurrent;
	var _global = (typeof(objTop) != 'object' || !objTop) ? new Object() : objTop;


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------

	/// <constructor>
	/// Creates a new NameSpaceManager instance.
	/// </constructor>
	function Init()
	{
	}


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method scope="public">
	/// Adds the given NameSpace to the global object.
	/// </method>
	/// <param name="strNameSpace" type="String">NameSpace to create, separated by '.'.</param>
	function Add(strNameSpace)
	{
		if (typeof(strNameSpace) != 'string')
			return;

		if (!strNameSpace)
			return;

		var nameSpaces = strNameSpace.split(NAME_SPACE_SEPARATOR);

		// initialize NameSpaces
		InitializeNameSpace(_global, nameSpaces, 0);

		// copy references into the scope, if there are new global namespaces
		InitScopeReference(nameSpaces);
	}
	this.Add = Add;


	/// <method scope="public">
	/// Gets the NameSpace with the given name.
	/// </method>
	/// <param name="strNameSpace" type="String">NameSpace to get, separated by '.'.</param>
	/// <returns type="NameSpaceObject">Returns the requested NameSpace or a null reference, if nothing was found.</returns>
	function GetNameSpace(strNameSpace)
	{
		if (typeof(strNameSpace) != 'string')
			return null;

		if (!strNameSpace)
			return null;

		var nameSpace = _global;
		var nameSpaces = strNameSpace.split(NAME_SPACE_SEPARATOR);

		// copy references into the scope, if there are new global namespaces
		InitScopeReference(nameSpaces);

		// search for the given NameSpace
		for (var i = 0; i < nameSpaces.length; ++i)
		{
			if (!nameSpace[nameSpaces[i]])
				return null;

			nameSpace = nameSpace[nameSpaces[i]];
		}
		return nameSpace;
	}
	this.GetNameSpace = GetNameSpace;


	/// <method scope="public">
	/// Checks, if the given NameSpace was loaded yet.
	/// </method>
	/// <param name="strNameSpace" type="String">NameSpace to search, separated by '.'.</param>
	/// <returns type="Boolean">Returns true, if the given NameSpace was loaded yet, otherwise false.</returns>
	function IsNameSpaceLoaded(strNameSpace)
	{
		return (_this.GetNameSpace(strNameSpace) != null);
	}
	this.IsNameSpaceLoaded = IsNameSpaceLoaded;


	/// <method>
	/// Initilizes the scope references of the given NameSpace.
	/// </method>
	/// <param name="arrNameSpacesNames" type="Array">NameSpace to reference.</param>
	function InitScopeReference(arrNameSpacesNames)
	{
		// if global and scope objects are the same
		if (_global == _scope)
			return;

		var firstNameSpace = arrNameSpacesNames[0];

		// if it is a new global NameSpace
		if (!_global[firstNameSpace])
			return;

		// create a reference from the global namespaces to the local scope namespaces
		if (!_scope[firstNameSpace])
		{
			_scope[firstNameSpace] = _global[firstNameSpace];
		}
	}


	/// <method>
	/// Creates the objects for the NameSpaces.
	/// </method>
	/// <param name="objParent" type="Object">Parent object.</param>
	/// <param name="arrNames" type="Array">NameSpace names.</param>
	/// <param name="intArrPointer" type="Number">Pointer to the current NameSpace name in the arrNames array.</param>
	function InitializeNameSpace(objParent, arrNames, intArrPointer)
	{
		var nameSpaceName = arrNames[intArrPointer];
		var nameSpaceObject = objParent[nameSpaceName];

		// init NameSpace object, if it was not initialized yet
		if (!nameSpaceObject)
		{
			objParent[nameSpaceName] = new NameSpaceObject(objParent, nameSpaceName);
		}

		if (++intArrPointer < arrNames.length)
		{
			// create NameSpace objects in a recursive loop
			InitializeNameSpace(objParent[nameSpaceName], arrNames, intArrPointer);
		}
	}


	/// <class>
	/// Manages the namespace directive.
	/// </class>
	/// <param name="objParent" type="Object">The global window object (this).</param>
	/// <param name="strName" type="String">Name of the representing NameSpace object.</param>
	function NameSpaceObject(objParent, strName)
	{
		//------------------------------------------------------------------------
		// Declarations
		//------------------------------------------------------------------------

		var _this = this;

		/// <property type="NameSpaceObject">
		/// Parent NameSpace. Contains null, if this is the top NameSpace object.
		/// </property>
		this.Parent = null;


		/// <property type="String">
		/// Name of this NameSpace, e.g. Web
		/// </property>
		this.Name = String(strName);


		/// <property type="String">
		/// Full qualified name of this NameSpace, e.g. JSTools.Web
		/// </property>
		this.FullName = null;


		//------------------------------------------------------------------------
		// Constructor
		//------------------------------------------------------------------------

		/// <constructor>
		/// Creates a new NameSpaceObject instance.
		/// </constructor>
		function Init()
		{
			if (objParent.FullName && objParent.Name)
			{
				_this.Parent = objParent;
				_this.FullName = _this.Parent.FullName + NAME_SPACE_SEPARATOR + _this.Name;
			}
			else
			{
				_this.Parent = null;
				_this.FullName = _this.Name;
			}
		}


		//------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------

		/// <method scope="public">
		/// Gets a string representation of this object.
		/// </method>
		function ToString()
		{
			return "[object NameSpace { " + _this.FullName + " } ]";
		}
		this.toString = ToString;

		Init();
	}
	Init();
}

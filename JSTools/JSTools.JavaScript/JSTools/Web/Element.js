namespace("JSTools.Web");


/// <class>
/// Represents an html element tag. Can be used as base class or as html
/// rendering help.
/// </class>
/// <param name="strTagName" type="String">Name of the element to initialize.</param>
JSTools.Web.Element = function(strTagName)
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	this.InitType(arguments, "JSTools.Web.Element");

	var _this			= this;
	var _attributes		= new JSTools.Util.Hashtable();
	var _tagName		= (strTagName) ? String(strTagName) : String.Empty;
	var _children		= [ ];
	var _parent			= null;


	/// <property type="Boolean">
	/// True to enable html rendering, otherwise false.
	/// </property>
	this.Enabled		= true;


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------

	/// <constructor>
	/// Creates a new JSTools.Web.Element instance.
	/// </constructor>
	function Init()
	{
		// override add method of _children array
		_children.Add = function(objToAdd)
		{
			if (objToAdd
				&& typeof(objToAdd) == 'object'
				&& objToAdd.IsTypeOf(JSTools.Web.Element))
			{
				objToAdd.SetParent(_this);
			}
			this[this.length] = objToAdd;
		}
	}


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Get the parent element of this instance.
	/// </method>
	/// <returns type="JSTools.Web.Element">Returns the parent instance
	/// or a null reference.</returns>
	this.GetParent = function()
	{
		return _attributes;
	}


	/// <method>
	/// Sets the given element as parent of this element.
	/// </method>
	/// <param name="objParent" type="JSTools.Web.Element">Returns the parent instance
	/// or a null reference.</returns>
	this.SetParent = function(objParent)
	{
		if (objParent
			&& typeof(objParent) == 'object'
			&& objParent.IsTypeOf(JSTools.Web.Element))
		{
			_parent = objParent;
		}
	}


	/// <method>
	/// Gets the stored attributes.
	/// </method>
	/// <returns type="JSTools.Util.Hashtable">Returns the attributes for this element.</returns>
	this.GetAttributes = function()
	{
		return _attributes;
	}


	/// <method>
	/// Gets the child elements. Only valid JSTools.Web.Element types
	/// will be rendered.
	/// </method>
	/// <returns type="Array">Returns the child elements.</returns>
	this.GetControls = function()
	{
		return _children;
	}


	/// <method>
	/// Renders the data stored in this element.
	/// </method>
	/// <returns type="String">Returns the html string representation of this object.</returns>
	this.Render = function()
	{
		if (!_this.Enabled)
			return String.Empty;

		var renderedString = "<" + _tagName + " ";

		for (var attributeName in _attributes.GetElements())
		{
			renderedString += String(attributeName) + "='" + String(_attributes.Get(attributeName)).replace(/'/g, "\\'") + "'";
		}

		if (_children.length > 0)
		{
			renderedString += ">";
			
			for (var i = 0; i < _children.length; ++i)
			{
				if (_children[i]
					&& typeof(_children[i]) == 'object'
					&& _children[i].IsTypeOf(JSTools.Web.Element))
				{
					renderedString += _children[i].Render();
				}
			}
			renderedString += "</" + _tagName + ">";
		}
		else
		{
			renderedString += "/>";
		}
		return renderedString;
	}
}
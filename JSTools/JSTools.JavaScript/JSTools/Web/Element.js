/*
 * JSTools.JavaScript / JSTools.net - A JavaScript/C# framework.
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

namespace("JSTools.Web");


/// <class>
/// Represents an html element tag. Can be used as base class or as a help for
/// rendering html tags.
/// </class>
/// <param name="strTagName" type="String">Name of the element to initialize.</param>
JSTools.Web.Element = function(strTagName)
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	this.InitType(arguments, "JSTools.Web.Element");

	var _this = this;
	var _attributes = new JSTools.Util.Hashtable();
	var _tagName = (strTagName) ? String(strTagName) : String.Empty;
	var _children = [ ];
	var _parent = null;


	/// <property type="Boolean">
	/// True to enable html rendering, otherwise false.
	/// </property>
	this.Enabled = true;


	/// <property type="Boolean">
	/// The full end tag will be rendered, if the element has no children.
	/// </property>
	this.RenderEndTag = false;


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
	function GetParent()
	{
		return _parent;
	}
	this.GetParent = GetParent;


	/// <method>
	/// Sets the given element as parent of this element.
	/// </method>
	/// <param name="objParent" type="JSTools.Web.Element">Returns the parent instance
	/// or a null reference.</returns>
	function SetParent(objParent)
	{
		if (objParent
			&& typeof(objParent) == 'object'
			&& objParent.IsTypeOf(JSTools.Web.Element))
		{
			_parent = objParent;
		}
	}
	this.SetParent = SetParent;


	/// <method>
	/// Gets the stored attributes.
	/// </method>
	/// <returns type="JSTools.Util.Hashtable">Returns the attributes for this element.</returns>
	function GetAttributes()
	{
		return _attributes;
	}
	this.GetAttributes = GetAttributes;


	/// <method>
	/// Gets the child elements. Only valid JSTools.Web.Element types
	/// will be rendered.
	/// </method>
	/// <returns type="Array">Returns the child elements.</returns>
	function GetControls()
	{
		return _children;
	}
	this.GetControls = GetControls;


	/// <method>
	/// Renders the data stored in this element.
	/// </method>
	/// <returns type="String">Returns the html string representation of this object.</returns>
	function Render()
	{
		if (!_this.Enabled)
			return String.Empty;

		var renderedString = "<" + _tagName;

		for (var attributeName in _attributes.GetElements())
		{
			renderedString += " "
				+ String(attributeName)
				+ "='"
				+ String(_attributes.Get(attributeName)).replace(/'/g, "\\'")
				+ "'";
		}

		if (_this.RenderEndTag)
		{
			renderedString += ">"
				+ RenderChildren()
				+ "</" + _tagName + ">";
		}
		else
		{
			renderedString += " />";
		}
		return renderedString;
	}
	this.Render = Render;


	/// <method>
	/// Renders the child elements.
	/// </method>
	/// <returns type="String">Returns the rendered child element string.</returns>
	function RenderChildren()
	{
		var renderedString = String.Empty;
		
		for (var i = 0; i < _children.length; ++i)
		{
			if (_children[i]
				&& typeof(_children[i]) == 'object'
				&& _children[i].IsTypeOf(JSTools.Web.Element))
			{
				renderedString += _children[i].Render();
			}
		}
		return renderedString;
	}
}
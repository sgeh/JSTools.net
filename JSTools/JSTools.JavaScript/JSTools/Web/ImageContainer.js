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
/// Represents a container for images. It will load the images with the given
/// sources and store them into the internal _images array. The stored images
/// are accessible through the GetSource() and GetImage() methods.
/// </class>
JSTools.Web.ImageContainer = function()
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	this.InitType(arguments, "JSTools.Web.ImageContainer");

	var IMAGE_TAG_NAME = "img";
	var BORDER_ATTRIB = "border";
	var SOURCE_ATTRIB = "src";
	var HEIGHT_ATTRIB = "height";
	var WIDTH_ATTRIB = "width";
	var ID_ATTRIB = "id";
	var NAME_ATTRIB = "name";

	var _this = this;
	var _images = new Array();


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Adds a new Image instance with the given param to this container.
	/// </method>
	/// <param name="strSource" type="String">Image source, equal to the src attribut of an image tag.</param>
	/// <param name="strName" type="String">Image name, equal to the name/id attribut of an image tag.
	/// This string should contain only alphanumeric characters (a-z A-Z).</param>
	/// <param name="intHeight" type="Integer">Height of the image to create. If you specify a height you
	/// have to specify the width too. This argument is optional.</param>
	/// <param name="intWidth" type="Integer">Width of the image to create. If you specify a width you
	/// have to specify the height too. This argument is optional.</param>
	function AddImage(strSource, strName, intHeight, intWidth)
	{
		var imageIndex = _images.length;

		if (isNaN(intHeight) || isNaN(intWidth))
		{
			_images.Add(new Image());
		}
		else
		{
			var width = Number(intWidth);
			var height = Number(intHeight);

			_images(new Image(width, height));
			_images[imageIndex].width = width;
			_images[imageIndex].height = height;
		}

		_images[imageIndex].src = String(strSource);
		_images[imageIndex].id = String(strName);
		_images[imageIndex].name = String(strName);
	}
	this.AddImage = AddImage;


	/// <method>
	/// Gets the source of an image.
	/// </method>
	/// <returns type="String">Returns the source of the image with the given
	/// name or a null reference if this container does not accomodate an image
	/// with the given name.</returns>
	function GetSource(strName)
	{
		if (_this.Contains(strName))
		{
			return _images[GetIndexOf(strName)].src;
		}
		return null;
	}
	this.GetSource = GetSource;


	/// <method>
	/// Checks whether this instance contains an image with the given name.
	/// </method>
	/// <param name="strName"Boolean type="String">Name of the image.</param>
	/// <returns type="String">Returns true if an image with the given name exists.</returns>
	function Contains(strImgName)
	{
		return (GetIndexOf(strImgName) != -1);
	}
	this.Contains = Contains;


	/// <method>
	/// Gets the image object reference.
	/// </method>
	/// <returns type="Image">Returns the Image instance with the given
	/// name or a null reference if this container does not accomodate an image
	/// with the given name.</returns>
	function GetImage(strName)
	{
		if (_this.Contains(strName))
		{
			return _images[GetIndexOf(strName)];
		}
		return null;
	}
	this.GetImage = GetImage;


	/// <method>
	/// Gets a random image, which was added to this container.
	/// </method>
	/// <returns type="Image">Returns a random image, which was added to this container.</returns>
	function GetRandomImage()
	{
		return _images[Math.floor(Math.random() * _images.length)];
	}
	this.GetRandomImage = GetRandomImage;


	/// <method>
	/// Gets the HTML Tag for the image.
	/// </method>
	/// <param name="strName" type="String">Name of the image.</param>
	/// <param name="intBorder" type="Integer">Value of the border attribute. Default is 0.</param>
	/// <param name="strNewId" type="String">New image id if needed.</param>
	/// <returns type="String">Returns the html tag of the image instance with the given
	/// name or a null reference if this container does not accomodate an image
	/// with the given name.</returns>
	function GetHtmlTag(strName, intBorder, strNewId)
	{
		if (_this.Contains(strName))
		{
			var image = _images[GetIndexOf(strName)];
			var imageId = (typeof(strNewId) != 'undefined') ? String(strNewId) : image.name;

			var imgElement = new JSTools.Web.Element(IMAGE_TAG_NAME);
			imgElement.GetAttributes().Add(BORDER_ATTRIB, (!isNaN(intBorder) ? Number(intBorder) : 0));
			imgElement.GetAttributes().Add(SOURCE_ATTRIB, image.src);
			imgElement.GetAttributes().Add(HEIGHT_ATTRIB, image.height);
			imgElement.GetAttributes().Add(WIDTH_ATTRIB, image.width);
			imgElement.GetAttributes().Add(ID_ATTRIB, imageId);
			imgElement.GetAttributes().Add(NAME_ATTRIB, imageId);
			return imgElement.Render();
		}
		return null;
	}
	this.GetHtmlTag = GetHtmlTag;


	/// <method>
	/// Gets the index of the image with the given name.
	/// </method>
	/// <param name="strName" type="String">Name of the image.</param>
	/// <returns type="Integer">Returns the index of the given image or
	/// -1 if an image with the given name could not be found.</returns>
	function GetIndexOf(strName)
	{
		for (var i = _images.length - 1; i > -1 && _images[i].name != String(strName); --i)
		{
			;
		}
		return i;
	}
}
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

	var _this	= this;
	var _images	= new Array();


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Adds a new Image instance with the given param to this container.
	/// </method>
	/// <param name="strSource" type="String"></param>
	/// <param name="strName" type="String"></param>
	/// <param name="intHeight" type="Integer"></param>
	/// <param name="intWidth" type="Integer">/param>
	this.AddImage = function(strSource, strName, intHeight, intWidth)
	{
		var imageIndex = _images.length;

		if (isNaN(intHeight) || isNaN(intWidth))
		{
			_images.Add(new Image());
		}
		else
		{
			_images(new Image(intWidth, intHeight));
			_images[imageIndex].height = intHeight;
			_images[imageIndex].width = intWidth;
		}

		_images[imageIndex].src = strSource;
		_images[imageIndex].id = strName;
		_images[imageIndex].name = strName;
	}


	/// <method>
	/// Gets the source of an image.
	/// </method>
	/// <returns type="String">Returns the source of the image with the given
	/// name or a null reference if this container does not accomodate an image
	/// with the given name.</returns>
	this.GetSource = function(strName)
	{
		if (_this.Contains(strName))
		{
			return _images[GetIndexOf(strName)].src;
		}
		return null;
	}


	/// <method>
	/// Checks whether this instance contains an image with the given name.
	/// </method>
	/// <param name="strName" type="String">Name of the image.</param>
	/// <returns type="String">Returns true if an image with the given name exists.</returns>
	this.Contains = function(strImgName)
	{
		return (GetIndexOf(strImgName) != -1);
	}


	/// <method>
	/// Gets the image object reference.
	/// </method>
	/// <returns type="String">Returns the Image instance with the given
	/// name or a null reference if this container does not accomodate an image
	/// with the given name.</returns>
	this.GetImage = function(strName)
	{
		if (_this.Contains(strName))
		{
			return _images[GetIndexOf(strName)];
		}
		return null;
	}


	/// <method>
	/// Gets the HTML Tag for the image.
	/// </method>
	/// <param name="strName" type="String">Name of the image.</param>
	/// <param name="intBorder" type="Integer">Value of the border attribute. Default is 0.</param>
	/// <param name="strNewId" type="String">New image id if needed.</param>
	/// <returns type="String">Returns the html tag of the image instance with the given
	/// name or a null reference if this container does not accomodate an image
	/// with the given name.</returns>
	this.GetHtmlTag = function(strName, intBorder, strNewId)
	{
		if (_this.Contains(strName))
		{
			var image = _images[GetIndexOf(strName)];
			var imageId = (typeof(strNewId) != 'undefined') ? String(strNewId) : image.name;

			return '<img border="'
				+ (!isNaN(intBorder) ? Number(intBorder) : 0)
				+ '" src="'
				+ image.src
				+ '" height="'
				+ image.height
				+ '" width="'
				+ image.width
				+ '" name="'
				+ imageId
				+ '" id="'
				+ imageId
				+ '" />';
		}
		return null;
	}


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
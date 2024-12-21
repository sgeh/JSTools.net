function ImageContainer()
{
	// adds a picture to the class
	this.AddImage = function(strSource,strName,intHeight,intWidth) {
		if(IsUndefined(intHeight) || IsUndefined(intWidth))
		{
			_imagesByName[strName] = new Image();
		}
		else
		{
			_imagesByName[strName] = new Image(intWidth,intHeight);
			_imagesByName[strName].height = intHeight;
			_imagesByName[strName].width = intWidth;
		}

		_imagesByName[strName].src = strSource;
		_imagesByName[strName].id = strName;

		if(!HasImage(strName))
		{
			_imagesByName[_imagesByName.length] = strName;
		}
	}


	// gets the source of an image
	this.GetSource = function(strName)
	{
		if(HasImage(strName))
		{
			return _imagesByName[strName].src;
		}
		return false;
	}


	// gets the image object reference
	this.GetObject = function(strName)
	{
		if(HasImage(strName))
		{
			return _imagesByName[strName];
		}
		return false;
	}


	// gets the an example HTML Tag for the image
	this.GetHtmlTag = function(strName,strOtherId)
	{
		if(HasImage(strName))
		{
			var newImageId = (!IsUndefined(strOtherId)) ? strOtherId : strName;
			return '<img border="0" src="' + _imagesByName[strName].src + '" height="' + _imagesByName[strName].height + '" width="'+_imagesByName[strName].width + '" name="' + newImageId+'" id="' + newImageId + '">';
		}
		return false;
	}


	// checks this object for an image
	this.HasImage = function(strImgName)
	{
		for(var e = 0; e < _imagesByName.length; ++e)
		{
			if(_imagesByName[e] == strImgName)
			{
				return true;
			}
		}
		return false;
	}



	//private statements

	var _imagesByName	= new Array();
}
ImageContainer.prototype.toString = function()
{
    return "[object ImageContainer]";
}
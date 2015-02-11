function DocumentChecker()
{
	// initialize document object arrays
	this.CheckDocObjects = function()
	{
		InitializeImages();
		InitializeForms();
		InitializeLinks();
	}


	// returns all checked images
	this.GetCheckedImages = function()
	{
		return _checkedImages;
	}


	// returns all checked forms
	this.GetCheckedForms = function()
	{
		return _checkedForms;
	}


	// returns all checked links
	this.GetCheckedLinks = function()
	{
		return _checkedForms;
	}



	//private statements

	// checks if the refresh is required
	function IsRefreshRequired(objDocElementsLength, objTargetLength)
	{
		if(!IsNull(objTargetLength)) {
			return objDocElementsLength.length != objTargetLength.length;
		}
		return true;
	}


	// actualizes the image cache object
	function RefreshDocumentObject(objDocument)
	{
		var elementContainer = new Array();

		for(var i = 0; i < objDocument.length; ++i)
		{
			if(!IsVoid(objDocument[i].id) && !IsVoid(objDocument[i].name))
			{
				if(IsVoid(objDocument[i].id))
				{
					objDocument[i].id = objDocument[i].name;
				}
				elementContainer[elementContainer.length] = elementContainer[objDocument[i].id] = objDocument[i];
			}
		}

		elementContainer.checkedLength = objDocument.length;
		return elementContainer;
	}


	// initializes image collection
	function InitializeImages()
	{
		if(IsRefreshRequired(document.images, _checkedImages))
		{
			_checkedImages			= RefreshDocumentObject(document.images);
			_checkedImages.startTag	= "img";
		}
	}


	// initializes form collection
	function InitializeForms()
	{
		if(IsRefreshRequired(document.forms, _checkedForms))
		{
			_checkedForms			= RefreshDocumentObject(document.forms);
			_checkedForms.startTag	= "form";
		}
	}


	// initializes link collection
	function InitializeLinks()
	{
		if(IsRefreshRequired(document.links, _checkedLinks))
		{
			_checkedLinks			= RefreshDocumentObject(document.links);
			_checkedLinks.startTag	= "a";
		}
	}



	var _checkedImages		= new Array();
	var _checkedLinks		= new Array();
	var _checkedForms		= new Array();
}
DocumentChecker.prototype.toString = function()
{
	return "[object DocumentChecker]";
}

window.DocChecker = new DocumentChecker();
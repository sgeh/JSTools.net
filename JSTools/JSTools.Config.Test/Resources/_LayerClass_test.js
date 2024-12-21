// create a new container manager instance
var _layerManager		= new LayerContainerManager();

var TD_LEFT_POSTFIX		= "ITDLeft";
var TD_RIGHT_POSTFIX	= "ITDRight";
var TD_FONT_POSTFIX		= "ITDFont";
var IMG_CHANGE			= "IImgChange";

var LAY_TOP_NAV			= 0x01;
var LAY_CHILD_NAV		= 0x02;
var LAY_INNER_CHILD_NAV	= 0x04;


// create layer content object
var LAYER_CONTENT					= new Object();
LAYER_CONTENT[LAY_TOP_NAV]			= { ForeColor: "#CCCCCC", LineColor: "#E5E5E5" };
LAYER_CONTENT[LAY_CHILD_NAV]		= { ForeColor: "#CCCCCC", LineColor: "#E5E5E5" };
LAYER_CONTENT[LAY_INNER_CHILD_NAV]	= { ForeColor: "#E5E5E5", LineColor: "#CCCCCC" };

/**
 * Returns the layer html code, which can be used to create layers.
 */
LAYER_CONTENT.getChildLayer = function(strLayerId, strLink, strLinkText, strForeColor, strLineColor)
{
	var htmlContent = '<table border="0" cellspacing="0" cellpadding="0" width="200" onmouseover="layer_OnMouseOver(\'' + strLayerId + '\', \'#339900\', \'#FFFFFF\', null);" onmouseout="layer_OnMouseOut(\'' + strLayerId + '\', \'' + strForeColor + '\', \'#000000\', null, this);">' +
	'<tr>' +
	'<td width="1" bgcolor="#FFFFFF"><img src="/0.gif" width="1" height="1"></td>' +
	'<td width="169" bgcolor="' + strLineColor + '"><img src="/0.gif" width="169" height="1"></td>' +
	'<td width="30" bgcolor="' + strLineColor + '"><img src="/0.gif" width="30" height="1"></td>' +
	'</tr>' +
	'<tr>' +
	'<td bgcolor="#FFFFFF"><img src="/0.gif" width="1" height="1"></td>' +
	'<td bgcolor="' + strForeColor + '" id="' + strLayerId + 'ITDLeft">' +
		'<img src="/0.gif" width="8" height="1">' +
		'<a href="javascript:changeUrl(\'' + strLink + '\');" class="secnv">' + strLinkText + '</a>' +
	'</td>' +
	'<td bgcolor="' + strForeColor + '" id="' + strLayerId + 'ITDRight">&nbsp;</td>' +
	'</tr>' +
	'</table>';

	return htmlContent;
};


/**
 * Returns the layer html code, which can be used to create the end layer.
 */
LAYER_CONTENT.getChildEndLayer = function(strLayerId, strLink, strLinkText, strForeColor, strLineColor)
{
	var htmlContent = '<table border="0" cellspacing="0" cellpadding="0">' +
	'<tr>' +
	'<td bgcolor="' + strLineColor + '"><img src="/0.gif" width="1" height="1"></td>' +
	'<td bgcolor="' + strForeColor + '"><img src="/0.gif" width="169" height="1"></td>' +
	'<td bgcolor="' + strForeColor + '"><img src="/0.gif" width="30" height="1"></td>' +
	'</tr>' +
	'</table>';

	return htmlContent;
};



/**
 * Capture mouse over event.
 */
function layer_OnMouseOver(strParentId, strBGColor, strFontColor, strNewGif)
{
	if (!document.getElementById)
		return;

	changeBGColor(strParentId, strBGColor);
	changeFontColor(strParentId, strFontColor);

	if (strNewGif)
	{
		changeImage(strParentId, strNewGif);
	}
}


/**
 * Capture mouse out event.
 */
function layer_OnMouseOut(strParentId, strBGColor, strFontColor, strNewGif, objTableElement)
{
	if (!document.getElementById)
		return;

	if (objTableElement)
	{
		if (thisBrowser.typ.isIE && hasSameParent(objTableElement, window.event.toElement))
		{
			cancelBubbling();
			return;
		}
	}
	layer_OnMouseOver(strParentId, strBGColor, strFontColor, strNewGif);
}


/**
 * Returns true, if the given parent element is the same element as
 * a parent of the objElement element.
 */
/*Boolean*/ function hasSameParent(objToCompare, objElement)
{
	if (!objToCompare || !objElement)
		return false;

	if (objToCompare == objElement)
	{
		return true;
	}
	else
	{
		return hasSameParent(objToCompare, objElement.parentElement);
	}
}


/**
 * Cancles the event bubbling.
 */
function cancelBubbling()
{
	if (window.event)
	{
		window.event.cancelBubble = true;

		if (window.event.stopPropagation)
		{
			window.event.stopPropagation();
		}
	}
}

/**
 * Changes the url and disables event bubbling.
 */
function changeUrl(strToChange)
{
	cancelBubbling();

	// HOTFIX: we have to change the handle, if we are in live environment.
	if (_showMode == "1")
	{
		strToChange = replaceCookieStatement(strToChange);
	}

	// HOTFIX: we have to change the host name, if we are in live environment.
	for (var dns in _configDNS)
	{
		if (self.location.host == dns)
		{
			// HOTFIX: We have replaced the contianer handle twice. Avoid that!
			// strToChange = self.location.protocol + "//" + strToChange.replace(_configDNS[dns], dns);
			strToChange = self.location.protocol + "//" + dns + strToChange;
			break;
		}
	}

	var uniqueId = strToChange.substring(0, strToChange.lastIndexOf("/"));
	_layerManager.hideContainer(uniqueId);
	self.location.href = strToChange + ".html";
}


/**
 *
 */
/*String*/ function replaceCookieStatement(strToChange)
{
	var cookie = getCookie("contPath");

	if (cookie)
	{
		cookie += "/home";
		var startIndex = strToChange.indexOf(cookie);

		if (startIndex != -1)
		{
			return strToChange.substring(0, startIndex) + strToChange.substring(startIndex + cookie.length);
		}
	}
	return strToChange;
}


/**
 *
 */
/*String*/ function getCookie(strName)
{
	if (document.cookie)
	{
		var cookies = String(document.cookie).split("; ");

		for (var i = 0; i < cookies.length; ++i)
		{
			var cookie = cookies[i].split("=");

			if (cookie[0] && cookie[0] == strName)
			{
				return cookie[1];
			}
		}
	}
	return null;
}


/**
 * Changes the font color of the given element object.
 */
function changeFontColor(strUniqueLayerID, strFontColor)
{
	if (!document.getElementById)
		return;

	var objElement = document.getElementById(strUniqueLayerID + TD_LEFT_POSTFIX);

	if (objElement && objElement.lastChild)
	{
		objElement.lastChild.style.color = strFontColor;
	}
}


/**
 * Changes the background-color of the specified layer cols.
 */
function changeBGColor(strUniqueLayerID, strNewBGColor, strFontColor)
{
	if (!document.getElementById)
		return;

	var tdLeft = document.getElementById(strUniqueLayerID + TD_LEFT_POSTFIX);
	var tdRight = document.getElementById(strUniqueLayerID + TD_RIGHT_POSTFIX);

	// check objects for validity
	if (tdLeft != null && tdRight != null)
	{
		tdLeft.style.backgroundColor = strNewBGColor;
		tdRight.style.backgroundColor = strNewBGColor;
	}
}


/**
 * Changes the source of the image with the specified id to the given image url.
 */
function changeImage(strUniqueLayerID, strNewImageURL)
{
	// check for ie or dom
	if (document.all || document.getElementById)
	{
		// check for valid image id
		if (document.images[strUniqueLayerID + IMG_CHANGE])
		{
			document.images[strUniqueLayerID + IMG_CHANGE].src = strNewImageURL;
		}
	}
}


/**
 * Contains all layer handling capatibilities for the navigation.
 */
function LayerContainerManager()
{
	var _containers			= [ ];
	var _this				= this;
	var _visibleContainer	= null;

	var _layerHideTimeOut	= 50; //ms
	var _layerHideTimerRef	= null;

	this.OnMouseOver		= null;
	this.OnMouseOut			= null;


	/**
	 * Returns the container with the specified name.
	 */
	/*LayerContainer*/ this.getContainer = function(strContainerName)
	{
		var containerIndex = this.getContainerIndex(strContainerName);

		if (containerIndex != null)
		{
			return _containers[containerIndex];
		}
		return null;
	}


	/**
	 * Returns the container with the specified name.
	 */
	/*Number*/ this.getContainerIndex = function(strContainerName)
	{
		for (var i = 0; i < _containers.length; ++i)
		{
			if (_containers[i].getName() == strContainerName)
			{
				return i;
			}
		}
		return null;
	}


	/**
	 * Adds the specified container to this object.
	 */
	/*void*/ this.add = function(objLayerContainer)
	{
		if (objLayerContainer)
		{
			_containers[_containers.length] = objLayerContainer;
		}
	}


	/**
	 * Hides all containers, and visualizes the container with the given name.
	 */
	/*void*/ this.showContainer = function(strName)
	{
		// container is already active, so do nothing
		if (this.getContainerIndex(strName) == _visibleContainer)
			return;

		// make sure that all timers are disabled.
		clearTimer();

		// opens specified container and closes the others
		for (var i = 0; i < _containers.length; ++i)
		{
			if (strName == _containers[i].getName())
			{
				// mark visible container
				_visibleContainer = i;

				_containers[i].showLayers();
				EnableMouseListening();
			}
			else
			{
				_containers[i].hideLayers();
			}
		}
	}


	/**
	 * Hides the specified container.
	 */
	/*void*/ this.hideContainer = function(strName)
	{
		var container = _this.getContainer(strName);

		if (container != null)
		{
			container.hideLayers();
			_visibleContainer = null;
		}
	}


	/**
	 * Hides all containers, which are registered.
	 */
	/*void*/ this.hideAll = function()
	{
		if (_visibleContainer != null)
		{
			DisableMouseListening();
			_visibleContainer = null;
		}
		for (var i = 0; i < _containers.length; ++i)
		{
			_containers[i].hideLayers();
		}
	}


	/**
	 *
	 */
	/*void*/ this.checkMousePosition = function(objEvent)
	{
		if (_visibleContainer == null)
			return;

		if (!_containers[_visibleContainer] || !_containers[_visibleContainer].isInitialized())
			return;

		var mouseLeft	= (thisBrowser.typ.isNC) ? objEvent.pageX : event.clientX;
		var mouseTop	= (thisBrowser.typ.isNC) ? objEvent.pageY : event.clientY;

		if (thisBrowser.typ.isIE)
		{
			mouseLeft += window.document.body.scrollLeft;
			mouseTop += window.document.body.scrollTop;
		}

		var isMouseOver = (isMouseOverContainer(mouseLeft, mouseTop) || isMouseOverParent(mouseLeft, mouseTop));

		// fire events
		if (!isMouseOver)
		{
			clearTimer();
			_layerHideTimerRef = window.setTimeout("_layerManager.hideContainer('" + _containers[_visibleContainer].getName() + "');", _layerHideTimeOut);
		}
	}


	/**
	 * Clears the timer specified in _layerHideTimerRef.
	 */
	function clearTimer()
	{
		if (_layerHideTimerRef != null)
		{
			window.clearTimeout(_layerHideTimerRef);
			_layerHideTimerRef = null;
		}
	}


	/**
	 * Returns true, if the mouse is over the visible container.
	 */
	function isMouseOverContainer(intMouseLeft, intMouseTop)
	{
		var layerTop	= _containers[_visibleContainer].getTop();
		var layerLeft	= _containers[_visibleContainer].getLeft();
		var layerBottom	= _containers[_visibleContainer].getHeight() + layerTop;
		var layerRight	= _containers[_visibleContainer].getWidth() + layerLeft;

		if (intMouseTop >= layerTop && intMouseLeft >= layerLeft)
		{
			return (intMouseTop <= layerBottom && intMouseLeft <= layerRight);
		}
		return false;
	}


	/**
	 * Returns true, if the mouse is over the parent of the visible container.
	 */
	function isMouseOverParent(intMouseLeft, intMouseTop)
	{
		var layer			= document.layer[_containers[_visibleContainer].getParentLayerId()];
		var parentTop		= _containers[_visibleContainer].getTop();
		var parentLeft		= _containers[_visibleContainer].getLeftMargin();
		var parentBottom	= layer.height + parentTop;
		var parentRight		= layer.width + parentLeft;

		if (intMouseTop >= parentTop && intMouseLeft >= parentLeft)
		{
			return (intMouseTop <= parentBottom && intMouseLeft <= parentRight);
		}
		return false;
	}


	/**
	 * Enables the mouse down event listener.
	 */
	/*void*/ function EnableMouseListening()
	{
		if (navigator.appName == "Netscape")
			document.captureEvents(Event.MOUSEMOVE);

		document.onmousemove = _this.checkMousePosition;
	}


	/**
	 * Disables the mousedown event listener.
	 */
	/*void*/ function DisableMouseListening()
	{
		if (navigator.appName == "Netscape")
			document.releaseEvents(Event.MOUSEMOVE);

		document.onmousemove = null;
	}
}



/**
 * Represents a layer clipper.
 */
/*class*/ function LayerContainer(strContainerName, strParentLayerId)
{
	var _container	 	= new Array();
	var _containerName	= String(strContainerName);
	var _parentLayerId	= strParentLayerId;

	var _topPosition	= 0;
	var _leftPosition	= 0;
	var _isInitialized	= false;

	var _leftMargin		= 15;
	var _browserInfo	= new BrowserInfo();

	var _height			= 0;
	var _width			= 200;

	var _currentHeight	= 0;
	var _currentWidth	= 0;


	/**
	 * Returns true, if this layer group is initialized.
	 */
	/*Boolean*/ this.isInitialized = function()
	{
		return _isInitialized;
	}


	/**
	 * Returns the margin to the top window border.
	 */
	/*Integer*/ this.getTop = function()
	{
		return _topPosition;
	}


	/**
	 * Returns the margin to the left window border.
	 */
	/*Integer*/ this.getLeft = function()
	{
		return _leftPosition;
	}


	/**
	 * Returns the height of the layer group.
	 */
	/*Integer*/ this.getHeight = function()
	{
		return _currentHeight;
	}


	/**
	 * Returns the width of the layer group
	 */
	/*Integer*/ this.getWidth = function()
	{
		return _currentWidth;
	}


	/**
	 * Returns the margin to the left border.
	 */
	/*Integer*/ this.getLeftMargin = function()
	{
		return _leftMargin;
	}


	/**
	 * Returns the parent layer id.
	 */
	/*String*/ this.getParentLayerId = function()
	{
		return _parentLayerId;
	}


	/**
	 * Returns the name of this container.
	 */
	/*String*/ this.getName = function()
	{
		return _containerName;
	}


	/**
	 * Sets the name of the layer.
	 */
	/*void*/ this.addLayerByName = function(strLayerId, strLayerText, strLayerHandle, intLayerEnum, blnIsEndingLayer)
	{
		_container[_container.length] =
		{
			LayerId: String(strLayerId),
			Text: String(strLayerText),
			Handle: String(strLayerHandle),
			Type: Number(intLayerEnum),
			IsEnd: Boolean(blnIsEndingLayer),
			LayerObject: null
		};
	}


	/**
	 * Creates the specified layers.
	 */
	/*void*/ this.createLayers = function()
	{
		if (!_isInitialized)
		{
			// init parent layer
			initParentLayer();

			var layerId		= "";
			var layerText	= "";
			var layerLink	= "";
			var foreColor	= "";
			var lineColor	= "";

			for (var i = 0; i < _container.length; ++i)
			{
				layerId		= _container[i].LayerId;
				layerText	= _container[i].Text;
				layerLink	= _container[i].Handle;
				foreColor	= LAYER_CONTENT[_container[i].Type].ForeColor;
				lineColor	= LAYER_CONTENT[_container[i].Type].LineColor;

				if (!_container[i].IsEnd)
				{
					document.createRunTimeLayer(_container[i].LayerId, LAYER_CONTENT.getChildLayer(layerId, layerLink, layerText, foreColor, lineColor));
				}
				else
				{
					document.createRunTimeLayer(_container[i].LayerId, LAYER_CONTENT.getChildEndLayer(layerId, layerLink, layerText, foreColor, lineColor));
				}

				_container[i].LayerObject = document.layer[layerId];
			}
			_isInitialized = true;
		}
	}


	/**
	 * Visualizes all child layers of this container.
	 */
	/*void*/ this.showLayers = function()
	{
		if (document.layers) // netscape 4.x is disabled
			return;

		if (!_isInitialized)
			this.createLayers();

		// reset current layer positions
		if (IsIE())
		{
			_currentHeight = _height - window.document.body.scrollLeft;
			_currentWidth = _width - window.document.body.scrollTop;
		}
		else
		{
			_currentHeight = _height;
			_currentWidth = _width;
		}

		for (var i = 0; i < _container.length; ++i)
		{
			arrangeLayer(_container[i].LayerObject);
			_container[i].LayerObject.show();
		}
	}


	/**
	 * Hides all child layers of this container.
	 */
	/*void*/ this.hideLayers = function()
	{
		if (document.layers) // netscape 4.x is disabled
			return;

		if (_isInitialized)
		{
			for (var i = 0; i < _container.length; ++i)
			{
				_container[i].LayerObject.hide();
			}
		}
	}


	/**
	 * Returns a layer object, which contains the specified id. Returns null,
	 * if no layer was found.
	 */
	/*CrossLayer*/ this.getLayer = function(strLayerId)
	{
		if (_isInitialized)
		{
			for (var i = 0; i < _container.length; ++i)
			{
				if (_container[i].LayerId == strLayerId)
				{
					return _container[i].LayerObject;
				}
			}
		}
		return null;
	}


	/**
	 * Arranges the specified layer.
	 */
	/*void*/ function arrangeLayer(objCrossLayer)
	{
		if (objCrossLayer.width > _currentWidth)
		{
			_currentWidth = objCrossLayer.width;
		}

		objCrossLayer.setTopMargin(_topPosition + _currentHeight);
		objCrossLayer.setLeftMargin(_leftPosition);
		_currentHeight += objCrossLayer.height;
	}


	/**
	 * Initializes the margin top of the parent layer.
	 */
	/*void*/ function initParentLayer()
	{
		document.createLayer(_parentLayerId);
		_leftPosition = _leftMargin + document.layer[_parentLayerId].width;

		if (IsIE())
		{
			setOffsetTop(document.all[_parentLayerId].parentElement);
		}
		else
		{
			_topPosition = document.layer[_parentLayerId].top;
		}
	}


	/**
	 * Sets the offset top for internet explorer.
	 */
	/*void*/ function setOffsetTop(objTop)
	{
		if (!objTop)
			return;

		// ignore html (IE 4) and td (IE 5.x+) tags
		if (objTop.tagName.toLowerCase() != "td" && objTop.tagName.toLowerCase() != "html")
			_topPosition += objTop.offsetTop;

		setOffsetTop(objTop.parentElement);
	}


	/**
	 * Returns true, if the client browser is an IE 6.0 or higher.
	 */
	/*Boolean*/ function IsIE()
	{
		return (_browserInfo.typ.isIE);
	}


	/**
	 * Returns true if a mac has opened this page.
	 */
	/*Boolean*/ function IsMac()
	{
		return (navigator.platform.toLowerCase().indexOf("mac") != -1);
	}
}
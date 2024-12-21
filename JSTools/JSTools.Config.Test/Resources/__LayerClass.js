function LayerHandler(layerName,strLayerStyleClass)
{
	// writes a new layer content
	this.SetContent = function(strContent)
	{
		if(LayerObjectIsReady("SetContent",strContent))
		{
			_content = strContent;
			window.setTimeout("window.document.Layer." + _id + ".InitializeNewContent();", 500);
		}
	}


	// sets the layer height
	this.SetHeight = function(intHeight)
	{
		if(LayerObjectIsReady("SetHeight",intHeight))
		{
			_height = ToNumber(intHeight);

			if(_isDOMorIE)
			{
				_layerObject.style.height = _height;
			}
			else if(_isNC)
			{
				_clip.height = _height;
			}
			DelegateEventToObject(LayerEvent.RESIZE);
		}
	}


	// sets the layer width
	this.SetWidth = function(intWidth)
	{
		if(LayerObjectIsReady("SetWidth",intWidth))
		{
			_width = ToNumber(intWidth);

			if(_isDOMorIE)
			{
				_layerObject.style.width = _width;
			}
			else if(_isNC)
			{
				_clip.width = _width;
			}
			DelegateEventToObject(LayerEvent.RESIZE);
		}
	}


	// sets the layer visibility
	this.SetVisibility = function(strVisibility)
	{
		if(LayerObjectIsReady("SetVisibility",strVisibility))
		{
			_visibility = (strVisibility.toLowerCase() == "visible") ? "visible" : "hidden";

			if(_isDOMorIE)
			{
				_layerObject.style.visibility = _visibility;
			}
			else if(_isNC)
			{
				_layerObject.visibility = (_visibility == "visible") ? "show" : "hide";
			}
		}
	}


	// sets the layer visibility = visible
	this.Show = function()
	{
		this.SetVisibility("visible");
	}


	// sets the layer visibility = hidden
	this.Hide = function()
	{
		this.SetVisibility("hidden");
	}


	// sets the layer clip
	this.SetClip = function(strClipSide, intClipValue)
	{
		if(LayerObjectIsReady("SetClip", strClipSide, intClipValue))
		{
			var editClip		= strClipSide.toLowerCase()
			var thisClipString	= "rect(";
			var thisSplitChars;

			for(var clipCount = 0; clipCount < _clipSides.length; ++clipCount)
			{
				if(_clipSides[clipCount] == editClip)
				{
					_clip[editClip] = ToNumber(intClipValue);
				}
				if(_isDOMorIE)
				{
					thisClipString += _clip[_clipSides[clipCount]] + "px" + ((clipCount + 1 == _clipSides.length) ? ")" : " ");
				}
			}

			if(_isDOMorIE)
			{
				_layerObject.style.clip = thisClipString;
			}
		}
	}


	// sets the layer z-index
	this.SetZIndex = function(intZIndex)
	{
		if(LayerObjectIsReady("SetZIndex", intZIndex))
		{
			_zIndex = ToNumber(intZIndex);

			if(_isDOMorIE)
			{
				_layerObject.style.zIndex = _zIndex;
			}
			else if(_isNC)
			{
				_layerObject.zIndex = _zIndex;
			}
		}
	}


	// sets the margin top
	this.SetTopMargin = function(intTopMargin, blnNoEvent)
	{
		if(LayerObjectIsReady("SetTopMargin", intTopMargin))
		{
			_topMargin = ToNumber(intTopMargin);

			if(_isDOMorIE)
			{
				_layerObject.style.top = _topMargin;
			}
			else if(_isNC)
			{
				_layerObject.top = _topMargin;
			}

			if(_delegateMoveEvent)
			{
				DelegateEventToObject(LayerEvent.MOVE);
			}
		}
	}


	// sets the margin left
	this.SetLeftMargin = function(intLeftMargin, blnNoEvent)
	{
		if(LayerObjectIsReady("SetLeftMargin", intLeftMargin))
		{
			_leftMargin = ToNumber(intLeftMargin);

			if(_isDOMorIE)
			{
				_layerObject.style.left = _leftMargin;
			}
			else if(_isNC)
			{
				_layerObject.left = _leftMargin;
			}

			if(_delegateMoveEvent)
			{
				DelegateEventToObject(LayerEvent.MOVE);
			}
		}
	}


	// moves the layer to the positions left and top positions
	this.MoveTo = function(intLeft,intTop)
	{
		if(LayerObjectIsReady("MoveTo", intLeft, intTop))
		{
			_delegateMoveEvent = false;

			this.SetLeftMargin(intLeft);
			this.SetTopMargin(intTop);
			DelegateEventToObject(LayerEvent.MOVE);

			_delegateMoveEvent = true;
		}
	}


	// get layer properties

	// returns the content
	this.GetContent = function()
	{
		return _content;
	}


	// returns the layer height
	this.GetHeight = function()
	{
		return _height;
	}


	// returns the layer width
	this.GetWidth = function()
	{
		return _width;
	}


	// returns the layer visibility
	this.GetVisibility = function()
	{
		return _visibility;
	}


	// returns a clip item of the layer
	this.GetClip = function(strItem)
	{
		var thisItem = String(strItem);

		for(var clipItem in _clip)
		{
			if(clipItem.toLowerCase() == thisItem.toLowerCase())
			{
				return _clip[clipItem];
			}
		}
		return "undefined";
	}


	// returns the layer z-index
	this.GetZIndex = function()
	{
		return _zIndex;
	}


	// returns the margin top (top)
	this.GetTopMargin = function()
	{
		return _topMargin;
	}


	// returns the margin left (left)
	this.GetLeftMargin = function()
	{
		return _leftMargin;
	}


	// returns all images which are registred in the layer with an id or a name
	this.GetImages = function()
	{
		return _images;
	}


	// returns only one image which is specificated in the strImageName
	this.GetImageById = function(strImageName)
	{
		return _images[strImageName];
	}


	// returns all forms which are registred in the layer with an id or a name
	this.GetForms = function()
	{
		return _forms;
	}


	// returns only one form which is specificated in the strFormName
	this.GetFormById = function(strFormName)
	{
		return _forms[strFormName];
	}


	// returns all links which are registred in the layer with an id or a name
	this.GetLinks = function()
	{
		return _links;
	}


	// returns only one link which is specificated in the strLinkName
	this.GetLinkById = function(strLinkName)
	{
		return _links[strLinkName];
	}


	// returns the layer id
	this.GetId = function()
	{
		return _id;
	}



	// public thread properties

	// writes the content into the layer
	this.InitializeNewContent = function()
	{
		if(_isDOMorIE)
		{
			_layerObject.innerHTML = _content;
			DocChecker.CheckDocObjects();
		}
		else if(_isNC)
		{
			WriteNCDocumentContent();
		}
		RefreshAll(false);
	}


	// searches layer object in window.document.Layer array
	this.CheckObjectStatus = function()
	{
		if(IsUndefined(window.document.Layer[_id]) && IsUndefined(_checkThread))
		{
			_checkThread = window.setInterval("window.document.Layer." + _id + ".CheckObjectStatus();",100);
		}
		else
		{
			if(!IsUndefined(_checkThread))
			{
				if(_checkThread)
				{
					window.clearInterval(_checkThread);
				}
				InitializeObject();
			}
		}
	}


	// adds a new eventlistener to the current layer object
	this.AddEventListener = function(intTypes, objFunction, strTriggerName)
	{
		var eventIndex = null;

		if(_event && objFunction && strTriggerName)
		{
			eventIndex = _event.HandleEvent(intTypes, objFunction, strTriggerName);
		}
		return eventIndex;
	}


	// removeses a layer eventlistener
	this.RemoveEventListener = function(intTypes, strItemName)
	{
		var eventLength = null;

		if(_event)
		{
			eventLength = _event.HandleEvent(intTypes, null, strItemName);
		}
		return eventLength;
	}


	// returns an instance of the _event object
	this.GetLayerEventObject = function()
	{
		return _event;
	}


	this.toString = function()
	{
		return "[object Layer]";
	}



	//private statements

	// init layer propertys

	// initializes the clip object
	function InitClip()
	{
		if(_isDOMorIE)
		{
			var myClip = String(_layerObject.style.clip).toLowerCase();

			_clip.top		= 0;
			_clip.left		= 0;
			_clip.right		= parseInt(_layerObject.offsetWidth);
			_clip.bottom	= parseInt(_layerObject.offsetHeight);

			if(!IsVoid(myClip) && myClip != "auto" && myClip != "rect()")
			{
				myClip = myClip.replace(/rect\(/,"");

				while(myClip.search(/[px]|[\)]/) != -1)
				{
					myClip = myClip.replace(/[px]|[\)]/,"");
				}
				myClip = myClip.split(" ");

				for(var clipName = 0; clipName < myClipNames.length; ++clipName)
				{
					_clip[myClipNames[clipName]] = ToNumber(myClip[clipName]);
				}
			}
		}
		else if(_isNC)
		{
			_clip = _layerObject.clip;
		}
	}


	// initializes the layer height
	function InitHeight()
	{
		if(_isDOMorIE)
		{
			_height = _clip.bottom - _clip.top;

			if(_height < 0 || isNaN(_height))
			{
				_height = 0;
			}
		}
		else if(_isNC)
		{
			_height = ToNumber(_clip.height);
		}
	}


	// initializes the layer width
	function InitWidth()
	{
		if(_isDOMorIE)
		{
			_width = _clip.right - _clip.left;

			if(_width < 0 || isNaN(_width))
			{
				_width = 0;
			}
		}
		else if(_isNC)
		{
			_width = ToNumber(_clip.width);
		}
	}


	// initializes the layer visibility
	function InitVisibility()
	{
		if(_isDOMorIE)
		{
			_visibility = _layerObject.style.visibility.toLowerCase();
		}
		else if(_isNC)
		{
			_visibility = (_layerObject.visibility == "show") ? "visible" : "hidden";
		}
	}


	// initializes the layer z-index
	function InitZIndex()
	{
		if(_isDOMorIE)
		{
			_zIndex = ToNumber(_layerObject.style.zIndex);
		}
		else if(_isNC)
		{
			_zIndex = ToNumber(_layerObject.zIndex);
		}
	}


	// initializes the margin top
	function InitTopMargin()
	{
		if(_isDOMorIE)
		{
			_topMargin = parseInt(_layerObject.offsetTop);
		}
		else if(_isNC)
		{
			_topMargin = _layerObject.top;
		}
	}


	// initializes the margin left
	function InitLeftMargin()
	{
		if(_isDOMorIE)
		{
			_leftMargin = parseInt(_layerObject.offsetLeft);
		}
		else if(_isNC)
		{
			_leftMargin = _layerObject.left;
		}
	}


	// initializes the layer images
	function InitImages()
	{
		if(_isDOMorIE)
		{
			_images = InitializeDocObject(DocChecker.GetCheckedImages());
		}
		else if(_isNC)
		{
			_images = _layerObject.document.images;
		}
	}


	// initializes the layer forms
	function InitForms()
	{
		if(_isDOMorIE)
		{
			_forms = InitializeDocObject(DocChecker.GetCheckedForms());
		}
		else if(_isNC)
		{
			_forms = _layerObject.document.forms;
		}
	}


	// initializes the layer links
	function InitLinks()
	{
		if(_isDOMorIE)
		{
			_links = InitializeDocObject(DocChecker.GetCheckedLinks());
		}
		else if(_isNC)
		{
			_links = _layerObject.document.links;
		}
	}


	// wirtes into the document for Netscape 4.x
	function WriteNCDocumentContent()
	{
		with(_layerObject.document)
		{
            for(var i = 0; i < 2; ++i)
            {
                open();
                write(_content);
                close();
            }
        }
	}


	// initializes the written layer object
	function InitializeObject()
	{
		GetLayerObject();
		RefreshAll(true);
		InitializeOrders();
	}


	// if the blnContentItemsOnly is true, all items will refresh
	function RefreshAll(blnContentItemsOnly)
	{
		if(blnContentItemsOnly)
		{
			InitClip();
			InitHeight();
			InitWidth();
			InitVisibility();
			InitZIndex();
			InitTopMargin();
			InitLeftMargin();
		}

		InitImages();
		InitForms();
		InitLinks();

		DelegateLoadingEvent();
	}


	// throws a loading or an initialize event
	function DelegateLoadingEvent()
	{
		if(IsNull(_event))
		{
			_event = new LayerEventHandler(_layerObject, _id);
			DelegateEventToObject(LayerEvent.LOAD);
		}
		else
		{
			DelegateEventToObject(LayerEvent.INITIALIZE);
		}
	}


	// delegates an event with a call to the event object
	function DelegateEventToObject(intNumber)
	{
		var eventName = LayerEvent.GetEventName(intNumber);
		_event.DelegateEvent(eventName, new LayerHandleEvent(_id, eventName));
	}


	// adds a layer to the document
	function AddLayer(strStyleClass)
	{
		var layerStyle = (IsUndefined(strStyleClass)) ? 'style="position:absolute;visibility:hidden"' : 'class="'+strStyleClass+'"';
		document.write('<div id="' + _id + '" name="' + _id + '" ' + layerStyle + '></div>');

		_checkThread = "undefined";
		CheckObjectStatus();
	}


	// creates a reference to the real browser layer object
	function GetLayerObject()
	{
		if(Browser.GetTyp().isDOM)
		{
			if(!IsUndefined(document.getElementById(_id)))
			{
				_layerObject = document.getElementById(_id);
			}
		}
		else
		{
			if(_isNC)
			{
				GetNetscapeLayer(window.document.layers);
			}
			if(Browser.GetTyp().isIE)
			{
				if(!IsUndefined(document.all[_id]))
				{
					_layerObject = document.all[_id];
				}
			}
		}
	}


	// searches in the netscape 4.x object model for layer references
	function GetNetscapeLayer(objLayer)
	{
		for(var l = 0; l < objLayer.length; ++l)
		{
			if(objLayer[l].name == _id)
			{
				_layerObject = objLayer[l];
				break;
			}
			GetNetscapeLayer(objLayer[l].document.layers);
		}
	}


	// initialize the document object which is specificated by the parameters
	function InitializeDocObject(myLayerObject)
	{
		var myLayerArray = new Array();
		var myLayerContent = String(_layerObject.innerHTML).toLowerCase();

		for(var myObj = 0; myObj < myLayerObject.length; ++myObj)
		{
			var normalObjectId = String(myLayerObject[myObj].id);
			var pattern = eval("/\<" + myLayerObject.startTag + "[^<|^>]{1,}\=.{0,1}" + normalObjectId.toLowerCase() + "[^<|^>]*\>/");

			if(myLayerContent.search(pattern) != -1)
			{
				myLayerArray[myLayerArray.length] = myLayerArray[normalObjectId] = myLayerObject[myObj];
			}
		}

		return myLayerArray;
	}


	// chooses, if the layerObject is ready
	function LayerObjectIsReady(strMethodName,strFirstMethodValue,strSecondMethodValue)
	{
		if(!_isValidBrowser)
		{
			return false;
		}
		else
		{
			if(!_layerObject)
			{
				LayerObjectAddOrder(strMethodName,strFirstMethodValue,strSecondMethodValue);
				return false;
			}
			return true;
		}
	}


	// wirtes the orders into a temp array
	function LayerObjectAddOrder(strMethod,strFirstParam,strSecondParam) {
		var delayLength = _layerObjectDelay.length;
		_layerObjectDelay[delayLength] = new Array();

		with(Functions)
		{
			Array.Add(_layerObjectDelay[delayLength], strMethod);

			if(!IsUndefined(strFirstParam))
			{
				Array.Add(_layerObjectDelay[delayLength], strFirstParam);
			}
			if(!IsUndefined(strSecondParam))
			{
				Array.Add(_layerObjectDelay[delayLength], strSecondParam);
			}
		}
	}


	// initializes the orders of the layerObjectDelay array
	function InitializeOrders()
	{
		for(var o = 0; o < _layerObjectDelay.length; ++o)
		{
			var thisEvalString = _layerObjectDelay[o][0] + "(";

			for(var c = 1; c < _layerObjectDelay[o].length-1; ++c)
			{
				thisEvalString += "'" + _layerObjectDelay[o][c] + "',";
			}

			eval(thisEvalString + "'" + _layerObjectDelay[o][_layerObjectDelay[o].length-1] + "');");
		}

		_layerObjectDelay = new Array();
	}


	// first layer object construction
	function InitializeLayerHandler()
	{
		if(_isDOMorIE)
		{
			DocChecker.CheckDocObjects();
		}

		GetLayerObject();

		if(IsUndefined(_layerObject))
		{
			AddLayer(strLayerStyleClass);
		}
		else
		{
			RefreshAll(true);
		}
	}



	var _height				= 0;
	var _width				= 0;
	var _visibility			= "hidden";
	var _zIndex				= 0;
	var _topMargin			= 0;
	var _leftMargin			= 0;

	var _delegateMoveEvent	= true;

	var _clip				= new Array();
	var _clipSides			= new Array("top", "right", "bottom", "left");

	var _id					= layerName;
	var _content			= "";

	var _images				= new Array();
	var _forms				= new Array();
	var _links				= new Array();

	var _layerObject		= "undefined";
	var _checkThread		= "undefined";

	var _isOP				= Browser.GetTyp().isOP;
	var _isNC				= Browser.GetTyp().isNC;
	var _isDOMorIE			= (Browser.GetTyp().isDOM || Browser.GetTyp().isIE);
	var _isValidBrowser		= (Browser.GetVersion() >= 4 || Browser.GetTyp().isDOM);
	var _layerObjectDelay	= new Array();
	var _event				= null;


	if(_isValidBrowser)
	{
		InitializeLayerHandler();
	}
}



//---------- GLOBAL STATEMENTS ----------//

function LayerContainer()
{
	
}
// interface to create layers
function CreateLayer(strLayerId,strStyleClass)
{
	return window.document.Layer[window.document.Layer.length] = window.document.Layer[ToString(strLayerId).ToEnumerable()] = new LayerHandler(strLayerId,strStyleClass);
}

// initialize basic arrays
with(window)
{
	document.Layer			= new Array();
	document.CreateLayer	= CreateLayer;
}

// document.write("<a href=\"#\" target='_self'" + ' onclick=\'\' onmouseover=""' +">This Is A Test</a>");
/* document.write("<a href=\"#\" target='_self'" + ' onclick=\'\' onmouseover=""' +">This Is A Test</a>"); */
document.write("<a href=\"#\" target='_self'" + ' onclick=\\\'/* javascript function here */\\\\\' onmouseover=\\"\\"' +">This Is A Test</a>");
JSTools.Web.Window.WindowOpener = function(objOptions)
{
	if (objOption != "[object WindowOptions]")
	{
		this.ThrowArgumentException("The given argument does not contain a WindowOptions object!");
	}

	var _winContent = "";
	var _winOptions = objOption;
	var _winName = _winOptions.Name;

	var _winHeight = ((_winOptions.Height) ? _winOptions.Height : 0);
	var _winWidth = ((_winOptions.Width) ? _winOptions.Width : 0);
	var _isAccessible = null;
	var _winObject = null;
	var _unloadIndex = -1;


	// closes the child window, when the parent closes
	function IsParentDependent(blnDepends)
	{
		if (blnDepends && _unloadIndex == -1)
		{
			_unloadIndex = window.EventHandler.AddEventListener("onunload", this.Close);
		}
		if (!blnDepends && _unloadIndex != -1)
		{
			window.EventHandler.RemoveEventListenerByIndex("onunload", _unloadIndex);
			_unloadIndex = -1;
		}
	}
	this.IsParentDependent = IsParentDependent;


	// returns the current window object, if accessable
	function GetWindowObject()
	{
		return ReturnValueIfAccessible(_winObject);
	}
	this.GetWindowObject = GetWindowObject;


	// sets a new content into the window, if accessable
	function SetContent(strContent, blnOpen, blnClose)
	{
		if(IsWindowValid())
		{
			if(blnOpen)
			{
				_winObject.document.open();
			}

			_winObject.document.write(strContent);
			_winContent = strContent;

			if(blnClose)
			{
				_winObject.document.close();
			}
		}
	}
	this.SetContent = SetContent;


	// sets a new window width, if accessable
	function SetWidth(intWidth)
	{
		if(_isAccessible)
		{
            _winWidth = intWidth;
            ResizeWindow(intWidth, "outerWidth");
        }
	}
	this.SetWidth = SetWidth;


	// sets a new window height, if accessable
	function SetHeight(intHeight)
	{
		if(_isAccessible)
		{
            _winHeight = intHeight;
            ResizeWindow(intHeight, "outerHeight");
        }
	}
	this.SetHeight = SetHeight;


	// returns the current content, if accessable
	function GetContent()
	{
		return ReturnValueIfAccessible(_winContent);
	}
	this.GetContent = GetContent;


	// returns the current width, if accessable
	function GetWidth()
	{
		return ReturnValueIfAccessible(_winWidth);
	}
	this.GetWidth = GetWidth;


	// returns the current height, if accessable
	function GetHeight()
	{
		return ReturnValueIfAccessible(_winHeight);
	}
	this.GetHeight = GetHeight;


	// returns the window name number, if accessable
	function GetWindowName()
	{
		return ReturnValueIfAccessible(_winName);
	}
	this.GetWindowName = GetWindowName;


	// closes the current window, if accessable
	function Close()
	{
		if(IsWindowValid())
		{
			_winObject = window.open("about:blank", _winName);
			_winObject.close();
		}
	}
	this.Close = Close;


	// opens a new url into the same window object, if accessable
	function RefreshWindow(objOption)
	{
		if(_isAccessible)
		{
			_winObject = OpenObjectOptionWindow(objOption, _winName);
			_isAccessible = IsWindowAccessible(objOption.Url);
		}
	}
	this.RefreshWindow = RefreshWindow;


	// returns true, if the window can be accessed
	function IsAccessible()
	{
		return (_winObject && !_winObject.closed && _isAccessible);
	}
	this.IsAccessible = IsAccessible;



	// resizes the window
	function ResizeWindow(intChange, strFunction)
	{
		if(IsWindowValid())
		{
			if(window.Browser.GetType().IsNS4)
			{
				_winObject[strFunction] = intChange;
			}
			else
			{
				_winObject.resizeTo(_winWidth, _winHeight);
			}
		}
	}


	// opens a new window object
	function OpenObjectOptionWindow(objWinOption, strWinName)
	{
		return window.open(objWinOption.Url, strWinName, objWinOption.GetArgumentString());
	}


	// checks if the window is open, accessible and has a valid opener
	function IsWindowValid()
	{
		return (_winObject && !_winObject.closed && _winObject.opener && _isAccessible);
	}


	// checks if the window is accessible
	function IsWindowAccessible(strUrl)
	{
		var indexHttpId = "http:";

		if(strUrl.indexOf(indexHttpId) == 0)
		{
			var newUrl = strUrl.substring(indexHttpId.length + 2, strUrl.length);
			var newUrlEndPos = ((newUrl.indexOf("/") != -1) ? newUrl.indexOf("/") : newUrl.length);

			return (newUrl.substring(0, newUrlEndPos) == window.location.host);
		}
		return true;
	}


	// returns null if the current window object isn't accessible, else objValue
	function ReturnValueIfAccessible(objValue)
	{
		return ((_isAccessible) ? objValue : null);
	}


	_winObject = OpenObjectOptionWindow(_winOptions, _winName);
	_isAccessible = IsWindowAccessible(_winOptions.Url);

	if (_winOptions.IsDependent)
	{
		this.IsParentDependent(true);
	}
}
WindowOpener.prototype.toString = function()
{
    return "[object WindowOpener]";
}
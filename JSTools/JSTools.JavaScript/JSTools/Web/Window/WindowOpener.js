function WindowOpener(objOption)
{
	if (objOption != "[object WindowOptions]")
	{
		this.ThrowArgumentException("The given argument does not contain a WindowOptions object!");
	}

	var _winContent			= "";
	var _winOptions			= objOption;
	var _winName			= _winOptions.Name;

	var _winHeight			= ((_winOptions.Height) ? _winOptions.Height : 0);
	var _winWidth			= ((_winOptions.Width) ? _winOptions.Width : 0);
	var _isAccessible 		= null;
	var _winObject			= null;
	var _unloadIndex		= -1;


	// closes the child window, when the parent closes
	this.IsParentDependent = function(blnDepends)
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


	// returns the current window object, if accessable
	this.GetWindowObject = function()
	{
		return ReturnValueIfAccessible(_winObject);
	}


	// sets a new content into the window, if accessable
	this.SetContent = function(strContent, blnOpen, blnClose)
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


	// sets a new window width, if accessable
	this.SetWidth = function(intWidth)
	{
		if(_isAccessible)
		{
            _winWidth = intWidth;
            ResizeWindow(intWidth, "outerWidth");
        }
	}


	// sets a new window height, if accessable
	this.SetHeight = function(intHeight)
	{
		if(_isAccessible)
		{
            _winHeight = intHeight;
            ResizeWindow(intHeight, "outerHeight");
        }
	}


	// returns the current content, if accessable
	this.GetContent = function()
	{
		return ReturnValueIfAccessible(_winContent);
	}


	// returns the current width, if accessable
	this.GetWidth = function()
	{
		return ReturnValueIfAccessible(_winWidth);
	}


	// returns the current height, if accessable
	this.GetHeight = function()
	{
		return ReturnValueIfAccessible(_winHeight);
	}


	// returns the window name number, if accessable
	this.GetWindowName = function()
	{
		return ReturnValueIfAccessible(_winName);
	}


	// closes the current window, if accessable
	this.Close = function()
	{
		if(IsWindowValid())
		{
			_winObject = window.open("about:blank", _winName);
			_winObject.close();
		}
	}


	// opens a new url into the same window object, if accessable
	this.RefreshWindow = function(objOption)
	{
		if(_isAccessible)
		{
			_winObject		= OpenObjectOptionWindow(objOption, _winName);
			_isAccessible	= IsWindowAccessible(objOption.Url);
		}
	}


	// returns true, if the window can be accessed
	this.IsAccessible = function()
	{
		return (_winObject && !_winObject.closed && _isAccessible);
	}



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
		var indexHttpId		= "http:";

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


	_winObject		= OpenObjectOptionWindow(_winOptions, _winName);
	_isAccessible	= IsWindowAccessible(_winOptions.Url);

	if (_winOptions.IsDependent)
	{
		this.IsParentDependent(true);
	}
}
WindowOpener.prototype.toString = function()
{
    return "[object WindowOpener]";
}
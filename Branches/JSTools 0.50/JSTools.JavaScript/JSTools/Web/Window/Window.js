namespace("JSTools.Web.Window");


/// <class>
/// 
/// 
/// window.onerror event is not provided by the Window class. You should
/// use the JSTools.ExceptionHandling.Handler class instead.
/// </class>
JSTools.Web.Window.Window = function(objTopWindow, objOptions)
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	if (!objTopWindow || typeof(objTopWindow) != 'object')
	{
		JSTools.Exception.Throw(
			new JSTools.ExceptionHandling.ArgumentException("The given argument is not a valid window object.", "objTopWindow") );
	}

	this.InitType(arguments, "JSTools.Web.Window.Window");

	var DEFAULT_WIN_URL = "about:blank";
	var EVENT_NAME_PATTERN = "on{0}";
	var BLUR_EVENT_NAME = "onblur";
	var FOCUS_EVENT_NAME = "onfocus";
	var LOAD_EVENT_NAME = "onload";
	var RESIZE_EVENT_NAME = "onresize";
	var UNLOAD_EVENT_NAME = "onunload";

	var _this = this;
	var _events = new JSTools.Event.SubjectList();
	var _window = objTopWindow;
	var _windowOptions = null;
	var _openedWindows = [ ];


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------

	/// <constructor>
	/// Creates a new JSTools.Web.Window.Window instance.
	/// </constructor>
	function Init()
	{
		if (objOptions
			&& typeof(objOptions) == 'object'
			&& objOptions.IsTypeOf(JSTools.Web.Window.WindowOptions))
		{
			_windowOptions = objOptions;
		}

		_window[BLUR_EVENT_NAME] = RecieveWindowEvent;
		_window[FOCUS_EVENT_NAME] = RecieveWindowEvent;
		_window[LOAD_EVENT_NAME] = RecieveWindowEvent;
		_window[RESIZE_EVENT_NAME] = RecieveWindowEvent;
		_window[UNLOAD_EVENT_NAME] = RecieveWindowEvent;
	}


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	function GetOptions()
	{
		return _windowOptions;
	}
	this.GetOptions = GetOptions;


	/// <method>
	/// Adds a new observer to the OnBlur subject (event).
	/// </method>
	/// <param name="varLogObserver" type="Function">Adds the given function object to the OnBlur event.</param>
	/// <param name="varLogObserver" type="JSTools.Reflection.MethodInfo">Adds the given MethodInfo object to the OnBlur event.</param>
	/// <param name="varLogObserver" type="JSTools.Event.IObserver">Adds the given IObserver object to the OnBlur event.</param>
	/// <returns type="Integer">Returns the index, at which the observer object has been added.
	/// Returns -1 if the given observer object is invalid and not added.</returns>
	function AddOnBlurEvent(varLogObserver)
	{
		return _events.Attach(BLUR_EVENT_NAME, varLogObserver);
	}
	this.AddOnBlurEvent = AddOnBlurEvent;


	/// <method>
	/// Removes an observer from the OnBlur subject (event).
	/// </method>
	/// <param name="varObserverToDetach" type="JSTools.Event.IObserver">Observer object which should be removed.</param>
	/// <param name="varObserverToDetach" type="Integer">Internal index of the observer object which should be removed.</param>
	function RemoveOnBlurEvent(varObserverToDetach)
	{
		_events.Detach(BLUR_EVENT_NAME, varObserverToDetach);
	}
	this.RemoveOnBlurEvent = RemoveOnBlurEvent;


	/// <method>
	/// Adds a new observer to the OnFocus subject (event).
	/// </method>
	/// <param name="varLogObserver" type="Function">Adds the given function object to the OnFocus event.</param>
	/// <param name="varLogObserver" type="JSTools.Reflection.MethodInfo">Adds the given MethodInfo object to the OnFocus event.</param>
	/// <param name="varLogObserver" type="JSTools.Event.IObserver">Adds the given IObserver object to the OnFocus event.</param>
	/// <returns type="Integer">Returns the index, at which the observer object has been added.
	/// Returns -1 if the given observer object is invalid and not added.</returns>
	function AddOnFocusEvent(varLogObserver)
	{
		return _events.Attach(FOCUS_EVENT_NAME, varLogObserver);
	}
	this.AddOnFocusEvent = AddOnFocusEvent;


	/// <method>
	/// Removes an observer from the OnFocus subject (event).
	/// </method>
	/// <param name="varObserverToDetach" type="JSTools.Event.IObserver">Observer object which should be removed.</param>
	/// <param name="varObserverToDetach" type="Integer">Internal index of the observer object which should be removed.</param>
	function RemoveOnFocusEvent(varObserverToDetach)
	{
		_events.Detach(FOCUS_EVENT_NAME, varObserverToDetach);
	}
	this.RemoveOnFocusEvent = RemoveOnFocusEvent;


	/// <method>
	/// Adds a new observer to the OnLoad subject (event).
	/// </method>
	/// <param name="varLogObserver" type="Function">Adds the given function object to the OnLoad event.</param>
	/// <param name="varLogObserver" type="JSTools.Reflection.MethodInfo">Adds the given MethodInfo object to the OnLoad event.</param>
	/// <param name="varLogObserver" type="JSTools.Event.IObserver">Adds the given IObserver object to the OnLoad event.</param>
	/// <returns type="Integer">Returns the index, at which the observer object has been added.
	/// Returns -1 if the given observer object is invalid and not added.</returns>
	function AddOnLoadEvent(varLogObserver)
	{
		return _events.Attach(LOAD_EVENT_NAME, varLogObserver);
	}
	this.AddOnLoadEvent = AddOnLoadEvent;


	/// <method>
	/// Removes an observer from the OnLoad subject (event).
	/// </method>
	/// <param name="varObserverToDetach" type="JSTools.Event.IObserver">Observer object which should be removed.</param>
	/// <param name="varObserverToDetach" type="Integer">Internal index of the observer object which should be removed.</param>
	function RemoveOnLoadEvent(varObserverToDetach)
	{
		_events.Detach(LOAD_EVENT_NAME, varObserverToDetach);
	}
	this.RemoveOnLoadEvent = RemoveOnLoadEvent;


	/// <method>
	/// Adds a new observer to the OnResize subject (event).
	/// </method>
	/// <param name="varLogObserver" type="Function">Adds the given function object to the OnResize event.</param>
	/// <param name="varLogObserver" type="JSTools.Reflection.MethodInfo">Adds the given MethodInfo object to the OnResize event.</param>
	/// <param name="varLogObserver" type="JSTools.Event.IObserver">Adds the given IObserver object to the OnResize event.</param>
	/// <returns type="Integer">Returns the index, at which the observer object has been added.
	/// Returns -1 if the given observer object is invalid and not added.</returns>
	function AddOnResizeEvent(varLogObserver)
	{
		return _events.Attach(RESIZE_EVENT_NAME, varLogObserver);
	}
	this.AddOnResizeEvent = AddOnResizeEvent;


	/// <method>
	/// Removes an observer from the OnResize subject (event).
	/// </method>
	/// <param name="varObserverToDetach" type="JSTools.Event.IObserver">Observer object which should be removed.</param>
	/// <param name="varObserverToDetach" type="Integer">Internal index of the observer object which should be removed.</param>
	function RemoveOnResizeEvent(varObserverToDetach)
	{
		_events.Detach(RESIZE_EVENT_NAME, varObserverToDetach);
	}
	this.RemoveOnResizeEvent = RemoveOnResizeEvent;


	/// <method>
	/// Adds a new observer to the OnUnload subject (event).
	/// </method>
	/// <param name="varLogObserver" type="Function">Adds the given function object to the OnUnload event.</param>
	/// <param name="varLogObserver" type="JSTools.Reflection.MethodInfo">Adds the given MethodInfo object to the OnUnload event.</param>
	/// <param name="varLogObserver" type="JSTools.Event.IObserver">Adds the given IObserver object to the OnUnload event.</param>
	/// <returns type="Integer">Returns the index, at which the observer object has been added.
	/// Returns -1 if the given observer object is invalid and not added.</returns>
	function AddOnUnloadEvent(varLogObserver)
	{
		return _events.Attach(UNLOAD_EVENT_NAME, varLogObserver);
	}
	this.AddOnUnloadEvent = AddOnUnloadEvent;


	/// <method>
	/// Removes an observer from the OnUnload subject (event).
	/// </method>
	/// <param name="varObserverToDetach" type="JSTools.Event.IObserver">Observer object which should be removed.</param>
	/// <param name="varObserverToDetach" type="Integer">Internal index of the observer object which should be removed.</param>
	function RemoveOnUnloadEvent(varObserverToDetach)
	{
		_events.Detach(UNLOAD_EVENT_NAME, varObserverToDetach);
	}
	this.RemoveOnUnloadEvent = RemoveOnUnloadEvent;


	/// <method>
	/// Gets the window instance which is handled by this object.
	/// </method>
	/// <returns type="Object">Returns the real window object.</returns>
	function GetObject()
	{
		return _window;
	}
	this.GetObject = GetObject;


	/// <method>
	/// Gets the distance between the top edge of the document and the leftmost
	/// portion of the content currently visible in the window. Default value is 0.
	/// </method>
	/// <returns type="Integer">Returns the top position of the scrollbar.</returns>
	function GetScrollTop()
	{
		if (typeof(window.pageYOffset) != 'undefined')
			return window.pageYOffset;
		else if (typeof(window.document.body) != 'undefined')
			return window.document.body.scrollTop;
		else
			return 0;
	}
	this.GetScrollTop = GetScrollTop;


	/// <method>
	/// Gets the distance between the left edge of the document and the leftmost
	/// portion of the content currently visible in the window. Default value is 0.
	/// </method>
	/// <returns type="Integer">Returns the left position of the scrollbar.</returns>
	function GetScrollLeft()
	{
		if (typeof(window.pageXOffset) != 'undefined')
			return window.pageXOffset;
		else if (typeof(window.document.body) != 'undefined')
			return window.document.body.scrollLeft;
		else
			return 0;
	}
	this.GetScrollLeft = GetScrollLeft;


	/// <method>
	/// Gets the height of the document, without toolbars. Default value is 0.
	/// </method>
	/// <returns type="Integer">Returns the height of the document.</returns>
	function GetHeight()
	{
		if (typeof(window.innerHeight) != 'undefined')
			return window.innerHeight;
		else if (typeof(window.document.body) != 'undefined')
			return window.document.body.offsetHeight;
		else
			return 0;
	}
	this.GetHeight = GetHeight;


	/// <method>
	/// Gets the width of the document. Default value is 0.
	/// </method>
	/// <returns type="Integer">Returns the width of the document.</returns>
	function GetWidth()
	{
		if (typeof(window.innerWidth) != 'undefined')
			return window.innerWidth;
		else if (typeof(window.document.body) != 'undefined')
			return window.document.body.offsetWidth;
		else
			return 0;
	}
	this.GetWidth = GetWidth;


	/// <method>
	/// Retrieves the x-coordinate of the left corner of the browser's client area, 
	/// relative to the left corner of the screen. Default return value is 0.
	/// </method>
	/// <returns type="Integer">Returns the x-coordinate of the top corner of the browser.</returns>
	function GetScreenLeft()
	{
		var value = 0;

		if (typeof(window.screenX) != 'undefined')
			value = window.screenX;
		else
			value = window.screenLeft;
		
		return (value < 0 || isNaN(value)) ? 0 : value;
	}
	this.GetScreenLeft = GetScreenLeft;


	/// <method>
	/// Retrieves the y-coordinate of the top corner of the browser's client area, 
	/// relative to the top corner of the screen. Default return value is 0.
	/// </method>
	/// <returns type="Integer">Returns the y-coordinate of the top corner of the browser.</returns>
	function GetScreenTop()
	{
		var value = 0;

		if (typeof(window.screenY) != 'undefined')
			value = window.screenY;
		else
			value = window.screenTop;
		
		return (value < 0 || isNaN(value)) ? 0 : value;
	}
	this.GetScreenTop = GetScreenTop;


 	/// <method>
	/// Sets the default status of this and all nested frame windows.
	/// </method>
	/// <param name="strDefaultStatus" type="String">Status text to set.</param>
	function SetDefaultStatus(strDefaultStatus)
	{
		SetPropertyRecursive(_window, "defaultStatus", strDefaultStatus);
	}
	this.SetDefaultStatus = SetDefaultStatus;


 	/// <method>
	/// Gets the default status text, equal to window.defaultStatus.
	/// </method>
	/// <returns type="String">Default status text.</returns>
	function GetDefaultStatus()
	{
		return _window.defaultStatus;
	}
	this.GetDefaultStatus = GetDefaultStatus;


 	/// <method>
	/// Overrides the content of this window with the given string.
	/// </method>
	/// <param name="strContentToWrite" type="String">Content which should be written.</param>
	function SetContent(strContentToWrite)
	{
		with (_window.document)
		{
			write(strContentToWrite);
			close();
		}
	}
	this.SetContent = SetContent;


 	/// <method>
	/// Sets the status of this and all nested frame windows.
	/// </method>
	/// <param name="strDefaultStatus" type="String">Status text to set.</param>
	function SetStatus(strStatus)
	{
		SetPropertyRecursive(_window, "status", strStatus);
	}
	this.SetStatus = SetStatus;


 	/// <method>
	/// Gets the status text, equal to window.status.
	/// </method>
	/// <returns type="String">Status text.</returns>
	function GetStatus(strStatus)
	{
		_window.status;
	}
	this.GetStatus = GetStatus;


	/// <method>
	/// Opens a new window with the specified options.
	/// </method>
	/// <param name="varOptions" type="JSTools.Web.Window.WindowOptions">Window options of the
	/// window to open. This parameter is optional. If you do not specify an option value, this
	/// function creates a new JSTools.Web.Window.WindowOptions with the default values.</param>
	/// <param name="varOptions" type="String">Url of the window to open.</param>
	/// <returns type="Object">Returns the real window object.</returns>
	function Open(varOptions)
	{
		var optionObject = varOptions;

		if (!optionObject
			|| typeof(optionObject) != 'object'
			|| !optionObject.IsTypeOf(JSTools.Web.Window.WindowOptions))
		{
			optionObject = new JSTools.Web.Window.WindowOptions();
			
			if (typeof(varOptions) == 'string')
				optionObject.Url = String(varOptions);
		}

		var newWindow = _window.open(optionObject.Url, optionObject.Name, optionObject.toString());
		var windowObject = new JSTools.Web.Window.Window(newWindow, optionObject);

		windowObject.SetOptions();
		_openedWindows.Add(windowObject);
	}
	this.Open = Open;


	/// <method>
	/// Sets the value of the given property for the specified window and all
	/// child windows.
	/// </method>
	/// <param name="objWindow" type="Object">Window to set the property.</param>
	/// <param name="strProperty" type="String">Name of the property to set.</param>
	/// <param name="strValue" type="String">Value to set.</param>
	function SetPropertyRecursive(objWindow, strProperty, strValue)
	{
		if (!objWindow || !objWindow.frames)
			return;

		objWindow[strProperty] = strValue;

		for (var i = 0; i < objWindow.frames.length; ++i)
		{
			SetPropertyRecursive(objWindow.frames[i], strProperty, strValue);
		}
	}


	/// <method>
	/// Recieves the event fired by the representing window.
	/// </method>
	/// <param name="objEvent" type="Object">DOM/Netscape Event object.</param>
	function RecieveWindowEvent(objEvent)
	{
		var eventObject = new JSTools.Web.Window.Event(objEvent);
		var eventName = String.Format(EVENT_NAME_PATTERN, eventObject.GetType());
		_events.Notify(, eventObject);
	}


	/// <method>
	/// Recieves the OnUnload event and closes the dependent windows.
	/// </method>
	/// <param name="objEvent" type="Object">JSTools Event object.</param>
	function CloseDependentWindows(objEvent)
	{
		for (var i = 0; i < _openedWindows.length; ++i)
		{
			if (_openedWindows[i]
				&& !_openedWindows[i].closed
				&& _openedWindows[i].opener == _window
				&& _openedWindows[i].location.host == _window.location.host
				&& _openedWindows[i].GetOptions()
				&& _openedWindows[i].GetOptions().Dependent)
			{
				_openedWindows[i] = _window.open(DEFAULT_WIN_URL, _openedWindows[i].GetOptions().Name);
				_openedWindows[i].close();
			}
		}
	}
	Init();
}


/// <property type="JSTools.Web.Window.Window">
/// Default JSTools.Web.Window.Window instance which represents the global window object.
/// </property>
JSTools.Window = new JSTools.Web.Window.Window(this.top);
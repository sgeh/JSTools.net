namespace("JSTools.Web.Window");


/// <enum>
/// Represents the bars to visualize of a window which should be opened.
/// </enum>
/// <field name="Location">Specifies whether to display the input field for
/// entering URLs directly into the browser.</field>
/// <field name="Menubar">Specifies whether to display the menu bar.</field>
/// <field name="Scroblars">Specifies whether to display horizontal and
/// vertical scroll bars. The default is true.</field>
/// <field name="Status">Specifies whether to add a status bar at the bottom
/// of the window.</field>
/// <field name="Toolbar">Specifies whether to display the browser toolbar,
/// making buttons such as Back, Forward, and Stop available.</field>
JSTools.Web.Window.ToolBars = new JSTools.Enum.FlagsEnum(
	"Location",
	"Menubar",
	"Scroblars",
	"Status",
	"Toolbar" );


/// <class>
/// 
/// </class>
JSTools.Web.Window.WindowOptions = function()
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	this.InitType(arguments, "JSTools.Web.Window.WindowOptions");

	var NAME_VALUE_SEPARATOR = "=";
	var PAIR_SEPARATOR = ",";
	var TRUE_VALUE = "1";
	var FALSE_VALUE = "0";

	var HEIGHT_ATTRIB = "height";
	var WIDTH_ATTRIB = "width";
	var TOP_ATTRIB = "top";
	var LEFT_ATTRIB = "left";
	var NC_TOP_ATTRIB = "screenY";
	var NC_LEFT_ATTRIB = "screenX";

	var NAME_ATTRIB = "name";
	var RESIZABLE_ATTRIB = "resizable";
	var FULLSCREEN_ATTRIB = "fullscreen";

	var _this = this;
	var _values = new JSTools.Util.Hashtable();


	/// <property type="JSTools.Web.Window.ToolBars">
	/// Gets/sets the toolbars of the window to visualize.
	/// </property>
	this.ToolBars = 0x00;


	/// <property type="Boolean">
	/// Specifies whether to display resize handles at the corners of the
	/// window. The default is true.
	/// </property>
	this.Resizable = true;


	/// <property type="Boolean">
	/// Specifies whether to display the browser in full-screen mode. The
	/// default is no.
	/// </property>
	this.Fullscreen = false; 


	/// <property type="Boolean">
	/// Gets/sets whether to close this window when the parent closes.
	/// The dependent window will only close, if the following conditions
	/// are true:
	///
	///  - Child window was not closed yet
	///  - The window which should close the child window must be the
	///    parent window object of the child window (opener).
	///  - The host of the parent and child window must be the same.
	/// </property>
	this.Dependent = false;


	/// <property type="Boolean">
	/// Gets/sets the target window location. The default is "about:blank".
	/// </property>
	this.Url = "about:blank";


	/// <property type="Boolean">
	/// Gets/sets the name of the window. Default is the guid of this type.
	/// This value should contain only alphanumeric characters (a-z A-Z).
	/// </property>
	this.Name = this.GetType().GetGuid().toString("{N}");


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------

	/// <constructor>
	/// Creates a new JSTools.Web.Window.WindowOptions instance.
	/// </constructor>
	function Init()
	{
		_values.Add(HEIGHT_ATTRIB, 100);
		_values.Add(WIDTH_ATTRIB, 140);

		// if the current browser is not netscape 4.x
		if (!JSTools.Browser.IsNS4)
		{
			_values.Add(TOP_ATTRIB, 0);
			_values.Add(LEFT_ATTRIB, 0);
		}
		else
		{
			_values.Add(NC_TOP_ATTRIB, 0);
			_values.Add(NC_LEFT_ATTRIB, 0);
		}
	}


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Gets the height of the window, in pixels. The minimum value
	/// is 100.
	/// </method>
	/// <returns type="Integer">Returns the height of the window, in pixels.</returns>
	function GetHeight()
	{
		return _values[HEIGHT_ATTRIB];
	}
	this.GetHeight = GetHeight;


	/// <method>
	/// Sets the height of the window, in pixels. The minimum value
	/// is 100.
	/// </method>
	/// <param name="intHeight" type="Integer">A number lager or equal than 100.</param>
	function SetHeight(intHeight)
	{
		if (!isNaN(intHeight) && Number(intHeight) >= 100)
			_values[HEIGHT_ATTRIB] = Number(intHeight);
	}
	this.SetHeight = SetHeight;


	/// <method>
	/// Gets the width of the window, in pixels. The minimum value is 140.
	/// </method>
	/// <returns type="Integer">Returns the width of the window, in pixels.</returns>
	function GetWidth()
	{
		return _values[WIDTH_ATTRIB];
	}
	this.GetWidth = GetWidth;


	/// <method>
	/// Sets the width of the window, in pixels. The minimum value is 140.
	/// </method>
	/// <param name="intHeight" type="Integer">A number lager or equal than 140.</param>
	function SetWidth(intWidth)
	{
		if (!isNaN(intWidth) && Number(intWidth) >= 140)
			_values[WIDTH_ATTRIB] = Number(intWidth);
	}
	this.SetWidth = SetWidth;


	/// <method>
	/// Gets the top position, in pixels. This value is relative to
	/// the upper-left corner of the screen. The value must be greater than
	/// or equal to 0. The default is 0.
	/// </method>
	/// <returns type="Integer">Returns the width of the window, in pixels.</returns>
	function GetTop()
	{
		return _values[TOP_ATTRIB];
	}
	this.GetTop = GetTop;


	/// <method>
	/// Sets the top position, in pixels. This value is relative to
	/// the upper-left corner of the screen. The value must be greater than
	/// or equal to 0. The default is 0.
	/// </method>
	/// <param name="intTop" type="Integer">A number lager or equal than 0.</param>
	function SetTop(intTop)
	{
		if (!isNaN(intTop) && Number(intTop) >= 0)
			_values[TOP_ATTRIB] = Number(intTop);
	}
	this.SetTop = SetTop;


	/// <method>
	/// Gets the left position, in pixels. This value is relative to the
	/// upper-left corner of the screen. The value must be greater than or
	/// equal to 0. The default is 0.
	/// </method>
	/// <returns type="Integer">Returns the width of the window, in pixels.</returns>
	function GetLeft()
	{
		return _values[LEFT_ATTRIB];
	}
	this.GetLeft = GetLeft;


	/// <method>
	/// Sets the left position, in pixels. This value is relative to the
	/// upper-left corner of the screen. The value must be greater than or
	/// equal to 0. The default is 0.
	/// </method>
	/// <param name="intLeft" type="Integer">A number lager or equal than 0.</param>
	function SetTop(intLeft)
	{
		if (!isNaN(intLeft) && Number(intLeft) >= 0)
			_values[LEFT_ATTRIB] = Number(intLeft);
	}
	this.SetTop = SetTop;


	/// <method>
	/// Creates a new string which represents the specified values. The returning value
	/// can be passed to the window.open() directive. If the Fullscreen flag is set,
	/// the height, width, top margin, left margin and toolbars values are overriden
	/// with screen height, screen width, top margin 0, left margin 0 and no toolbars.
	/// </method>
	/// <returns type="String">Returns the string representation of this object.</returns>
	function ToString()
	{
		// init fullscreen attribute
		if (_this.Fullscreen)
		{
			_values[HEIGHT_ATTRIB] = screen.height;
			_values[WIDTH_ATTRIB] = screen.width;
			_values[TOP_ATTRIB] = 0;
			_values[LEFT_ATTRIB] = 0;
			_this.ToolBars = 0x00;
		}

		var returnString = String.Empty;

		// init property values (width/height/top/left)
		for (var item in _values)
		{
			returnString += GetValuePair(item, _values[item]);
		}

		returnString += GetValuePair(RESIZABLE_ATTRIB, GetBooleanValue(_this.Resizable));
		returnString += GetValuePair(FULLSCREEN_ATTRIB, GetBooleanValue(_this.Fullscreen));
		returnString += GetToolbarFlags();

		return GetValidStringEnding(returnString);
	}
	this.toString = ToString;


	/// <method>
	/// Cuts the last pair separator.
	/// </method>
	/// <param name="strToCheck" type="String">String to cut.</param>
	/// <returns type="String">Returns the string without an ending pair separator.</returns>
	function GetValidStringEnding(strToCheck)
	{
		if (returnString.length > 0)
			return returnString.substring(0, returnString.length - 1);
		else
			return String.Empty;
	}


	/// <method>
	/// Gets a string which represents the ToolBars property value.
	/// </method>
	/// <returns type="String">Returns a string which represents the ToolBars property value.</returns>
	function GetToolbarFlags()
	{
		// init toolbar flags
		var returnString = String.Empty;
		var toolbarNames = JSTools.Web.Window.ToolBars.GetNames();

		for (var i = 0; i < toolbarNames.length; ++i)
		{
			var boolValue = ((_this.ToolBars & JSTools.Web.Window.ToolBars[toolbarNames[i]]) != 0);
			returnString += GetValuePair(toolbarNames[i], GetBooleanValue(boolValue));
		}
		return returnString;
	}


	/// <method>
	/// Gets a valid name/value pair string of the given values.
	/// </method>
	/// <param name="strName" type="String">Name of the name/value pair.</param>
	/// <param name="strValue" type="String">Value of the name/value pair.</param>
	/// <returns type="String">Returns a string which represents the given boolean.</returns>
	function GetValuePair(strName, strValue)
	{
		return strName + NAME_VALUE_SEPARATOR + strValue + PAIR_SEPARATOR;
	}


	/// <method>
	/// Converts the given boolean in a string.
	/// </method>
	/// <param name="blnToGet" type="Boolean">Boolean to convert.</param>
	/// <returns type="String">Returns a string which represents the given boolean.</returns>
	function GetBooleanValue(blnToGet)
	{
		return (blnToGet) ? TRUE_VALUE : FALSE_VALUE;
	}
	Init();
}
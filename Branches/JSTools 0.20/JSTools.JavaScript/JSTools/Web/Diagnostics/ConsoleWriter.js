function ConsoleWriter()
{
	var _this			= this;
	var _windowContent	= "";
	var _window			= null;

	var _selectedType	= null;
	var _entries		= new Array();
	var _entryTypes		= ConsoleWriter.MessageTypes.GetValues();

	for (var i = 0; i < _entryTypes.length; ++i)
	{
		_entries[_entryTypes[i]] = new Array();
	}


	//style definition
	var _styles							= new ConsoleStyle();
	_styles.WindowTitle					= "JavaScript Console Window";
	_styles.WindowOptions				= new WindowOptions();
	_styles.WindowOptions.IsDependent	= true;
	_styles.WindowOptions.Width			= 600;
	_styles.WindowOptions.Height		= 430;


	// prints a new message to the console and refreshes the console
	this.Write = function(strMessage)
	{
		if (CanMessageAppend())
		{
			_entries.GetLast().AppendMessage(strMessage);
		}
		else
		{
			CreateMessageEntry(ConsoleWriter.MessageTypes.Message, strMessage);
		}
		_this.Refresh();
	}


	// prints a new message to the console and adds a new line to it
	this.Writeln = function(strMessage)
	{
		_this.Write(strMessage + "\n");
	}


	// evaluates the given string and writes the result into the console
	this.EvaluateAndWrite = function(strCode)
	{
		var result = window.eval(strCode);
		_this.Writeln("[console result] " + result);
	}


	// writes the given message as warning into the console
	this.WriteWarning = function(strMessage)
	{
		var newEntry = new ConsoleMessage(ConsoleWriter.MessageTypes.Warning, strMessage + "\n");
		AddEntry(newEntry, ConsoleWriter.MessageTypes.Warning);
		_this.Refresh();
	}


	// visualizes the console to the user
	this.Show = function()
	{
		if (_window == null || !_window.IsAccessible())
		{
			_window = new WindowOpener(_styles.WindowOptions);
			WriteWindowContent();
		}
		else
		{
			_window.GetWindowObject().focus();
		}
	}


	// hides the console from the user
	this.Hide = function()
	{
		if (_window != null)
		{
			_window.Close();
			_window = null;
		}
	}


	// returns the array, which contains all messages
	this.GetMessages = function()
	{
		return _entries[ConsoleWriter.MessageTypes.Message];
	}


	// returns the array, which contains all warnings
	this.GetWarnings = function()
	{
		return _entries[ConsoleWriter.MessageTypes.Warning];
	}


	// returns the array, which contains all errors
	this.GetErrors = function()
	{
		return _entries[ConsoleWriter.MessageTypes.Error];
	}


	// refreshes the console window and rewrites the console entries.
	this.Refresh = function(strMessageType)
	{
		if (ConsoleWriter.MessageTypes.Contains(strMessageType))
		{
			_selectedType = strMessageType;
			RefreshType(strMessageType);
			WriteTextArea();
		}
		else
		{
			RefreshSelectedType();
		}
	}


	// refreshes to content of all entries and writes it into the area
	this.RefreshAll = function(blnIsWritten)
	{
		_selectedType = null;
		RefreshAll();
		WriteTextArea();
	}



	// returns true, if the last entry in the entries array contains a "Message" object which
	// does not hold a carriage return at the end of the line. Otherwise false.
	function CanMessageAppend()
	{
		return (_entries.GetLast() != null && _entries.GetLast().GetMessageType() == ConsoleWriter.MessageTypes.Message && _entries.GetLast().GetMessage().lastIndexOf("\n") != _entries.GetLast().GetMessage().length - 1);
	}


	// creates a new ConsoleMessage instance
	function CreateMessageEntry(strMessageType, strMessage)
	{
		var newEntry = new ConsoleMessage(strMessageType, strMessage);
		AddEntry(newEntry, strMessageType);
	}


	// adds the given console message instance to the given message type entry collection
	function AddEntry(objEntry, strMessageType)
	{
		_entries.Add(objEntry);
		_entries[strMessageType].Add(objEntry);
	}


	// creates a new ConsoleErrorMessage instance. reports a script error and writes it into the console
	function ReportError(objErrorArgument)
	{
		if (Exception.CanLog())
		{
			var newEntry = new ConsoleErrorMessage(objErrorArgument.GetMessage(), objErrorArgument.GetUrl(), objErrorArgument.GetLine());
			AddEntry(newEntry, ConsoleWriter.MessageTypes.Error);

			_this.Refresh();
		}
		objErrorArgument.SetReturnValue(Exception.CanCatch());
	}


	// calls the ToLineString method of all entries and fills it into the global window content variable
	function RefreshAll()
	{
		_windowContent	= "";

		for (var i = 0; i < _entries.length; ++i)
		{
			_windowContent += _entries[i].ToLineString();
		}
	}


	// if a type is pre selected, this function will reload only the pre selected, otherwise all values will be refreshed
	function RefreshSelectedType()
	{
		if (_selectedType != null)
		{
			RefreshType(_selectedType);
			WriteTextArea();
		}
		else
		{
			_this.RefreshAll(true);
		}
	}


	// calls the ToLineString method of the specific message type entries and fills it into the global window content variable
	function RefreshType(strMessageType)
	{
		_windowContent	= "";

		for (var i = 0; i < _entries[strMessageType].length; ++i)
		{
			_windowContent += _entries[strMessageType][i].ToLineString();
		}
	}


	// writes the window content into the debug text area
	function WriteTextArea()
	{
		if (HasValidTextArea())
		{
			_window.GetWindowObject().Console.value = _windowContent;
		}
	}


	// returns true, if the text area in the child window object is available and ready
	function HasValidTextArea()
	{
		return (_window != null && _window.IsAccessible() && _window.GetWindowObject().Console);
	}


	// designs the content for the child window
	function WriteWindowContent()
	{
		var bodyContent = "<p><h1>Debugging Console</h1></p>" +
			"<form name='consoleForm'>" +
			"<p><textarea id='consoleText' name='consoleText' cols='70' rows='16'></textarea></p>" +
			"<center>" +
			"<input type='text' size='50' value='...add value to console...' name='consoleAdd'>&nbsp;" +
			"<input type='button' value='evaluate' onclick='OnEvalClick()'>&nbsp;" +
			"<select onchange='OnFilterChange(this)'>" +
			"<option>all</option>";

		for (var i = 0; i < _entryTypes.length; ++i)
		{
			bodyContent += "<option value='" + _entryTypes[i] + "'>" + _entryTypes[i] + "</option>";
		}

		bodyContent += "</select>" +
			"</center>" +
			"</form>" +
			"<script language='javascript'>" +

			"function HasParentWindow() {" +
			"return (window.opener && !window.opener.closed); }" +

			"function OnEvalClick() {" +
			"if(HasParentWindow())" +
			"window.opener.Console.EvaluateAndWrite(document.forms.consoleForm.consoleAdd.value); }" +

			"function OnFilterChange(objSelect) {" +
			"var selectedValue = objSelect[objSelect.selectedIndex].value;" +
			"if(HasParentWindow()) {" +
			"if (!selectedValue) {" +
			"window.opener.Console.RefreshAll(); }" +
			"else {" +
			"window.opener.Console.Refresh(objSelect[objSelect.selectedIndex].value); }}}" +

			"window.onerror = window.opener.onerror;" +
			"window.Console = document.forms.consoleForm.consoleText;" +
			"</script>";

		_window.SetContent(_styles.Render(bodyContent), true, true);
		_this.Refresh();
	}
	window.EventHandler.AddErrorListener(ReportError);
}
ConsoleWriter.prototype.toString = function()
{
	return "[object ConsoleWriter]";
}
ConsoleWriter.MessageTypes		= new StringEnum(
	"Error",				// specifies error messages; the parser can throw it
	"Warning",				// specifies warning messages; the JSTools.net throws exceptions which are logged as warnings
	"Message" );			// specifies normal console messages; the user can write and append messages
window.Console = new ConsoleWriter();
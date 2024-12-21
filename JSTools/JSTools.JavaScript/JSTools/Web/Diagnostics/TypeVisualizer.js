function ReflectionGenerator(objObject)
{
	var _tableGenerator = new HtmlTableGenerator();
	var _allOperations = new Array();
	var _allProperties = new Array();

	var _refObject = objObject;
	var _itemIndex = 0;
	var _debugWindow = null;

	//style definition
	var _styles = new ConsoleStyle();
	_styles.WindowTitle = "Reflection Window";
	_styles.WindowOptions.Width = 600;
	_styles.WindowOptions.Height = 400;



	// returns an object which stores all property and operation members
	this.GetMembers = function()
	{
		return { Property: _allProperties, Method: _allOperations };
	}



	// sets all members of _refObject into the right array
	function GetObjectMembers()
	{
		SortNamedObjectMembers();
		SortIndexedObjectMembers();

		OpenNewReflectionWindow();
		WriteWindowContent();
	}


	// opens a new reflection window
	function OpenNewReflectionWindow()
	{
		_debugWindow = new WindowOpener(_style.WindowOptions);
	}


	// sorts all named members into the arrays
	function SortNamedObjectMembers()
	{
		for(var objItem in _refObject)
		{
			AddMemberToContainer(objItem);
			_itemIndex++;
		}
	}


	// sorts all indexed members into the arrays
	function SortIndexedObjectMembers()
	{
		if(!IsUndefined(_refObject.length))
		{
			for(var i = 0; i < _refObject.length; ++i)
			{
				AddMemberToContainer(i);
			}
			_itemIndex++;
		}
	}


	// adds a member to the right container
	function AddMemberToContainer(strMemberName)
	{
		if(IsValidObjectMember(strMemberName))
		{
            if(_refObject[strMemberName] && (_refObject[strMemberName].IsFunction || !IsUndefined(_refObject[strMemberName].arguments)))
            {
                _allOperations.Add(GetContainerObject(strMemberName));
            }
            else
            {
                _allProperties.Add(GetContainerObject(strMemberName));
            }
        }
	}


	// returns true, if strMember is a valid object
	function IsValidObjectMember(strMember)
	{
		if(window.Browser.GetType().IsIE && window.Browser.GetVersion() >= 5)
		{
			var containerItem = true;
			var evalString = "try {";
			evalString += "_refObject[strMember]; }";
			evalString += "catch(e) {";
			evalString += "containerItem = false; }";
			eval(evalString);
			return containerItem;
		}
		return true;
	}

	// returns an object which will stored into the container
	function GetContainerObject(strName)
	{
		return { object: strName, index: _itemIndex };
	}


	// writes the generated html into the window object
	function WriteWindowContent()
	{
		CreateTable();
		_debugWindow.SetContent("<html>" + _style.Render() + "<h1>Public Member Reflection of " + _refObject.toString() + ":</h1>" + _tableGenerator.Render(), true, true);
		_tableGenerator.Delete();
	}


	// returns the operation member html
	function CreateTable()
	{
		with (_tableGenerator)
		{
			AddAttribute("border", 0);
			AddAttribute("cellspacing", 0);
			AddAttribute("cellpadding", 0);
		}

		SetOperationValues();
		SetPropertyValues();
	}


	// creates the header row of the table
	function CreateOperationHeaderRow()
	{
		var topRow = _tableGenerator.CreateRow();

		with (topRow.CreateColumn())
		{
			AddAttribute("colspan", 4);
			SetValue = "<h2>All Methods:</h2>";
		}
	}


	// creates the title row of the table
	function CreateOperationTitleRow()
	{
		var titleRow = _tableGenerator.CreateRow();

		with (titleRow.CreateColumn())
		{
			AddAttribute("width", 60);
			SetValue("Index");
		}

		with (titleRow.CreateColumn())
		{
			AddAttribute("colspan", 2);
			AddAttribute("width", 700);
			SetValue("Name");
		}
	}


	// sets the operation values
	function SetOperationValues()
	{
		CreateOperationHeaderRow();
		CreateOperationTitleRow();

		for (var i = 0; i < _allOperations.length; ++i)
		{
			var contentRow = _tableGenerator.CreateRow();

			with (contentRow.CreateColumn())
			{
				AddAttribute("bgcolor", GetTableColor(i));
				SetValue(_allOperations[i].index);
			}

			with (contentRow.CreateColumn())
			{
				AddAttribute("colspan", 2);
				AddAttribute("bgcolor", GetTableColor(i));
				SetValue(_allOperations[i].object + "( [ arguments ] )");
			}
		}
	}


	// creates the header row of the table
	function CreatePropertyHeaderRow()
	{
		var topRow = _tableGenerator.CreateRow();

		with (topRow.CreateColumn())
		{
			AddAttribute("colspan", 4);
			SetValue("<br><h2>All Properties:</h2>");
		}
	}


	// creates the title row of the table
	function CreatePropertyTitleRow()
	{
		with (_tableGenerator.CreateRow())
		{
			CreateColumn().SetValue("Index");
			CreateColumn().SetValue("Name");
			CreateColumn().SetValue("Value");
		}
	}


	// sets the property values
	function SetPropertyValues()
	{
		CreatePropertyHeaderRow();
		CreatePropertyTitleRow();

		for(var i = 0; i < _allProperties.length; ++i)
		{
			var contentRow = _tableGenerator.CreateRow();
			var bgColor = GetTableColor(i);

			with (contentRow.CreateColumn())
			{
				AddAttribute("bgcolor", bgColor);
				SetValue(_allProperties[i].index);
			}

			with (contentRow.CreateColumn())
			{
				AddAttribute("bgcolor", bgColor);
				SetValue(_allProperties[i].object);
			}

			with (contentRow.CreateColumn())
			{
				AddAttribute("bgcolor", bgColor);
				SetValue(_refObject[_allProperties[i].object] + "&nbsp;");
			}
		}
	}


	// returns the table color
	function GetTableColor(intIndex)
	{
		return ((intIndex % 2 == 0) ? _style.TableColor : _style.WindowColor);
	}


	// initializes the reflection
	if(_refObject)
	{
		GetObjectMembers();
	}
}
ReflectionGenerator.prototype.toString = function()
{
	return "[object ReflectionGenerator]";
}
function HtmlTableGenerator(intRows, intColumns)
{
	// init type manager, required for inheritance
	this.InitTypeManager(arguments);

	// inherit this object from other classes
	arguments.Call(BaseTableElement);

	// copies a reference of the protected members for using from outside the constructor
	var _protected	= arguments.Protected;
	var _tableRows	= new Array();


	// returns the row collection
	this.GetRows = function()
	{
		return _tableRows;
	}


	// creates a new row object
	this.CreateRow = function()
	{
		var rowObject = new HtmlTableRow(_tableRows.length, this);
		_tableRows.Add(rowObject);
		return rowObject;
	}


	// creates a new column in the specified row object
	this.CreateColumn = function(objRow)
	{
		if (!IsUndefined(objRow) && objRow.CreateColumn)
		{
			return objRow.CreateColumn();
		}
		return null;
	}


	// resizes the current table
	this.ResizeTo = function(intRows, intColumns)
	{
		if (!IsUndefined(intRows) && !IsUndefined(intColumns))
		{
			for (var i = 0; i < intRows || i < _tableRows.length; ++i)
			{
				if (i < intRows)
				{
					if (IsUndefined(_tableRows[i]))
					{
						var row = this.CreateRow();
						row.ResizeTo(intColumns);
					}
				}
				else
				{
					_tableRows[i].Delete();
					_tableRows.Remove(i);
				}
			}
		}
	}


	// renders all values to a html column
	this.Render = function()
	{
		return "<table" + this.RenderAttributes() + ">" + CallRowMethod("Render") + "</table>";
	}


	// overwrites all attributes with an empty string
	this.Clean = function()
	{
		CallRowMethod("Clean");
		this.CleanAttributes();
	}


	// deletes all attribute values and removes all rows
	this.Delete = function()
	{
		this.ResizeTo(0, 0);
		this.DeleteAttributes();
	}


	// calls the "strMethodName" method and returns the called return values in a line separated string
	function CallRowMethod(strMethodName)
	{
		var callReturnValue = "";

		for (var i = 0; i < _tableRows.length; ++i)
		{
			callReturnValue += ToString(_tableRows[i][strMethodName]()) + "\n";
		}
		return callReturnValue;
	}

	this.ResizeTo(intRows, intColumns);
}
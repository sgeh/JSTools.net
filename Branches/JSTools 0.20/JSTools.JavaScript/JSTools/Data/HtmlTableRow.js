function HtmlTableRow(intIndex, objParentTable)
{
	// init type manager, required for inheritance
	this.InitTypeManager(arguments);

	// inherit this object from other classes
	arguments.Call(BaseTableElement);

	// copies a reference of the protected members for using from outside the constructor
	var _protected		= arguments.Protected;
	var _tableIndex		= (!IsUndefined(intIndex) ? ToNumber(intIndex) : -1);
	var _columns		= new Array();
	var _parentTable	= (!IsUndefined(objParentTable) ? objParentTable : null);



	// returns the parent table object
	this.GetParentTable = function()
	{
		return _parentTable;
	}


	// returns the index of the row in the data table
	this.GetIndex = function()
	{
		return _tableIndex;
	}


	// returns the column container
	this.GetColumns = function()
	{
		return _columns;
	}


	// creates a new column object
	this.CreateColumn = function()
	{
		var columnObject = new HtmlTableColumn(_columns.length, this);
		_columns.Add(columnObject);
		return columnObject;
	}


	// renders all values to a html column
	this.Render = function()
	{
		return "<tr" + this.RenderAttributes() + ">" + CallColumnMethod("Render") + "</tr>";
	}


	// overwrites all attributes with an empty string
	this.Clean = function()
	{
		CallColumnMethod("Clean");
		this.CleanAttributes();
	}


	// deletes all attribute values
	this.Delete = function()
	{
		this.ResizeTo(0);
		this.DeleteAttributes();
	}


	// resizes the _columns collection to the specified number of columns
	this.ResizeTo = function(intColumns)
	{
		if (IsNumeric(intColumns))
		{
			for (var i = 0; i < intColumns || i < _columns.length; ++i)
			{
				if (i < intColumns)
				{
					if (IsUndefined(_columns[i]))
					{
						this.CreateColumn();
					}
				}
				else
				{
					_columns[i].Delete();
					_columns.Remove(i);
				}
			}
		}
	}



	// calls the strMethodName method and returns the called return values in a line separated string
	function CallColumnMethod(strMethodName)
	{
		var callReturnValue = "";

		for (var i = 0; i < _columns.length; ++i)
		{
			callReturnValue += ToString(_columns[i][strMethodName]()) + "\n";
		}
		return callReturnValue;
	}
}
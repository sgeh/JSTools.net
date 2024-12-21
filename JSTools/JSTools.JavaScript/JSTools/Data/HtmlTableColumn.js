function HtmlTableColumn(intIndex, objRow)
{
	// init type manager, required for inheritance
	this.InitTypeManager(arguments);

	// inherit this object from other classes
	arguments.Call(BaseTableElement);

	// copies a reference of the protected members for using from outside the constructor
	var _protected		= arguments.Protected;
	var _rowIndex		= (!IsUndefined(intIndex) ? ToNumber(intIndex) : -1);
	var _columns		= new Array();
	var _columnValue	= "";
	var _parentRow		= (!IsUndefined(objRow) ? objRow : null);



	// returns the parent row object
	this.GetParentRow = function()
	{
		return _parentRow;
	}


	// returns the index of the column in the parent row container
	this.GetIndex = function()
	{
		return _rowIndex;
	}


	// sets a column value
	this.SetValue = function(strValue)
	{
		_columnValue = ToString(strValue);
	}


	// renders all values to a html column
	this.Render = function()
	{
		return "<td" + this.RenderAttributes() + ">" + _columnValue + "</td>";
	}


	// overwrites all attributes and the column value with a empty string
	this.Clean = function()
	{
		_columnValue = "";
		this.CleanAttributes();
	}


	// deletes all attribute values
	this.Delete = function()
	{
		_columnValue = null;
		this.DeleteAttributes();
	}
}
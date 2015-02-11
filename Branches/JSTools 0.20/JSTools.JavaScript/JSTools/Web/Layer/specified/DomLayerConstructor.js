function DomLayerConstructor(objDelegate)
{
	// init type manager, required for inheritance
	this.InitTypeManager(arguments);

	// inherit this object from other classes
	arguments.Call(BaseLayerConstructor);

	// copies a reference of the protected members for using from outside the constructor
	var _protected		= arguments.Protected;
	var _modelObject	= objDelegate;
	var _this			= this;



	// constructs the layer and searches the layer object, fires the "init" event
	this.Construct = function()
	{
		if (!IsLayerAvailable())
		{
			AddNewLayerToDocument();
		}
		else
		{
		}
	}


	// initializes the layer object and dispatches the "initComponent" event
	this.Init = function()
	{

	}



	// adds a new layer object to the document
	function AddNewLayerToDocument()
	{

	}


	// returns true, if the layer object was found
	function IsLayerAvailable()
	{
		return Boolean(GetLayerObject());
	}


	// returns the layer, which id was specified in the LayerHandler object
	function GetLayerObject()
	{
		return document.getElementById(_modelObject.LayerHandler.GetID());
	}
}
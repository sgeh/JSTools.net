function IeLayerGetter(objDelegate)
{
	// init type manager, required for inheritance
	this.InitTypeManager(arguments);

	// inherit this object from other classes
	arguments.Call(DomLayerGetter, objDelegate);

	// copies a reference of the protected members for using from outside the constructor
	var _protected		= arguments.Protected;
	var _delegateObject	= objDelegate;


	// returns all elements with the specified tag name in a new Array, [override]
	this.GetHtmlComponent = function(strTagName)
	{
		return _delegateObject.LayerObject.document.all.tags(strTagName);
	}
}
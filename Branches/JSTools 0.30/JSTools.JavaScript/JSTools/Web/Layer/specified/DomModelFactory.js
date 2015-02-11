function DomModelFactory(objLayerHandler)
{
	// init type manager, required for inheritance
	this.InitTypeManager(arguments);

	// inherit this object from other classes
	arguments.Call(BaseModelFactory);

	// copies a reference of the protected members for using from outside the constructor
	var _protected		= arguments.Protected;
	this.LayerHandler	= objLayerHandler;
	this.Constructor	= new DomLayerConstructor(this);
}
LayerHandler.ModelFactories.Add(new LayerFactoryDescription(DomModelFactory, window.Browser.IsDOM()));
function IeModelFactory(objLayerHandler)
{
	// init type manager, required for inheritance
	this.InitTypeManager(arguments);

	// inherit this object from other classes
	arguments.Call(BaseModelFactory);

	// copies a reference of the protected members for using from outside the constructor
	var _protected = arguments.Protected;

	this.LayerHandler = objLayerHandler;
	this.Constructor = new IeLayerConstructor(this);

	this.ValueGetter = new IeLayerGetter(this);
	this.ValueSetter = new IeLayerSetter(this);
}
LayerHandler.ModelFactories.Add(new LayerFactoryDescription(IeModelFactory, window.Browser.GetType().IsIE4));
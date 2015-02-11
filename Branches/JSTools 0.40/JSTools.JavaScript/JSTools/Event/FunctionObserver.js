namespace("JSTools.Event");


/// <class>
/// Represents a function observer.
/// </class>
/// <param name="objFunction" type="Function">Function to call.</param>
JSTools.Event.FunctionObserver = function(objFunction)
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	this.InitType(arguments, "JSTools.Event.FunctionObserver");
	this.Inherit(JSTools.Event.IObserver);
	
	var _this = this;
	var _functionToNotify = objFunction;


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Notifies the observer about an update.
	/// </method>
	/// <param name="objEvent" type="Object">An object instance, which represents the event argument.</param>
	function Update(objEvent)
	{
		Function.Call(_functionToNotify, objEvent);
	}
	this.Update = Update;
}

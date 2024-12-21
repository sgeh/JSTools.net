namespace("JSTools.Event");


/// <class>
/// Represents a JSTools.Reflection.MethodInfo observer. Do not create observers for private
/// methods. Inner scopes can't be invoked.
/// </class>
/// <param name="objMethodInfo" type="JSTools.Reflection.MethodInfo">Function info object to call.</param>
JSTools.Event.MethodInfoObserver = function(objMethodInfo)
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	this.InitType(arguments, "JSTools.Event.MethodInfoObserver");
	this.Inherit(JSTools.Event.IObserver);
	
	var _this				= this;
	var _methodInfoToNotify = objMethodInfo;


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Notifies the observer about an update.
	/// </method>
	/// <param name="objEvent" type="Object">An object instance, which represents the event argument.</param>
	this.Update = function(objEvent)
	{
		if (_methodInfoToNotify && typeof(_methodInfoToNotify) == 'object' && _methodInfoToNotify.Invoke)
		{
			_methodInfoToNotify.Invoke(objEvent);
		}
	}
}

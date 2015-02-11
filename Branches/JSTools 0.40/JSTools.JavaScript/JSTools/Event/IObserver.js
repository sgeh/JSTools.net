namespace("JSTools.Event");


/// <interface>
/// Defines an updating interface for objects that should be notified of changes in 
/// a subject.
/// </interface>
JSTools.Event.IObserver = function()
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	this.InitType(arguments, "JSTools.Event.IObserver");


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Notifies the observer about an update.
	/// </method>
	/// <param name="objEvent" type="Object">An object instance, which represents the event argument.</param>
	this.Update = function(objEvent) { }
}

namespace("JSTools.Timers");


/// <class>
/// A Timeout is used to raise one tick event at an user-defined timeout.
///
/// There are two different methods, which attach a function object
/// to the timeout event.
///
/// AddTimeoutMethodInfo
///  Attaches a method to the timeout event. The call will be executed in
///  the creation context (this pointer) of the object.
/// AddTimeoutFunction
///  Attaches a method to the timeout event. The call will be executed in
///  the global context (this pointer).
/// </class>
JSTools.Timers.Timeout = function()
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	this.InitType(arguments, "JSTools.Timers.Timeout");

	var DEFAULT_TIMEOUT		= 20;

	var _this				= this;
	var _timer				= new JSTools.Timers.Timer();


	/// <property type="Integer">
	/// Gets or sets the timeout (milliseconds) at which to call the Timeout event. Default is 20ms.
	/// </property>
	this.Timeout			= DEFAULT_TIMEOUT;


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------

	/// <constructor>
	/// Creates a new JSTools.Timers.Timeout instance.
	/// </constructor>
	function Init()
	{
		_this.AddTimeoutFunction(AutoStop);
	}


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// The returning array is passed as event argument to the tick method. This values are
	/// shared between your code and the reciever functions.
	/// </method>
	/// <returns type="Array">Returns an array, which is passed as event argument to the tick method.</returns>
	this.GetArguments = function()
	{
		return _timer.GetArguments();
	}


	/// <method>
	/// Attaches the given method to this timer. It will be called by each tick.
	/// </method>
	/// <param name="objMethodInfoToAdd" type="JSTools.Reflection.MethodInfo">Attaches the given Function or MethodInfo object.
	/// The given method will be called by each tick.</param>
	this.AddTimeoutMethodInfo = function(objMethodInfoToAdd)
	{
		return _timer.AddTickMethodInfo(objMethodInfoToAdd);
	}


	/// <method>
	/// Attaches the given method to this timer. It will be called by each tick.
	/// </method>
	/// <param name="objFunction" type="Function">Attaches the given Function or MethodInfo object.
	/// The given method will be called by each tick.</param>
	this.AddTimeoutFunction = function(objFunction)
	{
		return _timer.AddTickFunction(objFunction);
	}


	/// <method>
	/// Returns true, if the timer is enabled.
	/// </method>
	this.IsEnabled = function()
	{
		return _timer.IsEnabled();
	}


	/// <method>
	/// Stops the timer interval and sets the IsEnabled() flag to false.
	/// </method>
	this.Stop = function()
	{
		_timer.Stop();
	}


	/// <method>
	/// Starts the timer interval and sets the IsEnabled() flag to true. If the 
	/// </method>
	this.Start = function()
	{
		_timer.Interval = !isNaN(_this.Timeout) ? Number(_this.Timeout) : DEFAULT_TIMEOUT;
		_timer.Start();
	}


	/// <method>
	/// Stops the interval after the first tick event.
	/// </method>
	function AutoStop()
	{
		_timer.Stop();
	}
	Init();
}

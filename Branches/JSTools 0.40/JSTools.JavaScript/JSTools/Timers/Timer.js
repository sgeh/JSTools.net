namespace("JSTools.Timers");


/// <class>
/// A Timer is used to raise a tick event at user-defined intervals.
///
/// AddTickMethodInfo
///  Attaches a method to the tick event. The call will be executed in
///  the creation context (this pointer) of the object.
/// AddTickFunction
///  Attaches a method to the tick event. The call will be executed in
///  the global context (this pointer).
/// </class>
JSTools.Timers.Timer = function()
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	this.InitType(arguments, "JSTools.Timers.Timer");

	var DEFAULT_INTERVAL = 20;
	var SUBJECT_LIST_NAME = "ontick";

	var _this = this;
	var _timer = 0;
	var _isEnabled = false;
	var _tickEvents = new JSTools.Event.SubjectList();
	var _arguments = [ ];


	/// <property type="Integer">
	/// Gets or sets the interval (milliseconds) at which to raise the Tick event. Default is 20ms.
	/// </property>
	this.Interval = 0;


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------

	/// <constructor>
	/// Creates a new JSTools.Timers.Timer instance.
	/// </constructor>
	function Init()
	{
		_this.Interval = DEFAULT_INTERVAL;
	}


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// The returning array is passed as event argument to the tick method. This values are
	/// shared between your code and the reciever functions.
	/// </method>
	/// <returns type="Array">Returns an array, which is passed as event argument to the tick method.</returns>
	function GetArguments()
	{
		return _arguments;
	}
	this.GetArguments = GetArguments;


	/// <method>
	/// Attaches the given method to this timer. It will be called by each tick.
	/// </method>
	/// <param name="objMethodInfoToAdd" type="JSTools.Reflection.MethodInfo">Attaches the given Function or MethodInfo object.
	/// The given method will be called by each tick.</param>
	/// <returns type="Integer">Returns the index, at which the observer object has been added.
	/// Returns -1 if the given observer object is invalid and has not be added.</returns>
	function AddTickMethodInfo(objMethodInfoToAdd)
	{
		return _tickEvents.AttachMethodInfo(SUBJECT_LIST_NAME, objMethodInfoToAdd);
	}
	this.AddTickMethodInfo = AddTickMethodInfo;


	/// <method>
	/// Attaches the given method to this timer. It will be called by each tick.
	/// </method>
	/// <param name="objFunction" type="Function">Attaches the given Function or MethodInfo object.
	/// The given method will be called by each tick.</param>
	/// <returns type="Integer">Returns the index, at which the observer object has been added.
	/// Returns -1 if the given observer object is invalid and has not be added.</returns>
	function AddTickFunction(objFunction)
	{
		return _tickEvents.AttachFunction(SUBJECT_LIST_NAME, objFunction);
	}
	this.AddTickFunction = AddTickFunction;


	/// <method>
	/// Returns true, if the timer is enabled.
	/// </method>
	function IsEnabled()
	{
		return _isEnabled;
	}
	this.IsEnabled = IsEnabled;


	/// <method>
	/// Stops the timer interval and sets the IsEnabled() flag to false.
	/// </method>
	function Stop()
	{
		_isEnabled = false;
		
		if (_timer)
		{
			clearInterval(_timer);
			_timer = 0;
		}
	}
	this.Stop = Stop;


	/// <method>
	/// Starts the timer interval and sets the IsEnabled() flag to true.
	/// </method>
	function Start()
	{
		if (_isEnabled && _timer)
			_this.Stop();

		// reset timer, if it is invalid
		if (isNaN(_this.Interval))
			_this.Interval = DEFAULT_INTERVAL;

		_isEnabled = true;
		_timer = setInterval(Run, _this.Interval);
	}
	this.Start = Start;
	
	
	/// <method>
	/// Triggers the tick event. This method is called by the javascript interval.
	/// </method>
	function Run()
	{
		_tickEvents.Notify(SUBJECT_LIST_NAME, _arguments);
	}
}

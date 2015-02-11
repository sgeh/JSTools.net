namespace("JSTools.ExceptionHandling");


/// <class>
/// Stores and handles the error, log and warn events. To attach events, you can use
/// AddOn...Event(...) procedures. To remove events, use the corresponding
/// RemoveOn...Event(...) method.
/// </class>
JSTools.ExceptionHandling.Handler = function()
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	this.InitType(arguments, "JSTools.ExceptionHandling.Handler");

	var LOG_EVENT_NAME		= "onlog";
	var ERROR_EVENT_NAME	= "onerror";
	var WARN_EVENT_NAME		= "onwarn";

	var _this				= this;
	var _events				= new JSTools.Event.SubjectList();

	var _logs				= [ ];
	var _errors				= [ ];
	var _warnings			= [ ];


	/// <property type="JSTools.ExceptionHandling.ErrorEvent">
	/// Represents a bit mask used to filter the events.
	/// </property>
	this.EventHandling	= JSTools.ExceptionHandling.ErrorEvent.All;


	/// <property type="JSTools.ExceptionHandling.ErrorHandling">
	/// Determines wheter the exception should be thrown or catched.
	/// If the exception should be thrown, the public Log, Thorw and Warn
	/// methods will return false, otherwise true.
	/// </property>
	this.ErrorHandling	= JSTools.ExceptionHandling.ErrorHandling.Throw;


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------

	/// <constructor>
	/// Creates a new JSTools.ExceptionHandling.Handler instance.
	/// </constructor>
	function Init()
	{
	}


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Creates a new Array instance, which contains the logging messages.
	/// </method>
	/// <returns type="Array">Returns an array which contains the given log
	/// (JSTools.ExceptionHandling.Exception) instances.</returns>
	this.GetLogs = function()
	{
		return _logs.Copy();
	}


	/// <method>
	/// Creates a new Array instance, which contains the error messages.
	/// </method>
	/// <returns type="Array">Returns an array which contains the given error
	/// (JSTools.ExceptionHandling.Exception) instances.</returns>
	this.GetErrors = function()
	{
		return _errors.Copy();
	}


	/// <method>
	/// Creates a new Array instance, which contains the warning messages.
	/// </method>
	/// <returns type="Array">Returns an array which contains the given warning
	/// (JSTools.ExceptionHandling.Exception) instances.</returns>
	this.GetWarnings = function()
	{
		return _warnings.Copy();
	}


	/// <method>
	/// Adds a new observer to the OnLog subject (event).
	/// </method>
	/// <param name="varLogObserver" type="Function">Adds the given function object to the OnLog event.</param>
	/// <param name="varLogObserver" type="JSTools.Reflection.MethodInfo">Adds the given MethodInfo object to the OnLog event.</param>
	/// <param name="varLogObserver" type="JSTools.Event.IObserver">Adds the given IObserver object to the OnLog event.</param>
	/// <returns type="Integer">Returns the index, at which the observer object has been added.
	/// Returns -1 if the given observer object is invalid and not added.</returns>
	this.AddOnLogEvent = function(varLogObserver)
	{
		_events.Attach(LOG_EVENT_NAME, varLogObserver);
	}


	/// <method>
	/// Removes an observer from the OnLog subject (event).
	/// </method>
	/// <param name="varObserverToDetach" type="JSTools.Event.IObserver">Observer object which should be removed.</param>
	/// <param name="varObserverToDetach" type="Integer">Internal index of the observer object which should be removed.</param>
	this.RemoveOnLogEvent = function(varObserverToDetach)
	{
		_events.Detach(LOG_EVENT_NAME, varObserverToDetach);
	}


	/// <method>
	/// Adds a new observer to the OnError subject (event).
	/// </method>
	/// <param name="varLogObserver" type="Function">Adds the given function object to the OnError event.</param>
	/// <param name="varLogObserver" type="JSTools.Reflection.MethodInfo">Adds the given MethodInfo object to the OnError event.</param>
	/// <param name="varLogObserver" type="JSTools.Event.IObserver">Adds the given IObserver object to the OnError event.</param>
	/// <returns type="Integer">Returns the index, at which the observer object has been added.
	/// Returns -1 if the given observer object is invalid and not added.</returns>
	this.AddOnErrorEvent = function(varLogObserver)
	{
		_events.Attach(ERROR_EVENT_NAME, varLogObserver);
	}


	/// <method>
	/// Removes an observer from the OnError subject (event).
	/// </method>
	/// <param name="varObserverToDetach" type="JSTools.Event.IObserver">Observer object which should be removed.</param>
	/// <param name="varObserverToDetach" type="Integer">Internal index of the observer object which should be removed.</param>
	this.RemoveOnErrorEvent = function(varObserverToDetach)
	{
		_events.Detach(ERROR_EVENT_NAME, varObserverToDetach);
	}


	/// <method>
	/// Adds a new observer to the OnWarn subject (event).
	/// </method>
	/// <param name="varLogObserver" type="Function">Adds the given function object to the OnWarn event.</param>
	/// <param name="varLogObserver" type="JSTools.Reflection.MethodInfo">Adds the given MethodInfo object to the OnWarn event.</param>
	/// <param name="varLogObserver" type="JSTools.Event.IObserver">Adds the given IObserver object to the OnWarn event.</param>
	/// <returns type="Integer">Returns the index, at which the observer object has been added.
	/// Returns -1 if the given observer object is invalid and not added.</returns>
	this.AddOnWarnEvent = function(varLogObserver)
	{
		_events.Attach(WARN_EVENT_NAME, varLogObserver);
	}


	/// <method>
	/// Removes an observer from the OnWarn subject (event).
	/// </method>
	/// <param name="varObserverToDetach" type="JSTools.Event.IObserver">Observer object which should be removed.</param>
	/// <param name="varObserverToDetach" type="Integer">Internal index of the observer object which should be removed.</param>
	this.RemoveOnWarnEvent = function(varObserverToDetach)
	{
		_events.Detach(WARN_EVENT_NAME, varObserverToDetach);
	}


	/// <method>
	/// Creates a new log entry for the given message. If the Handling property contains the 
	/// JSTools.ExceptionHandling.ErrorEvent.Log flag the OnLog event will fire.
	/// </method>
	/// <returns type="Boolean">Returns true if the given error should not be visualized.</returns>
	this.Log = function(strMessage, strFile, intLine)
	{
		var exception = new JSTools.ExceptionHandling.Exception(
			strMessage,
			strFile,
			intLine,
			arguments.callee,
			new JSTools.ExceptionHandling.StackTrace());

		_logs.Add(exception);
		FireEvent(LOG_EVENT_NAME, exception, JSTools.ExceptionHandling.ErrorEvent.Log);

		return GetReturnValueFromHandling();
	}


	/// <method>
	/// Creates a new error entry for the given error. If the Handling property contains the 
	/// JSTools.ExceptionHandling.ErrorEvent.Error flag the OnError event will fire.
	/// </method>
	/// <returns type="Boolean">Returns true if the given error should not be visualized.</returns>
	this.Throw = function(strMessage, strFile, intLine)
	{
		var exception = new JSTools.ExceptionHandling.Exception(
			strMessage,
			strFile,
			intLine,
			arguments.callee,
			new JSTools.ExceptionHandling.StackTrace());

		_errors.Add(exception);
		FireEvent(ERROR_EVENT_NAME, exception, JSTools.ExceptionHandling.ErrorEvent.Error);

		return GetReturnValueFromHandling();
	}


	/// <method>
	/// Creates a new warning entry. If the Handling property contains the
	/// JSTools.ExceptionHandling.ErrorEvent.Warn flag the OnLog event is fired.
	/// </method>
	/// <returns type="Boolean">Returns true if the given error should not be visualized.</returns>
	this.Warn = function(strMessage, strFile, intLine)
	{
		var exception = new JSTools.ExceptionHandling.Exception(
			strMessage,
			strFile,
			intLine,
			arguments.callee,
			new JSTools.ExceptionHandling.StackTrace());

		_warnings.Add(exception);
		FireEvent(WARN_EVENT_NAME, exception, JSTools.ExceptionHandling.ErrorEvent.Warn);

		return GetReturnValueFromHandling();
	}


	/// <method>
	/// Checks whether the ErrorHandling property is equal to JSTools.ExceptionHandling.ErrorHandling.Catch.
	/// </method>
	/// <returns type="Boolean">Returns true if the given error should not be visualized.</returns>
	function GetReturnValueFromHandling()
	{
		return (_this.ErrorHandling == JSTools.ExceptionHandling.ErrorHandling.Catch);
	}


	/// <method>
	/// Fires the given event, if it is allowed.
	/// </method>
	/// <param name="strName" type="String">Event to fire.</returns>
	/// <param name="objException" type="JSTools.ExceptionHandling.Exception">Exception object to fire.</returns>
	/// <param name="enuToCheck" type="JSTools.ExceptionHandling.ErrorEvent">Event flag.</returns>
	function FireEvent(strName, objException, enuToCheck)
	{
		if (isNaN(_this.EventHandling)
			|| _this.EventHandling == JSTools.ExceptionHandling.ErrorEvent.None)
			return;

		if ((_this.EventHandling & enuToCheck) != 0
			|| _this.EventHandling == JSTools.ExceptionHandling.ErrorEvent.All)
			_events.Notify(strName, objException);
	}
	Init();
}


/// <enum>
/// Represents a bit mask used to filter events.
/// </enum>
/// <field name="None">The error is stored but no event will be fired.</field>
/// <field name="All">The error is stored and all events will be fired.</field>
/// <field name="Log">The log message is stored and an OnLog event will be fired.</field>
/// <field name="Error">The error message is stored and an OnError event will be fired.</field>
/// <field name="Warn">The warning message is stored and an OnWarn event will be fired.</field>
JSTools.ExceptionHandling.ErrorEvent = new JSTools.Enum.FlagsEnum(
	"None",
	"All",
	"Log",
	"Error",
	"Warn" );


/// <enum>
/// Used to determine the return value of the public Log, Thorw and Warn method.
/// </enum>
/// <field name="Catch">The public Log, Thorw and Warn methods will return true.</field>
/// <field name="Throw">The public Log, Thorw and Warn methods will return false.</field>
JSTools.ExceptionHandling.ErrorHandling = new JSTools.Enum.StringEnum(
	"Catch",
	"Throw" );


/// <property type="JSTools.ExceptionHandling.Handler">
/// Default JSTools.ExceptionHandling.Handler instance.
/// </property>
JSTools.Exception = new JSTools.ExceptionHandling.Handler();

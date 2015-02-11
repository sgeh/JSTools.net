function TimerThread(objFunctionRun)
{
	var _functionCall	= (IsFunction(objFunctionRun) ? objFunctionRun : null);

	var _timer			= null;
	var _timerOut		= null;
	var _timerState		= TimerThread.States.Unstarted;

	var _index			= null;
	var _holderName		= "";
	var _this			= this;
	var _arguments		= null;

	this.Interval		= 20;


	// returns the index on which this thread is registered
	this.GetIndex = function()
	{
		return _index;
	}


	// returns a string reference to the real timerthread object
	this.GetRefString = function()
	{
		return _holderName;
	}


	// returns the current status of this TimerThread object
	this.GetStatus = function()
	{
		return _timerState;
	}


	// returns true, if the current thread is "runnig" or "sleep"
	this.IsAlive = function()
	{
		return (_timerState == TimerThread.States.Running || _timerState == TimerThread.States.Sleep);
	}


	// returns true, if the current thread state is "running"
	this.IsActive = function()
	{
		return (_timerState == TimerThread.States.Running && _timer != null);
	}


	// sets a timeout for an active TimerThread interval
	this.Sleep = function(intTimeOut)
	{
		if (_this.IsActive() && IsNumeric(intTimeOut))
		{
			ClearProcesses();
			_timerState	= TimerThread.States.Sleep;
			_timerOut	= window.setTimeout(_holderName + ".Resume();", ToNumber(intTimeOut));
		}
	}


	// interrupts a sleeping thread state and resumes it
	this.Interrupt = function()
	{
		if (_timerState	== TimerThread.States.Sleep)
		{
			ClearTimeOut();
			_this.Resume();
		}
	}


	// aborts an active TimerInterval, this state can not be resumed
	this.Abort = function()
	{
		_this.Suspend();
		_timerState	= TimerThread.States.Aborted;
		window.TimerPool.Remove(_index);
	}


	// resumes a suspended thread
	this.Resume = function()
	{
		if (_timerState == TimerThread.States.Stopped)
		{
			_arguments = arguments;
			RunTimer();
		}
	}


	// starts the timer thread
	this.Start = function()
	{
		if (_timerState == TimerThread.States.Unstarted)
		{
			_arguments = arguments;
			RunTimer();
		}
	}


	// runs the specified function, this method could be overwritten
	// !!DO NOT USE THIS FUNCTION FROM GLOBAL SCRIPT CODE!!
	this.Run = function()
	{
		if (_functionCall != null)
		{
			if (_arguments != null && _arguments.length)
			{
				eval("_functionCall(" + String.CreateArgumentString(_arguments.length, "_arguments") + ");");
			}
			else
			{
				_functionCall();
			}
		}
	}


	// suspends an active TimerInterval or TimerTimeOut, this state can be resumed.
	// clears the timeout and the interval
	this.Suspend = function()
	{
		if (_this.IsActive())
		{
			_arguments = null;
			ClearProcesses();
		}
	}



	// clears the timeout and the interval processes
	function ClearProcesses()
	{
		if (_this.IsActive())
		{
			ClearTimer();
			ClearTimeOut();
			_timerState	= TimerThread.States.Stopped;
		}
	}


	// creates a new timer interval
	function RunTimer()
	{
		_timer		= window.setInterval(_holderName + ".Run()", _this.Interval);
		_timerState	= TimerThread.States.Running;
	}


	// clears an active interval
	function ClearTimer()
	{
		if (_timer != null)
		{
			window.clearInterval(_timer);
			_timer = null;
		}
	}


	// clears an active time out
	function ClearTimeOut()
	{
		if (_timerOut != null)
		{
			window.clearInterval(_timerOut);
			_timerOut = null;
		}
	}

	_index		= window.TimerPool.Register(this);
	_holderName	= "window.TimerPool.GetTimer(" + _index + ")";
}
TimerThread.prototype.toString = function()
{
    return "[object TimerThread]";
}
TimerThread.States =
{
	Unstarted:	"unstarted",	// Start				: Running
	Running:	"running",		// Sleep, Suspend		: Sleep, Stopped
	Sleep:		"sleep",		// Interrupt			: Running
	Stopped:	"stopped",		// Resume				: Running
	Aborted:	"aborted"
};
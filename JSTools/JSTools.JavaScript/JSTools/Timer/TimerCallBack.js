function TimerCallBack(objFunctionPtr, intTimeOut)
{
	var _this			= this;
	var _functionPtr	= objFunctionPtr;
	var _arguments		= null;
	var _timeOut		= (!IsUndefined(intTimeOut) ? intTimeOut : 0);

	var _timerCall		= null;


	// starts the call back, the argument collection is assigned to _arguments.
	// so Fire() can retrieve it
	this.Start = function()
	{
		if (IsValidCall())
		{
			_arguments = arguments;

			with (_timerCall)
			{
				Interval = intTimeOut;
				Start();
			}
		}
	}


	// fires the call back event
	function Fire()
	{
		if (IsValidCall() && !IsUndefined(_arguments))
		{
			_timerCall.Abort();
			_timerCall = null;
			eval("_functionPtr(" + String.CreateArgumentString(_arguments.length, "_arguments") + ");");
		}
	}


	// returns true, if the given parameters contains a valid function pointer
	function IsValidCall()
	{
		return (IsFunction(_functionPtr) && _timerCall != null);
	}

	var _timerCall		= new TimerThread(Fire);
}
TimerCallBack.prototype.toString = function()
{
    return "[object TimerCallBack]";
}
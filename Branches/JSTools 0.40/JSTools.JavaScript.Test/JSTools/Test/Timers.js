document.write("<h1>TEST TIMERS</h1>");

function TimerTestClass()
{
	this.InitType(arguments, "TimerTestClass");

	var _this = this;
	var _timer = null;
	var _timeout = null;
	var _called = 0;
	var _timeoutCalled = 0;

	function Init()
	{
		_timer = new JSTools.Timers.Timer();
		_timer.Interval = 500;
		_timer.GetArguments().Add("timer_running");
		_timer.AddTickFunction(_this.TestFunction);
		_timer.AddTickFunction(_this.TestFunction1);
		_timer.AddTickMethodInfo(_this.GetType().GetMethod("Testfunction2"));
		_timer.Start();
		
		_timeout = new JSTools.Timers.Timeout();
		_timeout.Timeout = 10000;
		_timeout.GetArguments().Add("timeout_running");
		_timeout.AddTimeoutFunction(_this.TestTimeout);
		_timeout.AddTimeoutMethodInfo(_this.GetType().GetMethod("TestTimeout"));
		_timeout.Start();
	}

	this.TestFunction = function(arrArguments)
	{
		++_called;

		alert("TIMER: TestFunction called [" + arrArguments + "]"
			+ "\nContext = " + this
			+ "\nScope = " + _this);

		if (!_timer.IsEnabled())
		{
			alert("Invalid Enabled status!");
		}

		if (_called == 2)
		{
			_timer.Stop();
		}

		if (_called == 3)
		{
			alert("Timer stopping failed!");
		}
	}

	this.TestFunction1 = function(arrArguments)
	{
		alert("TIMER: TestFunction1 called [" + arrArguments + "]"
			+ "\nContext = " + this
			+ "\nScope = " + _this);
	}

	this.Testfunction2 = function(arrArguments)
	{
		alert("TIMER: TestFunction2 called [" + arrArguments + "]"
			+ "\nContext = " + this
			+ "\nScope = " + _this);
	}

	this.TestTimeout = function(arrArguments)
	{
		++_timeoutCalled;

		alert("TIMEOUT: TestTimeout called [" + arrArguments + "]"
			+ "\nContext = " + this
			+ "\nScope = " + _this);
			
		if (_timeoutCalled > 2)
		{
			alert("Timeout was not stopped automatically!");
		}
	}
	Init();
}

document.write("test disabled");
//var timerTest = new TimerTestClass();
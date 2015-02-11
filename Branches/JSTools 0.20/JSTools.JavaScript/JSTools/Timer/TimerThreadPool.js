function TimerThreadPool()
{
	var _registeredTimers = new Array();

	// regesters a new TimerThread object in the _registeredTimers Array
	this.Register = function(objTimer)
	{
		if (String(objTimer) == "[object TimerThread]")
		{
			var timerCount = _registeredTimers.length;
			_registeredTimers[timerCount] = objTimer;
			return timerCount;
		}
		return null;
	}

	// removes the specified index from the timer spooler
	this.Remove = function(intIndex)
	{
		if (_registeredTimers.IsValidIndex(intIndex) && _registeredTimers[intIndex])
		{
			delete _registeredTimers[intIndex];
		}
	}

	// returns a timer object
	this.GetTimer = function(intIndex)
	{
		return (_registeredTimers.IsValidIndex(intIndex) ? _registeredTimers[intIndex] : null);
	}
}
TimerThreadPool.prototype.toString = function()
{
    return "[object TimerThreadPool]";
}

window.TimerPool = new TimerThreadPool();
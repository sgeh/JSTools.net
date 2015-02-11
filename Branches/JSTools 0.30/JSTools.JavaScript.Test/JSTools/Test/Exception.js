document.write("<h1>TEST EXCEPTION</h1>");

var exceptionOccurences = 0;
var logOccurences = 0;
var warnOccurences = 0;

var exceptionOk = false;
var logOk = false;
var warningOk = false;

function checkException(objException)
{
	++exceptionOccurences;
	exceptionOk = (objException.GetMessage() == "throw");
}

function checkLog(objException)
{
	++logOccurences;
	logOk = (objException.GetMessage() == "log");
}

function checkWarning(objException)
{
	++warnOccurences;
	warningOk = (objException.GetMessage() == "warn");
}

function fireEvents()
{
	var status = (JSTools.Exception.ErrorHandling == JSTools.ExceptionHandling.ErrorHandling.Catch);

	if (status != JSTools.Exception.Warn("warn", String.Empty, -1))
		document.write("Warn() has returned an invalid boolean");

	if (status != JSTools.Exception.Throw("throw", String.Empty, -1))
		document.write("Throw() has returned an invalid boolean");

	if (status != JSTools.Exception.Log("log", String.Empty, -1))
		document.write("Log() has returned an invalid boolean");
}

function drawExceptionStatus(intStatus, intMaxOccurences, blnOk, strEvent)
{
	if (intStatus != intMaxOccurences || !blnOk)
	{
		document.write("an error has occured while handling the exception event '"
			+ strEvent
			+ "' -> [status: " + intStatus + "; should: "
			+ intMaxOccurences
			+ "]<br>");
	}
	else
	{
		document.write("event '" + strEvent + "' successfull<br>");
	}
}

JSTools.Exception.AddOnLogEvent(checkLog);
JSTools.Exception.AddOnErrorEvent(checkException);
JSTools.Exception.AddOnWarnEvent(checkWarning);

JSTools.Exception.EventHandling = JSTools.ExceptionHandling.ErrorEvent.All;

fireEvents();

JSTools.Exception.EventHandling = JSTools.ExceptionHandling.ErrorEvent.None;

fireEvents();

JSTools.Exception.EventHandling |= JSTools.ExceptionHandling.ErrorEvent.Log;

fireEvents();

JSTools.Exception.EventHandling |= JSTools.ExceptionHandling.ErrorEvent.Error;

fireEvents();

JSTools.Exception.EventHandling |= JSTools.ExceptionHandling.ErrorEvent.Warn;

fireEvents();


// check result
drawExceptionStatus(logOccurences, 4, logOk, "log");
drawExceptionStatus(exceptionOccurences, 3, exceptionOk, "error");
drawExceptionStatus(warnOccurences, 2, warningOk, "warn");
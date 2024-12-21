document.write("<h1>TEST STACKTRACE</h1>");

var a1 = function()
{
	a2();
}

function a2()
{
	a3();
}

function a3()
{
	a4();
}

function a4()
{
	a5();
}

function a5()
{
	a6();
}

function a6()
{
	var stackTrace = new JSTools.ExceptionHandling.StackTrace();
	document.write("<p>Stacktrace: " + stackTrace.toString() + "</p>");
	document.write("<p>Method Count: " + stackTrace.GetMethodCount() + "</p>");
	document.write("<p>Method [0]: " + typeof(stackTrace.GetMethod(0)) + "</p>");
	
	if (!stackTrace.IsAvailable())
	{
		document.write("[Invalid Browser]");
	}
}

a1();

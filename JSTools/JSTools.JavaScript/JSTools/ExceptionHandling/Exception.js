namespace("JSTools.ExceptionHandling");


/// <class>
/// Represents an exception object, which stores information about an error. To throw an exception,
/// you should use JSTools.Exception.Throw(strMessage, strFile, intLine);
/// </class>
/// <param name="strMessage" type="String">Error message which describes this exception.</param>
/// <param name="strFile" type="String">File which has thrown this exception.</param>
/// <param name="intLine" type="Integer">Line number of the exception.</param>
/// <param name="objTargetSiteFunction" type="Function">Function which has thrown this exception.</param>
/// <param name="objStackTrace" type="JSTools.ExceptionHandling.StackTrace">Call stack object.</param>
JSTools.ExceptionHandling.Exception = function(strMessage, strFile, intLine, objTargetSiteFunction, objStackTrace)
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	this.InitType(arguments, "JSTools.ExceptionHandling.Exception");

	var _message	= (strMessage) ? String(strMessage) : String.Empty;
	var _file		= (strFile) ? String(strFile) : String.Empty;
	var _lineNumber	= (!isNaN(intLine)) ? intLine : -1;
	var _targetSite	= (typeof(objTargetSiteFunction) == 'function') ? objTargetSiteFunction : null;
	var _stackTrace	= null;


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------

	/// <constructor>
	/// Creates a new JSTools.ExceptionHandling.Exception instance.
	/// </constructor>
	function Init()
	{
		if (objStackTrace
			&& typeof(objStackTrace) == 'object'
			&& objStackTrace.IsTypeOf(JSTools.ExceptionHandling.StackTrace))
		{
			_stackTrace = objStackTrace;
		}
	}


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <constructor>
	/// Gets the StackTrace object of the exception.
	/// </constructor>
	this.GetStackTrace = function()
	{
		return _stackTrace;
	}


	/// <constructor>
	/// Gets the file name that throws the current exception.
	/// </constructor>
	this.GetFile = function()
	{
		return _file;
	}


	/// <constructor>
	/// Gets the file number that throws the current exception.
	/// </constructor>
	this.GetLineNumber = function()
	{
		return _lineNumber;
	}


	/// <constructor>
	/// Gets the exception message.
	/// </constructor>
	this.GetMessage = function()
	{
		return _message;
	}


	/// <constructor>
	/// Gets the file method that throws the current exception.
	/// </constructor>
	this.GetTargetSite = function()
	{
		return _targetSite;
	}
	Init();
}

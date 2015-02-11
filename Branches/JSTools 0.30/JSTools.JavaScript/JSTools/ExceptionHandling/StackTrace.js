namespace("JSTools.ExceptionHandling");


/// <class>
/// Provides informations about the call stack. This feature is not available in all browsers.
/// Mozilla 1.x will stop executing the current script (except 1.4), if you create an instance of
/// this class. Opera x.x/Surfin'Safari does not provide a Funciton.caller property, thus this
/// class will not return a valid call stack representation.
///
/// Caution:
/// This class uses deprecated functionalities (Function.caller).
/// </class>
JSTools.ExceptionHandling.StackTrace = function()
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	this.InitType(arguments, "JSTools.ExceptionHandling.StackTrace");

	var _this			= this;
	var _methods		= [ ];
	var _callee			= arguments.callee;
	var _isAvailable	= true;


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------

	/// <constructor>
	/// Creates a new JSTools.ExceptionHandling.StackTrace instance.
	/// </constructor>
	function Init()
	{
		if (typeof(_callee) != 'function' || !arguments.caller)
		{
			_isAvailable = false;
			return;
		}

		var functionObject = _callee.caller;

		while (typeof(functionObject) == 'function')
		{
			_methods.Add(functionObject);
			functionObject = eval("functionObject.caller");
		}
	}


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Checks whether the current browser supports reflection of the call stack.
	/// </method>
	/// <returns type="Boolean">Returns true if the current browser supports reflection of the call stack.</returns>
	this.IsAvailable = function()
	{
		return _isAvailable;
	}


	/// <method>
	/// Gets the count of methods in the call stack.
	/// </method>
	/// <returns type="Integer">Returns the count of methods in the call stack.</returns>
	this.GetMethodCount = function()
	{
		return _methods.length;
	}


	/// <method>
	/// Searches for the method at the given index.
	/// </method>
	/// <returns type="Function">Returns a function object if a function at the given index exist.
	/// Ohterwise you will obtian a null reference.</returns>
	this.GetMethod = function(intIndex)
	{
		if (typeof(intIndex) != 'number' || isNaN(intIndex))
			return null;

		if (intIndex > -1 && _methods.length > intIndex)
		{
			return _methods[intIndex];
		}
		return null;
	}


	/// <method>
	/// Returns a string representation of this call stack.
	/// </method>
	this.toString = function()
	{
		var stackTrace = String.Empty;

		for (var i = 0; i < _methods.length; ++i)
		{
			if (_methods[i].GetName() == String.Empty)
			{
				stackTrace += "[ function ]\n";
			}
			else
			{
				stackTrace += _methods[i].GetName() + "\n";
			}
		}
		return stackTrace;
	}
	Init();
}

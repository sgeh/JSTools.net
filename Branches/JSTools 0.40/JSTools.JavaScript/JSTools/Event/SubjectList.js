namespace("JSTools.Event");


/// <class>
/// Represents a list of Subject instances.
///
/// There are three different methods, which attach a function object
/// to the subject (event).
///
/// AttachMethodInfo
///  Attaches a method to the subject. The call will be executed in
///  the creation context (this pointer) of the object.
/// AttachFunction
///  Attaches a method to the subject. The call will be executed in
///  the global context (this pointer).
/// AttachObserver
///  Attaches a new observer to this subject list. You can implement there
///  your own behaviour.
/// </class>
JSTools.Event.SubjectList = function()
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	this.InitType(arguments, "JSTools.Event.SubjectList");

	var _this = this;
	var _subjects = null;


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------
	
	/// <constructor>
	/// Creates a new JSTools.Event.Subject instance.
	/// </constructor>
	function Init()
	{
		_this.Clear();
	}


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Removes all registered observer objects.
	/// </method>
	function Clear()
	{
		if (_subjects != null)
		{
			for (var i = 0; i < _subjects.length; ++i)
			{
				_subjects[_subjects[i]].Clear();
			}
		}
		_subjects = [ ];
	}
	this.Clear = Clear;


	/// <method>
	/// Attaches the given object as an observer to the subject with the given name.
	/// </method>
	/// <param name="strSubjectName" type="String">Subject container name.</param>
	/// <param name="varEventHandler" type="Function">Adds the given function object to the subject.</param>
	/// <param name="varEventHandler" type="JSTools.Reflection.MethodInfo">Adds the given MethodInfo object to the subject.</param>
	/// <param name="varEventHandler" type="JSTools.Event.IObserver">Adds the given IObserver object to the subject.</param>
	/// <returns type="Integer">Returns the index, at which the observer object has been added.
	/// Returns -1 if the given observer object is invalid and not added.</returns>
	function Attach(strSubjectName, varEventHandler)
	{
		if (!varEventHandler)
			return -1;

		if (typeof(varEventHandler) == 'function')
		{
			return _this.AttachFunction(strSubjectName, varEventHandler);
		}
		else if (typeof(varEventHandler) == 'object' && varEventHandler.IsTypeOf(JSTools.Event.IObserver))
		{
			return _this.AttachObserver(strSubjectName, varEventHandler);
		}
		else
		{
			return _this.AttachMethodInfo(strSubjectName, varEventHandler);
		}
	}
	this.Attach = Attach;


	/// <method>
	/// Attaches the given function as an observer to the subject with the given name.
	/// </method>
	/// <param name="strSubjectName" type="String">Subject container name.</param>
	/// <param name="objFunction" type="Function">Function to add.</param>
	/// <returns type="Integer">Returns the index, at which the observer object has been added.
	/// Returns -1 if the given observer object is invalid and not added.</returns>
	function AttachFunction(strSubjectName, objFunction)
	{
		var subjectName = String(strSubjectName);

		if (typeof(objFunction) == 'function' && IsValidName(subjectName))
		{
			return GetSubject(subjectName).Attach(new JSTools.Event.FunctionObserver(objFunction));
		}
		return -1;
	}
	this.AttachFunction = AttachFunction;


	/// <method>
	/// Attaches the given function as an observer to the subject with the given name.
	/// </method>
	/// <param name="strSubjectName" type="String">Subject container name.</param>
	/// <param name="objMethodInfo" type="JSTools.Reflection.MethodInfo">Function info object to call.</param>
	/// <returns type="Integer">Returns the index, at which the observer object has been added.
	/// Returns -1 if the given observer object is invalid and not added.</returns>
	function AttachMethodInfo(strSubjectName, objMethodInfo)
	{
		var subjectName = String(strSubjectName);

		if (objMethodInfo
			&& typeof(objMethodInfo) == 'object'
			&& objMethodInfo.Invoke
			&& IsValidName(subjectName))
		{
			return GetSubject(subjectName).Attach(new JSTools.Event.MethodInfoObserver(objMethodInfo));
		}
		return -1;
	}
	this.AttachMethodInfo = AttachMethodInfo;


	/// <method>
	/// Attaches the given observer function to this subject.
	/// </method>
	/// <param name="strSubjectName" type="String">Subject container name.</param>
	/// <param name="objIObserver" type="JSTools.Event.IObserver">Observer to attach.</param>
	/// <returns type="Integer">Returns the index, at which the observer object has been added.
	/// Returns -1 if the given observer object is invalid and not added.</returns>
	function AttachObserver(strSubjectName, objIObserver)
	{
		var subjectName = String(strSubjectName);

		if (objIObserver
			&& typeof(objIObserver) == 'object'
			&& objIObserver.IsTypeOf(JSTools.Event.IObserver)
			&& IsValidName(subjectName))
		{
			return GetSubject(subjectName).Attach(objIObserver);
		}
		return -1;
	}
	this.AttachObserver = AttachObserver;


	/// <method>
	/// Detaches the given observer object from the given subject.
	/// </method>
	/// <param name="strSubjectName" type="String">Subject container name.</param>
	/// <param name="varObserverToDetach" type="JSTools.Event.IObserver">Observer to detach.</param>
	/// <param name="varObserverToDetach" type="Integer">Index to detach.</param>
	function Detach(strSubjectName, varObserverToDetach)
	{
		if (typeof(varObserverToDetach) == 'number')
		{
			_this.DetachByIndex(strSubjectName, varObserverToDetach);
		}
		else
		{
			_this.DetachObserver(strSubjectName, varObserverToDetach);
		}
	}
	this.Detach = Detach;


	/// <method>
	/// Detaches the given observer object form this subject.
	/// </method>
	/// <param name="strSubjectName" type="String">Subject container name.</param>
	/// <param name="objIObserverToDetach" type="JSTools.Event.IObserver">Observer to detach.</param>
	function DetachObserver(strSubjectName, objIObserverToDetach)
	{
		var subjectName = String(strSubjectName);

		if (IsSubject(subjectName))
		{
			_subjects[subjectName].Detach(objIObserverToDetach);
		}
	}
	this.DetachObserver = DetachObserver;


	/// <method>
	/// Indicates whether the given subject name was registered.
	/// </method>
	/// <param name="strSubjectName" type="String">Subject name to check.</param>
	/// <returns type="Boolean">Retruns true, if the given subject name is registered.</returns>
	function Contains(strSubjectName)
	{
		return IsSubject(String(strSubjectName));
	}
	this.Contains = Contains;


	/// <method>
	/// Detaches an observer at the given index from this subject.
	/// </method>
	/// <param name="strSubjectName" type="String">Subject container name.</param>
	/// <param name="intIndex" type="Integer">Index to detach.</param>
	function DetachByIndex(strSubjectName, intIndex)
	{
		var subjectName = String(strSubjectName);

		if (IsSubject(subjectName))
		{
			_subjects[subjectName].DetachByIndex(intIndex);
		}
	}
	this.DetachByIndex = DetachByIndex;


	/// <method>
	/// Notifies the observers of all subjects about an update.
	/// </method>
	/// <param name="objEvent" type="Object">An object instance, which represents the subject argument.</param>
	function NotifyAll(objEvent)
	{
		for (var i = 0; i < _subjects.length; ++i)
		{
			_this.Notify(_subjects[i], objEvent);
		}
	}
	this.NotifyAll = NotifyAll;


	/// <method>
	/// Notifies the observer about an update.
	/// </method>
	/// <param name="strSubjectName" type="String">Subject container name.</param>
	/// <param name="objEvent" type="Object">An object instance, which represents the Subject argument.</param>
	function Notify(strSubjectName, objEvent)
	{
		var subjectName = String(strSubjectName);

		if (IsSubject(subjectName))
		{
			_subjects[subjectName].Notify(objEvent);
		}
	}
	this.Notify = Notify;


	/// <method>
	/// Checks whether the given subject type is an array.
	/// </method>
	/// <param name="strSubjectName" type="String">Subject name to check.</param>
	/// <returns type="Boolean">Retruns true, if the given subject type is an array.</returns>
	function IsSubject(strSubjectName)
	{
		return (typeof(_subjects[strSubjectName]) == 'object'
			&& _subjects[strSubjectName].IsTypeOf(JSTools.Event.Subject));
	}


	/// <method>
	/// Checks whether the given name could be used for storing a subject.
	/// </method>
	/// <param name="strSubjectName" type="String">Subject name to check.</param>
	/// <returns type="Boolean">Retruns true, if the given name could be used for storing a subject.</returns>
	function IsValidName(strSubjectName)
	{
		return (typeof(_subjects[strSubjectName]) != 'function'
			|| typeof(_subjects[strSubjectName]) == 'undefined');
	}


	/// <method>
	/// Gets a registered subject with the given name.
	/// </method>
	/// <param name="strSubjectName" type="String">Subject to get.</param>
	/// <returns type="JSTools.Event.Subject">Returns a subject, which was registered with the given name.</returns>
	function GetSubject(strSubjectName)
	{
		if (!IsSubject(strSubjectName))
		{
			_subjects[strSubjectName] = new JSTools.Event.Subject();
			_subjects.Add(strSubjectName);
		}
		return _subjects[strSubjectName];
	}
	Init();
}

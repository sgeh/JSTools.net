/*
 * JSTools.JavaScript / JSTools.net - A JavaScript/C# framework.
 * Copyright (C) 2005  Silvan Gehrig
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 *
 * Author:
 *  Silvan Gehrig
 */

namespace("JSTools.Event");


/// <class>
/// Provides an interface for attaching and detaching Observer objects. Any number of
/// Observer objects may observe a subject.
/// </class>
JSTools.Event.Subject = function()
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	this.InitType(arguments, "JSTools.Event.Subject");
	
	var _this = this;
	var _observers = null;


	//------------------------------------------------------------------------
	// Constructor
	//------------------------------------------------------------------------
	
	/// <constructor>
	/// Creates a new JSTools.Event.Subject  instance.
	/// </constructor>
	function Init()
	{
		_this.Clear();
	}


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Removes all registered observer object.
	/// </method>
	function Clear()
	{
		_observers = [ ];
	}
	this.Clear = Clear;


	/// <method>
	/// Attaches the given observer function to this subject.
	/// </method>
	/// <param name="objIObserver" type="JSTools.Event.IObserver">Observer to attach.</param>
	/// <returns type="Integer">Returns the index, at which the observer object has been added.
	/// Returns -1 if the given observer object is invalid and not added.</returns>
	function Attach(objIObserver)
	{
		if (objIObserver
			&& typeof(objIObserver) == 'object'
			&& objIObserver.IsTypeOf(JSTools.Event.IObserver))
		{
			_observers.Add(objIObserver);
			return _observers.length - 1;
		}
		return -1;
	}
	this.Attach = Attach;


	/// <method>
	/// Detaches the given observer object from this subject.
	/// </method>
	/// <param name="objIObserverToDetach" type="JSTools.Event.IObserver">Observer to detach.</param>
	function Detach(objIObserverToDetach)
	{
		_observers.Remove(objIObserverToDetach);
	}
	this.Detach = Detach;


	/// <method>
	/// Detaches an observer at the given index from this subject.
	/// </method>
	/// <param name="intIndex" type="Integer">Index to detach.</param>
	function DetachByIndex(intIndex)
	{
		_observers.RemoveAt(intIndex);
	}
	this.DetachByIndex = DetachByIndex;


	/// <method>
	/// Notifies the observer about an update.
	/// </method>
	/// <param name="objEvent" type="Object">An object instance, which represents the event argument.</param>
	function Notify(objEvent)
	{
		for (var i = 0; i < _observers.length; ++i)
		{
			_observers[i].Update(objEvent);
		}
	}
	this.Notify = Notify;

	Init();
}

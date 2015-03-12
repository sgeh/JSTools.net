/*
 * JSTools.JavaScript.Test / JSTools.net - A JavaScript/C# framework.
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

document.write("<h1>TEST EVENT</h1>");

var _eventCount;
var recieverMethod;

function testEvent()
{
	document.write("<h3>TEST SUBJECT</h3>");

	_eventCount = 0;
	var myEvent = new JSTools.Event.Subject();

	var reciever = new EventReciever2();
	recieverMethod = new JSTools.Event.MethodInfoObserver(reciever.GetType().GetMethod("innerReciever"));
	var recieverProtectedMethod = new JSTools.Event.MethodInfoObserver(reciever.GetType().GetMethod("innerProtectedReciever"));

	var attachIndex = myEvent.Attach(recieverMethod);
	myEvent.Attach(recieverProtectedMethod);
	myEvent.Attach(new JSTools.Event.FunctionObserver(eventReciever1));
	myEvent.Attach(new JSTools.Event.FunctionObserver(eventReciever2));

	++_eventCount;
	myEvent.Notify(myEvent);

	document.write("<p>methodInfo attached at [idx " + attachIndex + "]</p>");
	myEvent.Detach(recieverMethod);

	++_eventCount;
	myEvent.Notify(myEvent);
	
	myEvent.Clear();

	++_eventCount;
	myEvent.Notify(myEvent);


	document.write("<h3>TEST SUBJECTLIST</h3>");

	_eventCount = 0;
	var myEventList = new JSTools.Event.SubjectList();

	var onTestIndex = myEventList.AttachFunction("Add", eventReciever1);
	myEventList.AttachFunction("onTest", eventReciever1);
	myEventList.AttachFunction("onAfterTest", eventReciever1);
	myEventList.AttachMethodInfo("onAfterTest", reciever.GetType().GetMethod("innerProtectedReciever"));
	myEventList.AttachMethodInfo("onAfterTest", reciever.GetType().GetMethod("innerReciever"));

	++_eventCount;
	myEventList.NotifyAll();
	myEventList.Detach("onTest", onTestIndex);

	++_eventCount;
	myEventList.Notify("onTest", "<code>AN ARGUMENT</code>");
	myEventList.Clear();

	++_eventCount;
	myEventList.Notify("onAfterTest", myEventList);
	myEventList.Notify("onTest", "<code>AN ARGUMENT</code>");	
}


var _countEventReciever1 = 0;

function eventReciever1(objSubject)
{
	++_countEventReciever1;
	document.write("<p>" + _eventCount + ". event eventReciever1 fired -> subject [" + objSubject + "]</p>");
}


var _countInnerReciever = 0;
var _countInnerProtectedReciever = 0;

function EventReciever2()
{
	this.InitType(arguments, "EventReciever2");

	var _this = this;
	var _protected = this.Inherit(Object);

	this.innerReciever = function(objSubject)
	{
		++_countInnerReciever;
		document.write("<p>");

		if (_eventCount == 2)
		{
			document.write("Detaching failed! Method was called twice! ");
		}
		else
		{
			document.write(_eventCount + ". event this.innerReciever fired -> subject [" + objSubject + "] ");
		}
		document.write("Context = " + this);
		document.write("</p>");
	}

	_protected.innerProtectedReciever = function(objSubject)
	{
		++_countInnerProtectedReciever;
		document.write("<p>");

		if (_eventCount == 3)
		{
			document.write("Clearing failed! ");
		}
		else
		{
			document.write(_eventCount + ". event this.innerProtectedReciever fired -> subject [" + objSubject + "] ");
		}
		document.write("Context = " + this + " ");
		document.write("Scope = " + _this);
		document.write("</p>");
	}
}

var _countEventReciever2 = 0;

function eventReciever2(objSubject)
{
	++_countEventReciever2;
	document.write("<p>" + _eventCount + ". event eventReciever2 fired -> subject [" + objSubject + "]</p>");
}

testEvent();

if (_countEventReciever1 != 5)
{
	document.write("<p>Event handling of eventReciever1 has failed (" + _countEventReciever1 + "/5)!</p>");
}

if (_countInnerReciever != 2)
{
	document.write("<p>Event handling of innerReciever has failed (" + _countInnerReciever + "/2)!</p>");
}

if (_countInnerProtectedReciever != 3)
{
	document.write("<p>Event handling of innerProtectedReciever has failed (" + _countInnerProtectedReciever + "/3)!</p>");
}

if (_countEventReciever2 != 2)
{
	document.write("<p>Event handling of EventReciever2 has failed (" + _countEventReciever2 + "/2)!</p>");
}
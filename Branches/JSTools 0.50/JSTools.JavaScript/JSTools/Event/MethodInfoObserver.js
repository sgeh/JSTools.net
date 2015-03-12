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
/// Represents a JSTools.Reflection.MethodInfo observer. Do not create observers for private
/// methods. Inner scopes can't be invoked.
/// </class>
/// <param name="objMethodInfo" type="JSTools.Reflection.MethodInfo">Function info object to call.</param>
JSTools.Event.MethodInfoObserver = function(objMethodInfo)
{
	//------------------------------------------------------------------------
	// Declarations
	//------------------------------------------------------------------------

	this.InitType(arguments, "JSTools.Event.MethodInfoObserver");
	this.Inherit(JSTools.Event.IObserver);
	
	var _this = this;
	var _methodInfoToNotify = objMethodInfo;


	//------------------------------------------------------------------------
	// Methods
	//------------------------------------------------------------------------

	/// <method>
	/// Notifies the observer about an update.
	/// </method>
	/// <param name="objEvent" type="Object">An object instance, which represents the event argument.</param>
	function Update(objEvent)
	{
		if (_methodInfoToNotify && typeof(_methodInfoToNotify) == 'object' && _methodInfoToNotify.Invoke)
		{
			_methodInfoToNotify.Invoke(objEvent);
		}
	}
	this.Update = Update;
}

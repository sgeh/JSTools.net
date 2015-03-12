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

document.write("<h1>TEST INHERITANCE</h1>");

function Car()
{
	this.InitType(arguments, "Car");

	var _protected = this.Inherit(Object);
	_protected.car = true;
}


function Ford(strAttrib, intSpeed)
{
	this.InitType(arguments, "Ford");

	var _protected = this.Inherit(Car);
	_protected.ford = true;
	_protected[strAttrib] = intSpeed;
}

function Mustang()
{
	this.InitType(arguments, "Mustang");

	var FORD_SPEED = 204;
	var _protected = this.Inherit(Ford, "speed", FORD_SPEED);
	_protected.mustang = true;

	this.alertProtectedMembers = function()
	{
		if (!_protected.car || _protected.speed != FORD_SPEED)
		{
			document.write("<p>"
			+ _protected.car
			+ " && "
			+ _protected.speed
			+ " -> inheritance failed\n\nGUID: "
			+ this.GetType().GetGuid().toString()
			+ "</p>");
		}
	}

	this.GetProtectedArray = function()
	{
		return _protected;
	}
}


var date = new Date();

for (var i = 0; i < 100; ++i)
{
	var mustang = new Mustang();
	mustang.alertProtectedMembers();
}
document.write("<p>100x Inheritance: " + ((new Date() - date) / 1000) + " -> Base Types: " + mustang.GetType().GetBaseTypes() + "</p>");
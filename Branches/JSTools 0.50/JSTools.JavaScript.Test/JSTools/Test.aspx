<%@ Register TagPrefix="jstools" NameSpace="JSTools.Web.UI.Controls" Assembly="JSTools.Web" %>
<%@ Page language="c#" Codebehind="Test.aspx.cs" AutoEventWireup="false" Inherits="JSTools.JavaScript.Test.Test" %>
<%
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
%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<TITLE>JSTools</TITLE>
	<JSTools:Head runat="server" id="Head1"></JSTools:Head>
	<JSTools:Script optimization="Crunch" runat="server" id="Script1">function ConsoleMessage(strMessageType, strMessage)
{
	// init type manager, required for inheritance
	this.InitTypeManager(arguments);

	// copies a reference of the protected members for using from outside the constructor
	var _protected		= arguments.Protected;
	var _message		= ToString(strMessage);
	var _messageType	= strMessageType;


	// returns the message, which is given by the constructor or appended by the AppendMessage method
	this.GetMessage = function()
	{
		return _message;
	}


	// appends a string to the message, which is given by the constructor
	this.AppendMessage = function(strMessage)
	{
		_message += strMessage;
	}


	// overrides the message with a new value
	this.SetMessage = function(strValue)
	{
		_message = strValue;
	}


	// returns the message type (Warning, Message)
	this.GetMessageType = function()
	{
		return _messageType;
	}


	// returns the message and the type in a console like string format
	this.ToLineString = function()
	{
		return _protected.ToLineString();
	}


	// writes an anonym function pointer into the protected member container
	_protected.ToLineString = function()
	{
		return "~" + _messageType + ": " + _message;
	}
}</JSTools:Script>
	<frameset cols="50%,50%" border="0" framespacing="0" frameborder="NO">
		<frame name="left" src="frameSet_left.aspx" marginwidth="0" leftmargin="0" marginheight="0"
			topmargin="0" scrolling="yes" noresize frameborder="NO" border="0">
		<frame name="right" src="frameSet_right.html" marginwidth="0" leftmargin="0" marginheight="0"
			topmargin="0" scrolling="yes" noresize frameborder="NO" border="0">
	</frameset>
</HTML>

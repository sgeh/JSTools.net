<%@ Page language="c#" Codebehind="Test.aspx.cs" AutoEventWireup="false" Inherits="JSTools.JavaScript.Test.Test" %>
<%@ Register TagPrefix="jstools" NameSpace="JSTools.Web.UI.Controls" Assembly="JSTools.Web.UI" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<JSTools:Head runat="server">
		<title>JSTools</title>
	</JSTools:Head>
	<JSTools:Script optimization="Crunch" runat="server">function ConsoleMessage(strMessageType, strMessage)
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
}</JSTools:Script>
	<frameset cols="50%,50%" border="0" framespacing="0" frameborder="NO">
		<frame name="left" src="frameSet_left.aspx" marginwidth="0" leftmargin="0" marginheight="0" topmargin="0" scrolling="NO" noresize frameborder="NO" border="0" />
		<frame name="right" src="frameSet_right.html" marginwidth="0" leftmargin="0" marginheight="0" topmargin="0" scrolling="NO" noresize frameborder="NO" border="0" />
	</frameset>
</html>

<%@ Register TagPrefix="JSTools" Namespace="JSTools.Web.UI.Controls" Assembly="JSTools.Web" %>
<%@ Page language="c#" Codebehind="frameSet_left.aspx.cs" Trace="true" AutoEventWireup="false" Inherits="JSTools.JavaScript.Test.frameSet_left" %>
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
	<JSTools:Head runat="server" id="_head">
		<META content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<META content="C#" name="CODE_LANGUAGE">
		<META content="JavaScript" name="vs_defaultClientScript">
		<META content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<title><%# "data binding" %></title>
	</JSTools:Head>
	<body MS_POSITIONING="GridLayout">
<jstools:Script runat="server" Section="Head" Optimization="RemoveComments" id="Script1">
// using cookie libraries
using("JSTools.Web.Cookie");
</jstools:Script>
<b><%= "test" %> - <%# "data binding" %></b>
		<script language="javascript" type="text/javascript" src="Test/Guid.js"></script>
		<script language="javascript" type="text/javascript" src="Test/Inheritance.js"></script>
		<script language="javascript" type="text/javascript" src="Test/StringConverter.js"></script>
		<script language="javascript" type="text/javascript" src="Test/Reflection.js"></script>
		<script language="javascript" type="text/javascript" src="Test/StackTrace.js"></script>
		<script language="javascript" type="text/javascript" src="Test/Event.js"></script>
		<script language="javascript" type="text/javascript" src="Test/Timers.js"></script>
		<script language="javascript" type="text/javascript" src="Test/Exception.js"></script>
		<script language="javascript" type="text/javascript" src="Test/Cookie.js"></script>
		<script language="javascript" type="text/javascript" src="Test/SimpleObjectSerializer.js"></script>
		<form id="frameSet_left" method="post" runat="server">
		</form>
	</body>
</HTML>

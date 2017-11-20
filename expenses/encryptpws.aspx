<%@ Page language="c#" Inherits="expenses.encryptpws" Codebehind="encryptpws.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>encryptpws</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<asp:Label id="Label1" runat="server">Company ID</asp:Label>
			<asp:TextBox id="txtcompanyid" runat="server"></asp:TextBox>
			<asp:Button id="Button1" runat="server" Text="Button" onclick="Button1_Click"></asp:Button>
		</form>
	</body>
</HTML>

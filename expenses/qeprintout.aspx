<%@ Page language="c#" Inherits="expenses.qeprintout" Codebehind="qeprintout.aspx.cs" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD runat=server>
		<title>qeprintout</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><LINK href="images/styles.css" type=text/css rel=stylesheet >
      <link id="jQueryCss" runat="server" type="text/css" href="/static/js/jQuery/jquery-ui-1.9.2.custom.css" rel="Stylesheet" />
  </HEAD>
<body>
    <asp:Literal ID="litJavascript" runat="server"></asp:Literal>
    <cc1:ToolkitScriptManager ID="scriptman" runat="server" EnablePageMethods="True">
        <Scripts>
            <asp:ScriptReference Path="../Spend Management/shared/javascript/sel.main.js" />
            <asp:ScriptReference Path="../Spend Management/shared/javascript/sel.common.js" />
            <asp:ScriptReference Path="../Spend Management/shared/javaScript/shared.js" />
        </Scripts>
    </cc1:ToolkitScriptManager>
<form id=Form1 method=post runat="server">
<table border=0>
  <tr>
    <td>
      <table><asp:literal id=littopleft 
         runat="server" meta:resourcekey="littopleftResource1"></asp:Literal></TABLE></TD>
    <td>
      <table><asp:literal id=littopcenter 
         runat="server" meta:resourcekey="littopcenterResource1"></asp:Literal></TABLE></TD>
    <td>
      <table><asp:literal id=littopright 
         runat="server" meta:resourcekey="littoprightResource1"></asp:Literal></TABLE></TD></TR>
  <tr>
					<td colspan="3"><asp:Literal id="litform" runat="server" meta:resourcekey="litformResource1"></asp:Literal></td></TR>
				<tr>
					<td>
						<table><asp:Literal id="litbottomleft" runat="server" meta:resourcekey="litbottomleftResource1"></asp:Literal></table></td>
					<td>
						<table><asp:Literal id="litbottomcenter" runat="server" meta:resourcekey="litbottomcenterResource1"></asp:Literal></table></td>
					<td>
						<table><asp:Literal id="litbottomright" runat="server" meta:resourcekey="litbottomrightResource1"></asp:Literal></table></td>
				</tr></TABLE>
</FORM>
	</body>
</HTML>

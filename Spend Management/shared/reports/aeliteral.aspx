<%@ Page language="c#" Inherits="Spend_Management.aeliteral" Title="Add / Edit Static Column" Codebehind="aeliteral.aspx.cs" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>
<%@ Register Src="~/shared/usercontrols/tooltip.ascx" TagName="tooltip" TagPrefix="tooltip" %>

<html>
<head id="Head1" runat="server">
    <link rel="stylesheet" type="text/css" media="screen" href="../css/layout.css" />
    <link rel="stylesheet" type="text/css"  media="screen" href="../css/styles.aspx?style=logon" />
    <link id="favLink" runat="server" rel="shortcut icon" href="/favicon.ico" type="image/x-icon" />
</head>

<body>

<form id="Form1" runat="server">
    <asp:Literal ID="litstyles" runat="server"></asp:Literal>

    <cc1:ToolkitScriptManager ID="tsm" runat="server">
        <Services>
            <asp:ServiceReference Path="~/shared/webServices/svcTooltip.asmx" InlineScript="false" />
        </Services>
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/minify/sel.main.js" />
            <asp:ScriptReference Name="common" />
            <asp:ScriptReference Name="tooltips" />
        </Scripts>
    </cc1:ToolkitScriptManager>

	<div class="valdiv">
		<asp:ValidationSummary id="ValidationSummary1" runat="server" ShowMessageBox="True" ShowSummary="False" meta:resourcekey="ValidationSummary1Resource1"></asp:ValidationSummary>
	</div>
	<div class="inputpanel">
		<div class="inputpaneltitle">
            <asp:Label ID="lblgeneral" runat="server" Text="General Details" meta:resourcekey="lblgeneralResource1"></asp:Label></div>
		<table>
			<tr>
				<td class="labeltd"><asp:Label ID="lblcolumn" runat="server" Text="Column&nbsp;Name:" meta:resourcekey="lblcolumnResource1"></asp:Label></td>
				<td class="inputtd"><asp:TextBox id="txtname" runat="server" MaxLength="50" meta:resourcekey="txtnameResource1"></asp:TextBox></td>
				<td><asp:RequiredFieldValidator id="reqname" runat="server" ErrorMessage="Please enter a value for Column Name" ControlToValidate="txtname" meta:resourcekey="reqnameResource1">*</asp:RequiredFieldValidator></td>
                <td><span class="logontooltip"><asp:Image ID="imgTooltipName" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('5cf85412-1f64-4058-9a1c-9c0010be53f7', 'sm', this);" /></span></td>
			</tr>
			<tr>
				<td class="labeltd"><asp:Label ID="lblvalue" runat="server" Text="Value:" meta:resourcekey="lblvalueResource1"></asp:Label></td>
				<td class="inputtd"><asp:TextBox id="txtvalue" runat="server" meta:resourcekey="txtvalueResource1"></asp:TextBox></td>
                <td></td>
				<td><span class="logontooltip"><asp:Image ID="imgTooltipValue" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('95d88988-c504-4c33-94b8-b2c4fcb97791', 'sm', this);" /></span></td>
			</tr>
			<tr>
				<td class="labeltd"><asp:Label ID="lblruntime" runat="server" Text="Enter at Runtime:" meta:resourcekey="lblruntimeResource1"></asp:Label></td>
				<td class="inputtd"><asp:CheckBox id="chkruntime" runat="server" meta:resourcekey="chkruntimeResource1"></asp:CheckBox></td>
                <td></td>
                <td><span class="logontooltip"><asp:Image ID="imgTooltipRuntime" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('f49ec6bd-e9d5-41fd-9534-3ed650411898', 'sm', this);" /></span></td>
			</tr>
		</table>
        <asp:TextBox ID="txtorder" runat="server" meta:resourcekey="txtorderResource1"></asp:TextBox>
	</div>
	<div class="inputpanel">
	<a target="static" href="javascript:getStaticField();"><img border="0" src="/shared/images/buttons/btn_save.png" /></a> &nbsp;&nbsp;
		<a href="javascript:window.close();"><img border="0" src="../images/buttons/cancel_up.gif" /></a></div>
    <tooltip:tooltip ID="usrTooltip" runat="server"></tooltip:tooltip>
</form>
    </body>

</html>


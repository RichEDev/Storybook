<%@ Page language="c#" Inherits="Spend_Management.print" Codebehind="print.aspx.cs" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" EnableViewState="false" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head runat="server">
		<title>print</title>

	</head>
	<body>
          
    <asp:Literal ID="litstyles" runat="server"></asp:Literal>
    <asp:Literal ID="litJavascript" runat="server"></asp:Literal>

		<form method="post" runat="server">
		    <asp:ScriptManager ID="ScriptManager1" runat="server">
            <Scripts>
                <asp:ScriptReference Path="~/shared/javascript/minify/sel.main.js" />
                <asp:ScriptReference Name="common" />
                <asp:ScriptReference Path="~/shared/javaScript/minify/sel.jsonParse.js" />
                <asp:ScriptReference Path="~/shared/javaScript/minify/sel.grid.js" />
                <asp:ScriptReference Path="~/shared/javaScript/shared.js" />
            </Scripts>
            <Services>
                <asp:ServiceReference Path="~/shared/webServices/svcGrid.asmx" InlineScript="false" />
            </Services>    
            </asp:ScriptManager>
			<div class="inputpanel">
				<table>
					<tr>
						<td class="labeltd">
                            <asp:Label ID="lblemployeename" runat="server" Text="Employee Name:" meta:resourcekey="lblemployeenameResource1"></asp:Label>
						</td>
						<td class="inputtd"><asp:label id="lblemployee" runat="server" meta:resourcekey="lblemployeeResource1">Label</asp:label></td>
						<td class="labeltd">
                            <asp:Label ID="lblsignature" runat="server" Text="Claimant Signature:" meta:resourcekey="lblsignatureResource1"></asp:Label>
						</td>
						<td class="inputtd" style="WIDTH: 100px; BORDER-BOTTOM: black 1px solid">&nbsp;</td>
					</tr>
					<tr>
						<td class="labeltd">
                            <asp:Label ID="lbldatelbl" runat="server" Text="Date:" meta:resourcekey="lbldatelblResource1"></asp:Label>
						</td>
						<td class="inputtd"><asp:label id="lbldate" runat="server" meta:resourcekey="lbldateResource1">Label</asp:label></td>
						<td class="labeltd">
                            <asp:Label ID="lblapprovalsig" runat="server" Text="Approval Signature:" meta:resourcekey="lblapprovalsigResource1"></asp:Label>
						</td>
						<td class="inputtd" style="WIDTH: 100px; BORDER-BOTTOM: black 1px solid">&nbsp;</td>
					</tr>
					<tr>
						<td class="labeltd">
                            <asp:Label ID="lblnumreceiptslbl" runat="server" Text="Num Receipts:" meta:resourcekey="lblnumreceiptslblResource1"></asp:Label></td>
						<td class="inputtd"><asp:label id="lblnumreceipts" runat="server" meta:resourcekey="lblnumreceiptsResource1">Label</asp:label></td>
					</tr>
					<asp:panel id="fields" runat="server" meta:resourcekey="fieldsResource1"></asp:panel></table>
			</div>
            
			
            <div class="formpanel"><asp:Literal ID="litExpensesGrid" runat="server"></asp:Literal></div>
		</form>
	</body>
</html>

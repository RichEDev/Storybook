<%@ Page language="c#" Inherits="expenses.admin.changeadvance" MasterPageFile="~/expform.master" Codebehind="changeadvance.aspx.cs" %>
<%@ MasterType VirtualPath="~/expform.master" %>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
	<div class="valdiv">
		<asp:ValidationSummary id="ValidationSummary1" runat="server" Width="100%" meta:resourcekey="ValidationSummary1Resource1"></asp:ValidationSummary>
	</div>
	<div class="inputpanel">
		<div class="inputpaneltitle">
			<asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">Change Advance Amount</asp:Label></div>
		<table>
			<tr>
				<td class="labeltd">
					<asp:Label id="lblamount" runat="server" meta:resourcekey="lblamountResource1">New Amount:</asp:Label></td>
				<td class="inputtd">
					<asp:TextBox id="txtamount" runat="server" meta:resourcekey="txtamountResource1"></asp:TextBox></td>
				<td>
					<asp:RequiredFieldValidator id="reqamount" runat="server" ErrorMessage="Please enter the amount required" ControlToValidate="txtamount" meta:resourcekey="reqamountResource1">*</asp:RequiredFieldValidator>
					<asp:CompareValidator id="compamount" runat="server" ErrorMessage="Please enter a valid amount for this advance"
						ControlToValidate="txtamount" Operator="DataTypeCheck" Type="Currency" meta:resourcekey="compamountResource1">*</asp:CompareValidator></td>
			</tr>
		</table>
	</div>
	<div class="inputpanel">
		<asp:ImageButton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" meta:resourcekey="cmdokResource1"></asp:ImageButton>&nbsp;&nbsp;<a href="adminfloats.aspx"><img src="../buttons/cancel_up.gif"></a></div>

    </asp:Content>

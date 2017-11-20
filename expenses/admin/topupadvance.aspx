<%@ Page language="c#" Inherits="expenses.admin.topupadvance" MasterPageFile="~/expform.master" Codebehind="topupadvance.aspx.cs" %>
<%@ MasterType VirtualPath="~/expform.master" %>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
	<div class="valdiv">
		<asp:ValidationSummary id="ValidationSummary1" runat="server" meta:resourcekey="ValidationSummary1Resource1"></asp:ValidationSummary>
	</div>
	<div class="inputpanel">
	    <div><asp:Literal ID="litUpdateResponse" runat="server"></asp:Literal></div>
		<div class="inputpaneltitle">
            <asp:Label ID="lbltopup" runat="server" Text="Top-up Advance" meta:resourcekey="lbltopupResource1"></asp:Label></div>
		<table>
			<tr><td class="labeltd"><asp:Label id="lblforeignamount" runat="server" meta:resourcekey="lblamountResource1">Amount in <asp:Label ID="lblForeignCurrencyType" runat="server"></asp:Label></asp:Label></td><td><asp:TextBox id="txtForeignAmount" runat="server" Width="100px" meta:resourcekey="txtamountResource1"></asp:TextBox></td><td><asp:RequiredFieldValidator id="rfTxtForeignAmount" runat="server" ErrorMessage="Please enter the amount you would like to top-up the advance by" ControlToValidate="txtForeignAmount" meta:resourcekey="reqamountResource1">*</asp:RequiredFieldValidator></td></tr>
		</table>
	</div>
	<div class="inputpanel">
		<asp:ImageButton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" meta:resourcekey="cmdokResource1"></asp:ImageButton>&nbsp;&nbsp;
		<asp:ImageButton id="cmdcancel" runat="server" ImageUrl="../buttons/cancel_up.gif" CausesValidation="False" meta:resourcekey="cmdcancelResource1"></asp:ImageButton>
	</div>

    </asp:Content>


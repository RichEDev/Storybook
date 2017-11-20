<%@ Page language="c#" Inherits="expenses.admin.rejectadvance" MasterPageFile="~/expform.master" Codebehind="rejectadvance.aspx.cs" %>
<%@ MasterType VirtualPath="~/expform.master" %>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
	<div class="valdiv"><asp:ValidationSummary id="ValidationSummary2" runat="server" meta:resourcekey="ValidationSummary2Resource1"></asp:ValidationSummary>
	</div>
	<div class="inputpanel">
		<div class="inputpaneltitle">Reject Advance</div>
		<TABLE>
			<TR>
				<TD class="labeltd" vAlign="top">
					<asp:Label id="lblreason" runat="server" meta:resourcekey="lblreasonResource1">Reason for Rejection</asp:Label></TD>
				<TD class="inputtd" vAlign="top">
					<asp:TextBox id="txtreason" runat="server" TextMode="MultiLine" Height="120px" Width="272px" meta:resourcekey="txtreasonResource1"></asp:TextBox></TD>
				<td>
					<asp:RequiredFieldValidator id="reqreason" runat="server" ErrorMessage="Please enter a Reason for rejecting this advance"
						ControlToValidate="txtreason" meta:resourcekey="reqreasonResource1">*</asp:RequiredFieldValidator></td>
			</TR>
		</TABLE>
	</div>
	<div class="inputpanel">
		<asp:ImageButton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" meta:resourcekey="cmdokResource1"></asp:ImageButton>&nbsp;&nbsp;
		<asp:ImageButton id="cmdcancel" runat="server" ImageUrl="../buttons/cancel_up.gif" CausesValidation="False" meta:resourcekey="cmdcancelResource1"></asp:ImageButton>
	</div>

  </asp:Content>


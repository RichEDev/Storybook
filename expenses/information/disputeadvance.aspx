
<%@ Page language="c#" Inherits="expenses.information.disputeadvance" MasterPageFile="~/expform.master" Codebehind="disputeadvance.aspx.cs" %>
<%@ MasterType VirtualPath="~/expform.master" %>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
	<div class="valdiv">
		<asp:ValidationSummary id="ValidationSummary1" runat="server" meta:resourcekey="ValidationSummary1Resource1"></asp:ValidationSummary>
	</div>
	<div class="inputpanel">
		<div class="inputpaneltitle">
            <asp:Label ID="lbldispute" runat="server" Text="Dispute Advance" meta:resourcekey="lbldisputeResource1"></asp:Label></div>
		<TABLE>
			<TR>
				<TD class="labeltd" vAlign="top">
					<asp:Label id="lblreason" runat="server" meta:resourcekey="lblreasonResource1">Reason for Dispute</asp:Label></TD>
				<TD class="inputtd" vAlign="top">
					<asp:TextBox id="txtreason" runat="server" TextMode="MultiLine" Width="329px" Height="104px" meta:resourcekey="txtreasonResource1"></asp:TextBox></TD>
				<td>
					<asp:RequiredFieldValidator id="reqreason" runat="server" ErrorMessage="Please enter a dispute for this advance"
						ControlToValidate="txtreason" meta:resourcekey="reqreasonResource1">*</asp:RequiredFieldValidator></td>
			</TR>
		</TABLE>
	</div>
	<div class="inputpanel">
		<asp:ImageButton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" meta:resourcekey="cmdokResource1"></asp:ImageButton>&nbsp;&nbsp;
		<asp:ImageButton id="cmdcancel" runat="server" ImageUrl="../buttons/cancel_up.gif" CausesValidation="False" meta:resourcekey="cmdcancelResource1"></asp:ImageButton></div>

    </asp:Content>


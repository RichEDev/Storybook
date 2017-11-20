<%@ Page language="c#" Inherits="expenses.dispute" MasterPageFile="~/expform.master" Codebehind="dispute.aspx.cs" %>
<%@ MasterType VirtualPath="~/expform.master" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
<script language="javascript" type="text/javascript">
    function checkReasonLength(sender, args) {
        if (args.Value.length > 3800) {

            args.IsValid = false;

        }
    }
</script>
	<div class="valdiv"><asp:ValidationSummary id="ValidationSummary1" runat="server" meta:resourcekey="ValidationSummary1Resource1"></asp:ValidationSummary></div>
	<div class="inputpanel">
		<div class="inputpaneltitle">
            <asp:Label ID="lblgeneral" runat="server" Text="General Details" meta:resourcekey="lblgeneralResource1"></asp:Label></div>
		<table>
			<TR>
				<td class="labeltd" valign="top">
					<asp:Label id="lbldispute" runat="server" meta:resourcekey="lbldisputeResource1">Dispute:</asp:Label></td>
				<td class="inputtd">
					<asp:TextBox id="txtdispute" runat="server" TextMode="MultiLine" Height="104px" Width="329px" meta:resourcekey="txtdisputeResource1"></asp:TextBox><asp:RequiredFieldValidator id="reqdispute" runat="server" ErrorMessage="Please enter a dispute for this expense"
						ControlToValidate="txtdispute" meta:resourcekey="reqdisputeResource1">*</asp:RequiredFieldValidator><asp:CustomValidator ID="CustomValidator1" runat="server" ErrorMessage="The reason cannot be longer than 3800 characters." ControlToValidate="txtdispute" ClientValidationFunction="checkReasonLength" Text="*"></asp:CustomValidator></td>
			</TR>
		</table>
	</div>
	<div class="inputpanel">
		<asp:ImageButton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" meta:resourcekey="cmdokResource1"></asp:ImageButton>&nbsp;&nbsp;
		<asp:ImageButton id="cmdcancel" runat="server" ImageUrl="~/buttons/cancel_up.gif" CausesValidation="False" meta:resourcekey="cmdcancelResource1"></asp:ImageButton>
	</div>
	<asp:TextBox id="txtexpenseid" runat="server" Visible="False" meta:resourcekey="txtexpenseidResource1"></asp:TextBox>
	<asp:TextBox id="txtclaimid" runat="server" Visible="False" meta:resourcekey="txtclaimidResource1"></asp:TextBox>
       </asp:Content>


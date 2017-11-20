<%@ Page Language="C#" MasterPageFile="~/expform.master" AutoEventWireup="true" Inherits="deletereason" Title="Untitled Page" Codebehind="deletereason.aspx.cs" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>
<%@ MasterType VirtualPath="~/expform.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" Runat="Server">
<script language="javascript" type="text/javascript">
    function checkReasonLength(sender, args) {
        if (args.Value.length > 3800) {

            args.IsValid = false;

        }
    }
</script>
	<div class=valdiv>
			<asp:ValidationSummary id="ValidationSummary1" runat="server" meta:resourcekey="ValidationSummary1Resource1"></asp:ValidationSummary>
			</div>
			<div class=inputpanel>
				<div class=inputpaneltitle>
                    <asp:Label ID="lblgeneral" runat="server" Text="General Details" meta:resourcekey="lblgeneralResource1"></asp:Label></div>
				<TABLE>
					
					<TR>
						<TD valign="top" class=labeltd>
                            <asp:Label ID="lblreason" runat="server" Text="Label" meta:resourcekey="lblreasonResource1"></asp:Label></TD>
						<TD class=inputtd>
							<asp:TextBox id="txtreason" runat="server" TextMode="MultiLine" Width="329px" Height="104px" meta:resourcekey="txtreasonResource1"></asp:TextBox><asp:RequiredFieldValidator id="reqreason" runat="server" ErrorMessage="Please enter a Reason" ControlToValidate="txtreason" meta:resourcekey="reqreasonResource1">*</asp:RequiredFieldValidator><asp:CustomValidator ID="CustomValidator1" runat="server" ErrorMessage="The reason cannot be longer than 3800 characters." ControlToValidate="txtreason" ClientValidationFunction="checkReasonLength" Text="*"></asp:CustomValidator></TD>
					</TR>
				</TABLE>
			</div>
			<div class=inputpanel>
			<asp:ImageButton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" OnClick="cmdok_Click" meta:resourcekey="cmdokResource1"></asp:ImageButton>&nbsp;&nbsp;
				<asp:ImageButton id="cmdcancel" runat="server" ImageUrl="~/buttons/cancel_up.gif" CausesValidation="False" OnClick="cmdcancel_Click" meta:resourcekey="cmdcancelResource1"></asp:ImageButton>
			</div>
</asp:Content>


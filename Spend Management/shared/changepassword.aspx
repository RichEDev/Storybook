<%@ Page language="c#" Inherits="Spend_Management.changepassword" MasterPageFile="~/masters/smForm.master" Codebehind="changepassword.aspx.cs" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
    <div>
	<div class="valdiv">
	    <asp:Label id="lbltodo" runat="server" meta:resourcekey="lbltodoResource1"  Text="Label"></asp:Label>
	    <asp:ValidationSummary id="ValidationSummary1" runat="server" meta:resourcekey="ValidationSummary1Resource1"></asp:ValidationSummary>
	    <asp:Label id="lblmsg" runat="server" ForeColor="Red" Visible="False" meta:resourcekey="lblmsgResource1">Label</asp:Label>
	</div>
	<div class="formpanel formpanel_padding">
	    <div class="sectiontitle">Change Password</div>
	    <span id="spanOldPassword" runat="server"><div class="twocolumn" runat="server" id="divTwoCol"><asp:Label ID="lblold" runat="server" Text="Old Password" AssociatedControlID="txtold" meta:resourcekey="lbloldResource1"></asp:Label><span class="inputs"><asp:TextBox ID="txtold" runat="server" TextMode="Password"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator id="reqold" runat="server" ErrorMessage="Please enter the old password" ControlToValidate="txtold" meta:resourcekey="reqoldResource1">*</asp:RequiredFieldValidator></span><span class="inputtooltipfield">&nbsp;</span></div></span>
	    <div class="twocolumn"><asp:Label id="lblnew" runat="server" meta:resourcekey="lblnewResource1" AssociatedControlID="txtnew">New Password</asp:Label><span class="inputs"><asp:TextBox id="txtnew" runat="server" TextMode="Password" meta:resourcekey="txtnewResource1"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator id="reqpassword" runat="server" ErrorMessage="Please enter the new password" ControlToValidate="txtnew" meta:resourcekey="reqpasswordResource1">*</asp:RequiredFieldValidator></span><span class="inputtooltipfield">&nbsp;</span><asp:Label id="lblrenew" runat="server" meta:resourcekey="lblrenewResource1" AssociatedControlID="txtrenew">Confirm New Password</asp:Label><span class="inputs"><asp:TextBox id="txtrenew" runat="server" TextMode="Password" meta:resourcekey="txtrenewResource1"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator id="compnew" runat="server" ControlToValidate="txtrenew" ErrorMessage="Your new password and confirmation must be identical" ControlToCompare="txtnew" meta:resourcekey="compnewResource1">*</asp:CompareValidator><asp:RequiredFieldValidator id="reqrenew" runat="server" ErrorMessage="Please confirm the new password" ControlToValidate="txtrenew" meta:resourcekey="reqrenewResource1">*</asp:RequiredFieldValidator></span><span class="inputtooltipfield">&nbsp;</span></div>
        <asp:Literal ID="litpolicy" runat="server" meta:resourcekey="litpolicyResource1"></asp:Literal>
        </div>    
        
        <div class="formpanel formpanel_padding">
	    <div class="formbuttons"><asp:ImageButton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" meta:resourcekey="cmdokResource1"></asp:ImageButton> <asp:ImageButton id="cmdcancel" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" CausesValidation="False" meta:resourcekey="cmdcancelResource1"></asp:ImageButton></div>
	</div>
        </div>
    </asp:Content>


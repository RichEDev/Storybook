<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" Inherits="admin_aeemailsuffix" Title="Untitled Page" Codebehind="aeemailsuffix.aspx.cs" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" Runat="Server">

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" Runat="Server">
<div class="inputpanel">
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True" ShowSummary="False" meta:resourcekey="ValidationSummary1Resource1" />
    <div class="inputpaneltitle">
        <asp:Label ID="lblgeneraldetails" runat="server" Text="General Details" meta:resourcekey="lblgeneraldetailsResource1"></asp:Label></div>
    <table>
        <tr><td class="labeltd mandatory">
            <asp:Label ID="lblemailsuffix" runat="server" Text="E-mail Suffix" meta:resourcekey="lblemailsuffixResource1"></asp:Label></td><td class="inputtd">
            <asp:TextBox ID="txtsuffix" runat="server" meta:resourcekey="txtsuffixResource1"></asp:TextBox></td><td>
                <asp:RequiredFieldValidator ID="reqsuffix" runat="server" ErrorMessage="Please enter the e-mail suffix in the box provided" SetFocusOnError="True" ControlToValidate="txtsuffix" meta:resourcekey="reqsuffixResource1"></asp:RequiredFieldValidator></td><td><img id="imgtooltip348" onclick="SEL.Tooltip.Show('2a65aa26-75d6-4bfb-ae2a-4179f266eea6', 'ex', this);" src="../../shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon"/></td></tr>
    </table>
    
    
</div>
<div class="inputpanel">
    <asp:ImageButton ID="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" OnClick="cmdok_Click" meta:resourcekey="cmdokResource1" />&nbsp;&nbsp;<asp:ImageButton
        ID="cmdcancel" runat="server" ImageUrl="../../shared/images/buttons/cancel_up.gif" OnClick="cmdcancel_Click" CausesValidation="false" meta:resourcekey="cmdcancelResource1" />
</div>
<div class="inputpanel">
    <asp:Label ID="lblmsg" runat="server" Text="Label" ForeColor="Red" Visible="False" meta:resourcekey="lblmsgResource1"></asp:Label>
</div>
</asp:Content>


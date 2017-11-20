<%@ Page Language="C#" MasterPageFile="~/masters/AnonymousUser.master" AutoEventWireup="true" Inherits="Spend_Management.verify" Title="Employee Verification" Codebehind="verify.aspx.cs" %>
<asp:Content ID="Content3" ContentPlaceHolderID="pageContents" runat="Server">
<div class="formpanel" style="width: 800px;">
    <asp:Label ID="lblverify" runat="server" Text="Label">Thank you for confirming your details.  Your <asp:Literal ID="litMsgBrand" runat="server" Text="Expenses"></asp:Literal> account will now be activated by your system administrator. 

Once your account has been activated, you will receive confirmation by email and be able to use <asp:Literal ID="litMsgBrand2" runat="server" Text="Expenses"></asp:Literal>.</asp:Label>


</div>
</asp:Content>


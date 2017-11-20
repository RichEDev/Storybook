<%@ Page Language="C#" MasterPageFile="~/exptemplate.master" AutoEventWireup="true" CodeBehind="unallocated_cards.aspx.cs" Inherits="expenses.admin.unallocated_cards" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>
<%@ Register src="unallocatedcardnumbers.ascx" tagname="unallocatedcardnumbers" tagprefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
    <div class="inputpanel">
    <uc2:unallocatedcardnumbers ID="usrunallocatedcardnumbers" runat="server" />
    </div>
    <div class="inputpanel"><a href="statements.aspx"><img alt="Close" src="/shared/images/buttons/btn_close.png" border="0" /></a></div>
</asp:Content>

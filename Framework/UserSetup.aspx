<%@ Page MasterPageFile="~/FWMaster.master" Language="vb" AutoEventWireup="false"
    Inherits="Framework2006.UserSetup" CodeFile="UserSetup.aspx.vb" %>

<%@ MasterType VirtualPath="~/FWMaster.master" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
    <div class="inputpanel">
        <asp:Label ID="txtErrorString" runat="server" ForeColor="Red"></asp:Label></div>
    <div class="inputpanel">
        <div class="inputpaneltitle">
            Framework User Logons</div>
        <asp:Literal runat="server" ID="litUserList"></asp:Literal>
    </div>
    <div class="inputpanel">
        <asp:ImageButton runat="server" ID="cmdClose" ImageUrl="./buttons/page_close.gif"
            CausesValidation="false" />
    </div>
</asp:Content>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    <asp:LinkButton runat="server" ID="lnkNew" CssClass="submenuitem">New</asp:LinkButton>
    <asp:LinkButton runat="server" ID="lnkReset" CssClass="submenuitem">Reset Counters</asp:LinkButton>
    <asp:LinkButton ID="lnkLive" runat="server" CssClass="submenuitem" Visible="false">View Active</asp:LinkButton>
    <asp:LinkButton runat="server" ID="lnkArchived" CssClass="submenuitem">View Archived</asp:LinkButton>
    <a href="./help_text/default_csh.htm#1004" target="_blank" class="submenuitem">Help</a>
</asp:Content>

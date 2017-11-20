<%@ Page MasterPageFile="~/FWMaster.master" Language="vb" AutoEventWireup="false" Inherits="Framework2006.ViewHistory" CodeFile="ViewHistory.aspx.vb" %>
<%@ MasterType VirtualPath="~/FWMaster.master" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
<div class="inputpanel">
<div class="inputpaneltitle"><asp:Label id="lblTitle" runat="server"></asp:Label></div>
<asp:Literal runat="server" ID="litHistoryGrid"></asp:Literal>
</div>
<div class="inputpanel">
<asp:ImageButton runat="server" ID="cmdOK" ImageUrl="./buttons/page_close.gif" />
</div>
</asp:Content>
<asp:Content ID="Content4" runat="server" contentplaceholderid="contentmenu">
    <a href="./help_text/default_csh.htm#0" target="_blank" class="submenuitem">Help</a>
</asp:Content>


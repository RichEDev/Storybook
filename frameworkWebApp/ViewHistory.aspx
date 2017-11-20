<%@ Page MasterPageFile="~/FWMaster.master" Language="vb" AutoEventWireup="false" Inherits="frameworkWebApp.Framework2006.ViewHistory" Codebehind="ViewHistory.aspx.vb" %>
<%@ MasterType VirtualPath="~/FWMaster.master" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
<div class="inputpanel">
<div class="inputpaneltitle"><asp:Label id="lblTitle" runat="server"></asp:Label></div>
<asp:Literal runat="server" ID="litHistoryGrid"></asp:Literal>
</div>
<div class="inputpanel">
<asp:ImageButton runat="server" ID="cmdOK" ImageUrl="./buttons/page_close.png" />
</div>
</asp:Content>
<asp:Content ID="Content4" runat="server" contentplaceholderid="contentmenu">
</asp:Content>


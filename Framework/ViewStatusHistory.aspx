<%@ Page MasterPageFile="~/FWMaster.master" Language="vb" AutoEventWireup="false" Inherits="Framework2006.ViewStatusHistory"
    CodeFile="ViewStatusHistory.aspx.vb" %>
<%@ MasterType VirtualPath="~/FWMaster.master" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
    <div class="inputpanel">
        <div class="inputpaneltitle">Status History for Invoice
            <asp:Label ID="lblTitle" runat="server"></asp:Label></div>
        <asp:Literal runat="server" ID="litStatusData"></asp:Literal>
    </div>
    <div class="inputpanel">
    <asp:ImageButton runat="server" ID="cmdClose" ImageUrl="./buttons/page_close.gif" CausesValidation="false" />
    </div>
    <div class="inputpanel">
        <input id="hiddenComment" style="z-index: 101; left: 8px; position: absolute; top: 0px"
            type="hidden">
        <asp:Label ID="hiddenDesc" Style="z-index: 102; left: 8px; position: absolute; top: 48px"
            runat="server" Visible="False"></asp:Label>
        <asp:Label ID="hiddenInvID" Style="z-index: 103; left: 8px; position: absolute; top: 24px"
            runat="server" Visible="False"></asp:Label>
        <input id="hiddenLogId" style="z-index: 104; left: 8px; position: absolute; top: 64px"
            type="hidden">
    </div>
</asp:Content>
<asp:Content ID="Content4" runat="server" contentplaceholderid="contentmenu">
<a href="./help_text/default_csh.htm#0" target="_blank" class="submenuitem">Help</a>
                
</asp:Content>


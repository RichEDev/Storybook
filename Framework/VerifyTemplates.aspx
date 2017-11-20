<%@ Page MasterPageFile="~/FWMaster.master" Language="vb"
    AutoEventWireup="false" Inherits="Framework2006.VerifyTemplates" CodeFile="VerifyTemplates.aspx.vb" %>

<%@ MasterType VirtualPath="~/FWMaster.master" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
    <div class="inputpanel">
        <div class="inputpaneltitle">
            View / Edit Uploaded Email Templates</div>
        <asp:Literal ID="litTemplates" runat="server"></asp:Literal>
    </div>
    <div class="inputpanel">
    <asp:ImageButton runat="server" ID="cmdClose" ImageUrl="./buttons/page_close.gif" CausesValidation="false" />
    </div>
</asp:Content>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
<asp:LinkButton runat="server" ID="lnkUpload" CssClass="submenuitem">Upload Template</asp:LinkButton>
<a href="./help_text/default_csh.htm#1167" target="_blank" class="submenuitem">Help</a>
</asp:Content>

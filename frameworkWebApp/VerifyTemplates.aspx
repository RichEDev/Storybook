<%@ Page MasterPageFile="~/FWMaster.master" Language="vb"
    AutoEventWireup="false" Inherits="frameworkWebApp.Framework2006.VerifyTemplates" Codebehind="VerifyTemplates.aspx.vb" %>

<%@ MasterType VirtualPath="~/FWMaster.master" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
    <div class="formpanel formpanel_padding">
        <div class="sectiontitle">View / Edit Uploaded Email Templates</div>
        <div class="formpanel formpanel_padding"><asp:Literal ID="litTemplates" runat="server"></asp:Literal></div>
        <div class="formbuttons">
            <asp:ImageButton runat="server" ID="cmdClose" ImageUrl="./buttons/page_close.png" CausesValidation="false" />
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
<asp:LinkButton runat="server" ID="lnkUpload" CssClass="submenuitem">Upload Template</asp:LinkButton>
</asp:Content>

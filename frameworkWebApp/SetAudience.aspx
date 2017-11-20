<%@ Page Language="VB" MasterPageFile="~/FWMaster.master"
    AutoEventWireup="false" Inherits="frameworkWebApp.Framework2006.SetAudience" Codebehind="SetAudience.aspx.vb" %>

<%@ MasterType VirtualPath="~/FWMaster.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="Server">
<div class="inputpanel"><asp:Label runat="server" ID="lblErrorMessage" ForeColor="Red"></asp:Label></div>
    <div class="inputpanel">
    <asp:Label runat="server" ID="hiddenAudienceTable" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hiddenAudienceField" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hiddenEntityId" Visible="False">0</asp:Label>
        <div class="inputpaneltitle">
            Select Team Audience</div>
        <asp:CheckBoxList ID="chkTeamList" runat="server" RepeatColumns="3" RepeatDirection="Horizontal">
        </asp:CheckBoxList></div>
    <div class="inputpanel">
        <div class="inputpaneltitle">
            Select Individual Audience</div>
        <asp:CheckBoxList ID="chkPersonList" runat="server" RepeatColumns="3" RepeatDirection="Horizontal">
        </asp:CheckBoxList></div>
        <div class="inputpanel">
        <asp:ImageButton runat="server" ID="cmdOK" ImageUrl="./buttons/update.gif" />&nbsp;
        <asp:ImageButton runat="server" ID="cmdCancel" ImageUrl="./buttons/cancel.gif" CausesValidation="false" />
        </div>
</asp:Content>

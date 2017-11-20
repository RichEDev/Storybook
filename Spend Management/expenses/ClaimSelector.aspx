<%@ Page Title="" Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="ClaimSelector.aspx.cs" Inherits="Spend_Management.expenses.ClaimSelector" EnableSessionState="ReadOnly" %>

<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<%@ Register Src="../shared/usercontrols/ClaimSelection.ascx" TagName="ClaimSelection" TagPrefix="sm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
    <div class="formpanel formpanel_padding">
        <sm:ClaimSelection ID="ClaimSelection1" runat="server" />
    </div>
</asp:Content>

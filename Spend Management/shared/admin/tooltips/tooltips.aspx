<%@ Page Language="C#" MasterPageFile="~/masters/smTemplate.master" AutoEventWireup="true" CodeBehind="tooltips.aspx.cs" Inherits="Spend_Management.tooltips" Title="Tooltips" %>
<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>

<asp:Content ID="Content4" ContentPlaceHolderID="styles" runat="server">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
    <script src="/shared/javaScript/tooltips.js?date=22022018"></script>
    <div class="formpanel formpanel_padding">
        <div id="tooltipsGrid"></div>
    <div class="formbuttons">
    <asp:HyperLink ID="hlClose" runat="server"><asp:Image ID="btnClose" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png"></asp:Image></asp:HyperLink>
    </div>
    </div>
</asp:Content>

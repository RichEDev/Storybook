<%@ Page Language="C#" MasterPageFile="~/menu.master" AutoEventWireup="true" Inherits="mydetailsmenu" Title="Untitled Page" Codebehind="mydetailsmenu.aspx.cs" %>
<%@ MasterType VirtualPath="~/menu.master" %>
<asp:Content ContentPlaceHolderID="menuHeader" runat="server" ID="menuHeaderContent">
<asp:Literal ID="litMenuHeader" runat="server"></asp:Literal>
</asp:Content>
<%@ Page Language="C#" MasterPageFile="~/exptemplate.master" AutoEventWireup="true" CodeBehind="aemycar.aspx.cs" Inherits="expenses.aeCarPage" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>
<%@ Register TagPrefix="aeCars" TagName="aeCar" Src="~/aeCars.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
    <aeCars:aeCar runat="server" ID="aeCar" />
    <asp:Literal ID="litCars" runat="server"></asp:Literal>
</asp:Content>

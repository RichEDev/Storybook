<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="mycars.aspx.cs" Inherits="Spend_Management.mycars" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<%@ Register TagPrefix="aeCars" TagName="aeCar" Src="~/shared/usercontrols/aeCars.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">

<aeCars:aeCar runat="server" ID="aeCar" />
    <asp:Literal ID="litMyCars" runat="server"></asp:Literal></asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="scripts" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $(".formpanel:first").css("padding-bottom", 0);
        });
    </script>
    
</asp:Content>

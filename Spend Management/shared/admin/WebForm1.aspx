<%@ Page Title="" Language="C#" MasterPageFile="~/masters/smPagedForm.master" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="Spend_Management.shared.admin.WebForm1" %>
<%@ MasterType VirtualPath="~/masters/smPagedForm.master" %>
<%@ Register Namespace="AjaxControlToolkit.HTMLEditor" Assembly="AjaxControlToolkit" TagPrefix="HTMLEditor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
    <a href="javascript:changePage('General');" id="lnkGeneral" class="selectedPage">General
        Options</a> <a href="javascript:changePage('HelpInformation');" id="lnkHelpInformation">
                                Help Information</a>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="contentoptions" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">

    <div id="divPages">
        <div id="pgGeneral" class="primaryPage"></div>
        <div id="pgHelpInformation" class="subPage"><!-- style="display: none;">-->
<div class="formpanel"><div class="onecolumnlarge"><asp:Label ID="thing" runat="server" AssociatedControlID="testEditor">LABELSARETHEDEVIL</asp:Label>
    <span class="inputs"><HTMLEditor:Editor ID="testEditor" runat="server"></HTMLEditor:Editor></span></div>
    </div></div></div>
   <div class="formpanel"><div class="formbuttons"><asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/images/btn_down.gif" onclick="ImageButton1_Click" /></div></div>
</asp:Content>

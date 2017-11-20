<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="custom_entities.aspx.cs" Inherits="Spend_Management.custom_entities" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
    <div id="divNewCustomEntity" runat="server">
        <a href="aecustomentity.aspx" class="submenuitem">New GreenLight</a>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
<asp:ScriptManagerProxy runat="server" ID="smProxy">
<Scripts>
    <asp:ScriptReference Path="~/shared/javaScript/customEntityAdmin.js" />
</Scripts>
<Services>
    <asp:ServiceReference Path="~/shared/webServices/svcCustomEntities.asmx" />
</Services>
</asp:ScriptManagerProxy>
<script type="text/javascript" language="javascript">
    $(document).ready(function () {
        $('.buttonInner').focus(function () {
            $(this).css('background-position', 'left -155px');
            $(this).parent().css('background-position', 'right -155px');
        });
        $('.buttonInner').focusout(function () {
            $(this).css('background-position', 'left top');
            $(this).parent().css('background-position', 'right top');
        });
        $('.smallbuttonInner').focus(function () {
            $(this).css('background-position', 'left -34px');
            $(this).parent().css('background-position', 'right -34px');
        });
        $('.smallbuttonInner').focusout(function () {
            $(this).css('background-position', 'left top');
            $(this).parent().css('background-position', 'right top');
        });            
    });
</script>
    <div class="formpanel formpanel_padding">
        <div class="sectiontitle">GreenLight Management</div>
        <asp:Literal ID="litgrid" runat="server"></asp:Literal>
        <div class="formbuttons">
            <helpers:CSSButton ID="btnClose" runat="server" Text="close" OnClick="btnClose_Click" CausesValidation="false" />
        </div>
    </div>   

</asp:Content>

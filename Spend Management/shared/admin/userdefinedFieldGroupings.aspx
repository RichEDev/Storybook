<%@ Page Language="C#" MasterPageFile="~/masters/smTemplate.master" AutoEventWireup="true" CodeBehind="userdefinedFieldGroupings.aspx.cs" Inherits="Spend_Management.userdefinedFieldGroupings" %>
<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
    <a href="aeuserdefinedgrouping.aspx" class="submenuitem" ID="lnkAddUDFGroup" runat="server">New Grouping</a>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">    
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
    <script type="text/javascript" language="javascript" src="../javaScript/userdefinedGroupings.js"></script>
    <div class="formpanel formpanel_padding">
        <asp:Literal ID="litgrid" runat="server"></asp:Literal>
        <div class="formbuttons">
            <asp:ImageButton ID="btnCancel" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" OnClick="btnCancel_Click" CausesValidation="False" />
        </div>
    </div>
</asp:Content>

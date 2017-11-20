<%@ Page Language="C#" MasterPageFile="~/masters/smTemplate.master" AutoEventWireup="true" CodeBehind="tooltips.aspx.cs" Inherits="Spend_Management.tooltips" Title="Tooltips" %>
<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>
<%@ Register TagPrefix="igtbl" Namespace="Infragistics.WebUI.UltraWebGrid" Assembly="Infragistics4.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>

<asp:Content ID="Content4" ContentPlaceHolderID="styles" runat="server">
    <style>
        td:first-child {
            text-align: left!important;
        }

        tbody th:first-child {
            text-align: left!important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
<div class="inputpanel tooltiptableDesign">
    <igtbl:UltraWebGrid ID="gridtooltips" runat="server"
        oninitializelayout="gridtooltips_InitializeLayout" SkinId="gridskin" 
        oninitializerow="gridtooltips_InitializeRow">
        <DisplayLayout ViewType="OutlookGroupBy"></DisplayLayout>
    </igtbl:UltraWebGrid>
    </div>

    <div class="formpanel formpanel_padding">
    <div class="formbuttons">
    <asp:HyperLink ID="hlClose" runat="server"><asp:Image ID="btnClose" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png"></asp:Image></asp:HyperLink>
    </div>
    </div>
</asp:Content>

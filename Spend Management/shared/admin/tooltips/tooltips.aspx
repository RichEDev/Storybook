<%@ Page Language="C#" MasterPageFile="~/masters/smTemplate.master" AutoEventWireup="true" CodeBehind="tooltips.aspx.cs" Inherits="Spend_Management.tooltips" Title="Tooltips" %>
<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>

<asp:Content ID="Content4" ContentPlaceHolderID="styles" runat="server">
    <link href="/static/syncfusion/css/web/default-theme/ej.web.all.css" rel="stylesheet" />
    <style>
        th.e-headercell {
            color: #282827!important;
        }
    </style>

</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            PageMethods.GetGriDataSet(InitialiseGrid);

            function InitialiseGrid(data) {

                var gridData = JSON.parse(data);

                $("#tooltipsGrid").ejGrid({
                    
                    dataSource: gridData,
                    columns: [
                        { field: "page", headerText: "Page" },
                        { field: "description", headerText: "Descrption" },
                        { field: "text", headerText: "Text" }
                    ],
                    allowPaging: true,
                    allowGrouping: true,
                    allowSorting: true,
                    groupSettings: { groupedColumns: ["page"] },
                    allowTextWrap: true,
                    textWrapSettings: { wrapMode: "both" }
                });
            }
        });
    </script>
    <div class="formpanel formpanel_padding">
        <div id="tooltipsGrid"></div>
    <div class="formbuttons">
    <asp:HyperLink ID="hlClose" runat="server"><asp:Image ID="btnClose" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png"></asp:Image></asp:HyperLink>
    </div>
    </div>
</asp:Content>

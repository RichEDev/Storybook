<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true"
    CodeBehind="ImportHistory.aspx.cs" Inherits="Spend_Management.ImportHistory" %>

<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="contentmain" runat="server">
<script language="javascript" type="text/javascript">
    var modalID = '<%=logModalID %>';
    var filter_ddlID = '<%=ddlFilterID %>';
    var ddlLogViewType = '<% = ddlViewType.ClientID %>';
    var ddlLogElementType = '<% = ddlElementType.ClientID %>';
</script>
    <asp:ScriptManagerProxy runat="server" ID="smproxy">
        <Services>
            <asp:ServiceReference Path="~/shared/webServices/svcLogging.asmx" />
        </Services>
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/importexport.js" />
            <asp:ScriptReference Path="~/shared/javaScript/shared.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
    <div class="formpanel formpanel_padding">
        <div class="sectiontitle">Import History</div>
        <div class="twocolumn">
            <asp:Label runat="server" ID="lblFilter" AssociatedControlID="lstFilter">Filter</asp:Label>
            <span class="inputs">
                <asp:DropDownList runat="server" ID="lstFilter" CssClass="fillspan">
                    <asp:ListItem Text="All" Value="-1"></asp:ListItem>
                </asp:DropDownList>
            </span>
        </div>
        <div id="historygridholder">
            <asp:PlaceHolder runat="server" ID="phHistoryGrid"></asp:PlaceHolder>
        </div>
        <div class="formbuttons">
            <asp:ImageButton runat="server" ID="cmdClose" ImageUrl="~/shared/images/buttons/btn_close.png" AlternateText="Close" onclick="cmdClose_Click" />
        </div>
    </div>
    <asp:Panel ID="pnlLog" runat="server" CssClass="modalpanel">
        <div class="formpanel">
            <div class="sectiontitle">Import Log</div>
            <div class="twocolumn"><asp:Label runat="server" ID="lblViewType" AssociatedControlID="ddlViewType">Filter View</asp:Label><span class="inputs"><asp:DropDownList ID="ddlViewType" runat="server" onchange="javascript:ChangeLogViewType();" CssClass="fillspan"></asp:DropDownList></span><asp:Label runat="server" ID="lblElementType" AssociatedControlID="ddlElementType">Filter Element</asp:Label><span class="inputs"><asp:DropDownList ID="ddlElementType" runat="server" onchange="javascript:ChangeLogViewType();" CssClass="fillspan"></asp:DropDownList></span></div>
            <div id="logDiv" style="height: 500px; width: 100%; overflow: scroll;">
            </div>
            <div class="formbuttons">
                <asp:Image ID="btnCloseLog" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" />&nbsp;<asp:Image ID="btnPrint" runat="server" ImageUrl="~/shared/images/buttons/btn_print.png" onclick="javascript:viewLog();" />
            </div>
        </div>
        
    </asp:Panel>
    <cc1:ModalPopupExtender CancelControlID="btnCloseLog" ID="modLog" runat="server"
        TargetControlID="lnklog" PopupControlID="pnlLog" BackgroundCssClass="modalBackground">
    </cc1:ModalPopupExtender>
    <asp:HyperLink ID="lnklog" runat="server" Style="display: none;">HyperLink</asp:HyperLink>
</asp:Content>

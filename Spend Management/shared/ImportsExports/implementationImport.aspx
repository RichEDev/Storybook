<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/masters/smTemplate.master" CodeBehind="implementationImport.aspx.cs" Inherits="Spend_Management.implementationImport" %>

<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
    
    <asp:ScriptManagerProxy ID="smp" runat="server">
         <Services>
            <asp:ServiceReference Path="~/shared/webServices/importSvc.asmx" />
            <asp:ServiceReference Path="~/shared/webServices/svcLogging.asmx" />
         </Services>
         <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/spreadsheetImport.js" />
         </Scripts>
    </asp:ScriptManagerProxy>
    
    <script language="javascript" type="text/javascript">

        var tableID = '<%=tblProgress.ClientID %>';
        var statusID = '<%=statusDiv.ClientID %>';
        var uploadDivID = '<%=UploadDiv.ClientID %>';
        var modalID = '<%=modLog.ClientID %>';
        var ddlLogViewType = '<% = ddlViewType.ClientID %>';
        var ddlLogElementType = '<% = ddlElementType.ClientID %>';
    </script>

    <div class="inputpanel" id="UploadDiv" runat="server">
        <div class="inputpanel">
            <table>
                <tr>
                    <td class="labeltd">Source File</td>
                    <td class="inputtd">
                        <asp:FileUpload ID="txtfilename" Width="300px" runat="server" />
                    </td>
                    <td>
                        <asp:RequiredFieldValidator ID="reqfilename" runat="server" ErrorMessage="Please select the file you would like to import" Text="*" ControlToValidate="txtfilename"></asp:RequiredFieldValidator>
                    </td>
                </tr>
            </table>
        </div>
        
        <div class="inputpanel">
		    <asp:ImageButton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" 
                meta:resourcekey="cmdokResource1" onclick="cmdok_Click"></asp:ImageButton>&nbsp;&nbsp;
		    <asp:Image id="cmdcancel" runat="server" style="cursor: pionter; cursor: hand" 
                ImageUrl="~/shared/images/buttons/cancel_up.gif" 
                meta:resourcekey="cmdcancelResource1" onclick="javascript:window.location = '/home.aspx';" />
	    </div>
    </div>
    
    
    <div class="inputpanel">
            <div class="inputpanel">
                <asp:Table ID="tblProgress" CellPadding="5" Width="250px" CssClass="datatbl" runat="server">
                </asp:Table>
            </div>
        
            <div class="inputpanel">
                <div id="statusDiv" runat="server" style="display:none">Validating Spreadsheet</div>
            </div>
            <div class="inputpanel" id="importProgress" style="display:none">
                <div id="importProgressBg" style="width: 250px; background-image: url('/images/exportReportBackground250px.png'); height: 20px;">
                    <div id="importDone" style="background-image: url('/images/exportReport250px.png'); height: 20px;">&nbsp;</div>
                    <div id="importPercentDone">0%</div>
                </div>
            </div>
            <div class="inputpanel">
                <a href="javascript:showLogModal();" id="logLink" style="display:none"> View Log</a>
            </div>
    </div>

    
    <asp:Panel ID="pnlLog" runat="server" CssClass="modalpanel">
        <div class="formpanel">
            <div class="sectiontitle">Import Log</div>
            <div class="twocolumn"><asp:Label runat="server" ID="lblViewType" AssociatedControlID="ddlViewType">Filter</asp:Label><span class="inputs"><asp:DropDownList ID="ddlViewType" runat="server" onchange="javascript:ChangeLogViewType(logid);" CssClass="fillspan"></asp:DropDownList></span><asp:Label runat="server" ID="lblElementType" AssociatedControlID="ddlElementType">Filter Element</asp:Label><span class="inputs"><asp:DropDownList ID="ddlElementType" runat="server" onchange="javascript:ChangeLogViewType();" CssClass="fillspan"></asp:DropDownList></span></div>
            <div id="logDiv" style="height: 500px; width: 100%; overflow: scroll">
            </div>
            <div class="formbuttons">
                <asp:Image ID="btnCloseLog" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" />&nbsp;<asp:Image ID="btnPrint" runat="server" ImageUrl="~/shared/images/buttons/btn_print.png" onclick="javascript:viewLog();" />
            </div>
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender CancelControlID="btnCloseLog" ID="modLog" runat="server" TargetControlID="lnklog" PopupControlID="pnlLog" BackgroundCssClass="modalBackground">
    </cc1:ModalPopupExtender>
    
    <asp:HyperLink ID="lnklog" runat="server" style="display:none;">HyperLink</asp:HyperLink>
    
    

</asp:Content>
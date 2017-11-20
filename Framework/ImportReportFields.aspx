<%@ Page MasterPageFile="~/FWMaster.master" Language="vb"
    AutoEventWireup="false" Inherits="Framework2006.ImportReportFields" CodeFile="ImportReportFields.aspx.vb" %>

<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ MasterType VirtualPath="~/FWMaster.master" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
    <asp:Panel runat="server" ID="UploadSingleFile">
        <div class="inputpanel">
            <div class="inputpaneltitle">
                <asp:Label ID="lblTitle" runat="server"></asp:Label></div>
            <div>
                <asp:Label ID="lblResultMsg" runat="server"></asp:Label></div>
            <table>
                <tr>
                    <td class="labeltd">
                        <asp:Label ID="lblUploadLoc" runat="server">upload location</asp:Label></td>
                    <td class="inputtd">
                        <input style="width: 350px;" id="attachment" type="file" size="50" name="attachment"
                            runat="server" /></td>
                </tr>
            </table>
        </div>
        <div class="inputpanel">
            <asp:Literal ID="litUploadData" runat="server"></asp:Literal></div>
        <div class="inputpanel">
            <asp:ImageButton runat="server" ID="cmdOK" ImageUrl="./buttons/ok.gif" />&nbsp;&nbsp;<asp:ImageButton
                runat="server" ID="cmdCancel" CausesValidation="false" ImageUrl="./buttons/cancel.gif" />
        </div>
    </asp:Panel>
    <asp:Panel runat="server" ID="UploadGroupedFiles" Visible="false">
        <div class="inputpanel">
            <div class="inputpaneltitle">
                Import Report Fields - Grouped Import
            </div>
            <table class="datatbl" style="width: 570px;">
                <tr>
                    <th colspan="3">
                        Import location should contain the following files</th>
                </tr>
                <tr>
                    <td class="row1" align="center">
                        <asp:Image runat="server" ID="imgTableDef" ImageUrl="./buttons/cross.gif" /></td>
                    <td class="row1">
                        Table definitions</td>
                    <td class="row1">
                        <asp:FileUpload ID="fuTableDef" runat="server" Width="400px" /></td>
                </tr>
                <tr>
                    <td class="row2" align="center">
                        <asp:Image runat="server" ID="imgFieldDef" ImageUrl="./buttons/cross.gif" /></td>
                    <td class="row2">
                        Field definitions</td>
                    <td class="row2">
                        <asp:FileUpload ID="fuFieldDef" runat="server" Width="400px" /></td>
                </tr>
                <tr>
                    <td class="row1" align="center">
                        <asp:Image runat="server" ID="imgQueryJDef" ImageUrl="./buttons/cross.gif" /></td>
                    <td class="row1">
                        Join tables</td>
                    <td class="row1">
                        <asp:FileUpload ID="fuQueryJDef" runat="server" Width="400px" /></td>
                </tr>
                <tr>
                    <td class="row2" align="center">
                        <asp:Image runat="server" ID="imgQueryJBDef" ImageUrl="./buttons/cross.gif" /></td>
                    <td class="row2">
                        Join breakdowns</td>
                    <td class="row2">
                        <asp:FileUpload ID="fuQueryJBDef" runat="server" Width="400px" /></td>
                </tr>
                <tr>
                    <td class="row1" align="center">
                        <asp:Image runat="server" ID="imgViewGroupDef" ImageUrl="./buttons/cross.gif" /></td>
                    <td class="row1">
                        View groups</td>
                    <td class="row1">
                        <asp:FileUpload ID="fuViewGroupDef" runat="server" Width="400px" /></td>
                </tr>
                <tr>
                    <td class="row2" align="center">
                        <asp:Image runat="server" ID="imgAllowedDef" ImageUrl="./buttons/cross.gif" /></td>
                    <td class="row2">
                        Allowed tables</td>
                    <td class="row2">
                        <asp:FileUpload ID="fuAllowedDef" runat="server" Width="400px" /></td>
                </tr>
                <tr>
                    <td class="row1" align="center">
                        <asp:Image runat="server" ID="imgRptListItemsDef" ImageUrl="./buttons/cross.gif" /></td>
                    <td class="row1">
                        Report list items</td>
                    <td class="row1">
                        <asp:FileUpload ID="fuRptListItemsDef" runat="server" Width="400px" /></td>
                </tr>
                <tr>
                    <td class="row2" align="center">
                        <asp:Image runat="server" ID="imgCommonFieldsDef" ImageUrl="./buttons/cross.gif" /></td>
                    <td class="row2">
                        Common fields</td>
                    <td class="row2">
                        <asp:FileUpload ID="fuCommonFieldsDef" runat="server" Width="400px" /></td>
                </tr>
                <tr>
                    <td colspan="3" align="center">
                        <asp:Label runat="server" ID="lblError" ForeColor="red"></asp:Label></td>
                </tr>
                <tr>
                    <td colspan="3">
                        <asp:ImageButton runat="server" ID="cmdGroupUpload" ImageUrl="./buttons/ok.gif" />&nbsp;&nbsp;
                        <asp:ImageButton runat="server" ID="cmdGUCancel" ImageUrl="./buttons/cancel.gif"
                            CausesValidation="false" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content4" runat="server" contentplaceholderid="contentmenu">
    <a href="./help_text/default_csh.htm#0" target="_blank" class="submenuitem">Help</a>                
</asp:Content>


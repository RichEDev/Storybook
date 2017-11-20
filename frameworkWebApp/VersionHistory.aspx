<%@ Page MasterPageFile="~/FWMaster.master" Language="vb"
    AutoEventWireup="false" Inherits="frameworkWebApp.Framework2006.VersionHistory" Codebehind="VersionHistory.aspx.vb" %>

<%@ MasterType VirtualPath="~/FWMaster.master" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
    <div class="inputpanel">
        <div class="inputpaneltitle">
            <asp:Label ID="lblTitle" runat="server">title</asp:Label></div>
        <asp:Literal runat="server" ID="litHistoryGrid"></asp:Literal>
    </div>
    <asp:Panel ID="panelEditFields" runat="server" Visible="false" Width="100%">
        <div class="inputpanel">
            <div class="inputpaneltitle">
                History Element Details</div>
            <table>
                <tr>
                    <td class="labeltd">
                        <asp:Label ID="lblDate" runat="server">Date :</asp:Label></td>
                    <td class="inputtd">
                        <igtxt:WebDateTimeEdit ID="txtDate" TabIndex="1" runat="server">
                        </igtxt:WebDateTimeEdit>
                    </td>
                    <td class="labeltd">
                        <asp:Label ID="lblReseller" runat="server">Reseller :</asp:Label></td>
                    <td class="inputtd">
                        <asp:DropDownList ID="lstReseller" TabIndex="2" runat="server">
                        </asp:DropDownList></td>
                </tr>
                <tr>
                    <td class="labeltd">
                        <asp:Label ID="lblComment" runat="server">Comment :</asp:Label></td>
                    <td class="inputtd" colspan="3">
                        <asp:TextBox ID="txtComment" TabIndex="3" runat="server" Rows="2" TextMode="MultiLine"
                            MaxLength="100"></asp:TextBox></td>
                </tr>
            </table>
        </div>
        <div class="inputpanel">
            <asp:ImageButton runat="server" ID="cmdUpdate" ImageUrl="./buttons/update.gif" />&nbsp;
            <asp:ImageButton runat="server" ID="cmdCancel" ImageUrl="./buttons/cancel.gif" CausesValidation="false" />
        </div>
    </asp:Panel>
    <div class="inputpanel">
       <asp:ImageButton runat="server" ID="cmdOK" CausesValidation="false" ImageUrl="./buttons/page_close.png" />
    </div>
</asp:Content>
<asp:Content ID="Content4" runat="server" contentplaceholderid="contentmenu">      
</asp:Content>


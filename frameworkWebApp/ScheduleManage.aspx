<%@ Page MasterPageFile="~/FWMaster.master" Language="vb"
    AutoEventWireup="false" Inherits="frameworkWebApp.Framework2006.ScheduleManage" Codebehind="ScheduleManage.aspx.vb" %>

<%@ MasterType VirtualPath="~/FWMaster.master" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
    <div class="inputpanel">
        <asp:Label ID="lblErrorMsg" runat="server" ForeColor="Red"></asp:Label></div>
    <asp:Panel runat="server" ID="searchPanel">
        <div class="inputpanel formpanel_padding">
            <div class="inputpaneltitle">
                Locate contracts for action</div>
            <table>
                <tr>
                    <td class="labeltd">
                        <asp:Label ID="lblSelectCriteria" runat="server">Contract Description</asp:Label></td>
                    <td class="inputtd">
                        <asp:TextBox ID="txtSelectCriteria" TabIndex="1" runat="server" ToolTip="Wildcard search to be matched against contract description"></asp:TextBox></td>
                </tr>
                <tr>
                    <td class="labeltd">
                        <asp:Label ID="lblOR" runat="server">OR</asp:Label></td>
                    <td class="inputtd">
                        <asp:TextBox ID="txtSelectCriteria2" TabIndex="2" runat="server"></asp:TextBox>&nbsp;<asp:Label
                            ID="lblOptional" runat="server" Text="(Optional)"></asp:Label></td>
                </tr>
            </table>
        </div>
    
    <div class="inputpanel formpanel_padding">
        <asp:ImageButton runat="server" ImageUrl="./buttons/pagebutton_search.gif" ID="cmdSearch" />
    </div></asp:Panel>
    <div class="inputpanel formpanel_padding">
        <asp:Literal ID="litScheduleHTML" runat="server"></asp:Literal>
    </div>
    <div class="inputpanel formpanel_padding">
        <asp:ImageButton runat="server" ID="cmdOK" ImageUrl="./buttons/update.gif" />
        <asp:ImageButton runat="server" ID="cmdCancel" ImageUrl="./buttons/cancel.gif" CausesValidation="false" />
    </div>
</asp:Content>
<asp:Content ID="Content4" runat="server" contentplaceholderid="contentmenu">
</asp:Content>


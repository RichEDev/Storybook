<%@ Page MasterPageFile="~/FWMaster.master" Language="vb"
    AutoEventWireup="false" Inherits="Framework2006.FWSetupRecharge" CodeFile="FWSetupRecharge.aspx.vb" %>
<%@ MasterType VirtualPath="~/FWMaster.master" %>

<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
<div class="inputpanel">
<table>
<tr>
<td class="labeltd">Active Location</td>
<td class="inputtd">
    <asp:DropDownList ID="lstLocations" runat="server" AutoPostBack="True">
    </asp:DropDownList></td>
</tr>
</table>
</div>
<div class="inputpanel">
<div class="inputpaneltitle">Recharge Functionality Configuration</div>
    <table>
        <tr>
            <td class="labeltd">
                <b>Reference As</b></td>
            <td class="inputtd">
                <asp:DropDownList ID="lstReferenceAs" runat="server" Width="120px">
                </asp:DropDownList></td>
        </tr>
        <tr>
            <td class="labeltd">
                <b>Preferred Staff Reference</b></td>
            <td class="inputtd">
                <asp:DropDownList ID="lstStaffRef" runat="server" Width="120px">
                    <asp:ListItem Value="Representative">Representative</asp:ListItem>
                    <asp:ListItem Value="Champion">Champion</asp:ListItem>
                </asp:DropDownList></td>
        </tr>
        <tr>
            <td class="labeltd">
                <b>Recharge Period</b></td>
            <td class="inputtd">
                <asp:DropDownList ID="lstRechargePeriod" runat="server" Width="120px">
                    <asp:ListItem Value="0">Monthly</asp:ListItem>
                    <asp:ListItem Value="1">Quarterly</asp:ListItem>
                    <asp:ListItem Value="2">6 Monthly</asp:ListItem>
                </asp:DropDownList></td>
        </tr>
        <tr>
            <td class="labeltd">
                <b>Financial Year Commences</b></td>
            <td class="inputtd">
                <asp:DropDownList ID="lstFYCommences" runat="server" Width="120px">
                </asp:DropDownList></td>
        </tr>
        <tr>
            <td class="labeltd">
                <b>Contract Product Delete Action</b></td>
            <td class="inputtd">
                <asp:DropDownList ID="lstCPDeleteAction" runat="server" Width="120px">
                    <asp:ListItem Value="0">Delete</asp:ListItem>
                    <asp:ListItem Value="1">Archive</asp:ListItem>
                </asp:DropDownList></td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Literal ID="litMessage" runat="server"></asp:Literal></td>
        </tr>
    </table>
                
    </div>
    <div class="inputpanel">
        <asp:ImageButton runat=server ID="cmdUpdate" ImageUrl="./buttons/update.gif" />&nbsp;&nbsp;<asp:ImageButton runat=server ID="cmdDone" ImageUrl="./buttons/ok.gif" Visible=false />
    </div>
</asp:Content>
<asp:Content ID="Content4" runat="server" contentplaceholderid="contentmenu">
<a href="./help_text/default_csh.htm#1107" target="_blank" class="submenuitem">Help</a>
                
</asp:Content>


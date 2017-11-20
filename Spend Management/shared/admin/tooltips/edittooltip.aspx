<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="edittooltip.aspx.cs" Inherits="Spend_Management.edittooltip" Title="Edit Tooltip" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
<div class="inputpanel">
    <table>
        <tr><td class="labeltd">Tooltip Description:</td><td class="inputtd">
            <asp:Label ID="lbldescription" runat="server" Text="Label"></asp:Label></td></tr>
        <tr><td class="labeltd">Tooltip Text:</td><td class="inputtd">
            <asp:TextBox ID="txttooltip" runat="server" Rows="8" Width="250px" MaxLength="4000" TextMode="MultiLine"></asp:TextBox></td></tr>
            <tr><td></td><td class="inputtd">
                <asp:LinkButton ID="cmdrestore" runat="server" onclick="cmdrestore_Click">Restore default text</asp:LinkButton></td></tr>
    </table>
</div>
<div class="inputpanel">
    <asp:ImageButton ID="cmdok" ImageUrl="~/shared/images/buttons/btn_save.png" runat="server" 
        onclick="cmdok_Click" />&nbsp;&nbsp;<asp:ImageButton ID="cmdcancel" ImageUrl="~/shared/images/buttons/cancel_up.gif"
        runat="server" onclick="cmdcancel_Click" />
</div>
</asp:Content>

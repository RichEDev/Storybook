<%@ Page Language="C#" MasterPageFile="~/expform.master" AutoEventWireup="true" CodeBehind="editstatement.aspx.cs" Inherits="expenses.admin.editstatement" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/expform.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
<div class="inputpanel">
<table>
                    <tr><td class="labeltd">Statement Name</td><td class="inputtd">
                        <asp:TextBox ID="txtname" runat="server"></asp:TextBox></td><td>
                            <asp:RequiredFieldValidator ID="reqname" runat="server" ErrorMessage="Please enter a Statement Name in the box provided" ControlToValidate="txtname" Text="*"></asp:RequiredFieldValidator></td></tr>
                    <tr><td class="labeltd">Statement Date</td><td class="inputtd">
                        <asp:TextBox ID="txtstatementdate" runat="server"></asp:TextBox></td><td>
                        <asp:CompareValidator ID="compdate" runat="server" ControlToValidate="txtstatementdate" Type="Date" Operator="DataTypeCheck" Text="*" ErrorMessage="The statement date you have entered is not a valid date"></asp:CompareValidator></td></tr>
                </table>
</div>
<div class="inputpanel">
    <asp:ImageButton ID="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" 
        onclick="cmdok_Click" />&nbsp;&nbsp<a href="statements.aspx"><img alt="Cancel" src="../buttons/cancel_up.gif" /></a>
</div>
</asp:Content>

<%@ Page Language="C#" MasterPageFile="~/exptemplate.master" AutoEventWireup="true" CodeBehind="expensesConnect.aspx.cs" Inherits="expenses.expensesConnect" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmain">
    <div class="inputpanel">
        <div class="inputpaneltitle">
            <asp:Label ID="lblexpensesConnect" runat="server" Text="expensesConnect Details" meta:resourcekey="lblexpensesConnectDetails"></asp:Label>
            </div>
            <%--<table>
                <tr>
                    <td>--%>
                        <asp:Literal ID="litDetails" runat="server"></asp:Literal>
                    <%--</td>
                    
                </tr>
            </table>--%>
        
        
    </div>

</asp:Content>
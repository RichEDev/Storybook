<%@ Page Language="C#" MasterPageFile="~/exptemplate.master" AutoEventWireup="true" CodeBehind="corporate_cards.aspx.cs" Inherits="expenses.admin.corporate_cards" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>

<asp:Content ID="Content4" ContentPlaceHolderID="styles" runat="server">
    <style>
        table thead tr th {
         padding-left: 3px;
        }
                          
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
    <asp:HyperLink ID="HyperLink1" runat="server" CssClass="submenuitem" NavigateUrl="~/admin/aecorporatecard.aspx">Add Corporate Card</asp:HyperLink>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/sel.corporatecardproviders.js"/>
            <asp:ScriptReference Path="~/shared/javascript/sel.ajax.js"/>
        </Scripts>
    </asp:ScriptManagerProxy>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
<div class="formpanel">
		    
    <asp:Panel runat="server" ID="gridCards">   
        <asp:Literal ID="litgrid" runat="server"></asp:Literal>
    </asp:Panel>

    <div class="formbuttons">
        <asp:ImageButton ID="cmdClose" href="statements.aspx" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
    </div> 
</div>

</asp:Content>

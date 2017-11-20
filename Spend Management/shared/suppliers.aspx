<%@ Page Title="" Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="suppliers.aspx.cs" Inherits="Spend_Management.suppliers" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
<asp:HyperLink runat="server" ID="hypNewSupplier" CssClass="submenuitem" 
        NavigateUrl="~/shared/supplier_details.aspx?sid=0">New Supplier</asp:HyperLink>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
<asp:ScriptManagerProxy runat="server" ID="smProxy">
<Services>
<asp:ServiceReference InlineScript="true" Path="~/shared/webServices/svcSuppliers.asmx" />
</Services>
<Scripts>
<asp:ScriptReference Path="~/shared/javaScript/suppliers.js" />
<asp:ScriptReference Path="~/shared/javaScript/shared.js" />
</Scripts>
</asp:ScriptManagerProxy>
<div class="formpanel formpanel_padding">
<asp:PlaceHolder runat="server" ID="phGrid"></asp:PlaceHolder>
    <div class="formbuttons">
        <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
    </div>     
</div>
</asp:Content>

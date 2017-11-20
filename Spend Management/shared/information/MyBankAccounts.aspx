<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="MyBankAccounts.aspx.cs" Inherits="Spend_Management.shared.information.MyBankAccounts"%>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<%@ Register assembly="SpendManagementHelpers" namespace="SpendManagementHelpers" tagprefix="cc2" %>
<%@ Register src="~/shared/usercontrols/bankAccounts.ascx" tagName="maccount" tagPrefix="maccount" %>



<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
     <a href="javascript:SEL.BankAccounts.LoadBankAccountModal(SEL.BankAccounts.LoadType.New, null);" class="submenuitem">
         <asp:Label id="lblAddNew" runat="server" meta:resourcekey="Label2Resource1">New Bank Account</asp:Label></a></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server"></asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="contentmain" runat="server">

<div class="formpanel formpanel_padding">
    <div class="sectiontitle">My Bank Accounts</div>
    <maccount:maccount ID="usrBankAccounts" runat="server"></maccount:maccount>
    
    <div class="formbuttons">
            <cc2:CSSButton ID="btnClose" runat="server" Text="close" onclick="btnClose_Click"></cc2:CSSButton></div>
</div>
</asp:Content>
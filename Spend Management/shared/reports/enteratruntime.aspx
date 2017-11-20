<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" Inherits="reports_enteratruntime" Codebehind="enteratruntime.aspx.cs" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>
<%@ Register TagPrefix="reports" TagName="criteria" Src="runtimecriteria.ascx" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" Runat="Server">
<reports:criteria runat="server" ID="criteria"></reports:criteria>

<div class="inputpanel">
    <asp:ImageButton ID="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" OnClick="cmdok_Click" meta:resourcekey="cmdokResource1" />&nbsp;&nbsp;<a href="javascript:window.close();"><img border=0 src="../images/buttons/cancel_up.gif" /></a>
       
</div>
</asp:Content>


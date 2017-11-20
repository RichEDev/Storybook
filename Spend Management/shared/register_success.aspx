<%@ Page Language="C#" MasterPageFile="~/masters/AnonymousUser.Master" AutoEventWireup="true" Inherits="register_success" Codebehind="register_success.aspx.cs" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>
<%@ MasterType VirtualPath="~/masters/AnonymousUser.Master" %>
<asp:Content ID="Content3" ContentPlaceHolderID="pageContents" Runat="Server"><div class="inputpanel"><asp:Label ID="lblmsg" runat="server" Text="" meta:resourcekey="lblmsgResource1">Thank you for registering with <asp:Literal ID="litMsgBrand" runat="server" Text="Expenses"></asp:Literal>  &#13;&#10;&#13;&#10;You will shortly receive an email containing a web link.  Simply click this link to verify that the details you have just entered are correct.&#13;&#10;</asp:Label></div></asp:Content>


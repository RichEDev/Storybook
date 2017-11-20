<%@ Page MasterPageFile="~/FWMaster.master" Language="vb" AutoEventWireup="false" Inherits="Framework2006.ErrorPage" CodeFile="ErrorPage.aspx.vb" %>
<%@ MasterType VirtualPath="~/FWMaster.master" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
<div class="inputpanel">
<div class="inputpaneltitle">Framework Error</div>
<div align=center><asp:Label id="lblInfo" runat="server">An error has occurred on the server.</asp:Label></div>
<div align=center><asp:Label id="lblInfo2" runat="server"> Please contact your administrator</asp:Label></div>
<div align=center><asp:Label id="lblErrMsg" runat="server">error message</asp:Label></div>
</div>
</asp:Content>

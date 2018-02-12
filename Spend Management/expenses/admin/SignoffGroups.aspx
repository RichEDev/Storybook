<%@ Page language="c#" Inherits="Spend_Management.admingroups" MasterPageFile="~/masters/smTemplate.master" Codebehind="SignoffGroups.aspx.cs" %>

<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    
<a href="aeSignoffGroup.aspx" class="submenuitem"><asp:Label id="Label1" runat="server">New Signoff Group</asp:Label></a>
				
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">		
    
    <script type="text/javascript" src="/expenses/javaScript/sel.SignoffGroups.js?date=20180212"></script>

    <div class="formpanel formpanel_padding">
        <asp:Literal runat="server" ID="litSignOffGroups"></asp:Literal>
         <helpers:CSSButton runat="server" ID="btnClose" Text="close" OnClientClick="document.location = '../../usermanagementmenu.aspx';return false;" UseSubmitBehavior="False" />
    </div>
</asp:Content>



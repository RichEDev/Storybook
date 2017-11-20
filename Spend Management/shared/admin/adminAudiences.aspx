<%@ Page Language="C#" MasterPageFile="~/masters/smTemplate.master" AutoEventWireup="true" CodeBehind="adminAudiences.aspx.cs" Inherits="Spend_Management.adminAudiences" %>
<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
    <a href="aeAudience.aspx" title="New Audience" class="submenuitem">New Audience</a>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
    <asp:ScriptManagerProxy ID="smp" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/minify/sel.audiences.js" />
        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/shared/webServices/svcAudiences.asmx" InlineScript="false" />
        </Services>
    </asp:ScriptManagerProxy>
    <script type="text/javascript">       
        //<![CDATA[
        var audId;
        var entityIdentifier;
        var baseTableId;
        var parentRecordId; 
        var pnlAudienceGrid = '<% = pnlGrid.ClientID %>';
        //]]>
    </script>
    <div class="formpanel formpanel_padding">
    <asp:Panel ID="pnlGrid" runat="server">
        <asp:Literal ID="litGrid" runat="server"></asp:Literal>
    </asp:Panel>
    <div class="formbuttons">
    <asp:ImageButton runat="server" ID="btnClose" AlternateText="Close" 
            ImageUrl="~/shared/images/buttons/btn_close.png" onclick="btnClose_Click"></asp:ImageButton>
    </div>
    </div>
</asp:Content>

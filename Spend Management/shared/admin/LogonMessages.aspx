<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/masters/smTemplate.master" CodeBehind="LogonMessages.aspx.cs" Inherits="Spend_Management.shared.admin.LogonMessages" %>
<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>

<asp:content id="Content2" runat="server" contentplaceholderid="contentmenu"> 
     <asp:HyperLink runat="server" ID="lnkNewLogonMessages" NavigateUrl="aeLogonMessages.aspx"  CssClass="submenuitem">New Marketing Information</asp:HyperLink>
</asp:content>
<asp:content id="Content1" runat="server" contentplaceholderid="contentmain">
    <asp:ScriptManagerProxy runat="server" ID="smProxy">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/sel.logonMessages.js" />
            <asp:ScriptReference Name="tooltips" />
        </Scripts>
         <Services>
             <asp:ServiceReference Path="~/shared/webServices/svcAccountOptions.asmx" InlineScript="false" />
        <asp:ServiceReference Path="~/shared/webServices/svcLogonMessages.asmx" InlineScript="false" />
        <asp:ServiceReference Path="~/shared/webServices/svcTooltip.asmx" InlineScript="false" />
    </Services>
    </asp:ScriptManagerProxy>

    <div class="formpanel formpanel_padding">
        <div id="gridLogonMessages">
            <asp:Literal runat="server" ID="litLogonMessages"></asp:Literal>
        </div>        
    </div>

</asp:content>
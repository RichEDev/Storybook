<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SessionTimeoutManager.aspx.cs" MasterPageFile="~/masters/smTemplate.master" Inherits="Spend_Management.shared.admin.SessionTimeoutManager" %>

<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
    
    <asp:ScriptManagerProxy runat="server" ID="smProxy">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/sel.tooltips.js" />
            <asp:ScriptReference Path="~/shared/javaScript/minify/sel.ipfilters.js" />
        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/shared/webServices/svcIPFilters.asmx" InlineScript="false" />
            <asp:ServiceReference InlineScript="false" Path="~/shared/webServices/svcTooltip.asmx" />
        </Services>
    </asp:ScriptManagerProxy>

    <link rel="stylesheet" type="text/css" media="screen" href="<%= ResolveUrl("~/shared/css/layout.css") %>" />
    <link rel="stylesheet" type="text/css" media="screen" href="<% = ResolveUrl("~/shared/css/styles.aspx") %>" />

    <div class="formpanel">
        <div class="sectiontitle">General Details</div>

        <div class="twocolumn">
            <asp:Label CssClass="mandatory" ID="lblIdleTimeout" runat="server" meta:resourcekey="lblIdleTimeout" Text="Minutes of inactivity after which user is logged out*" AssociatedControlID="cmbIdleTimeout"></asp:Label>
            <span class="inputs">
                <asp:DropDownList CssClass="fillspan" ID="cmbIdleTimeout" runat="server">
                    <asp:ListItem Selected="True" Text="[None]" Value=""></asp:ListItem>
                    <asp:ListItem Text="5" Value="5"></asp:ListItem>
                    <asp:ListItem Text="10" Value="10"></asp:ListItem>
                    <asp:ListItem Text="20" Value="20"></asp:ListItem>
                    <asp:ListItem Text="30" Value="30"></asp:ListItem>
                    <asp:ListItem Text="45" Value="45"></asp:ListItem>
                    <asp:ListItem Text="60" Value="60"></asp:ListItem>
                    <asp:ListItem Text="90" Value="90"></asp:ListItem>                    
                </asp:DropDownList></span>
            <span class="inputicon">&nbsp;</span>
            <span class="inputtooltipfield">
                <asp:Image ID="imgTooltipIdleTimeout" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('E1A2E4B7-0407-4A63-A775-DBE718C0E481', 'sm', this);" /></span>
            <span class="inputvalidatorfield">
                <asp:RequiredFieldValidator ID="reqIdleTimeout" runat="server" ValidationGroup="vgMain" ErrorMessage="Please select minutes of inactivity after which user is logged out." ControlToValidate="cmbIdleTimeout" meta:resourcekey="reqipaddressResource1">*</asp:RequiredFieldValidator>
            </span>
        </div>

        <div class="twocolumn">
            <asp:Label CssClass="mandatory" ID="Label1" runat="server" meta:resourcekey="lblIdleTimeout" Text="Seconds for timeout countdown.*" AssociatedControlID="cmbCountdown"></asp:Label>
            <span class="inputs">
                <asp:DropDownList CssClass="fillspan" ID="cmbCountdown" runat="server">
                    <asp:ListItem Selected="True" Text="[None]" Value=""></asp:ListItem>
                    <asp:ListItem Text="20" Value="20"></asp:ListItem>
                    <asp:ListItem Text="30" Value="30"></asp:ListItem>
                    <asp:ListItem Text="60" Value="60"></asp:ListItem>
                    <asp:ListItem Text="90" Value="90"></asp:ListItem>
                    <asp:ListItem Text="120" Value="120"></asp:ListItem>
                </asp:DropDownList></span>
            <span class="inputicon">&nbsp;</span>
            <span class="inputtooltipfield">
                <asp:Image ID="imgTooltipCountdown" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('14535B8F-245F-499F-B8F4-3CB38314AC90', 'sm', this);" /></span>
            <span class="inputvalidatorfield">
                <asp:RequiredFieldValidator ID="reqCountdown" runat="server" ValidationGroup="vgMain" ErrorMessage="Please select seconds for timeout countdown." ControlToValidate="cmbCountdown" meta:resourcekey="reqCountdown">*</asp:RequiredFieldValidator>
            </span>
        </div>
        
        <div class="formbuttons">         
            <helpers:CSSButton id="btnSave" OnClick="btnSave_Click" runat="server" text="save" ValidationGroup="vgMain" CausesValidation="False" OnClientClick="return validateform('vgMain');" />
            <helpers:CSSButton id="btnCancel" OnClick="btnCancel_Click" runat="server" text="cancel" UseSubmitBehavior="True" />
        </div>
    </div>
</asp:Content>

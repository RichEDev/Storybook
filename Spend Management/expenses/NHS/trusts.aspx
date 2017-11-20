<%@ Page Language="C#" MasterPageFile="~/masters/smTemplate.master" AutoEventWireup="true" CodeBehind="trusts.aspx.cs" Inherits="Spend_Management.trusts" %>
<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>

<%@ Register Src="~/shared/usercontrols/tooltip.ascx" TagName="tooltip" TagPrefix="tooltip" %> 
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
<a href="javascript:NewTrust();" class="submenuitem">New Trust</a>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
    <Services>
        <asp:ServiceReference Path="~/expenses/webservices/ESR.asmx" />
    </Services>
    <Scripts>
        <asp:ScriptReference Path="~/expenses/javaScript/esrtrust.js" />
    </Scripts>
    </asp:ScriptManagerProxy>
<div class="formpanel formpanel_padding">
    <script language="javascript" type="text/javascript">
        var activeTrustID = null;
        


 
    </script>

    <div class="formpanel formpanel_padding">
    <asp:Literal ID="litGrid" runat="server"></asp:Literal>
    <div class="formbuttons">
        <img src="/shared/images/buttons/btn_close.png" alt="Close" onclick="window.location = '../../exportmenu.aspx';" />
    </div>
    </div>
    
    <asp:Panel ID="pnlTrust" runat="server" style="display: none; background-color: #ffffff; border: 1px solid #000000; padding: 20px;" CssClass="modalpanel formpanel">
            <tooltip:tooltip ID="usrTooltip" runat="server"></tooltip:tooltip>
           <div class="sectiontitle">NHS Trust Details</div>
            <div class="twocolumn">
                <asp:Label ID="lblTrustName" runat="server" AssociatedControlID="txtTrustName" Text="Trust Name *" CssClass="mandatory"></asp:Label><span class="inputs"><asp:TextBox ID="txtTrustName" runat="server" CssClass="fillspan" MaxLength="150"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="rfTrustName" runat="server" ControlToValidate="txtTrustName" Text="*" ErrorMessage="Please enter the trust name"></asp:RequiredFieldValidator></span>
                <asp:Label ID="lblTrustVPD" runat="server" AssociatedControlID="txtTrustVPD" Text="Trust VPD"></asp:Label><span class="inputs"><asp:TextBox ID="txtTrustVPD" runat="server" CssClass="fillspan" MaxLength="3"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
            </div>

        <div class="twocolumn">
                <asp:Label ID="lblPeriodType" runat="server" AssociatedControlID="ddlPeriodType" Text="Period Type"></asp:Label><span class="inputs"><asp:DropDownList ID="ddlPeriodType" runat="server"><asp:ListItem Text="Weekly" Value="W"></asp:ListItem><asp:ListItem Text="Monthly" Value="M"></asp:ListItem><asp:ListItem Text="Lunar Month" Value="L"></asp:ListItem><asp:ListItem Text="Bi-Week" Value="F"></asp:ListItem></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                <asp:Label ID="lblPeriodRun" runat="server" AssociatedControlID="ddlPeriodRun" Text="Period Run"></asp:Label><span class="inputs"><asp:DropDownList ID="ddlPeriodRun" runat="server"><asp:ListItem Text="Normal" Value="N"></asp:ListItem><asp:ListItem Text="Supplementary" Value="S"></asp:ListItem></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
        </div>
                

        <div class="twocolumn">
                <asp:Label ID="lblFTPAddress" runat="server" AssociatedControlID="txtFTPAddress" Text="FTP Address"></asp:Label><span class="inputs"><asp:TextBox ID="txtFTPAddress" runat="server" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                <asp:Label ID="lblFTPUsername" runat="server" AssociatedControlID="txtFTPUsername" Text="FTP Username"></asp:Label><span class="inputs"><asp:TextBox ID="txtFTPUsername" runat="server" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
        </div>

        <div class="twocolumn">
        <asp:Label ID="lblFTPPassword" runat="server" AssociatedControlID="txtFTPPassword" Text="FTP Password"></asp:Label><span class="inputs"><asp:TextBox ID="txtFTPPassword" runat="server" TextMode="Password" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
        <asp:Label ID="lblRunSequence" runat="server" AssociatedControlID="txtRunSequence" Text="Run Sequence"></asp:Label><span class="inputs"><asp:TextBox ID="txtRunSequence" runat="server" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RangeValidator ID="runSeqFormat" runat="server" ControlToValidate="txtRunSequence" Type="Integer" MinimumValue="0" MaximumValue="99999999" ErrorMessage="The Run Sequence must consist of numbers" Text="*" Display="Dynamic"></asp:RangeValidator></span>
        </div>
        <div class="twocolumn">
            <asp:Label ID="lblDelimiter" runat="server" AssociatedControlID="txtDelimiter" Text="Delimiter Character"></asp:Label><span class="inputs"><asp:TextBox ID="txtDelimiter" runat="server" MaxLength="5" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip427" alt="" class="tooltipicon" onclick="SEL.Tooltip.Show('1d468412-53ed-4c8f-b017-f63516e399c1', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" /></span><span class="inputvalidatorfield">&nbsp;</span>
            <asp:Label runat="server" ID="lblESRVersionNumber" AssociatedControlID="ddlESRVersionNumber" Text="ESR Interface Version"></asp:Label><span class="inputs"><asp:DropDownList runat="server" ID="ddlESRVersionNumber" CssClass="fillspan"><asp:ListItem Text="v1.0" Value="1"></asp:ListItem><asp:ListItem Text="v2.0" Value="2"></asp:ListItem></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip588" alt="" class="tooltipicon" onclick="SEL.Tooltip.Show('02ad8809-1d27-4f9b-8eec-882338c74076', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" /></span><span class="inputvalidatorfield">&nbsp;</span>
        </div>
        <div class="formbuttons"><img src="/shared/images/buttons/btn_save.png" onclick="SaveTrust();" alt="Save" /> <img src="/shared/images/buttons/cancel_up.gif" onclick="hideModal()" alt="Cancel" /></div>
    </asp:Panel>
    
    <asp:HyperLink ID="lnkTrustModal" runat="server" Text="&nbsp;" style="display: none;">&nbsp;</asp:HyperLink>
    
    <cc1:ModalPopupExtender ID="mdlTrust" runat="server" TargetControlID="lnkTrustModal" PopupControlID="pnlTrust" BackgroundCssClass="modalBackground"></cc1:ModalPopupExtender>
    
</div>
<script language="javascript" type="text/javascript">
    var modalClientID = '<% = mdlTrust.ClientID %>';
    var trustName = document.getElementById('<% = txtTrustName.ClientID %>');
    var trustVPD = document.getElementById('<% = txtTrustVPD.ClientID %>');
    var periodType = document.getElementById('<% = ddlPeriodType.ClientID %>');
    var periodRun = document.getElementById('<% = ddlPeriodRun.ClientID %>');
    var FTPAddress = document.getElementById('<% = txtFTPAddress.ClientID %>');
    var FTPUsername = document.getElementById('<% = txtFTPUsername.ClientID %>');
    var FTPPassword = document.getElementById('<% = txtFTPPassword.ClientID %>');
    var RunSequenceNumber = document.getElementById('<% = txtRunSequence.ClientID %>');
    var DelimiterCharacter = document.getElementById('<% = txtDelimiter.ClientID %>');
    var ESRInterfaceVersion = document.getElementById('<%= ddlESRVersionNumber.ClientID %>');
</script>
</asp:Content>

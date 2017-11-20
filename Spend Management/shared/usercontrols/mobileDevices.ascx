<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="mobileDevices.ascx.cs" Inherits="Spend_Management.mobileDevices" %>
<%@ Register Src="~/shared/usercontrols/tooltip.ascx" TagName="tooltip" TagPrefix="tooltip" %>
<%@ Register assembly="SpendManagementHelpers" namespace="SpendManagementHelpers" tagprefix="cc2" %>

<asp:ScriptManagerProxy ID="usemobiledevice_smp" runat="server">
    <Services>
        <asp:ServiceReference Path="~/shared/webServices/svcMobileDevices.asmx" InlineScript="false" />
        <asp:ServiceReference Path="~/shared/webServices/svcTooltip.asmx" InlineScript="false" />
    </Services>
    <Scripts>
        <asp:ScriptReference Name="tooltips" />
        <asp:ScriptReference Path="~/shared/javaScript/minify/sel.mobileDevices.js" />
    </Scripts>
</asp:ScriptManagerProxy>

    <asp:Literal ID="litMobileDevices" runat="server"></asp:Literal>
    
    <asp:Panel ID="AddEditDevice" runat="server" class="modalpanel" style="display: none;">
        <div class="formpanel">
            <div class="sectiontitle" id="divMobileDeviceHeader">&nbsp;</div>
            <div class="twocolumn">
                <asp:Label AssociatedControlID="txtDeviceName" ID="lblDeviceName" runat="server" CssClass="mandatory">Name*</asp:Label><span class="inputs"><asp:TextBox ID="txtDeviceName" runat="server" MaxLength="100" onkeydown="SEL.Forms.RunOnEnter(event, SEL.MobileDevices.SaveMobileDeviceOnEnter);"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip533" onmouseover="SEL.Tooltip.Show('e2b86a9a-8c8d-458d-9a2f-c0152f330aa1', 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">
                    <asp:RequiredFieldValidator ValidationGroup="vgAddEditMobile" ID="rfDeviceName" runat="server" ErrorMessage="Please enter a Name for the mobile device." ControlToValidate="txtDeviceName">*</asp:RequiredFieldValidator>
                </span>

                <asp:Label AssociatedControlID="ddlDeviceType" ID="lblDeviceType" runat="server" CssClass="mandatory">Type*</asp:Label><span class="inputs"><asp:DropDownList ID="ddlDeviceType" runat="server" onkeydown="SEL.Forms.RunOnEnter(event, SEL.MobileDevices.SaveMobileDeviceOnEnter);"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip532" onmouseover="SEL.Tooltip.Show('a7350c00-ec95-4b57-8006-0e09e398550b', 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">
                    <asp:RequiredFieldValidator ValidationGroup="vgAddEditMobile" ID="rfDeviceType" runat="server" ErrorMessage="Please select a Type for the mobile device." ControlToValidate="ddlDeviceType" InitialValue="0">*</asp:RequiredFieldValidator>
                </span>
            </div>

            <div id="divMobileDeviceInfo" class="comment" style="padding: 10px; vertical-align: top;"> 
                <span style="display: inline-block; -moz-inline-box;"><asp:Image runat="server" ID="imgIPhoneEdit" ImageUrl="~/shared/images/submenu_bg.jpg"/></span>
                    <span style="display: inline-block; padding-left: 10px; vertical-align: top; margin-top: 25px; -moz-inline-box;" id="spanMobileDeviceInfo">&nbsp;</span>
            </div>       
            
            <asp:Panel ID="pnlAddEditDeviceButtons" runat="server" CssClass="formbuttons"></asp:Panel>
        </div>
    </asp:Panel>
    <asp:HyperLink ID="lnkAddEditDevice" runat="server" style="display:none">&nbsp;</asp:HyperLink>
    <cc1:ModalPopupExtender ID="modalAddEditDevice" runat="server" PopupControlID="AddEditDevice" TargetControlID="lnkAddEditDevice"  BackgroundCssClass="modalBackground" ></cc1:ModalPopupExtender>
    <tooltip:tooltip ID="usrTooltip" runat="server"></tooltip:tooltip>
    <asp:Panel runat="server" ID="PairingNoticeModal" CssClass="modalpanel" style="display: none;">
        <div class="formpanel">
            <div class="sectiontitle">Mobile Device Activation Key</div>
            <div class="comment" style="padding: 10px; vertical-align: top;"><span style="display: inline-block; -moz-inline-box;" ><asp:Image runat="server" ID="imgIPhoneNew" ImageUrl="~/shared/images/submenu_bg.jpg" /></span>
            <span style="display: inline-block; padding-left: 10px; vertical-align: top; margin-top: 25px; -moz-inline-box;" id="spanPairingKeyInfo">&nbsp;</span></div>
            <asp:Panel runat="server" ID="pnlPairingInfoButtons" CssClass="formbuttons"></asp:Panel>
        </div>
    <asp:HyperLink runat="server" ID="lnkModalInfo" style="display: none;">&nbsp;</asp:HyperLink>
    <cc1:ModalPopupExtender runat="server" ID="modalPairingInfo" PopupControlID="PairingNoticeModal" TargetControlID="lnkModalInfo" BackgroundCssClass="modalBackground" />
    </asp:Panel>
   

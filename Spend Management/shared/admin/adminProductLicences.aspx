<%@ Page Title="" Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true"
    CodeBehind="adminProductLicences.aspx.cs" Inherits="Spend_Management.adminProductLicences" %>

<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
    <asp:LinkButton runat="server" ID="lnkAddLicence" CssClass="submenuitem" OnClientClick="javascript:editLicence(0);">Add</asp:LinkButton>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
    <asp:ScriptManagerProxy runat="server" ID="smproxy">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/shared.js" />
            <asp:ScriptReference Path="~/shared/javaScript/productLicences.js" />
        </Scripts>
        <Services></Services>
    </asp:ScriptManagerProxy>
    <script language="javascript" type="text/javascript">
        var modalpanelID = '<%=pnlProductLicenceDetail.ClientID %>';
    </script>
    <div class="formpanel">
        <div class="sectiontitle">
            Current Licences</div>
        <asp:Literal runat="server" ID="litLicenceGrid"></asp:Literal>
    </div>
    <asp:Panel runat="server" ID="pnlProductLicenceDetail">
        <div class="formpanel">
            <div class="twocolumn">
            <asp:Label runat="server" ID="lblLicenceKey" AssociatedControlID="txtLicenceKey" CssClass="mandatory">Licence Key *</asp:Label>
            <span class="inputs"><asp:TextBox runat="server" ID="txtLicenceKey" CssClass="fillspan" TabIndex="1" ValidationGroup="plicdetail"></asp:TextBox></span>
            <span class="inputicon">&nbsp;</span>
            <span class="inputtooltipfield">&nbsp;</span>
            <span class="inputvalidator"><asp:RequiredFieldValidator runat="server" ID="reqLicenceKey" ControlToValidate="txtLicenceKey" ValidationGroup="plicdetail" Text="*" ErrorMessage="Licence Key field is mandatory"></asp:RequiredFieldValidator></span>
            <asp:Label runat="server" ID="lblLocation" AssociatedControlID="txtLocation">Location</asp:Label>
            <span class="inputs"><asp:TextBox runat="server" ID="txtLocation" CssClass="fillspan" TabIndex="2" ValidationGroup="plicdetail"></asp:TextBox></span>
            <span class="inputicon">&nbsp;</span>
            <span class="inputtooltipfield">&nbsp;</span>
            <span class="inputvalidator">&nbsp;</span>
            </div>
            <div class="twocolumn">
                <asp:Label runat="server" ID="lblDateInstalled" AssociatedControlID="txtDateInstalled">Date Installed</asp:Label>
                <span class="inputs">
                    <asp:TextBox runat="server" ID="txtDateInstalled" CssClass="fillspan" TabIndex="3"
                        ValidationGroup="plicdetail"></asp:TextBox></span> <span class="inputicon">
                            <asp:Image runat="server" ID="imgDateInstalled" ImageUrl="~/shared/images/icons/cal.gif"
                                AlternateText="Calendar" /><cc1:CalendarExtender runat="server" ID="calexDateInstalled"
                                    TargetControlID="txtDateInstalled" PopupButtonID="imgDateInstalled" Format="dd/MM/yyyy"
                                    PopupPosition="TopRight"></cc1:CalendarExtender></span> <span class="inputtooltipfield">
                                        &nbsp;</span> <span class="inputvalidator">
                                            <asp:CompareValidator runat="server" ID="cmpDateInstalled" ControlToValidate="txtDateInstalled"
                                                Type="Date" Operator="DataTypeCheck" Text="*" ErrorMessage="Invalid date format entered"
                                                ValidationGroup="pdetail"></asp:CompareValidator>
                                            <cc1:ValidatorCalloutExtender runat="server" ID="cmpexDateInstalled" TargetControlID="cmpDateInstalled">
                                            </cc1:ValidatorCalloutExtender>
                                        </span>
                <asp:Label runat="server" ID="lblLicenceType" AssociatedControlID="lstLicenceType">Licence Type</asp:Label>
                <span class="inputs">
                <asp:DropDownList runat="server" ID="lstLicenceType" CssClass="fillspan" ValidationGroup="plicdetail" TabIndex="4"></asp:DropDownList>
                </span>
                <span class="inputicon">&nbsp;</span>
            <span class="inputtooltipfield">&nbsp;</span>
            <span class="inputvalidator">&nbsp;</span>
            </div>
            <div class="twocolumn">
            <asp:Label runat="server" ID="lblExpiry" AssociatedControlID="txtExpiry">Expiry</asp:Label>
            <span class="inputs"><asp:TextBox runat="server" ID="txtExpiry" CssClass="fillspan" ValidationGroup="plicdetail" TabIndex="5"></asp:TextBox></span>
            <span class="inputicon"><asp:Image runat="server" ID="imgExpiry" ImageUrl="~/shared/images/icons/cal.gif" /><cc1:CalendarExtender runat="server" ID="calexExpiry"
                                    TargetControlID="txtExpiry" PopupButtonID="imgExpiry" Format="dd/MM/yyyy"
                                    PopupPosition="TopRight"></cc1:CalendarExtender></span>
            <span class="inputtooltipfield">&nbsp;</span>
            <span class="inputvalidator"><asp:CompareValidator runat="server" ID="cmpExpiry" Type="Date" ValidationGroup="plicdetail" ControlToValidate="txtExpiry" Text="*" ErrorMessage="Invalid Date format specified for Expiry" Operator="DataTypeCheck"></asp:CompareValidator>
            <cc1:ValidatorCalloutExtender runat="server" TargetControlID="cmpExpiry" ID="cmpexExpiry"></cc1:ValidatorCalloutExtender>
            </span>
            <asp:Label runat="server" ID="lblRenewalType" AssociatedControlID="lstRenewalType">Licence Renewal Type</asp:Label>
            <span class="inputs"><asp:DropDownList runat="server" ID="lstRenewalType" CssClass="fillspan" TabIndex="6"></asp:DropDownList></span>
            <span class="inputicon">&nbsp;</span>
            <span class="inputtooltipfield">&nbsp;</span>
            <span class="inputvalidator">&nbsp;</span>
            </div>
            <div class="twocolumn">
                <asp:Label runat="server" ID="lblInstalledVersion" AssociatedControlID="txtInstalledVersion">Installed Version</asp:Label>
                <span class="inputs">
                    <asp:TextBox runat="server" ID="txtInstalledVersion" ValidationGroup="plicdetail"
                        CssClass="fillspan" MaxLength="50" TabIndex="7"></asp:TextBox></span> <span class="inputicon">&nbsp;</span>
                <span class="inputtooltipfield">&nbsp;</span> <span class="inputvalidator">&nbsp;</span>
                <asp:Label runat="server" ID="lblAvailableVersion" AssociatedControlID="txtAvailableVersion">Available Version</asp:Label>
                <span class="inputs">
                    <asp:TextBox runat="server" ID="txtAvailableVersion" CssClass="fillspan" ValidationGroup="plicdetail"
                        TabIndex="8" MaxLength="50"></asp:TextBox></span> <span class="inputicon">&nbsp;</span>
                <span class="inputtooltipfield">&nbsp;</span> <span class="inputvalidator">&nbsp;</span>
            </div>
            <div class="onecolumn">
                    <asp:Label runat="server" ID="lblUserCode" AssociatedControlID="txtUserCode">User Code</asp:Label>
                    <span class="inputs">
                        <asp:TextBox runat="server" ID="txtUserCode" ValidationGroup="plicdetail" TabIndex="9"
                            TextMode="MultiLine"></asp:TextBox></span> <span class="inputicon">&nbsp;</span>
                    <span class="inputtooltipfield">&nbsp;</span> <span class="inputvalidator">&nbsp;</span>
                </div>            
            <div class="twocolumn">
            <asp:Label runat="server" ID="lblNotifyDays" AssociatedControlID="txtNotifyDays">Notify days</asp:Label>
            <span class="inputs"><asp:TextBox runat="server" ID="txtNotifyDays" CssClass="fillspan" ValidationGroup="plicdetail" TabIndex="10"></asp:TextBox>
            </span>
            <span class="inputicon">&nbsp;</span>
            <span class="inputtooltipfield">&nbsp;</span>
            <span class="inputvalidator"><asp:CompareValidator runat="server" ID="cmpNotifyDays" ValidationGroup="plicdetail" ControlToValidate="txtNotifyDays" Operator="DataTypeCheck" Type="Integer" Text="*" ErrorMessage="Invalid numeric value entered for Notify days"></asp:CompareValidator>
            <cc1:ValidatorCalloutExtender runat="server" TargetControlID="cmpNotifyDays" ID="cmpexNotifyDays"></cc1:ValidatorCalloutExtender>
            </span>
            <asp:Label runat="server" ID="lblNotify" AssociatedControlID="txtNotify">Notify</asp:Label>
            <span class="inputs"><asp:TextBox runat="server" ID="txtNotify" CssClass="fillspan" ValidationGroup="plicdetail" TabIndex="11"></asp:TextBox></span>
            <span class="inputicon">&nbsp;</span>
            <span class="inputtooltipfield">&nbsp;</span>
            <span class="inputvalidator">&nbsp;</span>
            </div>
            <div class="twocolumn">
            <asp:Label runat="server" ID="lblSoftCopy" AssociatedControlID="chkSoftCopy">Soft Copy Held</asp:Label>
            <span class="inputs"><asp:CheckBox runat="server" ID="chkSoftCopy" ValidationGroup="plicdetail" TabIndex="12" /></span>
            <span class="inputicon">&nbsp;</span>
            <span class="inputtooltipfield">&nbsp;</span>
            <span class="inputvalidator">&nbsp;</span>            
            <asp:Label runat="server" ID="lblHardCopy" AssociatedControlID="chkHardCopy">Hard Copy Held</asp:Label>
            <span class="inputs"><asp:CheckBox runat="server" ID="chkHardCopy" ValidationGroup="plicdetail" TabIndex="13" /></span>
            <span class="inputicon">&nbsp;</span>
            <span class="inputtooltipfield">&nbsp;</span>
            <span class="inputvalidator">&nbsp;</span>
            </div>
            <div class="twocolumn">
            <%--<asp:Label runat="server" ID="lblNumCopies" AssociatedControlID="txtNumCopies">Nu</asp:Label>--%>
            </div>
            <div class="onecolumn">
            <asp:Label runat="server" ID="lblComments" AssociatedControlID="txtComments">Comments</asp:Label>
            <span class="inputs"><asp:TextBox runat="server" ID="txtComments" TextMode="MultiLine" TabIndex="15" ValidationGroup="plicdetail"></asp:TextBox></span>
            <span class="inputicon">&nbsp;</span>
            <span class="inputtooltipfield">&nbsp;</span>
            <span class="inputvalidator">&nbsp;</span>
            </div>
        </div>
        <div class="formbuttons">
			<asp:ImageButton runat="server" ID="cmdLicenceUpdate" ImageUrl="~/shared/images/buttons/btn_save.png" />
			<asp:ImageButton ID="cmdLicenceCancel" ImageUrl="~/shared/images/buttons/cancel_up.gif"
				runat="server" CausesValidation="False" />
		</div>
    </asp:Panel>
    <asp:LinkButton runat="server" ID="lnkOpenModal" Style="display: none;"></asp:LinkButton>
    <cc1:ModalPopupExtender ID="ModalPopupExtender1" runat="server" TargetControlID="lnkOpenModal"
        PopupControlID="pnlProductLicenceDetail" BackgroundCssClass="modalBackground"
        CancelControlID="cmdLicenceCancel">
    </cc1:ModalPopupExtender>
</asp:Content>

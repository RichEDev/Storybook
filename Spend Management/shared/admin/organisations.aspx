<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="organisations.aspx.cs" Inherits="Spend_Management.shared.admin.organisations" %>

<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<%@ Register Src="~/shared/usercontrols/addressDetailsPopup.ascx" TagName="Popup" TagPrefix="AddressDetails" %>


<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    <asp:HyperLink runat="server" ID="lnkNewOrganisation" NavigateUrl="javascript:SEL.Organisations.Organisation.New();" CssClass="submenuitem">New Organisation</asp:HyperLink>
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentleft"></asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
    <asp:ScriptManagerProxy ID="smp" runat="server">
        <Scripts>
            <asp:ScriptReference Name="organisations" />
        </Scripts>
    </asp:ScriptManagerProxy>
    
    <script type="text/javascript">
        $(document).ready(function ()
        {
            (function (o)
            {
                o.PanelTitle = "txtOrganisationModalTitle";
                o.Panel = "<% = this.pnlOrganisation.ClientID %>";
                o.Modal = "<% = this.mdlOrganisation.ClientID %>";
                o.NameValidator = "<% = this.rfvName.ClientID %>";
                o.Name = "<% = this.txtName.ClientID %>";
                o.ParentOrganisationID = "<% = this.txtParentOrganisation.ClientID %>";
                o.PrimaryAddressID = "<% = this.hidPrimaryAddressID.ClientID %>";
                o.PrimaryAddress = "<% = this.txtAddress.ClientID %>";
                o.Comment = "<% = this.txtComment.ClientID %>";
                o.Code = "<% = this.txtCode.ClientID %>";
            }(SEL.Organisations.Dom.Organisation));

            SEL.Organisations.Setup();
        }());
    </script>
    

    <div class="formpanel formpanel_padding" >
        <div id="gridOrganisations"><asp:PlaceHolder runat="server" ID="phOrganisationsGrid"></asp:PlaceHolder></div>
        <div class="formbuttons">
            <helpers:CSSButton runat="server" ID="btnClose" Text="close" OnClientClick="SEL.Organisations.Page.Close();return false;" UseSubmitBehavior="False" />
        </div>
    </div>
    
    <asp:Panel runat="server" ID="pnlOrganisation" CssClass="modalpanel" style="display: none;">
        <div class="formpanel" style="padding:20px 0 0 20px;">
            <div id="txtOrganisationModalTitle" class="sectiontitle">New Organisation</div>
        </div>
        <cc1:TabContainer runat="server" ID="tcOrganisation" CssClass="ajax__tab_xp formpanel">
            <cc1:TabPanel runat="server" ID="tpOrganisation" HeaderText="General Details">
                <ContentTemplate>
                    <div class="sectiontitle">General Details</div>
                    <div class="twocolumn">
                        <asp:Label runat="server" ID="lblName" AssociatedControlID="txtName" CssClass="mandatory">Organisation name*</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtName" MaxLength="256" /></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator runat="server" ID="rfvName" ValidationGroup="vgOrganisation" Text="*" ErrorMessage="Please enter an Organisation name." ControlToValidate="txtName" /></span><asp:Label runat="server" ID="lblCode" AssociatedControlID="txtCode">Code</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtCode" MaxLength="60" /></span><span class="inputicon"></span><span class="inputtooltipfield"></span>
                    </div>
                    
                    <div class="twocolumn">
                        <asp:Label runat="server" ID="lblParentOrganisation" AssociatedControlID="txtParentOrganisation">Parent organisation</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtParentOrganisation" ></asp:TextBox><asp:TextBox runat="server" ID="txtParentOrganisation_ID" style="display: none;"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:CompareValidator runat="server" ID="cvParentOrganisation" Text="*" ValidationGroup="vgOrganisation" ControlToValidate="txtParentOrganisation_ID" ErrorMessage="Please enter a valid Parent Organisation, or leave it empty." Display="Dynamic" Operator="NotEqual" ValueToCompare="-1"></asp:CompareValidator><asp:CustomValidator runat="server" ID="custParentOrganisation" ControlToValidate="txtParentOrganisation_ID" Text="*" Display="Dynamic" ErrorMessage="The Parent Organisation can not be the organisation you are editing." ValidationGroup="vgOrganisation" ClientValidationFunction="SEL.Organisations.Validate.ParentOrganisation"></asp:CustomValidator></span><asp:Label runat="server" ID="lblAddress" AssociatedControlID="txtAddress">Primary address</asp:Label><span class="inputs"><input type="text" runat="server" ID="txtAddress" ClientIDMode="Static" maxlength="256" class="ui-sel-address-picker" rel="hidPrimaryAddressID" /><asp:HiddenField runat="server" ID="hidPrimaryAddressID" ClientIDMode="Static" Value="" /></span><span class="inputicon"><asp:Image runat="server" ID="imgBinoculars" onmouseover="addressDetailsPopup.displayAddress(this.id, 'hidPrimaryAddressID', event);" onmouseout="addressDetailsPopup.hide();" ImageUrl="~/shared/images/icons/16/plain/find.png" style="border-width:0px;vertical-align:middle;padding-left:4px;" CssClass="btn" /></span><span class="inputtooltipfield"></span>
                    </div>

                    <div class="onecolumn">
                        <asp:Label id="lblComment" runat="server" AssociatedControlID="txtComment"><p class="labeldescription">Comment</p></asp:Label><span class="inputs"><asp:TextBox id="txtComment" runat="server" TextMode="MultiLine" textareamaxlength="4000" /></span><span class="inputicon"></span><span class="inputtooltipfield"></span>
                    </div>
                </ContentTemplate>
            </cc1:TabPanel>

 <%--           <cc1:TabPanel runat="server" ID="tpOrganisationAddress" HeaderText="Address">
                <ContentTemplate>
                    <div class="sectiontitle">Organisation Addresses</div>
                    <div class="twocolumn">
                    </div>
                </ContentTemplate>
            </cc1:TabPanel>--%>
        </cc1:TabContainer>

        <div class="formpanel">
            <div class="formbuttons">
                <helpers:CSSButton runat="server" ID="btnSaveOrganisation" Text="save" OnClientClick=" SEL.Organisations.Organisation.Save();return false; " UseSubmitBehavior="False" /> 
                <helpers:CSSButton runat="server" ID="btnCancelOrganisation" Text="cancel" OnClientClick=" SEL.Organisations.Organisation.Cancel();return false; " UseSubmitBehavior="False" />
            </div>
        </div>
    </asp:Panel>

    <cc1:ModalPopupExtender runat="server" ID="mdlOrganisation" BackgroundCssClass="modalBackground" TargetControlID="lnkOrganisation" PopupControlID="pnlOrganisation"></cc1:ModalPopupExtender>
    <asp:HyperLink runat="server" ID="lnkOrganisation" style="display: none;"></asp:HyperLink>
    
    
    <AddressDetails:Popup ID="addressDetailsPopup" runat="server" />
</asp:Content>
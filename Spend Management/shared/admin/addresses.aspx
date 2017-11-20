<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="addresses.aspx.cs" Inherits="Spend_Management.shared.admin.addresses" %>

<%@ Import Namespace="SpendManagementLibrary" %>

<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content runat="server" ID="customStyles" ContentPlaceHolderID="styles">
    <style type="text/css">
        #gridAwabels {
            height: 180px !important;
            overflow-y: auto;
        }

        #pnlDestinations {
            height: 180px !important;
            overflow-y: auto;
        }

        #ctl00_contentmain_pnlAddress {
            width: 73%;
        }

        #tbodyNewRecommendedDistance tr td:first-child{
            width:350px;
        }

       
    </style>
    
    <!--[if IE 7]>
        <style>
            .modalpanel {
                max-height: 875px;
            }
        .ajax__tab_xp .ajax__tab_body {
            border-top: 1px solid #999!important;
        }
             
        </style>
    <![endif]-->
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    <asp:HyperLink runat="server" ID="lnkNewAddress" NavigateUrl="javascript:SEL.Addresses.Address.New();" CssClass="submenuitem">New Address</asp:HyperLink>
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentleft"></asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
    <asp:ScriptManagerProxy ID="smp" runat="server">
        <Scripts>
            <asp:ScriptReference Name="addresses" />
        </Scripts>
    </asp:ScriptManagerProxy>
    
    <script type="text/javascript">
        $(document).ready(function()
        {
            (function(a)
            {
                a.PanelTitle = "txtAddressModalTitle";
                a.Panel = "<% = this.pnlAddress.ClientID %>";
                a.Modal = "<% = this.mdlAddress.ClientID %>";
                a.Search = "txtSearch";
                a.AddressName = "<%= this.txtAddressName.ClientID %>";
                a.Line1 = "<% = this.txtLine1.ClientID %>";
                a.Line1Validator = "<% = this.rfvLine1.ClientID %>";
                a.Line2 = "<% = this.txtLine2.ClientID %>";
                a.Line3 = "<% = this.txtLine3.ClientID %>";
                a.City = "<% = this.txtCity.ClientID %>";
                a.County = "<% = this.txtCounty.ClientID %>";
                a.Country = "<% = this.ddlCountry.ClientID %>";
                a.Postcode = "<% = this.txtPostcode.ClientID %>";
                a.PostcodeValidator = "<% = this.rfvPostcode.ClientID %>";
                a.Favourited = "<% = this.chkFavourite.ClientID %>";

                a.Tabs = "<% = this.tcAddresses.ClientID %>";
                
                a.AccountWideLabels.Panel = "<% = this.pnlAwabel.ClientID %>";
                a.AccountWideLabels.Modal = "<% = this.mdlAwabel.ClientID %>";
                a.AccountWideLabels.Grid = "gridAwabels";
                a.AccountWideLabels.Label = "<% = this.txtAwabel.ClientID %>";
                a.AccountWideLabels.PrimaryLabel = "<% = this.chkPrimaryAwabel.ClientID %>";
                
                a.RecommendedDistances.NewDestination = "tbodyNewRecommendedDistance";
                a.RecommendedDistances.DestinationSearch = "txtDestinationSearch";
                a.RecommendedDistances.DestinationSearchIdentifier = "txtDestinationSearchIdentifier";
                a.RecommendedDistances.Destinations = "tbodyRecommendedDistancesContainer";
                a.RecommendedDistances.DestinationsContainer = "pnlDestinations";

            } (SEL.Addresses.Dom.Address));

            SEL.Addresses.Setup();
            SEL.Addresses.Identifiers.PostcodeMandatory = <% = this.PostcodeMandatory ? "true" : "false" %>;
            SEL.Addresses.Identifiers.DefaultGlobalCountry = "<% = this.DefaultCountry %>";
            SEL.Addresses.Identifiers.ForceAddressNameEntry = <%= this.ForceAddressNameEntry ? "true" : "false" %>;
            SEL.Addresses.Identifiers.AddressNameEntryMessage = "<%= this.AddressNameEntryMessage %>";
            
        }());
    </script>
    

    <div class="formpanel formpanel_padding">
        <div id="gridAddresses">
            <asp:PlaceHolder runat="server" ID="phAddressesGrid"></asp:PlaceHolder>
        </div>
        <div class="formbuttons">
            <helpers:CSSButton runat="server" ID="btnClose" Text="close" OnClientClick=" SEL.Addresses.Page.Close();return false; " UseSubmitBehavior="False" />
        </div>
    </div>
    
    <asp:Panel runat="server" ID="pnlAddress" Style="display: none; height: 575px;">
        <div class="modalpanel">
            <div class="formpanel" style="padding: 0px; padding-top: 20px; padding-left: 20px;">
                <div id="txtAddressModalTitle" class="sectiontitle">New Address</div>
            </div>
            <cc1:TabContainer runat="server" ID="tcAddresses" CssClass="ajax__tab_xp formpanel" OnClientActiveTabChanged="SEL.Addresses.Address.Modal.Tabs.Change">
                <cc1:TabPanel runat="server" ID="tpAddress" HeaderText="Address">
                    <ContentTemplate>
                        <div class="sectiontitle">General Details</div>
                        <div class="twocolumn">
                            <label id="lblSearch" for="txtSearch">Search</label><span class="inputs"><input type="text" id="txtSearch" maxlength="256" rel="txtSearchIdentifier" /><input type="hidden" id="txtSearchIdentifier" /></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                            <asp:Label runat="server" ID="lblFavourite" AssociatedControlID="chkFavourite">Favourite</asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkFavourite" /></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                        </div>
                        <div class="twocolumn">
                            <asp:Label runat="server" ID="lblAddressName" AssociatedControlID="txtAddressName">Name</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtAddressName" MaxLength="250"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                            <asp:Label runat="server" ID="lblLine1" AssociatedControlID="txtLine1" CssClass="mandatory">Line 1*</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtLine1" MaxLength="256"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator runat="server" ID="rfvLine1" ValidationGroup="vgAddress" Text="*" ErrorMessage="Please enter a Line 1." ControlToValidate="txtLine1"></asp:RequiredFieldValidator></span>
                        </div>
                        <div class="twocolumn">
                            <asp:Label runat="server" ID="lblLine2" AssociatedControlID="txtLine2">Line 2</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtLine2" MaxLength="256"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                            <asp:Label runat="server" ID="lblLine3" AssociatedControlID="txtLine3">Line 3</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtLine3" MaxLength="256"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                        </div>
                        <div class="twocolumn">
                            <asp:Label runat="server" ID="lblCity" AssociatedControlID="txtCity">City</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtCity" MaxLength="256"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                            <asp:Label runat="server" ID="lblCounty" AssociatedControlID="txtCounty">County</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtCounty" MaxLength="256"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                        </div>
                        <div class="twocolumn">
                            <asp:Label runat="server" ID="lblCountry" AssociatedControlID="ddlCountry">Country</asp:Label><span class="inputs"><asp:DropDownList runat="server" ID="ddlCountry" /></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                            <asp:Label runat="server" ID="lblPostcode" AssociatedControlID="txtPostcode">Postcode</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtPostcode" MaxLength="32"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator runat="server" ID="rfvPostcode" ValidationGroup="vgAddress" Text="*" ErrorMessage="Please enter a Postcode." ControlToValidate="txtPostcode" Enabled="False"></asp:RequiredFieldValidator></span>
                        </div>
                    </ContentTemplate>
                </cc1:TabPanel>
                <cc1:TabPanel runat="server" ID="tpAccountWideLabels" HeaderText="Account-wide Labels">
                    <ContentTemplate>
                        <div class="sectiontitle">General Details</div>

                        <span class="selui-button-add" onclick="SEL.Addresses.Address.AccountWideLabels.New();">
                            <img id="btnAddAwabel" src="/shared/images/icons/16/plain/add2.png" alt="New Account-wide Label" title="New Account-wide Label" class="btn" />New Account-wide Label</span>

                        <div id="gridAwabels">
                            <asp:PlaceHolder runat="server" ID="phAwabelsGrid"></asp:PlaceHolder>
                        </div>
                    </ContentTemplate>
                </cc1:TabPanel>
                <cc1:TabPanel runat="server" ID="tpRecommendedDistances" HeaderText="Recommended Distances">
                    <ContentTemplate>
                        <div class="sectiontitle">Override a Recommended Distance</div>
                        <table class="cGrid" style="width: 100%;">
                            <thead>
                                <tr>
                                    <th>Destination</th>
                                    <th colspan="3">Outbound</th>
                                    <th colspan="3">Return</th>
                                    <th></th>
                                </tr>
                                <tr class="grid-subheading">
                                    <th></th>
                                    <th class="grid-subheading-column" style="width:70px">Custom</th>
                                    <th class="grid-subheading-column">Fastest</th>
                                    <th class="grid-subheading-column">Shortest</th>
                                    <th class="grid-subheading-column" style="width:70px">Custom</th>
                                    <th class="grid-subheading-column">Fastest</th>
                                    <th class="grid-subheading-column">Shortest</th>
                                    <th style="width:25px"></th>
                                </tr>
                            </thead>
                            <tbody id="tbodyNewRecommendedDistance">
                                <tr>
                                    <td class="row1">
                                        <input type="text" id="txtDestinationSearch" maxlength="256" rel="txtDestinationSearchIdentifier" style="width: 350px;" /><input type="hidden" id="txtDestinationSearchIdentifier" /></td>
                                    <td class="row1">
                                        <asp:TextBox runat="server" ID="tbAO" CssClass="addresses-outbound" MaxLength="8" /><cc1:FilteredTextBoxExtender runat="server" FilterType="Custom" ValidChars="0123456789." TargetControlID="tbAO" />
                                    </td>
                                    <td class="row1"><span class="addresses-outboundfastest"></span></td>
                                    <td class="row1"><span class="addresses-outboundshortest"></span></td>
                                    <td class="row1">
                                        <asp:TextBox runat="server" ID="tbAR" CssClass="addresses-return" MaxLength="8" /><cc1:FilteredTextBoxExtender runat="server" FilterType="Custom" ValidChars="0123456789." TargetControlID="tbAR" />
                                    </td>
                                    <td class="row1"><span class="addresses-returnfastest"></span></td>
                                    <td class="row1"><span class="addresses-returnshortest"></span></td>
                                    <td class="row1">
                                        <img id="btnAddRecommendedDistance" src="/shared/images/icons/16/plain/add2.png" alt="add distance" title="add distance" onclick="SEL.Addresses.Address.RecommendedDistances.Add();" class="btn" /></td>
                                </tr>
                            </tbody>
                        </table>
                        <div class="sectiontitle">Address Distances</div>
                        <div id="pnlDestinations">
                            <table class="cGrid" style="width: 100%;">
                                <thead>
                                    <tr>
                                        <th>Destination</th>
                                        <th colspan="3">Outbound</th>
                                        <th colspan="3">Return</th>
                                    </tr>
                                    <tr class="grid-subheading">
                                        <th></th>
                                        <th class="grid-subheading-column" style="width:70px">Custom</th>
                                        <th class="grid-subheading-column">Fastest</th>
                                        <th class="grid-subheading-column">Shortest</th>
                                        <th class="grid-subheading-column" style="width:70px">Custom</th>
                                        <th class="grid-subheading-column">Fastest</th>
                                        <th class="grid-subheading-column">Shortest</th>
                                    </tr>
                                </thead>
                                <tbody id="tbodyRecommendedDistancesContainer"></tbody>
                            </table>
                        </div>
                    </ContentTemplate>
                </cc1:TabPanel>
            </cc1:TabContainer>
            <div class="formpanel">
                <div class="formbuttons">
                    <helpers:CSSButton runat="server" ID="btnSaveAddress" Text="save" OnClientClick=" SEL.Addresses.Address.RecommendedDistances.CheckAddAndSave();return false; " UseSubmitBehavior="False" /> 
                    <helpers:CSSButton runat="server" ID="btnCancelAddress" Text="cancel" OnClientClick=" SEL.Addresses.Address.Cancel();return false; " UseSubmitBehavior="False" />
                </div>
            </div>
        </div>
    </asp:Panel>

    <cc1:ModalPopupExtender runat="server" ID="mdlAddress" BackgroundCssClass="modalBackground" TargetControlID="lnkAddress" PopupControlID="pnlAddress"></cc1:ModalPopupExtender>
    <asp:HyperLink runat="server" ID="lnkAddress" Style="display: none;"></asp:HyperLink>
                 
    <asp:Panel runat="server" ID="pnlAwabel" CssClass="modalpanel formpanel" Style="display: none;">
        <div class="sectiontitle" id="awabelTitle">New Account-wide Label</div>
        <div class="twocolumn">
            <asp:Label runat="server" ID="lblAwabel" AssociatedControlID="txtAwabel" CssClass="mandatory">Label*</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtAwabel" MaxLength="50"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator runat="server" ID="rfvAwabelLabel" ValidationGroup="vgAwabel" ErrorMessage="Please enter a Label." ControlToValidate="txtAwabel">*</asp:RequiredFieldValidator></span>
                            
            <asp:Label runat="server" ID="lblPrimaryAwabel" AssociatedControlID="chkPrimaryAwabel">Primary label</asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkPrimaryAwabel" /></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
        </div>
        <div class="formbuttons">
            <helpers:CSSButton runat="server" ID="btnSaveAwabel" Text="save" OnClientClick=" SEL.Addresses.Address.AccountWideLabels.Save();return false; " UseSubmitBehavior="False" /> 
            <helpers:CSSButton runat="server" ID="btnCancelAwabel" Text="cancel" OnClientClick=" SEL.Addresses.Address.AccountWideLabels.Cancel();return false; " UseSubmitBehavior="False" />
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender runat="server" ID="mdlAwabel" BackgroundCssClass="modalBackground" TargetControlID="lnkAwabel" PopupControlID="pnlAwabel" />
    <asp:HyperLink runat="server" ID="lnkAwabel" Style="display: none;"></asp:HyperLink>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="scripts" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('a:contains("New Address")').click(function() {
                $("body").css("overflow", "hidden");
            });

            

            $("#ctl00_contentmain_btnSaveAddress").click(function() {
                $("body").css("overflow", "auto");
            });

            $("#ctl00_contentmain_btnCancelAddress").click(function() {
                $("body").css("overflow", "auto");
            });     

        });
    </script>

</asp:Content>
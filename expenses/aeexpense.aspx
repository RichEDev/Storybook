<%@ Page Language="C#" Trace="false" MasterPageFile="~/expform.master" AutoEventWireup="true" Inherits="aeexpense" CodeBehind="aeexpense.aspx.cs" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>

<%@ Import Namespace="SpendManagementLibrary" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ MasterType VirtualPath="~/expform.master" %>
<%@ Register Src="~/shared/usercontrols/addressDetailsPopup.ascx" TagName="Popup" TagPrefix="AddressDetails" %>
<%@ Register Src="~/shared/usercontrols/aeCars.ascx" TagPrefix="aeCars" TagName="aeCar" %>
<%@ Register Src="~/shared/usercontrols/tooltip.ascx" TagName="tooltip" TagPrefix="tooltip" %>


<asp:Content ID="Content4" ContentPlaceHolderID="head" runat="Server">
    <!--for handling and applying dots for the overflow in expense item-->
    <style type="text/css">
        .ellide input[type=checkbox] {
            height: 15px;
        }

        .inputpaneltitle {
            margin-top: 30px;
        }

        .submenuitem:focus {
            outline: none;
        }

        select {
            width: 158px;
            border: 1px solid #BFBFBF;
            margin-top: 2px;
            margin-bottom: 2px;
            padding: 3px;
            height: 25px;
            font-size: 12px;
        }

        .datatbl {
            width: 75%;
        }

            .datatbl td {
                vertical-align: top;
            }

            .datatbl select {
                width: 99%;
                margin-left: 2px;
            }

            .datatbl input {
                width: 90%;
                margin-left: 2px;
                padding: 4px;
            }

        .addnewvehiclelink {
            text-decoration: underline;
        }

        .waitingtobeapproved {
            font-size: 13px;
            display: inline-block !important;
            margin: 4px 5px 5px 5px;
        }

        .helplinkicon {
            margin-bottom: 3px !important;
        }

        #imgtooltip236, #imgtooltip240, #imgtooltip242 {
            margin-right: 20px;
            margin-left: -25px;
        }

        .datatbl input[type="image"] {
            margin-left: -3px;
            margin-top: 4px;
            width: auto;
        }

        #ctl00_contentmain_imgcal {
            padding-left: 4px;
        }

        .btn {
            padding: 6px 8px 6px 4px;
        }
    </style>

</asp:Content>

<asp:Content ID="submenuContent" ContentPlaceHolderID="contentleft" runat="Server">
  <%--  <script type="text/javascript" language="javascript">

        Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function (sender, args) {
           
            console.info(SEL.Expenses.CostCodeBreakdown);

            if (SEL.Expenses.CostCodeBreakdown != undefined) {

                for (var i = 0; i < SEL.Expenses.CostCodeBreakdown.length; i++) {

                    if (SEL.Expenses.CostCodeBreakdown[i].control.includes("CostCode")) {
                        popDropdown(SEL.Expenses.CostCodeBreakdown[i].control, SEL.Expenses.CostCodeBreakdown[i].items);
                    }

                
                }

            }


          
    //        popDropdown(SEL.Expenses.CostCodeBreakdown[0]);
        });
    </script>--%>
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/scripts/addingexpenses.js?date=20170907" />
            <asp:ScriptReference Path="~/shared/javascript/attachments.js" />
            <asp:ScriptReference Name="tooltips" />
            <asp:ScriptReference Name="expenses" />
            <asp:ScriptReference Path="~/shared/javascript/sel.MileageGrid.js" />
            <asp:ScriptReference Path="~/shared/javascript/sel.ajax.js" />
            <asp:ScriptReference Path="~/shared/javascript/minify/jquery.ba-outside-events.min.js" />
            <asp:ScriptReference Path="~/shared/javascript/minify/jquery.tokeninput.js" />
            <asp:ScriptReference Path="~/shared/javaScript/minify/sel.addresses.and.travel.js" />
            <asp:ScriptReference Path="~/shared/javaScript/sel.claims.js?date=20161112" />
        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/shared/webServices/svcCars.asmx" />
        </Services>
    </asp:ScriptManagerProxy>

    <script type="text/javascript" language="javascript">
        var usualMileage = 0;
        var parddlst = null;
        var index;
        var carLinkID;
        var pceMissingPostCodes = '<% = pceMissingPostcodes.ClientID %>';
        var pceGreyAdd = '<% = pceGreyAdd.ClientID %>';
        var pceProcessingAdd = '<% = pceProcessingAdd.ClientID %>';
        var modalAddCar = '<%= modalAddCar.ClientID %>';
        var pnlSpecClientID = "<%=upnlSpecific.ClientID %>";
        var hdnExpDate = "<%=hdnExpDate.ClientID %>";
        var hdnWorkAddressID = "<%=hdnWorkAddressId.ClientID %>";
        var hdnClaimOwnerId = "<%=hdnClaimOwnerId.ClientID %>";
        var postcodeAnywhereKey = "<% =PostcodeAnywhereKey %>";
        var allowClaimantAddManualAddresses = <%= AllowClaimantAddManualAddresses ? "true" : "false" %>;
        var forceAddressNameEntry = <%= ForceAddressNameEntry ? "true" : "false" %>;
        var addressNameEntryMessage = "<%= AddressNameEntryMessage %>";
        var OrganisationSearches = {};
        var OrganisationSearch = null;
        var OrganisationSearchModal = '<% = mdlOrganisationSearch.ClientID %>';
        var OrganisationSearchPanel = '<% = pnlOrganisationSearch.ClientID %>';
        var CostCodeSearchPanel = '<% = pnlCostCodeSearch.ClientID %>';

        $(document).ready(function() { 
            
            (function(s) {
                s.AllowClaimantAddOrganisations = <%= AllowClaimantAddOrganisations ? "true" : "false" %>;
                s.OrganisationLabel = "<%= OrganisationLabel %>";
                s.HomeAddressKeyword = "<%= HomeAddressKeyword %>";
                s.WorkAddressKeyword = "<%= WorkAddressKeyword %>";
            }(SEL.Expenses.Settings));
            SEL.MileageGrid.setup();          
            (function(o) {
                o.Modal = OrganisationSearchModal;
                o.Panel = OrganisationSearchPanel;
            }(SEL.Expenses.Dom.OrganisationSearch));
        });


        function SetPreviousAddress(previousIDObject, currentIDObject) {
            var previousObject = document.getElementById(previousIDObject);
            var currentObject = document.getElementById(currentIDObject);
            if (previousIDObject.value != "" && currentObject.value != "") {
                if (previousObject.value != currentObject.value) {
                    previousObject.value = currentObject.value;
                }
            }
        }
        
        function validateAeExpense() {
            var errorMsg = null;
            $(".mileagePanel").each(function() {
                var enforcedJourneyRateOption = $("select[id*='cmbmileagecat'][data-enforced=true] option:selected", this);
                var selectedCarOption = $("select[id*=cmbcar] option:selected", this);
                if (enforcedJourneyRateOption.data("uom") == "km" && selectedCarOption.data("defaultuom") == "mile" &&
                    $(".userentereddistance", this).val()) {
                    errorMsg = "You cannot claim for '" + selectedCarOption.text() + "' which is in miles " +
                        "as the journey rate is enforced as '" + enforcedJourneyRateOption.text() + "' which is in km.";
                    return;
                } else if (enforcedJourneyRateOption.data("uom") == "mile" && selectedCarOption.data("defaultuom") == "km" &&
                    $(".userentereddistance", this).val()) {
                    errorMsg = "You cannot claim for '" + selectedCarOption.text() + "' which is in km " +
                        "as the journey rate is enforced as '" + enforcedJourneyRateOption.text() + "' which is in miles.";
                    return;
                }
            });
            if (errorMsg) {
                SEL.MasterPopup.ShowMasterPopup(errorMsg, "Message from " + moduleNameHTML);
                return false;
            }

            return true;
        }

        //Checks to see if the user has entered a valid rate in the exchange rate box. If they have not, they receive the relevant popup, and cannot save until it is corrected. 
        function checkExchangeRate() {
            var errorMsg = null;
            var editableRateVisible = $("#ctl00_contentmain_cellexchinput").is(":visible");            
            if (editableRateVisible) {
                var editableRateValue = $g("ctl00_contentmain_txtexchangerate").value;
                if (editableRateValue) {
                    if (parseFloat(editableRateValue) <= 0) {
                        errorMsg = "Please enter an exchange rate greater than 0.";
                    }
                    if (editableRateValue.trim() === "") {
                        errorMsg = "Please enter an exchange rate.";
                    }
                }
                else {
                    errorMsg = "Please enter an exchange rate.";
                }
            }
            if (errorMsg) {
                SEL.MasterPopup.ShowMasterPopup(errorMsg, "Message from " + moduleNameHTML);
                $("#ctl00_contentmain_exchangeratemandatory")[0].innerText = "*";
                $("#ctl00_contentmain_exchangeratemandatory")[0].style.color = "red";
                return false;
            }
            $("#ctl00_contentmain_exchangeratemandatory")[0].innerText = "";
            return true;
        }

    </script>
  
    <div class="panel">
        <asp:Literal ID="lititemcomment" runat="server" meta:resourcekey="lititemcommentResource1"></asp:Literal>

        <asp:UpdatePanel ID="upnlitems" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:CheckBoxList ID="chkitems" runat="server" CssClass="ellide" OnSelectedIndexChanged="chkitems_SelectedIndexChanged" AutoPostBack="True" RepeatLayout="Flow" meta:resourcekey="chkitemsResource1">
                </asp:CheckBoxList>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="upnlitems" DisplayAfter="0">
            <ProgressTemplate>
                <div class="left-menu-click-back">
                    <p style="color: #ffffff">updating your selection, please wait...</p>
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>

    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="Server">
    <tooltip:tooltip ID="usrTooltip" runat="server"></tooltip:tooltip>
    <div class="valdiv" id="valdiv">
        <asp:ValidationSummary ValidationGroup="vgAeExpenses" ID="vgAeExpenses" runat="server" Width="100%" ShowMessageBox="True" ShowSummary="False" meta:resourcekey="vtrsummaryResource1"></asp:ValidationSummary>
        <br />
        <asp:Label ID="lblmsg" runat="server" Visible="False" Font-Size="Small" ForeColor="Red" meta:resourcekey="lblmsgResource1">Label</asp:Label>
    </div>
    <div class="inputpanel">
        <div class="inputpaneltitle">
            <asp:Label ID="Label1" runat="server" meta:resourcekey="Label1Resource1" CssClass="inputpaneltitlelabel">General Details</asp:Label>
        </div>
        <asp:UpdatePanel ID="upnlgeneral" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Panel ID="pnlgeneral" runat="server" meta:resourcekey="pnlgeneralResource1">
                    <asp:HiddenField ID="hdnExpDate" runat="server" />
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional"
        ChildrenAsTriggers="True">
        <ContentTemplate>
            <asp:Literal ID="litcostcodeheader" runat="server" meta:resourcekey="litcostcodeheaderResource1"></asp:Literal>
            <asp:Table ID="tblcostcodes" runat="server" meta:resourcekey="tblcostcodesResource1">
            </asp:Table>
            <asp:TextBox ID="txtcostcodetotal" runat="server" Text="100" meta:resourcekey="txtcostcodetotalResource1"></asp:TextBox><asp:CompareValidator ValidationGroup="vgAeExpenses"
                ID="compcostcodetotal" ErrorMessage="The item breakdown must equal 100%" ControlToValidate="txtcostcodetotal" ValueToCompare="100" runat="server" meta:resourcekey="compcostcodetotalResource1"></asp:CompareValidator>
            <asp:Literal ID="litcostcodefooter" runat="server" meta:resourcekey="litcostcodefooterResource1"></asp:Literal>
        </ContentTemplate>
    </asp:UpdatePanel>

    <div class="inputpanel">

        <asp:UpdatePanel ID="upnlSpecific" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Panel ID="pnlspecific" runat="server" onclick="RadioBoxCheck()"  meta:resourcekey="pnlspecificResource1">
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div class="inputpanel">&nbsp;</div>
    <div class="inputpanel">
        <helpers:CSSButton id="cmdok" runat="server" text="save"  UseSubmitBehavior="True" OnClick="cmdok_Click" OnClientClick="if(!(validateAeExpense())) return false;"/>
        &nbsp;&nbsp;<helpers:CSSButton ID="cmdcancel" text="cancel" CausesValidation="False" runat="server"  OnClick="cmdcancel_Click" meta:resourcekey="cmdcancelResource1" />
    </div>
    <asp:Panel ID="pnlAddcar" runat="server" CssClass="modalpanel formpanel" Style="padding: 20px; width: 900px; display: none;">
        <div class="sectiontitle">Add new vehicle</div>
        <aeCars:aeCar ID="addCar" runat="server" inModalPopup="true" isAeExpenses="true" />
    </asp:Panel>
    <asp:LinkButton ID="lnkAddCar" runat="server" Style="display: none;">&nbsp;</asp:LinkButton>
    <cc1:ModalPopupExtender ID="modalAddCar" runat="server" PopupControlID="pnlAddcar" TargetControlID="lnkAddCar" BackgroundCssClass="modalBackground"></cc1:ModalPopupExtender>
    <cc1:PopupControlExtender ID="pceMissingPostcodes" runat="server" PopupControlID="pnlMissingPostcodes" TargetControlID="lnkMissingPostcodes" OffsetX="16" OffsetY="16"></cc1:PopupControlExtender>
    <asp:Panel ID="pnlMissingPostcodes" runat="server" CssClass="popupSemiTransparent">
        <div class="popupContent">
            Unable to calculate distance due<br />
            to missing postcode(s)
        </div>
    </asp:Panel>
    <asp:HyperLink ID="lnkMissingPostcodes" runat="server" Text="&nbsp;">&nbsp;</asp:HyperLink>


    <cc1:PopupControlExtender ID="pceGreyAdd" runat="server" PopupControlID="pnlGreyAdd" TargetControlID="lnkGreyAdd" OffsetX="17" OffsetY="16"></cc1:PopupControlExtender>
    <asp:Panel ID="pnlGreyAdd" runat="server" CssClass="popupSemiTransparent">
        <div class="popupContent">
            A new journey step cannot be created, please check<br />
            that all of the necessary fields have been completed
        </div>
    </asp:Panel>
    <asp:HyperLink ID="lnkGreyAdd" runat="server" Text="&nbsp;">&nbsp;</asp:HyperLink>
    <cc1:PopupControlExtender ID="pceProcessingAdd" runat="server" PopupControlID="pnlProcessingAdd" TargetControlID="lnkProcessingAdd" OffsetX="17" OffsetY="16"></cc1:PopupControlExtender>
    <asp:Panel ID="pnlProcessingAdd" runat="server" CssClass="popupSemiTransparent">
        <div class="popupContent">
            A new journey step cannot be created, please wait<br />
            for a moment whilst the information finishes loading
        </div>
    </asp:Panel>
    <asp:HyperLink ID="lnkProcessingAdd" runat="server" Text="&nbsp;">&nbsp;</asp:HyperLink>

    <AddressDetails:Popup ID="addressDetailsPopup" runat="server" />

    <asp:Panel runat="server" ID="mapContainer"></asp:Panel>
    <asp:HiddenField runat="server" ID="hdnClaimOwnerId" />
    <asp:HiddenField runat="server" ID="hdnWorkAddressId" />
    <asp:HiddenField runat="server" ClientIDMode="Static" ID="hdnSubCatDates"/>

    <asp:Panel runat="server" ID="pnlOrganisationSearch" CssClass="modalpanel formpanel" Style="display: none;">
        <div class="sectiontitle">
            <asp:Label runat="server" ID="lblOrganisationSearchTitle">Organisation</asp:Label>
            Search
        </div>
        <div class="searchgrid"></div>
        <div class="formbuttons">
            <helpers:CSSButton runat="server" ID="btnOrganisationSearchCancel" Text="cancel" OnClientClick="return false;" UseSubmitBehavior="False" />
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender runat="server" ID="mdlOrganisationSearch" BackgroundCssClass="modalBackground" TargetControlID="lnkOrganisationSearch" PopupControlID="pnlOrganisationSearch" CancelControlID="btnOrganisationSearchCancel" />
    <asp:LinkButton runat="server" ID="lnkOrganisationSearch" Style="display: none;"></asp:LinkButton>
   
    <asp:Panel runat="server" ID="pnlCostCodeSearch" CssClass="modalpanel formpanel" Style="display: none;">
        <div class="sectiontitle">
            <asp:Label runat="server" ID="lblCostCodeSearchTitle">Cost Code</asp:Label>
            Search
        </div>
        <div class="searchgrid"></div>
        <div class="formbuttons">
            <helpers:CSSButton runat="server" ID="btnCostCodeSearchCancel" Text="cancel" OnClientClick="return false;" UseSubmitBehavior="False" />
        </div>
    </asp:Panel>

    <div id="flagSummary">

        <div id="divFlags"></div>
    </div>

</asp:Content>

<asp:Content ID="sidemenuContentContent" runat="server" ContentPlaceHolderID="contentmenu">
    <asp:Literal ID="litclear" runat="server" meta:resourcekey="litclearResource1"></asp:Literal>
</asp:Content>
<asp:Content ID="scriptsContent" runat="server" ContentPlaceHolderID="scripts">
    <script type="text/javascript" src="<%=GlobalVariables.StaticContentLibrary%>/js/expense/jquery.smoothscroll.js"></script>
    <script type="text/javascript">

        function RadioBoxCheck() {
            var numItemsTextBox = document.getElementById(contentID + 'txtnumitems');
            if (numItemsTextBox) {
                var numitems = numItemsTextBox.value;
                for (var i = 0; i < numitems; i++) {
                    if ($('#optnormalreceiptno' + i).is(':checked')) {
                        if($("#optvatreceiptyes"+i).length>0 && $("#optvatreceiptno"+i).length>0)
                        {
                            $('#optvatreceiptyes' + i).prop("checked", false);
                            $('#optvatreceiptno' + i).prop("checked", true);
                            $('#optvatreceiptyes' + i).attr("disabled", true);
                            $('#optvatreceiptno' + i).attr("disabled", true);
                        }
                    }
                    else {
                        if($("#optvatreceiptyes"+i).length>0 && $("#optvatreceiptno"+i).length>0)
                        {
                            $('#optvatreceiptyes' + i).attr("disabled", false);
                            $('#optvatreceiptno' + i).attr("disabled", false);
                        }
                    }
                }    
            }
        }
        $(document).ready(function() {
            RadioBoxCheck();
            $(".addnewvehiclelink").click(function () {
                $("body").css("overflow", "hidden");

            });

            $(".formbuttons").click(function () {
                $("body").css("overflow", "auto");
            });

            var width = $(window).width(), height = $(window).height();
            $(".inputpaneltitle:first").css("margin-top", 0);
            $(".inputpanel:eq(2)").css("margin-top", "-30px");

            $('td:contains("Expense Item:")').css("padding-left", "20px");


            if($('[id^="ctl00_contentmain_pnl"]').parent().is('div'))
            {
                $('[id^="ctl00_contentmain_pnl"]').parent().css('height','auto');
            }
            

            setTimeout(function() { createScroll(false); }, 500);
            $(document).on('click','#<%=chkitems.ClientID%>', function() {
                setTimeout(function() {bufferTime(); }, 500);
            });

            function bufferTime() {
                if ($('#<%=UpdateProgress1.ClientID%>').is(':visible')) {
                    setTimeout(function() { bufferTime(); }, 300);
                } else {
                    createScroll(true);
                    RadioBoxCheck();
                }
            }

            function createScroll(postBack) {
                var height = $('#maindiv').innerHeight();
                var heightIgnore = $('#maindiv > div:eq(0)').height() + $('#maindiv > div:eq(1)').height() - $('#maindiv > div:eq(2)').height() + $('#maindiv > div:eq(2)').outerHeight() + 18;
                height = height - heightIgnore;
                var innerData = $('#submenu').children(':first').outerHeight();
                var scrollHeight = height - innerData - $('#submenu > div:eq(1) > div:eq(0)').outerHeight() - $('#submenu > div:eq(1) > div:eq(1)').outerHeight();
                var item = $('#submenu > div:eq(1) > div:eq(2)');
                item.height(scrollHeight);
                item.css({ 'margin-left': '4%' });
                if (postBack == true) {
                    item.getNiceScroll().resize();
                } else {
                    item.customScroll({ cursorcolor: "#19a2e6", autohidemode: false });
                }
                
                var itemHeight = $('#ctl00_contentleft_upnlitems').height();
                $('#<%=UpdateProgress1.ClientID%> > div').height(itemHeight);
            }

            $("#menu-toggle").click(function() {
                $('#submenu > div:eq(1) > div:eq(2)').getNiceScroll().hide();
                setTimeout(function() {
                    createScroll(true);
                    $('#submenu > div:eq(1) > div:eq(2)').getNiceScroll().show();
                }, 500);

                if ((width <= 1024) && (height <= 768)) {
                    if ($('#wrapper').hasClass('toggled')) {
                        $('.labeltd').css('width','');
                    } else {
                        $('.labeltd').css('width','99px');
                    }
                }
            });


            $(window).resize(function() {
                $('#submenu > div:eq(1) > div:eq(2)').getNiceScroll().hide();
                setTimeout(function() {
                    createScroll(true);
                    $('#submenu > div:eq(1) > div:eq(2)').getNiceScroll().show();
                }, 200);
            });


            if ((width <= 1024) && (height <= 768)) {
                $('.datatbl select').css('width','200px');
                $('body').css('display','inline-block');
                $('#from_search2, #to_search3').parent().css('width','36%');
                $('table.mileagegrid tbody input[type="text"]').css('width','75%');
                $('table.mileagegrid tbody td.distance input[type="text"]').css('width','40%');
                $('.distancedetailsheader').css('width','35%');
            }
            
        });

        function pageLoad() {
            SEL.Expenses.ExpenseItems.Filter();
        }
    </script>
</asp:Content>

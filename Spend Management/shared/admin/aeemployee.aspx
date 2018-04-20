<%@ Page Language="c#" Inherits="Spend_Management.aeemployee" MasterPageFile="~/masters/smPagedForm.master" CodeBehind="aeemployee.aspx.cs" %>

<%@ MasterType VirtualPath="~/masters/smPagedForm.master" %>

<%@ Register Src="../usercontrols/aeCars.ascx" TagName="aeCars" TagPrefix="uc2" %>
<%@ Register Src="../usercontrols/CostCentreBreakdown.ascx" TagName="costCentreBreakdown" TagPrefix="uc3" %>
<%@ Register Src="../usercontrols/UploadAttachment.ascx" TagName="UploadLicenceAttachment" TagPrefix="uc4" %>
<%@ Register Src="~/shared/usercontrols/mobileDevices.ascx" TagName="mdev" TagPrefix="mdev" %>
<%@ Register Src="~/shared/usercontrols/addressDetailsPopup.ascx" TagName="Popup" TagPrefix="AddressDetails" %>
<%@ Register src="~/shared/usercontrols/bankAccounts.ascx" tagName="maccount" tagPrefix="maccount" %>

<asp:Content ID="Content4" runat="server" ContentPlaceHolderID="styles">
    <style type="text/css">

        #ctl00_contentmain_cmdactivate {
            margin-top: -20px;
        }
        
        #ctl00_contentmain_imgCancelEmployee {
            margin-top: 2px;
        }

    </style>
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu" EnableViewState="false">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy2" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/sel.ajax.js" />
            <asp:ScriptReference Path="~/shared/javaScript/userdefined.js?date=20180417" />
            <asp:ScriptReference Path="~/shared/javaScript/employees.js?date=20171114" />

        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/shared/webServices/svcCars.asmx" />
            <asp:ServiceReference Path="~/shared/webServices/svcAutoComplete.asmx" />
            <asp:ServiceReference Path="~/shared/webServices/svcAuthoriserLevel.asmx" />
        </Services>
    </asp:ScriptManagerProxy>
    <asp:Literal runat="server" ID="pageOptions"></asp:Literal>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentoptions" runat="server">
    <div id="pgOptCars" style="display: none;">
        <a href="javascript:document.getElementById(txtstartodo).disabled = false;carid=0;showCarModal(true, true);" class="submenuitem">New Vehicle</a>
    </div>
    <div id="pgOptPoolCars" style="display: none;">
        <a href="javascript:showPoolCarModal(true);" class="submenuitem">New Pool Vehicle</a>
    </div>
    <div id="pgOptCorporateCards" style="display: none;">
        <a href="javascript:corporatecardid = 0;showCorporateCardModal();" class="submenuitem">New Corporate Card</a>
    </div>
    <div id="pgOptWorkLocations" style="display: none;">
        <a href="javascript:worklocationid=0;addNewWorkLocation = true; showWorkLocationModal(true);"
            class="submenuitem">New Work Address</a>
    </div>
    <div id="pgOptHomeLocations" style="display: none;">
        <a href="javascript:homelocationid=0;addNewHomeLocation = true; showHomeLocationModal(true);"
            class="submenuitem">New Home Address</a>
    </div>
    <div id="pgOptMobileDevices" style="display: none;">
        <a href="javascript:showMobileDevice(true);" class="submenuitem">New Mobile Device</a>
    </div>
    <div id="pgOptGeneral" style="">
        <asp:Literal ID="litShowEsr" runat="server"></asp:Literal>
    </div>
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">

    <script type="text/javascript" language="javascript">
        var txtusername = '<%=txtusername.ClientID %>';
        var txttitle = '<%=txttitle.ClientID %>';
        var txtfirstname = '<%=txtfirstname.ClientID %>';
        var txtmiddlenames = '<%=txtmiddlenames.ClientID %>';
        var txtsurname = '<%=txtsurname.ClientID %>';
        var txtmaidenname = '<%=txtmaidenname.ClientID %>';
        var txtpreferredname = '<%=txtpreferredname.ClientID %>';
        var txtemail = '<%=txtemail.ClientID %>';
        var txtextension = '<%=txtextension.ClientID %>';
        var txtmobileno = '<%=txtmobileno.ClientID %>';
        var txtpagerno = '<%=txtpagerno.ClientID %>';
        var txtemailhome = '<%=txtemailhome.ClientID %>';
        var txttelno = '<%=txttelno.ClientID %>';
        var txtfaxno = '<%=txtfaxno.ClientID %>';
        var txtcreditaccount = '<%=txtcreditaccount.ClientID %>';
        var txtpayroll = '<%=txtpayroll.ClientID %>';
        var txtposition = '<%=txtposition.ClientID %>';
        var txtninumber = '<%=txtninumber.ClientID %>';
        var txtemployeenumber = '<%=txtEmployeeNumber.ClientID %>';
        var txtnhsuniqueid = '<%=txtuniquenhsid.ClientID %>';
        var txthiredate = '<%=txthiredate.ClientID %>';
        var txtleavedate = '<%=txtleavedate.ClientID %>';
        var cmbcountry = '<%=cmbcountry.ClientID %>';
        var cmbcurrency = '<%=cmbcurrency.ClientID %>';

        var cmblinemanager = '<%= txtLineManagerSelect.ClientID %>';
        var txtLineManager = '<%= txtLineManager.ClientID %>';
        var txtLineManagerId = '<%= txtLineManager_ID.ClientID %>';
        var lineManagerSearchPanel = '<%= txtLineManagerSearchPanel.ClientID %>';
        var lineManagerSearchModal = '<%= txtLineManagerSearchModal.ClientID %>';
        var lineManagerCombo = AutoCompleteSearches.New('LineManager', txtLineManager, lineManagerSearchModal, lineManagerSearchPanel);

        var cmbsignoffs = '<%=cmbsignoffs.ClientID %>';
        var cmbsignoffcc = '<%=cmbsignoffcc.ClientID %>';
        var cmbsignoffpc = '<%=cmbsignoffpc.ClientID %>';
        var cmbadvancesgroup = '<% =cmbadvancesgroup.ClientID %>';

        var cmbgender = '<%=cmbgender.ClientID %>';
        var txtdateofbirth = '<%=txtdateofbirth.ClientID %>';
        var txtstartmiles = '<%=txtstartmiles.ClientID %>';
        var txtstartmilesdate = '<%=txtstartmileagedate.ClientID %>';
   <%--     var txtlicenceexpiry = '<%=txtlicenceexpiry.ClientID %>';
        var txtlicencenumber = '<%=txtlicencenumber.ClientID %>';
        var txtlastchecked = '<%=txtlastchecked.ClientID %>';--%>
        var modcorporatecard = '<%=modcorporatecard.ClientID %>';
        var cmbcardprovider = '<%=cmbcardprovider.ClientID %>';
        var txtcardnumber = '<%=txtcardnumber.ClientID %>';
        var chkCardActive = '<%=chkCardActive.ClientID %>';
        var modworklocation = '<%=modworklocation.ClientID %>';
        var modhomelocation = '<%=modhomelocation.ClientID %>';
        var modcar = '<%=modcar.ClientID %>';

        var modpoolcar = '<%= modpoolcar.ClientID %>';
        var poolCarSearchModal = '<%= txtPoolCarSearchModal.ClientID %>';
        var poolCarSearchPanel = '<%= txtPoolCarSearchPanel.ClientID %>';
        var txtPoolCar = '<%= txtPoolCar.ClientID %>';
        var poolCarCombo = AutoCompleteSearches.New("PoolCar", txtPoolCar, poolCarSearchModal, poolCarSearchPanel);

        var txtworklocation = '<%=txtworklocation.ClientID %>';
        var txtworklocationid = '<%=hdnWorkLocationID.ClientID %>';
        var txtworklocationstart = '<%=dtworklocationstart.ClientID %>';
        var txtworklocationend = '<%=dtworklocationend.ClientID %>';
        var chkworklocationtemporary = '<%=chkworklocationtemporary.ClientID %>';
        var chkworklocationprimaryrotational = '<%=chkPrimaryRotationalAddress.ClientID %>';
        var modNewAccessRoles = '<%=modNewAccessRoles.ClientID %>';
        var txthomelocation = '<%=txthomelocation.ClientID %>';
        var txthomelocationid = '<%= hdnHomeLocationID.ClientID %>';
        var txthomelocationstart = '<%=dthomelocationstart.ClientID %>';
        var txthomelocationend = '<%=dthomelocationend.ClientID %>';
        var ddlstLocale = '<%=ddlstLocale.ClientID %>';
        var modNewItemRoles = '<%=modNewItemRoles.ClientID %>';
        var modEditItemRole = '<%=modEditItemRole.ClientID%>'
        var NHSTrustIDClientID = '<% = ddlNHSTrust.ClientID %>';
        var emailNotificationsClientID = '<% = pnlEmailNotifications.ClientID %>';
        var emailNotificationsNHSClientID = '<% = pnlESREmailNotifications.ClientID %>';
        var modesrassignment = '<%= modESRAssignmentNumber.ClientID %>';
        var chkesractive = '<%=chkesractive.ClientID %>';
        var chkesrprimary = '<%=chkesrprimary.ClientID %>';
        var txtassignmentnumber = '<%=txtassignmentnumber.ClientID %>';
        var txtesrstartdate = '<%=txtesrstartdate.ClientID %>';
        var txtesrenddate = '<%=txtesrenddate.ClientID %>';
        var chkSendPasswordKey = '<% = chkSendPasswordEmail.ClientID %>';
        var modLicenceAttachmentID = '<% = modLicenceAttachment.ClientID %>';
        var modEsrDetailsID = '<% = modEsrDetail.ClientID %>';
        var pnlEsrDetailsID = '<% = pnlEsrDetails.ClientID %>';
        var lblEsrDetailsTitleID = '<% = lblEsrDetailsTitle.ClientID %>';
        var chkWelcomeEmail = '<% = chkWelcomeEmail.ClientID %>';
        var cmbAccessRoleSubAccount = '<%= cmbSubAccounts.ClientID %>';
        var cmbDefaultSubAccount = '<%= cmbDefaultSubAccount.ClientID %>';
        var pnlNewAccessRolesGrid = '<%=pnlAccessRoleList.ClientID %>';
        var pnlESRAssignmentsGrid = '<%=pnlESRAssignments.ClientID  %>';
        var divEsrDetailsID = '<% = divEsrDetails.ClientID %>';
        var pnlCarsGrid = '<%=pnlCars.ClientID %>';
        var pnlPoolCarsGrid = '<%=pnlPoolCars.ClientID %>';
        var pnlCorporateCardsGrid = '<% = pnlCorporateCards.ClientID %>';
        var pnlItemRoleListGrid = '<% = pnlItemRoleList.ClientID %>';
        var pnlHomeLocationsGrid = '<%=pnlHomeLocations.ClientID %>';
        var pnlWorkLocationsGrid = '<%=pnlWorkLocations.ClientID %>';
        var pnlAccessRolesGrid = '<%=pnlAccessRoles.ClientID %>';
        var pnlItemRolesGrid = '<%=pnlItemRoles.ClientID %>';
        var txtSignOffOwner = '<%=txtSignOffOwner.ClientID %>';
        var cmbAuthoriserLevel = '<%=cmbAuthoriserLevel.ClientID %>';
        var chkUnAssingDefaultApprover = '<%=chkUnAssingDefaultApprover.ClientID %>';
        var hdnDefaultAuthoriserLevelId = '<%=hdnDefaultAuthoriserLevelId.ClientID %>';
        var hdnAuthoriserLevel = '<%=hdnAuthoriserLevel.ClientID %>';
        var itemRoleValue = '<%=lblItemRoleValue.ClientID%>';
        var txtItemRoleStartDate = '<%=txtItemRoleStartDate.ClientID%>';
        var txtItemRoleEndDate = '<%=txtItemRoleEndDate.ClientID%>';
        var txtExcessMileage = '<%=txtExcessMileage.ClientID%>';
        $(document).ready(function () {
            InitialiseDatePicker();
            $('#<%=chkUnAssingDefaultApprover.ClientID%>').change(function () {
                if ($(this).is(':checked')) {
                    $('#<%=cmbAuthoriserLevel.ClientID%>').attr('disabled', false);
                }
                else {
                    $('#<%=cmbAuthoriserLevel.ClientID%>').attr('disabled', true);
                    var hdnAuthoriserLevelValue = $('#<%=hdnAuthoriserLevel.ClientID%>').val();
                    $('#<%=cmbAuthoriserLevel.ClientID%>').val(hdnAuthoriserLevelValue);

                }
           

            });
        });
    </script>

    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Services>
            <asp:ServiceReference Path="~/shared/webServices/svcCars.asmx" />
        </Services>
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/employees.js" />
        </Scripts>
    </asp:ScriptManagerProxy>

    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            /* Removes text nodes (whitespace) that causes formatting issues within formpanels */
            $('.formpanel div:not(.sectiontitle, .formbuttons)').contents().filter(function () {
                return this.nodeType === 3;
            }).remove();

            // intialise the address widgets, placing them alongside the address fields in the DOM
            $("input.ui-sel-address-picker").each(function () {
                $(this).address({
                    appendTo: $(this).parents(".modalpanel"),
                    enableLabels: false,
                    enableAccountWideLabels: false,
                    enableFavourites: false,
                    enableAccountWideFavourites: false
                });
            });
        });

        function showDocument(employeeid, carid, documenttype) {
            window.open("viewcardocument.aspx?employeeid=" + employeeid + "&carid=" + carid + "&documenttype=" + documenttype, null, 'menubar=no,toolbar=no');
        }
    </script>

    <div id="divPages">
        <div id="pgGeneral" class="primaryPage">
            <cc1:TabContainer ID="tabsGeneral" runat="server">
                <cc1:TabPanel runat="server" HeaderText="General Details" ID="tabGeneral">
                    <ContentTemplate>
                        <div class="formpanel">
                            <div class="sectiontitle">
                                <asp:Label ID="Label1" runat="server" meta:resourcekey="Label1Resource1">Logon Details</asp:Label>
                            </div>
                            <div class="twocolumn">
                                <asp:Label AssociatedControlID="txtusername" ID="lblusername" runat="server" meta:resourcekey="lblusernameResource1" CssClass="mandatory">Username*</asp:Label><span class="inputs"><asp:TextBox ID="txtusername" runat="server" MaxLength="50" meta:resourcekey="txtusernameResource1" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip43" onclick="SEL.Tooltip.Show('edcc79a5-503e-4ea9-b786-84c35cb932cf', 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ValidationGroup="vgMain" ID="requsername" runat="server" ErrorMessage="Please enter a Username for this employee" ControlToValidate="txtusername" meta:resourcekey="requsernameResource1">*</asp:RequiredFieldValidator></span>
                            </div>
                            <div class="sectiontitle">
                                <asp:Label ID="Label2" runat="server" meta:resourcekey="Label2Resource1">Employee Name</asp:Label>
                            </div>
                            <div class="twocolumn">
                                <asp:Label AssociatedControlID="txttitle" CssClass="mandatory" ID="lbltitle" runat="server" meta:resourcekey="lbltitleResource1">Title*</asp:Label><span class="inputs"><asp:TextBox ID="txttitle" runat="server" MaxLength="30" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ValidationGroup="vgMain" ID="reqtitle" runat="server" ErrorMessage="Please enter a valid Title." ControlToValidate="txttitle">*</asp:RequiredFieldValidator>
                                </span>
                                <asp:Label CssClass="mandatory" ID="lblfirstname" runat="server" meta:resourcekey="lblfirstnameResource1" AssociatedControlID="txtfirstname">First Name*</asp:Label>
                                <span class="inputs">
                                    <asp:TextBox ID="txtfirstname" runat="server" MaxLength="150" meta:resourcekey="txtfirstnameResource1" CssClass="fillspan"></asp:TextBox>
</span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqfirstname" runat="server" ErrorMessage="Please enter a valid First Name." ControlToValidate="txtfirstname" meta:resourcekey="reqfirstnameResource1" ValidationGroup="vgMain">*</asp:RequiredFieldValidator>
                                </span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label AssociatedControlID="txtmiddlenames" ID="Label10" runat="server">Middle Names</asp:Label>

                                <span class="inputs">
                                    <asp:TextBox ID="txtmiddlenames" runat="server" MaxLength="150" CssClass="fillspan"></asp:TextBox>

                                </span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;
                                </span>
                                <asp:Label ID="lblsurname" runat="server" CssClass="mandatory" AssociatedControlID="txtsurname">Surname*</asp:Label>

                                <span class="inputs">
                                    <asp:TextBox ID="txtsurname" runat="server" MaxLength="150" meta:resourcekey="txtsurnameResource1" CssClass="fillspan"></asp:TextBox>

                                </span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="surname" runat="server" ControlToValidate="txtsurname" ErrorMessage="Please enter a valid Surname." meta:resourceKey="surnameResource2" Text="*" ValidationGroup="vgMain"></asp:RequiredFieldValidator>
                                </span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label ID="Label11" runat="server" AssociatedControlID="txtmaidenname">Maiden Name</asp:Label><span class="inputs"><asp:TextBox ID="txtmaidenname" runat="server" CssClass="fillspan" MaxLength="150"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblPreferredName" runat="server" AssociatedControlID="txtpreferredname">Preferred Name</asp:Label><span class="inputs"><asp:TextBox ID="txtpreferredname" runat="server" CssClass="fillspan" MaxLength="150"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="sectiontitle">
                                <asp:Label ID="Label3" runat="server" meta:resourcekey="Label3Resource1">Employment Contact Details</asp:Label>
                            </div>
                            <div class="twocolumn">
                                <asp:Label ID="lblemail" runat="server" meta:resourcekey="lblemailResource1" AssociatedControlID="txtemail">Email Address</asp:Label>

                                <span class="inputs">
                                    <asp:TextBox ID="txtemail" runat="server" MaxLength="200" meta:resourcekey="txtemailResource1" CssClass="fillspan"></asp:TextBox>

                                </span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip50" alt="" class="tooltipicon" onclick="SEL.Tooltip.Show('375f54bc-ca51-4be9-8175-d788c07e09ac', 'sm', this);" src="../images/icons/16/plain/tooltip.png" /></span><span class="inputvalidatorfield"><asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtemail" ErrorMessage="Please enter a valid email address in the box provided" Text="*" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="vgMain"></asp:RegularExpressionValidator>
                                </span>
                                <asp:Label ID="lblextension" runat="server" meta:resourcekey="lblextensionResource1" AssociatedControlID="txtextension">Extension Number</asp:Label>

                                <span class="inputs">
                                    <asp:TextBox ID="txtextension" runat="server" meta:resourcekey="txtextensionResource1" MaxLength="50" CssClass="fillspan"></asp:TextBox>

                                </span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;
                                </span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label ID="lblmobile" runat="server" meta:resourcekey="lblmobileResource1" AssociatedControlID="txtmobileno">Mobile Number</asp:Label>

                                <span class="inputs">
                                    <asp:TextBox ID="txtmobileno" runat="server" meta:resourcekey="txtmobilenoResource1" MaxLength="50" CssClass="fillspan"></asp:TextBox>

                                </span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblpager" runat="server" meta:resourcekey="lblpagerResource1" AssociatedControlID="txtpagerno">Pager Number</asp:Label>

                                <span class="inputs">
                                    <asp:TextBox ID="txtpagerno" runat="server" meta:resourcekey="txtpagernoResource1" MaxLength="50" CssClass="fillspan"></asp:TextBox>

                                </span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="sectiontitle">
                                <asp:Label ID="Label30" runat="server" Text="Regional Settings"></asp:Label>


                            </div>
                            <div class="onecolumnsmall">
                                <asp:Label ID="Label33" runat="server" Text="Locale" AssociatedControlID="ddlstLocale"></asp:Label>

                                <span class="inputs">
                                    <asp:DropDownList ID="ddlstLocale" runat="server" CssClass="fillspan"></asp:DropDownList>

                                </span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>

                            <asp:PlaceHolder ID="holderUserdefined" runat="server"></asp:PlaceHolder>



                            <div id="divEmailEmp" class="sectiontitle" runat="server">Email Employee</div>



                            <span runat="server" id="spanPasswordKey">
                                <div class="twocolumn">
                                    <asp:Label ID="lblSendPasswordKey" runat="server" Text="Send Password Email" AssociatedControlID="chkSendPasswordEmail"></asp:Label>

                                    <span class="inputs">
                                        <asp:CheckBox ID="chkSendPasswordEmail" runat="server" Checked="True" onclick="checkSendPasswordEmail()" />
                                    </span>
                                </div>
                            </span>


                            <span runat="server" id="spanWelcomeEmail">
                                <div class="twocolumn">
                                    <asp:Label ID="lblWelcomeEmail" runat="server" Text="Send Welcome Email" AssociatedControlID="chkWelcomeEmail"></asp:Label>

                                    <span class="inputs">
                                        <asp:CheckBox ID="chkWelcomeEmail" runat="server" Checked="True" />
                                    </span>
                                </div>
                            </span>
                        </div>
                    </ContentTemplate>
                </cc1:TabPanel>
                <cc1:TabPanel runat="server" HeaderText="Permissions" ID="tabRoles">
                    <ContentTemplate>
                        <div class="formpanel">
                            <div class="sectiontitle">Employee Role Assignment</div>
                            <a href="javascript:populateNewAccessRoles();showNewAccessRoleModal(true);">Add Access Role</a>
                            <div class="twocolumn">
                                <asp:Label runat="server" ID="lblDefaultSubAccount" Text="Default Sub-Account" AssociatedControlID="cmbDefaultSubAccount"></asp:Label>
                                <span class="inputs">
                                    <asp:DropDownList runat="server" ID="cmbDefaultSubAccount" CssClass="fillspan"></asp:DropDownList></span>
                                <span class="inputicon">&nbsp;</span>
                                <span class="inputtooltipfield">&nbsp;</span>
                                <span class="inputvalidatorfield">
                                    <asp:CompareValidator ID="cvDefaultSubAccount" runat="server" ControlToValidate="cmbDefaultSubAccount" Type="Integer" Display="Dynamic" ValidationGroup="vgMain" ErrorMessage="You must declare a default subaccount for the employee" Text="*" ValueToCompare="0" Operator="GreaterThanEqual"></asp:CompareValidator></span>
                            </div>
                            <asp:Panel runat="server" ID="pnlAccessRoles">
                                <asp:Literal ID="litAccessRoles" runat="server"></asp:Literal>
                            </asp:Panel>
                        </div>
                    </ContentTemplate>
                </cc1:TabPanel>
                <cc1:TabPanel runat="server" HeaderText="Work" ID="tabWork">
                    <ContentTemplate>
                        <div class="formpanel">
                            <div class="sectiontitle">
                                <asp:Label ID="Label5" runat="server" meta:resourcekey="Label5Resource1">Employment Information</asp:Label>
                            </div>
                            <div class="twocolumn" id="creditDiv">
                                <asp:Label ID="lblcredit" runat="server" meta:resourcekey="lblcreditResource1" AssociatedControlID="txtcreditaccount">Credit Account</asp:Label><span class="inputs"><asp:TextBox MaxLength="50" ID="txtcreditaccount" runat="server" meta:resourcekey="txtcreditaccountResource1" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip59" onclick="SEL.Tooltip.Show('fdf32aa2-4e86-4319-9050-750f012b0aee', 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblpayroll" runat="server" meta:resourcekey="lblpayrollResource1" AssociatedControlID="txtpayroll">Payroll Number</asp:Label><span class="inputs"><asp:TextBox ID="txtpayroll" runat="server" meta:resourcekey="txtpayrollResource1" MaxLength="50" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label ID="lblposition" runat="server" meta:resourcekey="lblpositionResource1" AssociatedControlID="txtposition">Position</asp:Label><span class="inputs"><asp:TextBox ID="txtposition" runat="server" MaxLength="50" meta:resourcekey="txtpositionResource1" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="Label16" runat="server" Text="Label" AssociatedControlID="txtninumber">National Insurance Number</asp:Label><span class="inputs"><asp:TextBox MaxLength="50" ID="txtninumber" runat="server" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label ID="Label13" runat="server" AssociatedControlID="txthiredate">Hire Date</asp:Label><span class="inputs"><asp:TextBox ID="txthiredate" runat="server" MaxLength="50" CssClass="fillspan"></asp:TextBox><cc1:MaskedEditExtender ID="mskhiredate" MaskType="Date" Mask="99/99/9999" TargetControlID="txthiredate" CultureName="en-GB" runat="server"></cc1:MaskedEditExtender>
                                    <cc1:CalendarExtender ID="calhiredate" runat="server" TargetControlID="txthiredate" PopupButtonID="imghiredate" Format="dd/MM/yyyy"></cc1:CalendarExtender>
                                </span><span class="inputicon">
                                    <asp:Image ID="imghiredate" ImageUrl="../images/icons/cal.gif" runat="server" /></span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator ID="comphiredate" runat="server" ControlToValidate="txthiredate" ErrorMessage="The hire date you have entered is invalid" Operator="DataTypeCheck" Type="Date" ValidationGroup="vgMain" Text="*" Display="Dynamic"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpminhiredate" ControlToValidate="txthiredate" Type="Date" ValidationGroup="vgMain" Text="*" ValueToCompare="01/01/1900" Operator="GreaterThanEqual" ErrorMessage="Hire date must be on or after 01/01/1900" Display="Dynamic"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpmaxhiredate" ControlToValidate="txthiredate" Type="Date" ValidationGroup="vgMain" Text="*" ValueToCompare="31/12/3000" Operator="LessThanEqual" ErrorMessage="Hire date must be on or before 31/12/3000" Display="Dynamic"></asp:CompareValidator></span><asp:Label ID="Label17" runat="server" AssociatedControlID="txtleavedate">Termination Date</asp:Label><span class="inputs"><asp:TextBox ID="txtleavedate" runat="server" CssClass="fillspan"></asp:TextBox><cc1:MaskedEditExtender ID="MaskedEditExtender1" MaskType="Date" Mask="99/99/9999" TargetControlID="txtleavedate" CultureName="en-GB" runat="server"></cc1:MaskedEditExtender>
                                        <cc1:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtleavedate" PopupButtonID="imgleavedate" Format="dd/MM/yyyy"></cc1:CalendarExtender>
                                    </span><span class="inputicon">
                                        <asp:Image ID="imgleavedate" ImageUrl="../images/icons/cal.gif" runat="server" /></span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator ID="compleavedate" runat="server" ControlToValidate="txtleavedate" ErrorMessage="The Leave date you have entered is invalid" Operator="DataTypeCheck" Type="Date" ValidationGroup="vgMain" Text="*" Display="Dynamic"></asp:CompareValidator><asp:CompareValidator ID="compLeaveDateGreaterThanStart" runat="server" ErrorMessage="The leave date you have entered is earlier than the hire date" ControlToValidate="txtleavedate" ControlToCompare="txthiredate" Text="*" Type="Date" Operator="GreaterThanEqual" Display="Dynamic" ValidationGroup="vgMain"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpminleavedate" ControlToValidate="txtleavedate" Type="Date" ValidationGroup="vgMain" Text="*" ValueToCompare="01/01/1900" Operator="GreaterThanEqual" ErrorMessage="Leave date must be on or after 01/01/1900" Display="Dynamic"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpmaxleavedate" ControlToValidate="txtleavedate" Type="Date" ValidationGroup="vgMain" Text="*" ValueToCompare="31/12/3000" Operator="LessThanEqual" ErrorMessage="Leave date must be on or before 31/12/3000" Display="Dynamic"></asp:CompareValidator></span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label runat="server" ID="lblEmployeeNumber" AssociatedControlID="txtEmployeeNumber" Text="Employee Number"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtEmployeeNumber" CssClass="fillspan" MaxLength="30"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="onecolumnsmall">
                                <asp:Label ID="lblprimarycountry" runat="server" meta:resourcekey="lblprimarycountryResource1"
                                    AssociatedControlID="cmbcountry">Primary Country</asp:Label><span class="inputs"><asp:DropDownList CssClass="fillspan" ID="cmbcountry" runat="server" meta:resourcekey="cmbcountryResource1"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="onecolumnsmall">
                                <asp:Label ID="lblprimarycurrency" runat="server" meta:resourcekey="lblprimarycurrencyResource1" AssociatedControlID="cmbcurrency">Primary Currency</asp:Label><span class="inputs"><asp:DropDownList ID="cmbcurrency" runat="server" meta:resourcekey="cmbcurrencyResource1" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="onecolumnsmall" id="lineManagerDiv">
                                <asp:Label ID="lbllinemanager" runat="server" meta:resourcekey="lbllinemanagerResource1" AssociatedControlID="txtLineManagerSelect">Line Manager</asp:Label><span class="inputs"><asp:DropDownList ID="txtLineManagerSelect" runat="server" meta:resourcekey="cmblinemanagerResource1" CssClass="fillspan" onchange="lineManagerCombo.SelectChange();"></asp:DropDownList><asp:TextBox runat="server" ID="txtLineManager"></asp:TextBox><asp:TextBox runat="server" ID="txtLineManager_ID" Style="display: none;"></asp:TextBox></span><span class="inputicon"><asp:Image runat="server" ID="txtLineManagerSearchIcon" ImageUrl="/static/icons/16/new-icons/find.png" onclick="lineManagerCombo.Search()" /></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                            </div>

                            <div class="twocolumn" id="startMileageDiv">
                                <asp:Label ID="lblstartmileage" runat="server" meta:resourcekey="lblstartmileageResource1" AssociatedControlID="txtstartmiles">Starting Mileage</asp:Label><span class="inputs"><asp:TextBox ID="txtstartmiles" runat="server" meta:resourcekey="txtstartmilesResource1" CssClass="fillspan" MaxLength="6"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator ID="compstartmiles" runat="server" ErrorMessage="Please enter a valid value for Starting Mileage" ControlToValidate="txtstartmiles" Operator="DataTypeCheck" Type="Integer" meta:resourcekey="compstartmilesResource1" ValidationGroup="vgMain">*</asp:CompareValidator></span><asp:Label runat="server" ID="lblstartmileagedate" AssociatedControlID="txtstartmileagedate">Starting Mileage Date</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtstartmileagedate" CssClass="fillspan"></asp:TextBox><cc1:MaskedEditExtender ID="mskstartmileagedate" MaskType="Date" Mask="99/99/9999" TargetControlID="txtstartmileagedate" CultureName="en-GB" runat="server"></cc1:MaskedEditExtender>
                                    <cc1:CalendarExtender ID="calexstartmileagedate" runat="server" TargetControlID="txtstartmileagedate" PopupButtonID="imgstartmileagedate" Format="dd/MM/yyyy"></cc1:CalendarExtender>
                                </span><span class="inputicon">
                                    <asp:Image ID="imgstartmileagedate" ImageUrl="../images/icons/cal.gif" runat="server" /></span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator runat="server" ID="cmpminstartmileage" ControlToValidate="txtstartmileagedate" Type="Date" ValidationGroup="vgMain" Text="*" ValueToCompare="01/01/1900" Operator="GreaterThanEqual" ErrorMessage="Start mileage date must be on or after 01/01/1900" Display="Dynamic"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpmaxstartmileagedate" ControlToValidate="txtstartmileagedate" Type="Date" ValidationGroup="vgMain" Text="*" ValueToCompare="31/12/3000" Operator="LessThanEqual" ErrorMessage="Start mileage date must be on or before 31/12/3000" Display="Dynamic"></asp:CompareValidator></span>
                            </div>
                            <div class="twocolumn" id="startMileageDiv2">
                                <asp:Label ID="lblcurrentmileage" runat="server" meta:resourcekey="lblcurrentmileageResource1" AssociatedControlID="txtCurrentMileage">Current Mileage</asp:Label><span class="inputs"><asp:TextBox ID="txtCurrentMileage" runat="server" Enabled="false" meta:resourcekey="txtCurrentMileageResource1" CssClass="fillspan" MaxLength="6"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp</span>
                                <asp:Label ID="lblExcessMileage" runat="server" meta:resourcekey="lblexcessmileageResource1" AssociatedControlID="txtExcessMileage">Excess Mileage</asp:Label><span class="inputs"><asp:TextBox ID="txtExcessMileage" MaxLength="10" runat="server" meta:resourcekey="lblexcessmileageResource1" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltipExcessMileage" onclick="SEL.Tooltip.Show('5EE713F4-2C6B-4C73-9337-F8BB6AC0B216', 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"><asp:RegularExpressionValidator runat="server" ID="regexExcessMileage" ControlToValidate="txtExcessMileage" Text="*" ErrorMessage="Please enter a valid value for Excess Mileage" ValidationExpression="((\d+)((\.\d{1,10})?))$" ValidationGroup="vgMain" Display="Dynamic"></asp:RegularExpressionValidator></span> 
                            </div>
                            <asp:Panel ID="pnlNHSDetails" runat="server">
                                <div class="sectiontitle">
                                    <asp:Label ID="Label34" runat="server" Text="NHS Details"></asp:Label>
                                </div>
                                <div class="onecolumnsmall">
                                    <asp:Label ID="lblNHSTrust" runat="server" Text="Trust" AssociatedControlID="ddlNHSTrust"></asp:Label><span class="inputs"><asp:DropDownList ID="ddlNHSTrust" runat="server" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                                </div>
                                <div class="twocolumn">
                                    <asp:Label runat="server" ID="lblUniqueNHSId" Text="NHS Unique Id" AssociatedControlID="txtuniquenhsid"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtuniquenhsid" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                                </div>
                                <div class="sectiontitle">
                                    <asp:Label ID="Label35" runat="server" Text="ESR Assignment Numbers"></asp:Label>
                                </div>
                                <a href="javascript:addEsrAssignment();">Add ESR Assignment</a>
                                <asp:Panel ID="pnlESRAssignments" runat="server">
                                    <asp:Literal ID="litESRAssignments" runat="server"></asp:Literal>
                                </asp:Panel>
                            </asp:Panel>
                        </div>
                        <uc3:costCentreBreakdown ID="costcodebreakdown" UserControlDisplayType="Inline" EmptyValuesEnabled="True" runat="server" />
                    </ContentTemplate>








                </cc1:TabPanel>
                <cc1:TabPanel runat="server" HeaderText="Personal" ID="tabPersonal">
                    <ContentTemplate>
                        <div class="formpanel">
                            <div class="sectiontitle">
                                <asp:Label ID="Label8" runat="server" Text="Home Contact Details"></asp:Label>
                            </div>
                            <div class="twocolumn">
                                <asp:Label ID="lblemailhome" runat="server" meta:resourcekey="lblemailhomeResource1" AssociatedControlID="txtemailhome">Email Address</asp:Label><span class="inputs"><asp:TextBox ID="txtemailhome" runat="server" meta:resourcekey="txtemailhomeResource1" MaxLength="50" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RegularExpressionValidator ID="reghome" runat="server" ErrorMessage="Please enter a valid home email address in the box provided" Text="*" ControlToValidate="txtemailhome" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="vgMain"></asp:RegularExpressionValidator></span><asp:Label ID="lbltelno" runat="server" AssociatedControlID="txttelno">Telephone Number</asp:Label><span class="inputs"><asp:TextBox ID="txttelno" runat="server" MaxLength="50" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label ID="lblfaxno" runat="server" meta:resourcekey="lblfaxnoResource1" AssociatedControlID="txtfaxno">Fax Number</asp:Label><span class="inputs"><asp:TextBox ID="txtfaxno" MaxLength="50" runat="server" meta:resourcekey="txtfaxnoResource1" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><span class="inputs">&nbsp;</span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                          <%--  <div class="sectiontitle">
                                <asp:Label ID="lbldrivinglicencedetails" runat="server" Text="Driving Licence Details" meta:resourcekey="lbldrivinglicencedetailsResource1"></asp:Label>
                            </div>
                            <div class="twocolumn">
                                <asp:Label ID="lbllicencenumber" runat="server" Text="Licence Number" meta:resourcekey="lbllicencenumberResource1" AssociatedControlID="txtlicencenumber"></asp:Label><span class="inputs"><asp:TextBox ID="txtlicencenumber" runat="server" meta:resourcekey="txtlicencenumberResource1" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblexpirydate" runat="server" Text="Licence Expiry Date" meta:resourcekey="lblexpirydateResource1" AssociatedControlID="txtlicenceexpiry"></asp:Label><span class="inputs"><asp:TextBox ID="txtlicenceexpiry" runat="server" meta:resourcekey="txtlicenceexpiryResource1" CssClass="fillspan"></asp:TextBox><cc1:MaskedEditExtender ID="msklicenceexpiry" MaskType="Date" Mask="99/99/9999" TargetControlID="txtlicenceexpiry" CultureName="en-GB" runat="server" CultureAMPMPlaceholder="AM;PM" CultureCurrencySymbolPlaceholder="�" CultureDateFormat="DMY" CultureDatePlaceholder="/" CultureDecimalPlaceholder="." CultureThousandsPlaceholder="," CultureTimePlaceholder=":" Enabled="True"></cc1:MaskedEditExtender>
                                    <cc1:CalendarExtender ID="callicenceexpiry" runat="server" TargetControlID="txtlicenceexpiry" PopupButtonID="imglicenceexpiry" Format="dd/MM/yyyy" Enabled="True"></cc1:CalendarExtender>
                                </span><span class="inputicon">
                                    <asp:Image ID="imglicenceexpiry" ImageUrl="../images/icons/cal.gif" runat="server" meta:resourcekey="imglicenceexpiryResource1" /></span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator ID="complicenceexpiry" runat="server" ControlToValidate="txtlicenceexpiry" ErrorMessage="The licence expiry date you have entered is invalid" Operator="DataTypeCheck" Type="Date" meta:resourcekey="complicenceexpiryResource1" ValidationGroup="vgMain">*</asp:CompareValidator><asp:CompareValidator ID="cvLicenceExpiry" runat="server" ErrorMessage="The licence expiry date you have entered is invalid" ControlToValidate="txtlicenceexpiry" ValueToCompare="01/01/1901" Operator="GreaterThan" Type="Date" Display="Dynamic">*</asp:CompareValidator></span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label ID="lbllastchecked" runat="server" Text="Last Checked" meta:resourcekey="lbllastcheckedResource1" AssociatedControlID="txtlastchecked"></asp:Label><span class="inputs"><asp:TextBox ID="txtlastchecked" runat="server" meta:resourcekey="txtlastcheckedResource1" CssClass="fillspan"></asp:TextBox></span><span class="inputicon"><asp:Image ID="imglastchecked" ImageUrl="../images/icons/cal.gif" runat="server" meta:resourcekey="imglastcheckedResource1" /></span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><cc1:MaskedEditExtender ID="msklastchecked" MaskType="Date" Mask="99/99/9999" TargetControlID="txtlastchecked" CultureName="en-GB" runat="server" CultureAMPMPlaceholder="AM;PM" CultureCurrencySymbolPlaceholder="�" CultureDateFormat="DMY" CultureDatePlaceholder="/" CultureDecimalPlaceholder="." CultureThousandsPlaceholder="," CultureTimePlaceholder=":" Enabled="True"></cc1:MaskedEditExtender>
                                    <cc1:CalendarExtender ID="callastchecked" runat="server" TargetControlID="txtlastchecked" PopupButtonID="imglastchecked" Format="dd/MM/yyyy" Enabled="True"></cc1:CalendarExtender>
                                    <asp:CompareValidator ID="complicencelastchecked" runat="server" ControlToValidate="txtlastchecked" ErrorMessage="The Licence Last Checked date you have entered is invalid" Operator="DataTypeCheck" Type="Date" meta:resourcekey="complicencelastcheckedResource1" ValidationGroup="vgMain">*</asp:CompareValidator><asp:CompareValidator ID="cvlastchecked" runat="server" ErrorMessage="The licence last checked date you have entered is invalid" ControlToValidate="txtlastchecked" ValueToCompare="01/01/1901" Operator="GreaterThan" Type="Date" Display="Dynamic">*</asp:CompareValidator></span><asp:PlaceHolder ID="licenceCheckedBy" runat="server"></asp:PlaceHolder>
                                <span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label ID="lbldocument" runat="server" Text="Document" AssociatedControlID="holderlicence" meta:resourcekey="lbldocumentResource1"></asp:Label><span class="inputs"><div id="licenceDiv">
                                    <asp:PlaceHolder ID="holderlicence" runat="server"></asp:PlaceHolder>
                                </div>
                                </span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><span class="inputs">&nbsp;</span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>--%>

                            <div class="twocolumn">
                                <asp:Literal ID="litbankdetails" runat="server" meta:resourcekey="litbankdetailsResource1"></asp:Literal>
                                <maccount:maccount ID="usrBankAccounts" runat="server"></maccount:maccount>
                            </div>



                            <div class="sectiontitle">
                                <asp:Label ID="Label20" runat="server" Text="Personal Information"></asp:Label>
                            </div>
                            <div class="twocolumn">
                                <asp:Label ID="Label14" runat="server" AssociatedControlID="cmbgender">Gender</asp:Label><span class="inputs"><asp:DropDownList CssClass="fillspan" ID="cmbgender" runat="server">
                                    <asp:ListItem Value=""></asp:ListItem>
                                    <asp:ListItem>Female</asp:ListItem>
                                    <asp:ListItem>Male</asp:ListItem>
                                </asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="Label15" runat="server" AssociatedControlID="txtdateofbirth">Date of Birth</asp:Label><span class="inputs"><asp:TextBox ID="txtdateofbirth" runat="server" MaxLength="50" CssClass="fillspan"></asp:TextBox><cc1:MaskedEditExtender ID="mskdateofbirth" MaskType="Date" Mask="99/99/9999" TargetControlID="txtdateofbirth" CultureName="en-GB" runat="server"></cc1:MaskedEditExtender>
                                    <cc1:CalendarExtender ID="CalendarExtender2" runat="server" TargetControlID="txtdateofbirth" PopupButtonID="imgdateofbirth" Format="dd/MM/yyyy"></cc1:CalendarExtender>
                                </span><span class="inputicon">
                                    <asp:Image ID="imgdateofbirth" ImageUrl="../images/icons/cal.gif" runat="server" /></span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator ID="compdateofbirth" runat="server" ControlToValidate="txtdateofbirth" ErrorMessage="The date of birth you have entered is invalid" Operator="DataTypeCheck" Type="Date" ValidationGroup="vgMain">*</asp:CompareValidator><asp:CompareValidator ID="compdateofbirthgt" runat="server" ControlToValidate="txtdateofbirth" ErrorMessage="The date of birth must be after 01/01/1800" Operator="GreaterThan" Type="Date" ValidationGroup="vgMain" ValueToCompare="01/01/1800">*</asp:CompareValidator></span>
                            </div>
                        </div>
                    </ContentTemplate>
                </cc1:TabPanel>
                <cc1:TabPanel runat="server" HeaderText="Claims" ID="tabClaims">
                    <ContentTemplate>
                        <div class="formpanel">
                            <div class="sectiontitle">
                                <asp:Label ID="Label18" runat="server" Text="Claim Signoff"></asp:Label>
                            </div>
                            <div class="onecolumnsmall">
                                <asp:Label ID="lblsignoff" runat="server" meta:resourcekey="lblsignoffResource1" AssociatedControlID="cmbsignoffs">Signoff Group</asp:Label><span class="inputs"><asp:DropDownList ID="cmbsignoffs" runat="server" meta:resourcekey="cmbsignoffsResource1" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip65" onclick="SEL.Tooltip.Show('0bc5a045-c119-4ae3-bf6e-7c8bd4bfab7a', 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"><asp:CustomValidator ID="cvSignoffExpenses" runat="server" ControlToValidate="cmbsignoffs" ClientValidationFunction="ValidateSignoff" ErrorMessage="The selected Signoff Group cannot be used as the employee would approve one of their own stages" Text="*" ValidationGroup="vgMain"></asp:CustomValidator></span>
                            </div>
                            <div class="onecolumnsmall">
                                <asp:Label ID="lblsignoffcc" runat="server" meta:resourcekey="lblsignoffccResource1" AssociatedControlID="cmbsignoffcc">Signoff Group (Credit Card)</asp:Label><span class="inputs"><asp:DropDownList ID="cmbsignoffcc" runat="server" meta:resourcekey="cmbsignoffccResource1" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip351" onclick="SEL.Tooltip.Show('4ffa052c-cc76-430b-a7b9-b4804c5c0bdd', 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"><asp:CustomValidator ID="cvSignoffCreditCard" runat="server" ControlToValidate="cmbsignoffcc" ClientValidationFunction="ValidateSignoff" ErrorMessage="The selected Signoff Group (Credit Card) cannot be used as the employee would approve one of their own stages" Text="*" ValidationGroup="vgMain"></asp:CustomValidator></span>
                            </div>
                            <div class="onecolumnsmall">
                                <asp:Label ID="lblsignoffpc" runat="server" meta:resourcekey="lblsignoffpcResource1" AssociatedControlID="cmbsignoffpc">Signoff Group (Purchase Card)</asp:Label><span class="inputs"><asp:DropDownList ID="cmbsignoffpc" runat="server" meta:resourcekey="cmbsignoffpcResource1" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip352" onclick="SEL.Tooltip.Show('dac89fd2-b745-4ba8-a56c-4a5e26d228c9', 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"><asp:CustomValidator ID="cvSignoffPurchaseCard" runat="server" ControlToValidate="cmbsignoffpc" ClientValidationFunction="ValidateSignoff" ErrorMessage="The selected Signoff Group (Purchase Card) cannot be used as the employee would approve one of their own stages" Text="*" ValidationGroup="vgMain"></asp:CustomValidator></span>
                            </div>
                            <div class="onecolumnsmall">
                                <asp:Label ID="lbladvancessignoff" runat="server" meta:resourcekey="lbladvancessignoffResource1" AssociatedControlID="cmbadvancesgroup">Signoff Group (Advances)</asp:Label><span class="inputs"><asp:DropDownList ID="cmbadvancesgroup" runat="server" meta:resourcekey="cmbadvancesgroupResource1" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip66" onclick="SEL.Tooltip.Show('9df2585c-2267-4253-a16b-ab31ca1e698a', 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="sectiontitle">
                                <asp:Label ID="Label21" runat="server" Text="Item Roles" meta:resourcekey="lblitemrolesResource1"></asp:Label>
                            </div>
                            <a href="javascript:populateNewItemRoles();showNewItemRoleModal(true);">Add Item Role</a>
                            <asp:Panel runat="server" ID="pnlItemRoles">
                                <asp:Literal ID="lititemroles" runat="server"></asp:Literal>
                            </asp:Panel>
                        </div>
                    </ContentTemplate>

                </cc1:TabPanel>
                <cc1:TabPanel ID="tabEmailNotifications" runat="server" HeaderText="Notifications">
                    <ContentTemplate>

                        <div class="formpanel">

                            <asp:Panel ID="pnlEmailNotificationsSectionTitle" runat="server" CssClass="sectiontitle">Notifications</asp:Panel>
                            <asp:Panel ID="pnlEmailNotifications" runat="server"></asp:Panel>
                            <asp:Panel ID="pnlESREmailNotificatonsSectionTitle" runat="server" CssClass="sectiontitle">ESR Notifications</asp:Panel>
                            <asp:Panel ID="pnlESREmailNotifications" runat="server"></asp:Panel>
                        </div>
                    </ContentTemplate>

                </cc1:TabPanel>

                <cc1:TabPanel ID="tabAddAuthoriserLevel" Visible="false" runat="server" HeaderText="Authoriser Level">
                    <ContentTemplate>
                        <div class="formpanel">
                            <asp:Panel ID="pnlAuthoriserLevel" runat="server" CssClass="sectiontitle">Authoriser Level</asp:Panel>
                            <asp:Panel runat="server">
                                <div class="twocolumn">
                                    <label for="ctl00_contentmain_tabsGeneral_tabPersonal_txtemailhome" id="lblDefaultAuthoriser">Authoriser level</label>
                                    <span class="inputs">
                                        <asp:DropDownList ID="cmbAuthoriserLevel" runat="server" CssClass="fillspan"></asp:DropDownList>
                                    </span>
                                    <span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span>
                                </div>

                                <div class="twocolumn_label">
                                    <label runat="server" id="lblUnAssingDefaultApprover" visible="false"></label>
                                    <asp:CheckBox runat="server" visible="false" ID="chkUnAssingDefaultApprover"/>
                                </div>
                            </asp:Panel>
                        </div>
                    </ContentTemplate>

                </cc1:TabPanel>
            </cc1:TabContainer>
        </div>

        <asp:Panel ID="pnlAuthoriser" runat="server" class="modalpanel" Style="display: none;">
        </asp:Panel>
        <div id="pgCars" class="subPage" style="display: none;">
            <div class="formpanel formpanel_padding">
                <div class="sectiontitle">
                    <asp:Label ID="lblVehicles" runat="server" meta:resourcekey="Label6Resource1">Vehicles</asp:Label>
                </div>
                <asp:Panel runat="server" ID="pnlCars">
                    <asp:Literal ID="litcargrid" runat="server" meta:resourcekey="litcargridResource1"></asp:Literal>
                </asp:Panel>
            </div>
        </div>
        <div id="pgPoolCars" class="subPage" style="display: none;">
            <div class="formpanel formpanel_padding">
                <div class="sectiontitle">
                    <asp:Label ID="lblPoolVehicles" runat="server" meta:resourcekey="Label6Resource1">Pool Vehicles</asp:Label>
                </div>
                <asp:Panel ID="pnlPoolCars" runat="server">
                    <asp:Literal ID="litpoolcargrid" runat="server" meta:resourcekey="litcargridResource1"></asp:Literal>
                </asp:Panel>
            </div>
        </div>
        <div id="pgCorporateCards" class="subPage" style="display: none;">
            <div class="formpanel formpanel_padding">
                <div class="sectiontitle">
                    <asp:Label ID="lblcorporatecards" runat="server" Text="Corporate Cards" meta:resourcekey="lblcorporatecardsResource1"></asp:Label>
                </div>
                <asp:Panel runat="server" ID="pnlCorporateCards">
                    <asp:Literal ID="litcards" runat="server" meta:resourcekey="litcardsResource1"></asp:Literal>
                </asp:Panel>
            </div>
        </div>
        <div id="pgWorkLocations" class="subPage" style="display: none;">
            <div class="formpanel formpanel_padding">
                <div class="sectiontitle">
                    <asp:Label ID="Label9" runat="server" Text="Work Addresses"></asp:Label>
                </div>
                <asp:Panel ID="pnlWorkLocations" runat="server">
                    <asp:Literal ID="litworklocations" runat="server"></asp:Literal>
                </asp:Panel>
            </div>
        </div>
        <div id="pgHomeLocations" class="subPage" style="display: none;">
            <div class="formpanel formpanel_padding">
                <div class="sectiontitle">
                    <asp:Label ID="Label27" runat="server" Text="Home Addresses"></asp:Label>
                </div>
                <asp:Panel ID="pnlHomeLocations" runat="server">
                    <asp:Literal ID="lithomelocations" runat="server"></asp:Literal>
                </asp:Panel>
            </div>
        </div>
        <div id="pgMobileDevices" class="subPage" style="display: none;">
            <div class="formpanel formpanel_padding">
                <div class="sectiontitle">Mobile Devices</div>
                <mdev:mdev ID="usrMobileDevices" runat="server"></mdev:mdev>
            </div>

        </div>
        <div id="divTitle" class="subPage" style="display: none;">
            <div class="formpanel formpanel_padding" >
                <div class="sectiontitle">Add Exception</div>
                <mdev:mdev ID="mdevAdd" runat="server"></mdev:mdev>
            </div>

        </div>
    </div>
    <div class="formpanel formpanel_left">
        <div class="formbuttons">
            <asp:Label ID="spanSaveButton" runat="server"><a href="javascript:currentAction = 'OK';saveEmployee(true, true)">
                <img alt="Save" src="../images/buttons/btn_save.png" /></a>
                <asp:Image ID="cmdactivate" runat="server" Visible="False" ImageUrl="~/shared/images/buttons/pagebutton_activate.gif" onclick="ActivateEmployee();" CssClass="btn" /> </asp:Label>
            <a href="selectemployee.aspx">
                <asp:Image ID="imgCancelEmployee" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" AlternateText="Cancel" /></a>
        </div>
    </div>
    <div class="valdiv" id="valdiv">
        <asp:Label ID="lblmsg" runat="server" Visible="False" Font-Size="Small" ForeColor="Red"
            meta:resourcekey="lblmsgResource1">Label</asp:Label>
    </div>

    <asp:Panel ID="txtLineManagerSearchPanel" runat="server" CssClass="modalpanel formpanel" Style="display: none;">
        <div class="sectiontitle">Line Manager Search</div>
        <div class="searchgrid"></div>
        <div class="formbuttons">
            <helpers:CSSButton runat="server" ID="txtLineManagerSearchCancel" Text="cancel" />
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender runat="server" ID="txtLineManagerSearchModal" BackgroundCssClass="modalBackground" TargetControlID="txtLineManagerSearchLink" PopupControlID="txtLineManagerSearchPanel" CancelControlID="txtLineManagerSearchCancel"></cc1:ModalPopupExtender>
    <asp:LinkButton runat="server" ID="txtLineManagerSearchLink" Style="display: none;"></asp:LinkButton>

    <asp:Panel ID="pnlCorporateCard" runat="server" CssClass="modalpanel" Style="display: none;">
        <div class="formpanel">
            <div class="sectiontitle">
                <asp:Label ID="lblgeneraldetails" runat="server" Text="General Details" meta:resourcekey="lblgeneraldetailsResource1"></asp:Label>
            </div>
            <div class="onecolumnsmall">
                <asp:Label ID="lblcardprovider" runat="server" Text="Card Provider" AssociatedControlID="cmbcardprovider" meta:resourcekey="lblcardproviderResource1"></asp:Label><span class="inputs"><asp:DropDownList ID="cmbcardprovider" runat="server" meta:resourcekey="cmbcardproviderResource1" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
            </div>
            <div class="twocolumn">
                <asp:Label ID="lblcardnumber" runat="server" Text="Card Number" AssociatedControlID="txtcardnumber" meta:resourcekey="lblcardnumberResource1"></asp:Label><span class="inputs"><asp:TextBox ID="txtcardnumber" runat="server" meta:resourcekey="txtcardnumberResource1" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqcardnumber" Text="*" ControlToValidate="txtcardnumber" ValidationGroup="vgCorporateCard" runat="server" ErrorMessage="Please enter a card number in the box provided"></asp:RequiredFieldValidator></span><asp:Label ID="lblactive" runat="server" Text="Active" AssociatedControlID="chkCardActive" meta:resourcekey="lblactiveResource1"></asp:Label><span class="inputs"><asp:CheckBox ID="chkCardActive" runat="server" meta:resourcekey="chkCardActiveResource1" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
            </div>
            <div class="formbuttons">
                <a href="javascript:saveCorporateCard();">
                    <img src="/shared/images/buttons/btn_save.png" alt="OK" /></a>&nbsp;&nbsp;<asp:ImageButton ID="cmdcorporatecardcancel" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" CausesValidation="false" />
            </div>
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender ID="modcorporatecard" runat="server" TargetControlID="lnkcorporatecard"
        PopupControlID="pnlCorporateCard" BackgroundCssClass="modalBackground" CancelControlID="cmdcorporatecardcancel">
    </cc1:ModalPopupExtender>
    <asp:LinkButton ID="lnkcorporatecard" runat="server" Style="display: none;">LinkButton</asp:LinkButton>

    <asp:Panel ID="pnlWorkLocation" runat="server" CssClass="modalpanel" Style="display: none;">
        <div class="formpanel">
            <div class="sectiontitle">
                <asp:Label ID="Label22" runat="server" Text="General Details"></asp:Label>
            </div>
            <div class="twocolumn">
                <asp:Label ID="Label23" runat="server" Text="Address*" AssociatedControlID="txtworklocation" CssClass="mandatory"></asp:Label>
                <span class="inputs">
                    <asp:TextBox ID="txtworklocation" runat="server" MaxLength="50" meta:resourcekey="txthomeResource1" CssClass="fillspan ui-sel-address-picker" rel="hdnWorkLocationID"></asp:TextBox>
                    <asp:HiddenField ID="hdnWorkLocationID" runat="server" />
                </span>

                <span class="inputicon">
                    <asp:Image ID="imgWorkLocationSearch" runat="server" ImageUrl="~/shared/images/icons/16/Plain/find.png" />
                </span>

                <span class="inputtooltipfield">&nbsp;</span>
                <span class="inputvalidatorfield">
                    <helpers:RequiredHiddenFieldValidator runat="server" ID="reqworklocation" ControlToValidate="hdnWorkLocationId" ValidationGroup="vgWorkLocation" Display="Dynamic" Text="*" ErrorMessage="Please enter a work location address"></helpers:RequiredHiddenFieldValidator>
                </span>
            </div>
            <div class="twocolumn">
                <asp:Label ID="lblworklocationstart" runat="server" meta:resourcekey="lblstartResource1" AssociatedControlID="dtworklocationstart" class="mandatory">Start Date*</asp:Label><span class="inputs"><asp:TextBox ID="dtworklocationstart" runat="server" meta:resourcekey="dtstartResource1" ValidationGroup="vgWorkLocation" CssClass="fillspan"></asp:TextBox><cc1:MaskedEditExtender ID="mskworklocationstart" MaskType="Date" Mask="99/99/9999" TargetControlID="dtworklocationstart" CultureName="en-GB" runat="server" CultureAMPMPlaceholder="AM;PM" CultureCurrencySymbolPlaceholder="�" CultureDateFormat="DMY" CultureDatePlaceholder="/" CultureDecimalPlaceholder="." CultureThousandsPlaceholder="," CultureTimePlaceholder=":" Enabled="True"></cc1:MaskedEditExtender>
                    <cc1:CalendarExtender ID="calworklocationstart" runat="server" TargetControlID="dtworklocationstart" PopupButtonID="imgworklocationstart" Format="dd/MM/yyyy" Enabled="True"></cc1:CalendarExtender>
                </span><span class="inputicon">
                    <asp:Image ID="imgworklocationstart" ImageUrl="~/shared/images/icons/cal.gif" runat="server" meta:resourcekey="imgstartResource1" /></span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator ID="compworklocationstart" runat="server" ControlToValidate="dtworklocationstart" ErrorMessage="Please enter a valid Start Date" Type="Date" Operator="DataTypeCheck" meta:resourcekey="compstartResource1" ValidationGroup="vgworklocation" Display="Dynamic">*</asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpworklocationstartmax" Operator="LessThanEqual" ControlToValidate="dtworklocationstart" Type="Date" Text="*" ErrorMessage="Invalid work address start date provided. Date must be on or before 31/12/3000" ValidationGroup="vgWorkLocation" ValueToCompare="31/12/3000" Display="Dynamic"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpworklocationstartmin" Operator="GreaterThanEqual" ControlToValidate="dtworklocationstart" Type="Date" Text="*" ErrorMessage="Invalid work address start date provided. Date must be on or after 01/01/1900" ValidationGroup="vgWorkLocation" ValueToCompare="01/01/1900" Display="Dynamic"></asp:CompareValidator><asp:RequiredFieldValidator runat="server" ID="reqworklocationstart" Text="*" ErrorMessage="Please enter a valid start date" Display="Dynamic" ValidationGroup="vgWorkLocation" ControlToValidate="dtworklocationstart"></asp:RequiredFieldValidator></span><asp:Label ID="lblworklocationend" runat="server" meta:resourcekey="lblendResource1" AssociatedControlID="dtworklocationend">End Date</asp:Label><span class="inputs"><asp:TextBox ID="dtworklocationend" runat="server" meta:resourcekey="dtendResource1" ValidationGroup="vgWorkLocation"></asp:TextBox><cc1:MaskedEditExtender ID="mskworklocationend" MaskType="Date" Mask="99/99/9999" TargetControlID="dtworklocationend" CultureName="en-GB" runat="server" CultureAMPMPlaceholder="AM;PM" CultureCurrencySymbolPlaceholder="�" CultureDateFormat="DMY" CultureDatePlaceholder="/" CultureDecimalPlaceholder="." CultureThousandsPlaceholder="," CultureTimePlaceholder=":" Enabled="True"></cc1:MaskedEditExtender>
                        <cc1:CalendarExtender ID="calworklocationend" runat="server" TargetControlID="dtworklocationend" PopupButtonID="imgworklocationend" Format="dd/MM/yyyy" Enabled="True"></cc1:CalendarExtender>
                    </span><span class="inputicon">
                        <asp:Image ID="imgworklocationend" ImageUrl="~/shared/images/icons/cal.gif" runat="server" meta:resourcekey="imgendResource1" /></span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator ID="compworklocationend" runat="server" ControlToValidate="dtworklocationend" ErrorMessage="Please enter a valid end date" Type="Date" Operator="DataTypeCheck" meta:resourcekey="compendResource1" ValidationGroup="vgWorkLocation" Display="Dynamic">*</asp:CompareValidator><asp:CompareValidator ID="compworkendResource2" runat="server" ControlToValidate="dtworklocationend" ControlToCompare="dtworklocationstart" Operator="GreaterThan" ErrorMessage="Work location end date must be after start date" Text="*" Type="Date" ValidationGroup="vgWorkLocation" Display="Dynamic"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpworklocationendmax" ControlToValidate="dtworklocationend" Operator="LessThanEqual" Type="Date" Text="*" ErrorMessage="Invalid work address end date provided. Date must be on or before 31/12/3000" ValidationGroup="vgWorkLocation" ValueToCompare="31/12/3000" Display="Dynamic"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpworklocationendmin" ControlToValidate="dtworklocationend" Operator="GreaterThanEqual" Type="Date" Text="*" ErrorMessage="Invalid work address end date provided. Date must on or after 01/01/1900" ValidationGroup="vgWorkLocation" ValueToCompare="01/01/1900" Display="Dynamic"></asp:CompareValidator></span>
            </div>
            <div class="twocolumn">
                <asp:Label ID="Label25" runat="server" AssociatedControlID="chkworklocationtemporary" Text="Temporary Address"></asp:Label><span class="inputs"><asp:CheckBox ID="chkworklocationtemporary" runat="server" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                <asp:Label runat="server" AssociatedControlID="chkPrimaryRotationalAddress" Text="Nominated base"></asp:Label><span class="inputs"><asp:CheckBox ID="chkPrimaryRotationalAddress" runat="server" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
            </div>
            <div class="formbuttons">
                <a href="javascript:saveWorkLocation();">
                    <img src="/shared/images/buttons/btn_save.png" alt="OK" /></a>&nbsp;&nbsp;<asp:ImageButton ID="cmdworklocationcancel" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" CausesValidation="false" />
            </div>
        </div>
    </asp:Panel>

    <cc1:ModalPopupExtender ID="modworklocation" runat="server" TargetControlID="lnkworklocation"
        PopupControlID="pnlWorkLocation" BackgroundCssClass="modalBackground" CancelControlID="cmdworklocationcancel">
    </cc1:ModalPopupExtender>

    <asp:LinkButton ID="lnkworklocation" runat="server" Style="display: none;">LinkButton</asp:LinkButton>
    <asp:Panel ID="pnlHomeLocation" runat="server" CssClass="modalpanel" Style="display: none;">
        <div class="formpanel">
            <div class="sectiontitle">
                <asp:Label ID="Label28" runat="server" Text="General Details"></asp:Label>
            </div>
            <div class="onecolumn esrEditWarning">
                <div class="comment"><span class="commentBody">This address was imported from ESR and cannot be edited.</span></div>
            </div>
            <div class="twocolumn">
                <asp:Label ID="Label29" runat="server" Text="Address*" AssociatedControlID="txthomelocation" CssClass="mandatory"></asp:Label>
                <span class="inputs">
                    <asp:TextBox ID="txthomelocation" runat="server" MaxLength="50" meta:resourcekey="txthomeResource1" CssClass="fillspan ui-sel-address-picker" rel="hdnHomeLocationID"></asp:TextBox>
                    <asp:HiddenField ID="hdnHomeLocationID" runat="server" />
                </span>

                <span class="inputicon">
                    <asp:Image ID="imgHomeLocationSearch" runat="server" ImageUrl="~/shared/images/icons/16/plain/find.png" />
                </span>

                <span class="inputtooltipfield">&nbsp;</span>
                <span class="inputvalidatorfield">
                    <asp:RequiredFieldValidator runat="server" ID="reqhomelocationID" ControlToValidate="txthomelocation" Text="*" ErrorMessage="Please enter an address to save" ValidationGroup="vgHomeLocation" Display="Dynamic"></asp:RequiredFieldValidator>
                </span>
            </div>
            <div class="twocolumn">
                <asp:Label ID="Label31" runat="server" meta:resourcekey="lblstartResource1" AssociatedControlID="dthomelocationstart" CssClass="mandatory">Start Date*</asp:Label><span class="inputs"><asp:TextBox ID="dthomelocationstart" runat="server" meta:resourcekey="dtstartResource1" ValidationGroup="vgHomeLocation" CssClass="fillspan"></asp:TextBox><cc1:CalendarExtender ID="CalendarExtender3" runat="server" TargetControlID="dthomelocationstart" PopupButtonID="imghomelocationstart" Format="dd/MM/yyyy" Enabled="True"></cc1:CalendarExtender>
                </span><span class="inputicon">
                    <asp:Image ID="imghomelocationstart" ImageUrl="~/shared/images/icons/cal.gif" runat="server" meta:resourcekey="imgstartResource1" /></span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqStartDate" runat="server" ErrorMessage="Please enter a start date in the box provided" ControlToValidate="dthomelocationstart" Text="*" ValidationGroup="vgHomeLocation" Display="Dynamic"></asp:RequiredFieldValidator><asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="dthomelocationstart" ErrorMessage="Please enter a valid Start Date" Type="Date" Operator="DataTypeCheck" meta:resourcekey="compstartResource1" ValidationGroup="vgHomeLocation" Text="*" Display="Dynamic"></asp:CompareValidator><cc1:MaskedEditExtender ID="MaskedEditExtender4" MaskType="Date" Mask="99/99/9999" TargetControlID="dthomelocationstart" CultureName="en-GB" runat="server"></cc1:MaskedEditExtender>
                        <asp:CompareValidator runat="server" ID="cmphomelocationstartmin" ControlToValidate="dthomelocationstart" Operator="GreaterThanEqual" Type="Date" Text="*" ErrorMessage="Invalid home address start date provided. Date must be on or after 01/01/1900" ValidationGroup="vgHomeLocation" ValueToCompare="01/01/1900" Display="Dynamic"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmphomelocationstartmax" ControlToValidate="dthomelocationstart" Operator="LessThanEqual" Type="Date" Text="*" ErrorMessage="Invalid home address start date provided. Date must be on or before 31/12/3000" ValidationGroup="vgHomeLocation" ValueToCompare="31/12/3000" Display="Dynamic"></asp:CompareValidator></span><asp:Label ID="Label32" runat="server" meta:resourcekey="lblendResource1" AssociatedControlID="dthomelocationend">End Date</asp:Label><span class="inputs"><asp:TextBox ID="dthomelocationend" runat="server" meta:resourcekey="dtendResource1" ValidationGroup="vgHomeLocation"></asp:TextBox><cc1:CalendarExtender ID="CalendarExtender4" runat="server" TargetControlID="dthomelocationend" PopupButtonID="imghomelocationend" Format="dd/MM/yyyy" Enabled="True"></cc1:CalendarExtender>
                        </span><span class="inputicon">
                            <asp:Image ID="imghomelocationend" ImageUrl="~/shared/images/icons/cal.gif" runat="server" meta:resourcekey="imgendResource1" /></span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator ID="CompareValidator2" runat="server" ControlToValidate="dthomelocationend" ErrorMessage="Please enter a valid end date" Type="Date" Operator="DataTypeCheck" meta:resourcekey="compendResource1" ValidationGroup="vgHomeLocation" Text="*" Display="Dynamic"></asp:CompareValidator><cc1:MaskedEditExtender ID="MaskedEditExtender5" MaskType="Date" Mask="99/99/9999" TargetControlID="dthomelocationend" CultureName="en-GB" runat="server"></cc1:MaskedEditExtender>
                                <asp:CompareValidator ID="CompareValidator3" runat="server" ControlToValidate="dthomelocationend" ControlToCompare="dthomelocationstart" Operator="GreaterThan" ErrorMessage="Home location end date must be after start date" Text="*" Type="Date" ValidationGroup="vgHomeLocation" Display="Dynamic"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmphomelocationendmin" ControlToValidate="dthomelocationend" Operator="GreaterThanEqual" Type="Date" Text="*" ErrorMessage="Invalid home address end date provided. Date must be on or after 01/01/1900" ValidationGroup="vgHomeLocation" ValueToCompare="01/01/1900" Display="Dynamic"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmphomelocationendmax" ControlToValidate="dthomelocationend" Operator="LessThanEqual" Type="Date" Text="*" ErrorMessage="Invalid home address end date provided. Date must be on or after 31/12/3000" ValidationGroup="vgHomeLocation" ValueToCompare="31/12/3000" Display="Dynamic"></asp:CompareValidator></span>
            </div>
            <div class="formbuttons">
                <a href="javascript:saveHomeLocation();">
                    <img src="/shared/images/buttons/btn_save.png" alt="OK" /></a>&nbsp;&nbsp;<asp:ImageButton ID="cmdhomelocationcancel" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" CausesValidation="false" />
            </div>
            
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender ID="modhomelocation" runat="server" TargetControlID="lnkhomelocation"
        PopupControlID="pnlHomeLocation" BackgroundCssClass="modalBackground" CancelControlID="cmdhomelocationcancel">
    </cc1:ModalPopupExtender>
    <asp:LinkButton ID="lnkhomelocation" runat="server" Style="display: none;">LinkButton</asp:LinkButton>
    <asp:Panel runat="server" ID="pnlCar" CssClass="modalpanel" Style="display: none; height: 470px; width: 900px; padding: 4px; position: fixed">
        <uc2:aeCars ID="aeCar" runat="server" />
    </asp:Panel>
    <cc1:ModalPopupExtender ID="modcar" runat="server" TargetControlID="lnkcar" PopupControlID="pnlCar" BackgroundCssClass="modalBackground">
    </cc1:ModalPopupExtender>
    <asp:LinkButton ID="lnkcar" runat="server" Style="display: none;">LinkButton</asp:LinkButton>

    <asp:Panel runat="server" ID="pnlPoolCar" CssClass="modalpanel" Style="display: none;">
        <div class="formpanel">
            <div class="sectiontitle">
                <asp:Label ID="Label26" runat="server" Text="General Details"></asp:Label>
            </div>
            <div class="onecolumnsmall">
                <asp:Label ID="lblcars" runat="server" CssClass="labeltd" meta:resourcekey="lblcarsResource1" Text="Pool Vehicle" AssociatedControlID="txtPoolCarSelect"></asp:Label><span class="inputs"><asp:DropDownList ID="txtPoolCarSelect" runat="server" onchange="poolCarCombo.SelectChange();"></asp:DropDownList><asp:TextBox runat="server" ID="txtPoolCar"></asp:TextBox><asp:TextBox runat="server" ID="txtPoolCar_ID" Style="display: none;"></asp:TextBox></span><span class="inputicon"><asp:Image runat="server" ID="txtPoolCarSearchIcon" ImageUrl="/static/icons/16/new-icons/find.png" onclick="poolCarCombo.Search()" /></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            </div>
            <div class="formbuttons">
                <a href="javascript:savePoolCar();">
                    <img src="/shared/images/buttons/btn_save.png" alt="OK" /></a>&nbsp;&nbsp;<asp:ImageButton ID="cmdpoolcarcancel" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" CausesValidation="false" />
            </div>
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender ID="modpoolcar" runat="server" TargetControlID="lnkpoolcar" PopupControlID="pnlPoolCar" BackgroundCssClass="modalBackground" CancelControlID="cmdpoolcarcancel"></cc1:ModalPopupExtender>
    <asp:LinkButton ID="lnkpoolcar" runat="server" Style="display: none;">LinkButton</asp:LinkButton>



    <asp:Panel ID="txtPoolCarSearchPanel" runat="server" CssClass="modalpanel formpanel" Style="display: none;">
        <div class="sectiontitle">Pool Vehicle Search</div>
        <div class="searchgrid"></div>
        <div class="formbuttons">
            <helpers:CSSButton runat="server" ID="txtPoolCarSearchCancel" Text="cancel" />
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender runat="server" ID="txtPoolCarSearchModal" BackgroundCssClass="modalBackground" TargetControlID="txtPoolCarSearchLink" PopupControlID="txtPoolCarSearchPanel" CancelControlID="txtPoolCarSearchCancel"></cc1:ModalPopupExtender>
    <asp:LinkButton runat="server" ID="txtPoolCarSearchLink" Style="display: none;"></asp:LinkButton>


    <asp:Panel runat="server" ID="pnlNewAccessRoles" CssClass="modalpanel" Style="display: none; height: 90%; overflow: auto; padding: 4px;">
        <div class="formpanel">
            <div class="sectiontitle">Add Access Roles</div>
            <div class="twocolumn">
                <asp:Label runat="server" ID="lblSubAccount" CssClass="mandatory" AssociatedControlID="cmbSubAccounts">Sub-Account *</asp:Label><span class="inputs"><asp:DropDownList runat="server" ID="cmbSubAccounts" CssClass="fillspan" ValidationGroup="vgAccessRoles"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator runat="server" ID="cmpSubAccounts" ControlToValidate="cmbSubAccounts" Text="*" ErrorMessage="A Sub-Account to apply access roles for must be selected" Operator="GreaterThan" ValueToCompare="-1" Type="Integer" ValidationGroup="vgAccessRoles"></asp:CompareValidator></span>
            </div>
            <asp:Panel ID="pnlAccessRoleList" runat="server">
                <asp:Literal ID="litNewAccessRoles" runat="server"></asp:Literal>
            </asp:Panel>
            <div class="formbuttons">
                <a href="javascript:saveAccessRoles()">
                    <img alt="Save" src="/shared/images/buttons/btn_save.png" /></a>&nbsp;&nbsp;<asp:ImageButton ID="cmdaccessrolescancel" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" />
            </div>
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender ID="modNewAccessRoles" runat="server" TargetControlID="lnkNewAccessRoles"
        PopupControlID="pnlNewAccessRoles" BackgroundCssClass="modalBackground" CancelControlID="cmdaccessrolescancel">
    </cc1:ModalPopupExtender>
    <asp:LinkButton ID="lnkNewAccessRoles" runat="server" Style="display: none;">LinkButton</asp:LinkButton>
    <asp:Panel runat="server" ID="pnlNewItemRoles" CssClass="modalpanel" Style="display: none; height: 90%; overflow: auto; padding: 4px;">
        <div class="formpanel">
            <div class="sectiontitle">Add Item Roles</div>
            <asp:Panel ID="pnlItemRoleList" runat="server">
                <asp:Literal ID="litNewItemRoles" runat="server"></asp:Literal>
            </asp:Panel>
            <div class="formbuttons">
                <a href="javascript:saveItemRoles()">
                    <img alt="Save" src="../../shared/images/buttons/btn_save.png" /></a>&nbsp;&nbsp;<asp:ImageButton ID="cmditemrolecancel" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" />
            </div>
        </div>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlEditItemRole" CssClass="modalpanel" Style="display: none; padding: 4px;">
        <div class="formpanel">
            <div class="sectiontitle">Edit Employee Item Role</div>
            <div class="twocolumn">
                <asp:Label ID="lblItemRole" runat="server" Text="Item Role" AssociatedControlID="lblItemRoleValue" ></asp:Label><span class="inputs"><asp:Label ID="lblItemRoleValue"  CssClass="fillspan" runat="server"></asp:Label></span><span class="inputicon">&nbsp;</span><span class="inputvalidatorfield">&nbsp</span><span class="inputtooltipfield">&nbsp</span>
            </div>
            <div class="twocolumn">
                <asp:Label ID="lblItemRoleStartDate" runat="server" Text="Start date" AssociatedControlID="txtItemRoleStartDate" ></asp:Label><span class="inputs"><asp:TextBox ID="txtItemRoleStartDate" runat="server" CssClass="fillspan hasCalControl dateField"></asp:TextBox></span><span class="inputicon"><asp:Image runat="server" ID="imgItemRoleStartDate" ImageUrl="~/shared/images/icons/cal.gif" CssClass="dateCalImg" /></span><span class="inputvalidatorfield"></span><span class="inputtooltipfield">&nbsp</span> 
            </div>
            <div class="twocolumn">
                <asp:Label ID="lblItemRoleEndDate" runat="server" Text="End date" AssociatedControlID="txtItemRoleEndDate" ></asp:Label><span class="inputs"><asp:TextBox ID="txtItemRoleEndDate" runat="server" CssClass="fillspan hasCalControl dateField"></asp:TextBox></span><span class="inputicon"><asp:Image runat="server" ID="imgItemRoleEndDate" ImageUrl="~/shared/images/icons/cal.gif" CssClass="dateCalImg"/></span><span class="inputvalidatorfield"></span><span class="inputtooltipfield">&nbsp</span>
            </div>
            <div class="formbuttons">
                <a href="javascript:updateItemRole()">
                    <img alt="Save" src="../../shared/images/buttons/btn_save.png" />
                </a>&nbsp;&nbsp;
                <a href="javascript:hideItemRole()">
                    <img alt="Cancel" src="../../shared/images/buttons/cancel_up.gif" />
                </a>
            </div>
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender ID="modNewItemRoles" runat="server" TargetControlID="lnkNewItemRoles" PopupControlID="pnlNewItemRoles" BackgroundCssClass="modalBackground" CancelControlID="cmditemrolecancel"></cc1:ModalPopupExtender>
    <cc1:ModalPopupExtender ID="modEditItemRole" runat="server" TargetControlID="lnkEditItemRole" PopupControlID="pnlEditItemRole" BackgroundCssClass="modalBackground" CancelControlID="cmditemrolecancel"></cc1:ModalPopupExtender>
    
    <asp:LinkButton ID="lnkNewItemRoles" runat="server" Style="display: none;">LinkButton</asp:LinkButton>
    <asp:LinkButton ID="lnkEditItemRole" runat="server" Style="display: none;">LinkButton</asp:LinkButton>

    <asp:Panel ID="pnlESRAssignmentNumber" runat="server" CssClass="modalpanel" Style="display: none;">
        <div class="formpanel">
            <div class="sectiontitle">
                <asp:Label ID="Label36" runat="server" Text="General Details"></asp:Label>
            </div>
            <div class="twocolumn">
                <asp:Label ID="Label37" runat="server" Text="Assignment Number*" CssClass="mandatory" AssociatedControlID="txtassignmentnumber"></asp:Label><span class="inputs"><asp:TextBox ID="txtassignmentnumber" runat="server" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqESRAssignment" runat="server" ErrorMessage="Please enter an assignment number in the box provided" ControlToValidate="txtassignmentnumber" Text="*" ValidationGroup="vgESR"></asp:RequiredFieldValidator></span><span class="inputs">&nbsp;</span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
            </div>
            <div class="twocolumn">
                <asp:Label ID="lblesractive" runat="server" Text="Active" AssociatedControlID="chkesractive"></asp:Label><span class="inputs"><asp:CheckBox ID="chkesractive" runat="server" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="Label40" runat="server" Text="Primary Assignment" AssociatedControlID="chkesrprimary"></asp:Label><span class="inputs"><asp:CheckBox ID="chkesrprimary" runat="server" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
            </div>
            <div class="twocolumn">
                <asp:Label ID="Label38" runat="server" Text="Earliest Assignment Start Date*" CssClass="mandatory" AssociatedControlID="txtesrstartdate"></asp:Label><span class="inputs"><asp:TextBox ID="txtesrstartdate" runat="server" CssClass="fillspan"></asp:TextBox><cc1:MaskedEditExtender ID="MaskedEditExtender2" MaskType="Date" Mask="99/99/9999" TargetControlID="txtesrstartdate" CultureName="en-GB" runat="server" CultureAMPMPlaceholder="AM;PM" CultureCurrencySymbolPlaceholder="�" CultureDateFormat="DMY" CultureDatePlaceholder="/" CultureDecimalPlaceholder="." CultureThousandsPlaceholder="," CultureTimePlaceholder=":" Enabled="True"></cc1:MaskedEditExtender>
                    <cc1:CalendarExtender ID="CalendarExtender5" runat="server" TargetControlID="txtesrstartdate" PopupButtonID="imgesrstartdate" Format="dd/MM/yyyy" Enabled="True"></cc1:CalendarExtender>
                </span><span class="inputicon">
                    <asp:Image ID="imgesrstartdate" ImageUrl="../images/icons/cal.gif" runat="server" meta:resourcekey="imglastcheckedResource1" /></span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtesrstartdate" ValidationGroup="vgESR" ErrorMessage="Please enter a value for the Earliest Assignment Start Date" Text="*"></asp:RequiredFieldValidator><asp:CompareValidator runat="server" ID="cmpesrstartdatemax" ControlToValidate="txtesrstartdate" ValidationGroup="vgESR" Operator="LessThanEqual" Type="Date" Text="*" ValueToCompare="31/12/3000" ErrorMessage="Start Date must be earlier than 31/12/3000"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpesrstartdate" Operator="DataTypeCheck" Type="Date" ControlToValidate="txtesrstartdate" Text="*" ErrorMessage="Invalid start date provided" ValidationGroup="vgESR"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpesrstartdatemin" ControlToValidate="txtesrstartdate" Operator="GreaterThanEqual" Type="Date" ValueToCompare="01/01/1900" Text="*" ErrorMessage="Start date must be on or later than 01/01/1900" ValidationGroup="vgESR"></asp:CompareValidator></span><asp:Label ID="Label39" runat="server" Text="Final Assignment End Date" AssociatedControlID="txtesrenddate"></asp:Label><span class="inputs"><asp:TextBox ID="txtesrenddate" runat="server" CssClass="fillspan"></asp:TextBox><cc1:MaskedEditExtender ID="MaskedEditExtender3" MaskType="Date" Mask="99/99/9999" TargetControlID="txtesrenddate" CultureName="en-GB" runat="server" CultureAMPMPlaceholder="AM;PM" CultureCurrencySymbolPlaceholder="�" CultureDateFormat="DMY" CultureDatePlaceholder="/" CultureDecimalPlaceholder="." CultureThousandsPlaceholder="," CultureTimePlaceholder=":" Enabled="True"></cc1:MaskedEditExtender>
                        <cc1:CalendarExtender ID="CalendarExtender6" runat="server" TargetControlID="txtesrenddate" PopupButtonID="imgesrenddate" Format="dd/MM/yyyy" Enabled="True"></cc1:CalendarExtender>
                    </span><span class="inputicon">
                        <asp:Image ID="imgesrenddate" ImageUrl="../images/icons/cal.gif" runat="server" meta:resourcekey="imglastcheckedResource1" /></span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator runat="server" ID="cmpesrenddate" Operator="DataTypeCheck" Type="Date" ControlToValidate="txtesrenddate" ValidationGroup="vgESR" Text="*" ErrorMessage="Invalid end date provided"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpesrstartenddate" ControlToValidate="txtesrenddate" ControlToCompare="txtesrstartdate" Operator="GreaterThanEqual" Type="Date" Text="*" ErrorMessage="ESR Start Date must preceed the ESR End Date" EnableTheming="True" ValidationGroup="vgESR"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpesrenddatemin" ControlToValidate="txtesrenddate" Operator="GreaterThanEqual" Type="Date" ValueToCompare="01/01/1900" Text="*" ErrorMessage="End date must be on or later than 01/01/1900" ValidationGroup="vgESR"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpesrenddatemax" ControlToValidate="txtesrenddate" Operator="LessThanEqual" Type="Date" ValidationGroup="vgESR" Text="*" ValueToCompare="31/12/3000" ErrorMessage="End Date must be earlier than 31/12/3000"></asp:CompareValidator></span>
            </div>
            <div class="twocolumn">
                <asp:Label runat="server" AssociatedControlID="txtSignOffOwner">Supervisor
                </asp:Label><span
                    class="inputs">
                    <asp:TextBox runat="server" ID="txtSignOffOwner" CssClass="fillspan" />
                    <asp:TextBox runat="server" ID="txtSignOffOwner_ID" Style="display: none;" />
                </span><span
                    class="inputicon">&nbsp;
                </span><span
                    class="inputtooltipfield">&nbsp;
                </span><span
                    class="inputvalidatorfield">&nbsp;</span>
            </div>
            <div class="formbuttons">
                <a href="javascript:saveESRAssignment();">
                    <img src="../images/buttons/btn_save.png" alt="Save" /></a>&nbsp;&nbsp;<asp:ImageButton
                        ID="cmdesrcancel" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" />
            </div>
        </div>
    </asp:Panel>
    <asp:Panel ID="pnlLicenceAttachments" runat="server" CssClass="modalpanel" Style="display: none; width: 880px; height: 135px;">
        <div class="formpanel">
            <uc4:UploadLicenceAttachment ID="usrLicenceAttach" runat="server" />
            <div class="formbuttons">
                <asp:ImageButton ID="cmdLicenceAttachCancel" OnClientClick="javascript:refreshAttachDiv('licenceDiv', attachDocType, nEmployeeid);"
                    ImageUrl="../images/buttons/cancel_up.gif" runat="server" CausesValidation="False" Style="display: none;" />
            </div>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlEsrDetails" runat="server" CssClass="modalpanel" Style="display: none; width: 890px; height: 600px;">
        <div class="sm_panel">
            <div class="sectiontitle">
                <asp:Label ID="lblEsrDetailsTitle" runat="server" Text="Esr Details"></asp:Label>
            </div>
            <div id="divEsrDetails" runat="server" class="sm_panel" style="overflow: auto; height: 450px">
            </div>
            <div class="formbuttons">
                <asp:ImageButton ID="btnCancelEsr" OnClientClick="javascript:hideEsrDetailsModal();"
                    ImageUrl="~/shared/images/buttons/btn_close.png" runat="server" CausesValidation="False" />
            </div>
        </div>
    </asp:Panel>

    <cc1:ModalPopupExtender ID="modESRAssignmentNumber" runat="server" TargetControlID="lnkESRAssignmentNumber"
        PopupControlID="pnlESRAssignmentNumber" BackgroundCssClass="modalBackground"
        CancelControlID="cmdesrcancel">
    </cc1:ModalPopupExtender>
    <cc1:ModalPopupExtender runat="server" ID="modEsrDetail" TargetControlID="lnkEsrDetails" PopupControlID="pnlEsrDetails" BackgroundCssClass="modalBackground" CancelControlID="btnCancelEsr" />
    <asp:LinkButton ID="lnkESRAssignmentNumber" runat="server" Style="display: none;">LinkButton</asp:LinkButton>
    <asp:LinkButton ID="lnkEsrDetails" runat="server" Style="display: none;">LinkButton</asp:LinkButton>
    <cc1:ModalPopupExtender ID="modLicenceAttachment" runat="server" TargetControlID="lnkLicenceAttachment"
        PopupControlID="pnlLicenceAttachments" BackgroundCssClass="modalBackground" CancelControlID="cmdLicenceAttachCancel">
    </cc1:ModalPopupExtender>
    <asp:LinkButton ID="lnkLicenceAttachment" runat="server" Style="display: none;"></asp:LinkButton>
    <AddressDetails:Popup ID="addressDetailsPopup" runat="server" />
    <asp:HiddenField id="hdnAuthoriserLevel" runat="server" Value="0" />
     <asp:HiddenField id="hdnDefaultAuthoriserLevelId" runat="server" />
</asp:Content>

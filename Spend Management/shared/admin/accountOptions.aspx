<%@ Page Language="C#" MasterPageFile="~/masters/smPagedForm.master" AutoEventWireup="true" CodeBehind="accountOptions.aspx.cs" Inherits="Spend_Management.accountOptions" %>

<%@ MasterType VirtualPath="~/masters/smPagedForm.master" %>
<%@ Register Src="~/shared/usercontrols/tooltip.ascx" TagName="tooltip" TagPrefix="tooltip" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI.HtmlControls" Assembly="System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
    <a href="javascript:changePage('General');" id="lnkGeneral" class="selectedPage">General
        Options</a> <a href="javascript:changePage('NewExpenses');" id="lnkNewExpenses" runat="server" clientidmode="Static">New
            Expenses</a> <a href="javascript:changePage('EmailServer');" id="lnkEmailServer">Email
                Server</a> <a href="javascript:changePage('MainAdministrator');" id="lnkMainAdministrator">Main Administrator</a> <a href="javascript:changePage('RegionalSettings');" id="lnkRegionalSettings">Regional Settings</a> <a href="javascript:changePage('PasswordSettings');" id="lnkPasswordSettings">Password Settings</a>
    <asp:Literal ID="litESROptions" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentoptions" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="contentmain" runat="server">
    <tooltip:tooltip ID="usrTooltip" runat="server"></tooltip:tooltip>
    <asp:ScriptManagerProxy ID="smpAccountOptions" runat="server">
        <Services>
            <asp:ServiceReference Path="~/shared/webServices/svcAccountOptions.asmx" InlineScript="false" />
        </Services>
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/accountOptions.js?date=20180604" />
        </Scripts>
    </asp:ScriptManagerProxy>

    <script language="javascript" type="text/javascript">
        /* Removes text nodes (whitespace) that causes formatting issues within formpanels */

        var allowMileage = '<% = chkAllowMilage.ClientID %>';
        var hdnAllowMileage = '<% = hdnAllowMileage.ClientID %>';
        var activateCar = '<% = chkActivateCarOnUserAdd.ClientID %>';
        var allowUsersToAddCars = '<% = chkAllowUsersToAddCars.ClientID %>';
        var ActivateCarOnUserAdd = '<% = chkActivateCarOnUserAdd.ClientID %>';
        var EmployeeSpecifyCarStartDate = '<% = chkEmployeeSpecifyCarStartDate.ClientID %>'; 
        var EmployeeSpecifyCarStartDateMandatory = '<% = chkEmployeeSpecifyCarStartDateMandatory.ClientID %>'; 
        var pConstraint = '<% = cmblength.ClientID %>';
        var pLength1 = '<% = plength1.ClientID %>';
        var pLength2 = '<% = plength2.ClientID %>';
        var pLenLbl1 = '<% = lblMinimumPasswordLength.ClientID %>';
        var pLenLbl2 = '<% = lblMaximumPasswordLength.ClientID %>';
        var reqLength1 = '<% = reqLength1.ClientID %>';
        var compLength1 = '<% = complength1.ClientID %>';
        var compLength1LessThan = '<% = compLength1LessThan.ClientID %>';
        var compLength1Greater = '<% = compLength1Greater.ClientID %>';
        var reqLength2 = '<% = reqLength2.ClientID %>';
        var compLength2 = '<% = complength2.ClientID %>';
        var compLength2LessThan = '<% = compLength2LessThan.ClientID %>';
        var compLength2Greater = '<% = compLength2Greater.ClientID %>';
        var compMinLess = '<% = compMinLess.ClientID %>';
        var compMaxGreater = '<% = compMaxGreater.ClientID %>';
        var addScreenModalPopupID = '<% = mdlAddScreen.ClientID %>';
        var addScreenFieldName = '<% = lblFieldName.ClientID %>';
        var addScreenDisplayAs = '<% = txtDisplayAs.ClientID %>';
        var addScreenDisplayOnItem = '<% = chkDisplayOnItem.ClientID  %>';
        var addScreenDisplayFieldOnCash = "<% =  chkDisplayFieldOnCash.ClientID %>";
        var addScreenMandatoryOnCash = "<% =  chkMandatoryOnCash.ClientID  %>";
        var addScreenDisplayOnCC = "<% = chkDisplayOnCC.ClientID   %>";
        var addScreenMandatoryOnCC = "<% =  chkMandatoryOnCC.ClientID  %>";
        var addScreenDisplayOnPC = "<% = chkDisplayOnPC.ClientID  %>";
        var addScreenMandatoryOnPC = "<% = chkMandatoryOnPC.ClientID  %>";
        var addScreenHiddenField = "<% = hdnAddScreenCode.ClientID %>";
        var addScreenHiddenFieldID = "<% = hdnAddScreenFieldID.ClientID %>";
        var chkRecordOdometer = "<% = chkrecordodometer.ClientID %>";
        var optRecordOdoOnLogon = "<% = optodologin.ClientID %>";
        var spanOdoDay = "<% = spanOdoDay.ClientID %>";
        var txtOdoDay = "<% = txtodometerday.ClientID %>";
        var compOdoDayGreaterThan = "<% = compOdoDayGreaterThan.ClientID %>";
        var compOdoDayLessThan = "<% = compOdoDayLessThan.ClientID %>";
        var reqOdoDay = "<% = reqOdoDay.ClientID %>";
        var optLinkAttachmentDefault = "<% = optLinkAttachmentDefault.ClientID %>";

        var chkselfregempconact = "<% = chkselfregempconact.ClientID %>";
        var chkselfreghomaddr = "<% = chkselfreghomaddr.ClientID %>";
        var chkselfregempinfo = "<% = chkselfregempinfo.ClientID %>";
        var chkselfregrole = "<% = chkselfregrole.ClientID %>";
        var chkselfregitemrole = "<% = chkselfregitemrole.ClientID %>";
        var cmbdefaultrole = "<% = cmbdefaultrole.ClientID %>";
        var cmbdefaultitemrole = "<% = cmbdefaultitemrole.ClientID %>";
        var chkselfregsignoff = "<% = chkselfregsignoff.ClientID %>";
        var chkselfregadvancessignoff = "<% = chkselfregadvancessignoff.ClientID %>";
        var chkselfregdepcostcode = "<% = chkselfregdepcostcode.ClientID %>";
        var chkselfregbankdetails = "<% = chkselfregbankdetails.ClientID %>";
        var chkselfregcardetails = "<% = chkselfregcardetails.ClientID %>";
        var chkselfregudf = "<% = chkselfregudf.ClientID %>";
        var chkemployeedetailschanged = "<% = chkemployeedetailschanged.ClientID %>";

        var chkattachreceiptID = '<%=chkattach.ClientID %>';
        var chkmobiledevicesID = '<%=chkEnableMobileDevices.ClientID %>';
        var chkEnableClaimApprovalReminderID = '<%=chkEnableClaimApprovalReminder.ClientID %>';
        var chkEnableCurrentClaimsReminderID = '<%=chkEnableCurrentClaimsReminder.ClientID %>';
        var chkEnableAutomaticDrivingLicenceLookupID = '<%=chkDrivingLicenceLookup.ClientID %>';
        var ckkEnablePeriodicalCheckForReviews = '<%=this.chkLicenceReview.ClientID %>';
        var chkBlockDrivingLicence = '<%=this.chkblockdrivinglicence.ClientID%>';
        var chkLicenceReviewReminderNotification = '<%=this.chkLicenceReviewReminderNotification.ClientID%>';

        var chkblockmotexpiry = '<%=this.chkblockmotexpiry.ClientID%>';
        var chkblocktaxexpiry = '<%=this.chkblocktaxexpiry.ClientID%>';
        var chkVehicleDocumentLookup = '<%=this.chkVehicleDocumentLookup.ClientID%>';
        var spanVehicleDocumentLookups = '<%=this.spanVehicleDocumentLookups.ClientID%>';

        $(document).ready(function ()
        {
            if ($('#' + chkEnableCurrentClaimsReminderID)[0] !== undefined) {
                toggleValidator($('#' + chkEnableCurrentClaimsReminderID)[0]);
                toggleValidator($('#' + chkEnableClaimApprovalReminderID)[0]);
            }
            hideAutomaticDrivingLicenceLookup($('#' + chkEnableAutomaticDrivingLicenceLookupID)[0]);
            hideDrivingLicenceFrequencyPanel($('#' + ckkEnablePeriodicalCheckForReviews)[0], $('#' + chkBlockDrivingLicence)[0]);
            hideDrivingLicenceReviewReminderDaysPanel($('#' + chkLicenceReviewReminderNotification)[0]);
            hideAutomaticVehicleDocumentLookup($('#' + chkVehicleDocumentLookup)[0]);

        });
    </script>

    <div id="divPages">
        <div id="pgGeneral" style="width:98%;">
            <cc1:TabContainer ID="tabsGeneralOptions" runat="server">
                <cc1:TabPanel runat="server" HeaderText="Expense Options" ID="tabGeneral">
                    <HeaderTemplate>
                        General Details
                    </HeaderTemplate>

                    <ContentTemplate>

                        <div class="formpanel">
                            <div class="sectiontitle">General Options</div>
                            <div class="onecolumnsmall">
                                <asp:Label ID="lbldefaultdrilldown" runat="server" Text="Default drilldown report" meta:resourcekey="lbldefaultdrilldownResource1" AssociatedControlID="cmbdrilldown"></asp:Label>
                                <span class="inputs">
                                    <asp:DropDownList ID="cmbdrilldown" runat="server" meta:resourcekey="cmbdrilldownResource1"></asp:DropDownList>
                                </span>
                                <span class="inputicon">&nbsp;</span>
                                <span class="inputtooltipfield">&nbsp;</span>
                                <span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn tabcontent-div">
                                <asp:Label ID="lblMandatoryPostcodeForAddresses" runat="server" Text="Postcodes are mandatory when adding addresses" AssociatedControlID="chkMandatoryPostcodeForAddresses"></asp:Label><span class="inputs"><asp:CheckBox ID="chkMandatoryPostcodeForAddresses" runat="server" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><asp:Image ID="imgTooltipMandatoryPostcodeForAddresses" runat="server" AlternateText="Tooltip" CssClass="tooltipicon" onclick="SEL.Tooltip.Show('3e56ba0b-d851-451a-9a33-76683d2b5dd2', 'sm', this);" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" /></span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label ID="lblAllowUsersToAddCars" runat="server" Text="Allow employees to add new vehicles" AssociatedControlID="chkAllowUsersToAddCars"></asp:Label><span class="inputs"><asp:CheckBox ID="chkAllowUsersToAddCars" runat="server" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip385" onclick="SEL.Tooltip.Show('78c49bd1-107d-440b-a202-3266d35af047', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblActivateCarOnUserAdd" runat="server" Text="Activate vehicle when employee adds own" AssociatedControlID="chkActivateCarOnUserAdd"></asp:Label><span class="inputs"><asp:CheckBox ID="chkActivateCarOnUserAdd" runat="server" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn tabcontent-div">
                                <asp:Label runat="server" ID="lblEmployeeSpecifyCarStartDate" AssociatedControlID="chkEmployeeSpecifyCarStartDate"        meta:resourcekey="chkEmployeeSpecifyCarStartDate1">User can specify start date when adding own vehicle</asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkEmployeeSpecifyCarStartDate" CssClass="fillspan" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                                <asp:Label runat="server" ID="lblEmployeeSpecifyCarStartDateMandatory" AssociatedControlID="chkEmployeeSpecifyCarStartDateMandatory" meta:resourcekey="chkEmployeeSpecifyCarStartDateMandatory1">Start date when adding own vehicle is mandatory</asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkEmployeeSpecifyCarStartDateMandatory" CssClass="fillspan" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn" >
                                <asp:Label runat="server" ID="lblEmployeeSpecifyCarDOC" AssociatedControlID="chkEmployeeSpecifyCarDOC" CssClass="tabcontent-div" meta:resourcekey="chkEmployeeSpecifyCarDOC1">User can specify duty of care information when adding own vehicle</asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkEmployeeSpecifyCarDOC" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <span id="spanGeneralOptionsExpenses" runat="server">
                                 <div class="sectiontitle">VAT Options</div>
                                <div class="twocolumn " >
                                <asp:label ID="lblEnableVatOption" Text="Enable calculations for allocating fuel receipt VAT to mileage" runat="server" AssociatedControlID="chkEnableVatOptions"></asp:label>
                                <span class="inputs"><asp:CheckBox ID="chkEnableVatOptions" ClientIDMode="Static" runat="server" /></span>
                               <span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgChkVatOptionTooltip" onclick="SEL.Tooltip.Show('6F8D9F81-341A-4447-9FB3-156B24BA217A', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span>
                               </div>
                                <div class="twocolumn">
                                    <div class="sectiontitle">Hotel Review Options</div>
                                    <asp:Label ID="lblshowreviews" runat="server" meta:resourcekey="lblshowreviewsResource1" AssociatedControlID="chkshowreviews">Hotel reviews</asp:Label>

                                    <span class="inputs">
                                        <asp:CheckBox ID="chkshowreviews" runat="server" meta:resourcekey="chkshowreviewsResource1" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                                    <asp:Label ID="lblsendreview" runat="server" AssociatedControlID="chksendreviewrequest">Email claimants for hotel reviews</asp:Label><span class="inputs"><asp:CheckBox ID="chksendreviewrequest" runat="server" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                                </div>
                                <div class="twocolumn">
                                    <div class="sectiontitle">Claim Options</div>
                                    <div class="twocolumn">
                                        <asp:Label ID="lblpreapproval" runat="server" meta:resourcekey="lblpreapprovalResource1" AssociatedControlID="chkpreapproval">Claimants can enter pre approval claims</asp:Label>


                                        <span class="inputs">
                                            <asp:CheckBox ID="chkpreapproval" runat="server" meta:resourcekey="chkpreapprovalResource1" /></span>
                                        <span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>

                                        <asp:Label runat="server" ID="lblShowFullAddress" Text="Allow approvers to see claimant home address" AssociatedControlID="chkShowFullHomeAddress"></asp:Label>
                                        <span class="inputs">
                                            <asp:CheckBox runat="server" ID="chkShowFullHomeAddress" /></span>
                                        <span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><span class="inputtooltipfield"><img id="img2" onclick="SEL.Tooltip.Show('F295FAD5-852E-40E0-9647-3D0FBE9F998D', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span></span><span class="inputvalidatorfield">&nbsp;</span>
                                    </div>
                                </div>

                                <div class="twocolumn tabcontent-div">
                                    <asp:Label ID="lblTeamMemberApproveOwnClaims" runat="server" meta:resourcekey="lblTeamMemberApproveOwnClaimsResource1" AssociatedControlID="chkTeamMemberApproveOwnClaims">Allow team members to allocate and approve own claims</asp:Label>

                                    <span class="inputs">
                                        <asp:CheckBox ID="chkTeamMemberApproveOwnClaims" CssClass="fillspan" runat="server" meta:resourcekey="chkTeamMemberApproveOwnClaimsResource1" />

                                    </span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip511" onclick="SEL.Tooltip.Show('32ad3284-c618-4812-8939-abd0c6dd3b63', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblEmployeeApproveOwnClaims" runat="server" AssociatedControlID="chkEmployeeApproveOwnClaims">Allow employees and delegates to approve own claims</asp:Label>
                                    <span class="inputs">
                                        <asp:CheckBox ID="chkEmployeeApproveOwnClaims" runat="server" CssClass="fillspan" />
                                    </span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip512" onclick="SEL.Tooltip.Show('15bb667f-69b4-45fd-b141-a1d5d07ffaa1', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span>
                                </div>
                                <div class="twocolumn">
                                    <asp:Label ID="lblpartsubmittal" runat="server" meta:resourcekey="lblpartsubmittalResource1" AssociatedControlID="chkpartsubmittal">Claims can be part submitted</asp:Label>
                                    <span class="inputs">
                                        <asp:CheckBox ID="chkpartsubmittal" runat="server" meta:resourcekey="chkpartsubmittalResource1" />

                                    </span>
                                    <span class="inputicon">&nbsp;</span>
                                    <span class="inputtooltipfield">
                                        <img id="imgtooltip329" onclick="SEL.Tooltip.Show('dfb8dbfc-eda0-4e72-bae0-b435d0548b72', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" />
                                    </span>
                                    <span class="inputvalidatorfield">&nbsp;</span>
                                    <asp:Label ID="lblonlycashcredit" runat="server" meta:resourcekey="lblonlycashcreditResource1" AssociatedControlID="chkonlycashcredit">Cash and credit card items cannot be on the same claim</asp:Label>

                                    <span class="inputs">
                                        <asp:CheckBox ID="chkonlycashcredit" runat="server" meta:resourcekey="chkonlycashcreditResource1" />
                                    </span>
                                    <span class="inputicon">&nbsp;</span>
                                    <span class="inputtooltipfield">
                                        <img id="imgtooltip328" alt="" class="tooltipicon" onclick="SEL.Tooltip.Show('1e45ba7e-ad67-4806-9be5-553cb850a4d3', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" />
                                    </span>
                                    <span class="inputvalidatorfield">&nbsp;</span>
                                </div>
                                <div class="twocolumn">
                                    <asp:Label ID="lblEditPreviousClaim" runat="server" AssociatedControlID="chkEditPreviousClaim">Previous claims can be edited</asp:Label>
                                    <span class="inputs">
                                        <asp:CheckBox ID="chkEditPreviousClaim" runat="server" CssClass="fillspan" />
                                    </span>
                                    <span class="inputicon">&nbsp;</span>
                                    <span class="inputtooltipfield">
                                        <img id="img3" onclick="SEL.Tooltip.Show('E81C7826-539F-4869-B6C5-6A5DC486BA49', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" />
                                    </span>
                                    <span class="inputvalidatorfield">&nbsp;</span>
                                </div>
                                <div class="twocolumn">
                                    <asp:Label ID="lblNumberOfApproversToShowInClaimHistoryForApprovalMatrix" runat="server" AssociatedControlID="cboNumberOfApproversToShowInClaimHistoryForApprovalMatrix">Number of matrix approvers remembered for claimant</asp:Label>

                                    <span class="inputs">
                                        <asp:DropDownList ID="cboNumberOfApproversToShowInClaimHistoryForApprovalMatrix" runat="server" ClientIDMode="Static"></asp:DropDownList>

                                    </span><span class="inputicon">&nbsp;</span><img id="img330" alt="" class="tooltipicon" onclick="SEL.Tooltip.Show('35CEE06F-639B-478C-BCA1-19DADEB2B1A5', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" /><span class="inputtooltipfield">&nbsp;</span><span class="inputmultiplevalidatorfield"></span>
                                </div>
                                <div class="sectiontitle">Claim Frequency</div>
                                <div class="twocolumn"><asp:Label ID="lblclaimfrequency" runat="server" Text="Limit (0=unlimited)" AssociatedControlID="txtfrequencyvalue"></asp:Label><span class="inputs"><asp:TextBox id="txtfrequencyvalue" runat="server" MaxLength="2" ></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:CompareValidator id="compfreq" runat="server" Type="Integer" Operator="GreaterThanEqual" ControlToValidate="txtfrequencyvalue" ErrorMessage="Please enter a valid Limit." ValidationGroup="vgMain" ValueToCompare="0" meta:resourcekey="compfreqResource1">*</asp:CompareValidator></span><asp:Label ID="lblper" runat="server" Text="Period" AssociatedControlID="cmbfrequencytype"></asp:Label><span class="inputs"><asp:DropDownList id="cmbfrequencytype" runat="server" meta:resourcekey="cmbfrequencytypeResource1"></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
                                
                                <div class="sectiontitle">Email Reminders</div>
                                <div class="twocolumn">
                                <asp:label ID="lblEnableClaimApprovalReminder" Text="Remind approvers of pending claims" runat="server" AssociatedControlID="chkEnableClaimApprovalReminder"></asp:label>
                                <span class="inputs"><asp:CheckBox ID="chkEnableClaimApprovalReminder" ClientIDMode="Static" runat="server" /></span><span class="inputicon"></span><span class="inputicon"></span><span style="width:15px;display:inline-block"></span>
                                <span id="claimReminderFrequency">
                                <asp:label runat="server" ClientIDMode="Static" ID="lblClaimApprovalReminderFrequency" CssClass="mandatory" AssociatedControlID ="ddlClaimApprovalReminderFrequency" Text="Reminder frequency (days)*"></asp:label>
                                <span class="inputs"><asp:TextBox id="ddlClaimApprovalReminderFrequency" ClientIDMode="Static" runat="server" MaxLength="2" ></asp:TextBox></span><span class="inputicon"></span>
                                <img id="claimApprovalReminderTooltip" alt="" class="tooltipicon" onclick="SEL.Tooltip.Show('DB0C2851-2CC1-4E06-A0C6-EED7A6E74DAF', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" />
                                 <asp:CustomValidator runat="server" Text="*" ValidationGroup="vgMain" ID="frequencyValidator" ClientValidationFunction="frequencyDaysValidation" ControlToValidate="ddlClaimApprovalReminderFrequency" ClientIDMode="Static" ErrorMessage="Please enter a valid number of days for the Approver Reminder frequency." ValidateEmptyText="true"></asp:CustomValidator>
                               </span>
                               </div>

                                 <div class="twocolumn">
                                <asp:label ID="lblEnableCurrentClaimsReminder" Text="Remind claimants to submit their claims" runat="server" AssociatedControlID="chkEnableCurrentClaimsReminder"></asp:label>
                                <span class="inputs"><asp:CheckBox ID="chkEnableCurrentClaimsReminder" ClientIDMode="Static" runat="server" /></span><span class="inputicon"></span><span class="inputicon"></span><span style="width:15px;display:inline-block"></span>
                                <span id="currentClaimReminderFrequency">
                                <asp:label runat="server" ClientIDMode="Static" ID="lblCurrentClaimReminderFrequency" CssClass="mandatory" AssociatedControlID ="ddlCurrentClaimReminderFrequency" Text="Reminder frequency (days)*"></asp:label>
                                <span class="inputs"><asp:TextBox id="ddlCurrentClaimReminderFrequency" ClientIDMode="Static" runat="server" MaxLength="2" ></asp:TextBox></span><span class="inputicon"></span>
                                <img class="tooltipicon" onclick="SEL.Tooltip.Show('52DBFD5E-6462-4331-A19E-1DB25060D390', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" />
                                 <asp:CustomValidator runat="server" Text="*" ValidationGroup="vgMain" ID="currentClaimsFrequencyValidator" ClientValidationFunction="frequencyDaysValidation" ControlToValidate="ddlCurrentClaimReminderFrequency" ClientIDMode="Static" ErrorMessage="Please enter a valid number of days for the Claimant Reminder frequency." ValidateEmptyText="true"></asp:CustomValidator>
                               </span>
                               </div>
                               
                                 <div class="sectiontitle">Flag Options</div>
                                <div class="onecolumn"><asp:Label ID="lblflagmsg" runat="server" Text="When a claimant submits a claim, display the following message if an item is flagged" AssociatedControlID="txtflagmessage"></asp:Label><span class="inputs"><asp:TextBox ID="txtflagmessage" runat="server" MaxLength="4000" TextMode="MultiLine" meta:resourcekey="txtflagmessageResource1"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
                                <div class="sectiontitle">Odometer Options</div>
                                <div class="twocolumn tabcontent-div">
                                    <asp:Label ID="lblrecordodometer" runat="server" meta:resourcekey="lblrecordodometerResource1" AssociatedControlID="chkrecordodometer">Relevant claimants required to record odometer readings</asp:Label>
                                    <span class="inputs">
                                        <asp:CheckBox
                                            ID="chkrecordodometer" runat="server"
                                            meta:resourceKey="chkrecordodometerResource1"></asp:CheckBox>

                                    </span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator
                                        ID="compOdoDayGreaterThan" runat="server" ControlToValidate="txtodometerday"
                                        Operator="GreaterThan" Type="Integer" ValueToCompare="0" Text="*"
                                        ErrorMessage="The value you have entered for 'Odometer readings recorded every month on' must be greater than 0"
                                        Enabled="False" ValidationGroup="vgMain" Display="Dynamic"></asp:CompareValidator>

                                        <asp:CompareValidator ID="compOdoDayLessThan" runat="server" ControlToValidate="txtodometerday"
                                            Operator="LessThan" Type="Integer" ValueToCompare="29" Text="*"
                                            ErrorMessage="The value you have entered for 'Odometer readings recorded every month on' must be less than 29"
                                            Enabled="False" ValidationGroup="vgMain" Display="Dynamic"></asp:CompareValidator>

                                        <asp:CompareValidator ID="compNum" runat="server" ControlToValidate="txtodometerday" Operator="DataTypeCheck"
                                            Type="Integer" Text="*"
                                            ErrorMessage="The value you have entered must be a number" Enabled="False"
                                            ValidationGroup="vgMain" Display="Dynamic"></asp:CompareValidator>

                                        <asp:RequiredFieldValidator ID="reqOdoDay" runat="server" ControlToValidate="txtodometerday" Text="*"
                                            ErrorMessage="A value must be entered for 'Odometer readings recorded every month on'"
                                            Enabled="False" ValidationGroup="vgMain" Display="Dynamic"></asp:RequiredFieldValidator>
                                    </span><span id="spanOdoDay" runat="server" style="display: none;">
                                        <asp:Label ID="lblodometerday" runat="server" meta:resourcekey="lblodometerdayResource1" AssociatedControlID="txtodometerday">Odometer readings recorded every month on</asp:Label></span>
                                    <span class="inputs">
                                        <asp:TextBox ID="txtodometerday" MaxLength="2" runat="server" Style="display: none;" meta:resourceKey="txtodometerdayResource1"></asp:TextBox>

                                    </span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;
                                    </span>
                                </div>
                                <div class="twocolumn">
                                    <asp:Label ID="lblOptOdoLogin" runat="server" AssociatedControlID="optodologin">Odometer readings recorded when the claimant logs on</asp:Label>

                                    <span class="inputs">
                                        <asp:RadioButton ID="optodologin" runat="server" GroupName="odometerentry" meta:resourcekey="optodologinResource1"></asp:RadioButton>

                                    </span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblOptOdoSubmit" runat="server" AssociatedControlID="optodosubmit">Odometer readings recorded when claim submitted</asp:Label>

                                    <span class="inputs">
                                        <asp:RadioButton ID="optodosubmit" runat="server" GroupName="odometerentry" meta:resourcekey="optodosubmitResource1"></asp:RadioButton>

                                    </span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                                </div>
                                
                                <div class="sectiontitle">Currency Options</div>
                                <div class="twocolumn">
                                    <div class="comment" id="divComment" runat="server"><asp:Label runat="server" ID="lblExchangeRateWarning" runat="server" Text="" ></asp:Label></div>
                                    <asp:label ID="lblEnableAutoUpdateOfExchangeRates" Text="Allow automatic daily update of exchange rates" runat="server" AssociatedControlID="chkEnableAutoUpdateOfExchangeRates"></asp:label>
                                    <span class="inputs"><asp:CheckBox ID="chkEnableAutoUpdateOfExchangeRates" ClientIDMode="Static" runat="server" onchange="hideProvidersIfChecked();" /></span>
                                    <span class="inputicon">
                                    
                                    </span><span class="inputtooltipfield"><img id="enableAutoUpdateOfExchangeRatesTooltip" alt="" class="tooltipicon" onclick="SEL.Tooltip.Show('B7DE681E-AD78-4B6D-8EB1-9A3989017C99', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" /></span><span class="inputvalidatorfield">&nbsp;</span>
                                   <span id="exchangeRateProvider">
                                        <asp:Label ID="lblExchangeRateProvider" runat="server" Text="Exchange rate provider" meta:resourcekey="lblExchangeRateProviderResource" AssociatedControlID="ddlExchangeRateProvider"></asp:Label>
                                        <span class="inputs">
                                            <asp:DropDownList ID="ddlExchangeRateProvider" runat="server">
                                                <asp:ListItem Value="1">Open Exchange Rates</asp:ListItem>
                                            </asp:DropDownList></span><span class="inputicon"></span>
                                        
                                        <span class="inputtooltipfield"><img id="exchangeRateProviderTooltip" alt="" class="tooltipicon" onclick="SEL.Tooltip.Show('B06FA9F7-8EEF-45F3-897F-5E3CDF886B15', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" /></span><span class="inputvalidatorfield">&nbsp;</span>

                                       
                                    </span>
                                </div>

                                <div class="twocolumn">
                                    <div class="sectiontitle tabcontent-div1">Help &amp; Support Options</div>
                                    <asp:Label runat="server" AssociatedControlID="chkInternalTickets">Internal support tickets</asp:Label>
                                    <span class="inputs">
                                        <asp:CheckBox ID="chkInternalTickets" runat="server" />
                                    </span>
                                    <span class="inputicon">
                                        </span>
                                    <span class="inputtooltipfield"><img class="tooltipicon" onclick="SEL.Tooltip.Show('736FC38A-B03C-4BFA-BF9A-AC9D5BE2AAD3', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" /></span>
                                    <span class="inputvalidatorfield">&nbsp;</span>
                                </div>
                                
                                    <div class="sectiontitle">Corporate Cards</div>
                                    <div class="twocolumn"><asp:Label runat="server" AssociatedControlID="chkBlockUnmatchedExpenseItemsBeingSubmitted">Do not allow unmatched expense items to be submitted</asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkBlockUnmatchedExpenseItemsBeingSubmitted"></asp:CheckBox></span><span class="inputicon"></span><span class="inputtooltipfield"><img id="imgTooltipDoNotAllowUnmatchedExpenseItems" onclick="SEL.Tooltip.Show('55244FA2-E168-4EFF-AEE4-EFA23487183D', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"></span></div>
                                
                            </span>
                        </div>
                    </ContentTemplate>




                </cc1:TabPanel>
                <cc1:TabPanel ID="tabFWGeneral" runat="server" HeaderText="General Details">
                    <ContentTemplate>
                        <div class="formpanel">
                            <div class="sectiontitle">General Options</div>
                            <div class="twocolumn">
                                <asp:Label runat="server" ID="lblUploadAttachmentEnabled" AssociatedControlID="chkUploadAttachmentEnabled" Text="Upload attachments enabled?"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkUploadAttachmentEnabled" CssClass="fillspan"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                                <asp:Label runat="server" ID="lblHyperlinkAttachmentsEnabled" AssociatedControlID="chkHyperlinkAttachmentsEnabled" Text="Hyperlink Attachments Enabled?"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkHyperlinkAttachmentsEnabled" CssClass="fillspan"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label runat="server" ID="lblLinkAttachmentDefault" AssociatedControlID="optLinkAttachmentDefault" Text="Link attachment default"></asp:Label><span class="inputs"><asp:DropDownList runat="server" ID="optLinkAttachmentDefault" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                                <asp:Label runat="server" ID="lblMaxUploadSize" AssociatedControlID="txtMaxUploadSize" Text="Max file upload Size (Kbytes)"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtMaxUploadSize" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator runat="server" ID="cmpMaxUploadSize" ControlToValidate="txtMaxUploadSize" Type="Integer" Operator="DataTypeCheck" Text="*" ErrorMessage="Invalid entry in Max upload size field" ValidationGroup="vgMain"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpUploadFileSizeMin" ControlToValidate="txtMaxUploadSize" Operator="GreaterThanEqual" ValueToCompare="512" Type="Integer" Text="*" ErrorMessage="Minimum file upload size is 512 Kbytes" ValidationGroup="vgMain"></asp:CompareValidator></span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label runat="server" ID="lblNotesIconFlash" AssociatedControlID="chkNotesIconFlash" Text="Flashing notes icon?"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkNotesIconFlash" CssClass="fillspan"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                                <asp:Label runat="server" ID="lblShowProductInSearch" AssociatedControlID="chkShowProductInSearch" Text="Show product in home page search results"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkShowProductInSearch" CssClass="fillspan"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label runat="server" ID="lblFYStarts" AssociatedControlID="optFYStarts" Text="Financial year starts"></asp:Label><span class="inputs"><asp:DropDownList runat="server" ID="optFYStarts" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                                <asp:Label runat="server" ID="lblFYEnds" AssociatedControlID="optFYEnds" Text="Financial year ends"></asp:Label><span class="inputs"><asp:DropDownList runat="server" ID="optFYEnds" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label runat="server" ID="lblDefaultPageSize" AssociatedControlID="txtDefaultPageSize" Text="Default report page size (rows)"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtDefaultPageSize" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator runat="server" ID="cmpDefaultPageSize" ControlToValidate="txtDefaultPageSize" Text="*" ErrorMessage="Invalid data entry for Default page size field" Operator="DataTypeCheck" Type="Integer" ValidationGroup="vgMain"></asp:CompareValidator></span>
                                <asp:Label ID="lblfwEditMyDetails" runat="server" AssociatedControlID="chkfwEditMyDetails">Employees may edit their own personal details</asp:Label>
                                <span class="inputs">
                                    <asp:CheckBox ID="chkfwEditMyDetails" CssClass="fillspan" runat="server" />
                                </span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                        </div>

                        <div class="formpanel">
                            <div class="sectiontitle">Product Options</div>
                            <div class="twocolumn">
                                <asp:Label runat="server" ID="lblAutoUpdateLicenceTotal" AssociatedControlID="chkAutoUpdateLicenceTotal" Text="Auto update product licence totals"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkAutoUpdateLicenceTotal" CssClass="fillspan"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                        </div>
                        <div class="formpanel">
                            <div class="sectiontitle">Task Options</div>
                            <div class="twocolumn">
                                <asp:Label runat="server" ID="lblTaskDueDateMandatory" AssociatedControlID="chkTaskDueDateMandatory" Text="Task due date mandatory?"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkTaskDueDateMandatory" CssClass="fillspan"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                                <asp:Label runat="server" ID="lblTaskStartDateMandatory" AssociatedControlID="chkTaskStartDateMandatory" Text="Task start date mandatory?"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkTaskStartDateMandatory" CssClass="fillspan"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label runat="server" ID="lblTaskEndDateMandatory" AssociatedControlID="chkTaskEndDateMandatory" Text="Task end date mandatory?"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkTaskEndDateMandatory" CssClass="fillspan"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                                <asp:Label runat="server" ID="lblTaskEscalationRepeat" AssociatedControlID="txtTaskEscalationRepeat" Text="Task escalation repeat (days)"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtTaskEscalationRepeat" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator runat="server" ID="cmpTaskEscalationRepeat" ControlToValidate="txtTaskEscalationRepeat" Text="*" ErrorMessage="Invalid value entered for Task escalation repeat field" Operator="DataTypeCheck" Type="Integer" ValidationGroup="vgMain"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpTaskEscalationRepeatMin" ControlToValidate="txtTaskEscalationRepeat" Text="*" ErrorMessage="Task escalation repeat field must be greater than zero" Operator="GreaterThan" ValueToCompare="0" ValidationGroup="vgMain"></asp:CompareValidator></span>
                            </div>
                        </div>

                        <div class="formpanel">
                            <div class="sectiontitle tabcontent-div1" >Help &amp; Support Options</div>
                            <div class="twocolumn">
                                <asp:Label runat="server" AssociatedControlID="chkInternalTicketsFW">Internal support tickets</asp:Label>
                                <span class="inputs">
                                    <asp:CheckBox ID="chkInternalTicketsFW" runat="server" />
                                </span>
                                <span class="inputicon">
                                    <img class="tooltipicon" onclick="SEL.Tooltip.Show('736FC38A-B03C-4BFA-BF9A-AC9D5BE2AAD3', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" /></span>
                                <span class="inputtooltipfield">&nbsp;</span>
                                <span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                        </div>
                        
                    </ContentTemplate>
                </cc1:TabPanel>
                <cc1:TabPanel runat="server" HeaderText="Employee Options" ID="tabEmployees">
                    <HeaderTemplate>
                        Employees
                    </HeaderTemplate>


                    <ContentTemplate>
                        <div class="formpanel">
                            <div class="sectiontitle">Employee Options</div>
                            <div class="twocolumn">
                                    <asp:Label ID="Label14" runat="server" meta:resourcekey="Label14Resource1" AssociatedControlID="chkeditmydetails">Users may edit their own personal details</asp:Label>
                                <span class="inputs">
                                    <asp:CheckBox ID="chkeditmydetails" CssClass="fillspan" runat="server" meta:resourcekey="chkeditmydetailsResource1" />
                                </span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label ID="lblsearchemployees" runat="server" meta:resourcekey="lblsearchemployeesResource1" AssociatedControlID="chksearchemployees">Employee directory</asp:Label>
                                <span class="inputs">
                                    <asp:CheckBox ID="chksearchemployees" runat="server" meta:resourcekey="chksearchemployeesResource1" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblemployeedetailschanged" runat="server" AssociatedControlID="chkemployeedetailschanged">Users can notify administrators of change of details</asp:Label>
                                <span class="inputs">
                                    <asp:CheckBox ID="chkemployeedetailschanged" runat="server" ClientIDMode="Static" onclick="javascript:return EmpNotifyOfChangesClick();" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><asp:Image ID="imgTooltip587" runat="server" AlternateText="Tooltip" CssClass="tooltipicon" onclick="SEL.Tooltip.Show('32d7d7bb-69ea-43cf-a22a-dbf0a5306ef2', 'sm', this);" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" /></span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                        </div>
                    </ContentTemplate>
                </cc1:TabPanel>

                <cc1:TabPanel ID="tabSelfRegistration" runat="server" HeaderText="Self Registration">
                    <ContentTemplate>
                        <div class="formpanel">
                            <div class="sectiontitle">Self Registration Options</div>
                            <div class="twocolumn">
                                <asp:Label ID="lblallowregistration" runat="server" Text="Allow self registration" meta:resourcekey="lblallowregistrationResource1" AssociatedControlID="chkallowselfreg"></asp:Label><span class="inputs"><asp:CheckBox ID="chkallowselfreg" runat="server" meta:resourcekey="chkallowselfregResource1" onclick="javascript:CheckSelfRegistrationStatus(this);" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblemployeecontactdetails" runat="server" Text="Employment contact details" meta:resourcekey="lblemployeecontactdetailsResource1" AssociatedControlID="chkselfregempconact"></asp:Label><span class="inputs"><asp:CheckBox ID="chkselfregempconact" runat="server" meta:resourcekey="chkselfregempconactResource1" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label ID="lblhomeaddress" runat="server" Text="Home address/contact details" meta:resourcekey="lblhomeaddressResource1" AssociatedControlID="chkselfreghomaddr"></asp:Label><span class="inputs"><asp:CheckBox ID="chkselfreghomaddr" runat="server" meta:resourcekey="chkselfreghomaddrResource1" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblemployementinfo" runat="server" Text="Employment information" meta:resourcekey="lblemployementinfoResource1" AssociatedControlID="chkselfregempinfo"></asp:Label><span class="inputs"><asp:CheckBox ID="chkselfregempinfo" runat="server" meta:resourcekey="chkselfregempinfoResource1" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label ID="lblrole" runat="server" Text="Role" meta:resourcekey="lblroleResource1" AssociatedControlID="chkselfregrole"></asp:Label><span class="inputs"><asp:CheckBox ID="chkselfregrole" runat="server" meta:resourcekey="chkselfregroleResource1" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><span runat="server" id="itemRoleSpan"><asp:Label ID="lblItemRole" runat="server" Text="Item role" meta:resourcekey="lblItemRoleResource1" AssociatedControlID="chkselfregitemrole"></asp:Label><span class="inputs"><asp:CheckBox ID="chkselfregitemrole" runat="server" meta:resourcekey="chkselfregitemroleResource1" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span></span>
                            </div>
                            <div class="onecolumnsmall">
                                <asp:Label ID="lbldefaultrole" runat="server" Text="Default role" meta:resourcekey="lbldefaultroleResource1" AssociatedControlID="cmbdefaultrole"></asp:Label><span class="inputs"><asp:DropDownList ID="cmbdefaultrole" runat="server" meta:resourcekey="cmbdefaultroleResource1"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="onecolumnsmall" runat="server" id="defaultItemRoleRow">
                                <asp:Label ID="lblDefaultItemRole" runat="server" Text="Default item role" meta:resourcekey="lblDefaultItemRoleResource1" AssociatedControlID="cmbdefaultitemrole"></asp:Label><span class="inputs"><asp:DropDownList ID="cmbdefaultitemrole" runat="server" meta:resourcekey="cmbdefaultitemroleResource1"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn" runat="server" id="signoffRow">
                                <asp:Label ID="lblsignoffgroup" runat="server" Text="Signoff group" meta:resourcekey="lblsignoffgroupResource1" AssociatedControlID="chkselfregsignoff"></asp:Label><span class="inputs"><asp:CheckBox ID="chkselfregsignoff" runat="server" meta:resourcekey="chkselfregsignoffResource1" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lbladvancessignoff" runat="server" Text="Advances signoff group" meta:resourcekey="lbladvancessignoffResource1" AssociatedControlID="chkselfregadvancessignoff"></asp:Label><span class="inputs"><asp:CheckBox ID="chkselfregadvancessignoff" runat="server" meta:resourcekey="chkselfregadvancessignoffResource1" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label ID="lbldepartment" runat="server" Text="Default department/cost code" meta:resourcekey="lbldepartmentResource1" AssociatedControlID="chkselfregdepcostcode"></asp:Label><span class="inputs"><asp:CheckBox ID="chkselfregdepcostcode" runat="server" meta:resourcekey="chkselfregdepcostcodeResource1" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblbankdetails" runat="server" Text="Bank details" meta:resourcekey="lblbankdetailsResource1" AssociatedControlID="chkselfregbankdetails"></asp:Label><span class="inputs"><asp:CheckBox ID="chkselfregbankdetails" runat="server" meta:resourcekey="chkselfregbankdetailsResource1" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label ID="lblcardetails" runat="server" Text="Vehicle details" meta:resourcekey="lblcardetailsResource1" AssociatedControlID="chkselfregcardetails"></asp:Label><span class="inputs"><asp:CheckBox ID="chkselfregcardetails" runat="server" meta:resourcekey="chkselfregcardetailsResource1" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lbladditionaludfs" runat="server" Text="Additional user defined fields" meta:resourcekey="lbladditionaludfsResource1" AssociatedControlID="chkselfregudf"></asp:Label><span class="inputs"><asp:CheckBox ID="chkselfregudf" runat="server" meta:resourcekey="chkselfregudfResource1" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                        </div>
                    </ContentTemplate>




                </cc1:TabPanel>
                <cc1:TabPanel runat="server" HeaderText="Delegates" ID="tabDelegates">
                    <ContentTemplate>
                        <div class="formpanel">
                            <div class="comment" id="instructions">
                            <span>Named delegates are individuals who have been specifically selected as a delegate by another user within the My Details |  Delegate menu. This type of delegate can only logon as a delegate for the user(s) that have specifically named them as a delegate.
                             <br /><br />Access Role Delegates are users who have an Access Role which includes the Delegate Logon element. They can search for and act as delegate for any user with in the product.
                            </span></div>
                             <div id="delegateExpensesSubmitClaimDivSectionTiltle" runat="server"  class="sectiontitle">Delegate Options</div>
                            <div id="delegateExpensesSubmitClaimDiv"  runat="server" class="twocolumn">
                                <asp:Label ID="lbldelSubmitClaims" runat="server" Text="Named and access role delegates can submit claims" AssociatedControlID="chkDelsSubmitClaims"></asp:Label><span class="inputs"><asp:CheckBox ID="chkDelsSubmitClaims" runat="server" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><asp:Image ID="imgTooltipCanSubmit" runat="server" AlternateText="Tooltip" CssClass="tooltipicon" onclick="SEL.Tooltip.Show('84385146-9458-4DEA-A70E-1488551E1CBC', 'sm', this);" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" /></span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="sectiontitle">Named Delegate Options</div>
                            <div class="twocolumn tabcontent-div">
                                <asp:Label ID="lblcategories" runat="server" Text="Modify categories and system options" meta:resourcekey="lblcategoriesResource1" AssociatedControlID="chkdelsetup"></asp:Label><span class="inputs"><asp:CheckBox ID="chkdelsetup" runat="server" meta:resourcekey="chkdelsetupResource1" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblemployees" runat="server" Text="" meta:resourcekey="lblemployeesResource1" AssociatedControlID="chkdelemployeeadmin"></asp:Label><span class="inputs"><asp:CheckBox ID="chkdelemployeeadmin" runat="server" meta:resourcekey="chkdelemployeeadminResource1" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div id="delegateExpensesDiv" runat="server">
                                <div class="twocolumn">
                                    <asp:Label ID="lblcheckandpay" runat="server" Text="Check and pay expenses" meta:resourcekey="lblcheckandpayResource1" AssociatedControlID="chkdelcheckandpay"></asp:Label><span class="inputs"><asp:CheckBox ID="chkdelcheckandpay" runat="server" meta:resourcekey="chkdelcheckandpayResource1" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblquickedit" runat="server" Text="Quick edit design" meta:resourcekey="lblquickeditResource1" AssociatedControlID="chkdelqedesign"></asp:Label><span class="inputs"><asp:CheckBox ID="chkdelqedesign" runat="server" meta:resourcekey="chkdelqedesignResource1" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                                </div>
                                <div class="twocolumn">
                                    <asp:Label ID="lblmanageadvances" runat="server" Text="Manage and approve advance requests" meta:resourcekey="lblmanageadvancesResource1" AssociatedControlID="chkdelapprovals"></asp:Label><span class="inputs"><asp:CheckBox ID="chkdelapprovals" runat="server" meta:resourcekey="chkdelapprovalsResource1" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><span id="corporateCardsSpan" runat="server"><asp:Label ID="lblimportcorporatecard" runat="server" Text="Import corporate card statements" meta:resourcekey="lblimportcorporatecardResource1" AssociatedControlID="chkdelcorporatecard"></asp:Label><span class="inputs"><asp:CheckBox ID="chkdelcorporatecard" runat="server" meta:resourcekey="chkdelcorporatecardResource1" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span></span>
                                </div>
                            </div>
                            <div class="twocolumn">
                                <asp:Label ID="lblauditlog" runat="server" Text="Search and view the audit log" meta:resourcekey="lblauditlogResource1" AssociatedControlID="chkdelauditlog"></asp:Label><span class="inputs"><asp:CheckBox ID="chkdelauditlog" runat="server" meta:resourcekey="chkdelauditlogResource1" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblexportdata" runat="server" Text="Export data and view export history" meta:resourcekey="lblexportdataResource1" AssociatedControlID="chkdelexports"></asp:Label><span class="inputs"><asp:CheckBox ID="chkdelexports" runat="server" meta:resourcekey="chkdelexportsResource1" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label ID="lbladminreports" runat="server" Text="View administrator reports" meta:resourcekey="lbladminreportsResource1" AssociatedControlID="chkdelreports"></asp:Label><span class="inputs"><asp:CheckBox ID="chkdelreports" runat="server" meta:resourcekey="chkdelreportsResource1" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><span id="delegateExpensesSpan" runat="server"><asp:Label ID="lblclaimantreports" runat="server" Text="View claimant reports" meta:resourcekey="lblclaimantreportsResource1" AssociatedControlID="chkdelclaimantreports"></asp:Label><span class="inputs"><asp:CheckBox ID="chkdelclaimantreports" runat="server" meta:resourcekey="chkdelclaimantreportsResource1" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span></span>
                            </div>
                            <div class="sectiontitle">Delegate Access Role Options</div>
                            <div class="twocolumn">
                                <asp:Label ID="lblDelOptionsForAccessRole" runat="server" Text="Restrict users with the Delegate Logon element on their Access Role to the same options as named delegates" meta:resourcekey="lblDelOptionsForAccessRoleResource1" AssociatedControlID="chkDelOptionForDelAccessRole"></asp:Label><span class="inputs"><asp:CheckBox ID="chkDelOptionForDelAccessRole" runat="server" meta:resourcekey="chkDelOptionForDelAccessRoleResource1" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><asp:Image ID="Image1" runat="server" AlternateText="Tooltip" CssClass="tooltipicon" onclick="SEL.Tooltip.Show('8491C99C-F944-46ED-9059-B1A48F0A8534', 'sm', this);" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" /></span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                        </div>
                    </ContentTemplate>




                </cc1:TabPanel>
                <cc1:TabPanel ID="tabDeclarations" runat="server" HeaderText="Declaration">
                    <ContentTemplate>
                        <div class="formpanel">
                            <div class="sectiontitle">Declaration Options</div>
                            <div class="twocolumn">
                                <asp:Label ID="lblDeclaration" runat="server" AssociatedControlID="chkclaimantdeclaration">Electronic declaration</asp:Label><span class="inputs"><asp:CheckBox ID="chkclaimantdeclaration" runat="server" /></span>
                            </div>
                            <div class="onecolumn">
                                <asp:Label ID="lblTxtDeclarationMsg" runat="server" AssociatedControlID="txtdeclarationmsg">Claimant declaration message</asp:Label><span class="inputs"><asp:TextBox ID="txtdeclarationmsg" runat="server" TextMode="MultiLine" Rows="5" Width="300"></asp:TextBox></span>
                            </div>
                            <div class="onecolumn">
                                <asp:Label ID="lblTxtApproverDeclarationMsg" runat="server" AssociatedControlID="txtapproverdeclarationmsg">Approver declaration message</asp:Label><span class="inputs"><asp:TextBox ID="txtapproverdeclarationmsg" runat="server" TextMode="MultiLine" Rows="5" Width="300"></asp:TextBox></span>
                            </div>
                        </div>


                    </ContentTemplate>
                </cc1:TabPanel>
                <cc1:TabPanel runat="server" ID="tabMobileDevices" HeaderText="Mobile Devices">
                    <ContentTemplate>
                        <div class="formpanel">
                            <div class="sectiontitle">Mobile Device Options</div>
                            <div class="twocolumn">
                                <asp:Label runat="server" ID="lblEnableMobileDevices" AssociatedControlID="chkEnableMobileDevices" Text="Enable mobile devices"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkEnableMobileDevices" CssClass="fillspan" onclick="javascript:checkMobileDeviceReceiptCompatibility('mobileDevices');" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip575" onclick="SEL.Tooltip.Show('c792605c-dd13-4a3f-bcb2-d1bc0d840a24', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                        </div>
                    </ContentTemplate>
                </cc1:TabPanel>


                <cc1:TabPanel ID="tabContracts" runat="server" HeaderText="Contracts">
                    <ContentTemplate>
                        <div class="formpanel">
                            <div class="sectiontitle">Contract Options</div>
                            <div class="twocolumn">
                                <asp:Label runat="server" ID="lblContractKey" AssociatedControlID="txtContractKey" Text="Contract key prefix"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtContractKey" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator runat="server" ID="reqContractKey" ControlToValidate="txtContractKey" Text="*" ErrorMessage="Contract Key field is mandatory"></asp:RequiredFieldValidator></span>
                                <asp:Label runat="server" ID="lblScheduleDefault" AssociatedControlID="txtScheduleDefault" Text="Contract schedule default"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtScheduleDefault" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label runat="server" ID="lblContractDescriptionTitle" AssociatedControlID="txtContractDescriptionTitle" Text="Contract description title"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtContractDescriptionTitle" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator runat="server" ID="reqContractDescTitle" ControlToValidate="txtContractDescriptionTitle" Text="*" ErrorMessage="Contract description title is mandatory" ValidationGroup="vgMain"></asp:RequiredFieldValidator></span>
                                <asp:Label runat="server" ID="lblContractDescriptionTitleAbbrev" AssociatedControlID="txtContractDescriptionTitleAbbrev" Text="Abbreviated contract description title"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtContractDescriptionTitleAbbrev" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator runat="server" ID="reqContractDescriptionTitleAbbrev" ControlToValidate="txtContractDescriptionTitleAbbrev" Text="*" ErrorMessage="Contract description title abbreviated is mandatory" ValidationGroup="vgMain"></asp:RequiredFieldValidator></span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label runat="server" ID="lblContractCategoryTitle" AssociatedControlID="txtContractCategoryTitle" Text="Contract category title"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtContractCategoryTitle" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator runat="server" ID="reqContractCategoryTitle" ControlToValidate="txtContractCategoryTitle" Text="*" ErrorMessage="Contract category title is mandatory" ValidationGroup="vgMain"></asp:RequiredFieldValidator></span>
                                <asp:Label runat="server" ID="lblContractCategoryMandatory" AssociatedControlID="chkContractCategoryMandatory" Text="Contract category mandatory?"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkContractCategoryMandatory" CssClass="fillspan"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label runat="server" ID="lblPenaltyClauseTitle" AssociatedControlID="txtPenaltyClauseTitle" Text="Penalty clause title"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtPenaltyClauseTitle" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator runat="server" ID="reqPenaltyClauseTitle" ControlToValidate="txtPenaltyClauseTitle" Text="*" ErrorMessage="Penalty clause title is mandatory" ValidationGroup="vgMain"></asp:RequiredFieldValidator></span>
                                <asp:Label runat="server" ID="lblTermtypeActive" AssociatedControlID="chkTermTypeActive" Text="Term type field active?"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkTermTypeActive" CssClass="fillspan"></asp:CheckBox>
                                </span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label runat="server" ID="lblContractDatesMandatory" AssociatedControlID="chkContractDatesMandatory" Text="Contract dates mandatory?"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkContractDatesMandatory" CssClass="fillspan"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                                <asp:Label runat="server" ID="lblInflatorActive" AssociatedControlID="chkInflatorActive" Text="Inflator active?"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkInflatorActive" CssClass="fillspan"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label runat="server" ID="lblContractNumberGenerate" AssociatedControlID="chkContractNumberGenerate" Text="Auto generate contract numbers?"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkContractNumberGenerate" CssClass="fillspan"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                                <asp:Label runat="server" ID="lblContractNumberUpdatable" AssociatedControlID="chkContractNumberUpdatable" Text="Is Auto generated contract number updatable?"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkContractNumberUpdatable" CssClass="fillspan"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label runat="server" ID="lblContractNumberCurSeq" AssociatedControlID="txtContractNumberCurSeq" Text="Current contract number sequence value"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtContractNumberCurSeq" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label runat="server" ID="lblAutoUpdateCV" AssociatedControlID="chkAutoUpdateCV" Text="Auto update annual contract value field?"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkAutoUpdateCV" CssClass="fillspan"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                                <asp:Label runat="server" ID="lblArchivedNotesAdd" AssociatedControlID="chkArchivedNotesAdd" Text="Allow notes to be added to archived contracts?"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkArchivedNotesAdd" CssClass="fillspan"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label runat="server" ID="lblInvoiceFrequencyActive" AssociatedControlID="chkInvoiceFrequencyActive" Text="Invoice frequency active?"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkInvoiceFrequencyActive" CssClass="fillspan"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                                <asp:Label runat="server" ID="lblVariationAutoSeq" AssociatedControlID="chkVariationAutoSeq" Text="Auto sequence variations?"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkVariationAutoSeq" CssClass="fillspan"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label runat="server" ID="lblValueComments" AssociatedControlID="txtValueComments" Text="Financial value comments"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtValueComments" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                        </div>
                    </ContentTemplate>
                </cc1:TabPanel>
                <cc1:TabPanel ID="tabInvoices" runat="server" HeaderText="Invoices">
                    <ContentTemplate>
                        <div class="formpanel">
                            <div class="sectiontitle">Invoice Options</div>
                            <div class="twocolumn">
                                <asp:Label runat="server" ID="lblKeepInvForecasts" AssociatedControlID="chkKeepInvForecasts" Text="Keep invoice forecast on move to actual?"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkKeepInvForecasts" CssClass="fillspan"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label runat="server" ID="lblPONumberGenerate" AssociatedControlID="chkPONumberGenerate" Text="Auto generate purchase order numbers?"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkPONumberGenerate" CssClass="fillspan"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label runat="server" ID="lblPONumberFormat" AssociatedControlID="txtPONumberFormat" Text="Purchase order number format"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtPONumberFormat" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                                <asp:Label runat="server" ID="lblPOSequenceNumber" AssociatedControlID="txtPOSequenceNumber" Text="Current purchase order sequence number"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtPOSequenceNumber" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                        </div>
                    </ContentTemplate>
                </cc1:TabPanel>
                <cc1:TabPanel runat="server" ID="tabSupplier" HeaderText="Suppliers">
                    <ContentTemplate>
                        <div class="formpanel">
                            <div class="sectiontitle">Supplier Options</div>
                            <div class="twocolumn">
                                <asp:Label runat="server" ID="lblSupplierPrimaryTitle" AssociatedControlID="txtSupplierPrimaryTitle" Text="Supplier title"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtSupplierPrimaryTitle" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator runat="server" ID="reqSupplierTitle" ControlToValidate="txtSupplierPrimaryTitle" Text="*" ErrorMessage="Supplier primary title is mandatory" ValidationGroup="vgMain"></asp:RequiredFieldValidator></span>
                                <asp:Label runat="server" ID="lblSupplierRegionTitle" AssociatedControlID="txtSupplierRegionTitle" Text="Supplier region title"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtSupplierRegionTitle" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator runat="server" ID="reqSupplierRegionTitle" ControlToValidate="txtSupplierRegionTitle" Text="*" ErrorMessage="Supplier region title is mandatory" ValidationGroup="vgMain"></asp:RequiredFieldValidator></span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label runat="server" ID="lblSupplierCategoryTitle" AssociatedControlID="txtSupplierCategoryTitle" Text="Supplier category title"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtSupplierCategoryTitle" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator runat="server" ID="reqSupplierCategoryTitle" ControlToValidate="txtSupplierCategoryTitle" Text="*" ErrorMessage="Supplier category is mandatory" ValidationGroup="vgMain"></asp:RequiredFieldValidator></span>
                                <asp:Label runat="server" ID="lblSupplierCategoryMandatory" AssociatedControlID="chkSupplierCategoryMandatory" Text="Supplier category mandatory?"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkSupplierCategoryMandatory" CssClass="fillspan"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label runat="server" ID="lblSupplierVariationTitle" AssociatedControlID="txtSupplierVariationTitle" Text="Supplier variation title"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtSupplierVariationTitle" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator runat="server" ID="reqSupplierVariationTitle" ControlToValidate="txtSupplierVariationTitle" Text="*" ErrorMessage="Supplier variation title is mandatory" ValidationGroup="vgMain"></asp:RequiredFieldValidator></span>
                                <asp:Label runat="server" ID="lblSupplierStatusMandatory" AssociatedControlID="chkSupplierStatusMandatory" Text="Supplier status mandatory?"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkSupplierStatusMandatory" CssClass="fillspan"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label runat="server" ID="lblSupplierTurnoverEnabled" AssociatedControlID="chkSupplierTurnoverEnabled" Text="Supplier turnover enabled?"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkSupplierTurnoverEnabled" CssClass="fillspan"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                                <asp:Label runat="server" ID="lblSupplierNumEmployeesEnabled" AssociatedControlID="chkSupplierNumEmployeesEnabled" Text="Supplier number employees field enabled?"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkSupplierNumEmployeesEnabled" CssClass="fillspan"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label runat="server" ID="lblLastFinCheckEnabled" AssociatedControlID="chkLastFinCheckEnabled" Text="Last financial check enabled?"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkLastFinCheckEnabled" CssClass="fillspan"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                                <asp:Label runat="server" ID="lblLastFinStatusEnabled" AssociatedControlID="chkLastFinStatusEnabled" Text="Last financial status enabled?"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkLastFinStatusEnabled" CssClass="fillspan"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label runat="server" ID="lblIntContactEnabled" AssociatedControlID="chkIntContactEnabled" Text="Internal contact field enabled?"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkIntContactEnabled" CssClass="fillspan"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                                <asp:Label runat="server" ID="lblFYEEnabled" AssociatedControlID="chkFYEEnabled" Text="Financial year end enabled?"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkFYEEnabled" CssClass="fillspan"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                        </div>
                    </ContentTemplate>
                </cc1:TabPanel>
                <cc1:TabPanel runat="server" ID="tabTimeOut" HeaderText="Session Timeout">
                    <ContentTemplate>
                        <div class="formpanel">
                            <div class="sectiontitle">Session Timeout Options</div>
                            <div class="twocolumn tabcontent-div">
                                <asp:Label CssClass="mandatory" ID="lblIdleTimeout" runat="server" Text="Minutes of inactivity after which user is logged out*" AssociatedControlID="cmbIdleTimeout"></asp:Label>
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
                                    </asp:DropDownList>
                                </span>
                                <span class="inputicon">&nbsp;</span>
                                <span class="inputtooltipfield">
                                    <asp:Image ID="imgTooltipIdleTimeout" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('E1A2E4B7-0407-4A63-A775-DBE718C0E481', 'sm', this);" /></span>
                                <span class="inputvalidatorfield">
                                    <asp:RequiredFieldValidator ID="reqIdleTimeout" runat="server" ValidationGroup="vgMain" ErrorMessage="Please select minutes of inactivity after which user is logged out." ControlToValidate="cmbIdleTimeout" meta:resourcekey="reqipaddressResource1">*</asp:RequiredFieldValidator>
                                </span>
                            </div>

                            <div class="twocolumn">
                                <asp:Label CssClass="mandatory" ID="Label4" runat="server" Text="Seconds for timeout countdown.*" AssociatedControlID="cmbCountdown"></asp:Label>
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
                        </div>
                    </ContentTemplate>
                </cc1:TabPanel>       
                <cc1:TabPanel runat="server" ID="tabExpedite" HeaderText="Expedite">
                    <ContentTemplate>
                           <div class="formpanel">
                            <div class="sectiontitle">Validation Options</div>
                            <div class="twocolumn">
                                <asp:Label runat="server" ID="lblValidationOptionsReceipt" AssociatedControlID="chkValidationOptionsAllowReceipt" Text="Allow expense items less than the receipt total to pass validation"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkValidationOptionsAllowReceipt" CssClass="fillspan" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip576" onclick="SEL.Tooltip.Show('6CB645D1-886D-4ABA-9C5B-B9DADFA1CBF9', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                        </div>
                    </ContentTemplate>
                </cc1:TabPanel>
            </cc1:TabContainer>
        </div>
        <div id="pgEmailServer" class="subPage" style="display: none;">
            <div class="formpanel formpanel_padding">
                <div class="sectiontitle">Email Server</div>
                <div class="onecolumnsmall">
                    <asp:Label ID="lblTxtEmailServer" runat="server" AssociatedControlID="txtEmailServer" Text="Email server"></asp:Label><span class="inputs"><asp:TextBox ID="txtEmailServer" runat="server"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                </div>
                <div class="twocolumn" id="divExpensesEmailFrom">
                    <asp:RadioButton ID="optserver" runat="server" GroupName="source" meta:resourcekey="optserverResource1" Text="Expenses server" TextAlign="Left" CssClass="fillspan"></asp:RadioButton><asp:RadioButton ID="optclaimant" runat="server" GroupName="source" meta:resourcekey="optclaimantResource1" Text="User's email address" TextAlign="Left" CssClass="fillspan"></asp:RadioButton><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                </div>
                <div id="divContractEmailFrom">
                    <div class="twocolumn">
                        <asp:Label runat="server" ID="lblEmailFromAddress" AssociatedControlID="txtEmailFromAddress" Text="Email from address"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtEmailFromAddress" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator runat="server" ID="reqEmailFromAddress" ControlToValidate="txtEmailFromAddress" Text="*" ErrorMessage="Email from address is mandatory" ValidationGroup="vgMain" Display="Dynamic"></asp:RequiredFieldValidator><asp:RegularExpressionValidator runat="server" ID="regexEmailFromAddress" ControlToValidate="txtEmailFromAddress" Text="*" ErrorMessage="Invalid email From address entered" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="vgMain" Display="Dynamic"></asp:RegularExpressionValidator></span>
                        <asp:Label runat="server" ID="lblAuditorEmail" AssociatedControlID="txtAuditorEmail" Text="Auditor email address"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtAuditorEmail" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator runat="server" ID="reqAuditorEmail" ControlToValidate="txtAuditorEmail" Text="*" ErrorMessage="Auditor email address is mandatory" ValidationGroup="vgMain" Display="Dynamic"></asp:RequiredFieldValidator><asp:RegularExpressionValidator runat="server" ID="regexAuditorEmail" ControlToValidate="txtAuditorEmail" Text="*" ErrorMessage="Invalid auditor email address entered. Separate multiple emails only with a semicolon (;)" ValidationExpression="((\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)|(['\&quot;][^\<\>'\&quot;]*['\&quot;]\s*\<\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*\>))(;\s*((\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)|(['\&quot;][^\<\>'\&quot;]*['\&quot;]\s*\<\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*\>)))*" ValidationGroup="vgMain" Display="Dynamic"></asp:RegularExpressionValidator></span>
                    </div>
                    <div class="twocolumn">
                        <asp:Label runat="server" ID="lblErrorEmailSubmitAddress" AssociatedControlID="txtErrorEmailSubmitAddress" Text="Error submit email address"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtErrorEmailSubmitAddress" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator runat="server" ID="reqErrorEmailSubmitAddress" ControlToValidate="txtErrorEmailSubmitAddress" Text="*" ErrorMessage="Error email submit address is mandatory" ValidationGroup="vgMain" Display="Dynamic"></asp:RequiredFieldValidator><asp:RegularExpressionValidator runat="server" ID="regexErrorEmailSubmitAddress" ControlToValidate="txtErrorEmailSubmitAddress" Text="*" ErrorMessage="Invalid error email submit address entered" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="vgMain" Display="Dynamic"></asp:RegularExpressionValidator></span>
                        <asp:Label runat="server" ID="lblErrorSubmitFromAddress" AssociatedControlID="txtErrorSubmitFromAddress" Text="Error submit from email address"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtErrorSubmitFromAddress" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator runat="server" ID="reqErrorSubmitFromAddress" ControlToValidate="txtErrorSubmitFromAddress" Text="*" ErrorMessage="Error email submit from address is mandatory" ValidationGroup="vgMain" Display="Dynamic"></asp:RequiredFieldValidator><asp:RegularExpressionValidator runat="server" ID="regexErrorSubmitFromAddress" ControlToValidate="txtErrorSubmitFromAddress" Text="*" ErrorMessage="Invalid error email submit from address entered" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="vgMain" Display="Dynamic"></asp:RegularExpressionValidator></span>
                    </div>
                </div>
                <div class="twocolumn">
                    <asp:Label runat="server" ID="lblEmailAdministrator" AssociatedControlID="txtEmailAdministrator" Text="Email administrator address"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtEmailAdministrator" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator runat="server" ID="reqEmailAdministrator" ControlToValidate="txtEmailAdministrator" Text="*" ErrorMessage="Email administrator address is mandatory" ValidationGroup="vgMain" Display="Dynamic"></asp:RequiredFieldValidator><asp:RegularExpressionValidator runat="server" ID="regexEmailAdministrator" ControlToValidate="txtEmailAdministrator" Text="*" ErrorMessage="Invalid email administrator address entered" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="vgMain" Display="Dynamic"></asp:RegularExpressionValidator></span>
                </div>
            </div>
        </div>
        <div id="pgMainAdministrator" class="subPage" style="display: none;">
            <div class="formpanel formpanel_padding">
                <div class="sectiontitle">
                    Main Administrator
                </div>
                <div class="onecolumnsmall">
                    <asp:Label ID="lblMainAdministrator" runat="server" AssociatedControlID="ddlMainAdministrator" Text="Main administrator"></asp:Label><span class="inputs"><asp:DropDownList ID="ddlMainAdministrator" runat="server"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                </div>
            </div>
        </div>
        <div id="pgRegionalSettings" class="subPage" style="display: none;">
            <div class="formpanel formpanel_padding">
                <div class="sectiontitle">Regional Settings</div>
                <div class="onecolumnsmall">
                    <asp:Label ID="lblDefaultCountry" runat="server" Text="Default country" AssociatedControlID="ddlDefaultCountry"></asp:Label><span class="inputs"><asp:DropDownList ID="ddlDefaultCountry" runat="server"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                </div>
                <div class="onecolumnsmall">
                    <asp:Label ID="lblBaseCurrency" runat="server" Text="Base currency" AssociatedControlID="ddlBaseCurrency"></asp:Label><span class="inputs"><asp:DropDownList ID="ddlBaseCurrency" runat="server"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                </div>
                <div class="onecolumnsmall">
                    <asp:Label ID="lblDefaultLanguage" runat="server" Text="Default language" AssociatedControlID="ddlDefaultLanguage"></asp:Label><span class="inputs"><asp:DropDownList ID="ddlDefaultLanguage" runat="server"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                </div>
                <div class="onecolumnsmall">
                    <asp:Label ID="lblDefaultLocale" runat="server" Text="Default locale" AssociatedControlID="ddlDefaultLocale"></asp:Label><span class="inputs"><asp:DropDownList ID="ddlDefaultLocale" runat="server"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                </div>
            </div>
        </div>
        <div id="pgPasswordSettings" class="subPage" style="display: none;">
            <div class="formpanel formpanel_padding">
                <div class="sectiontitle">Password Settings</div>
                <div class="twocolumn">
                    <asp:Label ID="lblattempts" runat="server" meta:resourcekey="lblattemptsResource1" AssociatedControlID="txtattempts">Number of attempts before lock-out</asp:Label><span class="inputs"><asp:TextBox ID="txtattempts" MaxLength="3" runat="server" meta:resourcekey="txtattemptsResource1"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip181" onclick="SEL.Tooltip.Show('511b6f6d-871a-46a4-bb5c-7ade5ac924a0', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqAttempts" runat="server" Text="*" ControlToValidate="txtattempts" ErrorMessage="Please enter a value into the Number of attempts before lock-out box" ValidationGroup="vgMain"></asp:RequiredFieldValidator><asp:CompareValidator ID="compattempts" runat="server" ErrorMessage="Please enter a valid value into the Number of attempts before lock-out box" ControlToValidate="txtattempts" Operator="DataTypeCheck" Type="Integer" ValidationGroup="vgMain" meta:resourcekey="compattemptsResource1">*</asp:CompareValidator><asp:CompareValidator ID="compAttemptsGreater" runat="server" ErrorMessage="Cannot enter negative values into Number of attempts before lock-out box" ControlToValidate="txtattempts" Operator="GreaterThanEqual" ValueToCompare="0" Type="Integer" ValidationGroup="vgMain" meta:resourcekey="compAttemptsGreaterResource1">*</asp:CompareValidator><asp:CompareValidator ID="compLessThanAttempts" runat="server" ErrorMessage="Please enter a value less than or equal to 255 into Number of attempts before lock-out box" ControlToValidate="txtattempts" Operator="LessThanEqual" ValueToCompare="255" Type="Integer" ValidationGroup="vgMain" meta:resourcekey="compAttemptsLessThanResource1">*</asp:CompareValidator></span><asp:Label ID="lblexpires" runat="server" meta:resourcekey="lblexpiresResource1" AssociatedControlID="txtexpires">Password expires after x days (0 never expires)</asp:Label><span class="inputs"><asp:TextBox ID="txtexpires" runat="server" MaxLength="3" meta:resourcekey="txtexpiresResource1"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip182" onclick="SEL.Tooltip.Show('2ab50508-0cd9-4d21-b4fe-aa156c1e1302', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqExpires" runat="server" Text="*" ControlToValidate="txtexpires" ValidationGroup="vgMain" ErrorMessage="Please enter a value into the Password expires after x days box"></asp:RequiredFieldValidator><asp:CompareValidator ID="compexpires" runat="server" ErrorMessage="Please enter a valid value for Password expires after x days" ControlToValidate="txtexpires" Operator="DataTypeCheck" Type="Integer" ValidationGroup="vgMain" meta:resourcekey="compexpiresResource1">*</asp:CompareValidator><asp:CompareValidator ID="compExpiresGreater" runat="server" ErrorMessage="Cannot enter negative values into Password expires after x days box" ControlToValidate="txtexpires" Operator="GreaterThanEqual" ValueToCompare="0" Type="Integer" ValidationGroup="vgMain" meta:resourcekey="compExpiresGreaterResource1">*</asp:CompareValidator></span>
                </div>
                <div class="onecolumnsmall">
                    <asp:Label ID="lbllength" runat="server" meta:resourcekey="lbllengthResource1" AssociatedControlID="cmblength">Password length</asp:Label><span class="inputs"><asp:DropDownList ID="cmblength" runat="server" meta:resourcekey="cmblengthResource1" onchange="passOptChange();">
                        <asp:ListItem Value="1" meta:resourcekey="ListItemResource1">Password can be any length</asp:ListItem>
                        <asp:ListItem Value="2" meta:resourcekey="ListItemResource2">Password must be equal to</asp:ListItem>
                        <asp:ListItem Value="3" meta:resourcekey="ListItemResource3">Password must be greater than</asp:ListItem>
                        <asp:ListItem Value="4" meta:resourcekey="ListItemResource4">Password must be less than</asp:ListItem>
                        <asp:ListItem Value="5" meta:resourcekey="ListItemResource5">Password must be between</asp:ListItem>
                    </asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                </div>
                <div class="twocolumn">
                    <div id="plength1" runat="server">
                        <asp:Label ID="lblMinimumPasswordLength" runat="server" Text="Length" AssociatedControlID="txtlength1"></asp:Label><span class="inputs"><asp:TextBox ID="txtlength1" MaxLength="3" runat="server" meta:resourcekey="txtlength1Resource1"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqLength1" runat="server" ControlToValidate="txtlength1" Text="*" ErrorMessage="Please enter a value for the Minimum password length" ValidationGroup="vgMain" Enabled="false"></asp:RequiredFieldValidator><asp:CompareValidator ID="complength1" runat="server" ErrorMessage="Please enter a valid value for the Minimum password length" ControlToValidate="txtlength1" Operator="DataTypeCheck" Type="Integer" ValidationGroup="vgMain" Enabled="false" meta:resourcekey="complength1Resource1">*</asp:CompareValidator><asp:CompareValidator ID="compLength1Greater" runat="server" ErrorMessage="Cannot enter negative values into Minimum password length" ControlToValidate="txtlength1" Operator="GreaterThanEqual" ValueToCompare="0" Type="Integer" ValidationGroup="vgMain" Enabled="false" meta:resourcekey="compAttemptsGreaterResource1">*</asp:CompareValidator><asp:CompareValidator ID="compLength1LessThan" runat="server" ErrorMessage="Please enter a value less than 250 into Minimum password length" ControlToValidate="txtlength1" Operator="LessThan" ValueToCompare="250" Type="Integer" ValidationGroup="vgMain" Enabled="false" meta:resourcekey="compAttemptsLessThanResource1">*</asp:CompareValidator><asp:CompareValidator ID="compMinLess" runat="server" ErrorMessage="The Minimum password length must be less than the Maximum password length" Text="*" ControlToValidate="txtlength1" ValidationGroup="vgMain" Operator="LessThan" ControlToCompare="txtlength2" Type="Integer" Enabled="false"></asp:CompareValidator></span></div><div id="plength2" runat="server"><asp:Label ID="lblMaximumPasswordLength" runat="server" meta:resourcekey="lblandResource1" Text="Maximum length" AssociatedControlID="txtlength2"></asp:Label><span class="inputs"><asp:TextBox ID="txtlength2" MaxLength="3" runat="server" meta:resourcekey="txtlength2Resource1"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip183" onclick="SEL.Tooltip.Show('991f1c14-b0b1-4e90-a11d-3036b12d6c6b', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqLength2" runat="server" ControlToValidate="txtlength2" Text="*" ErrorMessage="Please enter a value for the Maximum password length" ValidationGroup="vgMain" Enabled="false"></asp:RequiredFieldValidator><asp:CompareValidator ID="complength2" runat="server" ErrorMessage="Please enter a valid value for the Maximum password length" ControlToValidate="txtlength2" Operator="DataTypeCheck" Type="Integer" ValidationGroup="vgMain" Enabled="false" meta:resourcekey="complength2Resource1">*</asp:CompareValidator><asp:CompareValidator ID="compLength2Greater" runat="server" ErrorMessage="Cannot enter negative values into Maximum password length" ControlToValidate="txtlength2" Operator="GreaterThanEqual" ValueToCompare="0" Type="Integer" ValidationGroup="vgMain" Enabled="false" meta:resourcekey="compLength2GreaterResource1">*</asp:CompareValidator><asp:CompareValidator ID="compLength2LessThan" runat="server" ErrorMessage="Please enter a value less than or equal to 250 into Maximum password length" ControlToValidate="txtlength2" Operator="LessThanEqual" ValueToCompare="250" Type="Integer" ValidationGroup="vgMain" Enabled="false" meta:resourcekey="compLength2LessThanResource1">*</asp:CompareValidator><asp:CompareValidator ID="compMaxGreater" runat="server" ErrorMessage="The Maximum password length must be greater than the Minimum password length" Text="*" ControlToValidate="txtlength2" ValidationGroup="vgMain" Operator="GreaterThan" ControlToCompare="txtlength1" Type="Integer" Enabled="false"></asp:CompareValidator></span></div>
                </div>
                <div class="twocolumn">
                    <asp:Label ID="lblupper" runat="server" meta:resourcekey="lblupperResource1" AssociatedControlID="chkupper">Password must contain upper case letters</asp:Label><span class="inputs"><asp:CheckBox ID="chkupper" runat="server" meta:resourcekey="chkupperResource1"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip186" onclick="SEL.Tooltip.Show('70e3271f-308c-49be-bb65-95b2f556e6e4', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblnumbers" runat="server" meta:resourcekey="lblnumbersResource1" AssociatedControlID="chknumbers">Password must contain numbers</asp:Label><span class="inputs"><asp:CheckBox ID="chknumbers" runat="server" meta:resourcekey="chknumbersResource1"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip187" onclick="SEL.Tooltip.Show('01af4e5d-461b-4a97-bc9a-51627d023a1e', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span>
                </div>
                <div class="twocolumn">
                    <asp:Label runat="server" ID="lblsymbol" AssociatedControlID="chksymbol">Password must contain a symbol character</asp:Label><span class="inputs"><asp:CheckBox ID="chksymbol" runat="server"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip482" onclick="SEL.Tooltip.Show('2f452cf6-b37f-4619-8a36-04edaf13984b', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span>
                </div>
                <div class="twocolumn">
                    <asp:Label ID="lblprevious" runat="server" meta:resourcekey="lblpreviousResource1" AssociatedControlID="txtprevious">Previous passwords not permitted</asp:Label><span class="inputs"><asp:TextBox ID="txtprevious" MaxLength="3" runat="server" meta:resourcekey="txtpreviousResource1"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip188" onclick="SEL.Tooltip.Show('7ca879c5-56c1-45f6-b7d0-ebbe5e17ebfe', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqPrev" runat="server" ControlToValidate="txtprevious" Text="*" ErrorMessage="Please enter a value for the Num previous passwords" ValidationGroup="vgMain"></asp:RequiredFieldValidator><asp:CompareValidator ID="comprevious" runat="server" ErrorMessage="Please enter a valid value for Num previous passwords" ControlToValidate="txtprevious" Operator="DataTypeCheck" Type="Integer" ValidationGroup="vgMain" meta:resourcekey="compreviousResource1">*</asp:CompareValidator><asp:CompareValidator ID="compPrevGreater" runat="server" ErrorMessage="Cannot enter negative values into Num previous passwords box" ControlToValidate="txtprevious" Operator="GreaterThanEqual" ValueToCompare="0" Type="Integer" ValidationGroup="vgMain" meta:resourcekey="compAttemptsGreaterResource1">*</asp:CompareValidator></span>
                </div>
                <div class="sectiontitle">Failed Logon Message Options</div>
                <div class="onecolumnsmall">
                    <asp:Label ID="lblaccountlocked" runat="server" meta:resourcekey="lblpreviousResource1" AssociatedControlID="txtaccountlocked">Account locked</asp:Label><span class="inputs"><asp:TextBox ID="txtaccountlocked" MaxLength="100" runat="server" ></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip188" onclick="SEL.Tooltip.Show('7BC49771-2DB1-4F71-A5EA-AA331C10345D', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"></span>
                </div>
                <div class="onecolumnsmall">
                    <asp:Label ID="lblaccountcurrentlylocked" runat="server" meta:resourcekey="lblpreviousResource1" AssociatedControlID="txtcurrentlylocked">Account currently locked</asp:Label><span class="inputs"> <asp:TextBox ID="txtcurrentlylocked" MaxLength="100" runat="server" ></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip188" onclick="SEL.Tooltip.Show('EAD529EF-1298-48FF-A8E7-89CC38319E5A', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span>
                </div>
            </div>
        </div>

        <div id="pgNewExpenses" class="subPage" style="display: none;">
            <cc1:TabContainer ID="tabsNewExpenses" runat="server">
                <cc1:TabPanel ID="tabAddScreen" runat="server" HeaderText="Field Settings">
                    <ContentTemplate>
                        <div class="formpanel">
                            <asp:Literal ID="litAddScreenTable" runat="server"></asp:Literal>
                        </div>
                    </ContentTemplate>
                </cc1:TabPanel>
                <cc1:TabPanel ID="tabcodeallocation" runat="server" HeaderText="Code Allocation">
                    <HeaderTemplate>Code Allocation</HeaderTemplate>
                    <ContentTemplate>


                        <div class="formpanel">
                            <div class="twocolumn tabcontent-div">
                                <asp:Label ID="lblcostcodeson" runat="server" meta:resourcekey="lblcostcodesonResource1" AssociatedControlID="chkcostcodeson">Items should be assigned to cost codes</asp:Label><span class="inputs"><asp:CheckBox ID="chkcostcodeson" runat="server" meta:resourcekey="chkcostcodesonResource1"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip1" onclick="SEL.Tooltip.Show('6e7ecd22-b0ea-4c04-a769-bf27151ce4b4', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblcostcodes" runat="server" meta:resourcekey="lblcostcodesResource1" AssociatedControlID="chkcostcodes">Claimants should be shown their cost code breakdown</asp:Label><span class="inputs"><asp:CheckBox ID="chkcostcodes" runat="server" TextAlign="Left" meta:resourcekey="chkcostcodesResource1"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip325" onclick="SEL.Tooltip.Show('5aa0f5e4-7a1e-4fa1-88a7-ce0b010f7863', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn tabcontent-div">
                                <asp:Label ID="lblcostcodesdesc" runat="server" meta:resourcekey="lblcostcodesdescResource1" AssociatedControlID="chkcostcodedesc">Claimants should be shown the cost code's description</asp:Label><span class="inputs"><asp:CheckBox ID="chkcostcodedesc" runat="server" TextAlign="Left" meta:resourcekey="chkcostcodedescResource1"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip4" onclick="SEL.Tooltip.Show('61108f13-e8c0-48e6-909c-f724a95e712e', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblcostcodeongendet" runat="server" meta:resourcekey="lblcostcodeongendetResource1" AssociatedControlID="chkcostcodeongendet">Costcodes are shown in general details</asp:Label><span class="inputs"><asp:CheckBox ID="chkcostcodeongendet" runat="server" meta:resourcekey="chkcostcodeongendetResource1"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip333" onclick="SEL.Tooltip.Show('ac9df55a-16de-4206-9ff9-916494a484ca', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn tabcontent-div">
                                <asp:Label ID="lbldepartmentson" runat="server" meta:resourcekey="lbldepartmentsonResource1" AssociatedControlID="chkdepartmentson">Items should be assigned to department codes</asp:Label><span class="inputs"><asp:CheckBox ID="chkdepartmentson" runat="server" meta:resourcekey="chkdepartmentsonResource1"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip5" onclick="SEL.Tooltip.Show('24367c15-1bb5-4c37-8929-5ca97697bf27', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="Label1" runat="server" meta:resourcekey="lbldepartmentResource1" AssociatedControlID="chkdepartment">Claimants should be shown their department breakdown</asp:Label><span class="inputs"><asp:CheckBox ID="chkdepartment" runat="server" meta:resourcekey="chkdepartmentResource1"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip326" onclick="SEL.Tooltip.Show('b9a6c92b-c43e-472a-86b0-fa20046cd235', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn tabcontent-div">
                                <asp:Label ID="lbldepartmentdesc" runat="server" meta:resourcekey="lbldepartmentdescResource1" AssociatedControlID="chkdepartmentdesc">Claimants should be shown the department's description</asp:Label><span class="inputs"><asp:CheckBox ID="chkdepartmentdesc" runat="server" TextAlign="Left" meta:resourcekey="chkdepartmentdescResource1"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip6" onclick="SEL.Tooltip.Show('27dd8d5b-29c8-4256-b31b-967d4f62dd1a', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lbldepartmentongendet" runat="server" meta:resourcekey="lbldepartmentongendetResource1" AssociatedControlID="chkdepartmentongendet">Departments are shown in general details</asp:Label><span class="inputs"><asp:CheckBox ID="chkdepartmentongendet" runat="server" TextAlign="Left" meta:resourcekey="chkdepartmentongendetResource1"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip334" onclick="SEL.Tooltip.Show('42a3ebcb-fa65-4129-a3ec-11493f2ebaac', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn tabcontent-div">
                                <asp:Label ID="lblprojectcodeson" runat="server" meta:resourcekey="lblprojectcodesonResource1" AssociatedControlID="chkprojectcodeson">Items should be assigned to project codes</asp:Label><span class="inputs"><asp:CheckBox ID="chkprojectcodeson" runat="server" meta:resourcekey="chkprojectcodesonResource1"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip291" onclick="SEL.Tooltip.Show('197788b1-cb9f-43aa-8df2-5ebe7786fba0', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblprojectcodes" runat="server" meta:resourcekey="lblprojectcodesResource1" AssociatedControlID="chkprojectcodes">Claimants should be shown their project code breakdown</asp:Label><span class="inputs"><asp:CheckBox ID="chkprojectcodes" runat="server" meta:resourcekey="chkprojectcodesResource1"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip327" onclick="SEL.Tooltip.Show('74d7271c-d006-43db-9bfd-d0b51219ba0f', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn tabcontent-div">
                                <asp:Label ID="lblprojectcodedesc" runat="server" meta:resourcekey="lblprojectcodedescResource1" AssociatedControlID="chkprojectcodedesc">Claimants should be shown the project code's description</asp:Label><span class="inputs"><asp:CheckBox ID="chkprojectcodedesc" runat="server" meta:resourcekey="chkprojectcodedescResource1"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip292" onclick="SEL.Tooltip.Show('f48f33ed-791e-4a58-acd7-f19f0eaf0d7f', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblprojectcodeongendet" runat="server" meta:resourcekey="lblprojectcodeongendetResource1" AssociatedControlID="chkprojectcodeongendet">Project codes are shown in general details</asp:Label><span class="inputs"><asp:CheckBox ID="chkprojectcodeongendet" runat="server" TextAlign="Left" meta:resourcekey="chkprojectcodeongendetResource1"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip335" onclick="SEL.Tooltip.Show('7d62cf3e-13e8-40f6-a9e8-8227cc107023', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn tabcontent-div">
                                <asp:Label ID="lblemployeeallocation" runat="server" AssociatedControlID="chkautoassignallocation" Text="Use default allocation if employee has none set" meta:resourcekey="lblemployeeallocationResource1"></asp:Label><span class="inputs"><asp:CheckBox ID="chkautoassignallocation" runat="server" meta:resourcekey="chkautoassignallocationResource1"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip336" onclick="SEL.Tooltip.Show('37d1879e-628c-4eb9-82eb-f717817b8b6d', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span><asp:Label runat="server" ID="lbldefaultcostcodeowner" AssociatedControlID="txtdefaultcostcodeowner" meta:resourcekey="lbldefaultcostcodeownerResource1">Default Cost Code Owner</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtdefaultcostcodeowner" CssClass="fillspan"></asp:TextBox><asp:TextBox runat="server" ID="txtdefaultcostcodeowner_ID" Text="-1" Style="display: none;"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip591" onclick="SEL.Tooltip.Show('10d85fe0-006f-4a42-91cd-7a6d7ac7b773', 'ex', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                        </div>
                    </ContentTemplate>
                </cc1:TabPanel>
                <cc1:TabPanel runat="server" ID="tabdutyofcare" HeaderText="Duty of Care">
                    <ContentTemplate>
                        <div class="formpanel">
                            <div id="dvlaLicenceElementSection" runat="server">
                            <div class="sectiontitle">Automatic Driver & Vehicle Check</div>
                            <div class="comment" id="drivingLicencelookupInstructions">
                            <span>There is a cost associated to the frequency of the Automatic Driver & Vehicle Checks. Changing the frequency will affect the amount of credits that you use.</span></div>
                            <div class="twocolumn">
                                    <asp:Label ID="lblDrivingLicenceLookup" runat="server" meta:resourcekey="lblDrivingLicenceLookupResource1" AssociatedControlID="chkDrivingLicenceLookup">Check claimant driving licences automatically</asp:Label>
                                    <span class="inputs">
                                        <asp:CheckBox ID="chkDrivingLicenceLookup" runat="server" onclick="hideAutomaticDrivingLicenceLookup(this)" meta:resourcekey="chkDrivingLicenceLookupResource1" />
                                    </span>
                                    <span class="inputicon">&nbsp;</span>
                                    <span class="inputtooltipfield">
                                        <img id="imgtooltip430" onclick="SEL.Tooltip.Show('0DBFFD92-2A31-476D-BCB7-CD7AEC5CDF6C', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" />
                                    </span>
                                    <span class="inputvalidatorfield">&nbsp;</span>
                                   
                              <span id="automaticDrivingLicenceLookupFrequency">
                                <asp:label runat="server" ClientIDMode="Static" ID="lblDrivingLicenceLookupFrequency" CssClass="automaticDrivingLicenceFrequency" AssociatedControlID ="ddlDrivingLicenceLookupFrequency" Text="Frequency of automatic driving licence checks"></asp:label>
                                <span class="inputs">
                                    <asp:DropDownList CssClass="fillspan" ID="ddlDrivingLicenceLookupFrequency" runat="server">
                                        <asp:ListItem Text="Every 1 month" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="Every 3 months" Value="3"></asp:ListItem>
                                        <asp:ListItem Text="Every 6 months" Value="6"></asp:ListItem>
                                        <asp:ListItem Selected="True" Text="Every 12 months" Value="12"></asp:ListItem>
                                    </asp:DropDownList>
                                    </span>
                                <span class="inputicon">&nbsp;</span>
                                    <span class="inputtooltipfield">
                                <img class="tooltipicon" onclick="SEL.Tooltip.Show('48711479-BF2C-4115-B783-2512BF3FF112', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" /></span>
                                    <span class="inputvalidatorfield">&nbsp;</span>
                               </span>
                                </div>
                             <div id="automaticAutoRevokeOfConsentLookupFrequency" class="twocolumn">
                                <asp:label runat="server" ClientIDMode="Static" ID="lblAutoRevokeOfConsentLookUpFrequency" AssociatedControlID="ddlAutoRevokeOfConsentLookupFrequency">Number of weeks before consent expiry to send notification</asp:label>
                                <span class="inputs">
                                    <asp:DropDownList CssClass="fillspan" ID="ddlAutoRevokeOfConsentLookupFrequency" runat="server">
                                        <asp:ListItem Text="1 Week" Value="7"></asp:ListItem>
                                        <asp:ListItem Text="2 Weeks" Value="14"></asp:ListItem>
                                        <asp:ListItem Text="4 Weeks" Value="28"></asp:ListItem>
                                    </asp:DropDownList>
                                    </span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img class="tooltipicon" onclick="SEL.Tooltip.Show('E5BF448A-8F0F-4D28-9391-732FED2BF0F3', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" /></span><span class="inputvalidatorfield">&nbsp;</span>
                                 </div>
                            </div>
                            <div class="sectiontitle">Approver</div>
                            <div class="twocolumn">
                                <asp:Label ID="lblLineManagerAsApprover" Text="Line manager" runat="server" AssociatedControlID="lineManagerAsApprover"></asp:Label>
                                <span class="inputs">
                                    <asp:RadioButton ID="lineManagerAsApprover" runat="server" GroupName="selectApprover" CssClass="fillspan" TextAlign="Left"></asp:RadioButton>
                                </span>
                                <span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                                <asp:Label ID="lblteamAsApprover" Text="Team" runat="server" AssociatedControlID="teamAsApprover"></asp:Label><span class="inputs"><asp:RadioButton ID="teamAsApprover" runat="server" GroupName="selectApprover" CssClass="fillspan" TextAlign="Left"></asp:RadioButton></span>
                                <span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>                          
                            </div>
                            <div runat="server" id="dutyOfCareApproverTeamSection" style="display:none;" class="twocolumn dutyOfCareApproverTeamSection">
                                <asp:Label ID="teamListHeader" runat="server" Text="Select team" AssociatedControlID="teamListForApprover"></asp:Label>
                                <span class="inputs">
                                    <asp:DropDownList id="teamListForApprover" runat="server" meta:resourcekey="cmblistResource1"></asp:DropDownList></span>
                                <span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span>
                                <span class="inputvalidatorfield">&nbsp;</span>
                            </div> 
                             <div class="sectiontitle">Documents Required</div>
                            <div class="twocolumn">
                               <asp:Label ID="lbldrivinglicence" runat="server" Text="Driving licence" meta:resourcekey="lbldrivinglicenceResource1" AssociatedControlID="chkblockdrivinglicence"></asp:Label><span class="inputs"><asp:CheckBox ID="chkblockdrivinglicence" onclick="hideDrivingLicenceFrequencyPanel(null, this)" runat="server" meta:resourcekey="chkblockdrivinglicenceResource1" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                                <asp:Label ID="lbltaxexpiry" runat="server" Text="Tax" meta:resourcekey="lbltaxexpiryResource1" AssociatedControlID="chkblocktaxexpiry"></asp:Label><span class="inputs"><asp:CheckBox ID="chkblocktaxexpiry" runat="server" meta:resourcekey="chkblocktaxexpiryResource1" onclick="hideAutomaticVehicleDocumentLookup(this)"/></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label ID="lblmotexpiry" runat="server" Text="MOT" meta:resourcekey="lblmotexpiryResource1" AssociatedControlID="chkblockmotexpiry"></asp:Label><span class="inputs"><asp:CheckBox ID="chkblockmotexpiry" runat="server" meta:resourcekey="chkblockmotexpiryResource1" onclick="hideAutomaticVehicleDocumentLookup(this)"/></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                                 <asp:Label ID="lblinsurancexpiry" runat="server" Text="Insurance" meta:resourcekey="lblinsurancexpiryResource1" AssociatedControlID="chkblockinsuranceexpiry"></asp:Label><span class="inputs"><asp:CheckBox ID="chkblockinsuranceexpiry" runat="server" meta:resourcekey="chkblockinsuranceexpiryResource1" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label ID="lblBreakdownCoverExpiry" runat="server" Text="Breakdown cover" meta:resourcekey="lblbreakdowncoverResource1" AssociatedControlID="chkblockbreakdowncoverexpiry"></asp:Label><span class="inputs"><asp:CheckBox ID="chkBlockBreakdownCoverExpiry" runat="server" meta:resourcekey="chkblockbreakdowncoverexpiryResource1" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                                <span runat="server" ID="VehicleLookupEnabled"><span runat="server" ID="spanVehicleDocumentLookups" ><asp:Label ID="lblVehicleDocumentLookup" AssociatedControlID="chkVehicleDocumentLookup" Text="Enable automatic document lookup" runat="server"></asp:Label><span class="inputs"><asp:CheckBox ID="chkVehicleDocumentLookup" runat="server"/></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip337" onclick="SEL.Tooltip.Show('D91BC012-825A-4357-BF8D-FE1FB9D5A4EF', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon"/></span><span class="inputvalidatorfield">&nbsp;</span></span></span>
                            </div>
                              <div class="sectiontitle">Email Reminders for Document Expiry</div>
                            <div class="twocolumn">
                                <asp:Label ID="lblClaimantReminder" runat="server" Text="Send reminder to claimant before " meta:resourcekey="lblclaimantremainderResource1" AssociatedControlID="cmbClaimantReminderDays"></asp:Label>
                                <span class="inputs">
                                    <asp:DropDownList ID="cmbClaimantReminderDays" runat="server">
                                        <asp:ListItem Value="-1" meta:resourcekey="ListItemResource1">Do not send reminder</asp:ListItem>
                                        <asp:ListItem Value="7" meta:resourcekey="ListItemResource2">7 Days</asp:ListItem>
                                        <asp:ListItem Value="14" meta:resourcekey="ListItemResource3">14 Days</asp:ListItem>
                                        <asp:ListItem Value="28" meta:resourcekey="ListItemResource3">28 Days</asp:ListItem>
                                    </asp:DropDownList></span>
                                <span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span>
                                <span class="inputvalidatorfield">&nbsp;</span>

                                <asp:Label ID="lblLineManagerReminder" runat="server" Text="Days prior to document expiry to send a reminder " meta:resourcekey="lbllinemanagerremainderResource1" AssociatedControlID="cmbLineManagerReminderDays"></asp:Label>
                                <span class="inputs">
                                    <asp:DropDownList ID="cmbLineManagerReminderDays" runat="server">
                                       <asp:ListItem Value="-1" meta:resourcekey="ListItemResource1">Do not send reminder</asp:ListItem>
                                        <asp:ListItem Value="7" meta:resourcekey="ListItemResource2">7 Days</asp:ListItem>
                                        <asp:ListItem Value="14" meta:resourcekey="ListItemResource3">14 Days</asp:ListItem>
                                        <asp:ListItem Value="28" meta:resourcekey="ListItemResource3">28 Days</asp:ListItem>
                                    </asp:DropDownList></span>
                                <span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span>
                                <span class="inputvalidatorfield">&nbsp;</span>
                                </div> 
                              <span id="DrivingLicenceReviewWrapper">
                            <div class="sectiontitle">Review Expiry & Reminder</div>
                            <div class="twocolumn">
                                <asp:Label ID="lblReviewPeriodically" runat="server" Text="Driving licence should be reviewed periodically" meta:resourcekey="lblReviewPeriodically" AssociatedControlID="chkLicenceReview"></asp:Label><span class="inputs"><asp:CheckBox ID="chkLicenceReview" runat="server" meta:resourcekey="chkLicenceReview" onclick="hideDrivingLicenceFrequencyPanel(this, null)" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><asp:Image runat="server" AlternateText="Tooltip" CssClass="tooltipicon" onclick="SEL.Tooltip.Show('BDD23BC2-E146-4DA7-AECE-B75784F64927', 'sm', this);" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" /></span><span class="inputvalidatorfield">&nbsp;</span>
                                <span class="DrivingLicenceReviewFrequency">
                                <asp:Label ID="lblReviewFrequency" runat="server" Text="Driving licence must be reviewed" meta:resourcekey="lblReviewFrequency" AssociatedControlID="cmbReviewFrequencyDays"></asp:Label>
                                <span class="inputs">
                                    <asp:DropDownList ID="cmbReviewFrequencyDays" runat="server">
                                        <asp:ListItem Value="1" meta:resourcekey="ListItemReviewFrequencyResource1">Every 1 month</asp:ListItem>
                                        <asp:ListItem Value="3" meta:resourcekey="ListItemReviewFrequencyResource2">Every 3 months</asp:ListItem>
                                        <asp:ListItem Value="6" meta:resourcekey="ListItemReviewFrequencyResource3">Every 6 months</asp:ListItem>
                                        <asp:ListItem Value="12" meta:resourcekey="ListItemReviewFrequencyResource3">Every 12 months</asp:ListItem>
                                    </asp:DropDownList></span>
                                <span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><asp:Image runat="server" AlternateText="Tooltip" CssClass="tooltipicon" onclick="SEL.Tooltip.Show('4277DA52-0284-4F89-ABCF-B370DF0409D6', 'sm', this);" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" /></span>
                                <span class="inputvalidatorfield">&nbsp;</span>
                                </span>
                                </div>                         
                                  <div class="twocolumn">  
                                      <span class="DrivingLicenceReviewFrequency">
                                      <asp:Label ID="lblReminderReviews" runat="server" Text="Send reminder to claimant prior to the review expiry date" meta:resourcekey="lblReminderReviews" AssociatedControlID="chkLicenceReviewReminderNotification"></asp:Label><span class="inputs"><asp:CheckBox ID="chkLicenceReviewReminderNotification" runat="server" meta:resourcekey="chkLicenceReviewReminderNotification" onclick="hideDrivingLicenceReviewReminderDaysPanel(this)" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><asp:Image runat="server" AlternateText="Tooltip" CssClass="tooltipicon" onclick="SEL.Tooltip.Show('2C41F70E-CE77-4202-AD41-F6FCDF2E1D51', 'sm', this);" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" /></span><span class="inputvalidatorfield">&nbsp;</span>                                      
                                      <span class="DrivingLicenceReviewReminderDays">
                                          <asp:Label ID="lblReminderReviewsDays" runat="server" Text="Days prior to review expiry to send a reminder " meta:resourcekey="lblReminderReviewsDays" AssociatedControlID="cmbReminderReviewsDays"></asp:Label>
                                          <span class="inputs">
                                          <asp:DropDownList ID="cmbReminderReviewsDays" runat="server">
                                              <asp:ListItem Value="7" meta:resourcekey="ListItemReminderReviewsDaysResource1">7 days</asp:ListItem>
                                              <asp:ListItem Value="14" meta:resourcekey="ListItemReminderReviewsDaysResource2">14 days</asp:ListItem>
                                              <asp:ListItem Value="28" meta:resourcekey="ListItemReminderReviewsDaysResource3">28 days</asp:ListItem>
                                          </asp:DropDownList></span>  
                                      <span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><asp:Image runat="server" AlternateText="Tooltip" CssClass="tooltipicon" onclick="SEL.Tooltip.Show('99B38FC7-EC21-49B1-8D22-E051AE10F956', 'sm', this);" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" /></span>
                                      <span class="inputvalidatorfield">&nbsp;</span>
                                      </span>
                                      </span>
                                      </div>
                              </span>
                            <div class="sectiontitle">Claim Options</div>
                            <div class="twocolumn">
                                <asp:Label ID="lblDutyOfCareDateOfExpenseCheck" runat="server" Text="Use date of expense for duty of care checks" meta:resourcekey="lblDutyOfCareDateOfExpenseCheckResource1" AssociatedControlID="chkDutyOfCareDateOfExpenseCheck"></asp:Label><span class="inputs"><asp:CheckBox ID="chkDutyOfCareDateOfExpenseCheck" runat="server" meta:resourcekey="chkDutyOfCareExpenseDateCheckResource1" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><asp:Image runat="server" AlternateText="Tooltip" CssClass="tooltipicon" onclick="SEL.Tooltip.Show('D7193C60-BE47-4E6B-9CB5-5D235787DC06', 'sm', this);" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" /></span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                        </div>
                    </ContentTemplate>
                </cc1:TabPanel>
                <cc1:TabPanel runat="server" ID="tabmileage" HeaderText="Addresses &amp; Distances">
                    <ContentTemplate>
                        <div class="formpanel">
                            <div class="sectiontitle">Addresses</div>
                            <div class="twocolumn">
                                <asp:Label ID="lbladdlocations" runat="server" meta:resourcekey="lbladdfromlocationsResource1" Text="Allow claimants to add manual addresses" AssociatedControlID="chkaddlocations"></asp:Label><span class="inputs"><asp:CheckBox ID="chkaddlocations" runat="server" meta:resourcekey="chkaddfromlocationsResource1"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip321" onclick="SEL.Tooltip.Show('81d21610-3e7c-4b64-a4d4-ef68c002feb7', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span>
                                <asp:Label ID="lblAddCompanyAddresses" runat="server" Text="Claimants can add new organisations" AssociatedControlID="chkClaimantsCanSaveCompanyAddresses"></asp:Label><span class="inputs"><asp:CheckBox ID="chkClaimantsCanSaveCompanyAddresses" runat="server"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip416" onclick="SEL.Tooltip.Show('17f8302c-f856-4015-87d7-9da4027aecc3', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>

                            <div class="twocolumn">
                                <asp:Label runat="server" Text="Home address keyword" AssociatedControlID="txtHomeAddressKeyword"></asp:Label>
                                <span class="inputs">
                                    <asp:TextBox ID="txtHomeAddressKeyword" runat="server" MaxLength="50"></asp:TextBox></span>
                                <span class="inputicon">&nbsp;</span>
                                <span class="inputtooltipfield">
                                    <img onclick="SEL.Tooltip.Show('473DC22D-8D5F-4F66-86E1-3791B8A80A92', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span>
                                <span class="inputvalidatorfield">
                                    <asp:CustomValidator runat="server" ControlToValidate="txtHomeAddressKeyword" ClientValidationFunction="ValidateAddressKeyword" ErrorMessage="The Home address keyword you have entered cannot be used as it contains non-alphanumeric characters or there is an address label with the same text" Text="*" ValidationGroup="vgMain"></asp:CustomValidator></span>

                                <asp:Label runat="server" Text="Work address keyword" AssociatedControlID="txtWorkAddressKeyword"></asp:Label>
                                <span class="inputs">
                                    <asp:TextBox ID="txtWorkAddressKeyword" runat="server" MaxLength="50"></asp:TextBox></span>
                                <span class="inputicon">&nbsp;</span>
                                <span class="inputtooltipfield">
                                    <img onclick="SEL.Tooltip.Show('F66B9DE6-6132-49F4-AA56-48E7F42FDE13', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span>
                                <span class="inputvalidatorfield">
                                    <asp:CustomValidator runat="server" ControlToValidate="txtWorkAddressKeyword" ClientValidationFunction="ValidateAddressKeyword" ErrorMessage="The Work address keyword you have entered cannot be used as it contains non-alphanumeric characters or there is an address label with the same text" Text="*" ValidationGroup="vgMain"></asp:CustomValidator></span>
                            </div>

                            <div class="twocolumn">
                                <asp:Label ID="lblForceAddressNameEntry" runat="server" Text="Enforce naming of addresses" AssociatedControlID="chkForceAddressNameEntry"></asp:Label>
                                <span class="inputs">
                                    <asp:CheckBox ID="chkForceAddressNameEntry" runat="server"></asp:CheckBox></span>
                                <span class="inputicon">&nbsp;</span>
                                <span class="inputtooltipfield">
                                    <img onclick="SEL.Tooltip.Show('BC0B382E-97F0-40A0-A2B9-028DD9397996', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span>
                                <span class="inputvalidatorfield">&nbsp;</span>

                                <asp:Label ID="Label3" runat="server" Text="Time to retain unused address labels" AssociatedControlID="cmbRetainLabelsTime"></asp:Label>
                                <span class="inputs">
                                    <asp:DropDownList runat="server" ID="cmbRetainLabelsTime">
                                        <asp:ListItem Text="Retain forever" Value="-1"></asp:ListItem>
                                        <asp:ListItem Text="1 month" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="2 months" Value="2"></asp:ListItem>
                                        <asp:ListItem Text="3 months" Value="3"></asp:ListItem>
                                        <asp:ListItem Text="6 months" Value="6"></asp:ListItem>
                                        <asp:ListItem Text="9 months" Value="9"></asp:ListItem>
                                        <asp:ListItem Text="1 year" Value="12"></asp:ListItem>
                                    </asp:DropDownList>
                                </span>
                            </div>

                            <div class="onecolumn">
                                <asp:Label ID="lblAddressNameEntryMessage" runat="server" AssociatedControlID="txtAddressNameEntryMessage">Address name entry message</asp:Label>
                                <span class="inputs">
                                    <asp:TextBox ID="txtAddressNameEntryMessage" runat="server" TextMode="MultiLine" Rows="5"></asp:TextBox></span>
                                <span class="inputicon">&nbsp;</span>
                                <span class="inputtooltipfield">
                                    <img onclick="SEL.Tooltip.Show('65F0942F-E201-42C1-AFB2-90B2E9B5A316', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span>
                            </div>

                            <div class="twocolumn">
                                <asp:Label ID="lblMultipleWorkAddress" runat="server" Text="Allow claimants to choose from multiple work addresses (if available)" AssociatedControlID="chkMultipleWorkAddress"></asp:Label>
                                <span class="inputs">
                                    <asp:CheckBox ID="chkMultipleWorkAddress" runat="server"></asp:CheckBox></span>
                                <span class="inputicon">&nbsp;</span>
                                <span class="inputtooltipfield">
                                    <img onclick="SEL.Tooltip.Show('9194DC27-213A-45AD-95CA-AD2577589EFE', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span>
                                <span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="sectiontitle">Distance Lookups &amp; Vehicles</div>
                            <div class="twocolumn">
                                <asp:Label ID="lblUseMapPoint" runat="server" AssociatedControlID="chkUseMapPoint" Text="Postcode anywhere"></asp:Label><span class="inputs"><asp:CheckBox ID="chkUseMapPoint" runat="server" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip338" onclick="SEL.Tooltip.Show('1bb457a2-c47f-4193-902e-1743f948634b', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span>
                                <span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblMileageCalculationType" runat="server" Text="Postcode anywhere calculation type" AssociatedControlID="cmbmileagecalculationtype"></asp:Label><span class="inputs"><asp:DropDownList ID="cmbmileagecalculationtype" runat="server">
                                    <asp:ListItem Value="1" Text="Shortest"></asp:ListItem>
                                    <asp:ListItem Value="2" Text="Quickest"></asp:ListItem>
                                </asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn">                                                                
                                <asp:Label ID="lblAllowMilage" runat="server" Text="Allow employees to select vehicle journey rate categories" AssociatedControlID="chkAllowMilage"></asp:Label><span class="inputs"><asp:CheckBox ID="chkAllowMilage" runat="server" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"></span><span class="inputvalidatorfield">&nbsp;</span>
                                <asp:Label ID="lblDisableCarsStartEndDate" runat="server" Text="Vehicles are not active if the date is outside of the start and end date" AssociatedControlID="chkDisableCarOutsideOfStartEndDate"></asp:Label><span class="inputs"><asp:CheckBox ID="chkDisableCarOutsideOfStartEndDate" runat="server" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="img1" onclick="SEL.Tooltip.Show('ECD53568-D64E-4021-9D32-922020C5282A', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span>                                                                
                            </div>
                            <div class="twocolumn">
                                <asp:Label ID="lblMultipleDestinations" AssociatedControlID="chkAllowMultipleDestinations" Text="Allow multiple destinations" runat="server"></asp:Label><span class="inputs"><asp:CheckBox ID="chkAllowMultipleDestinations" runat="server" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip337" onclick="SEL.Tooltip.Show('3b68f5e3-323d-4407-a5d7-e5a3027b78aa', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span>                                                                                                
                            </div>
                            <asp:HiddenField ID="hdnAllowMileage" runat="server" />
                        </div>
                    </ContentTemplate>
                </cc1:TabPanel>
                <cc1:TabPanel ID="tabother" runat="server" HeaderText="Other Preferences">
                    <ContentTemplate>
                        <div class="formpanel">
                            <div class="twocolumn tabcontent-div">
                                <asp:Label ID="lblsingleclaim" runat="server" meta:resourcekey="lblsingleclaimResource1" AssociatedControlID="chksingleclaim">Employees can only enter a single claim at a time</asp:Label><span class="inputs"><asp:CheckBox ID="chksingleclaim" runat="server" TextAlign="Left" meta:resourcekey="chksingleclaimResource1"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip7" onclick="SEL.Tooltip.Show('7bb89658-c8bd-41dd-80ee-63d3a869d0f1', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblattach" runat="server" meta:resourcekey="lblattachResource1" AssociatedControlID="chkattach">Receipts can be uploaded to expense items</asp:Label><span class="inputs"><asp:CheckBox ID="chkattach" runat="server" TextAlign="Left" meta:resourcekey="chkattachResource1" onclick="javascript:checkMobileDeviceReceiptCompatibility('attachReceipts');"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip8" onclick="SEL.Tooltip.Show('6edbdb9a-6565-4391-b71e-af5261902030', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label ID="lblexchangereadonly" runat="server" meta:resourcekey="lblexchangereadonlyResource1" AssociatedControlID="chkexchangereadonly">Claimants can not override the exchange rates</asp:Label><span class="inputs"><asp:CheckBox ID="chkexchangereadonly" runat="server" meta:resourcekey="chkexchangereadonlyResource1"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip293" onclick="SEL.Tooltip.Show('34bb807a-b895-4e69-a4e4-d76690dbdd5a', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span>
                            </div>
                        </div>
                    </ContentTemplate>
                </cc1:TabPanel>
            </cc1:TabContainer>
        </div>
        <div id="pgESROptions" class="subPage" style="display: none;">
            <div class="formpanel formpanel_padding">
                <div class="sectiontitle">General Options</div>
                <div class="twocolumn">
                    <asp:Label ID="lblCheckESRAssignmentsOnEmployeeAdd" runat="server" AssociatedControlID="chkESRAssignmentsOnEmployeeAdd">ESR assignment numbers mandatory on employee add</asp:Label><span class="inputs"><asp:CheckBox ID="chkESRAssignmentsOnEmployeeAdd" runat="server" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip424" onclick="SEL.Tooltip.Show('c6a3f96e-09f7-4287-bed2-55adacaa5e58', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span>
                    <asp:Label ID="lblEsrDetail" runat="server" AssociatedControlID="ddlEsrDetail">Display with assignment</asp:Label><span class="inputs"><asp:DropDownList runat="server" ID="ddlEsrDetail"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip592" onclick="SEL.Tooltip.Show('edf7e35e-618f-4d71-a9aa-11749b2c4633', 'ex', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span>
                </div>
                <div class="twocolumn tabcontent-div">
                    <asp:Label ID="DisplayEsrAddressesInSearchResults" runat="server" AssociatedControlID="chkDisplayEsrAddressesInSearchResults">Display imported ESR addresses in address search results</asp:Label><span class="inputs"><asp:CheckBox ID="chkDisplayEsrAddressesInSearchResults" runat="server" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img onclick="SEL.Tooltip.Show('EB3E613B-1737-4553-B12C-53908767C9DF', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span>
                </div>
                <div class="sectiontitle">ESR Inbound Options</div>
                <div class="twocolumn tabcontent-div">
                    <asp:Label runat="server" ID="lblSummaryEsrInboundFile" AssociatedControlID="chkSummaryEsrInboundFile" Text="Create summary ESR inbound file"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkSummaryEsrInboundFile" CssClass="fillspan" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgTooltipSummaryEsrInboundFile" onclick="SEL.Tooltip.Show('BF28BF01-E729-4C6F-A277-4F050336A8C2', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span>
                    <asp:Label runat="server" ID="lblEsrRounding" AssociatedControlID="ddlEsrRounding" Text="Rounding for ESR inbound mileage values"></asp:Label><span class="inputs"><asp:DropDownList ID="ddlEsrRounding" CssClass="fillspan"  runat="server"/></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgTooltipEsrRounding" onclick="SEL.Tooltip.Show('B34925C3-7F8A-4BA8-A70B-11AB14B50871', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span>
                </div>
                <div class="sectiontitle">ESR Outbound Options</div>
                <div class="twocolumn tabcontent-div">
                    <asp:Label ID="lblESRActivateType" runat="server" Text="ESR outbound employee activation type" meta:resourcekey="lblESRActivateTypeResource1" AssociatedControlID="ddlESRActivateType"></asp:Label><span class="inputs"><asp:DropDownList ID="ddlESRActivateType" runat="server">
                        <asp:ListItem Value="0" meta:resourcekey="ListItemResource1">None</asp:ListItem>
                        <asp:ListItem Value="1" meta:resourcekey="ListItemResource2">Manual</asp:ListItem>
                        <asp:ListItem Value="2" meta:resourcekey="ListItemResource3">Automatic</asp:ListItem>
                    </asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip418" onclick="SEL.Tooltip.Show('1e481c49-e658-4a4e-b08a-ff1a4da48391', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblESRArchiveType" runat="server" Text="ESR outbound employee archive type" meta:resourcekey="lblESRArchiveTypeResource1" AssociatedControlID="ddlESRArchiveType"></asp:Label><span class="inputs"><asp:DropDownList ID="ddlESRArchiveType" runat="server">
                        <asp:ListItem Value="0" meta:resourcekey="ListItemResource1">None</asp:ListItem>
                        <asp:ListItem Value="1" meta:resourcekey="ListItemResource2">Manual</asp:ListItem>
                        <asp:ListItem Value="2" meta:resourcekey="ListItemResource3">Automatic</asp:ListItem>
                    </asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip419" onclick="SEL.Tooltip.Show('dceaacd3-2a7c-40d8-904c-d5128fb722f4', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span>
                </div>
                <div class="twocolumn tabcontent-div">
                    <asp:Label ID="lblESRArchiveGrace" runat="server" AssociatedControlID="txtESRGracePeriod">ESR outbound archive grace period</asp:Label><span class="inputs"><asp:TextBox ID="txtESRGracePeriod" MaxLength="5" runat="server"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip420" onclick="SEL.Tooltip.Show('7a3c6072-8c68-404e-bd4a-0267a3ec26e1', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"><asp:CompareValidator ID="compGracePeriodType" runat="server" ErrorMessage="Please enter a valid value into the ESR outbound archive grace period box" ControlToValidate="txtESRGracePeriod" Operator="DataTypeCheck" Type="Integer" ValidationGroup="vgMain" meta:resourcekey="compGracePeriodTypeResource1">*</asp:CompareValidator><asp:CompareValidator ID="compGracePeriodGreater" runat="server" ErrorMessage="Cannot enter negative values into Number of attempts before lock-out box" ControlToValidate="txtESRGracePeriod" Operator="GreaterThanEqual" ValueToCompare="0" Type="Integer" ValidationGroup="vgMain" meta:resourcekey="compGracePeriodGreaterResource1">*</asp:CompareValidator></span>
                    <asp:Label ID="Label2" runat="server" AssociatedControlID="chkESRActivateCar">ESR outbound vehicle automatic activation</asp:Label><span class="inputs"><asp:CheckBox ID="chkESRActivateCar" runat="server" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip593" onclick="SEL.Tooltip.Show('fde204c4-0b4a-4353-b214-b0b73c0fd943', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"></span>
                </div>
                <div class="twocolumn tabcontent-div">
                    <asp:Label runat="server" ID="lblESRManualAssignmentSupervisor" AssociatedControlID="chkESRManualAssignmentSupervisor" Text="Manually set ESR assignment supervisor"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkESRManualAssignmentSupervisor" CssClass="fillspan" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgTooltipSummaryEsrAssignmentSupervisor" onclick="SEL.Tooltip.Show('614A1BB1-C908-44FC-8924-DF38C04AFD4E', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span>                    
                    <asp:Label runat="server" ID="lblESRPrimaryAddressOnly" AssociatedControlID="chkESRPrimaryAddressOnly" Text="Only use primary address for claimant home addresses"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkEsrPrimaryAddressOnly" CssClass="fillspan" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgTooltipSummaryEsrPrimaryAddressOnly" onclick="SEL.Tooltip.Show('c1afccf5-6b05-49c2-8a97-c24c7e803f5f', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span>
                </div>

                <div class="onecolumn ">
                    <asp:Label ID="lblESRUsernameFormat" runat="server" AssociatedControlID="txtESRUsernameFormat">ESR outbound username format</asp:Label><span class="inputs"><asp:TextBox ID="txtESRUsernameFormat" runat="server" TextMode="MultiLine" Enabled="false"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip421" onclick="SEL.Tooltip.Show('588769bc-0cc5-428e-8828-7b6360766fa5', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span>
                </div>
                <div class="onecolumn">
                    <asp:Label ID="lblESRHomeAddressFormat" runat="server" AssociatedControlID="txtESRHomeAddressFormat">ESR outbound home address format</asp:Label><span class="inputs"><asp:TextBox ID="txtESRHomeAddressFormat" runat="server" TextMode="MultiLine" Enabled="false"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip422" onclick="SEL.Tooltip.Show('7331a4a1-8bac-4e05-8de7-4fc09a47d737', 'sm', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span>
                </div>
            </div>
        </div>

    </div>
    <div class="formpanel formpanel_left">
        <div class="formbuttons">
        </div>
        <asp:ImageButton ID="btnSave" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" OnClientClick="$('#ctl00_contentmain_tabsGeneralOptions_tabSelfRegistration_cmbdefaultrole, #ctl00_contentmain_tabsGeneralOptions_tabSelfRegistration_cmbdefaultitemrole').attr('disabled', false); if (validateform('vgMain') === false) return false;"
            OnClick="btnSave_Click" />&nbsp;&nbsp;<asp:ImageButton ID="btnCancel" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif"
                OnClick="btnCancel_Click" CausesValidation="False" />
    </div>
    <asp:Panel ID="pnlAddScreen" runat="server" Style="background-color: #ffffff; border: 1px solid #000000; padding: 5px; display: none;">
        <div class="formpanel">
            <div class="sectiontitle">
                <asp:Label ID="lblFieldName" runat="server"></asp:Label>
            </div>
            <div class="twocolumn">
                <asp:Label ID="lblTxtDisplayAs" runat="server" Text="Display as" AssociatedControlID="txtDisplayAs"></asp:Label><span class="inputs"><asp:TextBox ID="txtDisplayAs" runat="server" MaxLength="50"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblChkDisplayOnItem" runat="server" Text="Display on individual item" AssociatedControlID="chkDisplayOnItem"></asp:Label><span class="inputs"><asp:CheckBox ID="chkDisplayOnItem" runat="server" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
            </div>
            <div class="twocolumn">
                <asp:Label ID="lblChkDisplayFieldOnCash" runat="server" Text="Display for cash" AssociatedControlID="chkDisplayFieldOnCash"></asp:Label><span class="inputs"><asp:CheckBox ID="chkDisplayFieldOnCash" runat="server" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblChkMandatoryOnCash" runat="server" Text="Mandatory on cash items" AssociatedControlID="chkMandatoryOnCash"></asp:Label><span class="inputs"><asp:CheckBox ID="chkMandatoryOnCash" runat="server" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
            </div>
            <div class="twocolumn">
                <asp:Label ID="lblChkDisplayOnCC" runat="server" Text="Display on credit card items" AssociatedControlID="chkDisplayOnCC"></asp:Label><span class="inputs"><asp:CheckBox ID="chkDisplayOnCC" runat="server" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblChkMandatoryOnCC" runat="server" Text="Mandatory on credit card items" AssociatedControlID="chkMandatoryOnCC"></asp:Label><span class="inputs"><asp:CheckBox ID="chkMandatoryOnCC" runat="server" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
            </div>
            <div class="twocolumn">
                <asp:Label ID="lblChkDisplayOnPC" runat="server" Text="Display on purchase card items" AssociatedControlID="chkDisplayOnPC"></asp:Label><span class="inputs"><asp:CheckBox ID="chkDisplayOnPC" runat="server" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblChkMandatoryOnPC" runat="server" Text="Mandatory on purchase card items" AssociatedControlID="chkMandatoryOnPC"></asp:Label><span class="inputs"><asp:CheckBox ID="chkMandatoryOnPC" runat="server" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
            </div>
            <asp:HiddenField ID="hdnAddScreenCode" runat="server" />
            <asp:HiddenField ID="hdnAddScreenFieldID" runat="server" />
            <div class="formbuttons">
                <asp:Image ID="imgSaveAddScreen" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" onclick="saveAddScreen()" />&nbsp;&nbsp;<asp:Image ID="imgCloseAddScreen" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" onclick="closeAddScreen()" />
            </div>
        </div>
    </asp:Panel>
    <asp:HyperLink ID="hlAddScreen" runat="server" Text="&nbsp;" Style="display: none;"></asp:HyperLink>
    <cc1:ModalPopupExtender ID="mdlAddScreen" runat="server" TargetControlID="hlAddScreen" PopupControlID="pnlAddScreen" BackgroundCssClass="modalBackground">
    </cc1:ModalPopupExtender>
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="scripts" runat="server">
   <script type="text/javascript">
       $(document).ready(function () {
           loadDutyOfCareFunctions();
           hideReminderFrequencyIfChecked();
           hideProvidersIfChecked();
           var width = $(window).width(), height = $(window).height();
           $('.formpanel div:not(.sectiontitle, .formbuttons)').contents().filter(function () {
               return this.nodeType === 3;
           }).remove();
       });
   </script>
</asp:Content>
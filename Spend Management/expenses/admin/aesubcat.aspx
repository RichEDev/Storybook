<%@ Page ValidateRequest="false" Language="c#" Inherits="Spend_Management.aesubcat" MasterPageFile="~/masters/smForm.master" CodeBehind="aesubcat.aspx.cs" EnableViewState="false" %>

<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="styles" runat="server">
    <style type="text/css">
     .sectiontitle {
         margin-top: 30px;
         margin-bottom: 15px;
     }

     .formpanel .twocolumn .inputs input{
         margin-top:0;
     }

     .twocolumn > span > img, .twocolumn span.inputicon input[type="image"]{
         padding-top:0;
     }

     input[type=file]:focus, input[type=checkbox]:focus, input[type=radio]:focus {
         outline: none !important;
     }

     #hometoOfficeOptions .twocolumn {
         margin-bottom:30px;
     }

     p{
         font-size: 13px;
     }
  </style>
    
     <!--[if IE 7]>
        <style>
            .formpanel {
                display: inline-block;
            }
             .inputs {
                 display: inline-block;
             }
            .formbuttons {
                display: inline-block;
            }
            textarea {
                margin-left: 13px;
            }
         .formbuttons img {
             margin-top: -14px;
         }
        </style>
    <![endif]-->

</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
    <asp:ScriptManagerProxy ID="ScriptMan" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/expenses/javaScript/subcats.js?date=20180125" />
            <asp:ScriptReference Path="~/shared/javaScript/userdefined.js?date=20180417" />
            <asp:ScriptReference Path="~/shared/javaScript/sel.ajax.js" />
        </Scripts>
    </asp:ScriptManagerProxy>

    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            var setFixedMilesTextbox = function () {
                if ($('#' + '<%=optDeductFixed.ClientID %>').is(":checked")) {
                    $('.fixedMiles').show();
                } else {
                    $('.fixedMiles').hide();
                }
            };
            $('#' + chkEnforceMileageCap).change(function () {
                 SetMaximumMilesTextbox();
            });

            SetMaximumMilesTextbox();
            SetupDateFields();

            $('.formpanel div:not(.sectiontitle, .formbuttons)').contents().filter(function () {
                return this.nodeType === 3;
            }).remove();

            $('#' + '<%=chkenablehometooffice.ClientID %>').change(function () {
                if ($(this).is(":checked")) {
                    setFixedMilesTextbox();
                    SetMaximumMilesTextbox();
                    setRotationalMileageRelatedFields();
                    $('.hometoOfficeZeroMiles').show();
                    $('#hometoOfficeOptions').show();
                } else {
                    $('.hometoOfficeZeroMiles').hide();
                    $('#hometoOfficeOptions').hide();
                }
            });

            $('#divCalculation3 :radio').change(function () {
                setFixedMilesTextbox();
                SetMaximumMilesTextbox();
                setRotationalMileageRelatedFields();
            });

            setDocOptions();

             $('#' + '<%=chkEnableDoc.ClientID %>').change(function () {
                 setClass1BusinessOptions();
            });
        });

        var ddlstCalculation = '<%=ddlstCalculation.ClientID %>';
        var chkmileage = '<%=chkmileage.ClientID %>';
        var chkstaff = '<%=chkstaff.ClientID %>';
        var chkothers = '<%=chkothers.ClientID %>';
        var chknodirectors = '<%=chknodirectors.ClientID %>';
        var chkreimbursable = '<%=chkreimbursable.ClientID %>';
        var txtVATStartDate = '<% = txtVatStartDate.ClientID %>';
        var txtVATEndDate = '<% = txtVatEndDate.ClientID %>';
        var txtVatAmount = '<% = txtvatamount.ClientID %>';
        var txtVATPercent = '<% = txtvatpercent.ClientID %>';
        var chkVATRReceipt = '<% = chkvatreceipt.ClientID %>';
        var txtVATLimitWithout = '<% = txtvatlimitwithout.ClientID %>';
        var txtVATLimitWith = '<% = txtvatlimitwith.ClientID %>';
        var pnlVatRanges = '<%=pnlVatRanges.ClientID %>';
        var pceVat = '<%=pceVat.ClientID %>';
        var chkattendees = '<%=chkattendees.ClientID %>';
        var chkattendeesmand = '<%=chkattendeesmand.ClientID %>';
        var chkbmiles = '<%=chkbmiles.ClientID %>';
        var chkcompany = '<%=chkcompany.ClientID %>';
        var chkentertainment = '<%=chkentertainment.ClientID %>';
        var chkeventinhome = '<%=chkeventinhome.ClientID %>';
        var chkfrom = '<%=chkfrom.ClientID %>';
        var chkhotel = '<%=chkhotel.ClientID %>';
        var chkhotelmand = '<%=chkhotelmand.ClientID %>';
        var chknonights = '<%=chknonights.ClientID %>';
        var chknopassengers = '<%=chknopassengers.ClientID %>';
        var chkpassengernames = '<%=chkpassengernames.ClientID %>';
        var chknormalreceipt = '<%=chknormalreceipt.ClientID %>';
        var chknorooms = '<%=chknorooms.ClientID %>';
        var chkotherdetails = '<%=chkotherdetails.ClientID %>';
        var chkpassenger = '<%=chkpassenger.ClientID %>';
        var chkpersonalguests = '<%=chkpersonalguests.ClientID %>';
        var chkpmiles = '<%=chkpmiles.ClientID %>';
        var chkreason = '<%=chkreason.ClientID %>';
        var chkreimbursable = '<%=chkreimbursable.ClientID %>';
        var chkremoteworkers = '<%=chkremoteworkers.ClientID %>';
        var chksplitpersonal = '<%=chksplitpersonal.ClientID %>';
        var chksplitremoteworkers = '<%=chksplitremoteworkers.ClientID %>';
        var chktip = '<%=chktip.ClientID %>';
        var chkto = '<%=chkto.ClientID %>';
        var chkvatnumber = '<%=chkvatnumber.ClientID %>';
        var chkvatnumbermand = '<%=chkvatnumbermand.ClientID %>';
        var chkvatreceipt = '<%=chkvatreceipt.ClientID %>';
        var cmbcategories = '<%=cmbcategories.ClientID %>';
        var cmbentertainment = '<%=cmbentertainment.ClientID %>';
        var cmbpdcats = '<%=cmbpdcats.ClientID %>';
        var cmbsplitpersonal = '<%=cmbsplitpersonal.ClientID %>';
        var cmbsplitremote = '<%=cmbsplitremote.ClientID %>';
        var cmbtotaltype = '<%=cmbtotaltype.ClientID %>';
        var ddlstcalculation = '<%=ddlstCalculation.ClientID %>';
        var txtaccountcode = '<%=txtaccountcode.ClientID %>';
        var txtallowanceamount = '<%=txtallowanceamount.ClientID %>';
        var txtalternateaccountcode = '<%=txtalternateaccountcode.ClientID %>';
        var txtcomment = '<%=txtcomment.ClientID %>';
        var txtdescription = '<%=txtdescription.ClientID %>';
        var txtshortsubcat = '<%=txtshortsubcat.ClientID %>';
        var txtsubcat = '<%=txtsubcat.ClientID %>';
        var modRole = '<%=modRole.ClientID %>';
        var tabGeneral = '<%=tabGeneral.ClientID %>';
        var modSplit = '<%=modSplit.ClientID %>';
        var tabAdditionalFields = '<%=tabAdditionalFields.ClientID %>';
        var ddlstDateRange = '<%=ddlstDateRange.ClientID %>';
        var ddlstItemRole = '<% =ddlstItemRole.ClientID %>';
        var chkaddtotemplate = '<%=chkaddtotemplate.ClientID %>';
        var txtlimitwithout = '<%=txtlimitwithout.ClientID %>';
        var txtlimitwith = '<%=txtlimitwith.ClientID %>';
        var chkenablehometooffice = '<%=chkenablehometooffice.ClientID %>';
        var optdeducthometooffice = '<%=optdeducthometooffice.ClientID %>';
        var optflaghometooffice = '<%=optflaghometooffice.ClientID %>';
        var optdeducthometoofficeonce = '<%=optDeductHomeToOfficeDistanceOnce.ClientID %>';
        var optdeducthometoofficeall = '<%=optDeductHomeToOfficeDistanceAll.ClientID %>';
        var optdeducthometoofficestart = '<%=optDeductHomeToOfficeDistanceStart.ClientID %>';
        var optdeductfirstorlasthome = '<%=optDeductFirstOrLastHome.ClientID %>';
        var optdeducthometoofficedistancefull = '<%=optDeductHomeToOfficeDistanceFull.ClientID %>';
        var optdeductfullhometoofficestart = '<%=optDeductFullHomeToOfficeDistanceStart.ClientID %>';
        var optDeductFixed = '<%=optDeductFixed.ClientID %>';
        var txtDeductFixed = '<%=txtDeductFixed.ClientID %>';
        var txtMileageCap= '<%=txtMileageCap.ClientID %>';
        var chkEnforceMileageCap = '<%=chkEnforceMileageCap.ClientID %>';
        var optRotationalMileage = '<%=optRotationalMileage.ClientID %>';
        var ddlstPublicTransportRate = '<%=ddlstPublicTransportRate.ClientID %>';
        var chkhometoifficeaszero = '<%=chkHomeToOfficeAsZero.ClientID %>';
        var ddlstMileageCategory = '<%=ddlstMileageCategory.ClientID %>';
        var chkIsRelocationMileage = '<%=chkIsRelocationMileage.ClientID %>';
        var chkHeavyBulky = '<%=chkHeavyBulky.ClientID %>';
        var reqstartdate = '<%=reqstartdate.ClientID %>';
        var reqenddate = '<%=reqenddate.ClientID %>';
        var cmbReimbursableItems = '<%=cmbReimbursableItems.ClientID %>';
        var ddlstReimburseMileageCategory = '<%=ddlstReimburseMileageCategory.ClientID %>';
        var pnlSplitListGrid = '<%=pnlSplitList.ClientID %>';
        var pnlSplitItemsGrid = '<%=pnlSplitItems.ClientID %>';
        var pnlRolesGrid = '<%=pnlRoles.ClientID %>';
        var expenseValidationEnabled = '<%= tabValidation.Visible %>';
        var chkEnableValidation = '<%=chkEnableValidation.ClientID %>';
        var txtValidatorNotes1 = '<%=txtValidatorNotes1.ClientID %>';
        var txtValidatorNotes2 = '<%=txtValidatorNotes2.ClientID %>';
        var txtValidatorNotes3 = '<%=txtValidatorNotes3.ClientID %>';
        var chkEnableDoc = '<%=chkEnableDoc.ClientID %>';
        var chkRequireClass1Insurance = '<%=chkRequireClass1Insurance.ClientID %>';
    </script>

    <cc1:TabContainer ID="TabContainer1" runat="server">
        <cc1:TabPanel ID="tabGeneral" runat="server" HeaderText="General Details">
        <ContentTemplate>
                <div class="formpanel">
                    <div class="sectiontitle">
                        <asp:Label ID="Label2" runat="server" meta:resourcekey="Label2Resource1">General Details</asp:Label>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="lblitem" runat="server" meta:resourcekey="lblitemResource1" AssociatedControlID="txtsubcat">Expense Item</asp:Label>
                        <span class="inputs">
                            <asp:TextBox ID="txtsubcat" runat="server" MaxLength="50" meta:resourcekey="txtsubcatResource1"></asp:TextBox>
                        </span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield">
                            <asp:RequiredFieldValidator ID="reqsubcat" runat="server" ControlToValidate="txtsubcat" ErrorMessage="Please enter a name for the Expense Item" meta:resourceKey="reqsubcatResource1" ValidationGroup="vgSubcat">*</asp:RequiredFieldValidator>
                        </span>
                        <asp:Label ID="lblshortname" runat="server" meta:resourcekey="lblshortnameResource1" AssociatedControlID="txtshortsubcat">Expense Item Abbreviated</asp:Label> 
                        <span class="inputs">
                            <asp:TextBox ID="txtshortsubcat" runat="server" MaxLength="50" meta:resourcekey="txtsubcatResource1"></asp:TextBox> 
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield">
                            <img id="imgtooltip363" alt="" class="tooltipicon" onclick="SEL.Tooltip.Show('d514aa4a-aab0-436b-b13c-81f0ebca99ac', 'sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" /></span><span class="inputvalidatorfield"></span>
                    </div>
                    <div class="onecolumnsmall">
                        <asp:Label ID="lblcategory" runat="server" meta:resourcekey="lblcategoryResource1" AssociatedControlID="cmbcategories">Expense Category</asp:Label>
                        <span class="inputs">
                            <asp:DropDownList ID="cmbcategories" runat="server" meta:resourcekey="cmbcategoriesResource1"></asp:DropDownList>
                        </span>
                        <span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield">
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Please select a Expense Category" ControlToValidate="cmbcategories" meta:resourcekey="RequiredFieldValidator1Resource1">*</asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="compmaster" runat="server" ErrorMessage="Please select an Expense Category" ControlToValidate="cmbcategories" ValueToCompare="0" Operator="NotEqual" meta:resourcekey="compmasterResource1" ValidationGroup="vgSubcat">*</asp:CompareValidator>
                        </span>
                    </div>
                    <div class="onecolumnsmall">
                        <asp:Label ID="lblp11d" runat="server" meta:resourcekey="lblp11dResource1" AssociatedControlID="cmbpdcats">P11D Category</asp:Label>
                        <span class="inputs">
                            <asp:DropDownList ID="cmbpdcats" runat="server" meta:resourcekey="cmbpdcatsResource1"></asp:DropDownList>
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="lblaccountcode" runat="server" meta:resourcekey="lblaccountcodeResource1" AssociatedControlID="txtaccountcode">Account Code</asp:Label>
                        <span class="inputs">
                            <asp:TextBox ID="txtaccountcode" runat="server" MaxLength="50" meta:resourcekey="txtaccountcodeResource1"></asp:TextBox>
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield">
                            <img id="imgtooltip301" alt="" class="tooltipicon" onclick="SEL.Tooltip.Show('8d4a2743-3fab-43d2-90cb-ed0ef60061fa', 'sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" />
                        </span>
                        <span class="inputvalidatorfield"></span>
                        <asp:Label ID="lblalternateaccountcode" runat="server" Text="Alternate Account Code" meta:resourcekey="lblalternateaccountcodeResource1" AssociatedControlID="txtalternateaccountcode"></asp:Label>

                        <span class="inputs">
                            <asp:TextBox ID="txtalternateaccountcode" runat="server" meta:resourcekey="txtalternateaccountcodeResource1"></asp:TextBox>
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                    </div>
                    <div class="onecolumn">
                        <asp:Label ID="lbldescription" runat="server" meta:resourcekey="lbldescriptionResource1" AssociatedControlID="txtdescription"><p class="labeldescription">Description</p></asp:Label>
                        <span class="inputs">
                            <asp:TextBox ID="txtdescription" runat="server" MaxLength="4000" TextMode="MultiLine" meta:resourcekey="txtdescriptionResource1"></asp:TextBox>
                        </span>
                    </div>
                    <div class="onecolumn">
                        <asp:Label ID="lblcomment" runat="server" meta:resourcekey="lblcommentResource1" AssociatedControlID="txtcomment"><p class="labeldescription">Comment</p></asp:Label>
                        <span class="inputs">
                            <asp:TextBox ID="txtcomment" runat="server" TextMode="MultiLine" meta:resourcekey="txtcommentResource1"></asp:TextBox> 
                        </span>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="lbltotalentered" runat="server" meta:resourcekey="lbltotalenteredResource1" AssociatedControlID="cmbtotaltype">Total entered as</asp:Label>
                        <span class="inputs">
                            <asp:DropDownList ID="cmbtotaltype" runat="server" meta:resourceKey="cmbtotaltypeResource1">
                                <asp:ListItem Text="Gross" Value="1"></asp:ListItem>
                                <asp:ListItem Text="NET" Value="2"></asp:ListItem>
                            </asp:DropDownList> 
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                        <asp:Label ID="lblreimbursable" runat="server" meta:resourcekey="lblreimbursableResource1" AssociatedControlID="chkreimbursable">Reimbursable</asp:Label> 
                        <span class="inputs">
                            <asp:CheckBox ID="chkreimbursable" runat="server" meta:resourceKey="chkreimbursableResource1"></asp:CheckBox>
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                    </div>

                    <div class="sectiontitle">
                        <asp:Label ID="Label3" runat="server" meta:resourcekey="Label3Resource1">Account Codes</asp:Label>
                    </div>
                    <asp:Literal ID="litcountries" runat="server" meta:resourcekey="litcountriesResource1"></asp:Literal>
                    <asp:PlaceHolder ID="holderCountries" runat="server"></asp:PlaceHolder>
                    <div class="sectiontitle">
                        <asp:Label ID="Label7" runat="server" meta:resourcekey="Label7Resource1">Calculation</asp:Label>
                    </div>
                    <div class="onecolumnsmall">
                        <asp:Label ID="Label13" runat="server" Text="Item Type" AssociatedControlID="ddlstCalculation"></asp:Label>
                        <span class="inputs">
                            <asp:DropDownList ID="ddlstCalculation" runat="server" onchange="changeCalculationDiv();">
                                <asp:ListItem Value="1" Text="Standard Item"></asp:ListItem>
                                <asp:ListItem Value="3" Text="Mileage (Pence Per Mile)"></asp:ListItem>
                                <asp:ListItem Value="2" Text="Meal"></asp:ListItem>
                                <asp:ListItem Value="4" Text="Daily Allowance"></asp:ListItem>
                                <asp:ListItem Value="5" Text="Fuel Receipt"></asp:ListItem>
                                <asp:ListItem Value="6" Text="Mileage (Based on Fuel Receipt)"></asp:ListItem>
                                <asp:ListItem Value="7" Text="Fixed Allowance"></asp:ListItem>
                                <asp:ListItem Value="8" Text="Fuel Card Mileage"></asp:ListItem>
                                <asp:ListItem Value="9" Text="Item Reimburse"></asp:ListItem>
                                <asp:ListItem Value="10" Text="Fixed Excess Mileage"></asp:ListItem>
                            </asp:DropDownList>
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                    </div>
                    <div id="divCalculation3" style="display: none;">
                        <div class="onecolumnsmall">
                            <asp:Label ID="Label38" runat="server" Text="Force Vehicle Journey Rate Category" AssociatedControlID="ddlstMileageCategory"></asp:Label>

                            <span class="inputs">
                                <asp:DropDownList ID="ddlstMileageCategory" runat="server"></asp:DropDownList>
                            </span>
                            <span class="inputicon"></span>
                            <span class="inputtooltipfield"></span>
                            <span class="inputvalidatorfield"></span>
                        </div>
                        <div class="twocolumn">
                            <asp:Label ID="Label14" runat="server" Text="Increase vehicle journey rate for passengers" AssociatedControlID="chkpassenger"></asp:Label>
                            <span class="inputs">
                                <asp:CheckBox ID="chkpassenger" runat="server" meta:resourcekey="chkpassengerResource1"></asp:CheckBox>
                            </span>
                            <span class="inputicon"></span>
                            <span class="inputtooltipfield"></span>
                            <span class="inputvalidatorfield"></span>
                            <asp:Label ID="Label43" runat="server" Text="Enable Relocation Mileage" AssociatedControlID="chkIsRelocationMileage"></asp:Label>
                            <span class="inputs">
                                <asp:CheckBox ID="chkIsRelocationMileage" runat="server"></asp:CheckBox>
                            </span>
                            <span class="inputicon"></span>
                            <span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                        </div>
                        <div class="twocolumn">
                            <asp:Label ID="lblHeavyBulkyMileage" runat="server" Text="Increase vehicle journey rate for heavy bulky equipment" AssociatedControlID="chkHeavyBulky"></asp:Label>
                            <span class="inputs">
                                <asp:CheckBox ID="chkHeavyBulky" runat="server" meta:resourcekey="chkHeavyBulkyResource1"></asp:CheckBox>
                            </span>
                            <span class="inputicon"></span>
                            <span class="inputtooltipfield"></span>
                            <span class="inputvalidatorfield"></span>
                        </div>
                        <div class="sectiontitle">
                            <asp:Label ID="Label40" runat="server" Text="Home to Office Mileage"></asp:Label>
                        </div>
                        <div class="twocolumn">
                            <asp:Label ID="Label39" runat="server" Text="Enable Home to Office Mileage" AssociatedControlID="chkenablehometooffice"></asp:Label>
                            <span class="inputs">
                                <asp:CheckBox ID="chkenablehometooffice" runat="server"></asp:CheckBox>
                            </span>
                            <span class="inputicon"></span>
                            <span class="inputtooltipfield"></span>
                            <span class="inputvalidatorfield"></span>
                            <asp:Label ID="lblHomeToOfficeAsZero" runat="server" Text="Home to Office is always zero" CssClass="hometoOfficeZeroMiles" AssociatedControlID="chkHomeToOfficeAsZero"></asp:Label>
                            <span class="inputs"><asp:CheckBox ID="chkHomeToOfficeAsZero" runat="server"  CssClass="hometoOfficeZeroMiles"/></span><span class="inputicon"></span>
                            <span class="inputtooltipfield hometoOfficeZeroMiles">
                                <img id="imgtooltip586" alt="" class="tooltipicon" onclick="SEL.Tooltip.Show('68b5437f-0935-4397-899c-f4482534bda2', 'sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" />
                            </span>
                            <span class="inputvalidatorfield"></span>
                        </div>
                        <div id="hometoOfficeOptions">maximumMiles
                             <div class="twocolumn">
                            <asp:Label runat="server" Text="Enforce mileage cap on Home to Office journeys" AssociatedControlID="chkEnforceMileageCap"></asp:Label>
                            <span class="inputs">
                               <asp:CheckBox ID="chkEnforceMileageCap" runat="server" />
                            </span>
                            <span class="inputicon"></span>
                            <span class="inputtooltipfield">
                            <img id="imgtooltipMaxMiles" alt="" class="tooltipicon" onclick="SEL.Tooltip.Show('3D0FE460-0DAE-403D-8AB4-C34AB30698C9','sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" />
                            </span>
                            <span class="inputvalidatorfield"></span>
                            <asp:Label runat="server" Text="Maximum number of miles*" AssociatedControlID="txtMileageCap" CssClass="mandatory maximumMiles"></asp:Label>
                            <span class="inputs">
                                <asp:TextBox ID="txtMileageCap" runat="server" MaxLength="4" CssClass="maximumMiles"/>
                            </span>
                            <span class="inputicon maximumMiles"></span>
                            <span class="inputtooltipfield maximumMiles"></span>
                            <span class="inputvalidatorfield maximumMiles">
                                <asp:RequiredFieldValidator  ID="reqFieldMaximumNoOfMiles" runat="server" ErrorMessage="Please enter a value for Maximum number of miles in the box provided." Text="*" ControlToValidate="txtMileageCap" ValidationGroup="vgHomeToOfficeMileageCap"></asp:RequiredFieldValidator>
                                <asp:CompareValidator ID="cmpFieldMaximumNoOfMiles1" runat="server" ErrorMessage="Please enter a value for Maximum number of miles." ControlToValidate="txtMileageCap" Operator="DataTypeCheck" Type="Double"  ValidationGroup="vgHomeToOfficeMileageCap">*</asp:CompareValidator>
                                <asp:CompareValidator ID="cmpFieldMaximumNoOfMiles2" runat="server" ErrorMessage="The Maximum number of miles must be less than or equal to 1000." ControlToValidate="txtMileageCap" Operator="LessThanEqual" ValueToCompare="1000" Type="Double"  ValidationGroup="vgHomeToOfficeMileageCap">*</asp:CompareValidator>
                                <asp:CompareValidator ID="cmpFieldMaximumNoOfMiles3" runat="server" ErrorMessage="The Maximum number of miles must be greater than 0." ControlToValidate="txtMileageCap" Operator="GreaterThan" ValueToCompare="0" Type="Double"  ValidationGroup="vgHomeToOfficeMileageCap">*</asp:CompareValidator>
                            </span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label ID="Label41" runat="server" Text="Enforce the lesser of Home to address & Office to address" AssociatedControlID="optdeducthometooffice"></asp:Label>
                                <span class="inputs">
                                    <asp:RadioButton ID="optdeducthometooffice" runat="server" GroupName="hometooffice" />
                                </span>
                                <span class="inputicon"></span>
                                <span class="inputtooltipfield">
                                    <img id="imgtooltip503" alt="" class="tooltipicon" onclick="SEL.Tooltip.Show('5c214b46-d7c5-4815-8b61-98fde21847ce','sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" />
                                </span>
                                <span class="inputvalidatorfield"></span>
                                <asp:Label ID="Label42" runat="server" Text="Flag if Home to location is greater than office to location" AssociatedControlID="optflaghometooffice"></asp:Label>
                                <span class="inputs">
                                    <asp:RadioButton ID="optflaghometooffice" runat="server" GroupName="hometooffice" />
                                </span>
                                <span class="inputicon"></span>
                                <span class="inputtooltipfield">
                                    <img id="imgtooltip504" alt="" class="tooltipicon" onclick="SEL.Tooltip.Show('63523e83-02e8-45cc-9a21-fbb45f4ec4de','sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" />
                                </span>
                                <span class="inputvalidatorfield"></span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label ID="lblDeductHomeToOfficeOnce" runat="server" Text="Deduct Home to Office Distance from journey once" AssociatedControlID="optDeductHomeToOfficeDistanceOnce"></asp:Label>
                                <span class="inputs">
                                    <asp:RadioButton ID="optDeductHomeToOfficeDistanceOnce" runat="server" GroupName="hometooffice" />
                                </span>
                                <span class="inputicon"></span>
                                <span class="inputtooltipfield">
                                    <img id="imgtooltip506" alt="" class="tooltipicon" onclick="SEL.Tooltip.Show('95d335e7-e5d3-4995-895d-6a95f8f43173', 'sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" />
                                </span>
                                <span class="inputvalidatorfield"></span>
                                <asp:Label ID="lblDeductHomeToOfficeAll" runat="server" Text="Deduct Home to Office distance every time home is visited" AssociatedControlID="optDeductHomeToOfficeDistanceAll"></asp:Label>
                                <span class="inputs">
                                    <asp:RadioButton ID="optDeductHomeToOfficeDistanceAll" runat="server" GroupName="hometooffice" />
                                </span>
                                <span class="inputicon"></span>
                                <span class="inputtooltipfield">
                                    <img id="imgtooltip507" alt="" class="tooltipicon" onclick="SEL.Tooltip.Show('7c9c0ece-111c-4c64-a1cc-2e00771a08c3','sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" />
                                </span>
                                <span class="inputvalidatorfield"></span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label ID="lblDeductHomeToOfficeStart" runat="server" Text="Deduct Home to Office Distance if journey starts or finishes at home" AssociatedControlID="optDeductHomeToOfficeDistanceStart"></asp:Label>
                                <span class="inputs">
                                    <asp:RadioButton ID="optDeductHomeToOfficeDistanceStart" runat="server" GroupName="hometooffice" />
                                </span>
                                <span class="inputicon"></span>
                                <span class="inputtooltipfield">
                                    <img id="imgtooltip508" alt="" class="tooltipicon" onclick="SEL.Tooltip.Show('5d3a3819-96d0-43c5-8f32-f3b1706bb585','sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" />
                                </span>
                                <span class="inputvalidatorfield"></span>
                                <asp:Label ID="lblDeductFirstOrLastHome" runat="server" Text="Deduct first and/or last Home to Office Distance from Journey" AssociatedControlID="optDeductFirstOrLastHome"></asp:Label>
                                <span class="inputs">
                                    <asp:RadioButton ID="optDeductFirstOrLastHome" runat="server" GroupName="hometooffice" />
                                </span>
                                <span class="inputicon"></span>
                                <span class="inputtooltipfield">
                                    <img id="imgtooltip509" alt="" class="tooltipicon" onclick="SEL.Tooltip.Show('7d78bd8b-377f-42b0-a599-9d21b4bacd56','sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" />
                                </span><span class="inputvalidatorfield"></span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label ID="lblDeductHomeToOfficeFull" runat="server" Text="Deduct Full Home to Office trip every time home is visited" AssociatedControlID="optDeductHomeToOfficeDistanceFull"></asp:Label>
                                <span class="inputs">
                                    <asp:RadioButton ID="optDeductHomeToOfficeDistanceFull" runat="server" GroupName="hometooffice" />
                                </span>
                                <span class="inputicon"></span>
                                <span class="inputtooltipfield">
                                    <img id="imgtooltip510" alt="" class="tooltipicon" onclick="SEL.Tooltip.Show('7b613846-c391-464b-b10f-0aa5bf743885','sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" />
                                </span>
                                <span class="inputvalidatorfield"></span>
                                <asp:Label ID="lblDeductFullHomeToOfficeStart" runat="server" Text="Deduct full home to office distance if journey starts/finishes at home" AssociatedControlID="optDeductFullHomeToOfficeDistanceStart"></asp:Label>
                                <span class="inputs">
                                    <asp:RadioButton ID="optDeductFullHomeToOfficeDistanceStart" runat="server" GroupName="hometooffice" />
                                </span>
                                <span class="inputicon"></span>
                                <span class="inputtooltipfield">
                                    <img id="imgtooltip511" alt="" class="tooltipicon" onclick="SEL.Tooltip.Show('38213958-5AE8-4CD0-9A28-55F3C87AB28B','sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" />
                                </span>
                                <span class="inputvalidatorfield"></span>
                            </div>
                            <div class="twocolumn">
                            <asp:Label runat="server" Text="Deduct fixed number of miles every time home is visited" AssociatedControlID="optDeductFixed"></asp:Label>
                            <span class="inputs">
                                <asp:RadioButton ID="optDeductFixed" runat="server" GroupName="hometooffice" />
                            </span>
                            <span class="inputicon"></span>
                            <span class="inputtooltipfield">
                            <img id="imgtooltip510" alt="" class="tooltipicon" onclick="SEL.Tooltip.Show('3C21AA39-D178-4DCA-95FE-3DD371EA9E7F','sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" />
                            </span>
                            <span class="inputvalidatorfield"></span>
                            <asp:Label runat="server" Text="Number of miles to deduct*" AssociatedControlID="txtDeductFixed" CssClass="mandatory fixedMiles"></asp:Label>
                            <span class="inputs">
                                <asp:TextBox ID="txtDeductFixed" runat="server" CssClass="fixedMiles" MaxLength="4"/>
                            </span>
                            <span class="inputicon"></span>
                            <span class="inputtooltipfield fixedMiles"></span>
                            <span class="inputvalidatorfield">
                                <asp:RequiredFieldValidator ID="reqDeductMiles1" runat="server" ErrorMessage="Please enter a value for Number of miles to deduct in the box provided." Text="*" ControlToValidate="txtDeductFixed" ValidationGroup="vgHomeToOffice"></asp:RequiredFieldValidator>
                                <asp:CompareValidator ID="reqDeductMiles2" runat="server" ErrorMessage="Please enter a valid value for Number of miles to deduct." ControlToValidate="txtDeductFixed" Operator="DataTypeCheck" Type="Double"  ValidationGroup="vgHomeToOffice">*</asp:CompareValidator>
                                <asp:CompareValidator ID="reqDeductMiles3" runat="server" ErrorMessage="The Number of miles to deduct must be less than or equal to 1000." ControlToValidate="txtDeductFixed" Operator="LessThanEqual" ValueToCompare="1000" Type="Double"  ValidationGroup="vgHomeToOffice">*</asp:CompareValidator>
                            </span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label runat="server" Text="Junior doctors rotational mileage" AssociatedControlID="optRotationalMileage"></asp:Label>
                                <span class="inputs">
                                    <asp:RadioButton ID="optRotationalMileage" runat="server" GroupName="hometooffice" />
                                </span>
                                <span class="inputicon"></span>
                                <span class="inputtooltipfield">
                                <img alt="" class="tooltipicon" onclick="SEL.Tooltip.Show('4ED340F7-0148-4025-9D6B-21B9A09F87F4','sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" />
                                </span>
                                <span class="inputvalidatorfield"></span>                              
                                <asp:Label runat="server" Text="Public transport rate*" AssociatedControlID="ddlstPublicTransportRate"  CssClass="mandatory publicTransportRateField"></asp:Label>
                                <span class="inputs publicTransportRateField">
                                    <asp:DropDownList ID="ddlstPublicTransportRate" runat="server"></asp:DropDownList>
                                </span>
                                <span class="inputicon publicTransportRateField"></span>
                                <span class="inputtooltipfield publicTransportRateField"><img alt="" class="tooltipicon" onclick="SEL.Tooltip.Show('D2686083-5806-4A0D-8360-4D8F614DBD90','sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" /></span>
                                <span class="inputvalidatorfield publicTransportRateField">
                                    <asp:CompareValidator  runat="server" ErrorMessage="Please select a Public transport rate." ControlToValidate="ddlstPublicTransportRate" ValueToCompare="0" Operator="NotEqual" ValidationGroup="vgRotationalMileage">*</asp:CompareValidator>
                                </span>
                            </div>
                        </div>
                    </div>
                    <div id="divCalculation2" style="display: none;">
                        <div class="twocolumn">
                            <asp:Label ID="Label15" runat="server" Text="Split Number of Others" AssociatedControlID="chkentertainment"></asp:Label>
                            <span class="inputs">
                                <asp:CheckBox ID="chkentertainment" runat="server" meta:resourcekey="chkentertainmentResource1"></asp:CheckBox>
                             </span>
                            <span class="inputicon"></span>
                            <span class="inputtooltipfield"></span>
                            <span class="inputvalidatorfield"></span>
                        </div>
                        <div class="onecolumnsmall">
                            <asp:Label ID="Label16" runat="server" Text="Split Others To" AssociatedControlID="cmbentertainment"></asp:Label>
                            <span class="inputs">
                                <asp:DropDownList ID="cmbentertainment" runat="server" meta:resourcekey="cmbentertainmentResource1"></asp:DropDownList>
                            </span>
                            <span class="inputicon"></span>
                            <span class="inputtooltipfield"></span>
                            <span class="inputvalidatorfield"></span>
                        </div>
                        <div class="twocolumn">
                            <asp:Label ID="Label17" runat="server" Text="Split Number of Spouses/Partners" AssociatedControlID="chksplitpersonal"></asp:Label>
                            <span class="inputs">
                                <asp:CheckBox ID="chksplitpersonal" runat="server" meta:resourcekey="chksplitpersonalResource1"></asp:CheckBox>
                            </span>
                            <span class="inputicon"></span>
                            <span class="inputtooltipfield"></span>
                            <span class="inputvalidatorfield"></span>
                        </div>
                        <div class="onecolumnsmall">
                            <asp:Label ID="Label18" runat="server" Text="Split Spouses/Partners To" AssociatedControlID="cmbsplitpersonal"></asp:Label>
                            <span class="inputs">
                                <asp:DropDownList ID="cmbsplitpersonal" runat="server" meta:resourcekey="cmbsplitpersonalResource1"></asp:DropDownList>
                            </span>
                            <span class="inputicon"></span>
                            <span class="inputtooltipfield"></span>
                            <span class="inputvalidatorfield"></span>
                        </div>
                        <div class="twocolumn">
                            <asp:Label ID="Label19" runat="server" Text="Split Number of Remote Workers" AssociatedControlID="chksplitremoteworkers"></asp:Label>
                            <span class="inputs">
                                <asp:CheckBox ID="chksplitremoteworkers" runat="server" meta:resourcekey="chksplitremoteworkersResource1"></asp:CheckBox>
                            </span>
                            <span class="inputicon"></span>
                            <span class="inputtooltipfield"></span>
                            <span class="inputvalidatorfield"></span>
                        </div>
                        <div class="onecolumnsmall">
                            <asp:Label ID="Label20" runat="server" Text="Split Remote Workers To" AssociatedControlID="cmbsplitremote"></asp:Label>
                            <span class="inputs">
                                <asp:DropDownList ID="cmbsplitremote" runat="server" meta:resourcekey="cmbsplitremoteResource1"></asp:DropDownList>
                            </span>
                            <span class="inputicon"></span>
                            <span class="inputtooltipfield"></span>
                            <span class="inputvalidatorfield"></span>
                        </div>
                    </div>
                    <div id="divCalculation4" style="display: none;">
                        <div class="twocolumn">
                            <asp:Literal ID="litallowances" runat="server"></asp:Literal>
                        </div>
                    </div>
                    <div id="divCalculation7" style="display: none;">
                        <div class="twocolumn">
                            <asp:Label ID="lblallowanceamount" runat="server" meta:resourcekey="lblallowanceamountResource1" AssociatedControlID="txtallowanceamount">Allowance amount</asp:Label>
                            <span class="inputs">
                                <asp:TextBox ID="txtallowanceamount" runat="server" meta:resourceKey="txtallowanceamountResource1"></asp:TextBox>
                            </span>
                            <span class="inputicon"></span>
                            <span class="inputtooltipfield"></span>
                            <span class="inputvalidatorfield">
                                <asp:CompareValidator ID="compallowanceamount" runat="server" ControlToValidate="txtallowanceamount" 
                                    ErrorMessage="Please enter a valid value for Allowance amount"
                                    meta:resourceKey="compallowanceamountResource1" Operator="DataTypeCheck"
                                    Type="Currency" ValidationGroup="vgSubcat">*</asp:CompareValidator>
                            </span>
                            <span class="inputs"></span>
                            <span class="inputicon"></span>
                            <span class="inputtooltipfield"></span>
                            <span class="inputvalidatorfield"></span>
                        </div>
                    </div>
                    <div id="divCalculation8" style="display: none;">
                        <div class="twocolumn">
                            <asp:Label ID="lblReimbursableItems" runat="server" meta:resourcekey="lblReimbursableItemsResource1" AssociatedControlID="cmbReimbursableItems">Reimbursable Items</asp:Label>
                            <span class="inputs">
                                <asp:DropDownList ID="cmbReimbursableItems" runat="server" meta:resourcekey="cmbReimbursableItems"></asp:DropDownList>
                            </span>
                            <span class="inputicon"></span>
                            <span class="inputtooltipfield"></span>
                            <span class="inputvalidatorfield"></span>
                        </div>
                        <div class="onecolumnsmall">
                            <asp:Label ID="lblReimburseForce" runat="server" Text="Force Vehicle Journey Rate Category" AssociatedControlID="ddlstReimburseMileageCategory"></asp:Label>
                            <span class="inputs">
                                <asp:DropDownList ID="ddlstReimburseMileageCategory" runat="server"></asp:DropDownList>
                            </span>
                            <span class="inputicon"></span>
                            <span class="inputtooltipfield"></span>
                            <span class="inputvalidatorfield"></span>
                        </div>
                    </div>                                                 
                    <div id="divDoc" style="display: none;">   
                        <div class="sectiontitle">
                        <asp:Label ID="lblDoc" runat="server" meta:resourcekey="Label7Resource1">Duty of Care</asp:Label>
                    </div>                         
                        <div class="twocolumn">
                            <asp:Label ID="lblEnableDoc" runat="server" Text="Enable duty of care" AssociatedControlID="chkEnableDoc"></asp:Label>
                            <span class="inputs">
                                <asp:CheckBox ID="chkEnableDoc" runat="server" meta:resourcekey="chkEnableDocResource1" Checked="false" ></asp:CheckBox>
                            </span>                            
                            <span class="inputicon"></span>
                            <span class="inputtooltipfield">
                            <img id="imgtooltip512" alt="" class="tooltipicon" onclick="SEL.Tooltip.Show('155D7E22-1A9C-4825-BB7A-7EE922218C3F','sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" />
                            </span>
                            <span class="inputvalidatorfield"></span>                     
                        </div>
                        <div class="twocolumn">
                            <asp:Label ID="lblRequireClass1Insurance" runat="server" Text="Class 1 business insurance required" AssociatedControlID="chkRequireClass1Insurance"></asp:Label>
                            <span class="inputs">
                                <asp:CheckBox ID="chkRequireClass1Insurance" runat="server" meta:resourcekey="chkRequireClass1InsuranceResource1"></asp:CheckBox>
                            </span>
                            <span class="inputicon"></span>
                            <span class="inputtooltipfield">
                                <img id="imgtooltip513" alt="" class="tooltipicon" onclick="SEL.Tooltip.Show('2FDA6913-E86D-4C05-962F-29949147197E', 'sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" />
                            </span>
                            <span class="inputvalidatorfield"></span>
                        </div>                       
                    </div>
                    <div id="splitExpenseItems" class="sectiontitle">
                        <asp:Label ID="Label4" runat="server" meta:resourcekey="Label4Resource1">Split Expense Items</asp:Label>
                    </div>
                    
                    <a id="linkSplitItem" href="javascript:populateSplitItems();popupSplitItemModal(true);">Add Split Item</a>
                    
                    <asp:Panel ID="pnlSplitItems" runat="server">
                        <asp:Literal ID="litsplit" runat="server" meta:resourcekey="litsplitResource1"></asp:Literal>
                    </asp:Panel>

                    <div class="sectiontitle">
                        <asp:Label ID="Label6" runat="server" meta:resourcekey="Label6Resource1">VAT Details</asp:Label>
                    </div>

                    <a href="javascript:activeVatRangeID = 0;ddlstDateRange_onchange();popupVatModal(true);">Add Vat Range</a><br />
                    
                    <asp:Panel ID="pnlVatRanges" runat="server">
                        <asp:Literal ID="litVatRates" runat="server"></asp:Literal>
                    </asp:Panel>

                    <asp:PlaceHolder ID="holderUserdefined" runat="server"></asp:PlaceHolder>
                </div>
            </ContentTemplate>
        </cc1:TabPanel>
        
        

        <cc1:TabPanel runat="server" ID="tabAdditionalFields" HeaderText="Additional Fields">
            <ContentTemplate>
                <div class="formpanel">
                    <div class="sectiontitle">
                        <asp:Label ID="Label8" runat="server" Text="General"></asp:Label>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="lblnormalreceipt" runat="server" meta:resourcekey="lblnormalreceiptResource1" AssociatedControlID="chknormalreceipt">Show Normal Receipt</asp:Label>
                        <span class="inputs">
                            <asp:CheckBox ID="chknormalreceipt" runat="server" meta:resourcekey="chknormalreceiptResource1"></asp:CheckBox>
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                        <asp:Label ID="Label9" runat="server" Text="Show VAT Number" AssociatedControlID="chkvatnumber"></asp:Label>
                        <span class="inputs">
                            <asp:CheckBox ID="chkvatnumber" runat="server" meta:resourcekey="chkvatnumberResource1"></asp:CheckBox>
                        </span>
                        <span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="Label21" runat="server" Text="Is the VAT Number mandatory" AssociatedControlID="chkvatnumbermand"></asp:Label>
                        <span class="inputs">
                            <asp:CheckBox ID="chkvatnumbermand" runat="server" meta:resourcekey="chkvatnumbermandResource1"></asp:CheckBox>
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                        <span class="inputs"></span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                    </div>
                    <div class="sectiontitle">
                        <asp:Label ID="Label22" runat="server" Text="Mileage"></asp:Label>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="lblmileage" runat="server" meta:resourcekey="lblmileageResource1" AssociatedControlID="chkmileage">Show Number of Miles</asp:Label>
                        <span class="inputs">
                            <asp:CheckBox ID="chkmileage" runat="server" meta:resourcekey="chkmileageResource1"></asp:CheckBox>
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="lblnopassengers" runat="server" meta:resourcekey="lblnopassengersResource1" AssociatedControlID="chknopassengers">Show Number of Passengers</asp:Label>
                        <span class="inputs">
                            <asp:CheckBox ID="chknopassengers" runat="server" meta:resourcekey="chknopassengersResource1"></asp:CheckBox>
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                        <asp:Label ID="lblpassengernames" runat="server" meta:resourcekey="lblmileageResource1" AssociatedControlID="chkpassengernames">Show Names of Passengers</asp:Label>
                        <span class="inputs">
                            <asp:CheckBox ID="chkpassengernames" runat="server" meta:resourcekey="chkpassengernamesResource1"></asp:CheckBox>
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="lblpmiles" runat="server" meta:resourcekey="lblpmilesResource1" AssociatedControlID="chkpmiles">Show Number of Personal Miles</asp:Label>
                        <span class="inputs">
                            <asp:CheckBox ID="chkpmiles" runat="server" meta:resourcekey="chkpmilesResource1"></asp:CheckBox>
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                        <asp:Label ID="lblbmiles" runat="server" meta:resourcekey="lblbmilesResource1" AssociatedControlID="chkbmiles">Show Number of Business Miles</asp:Label>
                        <span class="inputs">
                            <asp:CheckBox ID="chkbmiles" runat="server" meta:resourcekey="chkbmilesResource1"></asp:CheckBox>
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                    </div>
                    <div class="sectiontitle">
                        <asp:Label ID="Label23" runat="server" Text="Meals"></asp:Label>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="lblstaff" runat="server" meta:resourcekey="lblstaffResource1" AssociatedControlID="chkstaff">Show Number of Staff</asp:Label>
                        <span class="inputs">
                            <asp:CheckBox ID="chkstaff" runat="server" meta:resourcekey="chkstaffResource1"></asp:CheckBox>
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                        <asp:Label ID="lblothers" runat="server" meta:resourcekey="lblothersResource1" AssociatedControlID="chkothers">Show Number of Others</asp:Label>
                        <span class="inputs">
                            <asp:CheckBox ID="chkothers" runat="server" meta:resourcekey="chkothersResource1"></asp:CheckBox>
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="lblnodirectors" runat="server" meta:resourcekey="lblnodirectorsResource1" AssociatedControlID="chknodirectors">Show Number of Directors</asp:Label>
                        <span class="inputs">
                            <asp:CheckBox ID="chknodirectors" runat="server" meta:resourcekey="chknodirectorsResource1"></asp:CheckBox>
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                        <asp:Label ID="Label24" runat="server" Text="Show Number of Spouses/Partners" AssociatedControlID="chkpersonalguests"></asp:Label>
                        <span class="inputs">
                            <asp:CheckBox ID="chkpersonalguests" runat="server" meta:resourcekey="chkpersonalguestsResource1"></asp:CheckBox>
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="Label25" runat="server" Text="Show Number of Remote Workers" AssociatedControlID="chkremoteworkers"></asp:Label>
                        <span class="inputs">
                            <asp:CheckBox ID="chkremoteworkers" runat="server" meta:resourcekey="chkremoteworkersResource1"></asp:CheckBox>
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                        <asp:Label ID="lbleventinhome" runat="server" meta:resourcekey="lbleventinhomeResource1" AssociatedControlID="chkeventinhome">Show event in home city</asp:Label>
                        <span class="inputs">
                            <asp:CheckBox ID="chkeventinhome" runat="server" meta:resourcekey="chkeventinhomeResource1"></asp:CheckBox>
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="lblattendees" runat="server" meta:resourcekey="lblattendeesResource1" AssociatedControlID="chkattendees">Show Attendees List</asp:Label>
                        <span class="inputs">
                            <asp:CheckBox ID="chkattendees" runat="server" meta:resourcekey="chkattendeesResource1"></asp:CheckBox>
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                        <asp:Label ID="Label1" runat="server" meta:resourcekey="Label1Resource1" AssociatedControlID="chkattendeesmand">Attendees List is Mandatory</asp:Label>
                        <span class="inputs">
                            <asp:CheckBox ID="chkattendeesmand" runat="server" meta:resourcekey="chkattendeesmandResource1"></asp:CheckBox>
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="lbltip" runat="server" meta:resourcekey="lbltipResource1" AssociatedControlID="chktip">Show Tip</asp:Label>
                        <span class="inputs">
                            <asp:CheckBox ID="chktip" runat="server" meta:resourcekey="chktipResource1"></asp:CheckBox>
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                        <span class="inputs"></span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                    </div>
                    <div class="sectiontitle">
                        Hotels
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="lblnonights" runat="server" meta:resourcekey="lblnonightsResource1" AssociatedControlID="chknonights">Show Number of Nights</asp:Label>
                        <span class="inputs">
                            <asp:CheckBox ID="chknonights" runat="server" meta:resourcekey="chknonightsResource1"></asp:CheckBox>
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                        <asp:Label ID="lblnorooms" runat="server" meta:resourcekey="lblnoroomsResource1" AssociatedControlID="chknorooms">Show Number of Rooms</asp:Label>
                        <span class="inputs">
                            <asp:CheckBox ID="chknorooms" runat="server" meta:resourcekey="chknoroomsResource1"></asp:CheckBox>
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="lblhotel" runat="server" meta:resourcekey="lblhotelResource1" AssociatedControlID="chkhotel">Show Hotel Name/Rating</asp:Label>
                        <span class="inputs">
                            <asp:CheckBox ID="chkhotel" runat="server" meta:resourcekey="chkhotelResource1"></asp:CheckBox>
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                        <asp:Label ID="lblhotelmand" runat="server" meta:resourcekey="lblhotelmandResource1" AssociatedControlID="chkhotelmand">The Hotel Name is mandatory</asp:Label>
                        <span class="inputs">
                            <asp:CheckBox ID="chkhotelmand" runat="server" meta:resourcekey="chkhotelmandResource1"></asp:CheckBox>
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                    </div>
                    <div class="sectiontitle">
                        <asp:Label ID="Label5" runat="server" Text="General Details Fields"></asp:Label>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="Label10" runat="server" Text="Reason" AssociatedControlID="chkreason"></asp:Label>
                        <span class="inputs">
                            <asp:CheckBox ID="chkreason" runat="server" meta:resourcekey="chkreasonResource1" />
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                        <asp:Label ID="Label32" runat="server" Text="Other Details" AssociatedControlID="chkotherdetails"></asp:Label>
                        <span class="inputs">
                            <asp:CheckBox ID="chkotherdetails" runat="server" meta:resourcekey="chkotherdetailsResource1" />
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span> 

                    </div> 
                    <div class="twocolumn">
                        <asp:Label ID="lblfrom" runat="server" Text="Label" AssociatedControlID="chkfrom"></asp:Label>
                        <span class="inputs">
                            <asp:CheckBox ID="chkfrom" runat="server" />
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                        <asp:Label ID="lblto" runat="server" Text="Label" AssociatedControlID="chkto"></asp:Label>
                        <span class="inputs">
                            <asp:CheckBox ID="chkto" runat="server" />
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="lblcompany" runat="server" Text="Label" AssociatedControlID="chkcompany"></asp:Label>
                        <span class="inputs">
                            <asp:CheckBox ID="chkcompany" runat="server" />
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                        <span class="inputs"></span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                    </div>
                    <div class="sectiontitle">
                        <asp:Label ID="Label33" runat="server" Text="Other Fields"></asp:Label>
                    </div>
                    <asp:PlaceHolder ID="holderUDFs" runat="server"></asp:PlaceHolder>
                </div>
            </ContentTemplate>
        </cc1:TabPanel>
        
        
        
        

        <cc1:TabPanel runat="server" ID="tabRoles" HeaderText="Roles & Limits">
            <ContentTemplate>
                <div class="formpanel">
                    <div class="sectiontitle">
                        <asp:Label ID="lblExpenseItemDates" runat="server" Text="Allowed Dates"></asp:Label>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="lblStartDate" runat="server" meta:resourcekey="lblStartDateResource1" AssociatedControlID="txtStartDate">Start date</asp:Label>
                        <span class="inputs">
                            <asp:TextBox ID="txtStartDate" ClientIDMode="Static" CssClass="dateField" runat="server" MaxLength="10" meta:resourcekey="txtStartDateResource1"></asp:TextBox>
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield">
                            <img id="imgStartDateTooltip" alt="" class="tooltipicon" onclick="SEL.Tooltip.Show('4025C246-8907-40B0-9BD9-CCAEFE9327BB', 'ex', this);" src="../../shared/images/icons/16/plain/tooltip.png" />
                        </span>
                        <span class="inputvalidatorfield">
                            <asp:CompareValidator runat="server" ID="comptxtStartDate" ClientIDMode="Static" ControlToValidate="txtStartDate" Type="Date" ErrorMessage="Please enter a valid Start date" Text="*" Operator="DataTypeCheck" ValidationGroup="vgSubcat" Display="Dynamic"></asp:CompareValidator>
                        </span>
                        <asp:Label ID="lblEndDate" runat="server" meta:resourcekey="lblEndDateResource1" AssociatedControlID="txtEndDate">End date</asp:Label>
                        <span class="inputs">
                            <asp:TextBox ID="txtEndDate" ClientIDMode="Static" CssClass="dateField" runat="server" MaxLength="10" meta:resourcekey="txtEndDateResource1"></asp:TextBox>
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield">
                            <img id="imgEndDateTooltip" alt="" class="tooltipicon" onclick="SEL.Tooltip.Show('BC8456A8-3E74-4FF8-A3A5-E547E59EC612', 'ex', this);" src="../../shared/images/icons/16/plain/tooltip.png" />
                        </span>
                        <span class="inputvalidatorfield">
                            <asp:CompareValidator runat="server" ID="comptxtEndDate" ClientIDMode="Static" ControlToValidate="txtEndDate" Type="Date" ErrorMessage="Please enter a valid End date" Text="*" Operator="DataTypeCheck" ValidationGroup="vgSubcat" Display="Dynamic"></asp:CompareValidator>
                        </span>
                    </div>
                    <div class="sectiontitle">
                        <asp:Label ID="Label12" runat="server" meta:resourcekey="Label12Resource1">Allowed Roles</asp:Label>
                    </div>
                    <a href="javascript:popupRoleModal(true);">Add Item Role</a>
                    <br />
                    <asp:Panel ID="pnlRoles" runat="server">
                        <asp:Literal ID="litRoles" runat="server"></asp:Literal>
                    </asp:Panel>
                </div>
            </ContentTemplate> 
        </cc1:TabPanel>
        
        
        

        <cc1:TabPanel runat="server" ID="tabValidation" HeaderText="Validation" Visible="False">
            <ContentTemplate>
                <div class="formpanel">
                    <div class="sectiontitle">
                        <asp:Label ID="lblTitleValidation" runat="server" Text="Validation"></asp:Label>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="lblEnableValidation" runat="server" AssociatedControlID="chkEnableValidation"><p style="margin-top: 8px;width: 190px;">Validate this expense type</p></asp:Label>
                        <span class="inputs">
                            <asp:CheckBox ID="chkEnableValidation" ClientIDMode="Static" CssClass="checkbox" runat="server"></asp:CheckBox>
                        </span>
                        <span class="inputicon"></span>
                        <span class="inputtooltipfield">
                            <img id="imgtooltipValidateSubcat" alt="" class="tooltipicon" onclick="SEL.Tooltip.Show('B9290544-BB81-4102-8F47-88512244FFC2', 'ex', this);" src="../../shared/images/icons/16/plain/tooltip.png" />
                        </span>
                    </div>
                        <div class="onecolumn">
                        <asp:Label ID="lblValidatorNotes1" runat="server" AssociatedControlID="txtValidatorNotes1">Custom Validation Rule 1</asp:Label>
                        <span class="inputs">
                            <asp:HiddenField runat="server" id="validationCriterion1Id" ClientIDMode="Static" value="0"></asp:HiddenField>
                            <span class="inputs">
                                <asp:TextBox ID="txtValidatorNotes1" MaxLength="400" TextMode="MultiLine" ClientIDMode="Static" runat="server"></asp:TextBox>
                            </span>
                        </span>
                    </div>
                    <div class="onecolumn">
                        <asp:Label ID="lblValidatorNotes2" runat="server" AssociatedControlID="txtValidatorNotes2">Custom Validation Rule 2</asp:Label>
                        <span class="inputs">
                            <asp:HiddenField runat="server" id="validationCriterion2Id" ClientIDMode="Static" value="0"></asp:HiddenField>
                            <span class="inputs">
                                <asp:TextBox ID="txtValidatorNotes2" MaxLength="400" TextMode="MultiLine" ClientIDMode="Static" runat="server"></asp:TextBox>
                            </span>
                        </span>
                    </div>
                    <div class="onecolumn">
                        <asp:Label ID="lblValidatorNotes3" runat="server" AssociatedControlID="txtValidatorNotes3">Custom Validation Rule 3</asp:Label>
                        <span class="inputs">
                            <asp:HiddenField runat="server" id="validationCriterion3Id" ClientIDMode="Static" value="0"></asp:HiddenField>
                            <span class="inputs">
                                <asp:TextBox ID="txtValidatorNotes3" MaxLength="400" TextMode="MultiLine" ClientIDMode="Static" runat="server"></asp:TextBox>
                            </span>
                        </span>
                    </div>
            </ContentTemplate> 
        </cc1:TabPanel>
        

    </cc1:TabContainer>
    <div class="formpanel" style="padding-left:0px;">
        <div class="formbuttons">
            <a href="javascript:currentAction = 'OK';saveSubcat()">
                <img alt="Save" src="../../shared/images/buttons/btn_save.png" /></a>&nbsp;&nbsp;
            <a href="adminsubcats.aspx">
                <image src="../../shared/images/buttons/cancel_up.gif" />
            </a>
        </div>
    </div>
    

    <asp:Panel ID="pnlVat" runat="server" class="modalpanel">
        <div class="formpanel">
            <div class="sectiontitle">
                <asp:Label ID="Label44" runat="server" Text="Dates"></asp:Label>
            </div>
            <div class="twocolumn">
                <asp:Label ID="Label27" runat="server" Text="Date Range" AssociatedControlID="ddlstDateRange"></asp:Label>
                <span class="inputs">
                    <asp:DropDownList ID="ddlstDateRange" runat="server">
                        <asp:ListItem Text="After Or Equal To" Value="1"></asp:ListItem>
                        <asp:ListItem Value="3" Text="Any"></asp:ListItem>
                        <asp:ListItem Value="0" Text="Before"></asp:ListItem>
                        <asp:ListItem Value="2" Text="Between"></asp:ListItem>
                    </asp:DropDownList>
                </span>
                <span class="inputicon"></span>
                <span class="inputtooltipfield"></span>
                <span class="inputvalidatorfield"></span>
            </div>
            <div class="twocolumn">
                <span id="spanRangeStart">
                    <asp:Label ID="Label28" runat="server" Text="Start Date" AssociatedControlID="txtVatStartDate"></asp:Label>
                    <span class="inputs">
                        <asp:TextBox ID="txtVatStartDate" runat="server" meta:resourcekey="txtvatamountResource1"></asp:TextBox>
                        <cc1:MaskedEditExtender ID="mskvatstartdate" MaskType="Date" Mask="99/99/9999" TargetControlID="txtVatStartDate" CultureName="en-GB" runat="server"></cc1:MaskedEditExtender>
                        <cc1:CalendarExtender ID="calhiredate" runat="server" TargetControlID="txtVatStartDate" PopupButtonID="imgstartdate" Format="dd/MM/yyyy"></cc1:CalendarExtender>
                    </span>
                    <span class="inputicon">
                        <asp:Image ID="imgstartdate" ImageUrl="~/shared/images/icons/cal.gif" runat="server" />
                    </span>
                    <span class="inputtooltipfield"></span>
                    <span class="inputvalidatorfield">
                        <asp:RequiredFieldValidator ID="reqstartdate" runat="server" ErrorMessage="Please enter the start date of this range in the box provided" Text="*" ControlToValidate="txtVatStartDate" ValidationGroup="vgVATRate"></asp:RequiredFieldValidator>
                        <asp:CompareValidator ID="compstartdate" runat="server" ErrorMessage="The start date you have entered is invalid" Text="*" ControlToValidate="txtVatStartDate" ValidationGroup="vgVATRate" Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
                        <asp:CompareValidator ID="compStartLess" runat="server" ErrorMessage="The start date must be less than the end date" Text="*" ControlToValidate="txtVatStartDate" ValidationGroup="vgVATRate" Operator="LessThan" ControlToCompare="txtVatEndDate" Type="Date"></asp:CompareValidator>
                    </span>
                </span>
                <span id="spanRangeEnd">
                    <asp:Label ID="Label29" runat="server" Text="End Date" AssociatedControlID="txtVatEndDate"></asp:Label>
                    <span class="inputs">
                        <asp:TextBox ID="txtVatEndDate" runat="server" meta:resourcekey="txtvatamountResource1"></asp:TextBox>
                        <cc1:MaskedEditExtender ID="mskvatenddate" MaskType="Date" Mask="99/99/9999" TargetControlID="txtVatEndDate" CultureName="en-GB" runat="server"></cc1:MaskedEditExtender>
                            <cc1:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtVatEndDate" PopupButtonID="imgenddate" Format="dd/MM/yyyy"></cc1:CalendarExtender>
                    </span>
                    <span class="inputicon">
                            <asp:Image ID="imgenddate" ImageUrl="~/shared/images/icons/cal.gif" runat="server" />
                    </span>
                    <span class="inputtooltipfield"></span>
                    <span class="inputvalidatorfield">
                        <asp:RequiredFieldValidator ID="reqenddate" runat="server" ErrorMessage="Please enter the end date of this range in the box provided" Text="*" ControlToValidate="txtVatEndDate" ValidationGroup="vgVATRate"></asp:RequiredFieldValidator>
                        <asp:CompareValidator ID="compenddate" runat="server" ErrorMessage="The end date you have entered is invalid" Text="*" ControlToValidate="txtVatEndDate" ValidationGroup="vgVATRate" Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
                        <asp:CompareValidator ID="CompareValidator1" runat="server" ErrorMessage="The end date must be greater than the start date" Text="*" ControlToValidate="txtVatEndDate" ValidationGroup="vgVATRate" Operator="GreaterThan" ControlToCompare="txtVatStartDate" Type="Date"></asp:CompareValidator>
                    </span>
                </span>
            </div>
            <div class="sectiontitle">
                <asp:Label ID="Label26" runat="server" Text="General Details"></asp:Label>
            </div>
            <div class="twocolumn">
                <asp:Label ID="lblvatamount" runat="server" meta:resourcekey="lblvatamountResource1" AssociatedControlID="txtvatamount">VAT Amount</asp:Label>
                <span class="inputs">
                    <asp:TextBox ID="txtvatamount" runat="server" meta:resourcekey="txtvatamountResource1"></asp:TextBox>
                </span>
                <span class="inputicon"></span>
                <span class="inputtooltipfield"></span>
                <span class="inputvalidatorfield">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Please enter a value for VAT amount in the box provided" Text="*" ControlToValidate="txtvatamount" ValidationGroup="vgVATRate"></asp:RequiredFieldValidator>
                    <asp:CompareValidator ID="compvatamount" runat="server" ErrorMessage="Please enter a valid value for VAT Amount" ControlToValidate="txtvatamount" Operator="DataTypeCheck" Type="Double" meta:resourcekey="compvatamountResource1" ValidationGroup="vgVATRate">*</asp:CompareValidator>
                </span>
                <asp:Label ID="lblvatpercent" runat="server" meta:resourcekey="lblvatpercentResource1" AssociatedControlID="txtvatpercent">Vat Percent Claimable</asp:Label>
                <span class="inputs">
                    <asp:TextBox ID="txtvatpercent" runat="server" meta:resourcekey="txtvatpercentResource1"></asp:TextBox>
                </span>
                <span class="inputicon"></span>
                <span class="inputtooltipfield"></span>
                <span class="inputvalidatorfield">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="Please enter a value for VAT percent in the box provided" Text="*" ControlToValidate="txtvatpercent" ValidationGroup="vgVATRate"></asp:RequiredFieldValidator>
                    <asp:CompareValidator ID="compvatpercent" runat="server" ErrorMessage="Please enter a valid value for VAT Percent Claimable" ControlToValidate="txtvatpercent" Operator="DataTypeCheck" Type="Integer" meta:resourcekey="compvatpercentResource1" ValidationGroup="vgVATRate">*</asp:CompareValidator>
                </span>
            </div>
            <div class="twocolumn">
                <asp:Label ID="lblvatreceipt" runat="server" meta:resourcekey="lblvatreceiptResource1" AssociatedControlID="chkvatreceipt">VAT Receipt Needed</asp:Label>
                <span class="inputs">
                    <asp:CheckBox ID="chkvatreceipt" runat="server"></asp:CheckBox>
                </span>
                <span class="inputicon"></span>
                <span class="inputtooltipfield"></span>
                <span class="inputvalidatorfield"></span>
                <asp:Label ID="Label30" runat="server" Text="VAT Limit (Without Receipt)" AssociatedControlID="txtvatlimitwithout"></asp:Label>
                <span class="inputs">
                    <asp:TextBox ID="txtvatlimitwithout" runat="server" Width="50px" meta:resourcekey="txtvatlimitwithoutResource1">0</asp:TextBox>
                </span>
                <span class="inputicon"></span>
                <span class="inputtooltipfield">
                    <img id="imgtooltip364" onclick="SEL.Tooltip.Show('0d9cec77-fec2-4f32-829b-b574572889e3', 'sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" />
                </span>
                <span class="inputvalidatorfield">
                    <asp:CompareValidator ID="compvatlimitwithout" runat="server" ControlToValidate="txtvatlimitwithout" ErrorMessage="The Vat Limit (Without Receipt) you have entered is invalid" Operator="DataTypeCheck" Type="Double" meta:resourcekey="compvatlimitwithoutResource1" ValidationGroup="vgVATRate">*</asp:CompareValidator>
                </span>
            </div>
            <div class="twocolumn">
                <asp:Label ID="Label31" runat="server" Text="VAT Limit (With Receipt)" AssociatedControlID="txtvatlimitwith"></asp:Label>
                <span class="inputs">
                    <asp:TextBox ID="txtvatlimitwith" runat="server" meta:resourcekey="txtvatlimitwithResource1"></asp:TextBox>
                </span>
                <span class="inputicon"></span>
                <span class="inputtooltipfield">
                    <img id="imgtooltip365" onclick="SEL.Tooltip.Show('27c6ac2c-9298-4649-8e40-564445ac5604', 'sm', this);" src="../../shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" />
                </span>
                <span class="inputvalidatorfield">
                    <asp:CompareValidator ID="compvatlimitwith" runat="server" ControlToValidate="txtvatlimitwith" ErrorMessage="The Vat Limit (With Receipt) you have entered is invalid" Operator="DataTypeCheck" Type="Double" meta:resourcekey="compvatlimitwithResource1" ValidationGroup="vgVATRate">*</asp:CompareValidator>
                </span>
            </div>
            <div class="formbuttons">
                <a href="javascript:addVatRange(true);">
                    <img src="../../shared/images/buttons/btn_save.png" alt="OK" /></a>&nbsp;&nbsp;
                <a href="javascript:hideVatModal();">
                    <img src="../../shared/images/buttons/cancel_up.gif" alt="Cancel" />
                </a>
            </div>
        </div>
    </asp:Panel>
    
    
    <asp:LinkButton ID="lnkPopup" runat="server" Style="display: none;">LinkButton</asp:LinkButton>
    <cc1:ModalPopupExtender ID="pceVat" runat="server" TargetControlID="lnkPopup" PopupControlID="pnlVat" BackgroundCssClass="modalBackground">
    </cc1:ModalPopupExtender>
    
    
    <asp:Panel ID="pnlRole" runat="server" class="modalpanel">
        <div class="formpanel">
            <div class="sectiontitle">
                <asp:Label ID="Label11" runat="server" Text="General Details"></asp:Label>
            </div>
            <div class="twocolumn">
                <asp:Label ID="Label34" runat="server" Text="Item Role" AssociatedControlID="ddlstItemRole"></asp:Label>
                <span class="inputs">
                    <asp:DropDownList ID="ddlstItemRole" runat="server"></asp:DropDownList>
                </span>
                <span class="inputicon"></span>
                <span class="inputtooltipfield"></span>
                <span class="inputvalidatorfield"></span>
                <asp:Label ID="Label35" runat="server" Text="Add to Template" AssociatedControlID="chkaddtotemplate"></asp:Label>
                <span class="inputs">
                    <asp:CheckBox ID="chkaddtotemplate" runat="server" />
                </span>
                <span class="inputicon"></span>
                <span class="inputtooltipfield"></span>
                <span class="inputvalidatorfield"></span>
            </div>
            <div class="twocolumn">
                <asp:Label ID="Label36" AssociatedControlID="txtlimitwithout" runat="server" Text="Maximum Limit (Without Receipt)"></asp:Label>
                <span class="inputs">
                    <asp:TextBox ID="txtlimitwithout" runat="server"></asp:TextBox>
                </span>
                <span class="inputicon"></span>
                <span class="inputtooltipfield"></span>
                <span class="inputvalidatorfield">
                    <asp:CompareValidator ID="complimitwithout" runat="server" ErrorMessage="The value you have entered for Maximum Limit (Without Receipt) is invalid" Operator="GreaterThanEqual" Type="Currency" Text="*" ValueToCompare="0" ControlToValidate="txtlimitwithout" ValidationGroup="vgRole"></asp:CompareValidator>
                </span>
                <asp:Label ID="Label37" runat="server" Text="Maximum Limit (With Receipt)" AssociatedControlID="txtlimitwith"></asp:Label>
                <span class="inputs">
                    <asp:TextBox ID="txtlimitwith" runat="server"></asp:TextBox>
                </span>
                <span class="inputicon"></span>
                <span class="inputtooltipfield"></span>
                <span class="inputvalidatorfield">
                    <asp:CompareValidator ID="complimitwith" runat="server" ErrorMessage="The value you have entered for Maximum Limit (With Receipt) is invalid" Operator="GreaterThanEqual" Type="Currency" Text="*" ValueToCompare="0" ControlToValidate="txtlimitwith" ValidationGroup="vgRole"></asp:CompareValidator>
                </span>
            </div>
            <div class="formbuttons">
                <a href="javascript:saveRole()">
                    <img alt="Save" src="../../shared/images/buttons/btn_save.png" />
                </a>&nbsp;&nbsp;
                <asp:ImageButton ID="cmdrolecancel" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" />
            </div>
        </div>
    </asp:Panel>
    <asp:LinkButton ID="lnkRole" runat="server" Style="display: none;">LinkButton</asp:LinkButton>
    <cc1:ModalPopupExtender ID="modRole" runat="server" TargetControlID="lnkRole" PopupControlID="pnlRole" BackgroundCssClass="modalBackground" CancelControlID="cmdrolecancel">
    </cc1:ModalPopupExtender>
    <asp:Panel ID="pnlSplit" runat="server" class="modalpanel" Style="display: none; height: 550px; width: 800px; padding: 4px; position: fixed; overflow: auto;">
        <div class="formpanel" style="width: 770px;">
            <asp:Panel ID="pnlSplitList" runat="server">
            </asp:Panel>
            <div class="formbuttons">
                <a href="javascript:saveSplitItems()">
                    <img alt="Save" src="../../shared/images/buttons/btn_save.png" />
                </a>&nbsp;&nbsp;
                <asp:ImageButton ID="cmdsplitcancel" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" />
            </div>
        </div>
    </asp:Panel>

    <cc1:ModalPopupExtender ID="modSplit" CancelControlID="cmdsplitcancel" runat="server" TargetControlID="lnkAddSplit" PopupControlID="pnlSplit" BackgroundCssClass="modalBackground">
    </cc1:ModalPopupExtender>
    <asp:LinkButton ID="lnkAddSplit" runat="server" Style="display: none;">Add Split Item</asp:LinkButton>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="scripts" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $(".formpanel .sectiontitle:first-child").css("margin-top", 0);

            $(".formbuttons a:first img").css("margin-top", 0);

        });
    </script>
    
</asp:Content>


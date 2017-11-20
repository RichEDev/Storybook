
(function() {
    var scriptName = "FlagsAndLimits";
    function execute() {
        SEL.registerNamespace("SEL.FlagsAndLimits");
        SEL.FlagsAndLimits =
        {
            currentAction: null,
            flagID: null,
            ddlstFlagTypeID: null,
            ddlstActionID: null,
            txtFlagTextID: null,
            txtAmberToleranceID: null,
            ddlstInvalidDateTypeID: null,
            txtDateID: null,
            txtMonthsID: null,
            txtFrequencyID: null,
            txtPeriodID: null,
            ddlstPeriodyTypeID: null,
            txtLimitID: null,
            lblDateComparisonValueID: null,
            dynRolesID: null,
            dynSelectedRolesID: null,
            modRolesID: null,
            dynSelectedExpenseItemsID: null,
            dynExpenseItemsID: null,
            modExpenseItemsID: null,
            dynFlagInformationID: null,
            modFlagInformationID: null,
            lblFrequencyID: null,
            txtGroupLimitID: null,
            dynFlagInformation: null,
            modFlagInformation: null,
            modFieldsID: null,
            txtDescription: null,
            chkActive: null,
            chkClaimantJustificationRequired: null,
            chkDisplayFlagImmediately: null,
            txtNoFlagTolerance: null,
            reqMonths: null,
            reqDate: null,
            compMonths: null,
            compDate: null,
            ddlstFrequencyTypeID: null,
            ddlstFinancialYearID: null,
            reqFrequency: null,
            rangeFrequency: null,
            reqPeriod: null,
            rangePeriod: null,
            reqLimit: null,
            compLimit: null,
            reqGroupLimit: null,
            compGroupLimit: null,
            rangeNoFlagTolerance: null,
            rangeAmber: null,
            reqTipLimit: null,
            rangeTipLimit: null,
            txtTipLimit: null,
            ddlstFlagLevel: null,
            chkApproverJustificationRequired: null,
            chkIncreaseByNumOthers: null,
            chkDisplayLimit: null,
            txtNotesForAuthoriser: null,
            ddlstItemRoleInclusionType: null,
            ddlstExpenseItemInclusionType: null,
            CustomCriteriaPopulated: false,
            ItemRoleExistsMsg: 'The item role cannot be assigned to this flag as another flag already exists for the same item role and expense item combination.',
            ExpenseItemExistsMsg: 'The expense item cannot be assigned to this flag as another flag already exists for the same item role and expense item combination.',
            ItemRoleExpenseCombinationExistsMsg: 'The flag has been deactivated as another flag already exists for the same item role and expense item combination.',
            txtPassengerLimit: null,
            reqPassengerLimit: null,
            compPassengerLimit: null,
            txtRestrictDailyMileage: null,
            reqRestrictDailyMileage: null,
            compRestrictDailyMileage: null,
            ExpenseItemSelectionMandatoryMsg: 'Please select a list of expense items this rule should apply to. This rule cannot be applied to all expense items.',
            NoPermissionMsg: 'You do not have permission to add or update this flag rule.',
            DomIDs:
            {
                Filters: {
                    FilterModalObj: null,
                    FilterModal: {
                        Heading: null,
                        FilterDropDown: null,
                        Criteria1: null,
                        Criteria2: null,
                        Criteria1DropDown: null,
                        CriteriaListDropDown: null,
                        FilterRequiredValidator: null,
                        RequiredValidator1: null,
                        RequiredValidator2: null,
                        DataTypeValidator1: null,
                        DataTypeValidator2: null,
                        CompareValidator: null,
                        TimeValidator1: null,
                        TimeValidator2: null,
                        DateAndTimeCompareValidator: null,
                        TimeRangeCompareValidator: null,
                        CriteriaRow1: null,
                        CriteriaRow2: null,
                        CriteriaListRow: null,
                        Criteria1ImageCalc: null,
                        Criteria2ImageCalc: null,
                        Criteria1Time: null,
                        Criteria2Time: null,
                        List: null,
                        Criteria1Spacer: null,
                        Criteria2Spacer: null,
                        Criteria1Label: null,
                        Criteria2Label: null,
                        Criteria1ValidatorSpan: null,
                        Criteria2ValidatorSpan: null,
                        IntegerValidator1: null,
                        IntegerValidator2: null,
                        Runtime: null,
                        RuntimeRow: null
                    }
                }
            },
            Ids: {
                FilterDataType: null,
                FilterConditionType: null,
                List: null
            },

            configureTabs : function()
            {
                $("#tabs").tabs();
            },

            
            ddlstPeriodType_OnChange : function() {
                var ddlstPeriodType = $g(SEL.FlagsAndLimits.ddlstPeriodyTypeID);
                if (ddlstPeriodType.options[ddlstPeriodType.selectedIndex].value == '8')
                {
                    SEL.FlagsAndLimits.populateFinancialYear();
                    $g('divFinancialYear').style.display = '';
                }
                else
                {
                    $g('divFinancialYear').style.display = 'none';
                }
            },
            populateFinancialYear: function() {
                var ddlstFinancialYear = $g(SEL.FlagsAndLimits.ddlstFinancialYearID);
                if (ddlstFinancialYear.options.length == 0)
                {
                    Spend_Management.svcFlagRules.GetFinancialYearListItems(SEL.FlagsAndLimits.populateFinancialYearComplete);
                }
            },
            populateFinancialYearComplete: function (data) {
                var ddlstFinancialYear = $g(SEL.FlagsAndLimits.ddlstFinancialYearID);
                for (var i = 0; i< data.length; i++)
                {
                    var option = document.createElement("OPTION");
                    option.value = data[i].Value;
                    option.text = data[i].Text;
                    ddlstFinancialYear.options.add(option);
                }
            },
            ddlstAction_OnChange : function() {
                var ddlstAction = $g(SEL.FlagsAndLimits.ddlstActionID);
                var action = ddlstAction.options[ddlstAction.selectedIndex].value;
                var enable;
                if (action == '2') {
                    enable = false;
                    $g(SEL.FlagsAndLimits.txtNotesForAuthoriser).value = '';
                    $g(SEL.FlagsAndLimits.chkClaimantJustificationRequired).checked = false;
                    $g(SEL.FlagsAndLimits.chkApproverJustificationRequired).checked = false;
                    $g(SEL.FlagsAndLimits.chkDisplayFlagImmediately).checked = false;
                    
                } else {
                    enable = true;
                }
          
                    $g(SEL.FlagsAndLimits.txtNotesForAuthoriser).disabled = !enable;
                    $g(SEL.FlagsAndLimits.chkClaimantJustificationRequired).disabled = !enable;
                    $g(SEL.FlagsAndLimits.chkApproverJustificationRequired).disabled = !enable;
                    $g(SEL.FlagsAndLimits.chkDisplayFlagImmediately).disabled = !enable;

                    var ddlstFlagType = document.getElementById(this.ddlstFlagTypeID);
                    var flagType = ddlstFlagType.options[ddlstFlagType.selectedIndex].value;

                    $g(SEL.FlagsAndLimits.ddlstFlagLevel).disabled = !enable || (flagType == 2 || flagType == 3 || flagType == 8 || flagType == 9 || flagType == 14 || flagType == 16);
                
            },
            ddlstFrequencyType_OnChange: function() {
                var ddlstFrequencyType = $g(SEL.FlagsAndLimits.ddlstFrequencyTypeID);
                var frequencyType = parseInt(ddlstFrequencyType.options[ddlstFrequencyType.selectedIndex].value);
                var ddlstPeriodType = $g(SEL.FlagsAndLimits.ddlstPeriodyTypeID);
                var option;

                ddlstPeriodType.options.length = 0;
                switch (frequencyType)
                {
                    case 1:
                        $g('divFinancialYear').style.display = 'none';
                        option = document.createElement("OPTION");
                        option.value = '1';
                        option.text = 'Days';
                        ddlstPeriodType.options.add(option);
                        option = document.createElement("OPTION");
                        option.value = '2';
                        option.text = 'Weeks';
                        ddlstPeriodType.options.add(option);
                        option = document.createElement("OPTION");
                        option.value = '3';
                        option.text = 'Months';
                        ddlstPeriodType.options.add(option);
                        option = document.createElement("OPTION");
                        option.value = '4';
                        option.text = 'Years';
                        ddlstPeriodType.options.add(option);
                        break;
                    case 2:
                        option = document.createElement("OPTION");
                        option.value = '1';
                        option.text = 'Days';
                        ddlstPeriodType.options.add(option);
                        option = document.createElement("OPTION");
                        option.value = '5';
                        option.text = 'Calendar Weeks';
                        ddlstPeriodType.options.add(option);
                        option = document.createElement("OPTION");
                        option.value = '6';
                        option.text = 'Calendar Months';
                        ddlstPeriodType.options.add(option);
                        option = document.createElement("OPTION");
                        option.value = '7';
                        option.text = 'Calendar Years';
                        ddlstPeriodType.options.add(option);
                        option = document.createElement("OPTION");
                        option.value = '8';
                        option.text = 'Financial Years';
                        ddlstPeriodType.options.add(option);
                        break;
                }

            },
            
            ddlstDateComparisonType_OnChange: function() {
                var ddlstInvalidDateType = document.getElementById(this.ddlstInvalidDateTypeID);
                var datetype = parseInt(ddlstInvalidDateType.options[ddlstInvalidDateType.selectedIndex].value);
                var lblDateComparisonValue = document.getElementById(this.lblDateComparisonValueID);
                var txtDate = document.getElementById(this.txtDateID);
                var txtMonths = document.getElementById(this.txtMonthsID);

                switch (datetype) {
                    case 1:
                        lblDateComparisonValue.innerHTML = 'Date*';
                        lblDateComparisonValue.htmlFor = '';
                        lblDateComparisonValue.htmlFor = 'txtDate';
                        txtDate.style.display = '';
                        txtMonths.style.display = 'none';
                        $g(SEL.FlagsAndLimits.reqDate).enabled = true;
                        $g(SEL.FlagsAndLimits.compDate).enabled = true;
                        $g(SEL.FlagsAndLimits.reqMonths).enabled = false;
                        $g(SEL.FlagsAndLimits.compMonths).enabled = false;
                        $g(SEL.FlagsAndLimits.txtMonthsID).value = '';
                        break;
                    case 2:
                        lblDateComparisonValue.innerHTML = 'Number of months*';
                        txtDate.style.display = 'none';
                        txtMonths.style.display = '';
                        lblDateComparisonValue.htmlFor = txtMonths.id;
                        $g(SEL.FlagsAndLimits.reqDate).enabled = false;
                        $g(SEL.FlagsAndLimits.compDate).enabled = false;
                        $g(SEL.FlagsAndLimits.reqMonths).enabled = true;
                        $g(SEL.FlagsAndLimits.compMonths).enabled = true;
                        $g(SEL.FlagsAndLimits.txtDateID).value = '';
                        break;
                }
            },
            ddlstFlagType_OnChange: function() {
                var ddlstFlagType = document.getElementById(this.ddlstFlagTypeID);
                var flagType = parseInt(ddlstFlagType.options[ddlstFlagType.selectedIndex].value);
                var validator;
                ValidatorEnable($g(SEL.FlagsAndLimits.reqDate), false);
                ValidatorEnable($g(SEL.FlagsAndLimits.compDate), false);
                ValidatorEnable($g(SEL.FlagsAndLimits.reqMonths), false);
                ValidatorEnable($g(SEL.FlagsAndLimits.compMonths), false);
                ValidatorEnable($g(SEL.FlagsAndLimits.reqFrequency), false);
                ValidatorEnable($g(SEL.FlagsAndLimits.rangeFrequency), false);
                ValidatorEnable($g(SEL.FlagsAndLimits.reqPeriod), false);
                ValidatorEnable($g(SEL.FlagsAndLimits.rangePeriod), false);
                ValidatorEnable($g(SEL.FlagsAndLimits.reqLimit), false);
                ValidatorEnable($g(SEL.FlagsAndLimits.compLimit), false);
                ValidatorEnable($g(SEL.FlagsAndLimits.compGroupLimit), false);
                ValidatorEnable($g(SEL.FlagsAndLimits.reqGroupLimit), false);
                ValidatorEnable($g(SEL.FlagsAndLimits.rangeAmber), false);
                ValidatorEnable($g(SEL.FlagsAndLimits.rangeNoFlagTolerance), false);
                ValidatorEnable($g(SEL.FlagsAndLimits.reqTipLimit), false);
                ValidatorEnable($g(SEL.FlagsAndLimits.rangeTipLimit), false);
                ValidatorEnable($g(SEL.FlagsAndLimits.reqPassengerLimit), false);
                ValidatorEnable($g(SEL.FlagsAndLimits.compPassengerLimit), false);
                ValidatorEnable($g(SEL.FlagsAndLimits.reqRestrictDailyMileage), false);
                ValidatorEnable($g(SEL.FlagsAndLimits.compRestrictDailyMileage), false);

                SEL.FlagsAndLimits.ToggleExpenseItemInclusion(flagType);
                $g('divPassengerLimitRule').style.display = 'none';
                $g('divRestrictDailyMileage').style.display = 'none';
                $g('divCustomRule').style.display = 'none';
                $g('divReceiptLimitRule').style.display = 'none';
                document.getElementById('divLimitRule').style.display = 'none';
                document.getElementById('divTipLimitRule').style.display = 'none';
                document.getElementById('divInvalidDateRule').style.display = 'none';
                document.getElementById('divFrequencyRule').style.display = 'none';
                document.getElementById('divGroupLimitRule').style.display = 'none';
                document.getElementById('divFields').style.display = 'none';
                document.getElementById(SEL.FlagsAndLimits.txtLimitID).style.display = 'none';
                document.getElementById(SEL.FlagsAndLimits.txtFrequencyID).style.display = 'none';
                switch (flagType) {
                    case 1:
                        document.getElementById('divFields').style.display = '';
                        SEL.FlagsAndLimits.ToggleToleranceReadOnly(false);
                        break;
                    case 2:
                    case 3:
                        document.getElementById('divLimitRule').style.display = '';
                        $g('divReceiptLimitRule').style.display = '';
                        $g(SEL.FlagsAndLimits.rangeAmber).enabled = true;
                        $g(SEL.FlagsAndLimits.rangeNoFlagTolerance).enabled = true;
                        SEL.FlagsAndLimits.ToggleToleranceReadOnly(true);
                        if (flagType == 2) {
                            $g('divShowLimitToClaimant').style.display = 'none';
                        } else {
                            $g('divShowLimitToClaimant').style.display = '';
                        }
                        break;
                    case 5:
                        document.getElementById('divInvalidDateRule').style.display = '';
                        SEL.FlagsAndLimits.ddlstDateComparisonType_OnChange();
                        SEL.FlagsAndLimits.ToggleToleranceReadOnly(false);
                        break;
                    case 6:
                        document.getElementById('divFrequencyRule').style.display = '';
                        document.getElementById(SEL.FlagsAndLimits.lblFrequencyID).innerHTML = 'Frequency*';
                        document.getElementById(SEL.FlagsAndLimits.txtFrequencyID).style.display = '';
                        $g(SEL.FlagsAndLimits.reqFrequency).enabled = true;
                        $g(SEL.FlagsAndLimits.rangeFrequency).enabled = true;
                        $g(SEL.FlagsAndLimits.reqPeriod).enabled = true;
                        $g(SEL.FlagsAndLimits.rangePeriod).enabled = true;
                        
                        SEL.FlagsAndLimits.ddlstPeriodType_OnChange();
                        SEL.FlagsAndLimits.ddlstFrequencyType_OnChange();
                        SEL.FlagsAndLimits.ToggleToleranceReadOnly(false);
                        break;
                    case 7:
                        document.getElementById('divFrequencyRule').style.display = '';
                        document.getElementById(SEL.FlagsAndLimits.lblFrequencyID).innerHTML = 'Limit*';
                        document.getElementById(SEL.FlagsAndLimits.txtLimitID).style.display = '';
                        $g(SEL.FlagsAndLimits.reqLimit).enabled = true;
                        $g(SEL.FlagsAndLimits.compLimit).enabled = true;
                        $g(SEL.FlagsAndLimits.reqPeriod).enabled = true;
                        $g(SEL.FlagsAndLimits.rangePeriod).enabled = true;
                        SEL.FlagsAndLimits.ddlstPeriodType_OnChange();
                        SEL.FlagsAndLimits.ddlstFrequencyType_OnChange();
                        SEL.FlagsAndLimits.ToggleToleranceReadOnly(false);
                        break;
                    case 8:
                    case 9:
                        document.getElementById('divLimitRule').style.display = '';
                        document.getElementById('divGroupLimitRule').style.display = '';
                        $g(SEL.FlagsAndLimits.compGroupLimit).enabled = true;
                        $g(SEL.FlagsAndLimits.reqGroupLimit).enabled = true;
                        $g(SEL.FlagsAndLimits.rangeAmber).enabled = true;
                        $g(SEL.FlagsAndLimits.rangeNoFlagTolerance).enabled = true;
                        SEL.FlagsAndLimits.ToggleToleranceReadOnly(true);
                        break;
                    case 10:
                        $g('divCustomRule').style.display = '';
                        if (!SEL.FlagsAndLimits.CustomCriteriaPopulated) {
                            SEL.Reports.IDs.General.ReportOn = 'txtGuid';
                            SEL.FlagsAndLimits.Refresh();
                            SEL.FlagsAndLimits.CustomCriteriaPopulated = true;
                        }
                        SEL.FlagsAndLimits.ToggleToleranceReadOnly(false);
                        break;
                    case 14:
                        $g(SEL.FlagsAndLimits.reqTipLimit).enabled = true;
                        $g(SEL.FlagsAndLimits.rangeTipLimit).enabled = true;
                        document.getElementById('divLimitRule').style.display = '';
                        document.getElementById('divTipLimitRule').style.display = '';
                        $g(SEL.FlagsAndLimits.rangeAmber).enabled = true;
                        $g(SEL.FlagsAndLimits.rangeNoFlagTolerance).enabled = true;
                        SEL.FlagsAndLimits.ToggleToleranceReadOnly(true);
                        break;
                    case 16:
                        document.getElementById('divLimitRule').style.display = '';
                        $g(SEL.FlagsAndLimits.rangeNoFlagTolerance).enabled = true;
                        $g(SEL.FlagsAndLimits.rangeNoFlagTolerance).enabled = true;
                        SEL.FlagsAndLimits.ToggleToleranceReadOnly(true);
                        break;
                    case 19:
                        $g('divPassengerLimitRule').style.display = '';
                        $g(SEL.FlagsAndLimits.reqPassengerLimit).enabled = true;
                        $g(SEL.FlagsAndLimits.compPassengerLimit).enabled = true;
                        SEL.FlagsAndLimits.ToggleToleranceReadOnly(false);
                        break;
                    case 22:
                        $g('divRestrictDailyMileage').style.display = '';
                        $g(SEL.FlagsAndLimits.reqRestrictDailyMileage).enabled = true;
                        $g(SEL.FlagsAndLimits.compRestrictDailyMileage).enabled = true;
                        SEL.FlagsAndLimits.ToggleToleranceReadOnly(false);
                    default:
                        
                        SEL.FlagsAndLimits.ToggleToleranceReadOnly(false);
                        break;
                }
            },
            ToggleExpenseItemInclusion : function(flagType) {
                Spend_Management.svcFlagRules.GetFlagInclusionType(flagType, SEL.FlagsAndLimits.ToggleExpenseItemInclusionComplete);
            },
            ToggleExpenseItemInclusionComplete : function(inclusionType) {
                var ddlst = $g(SEL.FlagsAndLimits.ddlstExpenseItemInclusionType);
                if (inclusionType == 1) {
                    ddlst.disabled = false;
                } else {
                    ddlst.disabled = true;
                    ddlst.selectedIndex = 1;
                }
                SEL.FlagsAndLimits.ToggleExpenseItemsGrid();
            },
            ToggleToleranceReadOnly : function(enabled) {
                var ddlstFlagLevel = $g(SEL.FlagsAndLimits.ddlstFlagLevel);
                ddlstFlagLevel.disabled = enabled;
            },
            saveFlagRule: function (validateSelectedExpenseItems) {
                if (validateform('vgFlag') == false) { return; }

                var ddlstFlagType = document.getElementById(this.ddlstFlagTypeID);
                var ddlstAction = document.getElementById(this.ddlstActionID);
                var txtFlagText = document.getElementById(this.txtFlagTextID);
                var ddlstFlagLevel = $g(SEL.FlagsAndLimits.ddlstFlagLevel);
                var txtAmberTolerance = document.getElementById(this.txtAmberToleranceID);
                var ddlstInvalidDateType = document.getElementById(this.ddlstInvalidDateTypeID);
                var txtFrequency = document.getElementById(this.txtFrequencyID);
                var ddlstFrequencyType = $g(SEL.FlagsAndLimits.ddlstFrequencyTypeID);
                var txtPeriod = document.getElementById(this.txtPeriodID);
                var ddlstPeriodType = document.getElementById(this.ddlstPeriodyTypeID);
                var txtDate = document.getElementById(this.txtDateID);
                var txtMonths = document.getElementById(this.txtMonthsID);
                var txtLimit = document.getElementById(this.txtLimitID);
                var txtGroupLimit = document.getElementById(this.txtGroupLimitID);
                var flagType = parseInt(ddlstFlagType.options[ddlstFlagType.selectedIndex].value);
                var action = parseInt(ddlstAction.options[ddlstAction.selectedIndex].value);
                var flagtext = txtFlagText.value;
                var amberTolerance = null;
                var description = document.getElementById(this.txtDescription).value;
                var active = document.getElementById(this.chkActive).checked;
                var claimantJustificationRequired = $g(SEL.FlagsAndLimits.chkClaimantJustificationRequired).checked;
                var displayFlagImmediately = $g(SEL.FlagsAndLimits.chkDisplayFlagImmediately).checked;
                var txtNoFlagTolerance = $g(SEL.FlagsAndLimits.txtNoFlagTolerance);
                var financialYear;
                var txtTipLimit = $g(SEL.FlagsAndLimits.txtTipLimit);
                var tipLimit = null;
                var approverJustificationRequired = $g(SEL.FlagsAndLimits.chkApproverJustificationRequired).checked;
                var flagLevel = ddlstFlagLevel.options[ddlstFlagLevel.selectedIndex].value;
                var increasebynumothers = $g(SEL.FlagsAndLimits.chkIncreaseByNumOthers).checked;
                var displaylimit = $g(SEL.FlagsAndLimits.chkDisplayLimit).checked;
                var notesForAuthoriser = $g(SEL.FlagsAndLimits.txtNotesForAuthoriser).value;
                var ddlstItemRoleInclusionType = $g(SEL.FlagsAndLimits.ddlstItemRoleInclusionType);
                var itemRoleInclusionType = ddlstItemRoleInclusionType.options[ddlstItemRoleInclusionType.selectedIndex].value;
                var ddlstExpenseItemInclusionType = $g(SEL.FlagsAndLimits.ddlstExpenseItemInclusionType);
                var expenseItemInclusionType = ddlstExpenseItemInclusionType.options[ddlstExpenseItemInclusionType.selectedIndex].value;
                var txtPassengerLimit = $g(SEL.FlagsAndLimits.txtPassengerLimit);
                var txtRestrictDailyMileage = $g(SEL.FlagsAndLimits.txtRestrictDailyMileage);
                var passengerLimit;
                var dailyMileage;
                if (txtAmberTolerance.value != "") {
                    amberTolerance = txtAmberTolerance.value;
                }
                
                var noFlagTolerance = null;
                if (txtTipLimit.value != "")
                {
                    tipLimit = txtTipLimit.value;
                }
                if (txtNoFlagTolerance.value != "")
                {
                    noFlagTolerance = txtNoFlagTolerance.value;
                }
                var invaliddatetype = ddlstInvalidDateType.options[ddlstInvalidDateType.selectedIndex].value;
                var date = null;
                var formattedDate = null;
                if (txtDate.value != "") {
                    date = $("#" + SEL.FlagsAndLimits.txtDateID).datepicker("getDate");
                    formattedDate = date.format('yyyy/MM/dd');
                }

                var months = null;
                if (txtMonths.value != "") {
                    months = txtMonths.value;
                }
                var frequency = null;
                if (txtFrequency.value != "") {
                    frequency = txtFrequency.value;
                }
                var frequencyType = ddlstFrequencyType.options[ddlstFrequencyType.selectedIndex].value;
                var period = null;
                if (txtPeriod.value != "") {
                    period = txtPeriod.value;
                }
                var periodtype = ddlstPeriodType.options[ddlstPeriodType.selectedIndex].value;
                var limit = null;
                if (flagType == 7) {
                    if (txtLimit.value != "") {
                        limit = txtLimit.value;
                    }
                }
                else if (flagType == 8 || flagType == 9) {
                    if (txtGroupLimit.value != "") {
                        limit = txtGroupLimit.value;
                    }
                }

                if ((flagType == 6 || flagType == 7) && periodtype == 8) {
                    var ddlstFinancialYear = $g(SEL.FlagsAndLimits.ddlstFinancialYearID);
                    financialYear = ddlstFinancialYear.options[ddlstFinancialYear.selectedIndex].value;
                } else {
                    financialYear = null;
                }

                if (flagType == 19) {
                    passengerLimit = txtPassengerLimit.value;
                } else {
                    passengerLimit = null;
                }

                if (flagType === 22) {
                    dailyMileage = txtRestrictDailyMileage.value;
                } else {
                    dailyMileage = null;
                }
                var reportCriteria = null;
                if (flagType == 10)
                {
                    reportCriteria = SEL.Trees.Tree.Data.Get(SEL.Reports.IDs.CriteriaSelector.Drop, ['metadata']);
                }
                
                Spend_Management.svcFlagRules.SaveFlagRule(SEL.FlagsAndLimits.flagID, flagType, action, flagtext, invaliddatetype, formattedDate, months, amberTolerance, frequency, frequencyType, period, periodtype, limit, description, active, claimantJustificationRequired, displayFlagImmediately, noFlagTolerance, financialYear, tipLimit, flagLevel, approverJustificationRequired, increasebynumothers, displaylimit, reportCriteria, notesForAuthoriser, itemRoleInclusionType, expenseItemInclusionType, passengerLimit, validateSelectedExpenseItems, dailyMileage, SEL.FlagsAndLimits.saveFlagRuleComplete);
            },

            saveFlagRuleComplete: function (data) {
                if (data == -1 || data == -4) //hasn't selected an expense list
                {
                    SEL.MasterPopup.ShowMasterPopup(SEL.FlagsAndLimits.ExpenseItemSelectionMandatoryMsg);
                    return;
                }

                if (data == -2) {
                    SEL.MasterPopup.ShowMasterPopup(SEL.FlagsAndLimits.NoPermissionMsg);
                    return;
                }

                if (data == -3) {
                    SEL.MasterPopup.ShowMasterPopup(SEL.FlagsAndLimits.ItemRoleExpenseCombinationExistsMsg);
                    $g(SEL.FlagsAndLimits.chkActive).checked = false;
                    return;
                }

                SEL.FlagsAndLimits.flagID = data;

                switch (SEL.FlagsAndLimits.currentAction) {
                    case 'saveRoles':
                        SEL.FlagsAndLimits.popupRolesModal(false);
                        break;
                    case 'saveFlagRuleExpenseItems':
                        SEL.FlagsAndLimits.popupExpenseItemsModal(false);
                        break;
                    case 'saveFields':
                        SEL.FlagsAndLimits.popupFieldsModal(false);
                        break;
                    default:
                        document.location = 'flags.aspx';
                        break;
                }

            },
            populateSelectedRoles: function () {
                Spend_Management.svcFlagRules.CreateRolesGrid(SEL.FlagsAndLimits.flagID, SEL.FlagsAndLimits.createRolesGridComplete);
                
            },
            createRolesGridComplete : function (data)
            {
                $g('gridSelectedRoles').innerHTML = data[1];
                SEL.Grid.updateGrid(data[0]);

            },
            populateRoles: function () {
                Spend_Management.svcFlagRules.GetRoles(SEL.FlagsAndLimits.flagID, SEL.FlagsAndLimits.getRolesComplete);
                
            },
            getRolesComplete : function (data)
            {
                
                $g('divGridRoles').innerHTML = data[1];
                    SEL.Grid.updateGrid(data[0]);
                
    },
            popupRolesModal: function(save) {

                SEL.FlagsAndLimits.currentAction = 'saveRoles';

                if (save) {
                    this.saveFlagRule(false);
                    return;
                }
                $("#divItemRolesModal").css('max-height', '550px');
                $("#divItemRolesModal").dialog("open");
                return;
            },

            hideRolesModal: function() {
                $("#divItemRolesModal").dialog("close");
                return;
            },

            saveItemRoles: function() {
                var arrSplit;
                arrSplit = SEL.Grid.getSelectedItemsFromGrid('gridModalRoles');
                Spend_Management.svcFlagRules.SaveItemRoles(SEL.FlagsAndLimits.flagID, arrSplit, SEL.FlagsAndLimits.saveItemRolesComplete);
            },

            saveItemRolesComplete: function (result) {
                if (result == -1) {
                    SEL.MasterPopup.ShowMasterPopup(SEL.FlagsAndLimits.ItemRoleExistsMsg);
                } else {
                    SEL.FlagsAndLimits.hideRolesModal();
                    SEL.FlagsAndLimits.populateSelectedRoles();
                }
            },
            populateSelectedExpenseItems: function () {
                Spend_Management.svcFlagRules.CreateExpenseItemGrid(SEL.FlagsAndLimits.flagID, SEL.FlagsAndLimits.createExpenseItemGridComplete);
                
            },
            createExpenseItemGridComplete : function(data)
            {
                $g('divGridSelectedExpenseItems').innerHTML = data[1];
                SEL.Grid.updateGrid(data[0]);
            },
            populateExpenseItems: function () {
                Spend_Management.svcFlagRules.GetExpenseItems(SEL.FlagsAndLimits.flagID, SEL.FlagsAndLimits.getExpenseItemsComplete);
                

            },
            getExpenseItemsComplete : function(data)
            {
                $g('divGridExpenseItems').innerHTML = data[1];
                SEL.Grid.updateGrid(data[0]);
            },
            popupExpenseItemsModal: function(save) {

                SEL.FlagsAndLimits.currentAction = 'saveFlagRuleExpenseItems';

                if (save) {
                    this.saveFlagRule(false);
                    return;
                }

                $("#divExpenseItemsModal").css('max-height', '550px');
                $("#divExpenseItemsModal").dialog("open");
                return;
            },

            hideExpenseItemsModal: function() {
                $("#divExpenseItemsModal").dialog("close");
                return;
            },

            saveExpenseItems: function() {
                var arrSplit;
                arrSplit = SEL.Grid.getSelectedItemsFromGrid('gridModalExpenseItems');
                Spend_Management.svcFlagRules.SaveFlagRuleExpenseItems(SEL.FlagsAndLimits.flagID, arrSplit, SEL.FlagsAndLimits.saveFlagRuleExpenseItemsComplete);
            },

            saveFlagRuleExpenseItemsComplete: function (result) {
                if (result == -1) {
                    SEL.MasterPopup.ShowMasterPopup(SEL.FlagsAndLimits.ExpenseItemExistsMsg);
                } else {
                    SEL.FlagsAndLimits.hideExpenseItemsModal();
                    SEL.FlagsAndLimits.populateSelectedExpenseItems();
                }
            },

            displayFlagInformation: function(claimID, expenseIDs) {
                SEL.FlagsAndLimits.populateFlagInformation(claimID, expenseIDs);
                $find(SEL.FlagsAndLimits.modFlagInformationID).show();
            },

            populateFlagInformation: function(claimID, expenseIDs) {
                var behaviour = $find(SEL.FlagsAndLimits.dynFlagInformationID);

                if (behaviour) {
                    behaviour.populate(claimID + "," + expenseIDs);
                }
            },
            deleteFlagRule: function(flagID) {
                SEL.FlagsAndLimits.flagID = flagID;
                if (confirm('Are you sure you wish to delete the selected rule?')) {
                    Spend_Management.svcFlagRules.DeleteFlagRule(flagID, SEL.FlagsAndLimits.deleteFlagRuleComplete
                    );
                }
            },
            deleteFlagRuleComplete: function (data) {

                if (data == -1) {
                    SEL.MasterPopup.ShowMasterPopup('This flag rule cannot be deleted as it has been used. Please deactivate it instead.');
                    return;
                }
                SEL.Grid.refreshGrid('gridFlags', 1);
            },
            deleteAssociatedItemRole : function(itemRoleID) {
                Spend_Management.svcFlagRules.DeleteAssociatedItemRole(SEL.FlagsAndLimits.flagID, itemRoleID, SEL.FlagsAndLimits.deleteAssociatedItemRoleComplete);
            },
            deleteAssociatedItemRoleComplete : function() {
                SEL.Grid.refreshGrid('gridRoles');
            },
            deleteAssociatedExpenseItem: function (subcatID) {
                Spend_Management.svcFlagRules.DeleteAssociatedExpenseItem(SEL.FlagsAndLimits.flagID, subcatID, SEL.FlagsAndLimits.deleteAssociatedExpenseItemComplete);
            },
            deleteAssociatedExpenseItemComplete: function () {
                SEL.Grid.refreshGrid('gridExpenseItems');
            },
            showFlagModal: function() {

                var modal = $find(SEL.FlagsAndLimits.modFlagInformation);
                modal.show();
            },

            hideFlagModal: function() {

                var modal = $find(SEL.FlagsAndLimits.modFlagInformation);
                modal.hide();
            },
            populateSelectedFields: function() {
                Spend_Management.svcFlagRules.CreateFieldsGrid(SEL.FlagsAndLimits.flagID, SEL.FlagsAndLimits.populateSelectedFieldsComplete)
            },
            populateSelectedFieldsComplete: function (data) {
                $g('divSelectedFieldList').innerHTML = data[1];
                SEL.Grid.updateGrid(data[0]);
                
                
            },
            populateFields: function() {
                Spend_Management.svcFlagRules.GetFields(SEL.FlagsAndLimits.flagID, SEL.FlagsAndLimits.populateFieldsComplete)

            },
            populateFieldsComplete: function (data) {
                $g('divFieldList').innerHTML = data[1];
                SEL.Grid.updateGrid(data[0]);
                
            },
            popupFieldsModal: function(save) {

                SEL.FlagsAndLimits.currentAction = 'saveFields';

                if (save) {
                    this.saveFlagRule(false);
                    return;
                }
                $("#divFieldsModal").css('max-height', '550px');
                $("#divFieldsModal").dialog("open");
                
                return;
            },

            hideFieldsModal: function() {
                $("#divFieldsModal").dialog("close");
                return;
            },

            saveFields: function() {
                var arrSplit = new Array();
                arrSplit = SEL.Grid.getSelectedItemsGuidFromGrid('gridModalFields');
                Spend_Management.svcFlagRules.SaveFields(SEL.FlagsAndLimits.flagID, arrSplit, SEL.FlagsAndLimits.saveFieldsComplete);
            },

            saveFieldsComplete: function() {
                SEL.FlagsAndLimits.hideFieldsModal();
                SEL.FlagsAndLimits.populateSelectedFields();
            },

            Refresh: function () {
                var params = new SEL.Reports.Misc.WebserviceParameters.GetInitialNodes();
                SEL.Ajax.Service('/shared/webServices/svcReports.asmx/', 'GetInitialTreeNodes', params, SEL.FlagsAndLimits.RefreshComplete, SEL.Reports.Misc.ErrorHandler);

            },

            RefreshComplete: function (data) {
                var treeId = SEL.Reports.IDs.CriteriaSelector.Tree;
                if (data !== null && data !== undefined) {
                    SEL.Trees.Tree.Data.Set(treeId, data.d);
                }
                if (SEL.FlagsAndLimits.flagID !== null) {
                    var params = SEL.FlagsAndLimits.flagID;
                    SEL.Ajax.Service('/shared/webServices/svcReports.asmx/', 'GetSelectedFilterData', 'aaa', SEL.FlagsAndLimits.CriteriaNodesRefreshComplete, SEL.Reports.Misc.ErrorHandler);
                    SEL.Reports.CriteriaLoaded = true;
                }
            },
            CriteriaNodesRefreshComplete: function (data) {
                var treeId = SEL.Reports.IDs.CriteriaSelector.Drop;
                SEL.Trees.DomIDs.Filters.Drop = treeId;
                if (data !== null && data !== undefined) {
                    SEL.Trees.Tree.Data.Set(treeId, data.d);
                }
            },
            Clear: function () {
                var treeId = SEL.Reports.IDs.CriteriaSelector.Drop;
                SEL.Trees.Tree.Clear(treeId);
            },

            DeleteFlagField : function(fieldID) {
                Spend_Management.svcFlagRules.DeleteFlagField(SEL.FlagsAndLimits.flagID, fieldID, SEL.FlagsAndLimits.DeleteFlagFieldComplete)
            },

            DeleteFlagFieldComplete : function(data) {
                if (data == -1) {
                    SEL.MasterPopup.ShowMasterPopup(SEL.FlagsAndLimits.NoPermissionMsg);
                    return;
                }

                SEL.Grid.refreshGrid('gridFlagFields');
            },

            ToggleDisplayItemRoleGrid: function () {
                var ddlst = $g(SEL.FlagsAndLimits.ddlstItemRoleInclusionType);
                var gridDiv = $g('divItemRoles');
                if (ddlst.options[ddlst.selectedIndex].value == '1') {
                    gridDiv.style.display = 'none';
                } else {
                    gridDiv.style.display = '';
                }
            },

            ToggleExpenseItemsGrid: function () {
                var ddlst = $g(SEL.FlagsAndLimits.ddlstExpenseItemInclusionType);
                var gridDiv = $g('divExpenseItems');
                if (ddlst.options[ddlst.selectedIndex].value == '1') {
                    gridDiv.style.display = 'none';
                } else {
                    gridDiv.style.display = '';
                }
            },

            configureItemRoleModal: function () {
                $("#divItemRolesModal").dialog({
                    autoOpen: false,
                    resizable: false,
                    title: "Add Item Roles",
                    width: 550,
                    modal: true,
                    buttons: [
                        {
                            text: 'save',
                            id: 'btnItemRoleSave',
                            click: function () {
                                SEL.FlagsAndLimits.saveItemRoles();
                            }
                        }, { text: 'cancel', id: 'btnItemRoleCancel', click: function () { $(this).dialog('close'); } }
                    ],
                    close: function () {

                    }
                });

                $('#divItemRolesModal').keyup(function (e) {
                    if (e.keyCode == 13) {
                        SEL.FlagsAndLimits.saveItemRoles();
                    }
                });

                $('#btnItemRoleSave').keyup(function (e) {
                    if (e.keyCode == 13) {
                        SEL.FlagsAndLimits.saveItemRoles();
                    }
                });

                $('#btnItemRoleCancel').keyup(function (e) {
                    if (e.keyCode == 13) {
                        $("#divItemRolesModal").dialog('close');
                    }
                });
                //$("#dialog").dialog({ resizable: true, autoOpen: false, minWidth: 300, maxWidth: 300, height: 300, zIndex: SEL.CustomEntityAdministration.zIndices.Forms.AvailableFieldsModal(), stack: false, enableDock: true });
            },

            configureExpenseItemModal: function () {
                $("#divExpenseItemsModal").dialog({
                    autoOpen: false,
                    resizable: false,
                    title: "Add Expense Items",
                    width: 550,
                    modal: true,
                    buttons: [
                        {
                            text: 'save',
                            id: 'btnExpenseItemSave',
                            click: function () {
                                SEL.FlagsAndLimits.saveExpenseItems();
                            }
                        }, { text: 'cancel', id: 'btnExpenseItemCancel', click: function () { $(this).dialog('close'); } }
                    ],
                    close: function () {

                    }
                });

                $('#divExpenseItemsModal').keyup(function (e) {
                    if (e.keyCode == 13) {
                        SEL.FlagsAndLimits.saveExpenseItems();
                    }
                });

                $('#btnExpenseItemSave').keyup(function (e) {
                    if (e.keyCode == 13) {
                        SEL.FlagsAndLimits.saveExpenseItems();
                    }
                });

                $('#btnExpenseItemCancel').keyup(function (e) {
                    if (e.keyCode == 13) {
                        $('#divExpenseItemsModal').dialog('close');
                    }
                });
                //$("#dialog").dialog({ resizable: true, autoOpen: false, minWidth: 300, maxWidth: 300, height: 300, zIndex: SEL.CustomEntityAdministration.zIndices.Forms.AvailableFieldsModal(), stack: false, enableDock: true });
            },

            configureFieldModal: function () {
                $("#divFieldsModal").dialog({
                    autoOpen: false,
                    resizable: false,
                    title: "Add Fields",
                    width: 550,
                    modal: true,
                    buttons: [
                        {
                            text: 'save',
                            id: 'btnFieldSave',
                            click: function () {
                                SEL.FlagsAndLimits.saveFields();
                            }
                        }, { text: 'cancel', id: 'btnFieldCancel', click: function () { $(this).dialog('close'); } }
                    ],
                    close: function () {

                    }
                });

                $('#divFieldsModal').keyup(function (e) {
                    if (e.keyCode == 13) {
                        SEL.FlagsAndLimits.saveFields();
                    }
                });

                $('#btnFieldSave').keyup(function (e) {
                    if (e.keyCode == 13) {
                        SEL.FlagsAndLimits.saveFields();
                    }
                });

                $('#btnFieldCancel').keyup(function (e) {
                    if (e.keyCode == 13) {
                        $("#divFieldsModal").dialog('close');
                    }
                });
                //$("#dialog").dialog({ resizable: true, autoOpen: false, minWidth: 300, maxWidth: 300, height: 300, zIndex: SEL.CustomEntityAdministration.zIndices.Forms.AvailableFieldsModal(), stack: false, enableDock: true });
            }
        };
    }
    if (window.Sys && Sys.loader) {
        Sys.loader.registerScript(scriptName, null, execute);
    }
    else {
        execute();
    }

}
)();
var currentAction;
var enableClass1 = false;
function ddlstDateRange_onchange() {
    var dateRange = document.getElementById(ddlstDateRange).options[document.getElementById(ddlstDateRange).selectedIndex].value;
    
    switch (dateRange) {
        case '0':
        case '1':
            document.getElementById('spanRangeStart').style.display = '';
            document.getElementById('spanRangeEnd').style.display = 'none';
            ValidatorEnable(document.getElementById(reqstartdate), true);
            ValidatorEnable(document.getElementById(reqenddate), false);
            break;
        case '2':
            document.getElementById('spanRangeStart').style.display = '';
            document.getElementById('spanRangeEnd').style.display = '';
            document.getElementById(txtVATStartDate).disabled = false;
            document.getElementById(txtVATEndDate).disabled = false;
            ValidatorEnable(document.getElementById(reqstartdate), true);
            ValidatorEnable(document.getElementById(reqenddate), true);
            break;
        case '3':
            document.getElementById('spanRangeStart').style.display = 'none';
            document.getElementById('spanRangeEnd').style.display = 'none';
            ValidatorEnable(document.getElementById(reqstartdate), false);
            ValidatorEnable(document.getElementById(reqenddate), false);
            break;
    }
}

function setupEditCalculationDiv() {
    var calculation = document.getElementById(ddlstCalculation).options[document.getElementById(ddlstCalculation).selectedIndex].value;

    document.getElementById('divCalculation2').style.display = 'none';
    document.getElementById('divCalculation3').style.display = 'none';
    document.getElementById('divCalculation4').style.display = 'none';
    document.getElementById('divCalculation7').style.display = 'none';
    document.getElementById('divCalculation8').style.display = 'none';
    document.getElementById('divDoc').style.display = 'none';
    switch (calculation) {
        case '1':
            break;
        case '2':
            document.getElementById('divCalculation2').style.display = '';
            break;
        case '3':
            document.getElementById(chkmileage).disabled = true;
            document.getElementById('divCalculation3').style.display = '';
            if ($('#' + optDeductFixed).is(":checked")) {
                $('.fixedMiles').show();
            } else {
                $('.fixedMiles').hide();
            }
            SetMaximumMilesTextbox();
            setRotationalMileageRelatedFields();
            document.getElementById('divDoc').style.display = '';
            setDocOptions();
            break;
        case '4':
            document.getElementById('divCalculation4').style.display = '';
            break;
        case '5':
            document.getElementById(chkreimbursable).checked = false;
            document.getElementById(chkreimbursable).disabled = true;
            break;
        case '6':
            document.getElementById(chkmileage).checked = true;
            document.getElementById(chkmileage).disabled = true;
            document.getElementById('divDoc').style.display = '';
            setDocOptions();
            break;
        case '7':
            document.getElementById('divCalculation7').style.display = '';
            break;
        case '8':
            document.getElementById('divCalculation8').style.display = '';
            break;
        case '10':
            document.getElementById('divCalculation3').style.display = '';
            $("#divCalculation3").find("div:not(:first-child)").css("display", "none");
            document.getElementById('divDoc').style.display = '';
            break;
    }

}

function changeCalculationDiv() {
    var calculation = document.getElementById(ddlstCalculation).options[document.getElementById(ddlstCalculation).selectedIndex].value;

    document.getElementById('divCalculation2').style.display = 'none';
    document.getElementById('divCalculation3').style.display = 'none';
    document.getElementById('divCalculation4').style.display = 'none';
    document.getElementById('divCalculation7').style.display = 'none';
    document.getElementById('divCalculation8').style.display = 'none';
    document.getElementById('divDoc').style.display = 'none';
    
    switch (calculation) {
        case '1':
            document.getElementById(chkmileage).checked = false;
            document.getElementById(chkstaff).checked = false;
            document.getElementById(chkothers).checked = false;
            document.getElementById(chknodirectors).checked = false;
            document.getElementById(chkmileage).disabled = false;
            document.getElementById(chkstaff).disabled = false;
            document.getElementById(chkothers).disabled = false;
            document.getElementById(chknodirectors).disabled = false;
            document.getElementById(chkreimbursable).disabled = false;            
            break;
        case '2':
            document.getElementById(chkmileage).checked = false;
            document.getElementById(chkstaff).checked = true;
            document.getElementById(chkothers).checked = true;
            document.getElementById(chknodirectors).checked = true;
            document.getElementById(chkmileage).disabled = false;
            document.getElementById(chkstaff).disabled = true;
            document.getElementById(chkothers).disabled = true;
            document.getElementById(chknodirectors).disabled = true;
            document.getElementById(chkreimbursable).disabled = false;
            document.getElementById('divCalculation2').style.display = '';
            break;            
        case '3':
            document.getElementById(chkmileage).checked = true;
            document.getElementById(chkstaff).checked = false;
            document.getElementById(chkothers).checked = false;
            document.getElementById(chknodirectors).checked = false;
            document.getElementById(chkmileage).disabled = true;
            document.getElementById(chkstaff).disabled = false;
            document.getElementById(chkothers).disabled = false;
            document.getElementById(chknodirectors).disabled = false;
            document.getElementById(chkreimbursable).disabled = false;
            document.getElementById('divCalculation3').style.display = '';
            document.getElementById((chkenablehometooffice)).checked = false; 
            document.getElementById('divDoc').style.display = '';
            $("#divCalculation3 div").css("display", "");
            $("#" + txtDeductFixed).val('');
            $('#hometoOfficeOptions').hide();
            $('.hometoOfficeZeroMiles').hide();
            if ($('#' + optDeductFixed).is(":checked")) {
                $('.fixedMiles').show();
            } else {
                $('.fixedMiles').hide();
            }
            $("#" + ddlstPublicTransportRate).val('');
            setRotationalMileageRelatedFields();
            break;
        case '4':
            document.getElementById(chkmileage).checked = false;
            document.getElementById(chkstaff).checked = false;
            document.getElementById(chkothers).checked = false;
            document.getElementById(chknodirectors).checked = false;
            document.getElementById(chkmileage).disabled = false;
            document.getElementById(chkstaff).disabled = false;
            document.getElementById(chkothers).disabled = false;
            document.getElementById(chknodirectors).disabled = false;
            document.getElementById(chkreimbursable).disabled = false;
            document.getElementById('divCalculation4').style.display = '';
            break;
        case '5':
            document.getElementById(chkmileage).checked = false;
            document.getElementById(chkstaff).checked = false;
            document.getElementById(chkothers).checked = false;
            document.getElementById(chknodirectors).checked = false;
            document.getElementById(chkmileage).disabled = false;
            document.getElementById(chkstaff).disabled = false;
            document.getElementById(chkothers).disabled = false;
            document.getElementById(chknodirectors).disabled = false;
            document.getElementById(chkreimbursable).checked = false;
            document.getElementById(chkreimbursable).disabled = true;
            break;
        case '6':
            document.getElementById(chkstaff).checked = false;
            document.getElementById(chkothers).checked = false;
            document.getElementById(chknodirectors).checked = false;
            document.getElementById(chkstaff).disabled = false;
            document.getElementById(chkothers).disabled = false;
            document.getElementById(chknodirectors).disabled = false;
            document.getElementById(chkmileage).checked = true;
            document.getElementById(chkmileage).disabled = true;
            document.getElementById(chkreimbursable).disabled = false;
            document.getElementById('divDoc').style.display = '';
            break;
        case '7':
            document.getElementById(chkmileage).checked = false;
            document.getElementById(chkstaff).checked = false;
            document.getElementById(chkothers).checked = false;
            document.getElementById(chknodirectors).checked = false;
            document.getElementById(chkmileage).disabled = false;
            document.getElementById(chkstaff).disabled = false;
            document.getElementById(chkothers).disabled = false;
            document.getElementById(chknodirectors).disabled = false;
            document.getElementById(chkreimbursable).disabled = false;
            document.getElementById('divCalculation7').style.display = '';
            break;
        case '8':
            document.getElementById(chkmileage).checked = false;
            document.getElementById(chkstaff).checked = false;
            document.getElementById(chkothers).checked = false;
            document.getElementById(chknodirectors).checked = false;
            document.getElementById(chkmileage).disabled = true;
            document.getElementById(chkstaff).disabled = false;
            document.getElementById(chkothers).disabled = false;
            document.getElementById(chknodirectors).disabled = false;
            document.getElementById(chkreimbursable).disabled = false;
            document.getElementById('divCalculation8').style.display = '';
            break;
        case '10':
            document.getElementById(chkmileage).checked = false;
            document.getElementById(chkstaff).checked = false;
            document.getElementById(chkothers).checked = false;
            document.getElementById(chknodirectors).checked = false;
            document.getElementById(chkmileage).disabled = false;
            document.getElementById(chkstaff).disabled = false;
            document.getElementById(chkothers).disabled = false;
            document.getElementById(chknodirectors).disabled = false;
            document.getElementById(chkreimbursable).disabled = false;
            document.getElementById('divCalculation3').style.display = '';
            $("#divCalculation3 div:not(:first-child)").css("display", "none");
            document.getElementById('divDoc').style.display = '';
            break;
    }

    hideSplitItems(calculation);

    }


function popupVatModal(save) {
    currentAction = 'saveVAT';
    if (save)
    {
        saveSubcat();
        return;
    }
    $find(pceVat).show();
    return;
}

function hideVatModal() {
    $find(pceVat).hide();
    document.getElementById(txtVATStartDate).value = "";
    document.getElementById(txtVATEndDate).value = "";
    document.getElementById(txtVatAmount).value = "";
    document.getElementById(txtVATPercent).value = "";
    document.getElementById(chkVATRReceipt).checked = false;
    document.getElementById(txtVATLimitWithout).value = "";
    document.getElementById(txtVATLimitWith).value = "";
    document.getElementById(txtVATStartDate).disabled = false;
    document.getElementById(txtVATEndDate).disabled = false;
    return;
}
var activeVatRangeID = 0;
function editVatRange(vatRangeID) {
    var getVatRangeParams = function(subcatid, vatRangeId) {
        this.subCatID = subcatid;
        this.vatRangeID = vatRangeID;
    };
    activeVatRangeID = vatRangeID;
    var params = new getVatRangeParams(subcatid, vatRangeID);
    SEL.Ajax.Service('/shared/webServices/svcSubCats.asmx/', 'GetVatRange', params, editVatRangeReturn);
    //PageMethods.GetVatRange(subcatid, vatRangeID, editVatRangeReturn);
}

function editVatRangeReturn(returndata) {
    var data = returndata.d;
    $('#' + txtVatAmount).val(data.vatamount);
    $('#' + txtVATPercent).val(data.vatpercent);
    $('#' + chkVATRReceipt).prop("checked", data.vatreceipt);
    $('#' +txtVATLimitWithout).val(data.vatlimitwithout);
    $('#' + txtVATLimitWith).val(data.vatlimitwith);
    var date;
    if (data.daterangetype == 0) {
        date = new Date(parseInt(data.datevalue1.substr(6)));
        $('#' + txtVATStartDate).val(date.format('dd/MM/yyyy'));
        document.getElementById(txtVATStartDate).disabled = false;
        document.getElementById(txtVATEndDate).disabled = true;
        document.getElementById(ddlstDateRange).options[2].selected = true;
    }
    else if (data.daterangetype == 1) {
        date = new Date(parseInt(data.datevalue1.substr(6)));
        $('#' + txtVATStartDate).val(date.format('dd/MM/yyyy'));
    document.getElementById(txtVATStartDate).disabled = false;
    document.getElementById(txtVATEndDate).disabled = true;
    document.getElementById(ddlstDateRange).options[0].selected = true;
    }
    else if (data.daterangetype == 2) {
        date = new Date(parseInt(data.datevalue1.substr(6)));
        $('#' + txtVATStartDate).val(date.format('dd/MM/yyyy'));
        date = new Date(parseInt(data.datevalue2.substr(6)));
        $('#' + txtVATEndDate).val(date.format('dd/MM/yyyy'));
        document.getElementById(txtVATStartDate).disabled = false;
        document.getElementById(txtVATEndDate).disabled = false;
        document.getElementById(ddlstDateRange).options[3].selected = true;
    }
    else {
        // any
        document.getElementById(txtVATStartDate).disabled = true;
        document.getElementById(txtVATEndDate).disabled = true;
        document.getElementById(ddlstDateRange).options[1].selected = true;
    }
    ddlstDateRange_onchange();
    popupVatModal(true);
    return;
}

function addVatRange() {
    
    if (validateform('vgVATRate') == false) {
        return;
    }

    var date1 = null;
    if (document.getElementById(txtVATStartDate).value != '') {
        date1 = document.getElementById(txtVATStartDate).value.substring(6, 10) + "/" + document.getElementById(txtVATStartDate).value.substring(3, 5) + "/" + document.getElementById(txtVATStartDate).value.substring(0, 2);
    }
    var date2 = null;
    if (document.getElementById(txtVATEndDate).value != '') {
        date2 = document.getElementById(txtVATEndDate).value.substring(6, 10) + "/" + document.getElementById(txtVATEndDate).value.substring(3, 5) + "/" + document.getElementById(txtVATEndDate).value.substring(0, 2);
    }
    var vatamount = 0;

    if (document.getElementById(txtVatAmount).value != '') {
        vatamount = new Number(document.getElementById(txtVatAmount).value);
    }
    var vatpercent = 0;
    if (document.getElementById(txtVATPercent).value != '') {
        vatpercent = document.getElementById(txtVATPercent).value
    }
    var vatreceipt = document.getElementById(chkVATRReceipt).checked;
    var vatlimitwithout = 0;
    var vatlimitwith = 0;
    var datetype = document.getElementById(ddlstDateRange).value;

    
    if (document.getElementById(txtVATLimitWithout).value != '') {
        vatlimitwithout = document.getElementById(txtVATLimitWithout).value;
    }
    if (document.getElementById(txtVATLimitWith).value != '') {
        vatlimitwith = document.getElementById(txtVATLimitWith).value;
    }
    var addVatDateRangeParams = function(activeVatRangeId, subcatid, date3, date4, datetype1, vatamount1, vatpercent1, checked, vatlimitwithout1, vatlimitwith1) {
        this.reqVatDateRangeID = activeVatRangeId;
        this.subcatid = subcatid;
        this.date1 = date3;
        this.date2 = date4;
        this.datetype = datetype1;
        this.vatamount = vatamount1;
        this.vatpercent = vatpercent1;
        this.vatreceipt = checked;
        this.vatlimitwithout = vatlimitwithout1;
        this.vatlimitwith = vatlimitwith1;
    };
    var params = new addVatDateRangeParams(activeVatRangeID, subcatid, date1, date2, datetype, vatamount, vatpercent, vatreceipt, vatlimitwithout, vatlimitwith);
    SEL.Ajax.Service('/shared/webServices/svcSubCats.asmx/', 'AddVatDateRange', params, addVatRangeComplete);

    //PageMethods.AddVatDateRange(activeVatRangeID, subcatid, date1, date2, datetype, vatamount, vatpercent, vatreceipt, vatlimitwithout, vatlimitwith, addVatRangeComplete);

    activeVatRangeID = 0;

    return;
}

function addVatRangeComplete() {
    var createVATGridParams = function (subcatid) { this.contextKey = subcatid; };
    var params = new createVATGridParams(subcatid);
    SEL.Ajax.Service('/shared/webServices/svcSubCats.asmx/', 'createVATGrid', params, onAddVatRangeComplete);
    
    //PageMethods.createVATGrid(subcatid, onAddVatRangeComplete);
    
    hideVatModal();
}

function onAddVatRangeComplete(data) {
    if ($e(pnlVatRanges) === true) {
        $g(pnlVatRanges).innerHTML = data.d[2];
        SEL.Grid.updateGrid(data.d[1]);
    }
}

function deleteVatRange(vatrangeid) {
    if (confirm('Are you sure you wish to delete the selected VAT rate?')) {
        var deleteVatDateRange = function (vatrangeid) { this.vatRateID = vatrangeid; };
        var params = new deleteVatDateRange(vatrangeid);
        SEL.Ajax.Service('/shared/webServices/svcSubCats.asmx/', 'deleteVatDateRange', params, addVatRangeComplete);
        //PageMethods.deleteVatDateRange(vatrangeid, deleteVatDateRangeComplete);
    }
}

function saveSubcat()
{
    if (validateform('vgSubcat') == false)
        return;
    if ($('#' + chkenablehometooffice).is(':checked') && $('#' + chkEnforceMileageCap).is(':checked') && (validateform('vgHomeToOfficeMileageCap') === false)) {
            return;
    }
    if ($('#' + chkenablehometooffice).is(':checked') && $('#' + optDeductFixed).is(':checked')) {
        if (validateform('vgHomeToOffice') == false) 
            return;
    }

    if ($('#' + optRotationalMileage).is(':checked') && validateform('vgRotationalMileage') == false)
    {
        return;
    }

    var calculation = new Number(document.getElementById(ddlstCalculation).options[document.getElementById(ddlstCalculation).selectedIndex].value);
    var mileageapp = document.getElementById(chkmileage).checked;
    var staffapp = document.getElementById(chkstaff).checked;
    var othersapp = document.getElementById(chkothers).checked;
    var nodirectorsapp = document.getElementById(chknodirectors).checked;
    var reimbursable = document.getElementById(chkreimbursable).checked;
    var attendeesapp = document.getElementById(chkattendees).checked;
    var attendeesmand = document.getElementById(chkattendeesmand).checked;
    var bmilesapp = document.getElementById(chkbmiles).checked;
    var companyapp = document.getElementById(chkcompany).checked;
    var splitentertainment = document.getElementById(chkentertainment).checked;
    var eventinhomeapp = document.getElementById(chkeventinhome).checked;
    var fromapp = document.getElementById(chkfrom).checked;
    var hotelapp = document.getElementById(chkhotel).checked;
    var hotelmand = document.getElementById(chkhotelmand).checked;
    var nonights = document.getElementById(chknonights).checked;
    var nopassengersapp = document.getElementById(chknopassengers).checked;
    var passengernamesapp = document.getElementById(chkpassengernames).checked;
    var receipt = document.getElementById(chknormalreceipt).checked;
    var noroomsapp = document.getElementById(chknorooms).checked;
    var otherdetails = document.getElementById(chkotherdetails).checked;
    var passengersapp = document.getElementById(chkpassenger).checked;
    var nopersonalguests = document.getElementById(chkpersonalguests).checked;
    var pmilesapp = document.getElementById(chkpmiles).checked;
    var reasonapp = document.getElementById(chkreason).checked;
    var reimbursable = document.getElementById(chkreimbursable).checked;
    var noremoteworkers = document.getElementById(chkremoteworkers).checked;
    var splitpersonal = document.getElementById(chksplitpersonal).checked;
    var splitremote = document.getElementById(chksplitremoteworkers).checked;
    var tipapp = document.getElementById(chktip).checked;
    var toapp = document.getElementById(chkto).checked;
    var vatnumber = document.getElementById(chkvatnumber).checked;
    var vatnumbermand = document.getElementById(chkvatnumbermand).checked;
    var vatreceipt = document.getElementById(chkvatreceipt).checked;
    var categoryid = new Number(document.getElementById(cmbcategories).options[document.getElementById(cmbcategories).selectedIndex].value);
    var entertainmentid = new Number(document.getElementById(cmbentertainment).options[document.getElementById(cmbentertainment).selectedIndex].value);
    var pdcatid = new Number(document.getElementById(cmbpdcats).options[document.getElementById(cmbpdcats).selectedIndex].value);
    var personalid = new Number(document.getElementById(cmbsplitpersonal).options[document.getElementById(cmbsplitpersonal).selectedIndex].value);
    var remoteid = new Number(document.getElementById(cmbsplitremote).options[document.getElementById(cmbsplitremote).selectedIndex].value);
    var totaltype = document.getElementById(cmbtotaltype).options[document.getElementById(cmbtotaltype).selectedIndex].value;;
    var accountcode = document.getElementById(txtaccountcode).value;
    var allowanceamount = 0;
    if (document.getElementById(txtallowanceamount).value != '')
    {
        allowanceamount = document.getElementById(txtallowanceamount).value;
    }
    var alternateaccountcode = document.getElementById(txtalternateaccountcode).value;
    var comment = document.getElementById(txtcomment).value;
    var description = document.getElementById(txtdescription).value;
    var shortsubcat = document.getElementById(txtshortsubcat).value;
    var subcat = document.getElementById(txtsubcat).value;

    var isRelocationMileage = document.getElementById(chkIsRelocationMileage).checked;
    var allowHeavyBulkyMileage = document.getElementById(chkHeavyBulky).checked;
    var enablehometooffice = document.getElementById(chkenablehometooffice).checked;
    var deducthometooffice = 0;
    var homeToOfficeFixedMiles = null;
    var publicTransportRate = null;

    if (!homeToOfficeMileageOptionSelected() && $('#' + chkenablehometooffice).is(':checked')) {
        SEL.MasterPopup.ShowMasterPopup("Please select a home to office deduction option");
        return;
    }

    if (document.getElementById(optdeducthometooffice).checked) 
    {
        deducthometooffice = 1;
    }
    else if (document.getElementById(optflaghometooffice).checked) 
    {
        deducthometooffice = 2;
    }
    else if (document.getElementById(optdeducthometoofficeonce).checked)
    {
        deducthometooffice = 3;
    }
    else if (document.getElementById(optdeducthometoofficeall).checked)
    {
        deducthometooffice = 4;
    }
    else if (document.getElementById(optdeducthometoofficestart).checked)
    {
        deducthometooffice = 5;
    }
    else if (document.getElementById(optdeductfirstorlasthome).checked)
    {
        deducthometooffice = 6;
    }
    else if (document.getElementById(optdeducthometoofficedistancefull).checked)
    {
        deducthometooffice = 7;
    }
    else if (document.getElementById(optdeductfullhometoofficestart).checked)
    {
        deducthometooffice = 8;
    }
        else if ($("#" + optDeductFixed).prop("checked") && $('#' + chkenablehometooffice).is(':checked'))
    {
        deducthometooffice = 9;
        homeToOfficeFixedMiles = $.trim($("#" + txtDeductFixed).val());
        if (homeToOfficeFixedMiles.length == 0) {
            SEL.MasterPopup.ShowMasterPopup('If "Deduct a fixed number of miles" (on General Details > Home to Office Mileage) is selected, then "Number of fixed miles to deduct" must be specified.', 'Message from ' + moduleNameHTML);
            return;
        }

        homeToOfficeFixedMiles = parseFloat(homeToOfficeFixedMiles);
    }
    else if ($('#' + optRotationalMileage).prop("checked"))
    {
        deducthometooffice = 10;
        var publicTransportRateValue = $('#' + ddlstPublicTransportRate + ' option:selected').val();
        if (publicTransportRateValue != '0') {
            publicTransportRate = +publicTransportRateValue;
        }
    }
    
    var homeToOfficeAsZero = document.getElementById(chkhometoifficeaszero).checked;
    var enforceMileageCap = false;
    var homeToOfficeMileageCap = null;
    if ($('#' + chkenablehometooffice).is(':checked') && $("#" + chkEnforceMileageCap).prop("checked")) {
        enforceMileageCap = document.getElementById(chkEnforceMileageCap).checked;
        homeToOfficeMileageCap = $.trim($("#" + txtMileageCap).val());
        if (homeToOfficeMileageCap.length == 0) {
            SEL.MasterPopup.ShowMasterPopup('If "Enforce mileage cap on Home to Office journey" (on General Details > Home to Office Mileage) is selected, then "Maximum number of miles" must be specified.', 'Message from ' + moduleNameHTML);
            return;
        }
        homeToOfficeMileageCap = parseFloat(homeToOfficeMileageCap);
    }
    var mileageCategory = null;
    if (document.getElementById('divCalculation3').style.display === '') // is Pence per mile active option
    {
        if (document.getElementById(ddlstMileageCategory).options[document.getElementById(ddlstMileageCategory).selectedIndex].value != '0')
        {
            mileageCategory = new Number(document.getElementById(ddlstMileageCategory).options[document.getElementById(ddlstMileageCategory).selectedIndex].value);
        }
    }

    var reimbursableSubcatID = null;
    if (document.getElementById(cmbReimbursableItems).options[document.getElementById(cmbReimbursableItems).selectedIndex].value != '0')
    {
        reimbursableSubcatID = new Number(document.getElementById(cmbReimbursableItems).options[document.getElementById(cmbReimbursableItems).selectedIndex].value);
    }

    // need to check is actually displayed to prevent overwriting value of ddlMileageCategory which may be the active one
    if (document.getElementById('divCalculation8').style.display === '') // is fuel card mileage active option
    {
        if (document.getElementById(ddlstReimburseMileageCategory).options[document.getElementById(ddlstReimburseMileageCategory).selectedIndex].value != '0')
        {
            mileageCategory = new Number(document.getElementById(ddlstReimburseMileageCategory).options[document.getElementById(ddlstReimburseMileageCategory).selectedIndex].value);
        }
    }

    var addasnet;
    if (totaltype == '1') {
        addasnet = false;
    }
    else {
        addasnet = true;
    }

    var arrCountries = new Array();
    var arrAllowances = new Array();
    var txtCountryCode;
    
    for (var i = 0; i < lstCountries.length; i++) {
        txtCountryCode = document.getElementById(tabGeneral + '_txtaccountcode' + lstCountries[i]);
        if (txtCountryCode.value != '') {
            arrCountries.push(new Array(lstCountries[i], txtCountryCode.value));
        }
    }

    arrAllowances = SEL.Grid.getSelectedItemsFromGrid('gridAllowances');

    var udfs = new Array();
    var chkUDF;
    for (var i = 0; i < lstUDFs.length; i++) {
        chkUDF = document.getElementById(tabAdditionalFields + '_chkudf' + lstUDFs[i]);
        
        if (chkUDF.checked) {
            udfs.push(new Number(chkUDF.value));
        }
    }
    
    var startDateString = $('#txtStartDate').val();
    var endDateString = $('#txtEndDate').val();
    
    // If both dates are set, verify that the End date comes after the Start date
    if (startDateString.length > 7 && endDateString.length > 7)
    {
        var firstDate = $.datepicker.parseDate("dd/mm/yy", startDateString);
        var secondDate = $.datepicker.parseDate("dd/mm/yy", endDateString);

        if (firstDate > secondDate)
        {
            SEL.MasterPopup.ShowMasterPopup('The Start date for this Expense Item cannot be greater than the End date.', 'Message from ' + moduleNameHTML);

            // Invalidate validators
            var startDateValidator = $('#comptxtStartDate').get(0);
            startDateValidator.isvalid = false;
            ValidatorUpdateDisplay(startDateValidator);

            var endDateValidator = $('#comptxtEndDate').get(0);
            endDateValidator.isvalid = false;
            ValidatorUpdateDisplay(endDateValidator);

            return;
        }
    }
    
    //if 'increase rate for passengers' is set, either number or names of passengers must be ticked.
    if (passengersapp && !(passengernamesapp || nopassengersapp)) {
        SEL.MasterPopup.ShowMasterPopup('If "Increase vehicle journey rate for passengers" (on General Details) is checked, then either "Show number of passengers" or "Show names of passengers" must also be ticked (on Additional Fields).', 'Message from ' + moduleNameHTML);
        return;
    }
    
    // can't have names and number of passengers together, even if 'increase rate for passengers' isn't checked.
    if (passengernamesapp && nopassengersapp) {
        SEL.MasterPopup.ShowMasterPopup('Cannot have both "Show number of passengers" and "Show names of passengers" ticked.', 'Message from ' + moduleNameHTML);
        return;
    }
    
    //Duty of care options   
    var enableDoCCheck = document.getElementById(chkEnableDoc).checked;
    var requireClass1BusinessInsuranceCheck = document.getElementById(chkRequireClass1Insurance).checked;
      

    // vanquish validation
    var shouldValidate = false;
    var validatorNotes1 = "", validatorNotes2 = "", validatorNotes3 = "";
    var validatorId1 = -1, validatorId2 = -1, validatorId3 = -1;
    if (expenseValidationEnabled === 'True') {
        shouldValidate = document.getElementById(chkEnableValidation).checked;
        validatorId1 = document.getElementById('validationCriterion1Id').value;
        validatorId2 = document.getElementById('validationCriterion2Id').value;
        validatorId3 = document.getElementById('validationCriterion3Id').value;
        validatorNotes1 = document.getElementById(txtValidatorNotes1).value;
        validatorNotes2 = document.getElementById(txtValidatorNotes2).value;
        validatorNotes3 = document.getElementById(txtValidatorNotes3).value;   }

    var userdefined = getItemsFromPanel('vgSubcat');
  
    var saveSubcatParams = function (subcatid, subcat, categoryid, accountcode, description, allowanceamount, addasnet, mileageapp, staffapp, othersapp, attendeesapp, pmilesapp, bmilesapp, tipapp, eventinhomeapp, passengersapp, nopassengersapp, passengernamesapp, splitentertainment, entertainmentid, pdcatid, reimbursable, nonights, hotelapp, comment, attendeesmand, nodirectorsapp, alternateaccountcode, hotelmand, nopersonalguests, noremoteworkers, splitpersonal, splitremote, reasonapp, otherdetails, personalid, remoteid, noroomsapp, vatnumber, vatnumbermand, fromapp, toapp, companyapp, shortsubcat, receipt, calculation, arrCountries, arrAllowances, udfs, enablehometooffice, deducthometooffice, mileageCategory, isRelocationMileage, reimbursableSubcatID, allowHeavyBulkyMileage, userdefined, homeToOfficeAsZero, homeToOfficeFixedMiles, publicTransportRate, startDateString, endDateString, shouldValidate, validatorNotes, validatorIds, enableDoCCheck, requireClass1BusinessInsuranceCheck, homeToOfficeMileageCap, enforceMileageCap) {
        this.subcatid = subcatid;
        this.subcat = subcat;
        this.categoryid = categoryid;
        this.accountcode = accountcode;
        this.description = description;
        this.allowanceamount = allowanceamount;
        this.addasnet = addasnet;
        this.mileageapp = mileageapp;
        this.staffapp = staffapp;
        this.othersapp = othersapp;
        this.attendeesapp = attendeesapp;
        this.pmilesapp = pmilesapp;
        this.bmilesapp = bmilesapp;
        this.tipapp = tipapp;
        this.eventinhomeapp = eventinhomeapp;
        this.passengersapp = passengersapp;
        this.nopassengersapp = nopassengersapp;
        this.passengernamesapp = passengernamesapp;
        this.splitentertainment = splitentertainment;
        this.entertainmentid = entertainmentid;
        this.pdcatid = pdcatid;
        this.reimbursable = reimbursable;
        this.nonights = nonights;
        this.hotelapp = hotelapp;
        this.comment = comment;
        this.attendeesmand = attendeesmand;
        this.nodirectorsapp = nodirectorsapp;
        this.alternateaccountcode = alternateaccountcode;
        this.hotelmand = hotelmand;
        this.nopersonalguests = nopersonalguests;
        this.noremoteworkers = noremoteworkers;
        this.splitpersonal = splitpersonal;
        this.splitremote = splitremote;
        this.reasonapp = reasonapp;
        this.otherdetails = otherdetails;
        this.personalid = personalid;
        this.remoteid = remoteid;
        this.noroomsapp = noroomsapp;
        this.vatnumber = vatnumber;
        this.vatnumbermand = vatnumbermand;
        this.fromapp = fromapp;
        this.toapp = toapp;
        this.companyapp = companyapp;
        this.shortsubcat = shortsubcat;
        this.receipt = receipt;
        this.calculation = calculation;
        this.arrCountries = arrCountries;
        this.allowances = arrAllowances;
        this.associatedudfs = udfs;
        this.enableHomeToLocationMileage = enablehometooffice;
        this.hometolocationtype = deducthometooffice;
        this.mileageCategory = mileageCategory;
        this.isRelocationMileage = isRelocationMileage;
        this.reimbursableSubcatID = reimbursableSubcatID;
        this.allowHeavyBulkyMileage = allowHeavyBulkyMileage;
        this.udfs = userdefined;
        this.homeToOfficeAsZero = homeToOfficeAsZero;
        this.homeToOfficeFixedMiles = homeToOfficeFixedMiles;
        this.publicTransportRate = publicTransportRate;
        this.startDateString = startDateString;
        this.endDateString = endDateString;
        this.validate = shouldValidate;
        this.validationRequirementIds = validatorIds;
        this.validationRequirements = validatorNotes;
        this.enableDoc =enableDoCCheck;
        this.requireClass1BusinessInsurance = requireClass1BusinessInsuranceCheck;
        this.homeToOfficeMileageCap = homeToOfficeMileageCap;
        this.enforceMileageCap = enforceMileageCap;
    };
   

    var params = new saveSubcatParams(subcatid, subcat, categoryid, accountcode, description, allowanceamount, addasnet, mileageapp, staffapp, othersapp, attendeesapp, pmilesapp, bmilesapp, tipapp, eventinhomeapp, passengersapp, nopassengersapp, passengernamesapp, splitentertainment, entertainmentid, pdcatid, reimbursable, nonights, hotelapp, comment, attendeesmand, nodirectorsapp, alternateaccountcode, hotelmand, nopersonalguests, noremoteworkers, splitpersonal, splitremote, reasonapp, otherdetails, personalid, remoteid, noroomsapp, vatnumber, vatnumbermand, fromapp, toapp, companyapp, shortsubcat, receipt, calculation, arrCountries, arrAllowances, udfs, enablehometooffice, deducthometooffice, mileageCategory, isRelocationMileage, reimbursableSubcatID, allowHeavyBulkyMileage, userdefined, homeToOfficeAsZero, homeToOfficeFixedMiles, publicTransportRate, startDateString, endDateString, shouldValidate, [validatorNotes1, validatorNotes2, validatorNotes3], [validatorId1, validatorId2, validatorId3], enableDoCCheck, requireClass1BusinessInsuranceCheck, homeToOfficeMileageCap, enforceMileageCap);
    SEL.Ajax.Service('/shared/webServices/svcSubCats.asmx/', 'saveSubcat', params, saveSubcatComplete);

    //PageMethods.saveSubcat(subcatid, subcat, categoryid, accountcode, description, allowanceamount, addasnet, mileageapp, staffapp, othersapp, attendeesapp, pmilesapp, bmilesapp, tipapp, eventinhomeapp, passengersapp, nopassengersapp, passengernamesapp, splitentertainment, entertainmentid, pdcatid, reimbursable, nonights, hotelapp, comment, attendeesmand, nodirectorsapp, alternateaccountcode, hotelmand, nopersonalguests, noremoteworkers, splitpersonal, splitremote, reasonapp, otherdetails, personalid, remoteid, noroomsapp, vatnumber, vatnumbermand, fromapp, toapp, companyapp, shortsubcat, receipt, calculation, arrCountries, arrAllowances, udfs, enablehometooffice, deducthometooffice, mileageCategory, isRelocationMileage, reimbursableSubcatID, allowHeavyBulkyMileage, userdefined, homeToOfficeAsZero, homeToOfficeFixedMiles, startDateString, endDateString, shouldValidate, validatorNotes, saveSubcatComplete);
}

function homeToOfficeMileageOptionSelected() {
    if (document.getElementById(optdeducthometooffice).checked ||
        document.getElementById(optflaghometooffice).checked ||
        document.getElementById(optdeducthometoofficeonce).checked ||
        document.getElementById(optdeducthometoofficeall).checked ||
        document.getElementById(optdeducthometoofficestart).checked ||
        document.getElementById(optdeductfirstorlasthome).checked ||
        document.getElementById(optdeducthometoofficedistancefull).checked ||
        document.getElementById(optdeductfullhometoofficestart).checked ||
        $("#" + optDeductFixed).prop("checked") && $('#' + chkenablehometooffice).is(':checked') ||
        $('#' + optRotationalMileage).prop("checked")) {
        return true;
    }
    return false;
}

function saveSubcatFailure(data) {
    SEL.MasterPopup.ShowMasterPopup('Saving the subcat failed - ' + data.Message, 'Message from ' + moduleNameHTML);
}

function saveSubcatComplete(data) {
    switch (data.d) {
        case -1:
            alert('The expense item name you have entered already exists');
            return;
        default:
            subcatid = data.d;
            break;
    }
    switch (currentAction) {
        case 'saveRole':
            popupRoleModal(false);
            break;
        case 'saveVAT':
            popupVatModal(false);
            break;
        case 'OK':
            document.location = 'adminsubcats.aspx';
            break;
        case 'saveSplitItems':
            popupSplitItemModal(false);
            break;
    }
}

function popupRoleModal(save) {
    currentAction = 'saveRole';
    if (save) {
        saveSubcat();
        return;
    }
    $find(modRole).show();
    return;
}

function hideRoleModal() {
    $find(modRole).hide();
    return;
}

function popupSplitItemModal(save) {
    currentAction = 'saveSplitItems';
    if (save) {
        saveSubcat();
        return;
    }
    $find(modSplit).show();
    return;
}

function hideSplitItemModal() {
    $find(modSplit).hide();
    return;
}

function populateSplitItems() {
    var getSplitItems = function (subcatid) { this.contextKey = subcatid; };
    var params = new getSplitItems(subcatid);
    SEL.Ajax.Service('/shared/webServices/svcSubCats.asmx/', 'getSplitItems', params, populateSplitItemsComplete);
    //PageMethods.getSplitItems(subcatid, populateSplitItemsComplete);
}

function populateSplitItemsComplete(data) {
    if ($e(pnlSplitListGrid) === true) {
        $g(pnlSplitListGrid).innerHTML = data.d[2];
        SEL.Grid.updateGrid(data.d[1]);
    }
}

function saveSplitItems() {
    currentAction = 'saveSplitItems';
    if (subcatid == 0) {
        saveSubcat();
        return;
    }

    var arrSplit = new Array();
    arrSplit = SEL.Grid.getSelectedItemsFromGrid('gridModalSplit');
    var saveSplitItemsParams = function (subcatid, arrSplit) {
        this.subcatid = subcatid;
        this.splititems = arrSplit;
    };
    var params = new saveSplitItemsParams(subcatid, arrSplit);
    SEL.Ajax.Service('/shared/webServices/svcSubCats.asmx/', 'saveSplitItems', params, saveSplitItemsComplete);
    //PageMethods.saveSplitItems(subcatid, arrSplit, saveSplitItemsComplete);
}

function saveSplitItemsComplete(data) {
    if (data.d[0] == -1) {
        SEL.MasterPopup.ShowMasterPopup('The expense item ' + data.d[1] + ' cannot be added as a split to this item as this item is already a split of ' + data.d[2] + '.');
        return;
    }
    if (data.d[0] == -2) {
        //Cannot add this split item, as SubCatA is already referred as a split item to SubCatB or another split item associated to it.
        SEL.MasterPopup.ShowMasterPopup('Cannot add this split item, as ' + data.d[2] + ' is already referred as a split item to ' + data.d[1] + ' or another split item associated to it.');
        return;
    }
    populateSelectedSplitItems();
    hideSplitItemModal();
}

function populateSelectedSplitItems() {
    var createSplitItemGrid = function (subcatid) {
        this.contextKey = subcatid;
    };
    var params = new createSplitItemGrid(subcatid);
    SEL.Ajax.Service('/shared/webServices/svcSubCats.asmx/', 'createSplitItemGrid', params, populateSelectedSplitItemsComplete);
    //PageMethods.createSplitItemGrid(subcatid, populateSelectedSplitItemsComplete);
}

function populateSelectedSplitItemsComplete(data) {
    if ($e(pnlSplitItemsGrid) === true) {
        $g(pnlSplitItemsGrid).innerHTML = data.d[2];
        SEL.Grid.updateGrid(data.d[1]);
    }
}

function deleteSubcat(id) {
    subcatid = id;
    if (confirm('Are you sure you wish to delete the selected expense item?')) {
        PageMethods.deleteSubcat(accountid, subcatid, deleteSubcatComplete);

    }
}

function deleteSubcatComplete(data) {
    switch (data) {
        case 1:
            SEL.MasterPopup.ShowMasterPopup('This expense item cannot be deleted because it is assigned to one or more items.', 'Message from ' + moduleNameHTML);
            break;
        case 2:
            SEL.MasterPopup.ShowMasterPopup('This expense item cannot be deleted because it is assigned to one or more mobile expense items.', 'Message from ' + moduleNameHTML);
            break;
        case 3:
            SEL.MasterPopup.ShowMasterPopup('The expense item cannot be deleted as it is associated with one or more flag rules.', 'Message from ' + moduleNameHTML);
            break;
        case 4:
            SEL.MasterPopup.ShowMasterPopup('The expense item cannot be deleted as it is associated with one or more mobile journeys.', 'Message from ' + moduleNameHTML);
            break;
        case -10:
            SEL.MasterPopup.ShowMasterPopup('This expense item cannot be deleted because it is assigned to one or more GreenLights or user defined fields.', 'Message from ' + moduleNameHTML);
            break;
        default:
            SEL.Grid.deleteGridRow('gridSubcats', subcatid);
            break;
    }
}

function saveRole() {
    

    if (validateform('vgRole') == false) {
        return;
    }

    var roleid = document.getElementById(ddlstItemRole).options[document.getElementById(ddlstItemRole).selectedIndex].value;
    var addtotemplate = document.getElementById(chkaddtotemplate).checked;
    var receiptmaximum = 0;
    if (document.getElementById(txtlimitwithout).value != '') {
        receiptmaximum = new Number(document.getElementById(txtlimitwithout).value);
    }
    var maximum = 0;
    if (document.getElementById(txtlimitwith).value != '') {
        maximum = new Number(document.getElementById(txtlimitwith).value);
    }

    var saveRoleParams = function (subcatid, roleid, addtotemplate, receiptmaximum, maximum) {
        this.subcatid = subcatid;
        this.roleid = roleid;
        this.addtotemplate = addtotemplate;
        this.receiptmaximum = receiptmaximum;
        this.maximum = maximum;
    };
    var params = new saveRoleParams(subcatid, roleid, addtotemplate, receiptmaximum, maximum);
    SEL.Ajax.Service('/shared/webServices/svcSubCats.asmx/', 'saveRole', params, saveRoleComplete);
    //PageMethods.saveRole(subcatid, roleid, addtotemplate, receiptmaximum, maximum, saveRoleComplete);
}
function saveRoleComplete(data) {
    hideRoleModal();
    populateRoles();
}
function editRole(roleid) {
    var editRoleParams = function (subcatid, roleid) {
        this.subcatid = subcatid;
        this.roleid = roleid;
    };
    var params = new editRoleParams(subcatid, roleid);
    SEL.Ajax.Service('/shared/webServices/svcSubCats.asmx/', 'getRole', params, getRoleComplete);
    //PageMethods.getRole(subcatid, roleid, getRoleComplete);
}

function getRoleComplete(data) {

    for (var i = 0; i < document.getElementById(ddlstItemRole).options.length; i++) {
        if (document.getElementById(ddlstItemRole).options[i].value == data.d.roleid) {
            document.getElementById(ddlstItemRole).selectedIndex = i;
            break;
        }
    }
     
    document.getElementById(chkaddtotemplate).checked = data.d.isadditem;
    document.getElementById(txtlimitwithout).value = data.d.receiptmaximum;
    document.getElementById(txtlimitwith).value = data.d.maximum;
    popupRoleModal(true);
}

function populateRoles() {
    var editRoleParams = function (subcatid) {
        this.contextKey = subcatid;
    };
    var params = new editRoleParams(subcatid);
    SEL.Ajax.Service('/shared/webServices/svcSubCats.asmx/', 'createRoleGrid', params, populateRolesComplete);
    //PageMethods.createRoleGrid(subcatid, populateRolesComplete);
}

function populateRolesComplete(data) {
    if ($e(pnlRolesGrid) === true) {
        $g(pnlRolesGrid).innerHTML = data.d[2];
        SEL.Grid.updateGrid(data.d[1]);
    }
}

var roleid;
function deleteRole(id) {
    roleid = id;
    if (confirm('Are you sure you wish to remove access to this item for the selected role?')) {
        var editRoleParams = function (subcatid, roleid) {
            this.subcatid = subcatid;
            this.roleid = roleid;
        };
        var params = new editRoleParams(subcatid, roleid);
        SEL.Ajax.Service('/shared/webServices/svcSubCats.asmx/', 'deleteRole', params, deleteRoleComplete);
        //PageMethods.deleteRole(subcatid, roleid, deleteRoleComplete);
    }
}

function deleteRoleComplete(data) {
    SEL.Grid.deleteGridRow('gridRoles', roleid);
}


var splitItemID;
function deleteSplitItem(id) {
    splitItemID = id;
    if (confirm("Are you sure you wish to remove this split item?")) {
        var editRoleParams = function (subcatid, id) {
            this.subcatID = subcatid;
            this.subcatSplitID = id;
        };
        var params = new editRoleParams(subcatid, id);
        SEL.Ajax.Service('/shared/webServices/svcSubCats.asmx/', 'deleteSplitItem', params, deleteSplitItemComplete);
        //PageMethods.deleteSplitItem(subcatid, id, deleteSplitItemComplete);
    }
}

function deleteSplitItemComplete(data) {
    SEL.Grid.deleteGridRow("gridSplit", splitItemID);
}


function SetupDateFields()
{
    // Create the date picker controls
    $('.dateField').datepicker().attr('maxlength', 10);

    // Setup the focus events for date fields
    $('.dateField').focus(function()
    {
        // Remove the 'focus token' from the previous control
        $('.hasCalControl').removeClass('hasCalControl');

        // Give the 'focus token' to the new control        
        $(this).addClass('hasCalControl');
    });

    // Setup the blur events for date fields
    $('.dateField').blur(function()
    {
        var dateValue = $(this).val();

        if ($.isNumeric(dateValue) && dateValue.length === 8)
        {
            var newDateValue = dateValue.substring(0, 2) + "/";

            newDateValue += dateValue.substring(2, 4) + "/";

            newDateValue += dateValue.substring(4, dateValue.length);

            $(this).val(newDateValue);

            // Refresh validators if the field has been updated
            $(this).parent().nextAll('.inputvalidatorfield').first().children().each(function()
            {
                var val = $g($(this).attr('id'));
                ValidatorValidate(val);
            });
        }
        else
        {
            $(this).val(dateValue.replace(/\./g, '/').replace(/\-/g, '/'));
        }
    });

    // Setup the click events for calendar images
    $('.dateCalImg, .timeCalImg').click(function()
    {
        var inputControl = $(this).parent().prev().children().first();

        if (inputControl.is(':disabled')) return false;

        if (inputControl.hasClass('hasCalControl'))
        {
            var pickerDiv = $(this).hasClass('dateCalImg') ? '#ui-datepicker-div' : '#ui-timepicker-div';

            if ($(pickerDiv).css('display') === 'none')
            {
                inputControl.focus();
            }
            else
            {
                if ($(pickerDiv).is(':animated') === false)
                {
                    $(pickerDiv).fadeOut(100);
                }
            }
        }
        else
        {
            inputControl.focus();
        }
    });
}


function SetMaximumMilesTextbox() {
    if ($('#' + chkEnforceMileageCap).is(":checked")) {
        $('.maximumMiles').show();
    } else {
        $('.maximumMiles').hide();
    }
}

function setRotationalMileageRelatedFields()
{
    if ($('#' + optRotationalMileage).is(":checked"))
    {
        $('#' + chkhometoifficeaszero).prop('disabled', true).attr('checked', false);
        $('.publicTransportRateField').show();
        $('#' + chkEnforceMileageCap).prop('disabled', true).attr('checked', false);
        $("#" + txtMileageCap).val('');
        $('.maximumMiles').hide();
    }
    else
    {
        $('#' + chkhometoifficeaszero).prop('disabled', false);
        $('.publicTransportRateField').hide();
        $('#' + chkEnforceMileageCap).prop('disabled', false);
    } setRotationalMileageRelatedFields
}


//Below function is called on Expense item calculation type change
//Check the general option settings to enable/disable DOC/Class1 business option
function setDocOptions()
{
      var generalOptionParam = function (accountid) {
        this.accountid = accountid;
      };
      var params = new generalOptionParam(accountid);
      SEL.Ajax.Service('/shared/webServices/svcSubCats.asmx/', 'GetDoCGeneralOption', params, getDoCGeneralOptionComplete);
}

//Below function is called on Enable Doc option checked/unhecked to set Class 1 business check option 
//enableClass1 is false if the Insurance Expiry is not set in General option.
function setClass1BusinessOptions() {
    if (enableClass1) {
        if ($('#' + chkEnableDoc).is(":checked")) {
            $('#' + chkRequireClass1Insurance).prop('disabled', false);
        }
        else {
            $('#' + chkRequireClass1Insurance).prop('disabled', true);
        }
    }
}

function getDoCGeneralOptionComplete(data) {    
    switch (data.d) {
        case 0:
            $('#' + chkEnableDoc).prop('disabled', true);
            $('#' + chkRequireClass1Insurance).prop('disabled', true);
            break;
        case 1:
            $('#' + chkEnableDoc).prop('disabled', false);
            $('#' + chkRequireClass1Insurance).prop('disabled', true);
            break;
        case 2:
            $('#' + chkEnableDoc).prop('disabled', false);
            $('#' + chkRequireClass1Insurance).prop('disabled', false);
            enableClass1 = true;
            break;           
    }
    if (enableClass1) {
        setClass1BusinessOptions();
    }
}

function hideSplitItems(itemType) {
    if (itemType == 2) { //Calculation item type 2 is for the meal.
        $("#splitExpenseItems").hide();
        $("#linkSplitItem").hide();
        $("#ctl00_contentmain_TabContainer1_tabGeneral_pnlSplitItems").hide();
    } else {
        $("#splitExpenseItems").show();
        $("#linkSplitItem").show();
        $("#ctl00_contentmain_TabContainer1_tabGeneral_pnlSplitItems").show();
    }
}

$(document).ready(function() {
    hideSplitItems(document.getElementById(ddlstCalculation).options[document.getElementById(ddlstCalculation).selectedIndex].value);
})
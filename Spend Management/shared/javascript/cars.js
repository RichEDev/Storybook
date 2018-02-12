$(document).ready(function () {
  
    // show/hide the "previous vehicle" combobox when the yes/no option is changed, also sets its-self up if the option is already set
    $(".replaceCar input[type=radio]").on("change", function () {
        var element = $(this);
        if (element.is(":checked")) {
            $(".previousCar").toggle(element.val() == 1);
        }
    }).trigger("change");

    // provides buffering when used with $().on, e.g. to trigger a function some amount of time after the last keystroke
    var eventBuffer = (function () {
        var timer = 0;
        return function (callback, millis) {
            clearTimeout(timer);
            timer = setTimeout(callback, millis);
        };
    })();

    // any time the car registration field changes, check that the user isn't typing in a registration that is already used
    var registrationField = $("input.registrationNumber");
    registrationField.on("input onpropertychange keyup paste", function () {
        eventBuffer(function () {

            // only if it's a new vehicle
            if (carid === 0) {

                var registration = registrationField.val();
                if (registration.length > 3) {
                    SEL.Data.Ajax({
                        serviceName: "svcCars",
                        methodName: (isPoolCar) ? "CheckDuplicatePoolCarRegistration" : "CheckDuplicateCarRegistration",
                        data: {
                            employeeId: nEmployeeid,
                            registration: registration
                        },
                        success: function (response) {

                            var message = (isPoolCar) ? "There is already a pool vehicle with the registration " + registration : "You already have at least one vehicle with the registration " + registration + ", if you are waiting for a vehicle to be approved please contact your administrator.";

                            // don't stop the user from adding the vehicle, just warn them that they might be about to create a duplicate
                            if ("d" in response && response.d === true) {
                                SEL.MasterPopup.ShowMasterPopup(message, "Message from " + moduleNameHTML);
                            }
                        }
                    });
                }
            }
        }, 500);
    });
    if ($('#imgLookup').length > 0) {
        $('#' + txtregistration).change(function() { LookupVehicleDetails(); });
    }
});
var vehicleType = "";
var carid = 0;
var isAttach = false;
var isShallowSave = false;
var odometerid = 0;
var mcChecks = [];

function saveCar(commit) 
{
    if (validateform('ValidationSummaryAeCar') == false)
    {
        isAttach = false;
        return;
    }

    if (commit)
    {
        isShallowSave = false;
    }
    vehicleType = document.getElementById(cmbvehicletype).options[document.getElementById(cmbvehicletype).selectedIndex].text;
    var make = document.getElementById(txtmake).value;
    var model = document.getElementById(txtmodel).value;
    var registration = document.getElementById(txtregistration).value;
    var defaultunit = new Number(document.getElementById(cmbUom).options[document.getElementById(cmbUom).selectedIndex].value);
    
    var active = true;
    if(document.getElementById(chkactive) != undefined)
    {
        active = document.getElementById(chkactive).checked;
    }
    
    var vehicleEngineTypeId = new Number(document.getElementById(cmbcartype).options[document.getElementById(cmbcartype).selectedIndex].value);

    var fuelcard = false;
    if (document.getElementById(chkodometerreading) != undefined)
    {
        fuelcard = document.getElementById(chkodometerreading).checked;
    }
    
    var startodometer = 0;
    if (document.getElementById(txtstartodo) != undefined)
    {
        if (document.getElementById(txtstartodo).value != '')
        {
            startodometer = new Number(document.getElementById(txtstartodo).value);
        }
    }
    
    var endodometer = 0;
    if (document.getElementById(txtendodo) != undefined)
    {
        if (document.getElementById(txtendodo).value != '')
        {
            endodometer = new Number(document.getElementById(txtendodo).value);
        }
    }
    
    var enginesize = 0;
    if (document.getElementById(txtenginesize) != undefined)
    {
        if (document.getElementById(txtenginesize).value != '')
        {
            enginesize = document.getElementById(txtenginesize).value;
        }
    }

    var exemptfromhometooffice = false;
    if (document.getElementById(chkexemptfromhometooffice) != undefined)
    {
        exemptfromhometooffice =document.getElementById(chkexemptfromhometooffice).checked;
    }
  
    var startdate = null;    
    if (document.getElementById(txtstart) != undefined)
    {
        if (document.getElementById(txtstart).value != "")
        {
            startdate = document.getElementById(txtstart).value.substring(6, 10) + "/" + document.getElementById(txtstart).value.substring(3, 5) + "/" + document.getElementById(txtstart).value.substring(0, 2);
        }
    }
    
    var enddate = null;
    if (document.getElementById(txtend) != undefined)
    {
        if (document.getElementById(txtend).value != "")
        {
            enddate = document.getElementById(txtend).value.substring(6, 10) + "/" + document.getElementById(txtend).value.substring(3, 5) + "/" + document.getElementById(txtend).value.substring(0, 2);
        }
    }


    var vehicletypeid = new Number(document.getElementById(cmbvehicletype).options[document.getElementById(cmbvehicletype).selectedIndex].value);

    var mileagecats = SEL.Grid.getSelectedItemsFromGrid('gridMileage');

    var replacePreviousCar = ($(".replaceCar input[type=radio]:checked").val() === "1");
    var previousCarId = $(".previousCar select").val() || 0;

    var userdefined = getItemsFromPanel('ValidationSummaryAeCar');

   Spend_Management.svcCars.saveCar(carid, nEmployeeid, startdate, enddate, make, model, registration, active, vehicleEngineTypeId, startodometer, fuelcard, endodometer, defaultunit, enginesize, mileagecats, userdefined, approved, exemptfromhometooffice, replacePreviousCar, previousCarId, isAdmin, isShallowSave, vehicletypeid, saveCarComplete, commandFail);
}

function saveCarComplete(data) 
{   
    carid = data[0]; 
    if (isAttach) 
    {
        var ifrFile = document.getElementById('iFrCarDocAttach');
        ifrFile.contentWindow.document.getElementById('GenIDVal').value = carid;
        ifrFile.contentWindow.document.getElementById('DocType').value = attachDocType;
        
        isAttach = false;
        showAttachmentModal();
        return;
    }

    clearCarModal();

    if (data[1] == true) 
    {
        navigateTo(true, false);
    }
    else
    {
        navigateTo(false, false);
    }
}

function navigateTo(autoActivate, isCancel)
{
     var carLink;

    if (isShallowSave && isCancel)
    {
        /* A shallow save has taken place, but they have cancelled out, so need to delete any documents uploaded and the shallow saved vehicle */
        if (carid > 0)
        {
            Spend_Management.svcCars.deleteCar(employeeid, carid, onCarDeleteComplete);
        }
    }

    if (isPoolCar)
    {
        window.location = "poolcars.aspx";
    }
    else if (isAdmin) 
    {
        if (!editOnly)
        {
            hideCarModal();
            populateCarGrid();
        } else
        {
            window.location = returnURL;
        }
    }
    else 
    {      
        if (isAeExpense) 
        {
            if (!isCancel) 
            {
                if (carLinkID != undefined) 
                {
                    carLink = document.getElementById(contentID + carLinkID);
                }

                if (carLink != undefined) 
                {

                    if (autoActivate) 
                    {
                        carLink.innerHTML = "Vehicle Added.";

                        function afterPostBack() {
                            $get('__EVENTTARGET').value = $get('__EVENTARGUMENT').value = '';
                            Sys.WebForms.PageRequestManager.getInstance().remove_endRequest(afterPostBack);

                            // automatically select the newly created vehicle
                            $("#" + pnlSpecClientID + " select[id*=cmbcars]").val(carid);
                        }

                        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(afterPostBack);
                        __doPostBack(pnlSpecClientID, '');
                        vehicleDocuments("Your vehicle has been successfully added.");
                    }
                    else 
                    {
                        vehicleDocuments("Your vehicle has been added and is waiting for approval by an administrator. You need at least one approved <br />vehicle before you can claim mileage.")
                        $(".waitingtobeapproved[data-unapprovedmsgtemplate]").each(function() {
                            var msgTemplate = $(this).data("unapprovedmsgtemplate");
                            //this will be, say, "You have $UNAPPROVEDCOUNT vehicles awaiting approval"
                            //but is maintained on the C# side (at time of writing, cItemBuilder ~3051)
                            var unapprovedCount = parseInt($(this).data("unapprovedcount")) + 1;
                            var newText = msgTemplate.replace("$UNAPPROVEDCOUNT", unapprovedCount);
                            newText = newText.replace(/ 1 (\w+)s/gi, " 1 $1"); //remove pluralization if singular
                            $("span.waitingtobeapprovedtext", this).text(newText);
                            $(this).data("unapprovedcount", unapprovedCount);
                            if (unapprovedCount == 0) {
                                $(this).addClass("hidden");
                            } else {
                                $(this).removeClass("hidden");
                            }
                        });
                    }
                }
            }

            hideAddCarPanel();
        }
        else
        {        
            if (!isCancel)
            {
            
                if (autoActivate) {
                    vehicleDocuments("Your vehicle has been successfully added.");
                }
                else {
                    vehicleDocuments("The vehicle has been added and is waiting for approval by an administrator.");
                }
                $('#btnMasterPopup').click(function () {
                    window.location = returnURL;
                });
            }
            else {
                window.location = returnURL;
            }
        }

    }
}

function vehicleDocuments(messageHeader, isadmin) {
    var ViewGuid = 'F1EA11DD-A18F-466D-B638-1E2EA2201F85';
    if (isadmin) {
        ViewGuid = 'EEDC1A2B-77AD-484D-B0A1-DCF5BCD78299';
    }
    Spend_Management.svcCars.GetDocEntityAndViewId('F0247D8E-FAD3-462D-A19D-C9F793F984E8', ViewGuid, getDocEntityAndViewIdComplete)
    if (vehicleType === "" || sessionStorage.entityview == undefined) {
        window.setTimeout(function () { vehicleDocuments(messageHeader, isadmin); }, 10); //Taking some milliseconds to load vehicleType.
    }
    else {
        var message = "";
        if (CurrentUserInfo.AllowEmpToSpecifyCarDOCOnAdd === true || isadmin ===true)
        {
            var title = " The following documents must be added and approved before <br /> any claims for mileage can be made for this vehicle.<br />";
            if (CurrentUserInfo.Vehicle.BlockTax === "true") {
                if (message === "") { message = title; }
                message = message + "<br />\u2022 Tax";
            }
            if (CurrentUserInfo.Vehicle.BlockMOT === "true") {
                if (message === "") { message = title; }
                message = message + "<br />\u2022 MOT";
            }
            if (CurrentUserInfo.Vehicle.BlockInsurance === "true") {
                if (message === "") { message = title; }
                message = message + "<br />\u2022 Insurance";
            }
            if (CurrentUserInfo.Vehicle.BlockBreakDownCover === "true") {
                if (message === "") { message = title; }
                message = message + "<br />\u2022 Breakdown Cover";
            }
            if (vehicleType === "Bicycle") { message = ""; }

            if (message !== "") {
                message = message + "<br /><br /> <a href=/shared/viewentities.aspx?entityid=" + sessionStorage.entityview.split(',')[0] + "&viewid=" + sessionStorage.entityview.split(',')[1] + " target=_blank><u>Click here to add new vehicle documents now.</u></a>"
                if (isadmin) {
                    message = message + "<br /><br /> If you like to upload later please navigate to <b> Home | My Team's Duty of Care Documents | My Team's Vehicle Documents and click on New Vehicle Document</b>"
                }
                else {
                    message = message + "<br /><br /> If you like to upload later please navigate to <b> Home | My Details | My Duty of Care Documents | My Vehicle Documents and click on New Vehicle Document</b>"
                }
            }
        }
        sessionStorage.clear();
            SEL.MasterPopup.ShowMasterPopup(messageHeader + message + "<br /><br />");
            vehicleType = "";
    }
}

function getDocEntityAndViewIdComplete(data) {
    sessionStorage.setItem('entityview', data);
}


function populateCarGrid() {
    PageMethods.getCarGrid(nEmployeeid, getCarGridComplete);
}

function getCarGridComplete(data) {
    if ($e(pnlCarsGrid) === true) {
        $g(pnlCarsGrid).innerHTML = data[2];
        SEL.Grid.updateGrid(data[1]);
    }
}

function editCar(id) 
{
    //to hide the body scroll
    $('body').addClass('bodyScrollDisable');
    // When editing there's no need to offer to replace a previous vehicle, that functionality is just for a new vehicle
    // Set the yes/no to "No" so it will pass validation and hide the section
    $(".replaceCar input[value=0]").prop("checked", true);
    $(".replaceCar").closest(".formpanel").hide();
    
    carid = id;
    isShallowSave = false;
    if (!editOnly)
    {
        clearCarModal();
    }
    
    
    if (isPoolCar)
    {
        Spend_Management.svcCars.getPoolCar(id, getCarComplete, commandFail);
    }
    else
    {
        Spend_Management.svcCars.getCar(nEmployeeid, id, getCarComplete, commandFail);
    }
}

function getCarComplete(data) {

    document.getElementById(rvVehicleType).style.visibility = "hidden";
    document.getElementById(reqmake).style.visibility = "hidden";
    document.getElementById(reqmodel).style.visibility = "hidden";

    if (data[0].VehicleTypeID == 1) {
        document.getElementById(txtregistration).disabled = true;
        document.getElementById(txtenginesize).disabled = true;
        document.getElementById(cmbcartype).disabled = true;

        document.getElementById(reqreg).enabled = false;
        document.getElementById(rfEngineSize).enabled = false;
        document.getElementById(cvEngineSize).enabled = false;
        document.getElementById(rvEngineType).enabled = false;
    } else {
        document.getElementById(txtregistration).disabled = false;
        document.getElementById(txtenginesize).disabled = false;
        document.getElementById(cmbcartype).disabled = false;

        document.getElementById(reqreg).enabled = true;
        document.getElementById(rfEngineSize).enabled = true;
        document.getElementById(cvEngineSize).enabled = true;
        document.getElementById(rvEngineType).enabled = true;

        document.getElementById(reqreg).style.visibility = "hidden";
        document.getElementById(rfEngineSize).style.visibility = "hidden";
        document.getElementById(cvEngineSize).style.visibility = "hidden";
        document.getElementById(rvEngineType).style.visibility = "hidden";
    }

    mcChecks = new Array();
    if (data[0].Approved == false) {
        approved = false;
        document.getElementById(cmdSaveID).src = appPath + "/shared/images/buttons/pagebutton_activate.gif";
    }
    else {
        document.getElementById(cmdSaveID).src = appPath + "/shared/images/buttons/btn_save.png";
    }

    document.getElementById(txtmake).value = data[0].make;
    document.getElementById(txtmodel).value = data[0].model;
    document.getElementById(txtregistration).value = data[0].registration;
    $('#' + cmbcartype).val(data[0].VehicleEngineTypeId);
    var i;
    for (i = 0; i < document.getElementById(cmbUom).options.length; i++) {
        if (document.getElementById(cmbUom).options[i].value == data[0].defaultuom) {
            document.getElementById(cmbUom).selectedIndex = i;
            mcChecks[i] = new Array();
            mcChecks[i] = data[0].mileagecats;
            document.getElementById(cmbUom).onchange();
            break;
        }
    }

    document.getElementById(chkactive).checked = data[0].active;
    for (i = 0; i < document.getElementById(cmbcartype).options.length; i++) {
        if (document.getElementById(cmbcartype).options[i].value == data[0].cartypeid) {
            document.getElementById(cmbcartype).selectedIndex = i;
            break;
        }
    }

    for (i = 0; i < document.getElementById(cmbvehicletype).options.length; i++) {
        if (document.getElementById(cmbvehicletype).options[i].value == data[0].VehicleTypeID) {
            document.getElementById(cmbvehicletype).selectedIndex = i;
            break;
        }
    }
    document.getElementById(chkodometerreading).checked = data[0].fuelcard;
    document.getElementById(txtstartodo).value = data[0].odometer;
    document.getElementById(txtstartodo).disabled = false;

    if (parseInt(data[0].endodometer) != 0)
    {
        document.getElementById(txtendodo).value = data[0].endodometer;
    }
    document.getElementById(txtenginesize).value = data[0].EngineSize;

 
    if (data[0].startdate.format('dd/MM/yyyy') != '01/01/1900') {
        document.getElementById(txtstart).value = data[0].startdate.format('dd/MM/yyyy');
    }
    else {
        document.getElementById(txtstart).value = '';
    }
    if (data[0].enddate.format('dd/MM/yyyy') != '01/01/1900') {
        document.getElementById(txtend).value = data[0].enddate.format('dd/MM/yyyy');
    }
    else {
        document.getElementById(txtend).value = '';
    }
    document.getElementById(chkexemptfromhometooffice).checked = data[0].ExemptFromHomeToOffice;

    /*var mileagechkboxes = document.getElementsByName('selectgridMileage');

    for (var i = 0; i < mileagechkboxes.length; i++) {
    mileagechkboxes[i].checked = false;
    }
    for (var i = 0; i < data[0].mileagecats.length; i++) {
    for (var x = 0; x < mileagechkboxes.length; x++) {
    if (mileagechkboxes[x].value == data[0].mileagecats[i])
    {
    mileagechkboxes[x].checked = true;
    break;
    }
    }

    }*/

    var i;
    var j;
    var control;

    //Populate user defined fields from the web method
    for (i = 0; i < lstUserdefined.length; i++) {
        for (j = 0; j < data[1].length; j++) {
            control = undefined;

            if (lstUserdefined[i][0] == data[1][j][0]) {
                control = document.getElementById(lstUserdefined[i][2]);
            }

            if (control != undefined) {

                switch (lstUserdefined[i][1]) {
                    case 'Text':
                    case 'DynamicHyperlink':
                    case 'Currency':
                    case 'Number':
                    case 'Integer':
                        if (data[1][j][1] == null) {
                            control.value = '';
                        }
                        else {
                            control.value = data[1][j][1];
                        }
                        break;
                    case 'Relationship':
                        if (data[1][j][1] == null)
                        {
                            control.value = '';
                        }
                        else
                        {
                            control.value = data[1][j][2]; // text value
                            // put ID value into hidden field
                            var hiddenControl = $g(lstUserdefined[i][2] + '_ID');
                            if (hiddenControl != undefined)
                            {
                                hiddenControl.value = data[1][j][1];
                            }
                        }
                        break;
                    case 'LargeText':
                        if (data[1][j][1] == null) {
                            control.value = '';
                        }
                        else {
                            control.value = data[1][j][1];

                            var contentCtl = $get(lstUserdefined[i][2] + '_ctl02_ctl00');

                            if (contentCtl != null || contentCtl != undefined) {
                                contentCtl.contentWindow.document.body.innerHTML = data[1][j][1];
                            }
                        }
                        break;
                    case 'TickBox':
                        //control.selectedIndex = 0; -- THIS OVERRIDES THE DEFAULT
                        if (data[1][j][1] != null) {
                            // only set selected item if record held, otherwise leave as default
                            var selectVal = (data[1][j][1] == true ? '1' : '0');

                            for (var x = 0; x < control.options.length; x++) {
                                if (control.options[x].value == selectVal) {
                                    control.selectedIndex = x;
                                    break;
                                }
                            }
                        }
                        break;
                    case 'List':
                        control.selectedIndex = 0;
                        for (var x = 0; x < control.options.length; x++) {
                            if (control.options[x].value == data[1][j][1]) {
                                control.selectedIndex = x;
                                break;
                            }
                        }
                        break;
                    case 'DateTime':
                        if (data[1][j][1] == null) {
                            control.value = '';
                        }
                        else {
                            var tmpDate = data[1][j][1];
                            if (tmpDate.substring(0, 10) != '01/01/1900' && tmpDate != '00:00') {
                                control.value = data[1][j][1];
                            }
                        }
                }

            }
        }
    }

    var odoReadingsGridDetail = SEL.Grid.getGridById();
    if (odoReadingsGridDetail !== null) {
        odoReadingsGridDetail.filters[0].values1[0] = carid;
        odoReadingsGridDetail.enablePaging = true;
    }
    Spend_Management.svcCars.createOdoGrid(carid, createOdoGridComplete, commandFail);
    Spend_Management.svcCars.createEsrDetails(carid, createEsrDetailsComplete, commandFail);
    Spend_Management.svcCars.GetFinancialYears(nEmployeeid, GetFinancialYearsComplete, commandFail);
}

function clearCarModal()
{
    mcChecks = new Array();
    
    document.getElementById(txtmake).value = "";
    document.getElementById(txtmodel).value = "";
    document.getElementById(txtregistration).value = "";
    $("#lookupError").hide();

    document.getElementById(cmbUom).selectedIndex = 0;

    var chkactiveItem = document.getElementById(chkactive);
    if (chkactiveItem !== null)
    {
        chkactiveItem.checked = false;
    }
    
    document.getElementById(cmbcartype).selectedIndex = 0;
    var chkodometerreadingItem = document.getElementById(chkodometerreading);
    if (chkodometerreadingItem !== null)
    {
        chkodometerreadingItem.checked = false;    
    }
    
    var txtstartodoItem = document.getElementById(txtstartodo);
    if (txtstartodoItem !== null)
    {
        txtstartodoItem.value = "";    
    }
    
    var txtendodoItem = document.getElementById(txtendodo);
    if (txtendodoItem !== null)
    {
        txtendodoItem.value = "";    
    }    

    var txtenginesizeItem = document.getElementById(txtenginesize);
    if (txtenginesizeItem !== null)
    {
        txtenginesizeItem.value = "";    
    }


    if ($f(carTabContainer) !== null)
    {
        $f(carTabContainer).set_activeTabIndex(0);
    }

    var txtstartItem = document.getElementById(txtstart);
    if (txtstartItem !== null)
    {
        txtstartItem.value = "";
    }

    var txtendItem = document.getElementById(txtend);
    if (txtendItem !== null)
    {
        txtendItem.value = "";
    }

    var chkexemptfromhometoofficeItem = document.getElementById(chkexemptfromhometooffice);
    if (chkexemptfromhometoofficeItem !== null)
    {
        chkexemptfromhometoofficeItem.checked = false;    
    }

    var mileagechkboxes = document.getElementsByName('selectgridMileage');
    var i;

    for (i = 0; i < mileagechkboxes.length; i++) 
    {
        mileagechkboxes[i].checked = false;
    }

    if ($e(mileageGrid) === true) {
        $g(mileageGrid).innerHTML = "";
        $g(mileageGrid).innerHTML = "";
        if (carid === 0) {
            Spend_Management.svcCars.createMileageGrid(0, 1, 0, filterMileageGridComplete, commandFail);
        }
    }

    var litGrid = document.getElementById(pnlodogrid);

    if (litGrid != undefined) 
    {
        litGrid.innerHTML = "";
    }

    var control;

    //Populate user defined fields from the web method
    for (i = 0; i < lstUserdefined.length; i++)
    {
        control = document.getElementById(lstUserdefined[i][2]);
        
        if (control != undefined && lstUserdefined[i][3] == "ValidationSummaryAeCar")
        {
            switch (lstUserdefined[i][1])
            {
                case 'Text':
                case 'Currency':
                case 'Number':
                case 'Integer':
                case 'DateTime':
                case 'LargeText':
                case 'DynamicHyperlink':
                    control.value = "";
                    break;
                case 'Relationship':
                    control.value = "";
                    var hiddenControl = $g(lstUserdefined[i][2] + '_ID');
                    if (hiddenControl != undefined)
                    {
                        hiddenControl.value = '';
                    }
                    break;
                case 'TickBox':
                    if (lstUserdefined[i][5] != "") {
                        for (var idx = 0; idx < control.options.length; idx++) {
                            if (lstUserdefined[i][5] == control.options[idx].text) {
                                control.options[idx].selected = true;
                                break;
                            }
                        }
                    }
                    else {
                        control.selectedIndex = 0;
                    }
                    break;
                case 'List':
                    control.selectedIndex = 0;  
                    break;
            }
        }
    }

    Spend_Management.svcCars.GetFinancialYears(nEmployeeid, GetFinancialYearsComplete, commandFail);
}

//Odometer Reading Functions

function createOdoGridComplete(data) {
    if ($e(pnlodogrid) === true) {
        var rowCount = data[2].match(/<tr /g).length;
        if (rowCount > 1) {
            document.getElementById(txtstartodo).disabled = true;
            var oldReading = data[2].match(/>\d\w*</g)[0].replace("<", "").replace(">", "");
            var screenStart = document.getElementById(txtstartodo).value;
            if (oldReading !== screenStart) {
                document.getElementById(txtstartodo).value = oldReading;
            }
        }
        
        $g(pnlodogrid).innerHTML = data[2];
        SEL.Grid.updateGrid(data[1]);
    }
    if (!editOnly)
    {
        showCarModal();
    }
    
    return;
}

function createEsrDetailsComplete(data)
{
    if ($e(carEsrTabPage) === true) {
        if (data === "")
        {
            $g(pnlesrdetail).innerHTML = '<div class="sectiontitle">No ESR Vehicle details found.</div>';
            $f(carEsrTabPage).set_enabled(false);
        } else
        {
            $f(carEsrTabPage).set_enabled(true);
            $g(pnlesrdetail).innerHTML = data;
        }
    }
}

function GetFinancialYearsComplete(data)
{
    var financialYear = document.getElementById(ddlFinancialYear);
    if (financialYear != null) {
        removeOptions(financialYear);
        for (var x = 0; x < data.length; x++) {
            option = document.createElement("OPTION");
            option.value = data[x].Value;
            option.selected = data[x].Selected;
            option.text = data[x].Text;
            financialYear.options.add(option);
        }
    }
}

function removeOptions(selectbox) {
    var i;
    for (i = selectbox.options.length - 1; i >= 0; i--) {
        selectbox.remove(i);
    }
}

function appendAttachment() 
{
    if (carid == 0) 
    {
        isShallowSave = true;
        saveCar(false);
        return;
    }
    else 
    {
        var ifrFile = document.getElementById('iFrCarDocAttach');
        ifrFile.contentWindow.document.getElementById('GenIDVal').value = carid;
        ifrFile.contentWindow.document.getElementById('DocType').value = attachDocType;
        showAttachmentModal();
    }
}

function showAttachmentModal() 
{
    $find(modAttachmentID).show();
}

function hideAttachmentModal() 
{
    $find(modAttachmentID).hide();
}

function commandFail(error) 
{
    if (error["_message"] != null) 
    {
        SEL.MasterPopup.ShowMasterPopup(error["_message"], "Web Service Message");
    }
    else 
    {
        SEL.MasterPopup.ShowMasterPopup(error, "Web Service Message");
    }
    return;
}

function saveOdometerReading() 
{
    if (validateform('vgOdo') == false)
        return;

    var dateStamp = null;
    if (document.getElementById(txtReadingDateID).value != "") 
    {
        dateStamp = document.getElementById(txtReadingDateID).value.substring(6, 10) + "/" + document.getElementById(txtReadingDateID).value.substring(3, 5) + "/" + document.getElementById(txtReadingDateID).value.substring(0, 2);
    }

    var oldodometer = 0;
    if (document.getElementById(txtOldReadingID).value != '') 
    {
        oldodometer = new Number(document.getElementById(txtOldReadingID).value);
    }
    var newodometer = 0;
    if (document.getElementById(txtNewReadingID).value != '') 
    {
        newodometer = new Number(document.getElementById(txtNewReadingID).value);
    }

    Spend_Management.svcCars.saveOdometerReading(odometerid, carid, nEmployeeid, dateStamp, oldodometer, newodometer, saveOdometerReadingComplete, commandFail);
}

function saveOdometerReadingComplete(data) 
{
    odometerid = data;
    populateOdometerReadings();
    hideOdometerModal();
}

function editOdometerReading(odoID) 
{
    odometerid = odoID;
    Spend_Management.svcCars.getOdometerReading(odoID, editOdometerReadingComplete, commandFail);
}

function editOdometerReadingComplete(data) 
{
    document.getElementById(txtReadingDateID).value = data.datestamp.format('dd/MM/yyyy');
    document.getElementById(txtOldReadingID).value = data.oldreading;
    document.getElementById(txtNewReadingID).value = data.newreading;
    showOdometerModal(false);
}

function clearOdometerModal()
{
    document.getElementById(txtReadingDateID).value = "";
    document.getElementById(txtOldReadingID).value = "";
    document.getElementById(txtNewReadingID).value = "";
    return;
}

function showOdometerModal(isNew)
{
    if (isNew)
    {
        clearOdometerModal();
    }

    $find(modOdoReadingID).show();
}

function hideOdometerModal() 
{
    var modal = $find(modOdoReadingID);
    modal.hide();
}

function populateOdometerReadings() 
{
    Spend_Management.svcCars.createOdoGrid(carid, createOdoGridComplete, commandFail);
}

function deleteOdometerReading(odoID)
{
    Spend_Management.svcCars.deleteOdometerReading(carid, odoID, deleteOdoComplete, commandFail);
}
function deleteOdoComplete(data)
{
    populateOdometerReadings();
//    var litGrid = document.getElementById(pnlodogrid);

//    if (litGrid != undefined) 
//    {
//        litGrid.innerHTML = data;
//    }

    //showCarModal();
    return;
}


function filterMileageGrid() {
    var uom = document.getElementById(cmbUom).options[document.getElementById(cmbUom).selectedIndex].value;
    var financialYearID = document.getElementById(ddlFinancialYear).options[document.getElementById(ddlFinancialYear).selectedIndex].value;
    var engineTypeId = document.getElementById(cmbcartype).options[document.getElementById(cmbcartype).selectedIndex].value;
    Spend_Management.svcCars.createMileageGrid(uom, financialYearID, engineTypeId, filterMileageGridComplete, commandFail);
}

function filterMileageGridComplete(data) {
    if ($e(mileageGrid) === true) {
        $g(mileageGrid).innerHTML = data[2];
        SEL.Grid.updateGrid(data[1]);

        var selectedUom = document.getElementById(cmbUom).selectedIndex;
        if (mcChecks[selectedUom] != null) {
            var mileagechkboxes = document.getElementsByName('selectgridMileage');
            for (var i = 0; i < mileagechkboxes.length; i++) {
                mileagechkboxes[i].checked = false;
            }
            for (i = 0; i < mcChecks[selectedUom].length; i++) {
                for (var x = 0; x < mileagechkboxes.length; x++) {
                    if (mileagechkboxes[x].value == mcChecks[selectedUom][i]) {
                        mileagechkboxes[x].checked = true;
                        break;
                    }
                }
            }
        }
    }
    return;
}

function UpdateCarCache()
{
    if (isPoolCar)
    {
        Spend_Management.svcCars.updatePoolCarCache(carid);
    }
    else if (isAdmin)
    {
        Spend_Management.svcCars.updateEmployeeCache(nEmployeeid);
    }
    return;
}

function onCarDeleteComplete(retVal)
{
    if (retVal !== 0)
    {
        SEL.MasterPopup.ShowMasterPopup('An error occurred attempting to undo the added vehicle and associated documents', 'Message from ' + moduleNameHTML);
    }

    return;
}

function ResetCarID()
{
    carid = 0;
}

function LookupVehicleDetails() {
    var registration = $('#'+ txtregistration).val();
    if (registration && registration.length > 1) {
        $('#lookupError').text(' ').hide();
        $('#divWait').show();
        Spend_Management.svcCars.LookupVehicle(registration, LookupVehicleDetailsComplete);    
    }
    
}

function LookupVehicleDetailsComplete(car) {
    $('#divWait').hide();
    if (car !== undefined && car !== null) {
        $('#' + txtmake).val(car.make);
        $('#' + txtmodel).val(car.model);
        $('#' + txtregistration).val(car.registration);
        $('#' + txtenginesize).val(car.EngineSize);
        $('#' + cmbvehicletype).val(car.VehicleTypeID);
        $('#' + cmbcartype).val(car.VehicleEngineTypeId);
        filterMileageGrid();
        return;
    };

    $('#lookupError').text('Could not find vehicle in DVLA database').show();
}


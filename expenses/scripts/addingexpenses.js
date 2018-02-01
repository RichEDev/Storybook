// On page load and every .Net forms postback
Sys.Application.add_load(function (application, applicationEventArguments) {

    // rebind any existing address widgets to the replaced element
    if (applicationEventArguments._isPartialLoad) {
        $.address.rebind();
    }

    var extractIds = function (str) {
        var result = null;
        var match = /(\d+)_(\d+)/.exec(str);

        if (match !== null) {
            result = {
                id: +match[1],
                stepId: +match[2]
            };
        }

        return result;
    };


    // create a any new address widgets if necessary
    $("input[type=text].ui-sel-address-picker").each(function (index, element) {

        // bind/rebind all mileage address fields 
        var field = $(element);
        var hiddenField = SEL.AddressesAndTravel.GetAddressPickerHiddenField(field);

        // add expense item specific behaviour for "home" and "office" addresses and mileage calculations
        field.on({
            "focus": function () {
                // legacy address modal
                this.InitialValue = element.value;
            },
            "blur": function () {
                SEL.AddressesAndTravel.PopulateAddressPickerByKeyword(field);
            }
        });

        // when the hidden field changes update the mileage caculation for the row
        hiddenField.on("change", function () {
            var ids = extractIds(field.attr("id"));
            if (ids !== null) {
                processMileageValue(ids.id, ids.stepId);
            }
        });

        // if the widget hasn't already been created, create it
        var found = false;
        for (var i = 0; i < $.address.instances.length; i += 1) {
            if (field.attr("id") === $.address.instances[i].elementId) {
                found = true;
            }
        }

        if (found === false) {
            // attempt to retrieve the esr assignment <select>
            var esrAssignmentField = field.parents(".mileagePanel").find(".esrAssignment select").first();

            field.address({
                backgroundElement: hiddenField,
                enableLabels: !CurrentUserInfo.IsDelegate,
                enableFavourites: !CurrentUserInfo.IsDelegate,
                enableAddingAddresses: allowClaimantAddManualAddresses,
                enableHomeAndOffice: true,
                esrAssignmentField: esrAssignmentField
            });
        }
    });

    // attach organisation autocompletes
    $(".organisation-autocomplete").each(function (i, o) {
        SEL.AutoComplete.Bind(o.id, 15,
            '7bdaf84e-a373-4008-83d1-9e18aaa47f8e',
            '4d0f2409-0705-4f0f-9824-42057b25aebe',
            '4d0f2409-0705-4f0f-9824-42057b25aebe, ac87c4c4-9107-4555-b2a3-27109b3ebfbb', null,
            '{ 0: { "FieldID": "4B7873D6-8EDC-44D4-94F7-B8ABCBD87692", "ConditionType": 1, "ValueOne": "0", "ValueTwo": "", "Order": 0, "JoinViaID": 0 } }',
            500, null, "False", null, null, SEL.Expenses.Validate.Organisation.SaveIfNotExists);

        var searchName = $(o).data("search");
        if (typeof searchName === "string" && searchName !== "") {
            OrganisationSearches[searchName] = AutoCompleteSearches.New("Organisation", o.id, OrganisationSearchModal, OrganisationSearchPanel);
        }
    });

    // attach cost code autocompletes
    $(".costcode-autocomplete").each(function (i, o) {
        SEL.AutoComplete.Bind(o.id, 15,
            '7bdaf84e-a373-4008-83d1-9e18aaa47f8e',
            '4d0f2409-0705-4f0f-9824-42057b25aebe',
            '4d0f2409-0705-4f0f-9824-42057b25aebe, ac87c4c4-9107-4555-b2a3-27109b3ebfbb', null,
            '{ 0: { "FieldID": "4B7873D6-8EDC-44D4-94F7-B8ABCBD87692", "ConditionType": 1, "ValueOne": "0", "ValueTwo": "", "Order": 0, "JoinViaID": 0 } }',
            500, null, "False", null, null, SEL.Expenses.Validate.Organisation.SaveIfNotExists);

        var searchName = $(o).data("search");
        if (typeof searchName === "string" && searchName !== "") {
            CostCodeSearches[searchName] = AutoCompleteSearches.New("CostCode", o.id, CostCodeSearchModal, CostCodeSearchPanel);
        }
    });

});

function MileageDetailsUpdated(controlUpdated) {
    if (controlUpdated.InitialValue != controlUpdated.value && controlUpdated.value != "") {
        var txtCalculatedMileage;
        var txtMileage;
        var labCalcMiles;
        var hiddenFieldID;

        if (controlUpdated.id.indexOf("txtto") > -1) {
            hiddenFieldID = controlUpdated.id.replace("txtto", "txttoid");
            txtCalculatedMileage = document.getElementById(controlUpdated.id.replace("txtto", "txtCalcMiles"));
            txtMileage = document.getElementById(controlUpdated.id.replace("txtto", "txtmileage"));
            labCalcMiles = document.getElementById(controlUpdated.id.replace("txtto", "labCalcMiles"));
        }
        else if (controlUpdated.id.indexOf("txtfrom") > -1) {
            hiddenFieldID = controlUpdated.id.replace("txtfrom", "txtfromid");
            txtCalculatedMileage = document.getElementById(controlUpdated.id.replace("txtfrom", "txtCalcMiles"));
            txtMileage = document.getElementById(controlUpdated.id.replace("txtfrom", "txtmileage"));
            labCalcMiles = document.getElementById(controlUpdated.id.replace("txtfrom", "labCalcMiles"));
        }

        var hiddenField = document.getElementById(hiddenFieldID);
        if (hiddenField !== null) {
            hiddenField.value = "";
            txtCalculatedMileage.value = "";
            txtMileage.value = "";
            labCalcMiles.innerHTML = "";
        }
    }

    return;
}

function processMileageViaAutoComplete(source) {
    var sourceControlID = source._element.id;
    var expenseInstanceIDAndStepID = "";
    var expenseInstanceID;
    var stepID;
    if (sourceControlID.indexOf("txtto") > -1) {
        expenseInstanceIDAndStepID = sourceControlID.split("txtto");
    }
    else if (sourceControlID.indexOf("txtfrom") > -1) {
        expenseInstanceIDAndStepID = sourceControlID.split("txtfrom");
    }

    if (expenseInstanceIDAndStepID !== "") {
        expenseInstanceID = expenseInstanceIDAndStepID[1].split("_")[0];
        stepID = expenseInstanceIDAndStepID[1].split("_")[1];
    }

    if (expenseInstanceID != null) {
        processMileageValue(expenseInstanceID, stepID);
    }
}

function ShowRouteAddEditExpenses(mileageGrid) {
    var addressIDs = [];
    $("input[data-field='from_id'], input[data-field='to_id']", mileageGrid).each(function () {
        var id = $(this).val();
        if (id) {
            addressIDs.push(id);
        }
    });
    var claimEmpIdElement = $("input[id*=claimempid]", $(mileageGrid).closest(".mileagePanel"));
    var claimEmpId = 0;
    if (claimEmpIdElement.length > 0) {
        claimEmpId = claimEmpIdElement.val();
    }
    SEL.AddressesAndTravel.GetRouteByAddressIdsForEmployee(addressIDs, claimEmpId);
}

function processMileageValue(expenseInstanceID, journeyStepIndex) {
    var txtCalculatedMileage = document.getElementById(contentID + 'txtCalcMiles' + expenseInstanceID + "_" + journeyStepIndex);
    var txtMileage = document.getElementById(contentID + 'txtmileage' + expenseInstanceID + "_" + journeyStepIndex);
    var labCalcMiles = document.getElementById(contentID + 'labCalcMiles' + expenseInstanceID + "_" + journeyStepIndex);
    var txtToBox = document.getElementById(contentID + 'txtto' + expenseInstanceID + "_" + journeyStepIndex);
    var hdnToBoxID = document.getElementById(contentID + 'txttoid' + expenseInstanceID + "_" + journeyStepIndex);
    var txtFromBox = document.getElementById(contentID + 'txtfrom' + expenseInstanceID + "_" + journeyStepIndex);
    var hdnFromBoxID = document.getElementById(contentID + 'txtfromid' + expenseInstanceID + "_" + journeyStepIndex);
    var txtExpenseItemDate = document.getElementById(contentID + 'txtdate').value;
    txtExpenseItemDate = txtExpenseItemDate.substring(6, 10) + '/' + txtExpenseItemDate.substring(3, 5) + '/' + txtExpenseItemDate.substring(0, 2);
    var carID = 0;
    var cmbcars = document.getElementById(contentID + 'cmbcars' + expenseInstanceID);
    if (cmbcars != null) {
        if (cmbcars.selectedIndex == -1) {
            carID = 0;
        }
        else {
            carID = cmbcars.options[cmbcars.selectedIndex].value;
        }
    }

    if (hdnFromBoxID != null && hdnToBoxID != null && hdnFromBoxID.value !== "" && hdnToBoxID.value !== "") {
        var ajaxLoader = "<img src=\"./shared/images/ajax-loader.gif\" alt=\"Loading...\" style=\"vertical-align: middle;\" />";
        labCalcMiles.innerHTML = ajaxLoader;

        StartAddLoader(expenseInstanceID, journeyStepIndex);

        PageMethods.GetDistance(hdnFromBoxID.value, txtFromBox.value, hdnToBoxID.value, txtToBox.value, txtExpenseItemDate, carID,
            function (distance) {
                if (distance[0] == 1) {
                    var distanceTotal = document.getElementById(contentID + 'txtmileage' + expenseInstanceID + "_" + journeyStepIndex).value;

                    if (distance != distance[1]) {
                        document.getElementById(contentID + 'txtmileage' + expenseInstanceID + "_" + journeyStepIndex).value = distance[1];
                        document.getElementById(contentID + 'labCalcMiles' + expenseInstanceID + "_" + journeyStepIndex).innerHTML = distance[1];
                        document.getElementById(contentID + 'txtCalcMiles' + expenseInstanceID + "_" + journeyStepIndex).value = distance[1];
                    }
                }
                else if (distance[0] == -1) {
                    document.getElementById(contentID + 'txtmileage' + expenseInstanceID + "_" + journeyStepIndex).value = "";
                    document.getElementById(contentID + 'labCalcMiles' + expenseInstanceID + "_" + journeyStepIndex).innerHTML = "<img src=\"./shared/images/icons/warning.png\" alt=\"Missing Postcodes\" id=\"iconMissingPostcodes\" onmouseover=\"ShowMissingPostcodesPopup(event, this);\" onmouseout=\"HideMissingPostcodesPopup(event);\" style=\"cursor: pointer; vertical-align: middle;\" />";
                    document.getElementById(contentID + 'txtCalcMiles' + expenseInstanceID + "_" + journeyStepIndex).value = "-1";
                }

                StopAddLoader(expenseInstanceID, journeyStepIndex);
            },
            function (errorMessage) {
                txtCalculatedMileage.value = "";
                txtMileage.value = "";
                labCalcMiles.innerHTML = "";

                StopAddLoader(expenseInstanceID, journeyStepIndex);
            });
    }
    else {
        if (labCalcMiles != null && txtCalculatedMileage != null && txtMileage != null) {
            labCalcMiles.innerHTML = "";
            txtCalculatedMileage.value = "";
            txtMileage.value = "";
        }
    }
}



var addressOnFocus = "";
function SetExistingAddress(controlObject, expenseInstanceID, stepID) {
    if (controlObject.value !== "") {
        addressOnFocus = controlObject.value;
    }
    return;
}

function SetUpdateMileage(expenseInstanceID, stepID, value) {
    var hiddenField = document.getElementById(contentID + "hdnUpdateMileage" + expenseInstanceID + "_" + stepID);

    if (hiddenField !== null) {
        if (value === undefined) {
            hiddenField.value = "1";
        }
        else {
            hiddenField.value = value;
        }
    }
}


function LoadMileageDistance(fromControl, fromControlID, toControl, toControlID, expenseItemIndex, journeyStepIndex, previousFromID, previousToID, fromMethod) {
    //if(fromMethod == "onblur") { return; }
    var fromControl = document.getElementById(fromControl);
    var fromControlIDObject = document.getElementById(fromControlID);
    var fromPreviousControl = document.getElementById(previousFromID);
    var toControl = document.getElementById(toControl);
    var toControlIDObject = document.getElementById(toControlID);
    var toPreviousControl = document.getElementById(previousToID);
    var txtCalculatedMileage = document.getElementById(contentID + 'txtCalcMiles' + expenseItemIndex + "_" + journeyStepIndex).value;

    if (((toControlIDObject.value !== toPreviousControl.value && toPreviousControl.value !== "") || (toPreviousControl.value === "" && (txtCalculatedMileage === "" || txtCalculatedMileage == "0"))) || ((fromControlIDObject.value !== fromPreviousControl.value && fromPreviousControl.value !== "") || (fromPreviousControl.value === "" && (txtCalculatedMileage === "" || txtCalculatedMileage == "0")))) {


        if (fromControl !== null && toControl != null) {
            //setTimeout(function() {
            if (fromControl.value != "" && toControl.value != "") {
                var txtdate = document.getElementById(contentID + 'txtdate').value;
                var date = txtdate.substring(6, 10) + '/' + txtdate.substring(3, 5) + '/' + txtdate.substring(0, 2);
                var carid = 0;
                var cmbcars = document.getElementById(contentID + 'cmbcars' + expenseItemIndex);

                if (cmbcars != null) {
                    carid = cmbcars.options[cmbcars.selectedIndex].value;
                }

                var labCalcMilesObj = document.getElementById(contentID + 'labCalcMiles' + expenseItemIndex + "_" + journeyStepIndex);

                if (labCalcMilesObj !== null) {
                    labCalcMilesObj.innerHTML = '<img src="./shared/images/ajax-loader.gif" alt="Loading..." style="vertical-align: middle;" />';
                }

                StartAddLoader(expenseItemIndex, journeyStepIndex);

                PageMethods.GetDistance(fromControl.value, toControl.value, date, carid, function (distance) {
                    if (labCalcMilesObj !== null) {
                        labCalcMilesObj.innerHTML = '&nbsp;';
                    }

                    if (distance[0] == 1) {
                        var distanceTotal = document.getElementById(contentID + 'txtmileage' + expenseItemIndex + "_" + journeyStepIndex).value;

                        if (distance != distance[1]) {
                            document.getElementById(contentID + 'txtmileage' + expenseItemIndex + "_" + journeyStepIndex).value = distance[1];
                            document.getElementById(contentID + 'labCalcMiles' + expenseItemIndex + "_" + journeyStepIndex).innerHTML = distance[1];
                            document.getElementById(contentID + 'txtCalcMiles' + expenseItemIndex + "_" + journeyStepIndex).value = distance[1];
                        }
                    }
                    else if (distance[0] == -1) {
                        document.getElementById(contentID + 'txtmileage' + expenseItemIndex + "_" + journeyStepIndex).value = "";
                        document.getElementById(contentID + 'labCalcMiles' + expenseItemIndex + "_" + journeyStepIndex).innerHTML = "<img src=\"./shared/images/icons/warning.png\" alt=\"Missing Postcodes\" id=\"iconMissingPostcodes\" onmouseover=\"ShowMissingPostcodesPopup(event, this);\" onmouseout=\"HideMissingPostcodesPopup(event);\" style=\"cursor: pointer; vertical-align: middle;\" />";
                        document.getElementById(contentID + 'txtCalcMiles' + expenseItemIndex + "_" + journeyStepIndex).value = "-1";
                    }

                    StopAddLoader(expenseItemIndex, journeyStepIndex);

                    return;
                },
                    function (error) {
                        document.getElementById(contentID + 'labCalcMiles' + expenseItemIndex + "_" + journeyStepIndex).innerHTML = '';
                        document.getElementById(contentID + 'txtmileage' + expenseItemIndex + "_" + journeyStepIndex).value = "";
                        document.getElementById(contentID + 'txtCalcMiles' + expenseItemIndex + "_" + journeyStepIndex).value = "";

                        StopAddLoader(expenseItemIndex, journeyStepIndex);

                        return;
                    }
                    );
            }
            else {
                var box = document.getElementById(contentID + 'labCalcMiles' + expenseItemIndex + "_" + journeyStepIndex);
                if (box !== null) {
                    box.innerHTML = "";
                }
            }
            //}, 0);
        }
    }
    return;
}



function ShowMissingPostcodesPopup(evt, control) {
    stopPropagation(evt);
    $find(pceMissingPostCodes)._popupBehavior._parentElement = control;
    $find(pceMissingPostCodes)._popupElement.style.zIndex = GetHighestZIndex() + 1;
    $find(pceMissingPostCodes).showPopup();
    return;
}

function HideMissingPostcodesPopup(evt) {
    stopPropagation(evt);
    $find(pceMissingPostCodes).hidePopup();
    return;
}

function ShowDisabledAddPopup(evt, control) {
    stopPropagation(evt);
    $find(pceGreyAdd)._popupBehavior._parentElement = control;
    $find(pceGreyAdd)._popupElement.style.zIndex = GetHighestZIndex() + 1;
    $find(pceGreyAdd).showPopup();
    return;
}
function ShowProcessingAddPopup(evt, control) {
    stopPropagation(evt);
    $find(pceProcessingAdd)._popupBehavior._parentElement = control;
    $find(pceProcessingAdd)._popupElement.style.zIndex = GetHighestZIndex() + 1;
    $find(pceProcessingAdd).showPopup();
    return;
}
function HideAddPopups(evt) {
    stopPropagation(evt);
    $find(pceGreyAdd).hidePopup();
    $find(pceProcessingAdd).hidePopup();
    return;
}

function StartAddLoader(itemID, stepID) {
    var prefix = 'ctl00_contentmain_';
    var postfix = itemID + '_' + stepID;
    var iconAdd = document.getElementById(prefix + 'adddestination' + postfix);
    var iconGreyAdd = document.getElementById(prefix + 'greyCross' + postfix);
    var iconLoaderAdd = document.getElementById(prefix + 'ajaxLoader' + postfix);

    if (iconAdd != null) {
        iconAdd.style.display = 'none';
    }
    if (iconGreyAdd != null) {
        iconGreyAdd.style.display = 'none';
    }
    if (iconLoaderAdd != null) {
        iconLoaderAdd.style.display = '';
    }

    return;
}
function StopAddLoader(itemID, stepID) {
    var prefix = 'ctl00_contentmain_';
    var postfix = itemID + '_' + stepID;
    var iconAdd = document.getElementById(prefix + 'adddestination' + postfix);
    var iconGreyAdd = document.getElementById(prefix + 'greyCross' + postfix);
    var iconLoaderAdd = document.getElementById(prefix + 'ajaxLoader' + postfix);

    //    if (iconAdd != null)
    //    {
    //        iconAdd.style.display = '';
    //    }
    //    if (iconGreyAdd != null)
    //    {
    //        iconGreyAdd.style.display = 'none';
    //    }
    ShowAddCross(itemID, stepID);
    HideAddPopups(null);
    if (iconLoaderAdd != null) {
        iconLoaderAdd.style.display = 'none';
    }

    return;
}

function CheckValidMiles(itemID, stepID) {
    var prefix = 'ctl00_contentmain_';
    var postfix = itemID + '_' + stepID;

    var miles = document.getElementById(prefix + 'txtmileage' + postfix);
    if (miles !== null) {
        ShowAddCross(itemID, stepID);
    }

    return;
}

function ShowAddCross(itemID, stepID) {
    var prefix = 'ctl00_contentmain_';
    var postfix = itemID + '_' + stepID;
    var iconAdd = document.getElementById(prefix + 'adddestination' + postfix);
    var iconGreyAdd = document.getElementById(prefix + 'greyCross' + postfix);

    if (iconAdd != null && iconGreyAdd != null) {
        if (ValidateAddNewJourneyStep(itemID, stepID)) {
            iconAdd.style.display = '';
            iconGreyAdd.style.display = 'none';
        }
        else {
            iconAdd.style.display = 'none';
            iconGreyAdd.style.display = '';
        }
    }

    return;
}

function CheckValidLocation(control, itemID, stepID) {
    if (control !== undefined && control !== null) {
        if (control.InitialValue !== undefined && control.InitialValue !== null && control.InitialValue !== control.value) {
            InvalidateAddCross(itemID, stepID);
        }
    }

    return;
}

function InvalidateAddCross(itemID, stepID) {
    var prefix = 'ctl00_contentmain_';
    var postfix = itemID + '_' + stepID;
    var iconAdd = document.getElementById(prefix + 'adddestination' + postfix);
    var iconGreyAdd = document.getElementById(prefix + 'greyCross' + postfix);

    if (iconAdd != null && iconGreyAdd != null) {
        iconAdd.style.display = 'none';
        iconGreyAdd.style.display = '';
    }

    return;
}

function ValidateAddNewJourneyStep(itemID, stepID) {
    var prefix = 'ctl00_contentmain_';
    var postfix = itemID + '_' + stepID;

    var JourneyStepFields = {
        FromHidden: document.getElementById(prefix + 'txtfromid' + postfix),
        From: document.getElementById(prefix + 'txtfrom' + postfix),
        ToHidden: document.getElementById(prefix + 'txttoid' + postfix),
        To: document.getElementById(prefix + 'txtto' + postfix),
        Miles: document.getElementById(prefix + 'txtmileage' + postfix),
        CalculatedMiles: document.getElementById(prefix + 'txtCalcMiles' + postfix)
    };

    var prop;

    // check that all the elements have been found
    for (prop in JourneyStepFields) {
        if (JourneyStepFields.hasOwnProperty(prop)) {
            if (JourneyStepFields[prop] === null) {
                return false;
            }
        }
    }

    if (JourneyStepFields.From.value === '') { return false; }
    if (JourneyStepFields.To.value === '') { return false; }
    if (parseInt(JourneyStepFields.FromHidden.value, 10) < 1) { return false; }
    if (parseInt(JourneyStepFields.ToHidden.value, 10) < 1) { return false; }
    if (JourneyStepFields.Miles.value === '') { return false; }
    if (JourneyStepFields.CalculatedMiles.value === '') { return false; }
    //if (JourneyStepFields.From.InitialValue === undefined) { return false; }
    //if (JourneyStepFields.To.InitialValue === undefined) { return false; }

    return true;
}

function showAddCarPanel(carlnkID) {
    carLinkID = carlnkID;
    clearCarModal();
    ResetCarID();

    var modal = $find(modalAddCar);
    modal.show();
}

function hideAddCarPanel() {
    var modal = $find(modalAddCar);
    modal.hide();
}

function addCars() {
    if (Page_ClientValidate('ValidationSummaryAeCar') == true) {
        var EngineSize = document.getElementById(contentID + "addCar_txtEngineSize");
        var Make = document.getElementById(contentID + "addCar_txtmake");
        var Model = document.getElementById(contentID + "addCar_txtmodel");
        var RegNo = document.getElementById(contentID + "addCar_txtregno");
        var CarTypeID = document.getElementById(contentID + "addCar_cmbcartype");
        var cmbMileageID = document.getElementById(contentID + "addCar_cmbMileage");
        var uom = document.getElementById(contentID + "addCar_cmbUom");
        var MileageID;
        if (cmbMileageID == null) {
            MileageID = 0;
        } else {
            MileageID = cmbMileageID.value;
        }

        if (EngineSize != null && Make != null && Model != null && RegNo != null && CarTypeID != null) {
            PageMethods.AddCar(Model.value, Make.value, RegNo.value, EngineSize.value, CarTypeID.value, MileageID, uom.value, addCarComplete);
        }
    }
}

function addCarComplete(data) {
    hideAddCarPanel();

    if (data == true) {
        document.getElementById(contentID + carLinkID).innerHTML = "Car Added.";
        document.getElementById(contentID + carLinkID).href = "javascript:void(0);";
        __doPostBack(pnlSpecClientID, '');
    }
    else {
        document.getElementById(contentID + carLinkID).innerHTML = "Your car has been added and is waiting for approval by an administrator.";
        document.getElementById(contentID + carLinkID).href = "javascript:void(0);";
    }
}

//moved from aeexpense.aspx

function disabledPaymentMethod(advanceCtrl, paymentMethodCtrl) {
    var selectedAdvance = document.getElementById(advanceCtrl).selectedIndex;
    var paymentMethod = document.getElementById(paymentMethodCtrl);
    if (paymentMethod !== null) {
        if (selectedAdvance > 0) {
            paymentMethod.disabled = true;
        }
        else {
            paymentMethod.disabled = false;
        }
    }

}


function getExchangeRate(accountid) {


    var employeeid = 0;
    var txtdate = document.getElementById(contentID + 'txtdate').value;
    var currencyid = document.getElementById(contentID + 'cmbcurrency').options[document.getElementById(contentID + 'cmbcurrency').selectedIndex].value;


    var date = txtdate.substring(6, 10) + '/' + txtdate.substring(3, 5) + '/' + txtdate.substring(0, 2);
    SEL.Data.Ajax({
        url: "/webServices/svcAddEditExpenses.asmx/getExchangeRate",
        data: { accountid: accountid, employeeid: employeeid, currencyid: currencyid, date: date },
        success: function (response) {
            var data = response.d;
            getExchangeRateComplete(data);
        }
    });
    SEL.Data.Ajax({
        url: "/webServices/svcAddEditExpenses.asmx/getAdvances",
        data: { accountid: accountid, employeeid: employeeid, currencyid: currencyid },
        success: function (response) {
            var data = response.d;
            getAdvancesComplete(data);
        }
    });
}

function getExchangeRateComplete(data) {
    var exchangeRate;
    exchangeRate = document.getElementById(contentID + 'cellexchlbl');
    if (exchangeRate != null) {
        if (data[0] == true) //display
        {
            document.getElementById(contentID + 'cellexchlbl').style.display = '';
            document.getElementById(contentID + 'cellexchinput').style.display = '';
            document.getElementById(contentID + 'txtexchangerate').value = data[1];
            document.getElementById('imgtooltip243').style.display = '';            
        } else {
            document.getElementById(contentID + 'cellexchlbl').style.display = 'none';
            document.getElementById(contentID + 'cellexchinput').style.display = 'none';
            document.getElementById(contentID + 'txtexchangerate').value = '1';
            document.getElementById('imgtooltip243').style.display = 'none';
        }
    }
}

function getAdvancesComplete(data) {

    var numitems = document.getElementById(contentID + 'txtnumitems').value;

    var ddlst;

    for (var i = 0; i < numitems; i++) {
        if (document.getElementById(contentID + 'txtadvance' + i) != null) {
            document.getElementById(contentID + 'txtadvance' + i).value = '';
        }
        if (document.getElementById(contentID + 'cmbadvance' + i) != null) {
            var cmbpaymentmethod = document.getElementById(contentID + 'cmbpaymentmethod' + i);
            ddlst = document.getElementById(contentID + 'cmbadvance' + i);

            if (cmbpaymentmethod != null) {
                var itemType = cmbpaymentmethod.options[cmbpaymentmethod.selectedIndex].value;

                switch (itemType) {
                    case '1':
                        ddlst.options.length = 0;
                        for (var x = 0; x < data.length; x++) {
                            ddlst.options[x] = new Option(data[x][1], data[x][0]);
                        }
                        break;
                    case '2':
                        ddlst.options.length = 0;
                        break;
                    case '3':
                        ddlst.options.length = 0;
                        break;
                }
            }
            else {
                ddlst.options.length = 0;
                for (var x = 0; x < data.length; x++) {
                    ddlst.options[x] = new Option(data[x][1], data[x][0]);
                }
            }


        }

    }

}

function getAdvancesOnItemType() {
    var employeeid = 0;
    var currencyid = document.getElementById(contentID + 'cmbcurrency').options[document.getElementById(contentID + 'cmbcurrency').selectedIndex].value;
    SEL.Data.Ajax({
        url: "/webServices/svcAddEditExpenses.asmx/getAdvances",
        data: { accountid: accountid, employeeid: employeeid, currencyid: currencyid },
        success: function (response) {
            var data = response.d;
            getAdvancesComplete(data);
        }
    });
}

function cmbcountry_onchange() {
    var numitems = document.getElementById(contentID + 'txtnumitems').value;
    var cmbcountry = document.getElementById(contentID + 'cmbcountry');
    var countryid = cmbcountry.options[cmbcountry.selectedIndex].value;
    var countryname = $("#ctl00_contentmain_cmbcountry").find("option:selected").text();

    //Calls a service to get the alpha3 code of the selected country, using the response, if successful, to update the address picker widget.
    SEL.Data.Ajax({        
        url: "webServices/svcAddEditExpenses.asmx/GetPCAValidAlpha3CodeByCountryId",
        data: { accountid: accountid, countryid: countryid, countryname: countryname },
        success: function (response) {
            var data = response.d;
            if (data) { //If data is not null the alpha code is valid. If the code is not valid it will default to the company base country.
                $.address.options.DefaultCountryAlpha3Code = data;
                $(".ui-sel-tab-countries").attr("style", "background-image: url(\"/static/icons/32/plain/flag_" + countryname.toLowerCase().replace(/ /g, "_") + ".png\") !important;");
            }                        
        }
    });

    //stop mileage
    for (var i = 0; i < numitems; i++) {
        if (document.getElementById(contentID + 'txtmileage' + i) != null) {
            var txtmileage = document.getElementById(contentID + 'txtmileage' + i);
            if (countryid != homeCountry) {

                txtmileage.disabled = true;
                txtmileage.title = "Mileage can only be claimed in your home country.";
            }
            else {
                txtmileage.disabled = false;
                txtmileage.title = "";
            }
        }
    }

}

function getHotelDetails(id) {
    var hotel = document.getElementById(contentID + 'txthotel' + id).value;
    SEL.Data.Ajax({
        url: "/webServices/svcAddEditExpenses.asmx/getHotelDetails",
        data: { id: id, name: hotel },
        success: function (response) {
            var data = response.d;
            getHotelDetailsComplete(data);
        }
    });
}

function getHotelDetailsComplete(data) {
    document.getElementById(contentID + 'txthotelid' + data[0]).value = data[1];
    var disabled = true;
    if (data[1] > 0) {
        document.getElementById(contentID + 'txtaddress1' + data[0]).value = data[2];
        document.getElementById(contentID + 'txtaddress2' + data[0]).value = data[3];
        document.getElementById(contentID + 'txtcity' + data[0]).value = data[4];
        document.getElementById(contentID + 'txtcounty' + data[0]).value = data[5];
        document.getElementById(contentID + 'txtpostcode' + data[0]).value = data[6];
        document.getElementById(contentID + 'txtcountry' + data[0]).value = data[7];
    }
    else {
        disabled = false;
        document.getElementById(contentID + 'txtaddress1' + data[0]).value = '';
        document.getElementById(contentID + 'txtaddress2' + data[0]).value = '';
        document.getElementById(contentID + 'txtcity' + data[0]).value = '';
        document.getElementById(contentID + 'txtcounty' + data[0]).value = '';
        document.getElementById(contentID + 'txtpostcode' + data[0]).value = '';
        document.getElementById(contentID + 'txtcountry' + data[0]).value = '';
    }
    document.getElementById(contentID + 'txtaddress1' + data[0]).disabled = disabled;
    document.getElementById(contentID + 'txtaddress2' + data[0]).disabled = disabled;
    document.getElementById(contentID + 'txtcity' + data[0]).disabled = disabled;
    document.getElementById(contentID + 'txtcounty' + data[0]).disabled = disabled;
    document.getElementById(contentID + 'txtpostcode' + data[0]).disabled = disabled;
    document.getElementById(contentID + 'txtcountry' + data[0]).disabled = disabled;
}

function selectMileageCategory(id) {
    var ddlst = document.getElementById(contentID + 'cmbmileagecat' + id);
    if (ddlst != null) {
        var txtbox = document.getElementById(contentID + 'txtmileagecat' + id);
        txtbox.value = ddlst.options[ddlst.selectedIndex].value;
    }
}

function getMileageComment(id) {
    
    var txtdate = document.getElementById(contentID + 'txtdate').value;
    var cmbcars = document.getElementById(contentID + 'cmbcars' + id);
    var cmbmileagecat = document.getElementById(contentID + 'cmbmileagecat' + id);
    var cmbAssignment = document.getElementById('cmbESRAss' + id);
    var employeeid = $("input[id$='claimempid" + id + "']").val() || 0;
    var carid = 0;
    var mileageid = 0;
    var esrAssignmentId = 0;
    var date = txtdate.substring(6, 10) + '/' + txtdate.substring(3, 5) + '/' + txtdate.substring(0, 2);
    if (cmbcars != null) {
        carid = cmbcars.options[cmbcars.selectedIndex].value;
    }
       
    if (cmbmileagecat != null) {
        mileageid = cmbmileagecat.options[cmbmileagecat.selectedIndex].value;
    }

    if (cmbAssignment != null && cmbAssignment.selectedIndex !== -1) {
        esrAssignmentId = cmbAssignment.options[cmbAssignment.selectedIndex].value;
    }

    var subcatid = document.getElementById(contentID + 'txtsubcatid' + id).value;
    SEL.Data.Ajax({
        url: "/webServices/svcAddEditExpenses.asmx/getMileageComment",
        data: { id: id, accountid: accountid, employeeid: employeeid, carid: carid, mileageid: mileageid, date: date, subcatid: subcatid, esrAssignmentId: esrAssignmentId },
        success: function (response) {
            var data = response.d;
            getMileageCommentComplete(data);
        }
    });
}

function getDocComment(id,claimSubmitted) {
    var txtdate = document.getElementById(contentID + 'txtdate').value;   
    var cmbcars = document.getElementById(contentID + 'cmbcars' + id);
    var employeeid = $("input[id$='claimempid" + id + "']").val() || 0;
    var carid = 0;
    var date = txtdate.substring(6, 10) + '/' + txtdate.substring(3, 5) + '/' + txtdate.substring(0, 2);
    if (cmbcars != null) {        
        carid = cmbcars.options[cmbcars.selectedIndex].value;
    } 
    var subcatid = document.getElementById(contentID + 'txtsubcatid' + id).value;
    SEL.Data.Ajax({
        url: "/webServices/svcDutyOfCare.asmx/GetDutyOfCareComments",
        data: {id:id, accountid:accountid, employeeid:employeeid, carid:carid, subcatid: subcatid, date: date, claimSubmitted: claimSubmitted, isDelegate: CurrentUserInfo.IsDelegate },
        success: function (response) {
            var data = response.d;
            getDocComplete(data);
        }
    });
}

function getMileageCommentComplete(data) {
    $("*[id$='mileagecomment" + data[0] + "']").html(data[1]);
    $("*[id$='hometoofficecomment" + data[0] + "']").html(data[2]);

}

function getDocComplete(data) {
    var tableId = document.getElementById(contentID + 'tbl' + data[0]);
    var panelId = document.getElementById(contentID + 'pnl' + data[0]);
    $("*[id$='" + contentID + 'pnl' + data[0] + "']").find($('.error-comment')).first().remove();
    if (data[1] != "") {
        if ($("*[id$='" + contentID + 'pnl' + data[0] + "']").find($('.carandjourneyrate')) != null) {
            $("*[id$='" + contentID + 'pnl' + data[0] + "']")
                .find($('.carandjourneyrate'))
                .append("<div class='error-comment'><span><label for='name'>" + data[1] + "</span></div>");
            $("*[id$='" + contentID + 'pnl' + data[0] + "']").find($('.error-comment')).attr("style", "visibility: visible");
            if (tableId != null) {
                tableId.style.display = 'none';
            }
            if ($("*[id$='" + contentID + 'pnl' + data[0] + "']").find($('.helplinkouter')).first() != null) {
                $("*[id$='" + contentID + 'pnl' + data[0] + "']").find($('.helplinkouter')).attr("style", "visibility: visible");
                $("*[id$='" + contentID + 'pnl' + data[0] + "']").find($('.showhelplink')).attr("style", "visibility: visible");
            }        
            
        }
    } else {
        $("*[id$='" + contentID + 'pnl' + data[0] + "']").find($('.error-comment')).attr("style", "visibility: hidden");
        if (tableId != null) {
            tableId.style.display = 'block';
        }
        if ($("*[id$='" + contentID + 'pnl' + data[0] + "']").find($('.helplinkouter')).first() != null) {
            $("*[id$='" + contentID + 'pnl' + data[0] + "']").find($('.helplinkouter')).attr("style", "visibility: hidden");
            $("*[id$='" + contentID + 'pnl' + data[0] + "']").find($('.showhelplink')).attr("style", "visibility: hidden");
        }
    }
}

function checkUsualMileage(sender, args) {

    if (usualMileage == 0) {
        args.IsValid = true;
        return;
    }
    var index = sender.id.substring(contentID.length + 11, contentID.length + sender.id.length - 11);

    var txtmileage = document.getElementById(contentID + 'txtmileage' + index);

    if (txtmileage != null) {
        var mileage = new Number();
        mileage = txtmileage.value;
        if (mileage > usualMileage) {
            args.IsValid = false;
        }
    }


}

function validateJourneyGrid(sender, args) {
    var suffix = sender.id.substring(contentID.length + 14, contentID.length + sender.id.length - 14);
    var ids = suffix.split('_');
    var index = ids[0];
    var subcatid = ids[1];
    var txtfrom = document.getElementById(contentID + 'txtfrom' + suffix);
    var txtfromid = document.getElementById(contentID + 'txtfromid' + suffix);
    var txtto = document.getElementById(contentID + 'txtto' + suffix);
    var txttoid = document.getElementById(contentID + 'txttoid' + suffix);
    var txtmiles = document.getElementById(contentID + 'txtmileage' + suffix);

    // if any of the grid has been populated, then need to validate, otherwise, don't bother
    if (txtfromid.value !== '' || txttoid.value !== '' || txtfrom.value !== '' || txtto.value !== '' || (txtmiles.value !== '' && txtmiles.value !== '0')) {
        if (txtfromid.value === '' || txttoid.value === '' || txtfrom.value === '' || txtto.value === '' || txtmiles.value === '') {
            args.IsValid = false;
        }
    }

    return;
}

function checkAttendees(sender, args) {

    var index = sender.id.substring(contentID.length + 13, contentID.length + sender.id.length - 13);

    var txttotal = document.getElementById(contentID + 'txttotal' + index);

    if (txttotal != null) {
        if (txttotal.value == '') {
            args.IsValid = true;
            return;
        }

        var total = new Number();
        total = txttotal.value;

        if (total > 0) {


            if (args.Value == '') {

                args.IsValid = false;

            }
        }
    }
}

function checkReason(sender, args) {

    var index = sender.id.substring(contentID.length + 13, contentID.length + sender.id.length - 13);

    var txttotal = document.getElementById(contentID + 'txttotal' + index);

    if (txttotal != null) {
        if (txttotal.value == '') {
            args.IsValid = true;
            return;
        }

        var total = new Number();
        total = txttotal.value;

        if (total > 0) {


            if (args.Value == '0') {

                args.IsValid = false;

            }
        }
    }
}

function checkCompany(sender, args) {

    var index = sender.id.substring(contentID.length + 11, contentID.length + sender.id.length - 11);

    var txttotal = document.getElementById(contentID + 'txttotal' + index);

    if (txttotal == null) {
        txttotal = document.getElementById(contentID + 'txtmileage' + index);
    }
    if (txttotal == null) {
        txttotal = document.getElementById(contentID + 'txtmileage' + index + '_0');
    }

    if (txttotal != null) {
        if (txttotal.value == '') {
            args.IsValid = true;
            return;
        }

        var total = new Number();
        total = txttotal.value;

        if (total > 0) {


            if (args.Value == '') {

                args.IsValid = false;

            }
        }
    }
}

function checkFrom(sender, args) {

    var index = sender.id.substring(contentID.length + 8, contentID.length + sender.id.length - 8);

    var txttotal = document.getElementById(contentID + 'txttotal' + index);

    if (txttotal == null) {
        txttotal = document.getElementById(contentID + 'txtmileage' + index);
    }
    if (txttotal == null) {
        txttotal = document.getElementById(contentID + 'txtmileage' + index + '_0');
    }

    if (txttotal != null) {
        if (txttotal.value == '') {
            args.IsValid = true;
            return;
        }

        var total = new Number();
        total = txttotal.value;

        if (total > 0) {


            if (args.Value == '') {

                args.IsValid = false;

            }
        }
    }
}

function checkTo(sender, args) {

    var index = sender.id.substring(contentID.length + 6, contentID.length + sender.id.length - 6);

    var txttotal = document.getElementById(contentID + 'txttotal' + index);

    if (txttotal == null) {
        txttotal = document.getElementById(contentID + 'txtmileage' + index);
    }
    if (txttotal == null) {
        txttotal = document.getElementById(contentID + 'txtmileage' + index + '_0');
    }

    if (txttotal != null) {
        if (txttotal.value == '') {
            args.IsValid = true;
            return;
        }

        var total = new Number();
        total = txttotal.value;

        if (total > 0) {


            if (args.Value == '') {

                args.IsValid = false;

            }
        }
    }
}
function checkEmployees(sender, args) {

    var index = sender.id.substring(contentID.length + 9, contentID.length + sender.id.length - 9);
    var stafftotal = 0;
    var numdirectors = 0;
    var numemployees = 0;
    var txttotal = document.getElementById(contentID + 'txttotal' + index);
    var txtdirectors = document.getElementById(contentID + 'txtdirectors' + index);
    var txtemployees = document.getElementById(contentID + 'txtstaff' + index);
    if (txttotal != null) {
        if (txttotal.value == '') {
            args.IsValid = true;
            return;
        }

        var total = new Number();
        total = txttotal.value;

        if (total > 0) {

            if (txtdirectors != null) {
                if (numdirectors.value != "") {
                    numdirectors = txtdirectors.value;
                }
            }
            if (txtemployees != null) {
                if (txtemployees.value != "") {
                    numemployees = txtemployees.value;
                }
            }
            stafftotal = numdirectors + numemployees;
            if (stafftotal == 0) {

                args.IsValid = false;

            }
        }
    }
}
function calculateSplit(displayId, totalId, splitItems) {
    var total = new Number();
    var splitTotal = new Number();
    total = new Number(document.getElementById(contentID + totalId).value);



    for (var i = 0; i < splitItems.length; i++) {
        if (document.getElementById(contentID + splitItems[i]).value != '') {
            splitTotal = new Number(document.getElementById(contentID + splitItems[i]).value);

            total = total - splitTotal;
        }
    }

    total = Math.round(total * 100) / 100;
    document.getElementById(contentID + displayId).value = total;
}

function checkTip(sender, args) {

    var index = sender.id.substring(contentID.length + 7, contentID.length + sender.id.length - 7);

    var txttotal = document.getElementById(contentID + 'txttotal' + index);
    var tiplimit = document.getElementById(contentID + 'txttiplimit' + index).value;

    if (txttotal != null) {

        if (txttotal.value == '') {
            args.IsValid = true;
            return;
        }

        var total = new Number();
        total = txttotal.value;

        if (total > 0) {


            if (args.Value == '') {

                args.IsValid = false;

            }

            //check limit
            var tip = new Number();
            var tipallowed = new Number();
            tip = args.Value;
            tipallowed = (total / 100) * tiplimit;
            if (tip > tipallowed) {
                args.IsValid = false;
            }
        }
    }
}

function checkMandatory(sender, args) {

    var index = sender.id.substring(contentID.length + 44, (contentID.length + sender.id.length));

    var txttotal = document.getElementById(contentID + 'txttotal' + index);



    if (txttotal == null) {
        txttotal = document.getElementById(contentID + 'txtmileage' + index);

        if (txttotal == null) {
            //Check the number of allowances
            txttotal = document.getElementById(contentID + 'txtallowances' + index);
        }
    }

    if (txttotal != null) {

        if (txttotal.value == '') {
            args.IsValid = true;
            return;
        }

        var total = new Number();
        total = txttotal.value;

        if (total > 0) {


            if (args.Value == '') {
                args.IsValid = false;
                return;
            }

            var ctl = document.getElementById(sender.controltovalidate);
            if (sender != null && ctl.type == "select-one" && args.Value == 0) {
                args.IsValid = false;
                return;
            }
        }
    }

    //If item is a fixed allowance then need to check if the times/dates are the same or not.
    //If they are different then check the mandatory user defined field
    var txtstarttime = document.getElementById(contentID + 'txtstarttime' + index);
    var txtendtime = document.getElementById(contentID + 'txtendtime' + index);
    var txtenddate = document.getElementById(contentID + 'txtenddate' + index);
    var txtstartdate = document.getElementById(contentID + 'txtstartdate' + index);
    var txtDeductAmount = document.getElementById(contentID + 'txtdeductamount' + index);

    if ((txtstarttime != null && txtendtime != null) || (txtstartdate != null && txtenddate != null))
        //if (txtstarttime != null && txtendtime != null)  
    {
        if (txtstarttime.value == txtendtime.value && txtstartdate.value == txtenddate.value && txtDeductAmount.value === "") {
            args.IsValid = true;
            return;
        }
        else {
            if (args.Value == '') {
                args.IsValid = false;
                return;
            }

            var ctl = document.getElementById(sender.controltovalidate);
            if (sender != null && ctl.type == "select-one" && args.Value == 0) {
                args.IsValid = false;
                return;
            }
        }
    }

    if (document.getElementById(contentID + 'tbljourney' + index) != null) {
        var doValidate = false;
        var inputs = document.getElementById(contentID + 'tbljourney' + index).getElementsByTagName('INPUT');
        var hasFrom = false;
        var hasTo = false;
        for (var x = 0; x < inputs.length; x++) {
            if (inputs[x].id.substring(0, 27) == contentID + 'txtfromid') {
                var from = document.getElementById(inputs[x].id);
                if (from.value != '') {
                    hasFrom = true;
                }
            }
            if (inputs[x].id.substring(0, 25) == contentID + 'txttoid') {
                var to = document.getElementById(inputs[x].id);
                if (to.value != '') {
                    hasTo = true;
                }
            }
            if (inputs[x].id.substring(0, 28) == contentID + 'txtmileage') {
                var txtmileage = document.getElementById(inputs[x].id);
                if (txtmileage != null) {
                    if (txtmileage.value != '' && (hasFrom || hasTo)) {
                        doValidate = true;
                        break;
                    }
                }
            }
        }

        if (doValidate) {
            var ctl = document.getElementById(sender.controltovalidate);
            // list type
            if (sender != null && ctl.type == "select-one" && args.Value == 0) {
                args.IsValid = false;
                return;
            }
            // text box
            if (sender != null && ctl.type == "text" && args.Value == '') {
                args.IsValid = false;
                return;
            }
        }

    }
}

function clearGeneral() {
    var txt;

    txt = document.getElementById(contentID + 'txtdate');
    txt.value = getYesterdaysDate();


    txt = document.getElementById(contentID + 'txtfrom');
    if (txt != null) {
        txt.value = '';
    }

    txt = document.getElementById(contentID + 'cmbfrom');
    if (txt != null) {
        txt.selectedIndex = 0;
    }

    txt = document.getElementById(contentID + 'cmbto');
    if (txt != null) {
        txt.selectedIndex = 0;
    }
    txt = document.getElementById(contentID + 'txtto');
    if (txt != null) {
        txt.value = '';
    }

    txt = document.getElementById(contentID + 'cmbreason');
    if (txt != null) {
        txt.selectedIndex = 0;
    }

    txt = document.getElementById(contentID + 'cmbcountry');
    if (txt != null) {
        if (homeCountry != null) {
            for (i = 0; i < txt.options.length; i++) {
                if (txt.options[i].value == homeCountry) {
                    txt.selectedIndex = i;
                    break;
                }
            }
        }
        else {
            txt.selectedIndex = 0;
        }
    }

    txt = document.getElementById(contentID + 'cmbcurrency');
    if (txt != null) {
        if (homeCurrency != null) {
            for (i = 0; i < txt.options.length; i++) {
                if (txt.options[i].value == homeCurrency) {
                    txt.selectedIndex = i;
                    break;
                }
            }
        }
        else {
            txt.selectedIndex = 0;
        }
    }
    getExchangeRateComplete(false);

    txt = document.getElementById(contentID + 'txtotherdetails');
    if (txt != null) {
        txt.value = '';
    }

    txt = document.getElementById(contentID + 'txtcompany');
    if (txt != null) {
        txt.value = '';
    }
    txt = document.getElementById(contentID + 'txtcompanyid');
    if (txt != null) {
        txt.value = '';
    }
}

function getYesterdaysDate() {
    var currentTime = new Date();
    currentTime.setDate(currentTime.getDate() - 1);
    var month = currentTime.getMonth() + 1;
    var day = currentTime.getDate();
    var year = currentTime.getFullYear();
    return padInteger(day) + "/" + padInteger(month) + "/" + year;
}

function padInteger(num) {
    var s = "0" + num;
    return s.substr(s.length - 2);
}


function popChildDropDowns(parCtlName, type, filterid, parCtlIndex, accountid) {
    parddlst = document.getElementById(contentID + parCtlName)
    if (parddlst == null) {
        return;
    }

    var splitType = type.split(';');
    var splitFilterid = filterid.split(';');
    var parid;
    if (parddlst.name.match('txt')) {
        parid = document.getElementById(contentID + parCtlName).value;
    }
    else {
        parid = parddlst.options[parddlst.selectedIndex].value;
    }

    var splCtlLength = splitType.length - 1;

    for (i = 0; i < splCtlLength; i++) {
        SEL.Data.Ajax({
            url: "/webServices/svcAddEditExpenses.asmx/popChildControls",
            data: { sType:splitType[i], sFilterid:splitFilterid[i], id:parid, parCtlIndex: parCtlIndex, accountid:accountid },
            success: function (response) {
                var data = response.d;
                getChildDropDownsComplete(data);
            }
        });
    }
}

function getChildDropDownsComplete(data) {
    var chddlst;
    var items = data[1];

    switch (data[2]) {
        case 'general':
            {
                popDropdown(data[0], items)
                break;
            }
        case 'individual':
            {
                if (data[3] != "") {
                    popDropdown(data[0] + data[3], items);
                }
                else {
                    var numitems = document.getElementById(contentID + 'txtnumitems').value;

                    for (var i = 0; i < numitems; i++) {
                        popDropdown(data[0] + i, items);
                    }
                }
                break;
            }
        case 'breakdown':
            {
                if (data[3] != "") {
                    popDropdown(data[0] + data[3], items);
                }
                else {
                    var numbreakdownrows = document.getElementById(contentID + 'txtrows').value;

                    for (var i = 0; i <= numbreakdownrows; i++) {
                        popDropdown(data[0] + i, items);
                    }
                }
                break;
            }
    }
}

function popDropdown(ctlid, items) {
    var $ctl = $("#" + contentID + ctlid);

    if ($ctl.length == 0)
    {
        return;
    }

    $ctl.empty();

    if (items.length > 1)
    {
        $ctl.append($("<option/>")
            .val("0")
            .text("[None]"));
    }

    for (var i = 0; i < items.length; i++) {
        $ctl.append($("<option/>")
            .val(items[i][1])
            .text(items[i][0]));
    }
}

function getLocationChildrenComplete(data) {

    popChildDropDowns(data[0], data[1], data[2], data[3], data[4]);
}

function SetHiddenDateFieldValue(textbox) {
    var hdnDate = document.getElementById(hdnExpDate);
    hdnDate.value = textbox.value;
    return;
}

function RefreshCarDropDown() {
    var numitems = document.getElementById(contentID + 'txtnumitems').value;
    for (var i = 0; i < numitems; i++) {
        if (document.getElementById(contentID + 'cmbcars' + i) != null) {
            var txtExpenseItemDate = document.getElementById(contentID + 'txtdate').value;
            txtExpenseItemDate = txtExpenseItemDate.substring(6, 10) + '/' + txtExpenseItemDate.substring(3, 5) + '/' + txtExpenseItemDate.substring(0, 2);
            Spend_Management.svcCars.CreateCurrentValidCarDropDown(accountid, employeeid, txtExpenseItemDate, CreateCurrentValidCarDropDownComplete);
        }
    }
}

function refreshMileagePanel(claimSubmitted) {
    if (document.getElementById(contentID + 'txtnumitems') !== null) {
        var numitems = document.getElementById(contentID + 'txtnumitems').value;
        for (var i = 0; i < numitems; i++) {
            if (document.getElementById(contentID + 'cmbcars' + i) != null) {
                getDocComment(i, claimSubmitted);
            }
        }
    }
}
function CreateCurrentValidCarDropDownComplete(items) {
    var numitems = document.getElementById(contentID + 'txtnumitems').value;
    for (var i = 0; i < numitems; i++) {
        if (document.getElementById(contentID + 'cmbcars' + i) != null) {
            chddlst = document.getElementById(contentID + 'cmbcars' + i);

            if (chddlst != null) {
                chddlst.options.length = 0;

                for (var j = 0; j < items.length; j++) {
                    chddlst.options[j] = new Option(items[j].Text, items[j].Value);
                }
            }
        }
    }
}

function RefreshAssignmentDropDown(date) {
    SEL.Data.Ajax({
        url: "/webServices/svcAddEditExpenses.asmx/GetAssignmentListItems",
        data: { claimDate: date.value },
        success: function (response) {
            var data = response.d;
            GetAssignmentListComplete(data);
        }
    });
}

function GetAssignmentListComplete(data)
{
    $("select[id*='cmbESRAss']").each(function() {
        var $cmbESRAss = $(this).empty();
            if (data.length > 1) {
            $cmbESRAss.closest("tr").show();
            } else {
            $cmbESRAss.closest("tr").hide();
            }

            for (var x = 0; x < data.length; x++) {
            $cmbESRAss.append(
                $("<option/>")
                    .val(data[x].Value)
                    .prop("selected", data[x].Selected)
                    .text(data[x].Text)
            );
    }
    });
    SEL.AddressesAndTravel.UpdateHomeAndOfficeAddresses();
}

function ShowFlagModal(flags)
{
    SEL.Claims.ClaimViewer.GenerateFlagOutPut(flags);
    $("#flagSummary").css('max-height', '600px');
    $("#flagSummary").dialog("option", "width", 850);
    $("#flagSummary").dialog("option", "buttons", [{ text: 'close', id: 'btnCancelFlag', click: function () { $(this).dialog('close'); } }]);
    $("#flagSummary").dialog("open");
}
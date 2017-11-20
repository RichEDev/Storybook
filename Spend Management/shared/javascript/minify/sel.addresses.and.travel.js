/*global SEL:false, google:false, $g:false, Spend_Management: false */
(function (SEL, $g) {
    var scriptName = "AddressesAndTravel";

    function execute() {
        SEL.registerNamespace("SEL.AddressesAndTravel");
        SEL.AddressesAndTravel = {
            MessageGenericError: "Unable to generate recommended route.",

            RouteDomID: "route",
            MapDomID: "map",
            MapModalDomID: null,
            MapInfoDomID: null,
            MapModalInfoDomID: null,

            ShowLoading: function () {
                $g(SEL.AddressesAndTravel.RouteDomID).innerHTML = "";
                $g(SEL.AddressesAndTravel.MapDomID).innerHTML = "";
                SEL.AddressesAndTravel.ShowInfoModal();
            },
            UnableToGenerate: function (error) {
                SEL.MasterPopup.ShowMasterPopup(error || SEL.AddressesAndTravel.MessageGenericError);
            },
            GetRouteByClaimAndExpense: function (expenseItemID) {
                SEL.AddressesAndTravel.ShowLoading();
                Spend_Management.shared.webServices.svcAddresses.GetRouteByExpenseId(expenseItemID, SEL.AddressesAndTravel.GetRouteComplete, SEL.AddressesAndTravel.GetRouteFailed);
            },
            GetRouteByAddressIds: function (addressIDs) {
                SEL.AddressesAndTravel.ShowLoading();
                Spend_Management.shared.webServices.svcAddresses.GetRouteByAddressIds(addressIDs, SEL.AddressesAndTravel.GetRouteComplete, SEL.AddressesAndTravel.GetRouteFailed);
            },
            GetRouteByAddressIdsForEmployee: function (addressIDs, claimEmpId) {
                SEL.AddressesAndTravel.ShowLoading();
                Spend_Management.shared.webServices.svcAddresses.GetRouteByAddressIdsForEmployee(addressIDs, claimEmpId, SEL.AddressesAndTravel.GetRouteComplete, SEL.AddressesAndTravel.GetRouteFailed);
            },
            GetRouteComplete: function (response) {
                if (response === null || response.Error !== "") {
                    return SEL.AddressesAndTravel.GetRouteFailed(response.Error);
                }

                // route
                var distanceOrTime, route = "<div class=\"standardHeaderColours\" style=\"line-height: 30px; font-size: 16px; text-align: center\">" + response.TotalTimeFormatted + " / " + response.DistanceFormatted + " (" + response.RoutingType + ")</div>", stepImage;

                route += "<table style=\"margin: 4px;\">";
                for (var i = 0; i < response.Steps.length; i++) {

                    if (response.Steps[i].Action === "A") {
                        distanceOrTime = response.Steps[i].TotalTimeFormatted;
                    } else {
                        distanceOrTime = response.Steps[i].StepDistanceFormatted;
                    }

                    if (response.Steps[i].ActionImage.indexOf("unknown.png") === -1) {
                        stepImage = "<img src=\"" + response.Steps[i].ActionImage + "\" height=\"20\" width=\"20\" alt=\"" + response.Steps[i].Action + "\" />";
                    } else {
                        stepImage = "&nbsp;";
                    }

                    route += "<tr><td width=\"25\" valign=\"top\">" + stepImage + "</td><td width=\"210px\" style=\"font-size: 12px;\"><b>" + (response.Steps[i].StepNumber + 1) + ".</b>&nbsp;" + response.Steps[i].Description + "</td><td width=\"50\">&nbsp;</td></tr>";
                    route += "<tr><td>&nbsp;</td><td><div style=\"height:1px;background-color: #CCCCCC;overflow:hidden\"></div></td><td style=\"font-size: 10px; color: #666666\" valign=\"top\">" + distanceOrTime + "</td></tr>";
                }
                route += "</table>";



                $g(SEL.AddressesAndTravel.RouteDomID).innerHTML = route;

                // map
                SEL.AddressesAndTravel.HideInfoModal();
                SEL.Common.ShowModal(SEL.AddressesAndTravel.MapModalDomID);
                SEL.GoogleMaps.Create(SEL.AddressesAndTravel.MapDomID, response.Steps);
            },
            GetRouteFailed: function (error) {
                SEL.AddressesAndTravel.UnableToGenerate(error);
                SEL.AddressesAndTravel.HideInfoModal();
            },
            CloseRoute: function () {
                SEL.Common.HideModal(SEL.AddressesAndTravel.MapModalDomID);
            },
            ShowInfoModal: function () {
                SEL.Common.ShowModal(SEL.AddressesAndTravel.MapModalInfoDomID);
            },
            HideInfoModal: function () {
                SEL.Common.HideModal(SEL.AddressesAndTravel.MapModalInfoDomID);
            },
            SetModalInfoText: function (text) {
                $g(SEL.AddressesAndTravel.MapInfoDomID).innerHTML = text;
            },
            IsOtherUsersClaim: function() {
                return window.hdnClaimOwnerId && $("#" + window.hdnClaimOwnerId).val() && $("#employeeid").val() &&
                    $("#employeeid").val() != $("#" + window.hdnClaimOwnerId).val();
            },
            GetDateField : function(field) {
                // date field is either at the top of the page once (add/edit expense page), or there's one for each expense item row in the quick entry form
                return $("#ctl00_contentmain_txtdate").length ? $("#ctl00_contentmain_txtdate") : field.parents("tr").find("input[id*=ctl00_contentmain_txtdate]:visible");
            },
            GetAddressPickerHiddenField: function(addressPicker) {
                return $("input[id*=" + addressPicker.attr("rel") + "]");
            },
            PopulateAddressPickerByKeyword: function (addressPicker) {

                // in some scenarios the address can be matched and the "home" keyword left unchanged in the textbox
                // this prevents the keyword being matched repeatedly on subsequent blur events
                if ($(addressPicker).address("getBackgroundValue") != "") return;
                
                // attempt to remove the office flag
                addressPicker.removeAttr("data-office");

                // exact match for "home" or "office" or the custom keywords 
                var keyword = addressPicker.val();
                var regex = new RegExp("^home$|^office$|^" + SEL.Expenses.Settings.HomeAddressKeyword.toLowerCase() + "$|^" + SEL.Expenses.Settings.WorkAddressKeyword.toLowerCase() + "$");
                
                if (keyword.toLowerCase().search(regex) === 0) {

                    if ((keyword.toLowerCase() === "home" || keyword.toLowerCase() == SEL.Expenses.Settings.HomeAddressKeyword.toLowerCase()) && SEL.AddressesAndTravel.IsOtherUsersClaim()) {
                        //don't accept home if it's not our claim
                        if ($(addressPicker).address("getBackgroundValue")) {
                            //only clear it out if it's not already been populated (i.e. if they've just tried to type it in)
                            addressPicker.val("");
                        }
                        return;
                    }
                    var esrAssignmentField;
                    // attempt to retrieve the currently selected ESR number (assignment ID)
                    if (addressPicker.parents(".mileagePanel").length === 1)
                    {
                       esrAssignmentField = addressPicker.parents(".mileagePanel").find(".esrAssignment select").first();
                    } else {
                        esrAssignmentField = addressPicker.parents("tbody").find(".esrAssignment select").first();
                    }
                    

                    // there might be an esr field containing no <option> values, so val() could return null
                    var esrAssignmentId = (esrAssignmentField.length) ? (esrAssignmentField.val() || 0) : 0;

                    // if there's an esr assignment id and the work address keyword was used then flag the field
                    if (keyword.toLowerCase() === "office" || keyword.toLowerCase() == SEL.Expenses.Settings.WorkAddressKeyword.toLowerCase()) {
                        addressPicker.attr("data-office", true);
                    }

                    // get the "home" or "office" address
                    SEL.Data.Ajax({
                        serviceName: "svcAddresses",
                        methodName: "GetByKeyword",
                        data: {
                            keyword: keyword,
                            date: SEL.AddressesAndTravel.GetDateField(addressPicker).val(),
                            esrAssignmentId: esrAssignmentId
                        },
                        context: this,
                        success: function(data) {
                            var response = data.d;

                            if (response) {
                                // populate the fields with response values
                                $(addressPicker).address("value", response.FriendlyName, response.Identifier);
                                $(addressPicker).address("reset");

                                // fire the change event on the hidden field to force an update to the mileage calculation
                                SEL.AddressesAndTravel.GetAddressPickerHiddenField(addressPicker).change();

                                if (keyword.toLowerCase() === "office" ||
                                    keyword.toLowerCase() === SEL.Expenses.Settings.WorkAddressKeyword.toLowerCase()) {
                                    $('#' + hdnWorkAddressID).val(response.Identifier);
                                    SEL.AddressesAndTravel.UpdateOfficeAddressesWithNewValue(response.FriendlyName, response.Identifier);
                                }
                                addressPicker.closest("td").next().children("input[type=text]").focus();

                            } else {
                                // clear the fields
                                $(addressPicker).address("value", "", "");
                                $(addressPicker).address("reset");
                                if (esrAssignmentField.length) esrAssignmentField.val("");
                            }
                        }
                    });
                }
            },
            // re-gets the correct home address for any mileage grid address fields which are "home" (or the custom keyword)
            UpdateHomeAndOfficeAddresses: function ($element) {
                var element;
                if ($element === undefined || $element === null) {
                    element = $(document).find("input[name=mileagegridtable_from_address_text], input[name=mileagegridtable_to_address_text], input.ui-sel-address-picker");
                }
                else if ($element.parents(".mileagePanel").length === 1) {
                    element = $element.parents(".mileagePanel").find("input[name=mileagegridtable_from_address_text], input[name=mileagegridtable_to_address_text]");
                } else {
                    element = $element.parents('tbody').find('input.ui-sel-address-picker');
                }
         
                element.each(function (index, element) {
                    if ($(element).val()) {
                        // set the value (using the widgets "value" method) to the custom keyword, with no address id, then blur the field, this will cause the home/office address to be retrieved again
                        if ($(element).val().toLowerCase() === SEL.Expenses.Settings.HomeAddressKeyword.toLowerCase()) {
                            $(element).address("value", SEL.Expenses.Settings.HomeAddressKeyword, "").blur();
                        }
                    }
                    if ($(element).attr("data-office") === "true") {
                        $(element).address("value", SEL.Expenses.Settings.WorkAddressKeyword, "").blur();
                    }
                });
            },
            UpdateOfficeAddressesWithNewValue: function (friendlyName, identifier, $element) {
                var element;
                if ($element === undefined || $element === null) {
                    element = $(document).find("input[name=mileagegridtable_from_address_text], input[name=mileagegridtable_to_address_text], input.ui-sel-address-picker");
                }
                else if ($element.parents(".mileagePanel").length === 1) {
                    element = $element.parents(".mileagePanel").find("input[name=mileagegridtable_from_address_text], input[name=mileagegridtable_to_address_text]");
                } else {
                    element = $element.parents('tbody').find('input.ui-sel-address-picker');
                }

                element.each(function (index, element) {
                    // set the value (using the widgets "value" method) to the custom keyword, with no address id, then blur the field, this will cause the home/office address to be retrieved again
                    if ($(element).attr("data-office") === "true") {
                        $(element).address("value", friendlyName, identifier);
                        // fire the change event on the hidden field to force an update to the mileage calculation
                        SEL.AddressesAndTravel.GetAddressPickerHiddenField($(element)).change();
                    }
                });
            }

        };
    }

    if (window.Sys && window.Sys.loader) {
        window.Sys.loader.registerScript(scriptName, null, execute);
    }
    else {
        execute();
    }
}(SEL, $g));

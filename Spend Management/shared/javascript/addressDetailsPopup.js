function AddressDetailsPopup() {
    this.AddressDetailsSpan = document.getElementById(adpAddressDetailsSpan);
    this.show = function (control) {
        $find(adpPopup)._popupBehavior._parentElement = control;
        $find(adpPopup).showPopup();
        $find(adpPopup)._popupElement.style.zIndex = SEL.Common.GetHighestZIndexInt() + 1;
        return;
    };

    this.hide = function () {
        $find(adpPopup).hidePopup();
        return;
    };

    this.displayAddressWithElements = function(image, addressIDField, evt) {
        addressDetailsPopup.showLoader(true);
        addressDetailsPopup.AddressDetailsSpan.innerHTML = "";
        stopPropagation(evt);
        if (addressIDField !== null) {
            if (addressIDField.value == "" || isNaN(addressIDField.value) === true) {
                addressDetailsPopup.AddressDetailsSpan.innerHTML = "<div style=\"padding: 6px;\">No address selected</div>";
                document.getElementById(adpPanel).style.width = "160px";
                addressDetailsPopup.showLoader(false);
                addressDetailsPopup.show(image);
            }
            else {
                var addressID = addressIDField.value;
                var methodName = "Get";
                var data = {
                    addressIdentifier: +addressID,
                    labelId: 0
                };
                if (window.hdnClaimOwnerId) {
                    methodName = "GetForClaimOwner";
                    data.claimOwnerId = $("#" + hdnClaimOwnerId).val() || 0;
                }
                SEL.Data.Ajax({
                    serviceName: "svcAddresses",
                    methodName: methodName,
                    data: data,
                    complete: function (data) {
                        var addressDetails = JSON.parse(data.responseText).d;

                        if (addressDetails.IsPrivate) {
                            addressDetailsPopup.AddressDetailsSpan.innerHTML = "<div style=\"padding: 6px;\">You do not have permission to view the address</div>";
                        } else {
                            var tbl = document.createElement("table");

                            document.getElementById(adpPanel).style.width = "305px";

                            // todo display label/favourite information?
                            // todo only display rows which contain data, Line2, City, County and (depending on the account) Postcode
                            //addressDetailsPopup.addAddressDetailToTable(tbl, "Name", "<b>" + addressDetails.AddressName + "</b>");

                            addressDetailsPopup.addAddressDetailToTable(tbl, "Address Name", addressDetails.AddressName || "");
                            addressDetailsPopup.addAddressDetailToTable(tbl, "Line 1", addressDetails.Line1 || "");
                            addressDetailsPopup.addAddressDetailToTable(tbl, "Line 2", addressDetails.Line2 || "");
                            addressDetailsPopup.addAddressDetailToTable(tbl, "City", addressDetails.City || "");
                            addressDetailsPopup.addAddressDetailToTable(tbl, "County", addressDetails.County || "");
                            addressDetailsPopup.addAddressDetailToTable(tbl, "Postcode", addressDetails.Postcode || "");

                            /*
                            if (addressDetails.Country === null) {
                                addressDetails.Country = "";
                            }
                            addressDetailsPopup.addAddressDetailToTable(tbl, "Country", addressDetails.Country);
                            */
                            addressDetailsPopup.AddressDetailsSpan.innerHTML = "";
                            addressDetailsPopup.AddressDetailsSpan.appendChild(tbl);
                        }

                        addressDetailsPopup.showLoader(false);
                        addressDetailsPopup.show(image);
                    },
                    error: function () {
                        AddressDetailsPopup.AddressDetailsSpan.innerHTML = "Unable to retrieve address details.";
                        addressDetailsPopup.showLoader(false);
                        addressDetailsPopup.show(image);
                    }
                });
            }
        }
        else {
            alert("Invalid addressID field.");
        }
        return addressDetailsPopup;
    };

    this.displayAddress = function (imageID, hiddenFieldID, evt) {
        var addressIDField = document.getElementById(hiddenFieldID);
        var image = document.getElementById(imageID);
        return this.displayAddressWithElements(image, addressIDField, evt);
    };

    this.showLoader = function (trueOrFalse) {
        if (trueOrFalse === true) {
            document.getElementById(adpLoader).style.display = "";
        }
        else {
            document.getElementById(adpLoader).style.display = "none";
        }

        return;
    };

    this.addHomeAddressRow = function (tbl, text) {
        var row = tbl.insertRow(-1);
        var cell = row.insertCell(-1);
        cell.colSpan = 2;
        cell.innerHTML = text;
        return tbl;
    };

    this.addAddressDetailToTable = function (tbl, label, value) {
        var row = tbl.insertRow(-1);
        var cell = row.insertCell(-1);
        cell.setAttribute("class", "textlabel"); //For Most Browsers
        cell.setAttribute("className", "textlabel"); //For IE; harmless to other browsers.
        cell.width = "100px";
        cell.innerHTML = "<span class=\"textlabel\">" + label + "</span>";
        cell = row.insertCell(-1);
        cell.width = "200px";
        cell.innerHTML = value;
        return tbl;
    };

}

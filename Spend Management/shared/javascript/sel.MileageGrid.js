(function (moduleNameHTML, appPath) {
    var scriptName = "mileagegrid";
    function execute() {
        SEL.registerNamespace("SEL.MileageGrid");
        SEL.MileageGrid =
        {
            //this is looked up by the data-field attribute of the control that has got focus.
            mileageGridHelpTexts: {
                "from_search": "Enter the Postcode or if permitted, any part of the address where the journey step started from then pick the address from the results.",
                "to_search": "Enter the Postcode or if permitted, any part of the address where the journey step finished then pick the address from the results.",
                "userentereddistance": "The recommended distance is shown in the box on the right. If permitted, you can edit the box on the left to record the actual distance for the journey step.",
                "numpassengers_names": "Enter the names of passengers in the vehicle on this journey step.",
                "numpassengers_nonames": "Enter the number of passengers in the vehicle on this journey step.",
                "passengers" : "Click here to enter the passengers that were in the vehicle on this journey.",
                "heavybulkyequipment": "Tick this box if you were carrying heavy or bulky equipment on this journey step.",
                "unabletocalculateduetomissingpostcode": "The distance could not be calculated. Please enter this manually."
            },

            kilometresPerMile: 1.609344,

            setup: function () {
                //setup!=true is so each setup step only gets done once.
                //we use .mileagePanel not .mileagegridtable as single journey step doesn't have a mileagegridtable,
                //but it is still in a .mileagePanel.
                $(".mileagePanel[setup!=true]").each(function () {
                    var mileagePanel = $(this);
                    
                    $(".mileagegridtable", mileagePanel).each(function () {
                        //this sets the 'A', 'B', etc user-friendly row numbering
                        SEL.MileageGrid.setRowIdentifiers(this);
                    });

                    $(".mileagegridtable tbody tr:not(.template)", mileagePanel).each(function () {
                        SEL.MileageGrid.setupRow(this);
                        //need to loop rather than do them all at once as each one will have a
                        //different existing value to prepopulate
                    });
                    $(".mileagegridtable", mileagePanel)
                        .on("focus", "tr.template", function () {
                            SEL.MileageGrid.duplicateTemplateRow($(this).closest(".mileagegridtable"));
                        })
                        .on("click", "tr:not(.template) .passengersclick", SEL.MileageGrid.showPassengersDialog)
                        .on("keypress", ".passengersclick", function(e) {
                            if (e.which === 13) {
                                //if enter pressed on the 'Add passenger' link, invoke the dialog
                                SEL.MileageGrid.showPassengersDialog.call(this);
                            }
                        })
                        .on("click", ".deleterow", SEL.MileageGrid.deleteRow) //the red cross at the right
                    
                        //when an address has been retrieved (its hidden 'id' field populated),
                        //go and get the distance (if we've got both):
                        .on("change", "input[data-field$='_id']", SEL.MileageGrid.mileageGridAddressChanged)
                    
                        //checkboxes need a hidden, because the actual checkbox may be removed and
                        //if we used that in the postback the arrays of form fields would be a different length
                        .on("change", "input[type='checkbox']", SEL.MileageGrid.setCheckboxHidden)
                    
                        //show/hide the recommended distance tooltip on hover/leave (could probably be genericised)
                        .on("mouseenter", "tr:not(.template) *[data-field='recommendeddistance'], .deleterow", function () {
                            $(".mileagecomment", $(this).closest("td")).show()
                                .position({ my: "left top", at: "right+10 top", of: this });
                        })
                        .on("mouseleave", "tr:not(.template) *[data-field='recommendeddistance'], .deleterow", function() {
                            $(".mileagecomment", $(this).closest("td")).hide();
                        })
                    
                    
                        //show the address tooltip when hovering over the binoculars, but not on the template row
                        .on("mouseenter", "tr:not(.template) .binoculars", function (event) {
                            var image = $(this);
                            var hiddenField = $("input[data-field$='_id']", $(this).closest("td"));
                            SEL.MileageGrid.AddressDetailsPopup = addressDetailsPopup.displayAddressWithElements(image.get(0), hiddenField.get(0), event);
                        })
                        .on("mouseleave", ".binoculars", function() {
                            if (SEL.MileageGrid.AddressDetailsPopup && SEL.MileageGrid.AddressDetailsPopup.hide) {
                                SEL.MileageGrid.AddressDetailsPopup.hide();
                                SEL.MileageGrid.AddressDetailsPopup = null;
                            }
                        })
                    
                        //don't allow the user to use the controls in the template row
                        .on("keypress, input", "tr.template input", function(event) {
                            event.preventDefault();
                            $(this).val(null);
                            return false;
                        })
                    
                        //show the 'distance could not be calculated' warning when the user hovers over
                        //the yellow triangle (could probably be genericised)
                        .on("mouseenter", ".nodistancewarning", function() {
                            $(".nodistancecomment", $(this).closest("td"))
                                .show()
                                .position({ my: "left top", at: "right+10 top", of: this });
                        }).on("mouseleave", ".nodistancewarning", function() {
                            $(".nodistancecomment", $(this).closest("td")).fadeOut("fast");
                        });
                    
                    //checkboxes need a hidden, because the actual checkbox may be removed and
                    //if we used that in the postback the arrays of form fields would be a different length
                    $(".mileagegridtable input[type='checkbox']", mileagePanel).each(SEL.MileageGrid.setCheckboxHidden);

                    //we start off with just a template row, so need to create an actual row
                    $(".mileagegridtable", mileagePanel).each(function () {
                        var mileageGridTable = this;
                        if ($("tbody tr:not(.template)", mileageGridTable).length === 0) {
                            //(only if we haven't already got a non-template row)
                            SEL.MileageGrid.duplicateTemplateRow(mileageGridTable);
                        }
                    });

                    $("input[data-field='passengers'][type='hidden']", mileagePanel).each(function () {
                        var clicker = $(".passengersclick", $(this).closest("tr"));
                        $(this).data("clicker", clicker);
                        SEL.MileageGrid.updatePassengersText($(this));
                    });

                    $(".returntostart span", mileagePanel).on("click", function () {
                        var mileageGridTable = $(".mileagegridtable", $(this).closest("td"));
                        var firstrow = $("tbody tr:first", mileageGridTable);
                        var startId = $("input[data-field='from_id']", firstrow).val();
                        if ((!startId) || (!$("input[data-field='to_id']", firstrow).val())) {
                            SEL.MasterPopup.ShowMasterPopup("You must enter a valid from and to address in the first row of the mileage grid.", "Message from " + moduleNameHTML);
                        } else {
                            var lastCurrentRow = $("tbody tr:not(.template):last", mileageGridTable);
                            var lastToId = $("input[data-field='to_id']", lastCurrentRow).val();
                            if (lastToId === startId) {
                                SEL.MasterPopup.ShowMasterPopup("The destination on the last row is already the same as the start address.", "Message from " + moduleNameHTML);
                            } else {
                                SEL.MileageGrid.duplicateTemplateRow(mileageGridTable);
                                var newrow = $("tbody tr:not(.template):last", mileageGridTable);
                                $("input[data-field='to_id']", newrow).val(startId).change(); //set the address ID and fire the change event for distance calc
                                $("input[data-field='to_search']", newrow).val($("input[data-field='from_search']", firstrow).val()); //set search text
                            }
                        }
                    });
                    
                    // return to start message tooltip behaviour
                    $(".returntostart span", mileagePanel).on("mouseenter", function() {
                        $(this).siblings(".mileagecomment").show().position({ my: "left top", at: "right+10 top", of: this });
                    })
                    .on("mouseleave", function() {
                        $(this).siblings(".mileagecomment").hide();
                    });

                    $(".showhelplink", mileagePanel).click(function () {
                        var mileageGridTable = $(".mileagegridtable", mileagePanel);
                        SEL.MileageGrid.showMileageGridHelp(mileageGridTable, this);
                    });

                    // automatically display the contextual help if the "autoshow" class is present
                    if ($(".helptext", mileagePanel).hasClass("autoshow")) {
                        $(".showhelplink", mileagePanel).click();
                        SEL.MileageGrid.positionHelpIndicatorAndSetHelpText($("input[data-field=from_search]", mileagePanel).parent("td"), $(".mileagegridtable", mileagePanel));
                    }

                    $(".showmaplink span", mileagePanel).click(function () {
                        var mileageGrid = $(".mileagegridtable", $(this).closest(".mileagePanel"));
                        ShowRouteAddEditExpenses(mileageGrid);
                    });
                }).attr("setup", true);

                $(".carandjourneyrate[setup!=true]").each(function () {
                    //these controls aren't always nested within a div.mileagePanel - 
                    //such as is the case for single-step mileage
                    var carAndJourneyRate = $(this);
                    $("select[name*='cmbcars']", carAndJourneyRate)
                        .change(SEL.MileageGrid.setDefaultUom).each(SEL.MileageGrid.setDefaultUom)
                        .change(SEL.MileageGrid.populateMileageCats).each(SEL.MileageGrid.populateMileageCats);
                    //could probably be genericised
                    $("select[id*='cmbmileagecat']", carAndJourneyRate)
                        .on("mouseenter", function () {
                            $("*[id*='mileagecomment']", $(this).closest("div")).show();
                        })
                        .on("mouseleave", function () {
                            $("*[id*='mileagecomment']", $(this).closest("div")).hide();
                        })
                        .on("change", SEL.MileageGrid.updateMileageCatInfo);
                }).attr("setup", true);
                
                $(".hometoofficewarninglabel[setup!=true]").mouseenter(function () {
                    //home to office warning label isn't always in the mileage panel as it can
                    //appear on single step mode... however it isn't in the car panel either
                    var homeToOfficeComment = $(".hometoofficecomment", $(this).closest("td"));
                    $(homeToOfficeComment).show().position({ my: "left top", at: "right+10 top", of: this });
                }).mouseleave(function () {
                    var homeToOfficeComment = $(".hometoofficecomment", $(this).closest("td"));
                    $(homeToOfficeComment).fadeOut("fast");
                }).attr("setup", "true");

                
                $(".singlejourneystepmileage").closest("table").on("change", "input[id*='txtfromid'], input[id*='txttoid']", function () {
                    //calculate the mileage for single journey step mode
                    var table = $(this).closest("table");
                    var fromIdControl = $("input[id*='txtfromid']", table);
                    var toIdControl = $("input[id*='txttoid']", table);
                    if (fromIdControl.val() && toIdControl.val()) {
                        var widgetTable = $(this).closest('table');
                        SEL.MileageGrid.calculateMileage($(widgetTable).data('A' + fromIdControl.val()), $(widgetTable).data('A' + toIdControl.val()), table);
                    }
                });

                // when the esr assignment is changed any address fields flagged with the ESR attribute need repopulating
                $("td.esrAssignment select").on("change", function () {
                    SEL.AddressesAndTravel.UpdateHomeAndOfficeAddresses($(this));
                });

            },
            
            updateMileageCatInfo: function() {
                var id = $(this).data("id");
                //this sets the tooltips on the journey rate drop down and also the 'home to office deductions apply' text
                selectMileageCategory(id);
                getMileageComment(id);
            },
            
            populateMileageCats: function () {
                //populates the mileage cats dropdown (when the car changes, and on page load)
                //this replaces the ajax-control toolkit's cascading dropdown.
                var carsDropDown = $(this);
                var mileageCatsDropDown = $("select[id*='cmbmileagecat']", $(carsDropDown).closest(".carandjourneyrate"));
                var id = $(carsDropDown).val();
                if ($(mileageCatsDropDown).data("enforced")) {
                    //don't repopulate it if it's an enforced journey rate. But do set the tooltip
                    SEL.MileageGrid.updateMileageCatInfo.call(mileageCatsDropDown);
                } else {
                    var selectedValue = $("option:selected", mileageCatsDropDown).attr("value");
                    $("option", mileageCatsDropDown).remove();
                    if (id) {
                        SEL.Data.Ajax({
                            serviceName: "svcAutocomplete",
                            methodName: "getMileageCategoriesByCarId",
                            data: { carid: id },
                            success: function(data) {
                                $("option", mileageCatsDropDown).remove();
                                $(data.d).each(function() {
                                    var option = $("<option>").text(this.name).val(this.id);
                                    if (this.id == selectedValue) {
                                        //re-select the one that was previously selected
                                        $(option).attr("selected", "selected");
                                    }
                                    $(mileageCatsDropDown).append(option);
                                });
                                $(mileageCatsDropDown).change();
                            }
                        });
                    }
                }
            },
            
            setDefaultUom: function () {
                var defaultUom = $("option:selected", this).data("defaultuom");
                var mileageGridTable = $(".mileagegridtable", $(this).closest(".mileagePanel"));
                if (defaultUom) {
                    defaultUom = defaultUom.toLowerCase();
                    if (defaultUom === "mile") {
                        defaultUom += "s";
                    }
                    var userEnteredDistance = $("input[data-field='userentereddistance']", mileageGridTable);
                    var oldUom = $(userEnteredDistance).attr("placeholder");
                    if (oldUom !== "km") {
                        oldUom = "miles"; //i.e., it was 'distance', as it starts off as. We haven't set it yet,
                        //so it's as it came back from the database, i.e. miles.
                    }
                    userEnteredDistance.attr("placeholder", defaultUom);
                    $("input[data-field='uom']", mileageGridTable).val(defaultUom);
                    
                    var conversion = null;
                    if (oldUom && oldUom.toLowerCase() === "miles" && defaultUom === "km") {
                        //changed from mile to km
                        conversion = SEL.MileageGrid.kilometresPerMile;
                    } else if (oldUom && oldUom.toLowerCase() == "km" && defaultUom === "miles") {
                        //changed from km to mile
                        conversion = 1 / SEL.MileageGrid.kilometresPerMile;
                    }
                    if (conversion) {
                        //it's changed from miles to km, or vice versa
                        $("input[data-field='userentereddistance'], input[data-field='recommendeddistance']", mileageGridTable).each(function() {
                            var val = parseFloat($(this).val());
                            if (val) {
                                $(this).val((val * conversion).toFixed(2));
                            }
                        });
                    }
                }
            },
            
            validateNumPassengers: function (evt) { //only allow numbers
                evt = (evt) ? evt : window.event;
                var charCode = (evt.which) ? evt.which : evt.keyCode;
                if (charCode > 31 && (charCode < 48 || charCode > 57)) {
                    return false;
                }
                return true;
            },

            validateDistance: function (evt) { //only allow numbers (decimal)
                evt = (evt) ? evt : window.event;
                var charCode = (evt.which) ? evt.which : evt.keyCode;
                if (charCode > 31 && charCode != 46 && charCode != 44 && (charCode < 48 || charCode > 57)) {
                    return false; //46 = . 44 = ,
                }
                return true;
            },

            addCustomPassenger: function (button) {
                var dialog = $(button).closest(".custompassengerdialoginner");
                var input = $(dialog).data("input");
                var textbox = $("input[data-field='custompassenger']", dialog);
                //don't allow commas, ampersands or equals, as they are used as delimeters
                var item = { id: "0", name: $(textbox).val().replace(/[,&\=]/, ""), searchDisplay: $(textbox).val().replace(/[,&\=]/, "") };
                if (item.searchDisplay.trim()) {
                    $(input).tokenInput("add", item);
                }
                $(dialog).dialog("close");
            },

            cancelCustomPassenger: function (button) {
                $(button).closest(".custompassengerdialoginner").dialog("close");
            },

            formatResult: function (item) {
                //called by the token-input plugin to determine how to format the list item for each result
                if (item.id === "-1") {
                    return "<li class='cantfind'>" + item.searchDisplay + "</li>";
                    //this is the 'can't find what you're looking for?' one that appears at the bottom
                } else {
                    return "<li>" + item.searchDisplay + "</li>";
                }
            },

            updatePassengersText: function (input) {
                var passengersClicker = $(input).data("clicker");
                var val = $(input).val().trim();
                var numberOfPassengers = val == "" ? 0 : val.split(",").length; // ("".split(",").length returns 1 for some bizarre reason...)
                if (numberOfPassengers === 0) {
                    $("span", passengersClicker).text("Add Passenger");
                } else {
                    $("span", passengersClicker).text(numberOfPassengers + " Passenger" + (numberOfPassengers > 1 ? "s" : ""));
                }
                $("input[type=hidden][name$=_numpassengers]", $(passengersClicker).closest("tr")).val(numberOfPassengers);
            },

            onPassengerDelete: function () {
                if ($(this).data("isTokenInputOn")) {
                    //we don't want to do it in response to destroy
                    //otherwise it wouldn't remember the passengers.
                    SEL.MileageGrid.updatePassengersText(this);
                }
            },

            onPassengerAdd: function (item, textSearched) {
                var input = this;
                if (item.id === "-1") {
                    //...they've clicked 'Can't find who you're looking for'
                    var tr = $(input).data("tr");
                    var dialoginner = $(".custompassengerdialoginner", tr);
                    $("input[type='text'][data-field='custompassenger']", dialoginner).val(textSearched);
                    dialoginner.data("input", input).dialog({
                        dialogClass: "passengersdialog custompassengerdialog",
                        modal: true,
                        resizable: false,
                        draggable:false,
                        width: "360px",
                        height: "auto",
                        position: { my: "left+10px top+10px", at: "left top", of: input.parent() },
                        open: function () {
                            
                            $(".ui-widget-header", $(this).closest(".custompassengerdialog"))
                              .removeClass("ui-widget-header") //otherwise it overrides inputpaneltitle
                              .removeClass("ui-corner-all")
                              .addClass("inputpaneltitle");
                            //put the cursor to the end:
                            var textBox = $("input[type=text]", this)[0];
                            if (textSearched && textBox.createTextRange) { //only necessary for IE <9
                                var textRange = textBox.createTextRange();
                                textRange.collapse(true);
                                textRange.moveStart('character', textSearched.length);
                                textRange.moveEnd('character', textSearched.length);
                                textRange.select();
                            }

                            // pressing the "Enter" key should have the same behaviour as clicking the "save" button
                            $(textBox).on("keypress", $.proxy(function(event) {
                                if (event.key === "Enter") {
                                    var saveButton = $(this).find("input.buttonSave");
                                    saveButton.trigger("click", [saveButton[0]]);
                                }
                            }, this));
                        },
                        close: function () {
                            $(this).dialog("destroy");
                            $("li.token-input-input-token input").focus();
                        }
                    });

                } else {
                    //they've selected a (known) passenger, update the text of the link
                    SEL.MileageGrid.updatePassengersText(this);
                }
            },

            mileageGridAddressChanged: function () {
                var tr = $(this).closest("tr");
                var from = $("input[data-field='from_id']", tr);
                var to = $("input[data-field='to_id']", tr);
                //are they both filled in?
                if (from.val() && to.val()) {
                    var widgetTable = $(this).closest('table');
                    SEL.MileageGrid.calculateMileage($(widgetTable).data('A' + from.val()), $(widgetTable).data('A' + to.val()), tr);
                }
            },

            showMileageGridHelp: function (mileageGridTable, helplink) {
                var showHelp = !mileageGridTable.data("helpactive");
                if (showHelp) {
                    //if the help isn't already showing, display it.
                    $(".helptext", mileageGridTable.closest(".mileagePanel"))
                        .css("width", mileageGridTable.width() + "px") //necessary to make sure it doesn't forcibly
                                                                            //expand the cells when the text changes
                        .slideDown("fast", function () {
                            $(".helpindicator", mileageGridTable.closest(".mileagePanel")).show();
                            // set position and text for the first field
                            SEL.MileageGrid.positionHelpIndicatorAndSetHelpText($("input[data-field=from_search]", mileageGridTable).parent("td"), mileageGridTable);
                    });
                    mileageGridTable.data("helpactive", true).data("originalhelptext", $(helplink).text());
                    $(helplink).text("Close help");
                    mileageGridTable.on("focus", "tbody tr td input, tbody tr td div", function () {
                        //when a control is focused, position the arrow to point to to that control, and set the text, except the delete [x]
                        if ($(this).hasClass("deleterow")) {
                            return;
                        }

                        var td = $(this).closest("td");
                        SEL.MileageGrid.positionHelpIndicatorAndSetHelpText(td, mileageGridTable);
                    });
                } else {
                    $(".helpindicator", mileageGridTable.closest(".mileagePanel")).hide();
                    $(".helptext, .helpindicator", mileageGridTable.closest(".mileagePanel")).slideUp("fast");
                    mileageGridTable.data("helpactive", false);
                    $(helplink).text(mileageGridTable.data("originalhelptext"));
                }

                // remember the state of the contextual help
                SEL.Data.Ajax({
                    serviceName: "svcContextualHelp",
                    methodName: showHelp ? "MarkAsUnread" : "MarkAsRead",
                    data: {
                        contextualHelpId: 1 // this value is hardcoded until the rest of the contextual help feature is implemented
                    }
                });
            },

            positionHelpIndicatorAndSetHelpText: function (td, mileageGridTable) {
                //this puts the arrow next to the td of the control that's been focused.
                var col = $(td).index();
                var bodytd = $("tbody tr:first td", mileageGridTable).get(col);
                var tr = $("tr:first", mileageGridTable);
                var colPosRelativeToTable = $(bodytd).offset().left - $(tr).offset().left;
                var necessaryMiddlePosOfArrow = colPosRelativeToTable + $(bodytd).width() / 2;
                var positionOfArrow = necessaryMiddlePosOfArrow; //it's margin-left is set such that it 'points' to its set position.
                $(".helpindicator", mileageGridTable.closest(".mileagePanel")).animate({ left: positionOfArrow + "px" });
                
                var helptextkey = $("input:first", td).data("field");
                if (helptextkey === "numpassengers") {
                    //if we're in the passengers td, then 'numpassengers' is always the first input, but we want
                    //to display the help for either passenger names or passenger numbers, whichever is displayed.
                    if ($("td.passengernames", td.closest("tr")).is(":visible")) {
                        helptextkey += "_names";
                    } else {
                        helptextkey += "_nonames";
                    }
                }
                
                //lookup the text from the array defined at the top of this file
                $(".helptext span", mileageGridTable.closest(".mileagePanel")).text(SEL.MileageGrid.mileageGridHelpTexts[helptextkey]);
            },

            setCheckboxHidden: function () {
                //set a hidden form field to the checked status of the checkbox, as asp.net is not capable of parsing multiple checkbox form fields
                //of same name into an array.
                var field = $(this).data("field");
                var tr = $(this).closest("tr");
                $("input[type='hidden'][data-field='" + field + "']", tr).val($(this).is(":checked").toString());
            },

            deleteRow: function () {
                var mileageGridTable = $(this).closest(".mileagegridtable");
                var row = $(this).closest("tr");

                if (!row.hasClass("template")) {
                    row.remove();
                    if ($("tbody tr:not(.template)", mileageGridTable).length === 0) {
                        SEL.MileageGrid.duplicateTemplateRow(mileageGridTable);
                        //thus deleting the only row appears to have the effect of just blanking it out,
                        //rather than leaving us with fewer rows than we start with
                    }
                    SEL.MileageGrid.setRowIdentifiers(mileageGridTable);
                }
            },

            showPassengersDialog: function (e) {
                SEL.MileageGrid.positionHelpIndicatorAndSetHelpText($(this).closest("td"), $(this).closest(".mileagegridtable"));
                var passengersClicker = this;
                var tr = $(this).closest("tr");
                var passengersDialog = $(".passengersdialoginner", tr);
                passengersDialog.data("tr", tr);
                passengersDialog.dialog({
                    dialogClass: "passengersdialog",
                    modal:true,
                    resizable: false,
                    width: "375px",
                    height: "auto",
                    draggable: false,
                    open: function () {
                        $(".ui-widget-header", $(this).closest(".passengersdialog"))
                            .removeClass("ui-widget-header") //otherwise it overrides inputpaneltitle
                            .removeClass("ui-corner-all")       
                            .addClass("inputpaneltitle");
                        var passengersSearch = $("input[data-field='passengers']", this);
                        SEL.MileageGrid.setupPassengersInput(passengersSearch, tr);
                        passengersSearch.focus();
                        $(passengersSearch).data("clicker", passengersClicker);
                    },
                    close: function () {
                        $("input[data-field='passengers']", tr).val($("input[data-field='passengers']", passengersDialog).val());
                        $(this).dialog("destroy"); //puts it back into its original DOM position so can be used again#
                        var passengersVal = $("input[data-field='passengers']").val();
                        $("input[data-field='passengers']", this)
                            .data("isTokenInputOn", false)
                            .tokenInput("destroy").val(passengersVal);
                    },
                    position: { my: "left top", at: "right top", of: $(this) }
                });
            },

            closePassengersDialog: function (button) {
                var dialog = $(button).closest(".passengersdialoginner");
                var tr = $(dialog).data("tr");
                $("input[data-field='passengers']", tr).val($("input[data-field='passengers']", dialog).val());
                dialog.dialog("close");
            },

            setRowIdentifiers: function (mileageGridTable) {
                $("tbody tr", mileageGridTable).each(function (rowIndex) {
                    $("span[data-field='mileagerownum']", this).text(rowIndex + 1);
                });
            },


            calculateMileage: function (fromAddress, toAddress, container) {
                var recommendedDistanceInput = $("input[data-field='recommendeddistance']", container);
                var recommendedDistanceDiv = $("div[data-field='recommendeddistance']", container);
                var userEnteredDistance = $("input[data-field='userentereddistance']", container);
                var td = recommendedDistanceInput.closest("td");
                var loadingDiv = $(".loading", td);
                var warning = $(".warning", td);
                if (loadingDiv.css('visibility') === 'visible') {
                    return;
                }
                loadingDiv.css("visibility", "visible");
                warning.css("visibility", "hidden");
                $("input[data-field='to_search']", container).css("font-weight", "normal");
                $("input[data-field='from_search']", container).css("font-weight", "normal");

                fromAddress.CreatedOn = '';
                fromAddress.ModifiedOn = '';
                fromAddress.LookupDate = '';
                toAddress.CreatedOn = '';
                toAddress.ModifiedOn = '';
                toAddress.LookupDate = '';
                var params = {
                    fromAddress: fromAddress,
                    toAddress: toAddress
                };
                SEL.Data.Ajax({
                    data: params,
                    methodName: "GetCustomOrRecommendedDistance",
                    serviceName: "svcAddresses",
                    success: function (data) {
                        if (data.d === null) {
                            $(".nodistancecomment", td).text(SEL.MileageGrid.mileageGridHelpTexts.unabletocalculateduetomissingpostcode); // just in case it has been overriden by an invalid mobile location
                            warning.css("visibility", "visible");
                            //if it couldn't calculate the mileage, then blank it out if it was already set:
                            recommendedDistanceInput.val("");
                            recommendedDistanceDiv.text("");
                            userEnteredDistance.val("");
                        } else {
                            warning.css("visibility", "hidden");
                            var dist = parseFloat(data.d);
                            var uom = $("select[name*='cmbcars'] option:selected", container.closest(".mileagePanel")).data("defaultuom");
                            //it should always come back in miles, so if we're displaying in km, then we need to convert
                            if (uom && uom.toLowerCase() === "km") {
                                dist = dist * SEL.MileageGrid.kilometresPerMile;
                            }
                            dist = dist.toFixed(2);
                            recommendedDistanceInput.val(dist);
                            recommendedDistanceDiv.text(dist);
                            userEnteredDistance.val(dist);
                        }
                    },
                    error: function() {
                        $("div.warning", td).show();
                    },
                    complete: function () {
                        loadingDiv.css("visibility", "hidden"); //don't hide, as we want it to take up width
                    }
                });
            },

            duplicateTemplateRow: function (mileageGridTable) {

                var lastRow = $("tbody tr:not(.template):last", mileageGridTable);
                if (lastRow.length == 0 || //if we haven't already got an editable row, or...
                    ($("input[data-field='from_id']", lastRow).val() && $("input[data-field='to_id']", lastRow).val()) /* both from and to have been filled in.*/) {
                
                    //the old template row now becomes the edited row.
                    var newRow = $("tr.template", mileageGridTable);

                    //...and we make a new template row by copying the old one.
                    var newTemplateRow = newRow.clone(true);
                    SEL.MileageGrid.setSearchIdsOnRow(newRow);
                    newTemplateRow.insertAfter(newRow);

                    newRow.removeClass("template");

                    //set the 'from' to be the 'to' of the current row:
                    SEL.MileageGrid.setupRow(newRow);
                    var lastToAddressElement = $("input[data-field='to_search']", lastRow);
                    var lastToAddressText = lastToAddressElement.val();
                    var lastToAddressId = $("input[data-field='to_id']", lastRow).val();
                    $("input[data-field='from_search']", newRow).address("value", lastToAddressText, lastToAddressId).attr("data-office", lastToAddressElement.data("office"));

                    SEL.MileageGrid.setRowIdentifiers(mileageGridTable);

                    $("input[data-field='mileagegridtable_numrows']", mileageGridTable).val($("tr:not(.template)", mileageGridTable).length);//set the row num counter to be however many rows have been filled in (in the whole table)
                } else {
                    SEL.MasterPopup.ShowMasterPopup("You must enter a valid from and to address in the previous row of the mileage grid.", "Message from " + moduleNameHTML);
                }
            },

            setSearchIdsOnRow: function(tr) {
                //any search fields that haven't got an id, give them one
                if (SEL.MileageGrid.currentLatestId == undefined) {
                    SEL.MileageGrid.currentLatestId = 0;
                }
                $("input[data-field$='_search']", tr).each(function () {
                    var id = $(this).data("field") + SEL.MileageGrid.currentLatestId;
                    var hiddenId = "hidden_for_" + id;
                    var hidden = $("input[data-field$='_id'][type='hidden']", $(this).closest("td"));
                    $(this).attr("id", id);
                    $(this).attr("rel", hiddenId);
                    hidden.attr("id", hiddenId);
                    SEL.MileageGrid.currentLatestId += 1;
                });
            },
            
            setupRow: function (newTr) {
                if (!$(newTr).hasClass("template")) {
                    SEL.MileageGrid.setSearchIdsOnRow(newTr);
                }
                $("input[data-field$='_search']", newTr).each(function () {
                    $(this).address({
                        enableLabels: !CurrentUserInfo.IsDelegate,
                        enableFavourites: !CurrentUserInfo.IsDelegate,
                        enableAddingAddresses: allowClaimantAddManualAddresses,
                        backgroundElement: $("input[type='hidden'][data-field$='_id']", $(this).closest("td")),
                        enableHomeAndOffice: true,
                        esrAssignmentField: $(this).parents(".mileagePanel").find(".esrAssignment select").first(),
                        forceAddressNameEntry: forceAddressNameEntry,
                        strings: {
                            addressNameEntryMessage: addressNameEntryMessage
                        }
                    });
                });
            },

            setupPassengersInput: function (passengersInput, tr) {
                passengersInput.val($("input[type='hidden'][data-field='passengers']", tr).val());

                var searchEmployeesServiceUrl = SEL.Data.GetServiceUrl({ serviceName: "svcAutoComplete", methodName: "searchEmployees" });
                var existingPassengers = [];
                //passengersInput should be something like "ID=1&Name=Ben,ID=2&Name=Lyall,ID=3&Name=Paul"
                //this needs to be translated to the json 
                // [{id:1, name:Ben}, {id:2, name:Lyall}, {id:3, name: Paul}]
                if (passengersInput.val()) {
                    $(passengersInput.val().split(",")).each(function () {
                        if (this.length > 0) {
                            var passenger = {};
                            $(this.split("&")).each(function () {
                                var kvp = this.split("=");
                                if (kvp.length === 2) {
                                    passenger[kvp[0].toLowerCase()] = kvp[1];
                                }
                            });
                            existingPassengers.push(passenger);
                        }
                    });
                }
                if (!$(passengersInput).data("tokenInputObject")) { // if it's not already been created
                    $(passengersInput).data("tr", tr).data("isTokenInputOn", true).tokenInput(searchEmployeesServiceUrl, {
                        method: "POST",
                        contentType: "json",
                        jsonContainer: "d",
                        hintText: "Enter passenger name (don't add the driver)...",
                        minChars: 3,
                        hideOnBlur: true,
                        resultsFormatter: SEL.MileageGrid.formatResult,
                        prePopulate: existingPassengers,
                        onAdd: SEL.MileageGrid.onPassengerAdd,
                        onDelete: SEL.MileageGrid.onPassengerDelete,
                        tokenValue: SEL.MileageGrid.mileageGridTokenValue,
                        canInsert: SEL.MileageGrid.canInsertItemToTokenInput,
                        getComparisonValue: SEL.MileageGrid.getPassengerComparisonValue,
                        zindex: SEL.Common.GetHighestZIndex() + 1,
                        preventDuplicates: true,
                        onSend: function (ajax_params) {
                            ajax_params.contentType = "application/json";
                            // convert the data to a string if necessary
                            if ($.type(ajax_params.data) === 'object') {
                                ajax_params.data = JSON.stringify(ajax_params.data);
                            }
                        }
                    });
                }
            },
            
            getPassengerComparisonValue: function(item) {
                return JSON.stringify({ id: item.id, name: item.name });
                //there will be other properties like 'type' which will otherwise cause a false negative
            },

            mileageGridTokenValue: function (item) {
                var stringval = "ID=" + item.id.replace(/[,&\=]/, "") + "&Name=" + item.name.replace(/[,&\=]/, "");
                return stringval;
            },

            canInsertItemToTokenInput: function (item) {
                return item.id !== "-1";
            }
        };
    }

    if (window.Sys && window.Sys.loader) {
        window.Sys.loader.registerScript(scriptName, null, execute);
    }
    else {
        execute();
    }
}(moduleNameHTML, appPath));
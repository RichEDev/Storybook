/* <summary>Vehicle Journey Rate methods</summary> */
(function () {
    var scriptName = "VehicleJourneyRate";

    function execute() {
        SEL.registerNamespace("SEL.VehicleJourneyRate");
        SEL.VehicleJourneyRate =
        {
            CONTENT_MAIN: '',

            CurrentAction: null,
            CurrentMileageId: null,
            CurrentDateRangeId: null,
            CurrentDateRangeType: null,
            CurrentThresholdId: null,
            CurrentThresholdType: null,

            SetList: function (items, selection, controlId) {
                var $ddl = $('#' + controlId);

                if ($ddl.length > 0) {
                    $ddl.empty();

                    var itemArray = new String(items).split(',');

                    for (var i = 0; i < itemArray.length; i++) {
                        var $option = $("<option/>").val(itemArray[i]).appendTo($ddl);

                        switch (itemArray[i]) {
                            case '0':
                                $option.text(controlId == cmbDateRangeType ? 'Before' : 'Greater than or equal');
                                break;

                            case '1':
                                $option.text(controlId == cmbDateRangeType ? 'After or equal to' : 'Between');
                                break;

                            case '2':
                                $option.text(controlId == cmbDateRangeType ? 'Between' : 'Less than');
                                break;

                            case '3':
                                $option.text('Any');
                                break;
                        }
                    }

                    $ddl.val(selection);

                }
            },

            Save: function () {
                if (validateform('vgMain') == false) {
                    return;
                }

                var carsize = document.getElementById(txtcarsize).value;
                var description = document.getElementById(txtdescription).value;
                var threshold = new Number(document.getElementById(cmbThreshold).options[document.getElementById(cmbThreshold).selectedIndex].value);
                var currency = 0;

                if (document.getElementById(cmbCurrency).selectedIndex != -1) {
                    currency = new Number(document.getElementById(cmbCurrency).options[document.getElementById(cmbCurrency).selectedIndex].value);
                }
                var uom = new Number(document.getElementById(cmbUom).options[document.getElementById(cmbUom).selectedIndex].value);
                var calcmilestotal = document.getElementById(chkcalcmilestotal).checked;
                var userRate = document.getElementById(txtuserratetable);
                var userRateTable = '';
                var userRateFromEngine = 0;
                var userRateToEngine = 0;
                var financialYearID = new Number(document.getElementById(ddlFinancialYear).options[document.getElementById(ddlFinancialYear).selectedIndex].value);

                if (userRate !== null) {
                    userRateTable = document.getElementById(txtuserratetable).value;
                    userRateFromEngine = document.getElementById(txtuserratefromengine).value;
                    userRateToEngine = document.getElementById(txtuserratetoengine).value;
                }

                SEL.Ajax.Service("/shared/webServices/svcVehicleJourneyRate.asmx", "VehicleJourneyRateSave",
                {
                    "mileageId": SEL.VehicleJourneyRate.CurrentMileageId,
                    "carSize": carsize,
                    "comment": description,
                    "thresholdType": threshold,
                    "calcMilesTotal": calcmilestotal,
                    "mileUom": uom,
                    "currencyId": currency,
                    "userRateTable": userRateTable,
                    "userRateEngineFromSize": userRateFromEngine,
                    "userRateEngineToSize": userRateToEngine,
                    "financialYearId": financialYearID
                }, function (d) {
                    var data = d.d;
                    if (data == -1) {
                        alert('The Vehicle Journey Rate Category cannot be added as the category name you have entered already exists');
                        return;
                    }

                    SEL.VehicleJourneyRate.CurrentMileageId = data;

                    switch (SEL.VehicleJourneyRate.CurrentAction) {
                        case 'saveDateRange':
                            SEL.VehicleJourneyRate.CurrentThresholdType = null; //Set to null so to make sure the type is not Any if changed to something else
                            SEL.VehicleJourneyRate.DateRange.ShowModal(false);
                            break;
                        case 'OK':
                            document.location = 'adminmileage.aspx';
                            break;
                    }
                }, SEL.VehicleJourneyRate.ShowError);
            },

            Delete: function (mileageId) {
                if (confirm('Are you sure you wish to delete the selected vehicle journey rate category?')) {
                    SEL.Ajax.Service("/shared/webServices/svcVehicleJourneyRate.asmx", "VehicleJourneyRateDelete",
                    {
                        "mileageId": mileageId
                    }, function (d) {
                        SEL.Grid.deleteGridRow('gridMileage', mileageId);
                    }, SEL.VehicleJourneyRate.ShowError);
                }
            },

            DateRange:
            {
                cmbDateRangeType_onchange: function () {
                    var rangetype = document.getElementById(cmbDateRangeType).options[document.getElementById(cmbDateRangeType).selectedIndex].value;
                    switch (rangetype) {
                        case '0':
                        case '1':
                            document.getElementById('spanDateThreshold1').style.display = '';
                            document.getElementById('spanDateThreshold2').style.display = 'none';
                            ValidatorEnable(document.getElementById(reqDateVal1), true);
                            ValidatorEnable(document.getElementById(reqDateVal2), false);
                            ValidatorEnable(document.getElementById(cmpDateVal1min), true);
                            ValidatorEnable(document.getElementById(cmpDateVal1max), true);
                            ValidatorEnable(document.getElementById(cmpDateVal2min), false);
                            ValidatorEnable(document.getElementById(cmpDateVal2max), false);
                            break;

                        case '2':
                            document.getElementById('spanDateThreshold1').style.display = '';
                            document.getElementById('spanDateThreshold2').style.display = '';
                            ValidatorEnable(document.getElementById(reqDateVal1), true);
                            ValidatorEnable(document.getElementById(reqDateVal2), true);
                            ValidatorEnable(document.getElementById(cmpDateVal1min), true);
                            ValidatorEnable(document.getElementById(cmpDateVal1max), true);
                            ValidatorEnable(document.getElementById(cmpDateVal2min), true);
                            ValidatorEnable(document.getElementById(cmpDateVal2max), true);
                            break;

                        case '3':
                            document.getElementById('spanDateThreshold1').style.display = 'none';
                            document.getElementById('spanDateThreshold2').style.display = 'none';
                            ValidatorEnable(document.getElementById(reqDateVal1), false);
                            ValidatorEnable(document.getElementById(reqDateVal2), false);
                            ValidatorEnable(document.getElementById(cmpDateVal1min), false);
                            ValidatorEnable(document.getElementById(cmpDateVal1max), false);
                            ValidatorEnable(document.getElementById(cmpDateVal2min), false);
                            ValidatorEnable(document.getElementById(cmpDateVal2max), false);
                            break;
                    }
                },

                ShowModal: function (save) {
                    SEL.VehicleJourneyRate.CurrentAction = 'saveDateRange';
                    if (save) {
                        document.getElementById(txtDateVal1).value = "";
                        document.getElementById(txtDateVal2).value = "";

                        SEL.VehicleJourneyRate.CurrentDateRangeType = null;
                        SEL.VehicleJourneyRate.Save();
                        return;
                    }

                    if (SEL.VehicleJourneyRate.CurrentThresholdType == null) {
                        $('#lnkAddThreshold').show();
                    }
                    else {
                        $('#lnkAddThreshold').show();
                    }

                    SEL.Ajax.Service("/shared/webServices/svcVehicleJourneyRate.asmx", "DateRangeAvailableTypes",
                    {
                        "mileageId": SEL.VehicleJourneyRate.CurrentMileageId,
                        "dateRangeType": SEL.VehicleJourneyRate.CurrentDateRangeType
                    }, function (d) {
                        SEL.VehicleJourneyRate.SetList(d.d, SEL.VehicleJourneyRate.CurrentDateRangeType, cmbDateRangeType);

                        SEL.VehicleJourneyRate.DateRange.Threshold.PopulateGrid();
                        SEL.VehicleJourneyRate.DateRange.cmbDateRangeType_onchange();

                        $('#' + SEL.VehicleJourneyRate.CONTENT_MAIN + 'pnlDateRange').dialog("open");
                    }, SEL.VehicleJourneyRate.ShowError);
                },

                HideModal: function () {
                    $('#' + SEL.VehicleJourneyRate.CONTENT_MAIN + 'pnlDateRange').dialog("close");
                },

                Edit: function (id) {
                    SEL.Ajax.Service("/shared/webServices/svcVehicleJourneyRate.asmx", "DateRangeGet",
                    {
                        "dateRangeId": id,
                        "mileageId": SEL.VehicleJourneyRate.CurrentMileageId
                    }, function (d) {
                        var data = d.d;
                        SEL.VehicleJourneyRate.CurrentDateRangeId = data[0].mileagedateid;

                        if (data[0].dateRangetype != 0) {
                            if (data[0].dateValue1 != null) {
                                $("#" + txtDateVal1).datepicker("setDate", SEL.Ajax.ParseDate(data[0].dateValue1));
                            }
                            if (data[0].dateValue2 != null) {
                                $("#" + txtDateVal2).datepicker("setDate", SEL.Ajax.ParseDate(data[0].dateValue2));
                            }
                        }

                        SEL.VehicleJourneyRate.CurrentThresholdType = data[1];

                        SEL.VehicleJourneyRate.CurrentDateRangeType = data[0].daterangetype;
                        //SEL.VehicleJourneyRate.DateRange.cmbDateRangeType_onchange();
                        SEL.VehicleJourneyRate.DateRange.ShowModal(false);

                    }, SEL.VehicleJourneyRate.ShowError);
                },

                Save: function () {
                    if (SEL.VehicleJourneyRate.CurrentMileageId == 0) {
                        SEL.VehicleJourneyRate.CurrentAction = "SaveDateRange";
                        SEL.VehicleJourneyRate.Save();
                        return;
                    }
                    if (validateform('vgDateRange') == false) {
                        return;
                    }

                    var dateRangeType = Number($('#' + cmbDateRangeType).val());
                    var dateval1 = null;
                    var dateval2 = null;
                    switch (dateRangeType) {
                        case 0:
                        case 1:
                            dateval1 = document.getElementById(txtDateVal1).value.substring(6, 10) + "/" + document.getElementById(txtDateVal1).value.substring(3, 5) + "/" + document.getElementById(txtDateVal1).value.substring(0, 2);
                            break;

                        case 2:
                            dateval1 = document.getElementById(txtDateVal1).value.substring(6, 10) + "/" + document.getElementById(txtDateVal1).value.substring(3, 5) + "/" + document.getElementById(txtDateVal1).value.substring(0, 2);
                            dateval2 = document.getElementById(txtDateVal2).value.substring(6, 10) + "/" + document.getElementById(txtDateVal2).value.substring(3, 5) + "/" + document.getElementById(txtDateVal2).value.substring(0, 2);
                            break;
                    }

                    SEL.Ajax.Service("/shared/webServices/svcVehicleJourneyRate.asmx", "DateRangeSave",
                    {
                        "dateRangeId": SEL.VehicleJourneyRate.CurrentDateRangeId,
                        "mileageId": SEL.VehicleJourneyRate.CurrentMileageId,
                        "dateRangeType": dateRangeType,
                        "dateValue1": dateval1,
                        "dateValue2": dateval2
                    }, function (d) {
                        var data = d.d;
                        SEL.VehicleJourneyRate.CurrentDateRangeId = data[0];
                        if (data[1] != null && data[1] != "") {
                            SEL.MasterPopup.ShowMasterPopup(data[1], "Date Range Validation Failed");
                            return;
                        }

                        switch (SEL.VehicleJourneyRate.CurrentAction) {
                            case 'addThreshold':
                                document.getElementById(cmbRangeType).selectedIndex = 0;
                                document.getElementById(txtPassenger).value = "";
                                document.getElementById(txtPassengerx).value = "";
                                document.getElementById(txtHeavyBulky).value = "";
                                document.getElementById(txtThresholdVal1).value = "";
                                document.getElementById(txtThresholdVal2).value = "";

                                SEL.VehicleJourneyRate.DateRange.Threshold.Rate.PopulateGrid(false);

                                SEL.VehicleJourneyRate.DateRange.Threshold.ShowModal(false);
                                break;

                            case 'DateRangeOK':
                                if (data[2] == 3) {
                                    $('#lnkAddDateRange').hide();
                                }
                                else {
                                    $('#lnkAddDateRange').show();
                                }

                                SEL.VehicleJourneyRate.DateRange.HideModal();

                                break;
                        }

                        SEL.Ajax.Service("/shared/webServices/svcVehicleJourneyRate.asmx", "DateRangeGrid",
                        {
                            "contextKey": SEL.VehicleJourneyRate.CurrentMileageId
                        }, function (d) {
                            var data = d.d;
                            if ($e(dateRangeGrid) === true) {
                                $g(dateRangeGrid).innerHTML = data[2];
                                SEL.Grid.updateGrid(data[1]);
                            }
                        });
                    }, SEL.VehicleJourneyRate.ShowError);
                },

                Delete: function (dateRangeId) {
                    if (confirm('Are you sure you wish to delete the selected date range?')) {
                        SEL.Ajax.Service("/shared/webServices/svcVehicleJourneyRate.asmx", "DateRangeDelete",
                        {
                            "mileageId": SEL.VehicleJourneyRate.CurrentMileageId,
                            "mileageDateId": dateRangeId
                        }, function (d) {
                            if (d.d == 3) {
                                $('#lnkAddDateRange').hide();
                            }
                            else {
                                $('#lnkAddDateRange').show();
                            }

                            SEL.Grid.deleteGridRow('gridDateRanges', dateRangeId);
                        }, SEL.VehicleJourneyRate.ShowError);
                    }
                },

                Threshold:
                {
                    cmbRangeType_onchange: function () {
                        var rangetype = document.getElementById(cmbRangeType).options[document.getElementById(cmbRangeType).selectedIndex].value;
                        switch (rangetype) {
                            case '0':
                            case '2':
                                document.getElementById('spanThreshold1').style.display = '';
                                document.getElementById('spanThreshold2').style.display = 'none';
                                ValidatorEnable(document.getElementById(reqThreshold1), true);
                                ValidatorEnable(document.getElementById(reqThreshold2), false);
                                ValidatorEnable(document.getElementById(cmpThresholdVal1), true);
                                ValidatorEnable(document.getElementById(cmpThresholdVal2), false);
                                break;

                            case '1':
                                document.getElementById('spanThreshold1').style.display = '';
                                document.getElementById('spanThreshold2').style.display = '';
                                ValidatorEnable(document.getElementById(reqThreshold1), true);
                                ValidatorEnable(document.getElementById(reqThreshold2), true);
                                ValidatorEnable(document.getElementById(cmpThresholdVal1), true);
                                ValidatorEnable(document.getElementById(cmpThresholdVal2), true);
                                break;

                            case '3':
                                document.getElementById('spanThreshold1').style.display = 'none';
                                document.getElementById('spanThreshold2').style.display = 'none';
                                ValidatorEnable(document.getElementById(reqThreshold1), false);
                                ValidatorEnable(document.getElementById(reqThreshold2), false);
                                ValidatorEnable(document.getElementById(cmpThresholdVal1), false);
                                ValidatorEnable(document.getElementById(cmpThresholdVal2), false);
                                break;
                        }
                    },

                    PopulateGrid: function () {
                        SEL.Ajax.Service("/shared/webServices/svcVehicleJourneyRate.asmx", "ThresholdGrid",
                        {
                            "contextKey": SEL.VehicleJourneyRate.CurrentDateRangeId
                        }, function (d) {
                            var data = d.d;
                            if ($e(thresholdGrid) === true) {
                                $g(thresholdGrid).innerHTML = data[2];
                                SEL.Grid.updateGrid(data[1]);
                            }
                        });
                    },

                    ShowModal: function (save) {
                        SEL.VehicleJourneyRate.CurrentAction = 'addThreshold';
                        if (save) {
                            SEL.VehicleJourneyRate.CurrentThresholdType = null;
                            SEL.VehicleJourneyRate.DateRange.Save();
                            return;
                        }

                        SEL.Ajax.Service("/shared/webServices/svcVehicleJourneyRate.asmx", "ThresholdAvailableRangeTypes",
                        {
                            "mileageId": SEL.VehicleJourneyRate.CurrentMileageId,
                            "dateRangeId": SEL.VehicleJourneyRate.CurrentDateRangeId,
                            "thresholdType": SEL.VehicleJourneyRate.CurrentThresholdType
                        }, function (d) {
                            SEL.VehicleJourneyRate.SetList(d.d, SEL.VehicleJourneyRate.CurrentThresholdType, cmbRangeType);

                            SEL.VehicleJourneyRate.CurrentDateRangeType = document.getElementById(cmbDateRangeType).options[document.getElementById(cmbDateRangeType).selectedIndex].value;
                            SEL.VehicleJourneyRate.DateRange.Threshold.cmbRangeType_onchange();
                            $('#' + SEL.VehicleJourneyRate.CONTENT_MAIN + 'pnlthreshold').dialog("open");
                        }, SEL.VehicleJourneyRate.ShowError);
                    },

                    HideModal: function () {
                        $('#' + SEL.VehicleJourneyRate.CONTENT_MAIN + 'pnlthreshold').dialog("close");
                    },

                    Edit: function (id) {
                        SEL.Ajax.Service("/shared/webServices/svcVehicleJourneyRate.asmx", "ThresholdGet",
                        {
                            "thresholdId": id,
                            "dateRangeId": SEL.VehicleJourneyRate.CurrentDateRangeId,
                            "mileageId": SEL.VehicleJourneyRate.CurrentMileageId
                        }, function (d) {
                            var data = d.d;
                            SEL.VehicleJourneyRate.CurrentThresholdId = data.MileageThresholdId;
                            SEL.VehicleJourneyRate.CurrentThresholdType = data.RangeType;

                            $("#" + cmbRangeType).val(data.RangeType);
                            $("#" + txtThresholdVal1).val(data.RangeValue1);
                            $("#" + txtThresholdVal2).val(data.RangeValue2);
                            $("#" + txtPassenger).val(data.Passenger);
                            $("#" + txtPassengerx).val(data.PassengerX);
                            $("#" + txtHeavyBulky).val(data.HeavyBulkyEquipment);

                            SEL.VehicleJourneyRate.DateRange.Threshold.Rate.PopulateGrid(true);
                        }, SEL.VehicleJourneyRate.ShowError);
                    },

                    Save: function (deeper) {
                        if (validateform('vgThreshold') == false) {
                            return false;
                        }

                        var rangeType = new Number(document.getElementById(cmbRangeType).options[document.getElementById(cmbRangeType).selectedIndex].value);

                        var passenger = 0;
                        if (document.getElementById(txtPassenger).value != '') {
                            passenger = new Number(document.getElementById(txtPassenger).value);
                        }
                        var Passengerx = 0;
                        if (document.getElementById(txtPassengerx).value != '') {
                            Passengerx = new Number(document.getElementById(txtPassengerx).value);
                        }
                        var heavyBulky = 0;
                        if (document.getElementById(txtHeavyBulky).value != '') {
                            heavyBulky = new Number(document.getElementById(txtHeavyBulky).value);
                        }
                        var thresholdVal1 = null;
                        if (document.getElementById(txtThresholdVal1).value != '') {
                            thresholdVal1 = new Number(document.getElementById(txtThresholdVal1).value);
                        }
                        var thresholdVal2 = null;
                        if (document.getElementById(txtThresholdVal2).value != '') {
                            thresholdVal2 = new Number(document.getElementById(txtThresholdVal2).value);
                        }

                        if (rangeType != 3) {
                            SEL.VehicleJourneyRate.CurrentThresholdType = null;
                        }
                        else {
                            SEL.VehicleJourneyRate.CurrentThresholdType = rangeType;
                        }

                        SEL.Ajax.Service("/shared/webServices/svcVehicleJourneyRate.asmx", "ThresholdSave",
                        {
                            "thresholdId": SEL.VehicleJourneyRate.CurrentThresholdId,
                            "mileageId": SEL.VehicleJourneyRate.CurrentMileageId,
                            "dateRangeId": SEL.VehicleJourneyRate.CurrentDateRangeId,
                            "rangeType": rangeType,
                            "threshold1": thresholdVal1,
                            "threshold2": thresholdVal2,
                            "passenger": passenger,
                            "passengerx": Passengerx,
                            "heavyBulky": heavyBulky
                        }, function (d) {
                            var data = d.d;
                            if (data[1] != null && data[1] != "") {
                                SEL.MasterPopup.ShowMasterPopup(data[1], "Threshold Range Validation Failed");
                                return;
                            }

                            SEL.VehicleJourneyRate.DateRange.Threshold.PopulateGrid();
                            if (deeper) {
                                SEL.VehicleJourneyRate.CurrentThresholdId = data[0];
                            }
                            else {
                                SEL.VehicleJourneyRate.DateRange.Threshold.HideModal();
                            }
                        }, SEL.VehicleJourneyRate.ShowError);

                        return true;
                    },

                    Delete: function (thresholdId, dtRangeId) {
                        if (confirm('Are you sure you wish to delete the selected threshold?')) {
                            SEL.Ajax.Service("/shared/webServices/svcVehicleJourneyRate.asmx", "ThresholdDelete",
                            {
                                "mileageId": SEL.VehicleJourneyRate.CurrentMileageId,
                                "dateRangeId": dtRangeId,
                                "thresholdId": thresholdId
                            }, function (d) {
                                if (d.d == 3) {
                                    document.getElementById('lnkAddThreshold').style.display = 'none';
                                }
                                else {
                                    document.getElementById('lnkAddThreshold').style.display = '';
                                }

                                SEL.Grid.deleteGridRow('gridThresholds', thresholdId);
                            }, SEL.VehicleJourneyRate.ShowError);
                        }
                    },

                    Rate:
                    {
                        GRID_ID: 'gridRates',
                        HELP:
                        {
                            VEHICLE_ENGINE_TYPE: '2EA40129-E57B-4080-BCA7-DF1CC4124EB6',
                            RATE_PER_UNIT: 'FECCC7AC-F1D8-4182-8B94-26C041FCF2BC',
                            AMOUNT_FOR_VAT: '98A9BB51-8195-4619-BE79-BD4D20978A12'
                        },

                        CurrentId: null,

                        Edit: function (thresholdRateId) {
                            if (!SEL.VehicleJourneyRate.DateRange.Threshold.Save(true)) {
                                return;
                            }

                            SEL.VehicleJourneyRate.DateRange.Threshold.Rate.CurrentId = thresholdRateId;

                            var populate = function (data) {
                                var $vjrThresholdRate = data.d;

                                $("#" + SEL.VehicleJourneyRate.CONTENT_MAIN + "ddlVehicleEngineType").val($vjrThresholdRate.VehicleEngineTypeId);
                                $("#" + SEL.VehicleJourneyRate.CONTENT_MAIN + "txtRatePerUnit").val(SEL.Common.PreciseRound($vjrThresholdRate.RatePerUnit, 4));
                                $("#" + SEL.VehicleJourneyRate.CONTENT_MAIN + "txtAmountForVat").val(SEL.Common.PreciseRound($vjrThresholdRate.AmountForVat, 4));

                                $("#" + SEL.VehicleJourneyRate.CONTENT_MAIN + "pnlThresholdRate").dialog("open");
                            };

                            if (thresholdRateId == null) {
                                populate({
                                    "d":
                                    {
                                        "VehicleEngineTypeId": 0,
                                        "RatePerUnit": null,
                                        "AmountForVat": null
                                    }
                                });
                            }
                            else {
                                SEL.Ajax.Service("/shared/webServices/svcVehicleJourneyRate.asmx", "ThresholdRateGet", { "thresholdRateId": thresholdRateId }, populate,
                                    function (jqXhr, textStatus, errorThrown) {
                                        SEL.VehicleJourneyRate.ShowError("Error: " + textStatus + ", " + errorThrown);
                                    });
                            }
                        },

                        Save: function () {
                            $('#' + SEL.VehicleJourneyRate.CONTENT_MAIN + 'pnlThresholdRate .inputvalidatorfield').html('');

                            var ratePerUnit = $('#' + SEL.VehicleJourneyRate.CONTENT_MAIN + 'txtRatePerUnit').val();
                            if (ratePerUnit) {
                                if (!ratePerUnit.match(/^\d+(\.\d+)?$/)) {
                                    SEL.VehicleJourneyRate.ShowError('Please enter a valid Rate per mile/KM.', ['RatePerUnit']);
                                    return;
                                }
                            }

                            var amountForVat = $('#' + SEL.VehicleJourneyRate.CONTENT_MAIN + 'txtAmountForVat').val();
                            if (amountForVat) {
                                if (!amountForVat.match(/^\d+(\.\d+)?$/) || parseFloat(amountForVat) > parseFloat(ratePerUnit)) {
                                    SEL.VehicleJourneyRate.ShowError('Please enter a valid Amount for VAT.', ['AmountForVat']);
                                    return;
                                }
                            }

                            SEL.Ajax.Service("/shared/webServices/svcVehicleJourneyRate.asmx", "ThresholdRateSave", new SEL.VehicleJourneyRate.MileageThresholdRate(),
                                function (data) {
                                    var $response = data.d;
                                    if ($response.Success) {
                                        SEL.VehicleJourneyRate.DateRange.Threshold.Rate.PopulateGrid();
                                        SEL.VehicleJourneyRate.DateRange.Threshold.Rate.HideModal();
                                    }
                                    else {
                                        SEL.VehicleJourneyRate.ShowError($response.Message, $response.Controls);
                                    }
                                },
                                function (jqXhr, textStatus, errorThrown) {
                                    SEL.VehicleJourneyRate.ShowError("Error: " + textStatus + ", " + errorThrown);
                                });
                        },

                        Delete: function (mileageThresholdRateId) {
                            if (!confirm("Are you sure you wish to delete the selected Fuel rate?")) {
                                return;
                            }

                            SEL.Ajax.Service("/shared/webServices/svcVehicleJourneyRate.asmx", "ThresholdRateDelete", { "mileageThresholdRateId": mileageThresholdRateId },
                                function (data) {
                                    var $response = data.d;
                                    if ($response.Success) {
                                        SEL.VehicleJourneyRate.DateRange.Threshold.Rate.PopulateGrid();
                                    }
                                    else {
                                        SEL.VehicleJourneyRate.ShowError($response.Message);
                                    }
                                },
                                function (jqXhr, textStatus, errorThrown) {
                                    SEL.VehicleJourneyRate.ShowError("Error: " + textStatus + ", " + errorThrown);
                                });
                        },

                        PopulateGrid: function (showThresholdModal) {
                            SEL.Ajax.Service("/shared/webServices/svcVehicleJourneyRate.asmx", "ThresholdRateGrid", { "gridId": SEL.VehicleJourneyRate.DateRange.Threshold.Rate.GRID_ID, "thresholdId": SEL.VehicleJourneyRate.CurrentThresholdId },
                                function (gridData) {
                                    if ($e(gridData.d[0]) === true) {
                                        $g(gridData.d[0]).innerHTML = gridData.d[2];
                                        SEL.Grid.updateGrid(gridData.d[1]);
                                    }

                                    if (showThresholdModal) {
                                        SEL.VehicleJourneyRate.DateRange.Threshold.ShowModal(false);
                                    }
                                });
                        },

                        HideModal: function () {
                            $("#" + SEL.VehicleJourneyRate.CONTENT_MAIN + "pnlThresholdRate").dialog("close");
                        }
                    }
                }
            },

            ShowError: function (message, invalidControls) {
                if (message["_message"] != null) {
                    message = message["_message"];
                }

                SEL.MasterPopup.ShowMasterPopup((message || "An unknown error occurred.\nPlease try again.").replace(/\n/g, "<br />"), 'Message from ' + moduleNameHTML);
                if (invalidControls != null) {
                    for (var i = 0; i <= invalidControls.length; i++) {
                        $('#' + SEL.VehicleJourneyRate.CONTENT_MAIN + 'ddl' + invalidControls[i] + ", " +
                                '#' + SEL.VehicleJourneyRate.CONTENT_MAIN + 'txt' + invalidControls[i]).parent().nextAll('.inputvalidatorfield').first()
                            .append($('<span/>').addClass('invalid').text('*'));
                    }
                }
            },

            MileageThresholdRate: function () {
                this.thresholdRate =
                {
                    MileageThresholdRateId: SEL.VehicleJourneyRate.DateRange.Threshold.Rate.CurrentId,
                    MileageThresholdId: SEL.VehicleJourneyRate.CurrentThresholdId || null,
                    VehicleEngineTypeId: $('#' + SEL.VehicleJourneyRate.CONTENT_MAIN + 'ddlVehicleEngineType').val() || null,
                    RatePerUnit: $('#' + SEL.VehicleJourneyRate.CONTENT_MAIN + 'txtRatePerUnit').val() || null,
                    AmountForVat: $('#' + SEL.VehicleJourneyRate.CONTENT_MAIN + 'txtAmountForVat').val() || null
                }
            }

        };
    }

    if (window.Sys && Sys.loader) {
        Sys.loader.registerScript(scriptName, null, execute);
    }
    else {
        execute();
    }
})();

$(function () {
    var $panels = $('#' + SEL.VehicleJourneyRate.CONTENT_MAIN + 'pnlDateRange,' +
        '#' + SEL.VehicleJourneyRate.CONTENT_MAIN + 'pnlthreshold,' +
        '#' + SEL.VehicleJourneyRate.CONTENT_MAIN + 'pnlThresholdRate');
    $panels.dialog(
    {
        autoOpen: false,
        modal: true,
        resizable: false
    });
    $panels.parent().find('.ui-dialog-titlebar').remove();
    $panels.find('.formpanel').css('width', 'auto');

    $('#' + SEL.VehicleJourneyRate.CONTENT_MAIN + 'pnlDateRange').dialog("option", "width", 900);
    $('#' + SEL.VehicleJourneyRate.CONTENT_MAIN + 'pnlthreshold').dialog("option", {
        "width": 900,
        "close": function () {
            $('#' + SEL.VehicleJourneyRate.CONTENT_MAIN + 'cmbDateRangeType').focus();
        }
    });
    $('#' + SEL.VehicleJourneyRate.CONTENT_MAIN + 'pnlThresholdRate').dialog("option", {
        "width": 500,
        "close": function () {
            $('#' + SEL.VehicleJourneyRate.CONTENT_MAIN + 'cmbRangeType').focus();
        }
    });

    $('.dateField')
        .datepicker(
        {
            changeMonth: true,
            changeYear: true,
            dateFormat: "dd/mm/yy",
            firstDay: "1", // Monday
            showOtherMonths: true,
            selectOtherMonths: true
        })
        .attr('maxlength', 10);

    $('.4dp')
        .blur(function () {
            var $this = $(this);
            $this.val(SEL.Common.PreciseRound($this.val(), 4));
        });

});
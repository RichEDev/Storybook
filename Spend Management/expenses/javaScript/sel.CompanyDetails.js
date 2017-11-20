(function () {
    var scriptName = "companyDetails";
    function execute() {
        SEL.registerNamespace("SEL.CompanyDetails");
        SEL.CompanyDetails = {
            DOMIDs: {
                FinancialYearModal: null,
                FinancialYearDescription: null,
                FinancialYearStart: null,
                FinancialYearEnd: null,
                FinancialYearActive: null,
                FinancialYearPrimary: null
            },
            FinancialYearID: null,
            Year:
                {
                    Show: function () {
                        SEL.Common.ShowModal(SEL.CompanyDetails.DOMIDs.FinancialYearModal);
                    },

                    Hide: function () {
                        SEL.Common.HideModal(SEL.CompanyDetails.DOMIDs.FinancialYearModal);
                    },

                    New: function () {
                        SEL.CompanyDetails.Year.Clear();
                        SEL.CompanyDetails.Year.Show();
                    },
                    Edit: function (id) {
                        SEL.CompanyDetails.FinancialYearID = id;
                        var params = new SEL.CompanyDetails.Misc.WebserviceParameters.YearId();
                        SEL.Ajax.Service('/shared/webServices/svcFinancialYears.asmx/', 'Get', params, SEL.CompanyDetails.Year.GetComplete, SEL.CompanyDetails.Misc.ErrorHandler);

                    },
                    GetComplete: function (data) {
                        $g(SEL.CompanyDetails.DOMIDs.FinancialYearDescription).value = data.d.Description;
                        $g(SEL.CompanyDetails.DOMIDs.FinancialYearStart).value = data.d.YearStartMonthDay;
                        $g(SEL.CompanyDetails.DOMIDs.FinancialYearEnd).value = data.d.YearEndMonthDay;
                        $g(SEL.CompanyDetails.DOMIDs.FinancialYearActive).checked = data.d.Active;
                        $g(SEL.CompanyDetails.DOMIDs.FinancialYearPrimary).checked = data.d.Primary;
                        SEL.CompanyDetails.Year.Show();
                    },
                    Clear: function () {
                        SEL.CompanyDetails.FinancialYearID = 0;
                        $g(SEL.CompanyDetails.DOMIDs.FinancialYearDescription).value = '';
                        $g(SEL.CompanyDetails.DOMIDs.FinancialYearStart).value = '';
                        $g(SEL.CompanyDetails.DOMIDs.FinancialYearEnd).value = '';
                        $g(SEL.CompanyDetails.DOMIDs.FinancialYearActive).checked = true;
                        $g(SEL.CompanyDetails.DOMIDs.FinancialYearPrimary).checked = false;
                    },
                    Save: function () {
                        var params = new SEL.CompanyDetails.Misc.WebserviceParameters.SaveYear();
                        SEL.Ajax.Service('/shared/webServices/svcFinancialYears.asmx/', 'Save', params, SEL.CompanyDetails.Year.SaveComplete, SEL.CompanyDetails.Misc.ErrorHandler);
                    },
                    SaveComplete: function () {
                        SEL.CompanyDetails.Year.Clear();
                        SEL.CompanyDetails.Year.Hide();
                        SEL.CompanyDetails.Year.RefreshGrid();
                    },
                    Delete: function (id) {
                        var params = new SEL.CompanyDetails.Misc.WebserviceParameters.YearId(id);
                        SEL.Ajax.Service('/shared/webServices/svcFinancialYears.asmx/', 'Delete', params, SEL.CompanyDetails.Year.DeleteComplete, SEL.CompanyDetails.Misc.ErrorHandler);

                    },
                    DeleteComplete: function (data) {
                        switch (data.d) {
                            case -1:
                                //in use
                                SEL.MasterPopup.ShowMasterPopup("Cannot delete Year as it is associated to a Vehicle Journey Rate.", 'Message from ' + moduleNameHTML);
                                break;
                            case -2:
                                // primary
                                SEL.MasterPopup.ShowMasterPopup("Cannot delete Year as it is currently the primary Year for this company.", 'Message from ' + moduleNameHTML);
                                break;
                            case -4:
                                // 
                                SEL.MasterPopup.ShowMasterPopup("Cannot delete Year as it is being used on one or more flag rules.", 'Message from ' + moduleNameHTML);
                                break;
                            default:

                        }

                        SEL.CompanyDetails.Year.RefreshGrid();
                    },
                    Cancel: function () {
                        SEL.CompanyDetails.Year.Clear();
                        SEL.CompanyDetails.Year.Hide();
                    },
                    RefreshGrid: function () {
                        SEL.Grid.refreshGrid('gridFinancialYears', SEL.Grid.getCurrentPageNum('gridFinancialYears'));
                    }
                },
            SetupDateFields: function () {
                // Create the date picker controls
                $('.dateField').datepicker({
                    changeMonth: true,
                    changeYear: false,
                    showButtonPanel: true,
                    dateFormat: 'dd/mm'
                }).attr('maxlength', 5);

                // Setup the focus events for date fields
                $('.dateField').focus(function () {
                    // Remove the 'focus token' from the previous control
                    $('.hasCalControl').removeClass('hasCalControl');

                    // Give the 'focus token' to the new control        
                    $(this).addClass('hasCalControl');
                    SEL.CompanyDetails.ValidateDates($(this).attr('id'), $(this).val());
                });

                // Setup the blur events for date fields
                $('.dateField').blur(function () {
                    var dateValue = $(this).val();
                    var currentControlID = $(this).attr('id');
                    if ($.isNumeric(dateValue) && dateValue.length === 4) {
                        var newDateValue = dateValue.substring(0, 2) + "/";

                        newDateValue += dateValue.substring(2, 4) + "/";

                        $(this).val(newDateValue);

                        // Refresh validators if the field has been updated
                        $(this).parent().nextAll('.inputvalidatorfield').first().children().each(function () {
                            var val = $g($(this).attr('id'));
                            ValidatorValidate(val);
                        });
                    }
                    else {
                        $(this).val(dateValue.replace(/\./g, '/').replace(/\-/g, '/'));
                    }

                    dateValue = $(this).val();

                    SEL.CompanyDetails.ValidateDates($(this).attr('id'), $(this).val());
                });

                $('.dateField').change(function () {
                    SEL.CompanyDetails.ValidateDates($(this).attr('id'), $(this).val());
                });

                // Setup the click events for calendar images
                $('.dateCalImg, .timeCalImg').click(function () {
                    var inputControl = $(this).parent().prev().children().first();

                    if (inputControl.is(':disabled')) return false;

                    if (inputControl.hasClass('hasCalControl')) {
                        var pickerDiv = $(this).hasClass('dateCalImg') ? '#ui-datepicker-div' : '#ui-timepicker-div';

                        if ($(pickerDiv).css('display') === 'none') {
                            inputControl.focus();
                        }
                        else {
                            if ($(pickerDiv).is(':animated') === false) {
                                $(pickerDiv).fadeOut(100);
                            }
                        }
                    }
                    else {
                        inputControl.focus();
                    }

                    SEL.CompanyDetails.ValidateDates(inputControl.attr('id'), inputControl.val());
                });
            },
            ValidateDates: function (currentControlID, dateValue) {
                var separator = dateValue.indexOf("/");
                if (separator !== -1) {
                    var dayPart = parseInt(dateValue.substring(0, separator), 10);
                    var monthPart = parseInt(dateValue.substring(separator + 1), 10);
                    var currentDate = new Date(1900, monthPart - 1, dayPart);
                    if (currentControlID === SEL.CompanyDetails.DOMIDs.FinancialYearStart) {
                        currentDate.setDate(currentDate.getDate() - 1);
                        $('#' + SEL.CompanyDetails.DOMIDs.FinancialYearEnd).val(SEL.CompanyDetails.DayMonthFromDate(currentDate));

                    } else {
                        currentDate.setDate(currentDate.getDate() + 1);
                        $('#' + SEL.CompanyDetails.DOMIDs.FinancialYearStart).val(SEL.CompanyDetails.DayMonthFromDate(currentDate));
                    }
                }
            },
            DayMonthFromDate: function (dateValue) {
                var result = '';
                result = SEL.CompanyDetails.PadDate(dateValue.getDate()) + "/" + SEL.CompanyDetails.PadDate((dateValue.getMonth() + 1));
                return result;
            },
            PadDate: function (str) {
                while (str.length < 2)
                    str = '0' + str;
                return str;
            },
            Misc:
                {
                    WebserviceParameters:
                        {
                            SaveYear: function () {
                                this.financialYearID = SEL.CompanyDetails.FinancialYearID;
                                this.description = $g(SEL.CompanyDetails.DOMIDs.FinancialYearDescription).value;
                                this.yearStart = $g(SEL.CompanyDetails.DOMIDs.FinancialYearStart).value;
                                this.yearEnd = $g(SEL.CompanyDetails.DOMIDs.FinancialYearEnd).value;
                                this.active = $g(SEL.CompanyDetails.DOMIDs.FinancialYearActive).checked;
                                this.primary = $g(SEL.CompanyDetails.DOMIDs.FinancialYearPrimary).checked;
                            },
                            YearId: function (id) {
                                if (id === undefined) {
                                    this.id = SEL.CompanyDetails.FinancialYearID;
                                } else {
                                    this.id = id;
                                }

                            }
                        },
                    ErrorHandler: function (data) {
                        SEL.Common.WebService.ErrorHandler(data);
                    }
                }
        };
    }
    if (window.Sys && window.Sys.loader) {
        window.Sys.loader.registerScript(scriptName, null, execute);
    }
    else {
        execute();
    }
}(SEL, moduleNameHTML, appPath));


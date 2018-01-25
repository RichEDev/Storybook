(function ($, win, ValidatorUpdateDisplay, ValidatorEnable) {
    var filterModal = {
        init: function (options) {
            this.options = options;
        }
    };
    var methods = {
        Ids: {
            FilterDataType: null,
            FilterConditionType: null,
            List: null,
            Index: 0,
            CurrentId: null,
            EditMode: null,
            EditControl: null
        },
        Messages: {
            EmptyTree: 'There are no {0} selected.',
            NoneOption: '[None]',
            Filters:
            {
                NoFilterCriteriaSelected: 'Please select a Filter criteria.',
                NoFilterColumnSelected: 'Please select a column to apply a filter to.',
                NoListSelection: 'Please select an item from the list.',
                Title: 'Filter',
                Yes: 'Yes',
                No: 'No',
                Labels:
                {
                    DateTime: 'Date and time',
                    Date: 'Date',
                    Time: 'Time',
                    Number: 'Number',
                    Amount: 'Currency amount',
                    Value: 'Value',
                    YesNo: 'Yes or No',
                    Days: 'Number of days',
                    Months: 'Number of months',
                    Weeks: 'Number of weeks',
                    Years: 'Number of years'
                },
                ValidatorParts:
                {
                    PleaseEnter: 'Please enter',
                    PleaseSelect: 'Please select',
                    Date: 'date',
                    Time: 'time (hh:mm)',
                    DateTime: 'date and time',
                    Value: 'value',
                    Integer: 'whole number',
                    Currency: 'amount',
                    Decimal: 'number',
                    YesNo: 'Yes or No',
                    A: 'a',
                    An: 'an',
                    And: 'and',
                    Valid: 'valid',
                    GreaterThan: 'greater than',
                    For: 'for',
                    Between: 'between',
                    IntMax: '2,147,483,647',
                    IntMin: '-2,147,483,647',
                    XMin: '1',
                    XMax: '9999'
                }
            }
        },
        FilterModal:
        {
            FilterEditMode: null,
            FilterSearch: function (list, txt) {
                return list.filter(function (item) {
                    return item.Text.toLowerCase().startsWith(txt.toLowerCase());
                });
            },
            SetUpSearchTextBox: function () {
                var filteredList = [], domIDs = $.filterModal.options.domainRoot.DomIDs.Filters.FilterModal, filterModal = $.filterModal.Filters.FilterModal;
               if ($(".filter-search").length === 0) {
                   $('#' + domIDs.CriteriaListDropDown).before("<input type='text' class='filter-search' maxlength='20' value=' Search..' style = 'display: none;'>");
               };
                $('.filter-search').focus(function() {
                    if (this.value === " Search..")
                    { this.value = ''; }
                });

                $('.filter-search').blur(function () {
                    if (this.value === '') { this.value = ' Search..'; }
                });

                $('.filter-search').unbind('propertychange input').on('propertychange input', function () {
                    var searchText = $('.filter-search').val().trim();
                    if (searchText.length > 2) {
                        filteredList = filterModal.FilterSearch(filterModal.List, searchText);
                        filterModal.SetUpMultiSelectControl(filteredList);
                    }
                    else if (searchText.length === 0) {
                        filterModal.SetUpMultiSelectControl(filterModal.List);
                    }
                });
            },
            GetSelectedItem: function (list, val) {
                return list.filter(function (item) {
                    return item.Value === val;
                });
            },
            PopulateDropDown: function (dd, l, sel, useNone) {
                dd.options.length = 0;
                var hasSelected = 0,
                    t,
                    o;

                if (useNone) {
                    o = document.createElement('option');
                    o.value = '';
                    o.text = $.filterModal.Filters.Messages.NoneOption;
                    if (sel === undefined || sel === null) {
                        o.selected = true;
                    }
                    dd.options[0] = o;
                }
                
                var length = 100;
                if (l.length < length) {
                    length = l.length;
                }
                if (l !== null && l !== undefined) {
                    for (t = 0; t < length; t = t + 1) {
                        o = document.createElement("option");
                        o.value = l[t].Value;
                        o.text = l[t].Text;
                        if (hasSelected === 0 &&
                        ((sel !== undefined && sel !== null && sel === l[t].Value) ||
                            l[t].Selected === 'selected' ||
                            l[t].Selected === true)) {
                            o.selected = true;
                            hasSelected = 1;
                        } 
                        try {
                                // for IE earlier than version 8
                           dd.add(o, dd.options[null]);
                        } catch (e) {
                           dd.add(o, null);
                        }
                    }
                }
                return;
            },

            ChangeFilterCriteria: function (init, fromEnterAtRuntime) {
                var thisDomIds = $.filterModal.options.domainRoot.DomIDs.Filters.FilterModal,
                    thisIds = $.filterModal.Filters.Ids,
                    thisNs = $.filterModal.Filters.FilterModal,
                    localizer = $.filterModal.Filters.Messages.Filters,
                    row1 = $('#' + thisDomIds.CriteriaRow1),
                    row2 = $('#' + thisDomIds.CriteriaRow2),
                    criteriacontrol1 = $('#' + thisDomIds.Criteria1),
                    criteriacontrol2 = $('#' + thisDomIds.Criteria2),
                    timecontrol1 = $('#' + thisDomIds.Criteria1Time),
                    timecontrol2 = $('#' + thisDomIds.Criteria2Time),
                    calc1 = $('#' + thisDomIds.Criteria1ImageCalc),
                    calc2 = $('#' + thisDomIds.Criteria2ImageCalc),
                    criteriaspacer1 = $('#' + thisDomIds.Criteria1Spacer),
                    criteriaspacer2 = $('#' + thisDomIds.Criteria2Spacer),
                    label1 = $('#' + thisDomIds.Criteria1Label),
                    label2 = $('#' + thisDomIds.Criteria2Label),
                    hideListSelector,
                    yesnolist,
                    enterAtRuntime = false;

                if (!init) {
                    thisNs.ClearForm(fromEnterAtRuntime);
                }

                label1.text('{0} 1*'.format(localizer.Labels.Value));
                label2.text('{0} 2*');
                row1.css('display', 'none');
                row2.css('display', 'none');
                criteriaspacer1.css('display', '');
                criteriaspacer2.css('display', '');
                calc1.css('display', 'none');
                calc2.css('display', 'none');
                timecontrol1.css('display', 'none');
                timecontrol2.css('display', 'none');
                criteriacontrol1.css('display', '');
                $g(thisDomIds.Criteria1DropDown).style.display = 'none';
                criteriacontrol1.attr('maxlength', 150);
                criteriacontrol2.attr('maxlength', 150);
                criteriacontrol1.css('width', '160px');
                criteriacontrol2.css('width', '160px');
                timecontrol1.css('width', '40px');
                timecontrol2.css('width', '40px');
                criteriacontrol2.css('display', '');
                $('#' + thisDomIds.Criteria1).datepicker('destroy');
                $('#' + thisDomIds.Criteria2).datepicker('destroy');

                if (init !== true) {
                    thisIds.FilterConditionType = $('#' + thisDomIds.FilterDropDown).val().toString();
                } else {
                    $('#' + thisDomIds.FilterDropDown).val(thisIds.FilterConditionType);
                }

                hideListSelector = true;

                var runtimeRow = null;
                if ($.filterModal.options.domainRoot.DomIDs.Filters.FilterModal.Runtime !== null) {
                    runtimeRow = $('#' + $.filterModal.options.domainRoot.DomIDs.Filters.FilterModal.RuntimeRow);
                    runtimeRow.show();
                    enterAtRuntime = $g($.filterModal.options.domainRoot.DomIDs.Filters.FilterModal.Runtime).checked;
                    switch (thisIds.FilterConditionType) {
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '37':
                        case '38':
                        case '39':
                        case '40':
                        case '41':
                        case '42':
                        case '46':
                        case '8':
                        case '28':
                        case '29':
                        case '30':
                        case '31':
                        case '32':
                        case '33':
                        case '34':
                        case '35':
                        case '253':
                            break;
                        default:
                            runtimeRow.hide();
                            break;
                    }
                }

                if (!enterAtRuntime) {
                    switch (thisIds.FilterConditionType) {
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '37':
                        case '38':
                        case '39':
                        case '40':
                        case '41':
                        case '42':
                        case '46':
                        case '253':
                            if (thisIds.FilterDataType === "L" || thisIds.FilterDataType === "GL" || thisIds.FilterDataType === "CL") {
                                hideListSelector = false;

                                if ($('div:animated').length !== 0) {
                                    setTimeout(function () { thisNs.ToggleListSelectorVisibility(true); }, 400);
                                } else {
                                    thisNs.ToggleListSelectorVisibility(true);
                                }
                            } else {
                                row1.css('display', '');
                            }
                            break;
                        case '8':
                            row1.css('display', '');
                            row2.css('display', '');
                            if (thisIds.FilterDataType === 'T') {
                                timecontrol2.css('display', '');
                                criteriacontrol2.css('display', 'none');
                            }
                            if (thisIds.FilterDataType === 'DT') {
                                timecontrol2.css('display', '');
                                criteriacontrol2.css('display', '');
                            }
                            break;
                        case '28':
                        case '29':
                        case '30':
                        case '31':
                        case '32':
                        case '33':
                        case '34':
                        case '35':
                            row1.css('display', '');
                            row2.css('display', 'none');
                            break;
                        default:
                            break;
                    }

                    switch (thisIds.FilterDataType) {
                        case 'DT':
                            if (thisIds.FilterConditionType < 28 || thisIds.FilterConditionType > 35) {
                                label1.text('{0} 1*'.format(localizer.Labels.DateTime));
                                label2.text('{0} 2*'.format(localizer.Labels.DateTime));
                                timecontrol1.css('display', '');
                                thisNs.CreateDateControls();
                                label2.unbind('click').click(function () { thisNs.ToggleCalendar(2); });
                            } else {
                                thisNs.AdjustLabelsForFilterX(thisIds.FilterConditionType, label1, label2);
                                criteriacontrol1.css('width', '30px');
                                criteriacontrol1.attr('maxlength', 4);
                            }
                            break;
                        case 'D':
                            if (thisIds.FilterConditionType < 28 || thisIds.FilterConditionType > 35) {
                                label1.text('{0} 1*'.format(localizer.Labels.Date));
                                label2.text('{0} 2*'.format(localizer.Labels.Date));
                                thisNs.CreateDateControls();
                                label2.unbind('click').click(function () { thisNs.ToggleCalendar(2); });
                            } else {
                                thisNs.AdjustLabelsForFilterX(thisIds.FilterConditionType, label1, label2);
                                criteriacontrol1.css('width', '30px');
                                criteriacontrol1.attr('maxlength', 4);
                            }
                            break;
                        case 'T':
                            label1.text('{0} 1*'.format(localizer.Labels.Time));
                            label2.text('{0} 2*'.format(localizer.Labels.Time));
                            criteriaspacer1.css('display', 'none');
                            criteriaspacer2.css('display', 'none');
                            criteriacontrol1.css('display', 'none');
                            timecontrol1.css('display', '');
                            label2.unbind('click').click(function () { thisNs.ToggleCalendar(2); });
                            break;
                        case 'N':
                            label1.text('{0} 1*'.format(localizer.Labels.Number));
                            label2.text('{0} 2*'.format(localizer.Labels.Number));
                            criteriacontrol1.attr('maxlength', 14);
                            criteriacontrol2.attr('maxlength', 14);
                            label2.unbind('click').click(function () { criteriacontrol2.select(); });
                            break;
                        case 'M':
                        case 'FD':
                            label1.text('{0} 1*'.format(localizer.Labels.Number));
                            label2.text('{0} 2*'.format(localizer.Labels.Number));
                            criteriacontrol1.attr('maxlength', 17);
                            criteriacontrol2.attr('maxlength', 17);
                            thisNs.SetupDecimalControl(criteriacontrol1, thisIds.FilterPrecision);
                            thisNs.SetupDecimalControl(criteriacontrol2, thisIds.FilterPrecision);
                            label2.unbind('click').click(function () { criteriacontrol2.select(); });
                            break;
                        case 'C':
                            label1.text('{0} 1*'.format(localizer.Labels.Amount));
                            label2.text('{0} 2*'.format(localizer.Labels.Amount));
                            criteriacontrol1.attr('maxlength', 20);
                            criteriacontrol2.attr('maxlength', 20);
                            label2.unbind('click').click(function () { criteriacontrol2.select(); });
                            break;
                        case 'X':
                            label1.text('{0}*'.format(localizer.Labels.YesNo));
                            yesnolist = [];
                            yesnolist.push(thisNs.AddDropDownOption(localizer.Yes, '1'));
                            yesnolist.push(thisNs.AddDropDownOption(localizer.No, '2'));
                            criteriaspacer1.css('display', 'none');
                            criteriacontrol1.css('display', 'none');
                            criteriacontrol1 = $g(thisDomIds.Criteria1DropDown);
                            $('#' + thisDomIds.Criteria1DropDown).unbind("change").bind("change", function () {
                                if ($(this).val() !== '') {
                                    SEL.Common.Page_ClientValidateReset();
                                }
                            });
                            criteriacontrol1.style.display = '';
                            thisNs.PopulateDropDown(criteriacontrol1, yesnolist, null, true);
                            break;
                        case 'CL':
                        case 'GL':
                        case 'L':
                            criteriacontrol1.css('display', 'none');
                            criteriacontrol1 = $g(thisDomIds.Criteria1DropDown);
                            criteriacontrol1.style.display = '';
                            break;
                        default:
                            break;
                    }
                }

                if (hideListSelector) {
                    if ($('div:animated').length !== 0) {
                        setTimeout(function () { thisNs.ToggleListSelectorVisibility(false); }, 400);
                    } else {
                        thisNs.ToggleListSelectorVisibility(false);
                    }
                }

                if (!init) {
                    thisNs.UpdateDropdownValidatorIfNoneSelected(thisDomIds.FilterDropDown, thisDomIds.FilterRequiredValidator);
                }
            },

            ToggleListSelectorVisibility: function (show) {
                if ($('div:animated').length !== 0) {
                    $('div:animated').stop(true, true);
                }

                var thisNs = $.filterModal.Filters.FilterModal,
                    row3 = $('#' + $.filterModal.options.domainRoot.DomIDs.Filters.FilterModal.CriteriaListRow),
                    filterModalPanel = $('#' + $.filterModal.options.domainRoot.DomIDs.Filters.Panel);

                if (show === true) {
                    if (row3.css('display') === 'none') {
                        if (filterModalPanel.offset().top > 0) {
                            filterModalPanel.animate({ 'top': '-=150px', 'left': '-=90px', 'width': '593px' }, 200);

                            $('.ui-multiselect.ui-helper-clearfix.ui-widget').css('display', '');
                            row3.slideDown(200, function () { thisNs.SetupListHelp(false); });
                        } else {
                            filterModalPanel.css('width', '593px');
                            row3.css('display', '');
                            $('.ui-multiselect.ui-helper-clearfix.ui-widget').css('display', '');
                            thisNs.SetupListHelp(false);
                        }
                    }
                } else {
                    if (row3.css('display') !== 'none') {
                        if (filterModalPanel.offset().top > 0) {
                            filterModalPanel.animate({ 'top': '+=150px', 'left': '+=90px', 'width': '410px', 'height': '115px' }, 200, function () {
                                row3.css('display', 'none');
                                filterModalPanel.css('height', '');
                            });
                        } else {
                            filterModalPanel.css('width', '410px');
                            row3.css('display', 'none');
                            $('.ui-multiselect.ui-helper-clearfix.ui-widget').css('display', 'none');
                        }
                    }

                    $('#imgFilterModalHelp').css('display', 'none');
                }
            },

            AdjustLabelsForFilterX: function (conditiontype, label1, label2) {
                var localizer = $.filterModal.Filters.Messages.Filters.Labels;
                switch (conditiontype) {
                    case '28':
                    case '29':
                        label1.text('{0}*'.format(localizer.Days));
                        label2.text('');
                        break;
                    case '30':
                    case '31':
                        label1.text('{0}*'.format(localizer.Weeks));
                        label2.text('');
                        break;
                    case '32':
                    case '33':
                        label1.text('{0}*'.format(localizer.Months));
                        label2.text('');
                        break;
                    case '34':
                    case '35':
                        label1.text('{0}*'.format(localizer.Years));
                        label2.text('');
                        break;
                }
            },

            SetupDecimalControl: function (control, precision) {
                control.unbind('keyup.decimal').bind('keyup.decimal', function () {
                    var pointpos = control.attr('maxlength') - precision;
                    if (control.val().length >= pointpos) {
                        if (control.val().charAt(pointpos - 1) !== '.' && control.val().indexOf('.') === -1) {
                            control.val(control.val().slice(0, pointpos - 1) + '.' + control.val().slice(pointpos - 1));
                        }
                    }
                });
            },

            SetUpMultiSelectControl: function (filteredList) {
               
                var thisDomIds = $.filterModal.options.domainRoot.DomIDs.Filters.FilterModal,
                    thisIds = $.filterModal.Filters.Ids,
                    thisNs = $.filterModal.Filters.FilterModal,
                    selNode,
                    list,
                    ddList,
                    option,
                    container;

                if (thisIds.FilterDataType === 'GL' || thisIds.FilterDataType === 'L' || thisIds.FilterDataType === 'CL') {
                    if (filteredList == undefined) {
                        thisNs.PopulateDropDown($g(thisDomIds.CriteriaListDropDown), thisNs.List);
                    } else {
                        thisNs.PopulateDropDown($g(thisDomIds.CriteriaListDropDown), filteredList);
                    }
                    if (thisIds.FilterNode === null || thisIds.FilterNode === undefined) {
                        selNode = $.data(document, 'filter'+$.filterModal.Filters.Ids.CurrentId);
                    } else {
                        selNode = $('#' + thisIds.FilterNode).data();
                    }

                    var selectedItems = $('#' + thisDomIds.CriteriaListRow).find('div.selected:first').find('li:not(.ui-helper-hidden-accessible)');
                    if ((selNode && selNode.criterionOne) || selectedItems.length > 0) {
                        list = selectedItems.length > 0 ? selectedItems :  selNode.criterionOne.split(',');
                        ddList = $('#' + thisDomIds.CriteriaListDropDown);
                        for (var i = 0; i < list.length; i = i + 1) {
                            var value = selectedItems.length > 0 ? list[i].value : list[i];
                            option = ddList.find('option[value="' + value + '"]');
                            if (option.length > 0) {
                                option.attr('selected', 'selected');
                            } else {
                                var newoption = [];
                                if (selectedItems.length === 0)
                                {
                                    newoption = thisNs.GetSelectedItem(thisNs.List, list[i]);
                                }
                                option = document.createElement("option");
                                option.value = selectedItems.length > 0 ? list[i].value : newoption[0].Value;
                                option.text = selectedItems.length > 0 ? list[i].title : newoption[0].Text;
                                option.selected = true;
                                $g(thisDomIds.CriteriaListDropDown).add(option, $g(thisDomIds.CriteriaListDropDown).options[null]);
                            }
                        }
                    }

                    if ($('.ui-multiselect').length > 0) {
                        $('#' + thisDomIds.CriteriaListDropDown).multiselect("destroy");
                    }
                    $('#' + thisDomIds.CriteriaListDropDown).multiselect({ searchable: false });

                    container = $('.ui-multiselect.ui-helper-clearfix.ui-widget:first');
                    container.css('width', '591px');
                    container.children().each(function () {
                        $(this).css('width', '295px');
                        $(this).css('height', '300px');
                        $(this).find('ul:first').css('height', '270px');
                    });
                    $('.filter-search').show();
                }
            },

            AddDropDownOption: function (text, val) {
                return { Text: text, Value: val };
            },

            BuildValidators: function (type, label1, label2, useCompare, xDaysWeeksMonthsYearsType) {
                var thisDomIds = $.filterModal.options.domainRoot.DomIDs.Filters.FilterModal,
                    thisNs = $.filterModal.Filters.FilterModal,
                    rval1 = $g(thisDomIds.RequiredValidator1),
                    rval2 = $g(thisDomIds.RequiredValidator2),
                    dval1 = $g(thisDomIds.DataTypeValidator1),
                    dval2 = $g(thisDomIds.DataTypeValidator2),
                    cval = $g(thisDomIds.CompareValidator),
                    intVal1 = $g(thisDomIds.IntegerValidator1),
                    intVal2 = $g(thisDomIds.IntegerValidator2),
                    localizer = $.filterModal.Filters.Messages.Filters.ValidatorParts,
                    isStandard = true,
                    selection = '',
                    baseErrorMessage,
                    alterType = '',
                    baseMessage,
                    aOrAn,
                    min,
                    max;

                dval1.decimalchar = '.';
                dval2.decimalchar = '.';
                cval.decimalchar = '.';

                if (xDaysWeeksMonthsYearsType) {
                    min = localizer.XMin;
                    max = localizer.XMax;
                } else {
                    min = localizer.IntMin.replace(/,/g, '');
                    max = localizer.IntMax.replace(/,/g, '');
                }
                intVal1.minimumvalue = min;
                intVal1.maximumvalue = max;
                intVal2.minimumvalue = min;
                intVal2.maximumvalue = max;

                switch (type) {
                    case 'D':
                        selection = localizer.Date;
                        alterType = 'Date';
                        break;
                    case 'T':
                        selection = localizer.Time;
                        alterType = 'Date';
                        break;
                    case 'DT':
                        selection = localizer.DateTime;
                        alterType = 'Date';
                        break;
                    case 'N':
                        baseMessage = '{0} {1} {2} {3} {4} {5} {6} {7} {8}.'.format(localizer.PleaseEnter, localizer.A, localizer.Integer, localizer.Between, min, localizer.And, max, localizer.For).replace('{8}', '{0}');
                        intVal1.errormessage = baseMessage.format(label1);
                        intVal2.errormessage = baseMessage.format(label2);
                        selection = localizer.Integer;
                        alterType = 'Integer';
                        break;
                    case 'M':
                    case 'FD':
                        selection = localizer.Decimal;
                        alterType = 'Double';
                        break;
                    case 'C':
                        selection = localizer.Currency;
                        alterType = 'Double';
                        break;
                    case 'X':
                        isStandard = false;
                        break;
                    default:
                        selection = localizer.Value;
                        alterType = "String";
                }
                if (isStandard) {
                    aOrAn = selection === localizer.Currency ? localizer.An : localizer.A;
                    baseErrorMessage = '{0} {1} {2} {3} {4}.'.format(localizer.PleaseEnter, aOrAn, selection, localizer.For).replace('{4}', '{0}');
                    rval1.errormessage = baseErrorMessage.format(label1);
                    rval2.errormessage = baseErrorMessage.format(label2);

                    thisNs.AlterCompareValidator('DataTypeCheck', alterType, label1, dval1);
                    thisNs.AlterCompareValidator('DataTypeCheck', alterType, label2, dval2);
                    if (useCompare) {
                        thisNs.AlterCompareValidator('GreaterThan', alterType, label2, cval);
                    }
                }
            },

            AssignValidatorsOnSave: function (enterAtRuntime) {
                var thisDomIds = $.filterModal.options.domainRoot.DomIDs.Filters.FilterModal,
                    thisIds = $.filterModal.Filters.Ids,
                    thisNs = $.filterModal.Filters.FilterModal,
                    localizer = $.filterModal.Filters.Messages.Filters.ValidatorParts,
                    reqVal1 = $g(thisDomIds.RequiredValidator1),
                    reqVal2 = $g(thisDomIds.RequiredValidator2),
                    dataVal1 = $g(thisDomIds.DataTypeValidator1),
                    dataVal2 = $g(thisDomIds.DataTypeValidator2),
                    timeVal1 = $g(thisDomIds.TimeValidator1),
                    timeVal2 = $g(thisDomIds.TimeValidator2),
                    cmpVal = $g(thisDomIds.CompareValidator),
                    cmpDateAndTime = $g(thisDomIds.DateAndTimeCompareValidator),
                    cmpTimeRange = $g(thisDomIds.TimeRangeCompareValidator),
                    intVal1 = $g(thisDomIds.IntegerValidator1),
                    intVal2 = $g(thisDomIds.IntegerValidator2),
                    label1 = $('#' + thisDomIds.Criteria1Label).text().replace('*', ''),
                    label2 = $('#' + thisDomIds.Criteria2Label).text().replace('*', ''),
                    valspan1 = $('#' + thisDomIds.Criteria1ValidatorSpan),
                    valspan2 = $('#' + thisDomIds.Criteria2ValidatorSpan),
                    aOrAn,
                    criteria1,
                    criteria2;

                valspan1.attr('class', 'inputmultiplevalidatorfield');
                valspan2.attr('class', 'inputmultiplevalidatorfield');
                ValidatorEnable(reqVal1, false);
                ValidatorEnable(reqVal2, false);
                ValidatorEnable(dataVal1, false);
                ValidatorEnable(dataVal2, false);
                ValidatorEnable(cmpVal, false);
                ValidatorEnable(cmpDateAndTime, false);
                ValidatorEnable(timeVal1, false);
                ValidatorEnable(timeVal2, false);
                ValidatorEnable(cmpTimeRange, false);
                ValidatorEnable(intVal1, false);
                ValidatorEnable(intVal2, false);
                reqVal1.controltovalidate = thisDomIds.Criteria1;
                dataVal1.controltovalidate = thisDomIds.Criteria1;
                dataVal1.type = 'String';
                aOrAn = thisIds.FilterDataType === 'C' ? localizer.An : localizer.A;
                reqVal1.errormessage = '{0} {1} {2} {3} {4}.'.format(localizer.PleaseEnter, aOrAn, localizer.Value, localizer.For, label1);
                reqVal2.errormessage = '{0} {1} {2} {3} {4}.'.format(localizer.PleaseEnter, aOrAn, localizer.Value, localizer.For, label2);

                if (!enterAtRuntime) {
                    switch (thisIds.FilterDataType) {
                        case 'DT':
                        case 'D':
                            if (thisIds.FilterConditionType < 28 || thisIds.FilterConditionType > 35) {
                                thisNs.BuildValidators(thisIds.FilterDataType, label1, label2, true);
                            } else {
                                thisNs.BuildValidators('N', label1, label2, false, true);
                            }
                            break;
                        case 'T':
                        case 'N':
                        case 'FD':
                        case 'M':
                        case 'C':
                            thisNs.BuildValidators(thisIds.FilterDataType, label1, label2, true);
                            break;
                        case 'X':
                            reqVal1.errormessage = '{0} {1} {2} {3} {4}.'.format(localizer.PleaseSelect, localizer.A, localizer.Value, localizer.For, localizer.YesNo);
                            reqVal1.controltovalidate = thisDomIds.Criteria1DropDown;
                            thisNs.AlterCompareValidator('DataTypeCheck', 'YesNo', label1, reqVal1);
                            break;
                    }

                    switch (thisIds.FilterConditionType) {
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '37':
                        case '38':
                        case '39':
                        case '40':
                        case '41':
                        case '42':
                        case '46':
                            if (thisIds.FilterDataType !== 'L' && thisIds.FilterDataType !== 'GL' && thisIds.FilterDataType !== 'CL' && thisIds.FilterDataType !== 'T') {
                                ValidatorEnable(reqVal1, true);
                                ValidatorEnable(dataVal1, true);
                            }
                            if (thisIds.FilterDataType === 'T') {
                                ValidatorEnable(timeVal1, true);
                            }
                            if (thisIds.FilterDataType === 'DT') {
                                ValidatorEnable(reqVal1, true);
                                ValidatorEnable(timeVal1, true);
                                ValidatorEnable(dataVal1, true);
                            }
                            if (thisIds.FilterDataType === 'N') {
                                ValidatorEnable(intVal1, true);
                            }
                            break;
                        case '8':
                            switch (thisIds.FilterDataType) {
                                case 'N':
                                    ValidatorEnable(reqVal1, true);
                                    ValidatorEnable(dataVal1, true);
                                    ValidatorEnable(reqVal2, true);
                                    ValidatorEnable(dataVal2, true);
                                    ValidatorEnable(cmpVal, true);
                                    ValidatorEnable(intVal1, true);
                                    ValidatorEnable(intVal2, true);
                                    break;
                                case 'T':
                                    ValidatorEnable(timeVal1, true);
                                    ValidatorEnable(timeVal2, true);
                                    //do not enable the compare validator when all time fields are blank.
                                    var t1 = $('#' + thisDomIds.Criteria1Time).val().replace(' ', '').length > 0;
                                    var t2 = $('#' + thisDomIds.Criteria2Time).val().replace(' ', '').length > 0;
                                    if (t1 && t2) {
                                        ValidatorEnable(cmpTimeRange, true);
                                    }
                                    break;
                                case 'DT':
                                    ValidatorEnable(reqVal1, true);
                                    ValidatorEnable(reqVal2, true);
                                    ValidatorEnable(timeVal1, true);
                                    ValidatorEnable(timeVal2, true);
                                    ValidatorEnable(dataVal1, true);
                                    ValidatorEnable(dataVal2, true);
                                    //do not enable the compare validator when all date and time fields are blank.
                                    var dt1 = $('#' + thisDomIds.Criteria1).val().replace(' ', '').length > 0;
                                    var dt2 = $('#' + thisDomIds.Criteria2).val().replace(' ', '').length > 0;
                                    var dt3 = $('#' + thisDomIds.Criteria1Time).val().replace(' ', '').length > 0;
                                    var dt4 = $('#' + thisDomIds.Criteria2Time).val().replace(' ', '').length > 0;
                                    if (dt1 && dt2 && dt3 && dt4) {
                                        ValidatorEnable(cmpDateAndTime, true);
                                    }
                                    break;
                                default:
                                    ValidatorEnable(reqVal1, true);
                                    ValidatorEnable(dataVal1, true);
                                    ValidatorEnable(reqVal2, true);
                                    ValidatorEnable(dataVal2, true);
                                    ValidatorEnable(cmpVal, true);
                                    break;
                            }
                            break;
                        case '28':
                        case '29':
                        case '30':
                        case '31':
                        case '32':
                        case '33':
                        case '34':
                        case '35':
                            ValidatorEnable(reqVal1, true);
                            ValidatorEnable(dataVal1, true);
                            ValidatorEnable(intVal1, true);
                            break;
                        default:
                            break;
                    }
                    // handle special keywords
                    if (thisIds.FilterDataType === 'N') {
                        if ($('#' + thisDomIds.Criteria1).val().toUpperCase() === '@ME_ID') {
                            $('#' + thisDomIds.Criteria1).val('@ME_ID');
                            ValidatorEnable(dataVal1, false);
                            ValidatorEnable(cmpVal, false);
                            ValidatorEnable(intVal1, false);
                        }
                        if ($('#' + thisDomIds.Criteria2).val().toUpperCase() === '@ME_ID') {
                            $('#' + thisDomIds.Criteria2).val('@ME_ID');
                            ValidatorEnable(dataVal2, false);
                            ValidatorEnable(intVal2, false);
                            ValidatorEnable(cmpVal, false);
                        }
                        if ($('#' + thisDomIds.Criteria1).val().toUpperCase() === '@MY_HIERARCHY') {
                            $('#' + thisDomIds.Criteria1).val('@MY_HIERARCHY');
                            ValidatorEnable(dataVal1, false);
                            ValidatorEnable(cmpVal, false);
                            ValidatorEnable(intVal1, false);
                        }
                        if ($('#' + thisDomIds.Criteria2).val().toUpperCase() === '@MY_HIERARCHY') {
                            $('#' + thisDomIds.Criteria2).val('@MY_HIERARCHY');
                            ValidatorEnable(dataVal2, false);
                            ValidatorEnable(intVal2, false);
                            ValidatorEnable(cmpVal, false);
                        }
                        if ($('#' + thisDomIds.Criteria1).val().toUpperCase() === '@MY_COST_CODE_HIERARCY') {
                            $('#' + thisDomIds.Criteria1).val('@MY_COST_CODE_HIERARCY');
                            ValidatorEnable(dataVal1, false);
                            ValidatorEnable(cmpVal, false);
                            ValidatorEnable(intVal1, false);
                        }
                        if ($('#' + thisDomIds.Criteria2).val().toUpperCase() === '@MY_COST_CODE_HIERARCY') {
                            $('#' + thisDomIds.Criteria2).val('@MY_COST_CODE_HIERARCY');
                            ValidatorEnable(dataVal2, false);
                            ValidatorEnable(intVal2, false);
                            ValidatorEnable(cmpVal, false);
                        }
                    }
                    if (thisIds.FilterDataType === 'S' || thisIds.FilterDataType === 'FS' || thisIds.FilterDataType === 'LT') {
                        criteria1 = $('#' + thisDomIds.Criteria1);
                        criteria2 = $('#' + thisDomIds.Criteria2);

                        if (criteria1.val().toUpperCase() === '@ME_ID') {
                            criteria1.val('@ME_ID');
                            ValidatorEnable(dataVal1, false);
                            ValidatorEnable(intVal1, false);
                        }
                        if (criteria1.val().toUpperCase() === '@ME') {
                            criteria1.val('@ME');
                            ValidatorEnable(dataVal1, false);
                            ValidatorEnable(intVal1, false);
                        }
                        if (criteria1.val().toUpperCase() === '@MY_HIERARCHY') {
                            criteria1.val('@MY_HIERARCHY');
                            ValidatorEnable(dataVal1, false);
                            ValidatorEnable(intVal1, false);
                        }
                        if (criteria1.val().toUpperCase() === '@MY_COST_CODE_HIERARCY') {
                            criteria1.val('@MY_COST_CODE_HIERARCY');
                            ValidatorEnable(dataVal1, false);
                            ValidatorEnable(intVal1, false);
                        }
                        if (criteria2.val().toUpperCase() === '@ME_ID') {
                            criteria2.val('@ME_ID');
                            ValidatorEnable(dataVal1, false);
                            ValidatorEnable(intVal1, false);
                        }
                        if (criteria2.val().toUpperCase() === '@ME') {
                            criteria2.val('@ME');
                            ValidatorEnable(dataVal1, false);
                            ValidatorEnable(intVal1, false);
                        }
                        if (criteria2.val().toUpperCase() === '@MY_HIERARCHY') {
                            criteria2.val('@MY_HIERARCHY');
                            ValidatorEnable(dataVal1, false);
                            ValidatorEnable(intVal1, false);
                        }
                        if (criteria2.val().toUpperCase() === '@MY_COST_CODE_HIERARCY') {
                            criteria2.val('@MY_COST_CODE_HIERARCY');
                            ValidatorEnable(dataVal1, false);
                            ValidatorEnable(intVal1, false);
                        }
                    }
                }
            },
            Show: function (control) {
                var thisNs = $.filterModal.Filters.FilterModal;
                thisNs.CreateTimeControl(1);
                thisNs.CreateTimeControl(2);
                if ($.filterModal.options.domainRoot.DomIDs.Filters.FilterModalObj !== null && $.filterModal.options.domainRoot.DomIDs.Filters.FilterModalObj !== undefined) {
                    var zIndex = SEL.Common.GetHighestZIndexInt();
                    $('#' + $.filterModal.options.domainRoot.DomIDs.Filters.Panel).css("z-index", 2 + zIndex);
                    $f($.filterModal.options.domainRoot.DomIDs.Filters.FilterModalObj).show();
                    $('#' + $.filterModal.options.domainRoot.DomIDs.Filters.FilterModalObj + '_backgroundElement').css('z-index', 1 + zIndex);
                }
                
                thisNs.SetupTabIndex();
            },
            Service: function (path, method, params, sucessFunction, failFunction, isAsync) {
                if (!path.endsWith('/')) {
                    path = path + '/';
                };
                var async = false;
                if (isAsync == undefined) {
                    async = true;
                }
                $.ajax({
                    url: path + method,
                    type: 'POST',
                    data: params,
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    success: sucessFunction,
                    error: failFunction,
                    async: async
                });
            },
            SetupTabIndex: function () {
                $('[id$=_tcFilters_ddlFilter]').attr('tabindex', '9000');
                $('[id$=_tcFilters_txtFilterCriteria1]').attr('tabindex', '9001');
                $('[id$=_tcFilters_txtTimeCriteria1]').attr('tabindex', '9002');
                $('[id$=_tcFilters_cmbFilterCriteria1]').attr('tabindex', '9003');
                $('[id$=_tcFilters_txtFilterCriteria2]').attr('tabindex', '9004');
                $('[id$=_tcFilters_txtTimeCriteria2]').attr('tabindex', '9005');
                $('[id$=_tcFilters_btnFilterSave]').attr('tabindex', '9006');
                $('[id$=_tcFilters_btnFilterClose]').attr('tabindex', '9007');
                $('[id$=_tcFilters_ddlFilter]').focus();
            },

            Hide: function () {
                var thisDomIds = $.filterModal.options.domainRoot.DomIDs.Filters.FilterModal,
                    thisIds = $.filterModal.Filters.Ids;

                $('#' + thisDomIds.Criteria1).datepicker('destroy');
                $('#' + thisDomIds.Criteria2).datepicker('destroy');

                switch (thisIds.FilterDataType) {
                    case 'CL':
                    case 'GL':
                    case 'L':
                        $('.ui-multiselect.ui-helper-clearfix.ui-widget').remove();
                        break;
                }

                $('#imgFilterModalHelp').css('display', 'none');
                if ($.filterModal.options.domainRoot.DomIDs.Filters.FilterModalObj !== null && $.filterModal.options.domainRoot.DomIDs.Filters.FilterModalObj !== undefined) {
                    $f($.filterModal.options.domainRoot.DomIDs.Filters.FilterModalObj).hide();
                }
            },

            Cancel: function () {
                var thisNs = $.filterModal.options.domainRoot,
                    filterMod = thisNs.Filters.FilterModal;

                if (filterMod !== null && filterMod !== undefined && filterMod.FilterEditMode === 'drag') {
                    $.filterModal.options.domainRoot.Node.Remove.Single(thisNs.DomIDs.Filters.Drop, thisNs.IDs.FilterNode);
                }
                if ($.filterModal.Filters.Ids.CurrentId !== null) {
                    var existing = $.data(document, 'filter' + $.filterModal.Filters.Ids.CurrentId);
                    if (existing === undefined) {
                        $('#' + 'filter' + $.filterModal.Filters.Ids.CurrentId).remove();
                    }
                }
                $('.filter-search').val(' Search..').hide();
                $.filterModal.Filters.FilterModal.Hide();
            },

            ClearForm: function (fromEnterAtRuntime) {
                SEL.Common.Page_ClientValidateReset();
                var thisDomIds = $.filterModal.options.domainRoot.DomIDs.Filters.FilterModal,
                    row1 = $('#' + thisDomIds.CriteriaRow1),
                    row2 = $('#' + thisDomIds.CriteriaRow2);

                row1.css('display', 'none');
                row2.css('display', 'none');
                $('#' + thisDomIds.Criteria1).val('');
                $('#' + thisDomIds.Criteria2).val('');
                $('#' + thisDomIds.Criteria1Time).val('');
                $('#' + thisDomIds.Criteria2Time).val('');
                $g(thisDomIds.Criteria1DropDown).options.length = 0;
                $g(thisDomIds.CriteriaListDropDown).options.length = 0;
                if (fromEnterAtRuntime === undefined) {
                    if ($('#' + thisDomIds.Runtime).length > 0) {
                        $('#' + thisDomIds.Runtime).prop('checked', false);
                    }
                }
                $.filterModal.Filters.Ids.FilterConditionType = '0';
            },

            ToggleCalendar: function (valuenumber) {
                var thisDomIds = $.filterModal.options.domainRoot.DomIDs.Filters.FilterModal,
                    criteriacontrol = valuenumber === 1 ? $('#' + thisDomIds.Criteria1) : $('#' + thisDomIds.Criteria2);
                if ($('#ui-datepicker-div').css('display') === 'none') {
                    criteriacontrol.focus();
                } else {
                    $('#ui-datepicker-div').fadeOut(100);
                }
                return true;
            },

            ValidateTime: function (sender, args) {
                var thisDomIds = $.filterModal.options.domainRoot.DomIDs.Filters.FilterModal,
                    time,
                    hour,
                    minutes;

                if (sender.id === thisDomIds.TimeValidator1) {
                    time = $('#' + thisDomIds.Criteria1Time).val().split(':');
                } else {
                    time = $('#' + thisDomIds.Criteria2Time).val().split(':');
                }

                if (time.length !== 2) {
                    args.IsValid = false;
                    return;
                }
                hour = time[0];
                if (!(!isNaN(parseInt(hour, 10)) && isFinite(hour) && hour <= 23 && hour >= 0)) {
                    args.IsValid = false;
                    return;
                }
                minutes = time[1];
                if (!(!isNaN(parseInt(minutes, 10)) && isFinite(minutes) && hour <= 60 && hour >= 0)) {
                    args.IsValid = false;
                    return;
                }
                args.IsValid = true;
            },

            GetDateObjectFromUKDate: function (datestring) {
                var datepart,
                    timepart,
                    date,
                    year,
                    month,
                    day,
                    hours,
                    minutes;
                datepart = datestring.split(' ');
                timepart = datepart[1].split(':');
                date = datepart[0].split('/');
                year = parseInt(date[2], 10);
                month = parseInt(date[1], 10) - 1;
                day = parseInt(date[0], 10);
                hours = parseInt(timepart[0], 10);
                minutes = parseInt(timepart[1], 10);

                return new Date(year, month, day, hours, minutes);
            },

            ValidateDateTime: function (sender, args) {
                var thisNs = $.filterModal.Filters.FilterModal,
                    thisDomIds = $.filterModal.options.domainRoot.DomIDs.Filters.FilterModal,
                    date1,
                    date2,
                    time1,
                    time2;
                try {
                    date1 = $('#' + thisDomIds.Criteria1).val();
                    time1 = $('#' + thisDomIds.Criteria1Time).val();
                    date2 = $('#' + thisDomIds.Criteria2).val();
                    time2 = $('#' + thisDomIds.Criteria2Time).val();
                    args.IsValid = thisNs.GetDateObjectFromUKDate(date2 + ' ' + time2) > thisNs.GetDateObjectFromUKDate(date1 + ' ' + time1);
                } catch (exception) {
                    args.IsValid = false;
                }
            },

            ValidateTimeRange: function (sender, args) {
                var thisNs = $.filterModal.Filters.FilterModal,
                    thisDomIds = $.filterModal.options.domainRoot.DomIDs.Filters.FilterModal,
                    time1,
                    time2;
                try {
                    time1 = '01/01/2000 ' + $('#' + thisDomIds.Criteria1Time).val();
                    time2 = '01/01/2000 ' + $('#' + thisDomIds.Criteria2Time).val();
                    args.IsValid = thisNs.GetDateObjectFromUKDate(time2) > thisNs.GetDateObjectFromUKDate(time1);
                } catch (exception) {
                    args.IsValid = false;
                }
            },

            ExtractMetaDataToControls: function () {
                var thisDomIds = $.filterModal.options.domainRoot.DomIDs.Filters.FilterModal,
                    thisIds = $.filterModal.Filters.Ids,
                    thisNs = $.filterModal.Filters.FilterModal,
                    selNode = $('#' + thisIds.FilterNode).data(),
                    criteria1 = $('#' + thisDomIds.Criteria1),
                    criteria2 = $('#' + thisDomIds.Criteria2),
                    val1 = '',
                    val2 = '',
                    dt1,
                    dt2,
                    enterAtRuntime = false;
                if ($.filterModal.Filters.Ids.EditControl !== null) {

                    selNode = $.data(document, 'filter' + $.filterModal.Filters.Ids.CurrentId);
                    if (selNode === undefined || selNode === null) {
                        return;
                    }
                }
                if ($.filterModal.options.domainRoot.DomIDs.Filters.FilterModal.Runtime !== null) {
                    enterAtRuntime = selNode.runtime;
                    $('#' + $.filterModal.options.domainRoot.DomIDs.Filters.FilterModal.Runtime).prop("checked", enterAtRuntime);
                }

                if (selNode) {
                    if (selNode.criterionOne === '@ME' && thisIds.FilterDataType === 'GL') {
                        thisIds.FilterConditionType = 254; //@ME
                        val1 = '';
                        val2 = '';
                    } else if (selNode.criterionOne === '@MY_HIERARCHY' && thisIds.FilterDataType === 'GL') {
                        thisIds.FilterConditionType = 255; //@MY_HIERARCHY
                        val1 = '';
                        val2 = '';
                    } else if (selNode.criterionOne === '@MY_COST_CODE_HIERARCHY' && thisIds.FilterDataType === 'GL') {
                        thisIds.FilterConditionType = 250; //@MY_COST_CODE_HIERARCHY
                        val1 = '';
                        val2 = '';
                    } else {
                        thisIds.FilterConditionType = typeof selNode.conditionType === 'undefined' ? '' : selNode.conditionType.toString();
                        $('#' + thisNs.FilterDropDown).val(thisIds.FilterConditionType);
                        val1 = typeof selNode.criterionOne === 'undefined' ? '' : selNode.criterionOne.toString();
                        val2 = typeof selNode.criterionTwo === 'undefined' ? '' : selNode.criterionTwo.toString();

                        // Replace any HTML that has been escaped
                        val1 = val1.replace(/&lt;/g, '<').replace(/&gt;/g, '>');
                        val2 = val2.replace(/&lt;/g, '<').replace(/&gt;/g, '>');
                    }
                }

                criteria1.unbind('keyup.decimal').unbind('blur.date').unbind('blur.decimal');
                criteria2.unbind('keyup.decimal').unbind('blur.date').unbind('blur.decimal');

                thisNs.ChangeFilterCriteria(true);
                if (thisIds.FilterDataType !== 'DT') {
                    switch (thisIds.FilterDataType) {
                        case 'X':
                            val1 = (val1 === "0") ? "2" : "1";
                            criteria1 = $('#' + thisDomIds.Criteria1DropDown);
                            break;
                        case 'CL':
                        case 'L':
                        case 'GL':
                            criteria1 = null;
                            break;
                        case 'T':
                            criteria1 = $('#' + thisDomIds.Criteria1Time);
                            criteria2 = $('#' + thisDomIds.Criteria2Time);
                            break;
                    }
                    if (criteria1 !== null) {
                        criteria1.val(val1);
                    }
                    criteria2.val(val2);
                    if (thisIds.FilterDataType === 'M' || thisIds.FilterDataType === 'C' || thisIds.FilterDataType === 'FD') {
                        criteria1.bind('blur.decimal', function () { thisNs.Decimalate(criteria1, thisIds.FilterPrecision, thisDomIds.RequiredValidator1); });
                        criteria2.bind('blur.decimal', function () { thisNs.Decimalate(criteria2, thisIds.FilterPrecision, thisDomIds.RequiredValidator2); });
                    }
                    if (thisIds.FilterDataType === 'N') {
                        criteria1.bind('blur', function () { criteria1.val(criteria1.val().replace(/,/g, '')); });
                        criteria2.bind('blur', function () { criteria2.val(criteria2.val().replace(/,/g, '')); });
                    }
                } else {
                    if (val1 !== undefined && val1 !== null && val1 !== '') {
                        dt1 = val1.split(' ');
                        $('#' + thisDomIds.Criteria1).val(dt1[0]);
                        $('#' + thisDomIds.Criteria1Time).val(dt1[1]);
                    }
                    if (val2 !== undefined && val2 !== null && val2 !== '') {
                        dt2 = val2.split(' ');
                        $('#' + thisDomIds.Criteria2).val(dt2[0]);
                        $('#' + thisDomIds.Criteria2Time).val(dt2[1]);
                    }
                }
            },

            UpdateDropdownValidatorIfNoneSelected: function (controlId, reqValId) {
                if ($('#' + controlId + ' option:selected').text() === $.filterModal.Filters.Messages.NoneOption) {
                    var val = $g(reqValId);
                    val.isvalid = false;
                    ValidatorUpdateDisplay(val);
                }
            },

            Decimalate: function (control, precision, reqValId) {
                var val = $g(reqValId);

                var controlValue = control.val().replace(/,/g, '');

                if ($.isNumeric(controlValue)) {
                    var decimalPlace = controlValue.indexOf('.');

                    if (decimalPlace !== -1) {
                        if (controlValue.substring(decimalPlace + 1).length > precision) {
                            val.isvalid = false;
                            ValidatorUpdateDisplay(val);
                            return false;
                        }
                    }
                    val.isvalid = true;
                    ValidatorUpdateDisplay(val);
                }
                return true;
            },

            AlterCompareValidator: function (operator, datatype, label, validator) {
                var localizer = $.filterModal.Filters.Messages.Filters.ValidatorParts,
                    baseMessage,
                    cmpLabel;
                validator.errormessage = '';
                validator.operator = operator;
                validator.type = datatype;
                if (operator === 'DataTypeCheck') {
                    baseMessage = '{0} {1} {2} {X} {3} {4}.'.format(localizer.PleaseEnter, localizer.A, localizer.Valid, localizer.For, label).replace('{X}', '{0}');
                    switch (datatype) {
                        case 'Date':
                            validator.errormessage = baseMessage.format(localizer.Date);
                            break;
                        case 'Integer':
                            validator.errormessage = baseMessage.format(localizer.Integer);
                            break;
                        case 'Currency':
                            validator.errormessage = baseMessage.format(localizer.Currency);
                            break;
                        case 'Double':
                            validator.errormessage = baseMessage.format(localizer.Decimal);
                            break;
                        case 'YesNo':
                            validator.errormessage = '{0} {1} {2} {3} {4}.'.format(localizer.PleaseSelect, localizer.A, localizer.Value, localizer.For, localizer.YesNo);
                            break;
                        default:
                            break;
                    }
                } else {
                    cmpLabel = label.replace(' 2', ' 1');
                    baseMessage = '{0} {1} {2} {X} {3} {4} {5} {6}.'.format(localizer.PleaseEnter, localizer.A, localizer.Valid, localizer.GreaterThan, cmpLabel, localizer.For, label).replace('{X}', '{0}');
                    switch (datatype) {
                        case 'Date':
                            validator.errormessage = baseMessage.format(localizer.Date);
                            break;
                        case 'Integer':
                            validator.errormessage = baseMessage.format(localizer.Integer);
                            break;
                        case 'Currency':
                            validator.errormessage = baseMessage.format(localizer.Currency);
                            break;
                        case 'Double':
                            validator.errormessage = baseMessage.format(localizer.Decimal);
                            break;
                        default:
                            break;
                    }
                }
            },

            CreateDateControls: function () {
                var thisNs = $.filterModal.Filters.FilterModal,
                    thisDomIds = $.filterModal.options.domainRoot.DomIDs.Filters.FilterModal,
                    criteriacontrol1 = $('#' + thisDomIds.Criteria1),
                    criteriacontrol2 = $('#' + thisDomIds.Criteria2),
                    calc1 = $('#' + thisDomIds.Criteria1ImageCalc),
                    calc2 = $('#' + thisDomIds.Criteria2ImageCalc);

                criteriacontrol1.datepicker({
                    onSelect: function () {
                        thisNs.DateBlur(thisDomIds.Criteria1);
                    },
                    onClose: function () {
                        criteriacontrol1.unbind('keypress');
                        SEL.Common.BindEnterKeyForElement(criteriacontrol1.attr('id'), $.filterModal.Filters.FilterModal.Save);
                    }
                });
                criteriacontrol1.unbind('blur.date').bind('blur.date', function () {
                    thisNs.DateBlur(thisDomIds.Criteria1);
                });

                criteriacontrol2.datepicker({
                    onSelect: function () {
                        thisNs.DateBlur(thisDomIds.Criteria2);
                    },
                    onClose: function () {
                        criteriacontrol2.unbind('keypress');
                        SEL.Common.BindEnterKeyForElement(criteriacontrol2.attr('id'), $.filterModal.Filters.FilterModal.Save);
                    }
                });
                criteriacontrol2.unbind('blur.date').bind('blur.date', function () {
                    thisNs.DateBlur(thisDomIds.Criteria2);
                });

                calc1.css('display', '');
                calc2.css('display', '');
                criteriacontrol1.css('width', '70px');
                criteriacontrol2.css('width', '70px');
                criteriacontrol1.attr('maxlength', 10);
                criteriacontrol2.attr('maxlength', 10);
            },

            DateBlur: function (controlId) {
                var thisDomIds = $.filterModal.options.domainRoot.DomIDs.Filters.FilterModal,
                    control = controlId === thisDomIds.Criteria1 ? $('#' + thisDomIds.Criteria1) : $('#' + thisDomIds.Criteria2),
                    validator = controlId === thisDomIds.Criteria1 ? $g(thisDomIds.RequiredValidator1) : $g(thisDomIds.RequiredValidator2),
                    dataval = controlId === thisDomIds.Criteria1 ? $g(thisDomIds.DataTypeValidator1) : $g(thisDomIds.DataTypeValidator2),
                    cmpdatetime = $g(thisDomIds.DateAndTimeCompareValidator),
                    cmpval = $g(thisDomIds.CompareValidator);

                cmpdatetime.isvalid = true;
                dataval.isvalid = true;
                cmpval.isvalid = true;

                if (control.val() === '') {
                    validator.isvalid = false;
                } else {
                    validator.isvalid = true;
                }
                ValidatorUpdateDisplay(cmpdatetime);
                ValidatorUpdateDisplay(cmpval);
                ValidatorUpdateDisplay(dataval);
                ValidatorUpdateDisplay(validator);
            },

            FormatDate: function (date, format) {
                var oDate = date;
                if (date !== '') {
                    try {
                        date = $.datepicker.formatDate(format, new Date(date));
                        date = date.slice(0, 10);
                        if (date.slice(0, 1) === 'N') {
                            date = oDate;
                        }
                    } catch (e) {
                        date = oDate;
                    }
                }
                if (!date || typeof (date) === 'undefined') {
                    date = oDate;
                }
                return date;
            },

            CreateTimeControl: function (valuenumber) {
                var thisDomIds = $.filterModal.options.domainRoot.DomIDs.Filters.FilterModal,
                    timecontrol = valuenumber === 1 ? $('#' + thisDomIds.Criteria1Time) : $('#' + thisDomIds.Criteria2Time),
                    validator = valuenumber === 1 ? $g(thisDomIds.RequiredValidator1) : $g(thisDomIds.RequiredValidator2),
                    datavalidator = valuenumber === 1 ? $g(thisDomIds.DataTypeValidator1) : $g(thisDomIds.DataTypeValidator2),
                    timevalidator = valuenumber === 1 ? $g(thisDomIds.TimeValidator1) : $g(thisDomIds.TimeValidator2),
                    rangevalidator = $g(thisDomIds.TimeRangeCompareValidator);

                timecontrol.timepicker({
                    showCloseButton: true,
                    showNowButton: true,
                    showDeselectButton: true,
                    beforeShow: function () {
                        timecontrol.unbind('keypress');
                    },
                    onClose: function () {
                        $(this).unbind('keypress');
                        timevalidator.isvalid = true;
                        ValidatorUpdateDisplay(timevalidator);
                        datavalidator.isvalid = true;
                        ValidatorUpdateDisplay(datavalidator);
                        rangevalidator.isvalid = true;
                        ValidatorUpdateDisplay(rangevalidator);

                        SEL.Common.BindEnterKeyForSelector('#divCriteria1 input, #divCriteria2 input', $.filterModal.Filters.FilterModal.Save);
                        if (timecontrol.val() === '') {
                            validator.isvalid = false;
                            ValidatorUpdateDisplay(validator);
                        } else {
                            validator.isvalid = true;
                            ValidatorUpdateDisplay(validator);
                        }
                        return true;
                    }
                });
            },
            GetFieldID: function (control, torchMode) {
                var getFieldInfoForFilterComplete;
                if (torchMode == true) {
                    getFieldInfoForFilterComplete = function(f) {
                        var fdata = f.d, thisIds = $.filterModal.Filters.Ids;
                        thisIds.FilterFieldID = fdata[2];
                        $.filterModal.Filters.FilterModal.EditTorchMode(fdata[2]);
                    };
                } else {
                    getFieldInfoForFilterComplete = function(f) {
                        var fdata = f.d, thisIds = $.filterModal.Filters.Ids;
                        thisIds.FilterFieldID = fdata[2];
                        $.filterModal.Filters.FilterModal.Edit(fdata[2]);
                    };
                }

                $.filterModal.Filters.Ids.EditControl = control;
                
                var params = new $.filterModal.Filters.FilterModal.FieldInfoObject(currentProjectId, $('#' + control).text(), torchMode);
                
                SEL.Ajax.Service('/shared/webServices/svcDocumentMerge.asmx/', 'GetFieldInfoForFilter', params, getFieldInfoForFilterComplete, SEL.Common.WebService.ErrorHandler);
            },
            FieldInfoObject: function (projectId, fieldName) {
                this.projectId = projectId;
                this.fieldName = fieldName;
            },
            EditTorchMode: function(selNode) {
                var thisNs = $.filterModal.Filters.FilterModal;
                var thisIds = $.filterModal.Filters.Ids;
                var thisDomIds = $.filterModal.options.domainRoot.DomIDs.Filters.FilterModal;

                thisIds.FilterFieldID = selNode;
                thisNs.ClearForm();
                thisNs.SetupListHelp(true);

                var infoSuccess = function(f) {
                    var fdata = f.d;
                    thisIds.FilterDataType = fdata[0];
                    if (thisIds.FilterDataType === "L" || thisIds.FilterDataType === "GL" || thisIds.FilterDataType === "CL") {
                        thisNs.List = fdata[1];
                        $('#imgFilterModalHelp').css('display', 'inline-block');
                    } else {
                        thisIds.FilterPrecision = fdata[1];
                    }

                    var operatorSuccess = function(o) {
                        if (o !== null && o !== undefined) {
                            var odata = o.d;

                            thisNs.PopulateDropDown($g(thisDomIds.FilterDropDown), odata.CriteriaList, null, true);
                            thisNs.SetUpSearchTextBox();
                            thisNs.SetUpMultiSelectControl();
                            thisNs.ExtractMetaDataToControls();
                            thisNs.Show();
                        }
                        return true;
                    };

                    var operatorParams = new function () {
                        this.fieldType = thisIds.FilterDataType;
                        this.filterFieldId = thisIds.FilterFieldID;
                        this.showInOption = true;
                    };
                    SEL.Ajax.Service("/shared/webServices/svcTree.asmx/", "GetOperatorsListItemsByFilterFieldType", operatorParams, operatorSuccess, SEL.Common.WebService.ErrorHandler);
                    return true;
                };

                var params = new function () { this.fieldId = thisIds.FilterFieldID; };
                SEL.Ajax.Service("/shared/webServices/svcTree.asmx/", "GetFieldInfoForFilter", params, infoSuccess, SEL.Common.WebService.ErrorHandler);
            },
            Edit: function (selNode, treeId, click) {
                var thisNs = $.filterModal.Filters.FilterModal,
                    thisIds = $.filterModal.Filters.Ids,
                    thisDomIds = $.filterModal.options.domainRoot.DomIDs.Filters.FilterModal,
                    drop = $('#' + treeId),
                    localizer = $.filterModal.Filters.Messages.Filters,
                    fieldid;

                if (treeId !== undefined) {
                    thisNs.FilterEditMode = (click) ? 'click' : 'drag';
                    if (!selNode) {
                        selNode = drop.jstree('get_selected');
                        if (!selNode) {
                            SEL.MasterPopup.ShowMasterPopup(localizer.NoFilterColumnSelected);
                            return;
                        }
                        thisIds.FilterNode = selNode[0].id;
                    } else {
                        thisIds.FilterNode = thisNs.DedupeID(selNode[0].id, drop[0]);
                        selNode[0].id = thisIds.FilterNode;
                    }
                    if (thisNs.FilterEditMode === 'drag') {
                        selNode.css('display', 'none');
                    }

                    fieldid = selNode.attr('fieldid');
                    thisIds.FilterFieldID = fieldid;
                } else {
                    // Get the Field ID
                    thisIds.FilterFieldID = selNode;
                }


                thisNs.ClearForm();
                thisNs.SetupListHelp(true);
                if (treeId !== undefined) {
                    $('#' + thisDomIds.Heading).text('{0}: '.format(localizer.Title) + drop.jstree('get_text', selNode));
                }

                var infoSuccess = function (f) {
                    var fdata = f.d;
                    thisIds.FilterDataType = fdata[0];
                    if (thisIds.FilterDataType === "L" || thisIds.FilterDataType === "GL" || thisIds.FilterDataType === "CL") {
                        thisNs.List = fdata[1];
                        $('#imgFilterModalHelp').css('display', 'inline-block');
                    } else {
                        thisIds.FilterPrecision = fdata[1];
                    }

                    var operatorSuccess = function (o) {
                        var requiredAttributes;
                        var attributesToDisplay=[];
                        if (o !== null && o !== undefined) {
                            var odata = o.d.CriteriaList;
                            if (SEL.CustomEntityAdministration.ParentFilter.IsParentFilter) {
                               
                                var thisNsForm = SEL.CustomEntityAdministration.Forms.FormFieldDetails;
                                requiredAttributes = thisNsForm.filter(function (element) {
                                    return element.FieldType === 9 && element.AttributeID !== SEL.CustomEntityAdministration.ParentFilter.ChildElementId; ///field type 9 is for N:1 attributes
                                });
                                for (var i = 0; i < requiredAttributes.length; i++) {
                                    attributesToDisplay[i] = {
                                        Attributes: [],
                                        Enabled: true,
                                        Selected: false,
                                        Text: requiredAttributes[i].DisplayName,
                                        Value: requiredAttributes[i].AttributeID
                                    }
                                }
                            }
                            thisNs.PopulateDropDown($g(thisDomIds.FilterDropDown), SEL.CustomEntityAdministration.ParentFilter.IsParentFilter ? attributesToDisplay : odata, null, true);
                            thisNs.SetUpSearchTextBox();
                            thisNs.SetUpMultiSelectControl();
                            thisNs.ExtractMetaDataToControls();
                            thisNs.Show();
                        }
                        return true;
                    };

                    var operatorParams = new function () {
                        this.fieldType = thisIds.FilterDataType;
                        this.filterFieldId = thisIds.FilterFieldID;
                        this.showInOption = false; 
                    };
                    SEL.Ajax.Service("/shared/webServices/svcTree.asmx/", "GetOperatorsListItemsByFilterFieldType", operatorParams, operatorSuccess, SEL.Common.WebService.ErrorHandler);
                    return true;
                };

                var params = new function () { this.fieldId = thisIds.FilterFieldID; };
                SEL.Ajax.Service("/shared/webServices/svcTree.asmx/", "GetFieldInfoForFilter", params, infoSuccess, SEL.Common.WebService.ErrorHandler);

            },

            SetupListHelp: function (init) {
                var imgFilterModalHelp = $('#imgFilterModalHelp'),
                    viewFilterModalHelp;
                if (init) {
                    imgFilterModalHelp.css('display', 'none');
                } else {
                    imgFilterModalHelp.css('display', '');
                }
                imgFilterModalHelp.css('left', imgFilterModalHelp.parent().outerWidth() - 38);
                imgFilterModalHelp.css('top', imgFilterModalHelp.parent().outerHeight() - 38);
                imgFilterModalHelp.unbind('mouseenter').unbind('mouseleave').mouseenter(function () {
                    viewFilterModalHelp = $('#viewFilterModalHelpArea');
                    viewFilterModalHelp.css('zIndex', $.filterModal.options.domainRoot.zIndices.Misc.InformationMessage());
                    viewFilterModalHelp.css('left', $(this).offset().left - viewFilterModalHelp.outerWidth());
                    viewFilterModalHelp.css('top', $(this).offset().top - viewFilterModalHelp.outerHeight());
                    viewFilterModalHelp.css('position', 'absolute');
                    viewFilterModalHelp.stop(true, true).fadeIn(400);
                }).mouseleave(function () {
                    $('#viewFilterModalHelpArea').stop(true, true).fadeOut(200);
                });
            },

            DedupeID: function (id, jqContainer) {
                // NOTE: This code is written in such a way as to work with IE7.
                // It could be a lot cleaner by using jQuery's .find, however IE7 doesn't like that. Sorry.
                $(jqContainer.getElementsByTagName('li')).each(function (x, currentItem) {
                    if (id === currentItem.id) {
                        id = $.filterModal.Filters.FilterModal.DedupeID('copy_' + id, jqContainer);
                        return false;
                    }
                    return true;
                });
                return id;
            },

            Save: function (validationGroup) {
                var isParentFilter = SEL.CustomEntityAdministration === undefined ? false : SEL.CustomEntityAdministration.ParentFilter.IsParentFilter;
                var thisNs = $.filterModal.Filters.FilterModal,
                    thisIds = $.filterModal.Filters.Ids,
                    thisDomIds = $.filterModal.options.domainRoot.DomIDs.Filters.FilterModal,
                    localizer = $.filterModal.Filters.Messages.Filters,
                    isListItem = false,
                    fcv,
                    numSelected,
                    criteria1 = '',
                    criteria2 = '',
                    conditiontype = thisIds.FilterConditionType,
                    firstListItemText = '',
                    firstListItem,
                    control1,
                    control2,
                    conditionTypeText,
                    filterObj,
                    enterAtRuntime = false;
                if (conditiontype === undefined || conditiontype === null) {
                    return true;
                }
                if ($.filterModal.options.domainRoot.DomIDs.Filters.FilterModal.Runtime !== null) {
                    enterAtRuntime = $g($.filterModal.options.domainRoot.DomIDs.Filters.FilterModal.Runtime).checked;
                }

                if (validationGroup === undefined || enterAtRuntime) {
                    validationGroup = 'vgViewFilter';
                }

                thisNs.AssignValidatorsOnSave(enterAtRuntime);
                if (validateform(validationGroup) === false) {
                    return false;
                }
                if (thisIds.FilterDataType === 'L' || thisIds.FilterDataType === 'GL' || thisIds.FilterDataType === 'CL') {
                    $('.filter-search').val(' Search..').hide();
                    fcv = $('#' + thisDomIds.FilterDropDown).val();
                    if (fcv !== '254' && fcv !== '255' && fcv !== '250' && !isParentFilter) //@ME && @MY_HIERARCHY && @MY_COST_CODE_HIERARCHY
                    {
                        isListItem = true;
                        if ($('#' + thisDomIds.CriteriaListRow).css('display') === 'block') {
                            numSelected = $('#' + thisDomIds.CriteriaListRow).find('div.selected:first').find('li').length - 1;
                            if (numSelected === 0) {
                                SEL.MasterPopup.ShowMasterPopup(localizer.NoListSelection);
                                return false;
                            }
                        }
                    }
                }

                if (isParentFilter) {
                    criteria1 = "" + conditiontype + "";
                } else {

                    switch (thisIds.FilterDataType) {
                    case 'X':
                        criteria1 = ($('#' + thisDomIds.Criteria1DropDown).val() == "2") ? "0" : "1";
                        break;
                    case 'CL':
                    case 'L':
                    case 'GL':
                        if (fcv !== '254' && fcv !== '255' && fcv !== '250' && isParentFilter !== true) //@ME && @MY_HIERARCHY && @MY_COST_CODE_HIERARCHY
                        {
                            if (fcv !== '9' && fcv !== '10') {
                                firstListItem = $('#' + thisDomIds.CriteriaListRow).find('div.selected:first').find('li');

                                firstListItemText = firstListItem.eq(1).attr('title');

                                firstListItem.each(function(i, listItem) {
                                    if (i !== 0) {
                                        criteria1 += $(listItem).attr('value') + ',';
                                    }
                                });
                                criteria1 = criteria1.slice(0, criteria1.length - 1);
                            }
                        } else {
                            // added if condition to check whether it is relationship filter

                            if (isParentFilter) {
                                criteria1 = "" + conditiontype + "";
                            } else {
                                switch (fcv) {
                                case '254':
                                    criteria1 = '@ME';
                                    break;
                                case '255':
                                    criteria1 = "@MY_HIERARCHY";
                                    break;
                                case '250':
                                    criteria1 = '@MY_COST_CODE_HIERARCHY';
                                    break;
                                }
                            }
                        }
                        break;
                    case 'C':
                    case 'M':
                    case 'FD':
                        control1 = $('#' + thisDomIds.Criteria1);
                        control2 = $('#' + thisDomIds.Criteria2);

                        var control1IsValid, control2IsValid = true;
                        var decimalPrecisionErrorMsg = '';

                        if (thisNs.Decimalate(control1, thisIds.FilterPrecision, thisDomIds.RequiredValidator1) ===
                            false) {
                            decimalPrecisionErrorMsg = 'Please enter a maximum of ' +
                                thisIds.FilterPrecision +
                                ' decimal places for ' +
                                control1.parent().prevAll('label').text().replace('*', '') +
                                '.<br />';
                            control1IsValid = false;
                        }

                        if (thisNs.Decimalate(control2, thisIds.FilterPrecision, thisDomIds.RequiredValidator2) ===
                            false) {
                            decimalPrecisionErrorMsg += 'Please enter a maximum of ' +
                                thisIds.FilterPrecision +
                                ' decimal places for ' +
                                control2.parent().prevAll('label').text().replace('*', '') +
                                '.';
                            control2IsValid = false;
                        }

                        if (control1IsValid === false || control2IsValid === false) {
                            SEL.MasterPopup.ShowMasterPopup(decimalPrecisionErrorMsg);
                            return false;
                        }

                        criteria1 = control1.val();
                        criteria2 = control2.val();
                        break;
                    case 'T':
                        criteria1 = $('#' + thisDomIds.Criteria1Time).val();
                        criteria2 = $('#' + thisDomIds.Criteria2Time).val();
                        break;
                    case 'DT':
                        criteria1 = $('#' + thisDomIds.Criteria1).val() + ' ' + $('#' + thisDomIds.Criteria1Time).val();
                        criteria2 = $('#' + thisDomIds.Criteria2).val() + ' ' + $('#' + thisDomIds.Criteria2Time).val();
                        break;
                    case 'N':
                        control1 = $('#' + thisDomIds.Criteria1);
                        control2 = $('#' + thisDomIds.Criteria2);
                        control1.val(control1.val().replace(/,/g, ''));
                        control2.val(control2.val().replace(/,/g, ''));
                        criteria1 = control1.val();
                        criteria2 = control2.val();
                        break;
                    default:
                        criteria1 = $('#' + thisDomIds.Criteria1).val();
                        criteria2 = $('#' + thisDomIds.Criteria2).val();
                        break;
                    }
                }
                if (fcv !== '254' && fcv !== '255' && fcv !== '250' && !isParentFilter) {
                    conditionTypeText = $('#' + thisDomIds.FilterDropDown + ' :selected').text();
                } else {
                      
                    if (isParentFilter) {
                        conditiontype = 1;
                        conditionTypeText = "Equals";
                        isListItem = false;
                        
                    }
                    else{
                        conditionTypeText = $('#' + thisDomIds.FilterDropDown + ' option[value=1]').text();
                    }
                }

                // Escape any HTML that has been added to the criterias
                criteria1 = criteria1.replace(/</g, '&lt;').replace(/>/g, '&gt;');
                criteria2 = criteria2.replace(/</g, '&lt;').replace(/>/g, '&gt;');

                filterObj = { conditionType: conditiontype, criterionOne: criteria1, criterionTwo: criteria2, conditionTypeText: conditionTypeText, fieldType: thisIds.FilterDataType, isListItem: isListItem, firstListItemText: firstListItemText, runtime: enterAtRuntime };
                if ($.filterModal.Filters.Ids.EditControl !== null) {
                    $.data(document, 'filter' + $.filterModal.Filters.Ids.CurrentId, filterObj);
                }
                $('#' + thisIds.FilterNode).data(filterObj);
                $('#' + thisIds.FilterNode).removeAttr("summaryLayoutSet");
                $('#' + thisIds.FilterNode).css('display', 'block');
                if (thisNs.Hide) {
                    thisNs.Hide();
                }

                var tree = $('#' + $.filterModal.options.domainRoot.DomIDs.Filters.Drop);
                if (tree.jstree) {
                    if (tree.attr('nodenoun') === "filters") {
                        $.filterModal.options.domainRoot.Filters.RefreshFilterSummaryLayout($.filterModal.options.domainRoot.DomIDs.Filters.Drop);
                    }
                }

                return true;
            }
        }
    };

    $.filterModal = function (options) {
        filterModal.init($.extend($.filterModal.options, options));
        return this;
    };

    $.filterModal.options = {
        domainRoot: SEL.Trees
    };

    $.filterModal.Filters = methods;

})(jQuery, window, ValidatorUpdateDisplay, ValidatorEnable);
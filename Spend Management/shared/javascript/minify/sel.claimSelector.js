/// <reference path="~/shared/javaScript/shared.js" />
/// <summary>
/// Claim Selector.
/// </summary>    
(function (SEL, CurrentUserInfo, $g, $f, moduleNameHTML, moduleNameHTML)
{
    var scriptName = "ClaimSelector";

    function execute()
    {
        SEL.registerNamespace("SEL.ClaimSelector");
        SEL.ClaimSelector =
        {
            ClaimNumber: '',
            ClaimName: '',
            EmployeeId: '',
            ModalTitle: 'Message from ' + moduleNameHTML,
            HighClaimantCount: 'false',
            NumberOfClaimants: 0,
            ConditionType: 'All',
            RootClaimSelector: 'false',
            Selectable: 'false',
            DomIDs: {
                ClaimantText: 'ClaimantText',
                ClaimantId: 'ClaimantText_ID',
                ClaimNumber: 'ClaimNumberText',
                ClaimName: 'ClaimNameText',
                Grid: 'SearchGrid',
                ClaimantCombo: 'ClaimantCombo',
                ValidatorClaimantRequired: 'reqClaimant',
                ValidatorClaimantValid: 'cmpValidatorEmployeeId',
                ValidatorClaimNumberRequired: 'reqClaimNumber',
                ValidatorClaimNumberNumeric: 'cmpClaimNumberTextNumeric',
                ValidatorClaimNumberMin: 'cmpClaimNumberTextMin',
                ValidatorClaimNameRequired: 'reqClaimName',
                SearchModal: 'ClaimantSearchModal'
            },
            Messages: {
                ClaimDoesNotExist: 'This claim number does not exist.',
                ClaimNotApproved: 'This claim has not been approved.',
                ClaimNotInHierarchy: 'You are not authorised to view this claim.',
                ClaimNotPaid: 'This claim is approved but unpaid.'
            },
            PageLoadFunctions: function () {

                var fromClaimSelector = SEL.ClaimSelector.GetParameterValues('claimSelector');
                if (fromClaimSelector === 'true') {
                    var self = SEL.ClaimSelector;
                    var domIds = SEL.ClaimSelector.DomIDs;
                    var employeeid = SEL.ClaimSelector.GetParameterValues('claimant');
                    var claimName = SEL.ClaimSelector.GetParameterValues('claimName');
                    if (claimName == '') {
                        if ($('#ClaimantCombo').css('display') == 'none') {
                            SEL.ClaimSelector.setEmployee(employeeid);
                        }
                        else {
                            $('#ClaimantCombo').val(employeeid).change();
                        }
                    }
                    $('#' + domIds.ClaimantId).val(employeeid);
                    $('#' + domIds.ClaimName).val(claimName);
                    self.SearchEmployee();
                    SEL.ClaimSelector.FilterGrid();
                    SEL.ClaimSelector.ClickPager(SEL.ClaimSelector.GetParameterValues('pageNumber') - 1);

                }
            },
            Cancel: function (returnPage)
            {
                document.location = returnPage;
            },

            HideSearchModal: function ()
            {
                SEL.Common.HideModal(SEL.ClaimSelector.DomIDs.SearchModal);
            },

            Initialise: function ()
            {
                var self = SEL.ClaimSelector;
                var domIds = SEL.ClaimSelector.DomIDs;
                SEL.ClaimSelector.ClearClaimName();
                SEL.ClaimSelector.ClearClaimant();
                var claimNameBox = $('#' + domIds.ClaimName);
                var claimantBox = $('#' + domIds.ClaimantText);

                if (self.HighClaimantCount)
                {
                    claimantBox.bind("keyup", function () { self.ClearClaimName(); });
                }
                claimNameBox.bind("keyup", function () { self.ClearClaimant(); });
                $(':input:visible:first').focus();
            },

            ResetValidators: function ()
            {
                var domIDs = SEL.ClaimSelector.DomIDs;
                ValidatorEnable($g(domIDs.ValidatorClaimantRequired), true);
                ValidatorEnable($g(domIDs.ValidatorClaimantValid), true);
                ValidatorEnable($g(domIDs.ValidatorClaimNameRequired), true);
            },

            EnableRequiredFieldValidators: function ()
            {
                var self = SEL.ClaimSelector;
                var domIds = SEL.ClaimSelector.DomIDs;

                SEL.ClaimSelector.ResetValidators();

                self.ClaimName = $('#' + domIds.ClaimName).val();
                $('#ClaimNameText').val(self.ClaimName);
                if (self.HighClaimantCount === 'true')
                {
                    self.EmployeeId = $('#' + domIds.ClaimantId).val();
                }
                else
                {
                    self.EmployeeId = $('#' + domIds.ClaimantCombo).val();
                    if (self.EmployeeId != '0')
                    {
                        $('#' + domIds.ClaimantId).val(self.EmployeeId);
                    } else
                    {
                        $('#' + domIds.ClaimantId).val('');
                        $('#' + domIds.Grid)[0].innerHTML = '';
                    }
                }
                if (self.EmployeeId === undefined || self.EmployeeId === null || self.EmployeeId === '')
                {
                    self.EmployeeId = '0';
                }
                if (self.ClaimName != '')
                {
                    ValidatorEnable($g(domIds.ValidatorClaimantRequired), false);
                }
                if (self.EmployeeId !== '0' || $('#' + domIds.ClaimantText).val() !== '')
                {
                    ValidatorEnable($g(domIds.ValidatorClaimNameRequired), false);
                }
            },

            ClearClaimName: function ()
            {
                $('#' + SEL.ClaimSelector.DomIDs.ClaimName).val('');
            },

            ClearClaimant: function ()
            {
                var self = SEL.ClaimSelector.DomIDs;
                if (SEL.ClaimSelector.HighClaimantCount === 'true')
                {
                    $('#' + self.ClaimantText).val('');
                    $('#' + self.ClaimantId).val('');
                } else
                {
                    $('#' + self.ClaimantCombo + ' > [value="0"]').prop('selected', true);
                    $('#' + self.Grid)[0].innerHTML = '';
                }
            },

            UpdateSelectorURL: function (claimid, employeeid) {
                var currentPagenumber = $('#gridClaims_header .cgridnew-currentpage').text();
                var claimName = $('#ClaimNameText').val();
                window.location.href = "/expenses/claimViewer.aspx?claimid=" + claimid + "&claimSelector=true&pageNumber=" + currentPagenumber + "&claimant=" + employeeid + "&claimName=" + claimName + "&filterValue=" + $('#gridClaims_Filter').val();
                return false;
            },

            SearchClaimName: function (claimName)
            {
                SEL.ClaimSelector.UpdateURL(false, claimName);
                var domIDs = SEL.ClaimSelector.DomIDs;
                var gridElement = $('#' + domIDs.Grid)[0];
                $.ajax({
                    type: "POST",
                    url: window.appPath + "/shared/webServices/svcClaim.asmx/GetApprovedGridForClaimSelector",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    data: "{ 'employeeId': '" + 0 + "', 'accountId': '" + CurrentUserInfo.AccountID + "', 'claimName': '" + claimName + "', 'selectable': " + SEL.ClaimSelector.Selectable + "}",
                    success: function (data)
                    {
                        gridElement.innerHTML = data.d[1];
                        SEL.Grid.updateGrid(data.d[0]);
                    },
                    error: function (xmlHttpRequest, textStatus, errorThrown)
                    {
                        SEL.Common.WebService.ErrorHandler(errorThrown);
                    }
                });

                return false;
            },

            SearchEmployeeNumber: function (employeeId)
            {
                SEL.ClaimSelector.UpdateURL(true, employeeId);
                var domIDs = SEL.ClaimSelector.DomIDs;
                var gridElement = $('#' + domIDs.Grid)[0];
                if (employeeId === '0' || employeeId === '')
                {
                    gridElement.innerHTML = '';
                }
                else
                {
                    $.ajax({
                        type: "POST",
                        url: window.appPath + "/shared/webServices/svcClaim.asmx/GetApprovedGridForClaimSelector",
                        dataType: "json",
                        contentType: "application/json; charset=utf-8",
                        data: "{ 'employeeId': '" + employeeId + "', 'accountId': '" + CurrentUserInfo.AccountID + "', 'claimName': '', 'selectable': " + SEL.ClaimSelector.Selectable + "}",
                        success: function (data)
                        {
                            gridElement.innerHTML = data.d[1];
                            SEL.Grid.updateGrid(data.d[0]);
                        },
                        error: function (xmlHttpRequest, textStatus, errorThrown)
                        {
                            SEL.Common.WebService.ErrorHandler(errorThrown);
                        }
                    });
                }

                return false;
            },

            SearchEmployee: function ()
            {
                SEL.ClaimSelector.EnableRequiredFieldValidators();

                if (validateform('vgClaimSelector') === false)
                    return false;

                var self = SEL.ClaimSelector;

                if (self.ClaimName === '')
                {
                    self.SearchEmployeeNumber(self.EmployeeId);
                }
                else
                {
                    self.SearchClaimName(self.ClaimName);
                }

                return false;
            },

            GetParameterValues: function (param) {
                var url = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
                for (var i = 0; i < url.length; i++) {
                    var urlparam = url[i].split('=');
                    if (urlparam[0] == param) {
                        return urlparam[1];
                    }
                }
            },

            UpdateURL: function (fromEmployee, valueToUpdate) {

                if (SEL.ClaimSelector.GetParameterValues("claimSelector") == "true") {

                    var pageNumber = SEL.ClaimSelector.GetParameterValues("pageNumber");
                    var filterValue = SEL.ClaimSelector.GetParameterValues("filterValue");
                    var url;
                    if (fromEmployee === true) {
                        url = "/expenses/claimSelector.aspx?claimSelector=true&claimant=" + valueToUpdate + "&pageNumber=" + pageNumber + "&claimName=&filterValue=" + filterValue;
                        if ((/MSIE\s/.test(navigator.userAgent) && parseFloat(navigator.appVersion.split("MSIE")[1]) < 10) && (location.pathname + location.search) !== url) {
                            location.href = url;
                        } else {
                            window.history.pushState({ path: url }, "", url);
                        }
                    }
                    else {
                        url = "/expenses/claimSelector.aspx?claimSelector=true&claimant=&pageNumber=" + pageNumber + "&claimName=" + valueToUpdate + "&filterValue=" + filterValue;
                        if ((/MSIE\s/.test(navigator.userAgent) && parseFloat(navigator.appVersion.split("MSIE")[1]) < 10) && (location.pathname + location.search) !== url) {
                            location.href = url;
                        } else {
                            window.history.pushState({ path: url }, "", url);
                        }
                    }
                }
            },

            ClickPager: function (pageNumber) {
                var element = $(".cgridnew-pager")[pageNumber];
                if (element != 'undefined' && element != null) {
                    $(".cgridnew-pager")[pageNumber].click();
                }
                else {
                    setTimeout(function () {
                        SEL.ClaimSelector.ClickPager(pageNumber);//Need to wait until grid loads
                    }, 3000);
                }
            },

            FilterGrid: function () {
                if ($("#gridClaims_Filter").length != 0) {
                    var filterValue = SEL.ClaimSelector.GetParameterValues('filterValue');
                    filterValue = filterValue == 'undefined' ? '' : filterValue;
                    $('#gridClaims_Filter').val(filterValue);
                    document.getElementById('gridClaims_FilterLink').click();
                }
                else {
                    setTimeout(function () {
                        SEL.ClaimSelector.FilterGrid();//Need to wait until grid loads
                    }, 3000);
                }
            },

            setEmployee: function (employeeId) {
                SEL.Data.Ajax({
                    url: window.appPath + "/shared/webServices/svcClaim.asmx/getEmployeeById",
                    timeout: 20000,
                    data: { employeeId: employeeId },
                    success: function (data) {
                        $('#ClaimantText').val(data.d.FullNameUsername);
                    },
                });
                return false;
            },

        };
    }

    if (window.Sys && window.Sys.loader)
    {
        window.Sys.loader.registerScript(scriptName, null, execute);
    }
    else
    {
        execute();
    }
}
)(SEL, CurrentUserInfo, $g, $f, moduleNameHTML, moduleNameHTML);
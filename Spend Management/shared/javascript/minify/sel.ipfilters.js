(function () {
    var scriptName = "IPFilters";
    function execute() {
        SEL.registerNamespace("SEL.IPFilters");
        SEL.IPFilters =
        {
            CurrentActiveIPFilterID: null,
            TxtIPAddressDomID: null,
            ChkActiveDomID: null,
            TxtDescriptionDomID: null,
            MdlWindowDomID: null,
            ModalMessageIPFilterNotUnique: "The IP Address you have entered already exists",
            ModalMessageIPFilterNotValid: "The IP Address you have entered is not valid",
            ModalMessageConfirmIPFilterDelete: "Are you sure you wish to delete the selected IP Filter?",

            SaveIPFilter: function () {

                var ipAddress = $g(SEL.IPFilters.TxtIPAddressDomID);
                var ipDesc = $g(SEL.IPFilters.TxtDescriptionDomID);
                var ipActive = $g(SEL.IPFilters.ChkActiveDomID);

                if (ipAddress !== undefined && ipDesc !== undefined && ipActive !== undefined) {
                    if (SEL.Common.ValidateForm(null) === true) {
                        Spend_Management.svcIPFilters.saveIPFilter(SEL.IPFilters.CurrentActiveIPFilterID, ipAddress.value, ipDesc.value, ipActive.checked, SEL.IPFilters.SaveIPFilterComplete, SEL.IPFilters.ShowErrorMessage);
                    }
                }
            },

            SaveIPFilterComplete: function (data) {

                data = parseInt(data);

                if (data === -1) {
                    SEL.IPFilters.ShowErrorMessage(SEL.IPFilters.ModalMessageIPFilterNotUnique);
                }
                else if (data === -2) {
                    SEL.IPFilters.ShowErrorMessage(SEL.IPFilters.ModalMessageIPFilterNotValid);
                }
                else {
                    SEL.Common.HideModal(SEL.IPFilters.MdlWindowDomID);

                    SEL.Grid.refreshGrid("gridIPFilters", 0);
                }
            },

            DeleteIPFilter: function (IPFilterID) {

                if (confirm(SEL.IPFilters.ModalMessageConfirmIPFilterDelete)) {
                    Spend_Management.svcIPFilters.deleteIPFilter(IPFilterID, SEL.IPFilters.DeleteIPFilterComplete, SEL.IPFilters.ShowErrorMessage);
                }
            },

            DeleteIPFilterComplete: function (data) {

                data = parseInt(data);

                if (data !== 0) {
                    SEL.Grid.refreshGrid("gridIPFilters", 0);
                }
            },

            EditIPFilter: function (IPFilterID) {

                Spend_Management.svcIPFilters.getIPFilterByID(IPFilterID, SEL.IPFilters.EditIPFilterComplete, SEL.IPFilters.ShowErrorMessage);
            },

            EditIPFilterComplete: function (data) {

                var ipAddress = $g(SEL.IPFilters.TxtIPAddressDomID);
                var ipDesc = $g(SEL.IPFilters.TxtDescriptionDomID);
                var ipActive = $g(SEL.IPFilters.ChkActiveDomID);
                SEL.IPFilters.CurrentActiveIPFilterID = data.IPFilterID;

                if (ipAddress !== undefined && ipDesc !== undefined && ipActive !== undefined) {
                    ipAddress.value = data.IPAddress;
                    ipDesc.value = data.Description;
                    ipActive.checked = data.Active;
                }

                SEL.Common.ShowModal(SEL.IPFilters.MdlWindowDomID);
            },

            ShowIPFiltersModal: function () {

                var ipAddress = $g(SEL.IPFilters.TxtIPAddressDomID);
                var ipDesc = $g(SEL.IPFilters.TxtDescriptionDomID);
                var ipActive = $g(SEL.IPFilters.ChkActiveDomID);
                SEL.IPFilters.CurrentActiveIPFilterID = 0;

                if (ipAddress !== undefined && ipDesc !== undefined && ipActive !== undefined) {
                    ipAddress.value = "";
                    ipDesc.value = "";
                    ipActive.checked = false;
                }

                SEL.Common.ShowModal(SEL.IPFilters.MdlWindowDomID);
            },

            ShowErrorMessage: function (data) {
                SEL.MasterPopup.ShowMasterPopup(data);
                return;
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

(function ($) {

    var torchinator = {
        Init: function (popupElement, options) {
            var self = this;
            self.options = options;
            self.popup = $(popupElement);
            self.popup.dialog({
                title: 'Save Configuration',
                autoOpen: false,
                modal: true,
                width: 550,
                closeOnEscape: false,
                draggable: false,
                resizable: false,
                stack: false,
                buttons: [
                    {
                        text: "OK",
                        click: function (e) {
                            e.preventDefault();
                            $(this).dialog('close');
                        }
                    }
                ],
                open: function () {
                    var self = this;
                },
                close: function () {
                    var self = this;
                }
            });

            // expose obj to data cache so peeps can call internal methods
            $.data($(popupElement)[0], 'torchinator', this);
            this.LoadReports();
        },

        Save: function () {
            var self = this;
            self.popup.dialog("option", "dialogClass", "formpanel");
            var popupInner = this.popup.dialog('open');

            // hide the dialog's titlebar
            popupInner.siblings(".ui-dialog-titlebar").hide();
        },

        Cancel: function () {
            var self = this;
            self.popup.dialog('close');
        },

        Torch: function () {
            var self = this;
        },

        LoadReports: function () {
        },

        LoadReportsSuccess: function (data) {

        },

        Service: function (path, method, params, sucessFunction, failFunction) {
            if (!path.endsWith('/')) {
                path = path + '/';
            };
            $.ajax({
                url: path + method,
                type: 'POST',
                data: params,
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: sucessFunction,
                error: failFunction
            });
        }
    };

    // expose
    $.torchinator = function (popupElement, options) {
        torchinator.Init(popupElement, $.extend($.torchinator.options, options));
        return this;
    };

    // options
    $.torchinator.options = {

        // the $.ajax timeout in MILLISECONDS! 
        AJAXTimeout: 1000,

        // the name of the current module
        moduleName: 'Expenses',

        // the torch project
        projectId: 0
    };
})(jQuery);

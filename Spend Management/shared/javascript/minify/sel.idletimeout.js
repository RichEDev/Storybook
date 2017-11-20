(function ($, win) {

    var idleTimeout = {
        init: function (element, options) {
            var self = this, elem;
            this.options = options;
            this.logoImage = '<img src="' + options.brandingLogo + '" alt="" />';
            this.initialSessionTimeoutMessage = '<span id="pulse" class="target" style="margin-left: 0px; margin-right: 0px;"><h3 class="menuitemtitle" id="pulseTitle" style="margin-top: 0px; ">Inactivity Warning!</h3><div class="ui-dialog-buttonpane ui-widget-content ui-helper-clearfix"></div><div id="container"> <div id="leftDiv" style="float:left; width: 90px;"><div class="menubar" style="width: 90px; float: left;text-align: center; height: 80px;background-image: none;padding-left: 0px;border: 1px solid;  border-radius: 4px; border-color: #aaaaaa" ></br><span style="font-size: 32px;font-weight:bold;"><span id="sessionTimeoutCountdown">' + this.options.warningLength + '</span></span></br><span>seconds</span></div></div>  <div id="rightDiv" style="float:left; width: 250px;"><div class="menuitemdescription" style="float: left;padding-left:25px;">You will be logged out of ' + self.options.moduleName + ' shortly due to inactivity.  If you are still working press the button below.</div></div></div>';
            this.sessionTimeoutCountdownId = 'sessionTimeoutCountdown';
            this.expiredMessage = 'Your session has expired.  You are being logged out for security reasons.';
            this.warning = elem = $(element);
            this.warning.html(this.initialSessionTimeoutMessage);
            this.warning.dialog({
                title: 'Message from ' + this.options.moduleName,
                autoOpen: false,
                modal: true,
                width: 400,
                closeOnEscape: false,
                draggable: false,
                resizable: false,
                stack: false,
                buttons: [
                    {
                        text: "OK",
                        id: 'btnOkSessionTimeOut',
                        click: function (e) {
                            e.preventDefault();
                            $(this).dialog('close');
                        }
                    }
                ],
                open: function () {
                    $('#btnOkSessionTimeOut').html('<img src=/shared/images/buttons/ok.png\>');
                    // scrollbar fix for IE
                    var that = this;

                    $('#' + that.sessionTimeoutCountdownId).html($.idleTimeout.options.warningLength);
                    var overlay = $('.ui-widget-overlay');
                    overlay.fadeTo(($.idleTimeout.options.warningLength * 1000) - 2000, 0.97);
                },
                close: function () {
                    // reset overflow
                    win.clearInterval(self.countdown); // stop the countdown
                    $('#' + self.sessionTimeoutCountdownId).html(self.options.warningLength);
                    self.countdownOpen = false; // stop countdown
                    self._startTimer(); // start up the timer again
                    self._keepAlive(false); // ping server
                }
            });

            this.countdownOpen = false;
            this.failedRequests = this.options.failedRequests;
            this._startTimer();

            // expose obj to data cache so peeps can call internal methods
            $.data(elem[0], 'idletimout', this);

            // start the idle timer
            $.idleTimer(this.options.idleAfter * 1000);

            // once the user becomes idle
            $(document).bind("idle.idleTimer", function () {

                // if the user is idle and a countdown isn't already running
                if ($.data(document, 'idleTimer') === 'idle' && !self.countdownOpen) {
                    self._stopTimer();
                    self.countdownOpen = true;
                    self._idle();
                }
            });

            $('.ui-dialog').keydown(function (event) {
                if (event.keyCode === $.ui.keyCode.ENTER) {
                    self.warning.dialog('close');
                    return false;
                }
            });

        },

        _idle: function () {
            var self = this,
                counter = this.options.warningLength;
            this.warning.dialog("option", "dialogClass", "formpanel");

            // fire the onIdle function
            var dialogInner = this.warning.dialog('open');

            // hide the dialog's titlebar
            dialogInner.siblings(".ui-dialog-titlebar").hide();

            // forcefully set the z-index of the modal (and associated overlay - should always be the last one, since it was created last) to be higher than anything else on the page
            var dialog = dialogInner.parents(".ui-dialog");
            var overlay = dialog.nextAll(".ui-widget-overlay").last();
            var highestZIndex = SEL.Common.GetHighestZIndexInt();
            overlay.zIndex(highestZIndex + 1);
            dialog.zIndex(highestZIndex + 2);

            $('.ui-dialog-buttonpane > button:last').focus();

            // set inital value in the countdown placeholder

            // create a timer that runs every second
            this.countdown = win.setInterval(function () {
                if (--counter === 0) {
                    var logoutFail = function (data) {
                        if (data != null && data.responseText != null && data.responseText != '' && data.responseText.substr(0, 2) === 'OK') {
                            logoutSuccess({ 'd': responseText });
                        };

                        self.failedRequests--;
                    };
                    var logoutSuccess = function (data) {
                        window.clearInterval(self.countdown);
                        self.warning.html(self.expiredMessage);
                        self.warning.dialog('destroy');
                        self.warning.html(counter);

                        if (data != null && data.d != null && data.d.length > 3 && data.d.substr(3) != 'null')
                        {
                            document.location = data.d.substr(3);
                        }
                        else
                        {
                            location.reload();
                        }
                    };
                    var ResetSession = function () {

                    };

                    var params = new ResetSession();
                    self._Service(self.options.killSessionURL, 'ResetSession', params, logoutSuccess, logoutFail);
                    counter = self.options.warningLength;
                } else {
                    $('#' + self.sessionTimeoutCountdownId).html(counter);
                }
            }, 1000);
        },
        _Service: function (path, method, params, sucessFunction, failFunction) {
            if (!path.endsWith('/')) {
                path = path + '/';
            }
            $.ajax({
                url: path + method,
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',

                success: sucessFunction,
                error: failFunction
            });
        },

        _startTimer: function () {
            var self = this;
            var pollingInterval = this.options.pollingInterval;

            this.timer = win.setTimeout(function () {
                self._keepAlive();
            }, pollingInterval * 1000);
        },

        _stopTimer: function () {
            // reset the failed requests counter
            this.failedRequests = this.options.failedRequests;
            win.clearTimeout(this.timer);
        },

        _keepAlive: function (recurse) {
            var self = this;

            if (typeof recurse === "undefined") {
                recurse = true;
            }

            // if too many requests failed, abort
            if (!this.failedRequests) {
                this._stopTimer();
                return;
            }
            var pingFail = function () {
                //location.reload();
            };
            var pingSuccess = function () {
                if (recurse) {
                    self._startTimer();
                }
            };
            self._Service(self.options.keepAliveURL, 'PingSession', null, pingSuccess, pingFail);
        }
    };

    // expose
    $.idleTimeout = function (element, options) {
        idleTimeout.init(element, $.extend($.idleTimeout.options, options));
        return this;
    };

    // options
    $.idleTimeout.options = {
        // number of seconds after user is idle to show the warning
        warningLength: 10,

        // url to call to keep the session alive while the user is active
        keepAliveURL: window.appPath + "/shared/webServices/SvcSession.asmx/",

        // url to webservice that will kill the session
        killSessionURL: window.appPath + "/shared/webServices/SvcSession.asmx/",

        // the response from keepAliveURL must equal this text:
        serverResponseEquals: "OK",

        // user is considered idle after this many seconds.  10 minutes default
        idleAfter: 30,

        // a polling request will be sent to the server every X seconds
        pollingInterval: 180,

        // number of failed polling requests until we abort this script
        failedRequests: 5,

        // the $.ajax timeout in MILLISECONDS! 
        AJAXTimeout: 1000,

        // the name of the current module
        moduleName: 'Expenses',

        // the name of the image to use for the branding logo
        brandingLogo: window.appPath + '/shared/images/branding/'
    };
})(jQuery, window);

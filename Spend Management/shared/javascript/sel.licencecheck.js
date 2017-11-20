(function () {
    var scriptName = "DVLAConsent";

    function execute() {
        SEL.registerNamespace("SEL.DVLAConsent");

        SEL.DVLAConsent =
        {
            LoaderImagePath: null,
            PortalUrl: null,

            Driver: null,
            ValidLicence: false,
            EmployeeHasHelpAndSupportAccess: null,
            IntervalTimer: null,
            IntervalFrequency: 5000,
            IntervalAttempts: 0,
            IntervalAttemptsMaximum: 120,
            LookupIntervalFrequency: 10000,
            DomIdentifiers: {
                Firstname: null,
                Surname: null,
                Email: null,
                DrivingLicenceNumber: null,
                DateOfBirth: null,
                Sex: null,
                Middlename: null,

                ConfirmationFields: {
                    Firstname: null,
                    Middlename: null,
                    Surname: null,
                    Email: null,
                    DrivingLicenceNumber: null,
                    DateOfBirth: null,
                    Sex: null
                },

                Modals: {
                    ConsentPortalInformationModal: null,
                    DetailConfirmationModal: null,
                    ValidationWarningsModal: null
                }
            },

            ConsentResponseCodeDescription: {
                312: "<p class ='responseMessage'>The driving licence number you have entered already exists on the consent portal. Please ensure you have entered this correctly.</p><p>If the driving licence number you have entered matches the number shown on your licence, please contact your administrator.</p>",
                313: "<p class ='responseMessage'>The email address you have entered already exists on the consent portal. Please ensure you have entered this correctly.</p><p>If you are still unable to use this email address, please contact your administrator.</p>",
                301: "<p class ='responseMessage'>The driving licence number you have provided is not valid. Please ensure you have entered this correctly.</p><p>If the driving licence number you have entered matches the number shown on your licence, please contact your administrator.</p>",
                000: "<p class ='responseMessage'>Could not submit the request.</p>",
                314: "<p class ='responseMessage'>We are unable to add your driver details at this time. We appreciate your patience, please try again later.",
                401: "<p class ='responseMessage'>The email address that you have provided is not valid. Please provide an alternative email address.</p>",
                504: "<p class ='responseMessage'>We are unable to process your request. Our technical team has been notified and we appreciate your patience. Please try again later.</p>",
                506: "<p class ='responseMessage'>We are unable to process your request. Our technical team has been notified and we appreciate your patience. Please try again later.</p>",
                501: "<p class ='responseMessage'>We are unable to process your request. Your account administrator has been notified and we appreciate your patience. Please try again later.</p>"
            },
     
            SetupDialogs: function () {
                SEL.DVLAConsent.DomIdentifiers.Modals.ConsentPortalInformationModal = $("#ConsentPortalInformationModal");
                SEL.DVLAConsent.DomIdentifiers.Modals.DetailConfirmationModal = $("#DetailConfirmationModal");
                SEL.DVLAConsent.DomIdentifiers.Modals.ValidationWarningsModal = $("#ValidationWarningsModal");

                SEL.DVLAConsent.SetupConfirmationDialog();
                SEL.DVLAConsent.SetupConsentPortalInfoDialog();
                SEL.DVLAConsent.SetupValidationWarningsModal();
            },

            SetupValidationWarningsModal: function() {
                
                SEL.DVLAConsent.DomIdentifiers.Modals.ValidationWarningsModal.dialog({
                    autoOpen: false,
                    resizable: false,
                    title: "Message from " + window.moduleNameHTML,
                    width: 520,
                    modal: true,
                    buttons: [
                        {
                            text: "continue",
                            id: "btnContinue",
                            "class": "jQueryUIButton",
                            click: function () {
                                SEL.DVLAConsent.SubmitForm();
                                $(this).dialog("close");
                            }
                        }, {
                            text: "try again",
                            id: "btnTryAgain",
                            "class": "jQueryUIButton",
                            click: function () {
                                $(this).dialog("close");
                            }
                        }
                    ]
                });
            },

            HideModalCloseButton: function (modalIdentifier) {
                $(modalIdentifier).dialog("option", "dialogClass", "ui-no-close-button");
                $(modalIdentifier).dialog("option", "closeOnEscape", false);
            },

            ShowModalCloseButton: function (modalIdentifier) {
                $(modalIdentifier).dialog("option", "dialogClass", "");
                $(modalIdentifier).dialog("option", "closeOnEscape", true);
            },

            SetupConsentPortalInfoDialog: function () {
                $("#ConsentPortalInformationModal").dialog({
                    autoOpen: false,
                    resizable: false,
                    title: "Consent Portal Information",
                    width: 920,
                    modal: true,
                    buttons: [
                        {
                            text: 'next',
                            id: 'btnNextToSubmitConsent',
                            "class": "jQueryUIButton",
                            click: function () {
                                SEL.DVLAConsent.GenerateDVLAConsent();
                            }
                        },
                        {
                            text: 'cancel',
                            id: 'btnSubmitCancelToSubmitConsent',
                            "class": "jQueryUIButton",
                            click: function () {
                                SEL.DVLAConsent.SetupConsentPortalInfoDialog();
                                $(this).dialog('close');
                            }
                        }
                    ]
                });

                SEL.DVLAConsent.SetLoaderText("Please wait...", null, true);

                $("#consentInfoPlaceholder").addClass('centered');
            },

            SetupConfirmationDialog: function() {
                $("#DetailConfirmationModal").dialog({
                    autoOpen: false,
                    resizable: false,
                    title: "Driving Licence Detail Confirmation",
                    width: 920,
                    modal: true,
                    buttons: [
                        {
                            text: 'next',
                            id: 'btnNextToConfirmDetails',
                            "class": "jQueryUIButton",
                            click: function () {
                                SEL.DVLAConsent.GenerateDVLAConsent();
                                SEL.DVLAConsent.CloseConfirmationDialog();
                            }
                        },
                        {
                            text: 'cancel',
                            id: 'btnSubmitCancelConfirmDetails',
                            "class": "jQueryUIButton",
                            click: function() {
                                $(this).dialog('close');
                            }
                        }
                    ]
                });
            },

            SetLoaderText: function (primaryText, additionalText, showAjaxSpinner) {
                var markup = "";

                if (showAjaxSpinner === true) {
                    markup += '<img id="loader" src="' + SEL.DVLAConsent.LoaderImagePath + '" alt="Loading..."/>';
                }
                markup += '<p class="large">';

                if (primaryText === null) {
                    markup += "Please wait...</p>";
                }
                else {
                    markup += primaryText;
                }

                if (additionalText !== null) {
                    markup += "<p>" + additionalText + "</p>";
                }

                $("#consentInfoPlaceholder").html(markup);
            },

            SetButtons: function(buttons) {
                $(SEL.DVLAConsent.DomIdentifiers.Modals.ConsentPortalInformationModal).dialog("option", "buttons", buttons);
            },

            RemoveAllButtons: function() {
                $(SEL.DVLAConsent.DomIdentifiers.Modals.ConsentPortalInformationModal).dialog("option", "buttons", []);
            },

            CopyFieldsIntoConfirmationDialog: function() {
                var firstname = $("#" + SEL.DVLAConsent.DomIdentifiers.Firstname).val();
                var surname = $("#" + SEL.DVLAConsent.DomIdentifiers.Surname).val();
                var email = $("#" + SEL.DVLAConsent.DomIdentifiers.Email).val();
                var licence = $("#" + SEL.DVLAConsent.DomIdentifiers.DrivingLicenceNumber).val();
                var dateofbirth = $("#" + SEL.DVLAConsent.DomIdentifiers.DateOfBirth).val();
                var sex = $("#" + SEL.DVLAConsent.DomIdentifiers.Sex + " :selected").text();
                var middlename = $("#" + SEL.DVLAConsent.DomIdentifiers.Middlename).val();
                
                $("#" + SEL.DVLAConsent.DomIdentifiers.ConfirmationFields.Firstname).val(firstname);
                $("#" + SEL.DVLAConsent.DomIdentifiers.ConfirmationFields.Middlename).val(middlename);
                $("#" + SEL.DVLAConsent.DomIdentifiers.ConfirmationFields.Surname).val(surname);
                $("#" + SEL.DVLAConsent.DomIdentifiers.ConfirmationFields.Email).val(email);
                $("#" + SEL.DVLAConsent.DomIdentifiers.ConfirmationFields.DrivingLicenceNumber).val(licence);
                $("#" + SEL.DVLAConsent.DomIdentifiers.ConfirmationFields.DateOfBirth).val(dateofbirth);
                $("#" + SEL.DVLAConsent.DomIdentifiers.ConfirmationFields.Sex).val(sex);
            },

            OpenConfirmationDialog: function () {
                SEL.DVLAConsent.CopyFieldsIntoConfirmationDialog();
                $(SEL.DVLAConsent.DomIdentifiers.Modals.DetailConfirmationModal).dialog("open");
            },

            CloseConfirmationDialog: function() {
                $(SEL.DVLAConsent.DomIdentifiers.Modals.DetailConfirmationModal).dialog("close");
            },

            OpenConsentPortalInfoDialog: function() {
                $(SEL.DVLAConsent.DomIdentifiers.Modals.ConsentPortalInformationModal).dialog("open");
            },

            CloseConsentPortalInfoDialog: function() {
                $(SEL.DVLAConsent.DomIdentifiers.Modals.ConsentPortalInformationModal).dialog("close");
            },

            OpenValidationWarningModal: function() {
                $(SEL.DVLAConsent.DomIdentifiers.Modals.ValidationWarningsModal).dialog("open");
            },

            CloseValidationWarningModal: function () {
                $(SEL.DVLAConsent.DomIdentifiers.Modals.ValidationWarningsModal).dialog("close");
            },

            GenerateDVLAConsent: function () {
                if (SEL.Common.ValidateForm("vgDvlaLookupConsentValidation") === false) {
                      return false;
                }

                var firstname = SEL.DVLAConsent.DomIdentifiers.Firstname;
                var lastname = SEL.DVLAConsent.DomIdentifiers.Surname;
                var email = SEL.DVLAConsent.DomIdentifiers.Email;
                var drivinglicencenumber = SEL.DVLAConsent.DomIdentifiers.DrivingLicenceNumber;
                var dateofbirth = SEL.DVLAConsent.DomIdentifiers.DateOfBirth;
                var sex = SEL.DVLAConsent.DomIdentifiers.Sex;
                var middlename = SEL.DVLAConsent.DomIdentifiers.Middlename;

                SEL.DVLAConsent.ValidateLicenceNumber(firstname, lastname, drivinglicencenumber, dateofbirth, email, sex, middlename);

                return false;
            },

            SubmitForm: function () {
                var firstname = $("#" + SEL.DVLAConsent.DomIdentifiers.Firstname).val();
                var surname = $("#" + SEL.DVLAConsent.DomIdentifiers.Surname).val();
                var email = $("#" + SEL.DVLAConsent.DomIdentifiers.Email).val();
                var drivinglicencenumber = $("#" + SEL.DVLAConsent.DomIdentifiers.DrivingLicenceNumber).val();
                var dateofbirth = $("#" + SEL.DVLAConsent.DomIdentifiers.DateOfBirth).val();
                var sex = $("#" + SEL.DVLAConsent.DomIdentifiers.Sex).val();
                var middlename = $("#" + SEL.DVLAConsent.DomIdentifiers.Middlename).val();

                $.ajax({
                    type: "POST",
                    url: window.appPath + "/shared/webServices/svcLicenceCheck.asmx/GetConsentPortalAccess",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify({
                        "firstName": firstname,
                        "surname": surname,
                        "email": email,
                        "drivingLicencePlate": drivinglicencenumber,
                        "dateOfBirth": dateofbirth,
                        "sex": sex,
                        "middleName": middlename
                    }),
                    success: function (data) {
                        SEL.DVLAConsent.EmployeeCanRaiseSupportTicket(data); 
                    },
                    error: function (xmlHttpRequest, textStatus, errorThrown) {
                        SEL.Common.WebService.ErrorHandler(errorThrown);
                    }
                });
            },

            OpenSavedDrivingLicence: function(data) {
                SEL.Data.Ajax({
                    serviceName: "svcCustomEntities",
                    methodName: "GetFormSelectionAttributeMappedEditFormId",
                    data: { entityId: data.EntityId, viewId: data.ViewId, id: data.RecordId },
                    success: function () {
                        window.location = "/shared/aeentity.aspx?viewid=" + data.ViewId + "&entityid=" + data.EntityId + "&formid=" + data.FormId + "&tabid=0&id=" + data.RecordId;
                    }
                });
            },

            GetLicenceData: function () {
                $.ajax({
                    type: "GET",
                    url: window.appPath + "/shared/webServices/svcLicenceCheck.asmx/GetLicenceData",
                    dataType: "json",
                    data: {
                        isLookupDateUpdated: SEL.DVLAConsent.Driver.EmployeeLookUpDateHasUpdated
                    },
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        if (data.d[0] != null || data.d[1] !== "") {
                            SEL.DVLAConsent.StopCheckingIfLookupHasBeenCompleted();
                            SEL.DVLAConsent.SetLoaderText("Please wait...", "Your driving licence check is in progress..", true);
                            SEL.DVLAConsent.GetLicenceDataComplete(data);
                        }
                    },
                    error: function (xmlHttpRequest, textStatus, errorThrown) {
                        SEL.Common.WebService.ErrorHandler(errorThrown);
                    }
                });
            },
            GetLicenceDataComplete: function(data) {
                var lookupResponseCode = data;
                var message = "";
                SEL.DVLAConsent.RemoveAllButtons();
                if (lookupResponseCode.d[0] !== "") {
                    if (lookupResponseCode.d[0] !== "504" &&
                        lookupResponseCode.d[0] !== "501" &&
                        lookupResponseCode.d[0] !== "506") {
                        message = "<p class='responseMessage'>We could not perform a check for your driving licence with the details provided. Please contact your administrator.";
                        if (lookupResponseCode.d[1] === true) {
                            SEL.DVLAConsent.SetButtons([
                                {
                                    text: "raise a support ticket",
                                    id: "btnRaiseSupportTicket",
                                    "class": "jQueryUIButton",
                                    click: function() {
                                        window.location="/shared/helpAndSupportTicketNew.aspx?TicketType=2&Subject=";
                                    }
                                },
                                {
                                    text: "cancel",
                                    id: "btnCancel",
                                    "class": "jQueryUIButton",
                                    click: function () {
                                        SEL.DVLAConsent.SetupConsentPortalInfoDialog();
                                        $(this).dialog('close');
                                    }
                                }
                            ]);
                        } else {
                            SEL.DVLAConsent.SetButtons([
                                {
                                    text: "cancel",
                                    id: "btnCancel",
                                    "class": "jQueryUIButton",
                                    click: function () {
                                        $(this).dialog('close');
                                    }
                                }
                            ]);
                        }
                    } else {
                        message = SEL.DVLAConsent.ConsentResponseCodeDescription[lookupResponseCode.d[0].replace(/\s+/g, "")];
                        {
                            SEL.DVLAConsent.SetButtons([
                                {
                                    text: "cancel",
                                    id: "btnCancel",
                                    "class": "jQueryUIButton",
                                    click: function () {
                                        $(this).dialog('close');
                                    }
                                }
                            ]);
                        }
                    }
                    $("#consentInfoPlaceholder").removeClass('centered');
                    $("#consentInfoPlaceholder").html(message);
                    }
                else
                {
                    if (lookupResponseCode.d[2] != null) {
                        SEL.DVLAConsent.SetLoaderText("Finished!", "We've added your driving licence to Expenses. If you'd like to check it, click the <strong>view driving licence</strong> button below or click <strong>finish</strong> to go back to the home page.", false);

                        SEL.DVLAConsent.SetButtons([
                            {
                                text: "view driving licence",
                                id: "btnViewLicence",
                                "class": "jQueryUIButton",
                                click: function () {
                                    SEL.DVLAConsent.OpenSavedDrivingLicence(lookupResponseCode.d[2]);
                                }
                            },
                            {
                                text: "finish",
                                id: "btnFinish",
                                "class": "jQueryUIButton",
                                click: function () {
                                    window.location = "/home.aspx";
                                }
                            }
                        ]);

                    }
                }
            },

            EmployeeCanRaiseSupportTicket: function (driverData) {
                $.ajax({
                    type: "POST",
                    url: window.appPath + "/shared/webServices/svcLicenceCheck.asmx/EmployeeCanRaiseSupportTicket",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        SEL.DVLAConsent.EmployeeHasHelpAndSupportAccess = data.d;
                        SEL.DVLAConsent.GetConsentPortalAccessComplete(driverData);
                     },
                    error: function (xmlHttpRequest, textStatus, errorThrown) {
                        SEL.DVLAConsent.DomIdentifiers.EmployeeHasHelpAndSupportAccess = false;
                        SEL.Common.WebService.ErrorHandler(errorThrown);
                    }
                });
            },

            GetConsentPortalAccessComplete: function (data) {
                    SEL.DVLAConsent.Driver = data.d;
                    var employeeConsentDetails = data.d;
                    var message = "";
                    SEL.DVLAConsent.RemoveAllButtons();
                    if (employeeConsentDetails != null && (employeeConsentDetails.ResponseCode != null && employeeConsentDetails.ResponseCode !== "")) {
                        if (employeeConsentDetails.SecurityCode !== null && employeeConsentDetails.SecurityCode !== "" && employeeConsentDetails.ResponseCode === "312") {
                            message = "<p class ='responseMessage'>The driving licence number you have entered already exists on the consent portal. Changes to your details have not been updated.</p>";
                            message = message + "<p>Your previously recorded details have been re-sent to your registered email address.</p>";
                            message = message + "<p>Click <strong>next</strong> to look up your driving licence information now.</p>";

                            $("#consentInfoPlaceholder").removeClass('centered');
                            $("#consentInfoPlaceholder").html(message);
                            SEL.DVLAConsent.SetButtons([
                                {
                                    text: "next",
                                    id: "btnNextAfterTheError",
                                    "class": "jQueryUIButton",
                                    click: function() {
                                        SEL.DVLAConsent.SetupConsentPortalInfoDialog();

                                        SEL.DVLAConsent.SetLoaderText("Waiting for check consent...", null, true);

                                        SEL.DVLAConsent.RemoveAllButtons();

                                        SEL.DVLAConsent.StartCheckingIfConsentPortalHasBeenCompleted();
                                    }
                                },
                                {
                                    text: "cancel",
                                    id: "btnCancelAfterTheError",
                                    "class": "jQueryUIButton",
                                    click: function () {
                                        SEL.DVLAConsent.SetupConsentPortalInfoDialog();
                                        $(this).dialog('close');
                                    }
                                }
                            ]);
                        }
                        else {
                            var description = SEL.DVLAConsent.ConsentResponseCodeDescription[employeeConsentDetails.ResponseCode.replace(/\s+/g, "")];

                            if (employeeConsentDetails.ResponseCode === "314") {
                                if (SEL.DVLAConsent.EmployeeHasHelpAndSupportAccess) {
                                    SEL.DVLAConsent.SetButtons([
                                        {
                                            text: "raise a support ticket",
                                            id: "btnRaiseSupportTicket",
                                            "class": "jQueryUIButton",
                                            click: function() {
                                                window.location="/shared/helpAndSupportTicketNew.aspx?TicketType=2&Subject=";
                                            }
                                        },
                                        {
                                            text: "cancel",
                                            id: "btnCancel",
                                            "class": "jQueryUIButton",
                                            click: function() {
                                                SEL.DVLAConsent.SetupConsentPortalInfoDialog();
                                                $(this).dialog('close');
                                            }
                                        }
                                    ]);
                                }
                                else {
                                    SEL.DVLAConsent.SetButtons([
                                        {
                                            text: "cancel",
                                            id: "btnCancel",
                                            "class": "jQueryUIButton",
                                            click: function () {
                                                SEL.DVLAConsent.SetupConsentPortalInfoDialog();
                                                $(this).dialog('close');
                                            }
                                        }
                                    ]);

                                }

                            }
                            else {
                                SEL.DVLAConsent.SetButtons([
                                    {
                                        text: "cancel",
                                        id: "btnCancel",
                                        "class": "jQueryUIButton",
                                        click: function() {
                                            SEL.DVLAConsent.SetupConsentPortalInfoDialog();
                                            $(this).dialog('close');
                                        }
                                    }
                                ]);

                            }

                            $("#consentInfoPlaceholder").removeClass('centered');
                            $("#consentInfoPlaceholder").html(description);
                        }
                    }
                    else {
                        if (employeeConsentDetails !== null) {
                            SEL.DVLAConsent.PortalUrl = employeeConsentDetails.LicencePortalUrl + "/" + employeeConsentDetails.SecurityCode;

                            message = message + "<p class='responseMessage'>Thank you for submitting your details.</p>";
                            message = message + "<p class='responseMessage'>Click the <strong>next</strong> button to go to the portal where you will be able to provide your driving licence check consent.</p>";
                            message = message + "<p class='responseMessage'>We have emailed you with your secure key should you wish to finish at a later date.</p>";
                        }
                        if (message !== "") {
                            $("#consentInfoPlaceholder").removeClass('centered');
                            SEL.DVLAConsent.SetButtons([
                                {
                                    text: "next",
                                    id: "btnNextToConsentPortal",
                                    "class": "jQueryUIButton",
                                    click: SEL.DVLAConsent.OpenConsentPortal
                                },
                                {
                                    text: "cancel",
                                    id: "btnCancelToConsentPortal",
                                    "class": "jQueryUIButton",
                                    click: function () {
                                        SEL.DVLAConsent.SetupConsentPortalInfoDialog();
                                        $(this).dialog('close');
                                    }
                                }
                            ]);
                            SEL.DVLAConsent.ShowModalCloseButton(SEL.DVLAConsent.DomIdentifiers.Modals.ConsentPortalInformationModal);
                            $("#consentInfoPlaceholder").html(message);
                        }
                    }
                    SEL.DVLAConsent.OpenConsentPortalInfoDialog();
            },

            OpenConsentPortal: function () {
                if (SEL.DVLAConsent.PortalUrl !== null) {
                    window.open(SEL.DVLAConsent.PortalUrl, "_blank");

                    SEL.DVLAConsent.SetupConsentPortalInfoDialog();

                    SEL.DVLAConsent.SetLoaderText("Waiting for consent to be completed...", null, true);

                    SEL.DVLAConsent.RemoveAllButtons();

                    SEL.DVLAConsent.StartCheckingIfConsentPortalHasBeenCompleted();
                }
            },

            OpenAutolookupInfo: function () {
                SEL.DVLAConsent.SetupConsentPortalInfoDialog();

                SEL.DVLAConsent.SetLoaderText("Waiting for consent to be completed...", null, true);

                SEL.DVLAConsent.RemoveAllButtons();

                SEL.DVLAConsent.StartCheckingIfLookupHasBeenCompleted();
            },

            StartCheckingIfLookupHasBeenCompleted: function () {
                SEL.DVLAConsent.IntervalTimer = window.setInterval(SEL.DVLAConsent.GetLicenceData, SEL.DVLAConsent.LookupIntervalFrequency);
            },

            StopCheckingIfLookupHasBeenCompleted: function () {
                window.clearInterval(SEL.DVLAConsent.IntervalTimer);
            },

            StartCheckingIfConsentPortalHasBeenCompleted: function () {
                SEL.DVLAConsent.IntervalAttempts = 1;
                SEL.DVLAConsent.IntervalTimer = window.setInterval(SEL.DVLAConsent.HasUserProvidedConsent, SEL.DVLAConsent.IntervalFrequency);
            },

            StopCheckingIfConsentPortalHasBeenCompleted: function() {
                window.clearInterval(SEL.DVLAConsent.IntervalTimer);
            },

            HasUserProvidedConsent: function () {
                if (SEL.DVLAConsent.IntervalAttempts === SEL.DVLAConsent.IntervalAttemptsMaximum) {
                    SEL.DVLAConsent.StopCheckingIfLookupHasBeenCompleted();

                    SEL.DVLAConsent.SetButtons([
                        {
                            text: "cancel",
                            id: "btnCancel",
                            "class": "jQueryUIButton",
                            click: function () {
                                $(this).dialog("close");
                            }
                        }
                    ]);
                    SEL.DVLAConsent.ShowModalCloseButton(SEL.DVLAConsent.DomIdentifiers.Modals.ConsentPortalInformationModal);
                    $("#consentInfoPlaceholder").removeClass("centered");
                    $("#consentInfoPlaceholder").html("<p class='responseMessage'>We could not verify that your consent was completed, please try again after you have completed the consent process.</p>");

                    return;
                }

                if (SEL.DVLAConsent.Driver != null) {

                    SEL.DVLAConsent.IntervalAttempts++;

                    $.ajax({
                        type: "GET",
                        url: window.appPath + "/shared/webServices/svcLicenceCheck.asmx/HasUserProvidedConsent",
                        dataType: "json",
                        data: {
                            driverId: SEL.DVLAConsent.Driver.DriverId
                        },
                        contentType: "application/json; charset=utf-8",
                        success: function (response) {
                            if (response.d) {
                                var consentProvided = response.d;

                                if (consentProvided === true) {
                                    SEL.DVLAConsent.StopCheckingIfConsentPortalHasBeenCompleted();
                                    SEL.DVLAConsent.SetLoaderText("Please wait...", "We can see that you have finished providing consent. We will now automatically check and populate your driving licence information within Expenses.", true);
                                    SEL.DVLAConsent.GetLicenceData(SEL.DVLAConsent.Driver.EmployeeLookUpDateHasUpdated);
                                } 
                            }
                        },
                        error: function (xmlHttpRequest, textStatus, errorThrown) {
                            SEL.DVLAConsent.StopCheckingIfLookupHasBeenCompleted();
                            SEL.Common.WebService.ErrorHandler(errorThrown);
                        }
                    });

                }
            },

            ValidateLicenceNumber: function (firstName, surName, licenceNumber, dateOfBirth, email, sex, middleName) {
                SEL.DVLAConsent.ValidLicence = true;
                var firstname = $("#" + firstName).val().toUpperCase().replace(/^[ ]+|[ ]+$/g, '');
                var surname = $("#" + surName).val().toUpperCase().replace(/^[ ]+|[ ]+$/g, '');
                var middlename = $("#" + middleName).val().toUpperCase().replace(/^[ ]+|[ ]+$/g, '');
                var licencenumber = $("#" + licenceNumber).val().toUpperCase().replace(/^[ ]+|[ ]+$/g, '');
                var dateofbirth = $("#" + dateOfBirth).val();
                var firstNameSplit = firstname.substring(0, 1);
                var middleNameSplit = middlename.substring(0, 1);
                var sexValue = $("#" + sex).val();
                var errorMessage = "";

                var surnameToCheck;
                var surnameInLicenceNumber;
                var shorterSurname;

                if (surname.length > 4) {
                    surnameToCheck = surname.substring(0, 5);
                    surnameInLicenceNumber = licencenumber.substring(0, 5);
                } else {
                    surnameToCheck = surname.substring(0, surname.length);
                    surnameInLicenceNumber = licencenumber.substring(0, surname.length);
                    shorterSurname = licencenumber.substring(surname.length, 5);
                    if (isNaN(shorterSurname)) {
                        errorMessage = "surname";
                        SEL.DVLAConsent.ValidLicence = false;
                    }
                }

                if (surnameToCheck !== surnameInLicenceNumber) {
                    errorMessage = "surname";
                    SEL.DVLAConsent.ValidLicence = false;
                }

                if (firstNameSplit !== licencenumber.substring(11, 12)) {
                    if (errorMessage.length > 0) {
                        errorMessage = errorMessage + ", first name";
                    } else {
                        errorMessage = "first name";
                    }
                    SEL.DVLAConsent.ValidLicence = false;
                }

                if ((licencenumber.substring(12, 13) !== "9" || middlename.length > 0) && middleNameSplit !== licencenumber.substring(12, 13)) {
                    if (errorMessage.length > 0) {
                        errorMessage = errorMessage + ", middle name";
                    } else {
                        errorMessage = "middle name";
                    }
                    SEL.DVLAConsent.ValidLicence = false;
                }
                
                //DoB is represented in dd/mm/yyyy [Example :01/01/1980]
                //Sex Value = 2 "Female" ,Sex Value = 1 "male"  
                //7th letter in licencenumber represents gender(can be 0/1 for male , 5/6 for female)
                //For female first digit of month+5 -(5/6), male -(0/1)
                var dobSexValidation = true;
                if (sexValue === "2") {
                    if (licencenumber.substring(6, 7) != 5 && licencenumber.substring(6, 7) != 6) {

                        if (errorMessage.length > 0) {
                            errorMessage = errorMessage + ", sex";
                        } else {
                            errorMessage = "sex";
                        }
                        SEL.DVLAConsent.ValidLicence = false;
                    }

                    var licenceValueForSexAndDob = parseInt(dateofbirth.substring(3, 4)) + 5;
                    if (licenceValueForSexAndDob != licencenumber.substring(6, 7)) {
                        dobSexValidation = false;

                    }
                }
                   
                if (sexValue === "1") {
                    if ((licencenumber.substring(6, 7) != 0 && licencenumber.substring(6, 7) != 1)) {
                        if (errorMessage.length > 0) {
                            errorMessage = errorMessage + ", sex";
                        } else {
                            errorMessage = "sex";
                        }
                        SEL.DVLAConsent.ValidLicence = false;
                    }
                    if (parseInt(dateofbirth.substring(3, 4)) != licencenumber.substring(6, 7)) {
                            dobSexValidation = false;
                    }
                }
                
                //6 letter in licencenumber - third digit from year part(**Y*)
                //8 and 9th  letter in licencenumber - date part of DOB(DD)
                //9 letter in licencenumber - second digit of month part(*M)
                //10 letter in licencenumber - 2nd digit from year part(*Y**)

                if (dobSexValidation != true || dateofbirth.substring(0, 1) !== licencenumber.substring(8, 9) || dateofbirth.substring(1, 2) !== licencenumber.substring(9, 10) || dateofbirth.substring(4, 5) !== licencenumber.substring(7, 8) || dateofbirth.substring(8, 9) !== licencenumber.substring(5, 6) || dateofbirth.substring(9, 10) !== licencenumber.substring(10, 11)) {
                    if (errorMessage.length > 0) {
                        errorMessage = errorMessage + " and date of birth";
                    }
                    else {
                        errorMessage = "date of birth";
                    }
                    SEL.DVLAConsent.ValidLicence = false;
                }
                
                if (!SEL.DVLAConsent.ValidLicence) {
                    var modal = SEL.DVLAConsent.DomIdentifiers.Modals.ValidationWarningsModal;

                    var markup = "The driving licence number you have entered does not seem to match your " + errorMessage + ", please check that you have entered them correctly.";
                    $(modal).html(markup);

                    SEL.DVLAConsent.CloseConfirmationDialog();
                    SEL.DVLAConsent.OpenValidationWarningModal();

                }

                else {
                    SEL.DVLAConsent.RemoveAllButtons();
                    SEL.DVLAConsent.OpenConsentPortalInfoDialog();
                    SEL.DVLAConsent.SubmitForm(firstName, surName, licenceNumber, dateOfBirth, email, sex, middleName);
                }

            },
            InitializeDatePicker: function () {
                // Setup the click events for calendar images
                // Create the date picker controls
                $(".dateField").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: '-116Y',
                    maxDate: 0,
                    yearRange: "-116:+0",
                    dateFormat: 'dd/mm/yy'
                }).attr("maxlength", 10);

                $(".dateCalImg, .timeCalImg").click(function () {
                    var inputControl = $(this).parent().prev().children().first();

                    if (inputControl.is(":disabled")) return false;

                    if (inputControl.hasClass("hasCalControl")) {
                        var pickerDiv = $(this).hasClass("dateCalImg") ? "#ui-datepicker-div" : "#ui-timepicker-div";

                        if ($(pickerDiv).css("display") === "none") {
                            inputControl.focus();
                        }
                        else {
                            if ($(pickerDiv).is(":animated") === false) {
                                $(pickerDiv).fadeOut(100);
                            }
                        }
                    }
                    else {
                        inputControl.focus();
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
}());

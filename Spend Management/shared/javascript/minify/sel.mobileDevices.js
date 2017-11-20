(function () {
    var scriptName = "MobileDevices";
    function execute() {
        SEL.registerNamespace("SEL.MobileDevices");
        SEL.MobileDevices =
        {
            ModalWindowNewOrEditDeviceNotPaired: "<strong>The Activation Key is:&nbsp;%PAIRINGKEY%</strong><br /><br />You now need to activate your device.<br /><br />To do this, launch the app, enter the 16-digit key into the Activation screen and tap the Activate button.",
            ModalWindowEditPairedDeviceString: "<br /><br /><strong>The device has been linked to this account.<br /><br /><br />The Activation Key is:&nbsp;%PAIRINGKEY%</strong><br /><br />The Activation Key cannot be used with a device other than the one it is currently assigned to.<br /><br />For new devices, register a new device to obtain a new Activation Key.",
            ModalDontHaveAppMsg: "<br /><br /><br /><strong>Don't have the app?</strong><br /><br />To download, open a browser and enter the website address:&nbsp;<a target=\"_blank\" style=\"text-decoration: none; cursor: hand;\" href=\"http://get.expenses360.mobi\">get.expenses360.mobi</a><br /><br />Or visit %APPSTORE% and search for <strong>Expenses360</strong><br />",
            ModalWindowDeviceRegisteredString: "<strong>The device has been registered successfully.</strong><br /><br /><br />",
            ModalWindowDomID: null,
            ModalWindowDeviceHeader: null,
            ModalWindowSpanInfoDevice: null,
            ModalWindowSpanDeviceInfoDiv: null,
            ModalWindowSpanDeviceInfoImg: null,
            ModalWindowImgDeviceInfoDomID: null,
            ModalWindowSpanPairingInfo: null,
            ModalWindowDeviceName: null,
            ModalWindowDeviceType: null,
            ModalWindowSaveButtonDomID: null,
            ModalInfoWindowDomID: null,
            MobileDeviceGridDomID: null,
            ModalDeviceNameValidatorID: null,
            ModalDeviceTypeValidatorID: null,
            ConfirmDeleteMobilePhoneString: "Are you sure you want to delete this mobile device?",
            ConfirmDeleteMobilePhoneStringPaired: "Deletion of this device will deactivate the mobile device's application.",
            DuplicateMobilePhoneString: "This mobile device has already been added to the system.",
            DeleteMobileDeviceFailedMsg: "Deletion of the mobile device failed.",
            NewMobileDeviceString: "New Mobile Device",
            EditMobileDeviceString: "Mobile Device: {0}",
            CurrentPairingKey: null,
            CurrentSerialKey: null,
            CurrentLoadType: null,
            CurrentMobileDeviceID: null,
            CurrentEmployeeID: null,
            LoadType: { New: 1, Edit: 2 },
            /// <summary>
            /// This should be attached to controls with tooltips and will fetch the tooltip content and attach a 'close' event, the first parameter should be either the database help_text helpid or a custom string to display
            /// </summary>     
            CloseModal: function () {
                SEL.Common.HideModal(SEL.MobileDevices.ModalWindowDomID);
                return false;
            },
            CloseInfoModal: function () {
                SEL.Common.HideModal(SEL.MobileDevices.ModalInfoWindowDomID);
                return false;
            },
            ShowInfoModal: function () {
                SEL.Common.ShowModal(SEL.MobileDevices.ModalInfoWindowDomID);
                return false;
            },
            MobileDeviceType: function () {
                this.DeviceTypeId = null;
                this.DeviceTypeDescription = null;
            },
            MobileDevice: function () {
                this.MobileDeviceID = null;
                this.EmployeeID = null;
                this.DeviceType = new SEL.MobileDevices.MobileDeviceType();
                this.DeviceName = null;
                this.PairingKey = null;
                this.SerialKey = null;
                this.IsPaired = null;
            },
            SetModalEditBox: function (message) {
                $g(SEL.MobileDevices.ModalWindow.SpanEditDevice).innerHTML = message;
            },
            GetNewPairingKey: function () {
                Spend_Management.svcMobileDevices.GetNewPairingKey(SEL.MobileDevices.CurrentEmployeeID, SEL.MobileDevices.GetNewPairingKeyComplete);
            },
            GetNewPairingKeyComplete: function (pairingKey) {
                SEL.MobileDevices.CurrentPairingKey = pairingKey;
                SEL.MobileDevices.SetupModal(null, null, pairingKey, null, null);
                SEL.Common.ShowModal(SEL.MobileDevices.ModalWindowDomID);
            },
            LoadMobileDeviceModal: function LoadMobileDeviceModal(loadType, mobileDeviceID) {
                SEL.MobileDevices.SetupModalClean();
                SEL.MobileDevices.CurrentLoadType = loadType;
                SEL.MobileDevices.CurrentMobileDeviceID = mobileDeviceID;
                if (loadType === SEL.MobileDevices.LoadType.New) {
                    SEL.MobileDevices.GetNewPairingKey(SEL.MobileDevices.CurrentEmployeeID, SEL.MobileDevices.GetNewPairingKeyComplete);
                    $g(SEL.MobileDevices.ModalWindowSaveButtonDomID).onclick = function () { SEL.MobileDevices.SaveMobileDevice(loadType); return false; };
                }
                else {
                    $g(SEL.MobileDevices.ModalWindowSaveButtonDomID).onclick = function () { SEL.MobileDevices.SaveMobileDevice(loadType, mobileDeviceID); return false; };
                    $g(SEL.MobileDevices.ModalWindowDeviceType).disabled = true;

                    Spend_Management.svcMobileDevices.GetMobileDeviceByID(mobileDeviceID, SEL.MobileDevices.LoadMobileDeviceModalComplete);
                }
            },
            LoadMobileDeviceModalComplete: function (mobileDevice) {
                if (mobileDevice.PairingKey === null || mobileDevice.PairingKey === '') {
                    SEL.MobileDevices.GetNewPairingKey(SEL.MobileDevices.CurrentEmployeeID, SEL.MobileDevices.GetNewPairingKeyComplete);
                }
                SEL.MobileDevices.SetupModal(mobileDevice.DeviceType.DeviceTypeId, mobileDevice.DeviceName, mobileDevice.PairingKey, mobileDevice.SerialKey, mobileDevice.IsPaired);
            },
            DeleteMobileDevice: function (mobileDeviceID, paired) {
                if (confirm(SEL.MobileDevices.ConfirmDeleteMobilePhoneString + (paired === 'True' ? "\n" + SEL.MobileDevices.ConfirmDeleteMobilePhoneStringPaired : "")) === true) {
                    Spend_Management.svcMobileDevices.DeleteMobileDevice(mobileDeviceID, SEL.MobileDevices.DeleteMobileDeviceComplete);
                }
            },
            DeleteMobileDeviceComplete: function (deleted) {
                if (deleted === true) {
                    SEL.MobileDevices.RefreshGrid();
                } else {
                    SEL.MasterPopup.ShowMasterPopup(SEL.MobileDevices.DeleteMobileDeviceFailedMsg);
                }
            },
            RefreshGrid: function () {
                SEL.Grid.refreshGrid(SEL.MobileDevices.MobileDeviceGridDomID, SEL.Grid.getCurrentPageNum(SEL.MobileDevices.MobileDeviceGridDomID));
            },
            SetupModalClean: function () {
                $g(SEL.MobileDevices.ModalWindowDeviceType).disabled = false;
                $g(SEL.MobileDevices.ModalWindowDeviceName).value = "";
                $ddlSetSelected(SEL.MobileDevices.ModalWindowDeviceType, 0);
                SEL.Common.Page_ClientValidateReset();
            },
            SetupModal: function (deviceType, deviceName, PairingKey, SerialKey, IsPaired) {
                var downloadArea = '';
                SEL.MobileDevices.CurrentPairingKey = PairingKey;
                SEL.MobileDevices.CurrentSerialKey = SerialKey;
                if (IsPaired === null || IsPaired === false) /// Device is not paired so continue to show the pairing key
                {
                    if (SEL.MobileDevices.CurrentLoadType === SEL.MobileDevices.LoadType.Edit) {
                        $g(SEL.MobileDevices.ModalWindowSpanDeviceInfoDiv).style.display = '';
                        var currentImage = $g(SEL.MobileDevices.ModalWindowSpanDeviceInfoImg).src;
                        $g(SEL.MobileDevices.ModalWindowSpanDeviceInfoImg).src = currentImage.replace(currentImage.substring(currentImage.lastIndexOf("/") + 1), "submenu_bg.jpg");

                        $.ajax({
                            type: "POST",
                            url: appPath + "/shared/webServices/svcMobileDevices.asmx/GetMobileActivateHelpText",
                            dataType: "json",
                            contentType: "application/json; charset=utf-8",
                            data: "{ deviceType: " + deviceType + " }",
                            success: function (data)
                            {
                                downloadArea = data;
                                $g(SEL.MobileDevices.ModalWindowSpanDeviceInfo).innerHTML = SEL.Common.ReplaceTokens(SEL.MobileDevices.ModalWindowNewOrEditDeviceNotPaired, { "%SERIALKEY%": SerialKey, "%PAIRINGKEY%": PairingKey }) + SEL.Common.ReplaceTokens(SEL.MobileDevices.ModalDontHaveAppMsg, { "%APPSTORE%": downloadArea.d[0] });
                                
                                $g(SEL.MobileDevices.ModalWindowSpanDeviceInfoImg).src = currentImage.replace(currentImage.substring(currentImage.lastIndexOf("/") + 1), downloadArea.d[1]);
                                SEL.Common.ShowModal(SEL.MobileDevices.ModalWindowDomID);
                            },
                            error: function (XMLHttpRequest, textStatus, errorThrown) {
                                alert('An error occurred with service call.\n\n' + errorThrown);
                            }
                        });

                    } else {
                        $g(SEL.MobileDevices.ModalWindowSpanDeviceInfoDiv).style.display = 'none';
                    }
                } else { /// Show the serial number of the device
                    $g(SEL.MobileDevices.ModalWindowSpanDeviceInfoDiv).style.display = '';
                    var currentImage = $g(SEL.MobileDevices.ModalWindowSpanDeviceInfoImg).src;
                    $g(SEL.MobileDevices.ModalWindowSpanDeviceInfoImg).src = currentImage.replace(currentImage.substring(currentImage.lastIndexOf("/") + 1), "submenu_bg.jpg");
                    $.ajax({
                        type: "POST",
                        url: appPath + "/shared/webServices/svcMobileDevices.asmx/GetMobileActivateHelpText",
                        dataType: "json",
                        contentType: "application/json; charset=utf-8",
                        data: "{ deviceType: " + deviceType + " }",
                        success: function (data) {
                            downloadArea = data;
                            $g(SEL.MobileDevices.ModalWindowSpanDeviceInfo).innerHTML = SEL.Common.ReplaceTokens(SEL.MobileDevices.ModalWindowEditPairedDeviceString, { "%SERIALKEY%": SerialKey, "%PAIRINGKEY%": PairingKey });
                            $g(SEL.MobileDevices.ModalWindowSpanDeviceInfoImg).src = currentImage.replace(currentImage.substring(currentImage.lastIndexOf("/") + 1), downloadArea.d[1]);
                            SEL.Common.ShowModal(SEL.MobileDevices.ModalWindowDomID);
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            alert('An error occurred with service call.\n\n' + errorThrown);
                        }
                    });
                }
                if (SEL.MobileDevices.CurrentLoadType === SEL.MobileDevices.LoadType.Edit) {
                    $g(SEL.MobileDevices.ModalWindowDeviceHeader).innerHTML = SEL.MobileDevices.EditMobileDeviceString.format(deviceName);
                }
                else {
                    $g(SEL.MobileDevices.ModalWindowDeviceHeader).innerHTML = SEL.MobileDevices.NewMobileDeviceString;
                }
                if (deviceName !== null) {
                    $g(SEL.MobileDevices.ModalWindowDeviceName).value = deviceName;
                }
                if (deviceType !== null) {
                    $ddlSetSelected(SEL.MobileDevices.ModalWindowDeviceType, null, deviceType);
                }
            },
            SaveMobileDeviceOnEnter: function () {
                SEL.MobileDevices.SaveMobileDevice(SEL.MobileDevices.CurrentLoadType, SEL.MobileDevices.CurrentMobileDeviceID);
            },
            SaveMobileDevice: function (loadType, mobileDeviceID) {
                SEL.MobileDevices.CurrentLoadType = loadType;
                if (SEL.Common.ValidateForm("vgAddEditMobile") === true) {
                    //mobileDeviceID, employeeID, deviceTypeID, deviceName, Pairingkey, serialKey, isPaired
                    var mobileDevice = new SEL.MobileDevices.MobileDevice();
                    if (loadType === SEL.MobileDevices.LoadType.New) {
                        mobileDevice.MobileDeviceID = 0;
                    } else {
                        mobileDevice.MobileDeviceID = mobileDeviceID;
                    }

                    mobileDevice.DeviceName = $g(SEL.MobileDevices.ModalWindowDeviceName).value;
                    mobileDevice.PairingKey = SEL.MobileDevices.CurrentPairingKey;
                    mobileDevice.SerialKey = SEL.MobileDevices.CurrentSerialKey;
                    mobileDevice.DeviceType = new SEL.MobileDevices.MobileDeviceType();
                    mobileDevice.DeviceType.DeviceTypeId = $ddlValue(SEL.MobileDevices.ModalWindowDeviceType);
                    mobileDevice.DeviceType.DeviceTypeDescription = '';
                    mobileDevice.IsPaired = false;
                    mobileDevice.EmployeeID = SEL.MobileDevices.CurrentEmployeeID;

                    Spend_Management.svcMobileDevices.SaveMobileDevice(mobileDevice, SEL.MobileDevices.SaveMobileDeviceComplete);
                    return false;
                }
                return true;
            },
            SaveMobileDeviceComplete: function (newMobileDeviceID) {
                if (newMobileDeviceID > 0) {
                    SEL.MobileDevices.RefreshGrid();
                    SEL.MobileDevices.CloseModal();
                    var currentImage = $g(SEL.MobileDevices.ModalWindowSpanDeviceInfoImg).src;
                    $g(SEL.MobileDevices.ModalWindowSpanDeviceInfoImg).src = currentImage.replace(currentImage.substring(currentImage.lastIndexOf("/") + 1), "submenu_bg.jpg");

                    if (SEL.MobileDevices.CurrentLoadType === SEL.MobileDevices.LoadType.New)
                    {
                        var deviceType = $g(SEL.MobileDevices.ModalWindowDeviceType).value;
                        
                        $.ajax({
                            type: "POST",
                            url: appPath + "/shared/webServices/svcMobileDevices.asmx/GetMobileActivateHelpText",
                            dataType: "json",
                            contentType: "application/json; charset=utf-8",
                            data: "{ deviceType: " + deviceType + " }",
                            success: function (data) {
                                var downloadArea = data;
                                $g(SEL.MobileDevices.ModalWindowSpanPairingInfo).innerHTML = SEL.MobileDevices.ModalWindowDeviceRegisteredString + SEL.Common.ReplaceTokens(SEL.MobileDevices.ModalWindowNewOrEditDeviceNotPaired, { "%SERIALKEY%": SEL.MobileDevices.CurrentSerialKey, "%PAIRINGKEY%": SEL.MobileDevices.CurrentPairingKey }) + SEL.Common.ReplaceTokens(SEL.MobileDevices.ModalDontHaveAppMsg, { "%APPSTORE%": downloadArea.d[0] });
                                $g(SEL.MobileDevices.ModalWindowImgDeviceInfoDomID).src = currentImage.replace(currentImage.substring(currentImage.lastIndexOf("/") + 1), downloadArea.d[1]);
                                SEL.MobileDevices.ShowInfoModal();
                            },
                            error: function (XMLHttpRequest, textStatus, errorThrown) {
                                alert('An error occurred with service call.\n\n' + errorThrown);
                            }
                        });
                    }
                }
                else if (newMobileDeviceID === -1) {
                    SEL.MasterPopup.ShowMasterPopup(SEL.MobileDevices.DuplicateMobilePhoneString);
                }
            },
            updateGridFilterEmployeeId: function (employeeId) {
                SEL.MobileDevices.CurrentEmployeeID = employeeId;
                var values = [];
                values.push(employeeId);
                SEL.Grid.updateGridQueryFilterValues(SEL.MobileDevices.MobileDeviceGridDomID, '4761f874-8923-4a3c-9a7c-2679fcde87ab', values, null); // update the employee ID in the mobile grid filter
            }
        };
    }

    if (window.Sys && Sys.loader) {
        Sys.loader.registerScript(scriptName, null, execute);
    }
    else {
        execute();
    }
}
)();

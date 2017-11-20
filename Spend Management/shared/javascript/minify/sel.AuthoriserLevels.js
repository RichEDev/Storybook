(function (SEL, $, $g) {
    var scriptName = "AuthoriseLevel";
    function execute() {
        SEL.registerNamespace("SEL.AuthoriserLevel");
        SEL.AuthoriserLevel = {
            Menu: {
                Save: function (id) {
                    if (validateform('AuthoriserLevel') == false) {
                        return;
                    }
                    var amount = document.getElementById(txtAmount).value;
                    var description = document.getElementById(txtDescription).value;
                    var id = authoriserLevelDetailId;
                    SEL.Data.Ajax({
                        serviceName: "svcAuthoriserLevel",
                        methodName: "SaveAuthoriserLevel",
                        data: {
                            authoriserLevelId: id,
                            amount: amount,
                            decription: description
                        },
                        success: function (response) {
                            var data = response.d;
                            if (data > 0) {
                                window.location = 'AuthoriserLevel.aspx';
                            }

                            if (data == -999) {
                                SEL.MasterPopup.ShowMasterPopup("The amount you have entered already exists.");
                            }

                            if (data == -998) {
                                SEL.MasterPopup.ShowMasterPopup("The description you have entered already exists.");
                            }


                            if (data <= 0 && data !== -999 && data !== -998) {
                                SEL.MasterPopup.ShowMasterPopup("Unable to save Authoriser Level. Please contact your system administrator.");
                            }


                        },
                        error: function () {
                            SEL.MasterPopup.ShowMasterPopup("Error.", "Message from Authoriser Level");
                        }
                    });
                },
                Delete: function (authoriserLevelDetailId) {

                    SEL.Data.Ajax({
                        serviceName: "svcAuthoriserLevel",
                        methodName: "CheckAuthoriserLevelAssignToEmployee",
                        timeout: 20000,
                        data: {
                            authoriserLevelDetailId: authoriserLevelDetailId
                        },
                        success: function (response) {
                            var data = response.d;
                            var message = 'Are you sure you wish to delete the selected Authoriser Level?';
                            var isAssignAuthoriserLevel = false;
                            if (data == true) {
                                message = 'This Authoriser Level is associated to one or more approvers, deleting this level will result in them being reset to None. Are you sure you wish to continue?';
                                isAssignAuthoriserLevel = true;
                            }
                            if (confirm(message)) {
                                SEL.Data.Ajax({
                                    serviceName: "svcAuthoriserLevel",
                                    methodName: "DeleteAuthoriserLevelDetail",
                                    timeout: 20000,
                                    data: {
                                        authoriserLevelDetailId: authoriserLevelDetailId,
                                        IsAssignAuthoriserLevel: isAssignAuthoriserLevel
                                    },
                                    success: function (response) {
                                        var data = response.d;
                                        if (data > 0) {
                                            SEL.AuthoriserLevel.Menu.RefreshGrid();
                                        }
                                        if (data == -1) {
                                            SEL.MasterPopup.ShowMasterPopup("Unable to delete Authoriser Level. Please contact your system administrator.");
                                        }

                                    },
                                    error: function () {
                                        SEL.MasterPopup.ShowMasterPopup("Error.", "Message from Authoriser Level");
                                    }
                                });
                            }
                        },
                        error: function () {
                            SEL.MasterPopup.ShowMasterPopup("Error.", "Message from Authoriser Level");
                        }
                    });
                    return;
                },

                CancelAuthoriserLevel: function () {
                    window.location = 'AuthoriserLevel.aspx';
                },

                RefreshGrid: function () {
                    SEL.Grid.refreshGrid('gridAuthoriserLevelDetailGrid', SEL.Grid.getCurrentPageNum('gridAuthoriserLevelDetailGrid'));
                },
                UpdateEmployee: function (employeeId) {
                    if (validateform('AuthoriserLevel') == false) {
                        return;
                    }
                    if (employeeId == '') {
                        SEL.MasterPopup.ShowMasterPopup("Please enter valid employee name");
                        return false;
                    }
                    SEL.Data.Ajax({
                        serviceName: "svcAuthoriserLevel",
                        methodName: "UpdateEmployeeForDefaultAuthoriser",
                        timeout: 20000,
                        data: {
                            employeeId: employeeId
                        },
                        success: function (response) {
                            var data = response.d;
                            if (data > 0) {
                                SEL.AuthoriserLevel.Menu.RefreshGrid();
                            }
                        },
                        error: function () {
                            SEL.MasterPopup.ShowMasterPopup("Error.", "Message from Authoriser Level");
                        }
                    });

                    return;
                }
            }
        }
    }
    if (window.Sys && window.Sys.loader) {
        window.Sys.loader.registerScript(scriptName, null, execute);
    }
    else {
        execute();
    }
}(SEL, jQuery, $g));
document.onkeydown = function (e) {
    e = e || window.event;
    if (e.keyCode == 27) {
        if ($g('ctl00_pnlMasterPopup').style.display == '') {
            SEL.MasterPopup.HideMasterPopup();
            return;
        }
        SEL.AuthoriserLevel.CloseModal();
    }
}
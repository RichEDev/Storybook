(function(SEL, moduleNameHTML, appPath)
{
    var scriptName = "approvalMatrices";

    function execute()
    {
        SEL.registerNamespace("SEL.ApprovalMatrices");
        SEL.ApprovalMatrices =
        {
            IDs:
            {
                MatrixId: null,
                MatrixLevelId: null
            },
            
            DomIDs:
            {
                Summary:
                {
                    Grid: null
                },
                
                General:
                {
                    MatrixName: null,
                    MatrixDescription: null,
                    DefaultApprover: null,
                    DefaultApproverKey: null,
                    DefaultApproverMandatoryValidator: null,
                    DefaultApproverComparisonValidator: null
                },
                
                Level:
                {
                    Modal: null,
                    Panel: null,
                    ApprovalLimit: null,
                    ApproverKey: null,
                    Approver: null,
                    Grid: null,
                    ApprovalLimitMandatoryValidator: null,
                    ApprovalLimitComparisonValidator: null,
                    ApproverMandatoryValidator: null,
                    ApproverComparisonValidator: null
                }
            },
            
            Messages:
            {
                ModalTitle: 'Message from ' + moduleNameHTML
            },
            
            SetupEnterKeyBindings: function()
            {
                // Base Save
                SEL.Common.BindEnterKeyForSelector('.primaryPage', SEL.ApprovalMatrices.Matrix.Save);
                // Level Save
                SEL.Common.BindEnterKeyForSelector('#' + SEL.ApprovalMatrices.DomIDs.Level.Panel + ' .formpanel .twocolumn', SEL.ApprovalMatrices.Level.Save);
                SEL.Common.BindEnterKeyForElement(SEL.ApprovalMatrices.DomIDs.Level.ApprovalLimit, SEL.ApprovalMatrices.Level.Save);
                SEL.Common.BindEnterKeyForElement(SEL.ApprovalMatrices.DomIDs.Level.Approver, SEL.ApprovalMatrices.Level.Save);
                $(document).keydown(function (e) {
                    if (e.keyCode === 27) // esc
                    {
                        e.preventDefault();
                        if ($g(SEL.ApprovalMatrices.DomIDs.Level.Panel).style.display == '')
                        {
                            SEL.ApprovalMatrices.Level.Cancel();
                            return;
                        }
                    }
                });
            },
            
            Matrix:
            {
                Delete: function(matrixId)
                {
                    if (confirm('Are you sure you want to delete this approval matrix?'))
                    {
                        $.ajax({
                            url: appPath + '/shared/webServices/ApprovalMatrix.asmx/DeleteMatrix',
                            type: 'POST',
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            data: "{ matrixId: " + matrixId + " }",
                            success: function(r)
                            {
                                if (r.d === -1)
                                {
                                    SEL.MasterPopup.ShowMasterPopup('This approval matrix cannot currently be deleted as it is assigned to one or more signoff stages.', SEL.ApprovalMatrices.Messages.ModalTitle);
                                    return;
                                }

                                SEL.ApprovalMatrices.Matrix.RefreshGrid();
                            },
                            error: function(xmlHttpRequest, textStatus, errorThrown)
                            {
                                SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                    'Message from ' + moduleNameHTML);
                            }
                        });
                    }
                },
                
                Save: function (saveFor)
                {
                    if (validateform('vgMain') === false)
                    {
                        return false;
                    }

                    var matrixname = $g(SEL.ApprovalMatrices.DomIDs.General.MatrixName).value;
                    var matrixdescription = $g(SEL.ApprovalMatrices.DomIDs.General.MatrixDescription).value;
                    var defaultApproverkey = $g(SEL.ApprovalMatrices.DomIDs.General.DefaultApproverKey).value;

                    if (defaultApproverkey === "")
                    {
                        SEL.MasterPopup.ShowMasterPopup('Please enter a Default approver.', SEL.ApprovalMatrices.Messages.ModalTitle);
                        return false;
                    }

                    $.ajax({
                        url: appPath + '/shared/webServices/ApprovalMatrix.asmx/SaveMatrix',
                        type: 'POST',
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        data: "{ matrixId: " + SEL.ApprovalMatrices.IDs.MatrixId + ", matrixName: \"" + matrixname + "\", matrixDescription: \"" + matrixdescription + "\", defaultApproverKey: \"" + defaultApproverkey + "\" }",
                        success: function (r)
                        {
                            if (r.d === -1)
                            {
                                SEL.MasterPopup.ShowMasterPopup('The Approval matrix name already exists.', SEL.ApprovalMatrices.Messages.ModalTitle);
                                return false;
                            }

                            SEL.ApprovalMatrices.IDs.MatrixId = r.d;
                            var levelGrid = SEL.Grid.getGridById("gridMatrixLevels");
                            if (levelGrid !== null && typeof levelGrid === "object")
                            {
                                levelGrid.filters[0].values1 = [r.d];
                            }
                            
                            switch (saveFor)
                            {
                                case 'matrix':
                                    document.location = "ApprovalMatrices.aspx";
                                    break;
                                case 'levels':
                                    changePage('Levels');
                                    break;
                            }
                        },
                        error: function (xmlHttpRequest, textStatus, errorThrown)
                        {
                            SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                'Message from ' + moduleNameHTML);
                        }
                    });
                },

                Cancel: function()
                {
                    document.location = "ApprovalMatrices.aspx";
                },

                RefreshGrid: function ()
                {
                    SEL.Grid.refreshGrid('gridMatrices', SEL.Grid.getCurrentPageNum('gridMatrices'));
                }
            },
            
            Level:
            {
                Focus: function ()
                {
                    if (SEL.ApprovalMatrices.IDs.MatrixId === 0)
                    {
                        if (!SEL.ApprovalMatrices.Matrix.Save('levels'))
                        {
                            return;
                        }
                    }
                    else
                    {
                        changePage('Levels');
                    }
                },
                
                Edit: function(levelId)
                {
                    var thisNs = SEL.ApprovalMatrices;
                    
                    thisNs.Level.Modal.Clear();
                    thisNs.IDs.MatrixLevelId = levelId;
                    
                    $.ajax({
                        url: appPath + '/shared/webServices/ApprovalMatrix.asmx/GetLevel',
                        type: 'POST',
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        data: "{ matrixId: " + thisNs.IDs.MatrixId + ", matrixLevelId: \"" + thisNs.IDs.MatrixLevelId + "\" }",
                        success: function(r)
                        {
                            $g(thisNs.DomIDs.Level.ApprovalLimit).value = r.d.ApprovalLimit;
                            $g(thisNs.DomIDs.Level.ApproverKey).value = r.d.ApproverKey;
                            $g(thisNs.DomIDs.Level.Approver).value = r.d.ApproverFriendlyName;

                            $("#divApprovalMatrixLevelTitle").html("Level: " + r.d.ApprovalLimit);
                            thisNs.Level.Modal.Show();

                            $g(thisNs.DomIDs.Level.ApprovalLimit).focus();
                        },
                        error: function(xmlHttpRequest, textStatus, errorThrown)
                        {
                            errorThrown = errorThrown + ' ' + xmlHttpRequest.responseText;
                            SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                'Message from ' + moduleNameHTML);
                        }
                    });

                },
                
                New: function ()
                {
                    if (SEL.ApprovalMatrices.IDs.MatrixId === 0)
                    {
                        if (!SEL.ApprovalMatrices.Matrix.Save('levels'))
                        {
                            return;
                        }
                    }

                    SEL.ApprovalMatrices.IDs.MatrixLevelId = 0;
                    SEL.ApprovalMatrices.Level.Modal.Clear();
                    $("#divApprovalMatrixLevelTitle").html("New Level");
                    SEL.ApprovalMatrices.Level.Modal.Show();
                    $g(SEL.ApprovalMatrices.DomIDs.Level.ApprovalLimit).focus();
                },
                
                Save: function()
                {
                    if (validateform('vgLevel') === false)
                    {
                        return;
                    }

                    var approvalLimit = $g(SEL.ApprovalMatrices.DomIDs.Level.ApprovalLimit).value;
                    var approverkey = $g(SEL.ApprovalMatrices.DomIDs.Level.ApproverKey).value;

                    if (approverkey === "")
                    {
                        SEL.MasterPopup.ShowMasterPopup('Please enter a valid Approver.', SEL.ApprovalMatrices.Messages.ModalTitle);
                        return;
                    }

                    $.ajax({
                        url: appPath + '/shared/webServices/ApprovalMatrix.asmx/SaveLevel',
                        type: 'POST',
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        data: "{ matrixId: " + SEL.ApprovalMatrices.IDs.MatrixId + ", matrixLevelId: \"" + SEL.ApprovalMatrices.IDs.MatrixLevelId + "\", approvalLimit: " + approvalLimit + ", approverKey:\"" + approverkey + "\" }",
                        success: function(r)
                        {
                            if (r.d === -1)
                            {
                                SEL.MasterPopup.ShowMasterPopup('The Approval limit already exists.', SEL.ApprovalMatrices.Messages.ModalTitle);
                                return;
                            }
                            
                            SEL.ApprovalMatrices.IDs.MatrixLevelId = r.d;
                            SEL.ApprovalMatrices.Level.RefreshGrid();
                            SEL.ApprovalMatrices.Level.Modal.Hide();
                        },
                        error: function(xmlHttpRequest, textStatus, errorThrown)
                        {
                            errorThrown = errorThrown + ' ' + xmlHttpRequest.responseText;
                            SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                'Message from ' + moduleNameHTML);
                        }
                    });
                },
                
                RefreshGrid: function ()
                {
                    SEL.Grid.refreshGrid('gridMatrixLevels', SEL.Grid.getCurrentPageNum('gridMatrixLevels'));
                },
                
                Cancel: function()
                {
                    SEL.ApprovalMatrices.Level.Modal.Hide();
                },
                
                Delete: function (matrixLevelId)
                {
                    if (confirm('Are you sure you want to delete this level?'))
                    {
                        SEL.ApprovalMatrices.IDs.MatrixLevelId = matrixLevelId;
                        $.ajax({
                            url: appPath + '/shared/webServices/ApprovalMatrix.asmx/DeleteLevel',
                            type: 'POST',
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            data: "{ matrixId: " + SEL.ApprovalMatrices.IDs.MatrixId + ", matrixLevelId: " + SEL.ApprovalMatrices.IDs.MatrixLevelId + " }",
                            success: function(r)
                            {
                                SEL.ApprovalMatrices.IDs.MatrixLevelId = 0;
                                SEL.ApprovalMatrices.Level.RefreshGrid();
                            },
                            error: function(xmlHttpRequest, textStatus, errorThrown)
                            {
                                errorThrown = errorThrown + ' ' + xmlHttpRequest.responseText;
                                SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                    'Message from ' + moduleNameHTML);
                            }
                        });
                    }
                },
                
                Modal:
                {
                    Show: function()
                    {
                        SEL.Common.ShowModal(SEL.ApprovalMatrices.DomIDs.Level.Modal);
                    },
                    
                    Hide: function()
                    {
                        SEL.Common.HideModal(SEL.ApprovalMatrices.DomIDs.Level.Modal);
                    },

                    Clear: function ()
                    {
                        var modalDoms = SEL.ApprovalMatrices.DomIDs.Level;
                        $g(modalDoms.ApprovalLimit).value = '';
                        $g(modalDoms.ApproverKey).value = '';
                        $g(modalDoms.Approver).value = '';

                        $g(modalDoms.ApprovalLimitMandatoryValidator).isvalid = true;
                        ValidatorUpdateDisplay($g(modalDoms.ApprovalLimitMandatoryValidator));
                        $g(modalDoms.ApprovalLimitComparisonValidator).isvalid = true;
                        ValidatorUpdateDisplay($g(modalDoms.ApprovalLimitComparisonValidator));
                        $g(modalDoms.ApproverMandatoryValidator).isvalid = true;
                        ValidatorUpdateDisplay($g(modalDoms.ApproverMandatoryValidator));
                        $g(modalDoms.ApproverComparisonValidator).isvalid = true;
                        ValidatorUpdateDisplay($g(modalDoms.ApproverComparisonValidator));
                    }
                }
            }
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
}(SEL, moduleNameHTML, appPath));
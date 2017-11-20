/* <summary>Claims Methods</summary> */
(function() {
    var scriptName = "claims";

    function execute() {
        SEL.registerNamespace("SEL.Claims");
        SEL.Claims =
        {
            Constants:
            {
                ValidEnvelopeLetters: ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'K', 'L', 'M', 'N', 'O', 'P', 'R', 'S', 'T', 'V', 'W', 'X', 'Y', 'Z']
            },
            General: {
                PageLoaded: false,
                claimId: null,
                statementId: null,
                transactionId: null,
                viewFilter: 0,
                enableReceiptAttachments: null,
                enableJourneyDetailsLink: null,
                enableCorporateCardsLink: null,
                viewType: null,
                displayDeclaration: null,
                symbol: null,
                gridName: null,
                IsSplitClaim: false,
                EnvelopeNumberMarkup: "",
                MissingEnvelopes: null,
                DaysEnvelopeCanBeMissing: 5,
                PartSubmittal: false,
                ExpediteClient: false
            },
            IDs: {
                modDeclarationId: null,
                modFlagsId: null,
                modValidationId: null,
                modEnvelopeMissingId: null,
                txtSurnameId: null,
                cmbFilterId: null,
                popupTransactionId: null,
                modUnallocatedId: null,
                ddlstStatementId: null,
                modReturnId: null,
                txtReturnReasonId: null,
                lblApproverId: null,
                UnsubmitClaimAsApproverReasonTextId: null,
                UnsubmitClaimAsApproverModalId: null,
                CurrentEnvelopeId: null,
                txtClaimNameId: null,
                txtClaimDescriptionId: null,
                ddlstBusinessMileageId: null,
                compBusinessMileageId: null,
                reqClaimID: null
            },

            Messages:
            {
                ApproverUnsubmitClaim:
                {
                    FailedClaimHasBeenInvolvedInSelStage: "This claim cannot be unsubmitted as it has either passed or is currently in an Expedite Stage - Scan & Attach or Validation.",
                    FailedItemsNotReturnedByOtherApprovers: "This claim cannot be unsubmitted as there are other approvers at this stage that have not returned all of their items on the claim.",
                    FailedItemsApproved: "This claim cannot be unsubmitted because it has already been approved.",
                    FailedItemsApprovedByOtherApprovers: "This claim cannot be unsubmitted as there are other approvers at this stage that have approved items on the claim.",
                    FailedClaimHasBeenPaidBeforeValidate: "This claim cannot be unsubmitted as items on the claim have already been paid.",
                    FailStartedApprovalProcess: "This claim cannot be unsubmitted as it has already started the approval process.",
                    FailUnsubmitClaimProcess: "This claim cannot be unsubmitted as it has been escalated to you by someone else."
    },
                Mobile:
                {
                    ConfirmDeleteItem: "Are you sure you wish to delete the selected mobile expense item?",
                    ConfirmDeleteJourney: "Are you sure you wish to delete the selected mobile journey?"
                }
            },


            ClaimViewer:
                {
                    RootClaimSelector: false,
                SubmittingClaim: false,
                ContinueAlthoughAuthoriserIsOnHoliday: false,
                ShowBusinessMileage: false,
                    CopyExpense: function(expenseId) {
                        pagePath = "../aeexpense.aspx?claimid=" + SEL.Claims.General.claimId + "&expenseid=" + expenseId + "&action=4";
                        document.location = pagePath;
                    },
                    DetermineIfClaimCanBeSubmitted: function() {
                        $.ajax({
                        url: appPath + '/expenses/webServices/claims.asmx/DetermineIfClaimCanBeSubmitted',
                            type: 'POST',
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            data: "{ accountid: " + CurrentUserInfo.AccountID + " , claimid: " + SEL.Claims.General.claimId + ", employeeid: " + CurrentUserInfo.EmployeeID + ", viewfilter: " + SEL.Claims.General.viewFilter + " }",
                        success: function(data) {
                            if (data.d.NoDefaultAuthoriserPresent == true) {
                                SEL.MasterPopup.ShowMasterPopup('This claim cannot be submitted because there is no default authoriser set for your company. Please contact your administrator.',
                               'Message from ' + moduleNameHTML);
                                return false;
                            }
                            if (data.d.Reason > 0) {
                                SEL.Claims.ClaimViewer.DisplaySubmitClaimRejectionReason(data.d);
                            } else {
                                if (SEL.Claims.General.ExpediteClient) {
                                    $.address.value('');
                                    SEL.Claims.ClaimViewer.ShowEnvelopeModal();
                                } else {
                                    SEL.Claims.ClaimViewer.showSubmitModal();
                                }
                            }


                        },
                        error: function(xmlHttpRequest, textStatus, errorThrown) {
                            SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                'Message from ' + moduleNameHTML);
                        }
                    });
                },

                DisplaySubmitClaimRejectionReason: function(results) {

                    switch (results.Reason) {
                    case 1: //NoItems
                        SEL.MasterPopup
                            .ShowMasterPopup('A claim must contain at least one item if you wish to submit it for approval.');
                                        break;
                    case 2: //NoSignoffGroup
                        SEL.MasterPopup
                            .ShowMasterPopup('You cannot submit a claim because you have not been allocated a signoff group.');
                                        break;
                    case 3: //DelegatesProhibited
                                        SEL.MasterPopup.ShowMasterPopup("Delegates cannot submit claims.");
                                        break;
                    case 4: //CannotSignoffOwnClaim
                        SEL.MasterPopup
                            .ShowMasterPopup('You cannot submit this claim because your current signoff group would allow you to sign off your own claim, please contact your administrator to have this corrected.');
                                        break;
                    case 5: //OutstandingFlags
                        SEL.Claims.ClaimViewer.DisplayRevalidationFlags(results.FlagResults);
                                        break;
                    case 6: //MinimumAmountNotReached
                        SEL.MasterPopup
                            .ShowMasterPopup('You cannot submit this claim because the total claim amount is below ' +
                                results.MinimumAmount +
                                '.');
                        break;
                    case 7: //MaximumAmountNotReached
                        SEL.MasterPopup
                            .ShowMasterPopup('You cannot submit this claim because the total claim amount is above ' +
                                results.MaximumAmount +
                                '.');
                        break;
                    case 8: //No Line Manager
                        SEL.MasterPopup
                            .ShowMasterPopup('You cannot submit this claim because you have not been allocated a line manager.');
                        break;
                    case 9: //ApproverOnHoliday
                        SEL.Claims.ClaimViewer.ShowApproverOnHolidayModal();
                        break;
                    case 10: //FrequencyLimitBreached
                        var frequencyMsg = 'This claim cannot be submitted because you cannot submit more than ' +
                            results.FrequencyValue +
                            ' claim(s) per ';
                        switch (results.FrequencyPeriod) {
                        case 1:
                            frequencyMsg += 'month';
                            break;
                        case 2:
                            frequencyMsg += 'week';
                            break;
                                }
                        frequencyMsg += '.';
                        SEL.MasterPopup.ShowMasterPopup(frequencyMsg);
                        break;
                    case 11: //AssignmentSupervisorNotSpecified
                        SEL.MasterPopup
                            .ShowMasterPopup('This claim cannot currently be submitted as one or more assignment numbers associated to claim items do not have a supervisor specified.');
                        break;
                    case 12: //CostcodeOwnerNotSpecified
                        SEL.MasterPopup
                            .ShowMasterPopup('This claim cannot currently be submitted due to either one or more expense item cost code owners missing, no default cost code owner defined or no line manager defined for claimant. Next stage cannot be determined.');
                        break;
                    case 13:
                        SEL.MasterPopup
                            .ShowMasterPopup('This claim cannot be submitted because there are credit card items that have not been reconciled. Please reconcile these items and try again.');
                        break;
                    case 14:
                        SEL.MasterPopup
                            .ShowMasterPopup('This claim cannot be submitted because there are credit card items that have not been matched. Please match these items and try again.');
                        break;
                    case 15:
                        SEL.MasterPopup
                            .ShowMasterPopup('This claim cannot be submitted as the claimant would be able to approve their own claim.');
                        break;
                    case 16:
                        SEL.MasterPopup
                            .ShowMasterPopup('The claim cannot advance to the next stage as the claimant would be able to approve their own claim.');
                        break;
                    case 17:
                        break;
                    case 22:
                        SEL.MasterPopup
                            .ShowMasterPopup('A claim cannot be approved while there are still items waiting approval.');
                        break;
                    case 23:
                        SEL.MasterPopup
                            .ShowMasterPopup('The claim cannot be submitted as the name you have entered already exists.');
                        break;
                    case 24: //OutstandingFlags requiring justification by approver
                        SEL.Claims.CheckExpenseList.Approving = true;
                        SEL.Claims.ClaimViewer.DisplayRevalidationFlags(results.FlagResults);
                        break;
                    case 25: //AssignmentSupervisorNotSpecified
                        SEL.MasterPopup
                            .ShowMasterPopup('This claim cannot currently be approved as one or more assignment numbers associated to claim items do not have a supervisor specified.');
                        break;
                    case 26: //CostcodeOwnerNotSpecified
                        SEL.MasterPopup
                            .ShowMasterPopup('This claim cannot currently be approved due to either one or more expense item cost code owners missing, no default cost code owner defined or no line manager defined for claimant. Next stage cannot be determined.');
                        break;
                    case 28:
                        SEL.MasterPopup.ShowMasterPopup(results.Message);
                        if (SEL.Claims.General.displayDeclaration) {
                            $("#declaration").dialog("close");
                        }
                        break;
                     case 27:
                         SEL.MasterPopup.ShowMasterPopup('You cannot submit this claim because it has already been submitted.');
                        break;
                            }
                    },
                UnsubmitClaim: function() {
                        $.ajax({
                            url: appPath + '/expenses/webServices/claims.asmx/unsubmitClaim',
                            type: 'POST',
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            data: "{ accountId: " + CurrentUserInfo.AccountID + " , employeeId: " + CurrentUserInfo.EmployeeID + ", delegateId: " + CurrentUserInfo.DelegateEmployeeID + ", claimId: " + SEL.Claims.General.claimId + " }",
                            success: function (data) {
                                var statusMessages = SEL.Claims.Messages.ApproverUnsubmitClaim;
                                switch (data.d) {
                                    case -1:
                                        SEL.MasterPopup.ShowMasterPopup(statusMessages.FailedItemsApprovedByOtherApprovers);
                                        break;
                                    case -2:
                                        SEL.MasterPopup.ShowMasterPopup(statusMessages.FailedItemsApproved);
                                        break;
                                    case -3:
                                        SEL.MasterPopup.ShowMasterPopup(statusMessages.FailedItemsNotReturnedByOtherApprovers);
                                        break;
                                    case -4:
                                        SEL.MasterPopup.ShowMasterPopup(statusMessages.FailedClaimHasBeenPaidBeforeValidate);
                                        break;
                                    case -6:
                                        SEL.MasterPopup.ShowMasterPopup(statusMessages.FailedClaimHasBeenInvolvedInSelStage);
                                        break;
                                    case -5:
                                        SEL.MasterPopup.ShowMasterPopup(statusMessages.FailStartedApprovalProcess);
                                        break;
                                    default:
                                    document.location = 'claimViewer.aspx?claimId=' + SEL.Claims.General.claimId;
                                        break;
                                }
                            },
                        error: function(xmlHttpRequest, textStatus, errorThrown) {
                                SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                    'Message from ' + moduleNameHTML);
                            }
                        });
                    },

                FilterExpenseGrid: function(filter) {
                        SEL.Claims.General.viewFilter = filter;
                        $.ajax({
                            url: appPath + '/expenses/webServices/claims.asmx/filterExpenseGrid',
                            type: 'POST',
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            data: "{ accountId: " + CurrentUserInfo.AccountID + ", employeeId: " + CurrentUserInfo.EmployeeID + " , claimId: " + SEL.Claims.General.claimId + ", filter: " + SEL.Claims.General.viewFilter + ", viewType: " + SEL.Claims.General.viewType + ", enableReceiptAttachments: " + SEL.Claims.General.enableReceiptAttachments + ", enableJourneyDetailsLink: " + SEL.Claims.General.enableJourneyDetailsLink + ", enableCorporateCardLink: " + SEL.Claims.General.enableCorporateCardsLink + ", symbol: \"" + SEL.Claims.General.symbol + "\" }",
                        success: function(data) {
                                $g('divExpenseGrid').innerHTML = data.d[1];
                            },
                        error: function(xmlHttpRequest, textStatus, errorThrown) {
                                SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                    'Message from ' + moduleNameHTML);
                            }
                        });
                    },

                CheckTransactionCurrencyAndCountry: function(transactionId) {
                        SEL.Claims.General.transactionId = transactionId;

                        $.ajax({
                            url: appPath + '/expenses/webServices/claims.asmx/checkTransactionCurrencyAndCountry',
                            type: 'POST',
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            data: "{ transactionid: " + SEL.Claims.General.transactionId + " , accountid: " + CurrentUserInfo.AccountID + " }",
                        success: function(data) {
                                if (data.d === "") {
                                    pagePath = "../aeexpense.aspx?claimid=" + SEL.Claims.General.claimId + "&expenseid=" + SEL.Claims.General.statementId + "&transactionid=" + SEL.Claims.General.transactionId;
                                    document.location = pagePath;
                            } else {
                                    SEL.MasterPopup.ShowMasterPopup(data.d);
                                }
                            },
                        error: function(xmlHttpRequest, textStatus, errorThrown) {
                                SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                    'Message from ' + moduleNameHTML);
                            }
                        });
                    },

                ShowMatchTransactionGrid: function(transactionId) {
                        SEL.Claims.General.transactionId = transactionId;

                        $.ajax({
                            url: appPath + '/expenses/webServices/claims.asmx/getCardStatementMatchView',
                            type: 'POST',
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            data: "{ accountId: " + CurrentUserInfo.AccountID + ", employeeId: " + CurrentUserInfo.EmployeeID + ", claimId: " + SEL.Claims.General.claimId + ", viewType:" + SEL.Claims.General.viewType + " , transactionId: " + transactionId + ", symbol: \"" + SEL.Claims.General.symbol + "\" }",
                        success: function(data) {
                                $g('litMatchTransationGrid').innerHTML = data.d[1];
                                var modal = $f(SEL.Claims.IDs.modUnallocatedId);
                                modal.show();
                            },
                        error: function(xmlHttpRequest, textStatus, errorThrown) {
                                SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                    'Message from ' + moduleNameHTML);
                            }
                        });
                    },

                MatchTransaction: function() {
                        var expenseId = SEL.Grid.getSelectedItemFromGrid('gridMatchTransactions');
                        if (expenseId === 0) {
                            SEL.MasterPopup.ShowMasterPopup('Please select an expense item to match the transaction to');
                            return;
                        }

                        $.ajax({
                            url: appPath + '/expenses/webServices/claims.asmx/matchTransaction',
                            type: 'POST',
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            data: "{ accountId: " + CurrentUserInfo.AccountID + ", employeeId: " + CurrentUserInfo.EmployeeID + ", statementId: " + SEL.Claims.General.statementId + " , transactionId: " + SEL.Claims.General.transactionId + ", claimId: " + SEL.Claims.General.claimId + ", expenseId: " + expenseId + " }",
                        success: function(data) {
                                if (data.d === true) {
                                    SEL.Grid.refreshGrid('gridExpenses', 1);
                                    SEL.Claims.ClaimViewer.DdlstStatement_onchange();
                                    SEL.Claims.ClaimViewer.GetClaimTotalAndAmountPayable();
                                    var modal = $f(SEL.Claims.IDs.modUnallocatedId);
                                    modal.hide();
                            } else {
                                    SEL.MasterPopup.ShowMasterPopup('The match could not be made.<br /><br />Check transaction country status, currency or that the transaction total does not exceed the unallocated amount.');
                                }
                            },
                        error: function(xmlHttpRequest, textStatus, errorThrown) {
                                SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                    'Message from ' + moduleNameHTML);
                            }
                        });
                    },

                UnmatchTransaction: function(transactionId, expenseId) {
                        $.ajax({
                            url: appPath + '/expenses/webServices/claims.asmx/unmatchTransaction',
                            type: 'POST',
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            data: "{ accountId: " + CurrentUserInfo.AccountID + " , expenseId: " + expenseId + ", transactionId: " + transactionId + ", claimId: " + SEL.Claims.General.claimId + " }",
                        success: function(data) {
                                SEL.Grid.refreshGrid('gridExpenses', 1);
                                SEL.Grid.refreshGrid('gridTransactions', 1);
                                SEL.Claims.ClaimViewer.GetClaimTotalAndAmountPayable();
                            },
                        error: function(xmlHttpRequest, textStatus, errorThrown) {
                                SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                    'Message from ' + moduleNameHTML);
                            }
                        });

                    },

                DdlstStatement_onchange: function() {
                        var ddlstStatement = $g(SEL.Claims.IDs.ddlstStatementId);
                        $.ajax({
                            url: appPath + '/expenses/webServices/claims.asmx/generateCardTransactionGrid',
                            type: 'POST',
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            data: "{ accountId: " + CurrentUserInfo.AccountID + " , employeeId: " + CurrentUserInfo.EmployeeID + ", statementId: " + ddlstStatement.options[ddlstStatement.selectedIndex].value + " }",
                        success: function(data) {
                            $g('divCardTransactionGrid').innerHTML = data.d[1];
                            SEL.Claims.General.statementId = ddlstStatement.options[ddlstStatement.selectedIndex].value;
                        },
                        error: function(xmlHttpRequest, textStatus, errorThrown) {
                                SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                    'Message from ' + moduleNameHTML);
                            }
                        });
                    },

                GetClaimTotalAndAmountPayable: function() {
                        $.ajax({
                            url: appPath + '/expenses/webServices/claims.asmx/GetClaimTotalAndAmountPayable',
                            type: 'POST',
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            data: "{ accountId: " + CurrentUserInfo.AccountID + " , currentSubAccountId: " + CurrentUserInfo.SubAccountID + ", claimId: " + SEL.Claims.General.claimId + " }",
                        success: function(data) {
                                $g('divClaimTotal').innerHTML = data.d[0];
                                $g('spanAmountPayable').innerHTML = data.d[1];
                                $g('divNumberOfItems').innerHTML = data.d[2];
                            },
                        error: function(xmlHttpRequest, textStatus, errorThrown) {
                                SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                    'Message from ' + moduleNameHTML);
                            }
                        });
                    },

                ShowAdvanceAllocation: function(floatId, viewType) {
                        window.open("../advanceallocation.aspx?viewid=" + viewType + "&floatid=" + floatId, null, "width=700,height=500,scrollbars=yes");
                    },

                DisplayTransactionDetails: function(gridName, transactionId) {
                        SEL.Claims.General.transactionId = transactionId;
                        SEL.Claims.General.gridName = gridName;
                        $.ajax({
                            url: appPath + '/expenses/webServices/claims.asmx/getTransactionDetails',
                            type: 'POST',
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            data: "{ accountId: " + CurrentUserInfo.AccountID + " , transactionId: " + transactionId + " }",
                        success: function(data) {
                                var popup = $f(SEL.Claims.IDs.popupTransactionId);
                                $g('divTransactionDetails').innerHTML = data.d;
                                popup._popupBehavior._parentElement = document.getElementById('viewTransaction' + SEL.Claims.General.gridName + SEL.Claims.General.transactionId);
                                popup.showPopup();
                            },
                        error: function(xmlHttpRequest, textStatus, errorThrown) {
                                SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                    'Message from ' + moduleNameHTML);
                            }
                        });
                    },

                DeleteExpense: function(expenseId, showPrompt) {
                    if (!showPrompt || (showPrompt && confirm('Are you sure you wish to delete the selected expense?'))) {
                            $.ajax({
                                url: appPath + '/expenses/webServices/claims.asmx/deleteExpense',
                                type: 'POST',
                                contentType: 'application/json; charset=utf-8',
                                dataType: 'json',
                                data: "{ accountid: " + CurrentUserInfo.AccountID + " , expenseid: " + expenseId + ", claimid: " + SEL.Claims.General.claimId + ", employeeid: " + CurrentUserInfo.EmployeeID + " }",
                                success: function (data) {
                                    if (data.d === true) {
                                        document.location = "/claimsmenu.aspx";
                                    } else {
                                        SEL.Grid.refreshGrid('gridExpenses', 1);
                                        SEL.Claims.ClaimViewer.GetClaimTotalAndAmountPayable();
                                        if ($e('gridTransactions')) {
                                            SEL.Grid.refreshGrid('gridTransactions', 1);
                                        }
                                        if ($e('gridHistory')) {
                                            SEL.Grid.refreshGrid('gridHistory', 1);
                                        }
                                        document.location = 'claimViewer.aspx?claimId=' + SEL.Claims.General.claimId;
                                    }                                                                      
                                },
                            error: function(xmlHttpRequest, textStatus, errorThrown) {
                                    SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                        'Message from ' + moduleNameHTML);
                                }
                            });
                        }
                    },

                DeleteMobileExpenseItem: function(mobileItemId) {
                        if (confirm(SEL.Claims.Messages.Mobile.ConfirmDeleteItem)) {
                            $.ajax({
                                url: appPath + '/expenses/webServices/claims.asmx/deleteMobileExpenseItem',
                                type: 'POST',
                                contentType: 'application/json; charset=utf-8',
                                dataType: 'json',
                                data: "{ accountid: " + CurrentUserInfo.AccountID + " , mobileitemid: " + mobileItemId + ", employeeid: " + CurrentUserInfo.EmployeeID + " }",
                            success: function(data) {
                                    SEL.Grid.refreshGrid('gridMobileItems', 1);
                                },
                            error: function(xmlHttpRequest, textStatus, errorThrown) {
                                    SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                        'Message from ' + moduleNameHTML);
                                }
                            });
                        }
                    },

                DeleteMobileJourney: function(journeyId) {
                        if (confirm(SEL.Claims.Messages.Mobile.ConfirmDeleteJourney)) {
                            $.ajax({
                                url: appPath + '/expenses/webServices/claims.asmx/DeleteMobileJourney',
                                type: 'POST',
                                contentType: 'application/json; charset=utf-8',
                                dataType: 'json',
                                data: "{ accountid: " + CurrentUserInfo.AccountID + " , mobilejourneyid: " + journeyId + ", employeeid: " + CurrentUserInfo.EmployeeID + " }",
                            success: function(data) {
                                    SEL.Grid.refreshGrid('gridMobileJourneys', 1);
                                },
                            error: function(xmlHttpRequest, textStatus, errorThrown) {
                                    SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                        'Message from ' + moduleNameHTML);
                                }
                            });
                        }
                    },

                DeleteExpenseAsApprover: function(expenseid) {
                        if (confirm('Are you sure you wish to delete the selected expense?')) {
                            document.location = "../deletereason.aspx?action=3&expenseid=" + expenseid + "&claimid=" + SEL.Claims.General.claimId;
                        }
                },

                configureSubmitModal: function() {
                    $("#submitClaim").dialog({
                        autoOpen: false,
                        resizable: false,
                        title: "Submit Claim",
                        width: 920,
                        modal: true,
                        buttons: [
                            {
                                text: 'submit',
                                id: 'btnSubmit',
                                click: function() {
                                    SEL.Claims.ClaimViewer.SubmitClaimClick();
                                }
                            }, {
                                text: 'cancel',
                                id: 'btnSubmitCancel',
                                click: function() {
                                    SEL.Claims.ClaimViewer.ClearSubmitValidators();
                                    $(this).dialog('close');
                                }
                            }
                        ],
                        close: function() {

                        },
                        open: function () {
                            $('#btnSubmit').html('<img src=/shared/images/buttons/btn_save.png\>');
                            $('#btnSubmitCancel').html('<img src=/shared/images/buttons/cancel_up.gif\>');
                        }
                    });

                    $('#submitClaim').keyup(function(e) {
                        if (e.keyCode == 13) {
                            SEL.Claims.ClaimViewer.SubmitClaimClick();
                        }
                    });

                    $('#btnSubmit').keyup(function(e) {
                        if (e.keyCode == 13) {
                            SEL.Claims.ClaimViewer.SubmitClaimClick();
                        }
                    });

                    $('#btnSubmitCancel').keyup(function(e) {
                        if (e.keyCode == 13) {
                            SEL.Claims.ClaimViewer.ClearSubmitValidators();
                            $("#submitClaim").dialog('close');
                        }
                    });
                    //$("#dialog").dialog({ resizable: true, autoOpen: false, minWidth: 300, maxWidth: 300, height: 300, zIndex: SEL.CustomEntityAdministration.zIndices.Forms.AvailableFieldsModal(), stack: false, enableDock: true });
                },

                ClearSubmitValidators: function() {
                    if ($e("compApprover")) {
                        Array.remove(Page_Validators, $g("compApprover"));
                    }

                    if ($e("compCashCredit")) {
                        Array.remove(Page_Validators, $g("compCashCredit"));
                    }

                    if (SEL.Claims.ClaimViewer.OdometerReadings != null) {
                        for (var i = 0; i < SEL.Claims.ClaimViewer.OdometerReadings.length; i++) {
                            if ($e("reqodo" + SEL.Claims.ClaimViewer.OdometerReadings[i].CarID)) {
                                Array.remove(Page_Validators, $g("reqodo" + SEL.Claims.ClaimViewer.OdometerReadings[i].CarID));
                            }
                            if ($e("compodo" + SEL.Claims.ClaimViewer.OdometerReadings[i].CarID)) {
                                Array.remove(Page_Validators, $g("compodo" + SEL.Claims.ClaimViewer.OdometerReadings[i].CarID));
                            }
                        }
                    }
                },
                SubmitClaimClick: function() {
                    if (!SEL.Claims.ClaimViewer.ValidateClaim()) {
                        return;
                    }

                    if (SEL.Claims.General.displayDeclaration) {
                        $("#declaration").dialog("open");
                    } else {
                        SEL.Claims.ClaimViewer.submitClaim();
                    }
                },
                configureDeclarationModal: function() {
                    $("#declaration").dialog({
                        autoOpen: false,
                        resizable: false,
                        title: "Declaration",
                        width: 600,
                        modal: true,
                        buttons: [
                            {
                                text: 'I Accept',
                                id: 'btnAcceptDeclaration',
                                click: function() {
                                    SEL.Claims.ClaimViewer.submitClaim();
                                }
                            }, {
                                text: 'I Decline',
                                id: 'btnDeclineDeclaration',
                                click: function() {
                                    SEL.Claims.ClaimViewer.SubmittingClaim = false;
                                    $(this).dialog('close');
                                }
                            }
                        ],
                        close: function() {

                        },
                        open: function() {
                            $('#btnAcceptDeclaration').html('<img src=/shared/images/buttons/I_accept.png\>');
                            $('#btnDeclineDeclaration').html('<img src=/shared/images/buttons/I_decline.png\>');
                        }
                    });

                    $("#declaration").keyup(function(e) {
                        if (e.keyCode == 13) {
                            SEL.Claims.ClaimViewer.submitClaim();
                        }
                    });

                    $("#btnAcceptDeclaration").keyup(function(e) {
                        if (e.keyCode == 13) {
                            SEL.Claims.ClaimViewer.submitClaim();
                        }
                    });

                    $("#btnDeclineDeclaration").keyup(function(e) {
                        if (e.keyCode == 13) {
                            $("#declaration").dialog('close');
                        }
                    });
                    //$("#dialog").dialog({ resizable: true, autoOpen: false, minWidth: 300, maxWidth: 300, height: 300, zIndex: SEL.CustomEntityAdministration.zIndices.Forms.AvailableFieldsModal(), stack: false, enableDock: true });
                },
                configureApproverOnHolidayModal: function() {
                    $("#approveronHoliday").dialog({
                        autoOpen: false,
                        resizable: false,
                        title: "Authoriser on holiday",
                        width: 600,
                        modal: true,
                        buttons: [
                            {
                                text: 'continue',
                                id: 'btnHolidayContinue',
                                click: function() {
                                    SEL.Claims.ClaimViewer.SubmittingClaim = false;
                                    SEL.Claims.ClaimViewer.ContinueAlthoughAuthoriserIsOnHoliday = true;
                                    SEL.Claims.ClaimViewer.submitClaim();
                                }
                            }, {
                                text: 'cancel',
                                id: 'btnHolidayCancel',
                                click: function() {
                                    SEL.Claims.ClaimViewer.SubmittingClaim = false;
                                    SEL.Claims.ClaimViewer.ContinueAlthoughAuthoriserIsOnHoliday = false;
                                    SEL.Claims.ClaimViewer.SubmittingClaim = false;
                                    $("#approveronHoliday").dialog('close');
                                }
                            }
                        ],
                        close: function() {

                        }, open: function () {
                            $('#btnHolidayContinue').html('<img src=/shared/images/buttons/btn_continue.png\>');
                            $('#btnHolidayCancel').html('<img src=/shared/images/buttons/cancel_up.gif\>');
                        }
                    });

                    $('#approveronHoliday').keyup(function(e) {
                        if (e.keyCode == 13) {
                            SEL.Claims.ClaimViewer.SubmittingClaim = false;
                            SEL.Claims.ClaimViewer.ContinueAlthoughAuthoriserIsOnHoliday = true;
                            SEL.Claims.ClaimViewer.submitClaim();
                        }
                    });

                    $('#btnHolidayContinue').keyup(function(e) {
                        if (e.keyCode == 13) {
                            SEL.Claims.ClaimViewer.SubmittingClaim = false;
                            SEL.Claims.ClaimViewer.ContinueAlthoughAuthoriserIsOnHoliday = true;
                            SEL.Claims.ClaimViewer.submitClaim();
                        }
                    });

                    $('#btnHolidayCancel').keyup(function(e) {
                        if (e.keyCode == 13) {
                            SEL.Claims.ClaimViewer.SubmittingClaim = false;
                            SEL.Claims.ClaimViewer.ContinueAlthoughAuthoriserIsOnHoliday = false;
                            SEL.Claims.ClaimViewer.SubmittingClaim = false;
                            $("#approveronHoliday").dialog('close');
                        }
                    });
                    //$("#dialog").dialog({ resizable: true, autoOpen: false, minWidth: 300, maxWidth: 300, height: 300, zIndex: SEL.CustomEntityAdministration.zIndices.Forms.AvailableFieldsModal(), stack: false, enableDock: true });
                    },
                configureEnvelopeModal: function() {
                    $("#envelopeInfo").dialog({
                        autoOpen: false,
                        resizable: false,
                        title: "Submit Claim",
                        width: 920,
                        modal: true,
                        buttons: [
                            {
                                text: 'cancel',
                                id: 'btnEnvelopeCancel',
                                click: function() { $(this).dialog('close'); }
                            }
                        ],
                        close: function() {

                        }, open: function () {
                            $('#btnEnvelopeCancel').html('<img src=/shared/images/buttons/cancel_up.gif\>');
                        }
                    });

                    $('#btnEnvelopeCancel').keyup(function(e) {
                        if (e.keyCode == 13) {
                            SEL.Claims.ClaimViewer.SubmittingClaim = false;
                            SEL.Claims.ClaimViewer.ContinueAlthoughAuthoriserIsOnHoliday = false;
                            SEL.Claims.ClaimViewer.SubmittingClaim = false;
                            $("#envelopeInfo").dialog('close');
                        }
                    });
                    $("#envelopeInfo").css('max-height', '550px');
                    //$("#dialog").dialog({ resizable: true, autoOpen: false, minWidth: 300, maxWidth: 300, height: 300, zIndex: SEL.CustomEntityAdministration.zIndices.Forms.AvailableFieldsModal(), stack: false, enableDock: true });
                },
                showSubmitModal: function() {

                    Spend_Management.claims.generateClaimSubmitScreen(SEL.Claims.General.claimId, SEL.Claims.ClaimViewer.showSubmitModalComplete);
                },
                showSubmitModalComplete: function(data) {
                    SEL.Common.SetTextAreaMaxLength();
                    $g(SEL.Claims.IDs.txtClaimNameId).value = data.ClaimName;
                    $g(SEL.Claims.IDs.txtClaimDescriptionId).value = data.ClaimDescription;
                    if (data.PartSubmittal) {
                        $g('partSubmit').innerHTML = data.PartSubmittalForm;
                        addCustomValidator("compCashCredit", "tosubmitcash", "Please select the items you would like to submit.", "vgSubmit", "SEL.Claims.ClaimViewer.validateCashCredit");
                        if (SEL.Claims.General.viewFilter > 0) {
                            switch (SEL.Claims.General.viewFilter) {
                            case 1:
                                $g('tosubmitcash').checked = true;
                                break;
                            case 2:
                                $g('tosubmitcredit').checked = true;
                                break;
                            case 3:
                                $g('tosubmitpurchase').checked = true;
                                break;
                            }
                        }

                        ValidatorUpdateDisplay($g("compCashCredit"));
                    }
                    $g('claimApprover').innerHTML = data.Approvers;
                    if (data.Approvers != "") {
                        addMandatorySelectValidator("compApprover", "approver", "Please select a valid Approver.", "vgSubmit");
                        ValidatorUpdateDisplay($g("compApprover"));
                    }
                    $g('declarationText').innerHTML = data.Declaration;
                    $("#declaration").css('max-height', '250px');
                    $("#approveronHoliday").css('max-height', '250px');
                    if (data.FlagMessage != '') {
                        $g('flagWarning').innerHTML = data.FlagMessage;
                    } else {
                        $g('flagWarning').style.display = 'none';
                    }

                    if (data.ClaimIncludesFuelCardMileage) {
                        $("#divFuelCardMileage").show();
                    } else {
                        $("#divFuelCardMileage").hide();
                    }

                    $g(SEL.Claims.IDs.compBusinessMileageId).enabled = false;

                    if (data.OdometerRequired) {
                        if (data.ShowBusinessMileage) {
                            $g(SEL.Claims.IDs.compBusinessMileageId).enabled = true;
                            $("#divBusinessMileage").show();
                            SEL.Claims.ClaimViewer.ShowBusinessMileage = true;
                            var ddlstBusinessMileage = $g(SEL.Claims.IDs.ddlstBusinessMileageId);
                            ddlstBusinessMileage.selectedIndex = 0;
                        }

                        SEL.Claims.ClaimViewer.OdometerReadings = data.OdometerReadings;
                        $g('divOdometerReadings').style.display = '';
                        SEL.Claims.ClaimViewer.GenerateOdometerGrid(data.OdometerReadings);
                    } else {
                        $g('divOdometerReadings').style.display = 'none';
                    }
                    $("#submitClaim").css('max-height', '600px');
                    $("#submitClaim").dialog("open");
                    SEL.Common.GetBroadcastMessages(2000, false, 'submitclaim.aspx');

                    ValidatorUpdateDisplay($g(SEL.Claims.IDs.reqClaimID));
                    ValidatorUpdateDisplay($g(SEL.Claims.IDs.compBusinessMileageId));

                },
                ShowApproverOnHolidayModal: function() {
                    $("#declaration").dialog("close");
                    $("#approveronHoliday").dialog("open");
                },
                GenerateOdometerGrid: function(readings) {
                    var bottomcss = '';
                    var bottomspacecss = '';
                    $("#odometerReadings tr:gt(0)").remove();
                    for (var i = 0; i < readings.length; i++) {
                        if (readings.length - 1 == i) {
                            bottomcss = 'bottom';
                            bottomspacecss = 'bottom ';
                        }
                        $('#odometerReadings').append('<tr><td class="' + bottomspacecss + 'car">' + readings[i].CarMakeModel + '</td><td class="' + bottomspacecss + 'license">' + readings[i].CarRegistration + '</td><td class="' + bottomcss + '">' + readings[i].LastReadingDate + '</td><td class="' + bottomcss + '">' + readings[i].LastReading + '</td><td class="' + bottomcss + '"><input type="text" size="8" maxlength="10" id="newodo' + readings[i].CarID + '" name="newodo' + readings[i].CarID + '" value="" /></td><td class="' + bottomcss + '"><span id="reqodo' + readings[i].CarID + '" style="color: red;">*</span><span id="compodo' + readings[i].CarID + '" style="color:Red;">*</span></td><td class="' + bottomcss + '"><img onmouseover="SEL.Tooltip.Show(\'8E0B8077-6031-4653-AE59-8D2D6E74869D\', \'sm\', this);"  id="imgtooltip43" src="../../shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></td></tr>');

                        addMandatoryValidator('reqodo' + readings[i].CarID, "newodo" + readings[i].CarID, "Please enter the new odometer reading for " + readings[i].CarMakeModel + " (" + readings[i].CarRegistration + ").", "vgSubmit");
                        addGreaterThanOrEqualIntegerValidator('compodo' + readings[i].CarID, "newodo" + readings[i].CarID, "The new odometer reading for " + readings[i].CarMakeModel + " (" + readings[i].CarRegistration + ") must be more than the current odometer reading.", "vgSubmit", readings[i].LastReading);
                        ValidatorUpdateDisplay($g('reqodo' + readings[i].CarID));
                        ValidatorUpdateDisplay($g('compodo' + readings[i].CarID));
                    }

                },

                ShowEnvelopeModal: function() {
                    $("#envelopeInfo").dialog("option", "buttons", [
                        {
                            text: 'cancel',
                            id: 'btnEnvelopeCancel',
                            click: function() { $(this).dialog('close'); }
                        }
                    ]);

                    $('#btnEnvelopeCancel').keyup(function(e) {
                        if (e.keyCode == 13) {
                            SEL.Claims.ClaimViewer.SubmittingClaim = false;
                            SEL.Claims.ClaimViewer.ContinueAlthoughAuthoriserIsOnHoliday = false;
                            SEL.Claims.ClaimViewer.SubmittingClaim = false;
                            $("#envelopeInfo").dialog('close');
                        }
                    });

                    $("#envelopeInfo").dialog("open");
                },
                OdometerReadings: null,
                ValidateClaim: function() {
                    if (SEL.Common.ValidateForm('vgSubmit') == false) {
                        return;
                    }

                    if (SEL.Claims.ClaimViewer.OdometerReadings != null) {
                        var odometerReadings = SEL.Claims.ClaimViewer.validateOdometerReadings();
                        if (odometerReadings == null) {
                            SEL.Claims.ClaimViewer.SubmittingClaim = false;
                            return false;
                        }
                    }
                    return true;
                },

                submitClaim: function() {
                    if (SEL.Claims.ClaimViewer.SubmittingClaim) {
                        return;
                    }

                    SEL.Claims.ClaimViewer.SubmittingClaim = true;
                    if (SEL.Common.ValidateForm('vgSubmit') == false) {
                        SEL.Claims.ClaimViewer.SubmittingClaim = false;
                        return;
                    }

                    var claimName = $g(SEL.Claims.IDs.txtClaimNameId).value;
                    var description = $g(SEL.Claims.IDs.txtClaimDescriptionId).value;

                    //get approver if determined by claimant
                    var approver = null;
                    var ddlstApprover = null;
                    if ($e('approver')) {
                        ddlstApprover = $g('approver');
                    }
                    if (ddlstApprover != null) {
                        approver = ddlstApprover.options[ddlstApprover.selectedIndex].value;
                    }

                    //cash, credit, purchse card
                    var chkToSubmitCash = null;

                    if ($e('tosubmitcash')) {
                        chkToSubmitCash = $g('tosubmitcash');
                    }
                    var chkToSubmitCredit = null;
                    if ($e('tosubmitcredit')) {
                        chkToSubmitCredit = $g('tosubmitcredit');
                    }
                    var chkToSubmitPurchase = null;
                    if ($e('tosubmitpurchase')) {
                        chkToSubmitPurchase = $g('tosubmitpurchase');
                    }
                    var toSubmitCash = true;
                    var toSubmitCredit = true;
                    var toSubmitPurchase = true;

                    if (chkToSubmitCash != null) {
                        toSubmitCash = chkToSubmitCash.checked;
                    }

                    if (chkToSubmitCredit != null) {
                        toSubmitCredit = chkToSubmitCredit.checked;
                    }

                    if (chkToSubmitPurchase != null) {
                        toSubmitPurchase = chkToSubmitPurchase.checked;
                    }

                    var odometerReadings = null;
                    if (SEL.Claims.ClaimViewer.OdometerReadings != null) {
                        odometerReadings = SEL.Claims.ClaimViewer.validateOdometerReadings();
                        if (odometerReadings == null) {
                            SEL.Claims.ClaimViewer.SubmittingClaim = false;
                            return;
                        }
                    }

                    var businessMileage = false;
                    if (SEL.Claims.ClaimViewer.OdometerReadings != null && SEL.Claims.ClaimViewer.ShowBusinessMileage) {
                        var ddlstBusinessMileage = $g(SEL.Claims.IDs.ddlstBusinessMileageId);

                        if (ddlstBusinessMileage.options[ddlstBusinessMileage.selectedIndex].value == '1') {
                            businessMileage = true;
                        } else {
                            businessMileage = false;
                        }
                    }
                    Spend_Management.claims.SubmitClaim(SEL.Claims.General.claimId, claimName, description, toSubmitCash, toSubmitCredit, toSubmitPurchase, approver, odometerReadings, businessMileage, SEL.Claims.SubmitClaim.IgnoreApproverOnHoliday, 0, SEL.Claims.ClaimViewer.ContinueAlthoughAuthoriserIsOnHoliday, SEL.Claims.ClaimViewer.SubmitClaimComplete);
                },
                SubmitClaimComplete: function(data) {
                    SEL.Claims.ClaimViewer.SubmittingClaim = false;
                    if (data.Reason == 0 || data.Reason == 20) {
                        document.location = data.NewLocationURL;
                    } else {
                        SEL.Claims.ClaimViewer.DisplaySubmitClaimRejectionReason(data);
                    }
                },
                validateCashCredit: function(oSrc, args) {
                    var chkToSubmitCash = null;
                    if ($e('tosubmitcash')) {
                        chkToSubmitCash = $g('tosubmitcash');
                    }
                    var chkToSubmitCredit = null;
                    if ($e('tosubmitcredit')) {
                        chkToSubmitCredit = $g('tosubmitcredit');
                    }
                    var chkToSubmitPurchase = null;
                    if ($e('tosubmitpurchase')) {
                        chkToSubmitPurchase = $g('tosubmitpurchase');
                    }
                    var toSubmitCash = true;
                    var toSubmitCredit = true;
                    var toSubmitPurchase = true;

                    if (chkToSubmitCash != null) {
                        toSubmitCash = chkToSubmitCash.checked;
                    }

                    if (chkToSubmitCredit != null) {
                        toSubmitCredit = chkToSubmitCredit.checked;
                    }

                    if (chkToSubmitPurchase != null) {
                        toSubmitPurchase = chkToSubmitPurchase.checked;
                    }

                    args.IsValid = !(!toSubmitCash && !toSubmitCredit && !toSubmitPurchase);
                },

                validateOdometerReadings: function() {

                    var odometerReadings = [];


                    var i;
                    var newamount;
                    var cars = SEL.Claims.ClaimViewer.OdometerReadings;
                    for (i = 0; i < cars.length; i++) {
                        newamount = $('#newodo' + cars[i].CarID).val();

                        odometerReadings.push([cars[i].CarID, cars[i].LastReading, newamount]);
                    }

                    return odometerReadings;
                },
                configureFlagModal: function() {
                    $("#flagSummary").dialog({
                        autoOpen: false,
                        resizable: false,
                        modal: true,
                        title: "<img src=/static/icons/16/new-icons/information2.png\ align='absmiddle'/>&nbsp;&nbsp;Flag Details",
                        width: 850,
                        buttons: [
                            {
                                text: 'save',
                                id: 'btnSaveFlag',
                                click: function() {
                                    SEL.Claims.ClaimViewer.validateJustifications(this);
                                }
                            }, { text: 'cancel', id: 'btnCancelFlag', click: function() { $(this).dialog('close'); } }
                        ],
                        close: function() {

                        },
                        open: function() {
                            $('#btnSaveFlag').html('<img src=/shared/images/buttons/btn_save.png\>');
                                $('#btnCancelFlag').html('<img src=/shared/images/buttons/cancel_up.gif\>');
                        }
                    });


                    $('#flagSummary').keyup(function(e) {
                        if (e.keyCode == 13) {
                            SEL.Claims.ClaimViewer.validateJustifications(this);
                        }
                    });

                    $('#btnSaveFlag').keyup(function(e) {
                        if (e.keyCode == 13) {
                            SEL.Claims.ClaimViewer.validateJustifications($("#flagSummary"));
                        }
                    });

                    $('#btnCancelFlag').keyup(function(e) {
                        if (e.keyCode == 13) {
                            $("#flagSummary").dialog('close');
                        }
                    });

                    //$("#dialog").dialog({ resizable: true, autoOpen: false, minWidth: 300, maxWidth: 300, height: 300, zIndex: SEL.CustomEntityAdministration.zIndices.Forms.AvailableFieldsModal(), stack: false, enableDock: true });
                },
                ToggleFurtherDetailsDisplay: function(expenseId, flaggedItemId) {

                    var authoriserHelp = null;
                    if ($e('flagAuthoriserHelp' + expenseId + '_' + flaggedItemId)) {
                        authoriserHelp = $g('flagAuthoriserHelp' + expenseId + '_' + flaggedItemId);
                    }

                    var flagJustification = null;
                    if ($e('flagJustification' + expenseId + '_' + flaggedItemId)) {
                        flagJustification = $g('flagJustification' + expenseId + '_' + flaggedItemId);
                    }

                    var flagWrapper = null;
                    if ($e('flagDetailColumnWrapper' + expenseId + '_' + flaggedItemId)) {
                        flagWrapper = $g('flagDetailColumnWrapper' + expenseId + '_' + flaggedItemId);
                    }
                    var link = $g('furtherDetailsLink' + expenseId + '_' + flaggedItemId);

                    if (authoriserHelp != null && (authoriserHelp.style.display == '' || authoriserHelp.style.display == 'none')) {
                        authoriserHelp.style.display = 'block';
                    } else if (authoriserHelp != null) {
                        authoriserHelp.style.display = 'none';
                    }

                    if (flagJustification != null && (flagJustification.style.display == '' || flagJustification.style.display == 'none')) {
                        flagJustification.style.display = 'block';
                    } else if (flagJustification != null) {
                        flagJustification.style.display = 'none';
                    }

                    if (flagWrapper != null && (flagWrapper.style.display == '' || flagWrapper.style.display == 'none')) {
                        link.innerHTML = '&laquo; Hide further details.';
                        flagWrapper.style.display = 'block';
                    } else if (flagWrapper != null) {
                        flagWrapper.style.display = 'none';
                        link.innerHTML = '&raquo;  Show further details and justify.';
                    }
                    },

                validateJustifications: function(dialog) {
                    var justifications = [];
                    var elements = document.getElementsByName("claimantJustifications");
                    if (elements.length > 0) {
                        for (var i = 0; i < elements.length; i++) {
                            var id = elements[i].id.replace('justification_', '');
                            var value = elements[i].value;
                            if (value == 'Enter your justification here...') {
                                value = '';
                            }
                            justifications.push([id, value]);

                        }
                        if (justifications.length > 0) {
                            Spend_Management.claims.SaveClaimantJustifications(justifications);
                        }
                    }

                    //authoriser justifications
                    var authoriserJustifications = [];
                    var elements = document.getElementsByName("authoriserJustifications");
                    if (elements.length > 0) {
                        for (var i = 0; i < elements.length; i++) {
                            var id = elements[i].id.replace('authoriserJustification_', '');
                            var value = elements[i].value;

                            if (value == 'Please enter a justification for allowing this flag here.') {
                                value = '';
                            }

                            authoriserJustifications.push([id, value]);
                        }
                        if (authoriserJustifications.length > 0) {
                            Spend_Management.claims.SaveAuthoriserJustifications(authoriserJustifications);
                        }
                    }

                    $(dialog).dialog('close');
                },
                DisplayFlags: function(expenseIds, pageSource) {
                        $.ajax({
                            url: appPath + '/expenses/webServices/claims.asmx/displayFlags',
                            type: 'POST',
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                        data: "{ accountID: " + CurrentUserInfo.AccountID + " , claimID: " + SEL.Claims.General.claimId + ", expenseIDs: \"" + expenseIds + "\", pageSource: \"" + pageSource + "\" }",
                        success: function(data) {


                            //$g('divFlags').innerHTML = data.d;
                            if (data.d.List.length > 0) {
                                SEL.Claims.ClaimViewer.GenerateFlagOutPut(data.d);
                                $("#flagSummary").css('max-height', '600px');
                                $("#flagSummary").dialog("option", "width", 850);
                                $("#flagSummary").dialog("option", "buttons", [{ text: "save", id: 'btnSaveFlag', click: function() { SEL.Claims.ClaimViewer.validateJustifications(this); } }, { text: 'cancel', id: 'btnCancelFlag', click: function() { $(this).dialog('close'); } }]);
                                $("#flagSummary").dialog("open");
                                setTimeout(function () {
                                    $('#btnSaveFlag').html('<img src=/shared/images/buttons/btn_save.png\>');
                                    $('#btnCancelFlag').html('<img src=/shared/images/buttons/cancel_up.gif\>');

                                }, 150);
                            }

                            $(document).keydown(function(e) {
                                    if (e.keyCode === 27) // esc
                                    {
                                        SEL.Claims.ClaimViewer.HideFlagsModal();
                                    }
                                });

                            $('#flagSummary').keyup(function(e) {
                                if (e.keyCode == 13) {
                                    SEL.Claims.ClaimViewer.validateJustifications(this);
                                }
                            });

                            $('#btnSaveFlag').keyup(function(e) {
                                if (e.keyCode == 13) {
                                    SEL.Claims.ClaimViewer.validateJustifications($("#flagSummary"));
                                }
                            });

                            $('#btnCancelFlag').keyup(function(e) {
                                if (e.keyCode == 13) {
                                    $("#flagSummary").dialog('close');
                                }
                            });
                            },
                        error: function(xmlHttpRequest, textStatus, errorThrown) {
                                SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                    'Message from ' + moduleNameHTML);
                            }
                        });
                    },

                GenerateFlagOutPut: function(flags) {
                    $("#divFlags").empty();
                    $("#divFlags").append("<div id='flags'></div>");
                    var flagContainer = $('#flags');

                    if (flags.SubmittingClaim) {
                        flagContainer.append("<div class='comment' style='width: 95%;'>Unfortunately you cannot submit your claim as one or more items have been blocked or require you to provide further justification. Please review the list below and remove the item or provide a justification.</div>");
                    } else if (flags.Authorising && flags.AllowingOrApproving) {
                        flagContainer.append("<div class='comment' style='width: 95%;'>The below items have been flagged because they are outside of policy. Please review the list below and provide a justification for approving the breach.</div>");
                    }

                    for (var expenseLineNumber = 0; expenseLineNumber < flags.Count; expenseLineNumber++) {
                        var expense = flags.List[expenseLineNumber];
                        flagContainer.append("<div id='flaggedExpense" + expenseLineNumber + "'></div>");
                        var expenseContainer = $("#flaggedExpense" + expenseLineNumber);
                        expenseContainer.append("<div class='itemFlag'>" + expense.ExpenseSubcat + " " + expense.ExpenseTotal + ", " + expense.ExpenseDate + "</div>");
                        var flagLineNumber = 0;
                        for (var i = 0; i < expense.FlagCollection.length; i++) {
                            var flaggedItem = expense.FlagCollection[i].FlaggedItem;
                            if (!flags.OnlyDisplayBlockedItems || (flags.OnlyDisplayBlockedItems && flaggedItem.Action == 2)) {
                                if (flaggedItem.FlaggedJourneySteps != null) {
                                    for (var x = 0; x < flaggedItem.FlaggedJourneySteps.length; x++) {
                                        SEL.Claims.ClaimViewer.GenerateFlagline(expenseContainer, flaggedItem, expenseLineNumber, flagLineNumber, flags.Authorising, flags.ClaimantName, flags.Claimant, flags.SubmittingClaim, flags.Stage, flags.OnlyDisplayBlockedItems, flaggedItem.FlaggedJourneySteps[x].FlagDescription, expense.ExpenseID, x);
                                        flagLineNumber++;
                                    }
                                } else {
                                    SEL.Claims.ClaimViewer.GenerateFlagline(expenseContainer, flaggedItem, expenseLineNumber, flagLineNumber, flags.Authorising, flags.ClaimantName, flags.Claimant, flags.SubmittingClaim, flags.Stage, flags.OnlyDisplayBlockedItems, flaggedItem.FlagDescription, expense.ExpenseID, null);
                                    flagLineNumber++;
                                }
                                expenseContainer.append("<div class='flagSpacer'></div>");
                            }
                            expenseContainer.append("<div class='flagSpacer'></div>");
                        }

                    }

                    jQuery("span.timeago").timeago();

                },

                GenerateFlagline: function(flagContainer, flaggedItem, expenseLineNumber, flagLineNumber, authorising, claimantName, claimant, submittingClaim, stage, onlyDisplayBlockedItems, flagDescription, expenseID, journeyStepNumber) {

                    var flagStyle;
                    switch (flaggedItem.FlagColour) {
                    case 1:
                        flagStyle = 'informationFlag';
                        break;
                    case 2:
                        flagStyle = 'amberFlag';
                        break;
                    case 3:
                        flagStyle = 'redFlag';
                        break;
                    }

                    if (submittingClaim) {
                        if (flaggedItem.FailureReason == 1) {
                            flaggedItem.FlagTypeDescription += " - Expense item blocked - <a href=\"javascript:SEL.Claims.ClaimViewer.DeleteBlockedItem(" + expenseLineNumber + "," + expenseID + ");\">Delete expense item</a>";
                        } else if (flaggedItem.FailureReason == 2) {
                            flaggedItem.FlagTypeDescription += " - Justification required";
                        }
                    }
                    flagContainer.append("<div class='flagTitle " + flagStyle + "'>" + flaggedItem.FlagTypeDescription + "</div>");

                    flagContainer.append("<div id='flagWrapper" + expenseLineNumber + "_" + flagLineNumber + "' class='flagWrapper'></div>");
                    var flagWrapper = $("#flagWrapper" + expenseLineNumber + "_" + flagLineNumber);
                    flagWrapper.append("<div>" + flagDescription + "</div>");
                    if (flaggedItem.CustomFlagText != '') {
                        flagWrapper.append("<div>" + flaggedItem.CustomFlagText + "</div>");
                    }
                    flagWrapper.append("<div><a id='furtherDetailsLink" + expenseLineNumber + "_" + flagLineNumber + "' href='javascript:SEL.Claims.ClaimViewer.ToggleFurtherDetailsDisplay(" + expenseLineNumber + "," + flagLineNumber + ");'>&raquo; Show further details and justify.</a></div>");
                    if (flaggedItem.NotesForAuthoriser != '' && authorising) {
                        flagWrapper.append("<div class='comment flagAuthoriserHelp' id='flagAuthoriserHelp" + expenseLineNumber + "_" + flagLineNumber + "'>" + flaggedItem.NotesForAuthoriser + "</div>");
                    }

                    var claimantJustification;

                    if ((claimant && !onlyDisplayBlockedItems && flaggedItem.Action == 1 && stage == null) || authorising) {
                        html = "<div id='flagJustification" + expenseLineNumber + "_" + flagLineNumber + "' class='flagJustification'><textarea ";
                        if (claimant && stage == null && !authorising) {

                            var itemIdForJustificationBox;
      
                            // if the journey step number is not null means we have a mileage item with a journey grid
                            if (journeyStepNumber != null) {
                                itemIdForJustificationBox = flaggedItem.FlaggedJourneySteps[journeyStepNumber].FlaggedItemID;
                                claimantJustification = flaggedItem.FlaggedJourneySteps[journeyStepNumber].ClaimantJustification;
                            } else {
                                itemIdForJustificationBox = flaggedItem.FlaggedItemId;
                                claimantJustification = flaggedItem.ClaimantJustification;
                            }

                            html += "onfocus='SEL.Claims.ClaimViewer.CheckClaimantJustificationText(this, event);' onblur='SEL.Claims.ClaimViewer.CheckClaimantJustificationText(this, event);' name='claimantJustifications' id='justification_" + itemIdForJustificationBox + "'";
                            if ((flaggedItem.ClaimantJustification != null && flaggedItem.ClaimantJustification != '') || (claimant && flaggedItem.ClaimantJustificationMandatory)) {
                                html += " style='";
                            }
                            if (flaggedItem.ClaimantJustification != null && flaggedItem.ClaimantJustification != '') {
                                html += " color: #000;";
                            }
                            if (claimant && flaggedItem.ClaimantJustificationMandatory) {
                                html += "border-left: 3px solid #A94442;";
                            }
                            if ((flaggedItem.ClaimantJustification != null && flaggedItem.ClaimantJustification != '') || (claimant && flaggedItem.ClaimantJustificationMandatory)) {
                                html += "'";
                            }

                            html += ">";
                            if (flaggedItem.ClaimantJustification == null || flaggedItem.ClaimantJustification == '') {
                                html += "Enter your justification here...";
                            } else {

                                html += claimantJustification;
                            }

                        } else if (authorising) {
                            var currentJustification = null;
                            for (var x = 0; x < flaggedItem.AuthoriserJustifications.length; x++) {
                                var authoriserJustification = flaggedItem.AuthoriserJustifications[x];
                                if (authoriserJustification.Stage == stage) {
                                    currentJustification = authoriserJustification;
                                    break;
                                }
                            }
                            html += "onfocus='SEL.Claims.ClaimViewer.CheckAuthoriserJustificationText(this, event);' onblur='SEL.Claims.ClaimViewer.CheckAuthoriserJustificationText(this, event);' name='authoriserJustifications' id='authoriserJustification_" + flaggedItem.FlaggedItemId + "'";

                            if (currentJustification == null || (currentJustification != null && currentJustification.Stage == stage)) {

                                if ((currentJustification != null && currentJustification.Justification != "") || flaggedItem.AuthoriserJustificationMandatory) {
                                    html += " style='";
                                }
                                if (currentJustification != null && currentJustification.Justification != "") {
                                    html += " color: #000;";
                                }

                                if (flaggedItem.AuthoriserJustificationMandatory) {
                                    html += "border-left: 3px solid #A94442;";
                                }
                                if ((currentJustification != null && currentJustification.Justification != "") || flaggedItem.AuthoriserJustificationMandatory) {
                                    {
                                        html += "'";
                                    }
                                }
                            }

                            html += ">";
                            if (currentJustification == null) {
                                html += "Please enter a justification for allowing this flag here.";
                            } else {
                                if (currentJustification.Justification == "") {
                                    html += "Please enter a justification for allowing this flag here.";
                                } else {
                                    html += currentJustification.Justification;
                                }


                            }
                        }
                        html += "</textarea></div>";
                    }

                    flagWrapper.append(html);

                    flagWrapper.append("<div id='flagDetailColumnWrapper" + expenseLineNumber + "_" + flagLineNumber + "' class='flagDetailColumnWrapper'></div>");
                    var flagDetailColumnWrapper = $("#flagDetailColumnWrapper" + expenseLineNumber + "_" + flagLineNumber);
                    if (authorising || (!authorising && flaggedItem.Action == 2) || (claimant && stage != null)) {
                        flagDetailColumnWrapper.append("<div id='flagDetailLeftDiv" + expenseLineNumber + "_" + flagLineNumber + "' style='width: 48%;' class=\"flagDetailLeft\"></div>");
                    }

                    var leftDiv = $("#flagDetailLeftDiv" + expenseLineNumber + "_" + flagLineNumber);
                    if (authorising || (claimant && stage != null)) {
                        var html;

                        html = "<strong>Claimant Justification</strong><div class='justification claimantJustification'><div><strong>";
                        if (flaggedItem.ClaimantJustificationDelegateID != null) {
                            html += flaggedItem.DelegateName;
                        } else {
                            html += claimantName;
                        }

                        html += "</strong>";

                        if (journeyStepNumber != null) {
                           claimantJustification = flaggedItem.FlaggedJourneySteps[journeyStepNumber].ClaimantJustification;
                        } else {
                            claimantJustification = flaggedItem.ClaimantJustification;
                        }

                        if (claimantJustification == null || claimantJustification == '') {
                            html += "<div>Claimant did not provide a justification.</div>";
                        } else {
                            html += "<div>" + claimantJustification + "</div>";
                        }

                        html += "</div>";
                        leftDiv.append(html);


                        if (flaggedItem.AuthoriserJustifications.length > 0 && authorising) {
                            html = '';
                            for (var i = 0; i < flaggedItem.AuthoriserJustifications.length; i++) {
                                var authoriserJustification = flaggedItem.AuthoriserJustifications[i];
                                if (authoriserJustification.Stage != stage) {
                                    html += "<div class='justification";
                                    if (i > 0) {
                                        html += " additionalJustification";
                                    }
                                    html += "'>";

                                    var datestampString = authoriserJustification.DateStamp.replace("/Date(", "").replace(")/", "");

                                    var date = new Date(parseInt(datestampString));
                                    html += "<div><strong>" + authoriserJustification.FullName + "</strong>&nbsp;<span class='timeago date' title='" + date.toISOString() + "'>" + date + "</span></div>";
                                    html += "<div>" + authoriserJustification.Justification + "</div>";
                                    html += "</div>";

                                }
                            }
                            if (html != '') {
                                leftDiv.append("<strong>Authoriser Justifications</strong>");
                                leftDiv.append(html);
                            }
                        }
                    } else {
                        if (flaggedItem.Action == 2) {
                            if (submittingClaim) {
                                leftDiv.append("<div class='justification claimantJustification'>Since you added this expense item to your claim your administrator has changed our policy. In order to submit the claim this item must be corrected or deleted from your claim.</div>");
                            } else {
                                leftDiv.append("<strong>Expense item cannot be claimed</strong><div class='justification claimantJustification'>Our policy does not allow this item to be added to your claim because of this flag.</div>");
                            }
                        }
                    }

                    if (flaggedItem.AssociatedExpenses.length > 0) {
                        flagDetailColumnWrapper.append("<div id='flagDetailRightDiv" + expenseLineNumber + "_" + flagLineNumber + "' class='flagDetails' style='width: 48%;'></div>");
                        var rightDiv = $("#flagDetailRightDiv" + expenseLineNumber + "_" + flagLineNumber);
                        if (flaggedItem.Flagtype == 1 || flaggedItem.Flagtype == 20 || flaggedItem.Flagtype == 8 || flaggedItem.Flagtype == 9) //duplicate
                        {

                            switch (flaggedItem.Flagtype) {
                            case 1:
                                rightDiv.append("<div>We think it is a duplicate because the following details are the same:</div>");
                                break;
                            case 8:
                            case 9:
                                rightDiv.append("<div>Our policy has a combined daily limit for the following items:</div>");
                                break;
                            case 20:
                                rightDiv.append("<div>Only one of the following items may be claimed each day:</div>");
                                break;
                            }
                        }
                        rightDiv.append("<div class='flagDetailsSpecificDetails' id='flagDetailsSpecificDetails" + expenseLineNumber + "_" + flagLineNumber + "'></div>");
                        var specificDetails = $("#flagDetailsSpecificDetails" + expenseLineNumber + "_" + flagLineNumber);
                        switch (flaggedItem.Flagtype) {
                        case 1:
                            specificDetails.append("<strong>Duplicate Information</strong>");
                            html = '<ul>';
                            for (var i = 0; i < flaggedItem.Fields.length; i++) {
                                html += '<li>' + flaggedItem.Fields[i] + '</li>';
                            }

                            html += '</ul>';
                            specificDetails.append(html);
                            break;
                        }

                        if (flaggedItem.Flagtype == 8 || flaggedItem.Flagtype == 9 || flaggedItem.Flagtype == 20) {
                            html = '<ul>';
                            for (var i = 0; i < flaggedItem.AssociatedExpenseItems.length; i++) {
                                html += '<li>' + flaggedItem.AssociatedExpenseItems[i].Name + '</li>';
                            }
                            html += '</ul>';
                            specificDetails.append(html);
                        }

                        if (flaggedItem.AssociatedExpenses.length > 0) {
                            specificDetails.append("<strong>Related Expenses</strong>");


                            for (var i = 0; i < flaggedItem.AssociatedExpenses.length; i++) {
                                var associatedExpense = flaggedItem.AssociatedExpenses[i];
                                html = '<table>';
                                html += "<tr><td style='width: 40%'><strong>Claim name</strong></td><td>" + associatedExpense.ClaimName + "</td></tr>";
                                html += "<tr><td><strong>Date of expense</strong></td><td>" + associatedExpense.Date + "</td></tr>";
                                html += "<tr><td><strong>Expense item</strong></td><td>" + associatedExpense.ExpenseItem + "</td></tr>";
                                html += "<tr><td><strong>Reference Number</strong></td><td>" + associatedExpense.ReferenceNumber + "</td></tr>";
                                html += "<tr><td><strong>Total</strong></td><td>" + associatedExpense.Total + "</td></tr>";
                                html += '</table>';
                                specificDetails.append(html);
                            }


                        }
                    }
                },
                DisplayRevalidationFlags: function(summary) {
                    SEL.Claims.ClaimViewer.GenerateFlagOutPut(summary);

                    //add the submit button
                    if (!SEL.Claims.CheckExpenseList.Approving) {
                        $("#flagSummary").dialog("option", "buttons", [{ text: "submit", id: "btnSubmitFlag", click: function() { SEL.Claims.ClaimViewer.RevalidateAndSubmitClaim(); } }, { text: 'cancel', id: 'btnCancelFlag', click: function() { $(this).dialog('close'); } }]).load($('#btnSubmitFlag').html('<img src=/shared/images/buttons/btn_submit2.png\>'));
                        setTimeout(function() {
                            $('#btnCancelFlag').html('<img src=/shared/images/buttons/cancel_up.gif\>');
                        }, 150);
                        $('#flagSummary').keyup(function (e) {
                            if (e.keyCode == 13) {
                                SEL.Claims.ClaimViewer.RevalidateAndSubmitClaim();
                            }
                        });

                        $('#btnSubmitFlag').keyup(function(e) {
                            if (e.keyCode == 13) {
                                SEL.Claims.ClaimViewer.RevalidateAndSubmitClaim();
                            }
                        });

                        $('#btnCancelFlag').keyup(function(e) {
                            if (e.keyCode == 13) {
                                $("#flagSummary").dialog('close');
                            }
                        });
                    } else {
                        $("#flagSummary").dialog("option", "buttons", [{ text: "approve claim", id: "btnApproveClaimFlag", click: function() { SEL.Claims.CheckAndPay.CheckAuthoriserJustificationsAndApproveClaim(); } }, {
                                text: 'cancel', id: 'btnCancelFlag',
                                click: function() {
                                    SEL.Claims.CheckExpenseList.Approving = false;
                                    $(this).dialog('close');
                                }
                            }
                        ]);
                        $('#flagSummary').keyup(function(e) {
                            if (e.keyCode == 13) {
                                SEL.Claims.CheckAndPay.CheckAuthoriserJustificationsAndApproveClaim();
                            }
                        });

                        $('#btnApproveClaimFlag').keyup(function(e) {
                            if (e.keyCode == 13) {
                                SEL.Claims.CheckAndPay.CheckAuthoriserJustificationsAndApproveClaim();
                            }
                        });

                        $('#btnCancelFlag').keyup(function(e) {
                            if (e.keyCode == 13) {
                                $("#flagSummary").dialog('close');
                            }
                        });

                        setTimeout(function () {
                            $('#btnApproveClaimFlag').html('<img src=/shared/images/buttons/approve_claim.png\>');
                            $('#btnCancelFlag').html('<img src=/shared/images/buttons/cancel_up.gif\>');

                        }, 150);
                    }


                    $("#flagSummary").dialog("open");
                },

                DeleteBlockedItem: function(index, expenseID) {
                    var element = document.getElementById('flaggedExpense' + index);
                    element.parentNode.removeChild(element);
                    SEL.Claims.ClaimViewer.DeleteExpense(expenseID, false);
                    if ($("#flags").children().length == 1) { //no items left
                        SEL.Claims.ClaimViewer.RevalidateAndSubmitClaim();
                    }
                },
                RevalidateAndSubmitClaim: function() {
                    var justifications = [];
                    var elements = document.getElementsByName("claimantJustifications");
                    if (elements.length > 0) {
                        for (var i = 0; i < elements.length; i++) {
                            var id = elements[i].id.replace('justification_', '');
                            var value = elements[i].value;
                            if (value == '' || value == 'Enter your justification here...') {
                                SEL.MasterPopup.ShowMasterPopup('A justification must be provided for each flag in order to submit this claim.');
                                return;
                            } else {
                                justifications.push([id, value]);
                            }
                        }
                        if (justifications.length > 0) {
                            Spend_Management.claims.SaveClaimantJustifications(justifications);
                        }
                    }

                    $("#flagSummary").dialog("close");
                    SEL.Claims.ClaimViewer.DetermineIfClaimCanBeSubmitted();

                },

                HideFlagsModal: function() {
                        var modal = $f(SEL.Claims.IDs.modFlagsId);
                        modal.hide();
                    },

                DisplayValidation: function(expenseId) {
                        $.ajax({
                            url: appPath + '/expenses/webServices/claims.asmx/DisplayValidation',
                            type: 'POST',
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            data: "{ accountId: " + CurrentUserInfo.AccountID + " , claimId: " + SEL.Claims.General.claimId + ", expenseId: " + expenseId + " }",
                        success: function(data) {
                                var modal = $f(SEL.Claims.IDs.modValidationId);
                                $g('divValidation').innerHTML = data.d;
                                modal.show();

                                // create the accordion
                                var accordion = $('div.validation-results-accordion').accordion();
                                accordion.accordion("option", "active", parseInt(accordion.data('active')));

                            $(document).keydown(function(e) {
                                    if (e.keyCode === 27) // esc
                                    {
                                        SEL.Claims.ClaimViewer.HideValidationModal();
                                    }
                                });
                            },
                        error: function(xmlHttpRequest, textStatus, errorThrown) {
                                SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                    'Message from ' + moduleNameHTML);
                            }
                        });
                    },

                CheckAuthoriserJustificationText: function(control, ev) {
                    if (control.value == 'Please enter a justification for allowing this flag here.') {
                        control.value = '';
                        control.style.color = '#000';
                    } else if (control.value == '') {
                        control.value = 'Please enter a justification for allowing this flag here.';
                        control.style.color = '#A9A9A9';
                    }
                },

                HideValidationModal: function() {
                        var modal = $f(SEL.Claims.IDs.modValidationId);
                        modal.hide();
                    },

                ShowMissingEnvelopeModal: function() {
                        var envelopes = SEL.Claims.General.MissingEnvelopes;
                        var output = "<tr><th>Envelope Number</th><th>Date assumed sent</th><th class='envelope-missing-choice'>Posted Late</th><th class='envelope-missing-choice'>Presumed Lost</th></tr>";
                        var env;
                        for (var i = 0; i < envelopes.length; i++) {
                            env = envelopes[i];
                            output += "<tr><td>" + env.EnvelopeNumber + "</td><td>" + env.DatePosted + "</td><td class='envelope-missing-choice'><input type=\"radio\" name=\"" + env.Id + "\" value=\"false\" checked=\"checked\" /></td><td class='envelope-missing-choice'><input type=\"radio\" name=\"" + env.Id + "\" value=\"true\" /></td></tr>";
                        }

                        $('#ulMissingEnvelopeSwitches').html(output);
                        var modal = $f(SEL.Claims.IDs.modEnvelopeMissingId);
                        modal.show();
                        return false;
                    },

                HideEnvelopeMissingModal: function(submit) {

                        if (!submit) {
                            var modal = $f(SEL.Claims.IDs.modEnvelopeMissingId);
                            modal.hide();
                            return false;
                    }

                        var items = $('#ulMissingEnvelopeSwitches input:checked');

                        for (var i = 0; i < items.length; i++) {
                            SEL.Claims.General.MissingEnvelopes[i].HasSent = items[i].value;
                        }

                        var data = '{ "claimId" : ' + SEL.Claims.General.claimId + ',  "statuses" : ' + JSON.stringify(SEL.Claims.General.MissingEnvelopes) + " }";

                        $.ajax({
                            url: appPath + '/expenses/webServices/claims.asmx/UpdateEnvelopeMissingStatus',
                            type: 'POST',
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            data: data,
                        success: function(data) {
                                var modal = $f(SEL.Claims.IDs.modEnvelopeMissingId);
                                modal.hide();
                                $("#divEnvelopeMissingNotice").hide();
                                window.location = window.location;
                },
                        error: function(xmlHttpRequest, textStatus, errorThrown) {
                                SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                    'Message from ' + moduleNameHTML);
                            }
                        });

                        return false;
                },


                CheckClaimantJustificationText: function(control, ev) {
                    if (control.value == 'Enter your justification here...') {
                        control.value = '';
                        control.style.color = '#000';
                        control.style.fontStyle = 'normal';
                    } else if (control.value == '') {
                        control.value = 'Enter your justification here...';
                        control.style.color = '#A9A9A9';
                        control.style.fontStyle = 'italic';
                    }
                }

                },


            Summary:
                        {
                DeleteClaim: function(claimId) {
                                if (confirm('Are you sure you wish to delete the selected claim?')) {
                                    $.ajax({
                                        url: appPath + '/expenses/webServices/claims.asmx/deleteClaim',
                                        type: 'POST',
                                        contentType: 'application/json; charset=utf-8',
                                        dataType: 'json',
                                        data: "{ accountId: " + CurrentUserInfo.AccountID + ", claimId: " + claimId + " }",
                            success: function(data) {
                                            SEL.Grid.refreshGrid('gridClaims', 1);
                                        },
                            error: function(xmlHttpRequest, textStatus, errorThrown) {
                                            SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                                'Message from ' + moduleNameHTML);
                                        }
                                    });
                                }
                            }

                        },
            SubmitClaim:
            {
                IgnoreApproverOnHoliday: false,
                BusinessMileage: false,
                SetupSubmitClaimPage: function () {
                    var thisNs = SEL.Claims.SubmitClaim;
                    //SEL.Claims.General.claimId = SEL.Claims.SubmitClaim.claimid = $('#submitClaimInfo').attr('claimid');
                    SEL.Claims.General.EnvelopeNumberMarkup = $('input.receiptProcessInput').parent().clone().css('display', 'inline-block');

                    // This function is called whenever the page's address value gets changed
                    $.address.change(function (event) {
                        // The first time the page is loaded, do not attempt to navigate anywhere
                        if (SEL.Claims.General.PageLoaded === false) {
                            // If the page is loaded with address values set, reset it.
                            if (event.value !== '/') {
                                $.address.value('/');
                            }

                            SEL.Claims.General.PageLoaded = true;
                        }
                        else {
                            var navigateToPage = thisNs.NavigateToPage;

                            switch (event.value) {
                                case '/envelopeNumber':
                                    $("#envelopeInfo").dialog("option", "buttons", [{ text: "next", id: "btnEnvelopeNext", click: function() {
                                        if ($.address.value() === '/envelopeNumber') {
                                            // Only progress to the next page if the envelope numbers are valid
                                            if (SEL.Claims.SubmitClaim.ValidateEnvelopeNumber('#envelopeNumberBox .receiptProcessInput')) {
                                                if ($('#confirmButton').attr('confirmed') === 'true') {
                                                    $.address.value('referenceNumber');
                                                }
                                                else {
                                                    $("#dialog-confirm").dialog("open");
                                                }
                                            }
                                            else {
                                                SEL.MasterPopup.ShowMasterPopup('Invalid envelope numbers entered. Please enter envelope numbers in the format A-ABC-123.', 'Message from Expenses');
                                            }
                                        }
                                        else {
                                            $(this).dialog('close'); SEL.Claims.ClaimViewer.showSubmitModal();
                                        }
                                         
                                    }}, { text: 'cancel', id: "btnEnvelopeCancel", click: function() { $(this).dialog('close'); } }
                                    ]);

                                    setTimeout(function () {
                                        $('#btnEnvelopeNext').html('<img src=/shared/images/buttons/pagebutton_next.png\>');
                                        $('#btnEnvelopeCancel').html('<img src=/shared/images/buttons/cancel_up.gif\>');

                                    }, 150);

                                    $('#btnEnvelopeNext').keyup(function (e) {
                                        if ($.address.value() === '/envelopeNumber') {
                                            // Only progress to the next page if the envelope numbers are valid
                                            if (SEL.Claims.SubmitClaim.ValidateEnvelopeNumber('#envelopeNumberBox .receiptProcessInput')) {
                                                if ($('#confirmButton').attr('confirmed') === 'true') {
                                                    $.address.value('referenceNumber');
                                                }
                                                else {
                                                    $("#dialog-confirm").dialog("open");
                                                }
                                            }
                                            else {
                                                SEL.MasterPopup.ShowMasterPopup('Invalid envelope numbers entered. Please enter envelope numbers in the format A-ABC-123.', 'Message from Expenses');
                                            }
                                        }
                                        else {
                                            $(this).dialog('close'); SEL.Claims.ClaimViewer.showSubmitModal();
                                        }
                                    });

                                    $('#btnEnvelopeCancel').keyup(function (e) {
                                        if (e.keyCode == 13) {
                                            $("#envelopeInfo").dialog('close');
                                        }
                                    });
                                    navigateToPage.EnvelopeNumber();
                                    break;
                                case '/referenceNumber':
                                    navigateToPage.ReferenceNumber();
                                    break;
                                case '/submitClaim':
                                    $('#envelopeInfo').dialog('close'); SEL.Claims.ClaimViewer.showSubmitModal();
                                    break;
                                case '/':
                                    navigateToPage.SubmitOptions();
                                    break;
                                default:
                                    break;
                            }
                        }
                    });

                    thisNs.SetupConfirmationModal();

                    thisNs.SetupEnvelopeNumberInputMask();

                    thisNs.SetupPageNavigation();

                    thisNs.SetupDeleteEnvelopeButton();
                },
                // Bind the events for navigating around the page
                SetupPageNavigation: function () {
                    $('#alreadyAttachedBox').click(function () {
                        $.address.value('submitClaim');
                    });

                    $('#sendReceiptBox').click(function () {
                        $.address.value('envelopeNumber');
                    });

                    $('#envelopeProcessCancelButton').click(function () {
                        switch ($.address.value()) {
                            case '/referenceNumber':
                                $.address.value('envelopeNumber');
                                break;
                            case '/envelopeNumber':
                                $.address.value('');
                                break;
                            default:
                                window.location.href = "expenses/claimViewer.aspx?claimid=" + $('#submitClaimInfo').attr('claimid');
                                break;
                        }
                    });
                },
                // Setup the confirmation modal the user will see when completing the process
                SetupConfirmationModal: function () {
                    $('#dialog-confirm').dialog({
                        autoOpen: false,
                        dialogClass: 'no-close',
                        draggable: false,
                        resizable: false,
                        height: 165,
                        width: 320,
                        modal: true
                    });

                    $(window).resize(function () {
                        $('#dialog-confirm').dialog('option', 'position', 'center');
                    });

                    $('#cancelButton').click(function () {
                        $('#dialog-confirm').dialog('close');
                    });

                    $('#confirmButton').click(function () {
                        $(this).attr('confirmed', 'true');

                        $('#dialog-confirm').dialog('close');

                        $.address.value('referenceNumber');
                    });
                },
                // Set the input masks for each envelope number input box
                SetupEnvelopeNumberInputMask: function () {
                    // Hide the 'Add another envelope number' text when the user enters an input box
                    $('#envelopeNumberBox').on('focus', '.receiptProcessInput', function () {
                        var currentInput = $(this);

                        currentInput.alphanum({ allowSpace: false, allowUpper: true, allow: '-' });

                        if (currentInput.hasClass('addRow') && currentInput.val() === 'Add another envelope number') {
                            currentInput.removeClass('addRow').val('');
                        }
                    });

                    // Populate the last input box with 'Add another envelope number' if it is left empty
                    $('#envelopeNumberBox').on('blur', '.receiptProcessInput', function () {
                        var inputBox = $(this);
                        var value = inputBox.val();

                        $('.deleteEnvelopeNumber').not(':last').fadeIn(500).css('display', 'inline-block');
                        if (inputBox.parent().next().length === 0 && value.length === 0) {
                            inputBox.parent().find('.deleteEnvelopeNumber').css('display', 'none');

                            inputBox.addClass('addRow').val('Add another envelope number');
                        }
                        else {
                            if (value.length > 1 && value.length < 4 && (value.split('-').length - 1) !== 1) {
                                value = value.replace(/-/g, '');

                                inputBox.val(value.substring(0, 1) + '-' + value.substring(1, value.length));
                            }
                            else if (value.length > 4 && (value.split('-').length - 1) !== 2) {
                                value = value.replace(/-/g, '');

                                inputBox.val(value.substring(0, 1) + '-' + value.substring(1, 4) + '-' + value.substring(4, 7));
                            }

                            // Checks to see if another input box is required. 
                            // This is normally done on keypress, however we need to accommodate for 'paste'
                            SEL.Claims.SubmitClaim.AddAnotherEnvelopeInputBox(inputBox);

                            SEL.Claims.SubmitClaim.ValidateEnvelopeNumber(inputBox);
                        }
                    });

                    // Add another row if the user types a valid character into the last input box
                    $('#envelopeNumberBox').on('keypress', '.receiptProcessInput', function (e) {
                        var code = (e.keyCode ? e.keyCode : e.which);
                        
                        if (code === 45) {
                            // Don't allow the user to input dashes - we'll do that for them.
                            e.preventDefault();
                        }
                        else {
                            if (/[a-z0-9]/.test(String.fromCharCode(code))) {
                                var inputBox = $(this);
                                $('.deleteEnvelopeNumber').fadeIn(500).css('display', 'inline-block');
                                SEL.Claims.SubmitClaim.AddAnotherEnvelopeInputBox(inputBox);
                            }
                        }
                    });
                    
                    // Automatically add dashes when the user types
                    $('#envelopeNumberBox').on('keyup', '.receiptProcessInput', function (e) {
                        var code = (e.keyCode ? e.keyCode : e.which);

                        // Let the user press backspace and delete without interfering
                        if (code !== 8 && code !== 46) {
                            var inputBox = $(this);
                            var value = $(this).val();

                            if ((value.length === 1 && value !== '-') || value.length === 5) {
                                inputBox.val(value + '-');
                            }
                            else {
                                // TODO: Put the following into its own function for here and keypress
                                if (value.length > 1 && value.length < 4 && (value.split('-').length - 1) !== 1) {
                                    value = value.replace(/-/g, '');

                                    inputBox.val(value.substring(0, 1) + '-' + value.substring(1, value.length));
                                }
                                else if (value.length > 4 && (value.split('-').length - 1) !== 2) {
                                    value = value.replace(/-/g, '');

                                    inputBox.val(value.substring(0, 1) + '-' + value.substring(1, 4) + '-' + value.substring(4, 7));
                                }
                            }
                        }
                    });
                },
                // Deletes an envelope number input box from the page
                SetupDeleteEnvelopeButton: function () {
                    $('#envelopeNumberBox').on('click', '.deleteEnvelopeNumber', function () {
                        var envelopeRow = $(this).parent();
                        var envelopeInput = $(envelopeRow).find('input');
                        var envelopeNumber = envelopeInput.val();
                        var envelopeId = envelopeInput.attr('claimenvelopeid') || null;
                        if (String.isNullOrWhitespace(envelopeNumber) || envelopeId == null) {
                            if ($('#envelopeNumberBox .receiptProcessInputContainer').length === 1) {
                                envelopeRow.find('.receiptProcessInput').val('');
                            } else {
                                $('#envelopeNumberBox').animate({ height: '-=65' }, 500);
                                envelopeRow.slideUp(500, function () { envelopeRow.remove(); });
                            }
                        } else {
                            var claimId = SEL.Claims.General.claimId;
                            var accountId = CurrentUserInfo.AccountID;
                            $.ajax({
                                url: appPath + '/expenses/webServices/claims.asmx/ClearEnvelopeLinkage',
                                type: 'POST',
                                contentType: 'application/json; charset=utf-8',
                                dataType: 'json',
                                data: "{ accountId: " + accountId + " , claimId: " + claimId + " , envelopeId: '" + envelopeId + "' }",
                                success: function (data) {
                                    // If there is only one envelope number on the page, clear the input box
                                    if ($('#envelopeNumberBox .receiptProcessInputContainer').length === 2) {
                                        envelopeRow.find('.receiptProcessInput').val('');
                                    } else {
                                        $('#envelopeNumberBox').animate({ height: '-=65' }, 500);
                                        envelopeRow.slideUp(500, function () { envelopeRow.remove(); });
                                    }
                                },
                                error: function (xmlHttpRequest, textStatus, errorThrown) {
                                    SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                        'Message from ' + moduleNameHTML);
                                }
                            });
                        }
                    });
                },
                // Checks to see if another box is needed when the user begins to type in the last one
                AddAnotherEnvelopeInputBox: function (currentInputBox) {
                    
                    if (currentInputBox.parent().next().length === 0) {
                        var newItem = SEL.Claims.General.EnvelopeNumberMarkup.clone().css('display', 'none');
                        currentInputBox.parent().parent().append(newItem);
                        newItem.find('.receiptProcessInput').addClass('addRow').val('Add another envelope number');
                        $('#envelopeNumberBox').animate({ height: '+=65' }, 500);
                        newItem.stop().fadeIn(500);
                    } 
                },
                // Validates either an individual envelope number or a group of numbers, depending on the inputObject passed in
                ValidateEnvelopeNumber: function (inputObject) {
                    var allInputsValid = true;

                    var currentInputValid;

                    $(inputObject).each(function (i, input) {
                        currentInputValid = true;

                        if ($(input).hasClass('addRow') === false) {
                            var value = input.value.toUpperCase();

                            // value must be in the format A-AAA-111
                            if (/^[A-Z]{1}\-[A-Z]{3}\-[0-9]{3}$/.test(value)) {
                                var validLetters = SEL.Claims.Constants.ValidEnvelopeLetters;

                                var letterValue1 = $.inArray(value.substring(2, 3), validLetters);
                                var letterValue2 = $.inArray(value.substring(3, 4), validLetters);
                                var letterValue3 = $.inArray(value.substring(4, 5), validLetters);

                                var numberValue1 = parseInt(value.substring(6, 7));
                                var numberValue2 = parseInt(value.substring(7, 8));
                                var numberValue3 = parseInt(value.substring(8, 9));

                                var checkDigit = (3 * (letterValue1 + letterValue3 + numberValue1 + numberValue3) + letterValue2 + numberValue2) % 23;

                                if ((checkDigit < 0 || checkDigit > 22) || value.substring(0, 1) !== validLetters[checkDigit]) {
                                    currentInputValid = false;
                                }
                            }
                            else {
                                currentInputValid = false;
                            }

                            // Set the styling of the current row to either valid or invalid
                            if (currentInputValid) {
                                $(input).removeClass('invalidEntry');
                            }
                            else {
                                allInputsValid = false;

                                $(input).addClass('invalidEntry');
                            }

                            $($(input).nextSibling).fadeIn(500).css('display', 'inline-block');
                        }
                    });

                    return allInputsValid;
                },
                NavigateToPage:
                {
                    // Load the initial Submit Options page
                    SubmitOptions: function () {
                        $("#envelopeNumberBox [shown=true]").stop().fadeOut(500);

                        $('.receiptProcessBox').stop().fadeOut(500);

                        $('#loadingContainer').stop().fadeOut(500, function () {
                            $(this).addClass('offPage');
                        });

                        $('#submitClaimInfo').stop().fadeOut(500, function () {
                            $('#buttonContainer').stop().fadeOut(500, function () {

                                $('#envelopeProcessCancelButton').find('input').attr('value', 'cancel');

                                $('#sendReceiptBox, #alreadyAttachedBox').stop().fadeIn(500);

                                $('#buttonContainer').stop().fadeIn(500);
                            });
                        });
                    },

                    // Load the Envelope Number page
                    EnvelopeNumber: function () {
                        $('.receiptProcessBox').stop().fadeOut(500);

                        $('#loadingContainer').stop().fadeOut(500, function () {
                            $(this).addClass('offPage');
                        });

                        $('#buttonContainer').stop().fadeOut(500, function () {
                            var envelopeBox = $('#envelopeNumberBox');

                            // If the envelope number page has already been loaded, just show the page
                            if (envelopeBox.attr('alreadyloaded') === 'true') {

                                $('#envelopeProcessCancelButton').find('input').attr('value', 'previous');

                                var deleteButtons = $("#envelopeNumberBox [shown=true]");

                                deleteButtons.css('display', 'none'); // Ensure the delete buttons fadeIn correctly on old browsers

                                $('#envelopeNumberBox, #buttonContainer').stop().fadeIn(500);

                                $('#envelopeNumberBox .receiptProcessInputContainer .receiptProcessInput').first().select();

                                deleteButtons.stop().fadeIn(500).css('display', 'inline-block');
                            }
                            else {
                                $('#loadingContainer').removeClass('offPage').stop().fadeIn(500, function () {
                                    var claimId = $('#submitClaimInfo').attr('claimid');
                                    var accountId = $('#submitClaimInfo').attr('accountid');

                                    $.ajax({
                                        type: "POST",
                                        url: window.appPath + "/shared/webServices/svcClaim.asmx/GetExistingClaimEnvelopeNumbers",
                                        dataType: "json",
                                        contentType: "application/json; charset=utf-8",
                                        data: '{ claimId: ' + claimId + ', accountId: ' + accountId + ' }',
                                        success: function (data) {
                                            var firstInputBox = $('#envelopeNumberBox .receiptProcessInputContainer').first();
                                            var firstInputBoxClone = firstInputBox.clone();

                                            if (data.d.length === 0) {
                                                $('#envelopeNumberBox .receiptProcessInputContainer').first().before(firstInputBoxClone);

                                                envelopeBox.height(envelopeBox.height() + 65);
                                            }

                                            $(data.d).each(function (x, envelopeNum) {
                                                firstInputBoxClone = firstInputBox.clone();

                                                // Show the delete button
                                                firstInputBoxClone.find('.deleteEnvelopeNumber').attr('shown', 'true').fadeIn(500).css('display', 'inline-block');
                                                
                                                // Insert the cloned input row before the first, to push the first box down.
                                                $('#envelopeNumberBox .receiptProcessInputContainer').first().before(firstInputBoxClone);

                                                var inputBox = $('#envelopeNumberBox .receiptProcessInput').first();

                                                inputBox.val(envelopeNum.EnvelopeNumber).attr('claimenvelopeid', envelopeNum.ClaimEnvelopeId);

                                                envelopeBox.height(envelopeBox.height() + 65);
                                            });

                                            // Hide the first input row's delete button     
                                            firstInputBox = $('#envelopeNumberBox .receiptProcessInputContainer').first();

                                            if (firstInputBox.find('.receiptProcessInput').val() === '') {
                                                firstInputBox.find('.deleteEnvelopeNumber').removeAttr('shown').stop().css('display', 'none');
                                            }

                                            // Set the last input row's text to 'Add another envelope number'
                                            $('#envelopeNumberBox .receiptProcessInput').last().val('Add another envelope number').addClass('addRow');
                                        },
                                        error: function (xmlHttpRequest, textStatus, errorThrown) {
                                            SEL.Common.WebService.ErrorHandler(errorThrown);
                                        },
                                        complete: function () {
                                            $('#loadingContainer').stop().fadeOut(500, function () {
                                                $(this).addClass('offPage');
                                            });

                                            envelopeBox.attr('alreadyloaded', 'true');


                                            $('#envelopeProcessCancelButton').find('input').attr('value', 'previous');

                                            $('#envelopeNumberBox').stop().fadeIn(500).find('.receiptProcessInputContainer .receiptProcessInput').first().select();

                                            $('#buttonContainer').stop().fadeIn(500, function () {
                                                SEL.Claims.SubmitClaim.ApplyIE6ImageFix('envelopeNumberImage');
                                            });
                                        }
                                    });
                                });
                            }
                        });
                    },

                    // Load the Reference Number page
                    ReferenceNumber: function () {
                        // Save envelope numbers and calculate claim reference number if envelopes are on screen
                        if ($('#envelopeNumberBox').is(':visible')) {
                            var envelopes = [];

                            $('#envelopeNumberBox .receiptProcessInput').each(function (x, envelopeNumber) {
                                if (envelopeNumber.value !== '' && envelopeNumber.value !== 'Add another envelope number') {
                                    var envelopeId = $(envelopeNumber).attr('claimenvelopeid');

                                    if (typeof envelopeId === "undefined") {
                                        envelopeId = '0';
                                    }

                                    var envelope = "{ 'ClaimEnvelopeId': '" + envelopeId + "', 'EnvelopeNumber': '" + envelopeNumber.value + "' }";

                                    envelopes.push(envelope);
                                }
                            });

                            var claimId = $('#submitClaimInfo').attr('claimid');
                            var accountId = $('#submitClaimInfo').attr('accountid');

                            $('#buttonContainer').stop().fadeOut(500);

                            $("#envelopeNumberBox [shown=true]").stop().fadeOut(500);

                            // Place fadeOuts inside each other to avoid animation mishaps
                            $('#submitClaimInfo').stop().fadeOut(500, function () {
                                $('#envelopeNumberBox').stop().fadeOut(500, function () {
                                    $('#loadingContainer').removeClass('offPage').stop().fadeIn(500, function () {
                                        $.ajax({
                                            type: "POST",
                                            url: window.appPath + "/shared/webServices/svcClaim.asmx/SaveClaimEnvelopeNumbers",
                                            dataType: "json",
                                            contentType: "application/json; charset=utf-8",
                                            data: '{ envelopeNumbers: [' + envelopes + '], claimId: ' + claimId + ', accountId: ' + accountId + ' }',
                                            success: function (data) {
                                                var attachmentResult = data.d;
                                                if (attachmentResult.OverallResult) {
                                                    $('#referenceNumber').text(attachmentResult.ClaimReferenceNumber);
                                                } else {
                                                    var attachmentErrorText = "Validation failed - the following envelopes have errors:<br/><br/>";
                                                    for (var a = 0; a < attachmentResult.Results.length; ++a) {
                                                        var result = attachmentResult.Results[a];
                                                        if (!result.Success) {
                                                            if (result.EnvelopeNumber) {
                                                                attachmentErrorText += "<strong>" + result.EnvelopeNumber.toUpperCase() + ":</strong><br/>";
                                                            }
                                                            attachmentErrorText += result.Reason + "<br/>";
                                                        }
                                                        attachmentErrorText += "<br/>";
                                                    }

                                                    SEL.MasterPopup.ShowMasterPopup(attachmentErrorText);
                                                }
                                            },
                                            error: function (xmlHttpRequest, textStatus, errorThrown) {
                                                SEL.Common.WebService.ErrorHandler(errorThrown);
                                            },
                                            complete: function () {
                                                $('#loadingContainer').fadeOut(500, function () {
                                                    $(this).addClass('offPage');
                                                });

                                                $('#buttonContainer').stop().fadeIn(500, function () {
                                                    SEL.Claims.SubmitClaim.ApplyIE6ImageFix('referenceNumberImage');
                                                });

                                                if ($('#referenceNumber').text().length > 1) {
                                                    $('#referenceNumberBox').stop().fadeIn(500);
                                                } else {
                                                    $("#envelopeNumberBox").stop().fadeIn(500);
                                                    $('#envelopeProcessCancelButton').click();
                                                }

                                            }
                                        });
                                    });
                                });
                            });
                        }
                        else {
                            $('.receiptProcessBox:visible').stop().fadeOut(500);

                            $('#submitClaimInfo').stop().fadeOut(500, function () {
                                $('#buttonContainer').stop().fadeOut(500, function () {
                                    $('#referenceNumberBox').stop().fadeIn(500);

                                    $('#buttonContainer').stop().fadeIn(500, function () {
                                        SEL.Claims.SubmitClaim.ApplyIE6ImageFix('referenceNumberImage');
                                    });
                                });
                            });
                        }
                    },

                    // Load the final - unchanged - Submit Claim page
                    SubmitClaim: function () {
                        $('.receiptProcessBox:visible').stop().fadeOut(500);

                        $('#buttonContainer').stop().fadeOut(500, function () {
                            $('#submitClaimInfo').stop().fadeIn(500);
                        });
                    }
                },
                // Applies the IE6 transparancy fix for dynamically loaded images.
                ApplyIE6ImageFix: function (imageId) {
                    if (jQuery.support.opacity == false) {
                        var image = $('#' + imageId);

                        if (image.attr('iefixapplied') !== 'true') {
                            var imageSource = image.attr("src");

                            image.height(128).width(128);

                            image.css("filter", "progid:DXImageTransform.Microsoft.AlphaImageLoader(src='" + imageSource + "', sizingMethod='scale')");

                            image.attr('src', "/shared/images/blank.gif");

                            image.attr('iefixapplied', 'true');
                        }
                    }
                }

            },

            CheckExpenseList:
            {
                Approving: false,
                ShowUnsubmitClaimAsApproverModal: function () {
                    $.ajax({
                        url: appPath + '/expenses/webServices/claims.asmx/IsClaimUnsubmittable',
                        type: 'POST',
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        data: "{ claimId: " + SEL.Claims.General.claimId + " }",
                        success: function (data) {
                            var statusMessages = SEL.Claims.Messages.ApproverUnsubmitClaim;
                            switch (data.d) {
                                case -1:
                                    SEL.MasterPopup.ShowMasterPopup(statusMessages.FailedItemsApprovedByOtherApprovers);
                                    break;
                                case -2:
                                    SEL.MasterPopup.ShowMasterPopup(statusMessages.FailedItemsApproved);
                                    break;
                                case -3:
                                    SEL.MasterPopup.ShowMasterPopup(statusMessages.FailedItemsNotReturnedByOtherApprovers);
                                    break;
                                case -4:
                                    SEL.MasterPopup.ShowMasterPopup(statusMessages.FailedClaimHasBeenPaidBeforeValidate);
                                    break;
                                case -6:
                                    SEL.MasterPopup.ShowMasterPopup(statusMessages.FailedClaimHasBeenInvolvedInSelStage);
                                    break;
                                case -5:
                                    SEL.MasterPopup.ShowMasterPopup(statusMessages.FailStartedApprovalProcess);
                                    break;
                                case -7:
                                    SEL.MasterPopup.ShowMasterPopup(statusMessages.FailUnsubmitClaimProcess);
                                    break;
                                default:
                                    SEL.Claims.CheckExpenseList.ShowUnsubmitClaimAsApproverModalContinued();
                                    break;
                            }
                        },
                        error: function (xmlHttpRequest, textStatus, errorThrown) {
                            SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                'Message from ' + moduleNameHTML);
                        }
                    });

                },

                ShowUnsubmitClaimAsApproverModalContinued: function () {
                    $f(SEL.Claims.IDs.UnsubmitClaimAsApproverModalId).show();
                    $g(SEL.Claims.IDs.UnsubmitClaimAsApproverReasonTextId).focus();
                    $(document).keydown(function (e) {
                        if (e.keyCode === 27) // esc
                        {
                            $f(SEL.Claims.IDs.UnsubmitClaimAsApproverModalId).hide();
                        }
                    });
                },
                UnsubmitClaimAsApprover: function () {

                    if (validateform("vgUnsubmitReason") === false) {
                        return;
                    }

                    var reason = $g(SEL.Claims.IDs.UnsubmitClaimAsApproverReasonTextId).value;

                        if (reason.length > 4000) {
                            reason = reason.substring(0, 4000);
                        }
                    reason = reason.replace(/["']/g, "");

                    $.ajax({
                        url: appPath + '/expenses/webServices/claims.asmx/UnsubmitClaimAsApprover',
                        type: 'POST',
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        data: "{ claimId: " + SEL.Claims.General.claimId + " , reason: \"" + reason + "\" }",
                        success: function (data) {
                            var statusMessages = SEL.Claims.Messages.ApproverUnsubmitClaim;

                            switch (data.d) {
                                case -1:
                                SEL.MasterPopup.ShowMasterPopup(statusMessages.FailedItemsApprovedByOtherApprovers);
                                    break;
                                case -2:
                                SEL.MasterPopup.ShowMasterPopup(statusMessages.FailedItemsApproved);
                                    break;
                                case -3:
                                SEL.MasterPopup.ShowMasterPopup(statusMessages.FailedItemsNotReturnedByOtherApprovers);
                                    break;
                                case -4:
                                    SEL.MasterPopup.ShowMasterPopup(statusMessages.FailedClaimHasBeenPaidBeforeValidate);
                                    break;
                                case -6:
                                    SEL.MasterPopup.ShowMasterPopup(statusMessages.FailedClaimHasBeenInvolvedInSelStage);
                                    break;
                                case -5:
                                    SEL.MasterPopup.ShowMasterPopup(statusMessages.FailStartedApprovalProcess);
                                    break;
                                default:
                                document.location = "checkpaylist.aspx";
                                    break;
                            }
                        },
                        error: function (xmlHttpRequest, textStatus, errorThrown) {
                            SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                'Message from ' + moduleNameHTML);
                        }
                    });
                    
                },
                AllowSelected: function () {
                    var expenseIds = SEL.Grid.getSelectedItemsFromGrid('gridExpenses');
                    expenseIds = expenseIds.concat(SEL.Grid.getSelectedItemsFromGrid('gridReturned'));
                    if (expenseIds.length === 0) {
                        SEL.MasterPopup.ShowMasterPopup('Please select one or more items to allow.');
                        return;
                    }
                    $.ajax({
                        url: appPath + '/expenses/webServices/claims.asmx/allowSelected',
                        type: 'POST',
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        data: "{ accountId: " + CurrentUserInfo.AccountID + " , currentUserId: " + CurrentUserInfo.EmployeeID + ", delegateId: " + CurrentUserInfo.DelegateEmployeeID + " , claimId: " + SEL.Claims.General.claimId + ", expenseItemIds: [" + expenseIds + "] }",
                        success: function (data) {
                            if (data.d.FlaggedItemsManager != null) {
                                SEL.Claims.CheckAndPay.DisplayAuthoriserJustificationsRequiredFlags(data.d.FlaggedItemsManager);
                            } else {
                            SEL.Grid.refreshGrid('gridExpenses', 1);
                            SEL.Grid.refreshGrid('gridReturned', 1);
                            SEL.Grid.refreshGrid('gridApproved', 1);
                            SEL.Claims.CheckExpenseList.ToggleButtons(data.d.NumberOfApprovedItems, data.d.HasReturnedItems);                      
                            }
                            if (data.d.HasMessage === true) {
                                var message = 'Since the claim items you are allowing are over your authorisation limit, the claim items have been escalated for higher level approval.';
                                SEL.MasterPopup.ShowMasterPopup(message,
                                                               'Message from ' + moduleNameHTML);
                                var hdnLineManagerMessage = document.getElementById(hdnMessage);
                                hdnLineManagerMessage.value = message;
                                return false;
                            }

                            if (data.d.NoDefaultAuthoriserPresent === true) {
                               SEL.MasterPopup.ShowMasterPopup('No Default authoriser assigned. Please contact your system administrator.');
                                return false;
                            }
                        },
                        error: function (xmlHttpRequest, textStatus, errorThrown) {
                            SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                'Message from ' + moduleNameHTML);
                        }
                    });
                },

                ToggleButtons: function (numberOfUnapprovedItems, hasReturnedItems) {
                    if (numberOfUnapprovedItems === 0 && !hasReturnedItems) {
                        $g('divCheckClaim').style.display = 'none';
                        $g('divApproveClaim').style.display = '';

                        $("#approveClaimPrompt").dialog({
                            resizable: false,
                            title: "Approve Claim",
                            modal: true,
                            minWidth: 400,
                            buttons: [{
                                text: "approve claim",
                                "class": "jQueryUIButton",
                                click: function () {
                                    $(this).dialog("close");
                                    SEL.Claims.CheckExpenseList.ApproveClaim();
                                }
                            },{
                                text: "cancel",
                                "class": "jQueryUIButton",
                                click: function () {
                                    $(this).dialog("close");
                                }
                            }]
                        });
                    }
                    else {
                        $g('divCheckClaim').style.display = '';
                        $g('divApproveClaim').style.display = 'none';
                    }
                },

                UnapproveItem: function (expenseId) {
                    $.ajax({
                        url: appPath + '/expenses/webServices/claims.asmx/unapproveItem',
                        type: 'POST',
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        data: "{ accountId: " + CurrentUserInfo.AccountID + " , employeeId: " + CurrentUserInfo.EmployeeID + ", delegateId: " + CurrentUserInfo.DelegateEmployeeID+ " , expenseId: " + expenseId + " }",
                        success: function (data) {
                            SEL.Grid.refreshGrid('gridExpenses', 1);
                            SEL.Grid.refreshGrid('gridApproved', 1);
                            SEL.Claims.CheckExpenseList.ToggleButtons(1, false);
                        },
                        error: function (xmlHttpRequest, textStatus, errorThrown) {
                            SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                'Message from ' + moduleNameHTML);
                        }
                    });

                },

                ShowReturnModal: function () {
                    var expenseIds = SEL.Grid.getSelectedItemsFromGrid('gridExpenses');
                    expenseIds = expenseIds.concat(SEL.Grid.getSelectedItemsFromGrid('gridReturned'));
                    if (expenseIds.length === 0) {
                        SEL.MasterPopup.ShowMasterPopup('Please select one or more items to return.');
                        return;
                    }

                    // Empty the return modal textarea
                    var returnReasonTextBox = $g(SEL.Claims.IDs.txtReturnReasonId);
                    returnReasonTextBox.value = "";
                    $f(SEL.Claims.IDs.modReturnId).show();
                    returnReasonTextBox.focus();
                    $(document).keydown(function (e) {
                        if (e.keyCode === 27) // esc
                        {
                            $f(SEL.Claims.IDs.modReturnId).hide();
                        }
                    });
                },
                ReturnExpenses: function () {
                    if (validateform('valReturn') === false) {
                        return;
                    }

                    var expenseIds = SEL.Grid.getSelectedItemsFromGrid('gridExpenses');
                    expenseIds = expenseIds.concat(SEL.Grid.getSelectedItemsFromGrid('gridReturned'));
                    if (expenseIds.length === 0) {
                        return;
                    }

                    var reason = $g(SEL.Claims.IDs.txtReturnReasonId).value;
                    if (reason.length > 4000) {
                        reason = reason.substring(0, 4000);
                    }
                    reason = reason.replace(/["']/g, "");
                    $.ajax({
                        url: appPath + '/expenses/webServices/claims.asmx/returnExpenses',
                        type: 'POST',
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        data: "{ accountId: " + CurrentUserInfo.AccountID + " , employeeId: " + CurrentUserInfo.EmployeeID + ", delegateId: " + CurrentUserInfo.DelegateEmployeeID + ", claimId: " + SEL.Claims.General.claimId + ", items: [" + expenseIds + "] , reason: \"" + reason + "\" }",
                        success: function (data) {
                            SEL.Grid.refreshGrid('gridExpenses', 1);
                            SEL.Grid.refreshGrid('gridReturned', 1);
                            SEL.Grid.refreshGrid('gridHistory', 1);
                            $f(SEL.Claims.IDs.modReturnId).hide();
                        },
                        error: function (xmlHttpRequest, textStatus, errorThrown) {
                            SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                'Message from ' + moduleNameHTML);
                        }
                    });
                },
                ApproveClaim: function () {

                    if (SEL.Claims.CheckExpenseList.Approving === true) {

                        return;
                    }
                    SEL.Claims.CheckExpenseList.Approving = true;


                    if (SEL.Claims.General.displayDeclaration === true && !SEL.Claims.ClaimViewer.ContinueAlthoughAuthoriserIsOnHoliday) {
                        var mod = $f(SEL.Claims.IDs.modDeclarationId);
                        mod.show();
                    }
                    else {
                        $.ajax({
                            url: appPath + '/expenses/webServices/claims.asmx/approveClaim',
                            type: 'POST',
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            data: "{ accountId: " + CurrentUserInfo.AccountID + " , employeeId: " + CurrentUserInfo.EmployeeID + ", delegateId: " + CurrentUserInfo.DelegateEmployeeID + ", claimId: " + SEL.Claims.General.claimId + " }",
                            success: function (data) {

                                if (data.d.Reason == 0 || data.d.Reason == 20 || data.d.Reason == 18) {
                                SEL.Claims.CheckExpenseList.Approving = false;
                                document.location = 'checkpaylist.aspx';
                                }
                                else
                                {
                                    SEL.Claims.ClaimViewer.DisplaySubmitClaimRejectionReason(data.d);
                                }
                                
                            },
                            error: function (xmlHttpRequest, textStatus, errorThrown) {
                                SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                    'Message from ' + moduleNameHTML);
                            }
                        });
                    }
                },

                DeclarationAgreed: function () {
                    $.ajax({
                        url: appPath + '/expenses/webServices/claims.asmx/approveClaim',
                        type: 'POST',
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        data: "{ accountId: " + CurrentUserInfo.AccountID + " , employeeId: " + CurrentUserInfo.EmployeeID + ", delegateId: " + CurrentUserInfo.DelegateEmployeeID + ", claimId: " + SEL.Claims.General.claimId + " }",
                        success: function (data) {
                            SEL.Claims.CheckExpenseList.Approving = false;
                            if (data.d.Reason == 0 || data.d.Reason == 20 || data.d.Reason == 18) {
                                SEL.Claims.CheckExpenseList.Approving = false;
                                document.location = 'checkpaylist.aspx';
                            } else {
                                SEL.Claims.ClaimViewer.DisplaySubmitClaimRejectionReason(data.d);
                            }
                            
                        },
                        error: function (xmlHttpRequest, textStatus, errorThrown) {
                            SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                'Message from ' + moduleNameHTML);
                        }
                    });
                },
                DeclineApproverDeclaration: function () {
                    SEL.Claims.CheckExpenseList.Approving = false;
                }

            },
            CheckAndPay:
             {
                 AllocateToMe: function () {
                     var claimIds = SEL.Grid.getSelectedItemsFromGrid('gridUnallocated');
                     if (claimIds.length === 0) {
                         SEL.MasterPopup.ShowMasterPopup('Please select one or more claims to allocate.');
                         return;
                     }

                     $.ajax({
                         url: appPath + '/expenses/webServices/claims.asmx/AllocateClaims',
                         type: 'POST',
                         contentType: 'application/json; charset=utf-8',
                         dataType: 'json',
                         data: "{ accountId: " + CurrentUserInfo.AccountID + " , employeeId: " + CurrentUserInfo.EmployeeID + ", claimIds: [" + claimIds + "] }",
                         success: function (data) {
                             SEL.Grid.refreshGrid('gridClaims', 1);
                             SEL.Grid.refreshGrid('gridUnallocated', 1);

                             var allocateClaimsResult = data.d;
                             var messageHtml = '';
   
                             for (i = 0; i < allocateClaimsResult.length; i++) {
                                 if (allocateClaimsResult[i].Success === false) {
                                     messageHtml += '<span>The claim ' + allocateClaimsResult[i].ClaimName + ' has already been allocated to a different approver and removed from your claims list.</span><br/>';
                                 }
                             }

                             if (messageHtml) {
                                 SEL.MasterPopup.ShowMasterPopup(messageHtml, 'Message from ' + moduleNameHTML);
                             }
                         },
                         error: function (xmlHttpRequest, textStatus, errorThrown) {
                             SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                 'Message from ' + moduleNameHTML);
                         }
                     });

                 },

                 UnallocateClaim: function (claimId) {
                     $.ajax({
                         url: appPath + '/expenses/webServices/claims.asmx/unallocateClaim',
                         type: 'POST',
                         contentType: 'application/json; charset=utf-8',
                         dataType: 'json',
                         data: "{ accountId: " + CurrentUserInfo.AccountID + " , employeeId: " + CurrentUserInfo.EmployeeID + ", claimId: " + claimId + " }",
                         success: function (data) {
                             SEL.Grid.refreshGrid('gridClaims', 1);
                             SEL.Grid.refreshGrid('gridUnallocated', 1);
                         },
                         error: function (xmlHttpRequest, textStatus, errorThrown) {
                             SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                 'Message from ' + moduleNameHTML);
                         }
                     });
                 },

                 OneClickApproveClaim: function (claimId) {
                     $.ajax({
                         url: appPath + '/expenses/webServices/claims.asmx/OneClickApproveClaim',
                         type: 'POST',
                         contentType: 'application/json; charset=utf-8',
                         dataType: 'json',
                         data: "{ accountId: " + CurrentUserInfo.AccountID + " , employeeId: " + CurrentUserInfo.EmployeeID + ", delegateId: " + CurrentUserInfo.DelegateEmployeeID + ", claimId: " + claimId + " }",
                         success: function(data) {
                             if (data.d.Reason == 0 || data.d.Reason == 20 || data.d.Reason == 18) {
                                     SEL.Grid.refreshGrid('gridClaims', 1);
                             } else {
                                 SEL.Claims.ClaimViewer.DisplaySubmitClaimRejectionReason(data.d);
                             }

                         },
                         error: function (xmlHttpRequest, textStatus, errorThrown) {
                             SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                 'Message from ' + moduleNameHTML);
                         }
                     });
                 },

                 PayClaim: function (claimId) {
                     $.ajax({
                         url: appPath + '/expenses/webServices/claims.asmx/payClaim',
                         type: 'POST',
                         contentType: 'application/json; charset=utf-8',
                         dataType: 'json',
                         data: "{ accountId: " + CurrentUserInfo.AccountID + " , employeeId: " + CurrentUserInfo.EmployeeID + ", delegateId: " + CurrentUserInfo.DelegateEmployeeID + ", claimId: " + claimId + " }",
                         success: function (data) {
                             SEL.Grid.refreshGrid('gridClaims', 1);
                         },
                         error: function (xmlHttpRequest, textStatus, errorThrown) {
                             SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                 'Message from ' + moduleNameHTML);
                         }
                     });
                 },
                 DisplayAuthoriserJustificationsRequiredFlags: function (summary) {
                     SEL.Claims.ClaimViewer.GenerateFlagOutPut(summary);

                     //add the submit button
                     $("#flagSummary").dialog("option", "buttons", [{ text: "allow selected", id: "btnAllowSelectedFlag", click: function () { SEL.Claims.CheckAndPay.CheckAuthoriserJustificationsAndAllowSelected(); } }, { text: 'cancel', id: 'btnCancelFlag', click: function() { $(this).dialog('close'); } }]);

                     $("#flagSummary").dialog("open");

                     $('#flagSummary').keyup(function (e) {
                         if (e.keyCode == 13) {
                             SEL.Claims.CheckAndPay.CheckAuthoriserJustificationsAndAllowSelected();
                         }
                     });

                     $('#btnAllowSelectedFlag').keyup(function (e) {
                         if (e.keyCode == 13) {
                             SEL.Claims.CheckAndPay.CheckAuthoriserJustificationsAndAllowSelected();
                         }
                     });

                     $('#btnCancelFlag').keyup(function (e) {
                         if (e.keyCode == 13) {
                             $("#flagSummary").dialog('close');
                         }
                     });

                       setTimeout(function () {
                           $('#btnAllowSelectedFlag').html('<img src=/shared/images/buttons/allow_selected.png\>');
                            $('#btnCancelFlag').html('<img src=/shared/images/buttons/cancel_up.gif\>');

                        }, 150);

                 },
                 CheckAuthoriserJustificationsAndAllowSelected : function() {
                     var authoriserJustifications = [];
                     var elements = document.getElementsByName("authoriserJustifications");
                     if (elements.length > 0) {
                         for (var i = 0; i < elements.length; i++) {
                             var id = elements[i].id.replace('authoriserJustification_', '');
                             var value = elements[i].value;
                             if (value.length == 0 || value == 'Please enter a justification for allowing this flag here.') {
                                 SEL.MasterPopup.ShowMasterPopup('Please provide a justification for authorising the expenses listed.',
                                 'Message from ' + moduleNameHTML);
                                 return;
                             }

                              authoriserJustifications.push([id, value]) ;
                         }

                         if (authoriserJustifications.length > 0) {
                             Spend_Management.claims.SaveAuthoriserJustifications(authoriserJustifications, SEL.Claims.CheckAndPay.SaveAuthoriserJustificationsComplete);
                         }

                     }
                 },
                 CheckAuthoriserJustificationsAndApproveClaim: function () {
                     var authoriserJustifications = [];
                     var elements = document.getElementsByName("authoriserJustifications");
                     if (elements.length > 0) {
                         for (var i = 0; i < elements.length; i++) {
                             var id = elements[i].id.replace('authoriserJustification_', '');
                             var value = elements[i].value;
                             if (value.length == 0 || value == 'Please enter a justification for allowing this flag here.') {
                                 SEL.MasterPopup.ShowMasterPopup('Please provide a justification for authorising the expenses listed.',
                                 'Message from ' + moduleNameHTML);
                                 return;
                             }

                             authoriserJustifications.push([id, value]);
                         }

                         if (authoriserJustifications.length > 0) {
                             Spend_Management.claims.SaveAuthoriserJustifications(authoriserJustifications, SEL.Claims.CheckAndPay.SaveAuthoriserJustificationsComplete);
                         }

                     }
                 },
                 SaveAuthoriserJustificationsComplete: function () {
                     if (SEL.Claims.CheckExpenseList.Approving) {
                         SEL.Claims.CheckExpenseList.Approving = false;
                         SEL.Claims.CheckExpenseList.ApproveClaim();
                     } else {
                         SEL.Claims.CheckExpenseList.AllowSelected();
                     }
                     
                     $("#flagSummary").dialog("close");
                 },

                 FilterCheckAndPayGrids: function () {
                     var surname = $g(SEL.Claims.IDs.txtSurnameId).value;
                     var filter = $g(SEL.Claims.IDs.cmbFilterId).options[$g(SEL.Claims.IDs.cmbFilterId).selectedIndex].value;
                     $.ajax({
                         url: appPath + '/expenses/webServices/claims.asmx/filterCheckAndPayClaimGrid',
                         type: 'POST',
                         contentType: 'application/json; charset=utf-8',
                         dataType: 'json',
                         data: "{ accountId: " + CurrentUserInfo.AccountID + " , employeeId: " + CurrentUserInfo.EmployeeID + ", surname: \"" + surname + "\", filter: " + filter + " }",
                         success: function (data) {
                             $g('divGridClaims').innerHTML = data.d[1];
                         },
                         error: function (xmlHttpRequest, textStatus, errorThrown) {
                             SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                 'Message from ' + moduleNameHTML);
                         }
                     });
                     $.ajax({
                         url: appPath + '/expenses/webServices/claims.asmx/filterCheckAndPayUnallocatedGrid',
                         type: 'POST',
                         contentType: 'application/json; charset=utf-8',
                         dataType: 'json',
                         data: "{ accountId: " + CurrentUserInfo.AccountID + " , employeeId: " + CurrentUserInfo.EmployeeID + ", surname: \"" + surname + "\", filter: " + filter + " }",
                         success: function (data) {
                             $g('divGridUnallocated').innerHTML = data.d[1];
                         },
                         error: function (xmlHttpRequest, textStatus, errorThrown) {
                             SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                 'Message from ' + moduleNameHTML);
                         }
                     });
                 }
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


(function () {
    var scriptName = "Claims";
    function execute() {
        SEL.registerNamespace("SEL.Claims");
        SEL.Claims =
        {
            claimId: null,
            statementId: null,
            transactionId: null,
            viewFilter: 0,
            enableReceiptAttachments: null,
            enableJourneyDetailsLink: null,
            enableCorporateCardsLink: null,
            viewType: null,
            popupTransactionId: null,
            modUnallocatedId: null,
            ddlstStatementId: null,
            modReturnId: null,
            txtReturnReasonId: null,
            approvingClaim: null,
            displayDeclaration: null,
            modDeclarationId: null,
            symbol: null,
            gridName: null,
            modFlagsId: null,
            txtSurnameId: null,
            cmbFilterId: null,
            lblApproverId: null,
            lblDatePaidId: null,
            UnsubmitClaimAsApproverReasonTextId: null,
            UnsubmitClaimAsApproverModalId: null,
            IsSplitClaim: false,
            cmblinemanager: null,
            txtLineManager: null,
            txtLineManagerId: null,
            lineManagerSearchPanel: null,
            lineManagerSearchModal: null,
            lineManagerCombo: null, 
            Messages:
            {
                ApproverUnsubmitClaim:
                {
                    //ConfirmUnsubmit: "This will take the claim out of the approval process entirely and return it to the claimant as a current claim. Are you sure you wish to do this?",
                    FailedItemsNotReturnedByOtherApprovers: "This claim cannot be unsubmitted as there are other approvers at this stage that have not returned all of their items on the claim.",
                    FailedItemsApproved: "This claim cannot be unsubmitted as you have approved some of the items in the claim.",
                    FailedItemsApprovedByOtherApprovers: "This claim cannot be unsubmitted as there are other approvers at this stage that have approved items on the claim."
                }
            },

            submitClaim: function () {

                Spend_Management.claims.submitClaim(CurrentUserInfo.AccountID, SEL.Claims.claimId, CurrentUserInfo.EmployeeID, SEL.Claims.viewFilter, SEL.Claims.submitClaimComplete);
            },
            submitClaimComplete: function (data) {
                switch (data[0]) {
                    case "1":
                        SEL.MasterPopup.ShowMasterPopup('A claim must contain at least one item if you wish to submit it for approval.');
                        break;
                    case "3":
                        SEL.MasterPopup.ShowMasterPopup('You cannot submit a claim because you have not been allocated a signoff group.');
                        break;
                    case "4":
                        SEL.MasterPopup.ShowMasterPopup("Delegates cannot submit claims.");
                        break;
                    case "5":
                        SEL.MasterPopup.ShowMasterPopup('You cannot submit this claim because your current signoff group would allow you to sign off your own claim, please contact your administrator to have this corrected.');
                        break;
                    case "0":
                        document.location = data[1];
                        break;
                }
            },

            filterExpenseGrid: function (filter) {
                SEL.Claims.viewFilter = filter;
                Spend_Management.claims.filterExpenseGrid(CurrentUserInfo.AccountID, CurrentUserInfo.EmployeeID, SEL.Claims.claimId, SEL.Claims.viewFilter, SEL.Claims.viewType, SEL.Claims.enableReceiptAttachments, SEL.Claims.enableJourneyDetailsLink, SEL.Claims.enableCorporateCardsLink, SEL.Claims.symbol,SEL.Claims.filterExpenseGridComplete);
            },

            filterExpenseGridComplete: function (data) {
                $g('divExpenseGrid').innerHTML = data[1];
            },

            showAdvanceAllocation: function (floatId, viewType) {
                window.open("../advanceallocation.aspx?viewid=" + viewType + "&floatid=" + floatId, null, "width=700,height=500,scrollbars=yes");
            },

            checkTransactionCurrencyAndCountry: function (transactionId) {
                SEL.Claims.transactionId = transactionId;
                Spend_Management.claims.checkTransactionCurrencyAndCountry(SEL.Claims.transactionId, CurrentUserInfo.AccountID, SEL.Claims.checkTransactionCurrencyAndCountryComplete);
            },

            checkTransactionCurrencyAndCountryComplete: function (data) {
                if (data == "") {
                    pagePath = "../aeexpense.aspx?claimid=" + SEL.Claims.claimId + "&statementid=" + SEL.Claims.statementId + "&transactionid=" + SEL.Claims.transactionId;
                    document.location = pagePath;
                }
                else {
                    SEL.MasterPopup.ShowMasterPopup(data);
                }
            },

            displayTransactionDetails: function (gridName, transactionId) {
                SEL.Claims.transactionId = transactionId;
                SEL.Claims.gridName = gridName;
                Spend_Management.claims.getTransactionDetails(CurrentUserInfo.AccountID, transactionId, SEL.Claims.displayTransactionDetailsComplete);
            },

            displayTransactionDetailsComplete: function (data) {
                var popup = $find(SEL.Claims.popupTransactionId);
                $g('divTransactionDetails').innerHTML = data;
                popup._popupBehavior._parentElement = document.getElementById('viewTransaction' + SEL.Claims.gridName + SEL.Claims.transactionId);
                popup.showPopup();
                
            },

            showMatchTransactionGrid: function (transactionId) {
                SEL.Claims.transactionId = transactionId;
                Spend_Management.claims.getCardStatementMatchView(CurrentUserInfo.AccountID, CurrentUserInfo.EmployeeID, SEL.Claims.claimId, SEL.Claims.viewType, SEL.Claims.transactionId, SEL.Claims.symbol, SEL.Claims.showMatchTransactionGridComplete);
            },

            showMatchTransactionGridComplete: function (data) {
                $g('litMatchTransationGrid').innerHTML = data[1];
                var modal = $find(SEL.Claims.modUnallocatedId);
                modal.show();
            },

            matchTransaction: function () {
                var expenseId = SEL.Grid.getSelectedItemFromGrid('gridMatchTransactions');
                if (expenseId == 0)
                {
                    SEL.MasterPopup.ShowMasterPopup('Please select an expense item to match the transaction to');
                    return;
                }
                Spend_Management.claims.matchTransaction(CurrentUserInfo.AccountID, CurrentUserInfo.EmployeeID, SEL.Claims.statementId, SEL.Claims.transactionId, SEL.Claims.claimId, expenseId, SEL.Claims.matchTransactionComplete);        
            },

            matchTransactionComplete: function (matched)
            {
                if (matched == true)
                {
                    SEL.Grid.refreshGrid('gridExpenses', 1);
                    SEL.Claims.ddlstStatement_onchange();
                    SEL.Claims.GetClaimTotalAndAmountPayable();
                    var modal = $find(SEL.Claims.modUnallocatedId);
                    modal.hide();
                }
                else
                {
                    SEL.MasterPopup.ShowMasterPopup('The match could not be made.<br /><br />Check transaction country status, currency or that the transaction total does not exceed the unallocated amount.');
                }
            },

            unmatchTransaction: function (transactionId, expenseId) {
                Spend_Management.claims.unmatchTransaction(CurrentUserInfo.AccountID, expenseId, transactionId, SEL.Claims.claimId, SEL.Claims.unmatchTransactionComplete);
            },

            unmatchTransactionComplete: function () {
                SEL.Grid.refreshGrid('gridExpenses', 1);
                SEL.Grid.refreshGrid('gridTransactions', 1);
                SEL.Claims.GetClaimTotalAndAmountPayable();
            },

            ddlstStatement_onchange: function () {
                
                var ddlstStatement = $g(SEL.Claims.ddlstStatementId);
                Spend_Management.claims.generateCardTransactionGrid(CurrentUserInfo.AccountID, CurrentUserInfo.EmployeeID, ddlstStatement.options[ddlstStatement.selectedIndex].value, SEL.Claims.ddlstStatement_onchangeComplete);
            },

            ddlstStatement_onchangeComplete: function (data) {
                $g('divCardTransactionGrid').innerHTML = data[1];
                
            },

            unsubmitClaim: function () {
                Spend_Management.claims.unsubmitClaim(CurrentUserInfo.AccountID, CurrentUserInfo.EmployeeID, CurrentUserInfo.DelegateEmployeeID, SEL.Claims.claimId, SEL.Claims.unsubmitClaimComplete);
            },

            unsubmitClaimComplete: function (data) {
                if (data == 1) {
                    SEL.MasterPopup.ShowMasterPopup('This claim cannot be unsubmitted as it has already started the approval process.');
                }
                else {
                    document.location = 'claimViewer.aspx?claimId=' + SEL.Claims.claimId;
                }
            },

            ShowUnsubmitClaimAsApproverModal: function ()
            {
                if (SEL.Claims.IsSplitClaim === true)
                {
                    Spend_Management.claims.IsSplitApprovalClaimUnsubmittable(SEL.Claims.claimId,
                        function (data)
                        {
                            if (data === false)
                            {
                                SEL.MasterPopup.ShowMasterPopup(SEL.Claims.Messages.ApproverUnsubmitClaim.FailedItemsNotReturnedByOtherApprovers);
                                return;
                            }
                            else
                            {
                                SEL.Claims.ShowUnsubmitClaimAsApproverModalContinued();
                            }
                        },
                        SEL.Common.WebService.ErrorHandler);
                }
                else
                {
                    SEL.Claims.ShowUnsubmitClaimAsApproverModalContinued();
                }
            },

            ShowUnsubmitClaimAsApproverModalContinued: function ()
            {
                // clear the modal
                $g(SEL.Claims.UnsubmitClaimAsApproverReasonTextId).value = "";
                // show the modal
                $f(SEL.Claims.UnsubmitClaimAsApproverModalId).show();
            },

            UnsubmitClaimAsApprover: function ()
            {
                if (validateform("vgUnsubmitReason") === false)
                {
                    return;
                }
                
                var reason = $g(SEL.Claims.UnsubmitClaimAsApproverReasonTextId).value;
                if (reason.length > 4000)
                {
                    reason = reason.substring(0, 4000);
                }
                
                Spend_Management.claims.UnsubmitClaimAsApprover(SEL.Claims.claimId, reason,
                    function (data)
                    {
                        var statusMessages = SEL.Claims.Messages.ApproverUnsubmitClaim;
                        
                        if (data === -1)
                        {
                            SEL.MasterPopup.ShowMasterPopup(statusMessages.FailedItemsApprovedByOtherApprovers);
                        }
                        else if (data === -2)
                        {
                            SEL.MasterPopup.ShowMasterPopup(statusMessages.FailedItemsApproved);
                        }
                        else if (data === -3)
                        {
                            SEL.MasterPopup.ShowMasterPopup(statusMessages.FailedItemsNotReturnedByOtherApprovers);
                        }
                        else if (data === 0)
                        {
                            document.location = "checkpaylist.aspx";
                        }
                    },
                    SEL.Common.WebService.ErrorHandler);
            },

            deleteExpense: function (expenseId) {
                if (confirm('Are you sure you wish to delete the selected expense?')) {
                    Spend_Management.claims.deleteExpense(CurrentUserInfo.AccountID, expenseId, SEL.Claims.claimId, CurrentUserInfo.EmployeeID, SEL.Claims.deleteExpenseComplete);
                }
            },

            deleteExpenseComplete: function () {
                SEL.Grid.refreshGrid('gridExpenses', 1);
                SEL.Claims.GetClaimTotalAndAmountPayable();
                if ($e('gridTransactions')) {
                    SEL.Grid.refreshGrid('gridTransactions', 1);
                }
            },

            deleteMobileExpenseItem: function (mobileItemId) {
                if (confirm('Are you sure you wish to delete the selected mobile expense item?')) {
                    Spend_Management.claims.deleteMobileExpenseItem(CurrentUserInfo.AccountID, mobileItemId, CurrentUserInfo.EmployeeID, SEL.Claims.deleteMobileExpenseItemComplete);
                }
            },

            deleteMobileExpenseItemComplete: function () {
                SEL.Grid.refreshGrid('gridMobileItems', 1);
            },

            deleteExpenseAsApprover: function (expenseid) {
                if (confirm('Are you sure you wish to delete the selected expense?')) {
                    document.location = "../deletereason.aspx?action=3&expenseid=" + expenseid + "&claimid=" + SEL.Claims.claimId;
                }
            },

            allowSelected: function () {
                var expenseIds = SEL.Grid.getSelectedItemsFromGrid('gridExpenses');
                
                expenseIds = expenseIds.concat(SEL.Grid.getSelectedItemsFromGrid('gridReturned'));
                if (expenseIds.length == 0) {
                    SEL.MasterPopup.ShowMasterPopup('Please select one or more items to allow.');
                    return;
                }
                Spend_Management.claims.allowSelected(CurrentUserInfo.AccountID, SEL.Claims.claimId, expenseIds,SEL.Claims.allowSelectedComplete);
            },

            allowSelectedComplete: function (data) {
                SEL.Grid.refreshGrid('gridExpenses', 1);
                SEL.Grid.refreshGrid('gridReturned', 1);
                SEL.Grid.refreshGrid('gridApproved', 1);

                SEL.Claims.toggleButtons(data[0], data[1]);
            },

            toggleButtons : function (numberOfUnapprovedItems, hasReturnedItems)
            {
                if (numberOfUnapprovedItems == 0 && !hasReturnedItems) {
                    $g('divCheckClaim').style.display = 'none';
                    $g('divApproveClaim').style.display = '';
                }
                else {
                    $g('divCheckClaim').style.display = '';
                    $g('divApproveClaim').style.display = 'none';
                }
            },
            unapproveItem: function (expenseId) {
                Spend_Management.claims.unapproveItem(CurrentUserInfo.AccountID, expenseId, SEL.Claims.unapproveItemComplete);
            },

            unapproveItemComplete: function (data) {
                SEL.Grid.refreshGrid('gridExpenses', 1);
                SEL.Grid.refreshGrid('gridApproved', 1);
                SEL.Claims.toggleButtons(1, false);
            },

            showReturnModal: function () {
                var expenseIds = SEL.Grid.getSelectedItemsFromGrid('gridExpenses');
                expenseIds = expenseIds.concat(SEL.Grid.getSelectedItemsFromGrid('gridReturned'));
                if (expenseIds.length == 0) {
                    SEL.MasterPopup.ShowMasterPopup('Please select one or more items to return.');
                    return;
                }
                
                // Empty the return modal textarea
                $g(SEL.Claims.txtReturnReasonId).value = "";
                $find(SEL.Claims.modReturnId).show();
            },

            returnExpenses: function ()
            {
                if (validateform('valReturn') == false)
                {
                    return;
                }

                var expenseIds = SEL.Grid.getSelectedItemsFromGrid('gridExpenses');
                expenseIds = expenseIds.concat(SEL.Grid.getSelectedItemsFromGrid('gridReturned'));
                if (expenseIds.length == 0)
                {
                    return;
                }
                
                var reason = $g(SEL.Claims.txtReturnReasonId).value;
                if (reason.length > 4000)
                {
                    reason = reason.substring(0, 4000);
                }
                
                Spend_Management.claims.returnExpenses(CurrentUserInfo.AccountID, CurrentUserInfo.EmployeeID, CurrentUserInfo.DelegateEmployeeID, SEL.Claims.claimId, expenseIds, reason,
                    function (data)
                    {
                        SEL.Grid.refreshGrid('gridExpenses', 1);
                        SEL.Grid.refreshGrid('gridReturned', 1);
                        SEL.Grid.refreshGrid('gridHistory', 1);
                        $f(SEL.Claims.modReturnId).hide();
                    },
                    SEL.Common.WebService.ErrorHandler
                );
            },

            selectAllUnapprovedAndReturnedItems: function () {
                SEL.Grid.selectAllOnGrid('gridExpenses');
                SEL.Grid.selectAllOnGrid('gridReturned');
            },

            approveClaim: function () {

                if (SEL.Claims.approving == true) {

                    return;
                }
                SEL.Claims.approving = true;
                
                

                if (SEL.Claims.displayDeclaration === true) {
                    var mod = $find(SEL.Claims.modDeclarationId);
                    mod.show();
                }
                else {
                    Spend_Management.claims.approveClaim(CurrentUserInfo.AccountID, CurrentUserInfo.EmployeeID, CurrentUserInfo.DelegateEmployeeID, SEL.Claims.claimId, SEL.Claims.approveClaimComplete);
                }
            },

            approveClaimComplete: function (data) {
                SEL.Claims.approving = false;
                if (data == 1) {
                    SEL.MasterPopup.ShowMasterPopup('A claim cannot be approved while there are still items waiting approval.');
                }
                else if (data == 2) {
                    SEL.MasterPopup.ShowMasterPopup('This claim cannot currently be approved as the next approver is on holiday.');
                }
                else if (data == 3) {
                    SEL.MasterPopup.ShowMasterPopup('The system is currently set not to allow approvers to approve their own claims. You should unsubmit your claim and notify your administrator that your signoff group needs updating.');
                }
                else if (data == 4) {
                    SEL.MasterPopup.ShowMasterPopup('The signoff group for this claim would go to the claimant for approval at one of the stages and the system is set not to allow this.  The claimant should unsubmit their claim and notify an administrator that their signoff group needs updating.');
                }
                else if (data == 5) {
                    SEL.MasterPopup.ShowMasterPopup('This claim cannot currently be approved as one or more assignment numbers associated to claim items do not have a supervisor specified.');
                }
                else if (data == 6) {
                    SEL.MasterPopup.ShowMasterPopup('This claim cannot currently be approved due to either one or more expense item cost code owners missing, no default cost code owner defined or no line manager defined for claimant. Next stage cannot be determined.');
                }
                else {
                    document.location = 'checkpaylist.aspx';
                }
            },

            declarationAgreed: function () {

                Spend_Management.claims.approveClaim(CurrentUserInfo.AccountID, CurrentUserInfo.EmployeeID, CurrentUserInfo.DelegateEmployeeID, SEL.Claims.claimId, SEL.Claims.approveClaimComplete);
            },

            DeclineApproverDeclaration: function () {
                SEL.Claims.approving = false;
            },

            deleteClaim: function (claimId) {
                if (confirm('Are you sure you wish to delete the selected claim?')) {
                    Spend_Management.claims.deleteClaim(CurrentUserInfo.AccountID, claimId, SEL.Claims.deleteClaimComplete);
                }
            },

            deleteClaimComplete: function () {
                SEL.Grid.refreshGrid('gridClaims', 1);
            },

            displayFlags: function (expenseIds) {

                Spend_Management.claims.displayFlags(CurrentUserInfo.AccountID, SEL.Claims.claimId, expenseIds, SEL.Claims.displayFlagsComplete);
            },

            displayFlagsComplete: function (data) {
                var modal = $find(SEL.Claims.modFlagsId);
                $g('divFlags').innerHTML = data;
                modal.show();
                $(document).keydown(function (e) {
                    if (e.keyCode === 27) // esc
                    {
                        SEL.Claims.hideFlagsModal();
                    }
                });
            },

            hideFlagsModal: function () {
                var modal = $find(SEL.Claims.modFlagsId);
                modal.hide();
            },
            allocateToMe: function () {
                var claimIds = SEL.Grid.getSelectedItemsFromGrid('gridUnallocated');
                if (claimIds.length == 0) {
                    SEL.MasterPopup.ShowMasterPopup('Please select one or more claims to allocate.');
                    return;
                }

                Spend_Management.claims.allocateClaims(CurrentUserInfo.AccountID, CurrentUserInfo.EmployeeID, claimIds, SEL.Claims.allocateToMeComplete);
            },

            allocateToMeComplete: function () {
                SEL.Grid.refreshGrid('gridClaims', 1);
                SEL.Grid.refreshGrid('gridUnallocated', 1);
            },

            unallocateClaim: function (claimId) {
                Spend_Management.claims.unallocateClaim(CurrentUserInfo.AccountID, CurrentUserInfo.EmployeeID, claimId, SEL.Claims.unallocateClaimComplete);
            },

            unallocateClaimComplete: function () {
                SEL.Grid.refreshGrid('gridClaims', 1);
                SEL.Grid.refreshGrid('gridUnallocated', 1);
            },

            oneClickApproveClaim: function (claimId) {
                Spend_Management.claims.OneClickApproveClaim(CurrentUserInfo.AccountID, CurrentUserInfo.EmployeeID, CurrentUserInfo.DelegateEmployeeID, claimId, SEL.Claims.OneClickApproveClaimComplete);
            },

            OneClickApproveClaimComplete: function (data) {
                switch (data) {
                    case 7:
                        SEL.MasterPopup.ShowMasterPopup('This claim cannot currently be approved as the next approver is on holiday.');
                        break;
                    case 9:
                        SEL.MasterPopup.ShowMasterPopup('This claim cannot currently be approved as one or more assignment numbers associated to claim items do not have a supervisor specified.');
                        break;
                    case 10:
                        SEL.MasterPopup.ShowMasterPopup('This claim cannot currently be approved due to either one or more expense item cost code owners missing, no default cost code owner defined or no line manager defined for claimant. Next stage cannot be determined.');
                        break;
                    case 13:
                        SEL.MasterPopup.ShowMasterPopup('The system is set to not allow approvers to approve their own claims. You should unsubmit your claim and notify your administrator that your signoff group needs updating.');
                        break;
                    case 14:
                        SEL.MasterPopup.ShowMasterPopup('The system is set to not allow approvers to approve their own claims and the current signoff group for this claim would go to the claimant for approval at one of the stages.  The claimant should unsubmit their claim and notify their administrator that their signoff group needs updating.');
                        break;
                    default:
                        SEL.Grid.refreshGrid('gridClaims', 1);
                        break;
                }
            },

            payClaim: function (claimId) {
                Spend_Management.claims.payClaim(CurrentUserInfo.AccountID, CurrentUserInfo.EmployeeID, CurrentUserInfo.DelegateEmployeeID, claimId, SEL.Claims.payClaimComplete);
            },

            payClaimComplete: function () {
                SEL.Grid.refreshGrid('gridClaims', 1);
            },

            filterCheckAndPayGrids: function () {
                var surname = $g(SEL.Claims.txtSurnameId).value;
                var filter = $g(SEL.Claims.cmbFilterId).options[$g(SEL.Claims.cmbFilterId).selectedIndex].value;
                Spend_Management.claims.filterCheckAndPayClaimGrid(CurrentUserInfo.AccountID, CurrentUserInfo.EmployeeID, surname, filter, SEL.Claims.filterCheckAndPayClaimGridComplete);
                Spend_Management.claims.filterCheckAndPayUnallocatedGrid(CurrentUserInfo.AccountID, CurrentUserInfo.EmployeeID, surname, filter, SEL.Claims.filterCheckAndPayUnallocatedGridComplete);
            },

            filterCheckAndPayClaimGridComplete: function (data) {
                $g('divGridClaims').innerHTML = data[1];
            },

            filterCheckAndPayUnallocatedGridComplete: function (data) {
                $g('divGridUnallocated').innerHTML = data[1];
            },
            GetClaimTotalAndAmountPayable: function () {
                Spend_Management.claims.GetClaimTotalAndAmountPayable(CurrentUserInfo.AccountID, CurrentUserInfo.SubAccountID, SEL.Claims.claimId, SEL.Claims.GetClaimTotalAndAmountPayableComplete);
            },

            GetClaimTotalAndAmountPayableComplete: function (data) {
                $g('divClaimTotal').innerHTML = data[0];
                $g('spanAmountPayable').innerHTML = data[1];
                $g('divNumberOfItems').innerHTML = data[2];
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


/// <summary>
/// Attachment type Methods
/// </summary>    
(function () {
    var scriptName = "BudgetHolders";
    function execute() {
        SEL.registerNamespace("SEL.BudgetHolders");
        SEL.BudgetHolders =
        {
            budgetHolderID: null,
            deleteBudgetHolder: function (budgetholderid) {
                if (confirm('Are you sure you wish to delete the selected budget holder?')) {
                    SEL.BudgetHolders.budgetHolderID = budgetholderid;
                    Spend_Management.svcBudgetHolders.deleteBudgetHolder(budgetholderid, SEL.BudgetHolders.deleteBudgetHolderComplete);

                }
            },

            deleteBudgetHolderComplete: function (data) {
                switch (data) {
                    case -1:
                        SEL.MasterPopup.ShowMasterPopup('The budget holder cannot be deleted as it is assigned to one or more signoff groups.', 'Message from ' + moduleNameHTML);
                        break;
                    case -2:
                        SEL.MasterPopup.ShowMasterPopup('The budget holder cannot be deleted as one or more claims are in the approval process that involve this budget holder.', 'Message from ' + moduleNameHTML);
                        break;
                    case -3:
                        SEL.MasterPopup.ShowMasterPopup('The budget holder cannot be deleted as it is assigned to one or more audiences.', 'Message from ' + moduleNameHTML);
                        break;
                    case -4:
                        SEL.MasterPopup.ShowMasterPopup('The budget holder cannot be deleted as it is an owner of one or more cost codes.', 'Message from ' + moduleNameHTML);
                        break;
                    case -5:
                        SEL.MasterPopup.ShowMasterPopup('The budget holder cannot be deleted as it is assigned to one or more approval matrices.', 'Message from ' + moduleNameHTML);
                        break;
                    case -10:
                        SEL.MasterPopup.ShowMasterPopup('The budget holder cannot be deleted as it is assigned to one or more GreenLights or user defined fields.', 'Message from ' + moduleNameHTML);
                        break;
                    case 0:
                        SEL.Grid.deleteGridRow('gridBudgetHolders', SEL.BudgetHolders.budgetHolderID);
                        break;
                }
            },

            /// <summary>
            /// Catch any error and show an error message to users
            /// </summary>
            OnError: function (error) {
                if (error["_message"] != null) {
                    SEL.MasterPopup.ShowMasterPopup(error["_message"], "Credit Card Error");
                }
                else {
                    SEL.MasterPopup.ShowMasterPopup(error, "Credit Card Error");
                }
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

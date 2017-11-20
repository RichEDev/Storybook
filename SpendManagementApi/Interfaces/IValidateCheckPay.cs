namespace SpendManagementApi.Interfaces
{
    using SpendManagementLibrary;
    using System.Collections.Generic;
    using Common.Enums;
    
    public interface IValidateCheckPay
    {
        void ValidateUnapproveExpenseItemAction(IEnumerable<cExpenseItem> expenseItems, Models.Types.Claim claim);
        void ValidateApproveReturnExpenseItemsAction(Models.Types.Claim claim, IEnumerable<int> expenseIds, CheckAndPayAction action);
        void ValidateClaimForCheckAndPayAction(Models.Types.Claim claim); 
    }
}

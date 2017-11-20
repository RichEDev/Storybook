namespace SpendManagementApi.Models.Types
{
    using Interfaces;
    using SpendManagementLibrary.ExpenseItems;
    
    /// <summary>
    /// The response for AllowExpenseItems
    /// </summary>
    public class AllowExpenseItems : BaseExternalType, IApiFrontForDbObject<SpendManagementLibrary.ExpenseItems.AllowExpenseItemsResult, AllowExpenseItems>
    {
        /// <summary>
        /// The number of approved items
        /// </summary>
        public int? NumberOfApprovedItems { get; set; }

        /// <summary>
        /// Does the claim have returned items
        /// </summary>
        public bool HasReturnedItems { get; set; }
        
        /// <summary>
        /// The <see cref="FlaggedItemsManager">FlaggedItemsManager</see>
        /// </summary>
        public FlaggedItemsManager FlaggedItemsManager { get; set; }
        
        /// <summary>
        /// Does the result have a message
        /// </summary>
        public bool HasMessage { get; set; }
        
        /// <summary>
        /// Is there no default authoriser present
        /// </summary>
        public bool NoDefaultAuthoriserPresent { get; set; }

        /// <summary>
        /// Changes a SML AllowExpenseItemsResult to AllowExpenseItems API type
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="actionContext"></param>
        /// <returns>The <see cref="AllowExpenseItems"></see> AllowExpenseItems</returns>
        public AllowExpenseItems From(AllowExpenseItemsResult dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            this.NumberOfApprovedItems = dbType.NumberOfApprovedItems;
            this.HasReturnedItems = dbType.HasReturnedItems;
            this.FlaggedItemsManager = new FlaggedItemsManager().From(dbType.FlaggedItemsManager, actionContext);
            this.HasMessage = dbType.HasMessage;
            this.NoDefaultAuthoriserPresent = dbType.NoDefaultAuthoriserPresent;

            return this;
        }

        public AllowExpenseItemsResult To(IActionContext actionContext)
        {
            throw new System.NotImplementedException();
        }
    }
}
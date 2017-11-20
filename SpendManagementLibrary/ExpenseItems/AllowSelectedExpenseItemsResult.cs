namespace SpendManagementLibrary.ExpenseItems
{
    using Flags;

    /// <summary>
    /// A class to handle the result of an approve expense items action
    /// </summary>
    public class AllowExpenseItemsResult
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
        /// Initialises a new instance of the <see cref="AllowExpenseItemsResult">AllowExpenseItemsResult</see>/> class.
        /// </summary>
        /// <param name="numberOfApprovals">Number of approvals</param>
        /// <param name="hasReturnedItems">Does the claim have returned items</param>
        /// <param name="flaggedItemsManager">The <see cref="FlaggedItemsManager">FlaggedItemsManager</see></param>
        /// <param name="HasMessage"> Does the result have a message</param>
        /// <param name="noDefaultAuthoriserPresent">Is there no default authorier present</param>
        public AllowExpenseItemsResult(int? numberOfApprovals, bool hasReturnedItems, FlaggedItemsManager flaggedItemsManager, bool HasMessage, bool noDefaultAuthoriserPresent)
        {
            this.NumberOfApprovedItems = numberOfApprovals;
            this.HasReturnedItems = hasReturnedItems;
            this.FlaggedItemsManager = flaggedItemsManager;
            this.HasMessage = HasMessage;
            this.NoDefaultAuthoriserPresent = noDefaultAuthoriserPresent;
        }
    }
}
 
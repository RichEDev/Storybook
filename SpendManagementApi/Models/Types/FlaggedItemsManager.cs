namespace SpendManagementApi.Models.Types
{
    using System;
    using Interfaces;
    using System.Collections.Generic;

    /// <summary>
    /// Handles the Flagged items for an expense item.
    /// </summary>
    public class FlaggedItemsManager : BaseExternalType, IApiFrontForDbObject<SpendManagementLibrary.Flags.FlaggedItemsManager, FlaggedItemsManager>
    {
        /// <summary>
        /// Gets or sets whether it is the claimant generating the current summary.
        /// </summary>
        public bool Claimant { get; set; }

        /// <summary>
        /// Gets or sets The stage the claim is at the flag line is on.
        /// </summary>
        public int? Stage { get; set; }

        /// <summary>
        /// Gets or sets whether the claim is currently being authorised.
        /// </summary>
        public bool Authorising { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether submitting claim.
        /// </summary>
        public bool SubmittingClaim { get; set; }

        /// <summary>
        /// Gets or sets whether to only showed the flagged items that have caused a block.
        /// </summary>
        public bool OnlyDisplayBlockedItems { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PageSource { get; set; }

        /// <summary>
        /// Gets or sets whether the authoriser is currently allowing items or approving the claim.
        /// </summary>
        public bool AllowingOrApproving { get; set; }

        /// <summary>
        /// Gets or sets the claimant name.
        /// </summary>
        public string ClaimantName { get; set; }

        /// <summary>
        /// Gets or sets the expense collection.
        /// </summary>
        public List<ExpenseItemFlagSummary> ExpenseCollection { get; set; }

        /// <summary>
        /// The number of flagged items in the collection.
        /// </summary>
        public int Count => this.ExpenseCollection?.Count ?? 0;

        /// <summary>
        /// Initialises a new instance of the <see cref="SpendManagementLibrary.Flags.FlaggedItemsManager"/> class.
        /// </summary>
        /// <param name="claimant">
        /// >Whether it is the claimant generating the current summary.
        /// </param>
        /// <param name="authorising">
        /// Whether the claim is currently being authorised.
        /// </param>
        /// <param name="submittingClaim">
        /// Whether the summary is being generated because the claim is currently being submitted.
        /// </param>
        /// <param name="onlyDisplayBlockedItems">
        /// Determines whether to only showed the flagged items that have caused a block.
        /// </param>
        /// <param name="pageSource">
        /// Is the summary being generate by the clam viewer or check and pay.
        /// </param>
        /// <param name="stage">
        /// The stage the claim is at the flag line is on.
        /// </param>
        public FlaggedItemsManager(bool claimant, bool authorising, bool submittingClaim, bool onlyDisplayBlockedItems, string pageSource, int? stage = null)
        {
            this.Claimant = claimant;
            this.Authorising = authorising;
            this.SubmittingClaim = submittingClaim;
            this.OnlyDisplayBlockedItems = onlyDisplayBlockedItems;
            this.PageSource = pageSource;
            this.Stage = stage;
            this.ExpenseCollection = new List<ExpenseItemFlagSummary>();
        }

        /// <summary>
        /// The <see cref="FlaggedItemsManager">FlaggedItemsManager</see> constructor
        /// </summary>
        public FlaggedItemsManager()
        {         
        }

        /// <summary>
        /// Converts a spendmanagementlibrary class of FlaggedItemsManager to an API type
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        public FlaggedItemsManager From(SpendManagementLibrary.Flags.FlaggedItemsManager dbType, IActionContext actionContext)
        {

            if (dbType == null)
            {
                return null;
            }

            this.Claimant = dbType.Claimant;
            this.Stage = dbType.Stage;
            this.Authorising = dbType.Authorising;
            this.SubmittingClaim = dbType.SubmittingClaim;
            this.OnlyDisplayBlockedItems = dbType.OnlyDisplayBlockedItems;
            this.PageSource = dbType.PageSource;
            this.AllowingOrApproving = dbType.AllowingOrApproving;
            this.ClaimantName = dbType.ClaimantName;
       
            return this;
        }

        public SpendManagementLibrary.Flags.FlaggedItemsManager To(IActionContext actionContext)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a new flag summary item to the collection.
        /// </summary>
        /// <param name="summary">
        /// The flagged summary item.
        /// </param>
        public void Add(ExpenseItemFlagSummary summary)
        {
            this.ExpenseCollection.Add(summary);
        }
    }
}
namespace SpendManagementLibrary.Flags
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Web.Script.Serialization;

    /// <summary>
    /// TA manager containing all parameters and items to build the flag modal.
    /// </summary>
    [Serializable]
    [DataContract]
    public class FlaggedItemsManager
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="FlaggedItemsManager"/> class.
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
        /// Initialises a new instance of the <see cref="FlaggedItemsManager"/> class.
        /// </summary>
        public FlaggedItemsManager()
        {
            this.ExpenseCollection = new List<ExpenseItemFlagSummary>();
        }


        /// <summary>
        /// Gets or sets the summary where the summary is being generate by the clam viewer or check and pay.
        /// </summary> 
        [DataMember]
        public string PageSource { get; set; }

        /// <summary>
        /// The number of flagged items in the collection.
        /// </summary>
        [DataMember]
        public int Count
        {
            get
            {
                return this.ExpenseCollection.Count;
            }
        }

        /// <summary>
        /// Gets a readonly list of the flagged items.
        /// </summary>
        [DataMember]
        public ReadOnlyCollection<ExpenseItemFlagSummary> List
        {
            get
            {
                return this.ExpenseCollection.AsReadOnly();
            }
        }


        /// <summary>
        /// Gets the first flagged item from the collection.
        /// </summary>
        [DataMember]
        public List<FlagSummary> First
        {
            get
            {
                return this.ExpenseCollection.Count == 0 ? null : this.ExpenseCollection[0].FlagCollection;
            }
        }

        /// <summary>
        /// Gets or sets whether it is the claimant generating the current summary.
        /// </summary>
        [DataMember]
        public bool Claimant { get; set; }

        /// <summary>
        /// Gets or sets The stage the claim is at the flag line is on.
        /// </summary>
        [DataMember]
        public int? Stage { get; set; }

        /// <summary>
        /// Gets or sets whether the claim is currently being authorised.
        /// </summary>
        [DataMember]
        public bool Authorising { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether submitting claim.
        /// </summary>
        [DataMember]
        public bool SubmittingClaim { get; set; }

        /// <summary>
        /// Gets or sets whether to only showed the flagged items that have caused a block.
        /// </summary>
        [DataMember]
        public bool OnlyDisplayBlockedItems { get; set; }

        /// <summary>
        /// Gets or sets the claimant name.
        /// </summary>
        [DataMember]
        public string ClaimantName { get; set; }

        /// <summary>
        /// Gets or sets whether the authoriser is currently allowing items or approving the claim.
        /// </summary>
        [DataMember]
        public bool AllowingOrApproving { get; set; }

        /// <summary>
        /// Gets or sets the expense collection.
        /// </summary>
        [DataMember]
        private List<ExpenseItemFlagSummary> ExpenseCollection { get; set; }

        /// <summary>
        /// Removes any expenses where the flag collection is empty.
        /// </summary>
        public void RemoveEmptyFlags()
        {
            for (int i = this.ExpenseCollection.Count - 1; i >= 0; i--)
            {
                if (this.ExpenseCollection[i].FlagCollection.Count == 0)
                {
                    this.ExpenseCollection.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Tries to get the flag summary item.
        /// </summary>
        /// <param name="expenseID">
        /// The expense id.
        /// </param>
        /// <param name="summaryItem">
        /// The flag summary item.
        /// </param>
        /// <returns>
        /// Whether setting the summary item was successful.
        /// </returns>
        public bool TryGetValue(int expenseID, out ExpenseItemFlagSummary summaryItem)
        {
            foreach (ExpenseItemFlagSummary summary in this.ExpenseCollection.Where(summary => summary.ExpenseID == expenseID))
            {
                summaryItem = summary;
                return true;
            }

            summaryItem = null;
            return false;
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

        /// <summary>
        /// Removes all items in the Expense Collection.
        /// </summary>
        public void ClearExpenseCollection()
        {
            this.ExpenseCollection.Clear();   
        }

        /// <summary>
        /// Serializes the flag manager as json.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string Serialize()
        {
            return new JavaScriptSerializer().Serialize(this);
        }
    }
}

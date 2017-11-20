namespace SpendManagementLibrary.Expedite
{
    using System;

    /// <summary>
    /// Represents a rule that an ExpenseItem will be validated against.
    /// This might be linked to a particular field in the metabase fields table.
    /// </summary>
    [Serializable]
    public class ExpenseValidationCriterion
    {
        /// <summary>
        /// The unique Id of this item in the database.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Field, if there is one, of this item in the metabase.
        /// </summary>
        public Guid? FieldId { get; set; }

        /// <summary>
        /// The notes that the validator can use to determine
        /// whether or not the relevant expense item will pass, fail, or neither.
        /// </summary>
        public string Requirements { get; set; }

        /// <summary>
        /// Whether to mark the expense item as possibly fraudulent if this criterion fails.
        /// </summary>
        public bool FraudulentIfFailsVAT { get; set; }

        /// <summary>
        /// The human friendly message to display when the result is "Found and Matched"
        /// </summary>
        public string FriendlyMessageFoundAndMatched { get; set; }
        
        /// <summary>
        /// The human friendly message to display when the result is "Found not Matched"
        /// </summary>
        public string FriendlyMessageFoundNotMatched { get; set; }
        
        /// <summary>
        /// The human friendly message to display when the result is "Found not Readable"
        /// </summary>
        public string FriendlyMessageFoundNotReadable { get; set; }
        
        /// <summary>
        /// The human friendly message to display when the result is "Not Found"
        /// </summary>
        public string FriendlyMessageNotFound { get; set; }
        
        #region Properties for a CustomSubcatCriterion

        /// <summary>
        /// Determines whether this Criterion should be validated!
        /// If false, don't bother with creating a result from it.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// The Account Id of this item (if not generic) in the database.
        /// </summary>
        public int? AccountId { get; set; }

        /// <summary>
        /// The ExpenseSubCategory Id of this item (if not generic) in the client database.
        /// </summary>
        public int? SubcatId { get; set; }

        #endregion Properties for a CustomSubcatCriterion
    }
}

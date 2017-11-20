namespace SpendManagementApi.Models.Types.Expedite
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Interfaces;
    using Utilities;

    /// <summary>
    /// Represents a rule that an ExpenseItem will be validated against.
    /// This might be linked to a particular field in the metabase fields table.
    /// </summary>
    public class ExpenseValidationCriterion : BaseExternalType, IApiFrontForDbObject<SpendManagementLibrary.Expedite.ExpenseValidationCriterion, ExpenseValidationCriterion>
    {
        /// <summary>
        /// The unique Id of this item in the database.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Account Id of this item (if not generic) in the database.
        /// </summary>
        public new int? AccountId { get; set; }

        /// <summary>
        /// The ExpenseSubCategory Id of this item (if not generic) in the database.
        /// </summary>
        public int? ExpenseSubCategoryId { get; set; }

        /// <summary>
        /// Whether this result is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Whether to mark the expense item as possibly fraudulent if this criterion fails.
        /// </summary>
        public bool FraudulentIfFailsVAT { get; set; }

        /// <summary>
        /// The Field, if there is one, of this item in the metabase.
        /// </summary>
        public Guid? FieldId { get; set; }

        /// <summary>
        /// The notes that the validator can use to determine
        /// whether or not the relevant expense item will pass, fail, or neither.
        /// </summary>
        [MaxLength(4000, ErrorMessage = ApiResources.ErrorMaxLength + @"4000")]
        public string Requirements { get; set; }

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

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>An api Type</returns>
        public ExpenseValidationCriterion From(SpendManagementLibrary.Expedite.ExpenseValidationCriterion dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            Id = dbType.Id;
            AccountId = dbType.AccountId;
            ExpenseSubCategoryId = dbType.SubcatId;
            FieldId = dbType.FieldId;
            Requirements = dbType.Requirements;
            FraudulentIfFailsVAT = dbType.FraudulentIfFailsVAT;
            Enabled = dbType.Enabled;
            FriendlyMessageFoundAndMatched = dbType.FriendlyMessageFoundAndMatched;
            FriendlyMessageFoundNotMatched = dbType.FriendlyMessageFoundNotMatched;
            FriendlyMessageFoundNotReadable = dbType.FriendlyMessageFoundNotReadable;
            FriendlyMessageNotFound = dbType.FriendlyMessageNotFound;
            return this;
        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>A data access layer Type</returns>
        public SpendManagementLibrary.Expedite.ExpenseValidationCriterion To(IActionContext actionContext)
        {
            return new SpendManagementLibrary.Expedite.ExpenseValidationCriterion
            {
                Id = Id,
                AccountId = AccountId,
                SubcatId = ExpenseSubCategoryId,
                FieldId = FieldId,
                Requirements = Requirements,
                FraudulentIfFailsVAT = FraudulentIfFailsVAT,
                Enabled = Enabled,
                FriendlyMessageFoundAndMatched = FriendlyMessageFoundAndMatched,
                FriendlyMessageFoundNotMatched = FriendlyMessageFoundNotMatched,
                FriendlyMessageFoundNotReadable = FriendlyMessageFoundNotReadable,
                FriendlyMessageNotFound = FriendlyMessageNotFound
            };
        }
    }
}

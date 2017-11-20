namespace SpendManagementLibrary.Flags
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The flagged item.
    /// </summary>
    [Serializable]
    [DataContract]
    public class FlaggedItem
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="FlaggedItem"/> class.
        /// </summary>
        /// <param name="flaggeditemid">
        /// The id of the flagged item.
        /// </param>
        /// <param name="flagtype">
        /// The flag type. Duplicate, Limit without receipt etc.
        /// </param>
        /// <param name="flagdescription">
        /// The flag description.
        /// </param>
        /// <param name="customflagtext">
        /// The flag description provided by the client administrator.
        /// </param>
        /// <param name="flagId">
        /// The flag id.
        /// </param>
        /// <param name="flagcolour">
        /// The flag colour.
        /// </param>
        /// <param name="claimantjustification">
        /// The justification provided by the claimant.
        /// </param>
        /// <param name="associatedExpenses">
        /// The expense item that have caused this flag to occur.
        /// </param>
        /// <param name="authoriserjustifications">
        /// The justifications provided by the authorisers.
        /// </param>
        /// <param name="flagTypeDescription">
        /// The flag Type Description.
        /// </param>
        /// <param name="notesForAuthoriser">
        /// The notes For Authoriser.
        /// </param>
        /// <param name="associatedExpenseItems">
        /// The associated Expense Items.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="customFlagText">
        /// The custom flag text provided by administrators to show to claimants in the event of a breach.
        /// </param>
        /// <param name="claimantJustificationDelegateID">
        /// The employeeid of the delegate.
        /// </param>
        /// <param name="delegateName">
        /// The full name of the delegate.
        /// </param>
        /// <param name="claimantJustificationMandatory">
        /// Whether it is mandatory for the claimant to provide a justification
        /// </param>
        /// <param name="authoriserJustificationMandatory">
        /// Whether it is mandatory for the authoriser to provide a justification
        /// </param>
        /// ///
        public FlaggedItem(int flaggeditemid, FlagType flagtype, string flagdescription, string customflagtext, int flagId, FlagColour flagcolour, string claimantjustification, List<AssociatedExpense> associatedExpenses, List<AuthoriserJustification> authoriserjustifications, string flagTypeDescription, string notesForAuthoriser, List<AssociatedExpenseItem> associatedExpenseItems, FlagAction action, string customFlagText, int? claimantJustificationDelegateID, string delegateName, bool claimantJustificationMandatory, bool authoriserJustificationMandatory, bool requiresRevalidationOnClaimSubmittal)
        {
            this.FlaggedItemId = flaggeditemid;
            this.Flagtype = flagtype;
            this.FlagDescription = flagdescription;
            this.FlagText = customflagtext;
            this.FlagId = flagId;
            this.FlagColour = flagcolour;
            this.ClaimantJustification = claimantjustification;
            this.AssociatedExpenses = associatedExpenses;
            this.AuthoriserJustifications = authoriserjustifications;
            this.FlagTypeDescription = flagTypeDescription;
            this.NotesForAuthoriser = notesForAuthoriser;
            this.AssociatedExpenseItems = associatedExpenseItems;
            this.Action = action;
            this.CustomFlagText = customFlagText;
            this.ClaimantJustificationDelegateID = claimantJustificationDelegateID;
            this.DelegateName = this.ClaimantJustificationDelegateID.HasValue ? delegateName : string.Empty;
            this.ClaimantJustificationMandatory = claimantJustificationMandatory;
            this.AuthoriserJustificationMandatory = authoriserJustificationMandatory;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="FlaggedItem"/> class.
        /// </summary>
        /// <param name="flagdescription">
        /// The flagdescription.
        /// </param>
        /// <param name="flagtext">
        /// The flagtext.
        /// </param>
        /// <param name="flag">
        /// The flag.
        /// </param>
        /// <param name="flagColour">
        /// The flag colour.
        /// </param>
        /// <param name="flagTypeDescription">
        /// The flag Type Description.
        /// </param>
        /// <param name="notesForAuthoriser">
        /// The notes For Authoriser.
        /// </param>
        /// <param name="associatedExpenseItems">
        /// The associated Expense Items.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="customFlagText">
        /// The custom flag text provided by administrators to show to claimants in the event of a breach.
        /// </param>
        /// <param name="claimantJustificationMandatory">
        /// Whether it is mandatory for the claimant to provide a justification
        /// </param>
        /// <param name="authoriserJustificationMandatory">
        /// Whether it is mandatory for the authoriser to provide a justification
        /// </param>
        public FlaggedItem(string flagdescription, string flagtext, Flag flag, FlagColour flagColour, string flagTypeDescription, string notesForAuthoriser, List<AssociatedExpenseItem> associatedExpenseItems, FlagAction action, string customFlagText, bool claimantJustificationMandatory, bool authoriserJustificationMandatory)
        {
            this.FlagDescription = flagdescription;
            this.Flagtype = flag.FlagType;
            this.FlagText = flagtext;
            this.FlagId = flag.FlagID;
            this.FlagColour = flagColour;
            this.AssociatedExpenses = new List<AssociatedExpense>();
            this.FlagTypeDescription = flagTypeDescription;
            this.NotesForAuthoriser = notesForAuthoriser;
            this.AssociatedExpenseItems = associatedExpenseItems;
            this.Action = action;
            this.CustomFlagText = customFlagText;
            this.ClaimantJustificationMandatory = claimantJustificationMandatory;
            this.AuthoriserJustificationMandatory = authoriserJustificationMandatory;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="FlaggedItem"/> class.
        /// </summary>
        /// <param name="flagdescription">
        /// The flagdescription.
        /// </param>
        /// <param name="flagtext">
        /// The flagtext.
        /// </param>
        /// <param name="flag">
        /// The flag.
        /// </param>
        /// <param name="flagColour">
        /// The flag colour.
        /// </param>
        /// <param name="associatedExpenses">
        /// The associated expenses.
        /// </param>
        /// <param name="flagTypeDescription">
        /// The flag Type Description.
        /// </param>
        /// <param name="notesForAuthoriser">
        /// The notes For Authoriser.
        /// </param>
        /// <param name="associatedExpenseItems">
        /// The associated Expense Items.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="customFlagText">
        /// The custom flag text provided by administrators to show to claimants in the event of a breach.
        /// </param>
        /// <param name="claimantJustificationMandatory">
        /// Whether it is mandatory for the claimant to provide a justification
        /// </param>
        /// <param name="authoriserJustificationMandatory">
        /// Whether it is mandatory for the authoriser to provide a justification
        /// </param>
        public FlaggedItem(string flagdescription, string flagtext, Flag flag, FlagColour flagColour, List<AssociatedExpense> associatedExpenses, string flagTypeDescription, string notesForAuthoriser, List<AssociatedExpenseItem> associatedExpenseItems, FlagAction action, string customFlagText, bool claimantJustificationMandatory, bool authoriserJustificationMandatory)
        {
            this.FlagDescription = flagdescription;
            this.Flagtype = flag.FlagType;
            this.FlagText = flagtext;
            this.FlagId = flag.FlagID;
            this.FlagColour = flagColour;
            this.AssociatedExpenses = associatedExpenses;
            this.FlagTypeDescription = flagTypeDescription;
            this.NotesForAuthoriser = notesForAuthoriser;
            this.AssociatedExpenseItems = associatedExpenseItems;
            this.Action = action;
            this.CustomFlagText = customFlagText;
            this.ClaimantJustificationMandatory = claimantJustificationMandatory;
            this.AuthoriserJustificationMandatory = authoriserJustificationMandatory;
        }

        #region properties

        /// <summary>
        /// Gets or sets the flagged item id.
        /// </summary>
        [DataMember]
        public int FlaggedItemId { get; set; }

        /// <summary>
        /// Gets the flagtype.
        /// </summary>
        [DataMember]
        public FlagType Flagtype { get; private set; }

        /// <summary>
        /// Gets the standard text description provided by SEL
        /// </summary>
        [DataMember]
        public string FlagDescription { get; set; }

        /// <summary>
        /// Gets the custom text supplied by the client
        /// </summary>
        [DataMember]
        public string FlagText { get; private set; }

        /// <summary>
        /// Gets the flag id.
        /// </summary>
        [DataMember]
        public int FlagId { get; private set; }
        
        /// <summary>
        /// Gets the flag colour.
        /// </summary>
        [DataMember]
        public FlagColour FlagColour { get; private set; }

        /// <summary>
        /// Gets the claimant justification.
        /// </summary>
        [DataMember]
        public string ClaimantJustification { get; private set; }

        /// <summary>
        /// Gets the associated expenses.
        /// </summary>
        [DataMember]
        public List<AssociatedExpense> AssociatedExpenses { get; private set; }

        /// <summary>
        /// Gets or sets the reason why a flag has failed validation
        /// </summary>
        [DataMember]
        public FlagFailureReason FailureReason { get; set; }

        /// <summary>
        /// Gets the justifications provided by the authorisers
        /// </summary>
        [DataMember]
        public List<AuthoriserJustification> AuthoriserJustifications { get; private set; }

        /// <summary>
        /// Gets or sets the date of the expense the flagged item is for
        /// </summary>
        [DataMember]
        public DateTime ExpenseDate { get; set; }

        /// <summary>
        /// Gets or sets the total of the expense the flagged item is for
        /// </summary>
        [DataMember]
        public decimal ExpenseTotal { get; set; }

        /// <summary>
        /// Gets or sets the expense item the flaged item is for
        /// </summary>
        [DataMember]
        public string ExpenseSubcat { get; set; }

        /// <summary>
        /// Gets or set the currency symbol of the expense
        /// </summary>
        [DataMember]
        public string ExpenseCurrencySymbol { get; set; }

        [DataMember]
        public string FlagTypeDescription { get; protected set; }

        [DataMember]
        public string NotesForAuthoriser { get; private set; }

        [DataMember]
        public List<AssociatedExpenseItem> AssociatedExpenseItems { get; private set; }

        [DataMember]
        public FlagAction Action { get; private set; }

        /// <summary>
        /// Gets or sets the custom flag text provided by administrators to show to claimants in the event of a breach.
        /// </summary>
        [DataMember]
        public string CustomFlagText { get; private set; }

        /// <summary>
        /// Gets or sets the id of the employee providing the justification if it was a delegate
        /// </summary>
        [DataMember]
        public int? ClaimantJustificationDelegateID { get; private set; }

        /// <summary>
        /// The full name of the delegate providing the justification
        /// </summary>
        [DataMember]
        public string DelegateName { get; private set; }

        /// <summary>
        /// Gets or sets whether it is mandatory for the claimant to provide a justification
        /// </summary>
        [DataMember]
        public bool ClaimantJustificationMandatory { get; private set; }

        /// <summary>
        /// Gets or sets whether it is mandatory for the authoriser to provide a justification
        /// </summary>
        
        public bool AuthoriserJustificationMandatory { get; private set; }

        /// <summary>
        /// Gets or sets whether the flag needs to be revalidated upon claim submission
        /// </summary>
        [DataMember]
        public bool RequiresRevalidationOnClaimSubmittal { get; private set; }
        #endregion
    }
}

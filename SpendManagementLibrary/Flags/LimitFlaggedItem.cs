namespace SpendManagementLibrary.Flags
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The limit flagged item.
    /// </summary>
    [Serializable]
    [DataContract]
    public class LimitFlaggedItem : FlaggedItem
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="LimitFlaggedItem"/> class.
        /// </summary>
        /// <param name="flagdescription">
        /// The flag description.
        /// </param>
        /// <param name="customflagtext">
        /// The flag description provided by the client administrator.
        /// </param>
        /// <param name="flag">
        /// The flag.
        /// </param>
        /// <param name="flagColour">
        /// The flag Colour.
        /// </param>
        /// <param name="associatedexpenses">
        /// The associateditems.
        /// </param>
        /// <param name="exceededlimit">
        /// The amount the flag was exceeded by.
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
        /// ///
        public LimitFlaggedItem(
            string flagdescription,
            string customflagtext,
            Flag flag,
            FlagColour flagColour,
            List<AssociatedExpense> associatedexpenses,
            decimal exceededlimit, string flagTypeDescription, string notesForAuthoriser, List<AssociatedExpenseItem> associatedExpenseItems, FlagAction action, string customFlagText, bool claimantJustificationMandatory, bool authoriserJustificationMandatory)
            : base(flagdescription, customflagtext, flag, flagColour, associatedexpenses, flagTypeDescription, notesForAuthoriser, associatedExpenseItems, action, customFlagText, claimantJustificationMandatory, authoriserJustificationMandatory)
        {
            this.ExceededLimit = exceededlimit;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="LimitFlaggedItem"/> class.
        /// </summary>
        /// <param name="flaggeditemid">
        /// The flaggeditemid.
        /// </param>
        /// <param name="flagtype">
        /// The type of flag. Duplicate limit with a receipt etc.
        /// </param>
        /// <param name="flagdescription">
        /// The flag description.
        /// </param>
        /// <param name="customflagtext">
        /// The flag description provided by the client administrator.
        /// </param>
        /// <param name="flagId">
        /// The flag Id.
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
        /// <param name="exceededlimit">
        /// The amount the flag was exceeded by.
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
        /// ///
        public LimitFlaggedItem(
            int flaggeditemid,
            FlagType flagtype,
            string flagdescription,
            string customflagtext,
            int flagId,
            FlagColour flagcolour,
            string claimantjustification,
            List<AssociatedExpense> associatedExpenses,
            List<AuthoriserJustification> authoriserjustifications,
            decimal exceededlimit, string flagTypeDescription, string notesForAuthoriser, List<AssociatedExpenseItem> associatedExpenseItems, FlagAction action, string customFlagText, int? claimantJustificationDelegateID, string delegateName, bool claimantJustificationMandatory, bool authoriserJustificationMandatory,
            bool requiresRevalidationOnClaimSubmittal)
            : base(
                flaggeditemid,
                flagtype,
                flagdescription,
                customflagtext,
                flagId,
                flagcolour,
                claimantjustification,
                associatedExpenses,
                authoriserjustifications,
            flagTypeDescription,
            notesForAuthoriser,
            associatedExpenseItems,
            action,
            customFlagText,
            claimantJustificationDelegateID,
            delegateName,
            claimantJustificationMandatory,
            authoriserJustificationMandatory,
            requiresRevalidationOnClaimSubmittal)
        {
            this.ExceededLimit = exceededlimit;
        }
        #region Properties
        /// <summary>
        /// Gets the exceeded limit.
        /// </summary>
        [DataMember]
        public decimal ExceededLimit { get; private set; }
        #endregion
    }
}

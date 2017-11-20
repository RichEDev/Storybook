namespace SpendManagementLibrary.Flags
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The duplicate flagged item.
    /// </summary>
    [Serializable]
    [DataContract]
    public class DuplicateFlaggedItem : FlaggedItem
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="DuplicateFlaggedItem"/> class.
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
        /// <param name="duplicateExpense">
        /// The duplicate Expense.
        /// </param>
        /// <param name="flagTypeDescription">
        /// The flag type description.
        /// </param>
        /// <param name="notesForAuthoriser">
        /// The help notes to be displayed to the authoriser.
        /// </param>
        /// <param name="associatedExpenseItems">
        /// The associated Expense Items.
        /// </param>
        /// <param name="action">
        /// The flag action. Block or flag.
        /// </param>
        /// <param name="customFlagText">
        /// The custom flag text provided by administrators to show to claimants in the event of a breach.
        /// </param>
        /// <param name="fields">
        /// The list of field descriptions associated with the flag.
        /// </param>
        /// <param name="claimantJustificationMandatory">
        /// Whether it is mandatory for the claimant to provide a justification
        /// </param>
        /// <param name="authoriserJustificationMandatory">
        /// Whether it is mandatory for the authoriser to provide a justification
        /// </param>
        /// ///
        public DuplicateFlaggedItem(
            string flagdescription,
            string flagtext,
            Flag flag,
            FlagColour flagColour,
            AssociatedExpense duplicateExpense,
            string flagTypeDescription,
            string notesForAuthoriser,
            List<AssociatedExpenseItem> associatedExpenseItems,
            FlagAction action,
            string customFlagText,
            List<string> fields, 
            bool claimantJustificationMandatory, 
            bool authoriserJustificationMandatory)
            : base(
                flagdescription,
                flagtext,
                flag,
                flagColour,
                flagTypeDescription,
                notesForAuthoriser,
                associatedExpenseItems,
                action,
                customFlagText,
            claimantJustificationMandatory,
            authoriserJustificationMandatory)
        {
            this.AssociatedExpenses.Add(duplicateExpense);
            this.Fields = fields;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="DuplicateFlaggedItem"/> class.
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
        /// <param name="duplicateExpense">
        /// The duplicate Expense.
        /// </param>
        /// <param name="flagTypeDescription">
        /// The flag Type Description.
        /// </param>
        /// <param name="notesForAuthoriser">
        /// The help notes to be displayed to the authoriser.
        /// </param>
        /// <param name="associatedExpenseItems">
        /// The associated Expense Items.
        /// </param>
        /// <param name="action">
        /// The flag action.
        /// </param>
        /// <param name="customFlagText">
        /// The custom flag text provided by administrators to show to claimants in the event of a breach.
        /// </param>
        /// <param name="fields">
        /// The list of field descriptions associated with the flag.
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
        public DuplicateFlaggedItem(
            int flaggeditemid,
            FlagType flagtype,
            string flagdescription,
            string customflagtext,
            int flagId,
            FlagColour flagcolour,
            string claimantjustification,
            List<AssociatedExpense> associatedExpenses,
            List<AuthoriserJustification> authoriserjustifications,
            AssociatedExpense duplicateExpense, 
            string flagTypeDescription, 
            string notesForAuthoriser,
            List<AssociatedExpenseItem> associatedExpenseItems, 
            FlagAction action,
            string customFlagText,
            List<string> fields,
            int? claimantJustificationDelegateID,
            string delegateName, 
            bool claimantJustificationMandatory, 
            bool authoriserJustificationMandatory,
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
            this.Fields = fields;
        }
        #region Properties

        /// <summary>
        /// Gets the id of the expense that caused the duplicate.
        /// </summary>
        [DataMember]
        public int DuplicateExpenseID
        {
            get
            {
                if (this.AssociatedExpenses.Count > 0)
                {
                    return this.AssociatedExpenses[0].ExpenseID;
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets or sets a list of field descriptions that applies to this flag.
        /// </summary>
        [DataMember]
        public List<string> Fields { get; protected set; }
        #endregion
    }
}

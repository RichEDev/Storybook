namespace SpendManagementLibrary.Flags
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The mileage flagged item.
    /// </summary>
    [Serializable]
    [DataContract]
    public class MileageFlaggedItem : FlaggedItem
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="MileageFlaggedItem"/> class.
        /// </summary>
        /// <param name="flagdescription">
        /// The flag description.
        /// </param>
        /// <param name="flagtext">
        /// The flag text.
        /// </param>
        /// <param name="flag">
        /// The flag.
        /// </param>
        /// <param name="flagColour">
        /// The flag colour.
        /// </param>
        /// <param name="associatedExpenseItems"></param>
        /// <param name="action">Whether the expense has been flagged or blocked.</param>
        /// <param name="customFlagText">The custom flag text provided by administrators to show to claimants in the event of a breach.</param>
        /// <param name="claimantJustificationMandatory">
        /// Whether it is mandatory for the claimant to provide a justification
        /// </param>
        /// <param name="authoriserJustificationMandatory">
        /// Whether it is mandatory for the authoriser to provide a justification
        /// </param>
        /// ///
        public MileageFlaggedItem(
            string flagdescription,
            string flagtext,
            Flag flag,
            FlagColour flagColour,
            string flagTypeDescription,
            string notesForAuthoriser,
            List<AssociatedExpenseItem> associatedExpenseItems,
            FlagAction action,
            string customFlagText, 
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
            FlaggedJourneySteps = new List<JourneyStepFlaggedItem>();
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="MileageFlaggedItem"/> class.
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
        /// ///
        public MileageFlaggedItem(
            int flaggeditemid,
            FlagType flagtype,
            string flagdescription,
            string customflagtext,
            int flagId,
            FlagColour flagcolour,
            string claimantjustification,
            List<AssociatedExpense> associatedExpenses,
            List<AuthoriserJustification> authoriserjustifications,
            string flagTypeDescription,
            string notesForAuthoriser,
            List<AssociatedExpenseItem> associatedExpenseItems,
            FlagAction action,
            string customFlagText,
            int? claimantJustificationDelegateID,
            string delegateName, 
            bool claimantJustificationMandatory, 
            bool authoriserJustificationMandatory,
            bool requiresRevalidationOnClaimSubmittal)
            : base(flaggeditemid,
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
            FlaggedJourneySteps = new List<JourneyStepFlaggedItem>();
        }

        /// <summary>
        /// Adds the flagged journey step
        /// </summary>
        /// <param name="stepNumber">The journey step number</param>
        /// <param name="exceededAmount">The number of miles/passengers the flag rule was exceeded by</param>
        /// <param name="flagDescription">The flag description</param>
        /// <param name="flaggedItemId">The flagged item id</param>
        /// <param name="claimantJustification">The justification provided by the claimant</param>
        public void AddFlaggedJourneyStep(int stepNumber, decimal exceededAmount, string flagDescription, int flaggedItemId, string claimantJustification = null)
        {
            FlaggedJourneySteps.Add(new JourneyStepFlaggedItem(stepNumber, exceededAmount, flagDescription, flaggedItemId, claimantJustification));
        }

        #region Properties

        [DataMember]
        public List<JourneyStepFlaggedItem> FlaggedJourneySteps { get; set; }
        #endregion
    }


}

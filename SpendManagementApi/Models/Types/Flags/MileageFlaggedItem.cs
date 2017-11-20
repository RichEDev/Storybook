namespace SpendManagementApi.Models.Types.Flags
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SpendManagementApi.Interfaces;

    using SpendManagementLibrary.Flags;

    using AssociatedExpense = SpendManagementApi.Models.Types.AssociatedExpense;
    using AssociatedExpenseItem = SpendManagementApi.Models.Types.AssociatedExpenseItem;
    using AuthoriserJustification = SpendManagementApi.Models.Types.AuthoriserJustification;
    using FlagAction = SpendManagementApi.Common.Enums.FlagAction;
    using FlagColour = SpendManagementApi.Common.Enums.FlagColour;
    using FlaggedItem = SpendManagementApi.Models.Types.FlaggedItem;
    using FlagType = SpendManagementApi.Common.Enum.FlagType;

    /// <summary>
    /// The mileage flagged item.
    /// </summary>
    public class MileageFlaggedItem : FlaggedItem, IApiFrontForDbObject<SpendManagementLibrary.Flags.MileageFlaggedItem, MileageFlaggedItem>
    {
        /// <summary>
        /// Gets or sets the flagged journey steps.
        /// </summary>
        public List<JorneyStepFlaggedItem> FlaggedJourneySteps { get; set; }

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>An api Type</returns>
        public MileageFlaggedItem From(SpendManagementLibrary.Flags.MileageFlaggedItem dbType, IActionContext actionContext)
        {
            base.FlaggedItemId = dbType.FlaggedItemId;
            base.Flagtype = (FlagType)dbType.Flagtype;
            base.FlagTypeDescription = dbType.FlagTypeDescription;
            base.CustomFlagText = dbType.CustomFlagText;
            base.FlagId = dbType.FlagId;
            base.FlagColour = (FlagColour)dbType.FlagColour;
            base.ClaimantJustification = dbType.ClaimantJustification;
            base.AssociatedExpenses =
                dbType.AssociatedExpenses.Select(
                    expense =>
                    new AssociatedExpense(
                        expense.ClaimName,
                        Convert.ToDateTime(expense.Date),
                        expense.ReferenceNumber,
                        expense.Total,
                        expense.ExpenseItem,
                        expense.ExpenseID)).ToList();


            if (dbType.AuthoriserJustifications == null)
            {
                base.AuthoriserJustifications = new List<AuthoriserJustification>();
            }
            else
            {
                base.AuthoriserJustifications = dbType.AuthoriserJustifications.Select(
                      justifications =>
                      new AuthoriserJustification(
                          justifications.FlaggedItemID,
                          justifications.Stage,
                          justifications.FullName,
                          justifications.Justification,
                          justifications.DateStamp,
                          justifications.DelegateID)).ToList();

            }

            base.FlagDescription = dbType.FlagDescription;
            base.NotesForAuthoriser = dbType.NotesForAuthoriser;

            base.AssociatedExpenseItems =
              dbType.AssociatedExpenseItems.Select(
                  associatedExpenseItem =>
                  new AssociatedExpenseItem(associatedExpenseItem.SubcatID, associatedExpenseItem.Name)).ToList();

            base.Action = (FlagAction)dbType.Action;
            base.ClaimantJustificationDelegateID = dbType.ClaimantJustificationDelegateID;
            base.DelegateName = dbType.DelegateName;
            base.ClaimantJustificationMandatory = dbType.ClaimantJustificationMandatory;
            base.AuthoriserJustificationMandatory = dbType.AuthoriserJustificationMandatory;

            var flaggedJourneySteps = new List<JorneyStepFlaggedItem>();

            foreach (JourneyStepFlaggedItem flaggedStep in dbType.FlaggedJourneySteps)
            {
                flaggedJourneySteps.Add(new JorneyStepFlaggedItem().From(flaggedStep, actionContext));
            }

            this.FlaggedJourneySteps = flaggedJourneySteps;

            return this;
        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>A data access layer Type</returns>
        public SpendManagementLibrary.Flags.MileageFlaggedItem To(IActionContext actionContext)
        {
            throw new NotImplementedException();
        }
    }
}
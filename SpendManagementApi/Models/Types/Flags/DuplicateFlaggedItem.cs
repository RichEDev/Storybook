namespace SpendManagementApi.Models.Types
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SpendManagementApi.Interfaces;

    using FlagAction = SpendManagementApi.Common.Enums.FlagAction;
    using FlagColour = SpendManagementApi.Common.Enums.FlagColour;
    using FlagFailureReason = SpendManagementApi.Common.Enums.FlagFailureReason;
    using FlagType = SpendManagementApi.Common.Enum.FlagType;

    /// <summary>
    /// The duplicate flagged item.
    /// </summary>
    public class DuplicateFlaggedItem : FlaggedItem, IApiFrontForDbObject<SpendManagementLibrary.Flags.DuplicateFlaggedItem, DuplicateFlaggedItem>
    {
        /// <summary>
        /// Gets the id of the expense that caused the duplicate.
        /// </summary>
        public int DuplicateExpenseID
        {
            get
            {
                if (base.AssociatedExpenses.Count > 0)
                {
                    return this.AssociatedExpenses[0].ExpenseId;
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets or sets a list of field descriptions that applies to this flag.
        /// </summary>
        public List<string> Fields { get; protected set; }

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>An api Type</returns>
        public DuplicateFlaggedItem From(
            SpendManagementLibrary.Flags.DuplicateFlaggedItem dbType,
            IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            base.FlaggedItemId = dbType.FlaggedItemId;
            base.Flagtype = (FlagType)dbType.Flagtype;
            base.FlagTypeDescription = dbType.FlagTypeDescription;
            base.FlagText = dbType.FlagText;
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
            base.FailureReason = (FlagFailureReason)dbType.FailureReason;

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

            base.ExpenseDate = dbType.ExpenseDate;
            this.ExpenseTotal = dbType.ExpenseTotal;
            base.ExpenseSubcat = dbType.ExpenseSubcat;
            this.ExpenseCurrencySymbol = dbType.ExpenseCurrencySymbol;
            this.FlagDescription = dbType.FlagDescription;
            base.NotesForAuthoriser = dbType.NotesForAuthoriser;
            base.AssociatedExpenseItems =
                dbType.AssociatedExpenseItems.Select(
                    associatedExpenseItem =>
                    new AssociatedExpenseItem(associatedExpenseItem.SubcatID, associatedExpenseItem.Name)).ToList();
            base.Action = (FlagAction)dbType.Action;
            base.CustomFlagText = dbType.CustomFlagText;
            base.ClaimantJustificationDelegateID = dbType.ClaimantJustificationDelegateID;
            base.DelegateName = dbType.DelegateName;
            base.ClaimantJustificationMandatory = dbType.ClaimantJustificationMandatory;
            base.AuthoriserJustificationMandatory = dbType.AuthoriserJustificationMandatory;

            this.Fields = dbType.Fields;

            return this;

        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>A data access layer Type</returns>
        public SpendManagementLibrary.Flags.DuplicateFlaggedItem To(IActionContext actionContext)
        {
            throw new NotImplementedException();
        }
    }
}

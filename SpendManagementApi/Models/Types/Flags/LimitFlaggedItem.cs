namespace SpendManagementApi.Models.Types.Flags
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SpendManagementApi.Common.Enum;
    using SpendManagementApi.Common.Enums;
    using SpendManagementApi.Interfaces;

    /// <summary>
    /// The limit flagged item.
    /// </summary>
    public class LimitFlaggedItem : FlaggedItem, IApiFrontForDbObject<SpendManagementLibrary.Flags.LimitFlaggedItem, LimitFlaggedItem>
    {
        /// <summary>
        /// Gets the exceeded limit.
        /// </summary>
        public decimal ExceededLimit { get; private set; }

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>An api Type</returns>
        public LimitFlaggedItem From(SpendManagementLibrary.Flags.LimitFlaggedItem dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

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
            this.ExceededLimit = dbType.ExceededLimit;

            return this;
        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>A data access layer Type</returns>
        SpendManagementLibrary.Flags.LimitFlaggedItem IApiFrontForDbObject<SpendManagementLibrary.Flags.LimitFlaggedItem, LimitFlaggedItem>.To(IActionContext actionContext)
        {
            throw new NotImplementedException();
        }   
    }
}
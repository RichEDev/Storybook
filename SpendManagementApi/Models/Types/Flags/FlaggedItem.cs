namespace SpendManagementApi.Models.Types
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SpendManagementApi.Common.Enums;
    using Interfaces;

    using SpendManagementApi.Common.Enum;

    /// <summary>
    /// Represents an item which has been flagged by the flags and limits module.
    /// </summary>
    public class FlaggedItem
    {
        /// <summary>
        /// Gets or sets the flagged item id.
        /// </summary>

        public int FlaggedItemId { get; set; }

        /// <summary>
        /// Gets the flagtype.
        /// </summary>

        public FlagType Flagtype { get;  set; }

        /// <summary>
        /// Gets the standard text description provided by SEL
        /// </summary>
        public string FlagDescription { get; set; }

        /// <summary>
        /// Gets the custom text supplied by the client
        /// </summary>   
        public string FlagText { get;  set; }

        /// <summary>
        /// Gets the flag id.
        /// </summary>
        public int FlagId { get;  set; }

        /// <summary>
        /// Gets the flag colour.
        /// </summary>
        public FlagColour FlagColour { get;  set; }

        /// <summary>
        /// Gets the claimant justification.
        /// </summary>
        public string ClaimantJustification { get;  set; }

        /// <summary>
        /// Gets the associated expenses.
        /// </summary>
        public List<AssociatedExpense> AssociatedExpenses { get;  set; }

        /// <summary>
        /// Gets or sets the reason why a flag has failed validation
        /// </summary>
        public FlagFailureReason FailureReason { get; set; }

        /// <summary>
        /// Gets the justifications provided by the authorisers
        /// </summary>
        public List<AuthoriserJustification> AuthoriserJustifications { get;  set; }

        /// <summary>
        /// Gets or sets the date of the expense the flagged item is for
        /// </summary>
        public DateTime ExpenseDate { get; set; }

        /// <summary>
        /// Gets or sets the total of the expense the flagged item is for
        /// </summary>
        public decimal ExpenseTotal { get; set; }

        /// <summary>
        /// Gets or sets the expense item the flaged item is for
        /// </summary>
        public string ExpenseSubcat { get; set; }

        /// <summary>
        /// Gets or set the currency symbol of the expense
        /// </summary>
        public string ExpenseCurrencySymbol { get; set; }

        /// <summary>
        /// Gets or sets the Description for the flagged item
        /// </summary>
        public string FlagTypeDescription { get;  set; }

        /// <summary>
        /// Gets or sets the Notes for authorise for the flagged item
        /// </summary>
        public string NotesForAuthoriser { get;  set; }


        /// <summary>
        /// Gets or sets a list of associated expense items for the flagged item
        /// </summary>
        public List<AssociatedExpenseItem> AssociatedExpenseItems { get;  set; }

        /// <summary>
        /// Gets or sets the FlagAction for the flagged item
        /// </summary>
        public FlagAction Action { get;  set; }

        /// <summary>
        /// Gets or sets the custom flag text provided by administrators to show to claimants in the event of a breach.
        /// </summary>
        public string CustomFlagText { get;  set; }

        /// <summary>
        /// Gets or sets the id of the employee providing the justification if it was a delegate
        /// </summary>
        public int? ClaimantJustificationDelegateID { get;  set; }

        /// <summary>
        /// The full name of the delegate providing the justification
        /// </summary>
        public string DelegateName { get;  set; }

        /// <summary>
        /// Gets or sets whether it is mandatory for the claimant to provide a justification
        /// </summary>
        public bool ClaimantJustificationMandatory { get;  set; }

        /// <summary>
        /// Gets or sets whether it is mandatory for the authoriser to provide a justification
        /// </summary>

        public bool AuthoriserJustificationMandatory { get;  set; }

        /// <summary>
        /// Gets a value indicating whether as soon as the expense has been added the claimant should be notified about this flag
        /// </summary>
        public bool DisplayFlagImmediately { get; set; }


        /// <summary>
        /// Get or set whether the flagged item is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the account id.
        /// </summary>
        protected int AccountID { get; set; }

        /// <summary>
        /// Gets a value indicating whether the claimant must supply a justification if the item has been flagged
        /// </summary>
        public bool ClaimantJustificationRequired { get;  set; }

        /// <summary>
        /// Converts a spend management FlaggedItem to an api version.
        /// </summary>
        /// <param name="dbType">The spend management instance of FlaggedItem.</param>
        /// <param name="actionContext">The action context.</param>
        /// <returns>An api version of the journey step.</returns>
        public FlaggedItem From(SpendManagementLibrary.Flags.FlaggedItem dbType, IActionContext actionContext)
        {
            this.FlaggedItemId = dbType.FlaggedItemId;
            this.Flagtype = (FlagType) dbType.Flagtype;
            this.FlagTypeDescription = dbType.FlagTypeDescription;
            this.FlagText = dbType.FlagText;
            this.FlagId = dbType.FlagId;
            this.FlagColour = (FlagColour) dbType.FlagColour;
            this.ClaimantJustification = dbType.ClaimantJustification;
            this.AssociatedExpenses = dbType.AssociatedExpenses.Select(expense => new AssociatedExpense(expense.ClaimName,Convert.ToDateTime(expense.Date), expense.ReferenceNumber, expense.Total, expense.ExpenseItem, expense.ExpenseID)).ToList();
            this.FailureReason = (FlagFailureReason) dbType.FailureReason;

            if (dbType.AuthoriserJustifications == null)
            {
                this.AuthoriserJustifications = new List<AuthoriserJustification>();
            }
            else
            {
             dbType.AuthoriserJustifications.Select(justifications => new AuthoriserJustification(justifications.FlaggedItemID,justifications.Stage,justifications.FullName,justifications.Justification,justifications.DateStamp,justifications.DelegateID)).ToList();           
   
            }

            this.ExpenseDate = dbType.ExpenseDate;
            this.ExpenseTotal = dbType.ExpenseTotal;
            this.ExpenseSubcat = dbType.ExpenseSubcat;
            this.ExpenseCurrencySymbol = dbType.ExpenseCurrencySymbol;
            this.FlagDescription = dbType.FlagDescription;
            this.NotesForAuthoriser = dbType.NotesForAuthoriser;
            this.AssociatedExpenseItems = dbType.AssociatedExpenseItems.Select(associatedExpenseItem => new AssociatedExpenseItem(associatedExpenseItem.SubcatID, associatedExpenseItem.Name)).ToList();
            this.Action = (FlagAction) dbType.Action;
            this.CustomFlagText = dbType.CustomFlagText;
            this.ClaimantJustificationDelegateID = dbType.ClaimantJustificationDelegateID;
            this.DelegateName = dbType.DelegateName;
            this.ClaimantJustificationMandatory = dbType.ClaimantJustificationMandatory;
            this.AuthoriserJustificationMandatory = dbType.AuthoriserJustificationMandatory;

            return this;
        }
    }
}
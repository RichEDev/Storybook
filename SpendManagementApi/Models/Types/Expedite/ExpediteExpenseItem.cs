

namespace SpendManagementApi.Models.Types.Expedite
{
    using SpendManagementApi.Interfaces;

    using SpendManagementLibrary;

    using Spend_Management;

    using System;

    /// <summary>
    /// The expedite expense item. Contains the properties only used in expedite validate.
    /// </summary>
    public class ExpediteExpenseItem : BaseExternalType, IBaseClassToAPIType<cExpenseItem, ExpediteExpenseItem>
    {

        /// <summary>
        /// The unique Id of this item in the database.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The friendly code for this item.
        /// </summary>
        public string ReferenceNumber { get; set; }

        /// <summary>
        /// The Id of the <see cref="ExpenseSubCategory">ExpenseSubCategory</see> (or template) that this item is an instance of.
        /// </summary>
        public int ExpenseSubCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the expense sub category name.
        /// </summary>
        public string ExpenseSubCategoryName { get; set; }

        /// <summary>
        /// The Id of the claim to which this item belongs.
        /// </summary>
        public int ClaimId { get; set; }

        /// <summary>
        /// The date of this item.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The notes for disputing this item.
        /// </summary>
        public string DisputeNotes { get; set; }
           
        /// <summary>
        /// The total value of this item.
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// The total after conversion.
        /// </summary>
        public decimal ConvertedTotal { get; set; }

        /// <summary>
        /// The Grand Total.
        /// </summary>
        public decimal GrandTotal { get; set; }

        /// <summary>
        /// The Net Grand Total.
        /// </summary>
        public decimal GrandTotalNet { get; set; }

        /// <summary>
        /// The Grand Total VAT.
        /// </summary>
        public decimal GrandTotalVAT { get; set; }

        /// <summary>
        /// The Id of the transaction.
        /// </summary>
        public int TransactionId { get; set; }

        /// <summary>
        /// Whether this item has a receipt attached.
        /// </summary>
        public bool ReceiptAttached { get; set; }


        /// <summary>
        /// Gets or sets the Operator Validation Progress for the expense item
        /// </summary>
        public ExpediteOperatorValidationProgress OperatorValidationProgress { get; set; }

        /// <summary>
        /// Gets or sets the validation progress.
        /// </summary>
        public ExpenseValidationProgress ValidationProgress { get; set; }

        /// <summary>
        /// Gets or sets the employee id.
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the forename.
        /// </summary>
        public string Forename { get; set; }

        /// <summary>
        /// Gets or sets the surname.
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Gets or sets the claim name.
        /// </summary>
        public string ClaimName { get; set; }

        /// <summary>
        /// Gets or sets the account id.
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Converts a <see cref="cExpenseItem"/> type to a <see cref="ExpediteExpenseItem"/>
        /// </summary>
        /// <param name="dbType">
        /// The instance of <see cref="cExpenseItem"/>.
        /// </param>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        /// <returns>
        /// An instance of <see cref="ExpediteExpenseItem"/>.
        /// </returns>
        public ExpediteExpenseItem ToApiType(cExpenseItem dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            this.Id = dbType.expenseid;
            this.ReferenceNumber = dbType.refnum;
            this.ExpenseSubCategoryId = dbType.subcatid;
            this.ExpenseSubCategoryName = new cSubcats(actionContext.AccountId).GetSubcatById(dbType.subcatid).subcat;
            this.ClaimId = dbType.claimid;
            this.Date = dbType.date;
            this.DisputeNotes = dbType.Dispute;
            this.Total = dbType.total;
            this.ConvertedTotal = dbType.convertedtotal;
            this.GrandTotal = dbType.grandtotal;
            this.GrandTotalNet = dbType.grandnettotal;
            this.GrandTotalVAT = dbType.grandvattotal;
            this.TransactionId = dbType.transactionid;
            this.ReceiptAttached = dbType.receiptattached;
            this.OperatorValidationProgress = (ExpediteOperatorValidationProgress)dbType.OperatorValidationProgress;
            this.ValidationProgress = (ExpenseValidationProgress)dbType.ValidationProgress;
            this.AccountId = actionContext.AccountId;

            var claim = new cClaims(actionContext.AccountId).getClaimById(dbType.claimid);
            var employee = new cEmployees(actionContext.AccountId).GetEmployeeById(claim.employeeid);

            this.EmployeeId = claim.employeeid;
            this.Surname = employee.Surname;
            this.Forename = employee.Forename;
            this.ClaimName = claim.name;

            return this;

        }
    }
}
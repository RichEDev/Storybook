namespace SpendManagementApi.Models.Types
{
    using System;
    using System.Collections.Generic;

    using SpendManagementApi.Interfaces;

    using SpendManagementLibrary;

    /// <summary>
    /// A class that defines all the fields that makes up an advance.
    /// </summary>
    public class MyAdvance : IBaseClassToAPIType<cFloat, MyAdvance >
    {
        /// <summary>
        /// Gets or sets the account id.
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Gets or sets the advance id.
        /// </summary>
        public int AdvanceId { get; set; }

        /// <summary>
        /// Gets or sets the employee id.
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the currency id.
        /// </summary>
        public int CurrencyId { get; set; }

        /// <summary>
        /// Gets or sets the currency name.
        /// </summary>
        public string CurrencyName { get; set; }

        /// <summary>
        /// Gets or sets the currency symbol.
        /// </summary>
        public string CurrencySymbol { get; set; }

        /// <summary>
        /// Gets or sets the advance name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the reason for the advance.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets the date time the advance is required by.
        /// </summary>
        public DateTime RequiredBy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the advance has been approved.
        /// </summary>
        public bool Approved { get; set; }

        /// <summary>
        /// Gets or sets the employee id of the approver.
        /// </summary>
        public int Approver { get; set; }

        /// <summary>
        /// Gets or sets the approver name.
        /// </summary>
        public string ApproverName { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the foreign amount.
        /// </summary>
        public decimal ForeignAmount { get; set; }

        /// <summary>
        /// Gets or sets the exchange rate.
        /// </summary>
        public double ExchangeRate { get; set; }

        /// <summary>
        /// Gets or sets the stage.
        /// </summary>
        public byte Stage { get; set; }

        /// <summary>
        /// Gets the stage description.
        /// </summary>
        public string StageDescription { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the advance was rejected.
        /// </summary>
        public bool Rejected { get; set; }

        /// <summary>
        /// Gets or sets the reject reason.
        /// </summary>
        public string RejectReason { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the advance is disputed.
        /// </summary>
        public bool Disputed { get; set; }

        /// <summary>
        /// Gets or sets the reason for dispute.
        /// </summary>
        public string Dispute { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the advance is paid.
        /// </summary>
        public bool Paid { get; set; }

        /// <summary>
        /// Gets or sets the date paid.
        /// </summary>
        public DateTime DatePaid { get; set; }

        /// <summary>
        /// Gets or sets the issue number.
        /// </summary>
        public int IssueNumber{ get; set; }

        /// <summary>
        /// Gets or sets the base currency.
        /// </summary>
        public int BaseCurrency { get; set; }

        /// <summary>
        /// Gets or sets the is the advance is used.
        /// </summary>
        public decimal AdvanceUsed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the advance is settled.
        /// </summary>
        public bool Settled { get; set; }

        /// <summary>
        /// Gets or sets the allocations.
        /// </summary>
        public SortedList<int, decimal> Allocations { get; set; }

        /// <summary>
        /// Gets or sets the created on.
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the modified on.
        /// </summary>
        public DateTime ModifiedOn { get; set; }

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        public int ModifiedBy { get; set; }

        /// <summary>
        /// The to api type to convert to.
        /// </summary>
        /// <param name="dbType">
        /// The db type to convert from.
        /// </param>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        /// <returns>
        /// The <see cref="MyAdvance"/>.
        /// </returns>
        public MyAdvance ToApiType(cFloat dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            this.AccountId = dbType.accountid;
            this.AdvanceId = dbType.floatid;
            this.EmployeeId = dbType.employeeid;
            this.CurrencyId = dbType.currencyid;
            this.Name = dbType.name;
            this.Reason = dbType.reason;
            this.RequiredBy = dbType.requiredby;
            this.Approved = dbType.approved;
            this.Approver = dbType.approver;
            this.Amount = dbType.floatamount;
            this.ForeignAmount = dbType.foreignAmount;
            this.ExchangeRate = dbType.exchangerate;
            this.Stage = dbType.stage;
            this.Rejected = dbType.rejected;
            this.RejectReason = dbType.rejectreason;
            this.Disputed = dbType.disputed;
            this.Dispute = dbType.dispute;
            this.Paid = dbType.paid;
            this.DatePaid = dbType.datepaid;
            this.IssueNumber = dbType.issuenum;
            this.BaseCurrency = dbType.basecurrency;
            this.AdvanceUsed = dbType.floatused;
            this.Settled = dbType.settled;
            this.Allocations = dbType.allocations;
            this.CreatedOn = dbType.createdon;
            this.CreatedBy = dbType.createdby;
            this.ModifiedOn = dbType.modifiedon;
            this.ModifiedBy = dbType.modifiedby;

            return this;
        }
    }
}
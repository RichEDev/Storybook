namespace SpendManagementLibrary.Expedite.DTO
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Enumerators.Expedite;

    /// <summary>
    /// Represents a claim in the ReceiptManagement tree.
    /// </summary>
    [KnownType(typeof(ReceiptManagementClaim))]
    public class ReceiptManagementClaim
    {
        public ReceiptManagementClaim()
        {
            Children = new List<ReceiptManagementExpense>();
            DeletedReceipts = new List<ReceiptManagementDeletedReceiptInfo>();
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="ReceiptManagementClaim"/> class.
        /// </summary>
        /// <param name="claim">
        /// The current claim object.
        /// </param>
        /// <param name="symbol">
        /// Currency symbol for the currency used in claim.
        /// </param>
        public ReceiptManagementClaim(cClaim claim, string symbol)
        {

            Id = claim.claimid;
            Name = claim.name;
            NumberOfItems = string.Format("{0} item{1}", claim.NumberOfItems, claim.NumberOfItems == 1 ? string.Empty : "s");
            Date = claim.ClaimStage != ClaimStage.Current ? claim.datesubmitted.ToShortDateString() : "[Not submitted]";
            Total = Math.Round(claim.Total, 2, MidpointRounding.AwayFromZero).ToString(symbol + "###,###,##0.00");
            Children = new List<ReceiptManagementExpense>();
            DeletedReceipts = new List<ReceiptManagementDeletedReceiptInfo>();
        }

        /// <summary>
        /// All nodes need an id, if one isn't passed in an id will be created automatically
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The text that is displayed when the node is rendered
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Children is an array of nodes which represent the node's sub nodes
        /// </summary>
        public List<ReceiptManagementExpense> Children { get; set; }

        /// <summary>
        /// The header, which contains receipts.
        /// </summary>
        public ReceiptManagementHeader Header { get; set; }

        /// <summary>
        /// The number of expenses in this claim.
        /// </summary>
        public string NumberOfItems { get; set; }

        /// <summary>
        /// The date of the the claim.
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// The claim total.
        /// </summary>
        public string Total { get; set; }

        /// <summary>
        /// A list of deleted receipts and their reasons why.
        /// </summary>
        public List<ReceiptManagementDeletedReceiptInfo> DeletedReceipts { get; set; }
    }

    /// <summary>
    /// Represents the claim header in the ReceiptManagement tree.
    /// </summary>
    [KnownType(typeof(ReceiptManagementHeader))]
    public class ReceiptManagementHeader
    {
        public ReceiptManagementHeader()
        {
            Children = new List<ReceiptManagementReceipt>();
        }

        /// <summary>
        /// All nodes need an id, if one isn't passed in an id will be created automatically
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The text that is displayed when the node is rendered
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Children is an array of nodes which represent the node's sub nodes
        /// </summary>
        public List<ReceiptManagementReceipt> Children { get; set; }

    }

    /// <summary>
    /// Represents an expense in the ReceiptManagement tree.
    /// </summary>
    [KnownType(typeof(ReceiptManagementExpense))]
    public class ReceiptManagementExpense
    {
        public ReceiptManagementExpense()
        {
            Children = new List<ReceiptManagementReceipt>();
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="ReceiptManagementExpense"/> class.
        /// </summary>
        /// <param name="expense">
        /// The selected expense item.
        /// </param>
        /// <param name="subcat">
        /// Subcat Id of the expense item.
        /// </param>
        /// <param name="preventDelete">
        /// Whether expense item can be deleted.
        /// </param>
        /// <param name="editMessage">
        /// The edited message.
        /// </param>
        /// <param name="symbol">
        /// Currency symbol to display on reprots.
        /// </param>
        public ReceiptManagementExpense(cExpenseItem expense, cSubcat subcat, bool preventDelete, string editMessage, string symbol)
        {
            Id = expense.expenseid;
            Name = subcat.subcat;
            Reference = expense.refnum;
            Total = Math.Round(expense.total, 2, MidpointRounding.AwayFromZero).ToString(symbol + "###,###,##0.00");
            Date = expense.date.ToShortDateString();
            Children = new List<ReceiptManagementReceipt>();
            PreventDelete = preventDelete;
            EditMessage = editMessage;
            NeedsReceipts = expense.ValidationProgress == ExpenseValidationProgress.NoReceipts || expense.ValidationProgress == ExpenseValidationProgress.Required;
        }

        /// <summary>
        /// All nodes need an id, if one isn't passed in an id will be created automatically
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The text that is displayed when the node is rendered
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Children is an array of nodes which represent the node's sub nodes
        /// </summary>
        public List<ReceiptManagementReceipt> Children { get; set; }

        /// <summary>
        /// The reference number for this expense.
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// The date of this expense.
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// The expense total.
        /// </summary>
        public string Total { get; set; }

        /// <summary>
        /// Whether the item cannot be deleted.
        /// </summary>
        public bool PreventDelete { get; set; }

        /// <summary>
        /// Whether the item needs receipts.
        /// </summary>
        public bool NeedsReceipts { get; set; }

        /// <summary>
        /// The reason why the item cannot be deleted.
        /// </summary>
        public string EditMessage { get; set; }
    }

    /// <summary>
    /// Represents a receipt in the ReceiptManagement tree.
    /// </summary>
    [KnownType(typeof(ReceiptManagementReceipt))]
    public class ReceiptManagementReceipt
    {
        public ReceiptManagementReceipt()
        {
        }

        public ReceiptManagementReceipt(Receipt receipt, bool preventDelete, string editMessage, bool isImage)
        {
            Id = receipt.ReceiptId;
            Name = receipt.ReceiptId + "." + receipt.Extension;
            Url = receipt.TemporaryUrl;
            PreventDelete = preventDelete;
            EditMessage = editMessage;
            IsImage = isImage;
            Icon = receipt.IconUrl;
        }

        /// <summary>
        /// All nodes need an id, if one isn't passed in an id will be created automatically
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The text that is displayed when the node is rendered
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Url of this receipt
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Whether the item cannot be deleted
        /// </summary>
        public bool PreventDelete { get; set; }

        /// <summary>
        /// Whether the item is an image type
        /// </summary>
        public bool IsImage { get; set; }

        /// <summary>
        /// The Icon Url, if this receipt is not an image
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// The reason why the item cannot be deleted.
        /// </summary>
        public string EditMessage { get; set; }

        /// <summary>
        /// Used upon save - null if linkage was claim header, 
        /// value if linkage was to an expense.
        /// </summary>
        public int? ParentId { get; set; }
    }

    /// <summary>
    /// Represents the data needed for when a receipt is deleted.
    /// </summary>
    [KnownType(typeof(ReceiptManagementDeletedReceiptInfo))]
    public class ReceiptManagementDeletedReceiptInfo
    {
        /// <summary>
        /// The Id of the Receipt.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Id of the Expense the receipt was unlinked from. 
        /// If null, then the receipt was unlinked from the claim header.
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// The reason why it was deleted or unlinked.
        /// </summary>
        public string Reason { get; set; }
    }
}

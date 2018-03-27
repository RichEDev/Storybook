using Syncfusion.DocIO.DLS.XML;

namespace SpendManagementLibrary.Interfaces.Expedite
{
    using System.Collections.Generic;
    using SpendManagementLibrary.Expedite;

    /// <summary>
    /// The contract for concrete implmentations of ReceiptManagement.
    /// </summary>
    public interface IManageReceipts
    {
        /// <summary>
        /// Gets or sets the account Id. This affects directly which database is used.
        /// The constructor of any implementations should also take this as a property.
        /// </summary>
        int AccountId { get; set; }

        /// <summary>
        /// Gets or sets the employee Id. This is for logging in the database.
        /// The constructor of any implementations should also take this as a property.
        /// </summary>
        int EmployeeId { get; set; }

        /// <summary>
        /// Gets a single receipt by its ReceiptId.
        /// </summary>
        /// <param name="id">The database Id of the receipt.</param>
        /// <param name="fetchFromCloud">Whether to attempt to fetch from the cloud.</param>
        /// <returns>A list of Receipts.</returns>
        Receipt GetById(int id, bool fetchFromCloud = true);

        /// <summary>
        /// Gets the data from the cloud, in memory, and returns that data in a base-64 string.
        /// </summary>
        /// <param name="id">The database Id of the receipt.</param>
        /// <param name="isOrphan">Whether to look in the metabase for the receipt.</param>
        /// <returns>A base-64 string of the data.</returns>
        string GetData(int id, bool isOrphan = false);

        /// <summary>
        /// Gets all receipts for an account that are not assigned to a user, claim, or claim line (savedexpense).
        /// Only an account administrator should have visibility of these.
        /// </summary>
        /// <returns>A list of Receipts.</returns>
        IList<Receipt> GetUnassigned(bool fetchFromCloud = true);

        /// <summary>
        /// Gets all receipts for an envelope.
        /// </summary>
        /// <param name="envelopeId">The EnvelopeId of the envelope.</param>
        /// <param name="fetchFromCloud">Whether to attempt to fetch from the cloud.</param>
        /// <returns>A list of Receipts.</returns>
        IList<Receipt> GetByEnvelope(int envelopeId, bool fetchFromCloud = true);

        /// <summary>
        /// Gets all receipts for a claim line (savedexpense).
        /// </summary>
        /// <param name="expenseItem">The <see cref="cExpenseItem"/>.</param>
        /// <param name="currentUser">The <see cref="ICurrentUserBase"/>.</param>
        /// <param name="subCategory">The <see cref="cSubcat"/>.</param>
        /// <param name="claim">The <see cref="cClaim"/>.</param>
        /// <param name="fetchFromCloud">Whether to attempt to fetch from the cloud.</param>
        IList<Receipt> GetByClaimLine(cExpenseItem expenseItem, ICurrentUserBase currentUser, cSubcat subCategory, cClaim claim, bool fetchFromCloud = true);

        /// <summary>
        /// Gets all receipts for a claim.
        /// </summary>
        /// <param name="claimId">The ClaimId of the row in the claims table.</param>
        /// <param name="fetchFromCloud">Whether to attempt to fetch from the cloud.</param>
        /// <returns>A list of Receipts.</returns>
        IList<Receipt> GetByClaim(int claimId, bool fetchFromCloud = true);

        /// <summary>
        /// Gets all receipts for a claim line (savedexpense).
        /// </summary>
        /// <param name="employeeId">The EmployeeId of the row in the Employees table.</param>
        /// <param name="fetchFromCloud">Whether to attempt to fetch from the cloud.</param>
        /// <returns>A list of Receipts.</returns>
        IList<Receipt> GetByClaimant(int employeeId, bool fetchFromCloud = true);

        /// <summary>
        /// Gets all orphaned receipts from the metabase. 
        /// These are receipts where we cannot identify the account, which means we 
        /// don't know the claim, claimant, claim line, or even account.
        /// </summary>
        /// <param name="fetchFromCloud">Whether to attempt to fetch from the cloud.</param>
        /// <returns>A list of OrphanedReceipts.</returns>
        IList<OrphanedReceipt> GetOrphaned(bool fetchFromCloud = true);

        /// <summary>
        /// Checks whether all the validatable claim lines (savedexpense) for a given claim id have receipts.
        /// </summary>
        /// <param name="claimId">The Id of the claim.</param>
        /// <returns>A bool, indicating the result.</returns>
        bool CheckIfAllValidatableClaimLinesHaveReceiptsAttached(int claimId);

        /// <summary>
        /// Adds a receipt to the specified database.
        /// Linkages will be ignored. Please use the link methods.
        /// </summary>
        /// <param name="receipt">The receipt to add.</param>
        /// <param name="data">The actual binary data for the receipt.</param>
        /// <returns>The added receipt.</returns>
        Receipt AddReceipt(Receipt receipt, byte[] data);

        /// <summary>
        /// Adds an <see cref="OrphanedReceipt"/> to the metabase.
        /// This should only be called by Expedite staff when the receipt has no identifying info.
        /// </summary>
        /// <param name="receipt">The orphaned receipt.</param>
        /// <param name="data">The actual binary data for the file.</param>
        /// <returns>The added orphan.</returns>
        OrphanedReceipt AddOrphan(OrphanedReceipt receipt, byte[] data);

        /// <summary>
        /// Links a receipt to a claimant (employee).
        /// Calling this will remove any ClaimLine-level links, and any claim link.
        /// </summary>
        /// <param name="receiptId">The ReceiptId of the receipt.</param>
        /// <param name="employeeId">The EmployeeId of the row in the Employees table.</param>
        /// <returns></returns>
        Receipt LinkToClaimant(int receiptId, int employeeId);

        /// <summary>
        /// Links a receipt to a claim header. 
        /// A receipt can only be placed against one claim.
        /// Calling this will remove any ClaimLine-level links, and any claimant link.
        /// </summary>
        /// <param name="receiptId">The ReceiptId of the receipt.</param>
        /// <param name="claimId">The ClaimId of the row in the claims table.</param>
        /// <returns>The linked receipt.</returns>
        Receipt LinkToClaim(int receiptId, int claimId);
        
        /// <summary>
        /// Links a receipt to a claim line. Call this multiple times if needed.
        /// Receipts can now be stored against multiple lines. A line can also have multiple receipts.
        /// Calling this will remove any Claim link, and any claimant link, but not other claimline links.
        /// To remove a ClaimLine-level link, use <see cref="UnlinkFromClaimLine"/>.
        /// </summary>
        /// <param name="receiptId">The ReceiptId of the receipt.</param>
        /// <param name="savedExpenseId">The ExpenseId of the row in the savedexpenses table.</param>
        /// <returns>The linked receipt.</returns>
        Receipt LinkToClaimLine(int receiptId, int savedExpenseId);

        /// <summary>
        /// Removes the links between a receipt and a claim line. Call this multiple times if needed.
        /// Receipts can now be stored against multiple lines. A line can also have multiple receipts.
        /// </summary>
        /// <param name="receiptId">The ReceiptId of the receipt.</param>
        /// <param name="savedExpenseId">The ExpenseId of the row in the savedexpenses table.</param>
        /// <returns>The modified receipt.</returns>
        Receipt UnlinkFromClaimLine(int receiptId, int savedExpenseId);

        /// <summary>
        /// Updates a batch of receipt linkages in one go.
        /// </summary>
        /// <param name="toRemove">A dictionary of ReceiptIds, with their accompanying linkages TO BE REMOVED.</param>
        /// <param name="toAssign">A dictionary of ReceiptIds, with their accompanying linkages TO BE ADDED.</param>
        /// <returns>The modified receipt.</returns>
        void BatchUpdateReceiptLinkages(Dictionary<int, ReceiptOwnershipInfo> toRemove, Dictionary<int, ReceiptOwnershipInfo> toAssign);

        /// <summary>
        /// Takes an orphaned receipt from the metabase, and adds it to an account.
        /// This is achieved by getting the file from the cloud, generating a new receipt in the specified account,
        /// taking the newly generated Id and renaming and re-uploading the renamed file so there is a copy in the account
        /// database and folder structure in the cloud. Finally, <see cref="DeleteOrphan"/> is called on the old Id.
        /// This will remove the receipt from the metabase and the file itself from the cloud.
        /// </summary>
        /// <param name="id">The Id of the OrphanedReceipt.</param>
        /// <returns></returns>
        Receipt MoveOrphanToAccount(int id);

        /// <summary>
        /// Deletes a <see cref="Receipt"/>. Note that we don't actually delete normal Receipts. 
        /// A deleted flag is set against the receipt and it will not show up in results.
        /// </summary>
        /// <param name="id">The Id of the Receipt to delete.</param>
        /// <param name="actualDelete">Whether to actually delete the receipt. 
        /// Do not use unless you are certain it is legal to delete the receipt.</param>
        void DeleteReceipt(int id, bool actualDelete = false);

        /// <summary>
        /// Deletes an <see cref="OrphanedReceipt"/>. Note that this IS permanent and removes both
        /// the metabase entry and the file in the cloud. Call this only if you are SURE the receipt
        /// is allowed to be deleted.
        /// </summary>
        /// <param name="id">The Id of the OrphanedReceipt to delete.</param>
        void DeleteOrphan(int id);
    }
}

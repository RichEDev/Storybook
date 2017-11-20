namespace UnitTest2012Ultimate.API.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using SpendManagementLibrary.Expedite;
    using SpendManagementLibrary.Interfaces.Expedite;
    using SpendManagementLibrary.Enumerators.Expedite;

    public class TestReceipts : IManageReceipts
    {
        public static IList<Receipt> Receipts;
        public static IList<OrphanedReceipt> Orphans;

        public static readonly InvalidDataException ReceiptErrorIdIsNotZero = new InvalidDataException("You must set the Id of the Receipt to 0 to add it.");
        public static readonly InvalidDataException ReceiptErrorNoData = new InvalidDataException("There is no data for the receipt.");
        public static readonly InvalidDataException ReceiptErrorBadSave = new InvalidDataException("Item not saved.");
        public static readonly InvalidDataException ReceiptErrorNoReceipt = new InvalidDataException("A receipt does not exist with this Id.");

        #region Constructor

        public TestReceipts(int accountId, int employeeId)
        {
            AccountId = accountId;
            EmployeeId = employeeId;
            ResetData();
        }

        #endregion Constructor

        #region Public Properties

        public int AccountId { get; set; }

        /// <summary>
        /// Gets or sets the employee Id. This is for logging in the database.
        /// The constructor of any implementations should also take this as a property.
        /// </summary>
        public int EmployeeId { get; set; }

        #endregion Public Properties

        /// <summary>
        /// Resets the data in this Test Class.
        /// </summary>
        public void ResetData()
        {
            Receipts = new List<Receipt>
            {
                new Receipt
                {
                    ReceiptId = 1,
                    CreationMethod = ReceiptCreationMethod.UploadedByExpedite,
                    Extension = ".jpg",
                    ModifiedBy = GlobalTestVariables.AccountId,
                    ModifiedOn = DateTime.Now,
                    TemporaryUrl = GlobalTestVariables.ImagesPath + "testreceipt1.jpg",
                    ExpediteInfo = new ExpediteInfo
                    {
                        EnvelopeId = 1,
                        ExpediteUserName = "Henson"
                    },
                    OwnershipInfo = new ReceiptOwnershipInfo {EmployeeId = GlobalTestVariables.EmployeeId}
                },
                new Receipt
                {
                    ReceiptId = 2,
                    CreationMethod = ReceiptCreationMethod.UploadedByClaimant,
                    Extension = ".jpg",
                    ModifiedBy = GlobalTestVariables.AccountId,
                    ModifiedOn = DateTime.Now,
                    TemporaryUrl = GlobalTestVariables.ImagesPath + "testreceipt2.jpg",
                    ExpediteInfo = null,
                    OwnershipInfo = new ReceiptOwnershipInfo {ClaimId = 1}
                },
                new Receipt
                {
                    ReceiptId = 3,
                    CreationMethod = ReceiptCreationMethod.UploadedByMobile,
                    Extension = ".jpg",
                    ModifiedBy = GlobalTestVariables.AccountId,
                    ModifiedOn = DateTime.Now,
                    TemporaryUrl = GlobalTestVariables.ImagesPath + "testreceipt3.jpg",
                    ExpediteInfo = null,
                    OwnershipInfo = new ReceiptOwnershipInfo {ClaimLines = new List<int> {1}}
                },
                new Receipt
                {
                    ReceiptId = 4,
                    CreationMethod = ReceiptCreationMethod.Unknown,
                    Extension = ".jpg",
                    ModifiedBy = GlobalTestVariables.AccountId,
                    ModifiedOn = DateTime.Now,
                    TemporaryUrl = GlobalTestVariables.ImagesPath + "testreceipt4.jpg",
                    ExpediteInfo = null,
                    OwnershipInfo = new ReceiptOwnershipInfo {ClaimLines = new List<int> {1, 2, 3}}
                }
            };

            Orphans = new List<OrphanedReceipt>
            {
                new OrphanedReceipt
                {
                    ReceiptId = 1,
                    CreationMethod = ReceiptCreationMethod.UploadedByExpedite,
                    Extension = ".jpg",
                    TemporaryUrl = GlobalTestVariables.ImagesPath+"testorphanreceipt1.jpg"
                },
                new OrphanedReceipt
                {
                    ReceiptId = 2,
                    CreationMethod = ReceiptCreationMethod.UploadedByExpedite,
                    Extension = ".jpg",
                    TemporaryUrl = GlobalTestVariables.ImagesPath+"testorphanreceipt2.jpg"
                },

            };
        }

        /// <summary>
        /// Gets a single receipt by its ReceiptId.
        /// </summary>
        /// <param name="id">The database Id of the receipt.</param>
        /// <param name="fetchFromCloud">Whether to attempt to fetch from the cloud.</param>
        /// <returns>A list of Receipts.</returns>
        public Receipt GetById(int id, bool fetchFromCloud = true)
        {
            return Receipts.FirstOrDefault(r => r.ReceiptId == id);
        }

        /// <summary>
        /// Gets the data from the cloud, in memory, and returns that data in a base-64 string.
        /// </summary>
        /// <param name="id">The database Id of the receipt.</param>
        /// <param name="isOrphan">Whether to look in the metabase for the receipt.</param>
        /// <returns>A base-64 string of the data.</returns>
        public string GetData(int id, bool isOrphan = false)
        {
            const string image = "/9j/4AAQSkZJRgABAQEASABIAAD/2wBDAAUDBAQEAwUEBAQFBQUGBwwIBwcHBw8LCwkMEQ8SEhEP ERETFhwXExQaFRERGCEYGh0dHx8fExciJCIeJBweHx7/2wBDAQUFBQcGBw4ICA4eFBEUHh4eHh4e " +
                   "Hh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh7/wAARCAAkACgDASIA AhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQA " +
                   "AAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3 ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWm " +
                   "p6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEA AwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSEx " +
                   "BhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElK U1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3 " +
                   "uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwD7LrI8 ReKPDXhxA+v+INK0oEZX7ZdpEW+gYjP4V8//ALRfxpvbG/1TQdB1ObSNL0qRbbUdRtgDd3V0y7vs " +
                   "ttnhNo5eQ/d7c4DeO/s+eKfBeqfEK9Pi6z0ezjkty1rLqZFw0024ZaS4nyd+3pjYp54ziuDGY76v TnOEHLl3t/X9dS4xu0mz64/4XZ8J/N8v/hPNF3evncfnjFdR4d8V+GPEalvD/iLStVwMkWl2kpX6 " +
                   "hSSPxr5Nn+L3htPjinhxJNDPg3zwHvRbqU3/AGcpt3/d8rzCDnHUZztrlv2i/E/gnTPHGnHwjZ6J fbIC94+nbYTFJu+Vo54MHzMZzksowOOtcOHzavUrQpToNc0ea9/weisynCKTdz70or5h/Z1+Nd9c " +
                   "3ul6LrmqXGr6Nqk/2KyvrzH2yxu8ZW2uGHEocfcl6sc5HUKV7VOoqiujNqx5b+0d4G1W18W61oLq y3N1rd1rujs5wmox3Qj82JGPBljaNQEPLAnHJUNjeEf2dtd8T+DNK8QWGu2cct3MUubOeBka2USF " +
                   "G5z8zLgkqQvTGa+8PGXhXw94w0WTR/Euk2+pWTnOyVeUb+8rDlW9wQa8wb4NeJ9CJXwJ8TtUsbTO VsdXtUv41/2Vc4ZVH415uOoY1Q/2KaTvfX8Vs93r+ti4uDfvHzRL+zR4lX4gR+H11aFtJe2Nz/a3 " +
                   "2cgAAhSnl7vv5I43YxzntUHiv9nPW/DfhDWdfv8AxBZMbF/9FtooGZrpS4Vec/KzZGFAbkgZr6b/ AOEQ+O3+r/4SrwQV/wCen2Cff+WcU9fg74s13C+OPihqV1aE5ax0a0SxQ/7JkGWYH8K82jTz6U4+ " +
                   "0nFRVr+ffp1/4Yt+ytofN/7PngXVrvxRpfhxEb7b/bFnrGrheV023tS5RJCOFmkaQgJ1XAzjJAK+ 3PBXhHw54M0ZdI8M6Tb6baA7mWMEtI3952OWdvckmivoqVPkTbd29WYtm5RRRWogooooAKKKKAP/ 2Q==";
            return image;
        }

        /// <summary>
        /// Gets all receipts for an account that are not assigned to a user, claim, or claim line (savedexpense).
        /// Only an account administrator should have visibility of these.
        /// </summary>
        /// <returns>A list of Receipts.</returns>
        public IList<Receipt> GetUnassigned(bool fetchFromCloud = true)
        {
            return
                Receipts.Where(x =>
                        x.OwnershipInfo.EmployeeId == null && x.OwnershipInfo.ClaimId == null &&
                        x.OwnershipInfo.ClaimLines == null).ToList();
        }

        /// <summary>
        /// Gets all receipts for an envelope.
        /// </summary>
        /// <param name="envelopeId">The EnvelopeId of the envelope.</param>
        /// <param name="fetchFromCloud">Whether to attempt to fetch from the cloud.</param>
        /// <returns>A list of Receipts.</returns>
        public IList<Receipt> GetByEnvelope(int envelopeId, bool fetchFromCloud = true)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all receipts for a claim line (savedexpense).
        /// </summary>
        /// <param name="savedExpenseId">The ExpenseId of the row in the savedexpenses table.</param>
        /// <param name="fetchFromCloud">Whether to attempt to fetch from the cloud.</param>
        /// <returns>A list of Receipts.</returns>
        public IList<Receipt> GetByClaimLine(int savedExpenseId, bool fetchFromCloud = true)
        {
            return Receipts.Where(x => x.OwnershipInfo.ClaimLines.Contains(savedExpenseId)).ToList();
        }

        /// <summary>
        /// Gets all receipts for a claim.
        /// </summary>
        /// <param name="claimId">The ClaimId of the row in the claims table.</param>
        /// <param name="fetchFromCloud">Whether to attempt to fetch from the cloud.</param>
        /// <returns>A list of Receipts.</returns>
        public IList<Receipt> GetByClaim(int claimId, bool fetchFromCloud = true)
        {
            return Receipts.Where(x => x.OwnershipInfo.ClaimId == claimId).ToList();
        }

        /// <summary>
        /// Gets all receipts for a claim line (savedexpense).
        /// </summary>
        /// <param name="employeeId">The EmployeeId of the row in the Employees table.</param>
        /// <param name="fetchFromCloud">Whether to attempt to fetch from the cloud.</param>
        /// <returns>A list of Receipts.</returns>
        public IList<Receipt> GetByClaimant(int employeeId, bool fetchFromCloud = true)
        {
            return Receipts.Where(x => x.OwnershipInfo.EmployeeId == employeeId).ToList();
        }

        /// <summary>
        /// Gets all orphaned receipts from the metabase. 
        /// These are receipts where we cannot identify the account, which means we 
        /// don't know the claim, claimant, claim line, or even account.
        /// </summary>
        /// <param name="fetchFromCloud">Whether to attempt to fetch from the cloud.</param>
        /// <returns>A list of OrphanedReceipts.</returns>
        public IList<OrphanedReceipt> GetOrphaned(bool fetchFromCloud = true)
        {
            return Orphans.ToList();
        }

        /// <summary>
        /// Checks whether all the claim lines (savedexpense) for a given claim id have receipts.
        /// </summary>
        /// <param name="claimId">The Id of the claim.</param>
        /// <returns>A bool, indicating the result.</returns>
        public bool CheckIfAllValidatableClaimLinesHaveReceiptsAttached(int claimId)
        {
            return false;
        }

        /// <summary>
        /// Adds a receipt to the specified database.
        /// Linkages will be ignored. Please use the link methods.
        /// </summary>
        /// <param name="receipt">The receipt to add.</param>
        /// <param name="data">The actual binary data for the receipt.</param>
        /// <returns>The added receipt.</returns>
        public Receipt AddReceipt(Receipt receipt, byte[] data)
        {
            if (receipt.ReceiptId != 0)
            {
                throw ReceiptErrorIdIsNotZero;
            }

            if (data == null || data.Length == 0)
            {
                throw ReceiptErrorNoData;
            }

            var lastReceipt = Receipts.OrderBy(e => e.ReceiptId).LastOrDefault();
            receipt.ReceiptId = lastReceipt == null ? 1 : lastReceipt.ReceiptId + 1;
            receipt.OwnershipInfo = new ReceiptOwnershipInfo();
            receipt.TemporaryUrl = GlobalTestVariables.ImagesPath + "receipt" + receipt.ReceiptId + receipt.Extension;
            Receipts.Add(receipt);
            return receipt;
        }

        /// <summary>
        /// Adds an <see cref="OrphanedReceipt"/> to the metabase.
        /// This should only be called by Expedite staff when the receipt has no identifying info.
        /// </summary>
        /// <param name="receipt">The orphaned receipt.</param>
        /// <param name="data">The actual binary data for the file.</param>
        /// <returns>The added orphan.</returns>
        public OrphanedReceipt AddOrphan(OrphanedReceipt receipt, byte[] data)
        {
            if (receipt.ReceiptId != 0)
            {
                throw ReceiptErrorIdIsNotZero;
            }

            if (data == null || data.Length == 0)
            {
                throw ReceiptErrorNoData;
            }

            var lastReceipt = Receipts.OrderBy(e => e.ReceiptId).LastOrDefault();
            receipt.ReceiptId = lastReceipt == null ? 1 : lastReceipt.ReceiptId + 1;
            receipt.TemporaryUrl = GlobalTestVariables.ImagesPath + "receipt" + receipt.ReceiptId + receipt.Extension;
            Orphans.Add(receipt);
            return receipt;
        }

        /// <summary>
        /// Links a receipt to a claimant (employee).
        /// Calling this will remove any ClaimLine-level links, and any claim link.
        /// </summary>
        /// <param name="receiptId">The ReceiptId of the receipt.</param>
        /// <param name="employeeId">The EmployeeId of the row in the Employees table.</param>
        /// <returns></returns>
        public Receipt LinkToClaimant(int receiptId, int employeeId)
        {
            if (!Receipts.Select(x => x.ReceiptId).Contains(receiptId) || employeeId != GlobalTestVariables.EmployeeId)
            {
                throw ReceiptErrorBadSave;
            }

            var receipt = GetById(receiptId);
            receipt.OwnershipInfo = new ReceiptOwnershipInfo { EmployeeId = employeeId };
            receipt.ModifiedOn = DateTime.UtcNow;
            return receipt;
        }

        /// <summary>
        /// Links a receipt to a claim header. 
        /// A receipt can only be placed against one claim.
        /// Calling this will remove any ClaimLine-level links, and any claimant link.
        /// </summary>
        /// <param name="receiptId">The ReceiptId of the receipt.</param>
        /// <param name="claimId">The ClaimId of the row in the claims table.</param>
        /// <returns>The linked receipt.</returns>
        public Receipt LinkToClaim(int receiptId, int claimId)
        {
            if (!Receipts.Select(x => x.ReceiptId).Contains(receiptId) || claimId != 1)
            {
                throw ReceiptErrorBadSave;
            }

            var receipt = GetById(receiptId);
            receipt.OwnershipInfo = new ReceiptOwnershipInfo { ClaimId = claimId };
            receipt.ModifiedOn = DateTime.UtcNow;
            return receipt;
        }

        /// <summary>
        /// Links a receipt to a claim line. Call this multiple times if needed.
        /// Receipts can now be stored against multiple lines. A line can also have multiple receipts.
        /// Calling this will remove any Claim link, and any claimant link, but not other claimline links.
        /// To remove a ClaimLine-level link, use <see cref="IManageReceipts.UnlinkFromClaimLine"/>.
        /// </summary>
        /// <param name="receiptId">The ReceiptId of the receipt.</param>
        /// <param name="savedExpenseId">The ExpenseId of the row in the savedexpenses table.</param>
        /// <returns>The linked receipt.</returns>
        public Receipt LinkToClaimLine(int receiptId, int savedExpenseId)
        {
            if (!Receipts.Select(x => x.ReceiptId).Contains(receiptId) || savedExpenseId < 1 || savedExpenseId > 5)
            {
                throw ReceiptErrorBadSave;
            }

            var receipt = GetById(receiptId);
            if (receipt.OwnershipInfo == null)
            {
                receipt.OwnershipInfo = new ReceiptOwnershipInfo { ClaimId = savedExpenseId };
            }
            else
            {
                receipt.OwnershipInfo.ClaimId = receipt.OwnershipInfo.EmployeeId = null;
                if (receipt.OwnershipInfo.ClaimLines == null)
                {
                    receipt.OwnershipInfo.ClaimLines = new List<int> {savedExpenseId};
                }
                else if (!receipt.OwnershipInfo.ClaimLines.Contains(savedExpenseId))
                {
                    receipt.OwnershipInfo.ClaimLines.Add(savedExpenseId);
                }
            }

            receipt.ModifiedOn = DateTime.UtcNow;
            return receipt;
        }

        /// <summary>
        /// Removes the links between a receipt and a claim line. Call this multiple times if needed.
        /// Receipts can now be stored against multiple lines. A line can also have multiple receipts.
        /// </summary>
        /// <param name="receiptId">The ReceiptId of the receipt.</param>
        /// <param name="savedExpenseId">The ExpenseId of the row in the savedexpenses table.</param>
        /// <returns>The modified receipt.</returns>
        public Receipt UnlinkFromClaimLine(int receiptId, int savedExpenseId)
        {
            if (!Receipts.Select(x => x.ReceiptId).Contains(receiptId) || savedExpenseId < 1 || savedExpenseId > 5)
            {
                throw ReceiptErrorBadSave;
            }

            var receipt = GetById(receiptId);
            if (receipt.OwnershipInfo == null)
            {
                receipt.OwnershipInfo = new ReceiptOwnershipInfo { ClaimId = savedExpenseId };
            }
            else
            {
                receipt.OwnershipInfo.ClaimId = receipt.OwnershipInfo.EmployeeId = null;
                if (receipt.OwnershipInfo.ClaimLines.Contains(savedExpenseId))
                {
                    receipt.OwnershipInfo.ClaimLines.Remove(savedExpenseId);
                }

                if (receipt.OwnershipInfo.ClaimLines.Count == 0)
                {
                    receipt.OwnershipInfo.ClaimLines = null;
                }
            }

            receipt.ModifiedOn = DateTime.UtcNow;
            return receipt;
        }

        /// <summary>
        /// Updates a batch of receipt linkages in one go.
        /// </summary>
        /// <param name="toRemove">A dictionary of ReceiptIds, with their accompanying linkages TO BE REMOVED.</param>
        /// <param name="toAssign">A dictionary of ReceiptIds, with their accompanying linkages TO BE ADDED.</param>
        /// <returns>The modified receipt.</returns>
        public void BatchUpdateReceiptLinkages(Dictionary<int, ReceiptOwnershipInfo> toRemove, Dictionary<int, ReceiptOwnershipInfo> toAssign)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Takes an orphaned receipt from the metabase, and adds it to an account.
        /// This is achieved by getting the file from the cloud, generating a new receipt in the specified account,
        /// taking the newly generated Id and renaming and re-uploading the renamed file so there is a copy in the account
        /// database and folder structure in the cloud. Finally, <see cref="IManageReceipts.DeleteOrphan"/> is called on the old Id.
        /// This will remove the receipt from the metabase and the file itself from the cloud.
        /// </summary>
        /// <param name="id">The Id of the OrphanedReceipt.</param>
        /// <returns></returns>
        public Receipt MoveOrphanToAccount(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes a <see cref="Receipt"/>. Note that we don't actually delete normal Receipts. 
        /// A deleted flag is set against the receipt and it will not show up in results.
        /// </summary>
        /// <param name="id">The Id of the Receipt to delete.</param>
        /// <param name="actualDelete">Whether to actually delete the receipt. 
        /// Do not use unless you are certain it is legal to delete the receipt.</param>
        public void DeleteReceipt(int id, bool actualDelete = false)
        {
            var receipt = GetById(id);
            if (receipt == null)
            {
                throw ReceiptErrorBadSave;
            }
            Receipts.Remove(receipt);
        }

        /// <summary>
        /// Deletes an <see cref="OrphanedReceipt"/>. Note that this IS permanent and removes both
        /// the metabase entry and the file in the cloud. Call this only if you are SURE the receipt
        /// is allowed to be deleted.
        /// </summary>
        /// <param name="id">The Id of the OrphanedReceipt to delete.</param>
        public void DeleteOrphan(int id)
        {
            var receipt = Orphans.FirstOrDefault(o => o.ReceiptId == id);
            if (receipt == null)
            {
                throw ReceiptErrorBadSave;
            }
            Orphans.Remove(receipt);
        }
    }
}
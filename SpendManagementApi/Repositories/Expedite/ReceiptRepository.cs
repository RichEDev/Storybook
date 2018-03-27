namespace SpendManagementApi.Repositories.Expedite
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Interfaces;
    using Models.Common;
    using Models.Types.Expedite;
    using Utilities;
    using Spend_Management;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Interfaces.Expedite;

    /// <summary>
    /// ReceiptRepository manages data access for Receipts.
    /// </summary>
    internal class ReceiptRepository : BaseRepository<Receipt>, ISupportsActionContext
    {
        private readonly IManageReceipts _data;

        /// <summary>
        /// Creates a new ReceiptRepository with the passed in user.
        /// </summary>
        /// <param name="user">The current user.</param>
        /// <param name="actionContext">An implementation of ISupportsActionContext</param>
        public ReceiptRepository(ICurrentUser user, IActionContext actionContext)
            : base(user, actionContext, x => x.Id, x => x.Id.ToString(CultureInfo.InvariantCulture))
        {
            _data = ActionContext.Receipts;
        }

        /// <summary>
        /// Gets all the Receipts within the system.
        /// </summary>
        /// <returns>A list of Receipts.</returns>
        public override IList<Receipt> GetAll()
        {
            throw new NotImplementedException(ApiResources.ApiErrorNotImplementedReceiptGetAll);
        }

        /// <summary>
        /// Gets all the Receipts that are in the metabase and not attached to an account.
        /// </summary>
        /// <returns>A list of Receipts.</returns>
        public IList<Receipt> GetOrphaned()
        {
            return _data.GetOrphaned().Select(e => new Receipt().From(e, ActionContext)).ToList();
        }

        /// <summary>
        /// Gets an Receipt by it's ReceiptId, and populates the Data property.
        /// </summary>
        /// <param name="id">The Id of the Receipt.</param>
        /// <returns>The Receipt with the matching Id.</returns>
        public override Receipt Get(int id)
        {
            var receipt = new Receipt().From(_data.GetById(id), ActionContext);
            receipt.Data = _data.GetData(id);
            return receipt;
        }

        /// <summary>
        /// Gets all the Receipts that are in the metabase and not attached to an account.
        /// </summary>
        /// <returns>A list of Receipts.</returns>
        public Receipt GetOrphan(int id)
        {
            var orphan = new Receipt().From(_data.GetOrphaned().FirstOrDefault(o => o.ReceiptId == id), ActionContext);
            orphan.Data = _data.GetData(id, true);
            return orphan;
        }

        /// <summary>
        /// Gets all receipts for an account that are not assigned to a user, claim, or claim line (savedexpense).
        /// Only an account administrator should have visibility of these.
        /// </summary>
        /// <returns>A list of Receipts.</returns>
        public IList<Receipt> GetUnassigned()
        {
            _data.AccountId = User.AccountID;
            return _data.GetUnassigned().Select(e => new Receipt().From(e, ActionContext)).ToList();
        }

        /// <summary>
        /// Gets all the Receipts for a specific claim line, or 'savedexpense'.
        /// </summary>
        /// <param name="savedExpenseId">The Id of the expense.</param>
        /// <returns>A list of Receipts.</returns>
        public IList<Receipt> GetByClaimLine(int savedExpenseId)
        {
            _data.AccountId = User.AccountID;
            CheckClaimLineAndThrow(savedExpenseId);

            var expenseItem = ActionContext.Claims.getExpenseItemById(savedExpenseId);
            var claim = this.ActionContext.Claims.getClaimById(expenseItem.claimid);
            var subCategory = this.ActionContext.SubCategories.GetSubcatById(expenseItem.subcatid);

            return _data.GetByClaimLine(expenseItem, this.User, subCategory, claim).Select(e => new Receipt().From(e, ActionContext)).ToList();
        }

        /// <summary>
        /// Gets all the Receipts for a specific Claim header.
        /// </summary>
        /// <param name="claimId">The Id of the claim.</param>
        /// <returns>A list of Receipts.</returns>
        public IList<Receipt> GetByClaim(int claimId)
        {
            _data.AccountId = User.AccountID;
            CheckClaimAndThrow(claimId, User.AccountID);
            return _data.GetByClaim(claimId).Select(e => new Receipt().From(e, ActionContext)).ToList();
        }

        /// <summary>
        /// Gets all the Receipts for a specific Claimant, or Employee.
        /// </summary>
        /// <param name="claimantId">The EmployeeId of the claimant.</param>
        /// <returns>A list of Receipts.</returns>
        public IList<Receipt> GetByClaimant(int claimantId)
        {
            _data.AccountId = User.AccountID;
            CheckEmployeeAndThrow(claimantId, _data.AccountId);
            return _data.GetByClaimant(claimantId).Select(e => new Receipt().From(e, ActionContext)).ToList();
        }

        /// <summary>
        /// Creates an Receipt in the system and returns the newly created Receipt.
        /// Only the fileExtension and data properties are required. The rest are generated and repopulated.
        /// </summary>
        /// <param name="item">The item to create.</param>
        /// <returns>The newly created Receipt.</returns>
        public override Receipt Add(Receipt item)
        {
            _data.AccountId = User.AccountID;

            // before we upload the receipt, check the linkages.
            if (item.ClaimLines != null && item.ClaimLines.Any())
            {
                foreach (var claimLine in item.ClaimLines)
                {
                    CheckClaimLineAndThrow(claimLine);
                }
            }
            else if (item.ClaimId.HasValue)
            {
                CheckClaimAndThrow(item.ClaimId.Value, User.AccountID);
            }
            else if (item.EmployeeId.HasValue)
            {
                CheckEmployeeAndThrow(item.EmployeeId.Value, User.AccountID);
            }

            // convert base64string to byte[]
            var byteArray = Convert.FromBase64String(item.Data);

            // now upload the receipt.
            var modified = new Receipt().From(_data.AddReceipt(item.To(ActionContext), byteArray), ActionContext);

            // map the properties
            item.Id = modified.Id;
            item.CreationMethod = modified.CreationMethod;
            item.EnvelopeId = modified.EnvelopeId;

            // attempt attach to the most granular property first - claim lines, then claim, then employee
            if (item.ClaimLines != null && item.ClaimLines.Any())
            {
                var modifiedList = modified.ClaimLines.ToList();

                // work out which expenses need removing / adding 
                var toRemove = modifiedList.Where(i => !item.ClaimLines.Contains(i)).ToList();
                var toAdd = item.ClaimLines.Where(i => !modifiedList.Contains(i)).ToList();

                // remove and add respectively
                toRemove.ForEach(i => UnlinkFromClaimLine(item.Id, i));
                toAdd.ForEach(i => LinkToClaimLine(item.Id, i));

                // set response properties to null.
                item.ClaimId = item.EmployeeId = null;
            }
            else if (item.ClaimId.HasValue)
        {
                // do the linkage
                LinkToClaim(item.Id, item.ClaimId.Value);

                // set response properties to null.
                item.ClaimLines = null;
                item.EmployeeId = null;
        }
            else if (item.EmployeeId.HasValue)
            {
                // do the linkage
                LinkToClaimant(item.Id, item.EmployeeId.Value);

                // set response properties to null.
                item.ClaimLines = null;
                item.ClaimId = null;
            }
            
            return item;
        }

        /// <summary>
        /// Creates a receipt that is not stored against any account. Instead it is stored in the metabase, and in a different folder.
        /// Only the fileExtension and data properties are required. The rest are generated and repopulated.
        /// </summary>
        /// <param name="item">The item to create.</param>
        /// <returns>The newly created Receipt.</returns>
        public Receipt AddOrphan(Receipt item)
        {
            // convert base64string to byte[]
            var byteArray = Convert.FromBase64String(item.Data);

            return new Receipt().From(_data.AddOrphan(item.To(ActionContext), byteArray), ActionContext);
        }

        /// <summary>
        /// Links a receipt to a claimant (employee).
        /// Calling this will remove any ClaimLine-level links, and any claim link.
        /// </summary>
        /// <param name="receiptId">The ReceiptId of the receipt.</param>
        /// <param name="employeeId">The EmployeeId of the row in the Employees table.</param>
        /// <returns>The linked receipt.</returns>
        public Receipt LinkToClaimant(int receiptId, int employeeId)
        {
            _data.AccountId = CheckAccountAndThrow(User.AccountID);
            CheckReceiptAndThrow(receiptId);
            CheckEmployeeAndThrow(employeeId, _data.AccountId);
            return new Receipt().From(_data.LinkToClaimant(receiptId, employeeId), ActionContext);
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
            _data.AccountId = CheckAccountAndThrow(User.AccountID);
            CheckReceiptAndThrow(receiptId);
            CheckClaimAndThrow(claimId, User.AccountID);
            return new Receipt().From(_data.LinkToClaim(receiptId, claimId), ActionContext);
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
            _data.AccountId = CheckAccountAndThrow(User.AccountID);
            CheckReceiptAndThrow(receiptId);
            CheckClaimLineAndThrow(savedExpenseId);
            return new Receipt().From(_data.LinkToClaimLine(receiptId, savedExpenseId), ActionContext);
        }

        /// <summary>
        /// Removes the links between a receipt and a claim line. Call this multiple times if needed.
        /// Receipts can now be stored against multiple lines. A line can also have multiple receipts.
        /// </summary>
        /// <param name="receiptId">The ReceiptId of the receipt.</param>
        /// <param name="savedExpenseId">The ExpenseId of the row in the savedexpenses table.</param>
        /// <returns>The linked receipt.</returns>
        public Receipt UnlinkFromClaimLine(int receiptId, int savedExpenseId)
        {
            _data.AccountId = User.AccountID;
            CheckReceiptAndThrow(receiptId);
            CheckClaimLineAndThrow(savedExpenseId);
            return new Receipt().From(_data.UnlinkFromClaimLine(receiptId, savedExpenseId), ActionContext);
        }


        /// <summary>
        /// Deletes an Receipt, given it's Id.
        /// </summary>
        /// <param name="id">The Id of the Receipt to delete.</param>
        /// <returns>Null if the Receipt was deleted successfully.</returns>
        public override Receipt Delete(int id)
        {
            _data.AccountId = User.AccountID;
            CheckReceiptAndThrow(id);
            _data.DeleteReceipt(id);
            return null;
        }

        /// <summary>
        /// Deletes an Orphaned Receipt, given it's Id.
        /// </summary>
        /// <param name="id">The Id of the Receipt to delete.</param>
        /// <returns>Null if the Receipt was deleted successfully.</returns>
        public Receipt DeleteOrphan(int id)
        {
            var orphan = _data.GetOrphaned(false).FirstOrDefault(o => o.ReceiptId == id);
            if (orphan == null)
            {
                throw new InvalidDataException(ApiResources.ApiErrorReceiptDoesntExist);
            }
            _data.DeleteOrphan(id);
            return null;
        }

        /// <summary>
        /// Checks the Account, throwing an error if it doesn't exist.
        /// </summary>
        /// <param name="accountId">The Id to check.</param>
        /// <returns></returns>
        private int CheckAccountAndThrow(int? accountId)
        {
            if (accountId == null)
            {
                return User.AccountID;
            }

            var account = new cAccounts().GetAccountByID(accountId.Value);
            if (account == null)
            {
                throw new InvalidDataException(ApiResources.ApiErrorAccountDoesntExist);
            }
            return account.accountid;
        }

        /// <summary>
        /// Checks an Employee Id for a valid employee, throwing errors if they don't exist or are archived.
        /// </summary>
        /// <param name="employeeId">The Id of the employee.</param>
        /// <param name="accountId">The account Id to look for the employee.</param>
        private void CheckEmployeeAndThrow(int employeeId, int accountId)
        {
            ActionContext.AccountId = accountId;
            var employee = ActionContext.Employees.GetEmployeeById(employeeId);
            if (employee == null)
            {
                throw new ApiException(ApiResources.ApiErrorWrongEmployeeId, ApiResources.ApiErrorWrongEmployeeIdMessage);
            }

            if (employee.Archived || !employee.Active)
            {
                throw new InvalidDataException(ApiResources.ApiErrorEmployeeIsArchived + employeeId);
            }
        }

        /// <summary>
        /// Checks a Claim Id for a valid Claim, throwing an error if it doesn't exist.
        /// </summary>
        /// <param name="claimId">The Id of the claim.</param>
        /// <param name="accountId">The account in which to look for the claim.</param>
        private void CheckClaimAndThrow(int claimId, int? accountId)
        {
            ActionContext.AccountId = accountId ?? ActionContext.AccountId;
            var claim = ActionContext.Claims.getClaimById(claimId);
            if (claim == null)
            {
                throw new InvalidDataException(ApiResources.ApiErrorClaimDoesntExist);
            }
        }

        /// <summary>
        /// Checks an ExpenseItemId for a valid ExpenseItem, throwing an error if it doesn't exist.
        /// </summary>
        /// <param name="savedExpenseId">The Id of the ExpenseItem.</param>
        private void CheckClaimLineAndThrow(int savedExpenseId)
        {
            var expense = ActionContext.Claims.getExpenseItemById(savedExpenseId);
            if (expense == null)
            {
                throw new InvalidDataException(ApiResources.ApiErrorClaimLineDoesntExist);
            }
        }

        /// <summary>
        /// Checks a Receipt Id for a valid Receipt, throwing an error if it doesn't exist.
        /// </summary>
        /// <param name="receiptId">The Id of the Receipt.</param>
        private void CheckReceiptAndThrow(int receiptId)
        {
            var receipt = _data.GetById(receiptId, false);
            if (receipt == null)
            {
                throw new InvalidDataException(ApiResources.ApiErrorReceiptDoesntExist);
            }
        }
    }
}

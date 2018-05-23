namespace SpendManagementApi.Repositories.Expedite
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using BusinessLogic.Modules;

    using Interfaces;
    using Models.Common;

    using SpendManagementApi.Models.Types.Expedite;

    using Utilities;
    using Spend_Management;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Expedite;
    using SpendManagementLibrary.Interfaces.Expedite;

    using Action = Spend_Management.Action;
    using Receipt = SpendManagementApi.Models.Types.Expedite.Receipt;
    using Receipts = SpendManagementLibrary.Expedite.Receipts;

    /// <summary>
    /// ReceiptRepository manages data access for Receipts.
    /// </summary>
    internal class ReceiptRepository : BaseRepository<Receipt>, ISupportsActionContext
    {
        private readonly IManageReceipts _data;

        /// <summary>
        /// An instance of <see cref="IActionContext"/>
        /// </summary>
        private readonly IActionContext _actionContext = null;

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
        /// Initializes a new instance of the <see cref="ReceiptRepository"/> class.
        /// </summary>
        public ReceiptRepository(){}

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
        /// The get receipt by id for the expedite process.
        /// </summary>
        /// <param name="receiptId">
        /// The receipt id.
        /// </param>
        /// <param name="accountId">
        /// The account id the receipt belongs to.
        /// </param>
        /// <returns>
        /// The <see cref="Receipt"/>.
        /// </returns>
        public Receipt GetReceiptByIdForExpedite(int receiptId, int accountId)
        {
            var user = this.MockCurrentUser(accountId);
            IManageReceipts receipts = new Receipts(accountId, user.EmployeeID);
            var receipt = new Receipt().From(receipts.GetById(receiptId), null);
            receipt.Data = receipts.GetData(receiptId);
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
        /// <param name="accountId">The account Id the expense belongs to</param>
        /// <returns>A list of Receipts.</returns>
        public IList<Receipt> GetByClaimLine(int savedExpenseId, int accountId)
        {
            var claims = new cClaims(accountId);
          
            CheckClaimLineAndThrow(savedExpenseId, claims);

            var expenseItem = claims.getExpenseItemById(savedExpenseId);
            var claim = claims.getClaimById(expenseItem.claimid);
            var subCategories = new cSubcats(accountId);
            var subCategory = subCategories.GetSubcatById(expenseItem.subcatid);

            IManageReceipts manageReceipts = new Receipts(accountId, claim.employeeid);
            var user = MockCurrentUser(accountId);

            return manageReceipts.GetByClaimLine(expenseItem, user, subCategory, claim).Select(e => new Receipt().From(e, null)).ToList();
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
        /// Creates a Receipt in the account that is passed in as part of the request and returns the newly created Receipt.
        /// Only the accountId, fileExtension and data properties are required. The rest are generated and repopulated.
        /// </summary>
        /// <param name="item">The item to create.</param>
        /// <returns>The newly created Receipt.</returns>
        public Receipt AddExpediteReceipt(ExpediteReceipt item)
        {
            if (item.AccountId == null)
            {
                throw new ApiException(ApiResources.ResponseForbiddenNoAccountId, ApiResources.ApiErrorNoAccountForReceipt);
            }

            if (item.EmployeeId == null)
            {
                throw new ApiException(ApiResources.ApiErrorWrongEmployeeId, ApiResources.ApiErrorNoEmployeeForReceipt);
            }

            int accountId = (int)item.AccountId;
            var employees = new cEmployees(accountId);
            var receipts = new Receipts(accountId);
            var claims = new cClaims(accountId);

            // before we upload the receipt, check the linkages.
            if (item.ClaimLines != null && item.ClaimLines.Any())
            {
               foreach (var claimLine in item.ClaimLines)
                {
                    this.CheckClaimLineAndThrow(claimLine, claims);
                }
            }
            else if (item.ClaimId.HasValue)
            {
                this.CheckClaimAndThrow(item.ClaimId.Value, claims);
            }
            else if (item.EmployeeId.HasValue)
            {
                this.CheckEmployeeAndThrow(item.EmployeeId.Value, employees);
            }

            // convert base64string to byte[]
            var byteArray = Convert.FromBase64String(item.Data);

            // now upload the receipt.
            var modified = new Receipt().From(receipts.AddReceipt(item.To(this._actionContext), byteArray), this._actionContext);

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
                toRemove.ForEach(i => this.UnlinkFromClaimLine(item.Id, i, receipts, claims));
                toAdd.ForEach(i => this.LinkToClaimLine(item.Id, i, receipts, claims));

                // set response properties to null.
                item.ClaimId = item.EmployeeId = null;
            }
            else if (item.ClaimId.HasValue)
            {
                // do the linkage
                this.LinkToClaim(item.Id, item.ClaimId.Value, receipts, claims);

                // set response properties to null.
                item.ClaimLines = null;
                item.EmployeeId = null;
            }
            else if (item.EmployeeId.HasValue)
            {
                // do the linkage
                this.LinkToClaimant(item.Id, item.EmployeeId.Value, receipts, employees);

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
        /// Links a receipt to a claimant (employee).
        /// Calling this will remove any ClaimLine-level links, and any claim link.
        /// </summary>
        /// <param name="receiptId">The ReceiptId of the receipt.</param>
        /// <param name="employeeId">The EmployeeId of the row in the Employees table.</param>
        /// <param name="receipts"> An instance of the <see cref="Receipts"/> Receipts class. </param>
        /// <param name="employees"> An instance of the <see cref="cEmployees"/> cEmployees class. </param>
        /// <returns>The linked receipt.</returns>
        /// <remarks>Used when ActionContext is null</remarks>
        public Receipt LinkToClaimant(int receiptId, int employeeId, Receipts receipts, cEmployees employees)
        {
            this.CheckReceiptAndThrow(receiptId, receipts);
            this.CheckEmployeeAndThrow(employeeId, employees);
            return new Receipt().From(receipts.LinkToClaimant(receiptId, employeeId), this._actionContext);
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
        /// Links a receipt to a claim header. 
        /// A receipt can only be placed against one claim.
        /// Calling this will remove any ClaimLine-level links, and any claimant link.
        /// </summary>
        /// <param name="receiptId">The ReceiptId of the receipt.</param>
        /// <param name="claimId">The ClaimId of the row in the claims table.</param>
        /// <param name="receipts"> An instance of the <see cref="Receipts"/> Receipts class. </param>
        /// <param name="claims"> An instance of the <see cref="cClaims"/> cClaims class. </param>
        /// <returns>The linked receipt.</returns>
        /// <remarks>Used when ActionContext is null</remarks>
        public Receipt LinkToClaim(int receiptId, int claimId, Receipts receipts, cClaims claims)
        {
            this.CheckReceiptAndThrow(receiptId, receipts);
            this.CheckClaimAndThrow(claimId, claims);
            return new Receipt().From(receipts.LinkToClaim(receiptId, claimId), this._actionContext);
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
        /// Links a receipt to a claim line. Call this multiple times if needed.
        /// Receipts can now be stored against multiple lines. A line can also have multiple receipts.
        /// Calling this will remove any Claim link, and any claimant link, but not other claimline links.
        /// To remove a ClaimLine-level link, use <see cref="IManageReceipts.UnlinkFromClaimLine"/>.
        /// </summary>
        /// <param name="receiptId">The ReceiptId of the receipt.</param>
        /// <param name="savedExpenseId">The ExpenseId of the row in the savedexpenses table.</param>
        /// <param name="receipts"> An instance of the <see cref="Receipts"/> Receipts class. </param>
        /// <param name="claims"> An instance of the <see cref="cClaims"/> cClaims class. </param>
        /// <returns>The linked receipt.</returns>
        /// <remarks>Used when ActionContext is null</remarks>
        public Receipt LinkToClaimLine(int receiptId, int savedExpenseId, Receipts receipts, cClaims claims)
        {
            this.CheckReceiptAndThrow(receiptId, receipts);
            this.CheckClaimLineAndThrow(savedExpenseId, claims);
            return new Receipt().From(receipts.LinkToClaimLine(receiptId, savedExpenseId), this._actionContext);
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
        /// Removes the links between a receipt and a claim line. Call this multiple times if needed.
        /// Receipts can now be stored against multiple lines. A line can also have multiple receipts.
        /// </summary>
        /// <param name="receiptId"> The ReceiptId of the receipt. </param>
        /// <param name="savedExpenseId"> The ExpenseId of the row in the savedexpenses table. </param>
        /// <param name="receipts"> An instance of the <see cref="Receipts"/> Receipts class. </param>
        /// <param name="claims"> An instance of the <see cref="cClaims"/> cClaims class. </param>
        /// <returns> The linked receipt. </returns>
        /// <remarks>Used when ActionContext is null</remarks>
        public Receipt UnlinkFromClaimLine(int receiptId, int savedExpenseId, Receipts receipts, cClaims claims)
        {
            this.CheckReceiptAndThrow(receiptId, receipts);
            this.CheckClaimLineAndThrow(savedExpenseId, claims);
            return new Receipt().From(receipts.UnlinkFromClaimLine(receiptId, savedExpenseId), this._actionContext);
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
            this.ThrowEmployeeException(employee);
        }

        /// <summary>
        /// Checks an Employee Id for a valid employee, throwing errors if they don't exist or are archived.
        /// </summary>
        /// <param name="employeeId">The Id of the employee.</param>
        /// <param name="employees">The instance of <see cref="cEmployees"/>cClaims</param>
        /// <remarks>Used when ActionContext is null</remarks>
        private void CheckEmployeeAndThrow(int employeeId, cEmployees employees)
        {
            var employee = employees.GetEmployeeById(employeeId);
            this.ThrowEmployeeException(employee);
        }

        /// <summary>
        /// Throws exceptions if the employee doesn't exist or is archived
        /// </summary>
        /// <param name="employee">The employee</param>
        private void ThrowEmployeeException(Employee employee)
        {
            if (employee == null)
            {
                throw new ApiException(ApiResources.ApiErrorWrongEmployeeId, ApiResources.ApiErrorWrongEmployeeIdMessage);
            }

            if (employee.Archived || !employee.Active)
            {
                throw new InvalidDataException(ApiResources.ApiErrorEmployeeIsArchived + employee.EmployeeID);
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
        /// Checks a Claim Id for a valid Claim, throwing an error if it doesn't exist.
        /// </summary>
        /// <param name="claimId">The Id of the claim.</param>
        /// <param name="claims">The instance of <see cref="cClaims"/>cClaims</param>
        /// <remarks>Used when ActionContext is null</remarks>
        private void CheckClaimAndThrow(int claimId, cClaims claims)
        {
            var claim = claims.getClaimById(claimId);
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
        /// Checks an ExpenseItemId for a valid ExpenseItem, throwing an error if it doesn't exist.
        /// </summary>
        /// <param name="savedExpenseId">The Id of the ExpenseItem.</param>
        /// <param name="claims">The instance of <see cref="cClaims"/>cClaims</param>
        /// <remarks>Used when ActionContext is null</remarks>
        private void CheckClaimLineAndThrow(int savedExpenseId, cClaims claims)
        {
            var expense = claims.getExpenseItemById(savedExpenseId);
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


        /// <summary>
        /// Checks a Receipt Id for a valid Receipt, throwing an error if it doesn't exist.
        /// </summary>
        /// <param name="receiptId">The Id of the Receipt.</param>
        /// <param name="receipts"> An instance of the <see cref="Receipts"/> Receipts class. </param>
        /// <remarks>Used when ActionContext is null</remarks>
        private void CheckReceiptAndThrow(int receiptId, Receipts receipts)
        {
            var receipt = receipts.GetById(receiptId, false);
            if (receipt == null)
            {
                throw new InvalidDataException(ApiResources.ApiErrorReceiptDoesntExist);
            }
        }

        private ICurrentUser MockCurrentUser(int accountId)
        {
            int employeeId = 0;
            ICurrentUser user = new CurrentUser(accountId, employeeId, 0, Modules.Expenses, 1);
            return user;
        }
    }
}

namespace SpendManagementApi.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces;
    using Models.Common;
    using Models.Types.Employees;
    using SpendManagementApi.Models.Types;
    using SpendManagementLibrary;

    using Spend_Management;
    using Utilities;
    using CardStatement = SpendManagementApi.Models.Types.CardStatement;
    using CreditCardTransaction = SpendManagementApi.Models.Types.CreditCardTransaction;

    /// <summary>
    /// CorporateCardRepository manages data access for CorporateCards.
    /// </summary>
    internal class CorporateCardRepository : BaseRepository<CorporateCard>, ISupportsActionContext
    {
        /// <summary>
        ///  cEmployeeCorporateCards instance
        /// </summary>
        private cEmployeeCorporateCards _data;

        /// <summary>
        /// cEmployees instance
        /// </summary>
        private cEmployees _employees;

        /// <summary>
        /// cCardStatements instance
        /// </summary>
        private cCardStatements _cardStatements;

        /// <summary>
        /// Creates a new CorporateCardRepository with the passed in user.
        /// </summary>
        /// <param name="user">The current user.</param>
        /// <param name="actionContext">An implementation of ISupportsActionContext</param>
        public CorporateCardRepository(ICurrentUser user, IActionContext actionContext)
            : base(user, actionContext, x => x.Id, x => x.CardNumber)
        {
            this._data = this.ActionContext.EmployeeCorporateCards;
            this._employees = this.ActionContext.Employees;
            this._cardStatements = this.ActionContext.CardStatements;
        }

        /// <summary>
        /// Gets all the Cards in the system.
        /// </summary>
        /// <returns>All corporate cards</returns>
        public override IList<CorporateCard> GetAll()
        {
            return this._data.GetFlatList().Select(x => x.Cast<CorporateCard>()).ToList();
        }

        /// <summary>
        /// Gets a single CorporateCard by it's id.
        /// </summary>
        /// <param name="id">The id of the CorporateCard to get.</param>
        /// <returns>The CorporateCard.</returns>
        public override CorporateCard Get(int id)
        {
            return this._data.GetCorporateCardByID(id).Cast<CorporateCard>();
        }

        /// <summary>
        /// Gets all the CorporateCards for the given employee.
        /// </summary>
        /// <param name="id">The Id of the employee</param>
        /// <returns>List of all corporate cards for given employee</returns>
        public List<CorporateCard> GetAllCardsByEmployee(int id)
        {
            var employee = this._employees.GetEmployeeById(id);
            if (employee == null)
            {
                throw new ApiException(ApiResources.ApiErrorWrongEmployeeId, ApiResources.ApiErrorWrongEmployeeIdMessage);
            }
            return this._data.GetEmployeeCorporateCards(id).Select(x => x.Value.Cast<CorporateCard>()).ToList();
        }

        /// <summary>Adds a CorporateCard to the Employee with the supplied Id.</summary>
        /// <param name="item">The CorporateCard to add.</param>
        /// <returns>The added Corporate Card</returns>
        public override CorporateCard Add(CorporateCard item)
        {
            item.CreatedOn = DateTime.UtcNow;
            item.CreatedById = this.User.EmployeeID;
            return this.SaveCorporateCard(item);
        }

        /// <summary>Updates a CorporateCard.</summary>
        /// <param name="item">The item to update.</param>
        /// <returns>The updated CorporateCard.</returns>
        public override CorporateCard Update(CorporateCard item)
        {
            var corporateCard = this.Get(item.Id);

            if (corporateCard == null)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.ApiErrorValidCardId);
            }

            if (corporateCard.EmployeeId != item.EmployeeId)
            {
                throw new ApiException(ApiResources.ApiErrorDeleteUnsuccessful, ApiResources.ApiErrorCannotReAssignCardByChangingEmployee);
            }

            item.ModifiedOn = DateTime.UtcNow;
            item.ModifiedById = this.User.EmployeeID;
            return this.SaveCorporateCard(item);
        }

        /// <summary>
        /// Deletes a CorporateCard, given it's ID.
        /// </summary>
        /// <param name="id">The Id of the CorporateCard to delete.</param>
        /// <returns>Null, if the card was deleted successfully.</returns>
        public override CorporateCard Delete(int id)
        {
            // attempt the delete
            var result = this._data.DeleteCorporateCard(id);

            if (result < 1)
            {
                throw new ApiException(ApiResources.ApiErrorDeleteUnsuccessful, ApiResources.ApiErrorDeleteUnsuccessfulMessage);
            }

            return null;
        }

        /// <summary>
        /// Gets all the card statements for the given employee current user
        /// </summary>
        /// <returns>List of <see cref="Models.Types.CardStatement"/></returns>
        public List<CardStatement> GetCardStatementsForCurrentUser()
        {
            var cardStatements = this._data.GetCardStatementsByEmployee(this.User.EmployeeID);
            return cardStatements.Select(item => item.Cast<CardStatement>()).ToList();
        }

        /// <summary>
        /// Gets additional transaction information for given Transaction Id
        /// </summary>
        /// <param name="transactionId">
        /// Transaction Id of the statement
        /// </param>
        /// A Dictionary of transaction description and its value.
        /// </returns>
        public Dictionary<string, string> GetAdditionalTransactionInfo(int transactionId)
        {
            var validate = this.CheckTransactionOwnership(transactionId);
            switch (validate)
            {
                case -1:
                    throw new ApiException(
                        ApiResources.ApiErrorUnsuccessfulRetrievalOfAdditionalTransactionInformation,
                        ApiResources.ApiErrorNoTransactionFound);
                case 1:
                    throw new ApiException(
                        ApiResources.ApiErrorUnsuccessfulRetrievalOfAdditionalTransactionInformation,
                        ApiResources.ApiErrorTransactionInsufficientPermission);
                default:
                    try
                    {
                        cCardTransaction transaction = ActionContext.CardStatements.getTransactionById(transactionId);
                        cCardStatement statement = ActionContext.CardStatements.getStatementById(transaction.statementid);
                        cCardTemplate templateData = ActionContext.CardTemplates.getTemplate(statement.Corporatecard.cardprovider.cardprovider);

                        return this._cardStatements.GetTransactionData(templateData, transaction);
                    }
                    catch (Exception e)
                    {
                        throw new ApiException(ApiResources.ApiErrorGeneralError, e.Message);
                    }
            }
        }

        /// <summary>
        /// Gets additional transaction information for given Transaction Id when viewing as an approver.
        /// </summary>
        /// <param name="transactionId">
        /// The transaction id.
        /// </param>
        /// <param name="expenseId">
        /// The expense Id, used to check the current user is in the expense item's claim hierarchy.
        /// </param>
        /// <returns>
        /// A Dictionary of transaction description and its value.
        /// </returns>
        public Dictionary<string, string> GetAdditionalTransactionInfoAsApprover(int transactionId, int expenseId)
        {
            cExpenseItem item = new ExpenseItemRepository(this.User, this.ActionContext).GetExpenseItem(expenseId);

            if (item.transactionid != transactionId)
            {
                throw new ApiException(
                    ApiResources.ApiErrorUnsuccessfulRetrievalOfAdditionalTransactionInformation,
                    ApiResources.ApiErrorTransactionDoesNotBelongToExpense);              
            }
            
            //Checks claim ownership before proceeding, will throw exception if currenct user is not in the claim hierarchy
            new ClaimRepository(this.User, this.ActionContext).Get(item.claimid);

            cCardTransaction transaction = ActionContext.CardStatements.getTransactionById(transactionId);
            cCardStatement statement = ActionContext.CardStatements.getStatementById(transaction.statementid);
            cCardTemplate templateData = ActionContext.CardTemplates.getTemplate(statement.Corporatecard.cardprovider.cardprovider);

            return this._cardStatements.GetTransactionData(templateData, transaction);
        }

        /// <summary>
        /// Unmatch transaction 
        /// </summary>
        /// <param name="expenseId">Expense id</param>
        /// <returns>Numeric value</returns>
        public int UnmatchTransaction(int expenseId)
        {
            var expenseItemRepository = new ExpenseItemRepository(this.User, this.ActionContext);
            var claimRepository = new ClaimRepository(this.User, this.ActionContext);
            var expenseItem = expenseItemRepository.Get(expenseId);

            expenseItemRepository.CheckCurrentUserIsExpenseItemOwner(expenseItem, claimRepository);

            try
            {
                this._cardStatements.unmatchItem(expenseId);
            }
            catch (Exception ex)
            {
                return 0;
            }

            return 1;
        }

        /// <summary>
        /// Match transactions to expense item
        /// </summary>
        /// <param name="transactionId">
        /// The Transaction id
        /// </param>
        /// <param name="expenseId">
        /// The Expense id
        /// </param>
        /// <param name="isMobileRequest">
        /// If the request was from a mobile device.
        /// </param>
        /// <returns>
        /// The outcome of the action
        /// </returns>
        public int MatchTransaction(int transactionId, int expenseId, bool isMobileRequest)
        {
            var expenseItemRepository = new ExpenseItemRepository(this.User, this.ActionContext);
            var expenseItem = expenseItemRepository.Get(expenseId);

            if (expenseItem.TransactionId != 0)
            {
                throw new ApiException(
                       ApiResources.ApiErrorUnsuccessfulTransactionMatch,
                       ApiResources.ApiErrorExpenseItemAssociatedWithAnotherTransaction);
            }

            var claimsRepository = new ClaimRepository(this.User, this.ActionContext);

            expenseItemRepository.CheckCurrentUserIsExpenseItemOwner(expenseItem, claimsRepository);

            var validate = this.CheckTransactionOwnership(transactionId);
            switch (validate)
            {
                case -1:
                    throw new ApiException(
                        ApiResources.ApiErrorUnsuccessfulTransactionMatch,
                        ApiResources.ApiErrorNoTransactionFound);
                case 1:
                    throw new ApiException(
                        ApiResources.ApiErrorUnsuccessfulTransactionMatch,
                        ApiResources.ApiErrorTransactionInsufficientPermission);
                default:

                    var transaction = this._cardStatements.getTransactionById(transactionId);
                    var statement = this._cardStatements.getStatementById(transaction.statementid);
                    var result = this._cardStatements.matchTransaction(statement, transaction, this.ActionContext.Claims.getExpenseItemById(expenseId));

                    if (!result && !isMobileRequest)
                    {
                        throw new ApiException(
                            ApiResources.ApiErrorUnsuccessfulTransactionMatch,
                            ApiResources.ApiErrorTransactionMatchCouldNotBeMade);
                    }

                    return result ? 1 : 0;
            }


        }

        /// <summary>
        /// Get a list of <see cref="CreditCardTransaction"/> for the supplied statementId and current user
        /// </summary>
        /// <param name="statementId">
        /// The statementId.
        /// </param>
        /// <returns>
        /// A list of <see cref="CreditCardTransaction"/>
        /// </returns>
        public List<CreditCardTransaction> GetTransactionsForStatementId(int statementId)
        {
            cMisc misc = new cMisc(User.AccountID);
            var currenciesRepository = new CurrencyRepository(this.User, this.ActionContext);
            var employeeRepository = new EmployeeRepository(User, ActionContext);
            var primaryCurrencySymbol = employeeRepository.DeterminePrimaryCurrencySymbol(misc, currenciesRepository);

            List<SpendManagementLibrary.Cards.CreditCardTransaction> transactions = this._cardStatements.GetTransactionsForStatementId(this.User, statementId);
            List<CreditCardTransaction> creditCardTransactions = new List<CreditCardTransaction>();

            foreach (var transaction in transactions)
            {
                var creditCardTransaction = new CreditCardTransaction().ToApiType(transaction, this.ActionContext);
                creditCardTransaction.PrimaryCurrencySymbol = primaryCurrencySymbol;
                creditCardTransaction.RemainingAmountToBeReconciled = this._cardStatements.getUnallocatedAmount(transaction.TranactionId, this.User.EmployeeID);
                creditCardTransactions.Add(creditCardTransaction);
            }

            return creditCardTransactions;
        }

        /// <summary>
        /// Imports credit card transactions
        /// </summary>
        /// <param name="cardProviderName">
        /// The card provider name.
        /// </param>
        /// <param name="transactionData">
        /// The transaction import data.
        /// </param>
        /// <param name="statementDate">
        /// The statement date.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> statementId the transactions belong to.
        /// </returns>
        public int ImportCreditCardTransactions(string cardProviderName, string transactionData, DateTime statementDate)
        {                    
            cCardProvider provider = ActionContext.CardProviders.getProviderByName(cardProviderName);        
            cCorporateCard corporatecard = ActionContext.CorporateCards.GetCorporateCardById(provider.cardproviderid);

            string name = $"{provider.cardprovider} statement imported {DateTime.Now}";
            var statement = new cCardStatement(User.AccountID, corporatecard.cardprovider.cardproviderid, 0, name, statementDate, DateTime.Now.ToUniversalTime(), User.EmployeeID, DateTime.Now, User.EmployeeID);

            Byte[] transactions;

            try
            {
                transactions = Convert.FromBase64String(transactionData);
            }
            catch (Exception)
            {
                throw new ApiException(
                   ApiResources.ApiErrorGeneralError,
                   ApiResources.ApiErrorTransactionImportFailedToConvertTransactionData);
            }
   
            cCardTemplate template = ActionContext.CardTemplates.getTemplate(cardProviderName);
            cImport import = ActionContext.CardStatements.GetCardRecordTypeData(template, template.RecordTypes[CardRecordType.CardTransaction], transactions);
         
            //Add the statement
            int statementId = ActionContext.CardStatements.addStatement(statement);

            try
            {
                //Add the card transactions
                ActionContext.CardStatements.ImportCardTransactions(template, statementId, statement.Corporatecard.cardprovider, transactions, import);
            }
            catch (Exception ex)
            {

                throw new ApiException(
                     ApiResources.ApiErrorGeneralError,
                     ApiResources.ApiErrorTransactionImportFailed + ex.Message);
            }
 
            return statementId;
        }

        /// <summary>
        /// Deletes a statement by its Id.
        /// </summary>
        /// <param name="statementId">
        /// The statement Id.
        /// </param>
        /// <returns>
        /// The outcome of the action, whereby success is 1.
        /// </returns>
        /// <exception cref="ApiException">
        /// </exception>
        public int DeleteStatementById(int statementId)
        {
            cCardStatement statement = this._cardStatements.getStatementById(statementId);

            if (statement == null)
            {
                throw new ApiException(ApiResources.ApiErrorDeleteUnsuccessful, ApiResources.ApiErrorStatementNotFound);
            }
         
            bool statementDeleted = this._cardStatements.deleteStatement(statementId);
                               
            if (!statementDeleted)
            {
                throw new ApiException(ApiResources.ApiErrorDeleteUnsuccessful, ApiResources.ApiErrorStatementTransactionsAlreadyReconciled);
            }
        
            return 1;
        }

        /// <summary>
        /// This method is used for saving Corporate Card
        /// </summary>
        /// <param name="item">Corporate card <see cref="CorporateCard"/></param>
        /// <returns>Returns a Corporate card <see cref="CorporateCard"/></returns>
        private CorporateCard SaveCorporateCard(CorporateCard item)
        {
            item.Validate(this.ActionContext);

            var converted = item.Cast<cEmployeeCorporateCard>();
            var result = this._data.SaveCorporateCard(converted);

            // card number already exists
            if (result == -1)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.ApiErrorCardNumberExists);
            }

            item.Id = result;
            return this.Get(item.Id);
        }

        /// <summary>
        /// Checks the transaction belongs to the employee
        /// </summary>
        /// <param name="transactionId">
        /// The transaction id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> with the outcome of the check.
        /// </returns>
        private int CheckTransactionOwnership(int transactionId)
        {
            var transaction = this._cardStatements.getTransactionById(transactionId);
            if (transaction == null)
            {
                // Indicates that transaction did not exists in Server
                return -1;
            }

            var cardStatements = this._data.GetCardStatementsByEmployee(this.User.EmployeeID);

            // 0 indicates that statement id for transaction id and user's employee ID are same
            // 1 indicates that statement id for transaction id and user's employee ID are not same
            return cardStatements.Any(cardStatement => cardStatement.StatementId == transaction.statementid) ? 0 : 1;
        }

        /// <summary>
        /// Allocates a card to an employee.
        /// </summary>
        /// <param name="statementId">The Statement Id</param>
        /// <param name="employeeId">The Employee Id</param>
        /// <param name="cardNumber">The Card number</param>
        /// <returns>1 if card allocation successful, else 0</returns>
        public int AllocateCardToEmployee(int statementId, int employeeId, string cardNumber)
        {
            if (ActionContext.Employees.GetEmployeeById(employeeId) == null)
            {
                throw new ApiException(ApiResources.ApiErrorWrongEmployeeId, ApiResources.ApiErrorWrongEmployeeIdMessage);
            }

            var statement = ActionContext.CardStatements.getStatementById(statementId);

            if (statement == null)
            {
                throw new ApiException(ApiResources.ApiErrorUpdateUnsuccessfulMessage, ApiResources.ApiErrorStatementNotFound);
            }

            try
            {
                ActionContext.CardStatements.allocateCard(statementId, employeeId, cardNumber);
            }
            catch (Exception)
            {
                return 0;
            }

            return 1;
        }
    }
}

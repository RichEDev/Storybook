namespace SpendManagementApi.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;

    using Interfaces;
    using Models.Common;
    using Models.Types;
    using Utilities;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers.AuditLogger;

    using Spend_Management;

    /// <summary>
    /// The AdvancesRepository
    /// </summary>
    internal class AdvancesRepository : BaseRepository<Advance>, ISupportsActionContext
    {
        private readonly IDataFactory<IGeneralOptions, int> _generalOptionsFactory;

        /// <summary>
        /// The AdvancesRepository constructor
        /// </summary>
        /// <param name="user">The current user</param>
        /// <param name="actionContext">The actioncontext</param>
        public AdvancesRepository(ICurrentUser user, IActionContext actionContext)
            : base(user, actionContext, x => x.FloatId, x => x.Name)
        {
            this._generalOptionsFactory = WebApiApplication.container.GetInstance<IDataFactory<IGeneralOptions, int>>();
        }

        /// <summary>
        /// Get all of T
        /// </summary>
        /// <returns></returns>
        public override IList<Advance> GetAll()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get <see cref="Advance">Advance</see> by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override Advance Get(int id)
        {
            var advance = this.ActionContext.Advances.GetFloatById(id);
            return new Advance().From(advance, this.ActionContext);
        }

        /// <summary>
        /// Gets the <see cref="Advance">Advances</see> available for the supplied currencyId and current employee
        /// </summary>
        /// <param name="currencyId">The currencyId</param>
        /// <returns>A list of <see cref="Advance">Advances</see> </returns>
        public List<Advance> GetAvailableAdvances(int currencyId)
        {
            var availableAdances = new List<Advance>();

            var advances = this.ActionContext.Advances.GetAvailableAdvances(
                this.User.EmployeeID,
                currencyId,
                0,
                this.User.CurrentSubAccountId);

            foreach (var advance in advances)
            {
                var availableAdvance = new Advance().From(advance, this.ActionContext);
                var currency = this.ActionContext.Currencies.getCurrencyById(availableAdvance.CurrencyId);

                availableAdvance.DisplayName =
                    $"{availableAdvance.Name} ({this.ActionContext.GlobalCurrencies.getGlobalCurrencyById(currency.globalcurrencyid).label})";

                availableAdances.Add(availableAdvance);
            }

            return availableAdances.OrderBy(x => x.Name).ToList();
        }

        /// <summary>
        /// Gets the <see cref="Advance">Advances</see> available for current employee
        /// </summary>
        /// <returns>A list of <see cref="Advance">Advances</see> </returns>
        public List<Advance> GetAvailableAdvancesForUser()
        {
            var availableAdances = new List<Advance>();

            var advances = this.ActionContext.Advances.GetAvailableAdvancesForUser(
                this.User.EmployeeID,
                this.User.CurrentSubAccountId);

            foreach (var advance in advances)
            {
                var availableAdvance = new Advance().From(advance, this.ActionContext);
                var currency = this.ActionContext.Currencies.getCurrencyById(availableAdvance.CurrencyId);

                availableAdvance.DisplayName =
                    $"{availableAdvance.Name} ({this.ActionContext.GlobalCurrencies.getGlobalCurrencyById(currency.globalcurrencyid).label})";

                availableAdances.Add(availableAdvance);
            }

            return availableAdances.OrderBy(x => x.Name).ToList();
        }

        /// <summary>
        /// Get a list of unsettled <see cref="MyAdvance"/> for the current user
        /// </summary>
        /// <returns>
        /// The <see cref="List"/> of <see cref="MyAdvance"/>.
        /// </returns>
        public List<MyAdvance> GetUnsettledAdvancesForCurrentUser()
        {
            List<cFloat> advances = this.ActionContext.Advances.GetUnsettledAdvancesByEmployee(this.User.EmployeeID);

            List<MyAdvance> myAdvances = new List<MyAdvance>();
            foreach (var advance in advances)
            {
                var myAdvance = new MyAdvance().ToApiType(advance, this.ActionContext);

                var currenciesRepository = new CurrencyRepository(this.User, this.ActionContext);

                // use base currency symbol as requested amount gets converted to the user's base currency, so need to show the correct symbol
                myAdvance.CurrencySymbol = currenciesRepository.DetermineCurrencySymbol(myAdvance.BaseCurrency);

                cCurrency currency = this.ActionContext.Currencies.getCurrencyById(myAdvance.CurrencyId);
                myAdvance.CurrencyName = this.ActionContext.GlobalCurrencies
                    .getGlobalCurrencyById(currency.globalcurrencyid).label;

                if (myAdvance.Approver > 0)
                {
                    var employee = this.ActionContext.Employees.GetEmployeeById(myAdvance.Approver);
                    myAdvance.ApproverName = $"{employee.Surname}, {employee.Title} {employee.Forename}";
                }
                else
                {
                    myAdvance.ApproverName = string.Empty;
                }

                cGroup signOffGroup = this.ActionContext.SignoffGroups.GetGroupById(this.User.Employee.AdvancesSignOffGroup);
                myAdvance.StageDescription = $"{myAdvance.Stage} of {signOffGroup.stagecount}";

                if (myAdvance.ExchangeRate > 0)
                {
                    myAdvance.ForeignAmount = Math.Round(myAdvance.ForeignAmount, 2, MidpointRounding.AwayFromZero);
                }

                myAdvances.Add(myAdvance);
            }

            return myAdvances;
        }

        /// <summary>
        /// Gets the <see cref="Advance">Advances</see> available for claimant
        /// </summary>
        /// <param name="expenseId">Expenses id</param>
        /// <param name="employeeId">Employee id</param>
        /// <returns>A list of <see cref="Advance">Advances</see> </returns>
        public List<Advance> GetAvailableAdvancesForClaimant(int expenseId, int employeeId)
        {
            var expenseItemRepository = new ExpenseItemRepository(this.User, this.ActionContext);
            expenseItemRepository.ClaimantDataPermissionCheck(expenseId, employeeId);

            var availableAdances = new List<Advance>();
            var advances = this.ActionContext.Advances.GetAvailableAdvancesForUser(
                employeeId, this.User.CurrentSubAccountId);

            foreach (var advance in advances)
            {
                var availableAdvance = new Advance().From(advance, this.ActionContext);
                var currency = this.ActionContext.Currencies.getCurrencyById(availableAdvance.CurrencyId);

                availableAdvance.DisplayName = $"{availableAdvance.Name} ({this.ActionContext.GlobalCurrencies.getGlobalCurrencyById(currency.globalcurrencyid).label})";
                availableAdances.Add(availableAdvance);

                this.ActionContext.Advances.AuditViewAdvances( $"{advance.name} ({ this.ActionContext.Employees.GetEmployeeById(advance.employeeid).Username})",this.User, false);
            }

            return availableAdances.OrderBy(x => x.Name).ToList();
        }
        
        /// <summary>
        /// Saves an <see cref="Advance">Advance</see>
        /// </summary>
        /// <param name="advanceName">The advance name</param>
        /// <param name="advanceReason">The advance reason</param>
        /// <param name="amount">The advance amount</param>
        /// <param name="currencyId">The currency of the amount</param>
        /// <param name="requiredByDate">The date the currency is required by, if any</param>
        /// <returns>The saved <see cref="Advance">Advance</see></returns>
        public Advance RequestAdvance(string advanceName, string advanceReason, decimal amount, int currencyId, DateTime? requiredByDate)
        {
            var requiredBy = requiredByDate == null ? string.Empty : requiredByDate.ToString();
            var advanceId = this.ActionContext.Advances.requestFloat(
                this.User.EmployeeID,
                advanceName,
                advanceReason,
                amount,
                currencyId,
                requiredBy,
                this.GetEmployeeCurrencyId());

            var outcomeOfSave = this.ProcessRequestAdvanceOutcome(advanceId);

            if (outcomeOfSave != string.Empty)
            {
                return new Advance { Outcome = outcomeOfSave };
            }

            this.ReinitialiseCache();

            return this.Get(advanceId);
        }

        /// <summary>
        /// Updates an <see cref="Advance">Advance</see>
        /// </summary>
        /// <param name="advanceId">The advance Id</param>
        /// <param name="advanceName">The advance name</param>
        /// <param name="advanceReason">The advance reason</param>
        /// <param name="amount">The advance amount</param>
        /// <param name="currencyId">The currency of the amount</param>
        /// <param name="requiredByDate">The date the currency is required by, if any</param>
        /// <returns>The saved <see cref="Advance">Advance</see></returns>
        public Advance UpdateAdvance(int advanceId, string advanceName, string advanceReason, decimal amount, int currencyId, DateTime? requiredByDate)
        {
            var advance = new Advance();
            var message = this.ValidateAdvanceEmployee(advanceId);
            if (!string.IsNullOrEmpty(message))
            {
                // Validation failed
                advance.Outcome = message;
            }
            else
            {
                // Update Advance
                var output = this.ActionContext.Advances.updateFloat(
                    this.User.EmployeeID,
                    advanceId,
                    advanceName,
                    advanceReason,
                    amount,
                    currencyId,
                    requiredByDate == null ? string.Empty : requiredByDate.ToString(),
                    this.GetEmployeeCurrencyId()
                );

                var outcomeOfSave = this.ProcessRequestAdvanceOutcome(output);

                if (!string.IsNullOrEmpty(outcomeOfSave))
                {
                    advance.Outcome = outcomeOfSave;
                }
                else
                {
                    this.ReinitialiseCache();
                    advance = this.Get(advanceId);
                }
            }

            return advance;
        }

        /// <summary>
        /// Approves an Advance
        /// </summary>
        /// <param name="advanceId">The advanceId</param>
        /// <returns>The <see cref="Advance">Advance</see></returns>
        public Advance ApproveAdvance(int advanceId)
        {
            var advance = this.ValidateAdvanceApprover(advanceId);
            this.ActionContext.Advances.SendClaimToNextStage(advance, false, this.User.EmployeeID, advance.employeeid);
            this.ReinitialiseCache();

            return this.Get(advanceId);
        }

        /// <summary>
        /// Pays an Advance
        /// </summary>
        /// <param name="advanceId">The advanceId</param>
        /// <returns>The <see cref="Advance">Advance</see></returns>
        public Advance PayAdvance(int advanceId)
        {
            this.ValidateAdvanceApprover(advanceId);

            this.ActionContext.Advances.payAdvance(this.User.AccountID, advanceId);
            this.ReinitialiseCache();
            return this.Get(advanceId);
        }

        /// <summary>
        /// Deletes an Advance
        /// </summary>
        /// <param name="advanceId">The advanceId</param>
        /// Returns 'Success' if advance gets deleted successfully otherwise errom message.
        public string DeleteAdvance(int advanceId)
        {
            var message = this.ValidateAdvanceEmployee(advanceId);
            if (string.IsNullOrEmpty(message))
            {
                this.ActionContext.Advances.deleteFloat(advanceId);
                this.ReinitialiseCache();
                return "Success";
            }
            return message;
        }

        /// <summary>
        /// Create a new instance of <see cref="cFloats">To ensure the cahce has the correct Advance data</see>
        /// </summary>
        private void ReinitialiseCache()
        {
            this.ActionContext.Advances = new cFloats(this.User.AccountID);
        }

        /// <summary>
        /// Validates to ensure the approver action can take place on the Advance
        /// </summary>
        /// <param name="advanceId">The advanceId</param>
        /// <returns>The <see cref="cFloat">cFloat</see></returns>
        private cFloat ValidateAdvanceApprover(int advanceId)
        {
            cFloat advance = this.ActionContext.Advances.GetFloatById(advanceId);

            if (advance == null)
            {
                throw new ApiException(ApiResources.ApiErrorUpdateUnsuccessfulMessage, ApiResources.ApiErrorAdvanceNotFound);
            }

            if (advance.approver != this.User.EmployeeID)
            {
                throw new ApiException(ApiResources.ApiErrorUpdateUnsuccessfulMessage, ApiResources.ApiErrorNotAdvanceApprover);
            }

            return advance;
        }

        /// <summary>
        /// Validates to ensure the employee action can take place on the Advance
        /// </summary>
        /// <param name="advanceId">The advanceId</param>
        private string ValidateAdvanceEmployee(int advanceId)
        {
            var advance = this.ActionContext.Advances.GetFloatById(advanceId);

            if (advance == null)
            {
                return ApiResources.ApiErrorAdvanceNotFound;
            }

            if (advance.employeeid != this.User.EmployeeID)
            {
                return ApiResources.ApiErrorNotAdvanceOwner;
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets employee primary currency if has been set.
        /// otherwise account base currency.
        /// </summary>
        /// <returns>currency id</returns>
        private int GetEmployeeCurrencyId()
        {
            var employee = this.ActionContext.Employees.GetEmployeeById(this.User.EmployeeID);

            // Return employee Primary Currency ID
            if (employee.PrimaryCurrency != 0)
            {
                return employee.PrimaryCurrency;
            }

            // Return Account Base Currency ID
            var generalOptions = this._generalOptionsFactory[this.User.CurrentSubAccountId].WithCurrency();
            return (int)generalOptions.Currency.BaseCurrency;
        }

        /// <summary>
        /// Processes the outcome of saving a request to the database.
        /// </summary>
        /// <param name="advanceId">
        /// The advance id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> with the reason the save could not be fulfilled.
        /// </returns>
        private string ProcessRequestAdvanceOutcome(int advanceId)
        {
            switch (advanceId)
            {
                case 1: return "An exchange rate does not exist for the selected currency. Please consult your administrator";
                case 2: return "An advance with this name already exists";
                default: return string.Empty;
            }
        }
    }
}
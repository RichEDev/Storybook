namespace SpendManagementApi.Repositories
{
    using System;
    using System.Collections.Generic;

    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;

    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Mobile;

    using Spend_Management;
    using Spend_Management.shared.code.Mobile;

    using Address = SpendManagementApi.Models.Types.Address;
    using ExpenseItem = SpendManagementApi.Models.Types.ExpenseItem;

    /// <summary>
    /// The journey repository.
    /// </summary>
    internal class JourneyRepository : BaseRepository<JourneyStep>, ISupportsActionContext
    {
        private readonly IDataFactory<IGeneralOptions, int> _generalOptionsFactory;

        public JourneyRepository(ICurrentUser user, IActionContext actionContext)
            : base(user, actionContext, a => a.StepNumber, null)
        {
            this._generalOptionsFactory = WebApiApplication.container.GetInstance<IDataFactory<IGeneralOptions, int>>();
        }

        /// <summary>
        /// Converts a <see cref="MobileJourney">MobileJourney</see> to a <see cref="ExpenseItemDefinition">ExpenseItemDefinition</see>, 
        /// which can then be used to create an expense item.
        /// </summary>
        /// <param name="mobileJourney">
        /// The mobile journey.
        /// </param>
        /// <returns>
        /// <returns>A <see cref="ExpenseItemDefinitionResponse">ExpenseItemDefinitionResponse</see> which can be used to generate an expense item</returns>
        /// </returns>
        public ExpenseItemDefinition ReconcileMobileJourney(MobileJourney mobileJourney)
        {
            cJourneyStep[] journeySteps = MobileJourneyParser.GetAddressSuggestionsForMobileJourney(mobileJourney, this._generalOptionsFactory).ToArray();

            Employee employee = ActionContext.Employees.GetEmployeeById(this.User.EmployeeID);
            var subAccounts = new cAccountSubAccounts(this.User.AccountID);
            cAccountSubAccount subAccount = subAccounts.getSubAccountById(employee.DefaultSubAccount);

            var updatedJourneySteps = new SortedList<int, cJourneyStep>();

            foreach (cJourneyStep journeyStep in journeySteps)
            {
                updatedJourneySteps.Add(journeyStep.stepnumber, journeyStep);
            }
               
            var expenseItemRepository = new ExpenseItemRepository(this.User, ActionContext);
            var addressRepository = new AddressRepository(this.User, ActionContext);

            List<Address> homeAndOfficeAddresses = addressRepository.GetHomeAndOfficeAddresses(this.User.EmployeeID);

            List<JourneyStep> stepList = expenseItemRepository.ProcessSteps(0, updatedJourneySteps, 0, homeAndOfficeAddresses);
            ExpenseItemDefinition expenseItemDefinition = new ExpenseItemDefinition();

            // update expense item with the processed journey details
            var expenseItem = new ExpenseItem();

            expenseItem.JourneySteps = stepList;
            expenseItem.Date = mobileJourney.JourneyDateTime;
            expenseItem.ExpenseSubCategoryId = mobileJourney.SubcatId;
            expenseItemDefinition.ExpenseItem = expenseItem;

            return expenseItemDefinition;
        }

        /// <summary>
        /// Get all of T
        /// </summary>
        /// <returns></returns>
        public override IList<JourneyStep> GetAll()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get item with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override JourneyStep Get(int id)
        {
            throw new NotImplementedException();
        }
    }
}
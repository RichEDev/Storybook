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
    using SpendManagementLibrary.Expedite;
    using SpendManagementLibrary.Interfaces.Expedite;

    /// <summary>
    /// FundRepository manages data access for Fund.
    /// </summary> 
    internal class FundRepository : BaseRepository<FundManager>, ISupportsActionContext
    {
        private readonly IManageFunds _data;

        /// <summary>
        /// An instance of <see cref="IActionContext"/>
        /// </summary>
        private readonly IActionContext _actionContext = null;

        /// <summary>
        /// Creates a new FundRepository with the passed in user.
        /// </summary>
        /// <param name="user">The current user.</param>
        /// <param name="actionContext">An implementation of ISupportsActionContext</param>
        public FundRepository(ICurrentUser user, IActionContext actionContext)
            : base(user, actionContext, x => x.Id, x => x.Id.ToString(CultureInfo.InvariantCulture))
        {
            this._data = this.ActionContext.Fund;
        }

        /// <summary>
        /// Creates a new FundRepository
        /// </summary>
        /// <remarks>Used together with the NoAuthorisationRequired attribute</remarks>
        public FundRepository(){}

        /// <summary>
        /// Get all of T
        /// </summary>
        /// <returns></returns>
        public override IList<FundManager> GetAll()
        {
            throw new System.NotImplementedException();
        }
       
        /// <summary>
        /// Gets a fund by the account id.
        /// </summary>
        /// <param name="id">The expedite client account Id</param>
        /// <returns>A fund.</returns>
        public override FundManager Get(int id)
        {
            var funds = new Funds(id);
            var item = funds.GetFundAvailable(id);
            return new FundManager().From(item, this._actionContext);
         
        }

        /// <summary>
        /// Gets a customer fund limit by it's id.
        /// </summary>
        /// <param name="id">The Id</param>
        /// <returns>Customer fund limit.</returns>
        public FundManager GetFund(int id)
        {
            var funds = new Funds(id);
            var item = funds.GetFundLimit(id);
            return new FundManager().From(item, this._actionContext);

        }

        /// <summary>
        /// Update fund limit of customer by it's id.
        /// </summary>
        /// <param name="id">The Id</param>
        /// <param name="amount">The fund limit amount</param>
        /// <returns>Customer fund limit.</returns>
        public FundManager UpdateFundLimit(int id, decimal amount)
        {
            var funds = new Funds(id);
            var item = funds.UpdateFundLimit(id, amount);
            return new FundManager().From(item, this._actionContext);

        }

        /// <summary>
        /// Creates an fund transactiom in the system and returns the transaction id.
        /// </summary>
        /// <param name="item">The item to create.</param>
        /// <returns>The newly added transaction.</returns>
        public override FundManager Add(FundManager item)
        {
            var funds = new Funds(item.AccountId);
            return new FundManager().From(funds.AddFundTransaction(item.To(this._actionContext)), this._actionContext);
        }

    }
       
}

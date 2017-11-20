

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
    /// FundRepository manages data access for Fund.
    /// </summary> 
    internal class FundRepository : BaseRepository<FundManager>, ISupportsActionContext
    {

        private readonly IManageFunds _data;
     
        /// <summary>
        /// Creates a new ReceiptRepository with the passed in user.
        /// </summary>
        /// <param name="user">The current user.</param>
        /// <param name="actionContext">An implementation of ISupportsActionContext</param>
        public FundRepository(ICurrentUser user, IActionContext actionContext)
            : base(user, actionContext, x => x.Id, x => x.Id.ToString(CultureInfo.InvariantCulture))
        {
            _data = ActionContext.Fund;
        }

        /// <summary>
        /// Get all of T
        /// </summary>
        /// <returns></returns>
        public override IList<FundManager> GetAll()
        {
            throw new System.NotImplementedException();
        }
       
        /// <summary>
        /// Gets an Expense Category by it's id.
        /// </summary>
        /// <param name="id">The Id</param>
        /// <returns>An Expense Category.</returns>
        public override FundManager Get(int id)
        {
            var item = _data.GetFundAvailable(id);
            return new FundManager().From(item, ActionContext);
         
        }

        /// <summary>
        /// Gets an customer fund limit by it's id.
        /// </summary>
        /// <param name="id">The Id</param>
        /// <returns>Customer fund limit.</returns>
        public FundManager GetFund(int id)
        {
            var item = _data.GetFundLimit(id);
            return new FundManager().From(item, ActionContext);

        }

        /// <summary>
        /// Update fund limit of customer by it's id.
        /// </summary>
        /// <param name="id">The Id</param>
        /// <returns>Customer fund limit.</returns>
        public FundManager UpdateFundLimit(int id,decimal amount)
        {
            var item = _data.UpdateFundLimit(id,amount);
            return new FundManager().From(item, ActionContext);

        }

        /// <summary>
        /// Creates an fund transactiom in the system and returns the transaction id.
        /// </summary>
        /// <param name="item">The item to create.</param>
        /// <returns>The newly added transaction.</returns>
        public override FundManager Add(FundManager item)
        {
            return new FundManager().From(_data.AddFundTransaction(item.To(ActionContext)), ActionContext);
        }       

    }
       
}

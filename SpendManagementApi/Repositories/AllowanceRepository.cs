using System.Net;
using System.Web.Http;
using Microsoft.Ajax.Utilities;

namespace SpendManagementApi.Repositories
{
    using Interfaces;
    using Models.Common;
    using Models.Types;
    using Spend_Management;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Utilities;

    /// <summary>
    /// AllowanceRepository manages data access for Allowances.
    /// </summary>
    internal class AllowanceRepository : BaseRepository<Allowance>, ISupportsActionContext
    {
        private cAllowances _data;

        /// <summary>
        /// Creates a new AllowanceRepository with the passed in user.
        /// </summary>
        /// <param name="user">The current user.</param>
        /// <param name="actionContext">An implementation of ISupportsActionContext</param>
        public AllowanceRepository(ICurrentUser user, IActionContext actionContext) : base(user, actionContext, x => x.Id, x => x.Label)
        {
            _data = ActionContext.Allowances;
        }

        /// <summary>
        /// Gets all the Allowances within the system.
        /// </summary>
        /// <returns></returns>
        public override IList<Allowance> GetAll()
        {
            _data = new cAllowances(User.AccountID);
            return _data.GetCacheList().Select(b => new Allowance().From(b.Value, ActionContext)).OrderBy(x => x.Label).ToList();
        }

        /// <summary>
        /// Gets a single Allowance by it's id.
        /// </summary>
        /// <param name="id">The id of the Allowance to get.</param>
        /// <returns>The budget holder.</returns>
        public override Allowance Get(int id)
        {
            _data = new cAllowances(User.AccountID);
            var item = _data.getAllowanceById(id);
            if (item == null)
            {
                return null;
            }
            return new Allowance().From(item, ActionContext);
        }

        /// <summary>
        /// Adds a budget holder.
        /// </summary>
        /// <param name="item">The budget holder to add.</param>
        /// <returns></returns>
        public override Allowance Add(Allowance item)
        {
            item = base.Add(item);

            // throw if the currency id doesn't exist.
            var currencies = new cCurrencies(User.AccountID, User.CurrentSubAccountId);
            if (currencies.getCurrencyById(item.CurrencyId) == null)
            {
                throw new ApiException(ApiResources.ApiErrorUpdateObjectWithWrongId,
                    ApiResources.ApiErrorWrongCurrencyIdMessage);
            }


            // grab rates.
            var rates = item.AllowanceRates ?? new List<AllowanceRate>();
            item.AllowanceRates = new List<AllowanceRate>();
            var allowanceId = _data.saveAllowance(item.To(ActionContext));

            if (allowanceId == -1)
            {
                throw new ApiException(ApiResources.ApiErrorRecordAlreadyExists,
                        ApiResources.ApiErrorRecordAlreadyExistsMessage);
            }

            if (allowanceId < 1)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful,
                        ApiResources.ApiErrorSaveUnsuccessfulMessage);
            }

            // get the added item
            var result = Get(allowanceId);

            if (result == null)
            {
                throw new Exception("CACHE FAIL");
            }

            
            // attempt to save the rates.
            foreach (var rate in rates)
            {
                rate.AllowanceId = allowanceId;
                var rateId = _data.saveRate(rate.To(ActionContext));

                if (rateId == -1)
                {
                    throw new ApiException(ApiResources.ApiErrorRecordAlreadyExists,
                            ApiResources.ApiErrorRecordAlreadyExistsMessage + ApiResources.ApiErrorAllowanceRateAdditionFailure);
                }

                if (rateId < 1)
                {
                    throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful,
                            ApiResources.ApiErrorSaveUnsuccessfulMessage + ApiResources.ApiErrorAllowanceRateAdditionFailure);
                }

                // reassign
                rate.Id = rateId;
                result.AllowanceRates.Add(rate);
            }

            return result;
        }

        /// <summary>
        /// Updates a budget holder.
        /// </summary>
        /// <param name="item">The item to update.</param>
        /// <returns>The updated Allowance.</returns>
        public override Allowance Update(Allowance item)
        {
            item = base.Update(item);

            // attempt to get the existing one.
            var dbItem = Get(item.Id);
            if (dbItem == null)
            {
                throw new ApiException(ApiResources.ApiErrorRecordAlreadyExists,
                        ApiResources.ApiErrorRecordAlreadyExistsMessage);
            }
            
            // throw if the currency id doesn't exist.
            var currencies = new cCurrencies(User.AccountID, User.CurrentSubAccountId);
            if (currencies.getCurrencyById(item.CurrencyId) == null)
            {
                throw new ApiException(ApiResources.ApiErrorUpdateObjectWithWrongId, 
                    ApiResources.ApiErrorWrongCurrencyIdMessage);
            }


            // update the allowance rates.
            var toAdd = new List<int>();
            var toKeep = new List<int>();

            // loop through all the rates in the item
            foreach (var rate in item.AllowanceRates)
            {
                // make sure the allowance rate is added.
                rate.AllowanceId = item.Id;

                var match = false;
                // add any that have equivalents
                if (dbItem.AllowanceRates.Any(dbRate => rate.Id == dbRate.Id))
                {
                    match = true;
                    toKeep.Add(rate.Id);
                }

                // the rate must not exist, so mark it for addition.
                if (!match)
                {
                    toAdd.Add(rate.Id);
                }
            }

            // the ones to remove are the ones that aren't added or kept.
            var toGo = (from dbRate in dbItem.AllowanceRates where !toKeep.Contains(dbRate.Id) && !toAdd.Contains(dbRate.Id) select dbRate.Id).ToList();

            // remove any toGo  allowances from db (they wont be in item)
            foreach (var i in toGo)
            {
                _data.deleteAllowanceBreakdown(i);
            }

            // update any for keeps
            if (toKeep.Select(i => item.AllowanceRates.Find(r => r.Id == i)).Select(rate => _data.saveRate(rate.To(ActionContext))).Any(result => result < 1))
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful,
                    ApiResources.ApiErrorAllowanceRateAdditionFailure);
            }

            // add any new
            if (toAdd.Select(i => item.AllowanceRates.Find(r => r.Id == i)).Select(rate => rate.Id = _data.saveRate(rate.To(ActionContext))).Any(result => result < 1))
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful,
                    ApiResources.ApiErrorAllowanceRateAdditionFailure);
            }

            // now save the allowance.
            var id = _data.saveAllowance(item.To(ActionContext));
            if (id == 1)
            {
                throw new ApiException(ApiResources.ApiErrorRecordAlreadyExists,
                        ApiResources.ApiErrorRecordAlreadyExistsMessage);
            }

            return Get(item.Id);
        }

        /// <summary>
        /// Deletes a budget holder, given it's ID.
        /// </summary>
        /// <param name="id">The Id of the Allowance to delete.</param>
        /// <returns>The deleted budget holder.</returns>
        public override Allowance Delete(int id)
        {
            var item = base.Delete(id);

            var result = _data.deleteAllowance(item.Id);
            if (result != 0)
            {
                throw new ApiException(ApiResources.ApiErrorDeleteUnsuccessful,
                    ApiResources.ApiErrorDeleteUnsuccessfulMessage);
            }

            return item;
        }
    }
}
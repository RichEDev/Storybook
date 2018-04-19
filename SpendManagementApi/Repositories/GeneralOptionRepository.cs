namespace SpendManagementApi.Repositories
{  
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    using BusinessLogic.AccountProperties;
    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;

    using Attributes;
    using Interfaces;
    using Models.Common;
    using Models.Types;
    using Utilities;

    using Spend_Management;

    using SpendManagementLibrary;

    internal class GeneralOptionRepository : BaseRepository<GeneralOption>, ISupportsActionContext
    {
        private readonly IDataFactory<IAccountProperty, AccountPropertyCacheKey> _accountPropertiesFactory;

        /// <summary>
        /// Gets all of the available end points from the <see cref="GeneralOption">GeneralOptions</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        public GeneralOptionRepository(ICurrentUser user, IActionContext actionContext)
            : base(user, actionContext, x => x.SubAccountId, null)
        {
            this._accountPropertiesFactory = WebApiApplication.container.GetInstance<IDataFactory<IAccountProperty, AccountPropertyCacheKey>>();
        }

        /// <summary>
        /// Gets all <see cref="GeneralOption">GeneralOptions</see> in the system.
        /// </summary>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.GeneralOptions, AccessRoleType.View)]
        public override IList<GeneralOption> GetAll()
        {
            var returnList = this._accountPropertiesFactory.Get().Where(accountProperty => accountProperty.SubAccountId == this.User.CurrentSubAccountId).ToList();
            return returnList.Select(accountProperty => new GeneralOption().From(accountProperty)).ToList();
        }

        /// <summary>
        /// Get item with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The <see cref="GeneralOption">GeneralOption</see></returns>
        public override GeneralOption Get(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all general options for a particular sub account id.
        /// </summary>
        /// <param name="subAccountId">The sub account id.</param>
        /// <returns>All general options for the sub account.</returns>
        internal List<GeneralOption> GetAllBySubAccount(int subAccountId)
        {
            var returnList = this._accountPropertiesFactory.Get().Where(accountProperty => accountProperty.SubAccountId == this.User.CurrentSubAccountId).ToList();
            return returnList.Select(accountProperty => new GeneralOption().From(accountProperty)).ToList();
        }

        internal GeneralOption GetByKeyAndSubAccount(string key, int subAccountId)
        {
            var accountProperty = this._accountPropertiesFactory[new AccountPropertyCacheKey(key, subAccountId.ToString())];

            if (accountProperty == null)
            {
                throw new ApiException(ApiResources.ApiErrorGeneralOptionDoesntExist,
                    String.Format(ApiResources.ApiErrorGeneralOptionDoesntExistMessageKeySubAccount));
            }
            return new GeneralOption().From(accountProperty);
        }

        /// <summary>
        /// Gets a single generalOption by it's key.
        /// </summary>
        /// <param name="key">The key of the GeneralOption to get.</param>
        /// <returns>The generalOption</returns>
        public GeneralOption GetByKey(string key)
        {
            return this.GetByKeyAndSubAccount(key, User.CurrentSubAccountId);
        }

        /// <summary>
        /// Updates a <see cref="GeneralOption"/>.
        /// </summary>
        /// <param name="item">The <see cref="GeneralOption"/> to update.</param>
        /// <returns>The updated <see cref="GeneralOption"/>.</returns>
        public override GeneralOption Update(GeneralOption item)
        {
            // validates the key to ensure it is valid.
            this.GetByKey(item.Key);

            item = base.Update(item);
            var value = this._accountPropertiesFactory.Save(item.To());

            return this.GetByKey(value.Key);
        }


        /// <summary>
        /// Updates multiple<see cref="GeneralOption"/>.
        /// </summary>
        /// <param name="generalOptions">The list of<see cref="GeneralOption"/> to update.</param>
        /// <returns>The updated list of<see cref="GeneralOption"/>.</returns>
        public List<GeneralOption> UpdateMultipleGeneralOptions(List<GeneralOption> generalOptions)
        {
            // Get Sub Account ID once
            var subAccountId = generalOptions.FirstOrDefault().SubAccountId;

            foreach (var generalOption in generalOptions)
            {
                this.GetByKey(generalOption.Key);
                generalOption.SubAccountId = subAccountId;

                this._accountPropertiesFactory.Save(generalOption.To());
            }

            // Return all updated general options
            return generalOptions;
        }

        /// <summary>
        /// Saves the <see cref="GeneralOptionsDisplayFieldSetting"/> that can be enabled/disabled in general options to display specific fields when claiming for an expense
        /// </summary>
        /// <param name="fieldSetting">
        /// The field Setting to be saved
        /// </param>
        /// <returns>
        /// The <see cref="GeneralOptionsDisplayFieldSetting"/>.
        /// </returns>
        public GeneralOptionsDisplayFieldSetting SaveDisplayFieldSetting(GeneralOptionsDisplayFieldSetting fieldSetting)
        {
            List<cFieldToDisplay> lstFieldsToDisplay = new List<cFieldToDisplay> { fieldSetting.ToBaseClass(this.ActionContext) };

            this.ActionContext.Misc.UpdateFieldsToDisplay(lstFieldsToDisplay);
            return fieldSetting;
        }

    }
}

namespace SpendManagementApi.Repositories
{  
    using Spend_Management;
    using Attributes;
    using Interfaces;
    using Models.Common;
    using Models.Types;
    using Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    using SpendManagementLibrary;

    using GeneralOptions = SpendManagementLibrary.GeneralOptions.GeneralOptions;

    internal class GeneralOptionRepository : BaseRepository<GeneralOption>, ISupportsActionContext
    {
        private GeneralOptions _data;

        /// <summary>
        /// Gets all of the available end points from the <see cref="GeneralOption">GeneralOptions</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        public GeneralOptionRepository(ICurrentUser user, IActionContext actionContext)
            : base(user, actionContext, x => x.SubAccountId, null)
        {
            _data = ActionContext.GeneralOptions;
        }

        /// <summary>
        /// Gets all <see cref="GeneralOption">GeneralOptions</see> in the system.
        /// </summary>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.GeneralOptions, AccessRoleType.View)]
        public override IList<GeneralOption> GetAll()
        {
            return _data.GetList(this.User.CurrentSubAccountId).Select(b => new GeneralOption().From(b, ActionContext)).ToList();
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
            return _data.GetList(subAccountId).Select(b => new GeneralOption().From(b, ActionContext)).Where(account => account.SubAccountId == subAccountId).ToList();
        }

        internal GeneralOption GetByKeyAndSubAccount(string key, int subAccountId)
        {
            _data = new GeneralOptions(User.AccountID);
            var item = _data.GetGeneralOptionByKeyAndSubAccount(key, subAccountId);

            if (item == null)
            {
                throw new ApiException(ApiResources.ApiErrorGeneralOptionDoesntExist,
                    String.Format(ApiResources.ApiErrorGeneralOptionDoesntExistMessageKeySubAccount));
            }
            return new GeneralOption().From(item, ActionContext);
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
            this.UpdateGeneralOption(item.SubAccountId,item.Key,item.Value);
            return this.GetByKey(item.Key);
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
            var subAccountProperties = new Dictionary<string, string>();

            foreach (var generalOption in generalOptions)
            {
                this.GetByKey(generalOption.Key);
                subAccountProperties.Add(generalOption.Key, generalOption.Value);
            }

            var subAccounts = new cAccountSubAccounts(this.User.AccountID);
            subAccounts.SaveProperties(subAccountId, subAccountProperties, this.User.EmployeeID, null);
            subAccounts.InvalidateCache(subAccountId);

            // Return all updated general options
            return generalOptions;

        }

        /// <summary>
        /// The updates <see cref="GeneralOption"/> in the database
        /// </summary>
        /// <param name="subAccountId">
        /// The sub account.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        private void UpdateGeneralOption(int subAccountId, string key , string value)
        {
            var subAccounts = new cAccountSubAccounts(this.User.AccountID);      
            var subAccountProperties = new Dictionary<string, string> { { key, value } };
            subAccounts.SaveProperties(subAccountId, subAccountProperties, this.User.EmployeeID, null);
            subAccounts.InvalidateCache(subAccountId);
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

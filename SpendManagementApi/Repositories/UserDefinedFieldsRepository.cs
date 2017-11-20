namespace SpendManagementApi.Repositories
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Script.Serialization;

    using Interfaces;
    using Models.Common;
    using Models.Types;

    using SpendManagementApi.Models.Requests;
    using SpendManagementApi.Models.Types.Employees;

    using SpendManagementLibrary;
    using Spend_Management;
    using Currency = Models.Types.Currency;
    using CurrencyType = Common.Enums.CurrencyType;
    using Utilities;

    internal class UserDefinedFieldsRepository : BaseRepository<UserDefinedField>, ISupportsActionContext
    {
        private readonly cUserdefinedFields _userdefinedFields;

        public UserDefinedFieldsRepository(ICurrentUser user, IActionContext actionContext = null)
            : base(user, actionContext, udf => udf.UserDefinedId, null)
        {
            ActionContext.SubAccountId = User.CurrentSubAccountId;
            _userdefinedFields = this.ActionContext.UserDefinedFields;
        }

        //Returns the list of user defined functions
        public override IList<UserDefinedField> GetAll()
        {
            return this.ActionContext.UserDefinedFields.UserdefinedFields.Values.Select(udf => udf.Cast<UserDefinedField>(this.ActionContext)).ToList();
        }

        public override UserDefinedField Get(int udfId)
        {
            return this.ActionContext.UserDefinedFields.GetUserDefinedById(udfId).Cast<UserDefinedField>(this.ActionContext);
        }

        public override UserDefinedField Add(UserDefinedField userDefinedField)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see cref="UserDefinedField">UserDefinedField</see> it's Id
        /// </summary>
        /// <param name="userDefinedFieldId">
        /// The user defined field id.
        /// </param>
        /// <returns>
        /// The <see cref="UserDefinedField">UserDefinedField</see> which will be null if successful.
        /// </returns>
        public override UserDefinedField Delete(int userDefinedFieldId)
        {
            this.ActionContext.UserDefinedFields.DeleteUserDefined(userDefinedFieldId);
            return this.Get(userDefinedFieldId);
        }

        /// <summary>
        /// Only updates the archive status of the record
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public override UserDefinedField Update(UserDefinedField userDefinedField)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves a user defined field and returns an instance of the newly created user defined field
        /// </summary>
        /// <param name="userDefinedField">
        /// The <see cref="UserDefinedFieldRequest">UserDefinedFieldRequest</see>
        /// </param>
        /// <returns>
        /// The <see cref="UserDefinedField">UserDefinedField</see>
        /// </returns>
        /// <exception cref="ApiException">
        /// </exception>
        public UserDefinedField SaveUserDefinedField(UserDefinedFieldRequest userDefinedField)
        {
            int udfId = this.Save(userDefinedField);

            if (udfId == -1)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.UdfLabelAlreadyExists);
            }

            //re-insitalise object to update cache.
            var userDefinedFields = new cUserdefinedFields(User.AccountID);

            return userDefinedFields.GetUserDefinedById(udfId).Cast<UserDefinedField>(this.ActionContext);
        }

        /// <summary>
        /// Saves a user defined field to the DB.
        /// </summary>
        /// <param name="item">
        /// The <see cref="UserDefinedFieldRequest">UserDefinedFieldRequest</see>.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> of the saved user defined field
        /// </returns>
        private int Save(UserDefinedFieldRequest item)
        {
            cTables tables = new cTables(User.AccountID);
            cUserdefinedFieldGroupings groupings = new cUserdefinedFieldGroupings(User.AccountID);

            var elements = item.ListItems.Select(element => new JavaScriptSerializer().Serialize(element)).ToList();

            var assignmentId = ActionContext.UserDefinedFields.SaveUserDefinedField(
                  item.UserDefinedFieldId,
                  item.DisplayName,
                  item.Description,
                  item.Tooltip,
                  (byte)item.FieldType,
                  0,
                  item.Order,
                  (byte)item.Format,
                  item.ItemSpecific,
                  item.AllowSearch,
                  item.AppliesToTableId.ToString(),
                  item.HyperlinkText,
                  item.HyperlinkPath,
                  "0",
                  item.Mandatory,
                  null,
                  null,
                  item.MaxLength.Value,
                  item.Precision.Value,
                  elements.ToArray(),
                  item.Default,
                  item.MaxLength.Value,
                  item.AllowEmployeeToPopulate,
                  User,
                  tables,
                  groupings);

            return assignmentId;
        }
    }
}
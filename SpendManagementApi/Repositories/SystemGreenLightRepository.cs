namespace SpendManagementApi.Repositories
{
    using System;
    using Spend_Management;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Common;
    using System.Configuration;
    using SpendManagementLibrary;
    using SpendManagementApi.Models.Requests;
    using SpendManagementApi.Utilities;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    /// <summary>
    /// Manages to copy greenlight.
    /// </summary>
    public class SystemGreenLightRepository
    {

        /// <summary>
        /// Logic of coping system greenlight from source database to targetdatabase.
        /// </summary>
        /// <param name="greenLightEntity">System greenlight entity to copy</param>
        /// <param name="currentUser">Login user</param>
        /// <param name="request">Request message</param>
        /// <returns> Response message</returns>
        public SystemGreenLightResponse CopyCustomEntity(ICurrentUser currentUser, CustomEntityToCopy greenLightEntity, HttpRequestMessage request)
        {
            var response = new SystemGreenLightResponse();
            var apiResponseInformation = new ApiResponseInformation();
            int sourceAccountId = Convert.ToInt32(ConfigurationManager.AppSettings["SystemGreenLightSourceAccountId"]);
            string adminUsername = ConfigurationManager.AppSettings["SystemGreenLightAdminUserName"];

            if (currentUser.Employee.Username != adminUsername || currentUser.AccountID != sourceAccountId)
            {
                throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.Unauthorized, ApiResources.ResponseForbidddenUnAuthorisedUser));
            }
            var targetAccount = new cAccounts().GetAccountByID(greenLightEntity.TargetAccountId);


            if (sourceAccountId == 0) { throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.BadRequest, "Source account is not specified.")); }
            if (string.IsNullOrEmpty(adminUsername)) { throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.BadRequest, "Username is not specified.")); }

            //Get custom entities from the source database.
            var customEntity = new cCustomEntities(currentUser).getEntityById(greenLightEntity.EntityId);
         
            if (customEntity == null && targetAccount == null)
            {
                throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid custom entity and target account."));
            }
            if (targetAccount == null)
            {
                throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid target account."));
            }
            if (customEntity == null)
            {
                throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid custom entity."));
            }

            if (customEntity.DefaultCurrencyID != null)
            {
                var currency = new cCurrencies(targetAccount.accountid, null).getCurrencyById((int)customEntity.DefaultCurrencyID);
                if (currency == null)
                {
                    throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.NotFound, ApiResources.ResponseCurrencyNotExists));
                }
            }

            if (!customEntity.BuiltIn)
            {
                throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.Forbidden, ApiResources.ResponseSystemGreenLightOnly));
            }
            int authorisedEmployeeIdFromTargetDatabase = new cEmployees(greenLightEntity.TargetAccountId).getEmployeeidByUsername(greenLightEntity.TargetAccountId, adminUsername);

            //Get user from target database.
            var targetUser = cMisc.GetCurrentUser(greenLightEntity.TargetAccountId + "," + authorisedEmployeeIdFromTargetDatabase);
            var targetCustomEntities = new cCustomEntities(targetUser);

            int targetEntityId = targetCustomEntities.GetEntityIdByName(customEntity.entityname, customEntity.pluralname, customEntity.BuiltIn);
            var returnValue = targetCustomEntities.saveEntity(new cCustomEntity(targetEntityId, customEntity.entityname, customEntity.pluralname, customEntity.description, DateTime.Now, authorisedEmployeeIdFromTargetDatabase, DateTime.Now, authorisedEmployeeIdFromTargetDatabase, new SortedList<int, cAttribute>(), new SortedList<int, cCustomEntityForm>(), new SortedList<int, cCustomEntityView>(), customEntity.table, customEntity.AudienceTable, customEntity.EnableAttachments, customEntity.AudienceView, customEntity.AllowMergeConfigAccess, false, null, null, customEntity.EnableCurrencies, customEntity.DefaultCurrencyID, customEntity.EnablePopupWindow, null, customEntity.FormSelectionAttributeId, null, null, customEntity.SupportQuestion, customEntity.EnableLocking, customEntity.BuiltIn));
            switch (returnValue)
            {
                case -1:
                    throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.Forbidden, ApiResources.ResponseGreenLightNameExists));

                case -2:
                    throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.Forbidden, ApiResources.ResponseGreenLightPluralNameExists));

                case -3:
                    throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.Forbidden, ApiResources.ResponseGreenLightNameAndPluralNameExists));

                default:
                    if (returnValue > 0)
                    {
                        response.ResponseMessage = targetEntityId > 0
                                                       ? string.Format(ApiResources.ResponseGreenLightUpdatedSuccessfully, customEntity.entityname, targetAccount.companyid)
                                                       : string.Format(ApiResources.ResponseGreenLightCopiedSuccessfully, customEntity.entityname, targetAccount.companyid);
                        apiResponseInformation.Status = ApiStatusCode.Success;
                    }
                    else
                    {
                        response.ResponseMessage = ApiResources.ResponseFailedToCopyGreenLight;
                        apiResponseInformation.Status = ApiStatusCode.Failure;
                    }

                    break;
            }
            response.ResponseInformation = apiResponseInformation;
            return response;
        }
    }
}
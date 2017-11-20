namespace SpendManagementApi.Controllers.V1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using SpendManagementApi.Attributes;
    using SpendManagementLibrary.Mobile;
    using SpendManagementLibrary;
    using Spend_Management;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Requests;
    using System.Globalization;
    using System.IO;

    using SpendManagementApi.Common;
    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Utilities;
    using SpendManagementLibrary.Mobile;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Repositories;

    /// <summary>
    /// The controller handling journeys from mobile devices.
    /// </summary> 
    [Version(1)]
    [RoutePrefix("Journeys")]
    public class JourneysV1Controller : BaseApiController<JourneyStep>
    {

 
        /// <summary>
        /// Saves journeys for a mobile user.
        /// </summary>
        /// <param name="journeys">A list of <see cref="MobileJourney">journeys</see> to save.</param>
        /// <returns>A <see cref="SaveJourneyResult"></see> detailing success or failure.</returns>
        [Route("SaveJourneys")]
        [HttpPost]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public SaveJourneyResult SaveJourneys([FromBody] List<MobileJourney> journeys)
        {
            var result = new SaveJourneyResult() { FunctionName = "SaveJourneys" };
            try
            {
                if (journeys == null)
                {
                    result.ReturnCode = MobileReturnCode.InvalidJourneys;
                }
                else
                {
                    var mobileDevice = new cMobileDevices(this.CurrentUser.AccountID);
                    var serverIds = new SortedList<string, int>();

                    foreach (var journey in journeys)
                    {
                        if (!journey.Active)
                        {
                            //Validate subcat for the finished journey
                            var subcats = new cSubcats(this.CurrentUser.AccountID);
                            bool isValidForMileage = subcats.ValidateSubcatForMileage(journey.SubcatId, this.CurrentUser.EmployeeID);

                            if (!isValidForMileage)
                            {
                                result.ReturnCode = MobileReturnCode.InvalidDefaultMilageItem;
                                return result;
                            }
                        }
                      
                            var id = mobileDevice.SaveMobileJourney(
                                 CurrentUser.EmployeeID,
                                 journey.SubcatId,
                                 journey.JourneyJson,
                                 0,
                                 journey.JourneyDateTime,
                                 journey.JourneyStartTime,
                                 journey.JourneyEndTime,
                                 journey.Active);
                            serverIds.Add(journey.JourneyId.ToString(), id);
                        }
                        result.List = serverIds;                  
                }
            }
            catch (Exception)
            {
                throw new ApiException(ApiResources.InvalidJourney, ApiResources.InvalidJourneyMessage);
            }
            return result;
        }

        /// <summary>
        /// Update journeys for a mobile user.
        /// </summary>
        /// <param name="journeys">A list of <see cref="MobileJourney">journeys</see> to update.</param>
        /// <returns>A <see cref="SaveJourneyResult"></see> detailing success or failure.</returns>
        [Route("UpdateJourneys")]
        [HttpPut]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public SaveJourneyResult UpdateJourneys([FromBody] List<MobileJourney> journeys)
        {
            var result = new SaveJourneyResult();
            try
            {
                if (journeys == null)
                {
                    result.ReturnCode = MobileReturnCode.InvalidJourneys;
                }
                else
                {
                    var accountId = this.CurrentUser.AccountID;
                    var mobileDevice = new cMobileDevices(accountId);
                    foreach (var journey in journeys)
                    {
                        mobileDevice.UpdateMobileJourney(
                            journey.JourneyId,
                            journey.JourneyJson,
                            journey.Active
                            );
                    }
                }  
            }
            catch (Exception)
            {
                throw new ApiException(ApiResources.InvalidJourney, ApiResources.InvalidJourneyMessage);
            }                     
            return result;
        }

        /// <summary>
        /// Gets all active journeys for an Employee
        /// </summary>
        /// <returns>A list of <see cref="MobileJourneyResponse">MobileJourneyResponse</see>.</returns>
        [Route("GetEmployeeActiveJourneys")]
        [HttpGet]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public MobileJourneyResponse GetUsersActiveJourneys()
        {
            try
            {
                var accountId = this.CurrentUser.AccountID;
                var mobileDevice = new cMobileDevices(accountId);
                var response = InitialiseResponse<MobileJourneyResponse>();
          
                response.List = mobileDevice.GetEmployeeActiveJourneys(this.CurrentUser.EmployeeID);;
                return response;      
            }
            catch (Exception)
            {
                throw new ApiException(string.Format(ApiResources.ApiErrorXUnsuccessfulMessage, "Get Employee's Active Journeys"),
                 (ApiResources.ApiErrorGetEmployeeJourneysMessage));   
           
            }                     
        }

        /// <summary>
        /// Deletes a mobile journey
        /// </summary>
        /// <param name="journeyId">The journeyId</param>
        /// <returns>A <see cref="NumericResponse">NumericResponse</see> with the outcome of the delete action</returns>
        [Route("DeleteMobileJourney")]
        [HttpDelete]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public NumericResponse DeleteMobileJourney(int journeyId)
        {
            const string message = "Deletion of Mobile Journey";
            var accountId = this.CurrentUser.AccountID;
            var mobileDevice = new cMobileDevices(accountId);
            var response = InitialiseResponse<NumericResponse>();

            if (!mobileDevice.DoesJourneyBelongToEmployee(journeyId, this.CurrentUser.EmployeeID))
            {
                throw new ApiException(string.Format(ApiResources.ApiErrorXUnsuccessfulMessage, message),
                (ApiResources.ApiErrorJourneyDoesNotBelongToEmployee));

            }

            try
            {             
                response.Item = mobileDevice.DeleteMobileJourney(journeyId) ? 1 : 0;
                
                return response;
            }
            catch (Exception)
            {
                throw new ApiException(string.Format(ApiResources.ApiErrorXUnsuccessfulMessage, message),
                 (ApiResources.ApiErrorGetEmployeeJourneysMessage));
            }

        }

        /// <summary>
        /// Converts a <see cref="MobileJourney">MobileJourney</see> to a <see cref="ExpenseItemDefinition">ExpenseItemDefinition</see>, 
        /// which can then be used to create an expense item.
        /// </summary>
        /// <param name="mobileJourney">A <see cref="MobileJourney">MobileJourney</see> to reconcile</param>
        /// <returns>A <see cref="ExpenseItemDefinitionResponse">ExpenseItemDefinitionResponse</see> which can be used to generate an expense item</returns>
        [Route("ReconcileMobileJourney")]
        [HttpPost]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public ExpenseItemDefinitionResponse ReconcileMobileJourney([FromBody] MobileJourney mobileJourney)
        {
            var response = this.InitialiseResponse<ExpenseItemDefinitionResponse>();
           response.Item = ((JourneyRepository)this.Repository).ReconcileMobileJourney(mobileJourney);
            return response;
        }


    }
}
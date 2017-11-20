namespace Spend_Management.shared.webServices
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Web.Script.Services;
    using System.Web.Services;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Exceptions;

    /// <summary>
    /// Summary description for svcVehicleEngineType
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [ScriptService]
    public class svcVehicleEngineType : WebService
    {
        public class Response
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public string[] Controls { get; set; }
        }
        
        /// <summary>
        /// Save a Vehicle engine type to the database
        /// </summary>
        /// <param name="vehicleEngineType">The Vehicle engine type data to save</param>
        /// <returns>An object which contains true on success or a message on error</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public Response Save(VehicleEngineType vehicleEngineType)
        {
            try
            {
                CurrentUser user = cMisc.GetCurrentUser();

                if (!svcVehicleEngineType.CurrentUserHasPermission(user))
                {
                    return new Response { Message = "You do not have permission to save a Vehicle engine type." };
                }

                vehicleEngineType.Save(user);

                return new Response { Success = true };
            }
            catch (ValidationException ex)
            {
                return new Response { Message = ex.Message, Controls = new[] { ex.Field } };
            }
            catch (Exception ex)
            {
                return new Response { Message = String.Format("Error: {0}, {1}", ex.GetType().Name, ex.Message) };
            }
        }

        /// <summary>
        /// Delete a Vehicle engine type from the database
        /// </summary>
        /// <param name="vehicleEngineTypeId">The Vehicle engine type ID to delete</param>
        /// <returns>An object which contains true on success or a message on error</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public Response Delete(int vehicleEngineTypeId)
        {
            try
            {
                CurrentUser user = cMisc.GetCurrentUser();

                if (!svcVehicleEngineType.CurrentUserHasPermission(user))
                {
                    return new Response { Message = "You do not have permission to delete a Vehicle engine type." };
                }

                if (user.Account.IsNHSCustomer)
                {
                    var vet = VehicleEngineType.Get(user, vehicleEngineTypeId);
                    if (vet == null)
                    {
                        return new Response { Success = true };
                    }

                    if (VehicleEngineType.EsrReservedCodes.Contains(vet.Code, StringComparer.InvariantCultureIgnoreCase))
                    {
                        return new Response { Message = "You cannot delete this Vehicle engine type as it is used by ESR." };
                    }
                }

                if (VehicleJourneyRateThresholdRate.GetAll(user).Any(r => r.VehicleEngineTypeId == vehicleEngineTypeId))
                {
                    return new Response { Message = "You cannot delete this Vehicle engine type as it is in use by Vehicle journey rates." };
                }

                VehicleEngineType.Delete(user, vehicleEngineTypeId);

                return new Response { Success = true };
            }
            catch (Exception ex)
            {
                return new Response { Message = String.Format("Error: {0}, {1}", ex.GetType().Name, ex.Message) };
            }
        }

        private static bool CurrentUserHasPermission(ICurrentUser user)
        {
            return (user != null && user.Account != null &&
                    user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.VehicleEngineType, true));
        }

    }
}

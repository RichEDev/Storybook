namespace SpendManagementApi.Controllers.Mobile.V1
{
    using System;
    using System.Linq;
    using System.Web.Http;
    using System.Web.Http.Description;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Attributes.Mobile;

    using SpendManagementLibrary;

    using Spend_Management;

    using AllowanceTypesResult = SpendManagementLibrary.Mobile.AllowanceTypesResult;
    using cAPIAllowance = SpendManagementLibrary.Mobile.cAPIAllowance;

    /// <summary>
    /// The controller for handling allowances.
    /// </summary>
    [Version(1)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MobileAllowancesV1Controller : BaseMobileApiController
    {
        /// <summary>
        /// Gets the allowances for the mobile user
        /// </summary>
        /// <returns>The mobile users allowances.</returns>
        [HttpGet]
        [MobileAuth]
        [Route("mobile/allowances")]
        public AllowanceTypesResult Get()
        {
            AllowanceTypesResult result = new AllowanceTypesResult { FunctionName = "GetAllowanceTypes", ReturnCode = this.ServiceResultMessage.ReturnCode };

            if (this.ServiceResultMessage.ReturnCode == MobileReturnCode.Success)
            {
                try
                {
                    cAllowances allowances = new cAllowances(this.PairingKeySerialKey.PairingKey.AccountID);

                    result.AllowanceTypes = (from x in allowances.sortList().Values
                                             select new cAPIAllowance { AllowanceID = x.allowanceid, Allowance = x.allowance, Description = x.description }).ToList();
                }
                catch (Exception ex)
                {
                    cEventlog.LogEntry("MobileAPI.GetAllowanceTypes():Error:{ Pairingkey: " + this.PairingKeySerialKey.PairingKey + "\nSerialKey: " + this.PairingKeySerialKey.SerialKey + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                    // ReSharper disable once PossibleIntendedRethrow
                    throw ex;
                }
            }

            return result;
        }
    }
}
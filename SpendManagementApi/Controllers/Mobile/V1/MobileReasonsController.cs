namespace SpendManagementApi.Controllers.Mobile.V1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using System.Web.Http.Description;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Attributes.Mobile;

    using SpendManagementLibrary;

    using Spend_Management;

    using Reason = SpendManagementLibrary.Mobile.Reason;
    using ReasonResult = SpendManagementLibrary.Mobile.ReasonResult;

    /// <summary>
    /// The controller for handling reasons.
    /// </summary>
    [Version(1)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MobileReasonsV1Controller : BaseMobileApiController
    {
        /// <summary>
        /// Gets the reasons for the mobile user.
        /// </summary>
        /// <returns>A list of all reasons wrapped in a <see cref="ReasonResult"/></returns>
        [HttpGet]
        [MobileAuth]
        [Route("mobile/reasons")]
        public ReasonResult GetReasonsList()
        {
            ReasonResult result = new ReasonResult { FunctionName = "GetReasonsList", ReturnCode = this.ServiceResultMessage.ReturnCode };

            if (this.ServiceResultMessage.ReturnCode == MobileReturnCode.Success)
            {
                try
                {
                    cReasons clsReasons = new cReasons(this.PairingKeySerialKey.PairingKey.AccountID);
                    List<Reason> reasons =
                        clsReasons.CachedList()
                            .Select(
                                r =>
                                new Reason
                                    {
                                        accountcodenovat = r.accountcodenovat,
                                        accountcodevat = r.accountcodevat,
                                        accountid = r.accountid,
                                        createdby = r.createdby,
                                        createdon = r.createdon,
                                        description = r.description,
                                        modifiedby = r.modifiedby,
                                        modifiedon = r.modifiedon,
                                        reason = r.reason,
                                        reasonid = r.reasonid
                                    })
                            .ToList();

                    result.List = reasons;
                }
                catch (Exception ex)
                {
                    cEventlog.LogEntry(
                        "MobileAPI.GetReasonsList():Error:{ Pairingkey: " + this.PairingKeySerialKey.PairingKey
                        + "\nSerialKey: " + this.PairingKeySerialKey.SerialKey + "\nMessage: " + ex.Message + " }",
                        true,
                        System.Diagnostics.EventLogEntryType.Information,
                        cEventlog.ErrorCode.DebugInformation);

                    // ReSharper disable PossibleIntendedRethrow
                    throw ex;
                    // ReSharper restore PossibleIntendedRethrow
                }
            }

            return result;
        }
    }
}

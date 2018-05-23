namespace SpendManagementApi.Controllers.Mobile.V1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using System.Web.Http.Description;

    using BusinessLogic.DataConnections;
    using BusinessLogic.Reasons;

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
        private readonly IDataFactoryArchivable<IReason, int, int> _reasonFactory = FunkyInjector.Container.GetInstance<IDataFactoryArchivable<IReason, int, int>>();

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
                    var accountId = this.PairingKeySerialKey.PairingKey.AccountID;

                    var newReasons = this._reasonFactory.Get().Select(
                        r => new Reason
                                 {
                                     accountcodenovat = r.AccountCodeVat,
                                     accountcodevat = r.AccountCodeVat,
                                     accountid = accountId,
                                     createdby = r.CreatedBy,
                                     createdon = r.CreatedOn,
                                     description = r.Description,
                                     modifiedby = r.ModifiedBy,
                                     modifiedon = r.ModifiedOn,
                                     reason = r.Name,
                                     reasonid = r.Id
                                 }).ToList();

                    result.List = newReasons;
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

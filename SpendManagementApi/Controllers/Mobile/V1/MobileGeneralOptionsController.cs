namespace SpendManagementApi.Controllers.Mobile.V1
{
    using System;
    using System.Web.Http;
    using System.Web.Http.Description;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Attributes.Mobile;

    using SpendManagementLibrary;

    using Spend_Management;

    using GeneralOptions = SpendManagementLibrary.Mobile.GeneralOptions;

    /// <summary>
    /// The controller to handle all <see cref="cAccountProperties">cAccountProperties</see> for mobile users.
    /// </summary>
    [Version(1)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MobileGeneralOptionsV1Controller : BaseMobileApiController
    {
        /// <summary>
        /// Gets the general options applicable to the mobile user.
        /// </summary>
        /// <returns>The account properties that mobile users need to know.</returns>
        [HttpGet]
        [MobileAuth]
        [Route("mobile/generaloptions")]
        public GeneralOptions GetGeneralOptions()
        {
            GeneralOptions generalOptions = new GeneralOptions { FunctionName = "GetGeneralOptions", ReturnCode = this.ServiceResultMessage.ReturnCode };

            switch (this.ServiceResultMessage.ReturnCode)
            {
                case MobileReturnCode.Success:
                    try
                    {
                        cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(this.PairingKeySerialKey.PairingKey.AccountID);
                        cAccountSubAccount reqSubAccount = clsSubAccounts.getFirstSubAccount();
                        cAccountProperties reqProperties;
                        cAccounts accounts = new cAccounts();

                        if (reqSubAccount != null)
                        {
                            reqProperties = reqSubAccount.SubAccountProperties;
                        }
                        else
                        {
                            reqProperties = new cAccountProperties();
                        }

                        if (reqProperties.InitialDate != null)
                        {
                            generalOptions.InitialDate = ((DateTime)reqProperties.InitialDate).ToString("yyyyMMdd");
                        }

                        generalOptions.LimitMonths = reqProperties.LimitMonths;
                        generalOptions.FlagDate = reqProperties.FlagDate;
                        generalOptions.ApproverDeclaration = reqProperties.ApproverDeclarationMsg;
                        generalOptions.ClaimantDeclarationRequired = reqProperties.ClaimantDeclaration;
                        generalOptions.ClaimantDeclaration = reqProperties.DeclarationMsg;
                        generalOptions.AttachReceipts = reqProperties.AttachReceipts;
                        generalOptions.EnableMobileDevices = reqProperties.UseMobileDevices;
                        generalOptions.AllowMultipleDestinations = reqProperties.AllowMultipleDestinations;
                        generalOptions.PostcodesMandatory = reqProperties.MandatoryPostcodeForAddresses;
                        generalOptions.PostcodeAnywhereKey =
                            accounts.GetAccountByID(this.PairingKeySerialKey.PairingKey.AccountID).PostcodeAnywhereKey;
                    }
                    catch (Exception ex)
                    {
                        cEventlog.LogEntry("MobileAPI.GetGeneralOptions():Error:{ Pairingkey: " + this.PairingKeySerialKey.PairingKey + "\nSerialKey: " + this.PairingKeySerialKey.SerialKey + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                        throw;
                    }

                    break;
            }

            return generalOptions;
        }
    }
}

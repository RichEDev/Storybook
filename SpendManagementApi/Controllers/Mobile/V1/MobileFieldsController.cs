namespace SpendManagementApi.Controllers.Mobile.V1
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Web.Http.Description;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Attributes.Mobile;

    using SpendManagementLibrary;

    using Spend_Management;

    using AddExpensesScreenDetails = SpendManagementLibrary.Mobile.AddExpensesScreenDetails;
    using DisplayField = SpendManagementLibrary.Mobile.DisplayField;

    /// <summary>
    /// The controller to handle all <see cref="DisplayField">DisplayFields</see> for mobile users.
    /// </summary>
    [Version(1)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MobileFieldsV1Controller : BaseMobileApiController
    {
        /// <summary>
        /// Gets the fields for the mobile users account.
        /// </summary>
        /// <returns>A list of fields in a <see cref="AddExpensesScreenDetails"/> object</returns>
        [HttpGet]
        [MobileAuth]
        [Route("mobile/fields")]
        public AddExpensesScreenDetails GetFields()
        {
            AddExpensesScreenDetails details = new AddExpensesScreenDetails { FunctionName = "GetFields", ReturnCode = this.ServiceResultMessage.ReturnCode };

            if (this.ServiceResultMessage.ReturnCode == MobileReturnCode.Success)
            {
                try
                {
                    SortedList<string, DisplayField> fields = new SortedList<string, DisplayField>();
                    cMisc clsMisc = new cMisc(this.PairingKeySerialKey.PairingKey.AccountID);
                    SortedList<string, cFieldToDisplay> screenFields = clsMisc.GetAddScreenFields();
                    List<DisplayField> list = new List<DisplayField>();

                    foreach (KeyValuePair<string, cFieldToDisplay> kvp in screenFields)
                    {
                        switch (kvp.Key)
                        {
                            case "reason":
                            case "currency":
                            case "from":
                            case "to":
                            case "otherdetails":
                                DisplayField df = new DisplayField
                                {
                                    code = kvp.Value.code,
                                    description = kvp.Value.description,
                                    display = kvp.Value.display,
                                    individualItem = kvp.Value.individual
                                };

                                list.Add(df);
                                break;
                        }
                    }

                    details.Fields = fields;
                    details.List = list;
                }
                catch (Exception ex)
                {
                    cEventlog.LogEntry("MobileAPI.GetAddEditExpensesScreenSetup():Error:{ Pairingkey: " + this.PairingKeySerialKey.PairingKey + "\nSerialKey: " + this.PairingKeySerialKey.SerialKey + "\nEmployeeID: " + this.PairingKeySerialKey.PairingKey.EmployeeID + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                    // ReSharper disable once PossibleIntendedRethrow
                    throw ex;
                }
            }

            return details;
        }
    }
}

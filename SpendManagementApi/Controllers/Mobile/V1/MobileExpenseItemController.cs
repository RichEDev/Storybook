namespace SpendManagementApi.Controllers.Mobile.V1
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Web.Http.Description;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Attributes.Mobile;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Mobile;

    using Spend_Management;

    using ExpenseItem = SpendManagementLibrary.Mobile.ExpenseItem;

    /// <summary>
    /// The controller for saving mobile items from a device to the server.
    /// </summary>
    [Version(1)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MobileExpenseItemV1Controller : BaseMobileApiController
    {
        /// <summary>
        /// Saved an expense item from a mobile device to the database.
        /// </summary>
        /// <param name="items">An array of items to save.</param>
        /// <returns>Success or reason for failure in a <see cref="SaveExpenseItemResult"/></returns>
        [HttpPost]
        [MobileAuth]
        [Route("mobile/expenseitems/save")]
        public SaveExpenseItemResult SaveExpense([FromBody]List<ExpenseItem> items)
        {
            SaveExpenseItemResult result = new SaveExpenseItemResult { FunctionName = "SaveExpense", ReturnCode = this.ServiceResultMessage.ReturnCode };

            switch (this.ServiceResultMessage.ReturnCode)
            {
                case MobileReturnCode.Success:
                    try
                    {
                        SortedList<string, int> ids = new SortedList<string, int>();
                        cMobileDevices clsmobile = new cMobileDevices(this.PairingKeySerialKey.PairingKey.AccountID);
                        MobileDevice curDevice = clsmobile.GetDeviceByPairingKey(this.PairingKeySerialKey.PairingKey.Pairingkey);
                        int employeeid = this.PairingKeySerialKey.PairingKey.EmployeeID;

                        foreach (ExpenseItem item in items)
                        {
                            int id = clsmobile.saveMobileItem(employeeid, item.OtherDetails, item.ReasonID, item.Total, item.SubcatID, item.dtDate, item.CurrencyID, item.Miles, item.Quantity, item.FromLocation, item.ToLocation, item.dtAllowanceStartDate, item.dtAllowanceEndDate, item.AllowanceTypeID, item.AllowanceDeductAmount, item.ItemNotes, curDevice.DeviceType.DeviceTypeId, item.JourneySteps, mobileDeviceID: curDevice.MobileDeviceID, mobileExpenseID: item.MobileID);
                            ids.Add("MobileID", item.MobileID);
                            ids.Add("ServerID", id);
                        }

                        result.List = ids;
                    }
                    catch (Exception ex)
                    {
                        cEventlog.LogEntry("MobileAPI.SaveExpense():Error:{ Pairingkey: " + this.PairingKeySerialKey.PairingKey + "\nSerialKey: " + this.PairingKeySerialKey.SerialKey + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                        throw;
                    }

                    break;
            }

            return result;
        }
    }
}

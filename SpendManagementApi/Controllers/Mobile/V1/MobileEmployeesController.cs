namespace SpendManagementApi.Controllers.Mobile.V1
{
    using System;
    using System.Web.Http;
    using System.Web.Http.Description;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Attributes.Mobile;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;

    using Spend_Management;

    using EmployeeBasic = SpendManagementLibrary.Mobile.EmployeeBasic;

    /// <summary>
    /// The controller handling all employee actions for the Mobile API.
    /// </summary>
    [Version(1)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MobileEmployeesV1Controller : BaseMobileApiController
    {
        /// <summary>
        /// Gets basic details of the mobile user
        /// </summary>
        /// <returns>Details about the mobile user wrapped in a <see cref="EmployeeBasic"/> object</returns>
        [HttpGet]
        [Route("mobile/employees")]
        [MobileAuth]
        public EmployeeBasic Get()
        {
            EmployeeBasic emp = new EmployeeBasic { FunctionName = "GetEmployeeBasicDetails", ReturnCode = this.ServiceResultMessage.ReturnCode };

            if (this.ServiceResultMessage.ReturnCode == MobileReturnCode.Success)
            {
                try
                {
                    cEmployees clsEmployees = new cEmployees(this.PairingKeySerialKey.PairingKey.AccountID);
                    Employee reqEmployee = clsEmployees.GetEmployeeById(this.PairingKeySerialKey.PairingKey.EmployeeID);

                    emp.firstname = reqEmployee.Forename;
                    emp.surname = reqEmployee.Surname;
                    emp.isApprover = cAccessRoles.CanCheckAndPay(this.PairingKeySerialKey.PairingKey.AccountID, this.PairingKeySerialKey.PairingKey.EmployeeID);
                    emp.primaryCurrency = reqEmployee.PrimaryCurrency;
                }
                catch (Exception ex)
                {
                    cEventlog.LogEntry("MobileAPI.GetEmployeeBasicDetails():Error:{ Pairingkey: " + this.PairingKeySerialKey.PairingKey + "\nSerialKey: " + this.PairingKeySerialKey.SerialKey + "\nEmployeeID: " + this.PairingKeySerialKey.PairingKey.EmployeeID + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                    // ReSharper disable once PossibleIntendedRethrow
                    throw ex;
                }
            }

            return emp;
        }
    }
}

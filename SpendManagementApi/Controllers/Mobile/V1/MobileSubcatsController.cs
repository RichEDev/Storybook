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
    using SpendManagementLibrary.Employees;

    using Spend_Management;

    using ExpenseItemDetail = SpendManagementLibrary.Mobile.ExpenseItemDetail;
    using ExpenseItemResult = SpendManagementLibrary.Mobile.ExpenseItemResult;

    /// <summary>
    /// The controller for handling sub categories the user is allowed to claim.
    /// </summary>
    [Version(1)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MobileSubcatsV1Controller : BaseMobileApiController
    {
        /// <summary>
        /// Gets the sub categories available to the mobile user.
        /// </summary>
        /// <returns>A list of sub categories</returns>
        [MobileAuth]
        [Route("mobile/subcats")]
        [HttpGet]
        public ExpenseItemResult GetEmployeeSubCats()
        {
            ExpenseItemResult result = new ExpenseItemResult { FunctionName = "GetEmployeeSubCats", ReturnCode = this.ServiceResultMessage.ReturnCode };

            if (this.ServiceResultMessage.ReturnCode == MobileReturnCode.Success)
            {
                try
                {
                    SortedList<int, ExpenseItemDetail> expenseItems = new SortedList<int, ExpenseItemDetail>();

                    cEmployees clsEmployees = new cEmployees(this.PairingKeySerialKey.PairingKey.AccountID);
                    Employee reqEmployee = clsEmployees.GetEmployeeById(this.PairingKeySerialKey.PairingKey.EmployeeID);

                    ItemRoles clsItemRoles = new ItemRoles(this.PairingKeySerialKey.PairingKey.AccountID);

                    // Get all the item roles for this employee
                    List<EmployeeItemRole> lstItemRoles = reqEmployee.GetItemRoles().ItemRoles;

                    // Loop through all item roles
                    foreach (EmployeeItemRole itemRole in lstItemRoles)
                    {
                        // Get this specific item role
                        ItemRole tmpItemRole = clsItemRoles.GetItemRoleById(itemRole.ItemRoleId);

                        // Loop through the subcats on this item role
                        foreach (RoleSubcat reqRoleSubcat in tmpItemRole.Items.Values)
                        {
                            if (!expenseItems.ContainsKey(reqRoleSubcat.SubcatId))
                            {
                                var subcat = new cSubcats(this.PairingKeySerialKey.PairingKey.AccountID).GetSubcatById(reqRoleSubcat.SubcatId);
                                expenseItems.Add(
                                    reqRoleSubcat.SubcatId,
                                    new ExpenseItemDetail
                                    {
                                        ID = subcat.subcatid,
                                        Name = subcat.subcat,
                                        Description = subcat.description,
                                        Calculation = (int)subcat.calculation,
                                        AllowanceAmount = subcat.allowanceamount,
                                        CategoryID = subcat.categoryid,
                                        VatNumberApp = subcat.vatnumberapp,
                                        VatNumberMandatory = subcat.vatnumbermand,
                                        ShowFrom = subcat.fromapp,
                                        ShowTo = subcat.toapp,
                                        ShowNumberOfPassengers = subcat.nopassengersapp,
                                        ShowPassengerNames = subcat.passengernamesapp,
                                        ShowHeavyBulky = subcat.allowHeavyBulkyMileage
                                    });
                            }
                        }
                    }

                    result.List = expenseItems.Values.ToList();
                }
                catch (Exception ex)
                {
                    cEventlog.LogEntry(
                        "MobileAPI.GetEmployeeSubCats():Error:{ Pairingkey: " + this.PairingKeySerialKey.PairingKey.Pairingkey + "\nSerialKey: " + this.PairingKeySerialKey.SerialKey + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                    throw;
                }
            }

            return result;
        }
    }
}

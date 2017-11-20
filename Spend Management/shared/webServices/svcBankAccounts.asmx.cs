using System;
using System.Web.Services;
using SpendManagementLibrary;
using SpendManagementLibrary.Account;
using Spend_Management.shared.code;
using Spend_Management.shared.code.Validation.BankAccount;
using Spend_Management.shared.code.Validation.BankAccount.PostCodeAnywhere;

namespace Spend_Management.shared.webServices
{
    /// <summary>
    /// Summary description for svcBankAccounts
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class svcBankAccounts : System.Web.Services.WebService
    {
        /// <summary>
        /// to validate (where available) and save bank details 
        /// </summary>
        /// <param name="bankAccount">An instance f</param>
        /// <returns></returns>
        [WebMethod]
        public IBankAccountValid SaveBankAccount(BankAccount bankAccount)
        {
            try
            {
                var currentUser = cMisc.GetCurrentUser();
                var bankAccounts = new BankAccounts(currentUser.AccountID, currentUser.EmployeeID);
                var bankAccountValidationResults = new BankAccountValidationResults(currentUser);
                var countries = new cCountries(currentUser.AccountID, currentUser.CurrentSubAccountId);
                var validator = new BankAccountValidator(new PostCodeAnywhereBankAccountValidator(currentUser.Account), bankAccountValidationResults, bankAccounts, countries);
                var validateResult =  validator.Validate(bankAccount);
                if (validateResult.IsCorrect)
                {
                    if (bankAccount.AccountName != "") bankAccount.AccountName = bankAccount.AccountName.Trim();
                    if (bankAccount.AccountNumber != "") bankAccount.AccountNumber = bankAccount.AccountNumber.Trim();
                    if (bankAccount.SortCode != "") bankAccount.SortCode = bankAccount.SortCode.Trim();
                    if (bankAccount.Reference != "") bankAccount.Reference = bankAccount.Reference.Trim();

                    var saveResult = bankAccounts.SaveBankAccount(bankAccount, currentUser.EmployeeID);
                    if (saveResult == -1)
                    {
                        validateResult = new BankAccountInvalid(false, "The Account Name already exists.");
                    }
                    else
                    {
                        if (validateResult.IsCorrect)
                        {
                            bankAccount.BankAccountId = saveResult;
                            bankAccountValidationResults.Save(bankAccount, validateResult);
                        }
                    }
                }

                return validateResult;
            }
            catch (BankAccountValidationException e)
            {
                cEventlog.LogEntry(e.Message);
                return new BankAccountInvalid(false, "There was a problem with the Bank Validation service.");
            }
        }

        /// <summary>
        /// gets bank details by id
        /// </summary>
        /// <param name="bankAccountId"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public BankAccount GetBankAccountById(int bankAccountId)
        {
            var currentUser = cMisc.GetCurrentUser();
            var accounts = new BankAccounts(currentUser.AccountID,currentUser.EmployeeID );
            var reqBankAccount = accounts.GetBankAccountById(bankAccountId);
            return reqBankAccount;
        }


        /// <summary>
        /// Delete Bank Account selected on grid
        /// </summary>
        /// <param name="bankAccountId"></param>
        /// <param name="employeeId"></param> 
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public int DeleteBankAccount(int bankAccountId, int employeeId)
        {
            int returnCode = 0;
            var currentUser = cMisc.GetCurrentUser();
            var accounts = new BankAccounts(currentUser.AccountID, employeeId);
            returnCode = accounts.DeleteBankAccount(bankAccountId, employeeId);            
            return returnCode;
        }

        /// <summary>
        /// Archive/un-archive bank account
        /// </summary>
        /// <param name="bankAccountId"></param>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public int ChangeStatus(int bankAccountId,int employeeId)
        {
            int returnCode = 0;
            var currentUser = cMisc.GetCurrentUser();
            var accounts = new BankAccounts(currentUser.AccountID, employeeId);
            BankAccount account = accounts.GetBankAccountById(bankAccountId);
            returnCode= accounts.ChangeStatus(bankAccountId, !account.Archived, employeeId);          
            return returnCode;
        }

        /// <summary>
        /// Gets the primary country of the current user.
        /// </summary>
        /// <returns> primary country id</returns>
        [WebMethod(EnableSession = true)]
        public int GetBankPrimaryCountry()
        {
            var currentUser = cMisc.GetCurrentUser();
            string employeePrimaryCountry = cEmployees.GetEmployeePrimaryCountryById(currentUser.EmployeeID, currentUser.AccountID);
            if (!string.IsNullOrEmpty(employeePrimaryCountry))
            {
                var employeeCountryInfo = employeePrimaryCountry.Split(',');
                 return Convert.ToInt32(employeeCountryInfo[1]);
            }
            else
            {
                var subAccounts = new cAccountSubAccounts(currentUser.AccountID);
                cAccountProperties reqProperties = subAccounts.getSubAccountById(currentUser.CurrentSubAccountId).SubAccountProperties;
                return Convert.ToInt32(reqProperties.HomeCountry);
            }
        }

        /// <summary>
        /// Gets the specified employee bank account details by id
        /// </summary>
        /// <param name="bankAccountId">The bank account id</param>
        /// <param name="employeeId">The employee id</param>
        /// <returns>The bank account</returns>
        [WebMethod(EnableSession = true)]
        public BankAccount GetEmployeeBankAccountById(int bankAccountId, int employeeId)
        {
            var currentUser = cMisc.GetCurrentUser();

            if (currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.EmployeeBankAccounts, true) || currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.BankAccounts, true))
            {
                var accounts = new BankAccounts(currentUser.AccountID, employeeId);
                return accounts.GetBankAccountById(bankAccountId);
            }

            return null;
        }
    }
}

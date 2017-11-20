
namespace SpendManagementApi.Repositories
{
    using System.Linq;

    using Models.Requests;
    using Models.Responses;
    using Models.Types;
    using SpendManagementApi.Common;
    using SpendManagementLibrary;
    using SpendManagementLibrary.DVLA;
    using SpendManagementLibrary.Helpers;
    using Spend_Management;
    using Spend_Management.shared.code.DVLA;
    using EmployeesToPopulateDrivingLicence = Models.Types.EmployeesToPopulateDrivingLicence;

    /// <summary>
    /// Repository of drivinglicence
    /// </summary>
    internal class PopulateDrivingLicenceRepository 
    {
        /// <summary>
        /// Get accounts with auto populate general option enabled 
        /// </summary>
        /// <returns> Account details</returns>
        public GeneralOptionAccountsResponse GetAccountsWithDvlaConsentEnabled()
        {
            var accountList = new GeneralOptionAccountsResponse();
            var accounts = new cAccounts().GetAllAccounts();
            if (accounts == null || accounts.Count == 0) return accountList;
            foreach (var account in accounts)
            {
                var subAccounts = new cAccountSubAccounts(account.accountid);
                var reqSubAccount = subAccounts.getFirstSubAccount();
                if (reqSubAccount.SubAccountProperties.EnableAutomaticDrivingLicenceLookup && account.HasDvlaLookupKeyAndDvlaConnectLicenceElement(SpendManagementElement.DvlaConnect))
                {
                    accountList.AccountList.Add(new GeneralOptionEnabledAccount(account.accountid,account.companyid));
                }
            }
            return accountList;
        }


        /// <summary>
        /// Employee details which will be used to populate driving licence.
        /// </summary>
        /// <param name="accountId">Account from which need to fetch employee</param>
        /// <returns>Employee details</returns>
        public DvlaConsentEmployeesResponse GetEmployeeToPopulateDrivingLicence(int accountId)
        {
            var employee = new cEmployees(accountId);
            var employeeDetails = employee.GetEmployeesToPopulateDrivingLicence(accountId);
            var employeeDrivingLicenceList = new EmployeesToPopulateDrivingLicence().Data(employeeDetails, null);
            return employeeDrivingLicenceList;
        }

        /// <summary>
        /// Save driving licence information
        /// </summary>
        /// <param name="accountId">
        /// Account in which need to save
        /// </param>
        /// <param name="drivingLicenceDetails">
        /// Driving licence information
        /// </param>
        /// <returns>
        /// The <see cref="DvlaDrivingLicenceResponse"/> Dvla Driving licence auto lookup response.
        /// </returns>
        public DvlaDrivingLicenceResponse SaveDvlaDrivingLicenceInformation(int accountId, DrivingLicenceRequest drivingLicenceDetails)
        {
            var addDrivingLicence = new AutoPopulatedDrivingLicence();
            var employees = new cEmployees(accountId);
            var connection = new DatabaseConnection(cAccounts.getConnectionString(accountId));
            var documentList = drivingLicenceDetails.DrivingLicenceDetails.Select(drivingLicence => addDrivingLicence.AddDrivingLicenceInformation(employees, accountId, drivingLicence, false,false, connection)).ToList();
            var drivingLicenceResponse = new DvlaDrivingLicenceResponse { IsDrivingLicenceAdded = documentList.Count > 0, CurrentRecord = documentList };
            return drivingLicenceResponse;
        }

        /// <summary>
        /// Save driving licence information
        /// </summary>
        /// <param name="accountId">
        /// Account in which need to save
        /// </param>
        /// <param name="employeeDetails">
        /// The employee Details.
        /// </param>
        /// <returns>
        /// The <see cref="DrivingLicenceList"/>
        /// List of driving licence details
        /// </returns>
        public DrivingLicenceResponse PopulateDrivingLicenceInformation(int accountId, Spend_Management.shared.code.DVLA.EmployeesToPopulateDrivingLicence employeeDetails)
        {
            var drivingLicenceDetails = new AutoPopulatedDrivingLicence().PopulateDrivingLicences(employeeDetails, accountId, false);
            var responseList = new DrivingLicenceDetails().Data(drivingLicenceDetails);
            return responseList;
        }


        /// <summary>
        /// List of employee who has provided manual consent to dvla populate driving licence
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public DvlaConsentEmployeesResponse GetEmployeeWithManualIntervention(int accountId)
        {
            var employeeDetails = new EmployeesWithManualIntervention().GetEmployeeWithManualIntervention(accountId);
            var employeeDrivingLicenceList = new EmployeesToPopulateDrivingLicence().Data(employeeDetails, null);
            return employeeDrivingLicenceList;
        }

        /// <summary>
        /// The delete driving licence record.
        /// </summary>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <param name="viewid">
        /// The viewid.
        /// </param>
        /// <param name="entityid">
        /// The entityid.
        /// </param>
        /// <param name="recordid">
        /// The recordid.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> record value of the driving licence entity.
        /// </returns>
        public int DeleteDrivingLicenceRecord(int accountId, int employeeId, int viewid, int entityid, int recordid)
        {
            var curUser = cMisc.GetCurrentUser(accountId + "," + employeeId);
            var clsentities = new cCustomEntities(curUser);
            var entity = clsentities.getEntityById(entityid);
            return clsentities.DeleteCustomEntityRecord(entity, recordid, viewid);
        }

        /// <summary>
        /// The save driving licence review.
        /// </summary>
        /// <param name="drivingLicenceReview">
        /// The driving licence review.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <returns>
        /// The <see cref="DvlaDrivingLicenceResponse"/>.
        /// </returns>
        public DvlaDrivingLicenceResponse SaveDrivingLicenceReview(DrivingLicenceReviewRequest drivingLicenceReview, int accountId, int employeeId)
        {
            var addDrivingLicence = new AutoPopulatedDrivingLicence();
            var documentList = drivingLicenceReview.DrivingLicenceDetails.Select(drivingLicence => addDrivingLicence.SaveDrivingLicenceReview(drivingLicence, accountId, employeeId)).ToList();
            var drivingLicenceResponse = new DvlaDrivingLicenceResponse { IsDrivingLicenceAdded = documentList.Count > 0, CurrentRecord = documentList };
            return drivingLicenceResponse;
        }
    }
}
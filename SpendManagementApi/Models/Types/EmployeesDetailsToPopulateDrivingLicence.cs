namespace SpendManagementApi.Models.Types
{
    using System;
    using Interfaces;
    using Responses;

    /// <summary>
    /// Employee details to fetch driving licence from dvla portal
    /// </summary>
    public class EmployeesToPopulateDrivingLicence : BaseExternalType, IApiFrontForDbObject<SpendManagementLibrary.DVLA.EmployeesToPopulateDrivingLicence, EmployeesToPopulateDrivingLicence>
    {

        /// <summary>
        /// Employee details
        /// </summary>

        public SpendManagementLibrary.DVLA.EmployeesToPopulateDrivingLicence To(IActionContext actionContext)
        {
            throw new NotImplementedException();
        }

        public EmployeesToPopulateDrivingLicence From(SpendManagementLibrary.DVLA.EmployeesToPopulateDrivingLicence dbType, IActionContext actionContext)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Employee details to fetch driving licence
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="actionContext"></param>
        /// <returns>Response object for employee</returns>
        public DvlaConsentEmployeesResponse Data(SpendManagementLibrary.DVLA.EmployeesToPopulateDrivingLicence dbType, IActionContext actionContext)
        {
            var employeesDrivingLicenceList = new DvlaConsentEmployeesResponse();
            foreach (var employeeConsentDetails in dbType.EmployeeList)
            {
                employeesDrivingLicenceList.EmployeeList.Add(new EmployeeToPopulateDrivingLicence(employeeConsentDetails));
            }

            return employeesDrivingLicenceList;
        }
    }
}
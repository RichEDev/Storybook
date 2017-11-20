namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;
    using Common;
    using Types;

    /// <summary>
    /// List of employees details which are required to populate driving licence 
    /// </summary>
    public class DvlaConsentEmployeesResponse : ApiResponse<EmployeesToPopulateDrivingLicence>
    {

        /// <summary>
        /// List of employee details to fetch driving licence from dvla portal
        /// </summary>
        public List<EmployeeToPopulateDrivingLicence> EmployeeList = new List<EmployeeToPopulateDrivingLicence>();
    }
}
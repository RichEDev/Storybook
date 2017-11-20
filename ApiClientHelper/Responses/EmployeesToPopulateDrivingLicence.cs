namespace ApiClientHelper.Responses
{
    using System.Collections.Generic;

    /// <summary>
    /// Employees information to fetch driving licence
    /// </summary>
    public class EmployeesToPopulateDrivingLicence
    {
        /// <summary>
        /// List of employee details
        /// </summary>
        public List<EmployeeDetailsToPopulateDrivingLicence> EmployeeList = new List<EmployeeDetailsToPopulateDrivingLicence>();
    }
}

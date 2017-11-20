namespace SpendManagementApi.Models.Responses.SelfRegistration
{
    using System;
    using System.Collections.Generic;

    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Models.Types.Employees;

    /// <summary>
    /// Provides the response framework for self registration for the mobile api.
    /// All the data the client requires for the registration process is included inclduing validation results.
    /// </summary>
    public class SelfRegistrationResponse : ApiResponse
    {
        /// <summary>
        /// This is a Postcode Anywhere licence key setup for testing the self registration API address calls. This will be used in your lookups for addresses.
        /// </summary>
        /// <returns></returns>
        public string PostCodeAnywhereLicenseKey { get; set; }

        /// <summary>
        /// A list of countries available for this registration. Pairs of country id and country name are presented.
        /// </summary>
        public List<Tuple<int, string>> Countries { get; set; }

        /// <summary>
        /// A list of currencies available for this registration. Pairs of currency id and country name are presented.
        /// </summary>
        public List<Tuple<int, string>> Currencies { get; set; }

        /// <summary>
        /// A list of line managers available for this registration. A list of Employees are returned.
        /// </summary>
        public List<IEmployee> LineManagers { get; set; }

        /// <summary>
        /// A list of access roles available for this registration. A list of IAccessRole with the basic properties are returned.
        /// </summary>
        public List<IAccessRole> AccessRoles { get; set; }

        /// <summary>
        /// A list of signoff groups available for this registration. A list of SignoffGroupBasic is returned, which is a subset of the SignOffGroup class.
        /// </summary>
        public List<SignoffGroupBasic> SignoffGroups { get; set; }  

        /// <summary>
        /// A list of departments available for this registration. A list of department id and department name are returned.
        /// </summary>
        public List<Tuple<int, string>> Departments { get; set; }

        /// <summary>
        /// A list of cost codes available for this registration. A list of cost code id and cost code description name are returned.
        /// </summary>
        public List<Tuple<int, string>> CostCodes { get; set; }

        /// <summary>
        /// A list of project codes available for this registration. A list of project code id and project code description name are returned.
        /// </summary>
        public List<Tuple<int, string>> ProjectCodes { get; set; }

        /// <summary>
        /// A list of "mileage units of measure" available for this registration. The mileage unit of measure id and description are returned.
        /// </summary>
        public List<Tuple<int, string>> MileageUoMs { get; set; }

        /// <summary>
        /// A list of car engine types available for this registration. Car engine type id and description are returned.
        /// </summary>
        public List<Tuple<int, string>> CarEngineTypes { get; set; }

        /// <summary>
        /// A list of user defined fields available for this registration.
        /// </summary>
        public List<UserDefinedField> UserDefinedFields { get; set; }
    }
}
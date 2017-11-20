namespace ApiRpc.Classes.ApiCrud
{
    using ApiLibrary.DataObjects.ESR;
    using ApiLibrary.DataObjects.Spend_Management;

    using global::ApiRpc.Interfaces;

    /// <summary>
    /// The API crud list.
    /// </summary>
    public class ApiCrudList
    {
        /// <summary>
        /// Gets or sets The employee API.
        /// </summary>
        public IDataAccess<Employee> EmployeeApi { get; set; }

        /// <summary>
        /// Gets or sets The person API.
        /// </summary>
        public IDataAccess<EsrPerson> EsrPersonApi { get; set; }

        /// <summary>
        /// Gets or sets The assignment API.
        /// </summary>
        public IDataAccess<EsrAssignment> EsrAssignmentApi { get; set; }

        /// <summary>
        /// Gets or sets The assignment location API.
        /// </summary>
        public IDataAccess<EsrAssignmentLocation> EsrAssignmentLocationApi { get; set; }

        /// <summary>
        /// Gets or sets The address API.
        /// </summary>
        public IDataAccess<EsrAddress> EsrAddressApi { get; set; }

        /// <summary>
        /// Gets or sets The phone API.
        /// </summary>
        public IDataAccess<EsrPhone> EsrPhoneApi { get; set; }

        /// <summary>
        /// Gets or sets The vehicle API.
        /// </summary>
        public IDataAccess<EsrVehicle> EsrVehicleApi { get; set; }

        /// <summary>
        /// Gets or sets The location API.
        /// </summary>
        public IDataAccess<EsrLocation> EsrLocationApi { get; set; }

        /// <summary>
        /// Gets or sets The organisation API.
        /// </summary>
        public IDataAccess<EsrOrganisation> EsrOrganisationApi { get; set; }

        /// <summary>
        /// Gets or sets The position API.
        /// </summary>
        public IDataAccess<EsrPosition> EsrPositionApi { get; set; }

        /// <summary>
        /// Gets or sets the trust API.
        /// </summary>
        public IDataAccess<EsrTrust> EsrTrustApi { get; set; }

        /// <summary>
        /// Gets or sets the import template API.
        /// </summary>
        public IDataAccess<Template> TemplateApi { get; set; }

        /// <summary>
        /// Gets or sets the import template API.
        /// </summary>
        public IDataAccess<TemplateMapping> TemplateMappingApi { get; set; }

        /// <summary>
        /// Gets or sets the field API.
        /// </summary>
        public IDataAccess<Field> FieldApi { get; set; }

        /// <summary>
        /// Gets or sets the UDF API.
        /// </summary>
        public IDataAccess<UserDefinedField> UdfApi { get; set; }

        /// <summary>
        /// Gets or sets the account property API.
        /// </summary>
        public IDataAccess<AccountProperty> AccountPropertyApi { get; set; }

        /// <summary>
        /// Gets or sets the assignment COSTINGS API.
        /// </summary>
        public IDataAccess<EsrAssignmentCostings> EsrAssignmentCostingsApi { get; set; }

        /// <summary>
        /// Gets or sets the car API.
        /// </summary>
        public IDataAccess<Car> CarApi { get; set; }

        /// <summary>
        /// Gets or sets the cost code API.
        /// </summary>
        public IDataAccess<CostCode> CostCodeApi { get; set; }

        /// <summary>
        /// Gets or sets the employee cost code API.
        /// </summary>
        public IDataAccess<EmployeeCostCode> EmployeeCostCodeApi { get; set; }

        /// <summary>
        /// Gets or sets the car assignment number API.
        /// </summary>
        public IDataAccess<CarAssignmentNumberAllocation> CarAssignmentNumberApi { get; set; }

        /// <summary>
        /// Gets or sets the import log API.
        /// </summary>
        public IDataAccess<ImportLog> ImportLogApi { get; set; }

        /// <summary>
        /// Gets or sets the import history API.
        /// </summary>
        public IDataAccess<ImportHistory> ImportHistoryApi { get; set; }

        /// <summary>
        /// Gets or sets the user defined match API.
        /// </summary>
        public IDataAccess<UserDefinedMatchField> UserDefinedMatchApi { get; set; }

        /// <summary>
        /// Gets or sets the employee role API.
        /// </summary>
        public IDataAccess<EmployeeRole> EmployeeRoleApi { get; set; }

        /// <summary>
        /// Gets or sets the employee access role API.
        /// </summary>
        public IDataAccess<EmployeeAccessRole> EmployeeAccessRoleApi { get; set; }

        /// <summary>
        /// Gets or sets the vehicle journey rate API.
        /// </summary>
        public IDataAccess<VehicleJourneyRate> VehicleJourneyRateApi { get; set; }

        /// <summary>
        /// Gets or sets the car vehicle journey rate API.
        /// </summary>
        public IDataAccess<CarVehicleJourneyRate> CarVehicleJourneyRateApi { get; set; } 

        /// <summary>
        /// Gets or sets the spend management address API.
        /// </summary>
        public IDataAccess<Address> AddressApi { get; set; }
        
        /// <summary>
        /// Gets or sets the spend management organisation API.
        /// </summary>
        public IDataAccess<Organisation> OrganisationApi { get; set; }

        /// <summary>
        /// Gets or sets the employee home address API.
        /// </summary>
        public IDataAccess<EmployeeHomeAddress> EmployeeHomeAddressApi { get; set; }

        /// <summary>
        /// Gets or sets the employee work address API.
        /// </summary>
        public IDataAccess<EmployeeWorkAddress> EmployeeWorkAddressApi { get; set; }
    }
}
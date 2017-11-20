namespace EsrFramework.Models
{
    using System;

    public class EsrVehicle
    {
        #region Public Properties

        public int? EsrAssignId { get; set; }
        public long EsrAssignmentId { get; set; }
        public DateTime? EsrLastUpdate { get; set; }
        public long EsrPersonId { get; set; }
        public long EsrVehicleAllocationId { get; set; }
        public DateTime? EffectiveEndDate { get; set; }
        public DateTime? EffectiveStartDate { get; set; }
        public int? EngineCc { get; set; }
        public string FuelType { get; set; }
        public DateTime? InitialRegistrationDate { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Ownership { get; set; }
        public string RegistrationNumber { get; set; }
        public string UserRatesTable { get; set; }

        #endregion
    }
}
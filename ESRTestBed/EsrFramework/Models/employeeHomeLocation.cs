namespace EsrFramework.Models
{
    using System;

    public class EmployeeHomeLocation
    {
        #region Public Properties

        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int EmployeeId { get; set; }
        public int EmployeeLocationId { get; set; }
        public DateTime? EndDate { get; set; }
        public int? LocationId { get; set; }
        public DateTime StartDate { get; set; }

        #endregion
    }
}
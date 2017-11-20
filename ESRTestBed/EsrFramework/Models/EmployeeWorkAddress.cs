namespace EsrFramework.Models
{
    using System;

    public class EmployeeWorkAddress
    {
        #region Public Properties

        public bool Active { get; set; }
        public int AddressId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int EmployeeId { get; set; }
        public int EmployeeWorkAddressId { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public DateTime? StartDate { get; set; }
        public bool Temporary { get; set; }

        #endregion
    }
}
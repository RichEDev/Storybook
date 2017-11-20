namespace EsrFramework.Models
{
    using System;

    public class EmployeeHomeAddress
    {
        #region Public Properties

        public int AddressId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int EmployeeHomeAddressId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public DateTime? StartDate { get; set; }

        #endregion
    }
}
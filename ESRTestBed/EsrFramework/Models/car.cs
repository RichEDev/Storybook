namespace EsrFramework.Models
{
    using System;

    public class Car
    {
        #region Public Properties

        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? MotAttachId { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool Active { get; set; }
        public bool Approved { get; set; }
        public int CarId { get; set; }
        public byte? CarTypeId { get; set; }
        public byte DefaultUnit { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? EndDate { get; set; }
        public int? EndOdometer { get; set; }
        public int? EngineSize { get; set; }
        public bool ExemptFromHomeToOffice { get; set; }
        public bool FuelCard { get; set; }
        public int? InsuranceAttachId { get; set; }
        public int? InsuranceCheckedBy { get; set; }
        public DateTime? InsuranceExpiry { get; set; }
        public DateTime? InsuranceLastChecked { get; set; }
        public string InsuranceNumber { get; set; }
        public string Make { get; set; }
        public int? MileageId { get; set; }
        public string Model { get; set; }
        public int? MotCheckedBy { get; set; }
        public DateTime? MotExpiry { get; set; }
        public DateTime? MotLastChecked { get; set; }
        public string MotTestNumber { get; set; }
        public long? Odometer { get; set; }
        public string Registration { get; set; }
        public int? ServiceAttachId { get; set; }
        public int? ServiceCheckedBy { get; set; }
        public DateTime? ServiceExpiry { get; set; }
        public DateTime? ServiceLastChecked { get; set; }
        public DateTime? StartDate { get; set; }
        public int? TaxAttachId { get; set; }
        public int? TaxCheckedBy { get; set; }
        public DateTime? TaxExpiry { get; set; }
        public DateTime? TaxLastChecked { get; set; }

        #endregion
    }
}
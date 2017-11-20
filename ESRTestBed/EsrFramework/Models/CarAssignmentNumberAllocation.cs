namespace EsrFramework.Models
{
    public class CarAssignmentNumberAllocation
    {
        #region Public Properties

        public bool Archived { get; set; }
        public int CarId { get; set; }
        public int EsrAssignId { get; set; }
        public long EsrVehicleAllocationId { get; set; }

        #endregion
    }
}
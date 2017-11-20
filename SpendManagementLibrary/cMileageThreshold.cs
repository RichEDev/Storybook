namespace SpendManagementLibrary
{
    using System;

    /// <summary>
    /// Vehicle Journey Rate Date Range Threshold Range Type
    /// </summary>
    [Serializable]
    public enum RangeType
    {
        GreaterThanOrEqualTo = 0,
        Between,
        LessThan,
        Any
    }

    /// <summary>
    /// Vehicle Journey Rate Date Range Threshold
    /// </summary>
    [Serializable]
    public class cMileageThreshold
    {

        public int MileageThresholdId { get; private set; }

        public int MileageDateRangeId { get; private set; }

        public decimal? RangeValue1 { get; private set; }

        public decimal? RangeValue2 { get; private set; }

        public RangeType RangeType { get; private set; }

        public decimal Passenger { get; private set; }
        
        public decimal PassengerX { get; private set; }

        public DateTime CreatedOn { get; private set; }

        public int CreatedBy { get; private set; }

        public DateTime? ModifiedOn { get; private set; }

        public int? ModifiedBy { get; private set; }

        public decimal HeavyBulkyEquipment { get; private set; }

        public cMileageThreshold(int mileageThresholdId, int mileageDateRangeId, decimal? rangeValue1, decimal? rangeValue2, RangeType rangeType, decimal passenger, decimal passengerX, DateTime createdOn, int createdBy, DateTime? modifiedOn, int? modifiedBy, decimal heavyBulkyEquipment)
        {
            this.MileageThresholdId = mileageThresholdId;
            this.MileageDateRangeId = mileageDateRangeId;
            this.RangeValue1 = rangeValue1;
            this.RangeValue2 = rangeValue2;
            this.RangeType = rangeType;
            this.Passenger = passenger;
            this.PassengerX = passengerX;
            this.HeavyBulkyEquipment = heavyBulkyEquipment;

            this.CreatedOn = createdOn;
            this.CreatedBy = createdBy;
            this.ModifiedOn = modifiedOn;
            this.ModifiedBy = modifiedBy;
        }

    }

}

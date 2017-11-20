namespace SpendManagementApi.Models.Types
{
    using SpendManagementApi.Common.Enums;

        /// <summary>
        /// The mileage category basic
        /// </summary>
        public class MileageCategoryBasic
        {
            /// <summary>
            /// Gets or sets the mileage category id.
            /// </summary>
            public int MileageCategoryId { get; set; }

            /// <summary>
            /// Gets or sets the label.
            /// </summary>
            public string Label { get; set; }

            /// <summary>
            /// Gets or sets the unit of measure.
            /// </summary>
            public MileageUom UnitOfMeasure { get; set; }

            /// <summary>
            /// Gets or sets the tooltip information
            /// </summary>
            public string TooltipInfo { get; set; }

            /// <summary>
            /// Gets or sets the text home to office deduction rules
            /// </summary>
            public string HomeToOfficeDeductionRules { get; set; }
        }
}
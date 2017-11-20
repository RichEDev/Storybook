namespace SpendManagementLibrary
{
    using System;

    /// <summary>
    /// A simple structure to allow three important pieces of the subcat information to be represented.
    /// </summary>
    public class SubcatBasic
    {
        /// <summary>
        /// The subcat id
        /// </summary>
        public int SubcatId { get; set; }

        /// <summary>
        /// The subcat name
        /// </summary>
        public string Subcat { get; set; }

        /// <summary>
        /// The VatApp
        /// </summary>
        public bool VatApp { get; set; }

        /// <summary>
        /// The calculation type
        /// </summary>
        public CalculationType CalculationType { get; set; }

        /// <summary>
        /// P11D Category Id
        /// </summary>
        public int P11DCategoryId { get; set; }

        /// <summary>
        /// Category Id
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Subcat start date
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Subcat end date
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets whether the subcat is reimbursable
        /// </summary>
        public bool Reimbursable { get; set; }

        /// <summary>
        /// Gets or sets whether enable home to location mileage is enabled
        /// </summary>
        public bool EnableHomeToLocationMileage { get; set; }

        /// <summary>
        /// Gets or sets the home to location type
        /// </summary>
        public HomeToLocationType HomeToLocationType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether expense is mileage item.
        /// </summary>
        public bool Mileageapp { get; set; }


        /// <summary>
        ///  Gets or sets a value indicating whether expense is enabled for duty of care checks.
        /// </summary>
        public bool EnabledDoc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether expense is enabled for class1 business insurance check.
        /// </summary>
        public bool RequireClass1BusinessInsurance { get; set; }
    }
}

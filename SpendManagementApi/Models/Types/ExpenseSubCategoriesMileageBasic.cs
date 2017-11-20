namespace SpendManagementApi.Models.Types
{
    using System;

    using SpendManagementApi.Interfaces;

    using SpendManagementLibrary;

    /// <summary>
    /// The expense sub categories mileage basic.
    /// </summary>
    public class ExpenseSubCategoriesMileageBasic : IApiFrontForDbObject<cSubcat, ExpenseSubCategoriesMileageBasic>
    {

        /// <summary>
        /// Gets or sets unique Id of the sub category.
        /// </summary>
        public int SubCatId { get; set; }

        /// <summary>
        /// Gets or sets name of the sub category.
        /// </summary>
        public string SubCat { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether number of passengers applicable for the sub category.
        /// </summary>
        public bool NoPassengersApplicable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show names of passengers for the sub category.
        /// </summary>
        public bool PassengersNameApplicable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow heavy bulky mileage for the sub category.
        /// </summary>
        public bool AllowHeavyBulkyMileage { get; set; }

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>An api Type</returns>
        public ExpenseSubCategoriesMileageBasic From(cSubcat dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            this.SubCatId = dbType.subcatid;
            this.SubCat = dbType.subcat;
            this.AllowHeavyBulkyMileage = dbType.allowHeavyBulkyMileage;
            this.NoPassengersApplicable = dbType.nopassengersapp;
            this.PassengersNameApplicable = dbType.passengernamesapp;


            return this;
        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>A data access layer Type</returns>
        public cSubcat To(IActionContext actionContext)
        {
            throw new NotImplementedException();
        }
    }
}
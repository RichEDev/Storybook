namespace SpendManagementApi.Models.Types
{
    using System;

    using SpendManagementApi.Interfaces;

    using SpendManagementLibrary;

    /// <summary>
    /// A class to hold the basic details about an advance
    /// </summary>
    public class Advance :  BaseExternalType, IApiFrontForDbObject<cFloat, Advance>
    {
        /// <summary>
        /// Gets or sets the float id.
        /// </summary>
        public int FloatId { get; set; }

        /// <summary>
        /// Gets or sets the employee id.
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the currency id.
        /// </summary>
        public int CurrencyId { get; set; }

        /// <summary>
        /// Gets or sets the advance name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display name. A concatination of the advance name and currency name 
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the value if advance is available or not
        /// </summary>
        public bool Available { get; set; }

        /// <summary>
        /// Gets or sets the outcome of an action against an Advance.
        /// </summary>
        public string Outcome { get; set; }

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>An api Type</returns>
        public Advance From(cFloat dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }
          
            this.FloatId = dbType.floatid;
            this.EmployeeId = dbType.employeeid;
            this.CurrencyId = dbType.currencyid;
            this.Name = dbType.name;
            this.Available = dbType.floatamount > dbType.floatused;

            return this;
        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>A data access layer Type</returns>
        public cFloat To(IActionContext actionContext)
        {
            throw new NotImplementedException();
        }
    }
}
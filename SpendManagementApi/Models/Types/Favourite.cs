namespace SpendManagementApi.Models.Types
{
    using System;

    using SpendManagementApi.Interfaces;

    /// <summary>
    /// A class fo handleing employee personal favourite
    /// </summary>
    public class Favourite : BaseExternalType, IApiFrontForDbObject<SpendManagementLibrary.Addresses.Favourite, Favourite>
    {
        /// <summary>
        /// Gets or sets the favourite id.
        /// </summary>
        public int FavouriteId { get; set; }

        /// <summary>
        /// Gets or sets the employee id.
        /// </summary>
        public int? EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the address id.
        /// </summary>
        public int AddressId { get; set; }

        /// <summary>
        /// Gets or sets the global identifier.
        /// </summary>
        public string GlobalIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the address friendly name.
        /// </summary>
        public string AddressFriendlyName { get; set; }

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>A <see cref="Favourite">Favourite</see></returns>
        public Favourite From(SpendManagementLibrary.Addresses.Favourite dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            this.FavouriteId = dbType.FavouriteID;
            this.EmployeeId = dbType.EmployeeID;
            this.AddressId = dbType.AddressID;
            this.GlobalIdentifier = dbType.GlobalIdentifier;
            this.AddressFriendlyName = dbType.AddressFriendlyName;
            
            return this;
        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>A <see cref="SpendManagementLibrary.Addresses.Favourite">SpendManagementLibrary.Addresses.Favourite</see></returns>
        public SpendManagementLibrary.Addresses.Favourite To(IActionContext actionContext)
        {
            throw new NotImplementedException();
        }
    }
}
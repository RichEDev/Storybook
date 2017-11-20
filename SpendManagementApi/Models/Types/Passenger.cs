namespace SpendManagementApi.Models.Types
{
    using System;

    using SpendManagementApi.Interfaces;

    /// <summary>
    /// The passenger class for holding information about a passenger
    /// </summary>
    public class Passenger : BaseExternalType, IApiFrontForDbObject<SpendManagementLibrary.Mileage.Passenger, Passenger>
    {
        /// <summary>
        /// Gets or sets the PassengerId.
        /// </summary>
        public int? PassengerId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>An api Type</returns>
        public Passenger From(SpendManagementLibrary.Mileage.Passenger dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            this.PassengerId = dbType.EmployeeId;
            this.Name = dbType.Name;

            return this;
        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>A data access layer Type</returns>
        public SpendManagementLibrary.Mileage.Passenger To(IActionContext actionContext)
        {
            var passenger = (new SpendManagementLibrary.Mileage.Passenger { EmployeeId = this.PassengerId, Name = this.Name });
            return passenger;
        }
    }
}
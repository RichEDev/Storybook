
using SpendManagementLibrary;
namespace SpendManagementApi.Models.Types.MyDetails
{
    using SpendManagementApi.Interfaces;

    /// <summary>
    /// Represents an Employee vehicle within the system.
    /// </summary>
    public class VehicleDetail : IApiFrontForDbObject<cEmployeeCar, VehicleDetail> 
    {
        /// <summary>
        /// Gets the make of this vehicle.
        /// </summary>
        public string VehicleMake { get; set; }

        /// <summary>
        /// Gets the model of this vehicle.
        /// </summary>
        public string VehicleModel { get; set; }

        /// <summary>
        /// Gets the registration number of this vehicle.
        /// </summary>
        public string RegistrationNumber { get; set; }

        /// <summary>
        /// Gets the Vehicle Engine Type of this vehicle.
        /// </summary>
        public string VehicleEngineType { get; set; }

        /// <summary>
        /// Gets the Engine size of this vehicle.
        /// </summary>
        public int EngineSize { get; set; }

        /// <summary>
        /// Gets the default unit of this vehicle.
        /// </summary>
        public string DefaultUnit { get; set; }

        /// <summary>
        /// Gets the status of this vehicle.
        /// </summary>
        public bool VehicleStatus { get; set; }

        /// <summary>
        /// Whether the vehicle has been approved.
        /// </summary>
        public bool Approved { get; set; }

        /// <summary>
        /// Gets or sets the vehicle type description.
        /// </summary>
        public string VehicleTypeDescription { get; set; }

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>An api Type</returns>
        public VehicleDetail From(cEmployeeCar dbType, IActionContext actionContext)
        {
            this.VehicleMake = dbType.Make;
            this.VehicleModel = dbType.Model;
            this.RegistrationNumber = dbType.Registration;
            this.VehicleEngineType = dbType.CarType;
            this.EngineSize = dbType.Enginesize;
            this.DefaultUnit = dbType.Defaultunit;
            this.VehicleStatus = dbType.Active;
            this.Approved = dbType.Approved;
            return this;
        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>A data access layer Type</returns>
        public cEmployeeCar To(IActionContext actionContext)
        {
            //TODO: Not needed
            throw new System.NotImplementedException();
        }
    }
}